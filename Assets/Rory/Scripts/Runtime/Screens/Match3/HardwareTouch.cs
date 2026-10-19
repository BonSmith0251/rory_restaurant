using UnityEngine;
using UnityEngine.EventSystems;

namespace Rory.Screens.Match3
{
    /// <summary>Forwards the touches of a machine / dragged product to its <see cref="HardwareStation"/> (any phase, like
    /// the original TouchEvent listener: hovering counts as a touch).</summary>
    public class HardwareTouch : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerDownHandler, IDragHandler,
        IInitializePotentialDragHandler, IPointerUpHandler
    {
        public HardwareStation station;
        bool pressed;

        public void OnInitializePotentialDrag(PointerEventData e) => e.useDragThreshold = false;
        public void OnPointerEnter(PointerEventData e) => station.Report(HardwareStation.Phase.Touch, e);
        public void OnPointerMove(PointerEventData e)
        {
            if (!pressed) station.Report(HardwareStation.Phase.Touch, e);
        }

        public void OnPointerDown(PointerEventData e)
        {
            pressed = true;
            station.Report(HardwareStation.Phase.Touch, e);
        }

        public void OnDrag(PointerEventData e)
        {
            if (pressed) station.Report(HardwareStation.Phase.Moved, e);
        }

        public void OnPointerUp(PointerEventData e)
        {
            if (!pressed) return;
            pressed = false;
            station.Report(HardwareStation.Phase.Ended, e);
        }

        void OnDisable() => pressed = false;
    }
}
