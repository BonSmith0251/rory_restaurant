#!/usr/bin/env bash
# Compile-check the Unity port without Unity: builds stub UnityEngine, then all project scripts.
# Usage: unity_stubs/build.sh [project_assets_dir]   (default: modkit/unity_template/Assets)
HERE="$(cd "$(dirname "$0")" && pwd)"
ASSETS="${1:-$HERE/../unity_template/Assets}"
CSC="/d/BuildTools/MSBuild/Current/Bin/Roslyn/csc.exe"
FW="C:/Windows/Microsoft.NET/Framework64/v4.0.30319"
OUT="${OUT:-$HERE/out}"
mkdir -p "$OUT"
W() { cygpath -w "$1"; }
REFS=(-r:"$FW/System.dll" -r:"$FW/System.Core.dll" -r:"$FW/System.Xml.dll" -r:"$FW/System.Xml.Linq.dll")
"$CSC" -nologo -langversion:7.3 -t:library -out:"$(W "$OUT/UnityEngine.dll")" "${REFS[@]}" -r:"$FW/System.Drawing.dll" "$(W "$HERE/UnityStubs.cs")" "$(W "$HERE/FakeUnity.cs")" > "$OUT/stubs.log" 2>&1 || { cat "$OUT/stubs.log"; exit 1; }
mapfile -t RUNTIME < <(find "$ASSETS" -name '*.cs' -not -path '*/Editor/*' | sort | while read -r f; do cygpath -w "$f"; done)
mapfile -t EDITOR < <(find "$ASSETS" -name '*.cs' -path '*/Editor/*' | sort | while read -r f; do cygpath -w "$f"; done)
"$CSC" -nologo -langversion:7.3 -nowarn:1701,1702,0162,0164,0168,0219,0414,0649,0169,0067,0108 -t:library \
  -out:"$(W "$OUT/Assembly-CSharp.dll")" "${REFS[@]}" -r:"$(W "$OUT/UnityEngine.dll")" "${RUNTIME[@]}" > "$OUT/runtime.log" 2>&1
"$CSC" -nologo -langversion:7.3 -t:library -out:"$(W "$OUT/Assembly-CSharp-Editor.dll")" "${REFS[@]}" \
  -r:"$(W "$OUT/UnityEngine.dll")" -r:"$(W "$OUT/Assembly-CSharp.dll")" "${EDITOR[@]}" > "$OUT/editor.log" 2>&1
echo "runtime: ${#RUNTIME[@]} files, errors: $(grep -c ': error ' "$OUT/runtime.log"), warnings: $(grep -c ': warning ' "$OUT/runtime.log")"
echo "editor:  ${#EDITOR[@]} files, errors: $(grep -c ': error ' "$OUT/editor.log")"
exit 0
