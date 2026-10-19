using System.Collections;
using System.Linq;
using NUnit.Framework;
using Rory.Data;
using Rory.Popups;
using Rory.Popups.Game;
using Rory.Screens.HiddenObject;
using Rory.Tutorials;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using AchievementList = Rory.Achievements.Achievements;

namespace Rory.Tests
{
    /// <summary>The hidden object screen (MiniGameHO / HOClass): rooms, pixel clicks, hints, results, extras, tutorial.</summary>
    public class HiddenObjectTests
    {
        static GameSession Session => GameManager.Instance.Session;
        static Save.PlayerProfile Player => GameManager.Player;
        static PopupManager Popups => PopupManager.Current;
        static HiddenObjectController Ho => Object.FindFirstObjectByType<HiddenObjectController>();

        string saveFolder;

        [TearDown]
        public void TearDown()
        {
            Time.timeScale = 1;
            TutorialManager.ResetRuntimeOverrides();
        }

        /// <summary>Fresh saves with player "Tester" on story day <paramref name="day"/> (0-based), then loads the scene.</summary>
        IEnumerator Load(int day, bool extras = false, bool skipTutorials = true)
        {
            // a scene change of the previous test may still be fading in (its overlay blocks clicks)
            float until = Time.realtimeSinceStartup + 5;
            while (GameManager.Instance.Flow.IsTransitioning && Time.realtimeSinceStartup < until) yield return null;
            saveFolder = TestUtil.TempSaveFolder();
            GameManager.Instance.UseSaveFolder(saveFolder);
            GameManager.Instance.Saves.CreatePlayer(0, "Tester");
            GameManager.Instance.Saves.Select(0);
            Player.passedDays = day;
            if (skipTutorials) for (int i = 0; i < Player.tutorialShowed.Count; i++) Player.tutorialShowed[i] = true;
            Session.SelectDay(day);
            Session.extrasMode = extras;
            GameManager.Instance.Audio.MuteSound = false;
            GameManager.Instance.Audio.MuteMusic = false;
            yield return LoadSceneOnly();
        }

        static IEnumerator LoadSceneOnly()
        {
            SceneManager.LoadScene(GameScene.HiddenObject.ToString());
            yield return null;
            yield return null;
            Assert.IsNotNull(Ho, "No HiddenObjectController");
            Assert.IsTrue(Ho.IsReady);
        }

        /// <summary>Closes the day's beginning windows ("Continue").</summary>
        static IEnumerator CloseWindows()
        {
            for (int i = 0; i < 5 && Popups.HasOpenPopup; i++)
            {
                TestUtil.ClickLabel("Continue");
                yield return null;
            }
            yield return null;
            Assert.IsFalse(Popups.HasOpenPopup);
        }

        /// <summary>Capture, then one frame: the capture re-targets the canvases and their layout is only valid again
        /// on the next frame (raycasts in between miss).</summary>
        static IEnumerator Capture(string name)
        {
            TestUtil.Capture(name);
            yield return null;
        }

        static Vector2 RoomToStage(Vector2 room) => Ho.Zoom.RoomToStage(room);

