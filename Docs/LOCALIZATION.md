# Localization

Languages: English (source), Simplified Chinese (`zh-Hans`), Japanese (`ja`).
Built on the official **Unity Localization** package (1.5.4).

## How it works

* The key of every translation is the **English text itself**. `Loc.T("Story")` returns the current language's
  text, or the English text if there is no translation.
* Every TextMeshPro text created by the scene builders carries a `Localize` component, which translates its
  English text (also text that code assigns later) and re-translates when the language changes.
* Runtime sentences with values use one template: `Loc.F("Goal : serve {0} customer(s)", n)`.
* Tables (Unity Localization string table collections): `UI` (scene/prefab texts), `Game` (code texts,
  achievements), `Dialogue` (tutorial/story bubbles), `Events` (day event windows).
* Fonts: the game's fonts are Latin-only. Each language has a fallback font (`Resources/LanguageFonts`), put first
  in TextMesh Pro's fallback list when the language is selected:
  Chinese: ZCOOL KuaiLe, Japanese: M PLUS Rounded 1c (both SIL Open Font License, `Fonts/CJK`).
* Startup language: `-language=<code>` on the command line, else the player's last choice (PlayerPrefs
  `rory-language`, written when they pick one), else the OS language, else English. Players switch with the **Language** button on the
  main menu.

## Editing translations

The source of truth is `Assets/Rory/Localization/Source/<Table>.tsv` (UTF-8, tab separated; columns
`en`, `zh-Hans`, `ja`, `context`; `\n`, `\t`, `\\` are escaped). The `context` column says where the text is used.

1. Edit the TSV (any spreadsheet that keeps UTF-8 and tabs works), or edit the string tables in
   *Window > Asset Management > Localization Tables* and copy the changes back to the TSV.
2. `Rory > Localization > Import TSV into String Tables` (also part of `Rory > Setup Project`).
3. `python Tools/localization/check_glyphs.py` checks that each language's font has every character used.

Keep placeholders exactly: `{0}` `{1}` ..., `VALUE` (achievement counters), `(player)` and `(juice percentage)`
(tutorial). In the result summaries keep the `\t` before each colon; the window lines the colons up.

The current Chinese and Japanese texts are machine-drafted (by Claude) from the English, written
in Tools/localization/translations_*.py and applied with `Tools/localization/apply_translations.py`.
**Have native speakers review them**; after a review, edit the TSV directly (the .py drafts are only the
starting point; re-running `apply_translations.py` would overwrite TSV edits).

## When texts change

`Rory > Localization > Extract Strings to TSV` collects every English text from the scenes, prefabs, game data
and runtime scripts into the TSVs, keeping existing translations. New rows have empty translations (they show
in English until translated); rows no longer found are kept and marked `(not found)` in the context column.

## Adding a language

1. Add it to `Loc.Languages` (code + its own name) — the picker and setup use this list.
2. If it needs another script, add a font to `Fonts/CJK` (or a new folder) and to `FallbackFonts` in
   `LocalizationSetup.cs`.
3. `Rory > Localization > Setup Locales, Tables and Fonts`, then Extract, translate the new TSV column, Import.

## Not localized (yet)

Text painted into the original artwork stays English: the logo, the "Main Menu" heading on the menu board, the
"Change Player" button, the kitchen's speech bubbles and similar sprites. Localizing them needs translated
images (Unity Localization *Asset Tables* can swap sprites per language).
