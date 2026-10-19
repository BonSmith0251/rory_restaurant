"""Records the layout (logical) size of every sprite in Assets/Rory/Art/Sprites into LogicalSizes.json.

The layout of the game uses the original art's pixel sizes. When an image is later replaced by a higher-resolution
version with the same name, the importer compares its pixel size with the recorded logical size and sets the
sprite's pixels-per-unit so it keeps its layout size (see RoryAssetPostprocessor).

Run this only for NEW sprites (it never changes sizes already recorded, so HD replacements keep their original
logical size): python Tools/native/record_sprite_sizes.py
"""
import json
import os
from PIL import Image

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..", "Assets", "Rory", "Art", "Sprites")
OUT = os.path.join(ROOT, "LogicalSizes.json")

existing = {}
if os.path.exists(OUT):
    for item in json.load(open(OUT, encoding="utf-8"))["items"]:
        existing[item["path"]] = item

items = []
added = 0
for group in sorted(os.listdir(ROOT)):
    folder = os.path.join(ROOT, group)
    if not os.path.isdir(folder):
        continue
    for name in sorted(os.listdir(folder)):
        if not name.lower().endswith(".png"):
            continue
        path = f"{group}/{name}"
        if path in existing:
            items.append(existing[path])
            continue
        w, h = Image.open(os.path.join(folder, name)).size
        items.append({"path": path, "w": w, "h": h})
        added += 1

with open(OUT, "w", encoding="utf-8", newline="\n") as f:
    json.dump({"items": items}, f, indent=1)
print(f"{len(items)} sprites ({added} new) -> {OUT}")
