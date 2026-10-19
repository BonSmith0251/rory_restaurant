"""Asset analyzer: inventory, format details, atlas health and cross-reference validation.

Works on the installed game folder or on a workspace (edited files), through a small "view"
abstraction so the same checks validate mods before they are installed."""
import os
import re
import struct
from collections import Counter, defaultdict

from .formats import (decode_ean, load_image_file, jpeg_info, png_size, mp3_info, parse_atlas, parse_fnt,
                      is_atlas_xml, xml_check, xml_strict_issues, sniff, rc4)
from .swf import Swf, detect_ean_key, all_strings, TAG_NAMES, load_abc
from . import xmltable


class GameView:
    """Read files from a game folder (game formats)."""
    kind = "game"

    def __init__(self, game_dir):
        self.root = os.path.abspath(game_dir)
        self._files = []
        for d in ("data", "branding", "icons", "game-shell-data"):
            for r, _, fs in os.walk(os.path.join(self.root, d)):
                for f in fs:
                    self._files.append(os.path.relpath(os.path.join(r, f), self.root).replace(os.sep, "/"))
        for f in ("RorysRestaurant.swf", "META-INF/AIR/application.xml"):
            if os.path.exists(os.path.join(self.root, f)):
                self._files.append(f)
        self._files.sort()

    def files(self):
        return list(self._files)

    def read(self, rel):
        with open(os.path.join(self.root, rel.replace("/", os.sep)), "rb") as f:
            return f.read()

    def image(self, rel, key):
        return decode_ean(self.read(rel), key)

    def label(self, rel):
        return rel


class WorkspaceView:
    """Read a workspace, presenting files under their game paths."""
    kind = "workspace"

    def __init__(self, ws):
        self.ws = ws
        self.root = ws.path

    def files(self):
        return sorted(list(self.ws.m["files"]) + list(self.ws.m["swfs"]))

    def read(self, rel):
        f = self.ws.m["files"].get(rel)
        if f is None:  # swf: analysed from the original
            return self.ws.backup().original_bytes(rel)
        src = self.ws.override_path(rel) or f["work"]
        with open(self.ws.abspath(src), "rb") as fh:
            return fh.read()

    def image(self, rel, key):
        f = self.ws.m["files"][rel]
        return load_image_file(self.ws.abspath(f["work"]))

    def label(self, rel):
        f = self.ws.m["files"].get(rel)
        return "%s (%s)" % (rel, f["work"]) if f else rel


def _issue(rep, level, where, msg):
    rep["issues"].append(dict(level=level, where=where, msg=msg))


def _is_pot(n):
    return n > 0 and (n & (n - 1)) == 0


