using System;
using System.Collections.Generic;
using Rory.Tweening;
using UnityEngine;

namespace Rory.Screens.Match3
{
    /// <summary>Something the board tweens (tiles, ice overlays): stage x / y and uniform scale.</summary>
    public interface IBoardTweenTarget
    {
        float X { get; set; }
        float Y { get; set; }
        float Scale { get; set; }
    }

    [Flags]
    public enum BoardTweenProps
    {
        None = 0,
        X = 1,
        Y = 2,
        Scale = 4,
        Position = X | Y,
    }

    /// <summary>
    /// The board's tweens, with the TweenLite semantics the board logic depends on (the screen's general tweens use
    /// Rory.Tweening): a tween is created now and first rendered on the next <see cref="Step"/> (start values are read
    /// then); when it starts it removes its properties from other running tweens of the same target ("auto" overwrite),
    /// and a tween left without properties is dead and never calls its onComplete; killing properties
    /// (killTweensOf(target, false, {scaleX, scaleY})) works the same way. Stepped by the board with real time, also
    /// while the screen is paused (TweenLite kept running when the game's Update stopped).
    /// </summary>
    public class BoardTweens
    {
        class T
        {
            public IBoardTweenTarget target;
            public BoardTweenProps props;
            public float ex, ey, es;
            public float sx, sy, ss;
            public float duration;
            public float time;
            public EaseFunction ease;
            public Action onComplete;
            public bool started, completed, killed;
        }

        readonly List<T> tweens = new List<T>();

        public int Count => tweens.Count;

        /// <summary>
        /// TweenLite.to(target, duration, {props...}). Values for properties not in <paramref name="props"/> are ignored.
        /// </summary>
        public void To(IBoardTweenTarget target, float duration, BoardTweenProps props, float x = 0, float y = 0, float scale = 0,
            EaseFunction ease = null, float delay = 0, Action onComplete = null)
        {
            tweens.Add(new T
            {
                target = target, props = props, ex = x, ey = y, es = scale, duration = Mathf.Max(0, duration), time = -delay,
                ease = ease ?? Ease.QuadOut, onComplete = onComplete,
            });
        }

        /// <summary>killTweensOf(target, false, {props}): removes the properties from every tween of the target.</summary>
        public void KillProps(IBoardTweenTarget target, BoardTweenProps props)
        {
            foreach (var t in tweens)
                if (t.target == target && !t.killed && !t.completed) RemoveProps(t, props);
        }

        /// <summary>killTweensOf(target).</summary>
        public void Kill(IBoardTweenTarget target)
        {
            foreach (var t in tweens)
                if (t.target == target) t.killed = true;
        }

        public void KillAll()
        {
            foreach (var t in tweens) t.killed = true;
            tweens.Clear();
        }

        /// <summary>True while a live tween animates any of <paramref name="props"/> of the target.</summary>
        public bool IsTweening(IBoardTweenTarget target, BoardTweenProps props = (BoardTweenProps)7)
        {
            foreach (var t in tweens)
                if (t.target == target && !t.killed && !t.completed && (t.props & props) != 0) return true;
            return false;
        }

        static void RemoveProps(T t, BoardTweenProps props)
        {
            t.props &= ~props;
            if (t.props == BoardTweenProps.None) t.killed = true;
        }

        /// <summary>Advances every tween that existed before this call (tweens created meanwhile start next step).</summary>
        public void Step(float dt)
        {
            if (tweens.Count == 0) return;
            var snapshot = tweens.ToArray();
            foreach (var t in snapshot)
            {
                if (t.killed || t.completed) continue;
                Render(t, t.time + dt);
            }
            tweens.RemoveAll(t => t.killed || t.completed);
        }

        void Render(T t, float time)
        {
            t.time = time;
            if (time < 0) return;
            if (!t.started)
            {
                t.started = true;
                foreach (var other in tweens)
                    if (other != t && other.target == t.target && other.started && !other.completed && !other.killed)
                        RemoveProps(other, t.props);
                t.sx = t.target.X;
                t.sy = t.target.Y;
                t.ss = t.target.Scale;
            }
            float ratio = t.duration <= 0 ? 1 : t.ease(Mathf.Clamp01(time / t.duration));
            if (t.duration > 0 && time >= t.duration) ratio = 1;
            if ((t.props & BoardTweenProps.X) != 0) t.target.X = t.sx + (t.ex - t.sx) * ratio;
            if ((t.props & BoardTweenProps.Y) != 0) t.target.Y = t.sy + (t.ey - t.sy) * ratio;
            if ((t.props & BoardTweenProps.Scale) != 0) t.target.Scale = t.ss + (t.es - t.ss) * ratio;
            if (time >= t.duration)
            {
                t.completed = true;
                t.onComplete?.Invoke();
            }
        }
    }
}
