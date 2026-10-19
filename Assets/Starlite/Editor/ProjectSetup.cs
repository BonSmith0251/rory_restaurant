// First-open project setup: creates Assets/Scenes/Main.unity, adds it to the build and applies
// player settings matching the original game (1024x600 window, resizable).
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Starlite.EditorTools
{
    [InitializeOnLoad]
    public static class ProjectSetup
    {
        public const string ScenePath = "Assets/Scenes/Main.unity";
        const string Marker = "Assets/Starlite/Editor/.setup_done";

        static ProjectSetup()
        {
            EditorApplication.delayCall += Run;
        }

        [MenuItem("Rory's Restaurant/Re-run project setup")]
        static void Rerun()
        {
            if (File.Exists(Marker)) File.Delete(Marker);
            Run();
        }

        [MenuItem("Rory's Restaurant/Open main scene")]
        static void OpenMain()
        {
            if (File.Exists(ScenePath)) EditorSceneManager.OpenScene(ScenePath);
        }

        /// <summary>Creates the scene / build settings right now (used by batch builds, where delayCall may not run).</summary>
        public static void EnsureSetup()
        {
            if (File.Exists(Marker)) File.Delete(Marker);
            Run();
        }

        static void Run()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || File.Exists(Marker)) return;
            if (!File.Exists(ScenePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var cam = new GameObject("Main Camera").AddComponent<Camera>();
                cam.tag = "MainCamera";
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = UnityEngine.Color.black;
                cam.orthographic = true;
                EditorSceneManager.SaveScene(scene, ScenePath);
            }
            var scenes = EditorBuildSettings.scenes.ToList();
            if (!scenes.Any(s => s.path == ScenePath))
            {
                scenes.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
                EditorBuildSettings.scenes = scenes.ToArray();
            }
            PlayerSettings.companyName = "Origaming Media (Unity port)";
            PlayerSettings.productName = "Rory's Restaurant Deluxe";
            // Screen size and mode are set by the native project (Rory > Configure Player: 1920x1080).
            PlayerSettings.runInBackground = false;
            File.WriteAllText(Marker, "done");
            AssetDatabase.Refresh();
            if (EditorSceneManager.GetActiveScene().path != ScenePath) EditorSceneManager.OpenScene(ScenePath);
            Debug.Log("[Rory's Restaurant] Project setup complete. Press Play to start the game.");
        }
    }
}
