using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rory.Popups;
using Rory.Popups.Game;
using Rory.Save;
using Rory.Screens.Dessert;
using Rory.Tutorials;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using AchievementList = global::Rory.Achievements.Achievements;

namespace Rory.Tests
{
    /// <summary>
    /// Dessert making (frozen yogurt) screen: story days and the extras game. Expected positions are the original's
    /// (harness tree of the legacy port, day 38 / day 10 / extras).
    /// </summary>
    public class DessertTests
    {
        static GameSession Session => GameManager.Instance.Session;
        static PopupManager Popups => PopupManager.Current;
        static string saveFolder;

        DessertController c;

        [TearDown]
        public void TearDown() => Time.timeScale = 1;

        /// <summary>Fresh saves with player "Tester" at story day <paramref name="day"/> (0-based), then the scene.</summary>
        IEnumerator Load(int day, bool extras = false, System.Action<PlayerProfile> setup = null)
        {
            while (GameManager.Instance.Flow.IsTransitioning) yield return null;
            Time.timeScale = 1;
            saveFolder = TestUtil.TempSaveFolder();
            GameManager.Instance.UseSaveFolder(saveFolder);
            var p = GameManager.Instance.Saves.CreatePlayer(0, "Tester");
            p.passedDays = day;
            setup?.Invoke(p);
            Session.SelectDay(day);
            Session.extrasMode = extras;
            Session.coinsGot = 0;
            GameManager.Instance.Audio.MuteSound = false;
            GameManager.Instance.Audio.MuteMusic = false;
            yield return TestUtil.LoadScene(GameScene.Dessert, false);
            c = Object.FindFirstObjectByType<DessertController>();
            Assert.IsNotNull(c, "No DessertController");
        }

        static IEnumerator WaitUntil(System.Func<bool> condition, float timeout = 10, string what = "condition")
        {
            float end = Time.realtimeSinceStartup + timeout;
            while (!condition() && Time.realtimeSinceStartup < end) yield return null;
            Assert.IsTrue(condition(), "Timed out waiting for " + what);
        }

        static RectTransform Rt(string name) => TestUtil.Find<RectTransform>(name);

        /// <summary>Top-left of an object's rect in stage pixels (like the harness "bounds").</summary>
        static Vector2 TopLeft(string name)
        {
            var rt = Rt(name);
            var corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            var stage = UI.UIStage.Current;
            var local = stage.Rect.InverseTransformPoint(corners[1]); // top-left
            var r = stage.Rect.rect;
            return new Vector2(local.x - r.xMin, r.yMax - local.y);
        }

        static void AssertNear(Vector2 expected, Vector2 actual, string what, float tolerance = 1)
        {
            Assert.AreEqual(expected.x, actual.x, tolerance, what + " x");
            Assert.AreEqual(expected.y, actual.y, tolerance, what + " y");
        }

        /// <summary>Presses ingredient station <paramref name="id"/> like a touch (pointer down on its quad).</summary>
        internal static void Press(int id)
        {
            var tile = TestUtil.Find("Tile" + id);
            Assert.IsNotNull(tile, "Tile" + id);
            TestUtil.Press(tile, new Vector2(132 + id % 7 * 126, 426 + id / 7 * 115));
        }

        int Worth(IEnumerable<int> order) => order.Sum(id => c.ItemWorth(id));

        IEnumerator StartDay()
        {
            yield return null;
            Assert.IsTrue(Popups.HasOpenPopup, "Beginning window");
            Assert.IsTrue(c.HasWindow);
            TestUtil.Click("UniButton");
            yield return WaitUntil(() => c.OrderInPlace, 10, "first order");
        }

        IEnumerator ServeCurrentOrder()
        {
            yield return WaitUntil(() => c.OrderInPlace, 30, "an order");
            foreach (var id in c.OrderList.ToList()) Press(id);
        }

        // ------------------------------------------------------------------ layout

