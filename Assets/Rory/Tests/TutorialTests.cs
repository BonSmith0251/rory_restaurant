using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Rory.Tutorials;
using Rory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

namespace Rory.Tests
{
    /// <summary>
    /// Tutorial bubble and rule table. Runs on the MainMenu scene with the Tutorial prefab added under its stage and
    /// a fake screen host. Expected positions come from the original (harness tree of the legacy port).
    /// </summary>
    public class TutorialTests
    {
        const string PrefabPath = "Assets/Rory/Prefabs/Tutorial/Tutorial.prefab";

        class FakeHost : ITutorialHost
        {
            public bool kitchen;
            public string juice;
            public Dictionary<Vector2Int, Vector2> tiles = new Dictionary<Vector2Int, Vector2>();
            public bool hasItem;
            public Vector2 itemPos;
            public float itemHeight;
            public readonly List<TutorialCommand> commands = new List<TutorialCommand>();
            /// <summary>Tiles reported as "highlight" when asked to highlight a possible move.</summary>
            public List<Vector2Int> highlightTiles = new List<Vector2Int>();

            public bool IsKitchen => kitchen;
            public string JuicePercentageText => juice;

            public bool TryGetTilePosition(int col, int row, out Vector2 position)
            {
                // like the board: column x = 470 + col * 68, row y = 75 + row * 68 unless given
                if (!tiles.TryGetValue(new Vector2Int(col, row), out position)) position = new Vector2(470 + col * 68, 75 + row * 68);
                return true;
            }

            public bool TryGetFirstListedItem(out Vector2 position, out float height)
            {
                position = itemPos;
                height = itemHeight;
                return hasItem;
            }

            public void OnTutorialCommand(TutorialCommand command)
            {
                commands.Add(command);
                if (command.Type == TutorialCommandType.HighlightPossibleMove)
                    foreach (var t in highlightTiles)
                        TutorialManager.Current.Trigger(TutorialActions.Highlight,
                            new Dictionary<string, object> { [TutorialActions.KeyCol] = t.x, [TutorialActions.KeyRow] = t.y });
            }
        }

        TutorialManager tut;
        FakeHost host;
        UIStage stage;

        IEnumerator Setup(string playerName = "Tester")
        {
            TutorialManager.ResetRuntimeOverrides();
            yield return TestUtil.LoadScene(GameScene.MainMenu);
            GameManager.Instance.Saves.CreatePlayer(0, playerName);
            stage = Object.FindFirstObjectByType<UIStage>();
#if UNITY_EDITOR
            var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<TutorialManager>(PrefabPath);
            Assert.IsNotNull(prefab, "Tutorial prefab missing (run the build)");
            tut = Object.Instantiate(prefab, stage.transform);
#endif
            Assert.AreSame(tut, TutorialManager.Current);
            host = new FakeHost();
            tut.Host = host;
            yield return null;
        }

        /// <summary>Active part of the bubble by name (null when hidden).</summary>
        RectTransform Part(string name)
        {
            var t = (RectTransform)tut.transform.Find(name);
            return t != null && t.gameObject.activeInHierarchy ? t : null;
        }

        RectTransform AnyPart(string name) => (RectTransform)tut.transform.Find(name);
        string Text => AnyPart("Text").GetComponent<TextMeshProUGUI>().text;
        string ButtonLabel => AnyPart("NextButton").GetComponentInChildren<TextMeshProUGUI>().text;

        /// <summary>Stage-space bounds (x, y-down, w, h) of a RectTransform, scale included.</summary>
        Rect Bounds(RectTransform rt)
        {
            var c = new Vector3[4];
            rt.GetWorldCorners(c);
            var srt = (RectTransform)stage.transform;
            Vector2 a = srt.InverseTransformPoint(c[0]), b = srt.InverseTransformPoint(c[2]);
            var r = srt.rect;
            float x0 = Mathf.Min(a.x, b.x) - r.xMin, x1 = Mathf.Max(a.x, b.x) - r.xMin;
            float y0 = r.yMax - Mathf.Max(a.y, b.y), y1 = r.yMax - Mathf.Min(a.y, b.y);
            return new Rect(x0, y0, x1 - x0, y1 - y0);
        }

        void AssertBounds(string name, float x, float y, float w, float h)
        {
            var b = Bounds(Part(name));
            const float tol = 1.01f;
            Assert.That(b.x, Is.EqualTo(x).Within(tol), $"{name} x ({b})");
            Assert.That(b.y, Is.EqualTo(y).Within(tol), $"{name} y ({b})");
            Assert.That(b.width, Is.EqualTo(w).Within(tol), $"{name} width ({b})");
            Assert.That(b.height, Is.EqualTo(h).Within(tol), $"{name} height ({b})");
        }

