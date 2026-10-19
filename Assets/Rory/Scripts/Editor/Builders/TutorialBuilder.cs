using Rory.Tutorials;
using Rory.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.EditorTools
{
    /// <summary>
    /// The tutorial bubble (Tutorial.init): prefab Prefabs/Tutorial/Tutorial.prefab, placed in a scene by
    /// <see cref="AddTo"/>. Draw order textbox, avatar, arrow, Next button, text like the original addChild order.
    /// </summary>
    public static class TutorialBuilder
    {
        public const string PrefabPath = RoryPaths.Prefabs + "/Tutorial/Tutorial.prefab";

        [MenuItem("Rory/Build/Tutorial Prefab")]
        [RoryBuilder(30)]
        public static void BuildPrefab()
        {
            var root = (RectTransform)new GameObject("Tutorial", typeof(RectTransform)).transform;
            root.gameObject.layer = LayerMask.NameToLayer("UI");
            root.anchorMin = root.anchorMax = StageLayout.TopLeft;
            root.pivot = StageLayout.TopLeft;
            root.sizeDelta = Vector2.zero;
            root.anchoredPosition = Vector2.zero;
            var manager = root.gameObject.AddComponent<TutorialManager>();
            Sprite W(string n) => UIBuild.Sprite("window", n);

            // Laid out for a centred line (posX 512, posY 300); TutorialManager.Show moves everything per line.
            const float x = 512, y = 300;

            // textBox: centre pivot, scaleX 1.2
            var boxSprite = W("textbox");
            var box = UIBuild.ImageCentered(root, "TextBox", boxSprite, x, y);
            box.rectTransform.localScale = new Vector3(TutorialManager.TextBoxScaleX, 1, 1);
            float boxW = boxSprite.LogicalSize().x, boxH = boxSprite.LogicalSize().y;
            float scaledW = boxW * TutorialManager.TextBoxScaleX;

            // ava: pivotY = height, at the box's left edge + 5, bottom - 5
            var avaSprite = W("rory");
            var ava = UIBuild.Image(root, "Avatar", avaSprite, x - boxW / 2 * TutorialManager.TextBoxScaleX + 5, y + boxH / 2 - 5,
                0, avaSprite.LogicalSize().y);

            // arrow: centre pivot (texture points right; scaleX = arrowDirection)
            var arrow = UIBuild.ImageCentered(root, "Arrow", W("arrow"), x, y);

            // TNonStateButton(small_btn_blue, "Next", "bromine", 20, 0) with alignPivot, at the box centre + 110
            var button = UIBuild.Button(root, "NextButton", W("small_btn_blue"), x, y + TutorialManager.ButtonOffsetY,
                label: "Next", font: UIBuild.Fonts.Bromine, fontSize: 20, labelColor: Color.black);
            var label = button.GetComponentInChildren<TextMeshProUGUI>();

            // ExtendTextField(box.width - 135, box.height - 10, "", "pfennig", 20, 0), pivot (box.width/2 - 115, box.height/2 - 5)
            // (box.width includes the 1.2 scale, its pivot doesn't). Auto-size (max 20) so longer translations fit the box.
            var text = UIBuild.Text(root, "Text", "", UIBuild.Fonts.Pfennig, 20, Color.black, x, y,
                scaledW - 135, boxH - 10, scaledW / 2 - 115, boxH / 2 - 5, TextAlignmentOptions.Center, autoSize: true);

            UIBuild.SetField(manager, "textBox", box.rectTransform);
            UIBuild.SetField(manager, "avatar", ava.rectTransform);
            UIBuild.SetField(manager, "arrow", arrow.rectTransform);
            UIBuild.SetField(manager, "nextButton", button);
            UIBuild.SetField(manager, "nextLabel", label);
            UIBuild.SetField(manager, "text", text);

            foreach (Transform c in root) c.gameObject.SetActive(false);
            UIBuild.SavePrefab<TutorialManager>(root.gameObject, PrefabPath);
        }

        /// <summary>
        /// Adds the tutorial bubble to a scene: an instance of the prefab on the stage, directly below the Popups
        /// layer (in the original, windows and their black screen were drawn over the bubble and blocked its button;
        /// TutorialManager keeps that order at runtime too). Pass <paramref name="inOverlay"/> to put it in the
        /// Overlay layer above the popups instead. Call it after adding the screen content.
        /// </summary>
        public static TutorialManager AddTo(UIStage stage, bool inOverlay = false)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<TutorialManager>(PrefabPath);
            if (prefab == null)
            {
                BuildPrefab();
                prefab = AssetDatabase.LoadAssetAtPath<TutorialManager>(PrefabPath);
            }
            var parent = inOverlay ? UIBuild.Overlay(stage) : stage.transform;
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab.gameObject, parent);
            go.name = "Tutorial";
            var manager = go.GetComponent<TutorialManager>();
            if (inOverlay)
            {
                UIBuild.SetField(manager, "keepBelowPopups", false);
            }
            else
            {
                // above everything added so far; UIBuild.SaveScene then moves Popups and Overlay above it
                go.transform.SetAsLastSibling();
            }
            return manager;
        }
    }
}
