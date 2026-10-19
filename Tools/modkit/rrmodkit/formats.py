"""Binary formats used by the game, implemented with the standard library only.

.ean  RC4(key) -> zlib -> [u32 BE width][width*height ARGB pixels, BE, straight alpha]
      (decoded by TGlobalClass.readByteArrayToBitmap -> BitmapData.setPixels)
.eag  plain JPEG (loaded with flash.display.Loader, so PNG/GIF bytes also work)
"""
import hashlib
import struct
import zlib
import xml.parsers.expat
from xml.sax.saxutils import quoteattr

from . import DEFAULT_KEY

try:  # optional speed-ups
    import numpy as _np
except Exception:  # pragma: no cover
    _np = None


def sha1(data):
    return hashlib.sha1(data).hexdigest()


def sha1_file(path):
    h = hashlib.sha1()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(1 << 20), b""):
            h.update(chunk)
    return h.hexdigest()


# --------------------------------------------------------------------------- RC4

def rc4(key, data):
    if isinstance(key, str):
        key = key.encode("utf-8")
    S = list(range(256))
    j = 0
    klen = len(key)
    for i in range(256):
        j = (j + S[i] + key[i % klen]) & 255
        S[i], S[j] = S[j], S[i]
    n = len(data)
    ks = bytearray(n)
    i = j = 0
    for k in range(n):
        i = (i + 1) & 255
        si = S[i]
        j = (j + si) & 255
        sj = S[j]
        S[i] = sj
        S[j] = si
        ks[k] = S[(si + sj) & 255]
    # xor the whole buffer at once using big ints
    x = int.from_bytes(data, "little") ^ int.from_bytes(ks, "little")
    return x.to_bytes(n, "little")


# --------------------------------------------------------------------------- Image

class Image:
    """Simple RGBA8 image (straight alpha)."""

    __slots__ = ("w", "h", "px")

    def __init__(self, w, h, px=None):
        self.w, self.h = w, h
        self.px = bytearray(w * h * 4) if px is None else bytearray(px)
        if len(self.px) != w * h * 4:
            raise ValueError("pixel buffer size mismatch (%d != %dx%dx4)" % (len(self.px), w, h))

    def crop(self, x, y, w, h):
        out = Image(w, h)
        stride, ostride = self.w * 4, w * 4
        for row in range(h):
            sy = y + row
            if 0 <= sy < self.h:
                x0, x1 = max(0, x), min(self.w, x + w)
                if x1 > x0:
                    s = sy * stride + x0 * 4
                    d = row * ostride + (x0 - x) * 4
                    out.px[d:d + (x1 - x0) * 4] = self.px[s:s + (x1 - x0) * 4]
        return out

    def paste(self, img, x, y):
        """Replace pixels (no blending) - exactly what an atlas region needs."""
        stride, istride = self.w * 4, img.w * 4
        for row in range(img.h):
            dy = y + row
            if not 0 <= dy < self.h:
                continue
            x0, x1 = max(0, x), min(self.w, x + img.w)
            if x1 > x0:
                s = row * istride + (x0 - x) * 4
                d = dy * stride + x0 * 4
                self.px[d:d + (x1 - x0) * 4] = img.px[s:s + (x1 - x0) * 4]

    def fill(self, x, y, w, h, rgba=b"\0\0\0\0"):
        self.paste(Image(w, h, bytes(rgba) * (w * h)), x, y)

    def alpha_stats(self):
        a = self.px[3::4]
        n = len(a)
        if not n:
            return dict(opaque=0, transparent=0, partial=0)
        t = a.count(0)
        o = a.count(255)
        return dict(opaque=o / n, transparent=t / n, partial=(n - t - o) / n)

    def bbox(self):
        """Bounding box of non-transparent pixels or None."""
        a = self.px[3::4]
        w = self.w
        rows = [r for r in range(self.h) if any(a[r * w:(r + 1) * w])]
        if not rows:
            return None
        cols_any = bytearray(w)
        for r in rows:
            line = a[r * w:(r + 1) * w]
            for c in range(w):
                if line[c]:
                    cols_any[c] = 1
        cols = [c for c in range(w) if cols_any[c]]
        return cols[0], rows[0], cols[-1] - cols[0] + 1, rows[-1] - rows[0] + 1


