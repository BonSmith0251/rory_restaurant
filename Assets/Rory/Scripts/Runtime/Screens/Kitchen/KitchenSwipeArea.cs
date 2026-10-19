using System;
using Rory.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rory.Screens.Kitchen
{
    /// <summary>
    /// The shop table background (shopBackground with its swipe listener): reports the touch begin and every move
    /// in original stage x coordinates.
    /// </summary>
    public class KitchenSwipeArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        /// <summary>TouchPhase.BEGAN at stage x.</summary>
        public event Action<float> Began;
        /// <summary>TouchPhase.MOVED at stage x.</summary>
        public event Action<float> Moved;

        static float StageX(PointerEventData e)
        {
            var stage = UIStage.Current;
            return stage != null ? stage.ScreenToStage(e.position).x : e.position.x;
        }

        public void OnPointerDown(PointerEventData e) => Began?.Invoke(StageX(e));
        public void OnDrag(PointerEventData e) => Moved?.Invoke(StageX(e));
        public void OnPointerUp(PointerEventData e) { }
    }
}
