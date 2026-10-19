using Rory.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.Screens.Kitchen
{
    /// <summary>
    /// One plate of the shop carousel (Kitchen.as shopItemContainer[i] with plateAr[i] and itemAr[i]): a container
    /// whose pivot is the plate's centre, at stage (512 + i * 550, 280). <see cref="ItemIndex"/> is itemAr[i].itemIdx,
    /// the index of the shown item in the current shop's list.
    /// </summary>
    public class KitchenShopSlot : MonoBehaviour
    {
        [SerializeField] RectTransform container;
        [SerializeField] CanvasGroup group;
        [SerializeField] Image plate;
        [SerializeField] Image item;
        [Tooltip("Container pivot in container pixels (plate width / 2, plate height / 2)")]
        [SerializeField] Vector2 pivot;

        /// <summary>itemAr[i].itemIdx.</summary>
        public int ItemIndex { get; set; }

        public RectTransform Container => container;
        public CanvasGroup Group => group;
        public Image Plate => plate;
        public Image Item => item;

        /// <summary>Container x (stage pixels).</summary>
        public float X
        {
            get => container.anchoredPosition.x;
            set => container.anchoredPosition = new Vector2(value, container.anchoredPosition.y);
        }

        public float Scale
        {
            get => container.localScale.x;
            set => container.localScale = new Vector3(value, value, 1);
        }

        public float Alpha
        {
            get => group.alpha;
            set => group.alpha = value;
        }

        public bool PlateVisible
        {
            get => plate.gameObject.activeSelf;
            set => plate.gameObject.SetActive(value);
        }

        public bool ItemVisible
        {
            get => item.gameObject.activeSelf;
            set => item.gameObject.SetActive(value);
        }

        /// <summary>
        /// texture = sprite; readjustSize(); pivot = size / 2; x/y = (x, y) in container pixels
        /// (the plate's centre is (pivot.x, pivot.y)).
        /// </summary>
        public void SetItem(Sprite sprite, float x, float y)
        {
            item.sprite = sprite;
            item.SetNativeSize();
            var rt = item.rectTransform;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.SetStagePosition(x - pivot.x, y - pivot.y);
        }

        /// <summary>Item centred on the plate (x = plate.x, y = plate.y).</summary>
        public void SetItemOnPlate(Sprite sprite) => SetItem(sprite, pivot.x, pivot.y);
    }
}
