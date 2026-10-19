using UnityEngine;
using UnityEngine.UI;

namespace Rory.UI
{
    /// <summary>
    /// The 1024x600 design area of a screen (the original Starling stage). It sits under a Canvas whose
    /// CanvasScaler fits it to the window (letterboxed, content outside is clipped by the RectMask2D).
    /// Children are anchored at its top-left corner; see <see cref="StageLayout"/>.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UIStage : MonoBehaviour
    {
        public const float Width = 1024, Height = 600;

        public static UIStage Current { get; private set; }

        public RectTransform Rect => (RectTransform)transform;

        void OnEnable() => Current = this;

        void OnDisable()
        {
            if (Current == this) Current = null;
        }

        /// <summary>Screen pixels to stage coordinates (origin top-left, y down, like the original code).</summary>
        public Vector2 ScreenToStage(Vector2 screen)
        {
            var canvas = GetComponentInParent<Canvas>();
            var cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Rect, screen, cam, out var local);
            var r = Rect.rect;
            return new Vector2(local.x - r.xMin, r.yMax - local.y);
        }

        /// <summary>Stage coordinates of a point in <paramref name="rt"/>'s local space.</summary>
        public Vector2 LocalToStage(RectTransform rt, Vector2 local)
        {
            var world = rt.TransformPoint(local);
            var inStage = Rect.InverseTransformPoint(world);
            var r = Rect.rect;
            return new Vector2(inStage.x - r.xMin, r.yMax - inStage.y);
        }

        /// <summary>Configures a Canvas + scaler for the stage (used by the scene builders and at runtime).</summary>
        public static void SetupCanvas(Canvas canvas)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            if (!canvas.TryGetComponent(out CanvasScaler scaler)) scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(Width, Height);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            if (!canvas.TryGetComponent(out GraphicRaycaster _)) canvas.gameObject.AddComponent<GraphicRaycaster>();
        }
    }
}
