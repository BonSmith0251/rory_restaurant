using Rory.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.Popups.Game
{
    /// <summary>
    /// showGoalIconSmall(): the goal / expert badge that pops up in the HUD when the day's goal (or the expert
    /// target) is reached: 0.6x at 50% alpha shrinking to 0.35x at full alpha in 0.8 s (Back.easeIn), hidden 1 s
    /// later. Created by <see cref="GameWindows.ShowGoalIconSmall(bool, Vector2)"/> just below the popup layer.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class GoalIconSmall : MonoBehaviour
    {
        public Image icon;

        internal void Play(bool expert, Vector2 stagePosition)
        {
            var lib = GameWindowsLibrary.Instance;
            if (icon == null) icon = GetComponent<Image>();
            icon.sprite = expert ? lib.expert : lib.goal;
            icon.SetNativeSize();
            icon.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            ((RectTransform)transform).anchoredPosition = new Vector2(stagePosition.x, -stagePosition.y);
            transform.localScale = new Vector3(0.6f, 0.6f, 1);
            var c = icon.color;
            c.a = 0.5f;
            icon.color = c;
            transform.TweenScale(0.35f, 0.8f).SetEase(Ease.BackIn);
            icon.TweenAlpha(1, 0.8f).SetEase(Ease.BackIn).OnComplete(() =>
                Tweens.DelayedCall(1, () => { if (this != null) Destroy(gameObject); }, this));
        }
    }
}
