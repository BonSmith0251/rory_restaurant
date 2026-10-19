using UnityEngine;

namespace Rory.Tweening
{
    public delegate float EaseFunction(float t);

    /// <summary>The Penner easing equations with the same constants as TweenLite (Strong = Quint).</summary>
    public static class Ease
    {
        const float BackOvershoot = 1.70158f;
        const float HalfPi = Mathf.PI / 2;

        public static readonly EaseFunction Linear = t => t;

        public static readonly EaseFunction QuadIn = t => t * t;
        public static readonly EaseFunction QuadOut = t => t * (2 - t);
        public static readonly EaseFunction QuadInOut = t => t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;

        public static readonly EaseFunction CubicIn = t => t * t * t;
        public static readonly EaseFunction CubicOut = t => { t -= 1; return t * t * t + 1; };
        public static readonly EaseFunction CubicInOut = t => t < 0.5f ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;

        public static readonly EaseFunction QuartIn = t => t * t * t * t;
        public static readonly EaseFunction QuartOut = t => { t -= 1; return 1 - t * t * t * t; };
        public static readonly EaseFunction QuartInOut = t => t < 0.5f ? 8 * t * t * t * t : 1 - 8 * (t - 1) * (t - 1) * (t - 1) * (t - 1);

        public static readonly EaseFunction QuintIn = t => t * t * t * t * t;
        public static readonly EaseFunction QuintOut = t => { t -= 1; return t * t * t * t * t + 1; };
        public static readonly EaseFunction QuintInOut = t => t < 0.5f ? 16 * t * t * t * t * t : 1 + 16 * (t - 1) * (t - 1) * (t - 1) * (t - 1) * (t - 1);

        public static readonly EaseFunction StrongIn = QuintIn;
        public static readonly EaseFunction StrongOut = QuintOut;
        public static readonly EaseFunction StrongInOut = QuintInOut;

        public static readonly EaseFunction SineIn = t => 1 - Mathf.Cos(t * HalfPi);
        public static readonly EaseFunction SineOut = t => Mathf.Sin(t * HalfPi);
        public static readonly EaseFunction SineInOut = t => -0.5f * (Mathf.Cos(Mathf.PI * t) - 1);

        public static readonly EaseFunction ExpoIn = t => t <= 0 ? 0 : Mathf.Pow(2, 10 * (t - 1));
        public static readonly EaseFunction ExpoOut = t => t >= 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
        public static readonly EaseFunction ExpoInOut = t =>
            t <= 0 ? 0 : t >= 1 ? 1 : t < 0.5f ? 0.5f * Mathf.Pow(2, 20 * t - 10) : 1 - 0.5f * Mathf.Pow(2, -20 * t + 10);

        public static readonly EaseFunction CircIn = t => 1 - Mathf.Sqrt(1 - t * t);
        public static readonly EaseFunction CircOut = t => { t -= 1; return Mathf.Sqrt(1 - t * t); };
        public static readonly EaseFunction CircInOut = t =>
            t < 0.5f ? 0.5f * (1 - Mathf.Sqrt(1 - 4 * t * t)) : 0.5f * (Mathf.Sqrt(1 - (2 * t - 2) * (2 * t - 2)) + 1);

        public static readonly EaseFunction BackIn = t => t * t * ((BackOvershoot + 1) * t - BackOvershoot);
        public static readonly EaseFunction BackOut = t => { t -= 1; return t * t * ((BackOvershoot + 1) * t + BackOvershoot) + 1; };
        public static readonly EaseFunction BackInOut = t =>
        {
            const float s = BackOvershoot * 1.525f;
            t *= 2;
            if (t < 1) return 0.5f * (t * t * ((s + 1) * t - s));
            t -= 2;
            return 0.5f * (t * t * ((s + 1) * t + s) + 2);
        };

        public static readonly EaseFunction ElasticIn = t => 1 - ElasticOut(1 - t);
        public static readonly EaseFunction ElasticOut = t =>
        {
            if (t <= 0) return 0;
            if (t >= 1) return 1;
            const float p = 0.3f, s = p / 4;
            return Mathf.Pow(2, -10 * t) * Mathf.Sin((t - s) * (2 * Mathf.PI) / p) + 1;
        };
        public static readonly EaseFunction ElasticInOut = t => t < 0.5f ? ElasticIn(t * 2) * 0.5f : ElasticOut(t * 2 - 1) * 0.5f + 0.5f;

        public static readonly EaseFunction BounceOut = t =>
        {
            if (t < 1 / 2.75f) return 7.5625f * t * t;
            if (t < 2 / 2.75f) { t -= 1.5f / 2.75f; return 7.5625f * t * t + 0.75f; }
            if (t < 2.5f / 2.75f) { t -= 2.25f / 2.75f; return 7.5625f * t * t + 0.9375f; }
            t -= 2.625f / 2.75f;
            return 7.5625f * t * t + 0.984375f;
        };
        public static readonly EaseFunction BounceIn = t => 1 - BounceOut(1 - t);
        public static readonly EaseFunction BounceInOut = t => t < 0.5f ? BounceIn(t * 2) * 0.5f : BounceOut(t * 2 - 1) * 0.5f + 0.5f;
    }
}
