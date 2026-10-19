using Rory.Achievements;
using Rory.UI;
using UnityEngine;

namespace Rory.EditorTools
{
    public static class AchievementBuilder
    {
        /// <summary>
        /// Adds the achievement banner to a scene's Overlay layer (TWindowDisplay.achievementNotif: widget_BG scaled
        /// 0.5 x 0.2 with a red title and a black info text).
        /// </summary>
        public static AchievementNotifier AddNotifier(UIStage stage)
        {
            var overlay = UIBuild.Overlay(stage);
            var root = UIBuild.Container(overlay, "AchievementNotifier");
            var n = root.gameObject.AddComponent<AchievementNotifier>();
            var banner = UIBuild.Container(root, "Banner", AchievementNotifier.HiddenX, AchievementNotifier.Y);
            var bg = UIBuild.ImageCentered(banner, "Background", UIBuild.Sprite("window", "widget_BG"), 0, 0);
            bg.rectTransform.localScale = new Vector3(0.5f, 0.2f, 1);
            float w = 647 * 0.5f - 30, h = 558 * 0.2f;
            var title = UIBuild.TextCentered(banner, "Title", "MASTER", UIBuild.Fonts.Bromine, 20, Color.red, 0, -30, w, h / 2 - 20);
            var info = UIBuild.TextCentered(banner, "Info", "", UIBuild.Fonts.Bromine, 15, Color.black, 0, 10, w, h - 10);
            UIBuild.SetField(n, "banner", banner);
            UIBuild.SetField(n, "title", title);
            UIBuild.SetField(n, "info", info);
            return n;
        }
    }
}
