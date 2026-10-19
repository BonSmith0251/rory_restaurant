using UnityEditor;
using UnityEngine;

namespace Rory.EditorTools
{
    /// <summary>Import settings for everything under Assets/Rory, so new art and audio need no manual setup.</summary>
    public class RoryAssetPostprocessor : AssetPostprocessor
    {
        // Bump when the import settings change so Unity reimports the affected assets.
        public override uint GetVersion() => 2;

        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(RoryPaths.Art) && !assetPath.StartsWith(RoryPaths.BitmapFonts)) return;
            var ti = (TextureImporter)assetImporter;
            bool bitmapFont = assetPath.StartsWith(RoryPaths.BitmapFonts);

            ti.textureType = bitmapFont ? TextureImporterType.Default : TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.mipmapEnabled = false;
            ti.alphaIsTransparency = true;
            ti.wrapMode = TextureWrapMode.Clamp;
            ti.filterMode = FilterMode.Bilinear;
            ti.npotScale = TextureImporterNPOTScale.None;
            ti.maxTextureSize = 4096; // room for HD (2x) art

            var settings = new TextureImporterSettings();
            ti.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteGenerateFallbackPhysicsShape = false;
            ti.SetTextureSettings(settings);

            if (!bitmapFont)
            {
                ti.spritePixelsPerUnit = PixelsPerUnit(ti, assetPath);
                if (assetPath.StartsWith(RoryPaths.Sprites + "/")) context.DependsOnSourceAsset(LogicalSizes.Path);
            }

            // Hidden objects use alpha hit testing (Image.alphaHitTestMinimumThreshold) and need readable pixels.
            ti.isReadable = assetPath.StartsWith(RoryPaths.Sprites + "/HO");

            ti.textureCompression = assetPath.StartsWith(RoryPaths.Backgrounds) || assetPath.StartsWith(RoryPaths.Branding)
                ? TextureImporterCompression.CompressedHQ
                : TextureImporterCompression.Uncompressed;
        }

        /// <summary>
        /// Pixels per unit that keep an image's layout size whatever its resolution (HD art support):
        /// backgrounds and branding always span the 600-unit height (a 16:9 image then fills the 16:9 view);
        /// sprites keep the logical size recorded in Art/Sprites/LogicalSizes.json (their original pixel size).
        /// </summary>
        static float PixelsPerUnit(TextureImporter ti, string path)
        {
            const float reference = 100; // SpriteSizes.ReferencePixelsPerUnit
            ti.GetSourceTextureWidthAndHeight(out int w, out int h);
            if (w <= 0 || h <= 0) return reference;
            if (path.StartsWith(RoryPaths.Backgrounds) || path.StartsWith(RoryPaths.Branding))
                return reference * h / 600f;
            if (path.StartsWith(RoryPaths.Sprites + "/") && LogicalSizes.TryGet(path.Substring(RoryPaths.Sprites.Length + 1), out var size) && size.x > 0)
                return reference * w / size.x;
            return reference;
        }

        void OnPreprocessAudio()
        {
            if (!assetPath.StartsWith(RoryPaths.Audio)) return;
            var ai = (AudioImporter)assetImporter;
            bool music = assetPath.StartsWith(RoryPaths.Music);
            var s = ai.defaultSampleSettings;
            s.loadType = music ? AudioClipLoadType.Streaming : AudioClipLoadType.DecompressOnLoad;
            s.compressionFormat = AudioCompressionFormat.Vorbis;
            s.quality = music ? 0.6f : 0.8f;
            s.preloadAudioData = !music;
            ai.defaultSampleSettings = s;
            ai.loadInBackground = music;
        }
    }
}
