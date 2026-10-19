using System;
using UnityEditor;
using UnityEngine;

namespace Rory.EditorTools
{
    /// <summary>
    /// One-click (re)generation of every derived asset: fonts, sprite catalog + atlases, audio library,
    /// particle configs and game data. Safe to run repeatedly; existing assets are updated in place.
    /// </summary>
    public static class ProjectSetup
    {
        [MenuItem("Rory/Setup Project", priority = 0)]
        public static void SetupAll()
        {
            ConfigurePlayer();
            FontBuilder.ImportTmpEssentials();
            AssetDatabase.Refresh();
            FontBuilder.BuildFonts();
            CatalogBuilder.BuildSprites();
            CatalogBuilder.BuildAudio();
            CatalogBuilder.BuildParticles();
            GameDataImporter.ImportAll();
            LocalizationSetup.Setup();
            LocalizationTsv.Import();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Rory] Project setup complete");
        }

        /// <summary>Full HD player: 1920x1080, full screen by default (Options: Windowed = a 1920x1080 window).</summary>
        [MenuItem("Rory/Configure Player (1920x1080)")]
        public static void ConfigurePlayer()
        {
            PlayerSettings.defaultScreenWidth = 1920;
            PlayerSettings.defaultScreenHeight = 1080;
            PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
            PlayerSettings.defaultIsNativeResolution = false; // always 1920x1080, scaled to other monitors
            PlayerSettings.resizableWindow = true;
            PlayerSettings.allowFullscreenSwitch = true;
        }

        /// <summary>Batch mode entry: -executeMethod Rory.EditorTools.ProjectSetup.BatchSetup</summary>
        public static void BatchSetup()
        {
            try
            {
                SetupAll();
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
