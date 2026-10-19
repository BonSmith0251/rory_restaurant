"""Turns a new, larger picture into an HD replacement of existing sprites.

The new art is trimmed to its content and fitted (keeping its proportions) into the area the original sprite's
content occupied, on a canvas with the original's aspect ratio at up to 2x its logical size (never more pixels
than the new art has). The importer then keeps the layout size from LogicalSizes.json and draws it sharper.

  python Tools/native/fit_hd_art.py new_cheese.png kitchen/cheese.png kitchen/cheese_small.png game/keju.png
  python Tools/native/fit_hd_art.py --portrait new_face.png game/char_1.png make_dessert/char_1.png

--portrait: the art fills the original's height and sits on its bottom edge (customer busts); otherwise it is
centred in the original's content area. Targets are relative to Assets/Rory/Art/Sprites. The new art needs a
transparent background. Afterwards: Rory > Build > All Prefabs and Scenes.
"""
import argparse
import json
import os

from PIL import Image

SPRITES = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..", "Assets", "Rory", "Art", "Sprites")
MAX_SCALE = 2


def trim(im):
    box = im.getchannel("A").point(lambda a: 255 if a > 8 else 0).getbbox()
    return im.crop(box)


def fit(art, rel, portrait=False):
    lw, lh = LOGICAL[rel]
    path = os.path.join(SPRITES, rel)
    old = Image.open(path).convert("RGBA")
    bx0, by0, bx1, by1 = old.getchannel("A").point(lambda a: 255 if a > 8 else 0).getbbox()
    k = lw / old.width  # original pixels -> logical units
    bw, bh = (bx1 - bx0) * k, (by1 - by0) * k
    s = bh / art.height if portrait else min(bw / art.width, bh / art.height)  # art pixels -> units
    scale = max(1.0, min(MAX_SCALE, 1 / s))
    canvas = Image.new("RGBA", (round(lw * scale), round(lh * scale)), (0, 0, 0, 0))
    x = (bx0 * k + (bw - art.width * s) / 2) * scale
    y = ((by1 * k - art.height * s) if portrait else (by0 * k + (bh - art.height * s) / 2)) * scale
    size = (max(1, round(art.width * s * scale)), max(1, round(art.height * s * scale)))
    canvas.alpha_composite(art.resize(size, Image.LANCZOS), (round(x), round(y)))
    canvas.save(path, optimize=True)
    print(f"{rel}: {old.size} -> {canvas.size}")


if __name__ == "__main__":
    p = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    p.add_argument("--portrait", action="store_true")
    p.add_argument("art")
    p.add_argument("targets", nargs="+")
    a = p.parse_args()
    LOGICAL = {i["path"]: (i["w"], i["h"])
               for i in json.load(open(os.path.join(SPRITES, "LogicalSizes.json"), encoding="utf-8"))["items"]}
    art = trim(Image.open(a.art).convert("RGBA"))
    for t in a.targets:
        fit(art, t.replace("\\", "/"), a.portrait)
