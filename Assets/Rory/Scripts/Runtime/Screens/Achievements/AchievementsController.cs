using Rory.Save;
using Rory.Tweening;
using Rory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AchievementList = global::Rory.Achievements.Achievements;

namespace Rory.Screens.Achievements
{
    /// <summary>
    /// Achievement room (AchievementScreen): the ten badges in a clipped column scrolled with the bar on the right,
    /// title and info (or progress) of the selected one, and the back arrow (main menu or kitchen).
    /// </summary>
    public class AchievementsController : ScreenController
    {
        /// <summary>Height of one badge slot in the column.</summary>
        public const float SlotHeight = 470;
        /// <summary>Stage y of the column's pivot when the first badge is centred.</summary>
        public const float CenterY = 350;
        /// <summary>Stage position of the clip rectangle the column is drawn in (outerSprite.clipRect).</summary>
        public const float ClipX = 260, ClipY = 100;
        public const float ColumnX = 512;

        [SerializeField] RectTransform column;
        [SerializeField] Image[] badges;
        [SerializeField] Sprite[] unlockedSprites;
        [SerializeField] Sprite[] lockedSprites;
        [SerializeField] RectTransform bar;
        [SerializeField] RectTransform scroller;
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI info;
        [SerializeField] Button backButton;

        float barHeight;
        float columnHeight;

        PlayerProfile Profile => Player ?? new PlayerProfile();

        /// <summary>Stage y of the column (innerSprite.y).</summary>
        public float ColumnY
        {
            get => ClipY - column.anchoredPosition.y;
            private set => column.anchoredPosition = new Vector2(ColumnX - ClipX, -(value - ClipY));
        }

        float BarY => bar.GetStagePosition().y;

        float ScrollerY
        {
            get => scroller.GetStagePosition().y;
            set => scroller.SetStagePosition(scroller.GetStagePosition().x, value);
        }

        public TextMeshProUGUI Title => title;
        public TextMeshProUGUI Info => info;

        void Start()
        {
            var p = Profile;
            for (int i = 0; i < badges.Length; i++)
            {
                badges[i].sprite = p.unlocked[i] ? unlockedSprites[i] : lockedSprites[i];
                badges[i].SetNativeSize();
            }
            columnHeight = badges.Length * SlotHeight - SlotHeight;
            barHeight = bar.rect.height;
            backButton.onClick.AddListener(Back);

            // Starts on the first achievement: its title is preset; info or progress text
            title.text = AchievementList.Title[AchievementList.ServiceExcellence];
            info.text = InfoText(p, AchievementList.ServiceExcellence);
        }

        static string InfoText(PlayerProfile p, int id) =>
            p.unlocked[id] ? AchievementList.Info[id] : AchievementList.ProgressText(p, id);

        void Back() => GoTo(Session.achievementsBackToMainMenu ? GameScene.MainMenu : GameScene.Kitchen);

        // ------------------------------------------------------------------ scrolling (touchScroller)

        public void ScrollBegan(AchievementScrollHandle.Part part, float stageY)
        {
            title.gameObject.SetActive(false);
            info.gameObject.SetActive(false);
            if (part == AchievementScrollHandle.Part.Bar) ScrollerY = stageY;
            float ratio = (ScrollerY - BarY) / barHeight;
            column.KillTweens();
            Tweens.Float(column, ColumnY, -(ratio * columnHeight) + CenterY, 0.5f, y => ColumnY = y).SetEase(Ease.Linear);
        }

        public void ScrollMoved(float stageY)
        {
            if (ScrollerY >= BarY && ScrollerY <= BarY + barHeight) ScrollerY = stageY;
            if (ScrollerY < BarY) ScrollerY = BarY;
            if (ScrollerY > BarY + barHeight) ScrollerY = BarY + barHeight;
            float ratio = (ScrollerY - BarY) / barHeight;
            ColumnY = -(ratio * columnHeight) + CenterY;
        }

        public void ScrollEnded()
        {
            int index = (int)((ScrollerY - BarY) / (barHeight / 10));
            if (index > 9) index = 9;
            if (index < 10) ShowAchievement(index);
        }

        /// <summary>tweenInnerSprite: scrolls achievement <paramref name="index"/> to the centre and shows its texts.</summary>
        public void ShowAchievement(int index)
        {
            var p = Profile;
            column.KillTweens();
            title.text = AchievementList.Title[index];
            info.text = InfoText(p, index);
            Tweens.Float(column, ColumnY, -(index * SlotHeight) + CenterY, 0.3f, y => ColumnY = y).SetEase(Ease.Linear).OnComplete(() =>
            {
                info.gameObject.SetActive(true);
                title.gameObject.SetActive(true);
            });
        }
    }
}