def analyze(view, key=None, decode_images=True, log=None):
    rep = dict(source=view.root, view=view.kind, issues=[], inventory={}, textures=[], backgrounds=[], atlases=[],
               sounds=[], xml=[], fonts=[], swf={}, gameplay={}, references={})
    files = view.files()
    inv = defaultdict(lambda: [0, 0])
    for rel in files:
        try:
            size = len(view.read(rel)) if view.kind == "workspace" else os.path.getsize(
                os.path.join(view.root, rel.replace("/", os.sep)))
        except OSError:
            size = 0
        ext = os.path.splitext(rel)[1].lower() or "(none)"
        inv[ext][0] += 1
        inv[ext][1] += size
    rep["inventory"] = {k: dict(count=v[0], bytes=v[1]) for k, v in sorted(inv.items())}

    # ----------------------------------------------------------- SWF
    swf_strings = set()
    if "RorysRestaurant.swf" in files:
        try:
            swf = Swf(view.read("RorysRestaurant.swf"))
            k, src = detect_ean_key(swf)
            key = key or k
            abcs = load_abc(swf)
            classes = [c for a in abcs for c in a.classes]
            swf_strings = all_strings(swf)
            pkgs = Counter(c["fullname"].rsplit(".", 1)[0] if "." in c["fullname"] else "(top level)" for c in classes)
            rep["swf"] = dict(version=swf.version, frame=swf.frame_info(), key=key, key_source=src,
                              classes=len(classes), methods=sum(len(a.methods) for a in abcs),
                              packages=dict(pkgs.most_common(25)),
                              tags={k2: dict(count=v[0], bytes=v[1]) for k2, v in sorted(swf.tag_stats().items())},
                              embedded=[dict((k2, v) for k2, v in a.items()) for a in swf.assets()],
                              game_classes=sorted(c["fullname"] for c in classes
                                                  if not re.match(r"(starling|com\.|org\.|flash|mx\.|feathers)", c["fullname"])))
        except Exception as e:
            _issue(rep, "error", "RorysRestaurant.swf", "could not parse SWF: %s" % e)
    key = key or "crazy_face"

    texts = {}

    def text(rel):
        if rel not in texts:
            texts[rel] = view.read(rel)
        return texts[rel]

    # ----------------------------------------------------------- XML
    for rel in files:
        if rel.lower().endswith((".xml", ".fnt", ".pex")):
            d = text(rel)
            err = xml_check(d)
            entry = dict(file=rel, bytes=len(d), ok=err is None)
            if err:
                _issue(rep, "error", rel, "XML not well-formed: " + err)
            for s in xml_strict_issues(d):
                _issue(rep, "info", rel, s)
            rep["xml"].append(entry)

    # ----------------------------------------------------------- textures
    atlas_names = {}  # texture rel -> set of sprite names
    all_sprite_names = set()
    tex_dims = {}
    for rel in files:
        if not rel.lower().endswith(".ean"):
            continue
        t = dict(file=rel, bytes=len(text(rel)))
        if decode_images:
            try:
                img = view.image(rel, key)
                t.update(w=img.w, h=img.h, pot=_is_pot(img.w) and _is_pot(img.h), alpha=img.alpha_stats())
                tex_dims[rel] = (img.w, img.h)
                if max(img.w, img.h) > 2048:
                    _issue(rep, "warn", rel, "texture %dx%d exceeds 2048 (Stage3D baseline limit)" % (img.w, img.h))
                if log:
                    log("  texture %s %dx%d" % (rel, img.w, img.h))
            except Exception as e:
                _issue(rep, "error", rel, "cannot decode .ean: %s" % e)
        rep["textures"].append(t)
    for rel in files:
        if rel.lower().endswith((".eag", ".jpg", ".jpeg")) or (rel.lower().endswith(".png") and "/graphic/" in rel):
            d = text(rel)
            kind = sniff(d)
            info = jpeg_info(d) if kind == "jpg" else None
            ps = png_size(d) if kind == "png" else None
            e = dict(file=rel, bytes=len(d), format=kind)
            if info:
                e.update(w=info["w"], h=info["h"], progressive=info["progressive"])
            elif ps:
                e.update(w=ps[0], h=ps[1])
            if rel.lower().endswith(".eag") and kind not in ("jpg", "png", "gif"):
                _issue(rep, "error", rel, "background is %s - Loader can only read JPEG/PNG/GIF" % kind)
            rep["backgrounds"].append(e)

    # ----------------------------------------------------------- atlases
    for rel in files:
        if not rel.lower().endswith(".xml"):
            continue
        d = text(rel)
        if not is_atlas_xml(d):
            continue
        try:
            root, sprites = parse_atlas(d)
        except Exception as e:
            _issue(rep, "error", rel, "atlas parse failed: %s" % e)
            continue
        stem = os.path.splitext(root.get("imagePath", ""))[0]
        dirn = os.path.dirname(rel)
        tex = next((c for c in (dirn + "/" + stem + ext for ext in (".ean", ".png")) if c in files), None)
        a = dict(file=rel, texture=tex, sprites=len(sprites), trimmed=sum(1 for s in sprites if "frameX" in s))
        if not tex:
            _issue(rep, "error", rel, "texture for imagePath=%r not found" % root.get("imagePath"))
        names = Counter(s["name"] for s in sprites)
        for n, c in names.items():
            if c > 1:
                _issue(rep, "warn", rel, "duplicate sprite name %r (%d times)" % (n, c))
        dims = tex_dims.get(tex)
        if dims:
            W, H = dims
            used = 0
            for s in sprites:
                used += s["width"] * s["height"]
                if s["x"] < 0 or s["y"] < 0 or s["x"] + s["width"] > W or s["y"] + s["height"] > H:
                    _issue(rep, "error", rel, "sprite %r (%d,%d %dx%d) lies outside the %dx%d texture" % (
                        s["name"], s["x"], s["y"], s["width"], s["height"], W, H))
            a["coverage"] = round(used / float(W * H), 3)
            a["texture_size"] = "%dx%d" % (W, H)
            # overlaps (sweep on x)
            ss = sorted(sprites, key=lambda s: s["x"])
            overlaps = 0
            for i, s in enumerate(ss):
                for t2 in ss[i + 1:]:
                    if t2["x"] >= s["x"] + s["width"]:
                        break
                    if t2["y"] < s["y"] + s["height"] and s["y"] < t2["y"] + t2["height"]:
                        overlaps += 1
                        if overlaps <= 5:
                            _issue(rep, "warn", rel, "sprites %r and %r overlap" % (s["name"], t2["name"]))
            a["overlaps"] = overlaps
        rep["atlases"].append(a)
        atlas_names[rel] = set(names)
        all_sprite_names |= set(names)

    # ----------------------------------------------------------- fonts / particles (loose files)
    for rel in files:
        if rel.lower().endswith(".fnt"):
            try:
                f = parse_fnt(text(rel))
                page = f["pages"].get(0)
                prel = os.path.dirname(rel) + "/" + page if page else None
                e = dict(file=rel, face=f["info"].get("face"), size=f["info"].get("size"), chars=len(f["chars"]),
                         page=prel)
                lower = {x.lower(): x for x in files}
                prel = lower.get(prel.lower(), prel) if prel else None
                e["page"] = prel
                if prel and prel not in files:
                    _issue(rep, "warn", rel, "font page %s missing" % prel)
                rep["fonts"].append(e)
            except Exception as ex:
                _issue(rep, "error", rel, "font parse failed: %s" % ex)

    # ----------------------------------------------------------- sounds
    sound_list = []
    if "data/sound_file.xml" in files:
        t = xmltable.table(text("data/sound_file.xml"))
        sound_list = [r["fields"].get("#text", "") for r in t["rows"]]
        for s in sound_list:
            if "data/sounds/" + s not in files:
                _issue(rep, "error", "data/sound_file.xml", "lists %s but data/sounds/%s does not exist" % (s, s))
        dup = [s for s, c in Counter(sound_list).items() if c > 1]
        for s in dup:
            _issue(rep, "warn", "data/sound_file.xml", "%s listed more than once" % s)
    for rel in files:
        if rel.lower().endswith(".mp3"):
            d = text(rel)
            info = mp3_info(d) or {}
            e = dict(file=rel, bytes=len(d), listed=os.path.basename(rel) in sound_list, **info)
            if not info:
                _issue(rep, "error", rel, "no valid MP3 frames found")
            elif info.get("sample_rate") not in (44100, 22050, 11025, 5512):
                _issue(rep, "warn", rel, "sample rate %s Hz: Flash plays MP3s best at 44.1/22.05/11.025 kHz" % info.get("sample_rate"))
            if sound_list and not e["listed"]:
                _issue(rep, "info", rel, "not listed in data/sound_file.xml, so the game never loads it")
            rep["sounds"].append(e)

    # ----------------------------------------------------------- gameplay data cross references
    gp = rep["gameplay"]

    def tbl(rel, tag=None):
        return xmltable.table(text(rel), tag) if rel in files else None

    t = tbl("data/LevelSet.xml", "day")
    if t:
        days = [r["fields"] for r in t["rows"]]
        gp["levels"] = dict(count=len(days), by_type=dict(Counter(d.get("gameType", "") for d in days)),
                            by_region=dict(Counter(d.get("region", "") or "-" for d in days)))
        for i, dd in enumerate(days, 1):
            if dd.get("gameType") not in ("M3", "M3S", "HO", "DM"):
                _issue(rep, "warn", "data/LevelSet.xml", "day %d: unknown gameType %r" % (i, dd.get("gameType")))
            for fld in ("time", "goal", "expert"):
                v = dd.get(fld, "")
                if not re.fullmatch(r"-?\d+(\.\d+)?", v or ""):
                    _issue(rep, "error", "data/LevelSet.xml", "day %d: %s=%r is not a number" % (i, fld, v))
            if dd.get("gameType") in ("M3", "M3S") and dd.get("region") not in ("W", "C", "ME"):
                _issue(rep, "warn", "data/LevelSet.xml", "day %d: match-3 level with region %r" % (i, dd.get("region")))
    t = tbl("data/Customer.xml", "customer")
    if t:
        gp["customers"] = len(t["rows"])
        for r in t["rows"]:
            tn = r["fields"].get("@textureName")
            if tn and all_sprite_names and tn not in all_sprite_names:
                _issue(rep, "error", "data/Customer.xml", "customer %r uses missing sprite %r" % (r["fields"].get("@name"), tn))
    t = tbl("data/Dish.xml", "dish")
    if t:
        gp["dishes"] = dict(count=len(t["rows"]), by_region=dict(Counter(r["fields"].get("@region") for r in t["rows"])))
        for r in t["rows"]:
            f = r["fields"]
            ing = (f.get("@ingredient") or "").split(",")
            tgt = (f.get("@defaultTarget") or "").split(",")
            if len(ing) != len(tgt):
                _issue(rep, "error", "data/Dish.xml", "%r: %d ingredients but %d targets" % (f.get("@dishName"), len(ing), len(tgt)))
    t = tbl("data/LevelEvent.xml", "window")
    if t:
        gp["level_event_windows"] = len(t["rows"])
        for r in t["rows"]:
            tex = r["fields"].get("texture", "")
            if tex and all_sprite_names and tex not in all_sprite_names:
                _issue(rep, "error", "data/LevelEvent.xml", "%s: window texture %r not found in any atlas" % (r["context"], tex))
    t = tbl("data/Tutorial_and_Dialogue.xml", "text")
    if t:
        gp["dialogue_lines"] = len(t["rows"])
    mk = "data/graphic/hd/make_dessert.xml"
    t = tbl("data/MakeDessert.xml", "item")
    if t and mk in atlas_names:
        gp["dessert_items"] = len(t["rows"])
        for r in t["rows"]:
            n = r["fields"].get("@name")
            if n and n not in atlas_names[mk]:
                _issue(rep, "warn", "data/MakeDessert.xml", "item %r has no sprite in make_dessert atlas" % n)
    for rel in files:
        m = re.match(r"data/graphic/hd/(HO\d_[A-Z])_data\.xml$", rel)
        if m:
            atlas = "data/graphic/hd/%s.xml" % m.group(1)
            t = tbl(rel, "item")
            if t and atlas in atlas_names:
                for r in t["rows"]:
                    n = r["fields"].get("@name")
                    if n and n not in atlas_names[atlas]:
                        _issue(rep, "error", rel, "hidden object %r has no sprite in %s" % (n, atlas))
                missing = [n for n in atlas_names[atlas] if n not in {r["fields"].get("@name") for r in t["rows"]}]
                if missing:
                    _issue(rep, "info", rel, "%d atlas sprites have no placement (e.g. %s)" % (len(missing), ", ".join(sorted(missing)[:4])))

    # ----------------------------------------------------------- file usage vs code references
    if swf_strings:
        used, unused = [], []
        for rel in files:
            if rel in ("RorysRestaurant.swf",) or not rel.startswith("data/"):
                continue
            base = os.path.basename(rel)
            stem = os.path.splitext(base)[0]
            ref = (rel in swf_strings or base in swf_strings or
                   (rel.startswith("data/sounds/") and base in sound_list) or
                   (rel.startswith("data/graphic/hd/") and (stem in swf_strings or
                                                            stem.rsplit("_", 1)[0] in swf_strings or
                                                            re.sub(r"_(BG|data)$", "", stem) in swf_strings)))
            (used if ref else unused).append(rel)
        rep["references"] = dict(referenced=len(used), unreferenced=unused)
        for rel in unused:
            if not rel.startswith("data/sounds/"):
                _issue(rep, "info", rel, "not referenced by game code (the SWF embeds its own copy or it is unused)")

    order = {"error": 0, "warn": 1, "info": 2}
    rep["issues"].sort(key=lambda i: (order[i["level"]], i["where"]))
    rep["summary"] = dict(Counter(i["level"] for i in rep["issues"]))
    return rep


