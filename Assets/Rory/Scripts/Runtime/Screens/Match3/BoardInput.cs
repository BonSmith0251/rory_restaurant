using System;
using Rory.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rory.Screens.Match3
{
    /// <summary>
    /// The invisible 544 x 544 touch quad over the board. Turns pointer events into Starling touch phases in stage
    /// coordinates: press = BEGAN, every move while pressed = MOVED (no drag threshold, also outside the quad), release
    /// = ENDED, moves without a button = HOVER, leaving the quad = no touch.
    /// </summary>
    public class BoardInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IInitializePotentialDragHandler,
        IPointerMoveHandler, IPointerExitHandler
    {
        public event Action<BoardTouch, Vector2> Touched;

        bool pressed;

        static Vector2 Stage(PointerEventData e) => UIStage.Current != null ? UIStage.Current.ScreenToStage(e.position) : e.position;

        void Send(BoardTouch phase, PointerEventData e) => Touched?.Invoke(phase, Stage(e));

        public void OnInitializePotentialDrag(PointerEventData e) => e.useDragThreshold = false;

        public void OnPointerDown(PointerEventData e)
        {
            if (e.button != PointerEventData.InputButton.Left) return;
            pressed = true;
            Send(BoardTouch.Began, e);
        }

        public void OnDrag(PointerEventData e)
        {
            if (pressed) Send(BoardTouch.Moved, e);
        }

        public void OnPointerUp(PointerEventData e)
        {
            if (!pressed) return;
            pressed = false;
            Send(BoardTouch.Ended, e);
            if (!e.hovered.Contains(gameObject)) Touched?.Invoke(BoardTouch.Exit, Vector2.zero);
        }

        public void OnPointerMove(PointerEventData e)
        {
            if (!pressed) Send(BoardTouch.Hover, e);
        }

        public void OnPointerExit(PointerEventData e)
        {
            if (!pressed) Touched?.Invoke(BoardTouch.Exit, Vector2.zero);
        }

        void OnDisable() => pressed = false;
    }
}
