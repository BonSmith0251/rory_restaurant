using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace Rory.Localization
{
    /// <summary>
    /// Translation lookup on top of Unity Localization. The key of every entry is the English text itself, so
    /// <c>Loc.T("Select Player")</c> returns the selected language's text (or the English text when there is
    /// no translation). The string tables are edited in Window > Asset Management > Localization Tables or
    /// generated from Assets/Rory/Localization/Source/*.tsv (Rory > Localization).
    /// </summary>
    public static class Loc
    {
        /// <summary>String tables (Unity Localization collections), searched in this order.</summary>
        public static readonly string[] Tables = { "UI", "Game", "Dialogue", "Events" };

        public const string English = "en";

        /// <summary>PlayerPrefs key of the chosen language (read at startup by <see cref="SavedLanguageSelector"/>).</summary>
        public const string PlayerPrefsKey = "rory-language";

        /// <summary>Supported languages: locale code and the language's own name.</summary>
        public static readonly (string code, string name)[] Languages =
        {
            ("en", "English"),
            ("zh-Hans", "简体中文"),
            ("ja", "日本語"),
        };

        static readonly Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.Ordinal);
        static bool initialized;
        static string current = English;

        /// <summary>Raised after the language changed and the tables are loaded.</summary>
        public static event Action Changed;

        /// <summary>Locale code of the current language ("en", "zh-Hans", "ja").</summary>
        public static string Current
        {
            get
            {
                Init();
                return current;
            }
        }

        /// <summary>Loads the selected locale's tables (synchronously; they are small and preloaded).</summary>
        public static void Init()
        {
            if (initialized || !Application.isPlaying) return;
            initialized = true;
            try
            {
                LocalizationSettings.InitializationOperation.WaitForCompletion();
                LocalizationSettings.SelectedLocaleChanged += _ => Rebuild();
                Rebuild();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Rory] Localization unavailable, using English: {e.Message}");
            }
        }

        static void Rebuild()
        {
            map.Clear();
            var locale = LocalizationSettings.SelectedLocale;
            current = locale != null ? locale.Identifier.Code : English;
            if (current != English)
            {
                foreach (var name in Tables)
                {
                    StringTable table = null;
                    try { table = LocalizationSettings.StringDatabase.GetTable(name, locale); }
                    catch (Exception e) { Debug.LogWarning($"[Rory] No '{name}' table for {current}: {e.Message}"); }
                    if (table == null) continue;
                    foreach (var entry in table.Values)
                    {
                        if (string.IsNullOrEmpty(entry.Value)) continue;
                        var key = table.SharedData.GetKey(entry.KeyId);
                        if (key != null && !map.ContainsKey(key)) map[key] = entry.Value;
                    }
                }
            }
            LanguageFonts.Apply(current);
            Changed?.Invoke();
        }

        /// <summary>Translation of an English text (the English text itself if there is none).</summary>
        public static string T(string english)
        {
            if (string.IsNullOrEmpty(english)) return english ?? "";
            Init();
            return map.TryGetValue(english, out var t) ? t : english;
        }

        /// <summary>Translated template formatted with string.Format, e.g. Loc.F("Day {0}", day + 1).</summary>
        public static string F(string englishTemplate, params object[] args)
        {
            var template = T(englishTemplate);
            try
            {
                return string.Format(template, args);
            }
            catch (FormatException)
            {
                Debug.LogWarning($"[Rory] Bad translation template for '{englishTemplate}': '{template}'");
                return string.Format(englishTemplate, args);
            }
        }

        public static bool Has(string english)
        {
            Init();
            return english != null && map.ContainsKey(english);
        }

        /// <summary>Switches language (saved by Unity Localization's PlayerPrefs selector).</summary>
        public static void SetLanguage(string code)
        {
            Init();
            Locale locale = LocalizationSettings.AvailableLocales.GetLocale(code);
            if (locale == null)
            {
                Debug.LogWarning($"[Rory] Unknown language '{code}'");
                return;
            }
            LocalizationSettings.SelectedLocale = locale;
            // Remember the choice (read at startup by SavedLanguageSelector).
            PlayerPrefs.SetString(PlayerPrefsKey, code);
            PlayerPrefs.Save();
            // The package's SelectedLocaleChanged event comes later (after async loads); rebuild now so code
            // that runs right after the switch (e.g. the next scene's Start) already sees the new language.
            Rebuild();
        }

        /// <summary>The language's own name for a locale code.</summary>
        public static string NativeName(string code)
        {
            foreach (var (c, n) in Languages) if (c == code) return n;
            return code;
        }
    }
}
