using Rory.Data;
using Rory.Popups;
using Rory.Popups.Game;
using Rory.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.EditorTools
{
    /// <summary>
    /// Builds the in-game windows of TWindowDisplay (pause, confirmations, summaries, day event windows, the HUD goal
    /// badge) into Prefabs/Popups/Game and references them from Resources/GameWindows.asset (GameWindowsLibrary).
    /// Positions are relative to the window centre (stage 512, 300), y down, as in TWindowDisplay.initializeWindow/showWindow.
    /// </summary>
    public static class GameWindowsBuilder
    {
        const string Folder = RoryPaths.Prefabs + "/Popups/Game";
        const string LibraryPath = RoryPaths.Resources + "/GameWindows.asset";

        static Sprite W(string name) => UIBuild.Sprite("window", name);

        [MenuItem("Rory/Build/Game Window Prefabs")]
        [RoryBuilder(20)]
        public static void BuildPrefabs()
        {
            var lib = RoryAssets.LoadOrCreate<GameWindowsLibrary>(LibraryPath);
            lib.pause = UIBuild.SavePrefab<PausePopup>(BuildPause().gameObject, Folder + "/GamePause.prefab");
            lib.confirm = UIBuild.SavePrefab<WindowPopup>(BuildConfirm().gameObject, Folder + "/GameConfirm.prefab");
            lib.info = UIBuild.SavePrefab<WindowPopup>(BuildInfo().gameObject, Folder + "/GameInfo.prefab");
            lib.result = UIBuild.SavePrefab<ResultPopup>(BuildResult().gameObject, Folder + "/GameResult.prefab");
            lib.eventBig = UIBuild.SavePrefab<DayEventPopup>(BuildEvent(false).gameObject, Folder + "/GameEventBig.prefab");
            lib.eventSmall = UIBuild.SavePrefab<DayEventPopup>(BuildEvent(true).gameObject, Folder + "/GameEventSmall.prefab");
            lib.goalIconSmall = UIBuild.SavePrefab<GoalIconSmall>(BuildGoalIconSmall().gameObject, Folder + "/GoalIconSmall.prefab");
            lib.buttonOn = W("small_btn_green");
            lib.buttonOff = W("small_btn_red");
            lib.goal = W("goal");
            lib.expert = W("expert");
            lib.congrats = RoryAssets.Load<ParticleConfig>(RoryPaths.Effects + "/MainMenuScreen_Congrats.asset");
            EditorUtility.SetDirty(lib);
            AssetDatabase.SaveAssets();
        }

        static void HidePicture(WindowPopup w) => w.picture.gameObject.SetActive(false);

        // ------------------------------------------------------------------ PAUSE_GAME

        static PausePopup BuildPause()
        {
            var w = WindowBuilder.BuildWindow<PausePopup>(null, false, "GamePause");
            HidePicture(w);
            w.title.text = "Paused";
            w.info.text = "Goal : serve 5 customer(s)\nExpert : earn $220";
            WindowBuilder.Move(w.info, 0, 130);
            WindowBuilder.Label(w.leftButton, "Kitchen");
            WindowBuilder.Label(w.rightButton, "Resume");
            WindowBuilder.Move(w.leftButton, -150, 200);
            WindowBuilder.Move(w.rightButton, 140, 200);
            w.uniButton.gameObject.SetActive(false);

            // TextField(150|250, 50) at (x, y) top-left; TNonStateButton without alignPivot: (x, y) is its top-left.
            var t = w.transform;
            Toggle(t, "Sound", "SOUND :", 150, -200, -100, 0, -110, out w.soundButton, out w.soundLabel);
            Toggle(t, "Music", "MUSIC : ", 150, -200, -40, 0, -40, out w.musicButton, out w.musicLabel);
            Toggle(t, "FullScreen", "FULL SCREEN : ", 250, -240, 30, 0, 30, out w.fullScreenButton, out w.fullScreenLabel);
            return w;
        }

        static void Toggle(Transform parent, string name, string text, float textWidth, float textX, float textY,
            float buttonX, float buttonY, out Button button, out TextMeshProUGUI label)
        {
            UIBuild.Text(parent, name + "Text", text, UIBuild.Fonts.Bromine, 25, Color.black, textX, textY, textWidth, 50);
            button = UIBuild.Button(parent, name + "Button", W("small_btn_green"), buttonX, buttonY, topLeft: true,
                label: "ON", font: UIBuild.Fonts.Bromine, fontSize: 20, labelColor: Color.black);
            label = button.GetComponentInChildren<TextMeshProUGUI>();
        }

        // ------------------------------------------------------------------ CONFIRM_KITCHEN / CONFIRM_MAINMENU

        static WindowPopup BuildConfirm()
        {
            var w = WindowBuilder.BuildWindow<WindowPopup>(null, true, "GameConfirm");
            HidePicture(w);
            w.title.text = "Go to kitchen?";
            w.info.text = "Are you sure want to exit to the Kitchen? Your progress in this level will not be saved.";
            WindowBuilder.Move(w.info, 13, -15);
            WindowBuilder.Label(w.leftButton, "No");
            WindowBuilder.Label(w.rightButton, "Yes");
            w.uniButton.gameObject.SetActive(false);
            return w;
        }

        // ------------------------------------------------------------------ SHOW_EXPERT_NEEDED_INFO

        static WindowPopup BuildInfo()
        {
            var w = WindowBuilder.BuildWindow<WindowPopup>(null, true, "GameInfo");
            HidePicture(w);
            w.title.text = "Locked!";
            w.info.text = "To unlock this recipe, you need to collect 5 expert badges.\nCurrent Expert Badges: 0";
            WindowBuilder.Move(w.info, 13, 10);
            w.leftButton.gameObject.SetActive(false);
            w.rightButton.gameObject.SetActive(false);
            WindowBuilder.Label(w.uniButton, "OK");
            return w;
        }

        // ------------------------------------------------------------------ STORY_* / HO_* / DESSERT_* summaries

        static ResultPopup BuildResult()
        {
            var w = WindowBuilder.BuildWindow<ResultPopup>(null, false, "GameResult");
            HidePicture(w);
            w.title.text = "Congratulations!";
            w.info.text = "You've reached today's goal!";
            WindowBuilder.Move(w.info, 0, -170);
            w.leftButton.gameObject.SetActive(false);
            w.rightButton.gameObject.SetActive(false);
            WindowBuilder.Label(w.uniButton, "Kitchen");

            var t = w.transform;
            var bromine = UIBuild.Fonts.Bromine;
            // widgetBG.width / 2 = 323 (int), 350 high, pivot at the centre; summaryText at x - 100, summaryStats at x + 80.
            w.summaryText = UIBuild.Text(t, "SummaryText", "Customers Served\t\t :\nBeverage Served\t\t :\nFrench Fries Served\t :\n\nToday's Earnings\t\t :\nTotal Coins\t\t\t :",
                bromine, 20, Color.black, -100, 0, 323, 350, 161.5f, 175, TextAlignmentOptions.Left);
            w.summaryStats = UIBuild.Text(t, "SummaryStats", "0\n-\n-\n\n$0\n0 (+0)",
                bromine, 20, Color.black, 80, 0, 323, 350, 161.5f, 175, TextAlignmentOptions.Right);
            // widgetBG.width / 2 + 40 = 363 (int) x 100 at y + 125
            w.summaryTips = UIBuild.Text(t, "SummaryTips", "Well done! Try to reach Expert target to earn even more coins!",
                bromine, 20, Color.black, 0, 125, 363, 100, 181.5f, 50);
            // goalIcon at stage (780, 500), expertBlink 10 px lower, both centred
            w.goalIcon = UIBuild.ImageCentered(t, "GoalIcon", W("goal"), 780 - 512, 500 - 300);
            w.goalIcon.gameObject.SetActive(false);
            w.expertBlink = UIBuild.ImageCentered(t, "ExpertBlink", W("expert_blink"), 780 - 512, 510 - 300);
            w.expertBlink.gameObject.SetActive(false);
            // psCongrats: emitter at stage (512, 0), drawn above the windows
            w.congrats = UIBuild.Particles(t, "Congrats", RoryAssets.Load<ParticleConfig>(RoryPaths.Effects + "/MainMenuScreen_Congrats.asset"), 0, -300);
            // Stage-sized rect with the pivot (= emitter) at its top centre: a zero-size graphic on the stage edge would be
            // culled by the stage's RectMask2D.
            StageLayout.Place(w.congrats.rectTransform, 0, -300, UIStage.Width, UIStage.Height, UIStage.Width / 2, 0);
            return w;
        }

        // ------------------------------------------------------------------ LevelEvent.xml windows

        static DayEventPopup BuildEvent(bool small)
        {
            var w = WindowBuilder.BuildWindow<DayEventPopup>(null, small, small ? "GameEventSmall" : "GameEventBig");
            HidePicture(w);
            w.title.text = small ? "Title" : "New Customer!";
            w.info.text = "";
            WindowBuilder.Label(w.uniButton, "Continue");
            w.leftButton.gameObject.SetActive(false);
            w.rightButton.gameObject.SetActive(false);
            var t = w.transform;
            // unlockedItem: centred at (512, 270)
            w.unlockedItem = UIBuild.ImageCentered(t, "UnlockedItem", UIBuild.Sprite("game", "char_7"), 0, -30);
            w.unlockedItem.gameObject.SetActive(false);
            w.dishPreviews = new Image[DayEventPopup.MaxDishPreviews];
            for (int i = 0; i < w.dishPreviews.Length; i++)
            {
                var sprite = UIBuild.Sprite("game", "veggie_croissant");
                w.dishPreviews[i] = UIBuild.Image(t, "Dish" + i, sprite, 0, 0, 0, sprite.LogicalSize().y);
                w.dishPreviews[i].gameObject.SetActive(false);
            }
            return w;
        }

        // ------------------------------------------------------------------ showGoalIconSmall badge

        static GoalIconSmall BuildGoalIconSmall()
        {
            var sprite = W("goal");
            var img = UIBuild.Image(null, "GoalIconSmall", sprite, 0, 0, sprite.LogicalSize().x / 2, sprite.LogicalSize().y / 2);
            var icon = img.gameObject.AddComponent<GoalIconSmall>();
            icon.icon = img;
            return icon;
        }
    }
}
