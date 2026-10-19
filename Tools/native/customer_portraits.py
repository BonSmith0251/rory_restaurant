"""Builds the customer sprites from a sheet of 13 portraits so every customer sits at the same place.

The sheet (4 columns, rows of 4/4/4/1, cream background) holds the busts at one scale, cut flat at the bottom of
their tile. Each bust is cut out and drawn at one common scale into one common frame per game, bottom aligned
and centred, so all customers line up (the games anchor them by a corner of the sprite):

  game/char_N          88 x 100  (restaurant rows: bottom-right pivot)
  make_dessert/char_N  138 x 166 (dessert: char_1's old size, top-left pivot; char_13 is not used there)

The frame sizes are written to LogicalSizes.json. Afterwards: Rory > Build > All Prefabs and Scenes.
  python Tools/native/customer_portraits.py <sheet image>
"""
import json
import os
import sys

import numpy as np
from PIL import Image
from scipy import ndimage

SPRITES = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..", "Assets", "Rory", "Art", "Sprites")
FRAMES = {"game": ((88, 100), 13), "make_dessert": ((138, 166), 12)}
SCALE = 2  # pixels per layout unit
ROWS = [(5, 351), (357, 691), (697, 1044), (1047, 1396)]  # tile rows of the sheet (1122 x 1402)


def cut_portrait(cell):
    """Removes the cream/white background around a bust (connected to the top, left or right edge)."""
    rgb = np.asarray(cell.convert("RGB")).astype(int)
    light = (rgb.min(2) > 228) & (rgb.max(2) - rgb.min(2) < 22)
    lab, _ = ndimage.label(light)
    edge = (set(lab[0, :]) | set(lab[:, 0]) | set(lab[:, -1])) - {0}
    bg = np.isin(lab, list(edge))
    bg |= ndimage.binary_dilation(bg) & (rgb.min(2) > 200)  # the light fringe
    alpha = ndimage.gaussian_filter((~bg).astype(float), 0.7)
    alpha[~ndimage.binary_dilation(~bg)] = 0
    lab, n = ndimage.label(alpha > 0.05)  # only the bust itself (tiles overlap the neighbours' hair)
    if n > 1:
        alpha[lab != 1 + int(np.argmax(ndimage.sum(alpha > 0.05, lab, range(1, n + 1))))] = 0
    return Image.fromarray(np.dstack([rgb, (alpha * 255).round()]).astype(np.uint8), "RGBA")


def busts(sheet):
    """(bust, centre x) per customer; the bust is cut to its content, its bottom is the tile's bottom."""
    w = sheet.width / 4
    out = []
    for r, (y0, y1) in enumerate(ROWS):
        cells = [(346, 721)] if r == 3 else [(int(c * w) + 2, int((c + 1) * w) - 2) for c in range(4)]
        for x0, x1 in cells:
            b = cut_portrait(sheet.crop((x0, y0, x1, y1)))
            box = b.getchannel("A").point(lambda a: 255 if a > 8 else 0).getbbox()
            centre = (x1 - x0) / 2 - box[0]
            out.append((b.crop(box), centre))
    return out


def main(sheet_path):
    faces = busts(Image.open(sheet_path).convert("RGB"))
    tallest = max(b.height for b, _ in faces)
    lpath = os.path.join(SPRITES, "LogicalSizes.json")
    data = json.load(open(lpath, encoding="utf-8"))
    sizes = {i["path"]: i for i in data["items"]}
    for group, ((fw, fh), count) in FRAMES.items():
        s = fh / tallest * SCALE  # sheet pixels -> sprite pixels
        for n, (bust, centre) in enumerate(faces[:count], 1):
            canvas = Image.new("RGBA", (fw * SCALE, fh * SCALE), (0, 0, 0, 0))
            img = bust.resize((round(bust.width * s), round(bust.height * s)), Image.LANCZOS)
            x = round(canvas.width / 2 - centre * s)  # a bust wider than the frame is cut at the sides
            img = img.crop((max(0, -x), 0, min(img.width, canvas.width - x), img.height))
            canvas.alpha_composite(img, (max(0, x), canvas.height - img.height))
            rel = f"{group}/char_{n}.png"
            canvas.save(os.path.join(SPRITES, rel), optimize=True)
            sizes[rel].update(w=fw, h=fh)
            print(f"{rel}: {fw}x{fh} units, {canvas.size} px")
    with open(lpath, "w", encoding="utf-8", newline="\n") as f:
        json.dump(data, f, indent=1)


if __name__ == "__main__":
    main(sys.argv[1])
