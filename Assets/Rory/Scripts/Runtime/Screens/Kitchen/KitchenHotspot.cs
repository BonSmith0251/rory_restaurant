using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rory.Screens.Kitchen
{
    /// <summary>
    /// Hover reporting of a kitchen room hotspot (a bubble sprite or its invisible quad): the original lit the
    /// matching glow while the mouse hovered the bubble or the quad (Kitchen.touchButton / touchQuad HOVER).
    /// </summary>
    public class KitchenHotspot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("Kitchen.as button id (TutorialButtons.PLAY_ID, SHOP_ID, ...)")]
        public int buttonId;

        /// <summary>(button id, hovered)</summary>
        public event Action<int, bool> HoverChanged;

        public void OnPointerEnter(PointerEventData e) => HoverChanged?.Invoke(buttonId, true);
        public void OnPointerExit(PointerEventData e) => HoverChanged?.Invoke(buttonId, false);
    }
}