        Vector2 ArrowPos => AnyPart("Arrow").GetStagePosition();

        [UnityTest]
        public IEnumerator OpeningStoryStepsThrough()
        {
            yield return Setup();
            Assert.IsFalse(tut.IsShowing);
            Assert.IsNull(Part("TextBox"));
            Assert.AreEqual(-1, tut.CurrentDialogueId);
            // the bubble sits right below the popup layer (windows cover it like the original)
            Assert.AreEqual(Popups.PopupManager.Current.transform.GetSiblingIndex() - 1, tut.transform.GetSiblingIndex());

            int finished = -1;
            tut.DialogueFinished += id => finished = id;
            tut.Show(TutorialIds.OPENING_STORY);
            yield return null;
            Assert.IsTrue(tut.IsShowing);
            Assert.IsTrue(tut.IsButtonShowing);
            Assert.IsNotNull(Part("Text"));
            Assert.IsNotNull(Part("Avatar"));
            Assert.IsNull(Part("Arrow"), "no arrow on line 0");
            Assert.AreEqual("Finally, the days of my culinary school are over!", Text);
            Assert.AreEqual("Next", ButtonLabel);
            // harness: textbox (268,227,488x146) avatar (273,220,116x148) button (409,381,207x58) text (383,232,353x136)
            AssertBounds("TextBox", 268.4f, 227, 488.4f, 146);
            AssertBounds("Avatar", 272.8f, 220, 116, 148);
            AssertBounds("NextButton", 409, 381, 207, 58);
            AssertBounds("Text", 382.8f, 232, 353.4f, 136);
            TestUtil.Capture("Tutorial_Opening0");

            for (int i = 1; i <= 3; i++)
            {
                TestUtil.Click("NextButton");
                Assert.AreEqual(i, tut.DialogueCounter);
            }
            Assert.AreEqual("Of course!", ButtonLabel);
            TestUtil.Click("NextButton");
            Assert.AreEqual(4, tut.DialogueCounter);
            Assert.IsFalse(tut.IsButtonShowing, "action line has no button");
            Assert.IsNotNull(Part("Arrow"));
            Assert.That(ArrowPos.x, Is.EqualTo(450).Within(0.01f));
            Assert.That(ArrowPos.y, Is.EqualTo(470).Within(0.01f));
            Assert.Greater(AnyPart("Arrow").localScale.x, 0, "arrowDirection 1 points right");
            yield return TestUtil.Seconds(0.6f);
            Assert.Less(ArrowPos.x, 445, "arrow bounces 40 px back from its tip");
            Assert.That(ArrowPos.y, Is.EqualTo(470).Within(0.01f));
            TestUtil.Capture("Tutorial_Opening4");

            tut.Next();
            Assert.AreEqual(-1, tut.CurrentDialogueId);
            Assert.AreEqual(TutorialIds.OPENING_STORY, finished);
            Assert.IsFalse(tut.IsShowing);
            Assert.IsFalse(Tweening.Tweens.IsTweening(AnyPart("Arrow")), "arrow tween killed on hide");
        }

