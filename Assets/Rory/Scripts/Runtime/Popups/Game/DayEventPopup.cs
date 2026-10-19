using System.Collections.Generic;
using Rory.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.Popups.Game
{
    /// <summary>
    /// A LevelEvent.xml window (showBeginningWindow / showKitchenWindow): title, info and buttons placed by the
    /// data's offsets (relative to the window centre), an optional picture from the "game" atlas (unlockedItem,
    /// centred 30 px above the window centre) and, for the match-3 "Today's Preparation" variant, up to four dish
    /// previews standing on the window centre line. Chained by <see cref="GameWindows"/> (NEXT_WINDOW).
    /// </summary>
    public class DayEventPopup : WindowPopup
    {
        public const int MaxDishPreviews = 4;

        [Header("Day event")]
        [Tooltip("unlockedItem: the picture of a new customer / item (game atlas)")]
        public Image unlockedItem;
        [Tooltip("dishPreviewAr: today's dishes (Today's Preparation)")]
        public Image[] dishPreviews = new Image[MaxDishPreviews];

        /// <summary>Applies a data window: visibility, texts and offsets of every part.</summary>
        internal void Apply(EventWindowData w)
        {
            Part(title, w.title);
            Part(info, w.info);
            Part(leftButton, w.leftButton);
            Part(rightButton, w.rightButton);
            Part(uniButton, w.uniButton);
            SetLabel(leftButton, w.leftButton.text);
            SetLabel(rightButton, w.rightButton.text);
            SetLabel(uniButton, w.uniButton.text);
            if (title != null) title.text = w.title.text ?? "";
            if (info != null) info.text = w.info.text ?? "";
            SetUnlockedItem(string.IsNullOrEmpty(w.texture) ? null : GameDatabase.Instance.sprites.Get("game", w.texture), 1);
            ShowDishes(null);
        }

        static void Part(Component c, WindowElement e)
        {
            if (c == null) return;
            c.gameObject.SetActive(e.visible);
            Offset(c, e.offsetX, e.offsetY);
        }

        static void SetLabel(Button b, string text)
        {
            if (b == null) return;
            var label = b.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
            if (label != null) label.text = text ?? "";
        }

        /// <summary>Shows <paramref name="sprite"/> centred at (0, -30) from the window centre, or hides the picture.</summary>
        public void SetUnlockedItem(Sprite sprite, float scale)
        {
            if (unlockedItem == null) return;
            unlockedItem.gameObject.SetActive(sprite != null);
            if (sprite == null) return;
            unlockedItem.sprite = sprite;
            unlockedItem.SetNativeSize();
            unlockedItem.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            unlockedItem.transform.localScale = new Vector3(scale, scale, 1);
        }

        /// <summary>
        /// Dish previews of "Today's Preparation": each at 1.2x, its bottom edge on the window centre line, laid out
        /// side by side around x = 512 exactly like the original (spacing = scaled width + 10).
        /// </summary>
        public void ShowDishes(IList<Sprite> sprites)
        {
            int n = sprites == null ? 0 : Mathf.Min(sprites.Count, dishPreviews.Length);
            for (int k = 0; k < dishPreviews.Length; k++)
            {
                var img = dishPreviews[k];
                if (img == null) continue;
                img.gameObject.SetActive(k < n);
                if (k >= n) continue;
                img.sprite = sprites[k];
                img.SetNativeSize();
                var rt = img.rectTransform;
                rt.pivot = new Vector2(0, 0); // pivotX = 0, pivotY = height (bottom-left)
                rt.localScale = new Vector3(1.2f, 1.2f, 1);
                float w = rt.sizeDelta.x * 1.2f;
                float x = n % 2 == 0
                    ? 512 + (k - n / 2f) * (w + 10)
                    : 512 + (k - Mathf.Floor(n / 2f)) * (w + 10) - w / 2;
                Offset(img, x - 512, 0);
            }
        }

        /// <summary>widgetBG.scaleY (0.7 for big beginning windows).</summary>
        public void SetBackgroundScaleY(float scaleY)
        {
            if (background != null) background.transform.localScale = new Vector3(1, scaleY, 1);
        }
    }
}