        /// <summary>A stage point where a click hits <paramref name="item"/> itself (its opaque, uncovered pixels).</summary>
        internal static bool TryFindHitPoint(HiddenObjectItem item, out Vector2 point)
        {
            var c = item.Rect.parent == Ho.Zoom.transform ? RoomToStage(item.Rect.anchoredPosition * new Vector2(1, -1)) : item.Rect.anchoredPosition * new Vector2(1, -1);
            var size = item.Size;
            // every other pixel of the box, nearest to the centre first, so the point is well inside the object
            var candidates = new System.Collections.Generic.List<Vector2>();
            for (float dy = -size.y / 2 + 1; dy < size.y / 2 - 1; dy += 2)
                for (float dx = -size.x / 2 + 1; dx < size.x / 2 - 1; dx += 2)
                    candidates.Add(new Vector2(dx, dy));
            candidates.Sort((a, b) => a.sqrMagnitude.CompareTo(b.sqrMagnitude));
            var blockers = new System.Collections.Generic.HashSet<string>();
            foreach (var d in candidates)
            {
                var p = c + d;
                var hit = TestUtil.RaycastStage(p.x, p.y);
                if (hit == item.gameObject)
                {
                    point = p;
                    return true;
                }
                if (hit != null && blockers.Count < 8) blockers.Add(hit.name);
            }
            Debug.Log($"[Test] '{item.FileName}' at {c} size {size} parent {item.Rect.parent.name} covered by: {string.Join(", ", blockers)}; centre hit {TestUtil.RaycastStage(c.x, c.y)}; es {UnityEngine.EventSystems.EventSystem.current}; screen {TestUtil.StageToScreen(c.x, c.y)} {Screen.width}x{Screen.height}");
            point = default;
            return false;
        }

        /// <summary>Alpha of the object's sprite at stage point <paramref name="p"/> (room at x1).</summary>
        static float AlphaAt(HiddenObjectItem item, Vector2 p)
        {
            var s = item.Image.sprite;
            var loc = item.Location;
            int px = Mathf.FloorToInt(p.x - loc.x), py = Mathf.FloorToInt(p.y - loc.y);
            var r = s.textureRect;
            return s.texture.GetPixel((int)r.x + px, (int)(r.y + r.height) - 1 - py).a;
        }

        /// <summary>A stage point inside the object's box on a transparent pixel, where a click lands on the background.</summary>
        static bool TryFindTransparentPoint(HiddenObjectItem item, out Vector2 point)
        {
            var loc = item.Location;
            var size = item.Size;
            for (int y = 1; y < size.y - 1; y += 2)
                for (int x = 1; x < size.x - 1; x += 2)
                {
                    var p = new Vector2(loc.x + x + 0.5f, loc.y + y + 0.5f);
                    if (AlphaAt(item, p) > 0.01f) continue;
                    var hit = TestUtil.RaycastStage(p.x, p.y);
                    if (hit != null && hit.GetComponent<HiddenObjectBackground>() != null)
                    {
                        point = p;
                        return true;
                    }
                }
            point = default;
            return false;
        }

        static HiddenObjectItem DisplayedItem(int place) =>
            Ho.Items[Ho.Labels.First(l => l.aktif && !l.itemIsFound && l.placeNumber == place).idx];

        /// <summary>Clicks every shown, not found object until the list is done or <paramref name="count"/> objects are found.</summary>
        static IEnumerator FindItems(int count, float timeout = 60)
        {
            float end = Time.realtimeSinceStartup + timeout;
            int target = Mathf.Min(Ho.ItemFoundCount + count, Ho.ListedItems.Count);
            while (Ho.ItemFoundCount < target && Time.realtimeSinceStartup < end)
            {
                var shown = Ho.Labels.Where(l => l.aktif && !l.itemIsFound).ToList();
                foreach (var l in shown)
                {
                    if (Ho.ItemFoundCount >= target) break;
                    var item = Ho.Items[l.idx];
                    Assert.IsTrue(TryFindHitPoint(item, out var p), $"'{item.FileName}' cannot be clicked anywhere");
                    TestUtil.ClickStage(p.x, p.y);
                    Assert.IsTrue(item.Found, $"Clicking '{item.FileName}' at {p} did not find it");
                }
                yield return null;
            }
            Assert.AreEqual(target, Ho.ItemFoundCount, "Not every object could be found in time");
        }

        static IEnumerator WaitFor(System.Func<bool> condition, float timeout = 30)
        {
            float end = Time.realtimeSinceStartup + timeout;
            while (!condition() && Time.realtimeSinceStartup < end) yield return null;
            Assert.IsTrue(condition(), "Timed out");
        }

