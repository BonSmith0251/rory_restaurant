using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rory.Screens.Match3
{
    /// <summary>
    /// The coin pile of a served customer (coinsAr): touchable on its image grown by 80 px left/right and 20 px up/down
    /// (cSprite paddingX / paddingY; a transparent child graphic), green glow while hovered, collects on press.
    /// </summary>
    public class CoinButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public const float PaddingX = 80, PaddingY = 20;

        [SerializeField] Outline glow;

        public event Action Pressed;

        public void OnPointerDown(PointerEventData e)
        {
            if (e.button == PointerEventData.InputButton.Left) Pressed?.Invoke();
        }

        public void OnPointerEnter(PointerEventData e) => SetGlow(true);
        public void OnPointerExit(PointerEventData e) => SetGlow(false);

        void OnDisable() => SetGlow(false);

        void SetGlow(bool on)
        {
            if (glow != null) glow.enabled = on;
        }
    }
}
