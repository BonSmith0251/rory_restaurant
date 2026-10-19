using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Starlite
{
    /// <summary>Reads/writes numeric properties by name (fast path for display objects, reflection otherwise).</summary>
    public static class PropertyAccess
    {
        public static float Get(object target, string prop)
        {
            if (target is DisplayObject d)
            {
                switch (prop)
                {
                    case "x": return d.x;
                    case "y": return d.y;
                    case "alpha": return d.alpha;
                    case "scaleX": return d.scaleX;
                    case "scaleY": return d.scaleY;
                    case "scale": return d.scaleX;
                    case "rotation": return d.rotation;
                    case "pivotX": return d.pivotX;
                    case "pivotY": return d.pivotY;
                    case "width": return d.width;
                    case "height": return d.height;
                    case "skewX": return d.skewX;
                    case "skewY": return d.skewY;
                }
            }
            var m = Find(target, prop, out var f, out var p);
            object v = f != null ? f.GetValue(IsStatic(target) ? null : target) : p?.GetValue(IsStatic(target) ? null : target);
            return v == null ? 0 : Convert.ToSingle(v);
        }

        public static void Set(object target, string prop, float value)
        {
            if (target is DisplayObject d)
            {
                switch (prop)
                {
                    case "x": d.x = value; return;
                    case "y": d.y = value; return;
                    case "alpha": d.alpha = value; return;
                    case "scaleX": d.scaleX = value; return;
                    case "scaleY": d.scaleY = value; return;
                    case "scale": d.scaleX = d.scaleY = value; return;
                    case "rotation": d.rotation = value; return;
                    case "pivotX": d.pivotX = value; return;
                    case "pivotY": d.pivotY = value; return;
                    case "width": d.width = value; return;
                    case "height": d.height = value; return;
                    case "skewX": d.skewX = value; return;
                    case "skewY": d.skewY = value; return;
                }
            }
            Find(target, prop, out var f, out var p);
            object inst = IsStatic(target) ? null : target;
            if (f != null) f.SetValue(inst, ConvertTo(value, f.FieldType));
            else if (p != null && p.CanWrite) p.SetValue(inst, ConvertTo(value, p.PropertyType));
            else Debug.LogWarning($"[Starlite] cannot tween '{prop}' on {target}");
        }

        public static bool Has(object target, string prop)
        {
            if (target is DisplayObject && Array.IndexOf(DisplayProps, prop) >= 0) return true;
            return Find(target, prop, out var f, out var p) != null;
        }

        static readonly string[] DisplayProps = { "x", "y", "alpha", "scaleX", "scaleY", "scale", "rotation", "pivotX", "pivotY", "width", "height", "skewX", "skewY" };

        static bool IsStatic(object t) => t is Type;

        static object ConvertTo(float v, Type t)
        {
            if (t == typeof(float)) return v;
            if (t == typeof(int)) return (int)v;
            if (t == typeof(uint)) return (uint)Math.Max(0, v);
            if (t == typeof(double)) return (double)v;
            return Convert.ChangeType(v, t);
        }

        static readonly Dictionary<(Type, string), MemberInfo> sCache = new Dictionary<(Type, string), MemberInfo>();

        static MemberInfo Find(object target, string prop, out FieldInfo f, out PropertyInfo p)
        {
            f = null; p = null;
            if (target == null) return null;
            var type = target as Type ?? target.GetType();
            var flags = BindingFlags.Public | BindingFlags.NonPublic | (target is Type ? BindingFlags.Static : BindingFlags.Instance);
            if (!sCache.TryGetValue((type, prop), out var mi))
            {
                mi = (MemberInfo)type.GetField(prop, flags) ?? type.GetProperty(prop, flags);
                sCache[(type, prop)] = mi;
            }
            f = mi as FieldInfo;
            p = mi as PropertyInfo;
            return mi;
        }
    }

    public class Juggler : IAnimatable
    {
        readonly List<IAnimatable> mObjects = new List<IAnimatable>();
        float mElapsedTime;
        public float elapsedTime => mElapsedTime;

        public void add(IAnimatable obj)
        {
            if (obj == null || mObjects.Contains(obj)) return;
            mObjects.Add(obj);
            if (obj is EventDispatcher ed) ed.addEventListener(Event.REMOVE_FROM_JUGGLER, OnRemove);
        }

        public bool contains(IAnimatable obj) => mObjects.Contains(obj);

        public void remove(IAnimatable obj)
        {
            if (obj == null) return;
            if (obj is EventDispatcher ed) ed.removeEventListener(Event.REMOVE_FROM_JUGGLER, OnRemove);
            int i = mObjects.IndexOf(obj);
            if (i >= 0) mObjects[i] = null;
        }

        public void removeTweens(object target)
        {
            if (target == null) return;
            for (int i = mObjects.Count - 1; i >= 0; i--)
                if (mObjects[i] is Tween t && t.target == target) { t.removeEventListener(Event.REMOVE_FROM_JUGGLER, OnRemove); mObjects[i] = null; }
        }

        public bool containsTweens(object target)
        {
            foreach (var o in mObjects) if (o is Tween t && t.target == target) return true;
            return false;
        }

        public void purge()
        {
            for (int i = mObjects.Count - 1; i >= 0; i--)
            {
                if (mObjects[i] is EventDispatcher ed) ed.removeEventListener(Event.REMOVE_FROM_JUGGLER, OnRemove);
                mObjects[i] = null;
            }
        }

        public DelayedCall delayCall(Delegate call, float delay, params object[] args)
        {
            if (call == null) return null;
            var dc = new DelayedCall(call, delay, args);
            add(dc);
            return dc;
        }

        public DelayedCall delayCall(Action call, float delay) => delayCall((Delegate)call, delay);

        public DelayedCall repeatCall(Delegate call, float interval, int repeatCount = 0, params object[] args)
        {
            var dc = new DelayedCall(call, interval, args) { repeatCount = repeatCount };
            add(dc);
            return dc;
        }

        /// <summary>Starling's juggler.tween(target, time, {x: 10, transition: "easeOut", onComplete: f}).</summary>
        public Tween tween(object target, float time, Dictionary<string, object> properties)
        {
            var t = new Tween(target, time);
            foreach (var kv in properties)
            {
                switch (kv.Key)
                {
                    case "transition": t.transition = kv.Value as string ?? Transitions.LINEAR; break;
                    case "delay": t.delay = Convert.ToSingle(kv.Value); break;
                    case "repeatCount": t.repeatCount = Convert.ToInt32(kv.Value); break;
                    case "repeatDelay": t.repeatDelay = Convert.ToSingle(kv.Value); break;
                    case "reverse": t.reverse = Convert.ToBoolean(kv.Value); break;
                    case "roundToInt": t.roundToInt = Convert.ToBoolean(kv.Value); break;
                    case "onStart": t.onStart = kv.Value as Delegate; break;
                    case "onUpdate": t.onUpdate = kv.Value as Delegate; break;
                    case "onComplete": t.onComplete = kv.Value as Delegate; break;
                    case "onStartArgs": t.onStartArgs = kv.Value as object[]; break;
                    case "onUpdateArgs": t.onUpdateArgs = kv.Value as object[]; break;
                    case "onCompleteArgs": t.onCompleteArgs = kv.Value as object[]; break;
                    default: t.animate(kv.Key, Convert.ToSingle(kv.Value)); break;
                }
            }
            add(t);
            return t;
        }

        void OnRemove(Event e)
        {
            remove(e.target as IAnimatable);
            if (e.target is Tween t && t.isComplete && t.nextTween != null) add(t.nextTween);
        }

        public void advanceTime(float time)
        {
            int num = mObjects.Count;
            int cur = 0;
            mElapsedTime += time;
            if (num == 0) return;
            int i;
            for (i = 0; i < num; i++)
            {
                var obj = mObjects[i];
                if (obj != null)
                {
                    if (cur != i) { mObjects[cur] = obj; mObjects[i] = null; }
                    obj.advanceTime(time);
                    cur++;
                }
            }
            if (cur != i)
            {
                num = mObjects.Count; // objects may have been added during the loop
                while (i < num) mObjects[cur++] = mObjects[i++];
                mObjects.RemoveRange(cur, mObjects.Count - cur);
            }
        }
    }

    public class DelayedCall : EventDispatcher, IAnimatable
    {
        float mCurrentTime, mTotalTime;
        Delegate mCall;
        object[] mArgs;
        public int repeatCount = 1;

        public DelayedCall(Delegate call, float delay, object[] args = null) { reset(call, delay, args); }

        public DelayedCall reset(Delegate call, float delay, object[] args = null)
        {
            mCurrentTime = 0; mTotalTime = Math.Max(delay, 0.0001f); mCall = call; mArgs = args; repeatCount = 1;
            return this;
        }

        public bool isComplete => repeatCount == 1 && mCurrentTime >= mTotalTime;
        public float totalTime => mTotalTime;
        public float currentTime => mCurrentTime;

        public void advanceTime(float time)
        {
            float prev = mCurrentTime;
            mCurrentTime = Math.Min(mTotalTime, mCurrentTime + time);
            if (prev < mTotalTime && mCurrentTime >= mTotalTime)
            {
                if (repeatCount == 0 || repeatCount > 1)
                {
                    AS3.Invoke(mCall, mArgs);
                    if (repeatCount > 0) repeatCount -= 1;
                    mCurrentTime = 0;
                    advanceTime(prev + time - mTotalTime);
                }
                else
                {
                    var call = mCall;
                    var args = mArgs;
                    dispatchEventWith(Event.REMOVE_FROM_JUGGLER);
                    AS3.Invoke(call, args);
                }
            }
        }
    }

    public class Tween : EventDispatcher, IAnimatable
    {
        public object target { get; private set; }
        string mTransitionName;
        Func<float, float> mTransitionFunc;
        readonly List<string> mProps = new List<string>();
        readonly List<float> mStart = new List<float>();
        readonly List<float> mEnd = new List<float>();
        float mTotalTime, mCurrentTime, mProgress;
        public float delay;
        public bool roundToInt, reverse;
        public int repeatCount = 1;
        public float repeatDelay;
        public Delegate onStart, onUpdate, onRepeat, onComplete;
        public object[] onStartArgs, onUpdateArgs, onRepeatArgs, onCompleteArgs;
        public Tween nextTween;
        int mCurrentCycle;

        public Tween(object target, float time, object transition = null) { reset(target, time, transition); }

        public Tween reset(object target, float time, object transition = null)
        {
            this.target = target;
            mCurrentTime = 0; mTotalTime = Math.Max(0.0001f, time); mProgress = 0; delay = repeatDelay = 0;
            onStart = onUpdate = onComplete = onRepeat = null; onStartArgs = onUpdateArgs = onCompleteArgs = onRepeatArgs = null;
            roundToInt = reverse = false; repeatCount = 1; mCurrentCycle = -1; nextTween = null;
            if (transition is Func<float, float> f) { mTransitionFunc = f; mTransitionName = "custom"; }
            else this.transition = transition as string ?? Transitions.LINEAR;
            mProps.Clear(); mStart.Clear(); mEnd.Clear();
            return this;
        }

        public string transition
        {
            get => mTransitionName;
            set { mTransitionName = value; mTransitionFunc = Transitions.getTransition(value); }
        }

        public Func<float, float> transitionFunc { get => mTransitionFunc; set { mTransitionFunc = value; mTransitionName = "custom"; } }

        public void animate(string property, float endValue)
        {
            if (target == null) return;
            mProps.Add(property);
            mStart.Add(float.NaN);
            mEnd.Add(endValue);
        }

        public void scaleTo(float f) { animate("scaleX", f); animate("scaleY", f); }
        public void moveTo(float x, float y) { animate("x", x); animate("y", y); }
        public void fadeTo(float a) => animate("alpha", a);
        public void rotateTo(float angle, string type = "rad") => animate("rotation", type == "deg" ? angle * Mathf.Deg2Rad : angle);

        public float getEndValue(string property) { int i = mProps.IndexOf(property); return i >= 0 ? mEnd[i] : float.NaN; }

        public void advanceTime(float time)
        {
            if (time == 0 || (repeatCount == 1 && mCurrentTime == mTotalTime)) return;
            float previousTime = mCurrentTime;
            float restTime = mTotalTime - mCurrentTime;
            float carryOverTime = time > restTime ? time - restTime : 0;
            mCurrentTime += time;
            if (mCurrentTime <= 0) return;
            if (mCurrentTime > mTotalTime) mCurrentTime = mTotalTime;
            if (mCurrentCycle < 0 && previousTime <= 0 && mCurrentTime > 0)
            {
                mCurrentCycle++;
                AS3.Invoke(onStart, onStartArgs);
            }
            float ratio = mCurrentTime / mTotalTime;
            bool reversed = reverse && (mCurrentCycle % 2 == 1);
            mProgress = reversed ? mTransitionFunc(1 - ratio) : mTransitionFunc(ratio);
            for (int i = 0; i < mProps.Count; i++)
            {
                if (float.IsNaN(mStart[i])) mStart[i] = PropertyAccess.Get(target, mProps[i]);
                float v = mStart[i] + mProgress * (mEnd[i] - mStart[i]);
                if (roundToInt) v = Mathf.Round(v);
                PropertyAccess.Set(target, mProps[i], v);
            }
            AS3.Invoke(onUpdate, onUpdateArgs);
            if (previousTime < mTotalTime && mCurrentTime >= mTotalTime)
            {
                if (repeatCount == 0 || repeatCount > 1)
                {
                    mCurrentTime = -repeatDelay;
                    mCurrentCycle++;
                    if (repeatCount > 1) repeatCount--;
                    AS3.Invoke(onRepeat, onRepeatArgs);
                }
                else
                {
                    var cb = onComplete;
                    var args = onCompleteArgs;
                    dispatchEventWith(Event.REMOVE_FROM_JUGGLER);
                    AS3.Invoke(cb, args);
                }
            }
            if (carryOverTime > 0) advanceTime(carryOverTime);
        }

        public bool isComplete => mCurrentTime >= mTotalTime && repeatCount == 1;
        public float totalTime => mTotalTime;
        public float currentTime => mCurrentTime;
        public float progress => mProgress;
    }

    public static class Transitions
    {
        public const string LINEAR = "linear", EASE_IN = "easeIn", EASE_OUT = "easeOut", EASE_IN_OUT = "easeInOut",
            EASE_OUT_IN = "easeOutIn", EASE_IN_BACK = "easeInBack", EASE_OUT_BACK = "easeOutBack",
            EASE_IN_OUT_BACK = "easeInOutBack", EASE_OUT_IN_BACK = "easeOutInBack", EASE_IN_ELASTIC = "easeInElastic",
            EASE_OUT_ELASTIC = "easeOutElastic", EASE_IN_OUT_ELASTIC = "easeInOutElastic",
            EASE_OUT_IN_ELASTIC = "easeOutInElastic", EASE_IN_BOUNCE = "easeInBounce", EASE_OUT_BOUNCE = "easeOutBounce",
            EASE_IN_OUT_BOUNCE = "easeInOutBounce", EASE_OUT_IN_BOUNCE = "easeOutInBounce";

        static readonly Dictionary<string, Func<float, float>> sTransitions = new Dictionary<string, Func<float, float>>
        {
            [LINEAR] = r => r,
            [EASE_IN] = r => r * r * r,
            [EASE_OUT] = r => { float i = r - 1; return i * i * i + 1; },
            [EASE_IN_OUT] = r => Combined(r, sTrans(EASE_IN), sTrans(EASE_OUT)),
            [EASE_OUT_IN] = r => Combined(r, sTrans(EASE_OUT), sTrans(EASE_IN)),
            [EASE_IN_BACK] = r => { const float s = 1.70158f; return (float)(Math.Pow(r, 2) * ((s + 1) * r - s)); },
            [EASE_OUT_BACK] = r => { float i = r - 1; const float s = 1.70158f; return (float)(Math.Pow(i, 2) * ((s + 1) * i + s) + 1); },
            [EASE_IN_OUT_BACK] = r => Combined(r, sTrans(EASE_IN_BACK), sTrans(EASE_OUT_BACK)),
            [EASE_OUT_IN_BACK] = r => Combined(r, sTrans(EASE_OUT_BACK), sTrans(EASE_IN_BACK)),
            [EASE_IN_ELASTIC] = r =>
            {
                if (r == 0 || r == 1) return r;
                const float p = 0.3f, s = p / 4;
                float i = r - 1;
                return (float)(-1 * Math.Pow(2, 10 * i) * Math.Sin((i - s) * (2 * Math.PI) / p));
            },
            [EASE_OUT_ELASTIC] = r =>
            {
                if (r == 0 || r == 1) return r;
                const float p = 0.3f, s = p / 4;
                return (float)(Math.Pow(2, -10 * r) * Math.Sin((r - s) * (2 * Math.PI) / p) + 1);
            },
            [EASE_IN_OUT_ELASTIC] = r => Combined(r, sTrans(EASE_IN_ELASTIC), sTrans(EASE_OUT_ELASTIC)),
            [EASE_OUT_IN_ELASTIC] = r => Combined(r, sTrans(EASE_OUT_ELASTIC), sTrans(EASE_IN_ELASTIC)),
            [EASE_IN_BOUNCE] = r => 1 - BounceOut(1 - r),
            [EASE_OUT_BOUNCE] = BounceOut,
            [EASE_IN_OUT_BOUNCE] = r => Combined(r, sTrans(EASE_IN_BOUNCE), BounceOut),
            [EASE_OUT_IN_BOUNCE] = r => Combined(r, BounceOut, sTrans(EASE_IN_BOUNCE)),
        };

        static Func<float, float> sTrans(string n) => sTransitions[n];

        static float BounceOut(float r)
        {
            const float s = 7.5625f, p = 2.75f;
            if (r < 1 / p) return s * r * r;
            if (r < 2 / p) { r -= 1.5f / p; return s * r * r + 0.75f; }
            if (r < 2.5f / p) { r -= 2.25f / p; return s * r * r + 0.9375f; }
            r -= 2.625f / p;
            return s * r * r + 0.984375f;
        }

        static float Combined(float r, Func<float, float> a, Func<float, float> b) =>
            r < 0.5f ? 0.5f * a(r * 2) : 0.5f * b((r - 0.5f) * 2) + 0.5f;

        public static Func<float, float> getTransition(string name) => sTransitions.TryGetValue(name ?? LINEAR, out var f) ? f : sTransitions[LINEAR];
        public static void register(string name, Func<float, float> func) => sTransitions[name] = func;
    }
}
