"""Table view of repetitive XML data files with in-place (formatting/comment preserving) edits.

A "record" is every element with a chosen tag. Its fields are:
  @attr          attribute of the record
  #text          text of the record (if it has no child elements)
  child          text of a leaf child element
  child@attr     attribute of a leaf child element
"""
import re
import xml.parsers.expat
from collections import Counter
from xml.sax.saxutils import escape

from .formats import sha1, xml_lenient


class _Node:
    __slots__ = ("tag", "attrs", "start", "start_end", "end", "children", "text", "parent", "selfclose")

    def __init__(self, tag, attrs, start, parent):
        self.tag, self.attrs, self.start, self.parent = tag, attrs, start, parent
        self.children, self.text = [], []
        self.start_end = self.end = None
        self.selfclose = False


def _tag_end(data, i):
    """Index just past the '>' closing the tag starting at data[i] ('<')."""
    q = None
    j = i + 1
    while j < len(data):
        c = data[j]
        if q:
            if c == q:
                q = None
        elif c in (0x22, 0x27):
            q = c
        elif c == 0x3E:
            return j + 1
        j += 1
    raise ValueError("unterminated tag")


def _local(tag):
    return tag.rsplit("}", 1)[-1].rsplit(" ", 1)[-1]


def parse(data):
    p = xml.parsers.expat.ParserCreate()
    p.buffer_text = True
    root = _Node("#doc", {}, 0, None)
    stack = [root]

    def start(name, attrs):
        n = _Node(_local(name), attrs, p.CurrentByteIndex, stack[-1])
        n.start_end = _tag_end(data, n.start)
        n.selfclose = data[n.start_end - 2] == 0x2F  # '/>'
        stack[-1].children.append(n)
        stack.append(n)

    def end(name):
        n = stack.pop()
        if n.selfclose:
            n.end = n.start_end
        else:
            n.end = _tag_end(data, p.CurrentByteIndex)

    def chars(t):
        stack[-1].text.append(t)

    p.StartElementHandler = start
    p.EndElementHandler = end
    p.CharacterDataHandler = chars
    p.Parse(xml_lenient(data), True)
    return root


def _walk(n):
    for c in n.children:
        yield c
        yield from _walk(c)


def _is_leaf(n):
    return not n.children


def _fields(n):
    f = {}
    for k, v in n.attrs.items():
        f["@" + _local(k)] = v
    if _is_leaf(n):
        f["#text"] = "".join(n.text).strip()
    seen = Counter()
    for c in n.children:
        seen[c.tag] += 1
    for c in n.children:
        if _is_leaf(c) and seen[c.tag] == 1:
            f[c.tag] = "".join(c.text).strip()
            for k, v in c.attrs.items():
                f["%s@%s" % (c.tag, _local(k))] = v
    return f


def candidates(root):
    cnt = Counter()
    for n in _walk(root):
        if n.parent is not root:  # skip document element
            cnt[n.tag] += 1
    out = []
    for tag, c in cnt.items():
        if c < 2:
            continue
        sample = next(n for n in _walk(root) if n.tag == tag)
        nf = len(_fields(sample))
        if nf:
            out.append((tag, c, nf))
    out.sort(key=lambda t: (-(t[1] * min(t[2], 8)), t[0]))
    return [t[0] for t in out]


def _context(n):
    parts = []
    p = n.parent
    while p is not None and p.parent is not None and p.parent.parent is not None:
        sibs = [s for s in p.parent.children if s.tag == p.tag]
        parts.append("%s %d" % (p.tag, sibs.index(p) + 1))
        p = p.parent
    return " / ".join(reversed(parts))