        static ResultPopup TopResult => Popups.Open.Count > 0 ? Popups.Open[Popups.Open.Count - 1] as ResultPopup : null;

        // ================================================================== rooms

        static readonly (int day, string scene, int listed, int goal)[] Rooms =
        {
            (3, "HO1_A", 21, 15), (17, "HO1_B", 24, 18), (31, "HO1_C", 27, 20),
            (45, "HO2_A", 30, 22), (59, "HO2_B", 33, 24), (73, "HO2_C", 33, 24), (87, "HO1_A", 33, 24),
        };

        [UnityTest]
        public IEnumerator EveryRoomLoadsWithItsObjects()
        {
            foreach (var room in Rooms)
            {
                yield return Load(room.day);
                var ho = Ho;
                Assert.AreEqual(room.scene, ho.SceneId, $"day {room.day}");
                var layout = GameDatabase.Instance.GetHiddenObjectLayout(room.scene);
                Assert.IsNotNull(layout);
                Assert.AreEqual(layout.items.Count, ho.Items.Count);
                for (int i = 0; i < layout.items.Count; i++)
                {
                    var item = ho.Items[i];
                    var entry = layout.items[i];
                    Assert.AreEqual(entry.name, item.FileName);
                    Assert.IsNotNull(item.Image.sprite, entry.name);
                    Assert.AreEqual(entry.name, item.Image.sprite.name);
                    // display pivot at the centre: x = locX + width / 2
                    var pos = item.Rect.anchoredPosition;
                    Assert.AreEqual(entry.locX + item.Size.x / 2, pos.x, 0.01f, entry.name);
                    Assert.AreEqual(-(entry.locY + item.Size.y / 2), pos.y, 0.01f, entry.name);
                    bool bg2 = entry.name.EndsWith(",bg2");
                    Assert.AreEqual(!bg2, item.Image.raycastTarget, entry.name);
                    if (entry.name.EndsWith(",nm")) Assert.AreEqual(HiddenObjectItem.TypeObject, item.ItemType);
                }
                Assert.AreEqual(room.listed, ho.ListedItems.Count);
                Assert.AreEqual(room.listed, ho.ListedItems.Distinct().Count());
                Assert.IsTrue(ho.ListedItems.All(i => ho.Items[i].ItemType == HiddenObjectItem.TypeObject && ho.Items[i].Listed));
                Assert.AreEqual(room.goal, ho.GoalTarget);
                Assert.AreEqual(room.listed, ho.ExpertTarget);
                Assert.AreEqual(120, ho.TotalTime, 0.001f);
                Assert.AreEqual(room.scene + "_BG", Object.FindFirstObjectByType<HiddenObjectBackground>().GetComponent<UnityEngine.UI.Image>().sprite.name);
                // the first three objects are shown at 330 / 530 / 730, y 50
                for (int place = 0; place < 3; place++)
                {
                    var l = ho.Labels[place];
                    Assert.IsTrue(l.aktif);
                    Assert.AreEqual(place, l.placeNumber);
                    Assert.AreEqual(new Vector2(330 + place * 200, 50), l.position);
                    Assert.AreEqual(HiddenObjectController.FormatLabel(ho.Items[l.idx].ItemName), l.english);
                }
                Assert.IsTrue(ho.Labels.Skip(3).All(l => !l.aktif && l.position == new Vector2(220, -300)));
                Assert.AreEqual(string.Format("Goal: 0/{0}", room.goal), ho.MoneyText.text);
                // every story HO day shows "Let's tidy up this room."
                Assert.IsTrue(Popups.HasOpenPopup);
                if (room.day == 45) TestUtil.Capture("HO_HO2A_Window");
                yield return CloseWindows();
                yield return Capture($"HO_Room_{room.scene}_day{room.day + 1}");
            }
        }

