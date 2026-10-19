using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.UI
{
    /// <summary>Plays a list of sprites on an <see cref="Image"/> (the original Starling MovieClip).</summary>
    [RequireComponent(typeof(Image))]
    public class SpriteAnimator : MonoBehaviour
    {
        public List<Sprite> frames = new List<Sprite>();
        public float fps = 12;
        public bool loop = true;
        public bool playOnEnable = true;
        [Tooltip("Seconds of an empty frame after the last one (MovieClip.addFrame(Texture.empty, duration))")]
        public float blankAtEnd;

        Image image;
        float time;
        bool playing;

        public event Action Completed;

        public bool IsPlaying => playing;
        public int CurrentFrame { get; private set; }

        void Awake() => image = GetComponent<Image>();

        void OnEnable()
        {
            if (playOnEnable) Play();
        }

        public void Play(bool restart = true)
        {
            if (restart) SetFrame(0);
            playing = frames.Count > 0;
        }

        public void Stop() => playing = false;

        public void SetFrame(int frame)
        {
            if (image == null) image = GetComponent<Image>();
            if (frames.Count == 0) return;
            CurrentFrame = Mathf.Clamp(frame, 0, frames.Count - 1);
            time = CurrentFrame / Mathf.Max(0.01f, fps);
            image.sprite = frames[CurrentFrame];
        }

        void Update()
        {
            if (!playing || frames.Count == 0) return;
            time += Time.deltaTime;
            float length = frames.Count / Mathf.Max(0.01f, fps);
            float cycle = length + blankAtEnd;
            if (time >= cycle)
            {
                if (loop) time %= cycle;
                else
                {
                    time = cycle;
                    playing = false;
                }
            }
            bool blank = time >= length;
            image.enabled = !blank || !playing && blankAtEnd <= 0;
            int f = Mathf.Min((int)(time * fps), frames.Count - 1);
            if (f != CurrentFrame || image.sprite != frames[f])
            {
                CurrentFrame = f;
                image.sprite = frames[f];
            }
            if (!playing) Completed?.Invoke();
        }
    }
}
