"""Command line interface:  python rrtool.py <command> [options]"""
import argparse
import json
import os
import sys
import time

from . import __version__, DEFAULT_KEY
from .workspace import Workspace, restore, DEFAULT_WS, DEFAULT_BUILD, DEFAULT_GAME_DIR, DEFAULT_BACKUP


def _ws(args, need=True):
    ws = Workspace(args.workspace)
    if need and not ws.exists():
        sys.exit("No workspace at %s - run 'extract' first." % ws.path)
    return ws.load() if ws.exists() else ws


def cmd_extract(a):
    t = time.time()
    Workspace(a.workspace).extract(a.game, slice_sprites=not a.no_slice, force=a.force, key=a.key,
                                   log=print if a.verbose else (lambda m: None if m.startswith("  ") else print(m)))
    print("done in %.1fs" % (time.time() - t))


def cmd_analyze(a):
    from .analyze import analyze, format_text, GameView, WorkspaceView
    view = WorkspaceView(_ws(a)) if a.workspace_view else GameView(a.game)
    rep = analyze(view, key=a.key, decode_images=not a.fast)
    if a.json:
        with open(a.json, "w", encoding="utf-8") as f:
            json.dump(rep, f, indent=1, ensure_ascii=False)
        print("JSON report written to %s" % a.json)
    print(format_text(rep))
    return 1 if rep["summary"].get("error") else 0


def cmd_status(a):
    ws = _ws(a)
    n = 0
    for rel in sorted(ws.m["files"]):
        st = ws.file_status(rel)
        if st != "clean":
            print("  %-9s %s" % (st, rel))
            n += 1
    for x, at in sorted(ws.m["atlases"].items()):
        for name in sorted(at["sprites"]):
            st = ws.sprite_status(x, name)
            if st != "clean":
                print("  %-9s sprite %s :: %s" % (st, os.path.basename(x), name))
                n += 1
        for name, _ in ws.new_sprite_files(x):
            print("  %-9s sprite %s :: %s" % ("new", os.path.basename(x), name))
            n += 1
    for s, sw in sorted(ws.m["swfs"].items()):
        for cid, asset in sorted(sw["assets"].items(), key=lambda kv: int(kv[0])):
            st = ws.swf_asset_status(s, cid)
            if st != "clean":
                print("  %-9s %s#%s %s" % (st, s, cid, asset["file"]))
                n += 1
    print("%d change(s)" % n if n else "No changes.")
    inst = ws.backup().installed()
    if inst:
        print("Installed in game: %s" % ", ".join(inst))


def cmd_build(a):
    ws = _ws(a)
    outputs, errors = ws.build(out_dir=a.out, force_all=a.all)
    return 1 if errors else 0


def cmd_install(a):
    ws = _ws(a)
    try:
        ws.install(force_all=a.all)
    except RuntimeError as e:
        print(e)
        return 1


def cmd_restore(a):
    restore(a.game, a.backup)


def cmd_sync(a):
    ws = _ws(a)
    if a.repack:
        ws.sync_atlas(_atlas_rel(ws, a.repack), force_repack=True)
    elif not ws.sync_all():
        print("All atlases up to date.")


def _atlas_rel(ws, name):
    for x in ws.m["atlases"]:
        if x == name or os.path.basename(x) in (name, name + ".xml"):
            return x
    sys.exit("unknown atlas %s (choose from: %s)" % (name, ", ".join(os.path.basename(x)[:-4] for x in ws.m["atlases"])))


def cmd_revert(a):
    ws = _ws(a)
    if a.sprite:
        ws.revert_sprite(_atlas_rel(ws, a.target), a.sprite)
    elif "#" in a.target:
        s, _, cid = a.target.rpartition("#")
        ws.revert_swf_asset(s, cid)
    elif a.target in ws.m["files"]:
        ws.revert_file(a.target)
    else:
        ws.revert_file(_atlas_rel(ws, a.target))
    print("reverted")


def cmd_gui(a):
    from .server import serve
    ws = Workspace(a.workspace)
    if not ws.exists():
        print("No workspace yet - extracting game assets first...")
        ws.extract(a.game, log=lambda m: None if m.startswith("  ") else print(m))
    serve(ws.load(), port=a.port, open_browser=not a.no_browser)


def cmd_decode(a):
    from .formats import decode_ean, save_image_file
    with open(a.input, "rb") as f:
        img = decode_ean(f.read(), a.key or DEFAULT_KEY)
    out = a.output or os.path.splitext(a.input)[0] + ".png"
    save_image_file(img, out)
    print("%s -> %s (%dx%d)" % (a.input, out, img.w, img.h))


