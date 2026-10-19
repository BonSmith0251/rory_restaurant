"""SWF container + AVM2 (ABC) support: embedded asset extraction/replacement,
class listing, disassembly and automatic detection of the .ean RC4 key."""
import re
import struct
import zlib

from . import DEFAULT_KEY
from .formats import (Image, argb_to_rgba, rgba_to_argb, premultiply_argb, unpremultiply_argb,
                      read_png, write_png, sniff, jpeg_info)

TAG_NAMES = {0: "End", 1: "ShowFrame", 2: "DefineShape", 6: "DefineBits", 8: "JPEGTables", 9: "SetBackgroundColor",
             11: "DefineText", 14: "DefineSound", 20: "DefineBitsLossless", 21: "DefineBitsJPEG2", 22: "DefineShape2",
             26: "PlaceObject2", 32: "DefineShape3", 35: "DefineBitsJPEG3", 36: "DefineBitsLossless2",
             39: "DefineSprite", 41: "ProductInfo", 43: "FrameLabel", 56: "ExportAssets", 65: "ScriptLimits",
             69: "FileAttributes", 72: "DoABC", 73: "DefineFontAlignZones", 74: "CSMTextSettings",
             75: "DefineFont3", 76: "SymbolClass", 77: "Metadata", 82: "DoABC2", 83: "DefineShape4",
             86: "DefineSceneAndFrameLabelData", 87: "DefineBinaryData", 88: "DefineFontName", 90: "DefineBitsJPEG4",
             91: "DefineFont4"}


class _R:
    def __init__(self, b, p=0):
        self.b, self.p = b, p

    def u8(self):
        v = self.b[self.p]
        self.p += 1
        return v

    def u16(self):
        v = struct.unpack_from("<H", self.b, self.p)[0]
        self.p += 2
        return v

    def u32(self):
        v = struct.unpack_from("<I", self.b, self.p)[0]
        self.p += 4
        return v

    def d(self):
        v = struct.unpack_from("<d", self.b, self.p)[0]
        self.p += 8
        return v

    def u30(self):
        r = sh = 0
        for _ in range(5):
            b = self.u8()
            r |= (b & 0x7F) << sh
            sh += 7
            if not b & 0x80:
                break
        return r & 0xFFFFFFFF

    def s32(self):
        r = sh = 0
        for _ in range(5):
            b = self.u8()
            r |= (b & 0x7F) << sh
            sh += 7
            if not b & 0x80:
                break
        # AVM2 s32 is a variable-length 32-bit value; only bit 31 carries the sign
        r &= 0xFFFFFFFF
        return r - (1 << 32) if r & 0x80000000 else r


# --------------------------------------------------------------------------- container

