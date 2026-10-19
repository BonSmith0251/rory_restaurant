using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.Popups
{
    /// <summary>
    /// The standard window of the original game (widget_BG / small_widget_BG with a title, an info text,
    /// an optional picture and up to three buttons: left, right and a single centred "uni" button).
    /// Specialised windows (results, player select, pause, ...) are prefab variants that add their own parts.
    /// </summary>
    public class WindowPopup : Popup
    {
        [Header("Parts")]
        public Image background;
        public TextMeshProUGUI title;
        public TextMeshProUGUI info;
        public Image picture;
        public Button leftButton;
        public Button rightButton;
        public Button uniButton;

        [Serializable]
        public struct ButtonSpec
        {
            public string label;
            public Action onClick;
            public bool closes;

            public ButtonSpec(string label, Action onClick, bool closes = true)
            {
                this.label = label;
                this.onClick = onClick;
                this.closes = closes;
            }
        }

        public WindowPopup SetTitle(string text)
        {
            if (title != null)
            {
                title.text = text ?? "";
                title.gameObject.SetActive(!string.IsNullOrEmpty(text));
            }
            return this;
        }

        public WindowPopup SetInfo(string text)
        {
            if (info != null)
            {
                info.text = text ?? "";
                info.gameObject.SetActive(!string.IsNullOrEmpty(text));
            }
            return this;
        }

        public WindowPopup SetPicture(Sprite sprite)
        {
            if (picture != null)
            {
                picture.sprite = sprite;
                picture.gameObject.SetActive(sprite != null);
                if (sprite != null) picture.SetNativeSize();
            }
            return this;
        }

        /// <summary>Two buttons (left/right). Pass null for a side to hide it.</summary>
        public WindowPopup SetButtons(ButtonSpec? left, ButtonSpec? right)
        {
            Configure(leftButton, left);
            Configure(rightButton, right);
            Configure(uniButton, null);
            return this;
        }

        /// <summary>A single centred button.</summary>
        public WindowPopup SetButton(ButtonSpec uni)
        {
            Configure(leftButton, null);
            Configure(rightButton, null);
            Configure(uniButton, uni);
            return this;
        }

        public WindowPopup HideButtons()
        {
            Configure(leftButton, null);
            Configure(rightButton, null);
            Configure(uniButton, null);
            return this;
        }

        void Configure(Button button, ButtonSpec? spec)
        {
            if (button == null) return;
            button.gameObject.SetActive(spec.HasValue);
            button.onClick.RemoveAllListeners();
            if (!spec.HasValue) return;
            var s = spec.Value;
            var label = button.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null) label.text = s.label;
            button.onClick.AddListener(() =>
            {
                if (s.closes) Close();
                s.onClick?.Invoke();
            });
        }

        /// <summary>Moves a part by the offsets used in LevelEvent.xml (relative to the window centre, y down).</summary>
        public static void Offset(Component part, float offsetX, float offsetY)
        {
            if (part == null) return;
            var rt = (RectTransform)part.transform;
            rt.anchoredPosition = new Vector2(offsetX, -offsetY);
        }
    }
}