def table(data, tag=None):
    root = parse(data)
    cands = candidates(root)
    if not cands:
        return dict(tags=[], tag=None, columns=[], rows=[], sha=sha1(data))
    tag = tag if tag in cands else cands[0]
    nodes = [n for n in _walk(root) if n.tag == tag]
    cols = []
    rows = []
    for n in nodes:
        f = _fields(n)
        for k in f:
            if k not in cols:
                cols.append(k)
        rows.append(dict(context=_context(n), fields=f))
    if all(not r["fields"].get("#text") for r in rows) and "#text" in cols:
        cols.remove("#text")
    has_ctx = any(r["context"] for r in rows)
    return dict(tags=cands, tag=tag, columns=cols, rows=rows, sha=sha1(data), has_context=has_ctx)


def _enc(data):
    m = re.match(rb'<\?xml[^>]*encoding=["\']([A-Za-z0-9_\-]+)', data)
    return m.group(1).decode() if m else "utf-8"


def _set_attr(data, n, attr, value, enc):
    seg = data[n.start:n.start_end]
    v = escape(value, {'"': "&quot;"}).encode(enc)
    m = re.search(rb"(\s)(" + re.escape(attr.encode()) + rb")(\s*=\s*)([\"'])(.*?)\4", seg, re.S)
    if m:
        a, b = n.start + m.start(5), n.start + m.end(5)
        if m.group(4) == b"'":
            v = v.replace(b"'", b"&apos;")
        return a, b, v
    pos = n.start_end - (2 if n.selfclose else 1)
    while data[pos - 1:pos] in (b" ", b"\t"):
        pos -= 1
    return pos, pos, b" " + attr.encode() + b'="' + v + b'"'


def _set_text(data, n, value, enc):
    v = escape(value).encode(enc)
    if n.selfclose:
        return n.start_end - 2, n.end, b">" + v + b"</" + n.tag.encode() + b">"
    inner = data[n.start_end:data.rindex(b"<", n.start_end, n.end)]
    # keep surrounding whitespace of the original content
    lead = inner[:len(inner) - len(inner.lstrip())]
    trail = inner[len(inner.rstrip()):]
    if inner.strip().startswith(b"<!--"):
        lead = trail = b""
    a = n.start_end
    b = data.rindex(b"<", n.start_end, n.end)
    return a, b, lead + v + trail


def apply(data, tag, edits, expected_sha=None, row_ops=None):
    """edits: [{row, field, value}], row_ops: [{op: 'duplicate'|'delete', row}] (applied after edits)."""
    if expected_sha and sha1(data) != expected_sha:
        raise ValueError("file changed on disk since it was loaded - reload and try again")
    enc = _enc(data)
    root = parse(data)
    nodes = [n for n in _walk(root) if n.tag == tag]
    patches = []
    for e in edits or []:
        n = nodes[int(e["row"])]
        field = e["field"]
        value = str(e["value"])
        if field == "#text":
            patches.append(_set_text(data, n, value, enc))
        elif field.startswith("@"):
            patches.append(_set_attr(data, n, field[1:], value, enc))
        else:
            child, _, attr = field.partition("@")
            c = next((c for c in n.children if c.tag == child), None)
            if c is None:
                raise ValueError("row %s has no <%s> element" % (e["row"], child))
            patches.append(_set_attr(data, c, attr, value, enc) if attr else _set_text(data, c, value, enc))
    for op in row_ops or []:
        n = nodes[int(op["row"])]
        ls = data.rfind(b"\n", 0, n.start) + 1
        indent = data[ls:n.start] if not data[ls:n.start].strip() else b""
        if op["op"] == "duplicate":
            patches.append((n.end, n.end, b"\n" + indent + data[n.start:n.end]))
        elif op["op"] == "delete":
            a = ls if indent or ls == n.start else n.start
            b = n.end
            if data[b:b + 2] == b"\r\n":
                b += 2
            elif data[b:b + 1] == b"\n":
                b += 1
            patches.append((a, b, b""))
    patches.sort(key=lambda p: (p[0], p[1]), reverse=True)
    out = bytearray(data)
    last = None
    for a, b, v in patches:
        if last is not None and b > last:
            raise ValueError("overlapping edits")
        out[a:b] = v
        last = a
    return bytes(out)