        [UnityTest]
        public IEnumerator StoryDayLayoutHudAndPause()
        {
            yield return Load(38);
            var window = (WindowPopup)Popups.Open[0];
            Assert.AreEqual("Making Yogurt", window.title.text);
            Assert.AreEqual(3, c.Level, "day 39 = level (38 + 1) / 14 + 1");
            Assert.AreEqual(80, c.GoalTarget);
            Assert.AreEqual(110, c.ExpertTarget);
            Assert.AreEqual(60 * 30, c.TimeLeft, "time in original frames");
            Assert.AreEqual("$ 80", TestUtil.Find<TextMeshProUGUI>("GoalValueText").text);
            Assert.AreEqual("Goal:", TestUtil.Find<TextMeshProUGUI>("GoalLabelText").text);
            Assert.IsFalse(TestUtil.Find("LivesText") != null, "no lives in story mode");
            Assert.AreEqual("sunny-bliss", GameManager.Instance.Audio.CurrentMusic);
            TestUtil.Capture("Dessert_Window");

            // the HUD (harness bounds of the original)
            AssertNear(new Vector2(14, 14), TopLeft("PauseButton"), "pause");
            AssertNear(new Vector2(57, 14), TopLeft("SoundButton"), "sound");
            AssertNear(new Vector2(100, 14), TopLeft("MusicButton"), "music");
            AssertNear(new Vector2(730, 0), TopLeft("ScoreBoard"), "score board");
            AssertNear(new Vector2(273, 4), TopLeft("TimerFill"), "timer");
            AssertNear(new Vector2(727, 50), TopLeft("PreviewPaper"), "paper");
            AssertNear(new Vector2(-250, 89), TopLeft("Customer"), "customer out");
            AssertNear(new Vector2(-1266, 245), TopLeft("Pan"), "pan out");
            AssertNear(new Vector2(82, 376), TopLeft("Tile0"), "tile 0");
            AssertNear(new Vector2(838, 491), TopLeft("Tile13"), "tile 13");

            yield return StartDay();
            Assert.IsFalse(c.HasWindow);
            AssertNear(new Vector2(-20, 89), TopLeft("Customer"), "customer in");
            AssertNear(new Vector2(0, 240), TopLeft("CustomerBarBG"), "bar");
            AssertNear(new Vector2(5, 245), TopLeft("Heart"), "heart");
            AssertNear(new Vector2(47, 255), TopLeft("CustomerFill"), "patience");
            AssertNear(new Vector2(184, 245), TopLeft("Pan"), "pan in");
            int shown = 0;
            for (int i = 0; i < DessertController.PlateCount; i++)
                if (TestUtil.Find("Preview" + i) != null && Rt("Preview" + i).anchoredPosition.x > 700) shown++;
            Assert.AreEqual(c.OrderList.Count, shown, "previews on the paper");
            yield return TestUtil.Seconds(0.6f);
            TestUtil.Capture("Dessert_Stations");

            // sound / music toggles show the mute icons
            Assert.IsNull(TestUtil.Find("SoundMuteIcon"));
            TestUtil.Click("SoundButton");
            Assert.IsTrue(GameManager.Instance.Audio.MuteSound);
            AssertNear(new Vector2(74, 26), TopLeft("SoundMuteIcon"), "mute icon");
            TestUtil.Click("SoundButton");
            TestUtil.Click("MusicButton");
            Assert.IsNotNull(TestUtil.Find("MusicMuteIcon"));
            AssertNear(new Vector2(117, 26), TopLeft("MusicMuteIcon"), "music mute icon");
            TestUtil.Click("MusicButton");
            Assert.IsNull(TestUtil.Find("MusicMuteIcon"));

            // pause: the timer stops, the info is the day window's text, Resume continues
            TestUtil.Click("PauseButton");
            yield return null;
            Assert.IsTrue(c.Paused);
            var pause = Popups.Open.OfType<PausePopup>().FirstOrDefault();
            Assert.IsNotNull(pause, "pause window");
            Assert.AreEqual("Let's make some froyo!!", pause.info.text);
            float t = c.TimeLeft;
            yield return TestUtil.Seconds(0.3f);
            Assert.AreEqual(t, c.TimeLeft, "timer stopped while paused");
            TestUtil.Capture("Dessert_Pause");
            // the pause window's own toggle refreshes the HUD icon
            TestUtil.Click("SoundButton");
            Assert.IsTrue(TestUtil.Find("SoundMuteIcon") != null);
            TestUtil.Click("SoundButton");
            TestUtil.Click("RightButton");
            yield return null;
            Assert.IsFalse(c.Paused);
            Assert.IsFalse(Popups.HasOpenPopup);
            yield return TestUtil.Seconds(0.2f);
            Assert.Less(c.TimeLeft, t, "timer runs again");
        }

