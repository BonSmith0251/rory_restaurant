using System.Collections.Generic;
using Rory.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rory.Screens
{
    /// <summary>
    /// Publisher splashes (dumLoading): each image fades in (0.5 s), holds (1.5 s) and fades out; a click skips
    /// to the next one. Then the main menu.
    /// </summary>
    public class SplashController : ScreenController, IPointerClickHandler
    {
        [SerializeField] List<Image> splashes = new List<Image>();
        [SerializeField] float fadeDuration = 0.5f;
        [SerializeField] float holdDuration = 1.5f;

        int index = -1;
        Tween hold;
        bool hiding;

        void Start()
        {
            foreach (var s in splashes) s.color = new Color(1, 1, 1, 0);
            ShowNext();
        }

        void ShowNext()
        {
            index++;
            hiding = false;
            if (index >= splashes.Count)
            {
                GoTo(GameScene.MainMenu);
                return;
            }
            var img = splashes[index];
            img.TweenAlpha(1, fadeDuration).SetEase(Ease.Linear).OnComplete(() =>
                hold = Tweens.DelayedCall(holdDuration, Hide, this));
        }

        void Hide()
        {
            if (hiding || index >= splashes.Count) return;
            hiding = true;
            hold?.Kill();
            var img = splashes[index];
            img.KillTweens();
            img.TweenAlpha(0, fadeDuration).SetEase(Ease.Linear).OnComplete(ShowNext);
        }

        public void OnPointerClick(PointerEventData eventData) => Hide();
    }
}
