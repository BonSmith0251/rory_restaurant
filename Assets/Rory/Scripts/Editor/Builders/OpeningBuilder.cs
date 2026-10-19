using Rory.Screens.Opening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Rory.UI;

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
            // The three portraits cross-fade on top of each other, so their circles must coincide: each is placed
            // with its pivot at its circle's centre and scaled to the circle width of "cry" (the reference, at the
            // top-right corner where the original put the faces). Measured from the images, so HD art aligns too.
            var refSprite = O("cry");
            var refCircle = PortraitCircle(refSprite);
            float smileX = 1024 - refSprite.LogicalSize().x;
            var faceCentre = new Vector2(smileX + refCircle.center.x, refCircle.center.y);
            Image Face(string name, Sprite sprite)
            {
                var circle = PortraitCircle(sprite);
                var img = UIBuild.Image(s, name, sprite, faceCentre.x, faceCentre.y, circle.center.x, circle.center.y);
                float k = refCircle.width / circle.width;
                img.rectTransform.localScale = new Vector3(k, k, 1);
                return Hidden(img);
            }
            var smile = Face("Smile", O("smile"));
            var cry = Face("Cry", refSprite);
            var think = Face("Think", O("think"));
            var studentLoan = Hidden(UIBuild.Image(s, "StudentLoan", O("student_loan"), 10, 10));
            var brosur = Hidden(UIBuild.Image(s, "Brosur", O("brosur"), 0, 10));
            var lastSlide = Hidden(UIBuild.Image(s, "LastSlide", UIBuild.Background("last_slide_opening")));
            var lightBulb = Hidden(UIBuild.Image(s, "LightBulb", O("lightbulb"), 250, 20));
            var text = StoryBuild.TextBox(s, 30, 10, 0, 1004);

            // bubble at (cry.x + 100, cry.y + cry.height - 50)
            float cryH = refSprite.LogicalSize().y;
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

        /// <summary>
        /// The portrait circle of a face image, in layout units from its top-left: the bounds of the rows whose
        /// longest opaque run is at least 15% of the width (ignores sparkles and other small bits outside the circle).
        /// </summary>
        public static Rect PortraitCircle(Sprite sprite)
        {
            var tex = new Texture2D(2, 2);
            tex.LoadImage(System.IO.File.ReadAllBytes(AssetDatabase.GetAssetPath(sprite)));
            int w = tex.width, h = tex.height;
            var px = tex.GetPixels32();
            Object.DestroyImmediate(tex);
            int minX = int.MaxValue, maxX = -1, minY = int.MaxValue, maxY = -1;
            for (int row = 0; row < h; row++)
            {
                int y = h - 1 - row; // Unity rows start at the bottom
                int best = 0, bestFrom = 0, bestTo = -1, run = -1;
                for (int x = 0; x <= w; x++)
                {
                    bool opaque = x < w && px[y * w + x].a > 200;
                    if (opaque && run < 0) run = x;
                    if (!opaque && run >= 0)
                    {
                        if (x - run > best) { best = x - run; bestFrom = run; bestTo = x - 1; }
                        run = -1;
                    }
                }
                if (best < w * 0.15f) continue;
                minX = Mathf.Min(minX, bestFrom);
                maxX = Mathf.Max(maxX, bestTo);
                minY = Mathf.Min(minY, row);
                maxY = Mathf.Max(maxY, row);
            }
            if (maxX < 0) return new Rect(Vector2.zero, sprite.LogicalSize());
            float unit = 1 / sprite.Scale(); // image pixels -> layout units (HD art)
            return new Rect(minX * unit, minY * unit, (maxX - minX + 1) * unit, (maxY - minY + 1) * unit);
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

        /// <summary>White 70% Quad (full view width x 50) at the bottom with the black Pfennig narration text (left / top).</summary>
        public static TextMeshProUGUI TextBox(Transform parent, float fontSize, float textX, float textY, float textWidth)
        {
            // the bar spans the whole 16:9 view; the text stays where the original put it
            UIBuild.Quad(parent, "TextBox", -UIStage.SideMargin, 550, UIStage.ViewWidth, 50, new Color(1, 1, 1, 0.7f));
            return UIBuild.Text(parent, "Narration", "", UIBuild.Fonts.Pfennig, fontSize, Color.black,
                textX, 550 + textY, textWidth, 50, align: TextAlignmentOptions.TopLeft);
        }

        /// <summary>bubbleText (pivotX 269, scale 0, hidden) and bubbleTextField (Bromine, centred, hidden).</summary>
        public static StoryBubble Bubble(Transform parent, Sprite sprite, float x, float y, float fontSize)
        {
            var img = UIBuild.Image(parent, "Bubble", sprite, x, y, 269, 0);
            img.rectTransform.localScale = new Vector3(0, 0, 1);
            img.gameObject.SetActive(false);
            var size = sprite.LogicalSize();
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
