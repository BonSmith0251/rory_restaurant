using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Rory.Localization
{
    /// <summary>
    /// Fallback fonts per language (Resources/LanguageFonts). The game's fonts only contain Latin glyphs, so
    /// Chinese and Japanese characters come from the selected language's fallback font. Only the current
    /// language's font is used, so shared Han characters get the right regional shapes.
    /// </summary>
    [CreateAssetMenu(menuName = "Rory/Language Fonts")]
    public class LanguageFonts : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string code;
            public List<TMP_FontAsset> fallbacks = new List<TMP_FontAsset>();
        }

        public List<Entry> languages = new List<Entry>();

        static List<TMP_FontAsset> original;

        /// <summary>Puts <paramref name="code"/>'s fonts first in TMP's global fallback list.</summary>
        public static void Apply(string code)
        {
            var list = TMP_Settings.fallbackFontAssets;
            if (list == null) return;
            if (original == null)
            {
                original = new List<TMP_FontAsset>(list);
#if UNITY_EDITOR
                // TMP Settings is an asset: put the list back when play mode ends.
                UnityEditor.EditorApplication.playModeStateChanged += s =>
                {
                    if (s != UnityEditor.PlayModeStateChange.ExitingPlayMode || original == null) return;
                    TMP_Settings.fallbackFontAssets.Clear();
                    TMP_Settings.fallbackFontAssets.AddRange(original);
                };
#endif
            }
            list.Clear();
            var fonts = Resources.Load<LanguageFonts>("LanguageFonts");
            var entry = fonts != null ? fonts.languages.Find(e => e.code == code) : null;
            if (entry != null) list.AddRange(entry.fallbacks);
            list.AddRange(original);
        }

        /// <summary>The fonts of one language (for labels that must always show that language, e.g. its name).</summary>
        public static TMP_FontAsset FontFor(string code)
        {
            var fonts = Resources.Load<LanguageFonts>("LanguageFonts");
            var entry = fonts != null ? fonts.languages.Find(e => e.code == code) : null;
            return entry != null && entry.fallbacks.Count > 0 ? entry.fallbacks[0] : null;
        }
    }
}
