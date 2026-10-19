@echo off
rem Opens the RRModKit web editor (extracts the game assets on first run).
cd /d "%~dp0"
python rrtool.py gui
if errorlevel 1 pause
