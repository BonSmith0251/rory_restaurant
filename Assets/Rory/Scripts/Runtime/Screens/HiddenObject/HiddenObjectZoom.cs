using Rory.Tweening;
using Rory.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rory.Screens.HiddenObject
{
    /// <summary>
    /// MiniGameHO.zoomContainer and its touchContainer handler: the room (background + objects) that can be pinched
    /// to zoom (x1..x2, two touches) and dragged while zoomed in, never showing past the background's edges.
    /// Keeps the Starling transform (x, y, pivotX, pivotY, scale) and applies it to this zero-size RectTransform.
    /// Also tracks whether the current touch was dragged more than 25 px (touchDragged), which cancels object clicks.
    /// </summary>
    public class HiddenObjectZoom : MonoBehaviour, IInitializePotentialDragHandler, IDragHandler
    {
        public const float StageWidth = 1024, StageHeight = 600;
        public const float DragThreshold = 25;
        public const float MinScale = 1, MaxScale = 2;

        [Tooltip("_backgroundWidth (the room background is 1024 wide)")]
        [SerializeField] float backgroundWidth = StageWidth;

        public float X { get; private set; }
        public float Y { get; private set; }
        public float PivotX { get; private set; }
        public float PivotY { get; private set; }
        public float Scale { get; private set; } = 1;

        /// <summary>touchDragged: the touch moved more than 25 px (or two fingers moved).</summary>
        public bool TouchDragged { get; private set; }
        /// <summary>_HOClass.onScaleBack: the hint is zooming the room back to x1; touches don't move it meanwhile.</summary>
        public bool OnScaleBack { get; private set; }

        RectTransform Rect => (RectTransform)transform;
        Vector2 began, last;

        void Awake() => Apply();

        void Apply()
        {
            Rect.anchoredPosition = new Vector2(X - PivotX * Scale, -(Y - PivotY * Scale));
            Rect.localScale = new Vector3(Scale, Scale, 1);
        }

        /// <summary>Stage position of a point given in room coordinates (localToGlobal).</summary>
        public Vector2 RoomToStage(Vector2 room) => new Vector2(X + (room.x - PivotX) * Scale, Y + (room.y - PivotY) * Scale);

        /// <summary>Room coordinates of a stage point (globalToLocal).</summary>
        public Vector2 StageToRoom(Vector2 stage) => new Vector2(PivotX + (stage.x - X) / Scale, PivotY + (stage.y - Y) / Scale);

        static Vector2 ToStage(Vector2 screen) => UIStage.Current != null ? UIStage.Current.ScreenToStage(screen) : screen;

        // background bounds on the stage (_background.parent.localToGlobal(_background.bounds...))
        float BgLeft => X - PivotX * Scale;
        float BgRight => X + (backgroundWidth - PivotX) * Scale;
        float BgTop => Y - PivotY * Scale;
        float BgBottom => Y + (StageHeight - PivotY) * Scale;

        // ------------------------------------------------------------------ one touch (BEGAN / MOVED)

        public void OnInitializePotentialDrag(PointerEventData e)
        {
            began = last = ToStage(e.position);
            TouchDragged = false;
        }

        public void OnDrag(PointerEventData e)
        {
            if (Input.touchCount >= 2) return; // pinch, handled in Update
            var p = ToStage(e.position);
            if (Mathf.Abs(p.x - began.x) > DragThreshold || Mathf.Abs(p.y - began.y) > DragThreshold) TouchDragged = true;
            var delta = p - last;
            last = p;
            if (OnScaleBack) return;
            X += delta.x;
            Y += delta.y;
            // the original undoes the move on an axis that would show past the background
            if (BgLeft > StageWidth - backgroundWidth) X -= delta.x;
            else if (BgRight < StageWidth) X -= delta.x;
            if (BgTop > 0) Y -= delta.y;
            else if (BgBottom < StageHeight) Y -= delta.y;
            Apply();
        }

        // ------------------------------------------------------------------ two touches (pinch)

        void Update()
        {
            if (Input.touchCount != 2) return;
            var a = Input.GetTouch(0);
            var b = Input.GetTouch(1);
            if (a.phase == TouchPhase.Began || b.phase == TouchPhase.Began) TouchDragged = false;
            if (a.phase != TouchPhase.Moved && b.phase != TouchPhase.Moved) return;
            TouchDragged = true;
            if (OnScaleBack) return;
            Vector2 curA = ToStage(a.position), curB = ToStage(b.position);
            Vector2 prevA = ToStage(a.position - a.deltaPosition), prevB = ToStage(b.position - b.deltaPosition);
            float prevLen = (prevA - prevB).magnitude;
            if (prevLen <= 0) return;
            float sizeDiff = (curA - curB).magnitude / prevLen;
            Scale = Mathf.Clamp(Scale * sizeDiff, MinScale, MaxScale);
            // previous touch positions in the (re-scaled) room become the new pivot, the current midpoint its position
            Vector2 localA = StageToRoom(prevA), localB = StageToRoom(prevB);
            PivotX = (localA.x + localB.x) * 0.5f;
            PivotY = (localA.y + localB.y) * 0.5f;
            X = (curA.x + curB.x) * 0.5f;
            Y = (curA.y + curB.y) * 0.5f;
            if (BgLeft > StageWidth - backgroundWidth) PivotX = X = StageWidth - backgroundWidth;
            else if (BgRight < StageWidth) PivotX = X = StageWidth;
            if (BgTop > 0) PivotY = Y = 0;
            else if (BgBottom < StageHeight) PivotY = Y = StageHeight;
            Apply();
        }

        // ------------------------------------------------------------------ hint

        /// <summary>revealItem while zoomed in: back to x1 at (pivotX, pivotY) in 0.2 s (linear), touches blocked meanwhile.</summary>
        public void ScaleBack()
        {
            if (Scale <= 1) return;
            OnScaleBack = true;
            float s0 = Scale, x0 = X, y0 = Y, tx = PivotX, ty = PivotY;
            this.KillTweens();
            Tweens.To(this, 0.2f, p =>
            {
                Scale = Mathf.LerpUnclamped(s0, 1, p);
                X = Mathf.LerpUnclamped(x0, tx, p);
                Y = Mathf.LerpUnclamped(y0, ty, p);
                Apply();
            }).SetEase(Ease.Linear).OnComplete(() => OnScaleBack = false);
        }

        /// <summary>Sets the zoom directly (tests / tools): scale around the room point (pivotX, pivotY) shown at (x, y).</summary>
        public void SetTransform(float scale, float pivotX, float pivotY, float x, float y)
        {
            Scale = Mathf.Clamp(scale, MinScale, MaxScale);
            PivotX = pivotX;
            PivotY = pivotY;
            X = x;
            Y = y;
            Apply();
        }
    }
}