        [Test]
        public void LabelFormatting()
        {
            Assert.AreEqual("Alarm Clock", HiddenObjectController.FormatLabel("alarm clock"));
            Assert.AreEqual("Rubik's Cube", HiddenObjectController.FormatLabel("rubik_QUOTE_s cube"));
            Assert.AreEqual("Salt and Pepper", HiddenObjectController.FormatLabel("salt and pepper"));
            Assert.AreEqual("Cup of Tea", HiddenObjectController.FormatLabel("cup of tea"));
        }

        // ================================================================== clicks

        [UnityTest]
        public IEnumerator OpaquePixelFindsTransparentPixelMisses()
        {
            yield return Load(45);
            yield return CloseWindows();
            var ho = Ho;
            // a shown object with a transparent pixel over the background inside its box
            HiddenObjectItem item = null;
            Vector2 transparent = default;
            for (int place = 0; place < 3 && item == null; place++)
            {
                var candidate = DisplayedItem(place);
                if (TryFindTransparentPoint(candidate, out transparent)) item = candidate;
            }
            Assert.IsNotNull(item, "No shown object has a transparent pixel over the background");
            Assert.IsTrue(TryFindHitPoint(item, out var opaque), "Object has no clickable pixel");
            Assert.Greater(AlphaAt(item, opaque), 0.5f);

            var hit = TestUtil.ClickStage(transparent.x, transparent.y);
            Assert.IsNotNull(hit.GetComponent<HiddenObjectBackground>());
            yield return null;
            Assert.IsFalse(item.Found);
            Assert.AreEqual(0, ho.ItemFoundCount);

            // an object that is not on the list: a miss as well
            var unlisted = ho.Items.First(i => i.ItemType == HiddenObjectItem.TypeObject && !i.Listed && TryFindHitPoint(i, out _));
            TryFindHitPoint(unlisted, out var up);
            TestUtil.ClickStage(up.x, up.y);
            Assert.IsFalse(unlisted.Found);
            Assert.AreEqual(0, ho.ItemFoundCount);

            int bonusBefore = ho.BonusMoney;
            TestUtil.ClickStage(opaque.x, opaque.y);
            Assert.IsTrue(item.Found);
            Assert.AreEqual(1, ho.ItemFoundCount);
            Assert.AreEqual(bonusBefore + 5 * (3 + 1) + 5, ho.BonusMoney); // HO2_A is room 3
            Assert.AreEqual("Goal: 1/22", ho.MoneyText.text);
            Assert.AreEqual(1, Player.achievementCounter[AchievementList.HawkEye]);
            Assert.AreEqual(ho.Zoom.transform.parent, item.Rect.parent.parent, "found objects move to the stage layer");
            yield return TestUtil.Seconds(0.15f);
            yield return Capture("HO_Found_Pop");
            yield return TestUtil.Seconds(0.6f);
            yield return Capture("HO_Found_Flight");
            yield return WaitFor(() => ho.TotalItems == 29, 5);
            Assert.AreEqual(-200, item.Rect.anchoredPosition.x, 0.01f);
            // the free place is refilled with the 4th label
            Assert.IsTrue(ho.Labels[3].aktif);
            Assert.AreEqual(Ho.Labels.First(l => l.idx == item.Index).placeNumber, ho.Labels[3].placeNumber);
            yield return Capture("HO_Found_Refilled");
        }

