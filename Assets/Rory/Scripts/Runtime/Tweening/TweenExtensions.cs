using UnityEngine;
using UnityEngine.UI;

namespace Rory.Tweening
{
    /// <summary>
    /// Tween shortcuts for UI. Start values are read when the tween starts (after its delay), like TweenLite.to.
    /// Positions are anchoredPosition in stage pixels, so "y" goes up: use <c>StageLayout.ToAnchored</c> to convert
    /// original (x, y-down) coordinates.
    /// </summary>
    public static class TweenExtensions
    {
        public static Tween TweenAnchoredPosition(this RectTransform rt, Vector2 to, float duration)
        {
            Vector2 from = default;
            return Tweens.To(rt, duration, p => rt.anchoredPosition = Vector2.LerpUnclamped(from, to, p), () => from = rt.anchoredPosition);
        }

        public static Tween TweenAnchoredX(this RectTransform rt, float to, float duration)
        {
            float from = 0;
            return Tweens.To(rt, duration, p => rt.anchoredPosition = new Vector2(Mathf.LerpUnclamped(from, to, p), rt.anchoredPosition.y),
                () => from = rt.anchoredPosition.x);
        }

        public static Tween TweenAnchoredY(this RectTransform rt, float to, float duration)
        {
            float from = 0;
            return Tweens.To(rt, duration, p => rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, Mathf.LerpUnclamped(from, to, p)),
                () => from = rt.anchoredPosition.y);
        }

        public static Tween TweenScale(this Transform tr, float to, float duration) => tr.TweenScale(new Vector3(to, to, 1), duration);

        public static Tween TweenScale(this Transform tr, Vector3 to, float duration)
        {
            Vector3 from = default;
            return Tweens.To(tr, duration, p => tr.localScale = Vector3.LerpUnclamped(from, to, p), () => from = tr.localScale);
        }

        /// <summary>Rotates to <paramref name="toDegrees"/> around Z (counter-clockwise positive, Unity convention).</summary>
        public static Tween TweenRotation(this Transform tr, float toDegrees, float duration)
        {
            float from = 0;
            return Tweens.To(tr, duration, p => tr.localEulerAngles = new Vector3(0, 0, Mathf.LerpUnclamped(from, toDegrees, p)),
                () => from = NormalizeAngle(tr.localEulerAngles.z));
        }

        public static Tween TweenAlpha(this CanvasGroup g, float to, float duration)
        {
            float from = 0;
            return Tweens.To(g, duration, p => g.alpha = Mathf.LerpUnclamped(from, to, p), () => from = g.alpha);
        }

        public static Tween TweenAlpha(this Graphic g, float to, float duration)
        {
            float from = 0;
            return Tweens.To(g, duration, p =>
            {
                var c = g.color;
                c.a = Mathf.LerpUnclamped(from, to, p);
                g.color = c;
            }, () => from = g.color.a);
        }

        public static Tween TweenColor(this Graphic g, Color to, float duration)
        {
            Color from = default;
            return Tweens.To(g, duration, p => g.color = Color.LerpUnclamped(from, to, p), () => from = g.color);
        }

        public static Tween TweenFill(this Image img, float to, float duration)
        {
            float from = 0;
            return Tweens.To(img, duration, p => img.fillAmount = Mathf.LerpUnclamped(from, to, p), () => from = img.fillAmount);
        }

        public static void KillTweens(this Object target, bool complete = false) => Tweens.Kill(target, complete);

        static float NormalizeAngle(float a)
        {
            a %= 360;
            return a > 180 ? a - 360 : a;
        }
    }
}
