using Rory.Screens.LevelSelect;
using Rory.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.EditorTools
{
    /// <summary>Generates Scenes/LevelSelect.unity from LevelSelectScreen.initialize().</summary>
    public static class LevelSelectBuilder
    {
        static readonly int[] BadgeX = { 150, 386, 573, 188, 445, 651, 251 };
        static readonly int[] BadgeY = { 174, 121, 150, 332, 290, 307, 529 };

        // Day text fields: x, y, width, height, font size, rotation (degrees, clockwise), skewX, skewY (radians)
        static readonly (float x, float y, float w, float h, float size, float rot, float skewX, float skewY)[] Days =
        {
            (80, 70, 200, 150, 40, -20, 0, 0),
            (340, 40, 170, 110, 40, -6, -0.2f, 0.35f),
            (540, 97, 150, 100, 35, -10, -0.17f, 0),
            (130, 240, 200, 150, 40, -20, 0, 0),
            (390, 200, 180, 110, 40, -6, -0.2f, 0.29f),
            (590, 230, 160, 120, 40, -11, -0.17f, 0),
            (200, 420, 200, 160, 40, -15, 0, 0),
        };

        [MenuItem("Rory/Build/Scenes/Level Select")]
        [RoryBuilder(100)]
        public static void Build()
        {
            var stage = UIBuild.NewScene(out _);
            var s = stage.transform;
            Sprite W(string n) => UIBuild.Sprite("window", n);
            var goal = W("goal");
            var expert = W("expert");

            UIBuild.Image(s, "Background", UIBuild.Background("level_select_BG"));

            var dayTexts = new TextMeshProUGUI[7];
            var dayButtons = new Button[7];
            for (int i = 0; i < Days.Length; i++)
            {
                var d = Days[i];
                var t = SkewedText(s, $"Day{i + 1}", $"Day\n{i + 1}", UIBuild.Fonts.Bromine, d.size, TextAlignmentOptions.Center,
                    d.x, d.y, d.w, d.h, d.rot, d.skewX, d.skewY);
                t.fontStyle = FontStyles.Bold;
                t.raycastTarget = true;
                var b = t.gameObject.AddComponent<Button>();
                b.transition = Selectable.Transition.None;
                b.targetGraphic = t;
                dayTexts[i] = t;
                dayButtons[i] = b;
            }

            var badges = new Image[7];
            for (int i = 0; i < 7; i++)
            {
                badges[i] = UIBuild.ImageCentered(s, $"Badge{i + 1}", goal, BadgeX[i], BadgeY[i]);
                badges[i].rectTransform.localScale = new Vector3(0.3f, 0.3f, 1);
            }
            var badgeBig = UIBuild.Image(s, "BadgeBig", goal, 670, 380);
            badgeBig.rectTransform.localScale = new Vector3(0.75f, 0.75f, 1);

            var note = SkewedText(s, "NoteDescription", "", UIBuild.Fonts.JennaSue, 30, TextAlignmentOptions.TopLeft,
                430, 370, 350, 150, -5, -0.3f, 0.18f);
            // noteDescription2 is never filled by the original; kept for the layout.
            SkewedText(s, "NoteDescription2", "", UIBuild.Fonts.JennaSue, 30, TextAlignmentOptions.TopLeft,
                683, 393, 150, 150, -5, -0.3f, -0.1f);

            var check1 = UIBuild.Image(s, "CheckMark1", W("check_mark"), 635, 413);
            check1.rectTransform.localScale = new Vector3(0.5f, 0.5f, 1);
            var check2 = UIBuild.Image(s, "CheckMark2", W("check_mark"), 622, 446);
            check2.rectTransform.localScale = new Vector3(0.5f, 0.5f, 1);

            var play = UIBuild.Button(s, "PlayButton", W("small_btn_green"), 512, 550,
                label: "PLAY", font: UIBuild.Fonts.Bromine, fontSize: 30, labelColor: Color.white);
            var back = UIBuild.Button(s, "BackButton", W("small_btn_blue"), 880, 50,
                label: "Back", font: UIBuild.Fonts.Bromine, fontSize: 30, labelColor: Color.white);
            var arrow = UIBuild.Sprite("level_select", "arrow_button");
            var right = UIBuild.Button(s, "RightButton", arrow, 920, 400);
            var left = UIBuild.Button(s, "LeftButton", arrow, 80, 400);
            left.transform.localScale = new Vector3(-1, 1, 1);

            var c = new GameObject("LevelSelectController").AddComponent<LevelSelectController>();
            UIBuild.SetArray(c, "dayTexts", dayTexts);
            UIBuild.SetArray(c, "dayButtons", dayButtons);
            UIBuild.SetArray(c, "badges", badges);
            UIBuild.SetField(c, "badgeBig", badgeBig);
            UIBuild.SetField(c, "noteDescription", note);
            UIBuild.SetField(c, "checkMark1", check1.gameObject);
            UIBuild.SetField(c, "checkMark2", check2.gameObject);
            UIBuild.SetField(c, "playButton", play);
            UIBuild.SetField(c, "backButton", back);
            UIBuild.SetField(c, "rightButton", right);
            UIBuild.SetField(c, "leftButton", left);
            UIBuild.SetField(c, "goalSprite", goal);
            UIBuild.SetField(c, "expertSprite", expert);

            UIBuild.SaveScene("LevelSelect");
        }

        /// <summary>
        /// A TextField with Starling rotation and skew. RectTransforms cannot skew, so the 2x2 matrix
        /// Rotate(rotation) * Skew(skewX, skewY) is split (SVD) into rotation * scale on a parent container and a
        /// second rotation on the text: R(phi) * S * R(theta). The text keeps its top-left pivot at (x, y).
        /// </summary>
        static TextMeshProUGUI SkewedText(Transform parent, string name, string text, TMP_FontAsset font, float size,
            TextAlignmentOptions align, float x, float y, float w, float h, float rotationDegrees, float skewX, float skewY)
        {
            var holder = UIBuild.Container(parent, name + "Transform", x, y);
            var t = UIBuild.Text(holder, name, text, font, size, Color.black, 0, 0, w, h, align: align);

            // Starling (y down): a = cos(skewY), b = sin(skewY), c = -sin(skewX), d = cos(skewX), then rotate.
            float r = rotationDegrees * Mathf.Deg2Rad; // Starling rotation: clockwise in y-down space
            float a = Mathf.Cos(skewY), b = Mathf.Sin(skewY), c = -Mathf.Sin(skewX), d = Mathf.Cos(skewX);
            float cr = Mathf.Cos(r), sr = Mathf.Sin(r);
            // M (y down) = [[m00, m01], [m10, m11]] acting on column vectors (x, y)
            float m00 = cr * a - sr * b, m01 = cr * c - sr * d;
            float m10 = sr * a + cr * b, m11 = sr * c + cr * d;
            // to y up: F M F with F = diag(1, -1)
            float u00 = m00, u01 = -m01, u10 = -m10, u11 = m11;

            // 2x2 SVD: U = R(phi) * diag(sx, sy) * R(theta)
            float e = (u00 + u11) / 2, f = (u00 - u11) / 2, g = (u10 + u01) / 2, hh = (u10 - u01) / 2;
            float q = Mathf.Sqrt(e * e + hh * hh), rr = Mathf.Sqrt(f * f + g * g);
            float sx = q + rr, sy = q - rr;
            float a1 = Mathf.Atan2(g, f), a2 = Mathf.Atan2(hh, e);
            float theta = (a2 - a1) / 2, phi = (a2 + a1) / 2;

            holder.localEulerAngles = new Vector3(0, 0, phi * Mathf.Rad2Deg);
            holder.localScale = new Vector3(sx, sy, 1);
            t.rectTransform.localEulerAngles = new Vector3(0, 0, theta * Mathf.Rad2Deg);
            return t;
        }
    }
}
