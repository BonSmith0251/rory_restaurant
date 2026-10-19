"""AVM2 -> pseudo-ActionScript decompiler.

Rebuilds expressions by symbolic stack execution. Control flow is emitted as labels and
`goto`s (no loop structuring), which is easy to read and to port to C# (which has goto too).
Operand-stack values that live across jumps are spilled into `$sN` temporaries.
"""
import json
import os
import re

from .swf import _decode

PREC_PRIMARY, PREC_UNARY, PREC_MUL, PREC_ADD, PREC_SHIFT, PREC_REL, PREC_EQ = 20, 15, 13, 12, 11, 10, 9
PREC_BAND, PREC_BXOR, PREC_BOR, PREC_COND, PREC_ASSIGN = 8, 7, 6, 3, 1

BINOPS = {"add": ("+", PREC_ADD), "add_i": ("+", PREC_ADD), "subtract": ("-", PREC_ADD), "subtract_i": ("-", PREC_ADD),
          "multiply": ("*", PREC_MUL), "multiply_i": ("*", PREC_MUL), "divide": ("/", PREC_MUL), "modulo": ("%", PREC_MUL),
          "lshift": ("<<", PREC_SHIFT), "rshift": (">>", PREC_SHIFT), "urshift": (">>>", PREC_SHIFT),
          "bitand": ("&", PREC_BAND), "bitor": ("|", PREC_BOR), "bitxor": ("^", PREC_BXOR),
          "equals": ("==", PREC_EQ), "strictequals": ("===", PREC_EQ), "lessthan": ("<", PREC_REL),
          "lessequals": ("<=", PREC_REL), "greaterthan": (">", PREC_REL), "greaterequals": (">=", PREC_REL),
          "instanceof": ("instanceof", PREC_REL), "istypelate": ("is", PREC_REL), "astypelate": ("as", PREC_REL),
          "in": ("in", PREC_REL)}
CONDJUMPS = {"ifeq": ("==", False), "ifne": ("!=", False), "iflt": ("<", False), "ifle": ("<=", False),
             "ifgt": (">", False), "ifge": (">=", False), "ifstricteq": ("===", False), "ifstrictne": ("!==", False),
             "ifnlt": ("<", True), "ifnle": ("<=", True), "ifngt": (">", True), "ifnge": (">=", True)}
IDENT = re.compile(r"^[A-Za-z_$][\w$]*$")


class E:
    __slots__ = ("t", "p", "simple", "kind")

    def __init__(self, t, p=PREC_PRIMARY, simple=False, kind=None):
        self.t, self.p, self.simple, self.kind = t, p, simple, kind

    def __str__(self):
        return self.t

    def wrap(self, p):
        return "(%s)" % self.t if self.p < p else self.t


def lit(v):
    if isinstance(v, bool):
        return "true" if v else "false"
    if isinstance(v, str):
        return json.dumps(v, ensure_ascii=False)
    if isinstance(v, float):
        if v == int(v) and abs(v) < 1e15:
            return str(int(v))
        return repr(v)
    return str(v)


def const_str(v):
    if v is None:
        return None
    if v == "<null>":
        return "null"
    if v == "<undefined>":
        return "undefined"
    return lit(v)


def member(obj, name):
    if obj.kind == "find":  # findpropstrict/findproperty: scope lookup
        return name
    if IDENT.match(name or "") or (name and name[0] == "@" and IDENT.match(name[1:])):
        return "%s.%s" % (obj.wrap(PREC_PRIMARY), name)
    return "%s[%s]" % (obj.wrap(PREC_PRIMARY), lit(name))


