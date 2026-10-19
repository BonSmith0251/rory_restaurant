using UnityEditor;
using UnityEngine;

namespace Rory.EditorTools
{
    /// <summary>Import settings for everything under Assets/Rory, so new art and audio need no manual setup.</summary>
    public class RoryAssetPostprocessor : AssetPostprocessor
    {
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
            ti.maxTextureSize = 2048;

            var settings = new TextureImporterSettings();
            ti.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteGenerateFallbackPhysicsShape = false;
            ti.SetTextureSettings(settings);

            // Hidden objects use alpha hit testing (Image.alphaHitTestMinimumThreshold) and need readable pixels.
            ti.isReadable = assetPath.StartsWith(RoryPaths.Sprites + "/HO");

            ti.textureCompression = assetPath.StartsWith(RoryPaths.Backgrounds) || assetPath.StartsWith(RoryPaths.Branding)
                ? TextureImporterCompression.CompressedHQ
                : TextureImporterCompression.Uncompressed;
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
