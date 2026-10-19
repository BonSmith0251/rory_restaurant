using Rory.Screens.Ending;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Rory.UI;

namespace Rory.EditorTools
{
    /// <summary>Generates Scenes/Ending.unity from EndingClass.initialize().</summary>
    public static class EndingBuilder
    {
        [MenuItem("Rory/Build/Scenes/Ending")]
        [RoryBuilder(100)]
        public static void Build()
        {
            var stage = UIBuild.NewScene(out _);
            var s = stage.transform;
            Sprite E(string n) => UIBuild.Sprite("ending", n);

            var touch = StoryBuild.TouchArea(s);
            UIBuild.Image(s, "FirstSlide", UIBuild.Background("first_slide_ending"));
            var secondSlide = Hidden(UIBuild.Image(s, "SecondSlide", UIBuild.Background("second_slide_ending")));
            var happy = E("happy");
            float smileX = 1024 - happy.LogicalSize().x;
            var smile = Hidden(UIBuild.Image(s, "Smile", happy, smileX, 0));
            var wonderSprite = E("wonder");
            var wonder = Hidden(UIBuild.Image(s, "Wonder", wonderSprite, 0, 600 - wonderSprite.LogicalSize().y));
            var bubble = Hidden(UIBuild.Image(s, "ThoughtBubble", E("bubble"), 36, 46));
            var restaurant = Hidden(UIBuild.ImageCentered(s, "Restaurant", E("big_restaurant"), 172, 156.5f));
            var toFrench = Hidden(UIBuild.ImageCentered(s, "ToFrench", E("to_french"), 169, 152.5f));
            var higherEdu = Hidden(UIBuild.ImageCentered(s, "HigherEdu", E("higher_edu"), 172, 153));
            var finalSprite = E("final");
            var finalSlide = Hidden(UIBuild.Image(s, "FinalSlide", finalSprite, 512, 0, finalSprite.LogicalSize().x / 2, 0));
            var text = StoryBuild.TextBox(s, 24, 10, 5, 1024);
            // bubble at (smile.x + 100, smile.y + smile.height - 50)
            var speech = StoryBuild.Bubble(s, E("bubble_text"), smileX + 100, happy.LogicalSize().y - 50, 16);

            var c = new GameObject("EndingController").AddComponent<EndingController>();
            UIBuild.SetField(c, "touchArea", touch);
            UIBuild.SetField(c, "secondSlide", secondSlide);
            UIBuild.SetField(c, "smile", smile);
            UIBuild.SetField(c, "wonder", wonder);
            UIBuild.SetField(c, "bubble", bubble);
            UIBuild.SetField(c, "restaurant", restaurant);
            UIBuild.SetField(c, "toFrench", toFrench);
            UIBuild.SetField(c, "higherEdu", higherEdu);
            UIBuild.SetField(c, "finalSlide", finalSlide);
            UIBuild.SetField(c, "textField", text);
            UIBuild.SetField(c, "speech", speech);

            UIBuild.SaveScene("Ending");
        }

        static Image Hidden(Image img)
        {
            img.gameObject.SetActive(false);
            return img;
        }
    }
}