        [UnityTest]
        public IEnumerator DraggingCancelsTheClick()
        {
            yield return Load(45);
            yield return CloseWindows();
            var item = DisplayedItem(0);
            Assert.IsTrue(TryFindHitPoint(item, out var p));
            // press on the object, move 30 px (touchDragged), release: not found
            var zoom = Ho.Zoom.gameObject;
            TestUtil.Press(item.gameObject, p);
            Assert.IsTrue(item.Found, "a plain press/release finds it");
            yield return Load(45);
            yield return CloseWindows();
            item = DisplayedItem(0);
            Assert.IsTrue(TryFindHitPoint(item, out p));
            var data = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current)
            {
                position = TestUtil.StageToScreen(p.x, p.y),
            };
            UnityEngine.EventSystems.ExecuteEvents.Execute(item.gameObject, data, UnityEngine.EventSystems.ExecuteEvents.pointerDownHandler);
            UnityEngine.EventSystems.ExecuteEvents.Execute(Ho.Zoom.gameObject, data, UnityEngine.EventSystems.ExecuteEvents.initializePotentialDrag);
            data.position = TestUtil.StageToScreen(p.x + 30, p.y);
            UnityEngine.EventSystems.ExecuteEvents.Execute(Ho.Zoom.gameObject, data, UnityEngine.EventSystems.ExecuteEvents.dragHandler);
            Assert.IsTrue(Ho.Zoom.TouchDragged);
            // at x1 the room cannot be moved
            Assert.AreEqual(0, Ho.Zoom.X, 0.001f);
            UnityEngine.EventSystems.ExecuteEvents.Execute(item.gameObject, data, UnityEngine.EventSystems.ExecuteEvents.pointerUpHandler);
            Assert.IsFalse(item.Found);
        }

        // ================================================================== hint

        [UnityTest]
        public IEnumerator HintRevealsAShownObjectAndRefills()
        {
            yield return Load(45);
            yield return CloseWindows();
            var ho = Ho;
            Assert.IsTrue(ho.HasHint);
            Assert.IsTrue(ho.HintButton.gameObject.activeSelf);
            var hit = TestUtil.ClickStage(930, 60);
            Assert.AreEqual(ho.HintButton.gameObject, hit, "the HINT! button covers the jug");
            Assert.IsFalse(ho.HasHint);
            Assert.IsFalse(ho.HintButton.gameObject.activeSelf);
            var shown = ho.Labels.Where(l => l.aktif && !l.itemIsFound).Select(l => l.idx).ToList();
            CollectionAssert.Contains(shown, ho.RevealedIndex);
            Assert.IsTrue(ho.HintParticles.IsEmitting);
            yield return TestUtil.Seconds(0.4f);
            yield return Capture("HO_Hint_Path");
            yield return WaitFor(() => ho.HintCircling, 5);
            yield return TestUtil.Seconds(0.5f);
            yield return Capture("HO_Hint_Circle");
            // the fire circles the object twice (100 frames) and stops
            yield return WaitFor(() => !ho.HintCircling, 6);
            Assert.IsFalse(ho.HintParticles.IsEmitting);
            // a second click does nothing while the jug refills (15 s)
            Assert.Greater(ho.HintTimer, 0);
            ho.RevealItem();
            Assert.IsFalse(ho.HasHint);
            Time.timeScale = 10;
            yield return WaitFor(() => ho.HasHint, 5);
            Time.timeScale = 1;
            Assert.IsTrue(ho.HintButton.gameObject.activeSelf);
            Assert.GreaterOrEqual(ho.HintTimer, HiddenObjectController.HintRecoverTime);
        }

        [UnityTest]
        public IEnumerator FindingTheRevealedObjectStopsTheFire()
        {
            yield return Load(45);
            yield return CloseWindows();
            var ho = Ho;
            ho.RevealItem();
            var item = ho.Items[ho.RevealedIndex];
            Assert.IsTrue(TryFindHitPoint(item, out var p));
            TestUtil.ClickStage(p.x, p.y);
            Assert.IsTrue(item.Found);
            Assert.IsFalse(ho.HintParticles.IsEmitting);
            Assert.IsFalse(ho.HintCircling);
        }

        // ================================================================== pause, sound

