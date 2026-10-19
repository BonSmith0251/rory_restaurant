"""Local web editor (http://127.0.0.1:<port>) - stdlib http.server + a single-page UI in web/."""
import io
import json
import mimetypes
import os
import re
import socket
import subprocess
import threading
import traceback
import webbrowser
from http.server import ThreadingHTTPServer, BaseHTTPRequestHandler
from urllib.parse import urlparse, parse_qs

from . import xmltable, __version__
from .analyze import analyze, GameView, WorkspaceView
from .formats import (read_png, load_image_file, png_size, jpeg_info, mp3_info, parse_fnt, fnt_as_sprites, sniff,
                      xml_check, write_png, sha1)
from .swf import Swf, load_abc
from .workspace import Workspace, restore, DEFAULT_BUILD, _safe

WEB_DIR = os.path.join(os.path.dirname(os.path.abspath(__file__)), "web")
LOCK = threading.RLock()

TEXT_EXT = (".xml", ".fnt", ".pex", ".txt", ".json", ".msg")
IMAGE_EXT = (".png", ".jpg", ".jpeg", ".gif", ".bmp", ".ico")


class ApiError(Exception):
    pass


class App:
    def __init__(self, ws):
        self.ws = ws
        self._abc = None

    # ----------------------------------------------------------------- handles
    def resolve(self, h):
        """handle -> dict(path(work-rel), kind, status, rel...)"""
        m = self.ws.m
        if h.startswith("file:"):
            rel = h[5:]
            f = m["files"].get(rel)
            if not f:
                raise ApiError("unknown file %s" % rel)
            ov = self.ws.override_path(rel)
            return dict(h=h, rel=rel, work=ov or f["work"], base_work=f["work"], kind=f["kind"],
                        status=self.ws.file_status(rel), ext=os.path.splitext(ov or f["work"])[1].lower())
        if h.startswith("swf:"):
            swf_rel, _, cid = h[4:].rpartition("#")
            a = m["swfs"].get(swf_rel, {}).get("assets", {}).get(cid)
            if not a:
                raise ApiError("unknown swf asset %s" % h)
            return dict(h=h, swf=swf_rel, cid=cid, work=a["file"], base_work=a["file"], kind="swf",
                        status=self.ws.swf_asset_status(swf_rel, cid), ext="." + a["ext"], symbol=a["symbol"],
                        tag=a["tag"])
        raise ApiError("bad handle %s" % h)

    def view_type(self, r):
        ext = r["ext"]
        if r.get("kind") == "ean" or ext in IMAGE_EXT:
            return "image"
        if ext == ".mp3":
            return "sound"
        if ext == ".fnt":
            return "font"
        if ext in TEXT_EXT:
            return "text"
        return "binary"

    # ----------------------------------------------------------------- state
    def state(self):
        m = self.ws.m
        atlas_tex = {a["texture"]: x for x, a in m["atlases"].items()}
        cats = {}

        def add(cat, item):
            cats.setdefault(cat, []).append(item)

        unused = re.compile(r"^data/(fonts/|graphic/particle/|settings\.xml$)")
        for xml_rel, a in sorted(m["atlases"].items()):
            st = self.ws.file_status(xml_rel)
            tst = self.ws.file_status(a["texture"])
            nmod = sum(1 for n in a["sprites"] if self.ws.sprite_status(xml_rel, n) != "clean")
            status = "modified" if (st != "clean" or tst != "clean" or nmod) else "clean"
            add("Sprite atlases", dict(id="atlas:" + xml_rel, h="file:" + xml_rel, label=os.path.basename(xml_rel)[:-4],
                                       type="atlas", status=status, detail="%d sprites" % len(a["sprites"]), rel=xml_rel))
        for rel, f in sorted(m["files"].items()):
            if rel in m["atlases"] or rel in atlas_tex:
                continue
            r = self.resolve("file:" + rel)
            vt = self.view_type(r)
            item = dict(id="file:" + rel, h="file:" + rel, label=os.path.basename(rel), type=vt, status=r["status"], rel=rel)
            if unused.match(rel) or rel == "data/graphic/logo.swf":
                add("Unused loose copies", item)
            elif f["kind"] == "eag":
                add("Backgrounds (.eag)", item)
            elif f["kind"] == "ean":
                add("Other textures (.ean)", item)
            elif rel.startswith("data/sounds/"):
                add("Sounds", item)
            elif re.match(r"data/graphic/hd/HO\d_[A-Z]_data\.xml", rel):
                add("Hidden object layouts", item)
            elif rel.startswith("data/") and vt == "text":
                add("Game data", item)
            else:
                add("Launcher, icons & branding", item)
        for swf_rel, s in sorted(m["swfs"].items()):
            for cid, a in sorted(s["assets"].items(), key=lambda kv: int(kv[0])):
                r = self.resolve("swf:%s#%s" % (swf_rel, cid))
                add("Embedded in " + os.path.basename(swf_rel), dict(
                    id=r["h"], h=r["h"], label="#%s %s" % (cid, a["symbol"] or a["tag"]), type=self.view_type(r),
                    status=r["status"], detail=a["tag"]))
        order = ["Sprite atlases", "Backgrounds (.eag)", "Other textures (.ean)", "Game data", "Hidden object layouts",
                 "Sounds", "Embedded in RorysRestaurant.swf", "Embedded in logo.swf", "Launcher, icons & branding",
                 "Unused loose copies"]
        out = [dict(name=c, items=cats[c]) for c in order if c in cats]
        out += [dict(name=c, items=v) for c, v in cats.items() if c not in order]
        installed = self.ws.backup().installed()
        return dict(version=__version__, workspace=self.ws.path, game_dir=self.ws.game_dir, key=self.ws.key,
                    categories=out, installed=installed)

    # ----------------------------------------------------------------- item details
    def item(self, h):
        r = self.resolve(h)
        vt = self.view_type(r)
        p = self.ws.abspath(r["work"])
        info = dict(h=h, type=vt, status=r["status"], work=r["work"], size=os.path.getsize(p) if os.path.exists(p) else 0,
                    rel=r.get("rel"), kind=r["kind"], symbol=r.get("symbol"), tag=r.get("tag"))
        data = open(p, "rb").read() if os.path.exists(p) else b""
        if vt == "image":
            ps = png_size(data)
            ji = jpeg_info(data)
            if ps:
                info.update(w=ps[0], h_px=ps[1], format="png")
            elif ji:
                info.update(w=ji["w"], h_px=ji["h"], format="jpg")
            else:
                info.update(format=sniff(data))
        elif vt == "sound":
            info["mp3"] = mp3_info(data)
        elif vt in ("text", "font"):
            info["table_tags"] = []
            try:
                info["table_tags"] = xmltable.table(data)["tags"] if vt == "text" else []
            except Exception:
                pass
            info["xml_error"] = xml_check(data) if r["ext"] in (".xml", ".fnt", ".pex") else None
            if vt == "font":
                pair = self.ws.m.get("pairs", {}).get(r["work"])
                info["page_h"] = self._handle_for_work(pair) if pair else None
        if r.get("rel") and r["kind"] == "eag":
            info["has_override"] = bool(self.ws.override_path(r["rel"]))
        return info

    def _handle_for_work(self, work):
        for rel, f in self.ws.m["files"].items():
            if f["work"] == work:
                return "file:" + rel
        for swf_rel, s in self.ws.m["swfs"].items():
            for cid, a in s["assets"].items():
                if a["file"] == work:
                    return "swf:%s#%s" % (swf_rel, cid)
        return None

    def atlas(self, xml_rel):
        a = self.ws.m["atlases"].get(xml_rel)
        if not a:
            raise ApiError("not an atlas: %s" % xml_rel)
        root, sprites = self.ws.atlas_sprites(xml_rel)
        tex = self.resolve("file:" + a["texture"])
        ps = png_size(open(self.ws.abspath(tex["work"]), "rb").read(64))
        pending = [n for n, _ in self.ws.new_sprite_files(xml_rel)]
        return dict(xml=xml_rel, texture_h=tex["h"], texture_status=tex["status"], xml_status=self.ws.file_status(xml_rel),
                    w=ps[0], h=ps[1], root=root, pending_new=pending,
                    sprites=[dict(name=s["name"], x=s["x"], y=s["y"], w=s["width"], h=s["height"],
                                  frame=[s["frameX"], s["frameY"], s["frameWidth"], s["frameHeight"]] if "frameX" in s else None,
                                  status=s["_status"]) for s in sprites])

    def font(self, h):
        r = self.resolve(h)
        data = open(self.ws.abspath(r["work"]), "rb").read()
        f = parse_fnt(data)
        pair = self.ws.m.get("pairs", {}).get(r["work"])
        ph = self._handle_for_work(pair) if pair else None
        w = hh = 0
        if ph:
            ps = png_size(open(self.ws.abspath(self.resolve(ph)["work"]), "rb").read(64))
            w, hh = ps
        return dict(info=f["info"], common=f["common"], page_h=ph, w=w, h=hh,
                    sprites=[dict(name=s["name"], x=s["x"], y=s["y"], w=s["width"], h=s["height"], status="clean")
                             for s in fnt_as_sprites(f)])

    # ----------------------------------------------------------------- mutations
    def replace(self, h, data):
        r = self.resolve(h)
        k = sniff(data)
        vt = self.view_type(r)
        dest = r["base_work"]
        if r["kind"] == "ean" or (vt == "image" and r["ext"] == ".png"):
            if k != "png":
                raise ApiError("this texture must be replaced with a PNG file (got %s)" % k)
            read_png(data)  # validate
        elif r["kind"] == "eag":
            if k == "png":
                read_png(data)
                dest = os.path.splitext(r["base_work"])[0] + ".png"
            elif k != "jpg":
                raise ApiError("backgrounds accept JPEG or PNG (got %s)" % k)
            else:
                ov = self.ws.override_path(r["rel"])
                if ov:
                    os.remove(self.ws.abspath(ov))
        elif r["kind"] == "swf" and r["ext"] in (".png", ".jpg"):
            if k not in ("png", "jpg"):
                raise ApiError("embedded images accept PNG or JPEG")
            if k == "png":
                read_png(data)
            if r["ext"] == ".png" and k != "png":
                raise ApiError("this embedded bitmap must be replaced with a PNG")
            if r["ext"] == ".jpg" and k == "png":
                raise ApiError("upload a JPEG here (PNG replacement for JPEG characters: use the CLI)")
        elif vt == "sound":
            if not mp3_info(data):
                raise ApiError("not a valid MP3 file")
        elif vt in ("text", "font"):
            err = xml_check(data) if r["ext"] in (".xml", ".fnt", ".pex") else None
            if err:
                raise ApiError("XML error: " + err)
        with open(self.ws.abspath(dest), "wb") as f:
            f.write(data)
        return self._after_file_change(r)

    def _after_file_change(self, r):
        # editing an atlas texture directly: sprites must be re-sliced from it to stay consistent
        rel = r.get("rel")
        for xml_rel, a in self.ws.m["atlases"].items():
            if rel in (a["texture"], xml_rel):
                self._reslice(xml_rel)
        return dict(ok=True, status=self.resolve(r["h"])["status"])

    def _reslice(self, xml_rel):
        a = self.ws.m["atlases"][xml_rel]
        root, sprites = self.ws.atlas_sprites(xml_rel)
        img = load_image_file(self.ws.abspath(self.ws.m["files"][a["texture"]]["work"]))
        for s in sprites:
            sp = a["sprites"].get(s["name"])
            if sp:
                with open(self.ws.abspath(sp["file"]), "wb") as f:
                    f.write(write_png(img.crop(s["x"], s["y"], s["width"], s["height"])))
                sp["synced"] = self.ws.sha(sp["file"])
        self.ws.save()

    def revert(self, h):
        r = self.resolve(h)
        if r["kind"] == "swf":
            self.ws.revert_swf_asset(r["swf"], r["cid"])
        else:
            self.ws.revert_file(r["rel"], log=None)
        return dict(ok=True)

    def get_text(self, h):
        r = self.resolve(h)
        data = open(self.ws.abspath(r["work"]), "rb").read()
        enc = xmltable._enc(data)
        bom = data.startswith(b"\xef\xbb\xbf")
        try:
            text = data.decode("utf-8-sig" if enc.lower().replace("_", "-") in ("utf-8", "utf8") else enc)
        except (UnicodeDecodeError, LookupError):
            text = data.decode("latin-1")
            enc = "latin-1"
        return dict(text=text, encoding=enc, bom=bom, sha=sha1(data),
                    error=xml_check(data) if r["ext"] in (".xml", ".fnt", ".pex") else None)

    def put_text(self, h, body):
        r = self.resolve(h)
        cur = self.get_text(h)
        if body.get("sha") and body["sha"] != cur["sha"]:
            raise ApiError("file changed on disk since it was opened - reload first")
        enc = cur["encoding"] if cur["encoding"].lower() not in ("utf-8", "utf8") else "utf-8"
        data = body["text"].encode(enc, "xmlcharrefreplace")
        if cur["bom"]:
            data = b"\xef\xbb\xbf" + data
        err = xml_check(data) if r["ext"] in (".xml", ".fnt", ".pex") else None
        if err and not body.get("force"):
            raise ApiError("XML error, not saved: " + err)
        with open(self.ws.abspath(r["base_work"]), "wb") as f:
            f.write(data)
        res = self._after_file_change(r)
        res["sha"] = sha1(data)
        return res

    def get_table(self, h, tag=None):
        r = self.resolve(h)
        return xmltable.table(open(self.ws.abspath(r["work"]), "rb").read(), tag)

    def put_table(self, h, body):
        r = self.resolve(h)
        p = self.ws.abspath(r["base_work"])
        data = open(p, "rb").read()
        new = xmltable.apply(data, body["tag"], body.get("edits"), body.get("sha"), body.get("row_ops"))
        err = xml_check(new)
        if err:
            raise ApiError("edit would produce invalid XML: " + err)
        with open(p, "wb") as f:
            f.write(new)
        self._after_file_change(r)
        return xmltable.table(new, body["tag"])

    def sprite_path(self, xml_rel, name):
        a = self.ws.m["atlases"][xml_rel]
        sp = a["sprites"].get(name)
        if sp:
            return sp["file"]
        for n, wrel in self.ws.new_sprite_files(xml_rel):
            if n == name:
                return wrel
        raise ApiError("unknown sprite %s" % name)

    def put_sprite(self, xml_rel, name, data, new=False):
        if sniff(data) != "png":
            raise ApiError("sprites must be PNG files")
        img = read_png(data)
        a = self.ws.m["atlases"][xml_rel]
        if new:
            if not name or name in a["sprites"]:
                raise ApiError("sprite name empty or already exists")
            wrel = "%s/%s.png" % (a["dir"], _safe(name))
            if os.path.exists(self.ws.abspath(wrel)):
                raise ApiError("a sprite file with that name already exists")
        else:
            wrel = self.sprite_path(xml_rel, name)
        os.makedirs(os.path.dirname(self.ws.abspath(wrel)), exist_ok=True)
        with open(self.ws.abspath(wrel), "wb") as f:
            f.write(data)
        logs = []
        rep = self.ws.sync_atlas(xml_rel, log=logs.append)
        return dict(ok=True, report=rep, log=logs, size=[img.w, img.h])

    # ----------------------------------------------------------------- code browser
    def abcs(self):
        if self._abc is None:
            swf = Swf(self.ws.backup().original_bytes("RorysRestaurant.swf"))
            self._abc = load_abc(swf)
        return self._abc

    def classes(self):
        out = []
        for ai, abc in enumerate(self.abcs()):
            for c in abc.classes:
                meths = []
                for kind, nm, mi in [("init", "<init>", c["iinit"]), ("cinit", "<cinit>", c["cinit"])] + \
                        [(k, n, m) for k, n, m in c["itraits"] if m is not None] + \
                        [("static " + k, n, m) for k, n, m in c["ctraits"] if m is not None]:
                    meths.append(dict(name=nm, kind=kind, m=mi, abc=ai, size=len(abc.bodies.get(mi, b""))))
                fields = [dict(name=n, kind=k) for k, n, m in c["itraits"] + c["ctraits"] if m is None]
                out.append(dict(name=c["fullname"], super=c["super"], interface=c["interface"], methods=meths, fields=fields))
        out.sort(key=lambda c: c["name"].lower())
        return out

    def disasm(self, abc_i, m):
        abc = self.abcs()[abc_i]
        if m not in abc.bodies:
            return dict(text="(no body - native or interface method)")
        return dict(name=abc.methods[m]["qual"], text=abc.disassemble(m))

    def search_strings(self, q):
        q = q.lower()
        out = []
        for abc in self.abcs():
            out += [s for s in abc.strs if s and q in s.lower()]
        return sorted(set(out))[:500]

    # ----------------------------------------------------------------- build
    def build(self, install=False, force_all=False):
        logs = []
        try:
            if install:
                outputs = self.ws.install(log=logs.append, force_all=force_all)
                errors = []
            else:
                outputs, errors = self.ws.build(out_dir=DEFAULT_BUILD, force_all=force_all, log=logs.append)
            return dict(ok=not errors, files=sorted(outputs), errors=errors, log="\n".join(logs), build_dir=DEFAULT_BUILD)
        except Exception as e:
            logs.append("ERROR: %s" % e)
            return dict(ok=False, files=[], errors=[str(e)], log="\n".join(logs))

    def restore(self):
        logs = []
        n = restore(self.ws.game_dir, self.ws.m.get("backup_dir"), log=logs.append)
        return dict(ok=True, restored=n, log="\n".join(logs))

    def launch(self):
        exe = os.path.join(self.ws.game_dir, "RorysRestaurant.exe")
        if not os.path.exists(exe):
            raise ApiError("RorysRestaurant.exe not found")
        subprocess.Popen([exe], cwd=self.ws.game_dir)
        return dict(ok=True)


