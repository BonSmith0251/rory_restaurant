using System.Collections;
using NUnit.Framework;
using Rory.Data;
using Rory.Save;
using Rory.Screens.Achievements;
using Rory.Screens.Ending;
using Rory.Screens.LevelSelect;
using Rory.Screens.Opening;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using AchievementList = global::Rory.Achievements.Achievements;
using Rory.UI;

namespace Rory.Tests
{
    /// <summary>Opening, Ending, Achievements and LevelSelect screens.</summary>
    public class StoryScreensTests
    {
        static GameSession Session => GameManager.Instance.Session;
        static string saveFolder;

        /// <summary>Fresh saves with a selected player "Tester" prepared by <paramref name="setup"/>, then the scene.</summary>
        static IEnumerator LoadWithPlayer(GameScene scene, System.Action<PlayerProfile> setup = null)
        {
            // a transition left over from an earlier test (e.g. the splash fade-in) would swallow our requests
            while (GameManager.Instance.Flow.IsTransitioning) yield return null;
            saveFolder = TestUtil.TempSaveFolder();
            GameManager.Instance.UseSaveFolder(saveFolder);
            var p = GameManager.Instance.Saves.CreatePlayer(0, "Tester");
            setup?.Invoke(p);
            yield return TestUtil.LoadScene(scene, false);
        }

        static IEnumerator WaitUntil(System.Func<bool> condition, float timeout = 5)
        {
            float end = Time.realtimeSinceStartup + timeout;
            while (!condition() && Time.realtimeSinceStartup < end) yield return null;
            Assert.IsTrue(condition(), "Timed out");
        }

        static bool Active(string name) => TestUtil.Find(name) != null;

        // ------------------------------------------------------------------ Opening

        [UnityTest]
        public IEnumerator OpeningSlidesEndInKitchen()
        {
            yield return LoadWithPlayer(GameScene.Opening);
            var c = Object.FindFirstObjectByType<OpeningController>();
            Assert.IsNotNull(c);
            Assert.AreEqual(0, c.CurrentStory);
            StringAssert.StartsWith("One early morning", TestUtil.Find<TextMeshProUGUI>("Narration").text);
            TestUtil.Capture("Opening_0");

            for (int story = 1; story <= 6; story++)
            {
                TestUtil.Click("TouchArea");
                Assert.AreEqual(story, c.CurrentStory);
                // taps while the slide animates are ignored
                TestUtil.Click("TouchArea");
                Assert.AreEqual(story, c.CurrentStory);
                yield return WaitUntil(() => c.CanProceed);
                if (story >= 5) yield return TestUtil.Seconds(0.4f); // bubble scale-in, then its text
                TestUtil.Capture("Opening_" + story);
                switch (story)
                {
                    case 1:
                        Assert.AreEqual(1024 - TestUtil.Find<UnityEngine.UI.Image>("Smile").sprite.LogicalSize().x, TestUtil.Find<RectTransform>("Smile").anchoredPosition.x, 0.01f);
                        break;
                    case 3:
                        Assert.IsFalse(Active("Smile"));
                        Assert.IsTrue(Active("StudentLoan"));
                        break;
                    case 4:
                        Assert.IsFalse(Active("StudentLoan"));
                        break;
                    case 5:
                        Assert.IsFalse(Active("Cry"));
                        Assert.AreEqual("HMM...", TestUtil.Find<TextMeshProUGUI>("BubbleText").text);
                        break;
                    case 6:
                        Assert.IsTrue(Active("LightBulb"));
                        Assert.AreEqual(new Vector2(450, -300), TestUtil.Find<RectTransform>("Bubble").anchoredPosition);
                        StringAssert.StartsWith("I’ll open", TestUtil.Find<TextMeshProUGUI>("BubbleText").text);
                        StringAssert.StartsWith("Genius idea!", TestUtil.Find<TextMeshProUGUI>("Narration").text);
                        break;
                }
            }

            Assert.IsFalse(GameManager.Player.openingViewed);
            TestUtil.Click("TouchArea");
            Assert.AreEqual(OpeningController.LastStory, c.CurrentStory);
            Assert.IsTrue(GameManager.Player.openingViewed);
            var reloaded = new SaveSystem(saveFolder);
            reloaded.LoadAll();
            Assert.IsTrue(reloaded.GetSlot(0).openingViewed, "openingViewed not saved");
            yield return TestUtil.ExpectSceneRequest(GameScene.Kitchen);
        }

        // ------------------------------------------------------------------ Ending

