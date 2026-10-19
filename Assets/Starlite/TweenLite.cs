// GreenSock TweenLite / TimelineLite (v12 subset) emulation.
//   AS3: TweenLite.to(mc, 0.5, {x: 100, ease: Expo.easeOut, onComplete: done, onCompleteParams: [1]});
//   C#:  TweenLite.to(mc, 0.5f, new Vars { ["x"] = 100, ["ease"] = Expo.easeOut, ["onComplete"] = (Action<int>)done, ["onCompleteParams"] = new object[] { 1 } });
// Overwrite mode is GreenSock's default "auto": when a tween starts it removes the same
// properties from other active tweens of the same target.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Starlite
{
    /// <summary>Tween vars object (AS3 object literal).</summary>
    public class Vars : Dictionary<string, object>
    {
        public object onComplete { get => TryGetValue("onComplete", out var v) ? v : null; set => this["onComplete"] = value; }
    }

    public class Ease
    {
        readonly Func<float, float> mFunc;
        public Ease(Func<float, float> f) { mFunc = f; }
        public float getRatio(float p) => mFunc(p);
        public static implicit operator Func<float, float>(Ease e) => e.mFunc;
    }

    public static class Linear
    {
        public static readonly Ease easeNone = new Ease(p => p), ease = easeNone, easeIn = easeNone, easeOut = easeNone, easeInOut = easeNone;
    }

    public static class Quad_
    {
        public static readonly Ease easeIn = new Ease(p => p * p), easeOut = new Ease(p => p * (2 - p)),
            easeInOut = new Ease(p => p < 0.5f ? 2 * p * p : -1 + (4 - 2 * p) * p);
    }

    public static class Quint
    {
        public static readonly Ease easeIn = new Ease(p => p * p * p * p * p),
            easeOut = new Ease(p => { p -= 1; return p * p * p * p * p + 1; }),
            easeInOut = new Ease(p => { p *= 2; if (p < 1) return 0.5f * p * p * p * p * p; p -= 2; return 0.5f * (p * p * p * p * p + 2); });
    }

    public static class Expo
    {
        public static readonly Ease easeIn = new Ease(p => p == 0 ? 0 : Mathf.Pow(2, 10 * (p - 1)) - 0.001f),
            easeOut = new Ease(p => p == 1 ? 1 : 1 - Mathf.Pow(2, -10 * p)),
            easeInOut = new Ease(p => { p *= 2; return p < 1 ? 0.5f * Mathf.Pow(2, 10 * (p - 1)) : 0.5f * (2 - Mathf.Pow(2, -10 * (p - 1))); });
    }

    public static class Back
    {
        const float S = 1.70158f;
        public static readonly Ease easeIn = new Ease(p => p * p * ((S + 1) * p - S)),
            easeOut = new Ease(p => { p -= 1; return p * p * ((S + 1) * p + S) + 1; }),
            easeInOut = new Ease(p => { p *= 2; const float s = S * 1.525f; if (p < 1) return 0.5f * (p * p * ((s + 1) * p - s)); p -= 2; return 0.5f * (p * p * ((s + 1) * p + s) + 2); });
    }

    public static class Bounce
    {
        static float Out(float p)
        {
            if (p < 1 / 2.75f) return 7.5625f * p * p;
            if (p < 2 / 2.75f) { p -= 1.5f / 2.75f; return 7.5625f * p * p + 0.75f; }
            if (p < 2.5f / 2.75f) { p -= 2.25f / 2.75f; return 7.5625f * p * p + 0.9375f; }
            p -= 2.625f / 2.75f;
            return 7.5625f * p * p + 0.984375f;
        }
        public static readonly Ease easeOut = new Ease(Out), easeIn = new Ease(p => 1 - Out(1 - p)),
            easeInOut = new Ease(p => p < 0.5f ? (1 - Out(1 - p * 2)) * 0.5f : Out(p * 2 - 1) * 0.5f + 0.5f);
    }

    public static class Elastic
    {
        const float P = 0.3f, A = 1f;
        static readonly float S = P / (2 * Mathf.PI) * Mathf.Asin(1 / A);
        public static readonly Ease easeOut = new Ease(p => A * Mathf.Pow(2, -10 * p) * Mathf.Sin((p - S) * (2 * Mathf.PI) / P) + 1),
            easeIn = new Ease(p => { p -= 1; return -(A * Mathf.Pow(2, 10 * p) * Mathf.Sin((p - S) * (2 * Mathf.PI) / P)); }),
            easeInOut = new Ease(p =>
            {
                p = p * 2 - 1;
                const float p2 = P * 1.5f;
                float s2 = p2 / (2 * Mathf.PI) * Mathf.Asin(1 / A);
                return p < 0 ? -0.5f * (A * Mathf.Pow(2, 10 * p) * Mathf.Sin((p - s2) * (2 * Mathf.PI) / p2))
                             : A * Mathf.Pow(2, -10 * p) * Mathf.Sin((p - s2) * (2 * Mathf.PI) / p2) * 0.5f + 1;
            });
    }

    public abstract class Animation
    {
        public Vars vars;
        internal float startTime;     // position on the parent timeline
        internal float time;          // local playhead (negative while delayed)
        public float delay;
        protected float mDuration;
        internal bool mPaused, mKilled, mCompleted, mStarted;
        internal TimelineLite timeline; // null = root
        internal bool reversedFlag;

        public float duration() => mDuration;
        public virtual float totalDuration() => mDuration;
        public bool paused() => mPaused;
        public Animation pause() { mPaused = true; return this; }
        public Animation resume() { mPaused = false; return this; }
        public Animation play() { mPaused = false; return this; }
        public bool isActive() => !mPaused && !mKilled && !mCompleted && time >= 0;

        public Animation restart(bool includeDelay = false)
        {
            mPaused = false; mKilled = false; mCompleted = false;
            time = includeDelay ? -delay : 0;
            OnRestart();
            if (timeline == null) TweenEngine.Add(this);
            Render(time, true);
            return this;
        }

        protected internal virtual void OnRestart() { mStarted = false; }

        public virtual Animation kill(Vars vars = null, object target = null)
        {
            mKilled = true;
            TweenEngine.Remove(this);
            return this;
        }

        public float progress() => mDuration <= 0 ? 1 : Mathf.Clamp01(time / mDuration);

        internal abstract void Render(float t, bool force = false);

        protected void Call(string key)
        {
            if (vars == null || !vars.TryGetValue(key, out var cb) || cb == null) return;
            vars.TryGetValue(key + "Params", out var p);
            AS3.Invoke(cb as Delegate, p as object[] ?? (p is System.Collections.IList l ? ToArray(l) : null));
        }

        static object[] ToArray(System.Collections.IList l)
        {
            var a = new object[l.Count];
            l.CopyTo(a, 0);
            return a;
        }
    }

    public class TweenLite : Animation
    {
        public readonly object target;
        Func<float, float> mEase;
        readonly List<string> mProps = new List<string>();
        readonly List<float> mEnd = new List<float>();
        readonly List<string> mRelative = new List<string>();
        float[] mStart;
        static readonly HashSet<string> Reserved = new HashSet<string>
        {
            "ease", "delay", "onComplete", "onCompleteParams", "onUpdate", "onUpdateParams", "onStart", "onStartParams",
            "overwrite", "immediateRender", "paused", "useFrames", "onCompleteScope", "onUpdateScope", "onStartScope",
            "autoAlpha", "visible", "onReverseComplete", "onReverseCompleteParams", "repeat", "yoyo", "data", "lazy", "callbackScope"
        };

        public TweenLite(object target, float duration, Vars vars)
        {
            this.target = target;
            mDuration = Math.Max(0, duration);
            this.vars = vars ?? new Vars();
            delay = this.vars.TryGetValue("delay", out var d) ? Convert.ToSingle(d) : 0;
            mEase = this.vars.TryGetValue("ease", out var e) ? (e is Ease ee ? (Func<float, float>)ee : e as Func<float, float>) : null;
            if (mEase == null) mEase = Quad_.easeOut; // GreenSock default ease (Power1.easeOut)
            foreach (var kv in this.vars)
            {
                if (Reserved.Contains(kv.Key)) continue;
                if (kv.Value is string s && (s.StartsWith("+=") || s.StartsWith("-=")))
                {
                    mProps.Add(kv.Key); mEnd.Add(AS3.toNumber(s.Substring(2)) * (s[0] == '-' ? -1 : 1)); mRelative.Add(kv.Key);
                }
                else if (kv.Value is IConvertible)
                {
                    mProps.Add(kv.Key); mEnd.Add(Convert.ToSingle(kv.Value));
                }
            }
            if (this.vars.TryGetValue("autoAlpha", out var aa)) { mProps.Add("alpha"); mEnd.Add(Convert.ToSingle(aa)); }
            mPaused = this.vars.TryGetValue("paused", out var pz) && Convert.ToBoolean(pz);
            time = -delay;
            TweenEngine.Register(this);
            TweenEngine.Add(this);
        }

        public static TweenLite to(object target, float duration, Vars vars) => new TweenLite(target, duration, vars);

        public static TweenLite from(object target, float duration, Vars vars)
        {
            // swap: animate from the given values to the current ones
            var current = new Vars();
            var fromVars = new Vars();
            foreach (var kv in vars)
            {
                if (!Reserved.Contains(kv.Key) && kv.Value is IConvertible && !(kv.Value is string))
                {
                    current[kv.Key] = PropertyAccess.Get(target, kv.Key);
                    PropertyAccess.Set(target, kv.Key, Convert.ToSingle(kv.Value));
                }
                else fromVars[kv.Key] = kv.Value;
            }
            foreach (var kv in current) fromVars[kv.Key] = kv.Value;
            return new TweenLite(target, duration, fromVars);
        }

        public static TweenLite delayedCall(float delay, Delegate callback, object[] parameters = null, bool useFrames = false)
        {
            return new TweenLite(callback, 0, new Vars { ["delay"] = delay, ["onComplete"] = callback, ["onCompleteParams"] = parameters, ["overwrite"] = false });
        }

        public static TweenLite delayedCall(float delay, Action callback) => delayedCall(delay, (Delegate)callback);

        public static void killTweensOf(object target, bool onlyActive = false, Vars vars = null)
        {
            foreach (var t in TweenEngine.TweensOf(target))
            {
                if (onlyActive && !t.isActive()) continue;
                if (vars != null) t.KillProps(vars.Keys);
                else t.kill();
            }
        }

        public static void killDelayedCallsTo(Delegate func) => killTweensOf(func);
        public static void killDelayedCallsTo(Action func) => killTweensOf(func);

        public static List<TweenLite> getTweensOf(object target) => TweenEngine.TweensOf(target);

        internal void KillProps(IEnumerable<string> props)
        {
            foreach (var p in props)
            {
                int i = mProps.IndexOf(p);
                if (i < 0) continue;
                mProps.RemoveAt(i);
                mEnd.RemoveAt(i);
                if (mStart != null)
                {
                    var l = new List<float>(mStart);
                    l.RemoveAt(i);
                    mStart = l.ToArray();
                }
            }
            if (mProps.Count == 0 && !(target is Delegate)) kill();
        }

        public override Animation kill(Vars vars = null, object target = null)
        {
            if (vars != null) { KillProps(vars.Keys); return this; }
            base.kill();
            TweenEngine.Unregister(this);
            timeline?.RemoveChild(this);
            return this;
        }

        protected internal override void OnRestart()
        {
            base.OnRestart();
            if (mStart != null) for (int i = 0; i < mProps.Count; i++) PropertyAccess.Set(target, mProps[i], mStart[i]);
            mStart = null;
            TweenEngine.Register(this);
        }

        void Init()
        {
            mStarted = true;
            bool overwrite = !(vars.TryGetValue("overwrite", out var ow) && (ow is bool b && !b || ow is int n && n == 0 || ow as string == "none"));
            if (overwrite && mProps.Count > 0)
            {
                foreach (var other in TweenEngine.TweensOf(target))
                    if (other != this && other.mStarted && !other.mCompleted && !other.mKilled) other.KillProps(mProps);
            }
            mStart = new float[mProps.Count];
            for (int i = 0; i < mProps.Count; i++)
            {
                mStart[i] = target is Delegate ? 0 : PropertyAccess.Get(target, mProps[i]);
                if (mRelative.Contains(mProps[i])) mEnd[i] += mStart[i];
            }
            Call("onStart");
        }

        internal override void Render(float t, bool force = false)
        {
            time = t;
            if (mKilled || mCompleted) return;
            if (t < 0 && !force) return;
            if (t < 0) return;
            if (!mStarted) Init();
            float ratio = mDuration <= 0 ? 1 : mEase(Mathf.Clamp01(t / mDuration));
            if (mDuration > 0 && t >= mDuration) ratio = 1;
            for (int i = 0; i < mProps.Count && i < mStart.Length; i++)
                PropertyAccess.Set(target, mProps[i], mStart[i] + (mEnd[i] - mStart[i]) * ratio);
            if (vars.TryGetValue("visible", out var vis) && target is DisplayObject vd && t >= mDuration) vd.visible = Convert.ToBoolean(vis);
            if (vars.ContainsKey("autoAlpha") && target is DisplayObject ad) ad.visible = ad.alpha > 0;
            Call("onUpdate");
            if (t >= mDuration)
            {
                mCompleted = true;
                if (timeline == null) TweenEngine.Remove(this);
                TweenEngine.Unregister(this);
                Call("onComplete");
            }
        }
    }

    public class TimelineLite : Animation
    {
        readonly List<Animation> mChildren = new List<Animation>();

        public TimelineLite(Vars vars = null)
        {
            this.vars = vars ?? new Vars();
            delay = this.vars.TryGetValue("delay", out var d) ? Convert.ToSingle(d) : 0;
            mPaused = this.vars.TryGetValue("paused", out var pz) && Convert.ToBoolean(pz);
            time = -delay;
            TweenEngine.Add(this);
        }

        public override float totalDuration()
        {
            float end = 0;
            foreach (var c in mChildren) end = Math.Max(end, c.startTime + c.delay + c.totalDuration());
            return end;
        }

        public TimelineLite add(Animation child, float? position = null)
        {
            TweenEngine.Remove(child);
            child.timeline = this;
            child.startTime = position ?? totalDuration();
            child.time = -child.delay;
            mChildren.Add(child);
            mDuration = totalDuration();
            if (mCompleted && !mKilled)
            {
                // GreenSock re-enables a finished timeline when it gets new children; the playhead stays put
                mCompleted = false;
                if (timeline == null) TweenEngine.Add(this);
            }
            return this;
        }

        public TimelineLite append(Animation child, float offset = 0) => add(child, totalDuration() + offset);
        public TimelineLite insert(Animation child, float time = 0) => add(child, time);

        public TimelineLite to(object target, float duration, Vars vars, float? position = null)
        {
            var t = new TweenLite(target, duration, vars);
            return add(t, position);
        }

        public TimelineLite call(Delegate callback, object[] parameters = null, float? position = null) =>
            add(TweenLite.delayedCall(0, callback, parameters), position);

        internal void RemoveChild(Animation a) => mChildren.Remove(a);

        public TimelineLite clear(bool labels = true)
        {
            foreach (var c in mChildren.ToArray()) { c.timeline = null; c.kill(); }
            mChildren.Clear();
            mDuration = 0;
            return this;
        }

        public List<Animation> getChildren() => new List<Animation>(mChildren);

        protected internal override void OnRestart()
        {
            base.OnRestart();
            // restart children in reverse so the earliest tween's start values win
            for (int i = mChildren.Count - 1; i >= 0; i--)
            {
                var c = mChildren[i];
                c.mCompleted = false; c.mKilled = false;
                c.time = -c.delay;
                c.OnRestart();
            }
        }

        internal override void Render(float t, bool force = false)
        {
            time = t;
            if (mKilled || mCompleted || t < 0) return;
            if (!mStarted) { mStarted = true; Call("onStart"); }
            foreach (var c in mChildren.ToArray())
            {
                if (c.mPaused) continue;
                float local = t - c.startTime - c.delay;
                if (local >= 0 || force) c.Render(local, false);
            }
            Call("onUpdate");
            if (t >= totalDuration())
            {
                mCompleted = true;
                TweenEngine.Remove(this);
                Call("onComplete");
            }
        }
    }

    /// <summary>Root timeline: advances all top-level tweens each frame.</summary>
    public static class TweenEngine
    {
        static readonly List<Animation> sActive = new List<Animation>();
        static readonly Dictionary<object, List<TweenLite>> sByTarget = new Dictionary<object, List<TweenLite>>();

        internal static void Add(Animation a) { if (!sActive.Contains(a)) sActive.Add(a); }
        internal static void Remove(Animation a) => sActive.Remove(a);

        internal static void Register(TweenLite t)
        {
            if (t.target == null) return;
            if (!sByTarget.TryGetValue(t.target, out var l)) sByTarget[t.target] = l = new List<TweenLite>();
            if (!l.Contains(t)) l.Add(t);
        }

        internal static void Unregister(TweenLite t)
        {
            if (t.target != null && sByTarget.TryGetValue(t.target, out var l))
            {
                l.Remove(t);
                if (l.Count == 0) sByTarget.Remove(t.target);
            }
        }

        internal static List<TweenLite> TweensOf(object target)
        {
            if (target == null) return new List<TweenLite>();
            return sByTarget.TryGetValue(target, out var l) ? new List<TweenLite>(l) : new List<TweenLite>();
        }

        public static void Advance(float dt)
        {
            foreach (var a in sActive.ToArray())
            {
                if (a.mPaused || a.mKilled) continue;
                a.Render(a.time + dt);
            }
        }

        public static void KillAll() { sActive.Clear(); sByTarget.Clear(); }
    }
}
