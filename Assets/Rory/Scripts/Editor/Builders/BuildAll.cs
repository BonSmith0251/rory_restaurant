using System;
using UnityEditor;
using UnityEngine;

namespace Rory.EditorTools
{
    /// <summary>Regenerates prefabs and scenes from code. Scenes are outputs: edit them freely, but a rebuild overwrites them.</summary>
    public static class BuildAll
    {
        [MenuItem("Rory/Build/All Prefabs and Scenes", priority = 1)]
        public static void Build()
        {
            WindowBuilder.BuildPrefabs();
            SplashBuilder.Build();
            MainMenuBuilder.Build();
            AssetDatabase.SaveAssets();
            Debug.Log("[Rory] Prefabs and scenes built");
        }

        /// <summary>Batch mode: -executeMethod Rory.EditorTools.BuildAll.BatchBuild</summary>
        public static void BatchBuild()
        {
            try
            {
                Build();
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
