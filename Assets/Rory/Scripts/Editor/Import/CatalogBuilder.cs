using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Rory.Data;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace Rory.EditorTools
{
    /// <summary>Builds the SpriteCatalog, AudioLibrary, Sprite Atlases and particle configs.</summary>
    public static class CatalogBuilder
    {
        [MenuItem("Rory/Import/Sprite Catalog + Atlases")]
        public static void BuildSprites()
        {
            var catalog = RoryAssets.LoadOrCreate<SpriteCatalog>(RoryPaths.Data + "/SpriteCatalog.asset");
            catalog.groups.Clear();
            foreach (var dir in Directory.GetDirectories(RoryPaths.Sprites).OrderBy(d => d, StringComparer.Ordinal))
            {
                var folder = dir.Replace('\\', '/');
                var group = new SpriteCatalog.Group { name = Path.GetFileName(folder) };
                group.sprites.AddRange(LoadSprites(folder, "*.png"));
                catalog.groups.Add(group);
                BuildAtlas(group.name, folder);
            }
            var bg = new SpriteCatalog.Group { name = "Backgrounds" };
            bg.sprites.AddRange(LoadSprites(RoryPaths.Backgrounds, "*.*"));
            bg.sprites.AddRange(LoadSprites(RoryPaths.Branding, "*.*"));
            catalog.groups.Add(bg);
            EditorUtility.SetDirty(catalog);

            var db = RoryAssets.LoadOrCreate<GameDatabase>(RoryPaths.Resources + "/GameDatabase.asset");
            db.sprites = catalog;
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Rory] Sprite catalog: {catalog.groups.Sum(g => g.sprites.Count)} sprites in {catalog.groups.Count} groups");
        }

        static Sprite[] LoadSprites(string folder, string pattern) =>
            Directory.GetFiles(folder, pattern)
                .Where(f => !f.EndsWith(".meta") && !f.EndsWith(".xml"))
                .OrderBy(f => f, StringComparer.Ordinal)
                .Select(f => AssetDatabase.LoadAssetAtPath<Sprite>(f.Replace('\\', '/')))
                .Where(s => s != null)
                .ToArray();

        /// <summary>One Sprite Atlas per sprite folder, so every screen draws from a few textures.</summary>
        static void BuildAtlas(string name, string folder)
        {
            RoryAssets.EnsureFolder(RoryPaths.SpriteAtlases);
            var path = $"{RoryPaths.SpriteAtlases}/{name}.spriteatlas";
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
            if (atlas == null)
            {
                atlas = new SpriteAtlas();
                AssetDatabase.CreateAsset(atlas, path);
            }
            atlas.SetIncludeInBuild(true);
            atlas.SetPackingSettings(new SpriteAtlasPackingSettings
            {
                blockOffset = 1, padding = 2, enableRotation = false, enableTightPacking = false, enableAlphaDilation = true,
            });
            atlas.SetTextureSettings(new SpriteAtlasTextureSettings
            {
                readable = name.StartsWith("HO"), generateMipMaps = false, sRGB = true, filterMode = FilterMode.Bilinear,
            });
            var platform = atlas.GetPlatformSettings("DefaultTexturePlatform");
            platform.maxTextureSize = 4096;
            platform.textureCompression = TextureImporterCompression.Uncompressed;
            atlas.SetPlatformSettings(platform);
            atlas.Remove(atlas.GetPackables());
            atlas.Add(new[] { AssetDatabase.LoadAssetAtPath<DefaultAsset>(folder) });
            EditorUtility.SetDirty(atlas);
        }

        [MenuItem("Rory/Import/Audio Library")]
        public static void BuildAudio()
        {
            var lib = RoryAssets.LoadOrCreate<AudioLibrary>(RoryPaths.Data + "/AudioLibrary.asset");
            lib.music = LoadClips(RoryPaths.Music);
            lib.sfx = LoadClips(RoryPaths.Sfx);
            EditorUtility.SetDirty(lib);
            var db = RoryAssets.LoadOrCreate<GameDatabase>(RoryPaths.Resources + "/GameDatabase.asset");
            db.audio = lib;
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Rory] Audio library: {lib.music.Count} music, {lib.sfx.Count} sfx");
        }

        static System.Collections.Generic.List<AudioClip> LoadClips(string folder) =>
            Directory.GetFiles(folder, "*.mp3").Concat(Directory.GetFiles(folder, "*.wav")).Concat(Directory.GetFiles(folder, "*.ogg"))
                .OrderBy(f => f, StringComparer.Ordinal)
                .Select(f => AssetDatabase.LoadAssetAtPath<AudioClip>(f.Replace('\\', '/')))
                .Where(c => c != null)
                .ToList();

        [MenuItem("Rory/Import/Particle Configs")]
        public static void BuildParticles()
        {
            var matPath = RoryPaths.Root + "/Shaders/UIAdditive.mat";
            if (AssetDatabase.LoadAssetAtPath<Material>(matPath) == null)
                AssetDatabase.CreateAsset(new Material(Shader.Find("Rory/UI Additive")), matPath);

            foreach (var file in Directory.GetFiles(RoryPaths.Effects, "*Config.xml"))
            {
                var name = Path.GetFileNameWithoutExtension(file).Replace("Config", "");
                var cfg = RoryAssets.LoadOrCreate<ParticleConfig>($"{RoryPaths.Effects}/{name}.asset");
                ReadPex(XDocument.Load(file).Root, cfg);
                // MiniGameHO_FireConfig.xml pairs with MiniGameHO_FireParticle.png
                cfg.texture = AssetDatabase.LoadAssetAtPath<Sprite>($"{RoryPaths.Effects}/{name}Particle.png");
                if (cfg.texture == null) Debug.LogError($"[Rory] No particle texture for {name}");
                EditorUtility.SetDirty(cfg);
            }
            AssetDatabase.SaveAssets();
        }

        static void ReadPex(XElement r, ParticleConfig c)
        {
            float V(string n, string a = "value") => float.Parse((string)r.Element(n)?.Attribute(a) ?? "0", CultureInfo.InvariantCulture);
            Vector2 P(string n) => new Vector2(V(n, "x"), V(n, "y"));
            Color C(string n) => new Color(V(n, "red"), V(n, "green"), V(n, "blue"), V(n, "alpha"));

            c.sourcePositionVariance = P("sourcePositionVariance");
            c.speed = V("speed"); c.speedVariance = V("speedVariance");
            c.lifespan = V("particleLifeSpan"); c.lifespanVariance = V("particleLifespanVariance");
            c.angle = V("angle"); c.angleVariance = V("angleVariance");
            c.gravity = P("gravity");
            c.radialAcceleration = V("radialAcceleration"); c.radialAccelerationVariance = V("radialAccelVariance");
            c.tangentialAcceleration = V("tangentialAcceleration"); c.tangentialAccelerationVariance = V("tangentialAccelVariance");
            c.startColor = C("startColor"); c.startColorVariance = C("startColorVariance");
            c.endColor = C("finishColor"); c.endColorVariance = C("finishColorVariance");
            c.maxParticles = Mathf.RoundToInt(V("maxParticles"));
            c.startSize = V("startParticleSize"); c.startSizeVariance = V("startParticleSizeVariance");
            c.endSize = V("finishParticleSize"); c.endSizeVariance = V("FinishParticleSizeVariance");
            c.duration = V("duration");
            c.emitterType = Mathf.RoundToInt(V("emitterType"));
            c.maxRadius = V("maxRadius"); c.maxRadiusVariance = V("maxRadiusVariance");
            c.minRadius = V("minRadius"); c.minRadiusVariance = V("minRadiusVariance");
            c.rotatePerSecond = V("rotatePerSecond"); c.rotatePerSecondVariance = V("rotatePerSecondVariance");
            c.startRotation = V("rotationStart"); c.startRotationVariance = V("rotationStartVariance");
            c.endRotation = V("rotationEnd"); c.endRotationVariance = V("rotationEndVariance");
            c.additive = Mathf.RoundToInt(V("blendFuncDestination")) == 1;
        }
    }
}
