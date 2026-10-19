using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Rory.EditorTools
{
    /// <summary>
    /// Marks a static, parameterless builder method (prefab or scene generator) so that
    /// Rory > Build > All Prefabs and Scenes runs it. Lower order runs first; prefabs use order &lt; 100.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RoryBuilderAttribute : Attribute
    {
        public readonly int order;
        public RoryBuilderAttribute(int order = 100) { this.order = order; }
    }

    /// <summary>Regenerates prefabs and scenes from code. Scenes are outputs: edit them freely, but a rebuild overwrites them.</summary>
    public static class BuildAll
    {
        [MenuItem("Rory/Build/All Prefabs and Scenes", priority = 1)]
        public static void Build()
        {
            var builders = TypeCache.GetMethodsWithAttribute<RoryBuilderAttribute>()
                .Where(m => m.IsStatic && m.GetParameters().Length == 0)
                .OrderBy(m => m.GetCustomAttribute<RoryBuilderAttribute>().order)
                .ThenBy(m => m.DeclaringType.FullName, StringComparer.Ordinal)
                .ToList();
            foreach (var m in builders)
            {
                Debug.Log($"[Rory] Building {m.DeclaringType.Name}.{m.Name}");
                m.Invoke(null, null);
            }
            UpdateBuildSettings();
            AssetDatabase.SaveAssets();
            Debug.Log($"[Rory] Prefabs and scenes built ({builders.Count} builders)");
        }

        /// <summary>
        /// Build Settings = the scenes of the GameScene enum that exist, in enum order (Splash first), then the
        /// legacy port's Main scene while it still exists. Deterministic, so branches never conflict on it.
        /// </summary>
        public static void UpdateBuildSettings()
        {
            var list = new List<EditorBuildSettingsScene>();
            foreach (GameScene s in Enum.GetValues(typeof(GameScene)))
            {
                var path = $"{RoryPaths.Scenes}/{s}.unity";
                if (File.Exists(path)) list.Add(new EditorBuildSettingsScene(path, true));
            }
            const string legacy = "Assets/Scenes/Main.unity";
            if (File.Exists(legacy)) list.Add(new EditorBuildSettingsScene(legacy, true));
            EditorBuildSettings.scenes = list.ToArray();
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
