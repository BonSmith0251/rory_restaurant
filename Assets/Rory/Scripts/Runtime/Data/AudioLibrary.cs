using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rory.Data
{
    [CreateAssetMenu(menuName = "Rory/Audio Library")]
    public class AudioLibrary : ScriptableObject
    {
        public List<AudioClip> music = new List<AudioClip>();
        public List<AudioClip> sfx = new List<AudioClip>();

        Dictionary<string, AudioClip> lookup;

        void OnEnable() => lookup = null;

        /// <summary>Clip by file name without extension, e.g. "click_v1" or "sunny-bliss".</summary>
        public AudioClip Get(string clipName)
        {
            if (lookup == null)
            {
                lookup = new Dictionary<string, AudioClip>(StringComparer.Ordinal);
                foreach (var c in music) if (c != null) lookup[c.name] = c;
                foreach (var c in sfx) if (c != null) lookup[c.name] = c;
            }
            return clipName != null && lookup.TryGetValue(clipName, out var clip) ? clip : null;
        }
    }
}
