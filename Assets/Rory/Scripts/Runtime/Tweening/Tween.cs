using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rory.Tweening
{
    /// <summary>
    /// A running tween. Created by <see cref="Tweens"/> or the extension methods in <see cref="TweenExtensions"/>;
    /// it starts immediately (after its delay) and is removed when done or when its target is destroyed.
    /// </summary>
    public class Tween
    {
        internal UnityEngine.Object target;
        internal bool hasTarget;
        internal Action<float> apply;
        internal Action captureStart;
        internal float elapsed;
        bool started;

        public float Duration { get; internal set; }
        public float Delay { get; private set; }
        public EaseFunction EaseFn { get; private set; } = Ease.QuadOut;
        public bool Unscaled { get; private set; }
        public bool IsActive { get; internal set; } = true;
        public int Loops { get; private set; }
        public bool YoYo { get; private set; }

        Action onStart, onComplete;
        Action<float> onUpdate;

        public Tween SetEase(EaseFunction ease) { EaseFn = ease ?? Ease.Linear; return this; }
        public Tween SetDelay(float seconds) { Delay = Mathf.Max(0, seconds); return this; }
        /// <summary>Runs on unscaled time (keeps going while Time.timeScale = 0, e.g. pause menus).</summary>
        public Tween SetUnscaled(bool unscaled = true) { Unscaled = unscaled; return this; }
        /// <summary>Repeats the tween; -1 = forever. With <paramref name="yoyo"/> every other loop runs backwards.</summary>
        public Tween SetLoops(int loops, bool yoyo = false) { Loops = loops; YoYo = yoyo; return this; }
        public Tween OnStart(Action a) { onStart += a; return this; }
        public Tween OnUpdate(Action<float> a) { onUpdate += a; return this; }
        public Tween OnComplete(Action a) { onComplete += a; return this; }

        /// <summary>Stops the tween; with <paramref name="complete"/> it jumps to the end and calls OnComplete.</summary>
        public void Kill(bool complete = false)
        {
            if (!IsActive) return;
            IsActive = false;
            if (complete && TargetAlive)
            {
                Begin();
                apply?.Invoke(EaseFn(1));
                onComplete?.Invoke();
            }
        }

        internal bool TargetAlive => !hasTarget || target != null;

        void Begin()
        {
            if (started) return;
            started = true;
            captureStart?.Invoke();
            onStart?.Invoke();
        }

        /// <summary>Advances the tween; returns false when it is finished.</summary>
        internal bool Step(float dt)
        {
            if (!IsActive) return false;
            if (!TargetAlive) { IsActive = false; return false; }
            elapsed += dt;
            if (elapsed < Delay) return true;
            Begin();
            float local = elapsed - Delay;
            int runs = Loops + 1; // Loops < 0 = forever
            if (Duration > 0 && (Loops < 0 || local < Duration * runs))
            {
                int loop = (int)(local / Duration);
                float t = (local - loop * Duration) / Duration;
                Apply(YoYo && loop % 2 == 1 ? 1 - t : t);
                return true;
            }
            Apply(YoYo && Loops > 0 && Loops % 2 == 1 ? 0 : 1);
            IsActive = false;
            onComplete?.Invoke();
            return false;
        }

        void Apply(float t)
        {
            float v = EaseFn(t);
            apply?.Invoke(v);
            onUpdate?.Invoke(v);
        }
    }

    /// <summary>Lays tweens out one after another by adjusting their delays (TimelineLite.append).</summary>
    public class Sequence
    {
        public float Length { get; private set; }

        /// <summary>Starts <paramref name="tween"/> after everything appended so far.</summary>
        public Sequence Append(Tween tween)
        {
            tween.SetDelay(Length + tween.Delay);
            Length = tween.Delay + tween.Duration;
            return this;
        }

        /// <summary>Starts <paramref name="tween"/> at <paramref name="time"/> seconds into the sequence.</summary>
        public Sequence Insert(float time, Tween tween)
        {
            tween.SetDelay(time + tween.Delay);
            Length = Mathf.Max(Length, tween.Delay + tween.Duration);
            return this;
        }

        public Sequence AppendInterval(float seconds)
        {
            Length += seconds;
            return this;
        }

        public Sequence AppendCallback(Action callback, UnityEngine.Object owner = null)
        {
            Tweens.DelayedCall(Length, callback, owner);
            return this;
        }
    }

    public static class Tweens
    {
        /// <summary>Generic tween: <paramref name="apply"/> receives the eased progress 0..1.</summary>
        public static Tween To(UnityEngine.Object target, float duration, Action<float> apply, Action captureStart = null)
        {
            var t = new Tween
            {
                target = target, hasTarget = target != null, apply = apply, captureStart = captureStart, Duration = duration,
            };
            TweenRunner.Add(t);
            return t;
        }

        public static Tween Float(UnityEngine.Object target, float from, float to, float duration, Action<float> setter) =>
            To(target, duration, p => setter(Mathf.LerpUnclamped(from, to, p)));

        /// <summary>Calls <paramref name="callback"/> after <paramref name="delay"/> seconds (TweenLite.delayedCall).
        /// With an <paramref name="owner"/> the call is dropped if the owner is destroyed first.</summary>
        public static Tween DelayedCall(float delay, Action callback, UnityEngine.Object owner = null) =>
            To(owner, 0, null).SetDelay(delay).OnComplete(callback);

        /// <summary>Stops every tween on <paramref name="target"/> (TweenLite.killTweensOf).</summary>
        public static void Kill(UnityEngine.Object target, bool complete = false) => TweenRunner.KillTarget(target, complete);

        public static bool IsTweening(UnityEngine.Object target) => TweenRunner.HasTarget(target);

        public static Sequence Sequence() => new Sequence();
    }

    /// <summary>Hidden object that steps all tweens every frame.</summary>
    [AddComponentMenu("")]
    public class TweenRunner : MonoBehaviour
    {
        static TweenRunner instance;
        static readonly List<Tween> tweens = new List<Tween>();
        static readonly List<Tween> adding = new List<Tween>();
        static bool stepping;

        static void Ensure()
        {
            if (instance != null || !Application.isPlaying) return;
            var go = new GameObject("[Tweens]") { hideFlags = HideFlags.HideInHierarchy };
            DontDestroyOnLoad(go);
            instance = go.AddComponent<TweenRunner>();
        }

        internal static void Add(Tween t)
        {
            Ensure();
            (stepping ? adding : tweens).Add(t);
        }

        internal static void KillTarget(UnityEngine.Object target, bool complete)
        {
            foreach (var t in tweens.ToArray()) if (t.IsActive && t.hasTarget && t.target == target) t.Kill(complete);
            foreach (var t in adding.ToArray()) if (t.IsActive && t.hasTarget && t.target == target) t.Kill(complete);
        }

        internal static bool HasTarget(UnityEngine.Object target) =>
            tweens.Exists(t => t.IsActive && t.hasTarget && t.target == target) ||
            adding.Exists(t => t.IsActive && t.hasTarget && t.target == target);

        void Update()
        {
            stepping = true;
            float dt = Time.deltaTime, udt = Time.unscaledDeltaTime;
            for (int i = 0; i < tweens.Count; i++)
            {
                var t = tweens[i];
                bool alive;
                try
                {
                    alive = t.Step(t.Unscaled ? udt : dt);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    alive = false;
                }
                if (!alive) t.IsActive = false;
            }
            tweens.RemoveAll(t => !t.IsActive);
            stepping = false;
            if (adding.Count > 0)
            {
                tweens.AddRange(adding);
                adding.Clear();
            }
        }

        void OnDestroy()
        {
            if (instance == this) instance = null;
        }

        /// <summary>Clears all tweens (used by tests between runs).</summary>
        public static void Clear()
        {
            tweens.Clear();
            adding.Clear();
        }
    }
}
