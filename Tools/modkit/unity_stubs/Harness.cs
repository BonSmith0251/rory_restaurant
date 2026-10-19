// Headless play-test harness: boots the ported game on FakeUnity and runs a scripted session.
// Usage: Harness.exe <Assets/Resources dir> <output dir> <script file>
// Script commands (stage coordinates, 1024x600, y down):
//   wait <frames> | sec <seconds> | click <x> <y> | move <x> <y> | drag <x1> <y1> <x2> <y2> [steps]
//   key esc|enter|space|backspace | type <text> | shot <name> | tree [depth] | clicktext <text> | focus 0|1
//   expect <text>   (fails if no visible text field contains it) | log <message>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Starlite;
using UnityEngine;

static class Harness
{
    static string sOut;
    static int sShots;

    static int Main(string[] args)
    {
        if (args.Length < 3) { Console.WriteLine("usage: Harness <resourcesDir> <outDir> <script>"); return 1; }
        FakeUnity.ResourcesDir = args[0];
        sOut = args[1];
        Directory.CreateDirectory(sOut);
        FakeUnity.DataPath = Environment.GetEnvironmentVariable("FAKEUNITY_DATA") ?? Path.Combine(sOut, "persistent");
        Directory.CreateDirectory(FakeUnity.DataPath);
        FakeUnity.Verbose = true;
        var script = File.ReadAllLines(args[2]);
        try
        {
            typeof(GameBootstrap).GetMethod("Boot", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
            int lineNo = 0;
            foreach (var raw in script)
            {
                lineNo++;
                var line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("#")) continue;
                Console.WriteLine("> " + line);
                if (!Run(line)) { Console.WriteLine("SCRIPT FAILED at line " + lineNo + ": " + line); Shot("failed"); return 3; }
            }
        }
        catch (Exception e)
        {
            var inner = e is TargetInvocationException t && t.InnerException != null ? t.InnerException : e;
            Console.WriteLine("CRASH at frame " + FakeUnity.Frame + ": " + inner);
            try { Shot("crash"); } catch { }
            return 2;
        }
        int warnings = FakeUnity.Logs.Count(l => l.Contains("WARN") || l.Contains("ERROR"));
        Console.WriteLine($"OK: {FakeUnity.Frame} frames, {sShots} screenshots, {warnings} warnings/errors logged, sounds: {string.Join(",", FakeUnity.SoundsPlayed.Distinct().Take(20))}");
        return 0;
    }

    static void Frames(int n) { for (int i = 0; i < n; i++) FakeUnity.Step(); }

    static void MouseTo(float x, float y) => FakeUnity.Mouse = new Vector3(x, FakeUnity.ScreenHeight - y, 0);

    static void Click(float x, float y)
    {
        MouseTo(x, y);
        Frames(1); // hover
        FakeUnity.MouseDown = true; FakeUnity.MouseHeld = true;
        Frames(1);
        FakeUnity.MouseHeld = false; FakeUnity.MouseUp = true;
        Frames(1);
    }

    static void Shot(string name)
    {
        sShots++;
        var f = Path.Combine(sOut, $"{sShots:00}_{name}.png");
        FakeUnity.Screenshot(f);
        Console.WriteLine("  screenshot " + f);
    }

    static IEnumerable<DisplayObject> Walk(DisplayObject d)
    {
        yield return d;
        if (d is DisplayObjectContainer c)
            for (int i = 0; i < c.numChildren; i++)
                foreach (var x in Walk(c.getChildAt(i))) yield return x;
    }

    static bool IsShown(DisplayObject d)
    {
        for (var c = d; c != null; c = c.parent) if (!c.visible || c.alpha <= 0.01f) return false;
        return true;
    }

    static void Tree(DisplayObject d, int depth, int max, string indent = "")
    {
        var b = d.getBounds(Starling.Stage);
        string extra = d is TextField tf ? " \"" + tf.text.Replace("\n", "\\n") + "\"" : "";
        Console.WriteLine($"{indent}{d.GetType().Name} {d.name} vis={d.visible} a={d.alpha:0.##} touch={d.touchable} bounds=({b.x:0},{b.y:0},{b.width:0}x{b.height:0}){extra}");
        if (depth >= max || !(d is DisplayObjectContainer c)) return;
        for (int i = 0; i < c.numChildren; i++) Tree(c.getChildAt(i), depth + 1, max, indent + "  ");
    }