        [UnityTest]
        public IEnumerator Level1BlockNextAndTriggers()
        {
            yield return Setup();
            // first match: tiles (2,3), (3,3) in a row, (4,4) to be swapped up -> TILEX = col 4, TILEY = row 4
            host.highlightTiles = new List<Vector2Int> { new Vector2Int(2, 3), new Vector2Int(3, 3), new Vector2Int(4, 4) };
            tut.Show(TutorialIds.LEVEL_1);
            Assert.AreEqual("OK!", ButtonLabel);
            TestUtil.Click("NextButton");
            Assert.AreEqual(1, tut.DialogueCounter);
            Assert.That(ArrowPos.x, Is.EqualTo(350).Within(0.01f));
            Assert.That(ArrowPos.y, Is.EqualTo(20).Within(0.01f));
            Assert.Less(AnyPart("Arrow").localScale.x, 0);
            yield return null;
            TestUtil.Capture("Tutorial_Level1_1");

            // blockNext line: clicked_ok starts the level with 1 customer, the bubble hides and waits
            TestUtil.Click("NextButton");
            Assert.AreEqual(1, host.commands.Count);
            Assert.AreEqual(TutorialCommandType.StartLevel, host.commands[0].Type);
            Assert.AreEqual(1, host.commands[0].Customers);
            Assert.IsFalse(tut.IsShowing);
            Assert.AreEqual(TutorialIds.LEVEL_1, tut.CurrentDialogueId);
            Assert.AreEqual(1, tut.DialogueCounter);

            tut.Trigger(TutorialActions.Matched); // wrong action for this line
            Assert.AreEqual(1, tut.DialogueCounter);
            tut.Trigger(TutorialActions.CustomerIn, TutorialActions.KeyPosition, 0);
            Assert.AreEqual(2, tut.DialogueCounter);
            Assert.IsTrue(tut.IsShowing);

            // clicked_ok at line 2: highlight a move (the host reports 3 tiles), then line 3
            TestUtil.Click("NextButton");
            Assert.AreEqual(TutorialCommandType.HighlightPossibleMove, host.commands[1].Type);
            Assert.AreEqual(3, tut.DialogueCounter);
            Assert.AreEqual(470 + 4 * 68 + 40, tut.HighlightX);
            Assert.AreEqual(75 + 4 * 68, tut.HighlightY);
            AssertBounds("TextBox", 250 - 244.2f, 480 - 73, 488.4f, 146); // harness (6,407)
            Assert.IsFalse(tut.IsButtonShowing);

            tut.Trigger(TutorialActions.Matched);
            Assert.AreEqual(4, tut.DialogueCounter);
            TestUtil.Click("NextButton"); // blockNext "OK!" -> hide
            Assert.IsFalse(tut.IsShowing);
            tut.Trigger(TutorialActions.DishCompleted);
            Assert.AreEqual(5, tut.DialogueCounter);
            tut.Trigger(TutorialActions.ClickedCoin);
            Assert.IsTrue(tut.BlockCustomerIn);
            Assert.AreEqual(6, tut.DialogueCounter);
            TestUtil.Click("NextButton"); // blockNext: customers leave, customers may come again
            Assert.IsFalse(tut.BlockCustomerIn);
            Assert.AreEqual(TutorialCommandType.ClearHasWindow, host.commands[2].Type);
            Assert.AreEqual(TutorialCommandType.SendCustomersOutSatisfied, host.commands[3].Type);
            tut.Trigger(TutorialActions.CustomerIn, TutorialActions.KeyPosition, 1);
            Assert.AreEqual(6, tut.DialogueCounter, "only the customer in slot 2 continues");
            tut.Trigger(TutorialActions.CustomerIn, TutorialActions.KeyPosition, 2);
            Assert.AreEqual(7, tut.DialogueCounter);
            tut.Trigger(TutorialActions.ChangedPriority);
            Assert.AreEqual(8, tut.DialogueCounter);
            Assert.AreEqual("No problem!", ButtonLabel);
            TestUtil.Click("NextButton");
            Assert.AreEqual(-1, tut.CurrentDialogueId);
        }

