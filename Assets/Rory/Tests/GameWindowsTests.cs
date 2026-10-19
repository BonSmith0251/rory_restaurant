using System.Collections;
using System.Linq;
using NUnit.Framework;
using Rory.Popups;
using Rory.Popups.Game;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

namespace Rory.Tests
{
    /// <summary>The shared in-game windows (GameWindows), opened over the main menu scene.</summary>
    public class GameWindowsTests
    {
        static PopupManager Popups => PopupManager.Current;
        static Save.PlayerProfile Player => GameManager.Player;
        static GameSession Session => GameManager.Instance.Session;

        static IEnumerator Setup()
        {
            yield return TestUtil.LoadScene(GameScene.MainMenu);
            yield return null;
            GameManager.Instance.Saves.CreatePlayer(0, "Tester");
            GameManager.Instance.Saves.Select(0);
            Session.extrasMode = false;
            Session.isPlayingSpecialLevel = false;
            GameManager.Instance.Audio.MuteSound = false;
            GameManager.Instance.Audio.MuteMusic = false;
            GameManager.Instance.Saves.Settings.fullScreen = true;
        }

        static string Label(Component popup, string button) =>
            popup.GetComponentsInChildren<RectTransform>(true).First(t => t.name == button).GetComponentInChildren<TextMeshProUGUI>(true).text;

        static bool Active(Component popup, string child) =>
            popup.GetComponentsInChildren<RectTransform>(true).First(t => t.name == child).gameObject.activeInHierarchy;

        [UnityTest]
        public IEnumerator PauseAndConfirm()
        {
            yield return Setup();
            bool resumed = false, muteChanged = false;
            var pause = GameWindows.ShowPause(new PauseOptions
            {
                onResume = () => resumed = true,
                onMuteChanged = () => muteChanged = true,
                resetGoalOnExit = true,
                info = "Goal : serve 5 customer(s)\nExpert : earn $220",
            });
            yield return null;
            Assert.IsNotNull(pause);
            Assert.AreEqual("Paused", pause.title.text);
            Assert.AreEqual("Kitchen", Label(pause, "LeftButton"));
            Assert.AreEqual("Resume", Label(pause, "RightButton"));
            Assert.AreEqual("ON", pause.soundLabel.text);
            Assert.IsTrue(pause.info.gameObject.activeSelf);
            TestUtil.Capture("Window_Pause");

            TestUtil.Click("SoundButton");
            Assert.IsTrue(GameManager.Instance.Audio.MuteSound);
            Assert.AreEqual("OFF", pause.soundLabel.text);
            Assert.IsTrue(muteChanged);
            TestUtil.Click("MusicButton");
            TestUtil.Click("FullScreenButton");
            Assert.AreEqual("OFF", pause.musicLabel.text);
            Assert.AreEqual("OFF", pause.fullScreenLabel.text);
            yield return null;
            TestUtil.Capture("Window_Pause_Off");
            TestUtil.Click("SoundButton");
            TestUtil.Click("MusicButton");
            TestUtil.Click("FullScreenButton");
            Assert.IsFalse(GameManager.Instance.Audio.MuteSound);
            Assert.AreEqual("ON", pause.fullScreenLabel.text);

            // Kitchen -> "Go to kitchen?" over the pause; No returns to the pause
            TestUtil.Click("LeftButton");
            yield return null;
            Assert.AreEqual(2, Popups.Open.Count);
            var confirm = (WindowPopup)Popups.Open[1];
            Assert.AreEqual("Go to kitchen?", confirm.title.text);
            Assert.AreEqual("No", Label(confirm, "LeftButton"));
            Assert.AreEqual("Yes", Label(confirm, "RightButton"));
            TestUtil.Capture("Window_ConfirmKitchen");
            TestUtil.Click("LeftButton");
            yield return null;
            Assert.AreEqual(1, Popups.Open.Count);
            Assert.AreSame(pause, Popups.Open[0]);

            TestUtil.Click("RightButton"); // Resume
            yield return null;
            Assert.IsTrue(resumed);
            Assert.IsFalse(Popups.HasOpenPopup);

            // extras: "Main Menu", no info text
            Session.extrasMode = true;
            pause = GameWindows.ShowPause(new PauseOptions());
            yield return null;
            Assert.AreEqual("Main Menu", Label(pause, "LeftButton"));
            Assert.IsFalse(pause.info.gameObject.activeSelf);
            TestUtil.Capture("Window_Pause_Extras");
            pause.Close();
            Session.extrasMode = false;

            var menu = GameWindows.ShowConfirmMainMenu(false);
            yield return null;
            Assert.AreEqual("Main Menu?", menu.title.text);
            StringAssert.StartsWith("Are you sure want to exit to the Main Menu?", menu.info.text);
            TestUtil.Capture("Window_ConfirmMainMenu");
            menu.Close();
        }

