using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rory.Data
{
    /// <summary>
    /// Looks up sprites by the names used in the game data (customer textures, dishes, hidden objects, ...).
    /// Groups are the original atlas names (folders under Art/Sprites), plus "Backgrounds".
    /// Scenes and prefabs reference sprites directly; use this only for names that come from data.
    /// </summary>
    [CreateAssetMenu(menuName = "Rory/Sprite Catalog")]
    public class SpriteCatalog : ScriptableObject
    {
        [Serializable]
        public class Group
        {
            public string name;
            public List<Sprite> sprites = new List<Sprite>();
        }

        public List<Group> groups = new List<Group>();

        Dictionary<string, Dictionary<string, Sprite>> lookup;

        void OnEnable() => lookup = null;

        void Build()
        {
            lookup = new Dictionary<string, Dictionary<string, Sprite>>(StringComparer.Ordinal);
            foreach (var g in groups)
            {
                var d = new Dictionary<string, Sprite>(StringComparer.Ordinal);
                foreach (var s in g.sprites) if (s != null) d[s.name] = s;
                lookup[g.name] = d;
            }
        }

        /// <summary>Sprite <paramref name="spriteName"/> from group <paramref name="group"/>, or null.</summary>
        public Sprite Get(string group, string spriteName)
        {
            if (lookup == null) Build();
            return lookup.TryGetValue(group, out var d) && d.TryGetValue(spriteName, out var s) ? s : null;
        }

        /// <summary>Sprites named prefix + 1, 2, 3 ... (animation frames such as "api_1".."api_10").</summary>
        public List<Sprite> GetSequence(string group, string prefix, int first = 1)
        {
            var list = new List<Sprite>();
            for (int i = first; ; i++)
            {
                var s = Get(group, prefix + i);
                if (s == null) break;
                list.Add(s);
            }
            return list;
        }
    }
}