def make_handler(app):
    class H(BaseHTTPRequestHandler):
        server_version = "RRModKit/" + __version__

        def log_message(self, fmt, *args):
            pass

        def _send(self, code, body, ctype="application/json; charset=utf-8", extra=None):
            if isinstance(body, (dict, list)):
                body = json.dumps(body, ensure_ascii=False).encode("utf-8")
            elif isinstance(body, str):
                body = body.encode("utf-8")
            self.send_response(code)
            self.send_header("Content-Type", ctype)
            self.send_header("Content-Length", str(len(body)))
            self.send_header("Cache-Control", "no-store")
            for k, v in (extra or {}).items():
                self.send_header(k, v)
            self.end_headers()
            self.wfile.write(body)

        def _body(self):
            n = int(self.headers.get("Content-Length") or 0)
            return self.rfile.read(n) if n else b""

        def _json(self):
            b = self._body()
            return json.loads(b.decode("utf-8")) if b else {}

        def _check_origin(self):
            # block cross-site requests from other web pages to this local server
            origin = self.headers.get("Origin")
            host = self.headers.get("Host", "")
            if origin and urlparse(origin).netloc != host:
                raise ApiError("cross-origin request refused")
            if not re.match(r"^(127\.0\.0\.1|localhost)(:\d+)?$", host):
                raise ApiError("bad host header")

        def _file(self, path):
            ctype = mimetypes.guess_type(path)[0] or "application/octet-stream"
            if path.endswith((".fnt", ".pex")):
                ctype = "text/xml"
            with open(path, "rb") as f:
                self._send(200, f.read(), ctype)

        def do_GET(self):
            self._handle("GET")

        def do_POST(self):
            self._handle("POST")

        def _handle(self, method):
            u = urlparse(self.path)
            q = {k: v[0] for k, v in parse_qs(u.query).items()}
            try:
                self._check_origin()
                if method == "GET" and (u.path == "/" or u.path.startswith("/static/")):
                    name = "index.html" if u.path == "/" else u.path[len("/static/"):]
                    p = os.path.abspath(os.path.join(WEB_DIR, name))
                    if not p.startswith(WEB_DIR) or not os.path.isfile(p):
                        return self._send(404, "not found", "text/plain")
                    return self._file(p)
                with LOCK:
                    app.ws.reload_if_changed()
                    res = self._route(method, u.path, q)
                if isinstance(res, tuple):
                    return self._send(200, res[1], res[0], res[2] if len(res) > 2 else None)
                return self._send(200, res)
            except ApiError as e:
                return self._send(400, dict(error=str(e)))
            except Exception as e:
                traceback.print_exc()
                return self._send(500, dict(error="%s: %s" % (type(e).__name__, e)))

        def _route(self, method, path, q):
            G = method == "GET"
            if path == "/api/state" and G:
                return app.state()
            if path == "/api/item" and G:
                return app.item(q["h"])
            if path == "/api/raw" and G:
                r = app.resolve(q["h"])
                p = app.ws.abspath(r["work"])
                ctype = mimetypes.guess_type(p)[0] or "application/octet-stream"
                fn = os.path.basename(r.get("rel") or r["work"])
                if r["kind"] == "ean":
                    fn = os.path.basename(r["work"])
                extra = {"Content-Disposition": 'attachment; filename="%s"' % fn} if q.get("dl") else None
                return (ctype, open(p, "rb").read(), extra)
            if path == "/api/replace" and not G:
                return app.replace(q["h"], self._body())
            if path == "/api/revert" and not G:
                return app.revert(q["h"])
            if path == "/api/text":
                return app.get_text(q["h"]) if G else app.put_text(q["h"], self._json())
            if path == "/api/table":
                return app.get_table(q["h"], q.get("tag")) if G else app.put_table(q["h"], self._json())
            if path == "/api/atlas" and G:
                return app.atlas(q["xml"])
            if path == "/api/font" and G:
                return app.font(q["h"])
            if path == "/api/sprite":
                if G:
                    p = app.ws.abspath(app.sprite_path(q["xml"], q["name"]))
                    extra = {"Content-Disposition": 'attachment; filename="%s"' % os.path.basename(p)} if q.get("dl") else None
                    return ("image/png", open(p, "rb").read(), extra)
                return app.put_sprite(q["xml"], q.get("name", ""), self._body(), new=q.get("new") == "1")
            if path == "/api/sprite/revert" and not G:
                return dict(ok=True, report=app.ws.revert_sprite(q["xml"], q["name"]))
            if path == "/api/atlas/repack" and not G:
                logs = []
                rep = app.ws.sync_atlas(q["xml"], force_repack=True, log=logs.append)
                return dict(ok=True, report=rep, log=logs)
            if path == "/api/build" and not G:
                b = self._json()
                return app.build(install=bool(b.get("install")), force_all=bool(b.get("all")))
            if path == "/api/restore" and not G:
                return app.restore()
            if path == "/api/launch" and not G:
                return app.launch()
            if path == "/api/analyze" and G:
                from .analyze import format_text
                view = GameView(app.ws.game_dir) if q.get("target") == "game" else WorkspaceView(app.ws)
                rep = analyze(view, key=app.ws.key)
                rep["text"] = format_text(rep)
                return rep
            if path == "/api/classes" and G:
                return app.classes()
            if path == "/api/disasm" and G:
                return app.disasm(int(q.get("abc", 0)), int(q["m"]))
            if path == "/api/strings" and G:
                return app.search_strings(q.get("q", ""))
            raise ApiError("no such endpoint %s %s" % (method, path))

    return H


class _Server(ThreadingHTTPServer):
    # SO_REUSEADDR on Windows lets two processes share a port; claim it exclusively instead
    allow_reuse_address = False
    daemon_threads = True

    def server_bind(self):
        if hasattr(socket, "SO_EXCLUSIVEADDRUSE"):
            self.socket.setsockopt(socket.SOL_SOCKET, socket.SO_EXCLUSIVEADDRUSE, 1)
        super().server_bind()


def serve(ws, port=8765, open_browser=True):
    app = App(ws)
    srv = None
    for p in range(port, port + 20):
        try:
            srv = _Server(("127.0.0.1", p), make_handler(app))
            port = p
            break
        except OSError:
            continue
    if srv is None:
        raise RuntimeError("no free port found")
    url = "http://127.0.0.1:%d/" % port
    print("RRModKit editor running at %s  (Ctrl+C to stop)" % url, flush=True)
    if open_browser:
        threading.Timer(0.6, lambda: webbrowser.open(url)).start()
    try:
        srv.serve_forever()
    except KeyboardInterrupt:
        print("stopped")
    return srv