class Swf:
    def __init__(self, data):
        self.sig = data[:3]
        self.version = data[3]
        if self.sig == b"CWS":
            body = zlib.decompress(data[8:])
        elif self.sig == b"FWS":
            body = data[8:]
        elif self.sig == b"ZWS":
            import lzma
            props = data[12:17]
            body = lzma.LZMADecompressor(lzma.FORMAT_RAW, filters=[lzma._decode_filter_properties(lzma.FILTER_LZMA1, props)]).decompress(data[17:])
        else:
            raise ValueError("not a SWF file")
        nbits = body[0] >> 3
        hlen = (5 + nbits * 4 + 7) // 8 + 4
        self.header = body[:hlen]  # frame rect + frame rate + frame count
        self.tags = []  # [code, body(bytes), long_form]
        p = hlen
        while p < len(body):
            h = struct.unpack_from("<H", body, p)[0]
            p += 2
            code, ln = h >> 6, h & 0x3F
            long_form = ln == 0x3F
            if long_form:
                ln = struct.unpack_from("<I", body, p)[0]
                p += 4
            self.tags.append([code, bytes(body[p:p + ln]), long_form])
            p += ln
            if code == 0:
                break

    @classmethod
    def load(cls, path):
        with open(path, "rb") as f:
            return cls(f.read())

    def to_bytes(self, compress=True):
        out = bytearray(self.header)
        for code, body, long_form in self.tags:
            if long_form or len(body) >= 0x3F:
                out += struct.pack("<HI", (code << 6) | 0x3F, len(body))
            else:
                out += struct.pack("<H", (code << 6) | len(body))
            out += body
        total = 8 + len(out)
        sig = b"CWS" if compress else b"FWS"
        payload = zlib.compress(bytes(out), 9) if compress else bytes(out)
        return sig + bytes((self.version,)) + struct.pack("<I", total) + payload

    def frame_info(self):
        nbits = self.header[0] >> 3
        bits = int.from_bytes(self.header[:(5 + nbits * 4 + 7) // 8], "big")
        total = ((5 + nbits * 4 + 7) // 8) * 8
        vals = []
        for i in range(4):
            sh = total - 5 - nbits * (i + 1)
            v = (bits >> sh) & ((1 << nbits) - 1)
            if v & (1 << (nbits - 1)):
                v -= 1 << nbits
            vals.append(v / 20)
        rate = struct.unpack_from("<H", self.header, len(self.header) - 4)[0] / 256
        count = struct.unpack_from("<H", self.header, len(self.header) - 2)[0]
        return dict(width=vals[1] - vals[0], height=vals[3] - vals[2], fps=rate, frames=count)

    def symbols(self):
        out = {}
        for code, body, _ in self.tags:
            if code in (76, 56):
                r = _R(body)
                for _ in range(r.u16()):
                    cid = r.u16()
                    e = body.index(b"\0", r.p)
                    out.setdefault(cid, body[r.p:e].decode("utf-8", "replace"))
                    r.p = e + 1
        return out

    def abc_blocks(self):
        out = []
        for code, body, _ in self.tags:
            if code == 82:
                e = body.index(b"\0", 4)
                out.append(body[e + 1:])
            elif code == 72:
                out.append(body)
        return out

    def tag_stats(self):
        st = {}
        for code, body, _ in self.tags:
            n = TAG_NAMES.get(code, "Tag%d" % code)
            c, s = st.get(n, (0, 0))
            st[n] = (c + 1, s + len(body))
        return st

    # ----------------------------------------------------------------- assets

    def _find(self, cid, codes):
        for i, (code, body, _) in enumerate(self.tags):
            if code in codes and len(body) >= 2 and struct.unpack_from("<H", body)[0] == cid:
                return i
        return None

    def assets(self):
        """List editable embedded assets: binary data, bitmaps, JPEGs."""
        syms = self.symbols()
        out = []
        for i, (code, body, _) in enumerate(self.tags):
            if code not in (87, 20, 36, 21, 35, 90) or len(body) < 2:
                continue
            cid = struct.unpack_from("<H", body)[0]
            a = dict(id=cid, tag=code, tag_name=TAG_NAMES[code], symbol=syms.get(cid, ""), size=len(body))
            if code == 87:
                a["ext"] = sniff(body[6:])
                if a["ext"] == "xml" and is_atlas(body[6:]):
                    a["ext"] = "xml"
            elif code in (20, 36):
                a["ext"] = "png"
                a["format"], a["w"], a["h"] = body[2], struct.unpack_from("<H", body, 3)[0], struct.unpack_from("<H", body, 5)[0]
            else:
                img = _jpeg_payload(code, body)[0]
                a["ext"] = sniff(img) if sniff(img) in ("png", "gif") else "jpg"
                info = jpeg_info(img)
                if info:
                    a["w"], a["h"] = info["w"], info["h"]
                a["has_alpha"] = code in (35, 90) and bool(_jpeg_payload(code, body)[1])
            out.append(a)
        return out

    def export_asset(self, cid):
        """Return (ext, bytes) suitable for writing to disk."""
        i = self._find(cid, (87, 20, 36, 21, 35, 90))
        if i is None:
            raise KeyError(cid)
        code, body, _ = self.tags[i]
        if code == 87:
            return sniff(body[6:]), body[6:]
        if code in (20, 36):
            return "png", write_png(decode_lossless(code, body))
        img, alpha = _jpeg_payload(code, body)
        k = sniff(img)
        return (k if k in ("png", "gif") else "jpg"), img

    def replace_asset(self, cid, data):
        """Replace an embedded asset with new file bytes. Returns list of warnings."""
        warn = []
        i = self._find(cid, (87, 20, 36, 21, 35, 90))
        if i is None:
            raise KeyError(cid)
        code, body, long_form = self.tags[i]
        kind = sniff(data)
        if code == 87:
            self.tags[i] = [87, body[:6] + bytes(data), long_form]
        elif code in (20, 36):
            if kind != "png":
                raise ValueError("bitmap %d must be replaced with a PNG" % cid)
            self.tags[i] = [36, encode_lossless2(cid, read_png(data)), True]
        else:
            if kind == "png":
                self.tags[i] = [36, encode_lossless2(cid, read_png(data)), True]
                warn.append("JPEG character %d converted to lossless PNG bitmap" % cid)
            elif kind == "jpg":
                old, alpha = _jpeg_payload(code, body)
                oi, ni = jpeg_info(old), jpeg_info(data)
                if code in (35, 90) and alpha and oi and ni and (oi["w"], oi["h"]) == (ni["w"], ni["h"]):
                    self.tags[i] = [35, struct.pack("<HI", cid, len(data)) + bytes(data) + alpha, True]
                else:
                    if code in (35, 90) and alpha:
                        warn.append("character %d: new JPEG size differs, alpha channel dropped" % cid)
                    self.tags[i] = [21, struct.pack("<H", cid) + bytes(data), True]
            else:
                raise ValueError("unsupported replacement format for image %d" % cid)
        return warn


def is_atlas(data):
    return b"<TextureAtlas" in data[:600]


def _jpeg_payload(code, body):
    if code == 21:
        img, alpha = body[2:], b""
    elif code == 35:
        off = struct.unpack_from("<I", body, 2)[0]
        img, alpha = body[6:6 + off], body[6 + off:]
    else:  # 90
        off = struct.unpack_from("<I", body, 2)[0]
        img, alpha = body[8:8 + off], body[8 + off:]
    if img[:4] == b"\xff\xd9\xff\xd8":  # legacy erroneous header
        img = img[4:]
    return img, alpha


def decode_lossless(code, body):
    r = _R(body, 2)
    fmt = r.u8()
    w, h = r.u16(), r.u16()
    alpha = code == 36
    if fmt == 3:
        ncol = r.u8() + 1
        raw = zlib.decompress(body[r.p:])
        csz = 4 if alpha else 3
        table = raw[:ncol * csz]
        pix = raw[ncol * csz:]
        stride = (w + 3) & ~3
        img = Image(w, h)
        for y in range(h):
            for x in range(w):
                c = pix[y * stride + x]
                if alpha:
                    rgba = bytes(table[c * 4:c * 4 + 4])  # RGBA premultiplied
                    a = rgba[3]
                    if a and a != 255:
                        rgba = bytes(min(255, v * 255 // a) for v in rgba[:3]) + bytes((a,))
                else:
                    rgba = bytes(table[c * 3:c * 3 + 3]) + b"\xff"
                img.px[(y * w + x) * 4:(y * w + x) * 4 + 4] = rgba
        return img
    if fmt == 4:
        raw = zlib.decompress(body[r.p:])
        stride = ((w * 2) + 3) & ~3
        img = Image(w, h)
        for y in range(h):
            for x in range(w):
                v = struct.unpack_from(">H", raw, y * stride + x * 2)[0]
                img.px[(y * w + x) * 4:(y * w + x) * 4 + 4] = bytes(
                    (((v >> 10) & 31) * 255 // 31, ((v >> 5) & 31) * 255 // 31, (v & 31) * 255 // 31, 255))
        return img
    if fmt == 5:
        raw = zlib.decompress(body[r.p:])[:w * h * 4]
        if alpha:
            raw = unpremultiply_argb(raw)
        else:
            raw = bytearray(raw)
            raw[0::4] = b"\xff" * (w * h)
        return Image(w, h, argb_to_rgba(raw))
    raise ValueError("unknown lossless bitmap format %d" % fmt)


def encode_lossless2(cid, img):
    argb = premultiply_argb(rgba_to_argb(img.px))
    return struct.pack("<HBHH", cid, 5, img.w, img.h) + zlib.compress(bytes(argb), 9)


# --------------------------------------------------------------------------- ABC

class Abc:
    def __init__(self, abc):
        self.data = abc
        r = _R(abc, 4)
        n = r.u30()
        self.ints = [0] + [r.s32() for _ in range(max(0, n - 1))]
        n = r.u30()
        self.uints = [0] + [r.u30() for _ in range(max(0, n - 1))]
        n = r.u30()
        self.dbls = [0.0] + [r.d() for _ in range(max(0, n - 1))]
        n = r.u30()
        self.strs = [None]
        for _ in range(max(0, n - 1)):
            ln = r.u30()
            self.strs.append(abc[r.p:r.p + ln].decode("utf-8", "replace"))
            r.p += ln
        n = r.u30()
        self.nss = [None]
        for _ in range(max(0, n - 1)):
            k = r.u8()
            self.nss.append((k, self.strs[r.u30()]))
        n = r.u30()
        for _ in range(max(0, n - 1)):
            for _ in range(r.u30()):
                r.u30()
        n = r.u30()
        self.mns = [None]
        self.mn_kind = [None]
        for _ in range(max(0, n - 1)):
            k = r.u8()
            self.mn_kind.append(k)
            if k in (0x07, 0x0D):
                ns = r.u30()
                nm = r.u30()
                nsn = self.nss[ns][1] if ns and self.nss[ns] else ""
                self.mns.append((nsn, self.strs[nm]))
            elif k in (0x0F, 0x10):
                self.mns.append(("", self.strs[r.u30()]))
            elif k in (0x11, 0x12):
                self.mns.append(("", "<rtqnameL>"))
            elif k in (0x09, 0x0E):
                nm = r.u30()
                r.u30()
                self.mns.append(("", self.strs[nm]))
            elif k in (0x1B, 0x1C):
                r.u30()
                self.mns.append(("", "<multinameL>"))
            elif k == 0x1D:
                base = r.u30()
                params = [r.u30() for _ in range(r.u30())]
                self.mns.append(("", "%s.<%s>" % (self.mn(base), ",".join(self.mn(x) for x in params))))
            else:
                raise ValueError("unknown multiname kind 0x%x" % k)
        n = r.u30()
        self.methods = []
        for _ in range(n):
            pc = r.u30()
            ret = r.u30()
            ptypes = [r.u30() for _ in range(pc)]
            name = r.u30()
            fl = r.u8()
            opts = []
            if fl & 0x08:
                for _ in range(r.u30()):
                    vi = r.u30()
                    vk = r.u8()
                    opts.append(self.const_value(vk, vi))
            pnames = None
            if fl & 0x80:
                pnames = [self.strs[r.u30()] for _ in range(pc)]
            self.methods.append(dict(name=self.strs[name], qual=None, params=pc, ret=self.mn(ret) if ret else "*",
                                     ptypes=[self.mn(t) if t else "*" for t in ptypes], pnames=pnames,
                                     optional=opts, flags=fl))
        for _ in range(r.u30()):
            r.u30()
            for _ in range(r.u30()):
                r.u30()
                r.u30()

        def traits(full=None):
            out = []
            for _ in range(r.u30()):
                nm = r.u30()
                kind = r.u8()
                t = kind & 0x0F
                info = dict(name=self.mn(nm), kind=t, final=bool(kind & 0x10), override=bool(kind & 0x20))
                if t in (0, 6):
                    info["slot_id"] = r.u30()
                    tn = r.u30()
                    info["type"] = self.mn(tn) if tn else "*"
                    vi = r.u30()
                    info["value"] = self.const_value(r.u8(), vi) if vi else None
                    out.append(("var" if t == 0 else "const", self.mn(nm), None))
                elif t in (1, 2, 3):
                    r.u30()
                    m = r.u30()
                    info["method"] = m
                    out.append(({1: "method", 2: "getter", 3: "setter"}[t], self.mn(nm), m))
                elif t == 4:
                    info["slot_id"] = r.u30()
                    r.u30()
                    out.append(("class", self.mn(nm), None))
                elif t == 5:
                    info["slot_id"] = r.u30()
                    m = r.u30()
                    info["method"] = m
                    out.append(("function", self.mn(nm), m))
                if kind & 0x40:
                    for _ in range(r.u30()):
                        r.u30()
                if full is not None:
                    full.append(info)
            return out

        nc = r.u30()
        self.classes = []
        for _ in range(nc):
            nm = r.u30()
            sup = r.u30()
            fl = r.u8()
            if fl & 0x08:
                r.u30()
            ifaces = [r.u30() for _ in range(r.u30())]
            iinit = r.u30()
            full = []
            self.classes.append(dict(name=self.mn(nm), fullname=self.mn(nm, True), super=self.mn(sup) if sup else "",
                                     interface=bool(fl & 0x04), iinit=iinit, itraits=traits(full),
                                     interfaces=[self.mn(x) for x in ifaces], itraits_full=full))
        for c in self.classes:
            c["cinit"] = r.u30()
            full = []
            c["ctraits"] = traits(full)
            c["ctraits_full"] = full
        self.scripts = []
        for _ in range(r.u30()):
            init = r.u30()
            full = []
            traits(full)
            self.scripts.append(dict(init=init, traits=full))
        self.bodies = {}
        self.body_info = {}
        for _ in range(r.u30()):
            m = r.u30()
            r.u30()
            local_count = r.u30()
            r.u30()
            r.u30()
            cl = r.u30()
            self.bodies[m] = abc[r.p:r.p + cl]
            r.p += cl
            exc = []
            for _ in range(r.u30()):
                f, t, tg, et, vn = (r.u30() for _ in range(5))
                exc.append(dict(start=f, end=t, target=tg, type=self.mn(et) if et else "*", var=self.mn(vn) if vn else "e"))
            full = []
            traits(full)
            self.body_info[m] = dict(local_count=local_count, exceptions=exc, traits=full)
        for c in self.classes:
            cn = c["name"]
            self._q(c["iinit"], cn + ".<init>")
            self._q(c["cinit"], cn + ".<cinit>")
            for kind, nm, m in c["itraits"]:
                if m is not None:
                    self._q(m, "%s.%s%s" % (cn, {"getter": "get ", "setter": "set "}.get(kind, ""), nm))
            for kind, nm, m in c["ctraits"]:
                if m is not None:
                    self._q(m, "%s.static %s" % (cn, nm))

    def const_value(self, kind, idx):
        try:
            if kind == 0x03:
                return self.ints[idx]
            if kind == 0x04:
                return self.uints[idx]
            if kind == 0x06:
                return self.dbls[idx]
            if kind == 0x01:
                return self.strs[idx]
            if kind == 0x0B:
                return True
            if kind == 0x0A:
                return False
            if kind == 0x0C:
                return "<null>"
            if kind == 0x00:
                return "<undefined>"
            return "<ns>"
        except IndexError:
            return "<?>"

    def _q(self, m, q):
        if 0 <= m < len(self.methods) and not self.methods[m]["qual"]:
            self.methods[m]["qual"] = q

    def mn(self, i, full=False):
        if not i or i >= len(self.mns):
            return "*"
        v = self.mns[i]
        if self.mn_kind[i] in (0x0D, 0x10, 0x12, 0x0E, 0x1C):  # attribute multinames (E4X @name)
            return "@" + (v[1] or "*")
        if full and v[0]:
            return "%s.%s" % (v[0], v[1])
        return v[1] or "*"

    def mn_runtime(self, i):
        """Extra stack operands a multiname needs at runtime (name and/or namespace)."""
        k = self.mn_kind[i] if i and i < len(self.mn_kind) else None
        if k in (0x1B, 0x1C, 0x0F, 0x10):
            return 1
        if k in (0x11, 0x12):
            return 2
        return 0

    def find_methods(self, query):
        q = query.lower()
        return [(i, m) for i, m in enumerate(self.methods) if m["qual"] and q in m["qual"].lower() and i in self.bodies]

    def disassemble(self, method_index):
        return disassemble(self, self.bodies[method_index])

    def pushed_strings(self, method_index):
        return [a for op, a in iter_ops(self, self.bodies[method_index]) if op == "pushstring"]


_OPS = {0x02: ('nop', ''), 0x03: ('throw', ''), 0x04: ('getsuper', 'm'), 0x05: ('setsuper', 'm'), 0x06: ('dxns', 'u'),
        0x07: ('dxnslate', ''), 0x08: ('kill', 'u'), 0x09: ('label', ''), 0x0c: ('ifnlt', 'o'), 0x0d: ('ifnle', 'o'),
        0x0e: ('ifngt', 'o'), 0x0f: ('ifnge', 'o'), 0x10: ('jump', 'o'), 0x11: ('iftrue', 'o'), 0x12: ('iffalse', 'o'),
        0x13: ('ifeq', 'o'), 0x14: ('ifne', 'o'), 0x15: ('iflt', 'o'), 0x16: ('ifle', 'o'), 0x17: ('ifgt', 'o'),
        0x18: ('ifge', 'o'), 0x19: ('ifstricteq', 'o'), 0x1a: ('ifstrictne', 'o'), 0x1b: ('lookupswitch', 'L'),
        0x1c: ('pushwith', ''), 0x1d: ('popscope', ''), 0x1e: ('nextname', ''), 0x1f: ('hasnext', ''),
        0x20: ('pushnull', ''), 0x21: ('pushundefined', ''), 0x23: ('nextvalue', ''), 0x24: ('pushbyte', 'b'),
        0x25: ('pushshort', 'u'), 0x26: ('pushtrue', ''), 0x27: ('pushfalse', ''), 0x28: ('pushnan', ''),
        0x29: ('pop', ''), 0x2a: ('dup', ''), 0x2b: ('swap', ''), 0x2c: ('pushstring', 's'), 0x2d: ('pushint', 'i'),
        0x2e: ('pushuint', 'U'), 0x2f: ('pushdouble', 'd'), 0x30: ('pushscope', ''), 0x31: ('pushnamespace', 'u'),
        0x32: ('hasnext2', 'uu'), 0x35: ('li8', ''), 0x36: ('li16', ''), 0x37: ('li32', ''), 0x38: ('lf32', ''),
        0x39: ('lf64', ''), 0x3a: ('si8', ''), 0x3b: ('si16', ''), 0x3c: ('si32', ''), 0x3d: ('sf32', ''),
        0x3e: ('sf64', ''), 0x40: ('newfunction', 'u'), 0x41: ('call', 'u'), 0x42: ('construct', 'u'),
        0x43: ('callmethod', 'uu'), 0x44: ('callstatic', 'uu'), 0x45: ('callsuper', 'mu'), 0x46: ('callproperty', 'mu'),
        0x47: ('returnvoid', ''), 0x48: ('returnvalue', ''), 0x49: ('constructsuper', 'u'), 0x4a: ('constructprop', 'mu'),
        0x4c: ('callproplex', 'mu'), 0x4e: ('callsupervoid', 'mu'), 0x4f: ('callpropvoid', 'mu'), 0x50: ('sxi1', ''),
        0x51: ('sxi8', ''), 0x52: ('sxi16', ''), 0x53: ('applytype', 'u'), 0x55: ('newobject', 'u'),
        0x56: ('newarray', 'u'), 0x57: ('newactivation', ''), 0x58: ('newclass', 'u'), 0x59: ('getdescendants', 'm'),
        0x5a: ('newcatch', 'u'), 0x5d: ('findpropstrict', 'm'), 0x5e: ('findproperty', 'm'), 0x5f: ('finddef', 'm'),
        0x60: ('getlex', 'm'), 0x61: ('setproperty', 'm'), 0x62: ('getlocal', 'u'), 0x63: ('setlocal', 'u'),
        0x64: ('getglobalscope', ''), 0x65: ('getscopeobject', 'u'), 0x66: ('getproperty', 'm'),
        0x68: ('initproperty', 'm'), 0x6a: ('deleteproperty', 'm'), 0x6c: ('getslot', 'u'), 0x6d: ('setslot', 'u'),
        0x6e: ('getglobalslot', 'u'), 0x6f: ('setglobalslot', 'u'), 0x70: ('convert_s', ''), 0x71: ('esc_xelem', ''),
        0x72: ('esc_xattr', ''), 0x73: ('convert_i', ''), 0x74: ('convert_u', ''), 0x75: ('convert_d', ''),
        0x76: ('convert_b', ''), 0x77: ('convert_o', ''), 0x78: ('checkfilter', ''), 0x80: ('coerce', 'm'),
        0x82: ('coerce_a', ''), 0x85: ('coerce_s', ''), 0x86: ('astype', 'm'), 0x87: ('astypelate', ''),
        0x90: ('negate', ''), 0x91: ('increment', ''), 0x92: ('inclocal', 'u'), 0x93: ('decrement', ''),
        0x94: ('declocal', 'u'), 0x95: ('typeof', ''), 0x96: ('not', ''), 0x97: ('bitnot', ''), 0xa0: ('add', ''),
        0xa1: ('subtract', ''), 0xa2: ('multiply', ''), 0xa3: ('divide', ''), 0xa4: ('modulo', ''),
        0xa5: ('lshift', ''), 0xa6: ('rshift', ''), 0xa7: ('urshift', ''), 0xa8: ('bitand', ''), 0xa9: ('bitor', ''),
        0xaa: ('bitxor', ''), 0xab: ('equals', ''), 0xac: ('strictequals', ''), 0xad: ('lessthan', ''),
        0xae: ('lessequals', ''), 0xaf: ('greaterthan', ''), 0xb0: ('greaterequals', ''), 0xb1: ('instanceof', ''),
        0xb2: ('istype', 'm'), 0xb3: ('istypelate', ''), 0xb4: ('in', ''), 0xc0: ('increment_i', ''),
        0xc1: ('decrement_i', ''), 0xc2: ('inclocal_i', 'u'), 0xc3: ('declocal_i', 'u'), 0xc4: ('negate_i', ''),
        0xc5: ('add_i', ''), 0xc6: ('subtract_i', ''), 0xc7: ('multiply_i', ''), 0xd0: ('getlocal0', ''),
        0xd1: ('getlocal1', ''), 0xd2: ('getlocal2', ''), 0xd3: ('getlocal3', ''), 0xd4: ('setlocal0', ''),
        0xd5: ('setlocal1', ''), 0xd6: ('setlocal2', ''), 0xd7: ('setlocal3', ''), 0xef: ('debug', 'bubu'),
        0xf0: ('debugline', 'u'), 0xf1: ('debugfile', 's')}


def iter_ops(abc, code):
    """Yield (opname, arg-or-args, offset)-like tuples (op, args)."""
    for off, name, args in _decode(abc, code):
        yield name, (args[0] if len(args) == 1 else args)


def _decode(abc, code):
    r = _R(code)
    while r.p < len(code):
        at = r.p
        op = r.u8()
        name, fmt = _OPS.get(op, ("op_%02x" % op, ""))
        args = []
        for f in fmt:
            if f == "u":
                args.append(r.u30())
            elif f == "b":
                v = code[r.p]
                r.p += 1
                args.append(v - 256 if v > 127 else v)
            elif f == "s":
                args.append(abc.strs[r.u30()])
            elif f == "i":
                args.append(abc.ints[r.u30()])
            elif f == "U":
                args.append(abc.uints[r.u30()])
            elif f == "d":
                args.append(abc.dbls[r.u30()])
            elif f == "m":
                args.append(abc.mn(r.u30()))
            elif f == "o":
                o = code[r.p] | code[r.p + 1] << 8 | code[r.p + 2] << 16
                r.p += 3
                if o & 0x800000:
                    o -= 1 << 24
                args.append("L%d" % (r.p + o))
            elif f == "L":
                base = at
                d = code[r.p] | code[r.p + 1] << 8 | code[r.p + 2] << 16
                r.p += 3
                c = r.u30()
                targets = []
                for _ in range(c + 1):
                    o = code[r.p] | code[r.p + 1] << 8 | code[r.p + 2] << 16
                    r.p += 3
                    if o & 0x800000:
                        o -= 1 << 24
                    targets.append("L%d" % (base + o))
                if d & 0x800000:
                    d -= 1 << 24
                args.append("default:L%d [%s]" % (base + d, ", ".join(targets)))
        yield at, name, args


def disassemble(abc, code):
    lines = []
    for at, name, args in _decode(abc, code):
        a = " ".join(repr(x) if isinstance(x, str) and name in ("pushstring", "debugfile") else str(x) for x in args)
        lines.append("L%-6d %-16s %s" % (at, name, a))
    return "\n".join(lines)


def load_abc(swf):
    blocks = swf.abc_blocks()
    return [Abc(b) for b in blocks]


def detect_ean_key(swf):
    """Find the RC4 key used by TGlobalClass.readByteArrayToBitmap. Falls back to the default."""
    try:
        for abc in load_abc(swf):
            for mi, m in abc.find_methods("readByteArrayToBitmap"):
                s = abc.pushed_strings(mi)
                if s:
                    return s[0], "detected in %s" % m["qual"]
    except Exception as e:  # pragma: no cover
        return DEFAULT_KEY, "detection failed (%s), using default" % e
    return DEFAULT_KEY, "not found, using default"


ASSET_REF_RE = re.compile(r"\.(ean|eag|xml|mp3|png|jpg|fnt|pex|swf|json|txt)$", re.I)


def asset_references(swf):
    """Strings in the code that look like asset paths/names."""
    out = set()
    for abc in load_abc(swf):
        for s in abc.strs:
            if s and len(s) < 200 and (ASSET_REF_RE.search(s) or s.startswith("data/")):
                out.add(s)
    return sorted(out)


def all_strings(swf):
    out = set()
    for abc in load_abc(swf):
        out.update(s for s in abc.strs if s)
    return out
