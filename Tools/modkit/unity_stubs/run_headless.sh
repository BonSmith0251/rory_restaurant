#!/usr/bin/env bash
# Builds the port against FakeUnity and runs a scripted headless session.
# Usage: unity_stubs/run_headless.sh <script> [resources_dir] [out_dir]
#   resources_dir defaults to a fresh export in unity_stubs/out_run/project/Assets/Resources
HERE="$(cd "$(dirname "$0")" && pwd)"
MODKIT="$HERE/.."
SCRIPT="$1"
OUTRUN="$HERE/out_run"
RES="${2:-$OUTRUN/project/Assets/Resources}"
SHOTS="${3:-$OUTRUN/shots}"
CSC="/d/BuildTools/MSBuild/Current/Bin/Roslyn/csc.exe"
FW="C:/Windows/Microsoft.NET/Framework64/v4.0.30319"
W() { cygpath -w "$1"; }
set -e
if [ -z "$2" ] && [ ! -d "$RES/RR" ]; then
  (cd "$MODKIT" && python -I rrtool.py unity --out "$(W "$OUTRUN/project")" >/dev/null)
fi
OUT="$OUTRUN" bash "$HERE/build.sh" >/dev/null || { echo "STUB BUILD FAILED"; cat "$OUTRUN/stubs.log"; exit 1; }
if grep -q ": error" "$OUTRUN/runtime.log"; then grep ": error" "$OUTRUN/runtime.log" | head -30; echo "BUILD FAILED"; exit 1; fi
"$CSC" -nologo -langversion:9.0 -t:exe -out:"$(W "$OUTRUN/Harness.exe")" \
  -r:"$FW/System.dll" -r:"$FW/System.Core.dll" -r:"$FW/System.Xml.dll" -r:"$FW/System.Xml.Linq.dll" -r:"$FW/System.Drawing.dll" \
  -r:"$(W "$OUTRUN/UnityEngine.dll")" -r:"$(W "$OUTRUN/Assembly-CSharp.dll")" "$(W "$HERE/Harness.cs")"
rm -rf "$SHOTS"
set +e
"$OUTRUN/Harness.exe" "$(W "$RES")" "$(W "$SHOTS")" "$(W "$SCRIPT")"