        [UnityTest]
        public IEnumerator EndingSlidesEndInKitchen()
        {
            yield return LoadWithPlayer(GameScene.Ending);
            var c = Object.FindFirstObjectByType<EndingController>();
            Assert.IsNotNull(c);
            Assert.IsTrue(GameManager.Player.endingViewed, "Ending marks endingViewed on entry");
            Session.coinsGot = 123;
            TestUtil.Capture("Ending_0");

            for (int story = 1; story <= 8; story++)
            {
                TestUtil.Click("TouchArea");
                Assert.AreEqual(story, c.CurrentStory);
                yield return WaitUntil(() => c.CanProceed);
                if (story == 2 || story == 3) yield return TestUtil.Seconds(0.4f);
                TestUtil.Capture("Ending_" + story);
                switch (story)
                {
                    case 1:
                        Assert.AreEqual("One night, Rory is counting her profit.", TestUtil.Find<TextMeshProUGUI>("Narration").text);
                        break;
                    case 2:
                        Assert.AreEqual("30..40..50..", TestUtil.Find<TextMeshProUGUI>("BubbleText").text);
                        break;
                    case 3:
                        Assert.AreEqual(1024 - 298 + 100, TestUtil.Find<RectTransform>("Bubble").anchoredPosition.x, 0.01f);
                        break;
                    case 6:
                        Assert.IsFalse(Active("Restaurant"));
                        break;
                    case 7:
                        Assert.IsFalse(Active("ToFrench"));
                        break;
                    case 8:
                        Assert.AreEqual(0, TestUtil.Find<RectTransform>("FinalSlide").anchoredPosition.y, 0.01f);
                        break;
                }
            }
            TestUtil.Click("TouchArea");
            Assert.AreEqual(EndingController.LastStory, c.CurrentStory);
            Assert.AreEqual(0, Session.coinsGot);
            yield return TestUtil.ExpectSceneRequest(GameScene.Kitchen);
        }

        // ------------------------------------------------------------------ Achievements

        static void AchievementProfile(PlayerProfile p)
        {
            p.unlocked[AchievementList.BigBusiness] = true;
            p.achievementCounter[AchievementList.BigBusiness] = 100000;
            p.achievementCounter[AchievementList.ServiceExcellence] = 1532;
            p.achievementCounter[AchievementList.HawkEye] = 150;
        }

        [UnityTest]
        public IEnumerator AchievementsShowProgressAndScroll()
        {
            yield return LoadWithPlayer(GameScene.Achievements, AchievementProfile);
            var c = Object.FindFirstObjectByType<AchievementsController>();
            Assert.IsNotNull(c);
            Assert.AreEqual("Service Excellence", c.Title.text);
            Assert.AreEqual("You've served 1,532/1000 perfectly happy customers\n(80% patience or above)!", c.Info.text);
            Assert.AreEqual("service_exellence_locked", TestUtil.Find<Image>("Achievement0").sprite.name);
            Assert.AreEqual("big_business", TestUtil.Find<Image>("Achievement1").sprite.name);
            Assert.AreEqual("middle_east_recipe_locked", TestUtil.Find<Image>("Achievement2").sprite.name);
            Assert.AreEqual(350, c.ColumnY, 0.01f);
            TestUtil.Capture("Achievements");

            // Tap on the bar at the second tenth: the knob jumps there, the column scrolls to Big Business
            var bar = TestUtil.Find("ScrollBar");
            float tenth = 429 / 10f;
            TestUtil.Press(bar, new Vector2(770, 120 + tenth * 1.5f));
            Assert.AreEqual(120 + tenth * 1.5f, -TestUtil.Find<RectTransform>("Scroller").anchoredPosition.y, 0.5f);
            Assert.IsFalse(Active("Title"), "Texts hide while scrolling");
            yield return TestUtil.Seconds(0.5f);
            Assert.AreEqual("Big Business", c.Title.text);
            Assert.AreEqual(AchievementList.Info[AchievementList.BigBusiness], c.Info.text);
            Assert.IsTrue(Active("Title") && Active("Info"));
            Assert.AreEqual(350 - 470, c.ColumnY, 0.01f);
            TestUtil.Capture("Achievements_BigBusiness");

            // Drag the knob down to Hawk Eye
            var knob = TestUtil.Find("Scroller");
            float y0 = -knob.GetComponent<RectTransform>().anchoredPosition.y;
            TestUtil.Press(knob, new Vector2(770, y0), new Vector2(770, y0 + 50), new Vector2(770, 120 + tenth * 6.5f));
            yield return TestUtil.Seconds(0.5f);
            Assert.AreEqual("Hawk Eye", c.Title.text);
            Assert.AreEqual("You've found 150/200 Hidden Objects!", c.Info.text);
            Assert.AreEqual(350 - 6 * 470, c.ColumnY, 0.01f);
            TestUtil.Capture("Achievements_HawkEye");

            // Dragging past the end clamps the knob to the bottom: the last achievement
            y0 = -knob.GetComponent<RectTransform>().anchoredPosition.y;
            TestUtil.Press(knob, new Vector2(770, y0), new Vector2(770, 700));
            Assert.AreEqual(120 + 429, -knob.GetComponent<RectTransform>().anchoredPosition.y, 0.01f);
            yield return TestUtil.Seconds(0.5f);
            Assert.AreEqual("International Chef", c.Title.text);
            Assert.AreEqual(350 - 9 * 470, c.ColumnY, 0.01f);
            TestUtil.Capture("Achievements_Last");
        }

