using Rory.Data;
using Rory.Screens.HiddenObject;
using Rory.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.EditorTools
{
    /// <summary>
    /// Generates Scenes/HiddenObject.unity from MiniGameHO.initialize() / HOClass.initUIObjects(), and the prefabs the
    /// screen instantiates from the layout data (Prefabs/HiddenObject: a room object, a list label).
    /// </summary>
    public static class HiddenObjectBuilder
    {
        const string PrefabFolder = RoryPaths.Prefabs + "/HiddenObject";
        public const string ItemPrefabPath = PrefabFolder + "/HiddenObjectItem.prefab";
        public const string LabelPrefabPath = PrefabFolder + "/HiddenObjectLabel.prefab";

        [MenuItem("Rory/Build/Prefabs/Hidden Object")]
        [RoryBuilder(50)]
        public static void BuildPrefabs()
        {
            // a room object (HOObjects): Image + touch handler; sprite, size and position are set at runtime
            var item = UIBuild.Rect(null, "HiddenObjectItem", typeof(CanvasRenderer), typeof(Image), typeof(HiddenObjectItem));
            var img = item.GetComponent<Image>();
            img.raycastTarget = true;
            UIBuild.SetField(item.GetComponent<HiddenObjectItem>(), "image", img);
            UIBuild.SavePrefab<HiddenObjectItem>(item.gameObject, ItemPrefabPath);

            // a list label (HOText): TextField(190, 130, label, "bromine", 25, 0), centred, pivot at its centre
            var label = UIBuild.TextCentered(null, "HiddenObjectLabel", "Item", UIBuild.Fonts.Bromine, 25, Color.black,
                0, 0, 190, 130, autoSize: true, localize: false);
            UIBuild.SavePrefab<TextMeshProUGUI>(label.gameObject, LabelPrefabPath);
        }

        [MenuItem("Rory/Build/Scenes/Hidden Object")]
        [RoryBuilder(100)]
        public static void Build()
        {
            if (AssetDatabase.LoadAssetAtPath<HiddenObjectItem>(ItemPrefabPath) == null ||
                AssetDatabase.LoadAssetAtPath<TextMeshProUGUI>(LabelPrefabPath) == null)
                BuildPrefabs();

            var stage = UIBuild.NewScene(out _);
            // loaded after NewScene (a new scene unloads assets loaded before it)
            var itemPrefab = AssetDatabase.LoadAssetAtPath<HiddenObjectItem>(ItemPrefabPath);
            var labelPrefab = AssetDatabase.LoadAssetAtPath<TextMeshProUGUI>(LabelPrefabPath);
            var s = stage.transform;
            Sprite H(string n) => UIBuild.Sprite("Hidden_Object", n);

            var controller = new GameObject("HiddenObjectController").AddComponent<HiddenObjectController>();

            // ---------------------------------------------------------------- room (zoomContainer)
            var zoomRt = UIBuild.Container(s, "ZoomContainer");
            var zoom = zoomRt.gameObject.AddComponent<HiddenObjectZoom>();
            var bg = UIBuild.Image(zoomRt, "Background", UIBuild.Background("HO1_A_BG"), 0, 0, raycast: true);
            var bgTouch = bg.gameObject.AddComponent<HiddenObjectBackground>();
            UIBuild.SetField(bgTouch, "controller", controller);
            // the objects are instantiated after the background at runtime (from Db.hiddenObjectLayouts)

            // ---------------------------------------------------------------- HUD (addChild order of initialize())
            UIBuild.Image(s, "UIBackground", H("UI_bg"), 0, 0);
            var money = UIBuild.Text(s, "MoneyText", "Goal: 0/0", UIBuild.Fonts.JennaSue, 30, Color.white, 30, 58, 150, 80,
                align: TextAlignmentOptions.Left, autoSize: true, localize: false);

            var hintFill = UIBuild.Image(s, "HintFill", H("hint_fill"), 887, 25);
            hintFill.type = Image.Type.Filled;
            hintFill.fillMethod = Image.FillMethod.Vertical;
            hintFill.fillOrigin = (int)Image.OriginVertical.Bottom;
            var hintBg = UIBuild.Image(s, "HintBackground", H("hint_bg"), 862, 0, raycast: true); // touchable: blocks clicks
            var hint = HintButton(s, hintBg.sprite.LogicalSize(), 862 + 5, 0 + 20);

            UIBuild.Image(s, "TimerBackground", H("timer_BG"), 299, 103);
            var timeFill = UIBuild.Image(s, "TimerFill", H("timer_fill"), 371, 108);
            timeFill.type = Image.Type.Filled;
            timeFill.fillMethod = Image.FillMethod.Horizontal;
            timeFill.fillOrigin = (int)Image.OriginHorizontal.Left;
            UIBuild.Image(s, "TimerGlass", H("timer_glass"), 347, 98);

            var pause = UIBuild.Button(s, "PauseButton", H("pause_btn"), 30, 35);
            pause.transform.localScale = new Vector3(0.9f, 0.9f, 1);
            var sound = UIBuild.Button(s, "SoundButton", H("sound_btn"), 85, 35);
            sound.transform.localScale = new Vector3(0.9f, 0.9f, 1);
            var mute = UIBuild.ImageCentered(s, "MuteIcon", H("mute"), 85, 35);
            var music = UIBuild.Button(s, "MusicButton", UIBuild.Sprite("window", "music_icon"), 140, 35);
            music.transform.localScale = new Vector3(0.9f, 0.9f, 1);
            var muteMusic = UIBuild.ImageCentered(s, "MuteMusicIcon", H("mute"), 140, 35);

            // ---------------------------------------------------------------- object list (initUIObjects)
            UIBuild.Quad(s, "ListedItemsPlace", 220, 0, 620, 100, new Color(1, 1, 1, 0.5f));
            var labels = UIBuild.Container(s, "Labels");

            // ---------------------------------------------------------------- particles, found objects
            var fire = RoryAssets.Load<ParticleConfig>($"{RoryPaths.Effects}/MiniGameHO_Fire.asset");
            var ps = UIBuild.Particles(s, "HintParticles", fire, 0, 0);
            var psFound = UIBuild.Particles(s, "FoundParticles", fire, 0, 0);
            var found = UIBuild.Container(s, "FoundObjects");

            TutorialBuilder.AddTo(stage);
            AchievementBuilder.AddNotifier(stage);

            UIBuild.SetField(controller, "zoom", zoom);
            UIBuild.SetField(controller, "background", bg);
            UIBuild.SetField(controller, "itemPrefab", itemPrefab);
            UIBuild.SetField(controller, "moneyText", money);
            UIBuild.SetField(controller, "hintFill", hintFill);
            UIBuild.SetField(controller, "hintButton", hint);
            UIBuild.SetField(controller, "timeFill", timeFill);
            UIBuild.SetField(controller, "pauseButton", pause);
            UIBuild.SetField(controller, "soundButton", sound);
            UIBuild.SetField(controller, "musicButton", music);
            UIBuild.SetField(controller, "muteIcon", mute.gameObject);
            UIBuild.SetField(controller, "muteMusicIcon", muteMusic.gameObject);
            UIBuild.SetField(controller, "labelLayer", labels);
            UIBuild.SetField(controller, "labelPrefab", labelPrefab);
            UIBuild.SetField(controller, "hintParticles", ps);
            UIBuild.SetField(controller, "foundParticles", psFound);
            UIBuild.SetField(controller, "foundLayer", found);

            UIBuild.SaveScene(GameScene.HiddenObject.ToString());
        }

        /// <summary>
        /// hintText: TNonStateButton(Texture.empty(hint_bg size), "HINT!", "bromine", 25, black) placed with x/y and no
        /// alignPivot, i.e. top-left at (x, y) and scaling around its centre.
        /// </summary>
        static Button HintButton(Transform parent, Vector2 size, float x, float y)
        {
            var img = UIBuild.Quad(parent, "HintButton", 0, 0, size.x, size.y, new Color(1, 1, 1, 0), raycast: true);
            StageLayout.Place(img.rectTransform, x + size.x / 2, y + size.y / 2, size.x, size.y, size.x / 2, size.y / 2);
            var button = img.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.targetGraphic = img;
            img.gameObject.AddComponent<RoryButton>();
            var t = UIBuild.Text(img.transform, "Label", "HINT!", UIBuild.Fonts.Bromine, 25, Color.black, 0, 0, size.x, size.y, autoSize: true);
            t.rectTransform.anchorMin = t.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            t.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            t.rectTransform.anchoredPosition = Vector2.zero;
            return button;
        }
    }
}
