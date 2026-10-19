"""Generate a Unity project: C# port (unity_template) + game assets from the workspace."""
import json
import os
import shutil

from .workspace import MODKIT_DIR

TEMPLATE_DIR = os.path.join(MODKIT_DIR, "unity_template")
MARKER = "rrmodkit-unity.json"

PACKAGES = {
    "com.unity.ugui": "1.0.0",
    "com.unity.modules.ai": "1.0.0",
    "com.unity.modules.animation": "1.0.0",
    "com.unity.modules.assetbundle": "1.0.0",
    "com.unity.modules.audio": "1.0.0",
    "com.unity.modules.imageconversion": "1.0.0",
    "com.unity.modules.imgui": "1.0.0",
    "com.unity.modules.jsonserialize": "1.0.0",
    "com.unity.modules.particlesystem": "1.0.0",
    "com.unity.modules.physics": "1.0.0",
    "com.unity.modules.physics2d": "1.0.0",
    "com.unity.modules.screencapture": "1.0.0",
    "com.unity.modules.ui": "1.0.0",
    "com.unity.modules.uielements": "1.0.0",
    "com.unity.modules.unitywebrequest": "1.0.0",
    "com.unity.modules.unitywebrequestaudio": "1.0.0",
    "com.unity.modules.unitywebrequesttexture": "1.0.0",
    "com.unity.modules.video": "1.0.0",
}

GITIGNORE = """/[Ll]ibrary/
/[Tt]emp/
/[Oo]bj/
/[Bb]uild/
/[Bb]uilds/
/[Ll]ogs/
/[Uu]ser[Ss]ettings/
/[Mm]emoryCaptures/
*.csproj
*.sln
*.user
*.pidb
*.booproj
*.unityproj
.vs/
"""

LINK_XML = """<linker>
  <!-- TweenLite/Starlite tween game fields by name via reflection: keep the game assembly intact -->
  <assembly fullname="Assembly-CSharp" preserve="all"/>
</linker>
"""


def _copy(src, dst):
    os.makedirs(os.path.dirname(dst), exist_ok=True)
    shutil.copyfile(src, dst)


def _copytree(src, dst):
    for root, dirs, files in os.walk(src):
        rel = os.path.relpath(root, src)
        for f in files:
            if f.endswith(".meta") or f.startswith(".setup_done"):
                continue
            _copy(os.path.join(root, f), os.path.join(dst, rel, f))


def export(ws, out_dir, log=print):
    """ws: loaded Workspace. Returns number of asset files written."""
    out_dir = os.path.abspath(out_dir)
    if os.path.isdir(out_dir) and os.listdir(out_dir) and not os.path.exists(os.path.join(out_dir, MARKER)):
        raise RuntimeError("%s exists and is not a generated RRModKit Unity project" % out_dir)
    os.makedirs(out_dir, exist_ok=True)
    m = ws.m

    # 1) scripts, editor tooling, docs (regenerated every time; Library/ and user settings are untouched)
    for sub in ("Assets/Starlite", "Assets/Game"):
        src_dir = os.path.join(TEMPLATE_DIR, sub)
        dst = os.path.join(out_dir, sub)
        keep = set()
        for root, _, files in os.walk(src_dir):
            keep |= {os.path.normcase(os.path.join(dst, os.path.relpath(root, src_dir), f)) for f in files}
        if os.path.isdir(dst):  # drop scripts that no longer exist in the template (and their .meta)
            for root, _, files in os.walk(dst):
                for f in files:
                    full = os.path.join(root, f)
                    if f.endswith((".cs", ".shader")) and os.path.normcase(full) not in keep:
                        os.remove(full)
                        if os.path.exists(full + ".meta"):
                            os.remove(full + ".meta")
        _copytree(src_dir, dst)
    _copy(os.path.join(TEMPLATE_DIR, "PORTING.md"), os.path.join(out_dir, "PORTING.md"))
    with open(os.path.join(out_dir, "Assets", "link.xml"), "w", encoding="utf-8") as f:
        f.write(LINK_XML)
    os.makedirs(os.path.join(out_dir, "Packages"), exist_ok=True)
    manifest_path = os.path.join(out_dir, "Packages", "manifest.json")
    deps = dict(PACKAGES)
    if os.path.exists(manifest_path):  # keep packages the user added
        try:
            with open(manifest_path, encoding="utf-8") as f:
                deps.update(json.load(f).get("dependencies", {}))
        except Exception:
            pass
    with open(manifest_path, "w", encoding="utf-8") as f:
        json.dump({"dependencies": dict(sorted(deps.items()))}, f, indent=2)
    with open(os.path.join(out_dir, ".gitignore"), "w", encoding="utf-8") as f:
        f.write(GITIGNORE)

    # 2) game assets -> Assets/Resources/RR (mirrors the game's "data/" folder)
    res = os.path.join(out_dir, "Assets", "Resources", "RR")
    n = 0
    for rel, f in sorted(m["files"].items()):
        if rel.startswith("branding/"):  # splash screens shown by dumLoading
            _copy(ws.abspath(f["work"]), os.path.join(res, rel.replace("/", os.sep)))
            n += 1
            continue
        if not rel.startswith("data/"):
            continue
        sub = rel[len("data/"):]
        if sub.startswith("graphic/particle/") or sub == "settings.xml" or sub.endswith(".swf"):
            continue  # loose copies the game never loads (the SWF embeds its own)
        if sub.startswith("fonts/") and not sub.lower().endswith(".ttf"):
            continue
        src = ws.override_path(rel) or f["work"]
        base, _ = os.path.splitext(sub)
        ext = os.path.splitext(src)[1].lower()
        if f["kind"] in ("ean", "eag"):
            dst = base + ext  # decoded PNG / JPEG (or PNG override)
        else:
            dst = sub
        _copy(ws.abspath(src), os.path.join(res, dst.replace("/", os.sep)))
        n += 1

    # 3) assets that were embedded in the SWFs -> Resources/RR/embedded/<Symbol>
    for swf_rel, s in sorted(m["swfs"].items()):
        prefix = "" if swf_rel == "RorysRestaurant.swf" else os.path.splitext(os.path.basename(swf_rel))[0] + "_"
        for cid, a in sorted(s["assets"].items(), key=lambda kv: int(kv[0])):
            name = a["symbol"] or "char%s" % cid
            ext = a["ext"]
            if ext in ("fnt", "pex", "xml"):
                ext = "xml"
            _copy(ws.abspath(a["file"]), os.path.join(res, "embedded", "%s%s.%s" % (prefix, name, ext)))
            n += 1

    with open(os.path.join(out_dir, MARKER), "w", encoding="utf-8") as f:
        json.dump(dict(generator="RRModKit", workspace=ws.path, assets=n), f, indent=1)
    decomp = os.path.join(MODKIT_DIR, "decompiled")
    if os.path.isdir(decomp):  # original logic for reference (outside Assets, so Unity ignores it)
        ref = os.path.join(out_dir, "Reference", "decompiled-as3")
        if os.path.isdir(ref):
            shutil.rmtree(ref)
        _copytree(decomp, ref)
    readme_src = os.path.join(TEMPLATE_DIR, "README.md")
    if os.path.exists(readme_src):
        _copy(readme_src, os.path.join(out_dir, "README.md"))
    log("Unity project written to %s (%d asset files)" % (out_dir, n))
    return n