        // ------------------------------------------------------------------ serving

        [UnityTest]
        public IEnumerator CorrectDessertPaysItsWorth()
        {
            yield return Load(38);
            yield return StartDay();
            var order = c.OrderList.ToList();
            int worth = Worth(order);
            Assert.AreEqual(worth, c.Price);
            for (int i = 0; i < order.Count - 1; i++)
            {
                Press(order[i]);
                Assert.AreEqual(i + 1, c.CurrentLayer);
                Assert.IsTrue(TestUtil.Find("PlateItem" + order[i]) != null, "ingredient on the pan");
            }
            yield return TestUtil.Seconds(0.4f);
            TestUtil.Capture("Dessert_InProgress");

            Press(order[order.Count - 1]);
            Assert.AreEqual(worth, c.Earning);
            Assert.AreEqual(1, c.FroyoMade);
            Assert.AreEqual("$ " + worth, TestUtil.Find<TextMeshProUGUI>("ScoreText").text);
            Assert.IsFalse(c.OrderInPlace);
            for (int i = 0; i < DessertController.PlateCount; i++)
                Assert.IsNull(TestUtil.Find("Preview" + i), "previews hidden after serving");
            Assert.AreEqual(1, GameManager.Player.achievementCounter[AchievementList.FroyoMaster], "froyo master +1");
            yield return TestUtil.Seconds(0.2f);
            TestUtil.Capture("Dessert_Served");

            // 0.4 s later the customer leaves (1 s), the next one is in 1 s after that: then the next order
            yield return WaitUntil(() => c.OrderInPlace, 5, "next order");
            Assert.AreEqual(0, c.CurrentLayer);
            Assert.AreEqual(-20, Rt("Customer").anchoredPosition.x, 0.5f, "next customer in");
            Assert.AreEqual(1, c.FroyoMade);
        }

        [UnityTest]
        public IEnumerator WrongIngredientThrowsTheDessertAway()
        {
            yield return Load(38);
            yield return StartDay();
            var order = c.OrderList.ToList();
            Press(order[0]);
            Assert.AreEqual(1, c.CurrentLayer);
            int wrong = Enumerable.Range(0, DessertController.TileCount).First(id => order.Count < 2 || id != order[1]);
            Press(wrong);
            // back to the first layer, the pan is emptied, the customer keeps waiting
            Assert.AreEqual(0, c.CurrentLayer);
            for (int i = 0; i < DessertController.PlateCount; i++)
                Assert.IsNull(TestUtil.Find("PlateItem" + i), "pan emptied");
            Assert.IsTrue(c.OrderInPlace);
            Assert.AreEqual(0, c.Earning);
            Assert.AreEqual(0, c.FroyoMade);
            // a wrong first ingredient does the same
            Press(order[0] == 13 ? 12 : 13);
            Assert.AreEqual(0, c.CurrentLayer);
            // the right order is still paid in full
            foreach (var id in order) Press(id);
            Assert.AreEqual(Worth(order), c.Earning);
            Assert.AreEqual(1, c.FroyoMade);
        }

        // ------------------------------------------------------------------ end of the day