def cmd_encode(a):
    from .formats import encode_ean, load_image_file
    img = load_image_file(a.input)
    out = a.output or os.path.splitext(a.input)[0] + ".ean"
    with open(out, "wb") as f:
        f.write(encode_ean(img, a.key or DEFAULT_KEY))
    print("%s -> %s (%dx%d)" % (a.input, out, img.w, img.h))


def cmd_pack(a):
    from .formats import load_image_file, save_image_file, write_atlas, encode_ean, Image
    from .packer import pack
    files = sorted(f for f in os.listdir(a.folder) if f.lower().endswith(".png"))
    imgs = {os.path.splitext(f)[0]: load_image_file(os.path.join(a.folder, f)) for f in files}
    aw, ah, pos = pack([(n, i.w, i.h) for n, i in imgs.items()], padding=a.padding, max_size=a.max_size)
    atlas = Image(aw, ah)
    sprites = []
    for n in sorted(imgs):
        x, y = pos[n]
        atlas.paste(imgs[n], x, y)
        sprites.append(dict(name=n, x=x, y=y, width=imgs[n].w, height=imgs[n].h))
    base = a.output
    stem = os.path.basename(base)
    with open(base + ".xml", "wb") as f:
        f.write(write_atlas({"imagePath": stem + ".png"}, sprites))
    if a.png:
        save_image_file(atlas, base + ".png")
    else:
        with open(base + ".ean", "wb") as f:
            f.write(encode_ean(atlas, a.key or DEFAULT_KEY))
    print("packed %d sprites into %dx%d -> %s.%s + %s.xml" % (len(sprites), aw, ah, base, "png" if a.png else "ean", base))


def cmd_swf(a):
    from .swf import Swf, load_abc, asset_references, detect_ean_key, TAG_NAMES
    swf = Swf.load(a.swf)
    if a.disasm:
        for abc in load_abc(swf):
            for mi, m in abc.find_methods(a.disasm):
                print("// %s" % m["qual"])
                print(abc.disassemble(mi))
                print()
        return
    if a.classes:
        for abc in load_abc(swf):
            for c in sorted(abc.classes, key=lambda c: c["fullname"]):
                print("%s%s" % (c["fullname"], " extends " + c["super"] if c["super"] else ""))
        return
    if a.strings is not None:
        for abc in load_abc(swf):
            for s in sorted({s for s in abc.strs if s and a.strings.lower() in s.lower()}):
                print(repr(s))
        return
    print("SWF version %d, %s" % (swf.version, swf.frame_info()))
    key, src = detect_ean_key(swf)
    print(".ean key: %r (%s)" % (key, src))
    print("Tags:")
    for n, (c, s) in sorted(swf.tag_stats().items()):
        print("  %-30s %4d  %9d bytes" % (n, c, s))
    print("Embedded assets:")
    for x in swf.assets():
        print("  #%-4d %-20s %-40s %s" % (x["id"], x["tag_name"], x["symbol"], "%dx%d" % (x["w"], x["h"]) if "w" in x else x["ext"]))
    print("Asset references in code:")
    for s in asset_references(swf):
        print("  " + s)


def cmd_unity(a):
    from .unity_export import export
    ws = Workspace(a.workspace)
    if not ws.exists():
        print("No workspace yet - extracting game assets first...")
        ws.extract(a.game, log=lambda m: None if m.startswith("  ") else print(m))
    out = a.out or os.path.join(os.path.expanduser("~"), "Documents", "RorysRestaurantUnity")
    export(ws.load(), out)
    print("Open the folder with Unity Hub (Add project from disk), Unity 2021.3 LTS or newer; then press Play.")


def cmd_decompile(a):
    from .swf import Swf, load_abc
    from .decompile import decompile_all
    swf = Swf.load(a.swf)
    for abc in load_abc(swf):
        decompile_all(abc, a.out, include_libs=a.all)


