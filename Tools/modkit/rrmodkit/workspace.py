"""Workspace: extract game assets to editable files and rebuild/install them."""
import datetime
import json
import os
import re
import shutil

from . import DEFAULT_KEY, __version__
from .formats import (decode_ean, encode_ean, load_image_file, save_image_file, write_png, read_png,
                      parse_atlas, write_atlas, is_atlas_xml, parse_fnt, sha1, sha1_file, sniff)
from .packer import pack
from .swf import Swf, detect_ean_key

MODKIT_DIR = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DEFAULT_GAME_DIR = os.path.dirname(MODKIT_DIR)
DEFAULT_WS = os.path.join(MODKIT_DIR, "workspace")
DEFAULT_BUILD = os.path.join(MODKIT_DIR, "build")
DEFAULT_BACKUP = os.path.join(MODKIT_DIR, "backup")

INCLUDE_DIRS = ("data", "branding", "icons", "game-shell-data")
INCLUDE_FILES = ("META-INF/AIR/application.xml",)
SWF_FILES = ("RorysRestaurant.swf", "data/graphic/logo.swf")
MAIN_SWF = "RorysRestaurant.swf"
BUILD_MARKER = ".rrmodkit-build"
WS_MARKER = ".rrmodkit-workspace"


def _log(log, msg):
    if log:
        log(msg)


def _safe(name):
    s = re.sub(r'[<>:"/\\|?*\x00-\x1f]', "_", name).rstrip(" .")
    return s or "_"


def _j(*parts):
    return os.path.join(*parts)


def _rel_to_os(rel):
    return rel.replace("/", os.sep)


def _write(path, data):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, "wb") as f:
        f.write(data)


def _read(path):
    with open(path, "rb") as f:
        return f.read()


# --------------------------------------------------------------------------- backups / originals

class Backup:
    """Pristine copies of game files that were overwritten by `install`."""

    def __init__(self, game_dir, backup_dir=DEFAULT_BACKUP):
        self.game_dir = game_dir
        self.dir = backup_dir
        self.state_path = _j(backup_dir, "state.json")
        self.state = dict(backed_up=[], added=[])
        if os.path.exists(self.state_path):
            with open(self.state_path, "r", encoding="utf-8") as f:
                self.state.update(json.load(f))

    def save(self):
        os.makedirs(self.dir, exist_ok=True)
        with open(self.state_path, "w", encoding="utf-8") as f:
            json.dump(self.state, f, indent=1)

    def original_path(self, rel):
        if rel in self.state["backed_up"]:
            p = _j(self.dir, "original", _rel_to_os(rel))
            if os.path.exists(p):
                return p
        return _j(self.game_dir, _rel_to_os(rel))

    def original_bytes(self, rel):
        return _read(self.original_path(rel))

    def installed(self):
        return sorted(set(self.state["backed_up"]) | set(self.state["added"]))


# --------------------------------------------------------------------------- workspace