    /// <summary>Finds a swap on the match-3 board that makes a line of 3 and performs it as a drag.</summary>
    static bool AutoSwap()
    {
        var gs = Global.getInstance().gameScreen;
        if (gs?.tileAr == null) { Console.WriteLine("  no board"); return false; }
        var tiles = gs.tileAr.SelectMany(r => r).Where(t => t != null && !t.removed && t.parent != null && !t.frozen).ToList();
        if (tiles.Count == 0) { Console.WriteLine("  board empty"); return false; }
        var centers = tiles.Select(t => { var b = t.getBounds(Starling.Stage); return (t, x: b.x + b.width / 2, y: b.y + b.height / 2); }).ToList();
        float x0 = centers.Min(c => c.x), y0 = centers.Min(c => c.y);
        var grid = new Dictionary<(int, int), (Match3Object t, float x, float y)>();
        foreach (var c in centers) grid[((int)Math.Round((c.x - x0) / 68f), (int)Math.Round((c.y - y0) / 68f))] = c;
        int Type(Dictionary<(int, int), (Match3Object t, float x, float y)> g, int cx, int cy) => g.TryGetValue((cx, cy), out var v) ? v.t.type : -999;
        bool Line(Dictionary<(int, int), (Match3Object t, float x, float y)> g, int cx, int cy)
        {
            int ty = Type(g, cx, cy);
            if (ty == -999) return false;
            int h = 1, v = 1;
            for (int i = cx - 1; Type(g, i, cy) == ty; i--) h++;
            for (int i = cx + 1; Type(g, i, cy) == ty; i++) h++;
            for (int i = cy - 1; Type(g, cx, i) == ty; i--) v++;
            for (int i = cy + 1; Type(g, cx, i) == ty; i++) v++;
            return h >= 3 || v >= 3;
        }
        foreach (var kv in grid)
        {
            var (cx, cy) = kv.Key;
            foreach (var (dx, dy) in new[] { (1, 0), (0, 1) })
            {
                if (!grid.TryGetValue((cx + dx, cy + dy), out var other)) continue;
                var g2 = new Dictionary<(int, int), (Match3Object t, float x, float y)>(grid);
                g2[(cx, cy)] = other; g2[(cx + dx, cy + dy)] = kv.Value;
                if (Line(g2, cx, cy) || Line(g2, cx + dx, cy + dy))
                {
                    Console.WriteLine($"  swapping type {kv.Value.t.type} at ({cx},{cy}) with type {other.t.type} at ({cx + dx},{cy + dy})");
                    return Run($"drag {kv.Value.x:0} {kv.Value.y:0} {other.x:0} {other.y:0} 8");
                }
            }
        }
        Console.WriteLine("  no move found");
        return false;
    }

    /// <summary>Clicks an opaque pixel of a hidden object whose name is currently shown in the item list.</summary>
    static bool HoFind()
    {
        var ho = Global.getInstance()._minigameHO?._HOClass;
        if (ho?.itemHOAr == null) { Console.WriteLine("  no HO scene"); return false; }
        // only names visible inside the paper strip at the top (the rest of the list is scrolled out / clipped)
        var listed = ho.itemTextUIAr.Where(t => t != null && IsShown(t))
            .Where(t => { var tb = t.getBounds(Starling.Stage); float cy = tb.y + tb.height / 2, cx = tb.x + tb.width / 2; return cy > 0 && cy < 110 && cx > 180 && cx < 860; })
            .OrderBy(t => t.getBounds(Starling.Stage).x).Select(t => (t.text ?? "").ToLowerInvariant()).ToList();
        foreach (var it in ho.itemHOAr)
        {
            if (it == null || it.found || it.itemIsFound || it.itemType != HOObjects.AS_HO_ITEM || it.image == null || !IsShown(it.image)) continue;
            string nm = (it.itemName ?? "").Split(',')[0].ToLowerInvariant();
            if (!listed.Take(1).Concat(listed).Any(l => l == nm || l.Replace(" ", "") == nm.Replace(" ", ""))) continue;
            var b = it.image.getBounds(Starling.Stage);
            for (float fy = 0.5f; fy < 1; fy += 0.1f)
                for (float fx = 0.5f; fx < 1; fx += 0.1f)
                    foreach (var (px, py) in new[] { (b.x + b.width * fx, b.y + b.height * fy), (b.x + b.width * (1 - fx), b.y + b.height * (1 - fy)) })
                    {
                        if (Starling.Stage.hitTest(new Starlite.Point(px, py), true) == it.image)
                        {
                            Console.WriteLine($"  clicking '{it.itemName}' at ({px:0},{py:0})");
                            Click(px, py);
                            return true;
                        }
                    }
            Console.WriteLine($"  '{it.itemName}' is covered at all sample points");
        }
        Console.WriteLine("  nothing clickable; listed=" + string.Join(",", listed));
        return true;
    }

