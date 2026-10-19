using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using Rory.Data;
using Rory.Popups;
using Rory.Screens.Dessert;
using Rory.Screens.HiddenObject;
using Rory.Screens.Match3;
using Rory.Screens.MainMenu;
using Rory.Tutorials;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Rory.Tests
{
    /// <summary>
    /// End-to-end play: a bot plays the game through the real screen transitions (no direct scene setup), like a
    /// new player would. Any error or exception logged on the way fails the test.
    /// </summary>
    public class GameFlowTests
    {
        static GameManager Gm => GameManager.Instance;
        static GameScene? Scene => Gm.Flow.Current;

        [UnitySetUp]
        public IEnumerator Prepare()
        {
            Time.timeScale = 1;
            Rory.Localization.Loc.SetLanguage(Rory.Localization.Loc.English);
            while (Gm.Flow.IsTransitioning) yield return null; // a previous test may have left a fade running
        }

        [TearDown]
        public void Restore() => Time.timeScale = 1;

        // ------------------------------------------------------------------ the bot

        /// <summary>One bot action for the current screen; true if it did something.</summary>
        static bool Step()
        {
            if (Gm.Flow.IsTransitioning) return false;
            var popups = PopupManager.Current;
            if (popups != null && popups.HasOpenPopup)
            {
                var top = popups.Open[popups.Open.Count - 1];
                if (top is CreatePlayerPopup create)
                {
                    create.nameInput.text = "Bot";
                    return Press(create.rightButton);
                }
                if (top is WindowPopup w)
                {
                    if (top is PlayerSelectPopup) return Press(w.uniButton); // "Select"
                    return Press(w.uniButton) || Press(w.rightButton) || Press(w.leftButton);
                }
                return false;
            }
            var tut = TutorialManager.Current;
            if (tut != null && tut.IsButtonShowing)
            {
                var next = tut.GetComponentsInChildren<Button>().FirstOrDefault(b => b.name == "NextButton" && b.gameObject.activeInHierarchy);
                if (Press(next)) return true;
            }
            switch (Scene)
            {
                case GameScene.MainMenu: return Click("StoryButton");
                case GameScene.Opening:
                case GameScene.Ending: return Click("TouchArea");
                case GameScene.LevelSelect: return Click("PlayButton");
                case GameScene.Kitchen: return KitchenStep(tut);
                case GameScene.Match3: return Match3Step(tut);
                case GameScene.HiddenObject: return HiddenObjectStep();
                case GameScene.Dessert: return DessertStep();
            }
            return false;
        }

        static bool Press(Button b)
        {
            if (b == null || !b.gameObject.activeInHierarchy || !b.IsInteractable()) return false;
            b.onClick.Invoke();
            return true;
        }

        static bool Click(string name) => Press(TestUtil.Find<Button>(name));

        static void PointerDown(Component c)
        {
            var e = new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left };
            ExecuteEvents.Execute(c.gameObject, e, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(c.gameObject, e, ExecuteEvents.pointerUpHandler);
        }

        static int kitchenTry;

        /// <summary>Kitchen: play the next day, or follow whatever the tutorial arrow allows (shops, buy).</summary>
        static bool KitchenStep(TutorialManager tut)
        {
            if (tut != null && tut.CurrentDialogueId != TutorialIds.None && tut.IsShowing && !tut.IsButtonShowing)
            {
                // an action line: try the kitchen controls in turn; the tutorial lets only the right one work
                string[] order = { "RecipeQuad", "ShopQuad", "HardwareQuad", "RightButton", "BuyButton", "PlayQuad", "BackButton" };
                return Click(order[kitchenTry++ % order.Length]);
            }
            return Click("PlayQuad") || Click("BackButton");
        }

        static bool Match3Step(TutorialManager tut)
        {
            var game = Match3Controller.Current;
            if (game == null || !game.Ready || game.hasWindow) return false;
            foreach (var coin in game.Coins)
            {
                if (coin != null && coin.isActiveAndEnabled)
                {
                    PointerDown(coin);
                    return true;
                }
            }
            if (tut != null && tut.CurrentDialogueId == TutorialIds.LEVEL_1 && tut.DialogueCounter == 7 && game.Customers.Count > 1)
            {
                PointerDown(game.Customers[1]); // "click on their plate to change priority"
                return true;
            }
            var b = game.Board;
            // (hinted tiles pulse forever, so running tweens don't mean the board is busy)
            if (b.moveTile || b.isSwapping || b.AllTiles.Any(t => t.removed || t.onTween)) return false;
            var move = Match3Tests.FindMove(b);
            if (move == null) return false;
            var (from, to) = move.Value;
            b.lastCol = from.x;
            b.lastRow = from.y;
            b.Touch(BoardTouch.Hover, Match3Board.CellX(from.x), Match3Board.CellY(from.y));
            b.Touch(BoardTouch.Began, Match3Board.CellX(from.x), Match3Board.CellY(from.y));
            b.Touch(BoardTouch.Moved, Match3Board.CellX(to.x), Match3Board.CellY(to.y));
            b.Touch(BoardTouch.Ended, Match3Board.CellX(to.x), Match3Board.CellY(to.y));
            return true;
        }

        static bool HiddenObjectStep()
        {
            var ho = UnityEngine.Object.FindFirstObjectByType<HiddenObjectController>();
            if (ho == null || !ho.IsReady) return false;
            foreach (var l in ho.Labels.Where(l => l.aktif && !l.itemIsFound))
            {
                var item = ho.Items[l.idx];
                if (item.Found || !HiddenObjectTests.TryFindHitPoint(item, out var p)) continue;
                TestUtil.ClickStage(p.x, p.y);
                return true;
            }
            return false;
        }

        static bool DessertStep()
        {
            var d = UnityEngine.Object.FindFirstObjectByType<DessertController>();
            if (d == null || !d.OrderInPlace) return false;
            foreach (var id in d.OrderList.ToList()) DessertTests.Press(id);
            return true;
        }

        /// <summary>Lets the bot play until <paramref name="done"/> is true.</summary>
        static IEnumerator Play(Func<bool> done, float timeout, string what)
        {
            float end = Time.realtimeSinceStartup + timeout;
            int idle = 0;
            while (!done())
            {
                if (Time.realtimeSinceStartup > end)
                {
                    TestUtil.Capture("Flow_Stuck");
                    var tut = TutorialManager.Current;
                    Assert.Fail($"Stuck while {what}: scene {Scene}, popups " +
                                $"{(PopupManager.Current != null ? string.Join(",", PopupManager.Current.Open.Select(p => p.name)) : "-")}, " +
                                $"tutorial {(tut != null ? tut.CurrentDialogueId + "/" + tut.DialogueCounter + (tut.IsButtonShowing ? " (button)" : "") : "-")}");
                }
                bool acted = Step();
                idle = acted ? 0 : idle + 1;
                // act at most every few frames so tweens and fades can run like for a person
                yield return TestUtil.Frames(acted ? 4 : 1);
            }
        }

        static IEnumerator Reach(GameScene scene, float timeout) =>
            Play(() => Scene == scene && !Gm.Flow.IsTransitioning, timeout, "going to " + scene);

        // ------------------------------------------------------------------ tests

        [UnityTest]
        public IEnumerator NewPlayerPlaysTheFirstTwoDays()
        {
            Gm.UseSaveFolder(TestUtil.TempSaveFolder());
            SceneManager.LoadScene(GameScene.Splash.ToString());
            yield return TestUtil.Frames(2);
            yield return Reach(GameScene.MainMenu, 20);
            yield return Reach(GameScene.Opening, 20);
            Assert.AreEqual("Bot", GameManager.Player.name);
            yield return Reach(GameScene.Kitchen, 60);
            Assert.IsTrue(GameManager.Player.openingViewed);
            TestUtil.Capture("Flow_Kitchen_Day1");

            Time.timeScale = 4;
            yield return Reach(GameScene.Match3, 60);
            Assert.AreEqual(0, Gm.Session.currentPlayedLevel);
            TestUtil.Capture("Flow_Match3_Day1");
            yield return Reach(GameScene.Kitchen, 240);
            Time.timeScale = 1;
            TestUtil.Capture("Flow_Kitchen_AfterDay1");
            Assert.AreEqual(1, GameManager.Player.passedDays, "day 1 finished");
            Assert.IsTrue(GameManager.Player.goalAchieved[0], "the bot reaches day 1's goal");

            Time.timeScale = 4;
            yield return Reach(GameScene.Match3, 60);
            Assert.AreEqual(1, Gm.Session.currentPlayedLevel);
            yield return Reach(GameScene.Kitchen, 240);
            Time.timeScale = 1;
            Assert.AreEqual(2, GameManager.Player.passedDays, "day 2 finished");
            // after day 2 the kitchen runs the recipe shop tutorial; the bot follows it and opens the next day
            yield return Reach(GameScene.LevelSelect, 120);
            TestUtil.Capture("Flow_LevelSelect_Day3");
        }

        /// <summary>Plays the next story day (from the kitchen) whatever its type, and comes back to the kitchen.</summary>
        static IEnumerator PlayDayFromKitchen(int passedDays, GameScene expected)
        {
            var gm = Gm;
            gm.UseSaveFolder(TestUtil.TempSaveFolder());
            var p = gm.Saves.CreatePlayer(0, "Bot");
            p.openingViewed = true;
            p.passedDays = passedDays;
            p.lastDayKitchen = passedDays;
            for (int i = 0; i < p.tutorialShowed.Count; i++) p.tutorialShowed[i] = true;
            gm.Saves.Save();
            SceneManager.LoadScene(GameScene.Kitchen.ToString());
            yield return TestUtil.Frames(3);
            Time.timeScale = 4;
            yield return Reach(expected, 60);
            TestUtil.Capture("Flow_" + expected + "_Day" + (passedDays + 1));
            yield return Reach(GameScene.Kitchen, 300);
            Time.timeScale = 1;
            Assert.AreEqual(passedDays + 1, GameManager.Player.passedDays, "day finished and saved");
        }

        static int FirstDay(GameType type) =>
            GameDatabase.Instance.levels.days.FindIndex(d => d.gameType == type);

        [UnityTest]
        public IEnumerator HiddenObjectDayFromTheKitchen() =>
            PlayDayFromKitchen(FirstDay(GameType.HiddenObject), GameScene.HiddenObject);

        [UnityTest]
        public IEnumerator DessertDayFromTheKitchen() =>
            PlayDayFromKitchen(FirstDay(GameType.DessertMaking), GameScene.Dessert);

        [UnityTest]
        public IEnumerator FoodCriticDayFromTheKitchen() =>
            PlayDayFromKitchen(FirstDay(GameType.Match3Special), GameScene.Match3);

        [UnityTest]
        public IEnumerator LateMatch3DayFromTheKitchen() => PlayDayFromKitchen(40, GameScene.Match3);

        [UnityTest]
        public IEnumerator ExtrasHiddenObjectAndDessert()
        {
            Gm.UseSaveFolder(TestUtil.TempSaveFolder());
            SceneManager.LoadScene(GameScene.MainMenu.ToString());
            yield return TestUtil.Frames(3);
            Time.timeScale = 4;
            TestUtil.Click("ExtrasButton");
            yield return null;
            TestUtil.ClickLabel("Hidden Object");
            yield return Play(() => Scene == GameScene.HiddenObject && !Gm.Flow.IsTransitioning, 20, "opening extras HO");
            yield return Play(() => PopupManager.Current != null && PopupManager.Current.HasOpenPopup &&
                                    PopupManager.Current.Open.Last() is Popups.Game.ResultPopup, 300, "playing extras HO");
            TestUtil.Capture("Flow_ExtrasHO_Result");
            Time.timeScale = 1;
        }
    }
}
