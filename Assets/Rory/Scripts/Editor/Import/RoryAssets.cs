using System.IO;
using UnityEditor;
using UnityEngine;

namespace Rory.EditorTools
{
    public static class RoryAssets
    {
        /// <summary>Creates every missing folder of an "Assets/..." path.</summary>
        public static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }

        public static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            EnsureFolder(Path.GetDirectoryName(path).Replace('\\', '/'));
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        public static T Load<T>(string path) where T : Object
        {
            var a = AssetDatabase.LoadAssetAtPath<T>(path);
            if (a == null) Debug.LogError($"[Rory] Missing asset {path} ({typeof(T).Name})");
            return a;
        }

        /// <summary>Sprite in Art/Sprites/&lt;group&gt;/&lt;name&gt;.png.</summary>
        public static Sprite Sprite(string group, string name) => Load<Sprite>($"{RoryPaths.Sprites}/{group}/{name}.png");

        public static Sprite Background(string name) => Load<Sprite>($"{RoryPaths.Backgrounds}/{name}.jpg");
    }
}
