using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Rory.EditorTools
{
    /// <summary>The logical (layout) sizes of the sprites, from Art/Sprites/LogicalSizes.json (Tools/native/record_sprite_sizes.py).</summary>
    public static class LogicalSizes
    {
        [Serializable] class Item { public string path; public int w, h; }
        [Serializable] class File { public List<Item> items = new List<Item>(); }

        public const string Path = RoryPaths.Sprites + "/LogicalSizes.json";

        static Dictionary<string, Vector2> sizes;
        static DateTime loadedStamp;

        /// <summary>Logical size of "group/name.png" (relative to Art/Sprites), if recorded.</summary>
        public static bool TryGet(string relativePath, out Vector2 size)
        {
            Load();
            return sizes.TryGetValue(relativePath, out size);
        }

        static void Load()
        {
            var stamp = System.IO.File.Exists(Path) ? System.IO.File.GetLastWriteTimeUtc(Path) : DateTime.MinValue;
            if (sizes != null && stamp == loadedStamp) return;
            loadedStamp = stamp;
            sizes = new Dictionary<string, Vector2>(StringComparer.Ordinal);
            if (stamp == DateTime.MinValue) return;
            var data = JsonUtility.FromJson<File>(System.IO.File.ReadAllText(Path));
            foreach (var i in data.items) sizes[i.path] = new Vector2(i.w, i.h);
        }
    }
}