        [UnityTest]
        public IEnumerator PauseStopsTheTimer()
        {
            yield return Load(45);
            yield return CloseWindows();
            var ho = Ho;
            yield return TestUtil.Seconds(0.2f);
            Assert.Less(ho.TimeLeft, 120);
            TestUtil.Click("PauseButton");
            yield return null;
            Assert.IsTrue(ho.IsPaused);
            var pause = Popups.Open.Last() as PausePopup;
            Assert.IsNotNull(pause);
            Assert.AreEqual("Let's tidy up this room.", pause.info.text);
            yield return Capture("HO_Pause");
            float t = ho.TimeLeft;
            yield return TestUtil.Seconds(0.2f);
            Assert.AreEqual(t, ho.TimeLeft);
            TestUtil.Click("SoundButton"); // the pause window's toggle
            Assert.IsTrue(TestUtil.Find("MuteIcon").activeSelf);
            TestUtil.Click("SoundButton");
            TestUtil.ClickLabel("Resume");
            yield return null;
            Assert.IsFalse(ho.IsPaused);
            Assert.IsFalse(Popups.HasOpenPopup);
            // HUD toggles
            TestUtil.Click("MusicButton");
            Assert.IsTrue(GameManager.Instance.Audio.MuteMusic);
            Assert.IsTrue(TestUtil.Find("MuteMusicIcon").activeSelf);
            TestUtil.Click("MusicButton");
            Assert.IsNull(TestUtil.Find("MuteMusicIcon"));
        }

        // ================================================================== results

        [UnityTest]
        public IEnumerator FindingEverythingCompletesTheDayAndSaves()
        {
            yield return Load(45);
            yield return CloseWindows();
            var ho = Ho;
            int money = Player.money;
            Time.timeScale = 4;
            yield return FindItems(30);
            yield return WaitFor(() => ho.TotalItems == 0 && Popups.HasOpenPopup, 20);
            Time.timeScale = 1;
            var w = TopResult;
            Assert.IsNotNull(w, "no summary");
            Assert.AreEqual("Summary", w.title.text);
            Assert.AreEqual(46, Player.passedDays);
            Assert.AreEqual(30, Player.daysEarning[45]);
            Assert.IsTrue(Player.goalAchieved[45]);
            Assert.AreEqual(money + 6, Player.money);
            Assert.AreEqual(6, Session.coinsGot);
            Assert.AreEqual(30 * 25, ho.BonusMoney);
            Assert.AreEqual(1, Player.achievementCounter[AchievementList.QualifiedChef]);
            Assert.AreEqual(1, Player.achievementCounter[AchievementList.InternationalChef]);
            Assert.AreEqual(30, Player.achievementCounter[AchievementList.HawkEye]);
            Assert.AreEqual(750, Player.achievementCounter[AchievementList.BigBusiness]);
            Assert.AreEqual("Exp.: 30/30", ho.MoneyText.text);
            Assert.AreEqual(string.Format("30\n$750\n\n{0} (+6)", money + 6), w.summaryStats.text);
            Assert.IsTrue(w.Expert);
            yield return TestUtil.Seconds(0.5f);
            yield return Capture("HO_Result_Expert");

            // saved
            var saves = new Save.SaveSystem(saveFolder);
            saves.LoadAll();
            var saved = saves.GetSlot(0);
            Assert.AreEqual(46, saved.passedDays);
            Assert.AreEqual(30, saved.daysEarning[45]);
            Assert.AreEqual(money + 6, saved.money);

            TestUtil.ClickLabel("Kitchen");
            yield return TestUtil.ExpectSceneRequest(GameScene.Kitchen);
        }

