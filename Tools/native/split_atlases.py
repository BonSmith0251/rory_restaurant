"""Split Starling/TexturePacker atlases (PNG + XML) into one PNG per sprite.

Trimmed frames (frameX/frameY/frameWidth/frameHeight) are restored to their full frame size with
transparent padding, so every sprite can be placed with its untrimmed size like in the original game.
Usage: python split_atlases.py <atlas_dir> <out_dir>
"""
import os
import sys
import xml.etree.ElementTree as ET
from PIL import Image


def split(atlas_xml, out_root):
    root = ET.parse(atlas_xml).getroot()
    atlas_name = os.path.splitext(os.path.basename(atlas_xml))[0]
    sheet = Image.open(os.path.join(os.path.dirname(atlas_xml), root.get("imagePath"))).convert("RGBA")
    out_dir = os.path.join(out_root, atlas_name)
    os.makedirs(out_dir, exist_ok=True)
    count = 0
    for st in root.iter("SubTexture"):
        x, y, w, h = (int(float(st.get(k))) for k in ("x", "y", "width", "height"))
        if st.get("rotated") == "true":
            raise SystemExit(f"{atlas_xml}: rotated sprites are not supported ({st.get('name')})")
        fx = -int(float(st.get("frameX", 0)))
        fy = -int(float(st.get("frameY", 0)))
        fw = int(float(st.get("frameWidth", w)))
        fh = int(float(st.get("frameHeight", h)))
        frame = Image.new("RGBA", (fw, fh), (0, 0, 0, 0))
        frame.paste(sheet.crop((x, y, x + w, y + h)), (fx, fy))
        frame.save(os.path.join(out_dir, st.get("name") + ".png"), optimize=True)
        count += 1
    return atlas_name, count


if __name__ == "__main__":
    src, dst = sys.argv[1], sys.argv[2]
    for f in sorted(os.listdir(src)):
        if f.endswith(".xml"):
            print(*split(os.path.join(src, f), dst))
