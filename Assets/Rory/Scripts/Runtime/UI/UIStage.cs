using UnityEngine;
using UnityEngine.UI;

namespace Rory.UI
{
    /// <summary>
    /// The 1024x600 design area of a screen (the original Starling stage); all layout uses its units. It is centred
    /// in a 16:9 "Viewport" (<see cref="ViewWidth"/> x 600, clipped) that the CanvasScaler fits to the window, so at
    /// 1920x1080 one unit is 1.8 pixels and the <see cref="SideMargin"/> strips left and right show the extended
    /// backgrounds. Children are anchored at the stage's top-left corner; see <see cref="StageLayout"/>.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UIStage : MonoBehaviour
    {
        public const float Width = 1024, Height = 600;

        /// <summary>Width of the visible 16:9 area in stage units (600 * 16 / 9).</summary>
        public const float ViewWidth = Height * 16f / 9f;

        /// <summary>Extra visible width left and right of the 1024 design area.</summary>
        public const float SideMargin = (ViewWidth - Width) / 2;

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

        /// <summary>
        /// Makes a stretched RectTransform cover the whole 16:9 view instead of just the design area (full-screen
        /// dimming, backdrops). Use on children of a stage-sized layer.
        /// </summary>
        public static void FillView(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = new Vector2(-SideMargin, 0);
            rt.offsetMax = new Vector2(SideMargin, 0);
        }

        /// <summary>Configures a Canvas + scaler for the stage (used by the scene builders and at runtime).</summary>
        public static void SetupCanvas(Canvas canvas)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            if (!canvas.TryGetComponent(out CanvasScaler scaler)) scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ViewWidth, Height);
            scaler.referencePixelsPerUnit = SpriteSizes.ReferencePixelsPerUnit;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            if (!canvas.TryGetComponent(out GraphicRaycaster _)) canvas.gameObject.AddComponent<GraphicRaycaster>();
        }
    }
}