        [UnityTest]
        public IEnumerator CanClickThisButtonGating()
        {
            yield return Setup();
            Assert.IsTrue(tut.CanClickThisButton(TutorialButtons.SHOP_ID), "no tutorial: everything clickable");

            tut.Show(TutorialIds.AFTER_LEVEL_2);
            Assert.IsTrue(tut.CanClickThisButton(TutorialButtons.SHOP_ID), "line 0 doesn't restrict");
            TestUtil.Click("NextButton");
            Assert.AreEqual(1, tut.DialogueCounter);
            Assert.IsTrue(tut.CanClickThisButton(TutorialButtons.RECIPE_ID));
            Assert.IsFalse(tut.CanClickThisButton(TutorialButtons.SHOP_ID));
            Assert.IsFalse(tut.CanClickThisButton(TutorialButtons.NONE), "swipe blocked");
            tut.Trigger(TutorialActions.ClickedIngredients);
            Assert.AreEqual(1, tut.DialogueCounter);
            tut.Trigger(TutorialActions.ClickedRecipeShop);
            Assert.AreEqual(2, tut.DialogueCounter);
            Assert.IsTrue(tut.CanClickThisButton(TutorialButtons.SHOP_ID));
            TestUtil.Click("NextButton");
            Assert.IsFalse(tut.CanClickThisButton(TutorialButtons.BUY_BUTTON));
            Assert.IsTrue(tut.CanClickThisButton(TutorialButtons.RIGHT_BUTTON));
            tut.Trigger(TutorialActions.RightButton);
            Assert.IsTrue(tut.CanClickThisButton(TutorialButtons.BUY_BUTTON));
            Assert.IsFalse(tut.CanClickThisButton(TutorialButtons.RIGHT_BUTTON));
            tut.Trigger(TutorialActions.BuyItem);
            Assert.AreEqual(5, tut.DialogueCounter);

            tut.Show(TutorialIds.AFTER_LEVEL_8, 2);
            Assert.IsTrue(tut.CanClickThisButton(TutorialButtons.SHOP_ID));
            Assert.IsFalse(tut.CanClickThisButton(TutorialButtons.RECIPE_ID));

            int[] allowed = { 0, 1, 3, 4, 6 };
            for (int i = 0; i < allowed.Length; i++)
            {
                tut.Show(TutorialIds.LEVEL_11, 7 + i);
                for (int id = 0; id <= 10; id++)
                    Assert.AreEqual(id == allowed[i], tut.CanClickThisButton(id), $"LEVEL_11 line {7 + i} ingredient {id}");
            }
            // the dessert rules step through the ingredients and hide at the beverage
            tut.Show(TutorialIds.LEVEL_11, 7);
            foreach (int id in allowed) tut.Trigger(TutorialActions.TouchIngredient, TutorialActions.KeyId, id);
            Assert.AreEqual(11, tut.DialogueCounter);
            Assert.IsFalse(tut.IsShowing);
            tut.Trigger(TutorialActions.CheckTimesUp);
            Assert.AreEqual(12, tut.DialogueCounter);

            tut.Show(TutorialIds.AFTER_LEVEL_34, 1);
            Assert.IsFalse(tut.CanClickThisButton(TutorialButtons.SHOP_ID));
            Assert.IsTrue(tut.CanClickThisButton(TutorialButtons.HARDWARE_ID));
            tut.Trigger(TutorialActions.ClickedHardware);
            Assert.AreEqual(2, tut.DialogueCounter);
        }

