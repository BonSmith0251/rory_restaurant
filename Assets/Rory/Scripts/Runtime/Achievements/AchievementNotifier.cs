using System.Collections.Generic;
using Rory.Tweening;
using TMPro;
using UnityEngine;

namespace Rory.Achievements
{
    /// <summary>
    /// The small achievement banner that slides in from the right (x 1500 to 850 over 1 s, Expo), holds 1 s and
    /// slides out (Achievement.tweenNotif). Notifications are queued 6 s apart, like the original delayedCalls.
    /// Lives in the Overlay layer of game scenes; scene builders add it with AchievementBuilder.AddNotifier.
    /// </summary>
    public class AchievementNotifier : MonoBehaviour
    {
        public const float ShownX = 850, HiddenX = 1500, Y = 200;
        const float Spacing = 6;

        [SerializeField] RectTransform banner;
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI info;

        public static AchievementNotifier Current { get; private set; }

        readonly Queue<(string title, string info)> queue = new Queue<(string, string)>();
        float nextFree;

        void OnEnable() => Current = this;

        void OnDisable()
        {
            if (Current == this) Current = null;
        }

        void Awake() => banner.anchoredPosition = new Vector2(HiddenX, -Y);

        /// <summary>"Achievement unlocked" banner (Achievement.tweenAchievementNotif).</summary>
        public void ShowUnlocked(int id) => Enqueue(Achievements.Title[id], Achievements.Info[id]);

        /// <summary>Progress banner (Achievement.tweenAchievementProgress).</summary>
        public void ShowProgress(int id)
        {
            var p = GameManager.Player;
            if (p != null) Enqueue(Achievements.Title[id], Achievements.ProgressText(p, id));
        }

        /// <summary>Shows the banner for an <see cref="Achievements.Add"/> result.</summary>
        public void Show(int id, Achievements.Result result)
        {
            if (result == Achievements.Result.Unlocked) ShowUnlocked(id);
            else if (result == Achievements.Result.Progress) ShowProgress(id);
        }

        void Enqueue(string t, string i)
        {
            queue.Enqueue((t, i));
            float delay = Mathf.Max(0, nextFree - Time.time);
            nextFree = Time.time + delay + Spacing;
            Tweens.DelayedCall(delay, Play, this);
        }

        void Play()
        {
            if (queue.Count == 0) return;
            var (t, i) = queue.Dequeue();
            title.text = t;
            info.text = i;
            banner.KillTweens();
            banner.anchoredPosition = new Vector2(HiddenX, -Y);
            banner.TweenAnchoredX(ShownX, 1).SetEase(Ease.ExpoOut).OnComplete(() =>
                banner.TweenAnchoredX(HiddenX, 1).SetDelay(1).SetEase(Ease.ExpoIn));
        }
    }
}
