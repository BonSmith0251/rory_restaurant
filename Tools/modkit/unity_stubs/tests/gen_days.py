"""Generates a headless script: new player -> story -> play N match-3 days -> stop at the next screen."""
import sys

days = int(sys.argv[1]) if len(sys.argv) > 1 else 3
out = []
w = out.append
w("sec 12")
w("clicktext Story")
w("sec 2")
w("type Tester")
w("clicktext Done")
w("sec 3")
w("clicktext Select")
w("sec 3")
w("clicktext Story")
w("sec 3")
for _ in range(8):
    w("click 512 300")
    w("sec 2")
for _ in range(12):
    w("tryclicktext Next")
    w("sec 2")


def dismiss():
    for t in ("OK", "Next", "Continue"):
        w("tryclicktext " + t)


for day in range(1, days + 1):
    w("log ===== day %d =====" % day)
    w("click 539 415")  # kitchen PLAY bubble
    w("sec 3")
    dismiss()
    w("tryclicktext PLAY")  # level select
    w("sec 5")
    for i in range(80):
        dismiss()
        w("autoswap?")
        w("sec 1.2")
        w("click 150 108")
        w("click 150 228")
        w("click 150 348")
        w("tryclicktext Kitchen")
    w("sec 3")
    w("shot day%d_end" % day)
    for _ in range(6):
        dismiss()
        w("sec 1.5")
    w("shot day%d_kitchen" % day)
w("click 539 415")
w("sec 3")
w("tryclicktext PLAY")
w("sec 6")
w("shot next_mode")
w("tree 3")
print("\n".join(out))
