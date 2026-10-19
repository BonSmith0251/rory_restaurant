using UnityEngine;

namespace Rory.UI
{
    /// <summary>
    /// Layout size of sprites. Layout uses the original game's units (the 1024x600 design); an image that is drawn
    /// at a higher resolution (HD art) is imported with a proportionally higher pixels-per-unit, so its logical size
    /// stays the original one. Always use <see cref="LogicalSize"/> for layout, never <c>sprite.rect.size</c>.
    /// </summary>
    public static class SpriteSizes
    {
        /// <summary>Pixels per unit of original-resolution (1x) art; also the canvas' reference pixels per unit.</summary>
        public const float ReferencePixelsPerUnit = 100;

        public static Vector2 LogicalSize(this Sprite sprite) =>
            sprite == null ? Vector2.zero : sprite.rect.size * (ReferencePixelsPerUnit / sprite.pixelsPerUnit);

        /// <summary>How many image pixels per layout unit (1 for original art, 2 for 2x HD art).</summary>
        public static float Scale(this Sprite sprite) =>
            sprite == null ? 1 : sprite.pixelsPerUnit / ReferencePixelsPerUnit;
    }
}
