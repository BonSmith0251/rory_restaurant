using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Rory.Data;
using Rory.Localization;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Rory.EditorTools
{
    /// <summary>
    /// The translation source files: Localization/Source/&lt;Table&gt;.tsv, one row per English text with a column per
    /// language and a context column. <see cref="Extract"/> collects every English text of the game into them
    /// (keeping existing translations); <see cref="Import"/> writes them into the Unity string tables.
    /// Format: UTF-8, tab separated, first row = header; \n, \t and \\ are escaped.
    /// </summary>
    public static class LocalizationTsv
    {
        const string ContextColumn = "context";

        class Row
        {
            public string key;
            public readonly Dictionary<string, string> values = new Dictionary<string, string>();
            public string context = "";
        }

        static string PathOf(string table) => $"{LocalizationSetup.SourceFolder}/{table}.tsv";

        // ------------------------------------------------------------------ extract

        [MenuItem("Rory/Localization/Extract Strings to TSV")]
        public static void Extract()
        {
            var found = new Dictionary<string, List<(string key, string context)>>();
            foreach (var t in Loc.Tables) found[t] = new List<(string, string)>();
            var seen = new HashSet<string>(StringComparer.Ordinal);

            void Add(string table, string text, string context)
            {
                if (!Translatable(text) || !seen.Add(text)) return;
                found[table].Add((text, context));
            }

            // UI: every localized text in the generated scenes and prefabs.
            foreach (var (text, ctx) in SceneAndPrefabTexts()) Add("UI", text, ctx);

            // Game: achievements and literals passed to Loc.T / Loc.F in runtime code.
            for (int i = 0; i < Rory.Achievements.Achievements.Count; i++)
            {
                Add("Game", Rory.Achievements.Achievements.Title[i], $"achievement {i} title");
                Add("Game", Rory.Achievements.Achievements.Info[i], $"achievement {i} info");
                Add("Game", Rory.Achievements.Achievements.InfoProgress[i], $"achievement {i} progress (VALUE = number)");
            }
            foreach (var (text, ctx) in CodeLiterals()) Add("Game", text, ctx);

            var db = AssetDatabase.LoadAssetAtPath<GameDatabase>(RoryPaths.Resources + "/GameDatabase.asset");
            // Hidden-object list labels are built from the layout data ("alarm clock,nm" -> "Alarm Clock").
            foreach (var layout in db.hiddenObjectLayouts)
            foreach (var item in layout.items)
            {
                var parts = item.name.Split(',');
                if (parts.Length > 1 && parts[1] == "nm")
                    Add("Game", Rory.Screens.HiddenObject.HiddenObjectController.FormatLabel(parts[0]), $"hidden object ({layout.sceneId})");
            }
            for (int d = 0; d < db.dialogues.dialogues.Count; d++)
            {
                var lines = db.dialogues.dialogues[d].lines;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].text != "(start level)") Add("Dialogue", lines[i].text, $"dialogue {d} line {i}");
                    Add("Dialogue", lines[i].buttonText, $"dialogue {d} line {i} button");
                }
            }
            for (int d = 0; d < db.levelEvents.days.Count; d++)
            {
                var windows = db.levelEvents.days[d].windows;
                for (int w = 0; w < windows.Count; w++)
                {
                    var win = windows[w];
                    string c = $"day {d} window {w}";
                    Add("Events", win.title.text, c + " title");
                    Add("Events", win.info.text, c + " info");
                    Add("Events", win.leftButton.text, c + " left button");
                    Add("Events", win.rightButton.text, c + " right button");
                    Add("Events", win.uniButton.text, c + " button");
                }
            }

            int added = 0, total = 0;
            foreach (var table in Loc.Tables)
            {
                var rows = Read(table);
                var byKey = rows.ToDictionary(r => r.key, StringComparer.Ordinal);
                var result = new List<Row>();
                foreach (var (key, ctx) in found[table])
                {
                    if (!byKey.TryGetValue(key, out var row))
                    {
                        row = new Row { key = key };
                        added++;
                    }
                    row.context = ctx;
                    result.Add(row);
                    byKey.Remove(key);
                }
                // Keep rows that are no longer found (hand-added runtime texts), marked for review.
                foreach (var r in rows.Where(r => byKey.ContainsKey(r.key)))
                {
                    if (!r.context.StartsWith("(not found)")) r.context = "(not found) " + r.context;
                    result.Add(r);
                }
                Write(table, result);
                total += result.Count;
            }
            AssetDatabase.Refresh();
            Debug.Log($"[Rory] Extracted {total} strings ({added} new) to {LocalizationSetup.SourceFolder}");
            ReportMissing();
        }

        static bool Translatable(string s) =>
            !string.IsNullOrWhiteSpace(s) && s.Any(char.IsLetter) && !s.StartsWith("<sprite");

        static IEnumerable<(string, string)> SceneAndPrefabTexts()
        {
            var setup = EditorSceneManager.GetSceneManagerSetup();
            foreach (var path in Directory.GetFiles(RoryPaths.Scenes, "*.unity").OrderBy(p => p, StringComparer.Ordinal))
            {
                var scene = EditorSceneManager.OpenScene(path.Replace('\\', '/'), OpenSceneMode.Single);
                foreach (var go in scene.GetRootGameObjects())
                foreach (var l in go.GetComponentsInChildren<Localize>(true))
                    yield return (SourceOf(l), $"{scene.name}: {PathTo(l.transform)}");
            }
            if (setup.Length > 0) EditorSceneManager.RestoreSceneManagerSetup(setup);
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { RoryPaths.Prefabs }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                foreach (var l in prefab.GetComponentsInChildren<Localize>(true))
                    yield return (SourceOf(l), $"{Path.GetFileNameWithoutExtension(path)}: {PathTo(l.transform)}");
            }
        }

        static string SourceOf(Localize l) =>
            string.IsNullOrEmpty(l.Source) ? l.GetComponent<TMP_Text>().text : l.Source;

        static string PathTo(Transform t)
        {
            var parts = new List<string>();
            for (; t != null && t.name != "Stage" && t.name != "Canvas"; t = t.parent) parts.Insert(0, t.name);
            return string.Join("/", parts);
        }

        static readonly Regex LiteralCall = new Regex(@"\bLoc\.[TF]\(\s*@?""((?:[^""\\]|\\.)*)""", RegexOptions.Compiled);
        static readonly Regex ConstCall = new Regex(@"\bLoc\.[TF]\(\s*([A-Za-z_][A-Za-z0-9_]*)\s*[,)]", RegexOptions.Compiled);
        static readonly Regex ConstDecl = new Regex(@"\bconst\s+string\s+([A-Za-z_][A-Za-z0-9_]*)\s*=\s*""((?:[^""\\]|\\.)*)""", RegexOptions.Compiled);
        static readonly Regex AnyLiteral = new Regex(@"(?<!@)""((?:[^""\\]|\\.)*)""", RegexOptions.Compiled);

        static readonly string[] HeuristicFolders = { "Screens", "Popups", "Achievements", "UI" };

        // Lines whose literals are never player-visible text.
        static readonly string[] IgnoredLineMarkers =
        {
            "Debug.Log", "Resources.Load", "PlaySfx(", "PlayMusic(", "StopSfx(", "IsPlayingMusic(", "sprites.Get(", "GetSequence(",
            "Find(", "GetComponent", "Shader.Find", "PlayerPrefs", "SetTrigger", "Animator", "nameof(",
        };

        /// <summary>
        /// English texts in runtime scripts: literals passed to Loc.T / Loc.F (also via a const), plus every other
        /// literal that looks like player-visible text (has a space, starts upper-case or ends with ! ? .), since
        /// texts assigned to a TextMeshPro with a Localize component are translated at runtime.
        /// Identifier-like strings (sprite and sound names) are skipped; a few harmless extra rows are possible.
        /// </summary>
        static IEnumerable<(string, string)> CodeLiterals()
        {
            foreach (var file in Directory.GetFiles(RoryPaths.Root + "/Scripts/Runtime", "*.cs", SearchOption.AllDirectories)
                         .OrderBy(f => f, StringComparer.Ordinal))
            {
                var text = File.ReadAllText(file);
                // Only UI-facing code is scanned for loose literals; core/data/tutorial code holds ids and tokens.
                var rel = file.Replace('\\', '/');
                bool heuristic = HeuristicFolders.Any(f => rel.Contains("/Scripts/Runtime/" + f + "/"));
                var consts = new Dictionary<string, string>();
                foreach (Match m in ConstDecl.Matches(text)) consts[m.Groups[1].Value] = Regex.Unescape(m.Groups[2].Value);
                var lines = File.ReadAllLines(file);
                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var trimmed = line.TrimStart();
                    if (trimmed.StartsWith("//") || trimmed.StartsWith("[")) continue;
                    string ctx = $"{Path.GetFileName(file)}:{i + 1}";
                    foreach (Match m in LiteralCall.Matches(line)) yield return (Regex.Unescape(m.Groups[1].Value), ctx);
                    foreach (Match m in ConstCall.Matches(line))
                        if (consts.TryGetValue(m.Groups[1].Value, out var c)) yield return (c, ctx);
                    if (!heuristic || IgnoredLineMarkers.Any(line.Contains) || line.Contains("new GameObject(")) continue;
                    var code = Regex.Replace(line, @"//.*$", "");
                    foreach (Match m in AnyLiteral.Matches(code))
                    {
                        string s;
                        try { s = Regex.Unescape(m.Groups[1].Value); }
                        catch (ArgumentException) { continue; }
                        if (LooksLikeText(s)) yield return (s, ctx);
                    }
                }
            }
        }

        static bool LooksLikeText(string s)
        {
            if (s.Length < 2 || !s.Any(char.IsLetter)) return false;
            if (Regex.IsMatch(s, @"^[a-z0-9_\-\.]+$")) return false;        // sprite / sound / key names
            if (Regex.IsMatch(s, @"^[A-Za-z0-9_]+\.[a-z]{2,4}$")) return false; // file names
            if (s.Contains("/") && !s.Contains(" ")) return false;            // paths
            if (s == "OrIgAmInglOl") return false;                             // the cheat player name
            return s.Contains(" ") || char.IsUpper(s[0]) || "!?.".Contains(s[s.Length - 1]);
        }

        // ------------------------------------------------------------------ import

        [MenuItem("Rory/Localization/Import TSV into String Tables")]
        public static void Import()
        {
            int count = 0;
            foreach (var table in Loc.Tables)
            {
                var collection = LocalizationSetup.Collection(table);
                if (collection == null)
                {
                    Debug.LogError($"[Rory] No string table collection '{table}' (run Rory > Localization > Setup)");
                    continue;
                }
                var rows = File.Exists(PathOf(table)) ? Read(table) : new List<Row>();
                var keys = new HashSet<string>(rows.Select(r => r.key), StringComparer.Ordinal);
                foreach (var stale in collection.SharedData.Entries.Where(e => !keys.Contains(e.Key)).Select(e => e.Key).ToList())
                    collection.RemoveEntry(stale);
                foreach (var (code, _) in Loc.Languages)
                {
                    var st = collection.GetTable(code) as UnityEngine.Localization.Tables.StringTable;
                    if (st == null) continue;
                    foreach (var r in rows)
                    {
                        var value = code == Loc.English ? r.key : r.values.TryGetValue(code, out var v) ? v : "";
                        st.AddEntry(r.key, value);
                    }
                    EditorUtility.SetDirty(st);
                }
                EditorUtility.SetDirty(collection.SharedData);
                count += rows.Count;
            }
            AssetDatabase.SaveAssets();
            Debug.Log($"[Rory] Imported {count} strings into the string tables");
        }

        // ------------------------------------------------------------------ report

        [MenuItem("Rory/Localization/Report Missing Translations")]
        public static void ReportMissing()
        {
            foreach (var code in LocalizationSetup.TranslationCodes)
            {
                int missing = 0, total = 0;
                foreach (var table in Loc.Tables)
                foreach (var r in Read(table))
                {
                    total++;
                    if (!r.values.TryGetValue(code, out var v) || string.IsNullOrEmpty(v)) missing++;
                }
                Debug.Log($"[Rory] {code}: {total - missing}/{total} translated");
            }
        }

        // ------------------------------------------------------------------ TSV I/O

        static List<Row> Read(string table)
        {
            var rows = new List<Row>();
            var path = PathOf(table);
            if (!File.Exists(path)) return rows;
            var lines = File.ReadAllLines(path, Encoding.UTF8);
            if (lines.Length == 0) return rows;
            var header = lines[0].Split('\t');
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Length == 0) continue;
                var cells = lines[i].Split('\t');
                var row = new Row { key = Unescape(cells[0]) };
                for (int c = 1; c < header.Length && c < cells.Length; c++)
                {
                    if (header[c] == ContextColumn) row.context = Unescape(cells[c]);
                    else row.values[header[c]] = Unescape(cells[c]);
                }
                rows.Add(row);
            }
            return rows;
        }

        static void Write(string table, List<Row> rows)
        {
            Directory.CreateDirectory(LocalizationSetup.SourceFolder);
            var codes = LocalizationSetup.TranslationCodes;
            var sb = new StringBuilder();
            sb.Append("en\t").Append(string.Join("\t", codes)).Append('\t').Append(ContextColumn).Append('\n');
            foreach (var r in rows)
            {
                sb.Append(Escape(r.key));
                foreach (var c in codes) sb.Append('\t').Append(Escape(r.values.TryGetValue(c, out var v) ? v : ""));
                sb.Append('\t').Append(Escape(r.context)).Append('\n');
            }
            File.WriteAllText(PathOf(table), sb.ToString(), new UTF8Encoding(false));
        }

        static string Escape(string s) => (s ?? "").Replace("\\", "\\\\").Replace("\t", "\\t").Replace("\r", "").Replace("\n", "\\n");

        static string Unescape(string s)
        {
            var sb = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != '\\' || i + 1 >= s.Length)
                {
                    sb.Append(s[i]);
                    continue;
                }
                char n = s[++i];
                sb.Append(n == 'n' ? '\n' : n == 't' ? '\t' : n);
            }
            return sb.ToString();
        }

        /// <summary>Batch: -executeMethod Rory.EditorTools.LocalizationTsv.BatchExtract</summary>
        public static void BatchExtract()
        {
            try
            {
                LocalizationSetup.Setup();
                Extract();
                EditorApplication.Exit(0);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                EditorApplication.Exit(1);
            }
        }
    }
}