        [UnityTest]
        public IEnumerator ConfirmYesResetsUnfinishedGoal()
        {
            yield return Setup();
            Player.passedDays = 2;
            Session.currentPlayedLevel = 2; // 0-based: the third day, first play (== passedDays)
            Player.goalAchieved[2] = true;
            // (Kitchen has no scene yet: test the main menu variant, same reset logic)
            GameWindows.ShowConfirmMainMenu(true);
            yield return null;
            TestUtil.Click("RightButton"); // Yes
            Assert.IsFalse(Player.goalAchieved[2]);
            Assert.IsTrue(GameManager.Instance.Flow.IsTransitioning);
            float timeout = 10;
            while (GameManager.Instance.Flow.IsTransitioning && timeout > 0)
            {
                timeout -= Time.unscaledDeltaTime;
                yield return null;
            }
            Assert.AreEqual(GameScene.MainMenu, GameManager.Instance.Flow.Current);
        }

        [UnityTest]
        public IEnumerator StoryResults()
        {
            yield return Setup();
            Player.money = 12;
            Player.hardwareLevel[GameRules.Juicer] = 1;
            var r = new StoryDayResult
            {
                servedCustomers = 7, juiceServed = 2, friesServed = 1, earning = 240, expertTarget = 220, coinsGot = 6,
            };
            var w = GameWindows.ShowStoryLevelComplete(r);
            Assert.AreEqual("Congratulations!", w.title.text);
            Assert.AreEqual("You've reached today's goal!", w.info.text);
            Assert.AreEqual("7\n2\n-\n\n$240\n12 (+6)", w.summaryStats.text);
            Assert.AreEqual("Awesome! Couldn't have done it better myself!", w.summaryTips.text);
            Assert.AreEqual("Kitchen", Label(w, "UniButton"));
            Assert.IsTrue(w.Expert);
            Assert.IsTrue(w.goalIcon.gameObject.activeSelf);
            Assert.IsTrue(w.congrats.IsEmitting);
            yield return TestUtil.Seconds(0.3f);
            TestUtil.Capture("Window_StoryLevelComplete_Tween");
            yield return TestUtil.Seconds(1.2f);
            Assert.IsTrue(w.expertBlink.gameObject.activeSelf);
            TestUtil.Capture("Window_StoryLevelComplete");
            w.Close();

            r.specialLevel = true;
            r.vipBonus = 35;
            r.earning = 100;
            w = GameWindows.ShowStoryLevelComplete(r);
            Assert.IsFalse(w.Expert);
            StringAssert.Contains("VIP Rating Bonus", w.Labels);
            Assert.AreEqual("7\n2\n-\n35\n\n$100\n12 (+6)", w.summaryStats.text);
            Assert.AreEqual("Well done! Try to reach Expert target to earn even more coins!", w.summaryTips.text);
            yield return TestUtil.Seconds(1f);
            TestUtil.Capture("Window_StoryLevelComplete_VIP");
            w.Close();

            r = new StoryDayResult { servedCustomers = 0, coinsGot = 0 };
            Player.hardwareLevel[GameRules.Juicer] = 0;
            w = GameWindows.ShowStoryLevelFailed(r);
            Assert.AreEqual("Level Failed!", w.title.text);
            Assert.AreEqual("0\n-\n-\n\n12 (+0)", w.summaryStats.text);
            Assert.IsFalse(w.goalIcon.gameObject.activeSelf);
            Assert.IsFalse(w.congrats.IsEmitting);
            yield return null;
            TestUtil.Capture("Window_StoryLevelFailed");
            w.Close();

            w = GameWindows.ShowStoryCompleteAll(new StoryDayResult { servedCustomers = 9, earning = 900, expertTarget = 1000, coinsGot = 3 });
            Assert.AreEqual("Huraay!!", Label(w, "UniButton"));
            Assert.AreEqual("You've reached today's goal and finished all levels!", w.info.text);
            Assert.IsFalse(w.summaryTips.gameObject.activeSelf);
            yield return TestUtil.Seconds(1f);
            TestUtil.Capture("Window_StoryCompleteAll");
            w.Close();
        }

