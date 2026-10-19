using Rory.Data;
using Rory.Popups;
using Rory.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.EditorTools
{
    /// <summary>
    /// Builds the two standard windows of TWindowDisplay. Part positions are relative to the window centre
    /// (stage 512, 300), y down, exactly like the offsets in LevelEvent.xml.
    /// </summary>
    public static class WindowBuilder
    {
        public const string BigPrefab = RoryPaths.Prefabs + "/Popups/BigWindow.prefab";
        public const string SmallPrefab = RoryPaths.Prefabs + "/Popups/SmallWindow.prefab";

        [MenuItem("Rory/Build/Window Prefabs")]
        [RoryBuilder(10)]
        public static void BuildPrefabs()
        {
            var big = BuildWindow<WindowPopup>(null, false, "BigWindow");
            var small = BuildWindow<WindowPopup>(null, true, "SmallWindow");
            var db = RoryAssets.LoadOrCreate<GameDatabase>(RoryPaths.Resources + "/GameDatabase.asset");
            db.bigWindow = UIBuild.SavePrefab<WindowPopup>(big.gameObject, BigPrefab);
            db.smallWindow = UIBuild.SavePrefab<WindowPopup>(small.gameObject, SmallPrefab);
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Creates a window under <paramref name="parent"/> (a PopupManager layer, or null for a prefab root).
        /// The returned popup is active; scene popups should be deactivated by the caller.
        /// </summary>
        public static T BuildWindow<T>(Transform parent, bool small, string name) where T : WindowPopup
        {
            RectTransform root;
            if (parent != null) root = UIBuild.Container(parent, name, 512, 300);
            else
            {
                root = (RectTransform)new GameObject(name, typeof(RectTransform)).transform;
                root.anchorMin = root.anchorMax = StageLayout.TopLeft;
                root.pivot = StageLayout.TopLeft;
                root.sizeDelta = Vector2.zero;
                root.anchoredPosition = new Vector2(512, -300);
            }
            var w = root.gameObject.AddComponent<T>();
            var btn = UIBuild.Sprite("window", "small_btn_blue");

            if (!small)
            {
                w.background = UIBuild.ImageCentered(root, "Background", UIBuild.Sprite("window", "widget_BG"), 0, 0, true);
                w.picture = UIBuild.ImageCentered(root, "Picture", null, 0, -30);
                w.picture.gameObject.SetActive(false);
                w.title = UIBuild.TextCentered(root, "Title", "SUMMARY", UIBuild.Fonts.JennaSue, 60, Color.black, 0, -220, 547, 100);
                w.info = UIBuild.TextCentered(root, "Info", "", UIBuild.Fonts.Bromine, 25, Color.black, 0, -30, 547, 300);
                w.leftButton = WindowButton(root, "LeftButton", btn, -170, 200, 0.8f, "Left");
                w.rightButton = WindowButton(root, "RightButton", btn, 170, 200, 0.8f, "Right");
                w.uniButton = WindowButton(root, "UniButton", btn, 0, 200, 0.8f, "OK");
            }
            else
            {
                w.background = UIBuild.ImageCentered(root, "Background", UIBuild.Sprite("window", "small_widget_BG"), 0, 0, true);
                w.picture = UIBuild.ImageCentered(root, "Picture", null, 0, -30);
                w.picture.gameObject.SetActive(false);
                w.title = UIBuild.TextCentered(root, "Title", "Exit?", UIBuild.Fonts.JennaSue, 60, Color.black, 13, -110, 389, 70);
                w.info = UIBuild.TextCentered(root, "Info", "", UIBuild.Fonts.JennaSue, 40, Color.black, 13, -30, 389, 250);
                w.leftButton = WindowButton(root, "LeftButton", btn, -100, 130, 0.9f, "Left");
                w.rightButton = WindowButton(root, "RightButton", btn, 100, 130, 0.9f, "Right");
                // The small window's single button is text only in the original (its image is hidden).
                w.uniButton = WindowButton(root, "UniButton", btn, 0, 130, 1, "OK");
                w.uniButton.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
            return w;
        }

        public static Button WindowButton(Transform parent, string name, Sprite sprite, float x, float y, float scale, string label)
        {
            var b = UIBuild.Button(parent, name, sprite, x, y, label: label, font: UIBuild.Fonts.Bromine, fontSize: 20, labelColor: Color.black);
            b.transform.localScale = new Vector3(scale, scale, 1);
            return b;
        }

        /// <summary>Sets a button's label text.</summary>
        public static void Label(Button b, string text) => b.GetComponentInChildren<TextMeshProUGUI>(true).text = text;

        /// <summary>Moves a window part to (x, y) relative to the window centre.</summary>
        public static void Move(Component part, float x, float y) => ((RectTransform)part.transform).anchoredPosition = new Vector2(x, -y);
    }
}
