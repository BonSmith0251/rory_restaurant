"""Fits a redrawn HD background onto the original's layout, so items and buttons placed on it stay in place.

Redrawn art is often the old scene scaled and cropped to a new shape (e.g. 1024x600 enlarged to 1672x941, losing
the bottom). This measures how the new picture maps onto the old one (scale and offset per axis, from matching
edges in a grid of patches), resamples it into the old frame at the new art's own resolution, and fills any part
the new art does not cover from the old art (colour matched, blended). The result replaces the original file.

  python Tools/native/fit_hd_background.py <new image> Assets/Rory/Art/Backgrounds/HO1_A_BG.jpg [...pairs]

Afterwards: Rory > Build > All Prefabs and Scenes.
"""
import sys

import numpy as np
from PIL import Image, ImageFilter


def edges(im):
    return np.asarray(im.convert("L").filter(ImageFilter.FIND_EDGES).filter(ImageFilter.GaussianBlur(1.2))).astype(float)


def measure(old, new):
    """x_s = ax*x + bx, y_s = ay*y + by: where an old pixel lies in the new picture squeezed to the old size."""
    w, h = old.size
    eo, en = edges(old), edges(new.resize((w, h), Image.LANCZOS))
    P, R = 48, 28
    pts = []
    for py in range(R + 10, h - P - R - 10, 40):
        for px in range(R + 10, w - P - R - 10, 48):
            a = eo[py:py + P, px:px + P]
            a = a - a.mean()
            na = np.sqrt((a * a).sum())
            if na < 200:
                continue  # flat patch: nothing to match

            def score(dx, dy):
                b = en[py + dy:py + dy + P, px + dx:px + dx + P]
                b = b - b.mean()
                return (a * b).sum() / (na * np.sqrt((b * b).sum()) + 1e-9)

            c, dx, dy = max((score(dx, dy), dx, dy) for dy in range(-R, R + 1, 2) for dx in range(-R, R + 1, 2))
            c, dx, dy = max((score(dx + i, dy + j), dx + i, dy + j) for j in (-1, 0, 1) for i in (-1, 0, 1))
            if c > 0.6:
                pts.append((px + P / 2, py + P / 2, px + P / 2 + dx, py + P / 2 + dy))
    p = np.array(pts)
    keep = np.ones(len(p), bool)
    for _ in range(4):  # least squares, dropping outliers
        ax, bx = np.linalg.lstsq(np.c_[p[keep, 0], np.ones(keep.sum())], p[keep, 2], rcond=None)[0]
        ay, by = np.linalg.lstsq(np.c_[p[keep, 1], np.ones(keep.sum())], p[keep, 3], rcond=None)[0]
        keep = (np.abs(p[:, 2] - (ax * p[:, 0] + bx)) < 3) & (np.abs(p[:, 3] - (ay * p[:, 1] + by)) < 3)
    return ax, bx, ay, by, keep.sum(), len(p)


def fit(new_path, old_path):
    old = Image.open(old_path).convert("RGB")
    new = Image.open(new_path).convert("RGB")
    w, h = old.size
    W, H = new.size
    ax, bx, ay, by, good, total = measure(old, new)
    # output size: the new art's resolution, snapped to exactly the old shape (1024x600 -> multiples of 128x75)
    k = np.gcd(w, h)
    n = max(1, round(h * W / w / ax / (h // k)))
    ow, oh = n * (w // k), n * (h // k)
    S = ow / w  # output pixels per old pixel
    coeffs = (ax * W / (w * S), 0, bx * W / w, 0, ay * H / (h * S), by * H / h)
    out = np.asarray(new.transform((ow, oh), Image.AFFINE, coeffs, Image.BICUBIC)).astype(float)
    big = np.asarray(old.resize((ow, oh), Image.LANCZOS)).astype(float)
    # rows/columns the new art does not reach: old art, colour matched to the new, blended over 24 px
    ys = np.arange(oh) * coeffs[4] + coeffs[5]
    xs = np.arange(ow) * coeffs[0] + coeffs[2]
    inside = ((ys >= 0) & (ys <= H - 1))[:, None] & ((xs >= 0) & (xs <= W - 1))[None, :]
    if not inside.all():
        from scipy import ndimage
        dist = ndimage.distance_transform_edt(inside)
        ring = (dist > 0) & (dist < 60)
        m_n, s_n = out[ring].mean(0), out[ring].std(0) + 1e-6
        m_o, s_o = big[ring].mean(0), big[ring].std(0) + 1e-6
        fill = (big - m_o) / s_o * s_n + m_n
        wgt = np.clip(1 - dist / 24, 0, 1)[:, :, None]
        out = out * (1 - wgt) + fill * wgt
    Image.fromarray(np.clip(out, 0, 255).round().astype(np.uint8)).save(old_path, quality=95)
    print(f"{old_path}: {ow}x{oh} (x' = {ax:.4f}x{bx:+.1f}, y' = {ay:.4f}y{by:+.1f}, {good}/{total} patches); "
          f"filled from the old art: {100 * (1 - inside.mean()):.1f}%")


if __name__ == "__main__":
    args = sys.argv[1:]
    if not args or len(args) % 2:
        sys.exit(__doc__)
    for i in range(0, len(args), 2):
        fit(args[i], args[i + 1])