        [UnityTest]
        public IEnumerator CheckTutMarksShown()
        {
            yield return Setup();
            var p = GameManager.Player;
            var session = GameManager.Instance.Session;
            p.passedDays = 0;
            session.SelectDay(0); // first day

            Assert.IsFalse(tut.CheckTut(TutorialIds.AFTER_LEVEL_1, 1), "wrong day");
            Assert.IsTrue(tut.CheckTut(TutorialIds.LEVEL_1, 0));
            Assert.IsTrue(p.tutorialShowed[TutorialIds.LEVEL_1]);
            Assert.AreEqual(TutorialIds.LEVEL_1, tut.CurrentDialogueId);
            Assert.IsFalse(tut.CheckTut(TutorialIds.LEVEL_1, 0), "already shown");
            Assert.IsTrue(tut.CheckTut(TutorialIds.LEVEL_1, 0, true), "forced");

            // replaying an older day: the played day must match outside the kitchen
            p.passedDays = 2;
            session.SelectDay(1);
            Assert.IsFalse(tut.CheckTut(TutorialIds.AFTER_LEVEL_2, 2));
            Assert.IsFalse(p.tutorialShowed[TutorialIds.AFTER_LEVEL_2]);
            host.kitchen = true;
            Assert.IsTrue(tut.CheckTut(TutorialIds.AFTER_LEVEL_2, 2), "the kitchen ignores the played day");
            Assert.IsTrue(p.tutorialShowed[TutorialIds.AFTER_LEVEL_2]);
            Assert.IsTrue(tut.IsShowing);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ArrowTokensAndTextBoxMoves()
        {
            yield return Setup();

            // HOITEMX/HOITEMY: item left of centre and low -> box at (670, 250), arrow flipped and 100 px right
            host.hasItem = true;
            host.itemPos = new Vector2(400, 420);
            host.itemHeight = 60;
            tut.Show(TutorialIds.LEVEL_4, 4);
            Assert.That(ArrowPos.x, Is.EqualTo(400 - 40 + 100).Within(0.01f));
            Assert.That(ArrowPos.y, Is.EqualTo(420 + 30).Within(0.01f));
            Assert.Less(AnyPart("Arrow").localScale.x, 0, "direction overridden to -1");
            AssertBounds("TextBox", 670 - 244.2f, 250 - 73, 488.4f, 146); // harness (426,177)
            AssertBounds("Text", 670 - 129.2f, 250 - 68, 353.4f, 136);     // harness (541,182)
            yield return null;
            TestUtil.Capture("Tutorial_HOItem");
            // the override stays (the original wrote it into the dialogue XML), the data asset is untouched
            Assert.AreEqual(1, Data.GameDatabase.Instance.dialogues.dialogues[TutorialIds.LEVEL_4].lines[4].arrowDirection);
            host.itemPos = new Vector2(800, 100);
            tut.Show(TutorialIds.LEVEL_4, 4);
            Assert.That(ArrowPos.x, Is.EqualTo(760).Within(0.01f));
            Assert.Less(AnyPart("Arrow").localScale.x, 0);
            Assert.That(AnyPart("TextBox").GetStagePosition(), Is.EqualTo(new Vector2(335, 500)));
            tut.Trigger(TutorialActions.GetCorrectHoItem);
            Assert.AreEqual(5, tut.DialogueCounter);
            tut.Trigger(TutorialActions.UsedHint);
            Assert.IsFalse(tut.IsShowing);
            tut.Trigger(TutorialActions.GetCorrectHoItem);
            Assert.AreEqual(6, tut.DialogueCounter);

            // TORCHX/TORCHY = tile [1][1] (+40 x)
            host.tiles[new Vector2Int(1, 1)] = new Vector2(540, 145);
            tut.Show(TutorialIds.LEVEL_27, 2);
            Assert.That(ArrowPos, Is.EqualTo(new Vector2(580, 145)));
            tut.Trigger(TutorialActions.StartTorch);
            Assert.AreEqual(3, tut.DialogueCounter);
            Assert.AreEqual(TutorialCommandType.ClearHasWindow, host.commands[host.commands.Count - 1].Type);

            // SCUSTY: set by special_customer_in (120 + position * 118)
            tut.Show(TutorialIds.LEVEL_7, 2);
            tut.Trigger(TutorialActions.SpecialCustomerIn, TutorialActions.KeyPosition, 1);
            Assert.AreEqual(3, tut.DialogueCounter);
            Assert.That(ArrowPos, Is.EqualTo(new Vector2(100, 238)));
            Assert.AreEqual("SCUSTY", Data.GameDatabase.Instance.dialogues.dialogues[TutorialIds.LEVEL_7].lines[3].arrowY);
            TestUtil.Capture("Tutorial_Critic");
        }

        [UnityTest]
        public IEnumerator StartLevelLineJuiceAndPlayerName()
        {
            yield return Setup("Rory Fan");
            tut.Show(TutorialIds.AFTER_LEVEL_1);
            Assert.AreEqual("Great job, Rory Fan! I must say that we did really great yesterday!", Text);

            int startEvents = 0;
            tut.StartLevelLineReached += () => startEvents++;
            host.juice = "75%";
            tut.Show(TutorialIds.LEVEL_12);
            Assert.AreEqual(1, startEvents);
            Assert.IsFalse(tut.IsShowing);
            var last = host.commands[host.commands.Count - 1];
            Assert.AreEqual(TutorialCommandType.StartLevel, last.Type);
            Assert.IsTrue(last.FromStartLevelLine);
            Assert.AreEqual(3, last.Customers);

            tut.Trigger(TutorialActions.CustomerIn, TutorialActions.KeyPosition, 0);
            Assert.AreEqual(1, tut.DialogueCounter);
            Assert.AreEqual(TutorialCommandType.ClearHasWindow, host.commands[host.commands.Count - 1].Type);
            TestUtil.Click("NextButton");
            Assert.AreEqual(2, tut.DialogueCounter);
            Assert.That(Text, Does.StartWith("Currently the machine is 75% complete."));
            yield return null;
            TestUtil.Capture("Tutorial_Juice");

            // LEVEL_26: the first highlight freezes the tile the player must swap
            tut.Show(TutorialIds.LEVEL_26, 0); // "(start level)"
            host.commands.Clear();
            foreach (var t in new[] { new Vector2Int(5, 2), new Vector2Int(6, 2), new Vector2Int(7, 3) })
                tut.Trigger(TutorialActions.Highlight, new Dictionary<string, object> { ["col"] = t.x, ["row"] = t.y });
            Assert.AreEqual(1, host.commands.Count);
            Assert.AreEqual(TutorialCommandType.FreezeTile, host.commands[0].Type);
            Assert.AreEqual(new Vector2Int(5, 2), new Vector2Int(host.commands[0].Col, host.commands[0].Row));
            Assert.IsTrue(tut.IsHighlightedTile(6, 2));
            Assert.IsFalse(tut.IsHighlightedTile(0, 0));
            yield return null;
        }
    }
}
