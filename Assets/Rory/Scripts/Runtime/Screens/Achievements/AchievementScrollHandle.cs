using Rory.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rory.Screens.Achievements
{
    /// <summary>
    /// Touch handling of the achievement scroll bar and its knob (AchievementScreen.touchScroller): forwards
    /// press / move / release with the stage y of the pointer to the controller.
    /// </summary>
    public class AchievementScrollHandle : MonoBehaviour,
        IPointerDownHandler, IInitializePotentialDragHandler, IDragHandler, IPointerUpHandler
    {
        public enum Part { Bar = 1, Scroller = 2 }

        public Part part;
        [SerializeField] AchievementsController controller;

        static float StageY(PointerEventData e) => UIStage.Current != null ? UIStage.Current.ScreenToStage(e.position).y : 0;

        public void OnPointerDown(PointerEventData e) => controller.ScrollBegan(part, StageY(e));

        // Starling sends MOVED for every pointer move, so there is no drag threshold.
        public void OnInitializePotentialDrag(PointerEventData e) => e.useDragThreshold = false;

        public void OnDrag(PointerEventData e) => controller.ScrollMoved(StageY(e));

        public void OnPointerUp(PointerEventData e) => controller.ScrollEnded();
    }
}