def argb_to_rgba(src):
    out = bytearray(len(src))
    out[0::4] = src[1::4]
    out[1::4] = src[2::4]
    out[2::4] = src[3::4]
    out[3::4] = src[0::4]
    return out


def rgba_to_argb(src):
    out = bytearray(len(src))
    out[0::4] = src[3::4]
    out[1::4] = src[0::4]
    out[2::4] = src[1::4]
    out[3::4] = src[2::4]
    return out


def premultiply_argb(argb):
    """Straight ARGB -> premultiplied ARGB (SWF DefineBitsLossless2)."""
    if _np is not None:
        a = _np.frombuffer(bytes(argb), _np.uint8).reshape(-1, 4).astype(_np.uint32)
        al = a[:, 0:1]
        a[:, 1:] = (a[:, 1:] * al + 127) // 255
        return bytearray(a.astype(_np.uint8).tobytes())
    out = bytearray(argb)
    for i in range(0, len(out), 4):
        al = out[i]
        if al != 255:
            out[i + 1] = (out[i + 1] * al + 127) // 255
            out[i + 2] = (out[i + 2] * al + 127) // 255
            out[i + 3] = (out[i + 3] * al + 127) // 255
    return out


def unpremultiply_argb(argb):
    if _np is not None:
        a = _np.frombuffer(bytes(argb), _np.uint8).reshape(-1, 4).astype(_np.uint32)
        al = a[:, 0:1]
        safe = _np.where(al == 0, 1, al)
        c = (a[:, 1:] * 255 + safe // 2) // safe
        a[:, 1:] = _np.where(al == 0, 0, _np.minimum(c, 255))
        return bytearray(a.astype(_np.uint8).tobytes())
    out = bytearray(argb)
    for i in range(0, len(out), 4):
        al = out[i]
        if al == 0:
            out[i + 1] = out[i + 2] = out[i + 3] = 0
        elif al != 255:
            for k in (1, 2, 3):
                out[i + k] = min(255, (out[i + k] * 255 + al // 2) // al)
    return out


# --------------------------------------------------------------------------- EAN

class EanError(Exception):
    pass


def decode_ean(data, key=DEFAULT_KEY):
    """Return Image from .ean bytes."""
    try:
        raw = zlib.decompress(rc4(key, data))
    except zlib.error as e:
        raise EanError("zlib failed after RC4 - wrong key or not an .ean file (%s)" % e)
    if len(raw) < 4:
        raise EanError("payload too small")
    w = struct.unpack(">I", raw[:4])[0]
    npx = (len(raw) - 4) // 4
    if w == 0 or npx % w:
        raise EanError("pixel count %d not divisible by width %d" % (npx, w))
    h = npx // w
    return Image(w, h, argb_to_rgba(memoryview(raw)[4:4 + w * h * 4]))


def encode_ean(img, key=DEFAULT_KEY, level=9):
    raw = struct.pack(">I", img.w) + bytes(rgba_to_argb(img.px))
    return rc4(key, zlib.compress(raw, level))


# --------------------------------------------------------------------------- PNG

PNG_SIG = b"\x89PNG\r\n\x1a\n"


def _chunk(tag, data):
    return struct.pack(">I", len(data)) + tag + data + struct.pack(">I", zlib.crc32(tag + data) & 0xFFFFFFFF)


def write_png(img, level=6):
    stride = img.w * 4
    px = img.px
    if _np is not None and img.w > 1:
        # "Sub" filter on every row: good compression, vectorised
        a = _np.frombuffer(bytes(px), _np.uint8).reshape(img.h, img.w * 4)
        f = a.copy()
        f[:, 4:] = a[:, 4:] - a[:, :-4]
        out = _np.empty((img.h, stride + 1), _np.uint8)
        out[:, 0] = 1
        out[:, 1:] = f
        raw = out.tobytes()
    else:
        raw = bytearray()
        for y in range(img.h):
            raw.append(0)
            raw += px[y * stride:(y + 1) * stride]
        raw = bytes(raw)
    ihdr = struct.pack(">IIBBBBB", img.w, img.h, 8, 6, 0, 0, 0)
    return PNG_SIG + _chunk(b"IHDR", ihdr) + _chunk(b"IDAT", zlib.compress(raw, level)) + _chunk(b"IEND", b"")


def png_size(data):
    if data[:8] != PNG_SIG:
        return None
    return struct.unpack(">II", data[16:24])


def _unfilter(raw, w, h, bpp_bits, channels):
    """Undo PNG filtering; returns list of row bytearrays."""
    bpp = max(1, bpp_bits // 8)
    rowlen = (w * bpp_bits + 7) // 8
    rows = []
    prev = bytearray(rowlen)
    p = 0
    for _ in range(h):
        ft = raw[p]
        line = bytearray(raw[p + 1:p + 1 + rowlen])
        p += 1 + rowlen
        if ft == 0:
            pass
        elif ft == 1:
            if _np is not None and rowlen > 64:
                a = _np.frombuffer(bytes(line), _np.uint8).reshape(-1, bpp) if rowlen % bpp == 0 else None
                if a is not None:
                    line = bytearray((_np.cumsum(a, axis=0, dtype=_np.uint64) & 255).astype(_np.uint8).tobytes())
                else:
                    for i in range(bpp, rowlen):
                        line[i] = (line[i] + line[i - bpp]) & 255
            else:
                for i in range(bpp, rowlen):
                    line[i] = (line[i] + line[i - bpp]) & 255
        elif ft == 2:
            if _np is not None:
                line = bytearray((_np.frombuffer(bytes(line), _np.uint8) + _np.frombuffer(bytes(prev), _np.uint8)).tobytes())
            else:
                for i in range(rowlen):
                    line[i] = (line[i] + prev[i]) & 255
        elif ft == 3:
            for i in range(bpp):
                line[i] = (line[i] + (prev[i] >> 1)) & 255
            for i in range(bpp, rowlen):
                line[i] = (line[i] + ((line[i - bpp] + prev[i]) >> 1)) & 255
        elif ft == 4:
            for i in range(bpp):
                line[i] = (line[i] + prev[i]) & 255
            for i in range(bpp, rowlen):
                a = line[i - bpp]
                b = prev[i]
                c = prev[i - bpp]
                pa = b - c
                pb = a - c
                pc = pa + pb
                if pa < 0:
                    pa = -pa
                if pb < 0:
                    pb = -pb
                if pc < 0:
                    pc = -pc
                if pa <= pb and pa <= pc:
                    pr = a
                elif pb <= pc:
                    pr = b
                else:
                    pr = c
                line[i] = (line[i] + pr) & 255
        else:
            raise ValueError("bad PNG filter type %d" % ft)
        rows.append(line)
        prev = line
    return rows, p


def _rows_to_rgba(rows, w, depth, ctype, palette, trns):
    out = bytearray(w * len(rows) * 4)
    o = 0
    for line in rows:
        if depth == 16:
            line = line[0::2]  # take MSB of each sample
            d = 8
        else:
            d = depth
        if d < 8:
            vals = []
            mask = (1 << d) - 1
            for byte in line:
                for sh in range(8 - d, -1, -d):
                    vals.append((byte >> sh) & mask)
            vals = vals[:w]  # sub-byte depths only exist for 1-channel types
        else:
            vals = line
        if ctype == 6:
            out[o:o + w * 4] = vals[:w * 4]
        elif ctype == 2:
            seg = bytearray(w * 4)
            seg[0::4] = vals[0:w * 3:3]
            seg[1::4] = vals[1:w * 3:3]
            seg[2::4] = vals[2:w * 3:3]
            seg[3::4] = b"\xff" * w
            if trns is not None:
                for x in range(w):
                    if bytes(seg[x * 4:x * 4 + 3]) == trns:
                        seg[x * 4 + 3] = 0
            out[o:o + w * 4] = seg
        elif ctype == 4:
            seg = bytearray(w * 4)
            g = vals[0:w * 2:2]
            seg[0::4] = g
            seg[1::4] = g
            seg[2::4] = g
            seg[3::4] = vals[1:w * 2:2]
            out[o:o + w * 4] = seg
        elif ctype == 0:
            scale = 255 // ((1 << d) - 1)
            seg = bytearray(w * 4)
            for x in range(w):
                v = vals[x]
                g = v * scale
                seg[x * 4:x * 4 + 4] = bytes((g, g, g, 0 if (trns is not None and v == trns) else 255))
            out[o:o + w * 4] = seg
        elif ctype == 3:
            seg = bytearray(w * 4)
            for x in range(w):
                seg[x * 4:x * 4 + 4] = palette[vals[x]]
            out[o:o + w * 4] = seg
        o += w * 4
    return out


def read_png(data):
    if data[:8] != PNG_SIG:
        raise ValueError("not a PNG file")
    p = 8
    idat = []
    palette = None
    trns_raw = None
    w = h = depth = ctype = interlace = None
    while p < len(data):
        ln, tag = struct.unpack(">I4s", data[p:p + 8])
        body = data[p + 8:p + 8 + ln]
        p += 12 + ln
        if tag == b"IHDR":
            w, h, depth, ctype, _, _, interlace = struct.unpack(">IIBBBBB", body)
        elif tag == b"PLTE":
            palette = [bytes(body[i:i + 3]) + b"\xff" for i in range(0, len(body), 3)]
        elif tag == b"tRNS":
            trns_raw = body
        elif tag == b"IDAT":
            idat.append(body)
        elif tag == b"IEND":
            break
    if w is None:
        raise ValueError("PNG missing IHDR")
    raw = zlib.decompress(b"".join(idat))
    channels = {0: 1, 2: 3, 3: 1, 4: 2, 6: 4}[ctype]
    bpp_bits = channels * depth
    trns = None
    if trns_raw is not None:
        if ctype == 3 and palette:
            for i, a in enumerate(trns_raw[:len(palette)]):
                palette[i] = palette[i][:3] + bytes((a,))
        elif ctype == 0:
            trns = struct.unpack(">H", trns_raw[:2])[0]
            if depth == 16:
                trns >>= 8
        elif ctype == 2:
            r, g, b = struct.unpack(">HHH", trns_raw[:6])
            trns = bytes((r >> 8, g >> 8, b >> 8)) if depth == 16 else bytes((r, g, b))
    if ctype == 3 and palette:
        palette += [b"\0\0\0\xff"] * (256 - len(palette))
    if not interlace:
        rows, _ = _unfilter(raw, w, h, bpp_bits, channels)
        return Image(w, h, _rows_to_rgba(rows, w, depth, ctype, palette, trns))
    # Adam7
    img = Image(w, h)
    passes = [(0, 0, 8, 8), (4, 0, 8, 8), (0, 4, 4, 8), (2, 0, 4, 4), (0, 2, 2, 4), (1, 0, 2, 2), (0, 1, 1, 2)]
    off = 0
    for x0, y0, dx, dy in passes:
        pw = (w - x0 + dx - 1) // dx
        ph = (h - y0 + dy - 1) // dy
        if pw <= 0 or ph <= 0:
            continue
        rows, used = _unfilter(raw[off:], pw, ph, bpp_bits, channels)
        off += used
        sub = _rows_to_rgba(rows, pw, depth, ctype, palette, trns)
        for j in range(ph):
            for i in range(pw):
                s = (j * pw + i) * 4
                d = ((y0 + j * dy) * w + x0 + i * dx) * 4
                img.px[d:d + 4] = sub[s:s + 4]
    return img


def load_image_file(path):
    with open(path, "rb") as f:
        return read_png(f.read())


def save_image_file(img, path):
    import os
    os.makedirs(os.path.dirname(os.path.abspath(path)), exist_ok=True)
    with open(path, "wb") as f:
        f.write(write_png(img))


# --------------------------------------------------------------------------- JPEG / sniffing

def jpeg_info(data):
    if data[:2] != b"\xff\xd8":
        return None
    p = 2
    while p < len(data) - 9:
        if data[p] != 0xFF:
            p += 1
            continue
        m = data[p + 1]
        if m in (0xD8, 0x01) or 0xD0 <= m <= 0xD7:
            p += 2
            continue
        if m == 0xFF:
            p += 1
            continue
        ln = struct.unpack(">H", data[p + 2:p + 4])[0]
        if m in (0xC0, 0xC1, 0xC2, 0xC3, 0xC5, 0xC6, 0xC7, 0xC9, 0xCA, 0xCB, 0xCD, 0xCE, 0xCF):
            h, w = struct.unpack(">HH", data[p + 5:p + 9])
            return dict(w=w, h=h, progressive=m in (0xC2, 0xC6, 0xCA, 0xCE), components=data[p + 9])
        p += 2 + ln
    return None


def sniff(data):
    if data[:8] == PNG_SIG:
        return "png"
    if data[:3] == b"\xff\xd8\xff":
        return "jpg"
    if data[:4] in (b"GIF8",):
        return "gif"
    if data[:3] in (b"FWS", b"CWS", b"ZWS"):
        return "swf"
    if data[:3] == b"ID3" or (len(data) > 1 and data[0] == 0xFF and data[1] & 0xE0 == 0xE0):
        return "mp3"
    head = data[:200].lstrip(b"\xef\xbb\xbf \r\n\t")
    if head.startswith(b"<font"):
        return "fnt"
    if head.startswith(b"<particleEmitterConfig"):
        return "pex"
    if head.startswith(b"<"):
        return "xml"
    return "bin"


# --------------------------------------------------------------------------- MP3

_BR = {  # (version_is_mpeg1, layer) -> table
    (1, 3): [0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320],
    (1, 2): [0, 32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384],
    (1, 1): [0, 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448],
    (0, 3): [0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160],
    (0, 2): [0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160],
    (0, 1): [0, 32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256],
}
_SR = {3: [44100, 48000, 32000], 2: [22050, 24000, 16000], 0: [11025, 12000, 8000]}


def mp3_info(data):
    p = 0
    if data[:3] == b"ID3" and len(data) > 10:
        sz = data[6] << 21 | data[7] << 14 | data[8] << 7 | data[9]
        p = 10 + sz
    frames = 0
    dur = 0.0
    first = None
    bitrates = set()
    n = len(data)
    while p + 4 <= n:
        if data[p] != 0xFF or (data[p + 1] & 0xE0) != 0xE0:
            if frames == 0 and p < 65536:
                p += 1
                continue
            break
        b1, b2, b3 = data[p + 1], data[p + 2], data[p + 3]
        ver = (b1 >> 3) & 3
        layer = 4 - ((b1 >> 1) & 3)
        bri = b2 >> 4
        sri = (b2 >> 2) & 3
        if ver == 1 or layer == 4 or bri in (0, 15) or sri == 3:
            if frames == 0:
                p += 1
                continue
            break
        pad = (b2 >> 1) & 1
        br = _BR[(1 if ver == 3 else 0, layer)][bri] * 1000
        sr = _SR[ver][sri]
        if layer == 1:
            flen = (12 * br // sr + pad) * 4
            spf = 384
        else:
            spf = 1152 if (layer == 2 or ver == 3) else 576
            flen = spf // 8 * br // sr + pad
        if first is None:
            first = dict(mpeg={3: "1", 2: "2", 0: "2.5"}[ver], layer=layer, sample_rate=sr,
                         channels="mono" if (b3 >> 6) == 3 else "stereo")
        bitrates.add(br // 1000)
        frames += 1
        dur += spf / sr
        p += flen
    if not first:
        return None
    first.update(frames=frames, duration=round(dur, 3),
                 bitrate=(min(bitrates) if len(bitrates) == 1 else "VBR %d-%d" % (min(bitrates), max(bitrates))))
    return first


# --------------------------------------------------------------------------- Atlases / fonts

def parse_atlas(text_or_bytes):
    """Parse a Starling/TexturePacker atlas. Returns (root_attrs, [sprite dict])."""
    data = text_or_bytes.encode("utf-8") if isinstance(text_or_bytes, str) else text_or_bytes
    sprites = []
    root = {}
    p = xml.parsers.expat.ParserCreate()

    def start(name, attrs):
        if name == "TextureAtlas":
            root.update(attrs)
        elif name == "SubTexture":
            s = dict(attrs)
            for k in ("x", "y", "width", "height", "frameX", "frameY", "frameWidth", "frameHeight"):
                if k in s:
                    s[k] = int(float(s[k]))
            sprites.append(s)
    p.StartElementHandler = start
    p.Parse(xml_lenient(data), True)
    return root, sprites


def write_atlas(root_attrs, sprites, note="Generated by RRModKit"):
    lines = ['<?xml version="1.0" encoding="UTF-8"?>', "<!-- %s -->" % note]
    attrs = "".join(" %s=%s" % (k, quoteattr(str(v))) for k, v in root_attrs.items())
    lines.append("<TextureAtlas%s>" % attrs)
    for s in sprites:
        parts = ["name=%s" % quoteattr(s["name"])]
        for k in ("x", "y", "width", "height", "frameX", "frameY", "frameWidth", "frameHeight", "rotated", "pivotX", "pivotY"):
            if k in s:
                parts.append("%s=%s" % (k, quoteattr(str(s[k]))))
        for k, v in s.items():
            if k not in ("name", "x", "y", "width", "height", "frameX", "frameY", "frameWidth", "frameHeight",
                         "rotated", "pivotX", "pivotY") and not k.startswith("_"):
                parts.append("%s=%s" % (k, quoteattr(str(v))))
        lines.append("    <SubTexture %s/>" % " ".join(parts))
    lines.append("</TextureAtlas>")
    return ("\n".join(lines) + "\n").encode("utf-8")


def is_atlas_xml(data):
    return b"<TextureAtlas" in data[:600]


def parse_fnt(data):
    """BMFont XML -> dict(info, common, pages{id:file}, chars[...], kernings)"""
    res = dict(info={}, common={}, pages={}, chars=[], kernings=0)
    p = xml.parsers.expat.ParserCreate()

    def start(name, a):
        if name == "info":
            res["info"] = dict(a)
        elif name == "common":
            res["common"] = dict(a)
        elif name == "page":
            res["pages"][int(a.get("id", 0))] = a.get("file")
        elif name == "char":
            c = {k: int(float(v)) for k, v in a.items() if k != "letter"}
            res["chars"].append(c)
        elif name == "kerning":
            res["kernings"] += 1
    p.StartElementHandler = start
    p.Parse(xml_lenient(data), True)
    return res


def fnt_as_sprites(fnt):
    out = []
    for c in fnt["chars"]:
        cid = c.get("id", 0)
        ch = chr(cid) if 32 < cid < 0x110000 else ""
        out.append(dict(name="%d %s" % (cid, ch), x=c.get("x", 0), y=c.get("y", 0),
                        width=c.get("width", 0), height=c.get("height", 0), page=c.get("page", 0)))
    return out


def xml_lenient(data):
    """Flash's XML parser accepts '--' inside comments; strict parsers don't.
    Replace those with '- ' (same length, so byte offsets are preserved)."""
    if isinstance(data, str):
        data = data.encode("utf-8")
    if b"<!--" not in data:
        return data
    out = bytearray(data)
    i = 0
    while True:
        a = data.find(b"<!--", i)
        if a < 0:
            break
        b = data.find(b"-->", a + 4)
        if b < 0:
            break
        j = a + 4
        while True:
            k = data.find(b"--", j, b)
            if k < 0:
                break
            out[k + 1] = 0x20
            j = k + 2
        if b > a + 4 and data[b - 1] == 0x2D:  # '--->' ending
            out[b - 1] = 0x20
        i = b + 3
    return bytes(out)


def xml_strict_issues(data):
    """Things Flash tolerates but strict XML does not (informational)."""
    return ["comment contains '--' (Flash accepts it)"] if xml_lenient(data) != data else []


def xml_check(data):
    """Return None if well-formed (Flash-lenient) else 'line:col message'."""
    p = xml.parsers.expat.ParserCreate()
    try:
        p.Parse(xml_lenient(data), True)
        return None
    except xml.parsers.expat.ExpatError as e:
        return "line %d, col %d: %s" % (e.lineno, e.offset + 1, xml.parsers.expat.ErrorString(e.code))