        [UnityTest]
        public IEnumerator TimeRunningOutFailsTheDay()
        {
            yield return Load(45);
            yield return CloseWindows();
            var ho = Ho;
            Time.timeScale = 4;
            yield return FindItems(2);
            Time.timeScale = 1;
            ho.TimeLeft = 0.01f;
            yield return WaitFor(() => ho.IsGameOver && Popups.HasOpenPopup, 5);
            var w = TopResult;
            Assert.IsNotNull(w);
            Assert.AreEqual("Level Failed", w.title.text);
            Assert.AreEqual(45, Player.passedDays);
            Assert.AreEqual(0, Player.daysEarning[45]);
            Assert.AreEqual(0, Session.coinsGot);
            Assert.AreEqual(2 * 25, Player.achievementCounter[AchievementList.BigBusiness], "the score still counts for Big Business");
            Assert.AreEqual(0, Player.achievementCounter[AchievementList.QualifiedChef]);
            var saves = new Save.SaveSystem(saveFolder);
            saves.LoadAll();
            Assert.AreEqual(50, saves.GetSlot(0).achievementCounter[AchievementList.BigBusiness]);
            yield return TestUtil.Seconds(0.3f);
            yield return Capture("HO_Result_Failed");
        }

        [UnityTest]
        public IEnumerator ReachingTheGoalBeforeTheTimeRunsOut()
        {
            yield return Load(17); // HO1_B: goal 18, expert 24 (day 3 always forces the LEVEL_4 tutorial)
            yield return CloseWindows();
            var ho = Ho;
            Time.timeScale = 4;
            yield return FindItems(17);
            Time.timeScale = 1;
            Assert.AreEqual("Goal: 17/18", ho.MoneyText.text);
            Assert.IsFalse(Player.goalAchieved[17]);
            yield return FindItems(1);
            Assert.IsTrue(Player.goalAchieved[17]);
            Assert.AreEqual("Exp.: 18/24", ho.MoneyText.text);
            Assert.IsNotNull(Object.FindFirstObjectByType<GoalIconSmall>(), "goal badge in the HUD");
            yield return TestUtil.Seconds(0.3f);
            yield return Capture("HO_GoalReached");
            yield return TestUtil.Seconds(1.5f);
            ho.TimeLeft = 0;
            yield return WaitFor(() => Popups.HasOpenPopup, 5);
            var w = TopResult;
            Assert.AreEqual("Summary", w.title.text);
            Assert.IsFalse(w.Expert);
            Assert.AreEqual(18, Player.passedDays);
            Assert.AreEqual(18, Player.daysEarning[17]);
            Assert.AreEqual(3, Session.coinsGot);
            Assert.AreEqual(0, Player.achievementCounter[AchievementList.InternationalChef]);
            Assert.AreEqual(1, Player.achievementCounter[AchievementList.QualifiedChef]);
            yield return Capture("HO_Result_Goal");
        }

        // ================================================================== extras

        [UnityTest]
        public IEnumerator ExtrasModeScoresAndNextLevelKeepsTheScore()
        {
            yield return Load(10, extras: true);
            var ho = Ho;
            Assert.IsFalse(Popups.HasOpenPopup, "no day windows in extras mode");
            Assert.AreEqual("HO1_A", ho.SceneId);
            Assert.AreEqual(0, ho.CurrentLevel);
            Assert.AreEqual(120, ho.TotalTime, 0.001f);
            Assert.AreEqual(21, ho.ListedItems.Count);
            Assert.AreEqual("$ 0", ho.MoneyText.text);
            Time.timeScale = 4;
            yield return FindItems(1);
            Assert.AreEqual("$ 10", ho.MoneyText.text);
            Assert.AreEqual(0, Player.achievementCounter[AchievementList.HawkEye], "no Hawk Eye in extras");
            yield return FindItems(20);
            yield return WaitFor(() => ho.TotalItems == 0 && Popups.HasOpenPopup, 20);
            Time.timeScale = 1;
            var w = TopResult;
            Assert.AreEqual("Summary", w.title.text);
            Assert.AreEqual("21\n210", w.summaryStats.text);
            Assert.AreEqual(10, Player.passedDays, "extras don't change the story");
            yield return Capture("HO_Extras_Complete");

            TestUtil.ClickLabel("Next Level");
            yield return TestUtil.ExpectSceneRequest(GameScene.HiddenObject);
            yield return WaitFor(() => !GameManager.Instance.Flow.IsTransitioning, 5);
            ho = Ho;
            Assert.IsTrue(ho.IsReady);
            // the original reset currentLevel to 0 in extras mode, so "Next Level" replays the first room
            Assert.AreEqual(0, ho.CurrentLevel);
            Assert.AreEqual("HO1_A", ho.SceneId);
            Assert.AreEqual(210, ho.BonusMoney, "the score carries over");
            Assert.AreEqual("$ 210", ho.MoneyText.text);
            Assert.AreEqual(0, ho.ItemFoundCount);

            ho.TimeLeft = 0;
            yield return WaitFor(() => Popups.HasOpenPopup, 5);
            Assert.AreEqual("Level Failed!", TopResult.title.text);
            yield return Capture("HO_Extras_Failed");
            TestUtil.ClickLabel("Main Menu");
            yield return TestUtil.ExpectSceneRequest(GameScene.MainMenu);
            Assert.AreEqual(210, Session.hoBonusMoney);
        }

