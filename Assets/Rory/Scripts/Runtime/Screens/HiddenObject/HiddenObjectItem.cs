using Rory.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rory.Screens.HiddenObject
{
    /// <summary>
    /// One piece of a hidden-object room (HOObjects + its PixelPerfectObject image): a listed-able object ("name,nm"),
    /// a foreground part that covers objects ("name,bg", clickable on its opaque pixels) or a decoration that ignores
    /// touches ("name,bg2"). Instantiated by <see cref="HiddenObjectController"/> from the layout data; the pivot is
    /// the image centre like the original display Sprite (x = locX + width / 2).
    /// Touches go to the controller (HOClass.touchHOObjects); drags bubble up to <see cref="HiddenObjectZoom"/>.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class HiddenObjectItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        // HOObjects / HOClass item types
        public const int TypeObject = 1;    // AS_HO_OBJECT_ID ("nm")
        public const int TypeBackground = 2; // AS_BG_ID ("bg")
        public const int TypeCoin = 4;       // AS_COIN_ID ("nm,coin")
        public const int TypeLogo = 5;       // AS_LOGO_ID ("nm,logo")
        public const int TypeMultiple = 6;   // AS_MULTIPLE_ID ("nm,mul")
        public const int TypeBackground2 = 7; // AS_BG2_ID ("bg2")

        /// <summary>Alpha a click needs on an object ("nm") to hit it (handbook: HO clicks are pixel precise).</summary>
        public const float ObjectHitThreshold = 0.5f;
        /// <summary>"bg" parts kept the original PixelHitArea with threshold 255: only fully opaque pixels block.</summary>
        public const float BackgroundHitThreshold = 0.996f;

        [SerializeField] Image image;

        HiddenObjectController owner;

        /// <summary>itemIdx: index in the layout (display.name in the original).</summary>
        public int Index { get; private set; }
        /// <summary>itemNameAr: the name before the "," (before the "_" for multiples), e.g. "alarm clock".</summary>
        public string ItemName { get; private set; }
        /// <summary>The layout entry, e.g. "alarm clock,nm" (itemFileNameAr; also the sprite name).</summary>
        public string FileName { get; private set; }
        /// <summary>itemTypeAr (TypeObject, TypeBackground, ...).</summary>
        public int ItemType { get; private set; }
        /// <summary>itemPosXAr / itemPosYAr: top-left in the room (original coordinates).</summary>
        public Vector2 Location { get; private set; }
        public bool Listed { get; internal set; }
        public bool Found { get; internal set; }
        public bool OnTween { get; internal set; }

        public Image Image => image;
        public RectTransform Rect => (RectTransform)transform;
        /// <summary>display.width / height (the image size; the display itself is not scaled inside the room).</summary>
        public Vector2 Size => image.rectTransform.rect.size;

        internal void Setup(HiddenObjectController controller, int index, string fileName, string itemName, int type, Sprite sprite,
            float locX, float locY)
        {
            owner = controller;
            Index = index;
            FileName = fileName;
            ItemName = itemName;
            ItemType = type;
            Location = new Vector2(locX, locY);
            name = index.ToString();
            if (image == null) image = GetComponent<Image>();
            image.sprite = sprite;
            var size = sprite != null ? sprite.rect.size : Vector2.zero;
            StageLayout.Place(Rect, locX + size.x / 2, locY + size.y / 2, size.x, size.y, size.x / 2, size.y / 2);
            // AS_HO_ITEM: hitArea = null; AS_BG2: not touchable; the others keep the pixel hit area
            image.raycastTarget = type != TypeBackground2;
            image.alphaHitTestMinimumThreshold = type == TypeObject ? ObjectHitThreshold : BackgroundHitThreshold;
        }

        public void OnPointerDown(PointerEventData e)
        {
            if (owner != null) owner.OnItemPointerDown(this);
        }

        public void OnPointerUp(PointerEventData e)
        {
            if (owner != null) owner.OnItemPointerUp(this);
        }
    }
}