    static bool Run(string line)
    {
        var p = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string cmd = p[0].ToLowerInvariant();
        string rest = line.Length > p[0].Length ? line.Substring(p[0].Length).Trim() : "";
        float F(int i) => float.Parse(p[i], System.Globalization.CultureInfo.InvariantCulture);
        switch (cmd)
        {
            case "wait": Frames((int)F(1)); return true;
            case "sec": Frames((int)(F(1) * 60)); return true;
            case "click": Click(F(1), F(2)); return true;
            case "move": MouseTo(F(1), F(2)); Frames(1); return true;
            case "drag":
            {
                int steps = p.Length > 5 ? (int)F(5) : 10;
                MouseTo(F(1), F(2)); Frames(1);
                FakeUnity.MouseDown = true; FakeUnity.MouseHeld = true; Frames(1);
                for (int i = 1; i <= steps; i++) { MouseTo(F(1) + (F(3) - F(1)) * i / steps, F(2) + (F(4) - F(2)) * i / steps); Frames(1); }
                FakeUnity.MouseHeld = false; FakeUnity.MouseUp = true; Frames(1);
                return true;
            }
            case "key":
            {
                var k = rest.ToLowerInvariant() == "esc" ? KeyCode.Escape : rest.ToLowerInvariant() == "enter" ? KeyCode.Return :
                        rest.ToLowerInvariant() == "space" ? KeyCode.Space : KeyCode.Backspace;
                FakeUnity.KeysDown.Add(k);
                if (k == KeyCode.Backspace) FakeUnity.InputString = "\b";
                Frames(1);
                return true;
            }
            case "type": FakeUnity.InputString = rest; Frames(1); return true;
            case "shot": Shot(rest.Length > 0 ? rest : "shot"); return true;
            case "tree": Tree(Starling.Stage, 0, p.Length > 1 ? (int)F(1) : 6); return true;
            case "focus": FakeUnity.Focus(p[1] == "1"); Frames(1); return true;
            case "log": Console.WriteLine("  # " + rest); return true;
            case "autoswap": return AutoSwap();
            case "autoswap?": AutoSwap(); return true;
            case "hofind": return HoFind();
            case "day":
            {
                int d = (int)F(1);
                TSavegame.currentPlayer.passedDays = d;
                TSavegame.currentPlayer.lastDayKitchen = d;
                TGlobalClass.currentPlayedLevel = d;
                string region = TGlobalClass.levelSetXML["day"][d]["region"];
                if (!string.IsNullOrEmpty(region)) TGlobalClass.currentRegionDish = region;
                Console.WriteLine("  passedDays=" + d + " gameType=" + (string)TGlobalClass.levelSetXML["day"][d]["gameType"]);
                return true;
            }
            case "goto":
            {
                var g = Global.getInstance();
                Screen scr = rest switch
                {
                    "ho" => g._minigameHO, "dessert" => g._minigameDessert, "match3" => g.gameScreen, "kitchen" => g._kitchen,
                    "levelselect" => g._levelSelectScreen, "menu" => g._mainMenuScreen, "achievement" => g._achievementScreen,
                    "ending" => g._ending, "opening" => g._opening, _ => null
                };
                if (scr == null) { Console.WriteLine("  unknown screen " + rest); return false; }
                g._game.fadeToScreen(scr);
                Frames(1);
                return true;
            }
            case "expect":
            case "clicktext":
            case "tryclicktext":
            {
                var hit = Walk(Starling.Stage).OfType<TextField>()
                    .Where(t => IsShown(t) && t.text != null && t.text.IndexOf(rest, StringComparison.OrdinalIgnoreCase) >= 0)
                    .LastOrDefault();
                if (hit == null && cmd == "tryclicktext") { Console.WriteLine("  (not present)"); return true; }
                if (hit == null)
                {
                    var texts = Walk(Starling.Stage).OfType<TextField>().Where(IsShown).Select(t => t.text).Where(t => !string.IsNullOrEmpty(t));
                    Console.WriteLine("  text not found: \"" + rest + "\". Visible texts: " + string.Join(" | ", texts.Take(40)));
                    return false;
                }
                if (cmd != "expect")
                {
                    var b = hit.getBounds(Starling.Stage);
                    Click(b.x + b.width / 2, b.y + b.height / 2);
                }
                return true;
            }
        }
        Console.WriteLine("  unknown command " + cmd);
        return false;
    }
}