        [UnityTest]
        public IEnumerator AchievementsBackToMainMenu()
        {
            yield return LoadWithPlayer(GameScene.Achievements);
            Session.achievementsBackToMainMenu = true;
            TestUtil.Click("BackButton");
            yield return TestUtil.ExpectSceneRequest(GameScene.MainMenu);
        }

        [UnityTest]
        public IEnumerator AchievementsBackToKitchen()
        {
            yield return LoadWithPlayer(GameScene.Achievements);
            Session.achievementsBackToMainMenu = false;
            TestUtil.Click("BackButton");
            yield return TestUtil.ExpectSceneRequest(GameScene.Kitchen);
        }

        // ------------------------------------------------------------------ Level select

        static DayData Day(int index) => GameDatabase.Instance.levels.days[index];

        /// <summary>30 days passed; day 29 (index 28) at expert level, day 30 at goal, day 31 (the next) flagged as done.</summary>
        static void LevelProfile(PlayerProfile p)
        {
            p.passedDays = 30;
            for (int i = 0; i < 28; i++) p.goalAchieved[i] = true;
            p.goalAchieved[28] = true;
            p.daysEarning[28] = Day(28).expert + 10;
            p.goalAchieved[29] = true;
            p.daysEarning[29] = 1;
            p.goalAchieved[30] = true; // reset by the screen like the original
            p.daysEarning[21] = 777;
        }

        static TextMeshProUGUI DayText(int slot) => TestUtil.Find<TextMeshProUGUI>("Day" + (slot + 1));

        [UnityTest]
        public IEnumerator LevelSelectShowsPagesAndBadges()
        {
            yield return LoadWithPlayer(GameScene.LevelSelect, LevelProfile);
            var c = Object.FindFirstObjectByType<LevelSelectController>();
            Assert.IsNotNull(c);
            var p = GameManager.Player;
            Assert.IsFalse(p.goalAchieved[30], "Next day's goal flag is cleared");
            Assert.AreEqual(30, Session.currentPlayedLevel);
            Assert.IsFalse(Session.isPlayingSpecialLevel);

            Assert.AreEqual(4, c.CurrentPage);
            Assert.AreEqual(30, c.SelectedDay);
            Assert.AreEqual("Day\n29", DayText(0).text);
            Assert.AreEqual("Day\n30", DayText(1).text);
            Assert.AreEqual("Day\n31", DayText(2).text);
            for (int i = 3; i < 7; i++)
            {
                Assert.AreEqual("", DayText(i).text, "Locked day shows nothing");
                Assert.IsFalse(DayText(i).GetComponent<Button>().interactable);
            }
            Assert.AreEqual(Color.red, DayText(2).color);
            Assert.AreEqual(Color.black, DayText(0).color);
            Assert.AreEqual("expert", TestUtil.Find<Image>("Badge1").sprite.name);
            Assert.AreEqual("goal", TestUtil.Find<Image>("Badge2").sprite.name);
            Assert.IsFalse(Active("Badge3"));
            Assert.IsFalse(Active("BadgeBig"));
            Assert.IsFalse(Active("CheckMark1") || Active("CheckMark2"));
            Assert.IsFalse(Active("RightButton"));
            Assert.IsTrue(Active("LeftButton"));
            Assert.AreEqual($"Middle East Cuisine (Match 3)\nGoal: serve {Day(30).goal} customer(s)\nExpert: earn $ {Day(30).expert}\nYour Best Score: 0",
                TestUtil.Find<TextMeshProUGUI>("NoteDescription").text);
            TestUtil.Capture("LevelSelect");

            // Day 29: expert -> both check marks and the big expert badge
            TestUtil.Click("Day1");
            Assert.AreEqual(28, c.SelectedDay);
            Assert.AreEqual(Color.red, DayText(0).color);
            Assert.AreEqual(Color.black, DayText(2).color);
            Assert.IsTrue(Active("CheckMark1") && Active("CheckMark2"));
            Assert.AreEqual("expert", TestUtil.Find<Image>("BadgeBig").sprite.name);
            TestUtil.Capture("LevelSelect_Day29");

            // Day 30: goal only
            TestUtil.Click("Day2");
            Assert.IsTrue(Active("CheckMark1"));
            Assert.IsFalse(Active("CheckMark2"));
            Assert.AreEqual("goal", TestUtil.Find<Image>("BadgeBig").sprite.name);

            // Previous page: days 22..28, all done, first day selected
            TestUtil.Click("LeftButton");
            Assert.AreEqual(3, c.CurrentPage);
            Assert.AreEqual(21, c.SelectedDay);
            Assert.AreEqual("Day\n22", DayText(0).text);
            Assert.AreEqual("Day\n28", DayText(6).text);
            Assert.AreEqual(Color.red, DayText(0).color);
            Assert.AreEqual(Color.black, DayText(1).color);
            Assert.IsTrue(Active("RightButton") && Active("LeftButton"));
            for (int i = 0; i < 7; i++) Assert.IsTrue(Active("Badge" + (i + 1)));
            StringAssert.EndsWith("Your Best Score: 777", TestUtil.Find<TextMeshProUGUI>("NoteDescription").text);
            // Day 25 is a dessert day, day 28 a food critic (special match-3) day
            TestUtil.Click("Day4");
            StringAssert.StartsWith($"Froyo Business (Minigame)\nGoal: earn ${Day(24).goal}\nExpert: earn ${Day(24).expert}",
                TestUtil.Find<TextMeshProUGUI>("NoteDescription").text);
            TestUtil.Click("Day7");
            StringAssert.Contains(" food critic", TestUtil.Find<TextMeshProUGUI>("NoteDescription").text);
            TestUtil.Capture("LevelSelect_Page4");

            // Page 3 has the hidden object day 18
            TestUtil.Click("LeftButton");
            TestUtil.Click("Day4");
            StringAssert.StartsWith($"Cleaning up (Hidden Objects)\nGoal: find {Day(17).goal} items\nExpert: {Day(17).expert} items",
                TestUtil.Find<TextMeshProUGUI>("NoteDescription").text);

            TestUtil.Click("RightButton");
            TestUtil.Click("RightButton");
            Assert.AreEqual(4, c.CurrentPage);
            Assert.AreEqual(30, c.SelectedDay);
            Assert.IsFalse(Active("RightButton"));
        }

