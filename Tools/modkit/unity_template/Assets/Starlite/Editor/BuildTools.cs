// Windows build from the menu or the command line:
//   Unity.exe -batchmode -quit -projectPath <project> -executeMethod Starlite.EditorTools.BuildTools.BuildWindows -logFile build.log
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Starlite.EditorTools
{
    public static class BuildTools
    {
        [MenuItem("Rory's Restaurant/Build Windows player")]
        public static void BuildWindows()
        {
            ProjectSetup.EnsureSetup();
            var opts = new BuildPlayerOptions
            {
                scenes = new[] { ProjectSetup.ScenePath },
                locationPathName = Path.Combine("Build", "Windows", "RorysRestaurant.exe"),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None,
            };
            var report = BuildPipeline.BuildPlayer(opts);
            Debug.Log("[Rory's Restaurant] Build " + report.summary.result + ": " + opts.locationPathName);
            if (Application.isBatchMode) EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
        }
    }
}