class MethodDecompiler:
    def __init__(self, abc, mi, indent="    ", this_class=None, depth=0):
        self.abc = abc
        self.mi = mi
        self.ind = indent
        self.depth = depth
        self.m = abc.methods[mi]
        self.info = abc.body_info.get(mi, {})
        self.code = abc.bodies.get(mi)
        self.this_class = this_class
        pc = self.m["params"]
        self.reg = {0: "this"}
        for i in range(pc):
            self.reg[i + 1] = (self.m["pnames"][i] if self.m["pnames"] else "arg%d" % (i + 1))
        if self.m["flags"] & 0x05:
            self.reg[pc + 1] = "rest" if self.m["flags"] & 0x04 else "arguments"
        self.slots = {t["slot_id"]: t["name"] for t in self.info.get("traits", []) if "slot_id" in t}
        self.declared = set()

    def regname(self, n):
        return self.reg.get(n, "_local%d" % n)

    # ----------------------------------------------------------------- signature
    def signature(self, name, kind=""):
        m = self.m
        ps = []
        opts = m["optional"]
        first_opt = m["params"] - len(opts)
        for i in range(m["params"]):
            s = "%s:%s" % (self.reg[i + 1], m["ptypes"][i])
            if i >= first_opt:
                s += " = " + const_str(opts[i - first_opt])
            ps.append(s)
        if m["flags"] & 0x04:
            ps.append("...rest")
        return "function %s%s(%s):%s" % (kind, name, ", ".join(ps), m["ret"])

    # ----------------------------------------------------------------- body
    def decompile(self):
        if self.code is None:
            return ["%s// native / abstract" % self.ind]
        instrs = list(_decode(self.abc, self.code))
        offsets = [o for o, _, _ in instrs]
        sizes = {}
        for i, (o, _, _) in enumerate(instrs):
            sizes[o] = (offsets[i + 1] if i + 1 < len(offsets) else len(self.code)) - o
        targets = set()
        for o, name, args in instrs:
            if name in CONDJUMPS or name in ("jump", "iftrue", "iffalse"):
                targets.add(int(args[0][1:]))
            elif name == "lookupswitch":
                for t in re.findall(r"L(\d+)", args[0]):
                    targets.add(int(t))
        exc = self.info.get("exceptions", [])
        exc_targets = {e["target"]: e for e in exc}
        targets |= set(exc_targets)
        try_starts = {}
        try_ends = {}
        for e in exc:
            try_starts.setdefault(e["start"], []).append(e)
            try_ends.setdefault(e["end"], []).append(e)

        out = []
        ind = self.ind
        stack = []
        scope = []
        depth_at = {}
        dead = False
        tmp = [0]

        def emit(s):
            out.append(ind + s)

        def spill():
            nonlocal stack
            new = []
            for i, e in enumerate(stack):
                name = "$s%d" % i
                if str(e) != name:
                    emit("%s = %s;" % (name, e))
                new.append(E(name, simple=True))
            stack = new

        def pop():
            if not stack:
                return E("$underflow", simple=True)
            return stack.pop()

        def popn(n):
            if n == 0:
                return []
            vals = [pop() for _ in range(n)]
            vals.reverse()
            return vals

        def args_str(a):
            return ", ".join(x.wrap(PREC_ASSIGN + 1) for x in a)

        def mname_pop(mn, rt):
            """pop runtime multiname parts; returns (prefix_fn) to build member access"""
            extra = popn(rt)
            return extra

        def access(obj, mn, extra):
            if extra:
                return "%s[%s]" % (obj.wrap(PREC_PRIMARY), extra[-1])
            return member(obj, mn)

        def jump_to(label):
            spill()
            depth_at[label] = max(depth_at.get(label, 0), len(stack))

        for o, name, args in instrs:
            if o in try_ends:
                for e in try_ends[o]:
                    emit("// } end try (handler L%d)" % e["target"])
            if o in targets:
                if not dead:
                    spill()
                    depth_at[o] = max(depth_at.get(o, 0), len(stack))
                d = depth_at.get(o, 0)
                if o in exc_targets:
                    e = exc_targets[o]
                    out.append("%sL%d: // catch (%s:%s)" % (ind[:-2] if len(ind) > 2 else "", o, e["var"], e["type"]))
                    stack = [E("$exception", simple=True)]
                    scope = scope[:1]
                else:
                    out.append("%sL%d:" % (ind[:-2] if len(ind) > 2 else "", o))
                    stack = [E("$s%d" % i, simple=True) for i in range(d)]
                dead = False
            if o in try_starts:
                for e in try_starts[o]:
                    emit("// try { (catch %s:%s -> L%d)" % (e["var"], e["type"], e["target"]))
            if dead and name not in ("label",):
                continue

            mi = None
            if name in ("getlocal0", "getlocal1", "getlocal2", "getlocal3"):
                stack.append(E(self.regname(int(name[-1])), simple=True))
            elif name == "getlocal":
                stack.append(E(self.regname(args[0]), simple=True))
            elif name in ("setlocal0", "setlocal1", "setlocal2", "setlocal3", "setlocal"):
                n = int(name[-1]) if name != "setlocal" else args[0]
                v = pop()
                rn = self.regname(n)
                if v.kind == "activation":
                    self.reg[n] = "$activation"
                    continue
                if rn == str(v):
                    continue
                prefix = ""
                if n > self.m["params"] and rn not in self.declared and rn.startswith("_local"):
                    self.declared.add(rn)
                    prefix = "var "
                emit("%s%s = %s;" % (prefix, rn, v))
            elif name in ("kill", "label", "nop", "debugline", "debugfile", "debug", "dxns", "dxnslate"):
                if name == "debug" and args and args[0] == 1:
                    pass
            elif name in ("inclocal", "inclocal_i"):
                emit("%s++;" % self.regname(args[0]))
            elif name in ("declocal", "declocal_i"):
                emit("%s--;" % self.regname(args[0]))
            elif name in ("pushbyte", "pushshort", "pushint", "pushuint", "pushdouble"):
                stack.append(E(lit(args[0]), simple=True))
            elif name == "pushstring":
                stack.append(E(lit(args[0]), simple=True))
            elif name in ("pushtrue", "pushfalse", "pushnull", "pushundefined", "pushnan"):
                stack.append(E({"pushtrue": "true", "pushfalse": "false", "pushnull": "null",
                                "pushundefined": "undefined", "pushnan": "NaN"}[name], simple=True))
            elif name == "pushnamespace":
                stack.append(E("namespace#%d" % args[0], simple=True))
            elif name == "pop":
                v = pop()
                if not v.simple:
                    emit("%s;" % v)
            elif name == "dup":
                v = pop()
                if v.simple or v.kind in ("find", "activation", "scope"):
                    stack += [v, v]
                else:
                    t = "$t%d" % tmp[0]
                    tmp[0] += 1
                    emit("var %s = %s;" % (t, v))
                    stack += [E(t, simple=True), E(t, simple=True)]
            elif name == "swap":
                if len(stack) >= 2:
                    stack[-1], stack[-2] = stack[-2], stack[-1]
            elif name == "pushscope" or name == "pushwith":
                scope.append(pop())
            elif name == "popscope":
                if scope:
                    scope.pop()
            elif name == "getscopeobject":
                i = args[0]
                stack.append(scope[i] if i < len(scope) else E("scope%d" % i, simple=True, kind="scope"))
            elif name == "getglobalscope":
                stack.append(E("global", simple=True, kind="scope"))
            elif name == "newactivation":
                stack.append(E("$activation", simple=True, kind="activation"))
            elif name == "newcatch":
                stack.append(E("$catchscope", simple=True, kind="scope"))
            elif name in ("findpropstrict", "findproperty", "finddef"):
                stack.append(E(args[0], simple=True, kind="find"))
            elif name == "getlex":
                stack.append(E(args[0], simple=True))
            elif name in ("getproperty", "getsuper"):
                extra = popn(self._rt(o))
                obj = pop()
                if name == "getsuper":
                    stack.append(E("super." + args[0]))
                else:
                    stack.append(E(access(obj, args[0], extra), simple=False))
            elif name in ("setproperty", "initproperty", "setsuper"):
                v = pop()
                extra = popn(self._rt(o))
                obj = pop()
                target = "super." + args[0] if name == "setsuper" else access(obj, args[0], extra)
                emit("%s = %s;" % (target, v))
            elif name == "deleteproperty":
                extra = popn(self._rt(o))
                obj = pop()
                stack.append(E("delete " + access(obj, args[0], extra), PREC_UNARY))
            elif name in ("getslot", "getglobalslot"):
                obj = pop() if name == "getslot" else E("global", kind="scope")
                stack.append(E(self._slot(obj, args[0]), simple=obj.kind == "activation"))
            elif name in ("setslot", "setglobalslot"):
                v = pop()
                obj = pop() if name == "setslot" else E("global", kind="scope")
                emit("%s = %s;" % (self._slot(obj, args[0]), v))
            elif name in ("callproperty", "callproplex", "callpropvoid", "callsuper", "callsupervoid"):
                mn, argc = args
                a = popn(argc)
                extra = popn(self._rt(o))
                obj = pop()
                if name.startswith("callsuper"):
                    call = "super.%s(%s)" % (mn, args_str(a))
                else:
                    call = "%s(%s)" % (access(obj, mn, extra), args_str(a))
                if name.endswith("void"):
                    emit(call + ";")
                else:
                    stack.append(E(call))
            elif name in ("callmethod", "callstatic"):
                a = popn(args[1])
                obj = pop()
                stack.append(E("%s.method#%d(%s)" % (obj.wrap(PREC_PRIMARY), args[0], args_str(a))))
            elif name == "call":
                a = popn(args[0])
                recv = pop()
                f = pop()
                stack.append(E("%s(%s)" % (f.wrap(PREC_PRIMARY), args_str(a))))
            elif name == "construct":
                a = popn(args[0])
                f = pop()
                stack.append(E("new %s(%s)" % (f.wrap(PREC_PRIMARY), args_str(a))))
            elif name == "constructprop":
                mn, argc = args
                a = popn(argc)
                extra = popn(self._rt(o))
                obj = pop()
                stack.append(E("new %s(%s)" % (access(obj, mn, extra), args_str(a))))
            elif name == "constructsuper":
                a = popn(args[0])
                pop()
                emit("super(%s);" % args_str(a))
            elif name == "applytype":
                a = popn(args[0])
                base = pop()
                stack.append(E("%s.<%s>" % (base, ", ".join(map(str, a))), simple=True))
            elif name == "newobject":
                kv = popn(args[0] * 2)
                items = ["%s: %s" % (kv[i], kv[i + 1].wrap(PREC_ASSIGN + 1)) for i in range(0, len(kv), 2)]
                stack.append(E("{%s}" % ", ".join(items)))
            elif name == "newarray":
                a = popn(args[0])
                stack.append(E("[%s]" % args_str(a)))
            elif name == "newfunction":
                stack.append(E(self._closure(args[0])))
            elif name == "newclass":
                pop()
                stack.append(E("class#%d" % args[0], simple=True))
            elif name in BINOPS:
                op, p = BINOPS[name]
                b = pop()
                a = pop()
                stack.append(E("%s %s %s" % (a.wrap(p), op, b.wrap(p + 1)), p))
            elif name == "not":
                stack.append(E("!" + pop().wrap(PREC_UNARY), PREC_UNARY))
            elif name in ("negate", "negate_i"):
                stack.append(E("-" + pop().wrap(PREC_UNARY), PREC_UNARY))
            elif name == "bitnot":
                stack.append(E("~" + pop().wrap(PREC_UNARY), PREC_UNARY))
            elif name == "typeof":
                stack.append(E("typeof " + pop().wrap(PREC_UNARY), PREC_UNARY))
            elif name in ("increment", "increment_i"):
                stack.append(E("%s + 1" % pop().wrap(PREC_ADD), PREC_ADD))
            elif name in ("decrement", "decrement_i"):
                stack.append(E("%s - 1" % pop().wrap(PREC_ADD), PREC_ADD))
            elif name in ("convert_i", "convert_u", "convert_d", "convert_s", "convert_b", "convert_o"):
                v = pop()
                fn = {"convert_i": "int", "convert_u": "uint", "convert_d": "Number", "convert_s": "String",
                      "convert_b": "Boolean", "convert_o": ""}[name]
                stack.append(E("%s(%s)" % (fn, v)) if fn and not v.t.startswith(fn + "(") else v)
            elif name in ("coerce", "coerce_a", "coerce_s", "checkfilter", "esc_xelem", "esc_xattr"):
                if name in ("esc_xelem", "esc_xattr"):
                    stack.append(E("String(%s)" % pop()))
            elif name in ("astype", "istype"):
                v = pop()
                stack.append(E("%s %s %s" % (v.wrap(PREC_REL), "as" if name == "astype" else "is", args[0]), PREC_REL))
            elif name == "getdescendants":
                extra = popn(self._rt(o))
                obj = pop()
                stack.append(E("%s..%s" % (obj.wrap(PREC_PRIMARY), args[0])))
            elif name == "hasnext2":
                obj_r, idx_r = args
                stack.append(E("hasnext2(%s, %s)" % (self.regname(obj_r), self.regname(idx_r))))
                emit("// for-in iteration over %s (index in %s)" % (self.regname(obj_r), self.regname(idx_r)))
            elif name == "hasnext":
                i = pop()
                ob = pop()
                stack.append(E("hasnext(%s, %s)" % (ob, i)))
            elif name in ("nextname", "nextvalue"):
                i = pop()
                ob = pop()
                stack.append(E("%s(%s, %s)" % (name, ob, i)))
            elif name in ("iftrue", "iffalse"):
                c = pop()
                cond = c.t if name == "iftrue" else "!" + c.wrap(PREC_UNARY)
                jump_to(int(args[0][1:]))
                emit("if (%s) goto %s;" % (cond, args[0]))
            elif name in CONDJUMPS:
                op, neg = CONDJUMPS[name]
                b = pop()
                a = pop()
                cond = "%s %s %s" % (a.wrap(PREC_EQ + 1), op, b.wrap(PREC_EQ + 1))
                if neg:
                    cond = "!(%s)" % cond
                jump_to(int(args[0][1:]))
                emit("if (%s) goto %s;" % (cond, args[0]))
            elif name == "jump":
                jump_to(int(args[0][1:]))
                emit("goto %s;" % args[0])
                dead = True
            elif name == "lookupswitch":
                idx = pop()
                spill()
                m2 = re.match(r"default:L(\d+) \[(.*)\]", args[0])
                default = int(m2.group(1))
                cases = [int(x[1:]) for x in m2.group(2).split(", ") if x]
                for t in [default] + cases:
                    depth_at[t] = max(depth_at.get(t, 0), len(stack))
                emit("switch (%s) { %s default: goto L%d; }" % (
                    idx, " ".join("case %d: goto L%d;" % (i, t) for i, t in enumerate(cases)), default))
                dead = True
            elif name == "returnvalue":
                emit("return %s;" % pop())
                dead = True
            elif name == "returnvoid":
                emit("return;")
                dead = True
            elif name == "throw":
                emit("throw %s;" % pop())
                dead = True
            elif name in ("li8", "li16", "li32", "lf32", "lf64"):
                stack.append(E("%s(%s)" % (name, pop())))
            elif name in ("si8", "si16", "si32", "sf32", "sf64"):
                a = pop()
                v = pop()
                emit("%s(%s, %s);" % (name, a, v))
            elif name in ("sxi1", "sxi8", "sxi16"):
                stack.append(E("%s(%s)" % (name, pop())))
            else:
                emit("/* unhandled %s %s */" % (name, args))
        # strip trailing bare "return;" at the end of void methods
        if out and out[-1].strip() == "return;":
            out.pop()
        return out

    def _rt(self, off):
        # find multiname index at this offset (re-read u30 after opcode)
        code = self.code
        p = off + 1
        r = 0
        sh = 0
        while True:
            b = code[p]
            p += 1
            r |= (b & 0x7F) << sh
            sh += 7
            if not b & 0x80:
                break
        return self.abc.mn_runtime(r)

    def _slot(self, obj, sid):
        if obj.kind == "activation":
            return self.slots.get(sid, "slot%d" % sid)
        if obj.kind == "scope" and str(obj) == "global":
            for s in self.abc.scripts:
                for t in s["traits"]:
                    if t.get("slot_id") == sid:
                        return t["name"]
        return "%s.slot%d" % (obj.wrap(PREC_PRIMARY), sid)

    def _closure(self, mi):
        sub = MethodDecompiler(self.abc, mi, self.ind + "    ", self.this_class, self.depth + 1)
        # closures can't see parent registers, but name params uniquely to avoid confusion
        for k in list(sub.reg):
            if k > 0 and sub.reg[k].startswith("arg"):
                sub.reg[k] = "f%d_%s" % (self.depth + 1, sub.reg[k])
        body = sub.decompile()
        head = sub.signature("").replace("function (", "function(")
        return head + " {\n" + "\n".join(body) + "\n" + self.ind + "}"