        [UnityTest]
        public IEnumerator HiddenObjectAndDessertResults()
        {
            yield return Setup();
            Player.money = 6;
            var ho = new HiddenObjectResult { itemsFound = 30, earning = 750, itemsLeft = 0, coinsGot = 6 };
            var w = GameWindows.ShowHOLevelComplete(ho);
            Assert.AreEqual("Summary", w.title.text);
            Assert.AreEqual("Objects Found\t\t:\nToday's Earning\t:\n\nTotal Coins\t\t:", w.Labels);
            Assert.AreEqual("30\n$750\n\n6 (+6)", w.summaryStats.text);
            Assert.IsTrue(w.Expert);
            yield return TestUtil.Seconds(1.5f);
            TestUtil.Capture("Window_HOLevelComplete");
            w.Close();

            w = GameWindows.ShowHOLevelFailed(new HiddenObjectResult { itemsFound = 9, earning = 225 });
            Assert.AreEqual("Level Failed", w.title.text);
            Assert.IsTrue(w.congrats.IsEmitting); // (sic) the original plays the particles here too
            yield return null;
            TestUtil.Capture("Window_HOLevelFailed");
            w.Close();

            bool next = false;
            w = GameWindows.ShowHOExtrasComplete(new HiddenObjectResult { itemsFound = 30, earning = 900 }, () => next = true);
            Assert.AreEqual("Next Level", Label(w, "UniButton"));
            Assert.AreEqual("30\n900", w.summaryStats.text);
            yield return null;
            TestUtil.Capture("Window_HOExtrasComplete");
            TestUtil.Click("UniButton");
            Assert.IsTrue(next);
            w.Close();

            w = GameWindows.ShowHOExtrasFailed(new HiddenObjectResult { itemsFound = 12, earning = 300 });
            Assert.AreEqual("Main Menu", Label(w, "UniButton"));
            yield return null;
            TestUtil.Capture("Window_HOExtrasFailed");
            w.Close();
            w = GameWindows.ShowHOExtrasCompleteAll(new HiddenObjectResult { itemsFound = 30, earning = 1200 });
            Assert.AreEqual("You've completed all levels.", w.info.text);
            w.Close();

            var d = new DessertResult { froyoServed = 14, earning = 320, expertTarget = 300, coinsGot = 6 };
            w = GameWindows.ShowDessertLevelComplete(d);
            Assert.AreEqual("Level Complete!", w.title.text);
            Assert.AreEqual("14\n$320\n\n6 (+6)", w.summaryStats.text);
            Assert.IsTrue(w.Expert);
            yield return TestUtil.Seconds(1.5f);
            TestUtil.Capture("Window_DessertLevelComplete");
            w.Close();
            w = GameWindows.ShowDessertLevelFailed(d);
            Assert.AreEqual("Kitchen", Label(w, "UniButton"));
            yield return null;
            TestUtil.Capture("Window_DessertLevelFailed");
            w.Close();
            w = GameWindows.ShowDessertExtrasComplete(d);
            Assert.AreEqual("Froyo Served\t\t:", w.Labels);
            Assert.AreEqual("14", w.summaryStats.text);
            yield return null;
            TestUtil.Capture("Window_DessertExtrasComplete");
            w.Close();
            w = GameWindows.ShowDessertExtrasFailed(d);
            Assert.AreEqual("You Failed!", w.title.text);
            w.Close();
        }

        [UnityTest]
        public IEnumerator ExpertNeededInfo()
        {
            yield return Setup();
            Player.achievementCounter[Achievements.Achievements.InternationalChef] = 3;
            var w = GameWindows.ShowExpertNeededInfo(10);
            yield return null;
            Assert.AreEqual("Locked!", w.title.text);
            Assert.AreEqual("To unlock this recipe, you need to collect 10 expert badges.\nCurrent Expert Badges: 3", w.info.text);
            TestUtil.Capture("Window_ExpertNeeded");
            TestUtil.Click("UniButton");
            yield return null;
            Assert.IsFalse(Popups.HasOpenPopup);
        }

