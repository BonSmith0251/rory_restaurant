using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;

namespace Rory.EditorTools
{
    /// <summary>
    /// TextMeshPro setup: imports TMP Essential Resources, makes dynamic SDF font assets for the TTF fonts and
    /// static bitmap font assets for the Komikandy bitmap fonts (keeping their painted colours).
    /// </summary>
    public static class FontBuilder
    {
        const string EssentialsPackage = "Packages/com.unity.ugui/Package Resources/TMP Essential Resources.unitypackage";
        public const string TmpSettingsPath = "Assets/TextMesh Pro/Resources/TMP Settings.asset";

        public static bool HasTmpEssentials => File.Exists(TmpSettingsPath);

        [MenuItem("Rory/Import/TMP Essential Resources")]
        public static void ImportTmpEssentials()
        {
            if (HasTmpEssentials) return;
            AssetDatabase.ImportPackage(Path.GetFullPath(EssentialsPackage), false);
            AssetDatabase.Refresh();
        }

        [MenuItem("Rory/Import/Fonts")]
        public static void BuildFonts()
        {
            foreach (var ttf in Directory.GetFiles(RoryPaths.Fonts, "*.ttf"))
                BuildDynamicFont(ttf.Replace('\\', '/'));
            foreach (var fnt in Directory.GetFiles(RoryPaths.BitmapFonts, "*.fnt.xml"))
                BuildBitmapFont(fnt.Replace('\\', '/'));
            AssetDatabase.SaveAssets();
        }

        static string FontAssetPath(string sourcePath) =>
            $"{RoryPaths.Fonts}/{Path.GetFileName(sourcePath).Split('.')[0]} SDF.asset";

        static void BuildDynamicFont(string ttfPath)
        {
            var path = FontAssetPath(ttfPath);
            if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path) != null) return;
            var font = AssetDatabase.LoadAssetAtPath<Font>(ttfPath);
            var fa = TMP_FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic);
            fa.name = Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(fa, path);
            fa.atlasTextures[0].name = fa.name + " Atlas";
            AssetDatabase.AddObjectToAsset(fa.atlasTextures[0], fa);
            fa.material.name = fa.name + " Material";
            AssetDatabase.AddObjectToAsset(fa.material, fa);
            EditorUtility.SetDirty(fa);
        }

        static void SetInternal(object target, string property, object value) =>
            target.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(target, value);

        /// <summary>AngelCode BMFont XML (as exported by the original game) to a static TMP font asset.</summary>
        static void BuildBitmapFont(string fntPath)
        {
            var root = XDocument.Load(fntPath).Root;
            float A(XElement e, string n) => float.Parse((string)e.Attribute(n) ?? "0", CultureInfo.InvariantCulture);
            var info = root.Element("info");
            var common = root.Element("common");
            var texPath = $"{RoryPaths.BitmapFonts}/{(string)root.Element("pages").Element("page").Attribute("file")}";
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            int texW = tex.width, texH = tex.height;

            var path = $"{RoryPaths.Fonts}/{Path.GetFileName(fntPath).Split('.')[0]}.asset";
            var fa = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            bool create = fa == null;
            if (create) fa = ScriptableObject.CreateInstance<TMP_FontAsset>();
            fa.name = Path.GetFileNameWithoutExtension(path);

            float size = A(info, "size"), lineHeight = A(common, "lineHeight"), baseLine = A(common, "base");
            fa.faceInfo = new FaceInfo
            {
                familyName = (string)info.Attribute("face"),
                styleName = "Regular",
                pointSize = (int)size,
                scale = 1,
                lineHeight = lineHeight,
                ascentLine = baseLine,
                capLine = baseLine * 0.8f,
                meanLine = baseLine * 0.55f,
                baseline = 0,
                descentLine = baseLine - lineHeight,
                underlineOffset = -(lineHeight - baseLine) * 0.5f,
                underlineThickness = Mathf.Max(1, size / 16),
                strikethroughOffset = baseLine * 0.3f,
                superscriptOffset = baseLine * 0.5f,
                superscriptSize = 0.5f,
                subscriptOffset = -(lineHeight - baseLine) * 0.5f,
                subscriptSize = 0.5f,
                tabWidth = size,
            };

            fa.glyphTable.Clear();
            fa.characterTable.Clear();
            var glyphs = new Dictionary<uint, Glyph>();
            uint index = 1; // glyph index 0 is reserved for the missing glyph
            foreach (var c in root.Element("chars").Elements("char"))
            {
                uint id = (uint)A(c, "id");
                float x = A(c, "x"), y = A(c, "y"), w = A(c, "width"), h = A(c, "height");
                var glyph = new Glyph(index++,
                    new GlyphMetrics(w, h, A(c, "xoffset"), baseLine - A(c, "yoffset"), A(c, "xadvance")),
                    new GlyphRect((int)x, (int)(texH - y - h), (int)w, (int)h));
                fa.glyphTable.Add(glyph);
                fa.characterTable.Add(new TMP_Character(id, fa, glyph));
                glyphs[id] = glyph;
            }

            var pairs = new List<GlyphPairAdjustmentRecord>();
            var kernings = root.Element("kernings");
            if (kernings != null)
            {
                foreach (var k in kernings.Elements("kerning"))
                {
                    if (!glyphs.TryGetValue((uint)A(k, "first"), out var g1) || !glyphs.TryGetValue((uint)A(k, "second"), out var g2)) continue;
                    pairs.Add(new GlyphPairAdjustmentRecord(
                        new GlyphAdjustmentRecord(g1.index, new GlyphValueRecord(0, 0, A(k, "amount"), 0)),
                        new GlyphAdjustmentRecord(g2.index, new GlyphValueRecord(0, 0, 0, 0))));
                }
            }
            fa.fontFeatureTable.glyphPairAdjustmentRecords = pairs;

            SetInternal(fa, "version", "1.1.0"); // an empty version makes TMP "upgrade" the asset from the legacy format
            SetInternal(fa, "atlasWidth", texW);
            SetInternal(fa, "atlasHeight", texH);
            SetInternal(fa, "atlasPadding", 2);
            SetInternal(fa, "atlasRenderMode", GlyphRenderMode.RASTER);
            fa.atlasPopulationMode = AtlasPopulationMode.Static;
            fa.atlasTextures = new[] { tex };

            // The Komikandy glyphs are painted (gold gradient, outlines), so use a shader that keeps texture RGB.
            var shader = Shader.Find("TextMeshPro/Bitmap Custom Atlas") ?? Shader.Find("TextMeshPro/Bitmap");
            var mat = fa.material;
            if (mat == null)
            {
                mat = new Material(shader) { name = fa.name + " Material" };
                fa.material = mat;
            }
            mat.shader = shader;
            mat.SetTexture(ShaderUtilities.ID_MainTex, tex);

            if (create)
            {
                AssetDatabase.CreateAsset(fa, path);
                AssetDatabase.AddObjectToAsset(mat, fa);
            }
            fa.ReadFontAssetDefinition();
            EditorUtility.SetDirty(fa);
            EditorUtility.SetDirty(mat);
        }
    }
}
