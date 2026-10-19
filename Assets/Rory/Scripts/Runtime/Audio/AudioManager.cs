using System;
using System.Collections.Generic;
using Rory.Data;
using UnityEngine;

namespace Rory.Audio
{
    /// <summary>
    /// Music (one looping source with a short cross-fade) and sound effects (a pool of 32 sources, like the
    /// original SoundLib channels). Clips are found by name in the <see cref="AudioLibrary"/>.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        const int SfxChannels = 32;

        [SerializeField] float musicVolume = 1;
        [SerializeField] float sfxVolume = 1;
        [SerializeField] float musicFade = 0.5f;

        AudioSource music;
        readonly List<AudioSource> sfx = new List<AudioSource>();
        int nextChannel;
        bool muteMusic, muteSound;
        float musicFadeTarget = 1, musicFadeLevel = 1;
        AudioClip pendingMusic;

        public event Action MuteChanged;

        public string CurrentMusic => music != null && music.clip != null ? music.clip.name : null;

        public bool MuteMusic
        {
            get => muteMusic;
            set { if (muteMusic == value) return; muteMusic = value; ApplyVolumes(); MuteChanged?.Invoke(); }
        }

        public bool MuteSound
        {
            get => muteSound;
            set { if (muteSound == value) return; muteSound = value; ApplyVolumes(); MuteChanged?.Invoke(); }
        }

        AudioLibrary Library => GameDatabase.Instance != null ? GameDatabase.Instance.audio : null;

        void Awake()
        {
            music = gameObject.AddComponent<AudioSource>();
            music.loop = true;
            music.playOnAwake = false;
            for (int i = 0; i < SfxChannels; i++)
            {
                var s = gameObject.AddComponent<AudioSource>();
                s.playOnAwake = false;
                sfx.Add(s);
            }
            ApplyVolumes();
        }

        void ApplyVolumes()
        {
            if (music != null) music.volume = muteMusic ? 0 : musicVolume * musicFadeLevel;
            foreach (var s in sfx) s.mute = muteSound;
        }

        /// <summary>Plays a looping music track; does nothing if it is already playing.</summary>
        public void PlayMusic(string clipName)
        {
            var clip = Library != null ? Library.Get(clipName) : null;
            if (clip == null)
            {
                Debug.LogWarning($"[Rory] Unknown music '{clipName}'");
                return;
            }
            if (music.clip == clip && music.isPlaying) return;
            if (!music.isPlaying) StartMusic(clip);
            else
            {
                pendingMusic = clip;
                musicFadeTarget = 0;
            }
        }

        /// <summary>True if one of the given tracks is playing (e.g. the two random match-3 tracks).</summary>
        public bool IsPlayingMusic(params string[] names) => Array.IndexOf(names, CurrentMusic) >= 0 && music.isPlaying;

        public void StopMusic()
        {
            pendingMusic = null;
            music.Stop();
            music.clip = null;
        }

        public void PauseMusic() => music.Pause();
        public void ResumeMusic() => music.UnPause();

        void StartMusic(AudioClip clip)
        {
            music.clip = clip;
            musicFadeLevel = 1;
            musicFadeTarget = 1;
            ApplyVolumes();
            music.Play();
        }

        void Update()
        {
            if (Mathf.Approximately(musicFadeLevel, musicFadeTarget)) return;
            musicFadeLevel = Mathf.MoveTowards(musicFadeLevel, musicFadeTarget, Time.unscaledDeltaTime / Mathf.Max(0.01f, musicFade));
            ApplyVolumes();
            if (musicFadeLevel <= 0 && pendingMusic != null)
            {
                var clip = pendingMusic;
                pendingMusic = null;
                StartMusic(clip);
            }
        }

        /// <summary>Plays a sound effect by name (e.g. "click_v1"). Returns the source, or null when muted/unknown.</summary>
        public AudioSource PlaySfx(string clipName, float volume = 1, bool loop = false)
        {
            if (muteSound) return null;
            var clip = Library != null ? Library.Get(clipName) : null;
            if (clip == null)
            {
                Debug.LogWarning($"[Rory] Unknown sound '{clipName}'");
                return null;
            }
            var s = sfx[nextChannel];
            nextChannel = (nextChannel + 1) % sfx.Count;
            s.clip = clip;
            s.volume = sfxVolume * volume;
            s.loop = loop;
            s.Play();
            return s;
        }

        /// <summary>Stops every source playing <paramref name="clipName"/> (looping sizzles etc.).</summary>
        public void StopSfx(string clipName)
        {
            foreach (var s in sfx)
                if (s.clip != null && s.clip.name == clipName) s.Stop();
        }

        public void StopAllSfx()
        {
            foreach (var s in sfx) s.Stop();
        }

        public void PauseSfx(bool paused)
        {
            foreach (var s in sfx)
            {
                if (paused) s.Pause();
                else s.UnPause();
            }
        }
    }
}