        [UnityTest]
        public IEnumerator LevelSelectAllDaysDone()
        {
            yield return LoadWithPlayer(GameScene.LevelSelect, p => p.passedDays = GameRules.TotalDays);
            var c = Object.FindFirstObjectByType<LevelSelectController>();
            Assert.AreEqual(12, c.CurrentPage);
            Assert.AreEqual(90, c.SelectedDay);
            Assert.AreEqual("Day\n85", DayText(0).text);
            Assert.AreEqual("Day\n91", DayText(6).text);
            Assert.AreEqual(Color.red, DayText(6).color);
            Assert.IsFalse(Active("RightButton"));
            Assert.IsTrue(Active("LeftButton"));
        }

        /// <summary>Opens the page of <paramref name="day"/> (0-based), selects it and presses Play.</summary>
        static IEnumerator PlayDay(int day, GameScene expected)
        {
            yield return LoadWithPlayer(GameScene.LevelSelect, LevelProfile);
            var c = Object.FindFirstObjectByType<LevelSelectController>();
            while (c.CurrentPage > day / 7) TestUtil.Click("LeftButton");
            TestUtil.Click("Day" + (day % 7 + 1));
            Assert.AreEqual(day, c.SelectedDay);
            TestUtil.Click("PlayButton");
            Assert.AreEqual(day, Session.currentPlayedLevel, "Session day is 0-based");
            Assert.AreSame(Day(day), Session.CurrentDay);
            Assert.AreEqual(Day(day).region ?? "", Session.currentRegionDish);
            Assert.AreEqual(Day(day).gameType == GameType.Match3Special, Session.isPlayingSpecialLevel);
            yield return TestUtil.ExpectSceneRequest(expected);
        }

        [UnityTest] public IEnumerator LevelSelectPlaysMatch3Day() => PlayDay(28, GameScene.Match3);
        [UnityTest] public IEnumerator LevelSelectPlaysSpecialMatch3Day() => PlayDay(27, GameScene.Match3);
        [UnityTest] public IEnumerator LevelSelectPlaysDessertDay() => PlayDay(24, GameScene.Dessert);
        [UnityTest] public IEnumerator LevelSelectPlaysHiddenObjectDay() => PlayDay(17, GameScene.HiddenObject);

        [UnityTest]
        public IEnumerator LevelSelectBackToKitchen()
        {
            yield return LoadWithPlayer(GameScene.LevelSelect, LevelProfile);
            TestUtil.Click("BackButton");
            yield return TestUtil.ExpectSceneRequest(GameScene.Kitchen);
        }
    }
}
