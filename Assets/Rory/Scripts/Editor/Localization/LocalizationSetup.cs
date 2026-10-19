using System.IO;
using System.Linq;
using Rory.Localization;
using TMPro;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.TextCore.LowLevel;

namespace Rory.EditorTools
{
    /// <summary>
    /// Creates the Unity Localization setup: settings, the four locales, the string table collections of
    /// <see cref="Loc.Tables"/>, the CJK fallback font assets and Resources/LanguageFonts. Idempotent.
    /// </summary>
    public static class LocalizationSetup
    {
        public const string Folder = RoryPaths.Root + "/Localization";
        public const string SourceFolder = Folder + "/Source";
        const string CjkFonts = RoryPaths.Fonts + "/CJK";

        /// <summary>Fallback font file per language (in Fonts/CJK).</summary>
        static readonly (string code, string ttf)[] FallbackFonts =
        {
            ("zh-Hans", "ZCOOLKuaiLe-Regular"),
            ("ja", "MPLUSRounded1c-Regular"),
            ("ko", "Jua-Regular"),
        };

        [MenuItem("Rory/Localization/Setup Locales, Tables and Fonts")]
        public static void Setup()
        {
            RoryAssets.EnsureFolder(Folder + "/Locales");
            RoryAssets.EnsureFolder(Folder + "/Tables");

            var settings = LocalizationEditorSettings.ActiveLocalizationSettings;
            if (settings == null)
            {
                var path = Folder + "/LocalizationSettings.asset";
                settings = AssetDatabase.LoadAssetAtPath<LocalizationSettings>(path);
                if (settings == null)
                {
                    settings = ScriptableObject.CreateInstance<LocalizationSettings>();
                    settings.name = "LocalizationSettings";
                    AssetDatabase.CreateAsset(settings, path);
                }
                LocalizationEditorSettings.ActiveLocalizationSettings = settings;
            }

            // Startup: command line (-language=ja), the player's last choice, the OS language, else English.
            var selectors = settings.GetStartupLocaleSelectors();
            selectors.Clear();
            selectors.Add(new CommandLineLocaleSelector());
            selectors.Add(new PlayerPrefLocaleSelector());
            selectors.Add(new SystemLocaleSelector());
            selectors.Add(new SpecificLocaleSelector { LocaleId = new LocaleIdentifier(Loc.English) });
            EditorUtility.SetDirty(settings);

            foreach (var (code, _) in Loc.Languages)
            {
                if (LocalizationEditorSettings.GetLocale(code) != null) continue;
                var locale = Locale.CreateLocale(code);
                AssetDatabase.CreateAsset(locale, $"{Folder}/Locales/{code}.asset");
                LocalizationEditorSettings.AddLocale(locale);
            }

            foreach (var name in Loc.Tables)
            {
                var collection = LocalizationEditorSettings.GetStringTableCollection(name)
                                 ?? LocalizationEditorSettings.CreateStringTableCollection(name, Folder + "/Tables");
                foreach (var (code, _) in Loc.Languages)
                    if (collection.GetTable(code) == null) collection.AddNewTable(code);
                collection.SetPreloadTableFlag(true);
                EditorUtility.SetDirty(collection);
            }

            BuildFallbackFonts();
            AssetDatabase.SaveAssets();
            Debug.Log("[Rory] Localization setup complete");
        }

        static void BuildFallbackFonts()
        {
            var lf = RoryAssets.LoadOrCreate<LanguageFonts>(RoryPaths.Resources + "/LanguageFonts.asset");
            lf.languages.Clear();
            lf.languages.Add(new LanguageFonts.Entry { code = Loc.English });
            foreach (var (code, ttf) in FallbackFonts)
            {
                var fa = DynamicFont($"{CjkFonts}/{ttf}.ttf");
                lf.languages.Add(new LanguageFonts.Entry { code = code, fallbacks = { fa } });
            }
            // Japanese bold (credits titles) uses the real bold face instead of a faux bold.
            var ja = DynamicFont($"{CjkFonts}/MPLUSRounded1c-Regular.ttf");
            var jaBold = DynamicFont($"{CjkFonts}/MPLUSRounded1c-Bold.ttf");
            var weights = ja.fontWeightTable;
            if (weights != null && weights.Length > 7 && weights[7].regularTypeface != jaBold)
            {
                weights[7].regularTypeface = jaBold;
                EditorUtility.SetDirty(ja);
            }
            EditorUtility.SetDirty(lf);
        }

        /// <summary>Dynamic SDF font asset for a CJK font: small sampling size and big multi-atlas pages, since
        /// only the characters actually shown get rendered.</summary>
        static TMP_FontAsset DynamicFont(string ttfPath)
        {
            var path = $"{CjkFonts}/{Path.GetFileNameWithoutExtension(ttfPath)} SDF.asset";
            var fa = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if (fa != null) return fa;
            var font = RoryAssets.Load<Font>(ttfPath);
            fa = TMP_FontAsset.CreateFontAsset(font, 48, 5, GlyphRenderMode.SDFAA, 2048, 2048, AtlasPopulationMode.Dynamic, true);
            fa.name = Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(fa, path);
            fa.atlasTextures[0].name = fa.name + " Atlas";
            AssetDatabase.AddObjectToAsset(fa.atlasTextures[0], fa);
            fa.material.name = fa.name + " Material";
            AssetDatabase.AddObjectToAsset(fa.material, fa);
            EditorUtility.SetDirty(fa);
            return fa;
        }

        public static StringTableCollection Collection(string name) =>
            LocalizationEditorSettings.GetStringTableCollection(name);

        public static string[] TranslationCodes => Loc.Languages.Select(l => l.code).Where(c => c != Loc.English).ToArray();
    }
}
