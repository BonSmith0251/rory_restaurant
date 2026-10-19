# RRModKit: Rory's Restaurant Deluxe asset toolkit

Extract, analyze, edit and rebuild every asset of *Rory's Restaurant Deluxe* (Adobe AIR / Starling).
It needs only Python 3.8+ and the standard library. numpy is used for speed if it is installed.

## Quick start

Double-click **`RRModKit Editor.bat`**, or run:

```
python rrtool.py gui
```

On the first run, all assets are decrypted into `modkit/workspace/` (about 6 seconds). The editor then opens in your browser:

- **Sprite atlases**: view every texture with sprite boxes. Click a sprite to export it or replace it (or drag a PNG onto it).
  - Same-size images are painted into place.
  - Different sizes or new sprites trigger an automatic repack (MaxRects, power-of-two textures, max 2048 px). The XML is rewritten to match.
- **Game data** (levels, dishes, customers, dialogue, dessert items, hidden-object layouts): open as editable tables.
  - Edits are written *in place*, so comments, formatting and line endings are preserved.
  - Rows can be duplicated or deleted. A raw source editor with XML validation is also available.
- **Backgrounds, sounds, icons**: preview or play, download, replace, revert.
- **Embedded in RorysRestaurant.swf**: the bitmap fonts actually used by the game (glyph viewer), particle configs, and the settings XML.
- **Analyze**: validates the workspace or the installed game. It checks broken sprite references, sound list vs files, atlas bounds and overlaps, and numeric level data.
- **Code**: browse all 201 ActionScript classes and disassemble any method.
- **Build** writes converted game files to `modkit/build/`.
- **Install to game** copies them into the game. Originals are backed up first to `modkit/backup/original/`.
- **Restore originals** undoes every install. **▶ Play** starts the game.

You can also edit the workspace files directly with any editor (Photoshop, GIMP, Audacity, a text editor). `build`/`install` detects changed files by hash.

## Command line

```
python rrtool.py extract [--force] [--no-slice]   # decrypt everything into workspace/
python rrtool.py analyze [-W] [--json report.json] # analyze game (or -W: workspace)
python rrtool.py status                            # list what you changed
python rrtool.py build [--all] [--out DIR]         # rebuild changed assets -> build/
python rrtool.py install                           # build + copy into game (with backup)
python rrtool.py restore                           # put all original files back
python rrtool.py revert data/Dish.xml              # revert a file / atlas / sprite / swf asset
python rrtool.py revert kitchen --sprite bacon
python rrtool.py sync [--repack kitchen]           # apply edited sprite PNGs to atlases
python rrtool.py decode file.ean [out.png]         # single-file conversion
python rrtool.py encode in.png [out.ean]
python rrtool.py pack <png folder> <out> [--png]   # build a new Starling atlas
python rrtool.py swf [--classes|--disasm NAME|--strings TEXT]
python rrtool.py gui [--port 8765]
```

Global options: `--game DIR`, `--workspace DIR`, `--key KEY`.

## Unity port

```
python rrtool.py unity [--out DIR]     # default: ~/Documents/RorysRestaurantUnity
python rrtool.py decompile             # readable pseudo-AS3 of the game code -> modkit/decompiled/
```

`unity` generates a complete Unity project (2021.3 LTS or newer) containing:
- the game logic ported class-by-class to C# (`unity_template/Assets/Game`);
- **Starlite**, a Starling/Flash/TweenLite compatibility layer on Unity UI (`unity_template/Assets/Starlite`);
- the workspace assets under `Assets/Resources/RR`, so your mods are included.

See the generated project's `README.md`. `unity_template/PORTING.md` documents the AS3→C# conventions.

**Testing without Unity** (`unity_stubs/`):
- `build.sh` compile-checks all C# against a functional stand-in for UnityEngine ("FakeUnity").
- `run_headless.sh <script>` boots the real game on FakeUnity, plays a scripted session and saves screenshots
  from a software renderer to `unity_stubs/out_run/shots/`.
- Example scripts are in `unity_stubs/tests/`, and `tests/gen_days.py` generates multi-day runs.
- Script commands: `sec`, `click`, `drag`, `clicktext`, `type`, `key`, `shot`, `tree`, `autoswap`, `hofind`, `day N`, `goto <screen>`.

## Workspace layout

| Path | Contents |
|---|---|
| `files/<game path>` | every game file. `.ean` becomes `.png` (decrypted RGBA). `.eag` becomes `.jpg`. Everything else is copied as-is |
| `sprites/<atlas>/<name>.png` | every atlas sprite sliced out. Edit these, or drop new PNGs here to *add* sprites |
| `swf/RorysRestaurant/<id>_<symbol>.<ext>` | assets embedded in the SWF (fonts `.fnt` + `.png`, particles `.pex`, settings) |
| `manifest.json` | hashes and mappings used for change detection |

For a background (`.eag`), you can put a `.png` next to the `.jpg` with the same name. It is used instead, which gives lossless output or transparency.

## File formats (reverse engineered)

| Format | Layout |
|---|---|
| `.ean` | `RC4(key="crazy_face")` → zlib → `[u32 BE width][width×height × ARGB, BE, straight alpha]`. Height = (len−4)/4/width. Decoded by `TGlobalClass.readByteArrayToBitmap` → `BitmapData.setPixels`. The key is auto-detected from the SWF bytecode. |
| `.eag` | plain JPEG. It is loaded with `flash.display.Loader`, so PNG/GIF bytes also work. |
| atlas `.xml` | Starling/TexturePacker `TextureAtlas`. Some sprites are trimmed (`frameX/Y/Width/Height`). |
| `*_data.xml` | hidden-object placement (`item name locX locY`). |
| SWF | the game embeds its own bitmap fonts (DefineBinaryData + DefineBitsLossless2), particle configs and settings. The loose copies in `data/fonts`, `data/graphic/particle`, `data/settings.xml` and `data/graphic/logo.swf` are **never loaded**. Edit the `swf/` versions instead. |

Some original XML files contain `--` inside comments. Flash accepts this, strict XML parsers don't. The toolkit handles it the way Flash does.

## Notes

- Install and restore write into `C:\Program Files (x86)\…`, so they need an administrator account. The AIR runtime does not verify the package signature, so modified files load normally.
- Save games live in `%APPDATA%\RorysRestaurant\Local Store\` and are not touched.
- Keep textures at or below 2048×2048 (the Stage3D baseline limit). The repacker warns if an atlas needs 4096.
- `extract --force` recreates the workspace and **discards your edits**. Back up `workspace/` first if needed.
