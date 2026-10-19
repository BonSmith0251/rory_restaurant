// Import settings for the converted game assets under Assets/Resources/RR:
// textures stay pixel-exact (no NPOT rescaling, no mipmaps, uncompressed), music streams.
using UnityEditor;
using UnityEngine;

namespace Starlite.EditorTools
{
    public class StarliteImportSettings : AssetPostprocessor
    {
        static bool IsGameAsset(string path) => path.Replace('\\', '/').Contains("/Resources/RR/");

        void OnPreprocessTexture()
        {
            if (!IsGameAsset(assetPath)) return;
            var ti = (TextureImporter)assetImporter;
            ti.textureType = TextureImporterType.Default;
            ti.npotScale = TextureImporterNPOTScale.None;
            ti.mipmapEnabled = false;
            ti.alphaIsTransparency = true;
            ti.wrapMode = TextureWrapMode.Clamp;
            ti.filterMode = FilterMode.Bilinear;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.maxTextureSize = 4096;
            ti.sRGBTexture = true;
            // hidden-object atlases need CPU pixels for pixel-perfect touch (PixelHitArea)
            ti.isReadable = System.IO.Path.GetFileName(assetPath).StartsWith("HO");
        }

        void OnPreprocessAudio()
        {
            if (!IsGameAsset(assetPath)) return;
            var ai = (AudioImporter)assetImporter;
            var s = ai.defaultSampleSettings;
            bool music = new System.IO.FileInfo(assetPath).Length > 200 * 1024;
            s.loadType = music ? AudioClipLoadType.Streaming : AudioClipLoadType.DecompressOnLoad;
            s.compressionFormat = AudioCompressionFormat.Vorbis;
            s.quality = 0.7f;
            ai.defaultSampleSettings = s;
            ai.loadInBackground = music;
        }
    }
}
