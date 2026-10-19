"""MaxRects (best short side fit) texture packer producing power-of-two atlases."""


class _MaxRects:
    def __init__(self, w, h):
        self.w, self.h = w, h
        self.free = [(0, 0, w, h)]

    def insert(self, w, h):
        best = None
        for fx, fy, fw, fh in self.free:
            if w <= fw and h <= fh:
                short = min(fw - w, fh - h)
                long_ = max(fw - w, fh - h)
                key = (short, long_, fy, fx)
                if best is None or key < best[0]:
                    best = (key, fx, fy)
        if best is None:
            return None
        x, y = best[1], best[2]
        self._split(x, y, w, h)
        return x, y

    def _split(self, x, y, w, h):
        new = []
        for f in self.free:
            fx, fy, fw, fh = f
            if x >= fx + fw or x + w <= fx or y >= fy + fh or y + h <= fy:
                new.append(f)
                continue
            if x > fx:
                new.append((fx, fy, x - fx, fh))
            if x + w < fx + fw:
                new.append((x + w, fy, fx + fw - x - w, fh))
            if y > fy:
                new.append((fx, fy, fw, y - fy))
            if y + h < fy + fh:
                new.append((fx, y + h, fw, fy + fh - y - h))
        # prune contained rectangles
        new = [r for r in new if r[2] > 0 and r[3] > 0]
        pruned = []
        for i, a in enumerate(new):
            contained = False
            for j, b in enumerate(new):
                if i != j and a[0] >= b[0] and a[1] >= b[1] and a[0] + a[2] <= b[0] + b[2] and a[1] + a[3] <= b[1] + b[3]:
                    if a != b or i > j:
                        contained = True
                        break
            if not contained:
                pruned.append(a)
        self.free = pruned


def _sizes(max_size, min_w, min_h):
    sizes = []
    s = 1
    pots = []
    while s <= max_size:
        pots.append(s)
        s *= 2
    for w in pots:
        for h in pots:
            if w >= min_w and h >= min_h:
                sizes.append((w * h, max(w, h), w, h))
    sizes.sort()
    return [(w, h) for _, _, w, h in sizes]


def pack(items, padding=2, border=2, max_size=2048, pot=True):
    """items: list of (key, w, h). Returns (atlas_w, atlas_h, {key: (x, y)}).

    Raises ValueError if the items don't fit in max_size x max_size.
    """
    order = sorted(items, key=lambda it: (-max(it[1], it[2]), -(it[1] * it[2]), str(it[0])))
    need_w = max([w for _, w, _ in items] + [1]) + border * 2
    need_h = max([h for _, _, h in items] + [1]) + border * 2
    area = sum((w + padding) * (h + padding) for _, w, h in items)
    if pot:
        candidates = _sizes(max_size, need_w, need_h)
    else:
        candidates = [(max_size, max_size)]
    for aw, ah in candidates:
        if aw * ah < area:
            continue
        mr = _MaxRects(aw - border * 2 + padding, ah - border * 2 + padding)
        pos = {}
        ok = True
        for key, w, h in order:
            p = mr.insert(w + padding, h + padding)
            if p is None:
                ok = False
                break
            pos[key] = (p[0] + border, p[1] + border)
        if ok:
            if not pot:
                aw = max(x + dict((k, w) for k, w, _ in items)[k] for k, (x, _) in pos.items()) + border
                ah = max(y + dict((k, h) for k, _, h in items)[k] for k, (_, y) in pos.items()) + border
            return aw, ah, pos
    raise ValueError("sprites do not fit into a %dx%d texture" % (max_size, max_size))