        [UnityTest]
        public IEnumerator TimeUpWithoutGoalFailsTheDay()
        {
            yield return Load(38);
            yield return StartDay();
            Time.timeScale = 20;
            yield return WaitUntil(() => Popups.Open.OfType<ResultPopup>().Any(), 30, "result window");
            Time.timeScale = 1;
            var w = Popups.Open.OfType<ResultPopup>().First();
            Assert.AreEqual("Level Failed!", w.title.text);
            Assert.IsTrue(c.TimesUp);
            Assert.AreEqual("0\n$0\n\n0 (+0)", w.summaryStats.text);
            var p = GameManager.Player;
            Assert.AreEqual(38, p.passedDays);
            Assert.AreEqual(0, p.daysEarning[38]);
            Assert.AreEqual(0, p.money);
            yield return TestUtil.Seconds(0.5f);
            TestUtil.Capture("Dessert_LevelFailed");
            // Kitchen
            TestUtil.Click("UniButton");
            yield return TestUtil.ExpectSceneRequest(GameScene.Kitchen);
        }

        [UnityTest]
        public IEnumerator TimeUpAfterExpertCompletesAndSaves()
        {
            yield return Load(38);
            yield return StartDay();
            var p = GameManager.Player;
            Time.timeScale = 5;
            while (c.Earning < c.ExpertTarget && !c.TimesUp)
            {
                bool wasGoal = c.Earning >= c.GoalTarget;
                yield return ServeCurrentOrder();
                if (!wasGoal && c.Earning >= c.GoalTarget)
                {
                    Assert.IsTrue(p.goalAchieved[38], "goal flag set when the goal is reached");
                    Assert.AreEqual("EXPERT:", TestUtil.Find<TextMeshProUGUI>("GoalLabelText").text);
                    Assert.AreEqual("$ 110", TestUtil.Find<TextMeshProUGUI>("GoalValueText").text);
                    Assert.IsNotNull(TestUtil.Find("GoalIconSmall"), "goal badge in the HUD");
                }
            }
            Assert.GreaterOrEqual(c.Earning, c.ExpertTarget, "expert reached before the time ran out");
            int earning = c.Earning;
            int served = c.FroyoMade;
            Time.timeScale = 20;
            yield return WaitUntil(() => Popups.Open.OfType<ResultPopup>().Any(), 60, "result window");
            Time.timeScale = 1;
            var w = Popups.Open.OfType<ResultPopup>().First();
            Assert.AreEqual("Level Complete!", w.title.text);
            Assert.IsTrue(w.Expert);
            // the customer waiting when the time ran out could still be served: read the final values
            earning = c.Earning;
            served = c.FroyoMade;
            Assert.AreEqual($"{served}\n${earning}\n\n6 (+6)", w.summaryStats.text);
            Assert.AreEqual(39, p.passedDays);
            Assert.AreEqual(earning, p.daysEarning[38]);
            Assert.AreEqual(6, p.money);
            Assert.AreEqual(6, Session.coinsGot);
            Assert.AreEqual(1, p.achievementCounter[AchievementList.QualifiedChef]);
            Assert.AreEqual(1, p.achievementCounter[AchievementList.InternationalChef]);
            Assert.AreEqual(earning, p.achievementCounter[AchievementList.BigBusiness]);
            Assert.AreEqual(served, p.achievementCounter[AchievementList.FroyoMaster]);

            var saved = new SaveSystem(saveFolder);
            saved.LoadAll();
            var s = saved.GetSlot(0);
            Assert.AreEqual(39, s.passedDays, "saved");
            Assert.AreEqual(earning, s.daysEarning[38]);
            Assert.AreEqual(6, s.money);
            Assert.IsTrue(s.goalAchieved[38]);
            yield return TestUtil.Seconds(1f);
            TestUtil.Capture("Dessert_LevelComplete");
        }

