"""Writes draft (Simplified Chinese, Japanese) translations into Assets/Rory/Localization/Source/*.tsv.

Usage: python Tools/localization/apply_translations.py [--initial]
Default: fills only EMPTY cells, from the dictionaries keyed by exact English text (translations_kitchen.py, ...),
so reviewed TSV edits are never overwritten.
--initial: the first full draft (translations_ui_game.py / translations_story.py), matched by row order with an
English-prefix check; it overwrites, and only works on the tables as they were first extracted.
Then run Rory > Localization > Import TSV into String Tables (or Setup Project).
"""
import os
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, HERE)
from translations_ui_game import UI, GAME  # noqa: E402
from translations_story import DIALOGUE, EVENTS  # noqa: E402
from translations_kitchen import KITCHEN  # noqa: E402
from translations_minigames import DESSERT, HIDDEN_OBJECT, MATCH3  # noqa: E402

# Key-based drafts added after the first pass (exact English key -> (zh-Hans, ja)).
DRAFTS = {}
for d in (KITCHEN, DESSERT, HIDDEN_OBJECT, MATCH3):
    DRAFTS.update(d)

# Characters a language's font lacks, replaced on write ({code: {bad: good}}).
FIXUPS = {}

SOURCE = os.path.join(HERE, "..", "..", "Assets", "Rory", "Localization", "Source")


def unescape(s):
    out, i = [], 0
    while i < len(s):
        c = s[i]
        if c == "\\" and i + 1 < len(s):
            n = s[i + 1]
            out.append("\n" if n == "n" else "\t" if n == "t" else n)
            i += 2
        else:
            out.append(c)
            i += 1
    return "".join(out)


def escape(s):
    return s.replace("\\", "\\\\").replace("\t", "\\t").replace("\r", "").replace("\n", "\\n")


def check(key, code, text, where):
    for ph in ("{0}", "{1}", "{2}", "{3}", "VALUE", "(player)", "(juice percentage)"):
        if (ph in key) != (ph in text):
            raise SystemExit(f"{where} {code}: placeholder {ph} mismatch")


def fill(table):
    """Fills empty translation cells from DRAFTS."""
    path = os.path.join(SOURCE, table + ".tsv")
    lines = [l for l in open(path, encoding="utf-8").read().split("\n") if l]
    header = lines[0].split("\t")
    rows = [l.split("\t") for l in lines[1:]]
    col = {name: i for i, name in enumerate(header)}
    filled = 0
    for row in rows:
        key = unescape(row[0])
        if key not in DRAFTS:
            continue
        while len(row) < len(header):
            row.append("")
        for code, text in zip(("zh-Hans", "ja"), DRAFTS[key]):
            if row[col[code]]:
                continue
            for bad, good in FIXUPS.get(code, {}).items():
                text = text.replace(bad, good)
            check(key, code, text, f"{table} '{key[:30]}'")
            row[col[code]] = escape(text)
            filled += 1
    with open(path, "w", encoding="utf-8", newline="\n") as f:
        f.write("\n".join(["\t".join(header)] + ["\t".join(r) for r in rows]) + "\n")
    print(f"{table}: {filled} cells filled")


def apply(table, entries):
    path = os.path.join(SOURCE, table + ".tsv")
    lines = [l for l in open(path, encoding="utf-8").read().split("\n") if l]
    header = lines[0].split("\t")
    rows = [l.split("\t") for l in lines[1:]]
    if len(rows) != len(entries):
        raise SystemExit(f"{table}: {len(rows)} rows but {len(entries)} translations")
    col = {name: i for i, name in enumerate(header)}
    for n, (row, (prefix, zh, ja)) in enumerate(zip(rows, entries), 1):
        key = unescape(row[0])
        if not key.startswith(prefix):
            raise SystemExit(f"{table} row {n}: expected '{prefix}...', found '{key[:40]}'")
        for code, text in (("zh-Hans", zh), ("ja", ja)):
            for bad, good in FIXUPS.get(code, {}).items():
                text = text.replace(bad, good)
            for ph in ("{0}", "{1}", "{2}", "{3}", "VALUE", "(player)", "(juice percentage)"):
                if (ph in key) != (ph in text):
                    raise SystemExit(f"{table} row {n} {code}: placeholder {ph} mismatch")
            while len(row) < len(header):
                row.append("")
            row[col[code]] = escape(text)
    with open(path, "w", encoding="utf-8", newline="\n") as f:
        f.write("\n".join(["\t".join(header)] + ["\t".join(r) for r in rows]) + "\n")
    print(f"{table}: {len(rows)} rows translated")


if __name__ == "__main__":
    if "--initial" in sys.argv:
        apply("UI", UI)
        apply("Game", GAME)
        apply("Dialogue", DIALOGUE)
        apply("Events", EVENTS)
    for t in ("UI", "Game", "Dialogue", "Events"):
        fill(t)