        [UnityTest]
        public IEnumerator BeginningWindowsChain()
        {
            yield return Setup();
            Player.passedDays = 0;
            bool done = false;
            var opts = new BeginningWindowOptions
            {
                startIndex = 0, goalTarget = 4, expertTarget = 210, todaysDishes = new[] { 0, 0 }, onAllClosed = () => done = true,
            };
            Assert.IsTrue(GameWindows.ShowBeginningWindows(0, opts));
            yield return null;
            var w = Popups.Find<DayEventPopup>();
            Assert.AreEqual("New Customer!", w.title.text);
            Assert.AreEqual("Continue", Label(w, "UniButton"));
            Assert.IsTrue(w.unlockedItem.gameObject.activeSelf);
            Assert.AreEqual("char_7", w.unlockedItem.sprite.name);
            Assert.AreEqual(0.7f, w.background.transform.localScale.y, 1e-4f);
            TestUtil.Capture("Window_DayEvent_NewCustomer");
            int lastCust = Player.lastCustIdx;
            for (int i = 0; i < 3; i++)
            {
                TestUtil.Click("UniButton");
                yield return null;
            }
            Assert.AreEqual(lastCust, Player.lastCustIdx, "no customer side effect on the first day");
            w = Popups.Find<DayEventPopup>();
            Assert.AreEqual(1, Popups.Open.Count);
            Assert.AreEqual("Today's Preparation", w.title.text);
            Assert.AreEqual("Goal : serve 4 customer(s)\nExpert : earn $210", w.info.text);
            Assert.AreEqual(1, w.dishPreviews.Count(d => d.gameObject.activeSelf));
            Assert.IsFalse(w.unlockedItem.gameObject.activeSelf);
            Assert.AreEqual(3, Session.currentNotifWindow);
            TestUtil.Capture("Window_DayEvent_TodaysPreparation");
            Assert.AreEqual(w.info.text, GameWindows.LastDayInfo);
            TestUtil.Click("UniButton");
            yield return null;
            Assert.IsTrue(done);
            Assert.IsFalse(Popups.HasOpenPopup);

            // Day 3 introduces customer 3: rotation restarts there
            Player.passedDays = 2;
            done = false;
            Session.isPlayingSpecialLevel = true;
            opts.todaysDishes = new[] { 0, 1, 1, 2 };
            opts.goalTarget = 6;
            opts.expertTarget = 310;
            GameWindows.ShowBeginningWindows(2, opts);
            yield return null;
            Assert.AreEqual(3, Player.lastCustIdx);
            CollectionAssert.AreEqual(new[] { 7, 1, 4, 3 }, Session.tempCustUse);
            TestUtil.Click("UniButton");
            yield return null;
            w = Popups.Find<DayEventPopup>();
            Assert.AreEqual("Goal : serve 6 food critic\nExpert : earn $310", w.info.text);
            Assert.AreEqual(3, w.dishPreviews.Count(d => d.gameObject.activeSelf));
            TestUtil.Capture("Window_DayEvent_ThreeDishes");
            TestUtil.Click("UniButton");
            Assert.IsTrue(done);

            // HO day (index 3): "Hidden Object" window, no dish previews
            Assert.IsTrue(GameWindows.ShowBeginningWindows(3, new BeginningWindowOptions()));
            yield return null;
            w = Popups.Find<DayEventPopup>();
            Assert.AreEqual("Hidden Object", w.title.text);
            TestUtil.Capture("Window_DayEvent_HiddenObject");
            w.Close();
            Assert.IsFalse(GameWindows.ShowBeginningWindows(3, new BeginningWindowOptions { startIndex = 5 }));
        }

        [UnityTest]
        public IEnumerator GoalIconSmallPopsAndHides()
        {
            yield return Setup();
            var icon = GameWindows.ShowGoalIconSmall(true, GameWindows.GoalIconSmallMatch3);
            Assert.IsNotNull(icon);
            Assert.AreEqual("expert", icon.icon.sprite.name);
            yield return TestUtil.Seconds(0.9f);
            Assert.AreEqual(0.35f, icon.transform.localScale.x, 0.01f);
            TestUtil.Capture("Window_GoalIconSmall");
            yield return TestUtil.Seconds(1.3f);
            Assert.IsTrue(icon == null, "badge should be gone after 1 s");
        }
    }
}