def format_text(rep):
    L = []
    w = L.append
    w("RRModKit analysis of %s (%s)" % (rep["source"], rep["view"]))
    w("=" * 78)
    w("Inventory:")
    for ext, v in rep["inventory"].items():
        w("  %-8s %4d files %10.1f KB" % (ext, v["count"], v["bytes"] / 1024))
    s = rep.get("swf") or {}
    if s:
        w("")
        w("SWF: version %s, %s, %d classes / %d methods, .ean key %r (%s)" % (
            s["version"], "%(width)gx%(height)g @%(fps)gfps" % s["frame"], s["classes"], s["methods"], s["key"], s["key_source"]))
        w("  Embedded assets:")
        for a in s["embedded"]:
            w("    #%-3d %-20s %-40s %s" % (a["id"], a["tag_name"], a["symbol"] or "-",
                                         ("%dx%d" % (a["w"], a["h"])) if "w" in a else a.get("ext", "")))
        w("  Game classes: %s" % ", ".join(s["game_classes"][:60]) + (" ..." if len(s["game_classes"]) > 60 else ""))
    w("")
    w("Textures (.ean):")
    for t in rep["textures"]:
        al = t.get("alpha") or {}
        w("  %-40s %-10s %8.1f KB  %s  transparent %3d%%" % (
            t["file"], "%sx%s" % (t.get("w", "?"), t.get("h", "?")), t["bytes"] / 1024,
            "POT " if t.get("pot") else "NPOT", round(al.get("transparent", 0) * 100)))
    w("")
    w("Backgrounds / images:")
    for b in rep["backgrounds"]:
        w("  %-40s %-5s %-10s %8.1f KB" % (b["file"], b["format"], "%sx%s" % (b.get("w", "?"), b.get("h", "?")), b["bytes"] / 1024))
    w("")
    w("Atlases:")
    for a in rep["atlases"]:
        w("  %-40s %3d sprites (%d trimmed) on %-9s coverage %s%%" % (
            a["file"], a["sprites"], a["trimmed"], a.get("texture_size", "?"),
            round(a["coverage"] * 100) if "coverage" in a else "?"))
    w("")
    w("Sounds: %d files, %.1f s total" % (len(rep["sounds"]), sum(x.get("duration", 0) for x in rep["sounds"])))
    for x in rep["sounds"]:
        w("  %-45s %6.2fs %5s kbps %6s Hz %s%s" % (x["file"], x.get("duration", 0), x.get("bitrate", "?"),
                                                  x.get("sample_rate", "?"), x.get("channels", ""),
                                                  "" if x.get("listed") else "  (unlisted)"))
    w("")
    w("Gameplay data: %s" % ", ".join("%s=%s" % kv for kv in rep["gameplay"].items()))
    if rep.get("references"):
        w("Files referenced by code: %d, unreferenced: %d" % (rep["references"]["referenced"], len(rep["references"]["unreferenced"])))
    w("")
    w("Issues: %s" % (", ".join("%d %s" % (v, k) for k, v in rep["summary"].items()) or "none"))
    for i in rep["issues"]:
        w("  [%-5s] %s: %s" % (i["level"], i["where"], i["msg"]))
    return "\n".join(L)