def main(argv=None):
    p = argparse.ArgumentParser(prog="rrtool", description="RRModKit %s - Rory's Restaurant Deluxe asset toolkit" % __version__)
    p.add_argument("--game", default=DEFAULT_GAME_DIR, help="game folder (default: %(default)s)")
    p.add_argument("--workspace", "-w", default=DEFAULT_WS, help="workspace folder (default: %(default)s)")
    p.add_argument("--key", default=None, help="override the .ean RC4 key (auto-detected from the SWF)")
    sub = p.add_subparsers(dest="cmd")

    s = sub.add_parser("extract", help="decrypt/extract all assets into an editable workspace")
    s.add_argument("--force", action="store_true", help="overwrite an existing workspace (loses edits!)")
    s.add_argument("--no-slice", action="store_true", help="don't slice atlases into individual sprite PNGs")
    s.add_argument("-v", "--verbose", action="store_true")
    s.set_defaults(fn=cmd_extract)

    s = sub.add_parser("analyze", help="analyze assets and validate cross references")
    s.add_argument("--workspace-view", "-W", action="store_true", help="analyze the workspace (your edits) instead of the game")
    s.add_argument("--json", help="also write a JSON report")
    s.add_argument("--fast", action="store_true", help="skip decoding textures")
    s.set_defaults(fn=cmd_analyze)

    s = sub.add_parser("status", help="list modified workspace files and sprites")
    s.set_defaults(fn=cmd_status)

    s = sub.add_parser("sync", help="apply edited sprite PNGs to their atlases (done automatically by build)")
    s.add_argument("--repack", metavar="ATLAS", help="force a full repack of one atlas")
    s.set_defaults(fn=cmd_sync)

    s = sub.add_parser("build", help="rebuild changed assets into game format")
    s.add_argument("--out", default=DEFAULT_BUILD, help="output folder (default: %(default)s)")
    s.add_argument("--all", action="store_true", help="rebuild every file, not just changed ones")
    s.set_defaults(fn=cmd_build)

    s = sub.add_parser("install", help="build and copy changed files into the game (backs up originals)")
    s.add_argument("--all", action="store_true")
    s.set_defaults(fn=cmd_install)

    s = sub.add_parser("restore", help="restore all original game files from the backup")
    s.add_argument("--backup", default=DEFAULT_BACKUP)
    s.set_defaults(fn=cmd_restore)

    s = sub.add_parser("revert", help="revert a workspace file, atlas, sprite or SWF asset to the original")
    s.add_argument("target", help="game path (data/Dish.xml), atlas name (kitchen) or swf asset (RorysRestaurant.swf#22)")
    s.add_argument("--sprite", help="sprite name inside the atlas")
    s.set_defaults(fn=cmd_revert)

    s = sub.add_parser("gui", help="open the web editor (extracts first if needed)")
    s.add_argument("--port", type=int, default=8765)
    s.add_argument("--no-browser", action="store_true")
    s.set_defaults(fn=cmd_gui)

    s = sub.add_parser("decode", help="decode a single .ean file to PNG")
    s.add_argument("input")
    s.add_argument("output", nargs="?")
    s.set_defaults(fn=cmd_decode)

    s = sub.add_parser("encode", help="encode a PNG into a .ean file")
    s.add_argument("input")
    s.add_argument("output", nargs="?")
    s.set_defaults(fn=cmd_encode)

    s = sub.add_parser("pack", help="pack a folder of PNGs into a new atlas (.ean/.png + Starling .xml)")
    s.add_argument("folder")
    s.add_argument("output", help="output path without extension")
    s.add_argument("--png", action="store_true", help="write a PNG texture instead of .ean")
    s.add_argument("--padding", type=int, default=2)
    s.add_argument("--max-size", type=int, default=2048)
    s.set_defaults(fn=cmd_pack)

    s = sub.add_parser("swf", help="inspect the game SWF (tags, embedded assets, classes, disassembly)")
    s.add_argument("swf", nargs="?", default=os.path.join(DEFAULT_GAME_DIR, "RorysRestaurant.swf"))
    s.add_argument("--classes", action="store_true", help="list classes")
    s.add_argument("--disasm", metavar="METHOD", help="disassemble methods whose name contains METHOD")
    s.add_argument("--strings", metavar="TEXT", help="search string constants")
    s.set_defaults(fn=cmd_swf)

    s = sub.add_parser("unity", help="generate a Unity project (C# port + converted assets from the workspace)")
    s.add_argument("--out", help="project folder (default: ~/Documents/RorysRestaurantUnity)")
    s.set_defaults(fn=cmd_unity)

    s = sub.add_parser("decompile", help="decompile the game's ActionScript into readable pseudo-AS3")
    s.add_argument("swf", nargs="?", default=os.path.join(DEFAULT_GAME_DIR, "RorysRestaurant.swf"))
    s.add_argument("--out", default=os.path.join(os.path.dirname(DEFAULT_WS), "decompiled"))
    s.add_argument("--all", action="store_true", help="include library classes (Starling, TweenLite, ...)")
    s.set_defaults(fn=cmd_decompile)

    a = p.parse_args(argv)
    if not getattr(a, "fn", None):
        p.print_help()
        return 0
    return a.fn(a) or 0
