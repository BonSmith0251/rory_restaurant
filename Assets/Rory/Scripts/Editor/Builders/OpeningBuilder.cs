using Rory.Screens.Opening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.EditorTools
{
    /// <summary>Generates Scenes/Opening.unity from OpeningScreen.initialize().</summary>
    public static class OpeningBuilder
    {
        [MenuItem("Rory/Build/Scenes/Opening")]
        [RoryBuilder(100)]
        public static void Build()
        {
            var stage = UIBuild.NewScene(out _);
            var s = stage.transform;
            Sprite O(string n) => UIBuild.Sprite("opening", n);

            var touch = StoryBuild.TouchArea(s);
            UIBuild.Image(s, "FirstSlide", UIBuild.Background("first_slide_opening"));
            var mask = Hidden(UIBuild.Image(s, "Mask", RoryAssets.Load<Sprite>($"{RoryPaths.Backgrounds}/mask.png")));
            var smileSprite = O("smile");
            float smileX = 1024 - smileSprite.rect.width;
            var smile = Hidden(UIBuild.Image(s, "Smile", smileSprite, smileX, 0));
            var cry = Hidden(UIBuild.Image(s, "Cry", O("cry"), smileX, 0));
            var think = Hidden(UIBuild.Image(s, "Think", O("think"), smileX, 0));
            var studentLoan = Hidden(UIBuild.Image(s, "StudentLoan", O("student_loan"), 10, 10));
            var brosur = Hidden(UIBuild.Image(s, "Brosur", O("brosur"), 0, 10));
            var lastSlide = Hidden(UIBuild.Image(s, "LastSlide", UIBuild.Background("last_slide_opening")));
            var lightBulb = Hidden(UIBuild.Image(s, "LightBulb", O("lightbulb"), 250, 20));
            var text = StoryBuild.TextBox(s, 30, 10, 0, 1004);

            // bubble at (cry.x + 100, cry.y + cry.height - 50)
            float cryH = cry.sprite.rect.height;
            var bubble = StoryBuild.Bubble(s, O("bubble_text"), smileX + 100, cryH - 50, 25);

            var c = new GameObject("OpeningController").AddComponent<OpeningController>();
            UIBuild.SetField(c, "touchArea", touch);
            UIBuild.SetField(c, "mask", mask);
            UIBuild.SetField(c, "smile", smile);
            UIBuild.SetField(c, "cry", cry);
            UIBuild.SetField(c, "think", think);
            UIBuild.SetField(c, "studentLoan", studentLoan);
            UIBuild.SetField(c, "brosur", brosur);
            UIBuild.SetField(c, "lastSlide", lastSlide);
            UIBuild.SetField(c, "lightBulb", lightBulb);
            UIBuild.SetField(c, "textField", text);
            UIBuild.SetField(c, "bubble", bubble);

            UIBuild.SaveScene("Opening");
        }

        static Image Hidden(Image img)
        {
            img.gameObject.SetActive(false);
            return img;
        }
    }

    /// <summary>Shared parts of the Opening and Ending slideshows.</summary>
    public static class StoryBuild
    {
        /// <summary>Full-screen invisible Quad that takes the taps (touchQuad, alpha 0, at the bottom of the list).</summary>
        public static Button TouchArea(Transform parent)
        {
            var quad = UIBuild.Quad(parent, "TouchArea", 0, 0, 1024, 600, new Color(1, 1, 1, 0), true);
            var b = quad.gameObject.AddComponent<Button>();
            b.transition = Selectable.Transition.None;
            b.targetGraphic = quad;
            return b;
        }

        /// <summary>White 70% Quad (1024x50) at the bottom with the black Pfennig narration text (left / top).</summary>
        public static TextMeshProUGUI TextBox(Transform parent, float fontSize, float textX, float textY, float textWidth)
        {
            UIBuild.Quad(parent, "TextBox", 0, 550, 1024, 50, new Color(1, 1, 1, 0.7f));
            return UIBuild.Text(parent, "Narration", "", UIBuild.Fonts.Pfennig, fontSize, Color.black,
                textX, 550 + textY, textWidth, 50, align: TextAlignmentOptions.TopLeft);
        }

        /// <summary>bubbleText (pivotX 269, scale 0, hidden) and bubbleTextField (Bromine, centred, hidden).</summary>
        public static StoryBubble Bubble(Transform parent, Sprite sprite, float x, float y, float fontSize)
        {
            var img = UIBuild.Image(parent, "Bubble", sprite, x, y, 269, 0);
            img.rectTransform.localScale = new Vector3(0, 0, 1);
            img.gameObject.SetActive(false);
            var size = sprite.rect.size;
            var text = UIBuild.Text(parent, "BubbleText", "AAAAA", UIBuild.Fonts.Bromine, fontSize, Color.black,
                x + StoryBubble.TextOffsetX, y + StoryBubble.TextOffsetY, size.x - 30, size.y - 28, 269, 0);
            text.gameObject.SetActive(false);
            var b = img.gameObject.AddComponent<StoryBubble>();
            UIBuild.SetField(b, "bubble", img.rectTransform);
            UIBuild.SetField(b, "text", text);
            return b;
        }
    }
}
