using UnityEngine;
using UnityEngine.EventSystems;

namespace Rory.Screens.HiddenObject
{
    /// <summary>
    /// The room background (MiniGameHO._background, touchBackground): a touch that started on it and ends counts as a
    /// miss click. Drags bubble up to <see cref="HiddenObjectZoom"/>.
    /// </summary>
    public class HiddenObjectBackground : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] HiddenObjectController controller;

        // needed so the EventSystem sends the matching pointer-up here (Starling ENDED goes to the touched object)
        public void OnPointerDown(PointerEventData e) { }

        public void OnPointerUp(PointerEventData e)
        {
            if (controller != null) controller.OnBackgroundPointerUp();
        }
    }
}
