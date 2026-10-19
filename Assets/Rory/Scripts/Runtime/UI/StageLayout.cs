using UnityEngine;

namespace Rory.UI
{
    /// <summary>
    /// Original coordinates (x right, y down, pivot in pixels) to RectTransforms anchored at the parent's
    /// top-left corner. Every element under a <see cref="UIStage"/> uses anchorMin = anchorMax = (0, 1), and
    /// containers have zero size, so a child's anchoredPosition is (x, -y) relative to its parent's pivot point,
    /// exactly like a Starling child inside a Sprite.
    /// </summary>
    public static class StageLayout
    {
        public static readonly Vector2 TopLeft = new Vector2(0, 1);

        public static Vector2 ToAnchored(float x, float y) => new Vector2(x, -y);
        public static Vector2 ToAnchored(Vector2 stage) => new Vector2(stage.x, -stage.y);

        /// <summary>Places <paramref name="rt"/> like a Starling display object at (x, y) with a pixel pivot.</summary>
        public static void Place(RectTransform rt, float x, float y, float width, float height, float pivotX = 0, float pivotY = 0)
        {
            rt.anchorMin = rt.anchorMax = TopLeft;
            rt.sizeDelta = new Vector2(width, height);
            rt.pivot = new Vector2(width > 0 ? pivotX / width : 0, height > 0 ? 1 - pivotY / height : 1);
            rt.anchoredPosition = new Vector2(x, -y);
        }

        /// <summary>Moves to original coordinates (x, y-down) keeping size and pivot.</summary>
        public static void SetStagePosition(this RectTransform rt, float x, float y)
        {
            rt.anchorMin = rt.anchorMax = TopLeft;
            rt.anchoredPosition = new Vector2(x, -y);
        }

        public static Vector2 GetStagePosition(this RectTransform rt) => new Vector2(rt.anchoredPosition.x, -rt.anchoredPosition.y);

        /// <summary>
        /// Sets the pivot in pixels from the top-left corner. Like Starling's pivotX/pivotY the position stays,
        /// so the graphic moves to put the new pivot at it.
        /// </summary>
        public static void SetPivotPixels(this RectTransform rt, float px, float py)
        {
            var size = rt.rect.size;
            rt.pivot = new Vector2(size.x > 0 ? px / size.x : 0, size.y > 0 ? 1 - py / size.y : 1);
        }

        /// <summary>Starling rotation (radians, clockwise) to a RectTransform rotation.</summary>
        public static void SetStarlingRotation(this RectTransform rt, float radians) =>
            rt.localEulerAngles = new Vector3(0, 0, -radians * Mathf.Rad2Deg);
    }
}