        [UnityTest]
        public IEnumerator ReplayBelowBestKeepsTheRecord()
        {
            // day already played with $100 (goal 80, expert 110): a lower result still "completes" without rewards
            yield return Load(38, false, pr =>
            {
                pr.passedDays = 40;
                pr.daysEarning[38] = 100;
                pr.goalAchieved[38] = true;
                pr.money = 50;
            });
            Assert.AreEqual("EXPERT:", TestUtil.Find<TextMeshProUGUI>("GoalLabelText").text, "goal already achieved");
            Assert.AreEqual("$ 110", TestUtil.Find<TextMeshProUGUI>("GoalValueText").text);
            yield return StartDay();
            Time.timeScale = 5;
            while (c.Earning < c.GoalTarget && !c.TimesUp) yield return ServeCurrentOrder();
            Assert.GreaterOrEqual(c.Earning, c.GoalTarget);
            Time.timeScale = 20;
            yield return WaitUntil(() => Popups.Open.OfType<ResultPopup>().Any(), 60, "result window");
            Time.timeScale = 1;
            var p = GameManager.Player;
            var w = Popups.Open.OfType<ResultPopup>().First();
            Assert.AreEqual("Level Complete!", w.title.text);
            Assert.AreEqual(40, p.passedDays, "a replay doesn't pass a day");
            Assert.AreEqual(System.Math.Max(100, c.Earning), p.daysEarning[38]);
            int expectedMoney = c.Earning >= 110 ? 53 : 50;
            Assert.AreEqual(expectedMoney, p.money, "3 coins only when expert is reached for the first time");
        }

        // ------------------------------------------------------------------ extras

        [UnityTest]
        public IEnumerator ExtrasLevelsAndLives()
        {
            yield return Load(5, true);
            yield return null;
            Assert.AreEqual(1, c.Level);
            Assert.AreEqual(3, c.Lives);
            Assert.AreEqual(100, c.GoalTarget);
            Assert.AreEqual(120 * 30, c.TimeLeft);
            Assert.IsFalse(Popups.HasOpenPopup, "no day window in extras");
            Assert.IsNotNull(TestUtil.Find("BlackScreen"), "level banner over the black screen");
            Assert.AreEqual("LEVEL: 1", TestUtil.Find<TextMeshProUGUI>("BigInfoText").text);
            Assert.AreEqual("LIVES x3", TestUtil.Find<TextMeshProUGUI>("LivesText").text);
            AssertNear(new Vector2(300, 30), TopLeft("LivesText"), "lives text");
            yield return TestUtil.Seconds(0.9f);
            TestUtil.Capture("Dessert_ExtrasBanner");
            Assert.IsTrue(c.HasWindow, "the level starts after the banner");
            yield return WaitUntil(() => c.OrderInPlace, 6, "first order");
            Assert.IsNull(TestUtil.Find("BlackScreen"));
            Assert.AreEqual(0, GameManager.Player.achievementCounter[AchievementList.FroyoMaster]);

            // level 1 -> 2: earnings reset, new goal and time, banner again
            Time.timeScale = 5;
            while (c.Level == 1)
            {
                Assert.IsFalse(c.TimesUp, "level 1 goal reached in time");
                yield return ServeCurrentOrder();
            }
            Time.timeScale = 1;
            Assert.AreEqual(2, c.Level);
            Assert.AreEqual(0, c.Earning);
            Assert.AreEqual(120, c.GoalTarget);
            Assert.AreEqual(130 * 30, c.TimeLeft, 10);
            Assert.AreEqual("$ 120", TestUtil.Find<TextMeshProUGUI>("GoalValueText").text);
            Assert.AreEqual("LEVEL: 2", TestUtil.Find<TextMeshProUGUI>("BigInfoText").text);
            Assert.AreEqual(0, GameManager.Player.achievementCounter[AchievementList.FroyoMaster], "no froyo master in extras");

            // customers leave: a life each; the last one ends the game
            Time.timeScale = 20;
            yield return WaitUntil(() => c.Lives == 2, 30, "a life lost");
            Assert.AreEqual("LIVES x2", TestUtil.Find<TextMeshProUGUI>("LivesText").text);
            yield return WaitUntil(() => Popups.Open.OfType<ResultPopup>().Any(), 60, "result window");
            Time.timeScale = 1;
            Assert.AreEqual(0, c.Lives);
            var w = Popups.Open.OfType<ResultPopup>().First();
            Assert.AreEqual("You Failed!", w.title.text);
            Assert.AreEqual(c.FroyoMade.ToString(), w.summaryStats.text);
            yield return TestUtil.Seconds(1.5f);
            Assert.AreEqual(1, Popups.Open.OfType<ResultPopup>().Count(), "the failed window opens once");
            TestUtil.Capture("Dessert_ExtrasFailed");
            TestUtil.Click("UniButton");
            yield return TestUtil.ExpectSceneRequest(GameScene.MainMenu);
        }

