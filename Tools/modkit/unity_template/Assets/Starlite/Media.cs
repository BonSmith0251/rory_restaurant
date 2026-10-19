// flash.media emulation on Unity AudioSources.
using System.Collections.Generic;
using UnityEngine;

namespace Starlite
{
    public class SoundTransform
    {
        public float volume, pan;
        public SoundTransform(float volume = 1, float pan = 0) { this.volume = volume; this.pan = pan; }
    }

    public class Sound : EventDispatcher
    {
        public AudioClip clip;
        public Sound(AudioClip clip = null) { this.clip = clip; }

        /// <summary>flash Sound.load: resolves the URL in Resources and dispatches COMPLETE on the next frame.</summary>
        public void load(URLRequest req, object context = null)
        {
            clip = Resources.Load<AudioClip>(AssetManager.ToResourcePath(req.url));
            Starling.juggler.delayCall((System.Action)(() =>
            {
                if (clip == null) { Debug.LogWarning("[Sound] not found: " + req.url); dispatchEvent(new IOErrorEvent(IOErrorEvent.IO_ERROR, req.url)); }
                else dispatchEvent(new Event(Event.COMPLETE));
            }), 0);
        }
        public float length => clip != null ? clip.length * 1000f : 0;

        /// <summary>startTime in ms; loops = extra repetitions (flash semantics: 0 plays once).</summary>
        public SoundChannel play(float startTime = 0, int loops = 0, SoundTransform sndTransform = null)
        {
            if (clip == null) return null;
            return SoundHost.Instance.Play(this, startTime, loops, sndTransform ?? new SoundTransform());
        }
    }

    public class SoundChannel : EventDispatcher
    {
        internal AudioSource source;
        internal int loopsLeft;
        internal bool stopped;
        SoundTransform mTransform = new SoundTransform();

        public float position => source != null ? source.time * 1000f : 0;

        public SoundTransform soundTransform
        {
            get => mTransform;
            set
            {
                mTransform = value ?? new SoundTransform();
                if (source != null) { source.volume = mTransform.volume; source.panStereo = mTransform.pan; }
            }
        }

        public void stop()
        {
            stopped = true;
            if (source != null) source.Stop();
            SoundHost.Instance.Release(this);
        }

        public float leftPeak => 0;
        public float rightPeak => 0;
    }

    public class SoundMixer
    {
        public static void stopAll() => SoundHost.Instance.StopAll();
        public static SoundTransform soundTransform
        {
            get => new SoundTransform(AudioListener.volume);
            set => AudioListener.volume = value?.volume ?? 1;
        }
    }

    internal class SoundHost : MonoBehaviour
    {
        static SoundHost sInstance;
        readonly List<SoundChannel> mPlaying = new List<SoundChannel>();
        readonly Stack<AudioSource> mPool = new Stack<AudioSource>();

        internal static SoundHost Instance
        {
            get
            {
                if (sInstance == null)
                {
                    var go = new GameObject("StarliteAudio");
                    DontDestroyOnLoad(go);
                    go.AddComponent<AudioListener>();
                    sInstance = go.AddComponent<SoundHost>();
                }
                return sInstance;
            }
        }

        internal SoundChannel Play(Sound s, float startMs, int loops, SoundTransform t)
        {
            var src = mPool.Count > 0 ? mPool.Pop() : gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.clip = s.clip;
            src.loop = loops >= 999;
            src.time = Mathf.Clamp(startMs / 1000f, 0, Mathf.Max(0, s.clip.length - 0.01f));
            var ch = new SoundChannel { source = src, loopsLeft = loops };
            ch.soundTransform = t;
            src.Play();
            mPlaying.Add(ch);
            return ch;
        }

        internal void Release(SoundChannel ch)
        {
            if (!mPlaying.Remove(ch)) return;
            if (ch.source != null) { ch.source.clip = null; mPool.Push(ch.source); ch.source = null; }
        }

        internal void StopAll()
        {
            foreach (var ch in mPlaying.ToArray()) ch.stop();
        }

        void Update()
        {
            foreach (var ch in mPlaying.ToArray())
            {
                if (ch.source == null || ch.stopped || ch.source.loop) continue;
                if (!ch.source.isPlaying && AudioListener.pause == false)
                {
                    if (ch.loopsLeft > 0)
                    {
                        ch.loopsLeft--;
                        ch.source.time = 0;
                        ch.source.Play();
                        continue;
                    }
                    Release(ch);
                    ch.dispatchEventWith(Event.SOUND_COMPLETE);
                }
            }
        }
    }
}
