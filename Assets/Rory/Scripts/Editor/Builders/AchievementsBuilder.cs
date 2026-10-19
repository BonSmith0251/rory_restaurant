using System.Linq;
using Rory.Screens.Achievements;
using Rory.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using AchievementList = global::Rory.Achievements.Achievements;

namespace Rory.EditorTools
{
    /// <summary>Generates Scenes/Achievements.unity from AchievementScreen.initialize().</summary>
    public static class AchievementsBuilder
    {
        // Atlas names of the badges; locked ones are <lockedName>_locked (the original reuses middle_east_recipe for 2..4).
        static readonly string[] Names =
        {
            "service_exellence", "big_business", "western_recipe", "chinese_recipe", "middle_east_recipe",
            "secret_ingredient", "hawkeye", "froyo_master", "qualified_chef", "international_chef",
        };
        static readonly string[] LockedNames =
        {
            "service_exellence", "big_business", "middle_east_recipe", "middle_east_recipe", "middle_east_recipe",
            "secret_ingredient", "hawkeye", "froyo_master", "qualified_chef", "international_chef",
        };

        [MenuItem("Rory/Build/Scenes/Achievements")]
        [RoryBuilder(100)]
        public static void Build()
        {
            var stage = UIBuild.NewScene(out _);
            var s = stage.transform;
            Sprite A(string n) => UIBuild.Sprite("achievement", n);

            UIBuild.Image(s, "Background", UIBuild.Background("achievement_BG"));

            // outerSprite.clipRect = (260, 100, 480, 470); innerSprite (pivot 237.5, 235) at (512, 350)
            var clip = UIBuild.Rect(s, "Clip", typeof(RectMask2D));
            StageLayout.Place(clip, AchievementsController.ClipX, AchievementsController.ClipY, 480, 470);
            var column = UIBuild.Container(clip, "Column",
                AchievementsController.ColumnX - AchievementsController.ClipX, AchievementsController.CenterY - AchievementsController.ClipY);
            var content = UIBuild.Container(column, "Content", -237.5f, -235);
            var unlocked = Names.Select(A).ToArray();
            var locked = LockedNames.Select(n => A(n + "_locked")).ToArray();
            var badges = new Image[Names.Length];
            for (int i = 0; i < Names.Length; i++)
                badges[i] = UIBuild.ImageCentered(content, $"Achievement{i}", locked[i], 237.5f,
                    i * AchievementsController.SlotHeight + AchievementsController.SlotHeight / 2);

            var barSprite = A("scroll_bar");
            var bar = UIBuild.Image(s, "ScrollBar", barSprite, 770, 120, barSprite.rect.width / 2, 0, true);
            var scroller = UIBuild.ImageCentered(s, "Scroller", A("scroller"), 770, 120, true);

            var title = UIBuild.Text(s, "Title", AchievementList.Title[0], UIBuild.Fonts.JennaSue, 40, Color.black,
                512, 145, 465, 150, 237.5f, 75);
            title.fontStyle = FontStyles.Bold;
            var info = UIBuild.Text(s, "Info", AchievementList.InfoProgress[0], UIBuild.Fonts.JennaSue, 35, Color.black,
                512, 500, 465, 150, 237.5f, 75);

            // Back: the "arrow" cSprite flipped (scaleX = -1), centred at (60, 550); a plain touch, no click sound.
            var backImg = UIBuild.ImageCentered(s, "BackButton", A("arrow"), 60, 550, true);
            backImg.rectTransform.localScale = new Vector3(-1, 1, 1);
            var back = backImg.gameObject.AddComponent<Button>();
            back.transition = Selectable.Transition.None;
            back.targetGraphic = backImg;

            var c = new GameObject("AchievementsController").AddComponent<AchievementsController>();
            foreach (var (img, part) in new[] { (bar, AchievementScrollHandle.Part.Bar), (scroller, AchievementScrollHandle.Part.Scroller) })
            {
                var h = img.gameObject.AddComponent<AchievementScrollHandle>();
                h.part = part;
                UIBuild.SetField(h, "controller", c);
            }
            UIBuild.SetField(c, "column", column);
            UIBuild.SetArray(c, "badges", badges);
            UIBuild.SetArray(c, "unlockedSprites", unlocked);
            UIBuild.SetArray(c, "lockedSprites", locked);
            UIBuild.SetField(c, "bar", bar.rectTransform);
            UIBuild.SetField(c, "scroller", scroller.rectTransform);
            UIBuild.SetField(c, "title", title);
            UIBuild.SetField(c, "info", info);
            UIBuild.SetField(c, "backButton", back);

            UIBuild.SaveScene("Achievements");
        }
    }
}