        // ================================================================== tutorial

        [UnityTest]
        public IEnumerator Level4TutorialPointsAtTheFirstObject()
        {
            yield return Load(3, skipTutorials: false);
            var ho = Ho;
            var tut = TutorialManager.Current;
            Assert.AreEqual(TutorialIds.LEVEL_4, tut.CurrentDialogueId);
            Assert.AreEqual(0, tut.DialogueCounter);
            Assert.IsTrue(Player.tutorialShowed[TutorialIds.LEVEL_4]);
            Assert.IsTrue(Popups.HasOpenPopup, "the day window is shown over the tutorial");
            yield return CloseWindows();
            Assert.IsTrue(tut.IsButtonShowing);
            // the timer waits for the Next button
            yield return TestUtil.Seconds(0.2f);
            Assert.AreEqual(ho.TotalTime, ho.TimeLeft);
            yield return Capture("HO_Tutorial_Line0");

            // clicks on objects are ignored before line 4
            var first = ho.Items[ho.ListedItems[0]];
            Assert.IsTrue(TryFindHitPoint(first, out var p));
            TestUtil.ClickStage(p.x, p.y);
            Assert.IsFalse(first.Found);

            for (int i = 0; i < 4; i++)
            {
                TestUtil.Click("NextButton");
                yield return null;
            }
            Assert.AreEqual(4, tut.DialogueCounter);
            Assert.IsFalse(tut.IsButtonShowing);
            // HOITEMX / HOITEMY: the arrow points at the first listed object
            var arrow = TestUtil.Find<RectTransform>("Arrow");
            var loc = first.Location;
            float ax = loc.x - 40 + (loc.x - 40 > 512 ? 0 : 100);
            Assert.AreEqual(ax, arrow.anchoredPosition.x, 41f);
            Assert.AreEqual(-(int)(loc.y + first.Size.y / 2), arrow.anchoredPosition.y, 0.5f);
            yield return Capture("HO_Tutorial_Line4");

            Assert.IsTrue(TryFindHitPoint(first, out p));
            TestUtil.ClickStage(p.x, p.y);
            Assert.IsTrue(first.Found);
            Assert.AreEqual(5, tut.DialogueCounter);
            yield return Capture("HO_Tutorial_Line5");
            TestUtil.Click("HintButton");
            Assert.IsFalse(ho.HasHint);
            Assert.IsFalse(tut.IsShowing, "used_hint hides the bubble");
            Assert.AreEqual(TutorialIds.LEVEL_4, tut.CurrentDialogueId);
            var revealed = ho.Items[ho.RevealedIndex];
            Assert.IsTrue(TryFindHitPoint(revealed, out p));
            TestUtil.ClickStage(p.x, p.y);
            Assert.AreEqual(6, tut.DialogueCounter);
            Assert.IsTrue(tut.IsButtonShowing);
        }
    }
}
