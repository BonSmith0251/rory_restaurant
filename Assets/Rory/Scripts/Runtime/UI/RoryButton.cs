using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rory.UI
{
    /// <summary>
    /// Feel of the original buttons on top of a standard <see cref="Button"/>: grows a little on hover
    /// (TNonStateButton), shrinks while pressed (ButtonWithSound) and plays "click_v1" when clicked.
    /// </summary>
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public class RoryButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Tooltip("Scale while hovered (1.05; 1.2 for the kitchen arrows)")]
        public float hoverScale = 1.05f;
        [Tooltip("Scale while pressed")]
        public float pressScale = 0.9f;
        [Tooltip("Sound played on click (empty = none)")]
        public string clickSound = "click_v1";

        Vector3 baseScale = Vector3.one;
        bool hover, pressed;
        Button button;

        void Awake()
        {
            button = GetComponent<Button>();
            baseScale = transform.localScale;
        }

        void OnDisable()
        {
            hover = pressed = false;
            Apply();
        }

        /// <summary>Enables/disables the button; disabled buttons are drawn at 50% alpha like the original.</summary>
        public bool Interactable
        {
            get => GetComponent<Button>().interactable;
            set
            {
                if (button == null) button = GetComponent<Button>();
                button.interactable = value;
                if (!TryGetComponent(out CanvasGroup group)) group = gameObject.AddComponent<CanvasGroup>();
                group.alpha = value ? 1 : alphaWhenDisabled;
                Apply();
            }
        }

        [Tooltip("Alpha while not interactable")]
        public float alphaWhenDisabled = 0.5f;

        /// <summary>Call after changing the button's scale from code, so hover/press scale around the new value.</summary>
        public void SetBaseScale(Vector3 scale)
        {
            baseScale = scale;
            Apply();
        }

        public void OnPointerEnter(PointerEventData e) { hover = true; Apply(); }
        public void OnPointerExit(PointerEventData e) { hover = false; pressed = false; Apply(); }
        public void OnPointerDown(PointerEventData e) { pressed = true; Apply(); }
        public void OnPointerUp(PointerEventData e) { pressed = false; Apply(); }

        // The sound is played from the pointer event, not from onClick, so code that resets the button's
        // onClick listeners (RemoveAllListeners) keeps it.
        public void OnPointerClick(PointerEventData e)
        {
            if (e.button != PointerEventData.InputButton.Left || button == null || !button.IsInteractable()) return;
            if (!string.IsNullOrEmpty(clickSound) && GameManager.Instance != null) GameManager.Instance.Audio.PlaySfx(clickSound);
        }

        void Apply()
        {
            bool interactable = button == null || button.IsInteractable();
            float s = interactable ? (pressed ? pressScale : 1) * (hover ? hoverScale : 1) : 1;
            transform.localScale = baseScale * s;
        }
    }
}
