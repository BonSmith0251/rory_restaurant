using System;
using System.Collections.Generic;
using Rory.Tweening;
using UnityEngine;

namespace Rory.Screens.Dessert
{
    /// <summary>
    /// The TimelineLite of MiniGameMakeDessert: the customer, his patience bar, the pan and the dessert on it slide
    /// out (position 0) and back in (position 1), and the timeline's onComplete (checkTimesUp) starts the next order.
    /// Only the parts of GreenSock v12 the screen relies on are emulated, with GreenSock's semantics:
    /// <list type="bullet">
    /// <item>tweens of the anchored x of RectTransforms at absolute positions; start values are read when a tween
    /// first renders; a starting tween kills running tweens of the same target (overwrite "auto");</item>
    /// <item><see cref="Restart"/> moves the playhead to 0 without firing callbacks (restart(false, suppressEvents = true)),
    /// rendering earlier tweens back to their start values;</item>
    /// <item>adding a tween to a completed timeline re-enables it with the playhead where it stopped;</item>
    /// <item><see cref="Pause"/> / <see cref="Resume"/> freeze the playhead (the deactivate pause of the original).</item>
    /// </list>
    /// A plain object advanced by its owner every frame (<see cref="Advance"/>), so it only runs while the screen exists.
    /// </summary>
    public class DessertTimeline
    {
        class Entry
        {
            public RectTransform target;
            public float from, to, start, duration;
            public EaseFunction ease;
            public Action onComplete;
            public bool started, done;
        }

        readonly List<Entry> entries = new List<Entry>();

        /// <summary>Playhead in seconds.</summary>
        public float Time { get; private set; }
        public bool Paused { get; private set; }
        /// <summary>The playhead reached the end (an empty timeline is complete).</summary>
        public bool Completed { get; private set; } = true;
        /// <summary>vars.onComplete: called each time the playhead reaches the end.</summary>
        public Action OnComplete { get; set; }

        public int Count => entries.Count;

        /// <summary>End of the last tween (totalDuration).</summary>
        public float Duration
        {
            get
            {
                float d = 0;
                foreach (var e in entries) d = Mathf.Max(d, e.start + e.duration);
                return d;
            }
        }

        /// <summary>timeline.to(target, duration, {x, ease, onComplete}, position).</summary>
        public void To(RectTransform target, float x, float duration, EaseFunction ease, float position, Action onComplete = null)
        {
            entries.Add(new Entry
            {
                target = target, to = x, duration = Mathf.Max(0, duration), ease = ease ?? Ease.QuadOut, start = position,
                onComplete = onComplete,
            });
            if (Completed && Time < Duration) Completed = false;
        }

        /// <summary>restart(): playhead to 0 (unpaused), earlier tweens back to their start values, no callbacks.</summary>
        public void Restart()
        {
            Paused = false;
            Time = 0;
            // rendering backwards to 0: the latest tweens first, so the earliest tween's start value wins
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                var e = entries[i];
                if (e.started && e.target != null) SetX(e.target, e.from);
                e.started = e.done = false;
            }
            Completed = entries.Count == 0 || Duration <= 0;
        }

        public void Pause() => Paused = true;
        public void Resume() => Paused = false;

        /// <summary>clear(): removes (kills) every tween; the playhead stays.</summary>
        public void Clear() => entries.Clear();

        /// <summary>Moves the playhead by <paramref name="dt"/> seconds and renders the tweens.</summary>
        public void Advance(float dt)
        {
            if (Paused || Completed) return;
            Time += dt;
            foreach (var e in entries.ToArray())
            {
                if (e.done || !entries.Contains(e)) continue;
                float local = Time - e.start;
                if (local < 0) continue;
                if (e.target == null)
                {
                    e.done = true;
                    continue;
                }
                if (!e.started) Begin(e);
                bool end = local >= e.duration;
                float ratio = end ? 1 : e.ease(local / e.duration);
                SetX(e.target, Mathf.LerpUnclamped(e.from, e.to, ratio));
                if (end)
                {
                    e.done = true;
                    e.onComplete?.Invoke();
                }
            }
            float total = Duration;
            if (Time >= total)
            {
                Time = total;
                Completed = true;
                OnComplete?.Invoke();
            }
        }

        void Begin(Entry e)
        {
            e.started = true;
            // overwrite "auto": a running tween of the same property on the same target loses it (and so is killed)
            foreach (var other in entries)
                if (other != e && other.target == e.target && other.started && !other.done) other.done = true;
            e.from = e.target.anchoredPosition.x;
        }

        static void SetX(RectTransform rt, float x) => rt.anchoredPosition = new Vector2(x, rt.anchoredPosition.y);
    }
}