class Workspace:
    def __init__(self, path=DEFAULT_WS):
        self.path = os.path.abspath(path)
        self.manifest_path = _j(self.path, "manifest.json")
        self.m = None
        self._sha_cache = {}

    # ----------------------------------------------------------------- manifest
    def exists(self):
        return os.path.exists(self.manifest_path)

    def load(self):
        with open(self.manifest_path, "r", encoding="utf-8") as f:
            self.m = json.load(f)
        self._mtime = os.stat(self.manifest_path).st_mtime_ns
        return self

    def reload_if_changed(self):
        """Pick up manifest changes made by another process (e.g. the CLI while the editor runs)."""
        try:
            if os.stat(self.manifest_path).st_mtime_ns != getattr(self, "_mtime", None):
                self.load()
        except FileNotFoundError:
            pass

    def save(self):
        tmp = self.manifest_path + ".tmp"
        with open(tmp, "w", encoding="utf-8") as f:
            json.dump(self.m, f, indent=1, ensure_ascii=False)
        os.replace(tmp, self.manifest_path)
        self._mtime = os.stat(self.manifest_path).st_mtime_ns

    @property
    def game_dir(self):
        return self.m["game_dir"]

    @property
    def key(self):
        return self.m.get("key", DEFAULT_KEY)

    def backup(self):
        return Backup(self.game_dir, self.m.get("backup_dir", DEFAULT_BACKUP))

    def abspath(self, wrel):
        p = os.path.abspath(_j(self.path, _rel_to_os(wrel)))
        if not (p == self.path or p.startswith(self.path + os.sep)):
            raise ValueError("path escapes workspace: %s" % wrel)
        return p

    def sha(self, wrel):
        p = self.abspath(wrel)
        try:
            st = os.stat(p)
        except FileNotFoundError:
            return None
        k = (p, st.st_mtime_ns, st.st_size)
        v = self._sha_cache.get(k)
        if v is None:
            v = sha1_file(p)
            self._sha_cache[k] = v
        return v

    # ----------------------------------------------------------------- extraction
    def extract(self, game_dir=DEFAULT_GAME_DIR, slice_sprites=True, force=False, key=None, log=print,
                backup_dir=DEFAULT_BACKUP):
        game_dir = os.path.abspath(game_dir)
        if self.exists() and not force:
            raise RuntimeError("workspace already exists at %s (use --force to overwrite)" % self.path)
        if os.path.isdir(self.path) and force:
            if not os.path.exists(self.manifest_path) and not os.path.exists(_j(self.path, WS_MARKER)) \
                    and os.listdir(self.path):
                raise RuntimeError("refusing to wipe non-workspace folder %s" % self.path)
            shutil.rmtree(self.path)
        os.makedirs(self.path, exist_ok=True)
        _write(_j(self.path, WS_MARKER), b"")
        bk = Backup(game_dir, backup_dir)
        if key is None:
            key, src = detect_ean_key(Swf(bk.original_bytes(MAIN_SWF)))
            _log(log, "RC4 key: %r (%s)" % (key, src))
        self.m = dict(tool="RRModKit", version=__version__, created=datetime.datetime.now().isoformat(timespec="seconds"),
                      game_dir=game_dir, backup_dir=os.path.abspath(backup_dir), key=key, files={}, atlases={},
                      swfs={}, pairs={})
        rels = []
        for d in INCLUDE_DIRS:
            base = _j(game_dir, d)
            for root, _, files in os.walk(base):
                for fn in files:
                    rels.append(os.path.relpath(_j(root, fn), game_dir).replace(os.sep, "/"))
        rels += [r for r in INCLUDE_FILES if os.path.exists(_j(game_dir, _rel_to_os(r)))]
        rels = sorted(r for r in rels if r not in SWF_FILES)
        relset = set(rels)
        for rel in rels:
            self._extract_file(bk, rel, relset, log)
        for rel in SWF_FILES:
            if os.path.exists(bk.original_path(rel)):
                self._extract_swf(bk, rel, log)
        # atlases
        for rel, f in list(self.m["files"].items()):
            if rel.lower().endswith(".xml") and f["kind"] == "copy":
                data = _read(self.abspath(f["work"]))
                if is_atlas_xml(data):
                    self._register_atlas(rel, data, slice_sprites, log)
        self._pair_fonts()
        self.save()
        _log(log, "Workspace ready: %s" % self.path)
        return self

    def _work_name_for(self, rel, relset):
        base, ext = os.path.splitext(rel)
        ext = ext.lower()
        if ext == ".ean":
            cand = base + ".png"
            return ("ean", cand if cand not in relset else rel + ".png")
        if ext == ".eag":
            cand = base + ".jpg"
            return ("eag", cand if cand not in relset else rel + ".jpg")
        return ("copy", rel)

    def _materialize(self, bk, rel, kind, work):
        data = bk.original_bytes(rel)
        info = dict(orig_sha=sha1(data), orig_size=len(data))
        if kind == "ean":
            img = decode_ean(data, self.key)
            save_image_file(img, self.abspath(work))
            info.update(w=img.w, h=img.h)
        else:
            _write(self.abspath(work), data)
        return info

    def _extract_file(self, bk, rel, relset, log):
        kind, wrel = self._work_name_for(rel, relset)
        work = "files/" + wrel
        if kind == "eag":
            data = bk.original_bytes(rel)
            if sniff(data) == "png":
                work = "files/" + os.path.splitext(rel)[0] + ".png"
        _log(log, "  %-5s %s" % (kind, rel))
        info = self._materialize(bk, rel, kind, work)
        info.update(kind=kind, work=work, sha=self.sha(work))
        self.m["files"][rel] = info

    def _extract_swf(self, bk, rel, log):
        swf = Swf(bk.original_bytes(rel))
        d = "swf/" + _safe(os.path.splitext(os.path.basename(rel))[0])
        entry = dict(dir=d, assets={}, orig_sha=sha1_file(bk.original_path(rel)))
        for a in swf.assets():
            ext, data = swf.export_asset(a["id"])
            name = "%d_%s.%s" % (a["id"], _safe(a["symbol"]) if a["symbol"] else "char", ext)
            wrel = d + "/" + name
            _write(self.abspath(wrel), data)
            entry["assets"][str(a["id"])] = dict(file=wrel, sha=self.sha(wrel), tag=a["tag_name"], symbol=a["symbol"], ext=ext)
            _log(log, "  swf   %s#%d -> %s" % (rel, a["id"], wrel))
        self.m["swfs"][rel] = entry

    def _texture_for(self, xml_rel, root_attrs):
        d = os.path.dirname(xml_rel)
        img = root_attrs.get("imagePath", "")
        stem = os.path.splitext(img)[0] if img else os.path.splitext(os.path.basename(xml_rel))[0]
        for ext in (".ean", ".png", ".eag", ".jpg"):
            cand = (d + "/" if d else "") + stem + ext
            if cand in self.m["files"]:
                return cand
        return None

    def _register_atlas(self, xml_rel, data, slice_sprites, log):
        root, sprites = parse_atlas(data)
        tex = self._texture_for(xml_rel, root)
        if not tex:
            _log(log, "  ! atlas %s: texture %s not found" % (xml_rel, root.get("imagePath")))
            return
        sdir = "sprites/" + _safe(os.path.splitext(os.path.basename(xml_rel))[0])
        entry = dict(texture=tex, dir=sdir, sprites={})
        img = load_image_file(self.abspath(self.m["files"][tex]["work"])) if slice_sprites else None
        used = set()
        for s in sprites:
            fn = _safe(s["name"])
            k = 2
            while fn.lower() in used:
                fn = "%s_%d" % (_safe(s["name"]), k)
                k += 1
            used.add(fn.lower())
            wrel = "%s/%s.png" % (sdir, fn)
            if img is not None:
                _write(self.abspath(wrel), write_png(img.crop(s["x"], s["y"], s["width"], s["height"])))
            sha = self.sha(wrel)
            entry["sprites"][s["name"]] = dict(file=wrel, sha=sha, synced=sha)
        self.m["atlases"][xml_rel] = entry
        _log(log, "  atlas %s: %d sprites -> %s" % (xml_rel, len(sprites), sdir))

    def _pair_fonts(self):
        pairs = {}
        for rel, f in self.m["files"].items():
            if rel.lower().endswith(".fnt"):
                try:
                    fnt = parse_fnt(_read(self.abspath(f["work"])))
                    page = fnt["pages"].get(0)
                    cand = (os.path.dirname(rel) + "/" + page).lower() if page else None
                    match = next((r for r in self.m["files"] if r.lower() == cand), None)
                    if match:
                        pairs[f["work"]] = self.m["files"][match]["work"]
                except Exception:
                    pass
        for rel, s in self.m["swfs"].items():
            by_sym = {a["symbol"]: a for a in s["assets"].values()}
            for a in s["assets"].values():
                if a["ext"] == "fnt" and a["symbol"].endswith("XML"):
                    img = by_sym.get(a["symbol"][:-3])
                    if img:
                        pairs[a["file"]] = img["file"]
        self.m["pairs"] = pairs

    # ----------------------------------------------------------------- status
    def file_status(self, rel):
        f = self.m["files"][rel]
        ov = self.override_path(rel)
        if ov:
            return "override"
        cur = self.sha(f["work"])
        if cur is None:
            return "missing"
        return "modified" if cur != f["sha"] else "clean"

    def override_path(self, rel):
        f = self.m["files"][rel]
        if f["kind"] != "eag":
            return None
        ov = os.path.splitext(f["work"])[0] + ".png"
        if ov != f["work"] and os.path.exists(self.abspath(ov)):
            return ov
        return None

    def sprite_status(self, xml_rel, name):
        sp = self.m["atlases"][xml_rel]["sprites"].get(name)
        if not sp:
            return "new"
        cur = self.sha(sp["file"])
        if cur is None:
            return "missing"
        return "modified" if cur != sp["sha"] else "clean"

    def swf_asset_status(self, swf_rel, cid):
        a = self.m["swfs"][swf_rel]["assets"][str(cid)]
        cur = self.sha(a["file"])
        if cur is None:
            return "missing"
        return "modified" if cur != a["sha"] else "clean"

    def atlas_sprites(self, xml_rel):
        """Current sprite list from the working atlas XML, annotated."""
        a = self.m["atlases"][xml_rel]
        root, sprites = parse_atlas(_read(self.abspath(self.m["files"][xml_rel]["work"])))
        for s in sprites:
            sp = a["sprites"].get(s["name"])
            s["_file"] = sp["file"] if sp else None
            s["_status"] = self.sprite_status(xml_rel, s["name"]) if sp else "unsliced"
        return root, sprites

    def new_sprite_files(self, xml_rel):
        a = self.m["atlases"][xml_rel]
        known = {sp["file"].lower() for sp in a["sprites"].values()}
        d = self.abspath(a["dir"])
        out = []
        if os.path.isdir(d):
            for fn in sorted(os.listdir(d)):
                if fn.lower().endswith(".png"):
                    wrel = a["dir"] + "/" + fn
                    if wrel.lower() not in known:
                        out.append((os.path.splitext(fn)[0], wrel))
        return out

    # ----------------------------------------------------------------- atlas sync / repack
    def sync_atlas(self, xml_rel, force_repack=False, log=print, max_size=2048):
        """Apply edited/added sprite PNGs to the atlas texture (and XML if repacking)."""
        a = self.m["atlases"][xml_rel]
        xf = self.m["files"][xml_rel]
        tf = self.m["files"][a["texture"]]
        root, sprites = parse_atlas(_read(self.abspath(xf["work"])))
        by_name = {s["name"]: s for s in sprites}
        changed = []
        for name, sp in a["sprites"].items():
            cur = self.sha(sp["file"])
            if cur is not None and cur != sp["synced"] and name in by_name:
                changed.append(name)
        new = self.new_sprite_files(xml_rel)
        if not changed and not new and not force_repack:
            return None
        report = dict(atlas=xml_rel, changed=changed, added=[n for n, _ in new], repacked=False, warnings=[])
        atlas = load_image_file(self.abspath(tf["work"]))
        imgs = {}
        for name in changed:
            imgs[name] = load_image_file(self.abspath(a["sprites"][name]["file"]))
        for name, wrel in new:
            imgs[name] = load_image_file(self.abspath(wrel))
        need_repack = force_repack or bool(new) or any(
            (imgs[n].w, imgs[n].h) != (by_name[n]["width"], by_name[n]["height"]) for n in changed)
        if not need_repack:
            for n in changed:
                s = by_name[n]
                atlas.fill(s["x"], s["y"], s["width"], s["height"])
                atlas.paste(imgs[n], s["x"], s["y"])
        else:
            content = {}
            for s in sprites:
                n = s["name"]
                if n in imgs:
                    im = imgs[n]
                    if (im.w, im.h) != (s["width"], s["height"]) and "frameX" in s:
                        fx, fy = -s["frameX"], -s["frameY"]
                        if (im.w, im.h) == (s["frameWidth"], s["frameHeight"]):
                            for k in ("frameX", "frameY", "frameWidth", "frameHeight"):
                                s.pop(k, None)
                        else:
                            s["frameWidth"] = max(s["frameWidth"], fx + im.w)
                            s["frameHeight"] = max(s["frameHeight"], fy + im.h)
                            report["warnings"].append("%s: trimmed sprite resized; kept its frame offset" % n)
                    content[n] = im
                else:
                    content[n] = atlas.crop(s["x"], s["y"], s["width"], s["height"])
                s["width"], s["height"] = content[n].w, content[n].h
            for name, _ in new:
                if name in by_name:
                    continue
                s = dict(name=name, x=0, y=0, width=imgs[name].w, height=imgs[name].h)
                sprites.append(s)
                by_name[name] = s
                content[name] = imgs[name]
            items = [(s["name"], s["width"], s["height"]) for s in sprites]
            try:
                aw, ah, pos = pack(items, max_size=max_size)
            except ValueError:
                aw, ah, pos = pack(items, max_size=4096)
                report["warnings"].append("atlas needed 4096px - may fail on low-end GPUs (Stage3D baseline limit is 2048)")
            from .formats import Image
            atlas = Image(aw, ah)
            for s in sprites:
                s["x"], s["y"] = pos[s["name"]]
                atlas.paste(content[s["name"]], s["x"], s["y"])
            root_attrs = dict(root)
            xml_bytes = write_atlas(root_attrs, sprites, "Repacked by RRModKit %s" % __version__)
            _write(self.abspath(xf["work"]), xml_bytes)
            report.update(repacked=True, size="%dx%d" % (aw, ah))
            tf["w"], tf["h"] = aw, ah
        save_image_file(atlas, self.abspath(tf["work"]))
        for n in changed:
            a["sprites"][n]["synced"] = self.sha(a["sprites"][n]["file"])
        for name, wrel in new:
            sha = self.sha(wrel)
            # sha recorded as "" so the sprite shows up as modified vs. the original game
            a["sprites"][name] = dict(file=wrel, sha="", synced=sha, added=True)
        self.save()
        _log(log, "  atlas %s: %d changed, %d added%s" % (
            xml_rel, len(changed), len(new), ", repacked to " + report["size"] if report["repacked"] else ""))
        for w in report["warnings"]:
            _log(log, "    ! " + w)
        return report

    def sync_all(self, log=print):
        return [r for r in (self.sync_atlas(x, log=log) for x in list(self.m["atlases"])) if r]

    # ----------------------------------------------------------------- revert
    def revert_file(self, rel, log=print):
        f = self.m["files"][rel]
        bk = self.backup()
        ov = self.override_path(rel)
        if ov:
            os.remove(self.abspath(ov))
        self._materialize(bk, rel, f["kind"], f["work"])
        f["sha"] = self.sha(f["work"])
        # keep atlas sprites consistent with the reverted texture/xml
        for xml_rel, a in self.m["atlases"].items():
            if rel in (xml_rel, a["texture"]):
                other = a["texture"] if rel == xml_rel else xml_rel
                if self.file_status(other) != "clean":
                    self._materialize(bk, other, self.m["files"][other]["kind"], self.m["files"][other]["work"])
                    self.m["files"][other]["sha"] = self.sha(self.m["files"][other]["work"])
                d = self.abspath(a["dir"])
                if os.path.isdir(d):
                    shutil.rmtree(d)
                self._register_atlas(xml_rel, _read(self.abspath(self.m["files"][xml_rel]["work"])), True, log)
        self.save()

    def revert_sprite(self, xml_rel, name):
        a = self.m["atlases"][xml_rel]
        sp = a["sprites"][name]
        if sp.get("added"):
            os.remove(self.abspath(sp["file"]))
            del a["sprites"][name]
            self.save()
            # remove it from the atlas by repacking
            root, sprites = parse_atlas(_read(self.abspath(self.m["files"][xml_rel]["work"])))
            sprites = [s for s in sprites if s["name"] != name]
            _write(self.abspath(self.m["files"][xml_rel]["work"]), write_atlas(root, sprites, "Edited by RRModKit"))
            return self.sync_atlas(xml_rel, force_repack=True, log=None)
        bk = self.backup()
        root, osprites = parse_atlas(bk.original_bytes(xml_rel))
        o = {s["name"]: s for s in osprites}[name]
        tex = decode_ean(bk.original_bytes(a["texture"]), self.key) if self.m["files"][a["texture"]]["kind"] == "ean" \
            else read_png(bk.original_bytes(a["texture"]))
        _write(self.abspath(sp["file"]), write_png(tex.crop(o["x"], o["y"], o["width"], o["height"])))
        sp["sha"] = self.sha(sp["file"])
        return self.sync_atlas(xml_rel, log=None)

    def revert_swf_asset(self, swf_rel, cid):
        a = self.m["swfs"][swf_rel]["assets"][str(cid)]
        swf = Swf(self.backup().original_bytes(swf_rel))
        _, data = swf.export_asset(int(cid))
        _write(self.abspath(a["file"]), data)
        a["sha"] = self.sha(a["file"])
        self.save()

    # ----------------------------------------------------------------- rebuild
    def build(self, out_dir=DEFAULT_BUILD, force_all=False, log=print):
        """Convert changed workspace files back to game formats. Returns {rel: bytes}."""
        self.sync_all(log=log)
        outputs = {}
        errors = []
        for rel, f in sorted(self.m["files"].items()):
            src = self.override_path(rel) or f["work"]
            cur = self.sha(src)
            if cur is None:
                errors.append("%s: working file %s missing - skipped" % (rel, src))
                continue
            if not force_all and src == f["work"] and cur == f["sha"]:
                continue
            path = self.abspath(src)
            try:
                if f["kind"] == "ean":
                    img = load_image_file(path)
                    data = encode_ean(img, self.key)
                    _log(log, "  ean   %s (%dx%d)" % (rel, img.w, img.h))
                elif f["kind"] == "eag":
                    data = _read(path)
                    k = sniff(data)
                    if k not in ("jpg", "png", "gif"):
                        raise ValueError("background must be JPEG/PNG/GIF, got %s" % k)
                    _log(log, "  eag   %s (%s)" % (rel, k))
                else:
                    data = _read(path)
                    if rel.lower().endswith((".xml", ".fnt", ".pex")):
                        from .formats import xml_check
                        err = xml_check(data)
                        if err:
                            raise ValueError("XML not well-formed: " + err)
                    _log(log, "  copy  %s" % rel)
                outputs[rel] = data
            except Exception as e:
                errors.append("%s: %s" % (rel, e))
        bk = self.backup()
        for rel, s in sorted(self.m["swfs"].items()):
            changed = [(cid, a) for cid, a in s["assets"].items() if self.sha(a["file"]) not in (None, a["sha"])]
            if not changed:
                continue
            swf = Swf(bk.original_bytes(rel))
            for cid, a in changed:
                try:
                    for w in swf.replace_asset(int(cid), _read(self.abspath(a["file"]))):
                        _log(log, "    ! " + w)
                    _log(log, "  swf   %s#%s %s" % (rel, cid, a["file"]))
                except Exception as e:
                    errors.append("%s#%s: %s" % (rel, cid, e))
            outputs[rel] = swf.to_bytes()
        for e in errors:
            _log(log, "  ERROR " + e)
        if out_dir:
            out_dir = os.path.abspath(out_dir)
            if os.path.isdir(out_dir):
                if os.path.exists(_j(out_dir, BUILD_MARKER)) or not os.listdir(out_dir):
                    shutil.rmtree(out_dir)
                else:
                    raise RuntimeError("output folder %s exists and is not an RRModKit build folder" % out_dir)
            os.makedirs(out_dir, exist_ok=True)
            _write(_j(out_dir, BUILD_MARKER), b"")
            for rel, data in outputs.items():
                _write(_j(out_dir, _rel_to_os(rel)), data)
            with open(_j(out_dir, "build_report.json"), "w", encoding="utf-8") as fh:
                json.dump(dict(files=sorted(outputs), errors=errors,
                               built=datetime.datetime.now().isoformat(timespec="seconds")), fh, indent=1)
            _log(log, "Build: %d file(s) -> %s" % (len(outputs), out_dir))
        return outputs, errors

    def install(self, log=print, force_all=False):
        """Build and copy into the game folder, keeping pristine backups."""
        outputs, errors = self.build(out_dir=DEFAULT_BUILD, force_all=force_all, log=log)
        if errors:
            raise RuntimeError("build had errors, nothing installed:\n" + "\n".join(errors))
        bk = self.backup()
        backed = set(bk.state["backed_up"])
        added = set(bk.state["added"])
        game = self.game_dir
        for rel, data in sorted(outputs.items()):
            dest = _j(game, _rel_to_os(rel))
            if os.path.exists(dest) and rel not in backed and rel not in added:
                bpath = _j(bk.dir, "original", _rel_to_os(rel))
                os.makedirs(os.path.dirname(bpath), exist_ok=True)
                shutil.copy2(dest, bpath)
                backed.add(rel)
            elif not os.path.exists(dest):
                added.add(rel)
            _write(dest, data)
            _log(log, "  installed %s" % rel)
        # files modded earlier but now back to original in the workspace
        for rel in sorted(backed - set(outputs)):
            shutil.copy2(_j(bk.dir, "original", _rel_to_os(rel)), _j(game, _rel_to_os(rel)))
            _log(log, "  restored  %s" % rel)
        for rel in sorted(added - set(outputs)):
            p = _j(game, _rel_to_os(rel))
            if os.path.exists(p):
                os.remove(p)
            added.discard(rel)
        bk.state["backed_up"] = sorted(backed)
        bk.state["added"] = sorted(added)
        bk.save()
        _log(log, "Installed %d file(s) into %s" % (len(outputs), game))
        return outputs


def restore(game_dir=DEFAULT_GAME_DIR, backup_dir=DEFAULT_BACKUP, log=print):
    bk = Backup(os.path.abspath(game_dir), backup_dir)
    n = 0
    for rel in bk.state["backed_up"]:
        src = _j(bk.dir, "original", _rel_to_os(rel))
        if os.path.exists(src):
            shutil.copy2(src, _j(bk.game_dir, _rel_to_os(rel)))
            _log(log, "  restored %s" % rel)
            n += 1
    for rel in bk.state["added"]:
        p = _j(bk.game_dir, _rel_to_os(rel))
        if os.path.exists(p):
            os.remove(p)
            _log(log, "  removed  %s" % rel)
    bk.state = dict(backed_up=[], added=[])
    bk.save()
    _log(log, "Restored %d original file(s)" % n)
    return n