def decompile_class(abc, c):
    lines = []
    ext = " extends %s" % c["super"] if c["super"] and c["super"] != "Object" else ""
    impl = " implements %s" % ", ".join(c["interfaces"]) if c["interfaces"] else ""
    pkg = c["fullname"].rsplit(".", 1)[0] if "." in c["fullname"] else ""
    lines.append("package %s {" % pkg if pkg else "package {")
    lines.append("%s %s%s%s {" % ("interface" if c["interface"] else "class", c["name"], ext, impl))

    def fields(traits, static):
        for t in traits:
            if t["kind"] in (0, 6):
                v = const_str(t.get("value"))
                lines.append("    %s%s %s:%s%s;" % ("static " if static else "", "var" if t["kind"] == 0 else "const",
                                                      t["name"], t["type"], " = " + v if v is not None else ""))

    def methods(traits, static):
        for t in traits:
            if t["kind"] in (1, 2, 3):
                kind = {1: "", 2: "get ", 3: "set "}[t["kind"]]
                md = MethodDecompiler(abc, t["method"], "        ", c["name"])
                lines.append("")
                lines.append("    %s%s%s {" % ("static " if static else "", "override " if t["override"] else "",
                                                md.signature(t["name"], kind)))
                lines.extend(md.decompile())
                lines.append("    }")

    fields(c["ctraits_full"], True)
    fields(c["itraits_full"], False)
    ci = MethodDecompiler(abc, c["cinit"], "        ", c["name"])
    body = ci.decompile()
    if body:
        lines.append("")
        lines.append("    // static initializer")
        lines.append("    static {")
        lines.extend(body)
        lines.append("    }")
    md = MethodDecompiler(abc, c["iinit"], "        ", c["name"])
    lines.append("")
    lines.append("    " + md.signature(c["name"]) + " {")
    lines.extend(md.decompile())
    lines.append("    }")
    methods(c["ctraits_full"], True)
    methods(c["itraits_full"], False)
    lines.append("}")
    lines.append("}")
    return "\n".join(lines) + "\n"


LIB_RE = re.compile(r"^(starling|com\.|flash|mx\.|org\.|air\.)")


def decompile_all(abc, out_dir, include_libs=False, log=print):
    os.makedirs(out_dir, exist_ok=True)
    for fn in os.listdir(out_dir):
        if fn.endswith(".as"):
            os.remove(os.path.join(out_dir, fn))
    n = 0
    for c in abc.classes:
        if not include_libs and LIB_RE.match(c["fullname"]):
            continue
        text = decompile_class(abc, c)
        fn = c["fullname"].replace(":", ".")
        path = os.path.join(out_dir, fn + ".as")
        k = 2
        while os.path.exists(path) and n:
            path = os.path.join(out_dir, "%s_%d.as" % (fn, k))
            k += 1
        with open(path, "w", encoding="utf-8") as f:
            f.write(text)
        n += 1
    if log:
        log("decompiled %d classes -> %s" % (n, out_dir))
    return n