        // ------------------------------------------------------------------ LEVEL_11 tutorial

        [UnityTest]
        public IEnumerator Level11TutorialGatesTheIngredients()
        {
            yield return Load(10);
            var tut = TutorialManager.Current;
            Assert.IsNotNull(tut);
            Assert.AreEqual(TutorialIds.LEVEL_11, tut.CurrentDialogueId);
            Assert.AreEqual(0, tut.DialogueCounter);
            Assert.IsTrue(Popups.HasOpenPopup, "day window over the bubble");
            Assert.AreEqual(1, c.Level);
            TestUtil.Click("UniButton");
            yield return null;
            Assert.IsTrue(c.HasWindow, "the bubble's Next button holds the level");
            for (int line = 0; line < 4; line++)
            {
                TestUtil.Click("NextButton");
                Assert.AreEqual(line + 1, tut.DialogueCounter);
            }
            // line 4 ("I have prepared everything", blockNext): Next starts the level and hides the bubble
            TestUtil.Click("NextButton");
            Assert.IsFalse(c.HasWindow);
            Assert.IsFalse(tut.IsShowing);
            Assert.AreEqual(4, tut.DialogueCounter);
            // check_times_up when the customer is in: line 5, and the tutorial order
            yield return WaitUntil(() => c.OrderInPlace, 5, "tutorial order");
            Assert.AreEqual(5, tut.DialogueCounter);
            CollectionAssert.AreEqual(new[] { 0, 1, 3, 4, 6 }, c.OrderList.ToArray());
            Assert.AreEqual(15, c.Price);
            // touches are ignored while the Next button shows
            Press(0);
            Assert.AreEqual(0, c.CurrentLayer);
            TestUtil.Click("NextButton");
            TestUtil.Click("NextButton");
            Assert.AreEqual(7, tut.DialogueCounter);
            Assert.IsFalse(tut.IsButtonShowing);
            float time = c.TimeLeft;

            int[] steps = { 0, 1, 3, 4 };
            for (int i = 0; i < steps.Length; i++)
            {
                // anything but the ingredient the bubble points at is ignored (no trash either)
                int other = steps[i] == 2 ? 5 : 2;
                Press(other);
                Assert.AreEqual(i, c.CurrentLayer, "blocked ingredient " + other);
                Assert.AreEqual(7 + i, tut.DialogueCounter);
                Press(steps[i]);
                Assert.AreEqual(i + 1, c.CurrentLayer);
                Assert.AreEqual(8 + i, tut.DialogueCounter);
            }
            yield return TestUtil.Seconds(0.5f);
            Assert.AreEqual(time, c.TimeLeft, "no timer during LEVEL_11");
            TestUtil.Capture("Dessert_Tutorial");
            // the beverage: served, the bubble hides until the next customer is in
            Press(6);
            Assert.AreEqual(15, c.Earning);
            Assert.IsFalse(tut.IsShowing);
            yield return WaitUntil(() => c.OrderInPlace, 5, "next customer");
            Assert.AreEqual(12, tut.DialogueCounter, "check_times_up shows line 12");
            Assert.IsTrue(tut.IsShowing);
            Assert.AreEqual("char_2", TestUtil.Find<Image>("Customer").sprite.name);
            CollectionAssert.AreEqual(new[] { 0, 1, 3, 4, 6 }, c.OrderList.ToArray());
            TestUtil.Click("NextButton");
            TestUtil.Click("NextButton");
            TestUtil.Click("NextButton");
            Assert.AreEqual(TutorialIds.None, tut.CurrentDialogueId);
            Assert.IsTrue(GameManager.Player.tutorialShowed[TutorialIds.LEVEL_11]);
            yield return TestUtil.Seconds(0.3f);
            Assert.Less(c.TimeLeft, time, "the timer starts after the tutorial");
        }
    }
}
