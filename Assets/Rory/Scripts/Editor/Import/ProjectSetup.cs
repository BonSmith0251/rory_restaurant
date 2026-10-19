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
