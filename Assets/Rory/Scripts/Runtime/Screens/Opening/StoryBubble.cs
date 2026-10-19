using Rory.Tweening;
using Rory.UI;
using TMPro;
using UnityEngine;

namespace Rory.Screens.Opening
{
    /// <summary>
    /// Speech bubble of the story slideshows (OpeningScreen / EndingClass bubbleText + bubbleTextField):
    /// the bubble scales in from 0 in 0.3 s, then its text appears; hiding hides the text and scales the bubble out.
    /// The text sits at (bubble.x + 15, bubble.y + 28) with the same 269 px pivot as the bubble.
    /// </summary>
    public class StoryBubble : MonoBehaviour
    {
        public const float Duration = 0.3f;
        public const float TextOffsetX = 15, TextOffsetY = 28;

        [SerializeField] RectTransform bubble;
        [SerializeField] TextMeshProUGUI text;

        public RectTransform Bubble => bubble;
        public TextMeshProUGUI Text => text;

        /// <summary>Moves the bubble (and its text) to original coordinates.</summary>
        public void MoveTo(float x, float y)
        {
            bubble.SetStagePosition(x, y);
            text.rectTransform.SetStagePosition(x + TextOffsetX, y + TextOffsetY);
        }

        public void MoveToX(float x) => MoveTo(x, bubble.GetStagePosition().y);

        public float X => bubble.GetStagePosition().x;

        /// <summary>tweenBubbleText: scale in, then show <paramref name="message"/>.</summary>
        public void Show(string message)
        {
            bubble.gameObject.SetActive(true);
            // TweenLite's default overwrite mode kills the running scale tween of the bubble.
            bubble.KillTweens();
            bubble.TweenScale(1, Duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                text.text = message;
                text.gameObject.SetActive(true);
            });
        }

        /// <summary>tweenBubbleTextOut: hide the text, scale out, then hide the bubble.</summary>
        public void Hide()
        {
            text.gameObject.SetActive(false);
            bubble.KillTweens();
            bubble.TweenScale(0, Duration).SetEase(Ease.Linear).OnComplete(() => bubble.gameObject.SetActive(false));
        }

        /// <summary>TweenLite.killTweensOf(bubbleText).</summary>
        public void Stop() => bubble.KillTweens();
    }
}
