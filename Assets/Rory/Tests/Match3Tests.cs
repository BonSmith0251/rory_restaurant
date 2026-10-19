using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rory.Data;
using Rory.Popups;
using Rory.Popups.Game;
using Rory.Save;
using Rory.Screens.Match3;
using Rory.Tutorials;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Rory.Tests
{
    /// <summary>
    /// Match-3 restaurant: the board model on its own (seeded, stepped by hand) and the Match3 scene (day start, serving,
    /// day end, pause, tutorials, captures).
    /// </summary>
    public class Match3Tests
    {
        // ================================================================== board model (no scene)

        class FakeHost : IMatch3BoardHost
        {
            public readonly List<string> sounds = new List<string>();
            public readonly List<(BoardEffectKind, float, float)> effects = new List<(BoardEffectKind, float, float)>();
            public readonly List<Vector2Int> collected = new List<Vector2Int>();
            public readonly List<Vector2Int> highlights = new List<Vector2Int>();
            public readonly List<int> goodMoves = new List<int>();
            public readonly List<BoardTile> cookiesGiven = new List<BoardTile>();
            public int matched, torches, cookiesUsed, noMoves, touchBegan;
            public bool blackScreen;
            public Match3Board board;

            public void PlaySfx(string name) => sounds.Add(name);
            public void PlayEffect(BoardEffectKind kind, float x, float y) => effects.Add((kind, x, y));
            public bool TouchBlockedByTutorial => false;
            public bool DragBlockedByTutorial(int lastCol, int lastRow, int col, int row, int lastTileType) => false;
            public bool MatchMustFreeFrozenTile => false;
            public bool HintOnlyBreadAndLettuce => false;
            public void OnHighlightDispatched(int col, int row) => highlights.Add(new Vector2Int(col, row));
            public void OnMatched() => matched++;
            public void OnStartTorch() => torches++;
            public void OnTouchBegan() => touchBegan++;
            public void OnTileCollected(int col, int row) => collected.Add(new Vector2Int(col, row));
            public void AfterTilesCollected() { }
            public void CheckCustomerOrders(int col, int row) { }
            public int GetWantedIngredient() => 0;
            public void OnCookieUsed() => cookiesUsed++;
            public void GiveCookies(BoardTile tile) => cookiesGiven.Add(tile);
            public void OnGoodMove(int length) => goodMoves.Add(length);
            public void ShowNoPossibleMoves()
            {
                noMoves++;
                board.OnNoMovesTextArrived();
            }
            public void SetBlackScreen(bool visible) => blackScreen = visible;
        }

        static readonly int[] BaseTypes = { 1, 9, 7, 2 };
        static readonly List<int> Western = new List<int> { 1, 9, 7, 2, 4, 3 };

        static Match3Board NewBoard(out FakeHost host, int seed = 7, int playedDay = 1)
        {
            host = new FakeHost();
            var b = new Match3Board(host, new Match3Rng(seed)) { AvailableTypes = new List<int>(Western), PlayedDay = playedDay };
            host.board = b;
            return b;
        }

        /// <summary>A board without any match: type(c, r) = BaseTypes[(c + 2r) % 4].</summary>
        static void FillNoMatch(Match3Board b)
        {
            for (int c = 0; c < 8; c++)
                for (int r = 0; r < 8; r++)
                    b.Tiles[c][r].type = BaseTypes[(c + 2 * r) % 4];
        }

        /// <summary>Steps the board like the screen (tweens by time, one Tick per 1/30 s) for <paramref name="seconds"/>.</summary>
        static void Run(Match3Board b, float seconds)
        {
            int frames = Mathf.CeilToInt(seconds * 30);
            for (int i = 0; i < frames; i++)
            {
                b.StepTweens(Match3Controller.FrameTime);
                b.Tick();
            }
        }

        static void RunUntilStable(Match3Board b, float maxSeconds = 20)
        {
            for (int i = 0; i < maxSeconds * 30; i++)
            {
                b.StepTweens(Match3Controller.FrameTime);
                b.Tick();
                if (!b.moveTile && !b.isSwapping && b.Tweens.Count == 0 && b.AllTiles.All(t => !t.removed && !t.onTween)) return;
            }
            Assert.Fail("Board did not settle");
        }

        static int[] Types(Match3Board b)
        {
            var a = new int[64];
            for (int c = 0; c < 8; c++)
                for (int r = 0; r < 8; r++)
                    a[c * 8 + r] = b.Tiles[c][r].type;
            return a;
        }

        static void AssertSettled(Match3Board b)
        {
            for (int c = 0; c < 8; c++)
                for (int r = 0; r < 8; r++)
                {
                    var t = b.Tiles[c][r];
                    Assert.AreEqual(c, t.col);
                    Assert.AreEqual(r, t.row);
                    Assert.AreEqual(c * 8 + r, t.idx);
                    Assert.AreEqual(Match3Board.CellX(c), t.x, 0.01f);
                    Assert.AreEqual(Match3Board.CellY(r), t.y, 0.01f);
                    Assert.IsFalse(t.removed || t.isDropping || t.onTween || t.isSwapping, t.ToString());
                    Assert.AreEqual(1, t.scale, 0.001f);
                }
            Assert.AreEqual(0, b.LookForMatches().Count, "matches left after the cascade");
        }

        [Test]
        public void BoardGenerationIsSeededAndPlayable()
        {
            var a = NewBoard(out _, 123);
            a.Generate();
            var b = NewBoard(out _, 123);
            b.Generate();
            var c = NewBoard(out _, 124);
            c.Generate();
            Assert.AreEqual(Types(a), Types(b), "same seed, same board");
            Assert.AreNotEqual(Types(a), Types(c), "another seed, another board");
            Assert.AreEqual(0, a.LookForMatches().Count);
            Assert.IsTrue(a.LookForPossibles());
            Assert.IsTrue(Types(a).All(t => Western.Contains(t)));
            for (int col = 0; col < 8; col++)
                for (int row = 0; row < 8; row++)
                {
                    Assert.AreEqual(col * 68 + 494, a.Tiles[col][row].x);
                    Assert.AreEqual(row * 68 + 73.5f, a.Tiles[col][row].y);
                }
        }

        [Test]
        public void SwapMatchRemovesRefillsAndCascades()
        {
            var b = NewBoard(out var host);
            FillNoMatch(b);
            // row 3: chicken at columns 0, 1 and 3; swapping (2,3) and (3,3) lines them up
            b.Tiles[0][3].type = b.Tiles[1][3].type = b.Tiles[3][3].type = 4;
            Assert.AreEqual(0, b.LookForMatches().Count);
            b.lastCol = 2;
            b.lastRow = 3;
            var a = b.Tiles[2][3];
            var t = b.Tiles[3][3];
            b.Touch(BoardTouch.Began, Match3Board.CellX(2), Match3Board.CellY(3));
            Assert.AreEqual(1, host.touchBegan);
            b.Touch(BoardTouch.Moved, Match3Board.CellX(3), Match3Board.CellY(3));
            Assert.IsTrue(b.isSwapping, "drag to the right neighbour swaps");
            Assert.AreSame(t, b.Tiles[2][3]);
            Assert.AreSame(a, b.Tiles[3][3]);
            Run(b, 0.25f);
            Assert.AreEqual(1, host.matched);
            Assert.AreEqual(3, host.collected.Count, "three pieces collected");
            CollectionAssert.AreEquivalent(new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) }, host.collected,
                "the matched pieces bubbled up to the top of their columns");
            Assert.AreEqual(3, host.effects.Count(e => e.Item1 == BoardEffectKind.Star), "chicken plays the star effect");
            Assert.IsTrue(host.sounds.Contains("chicken_sizzle"));
            RunUntilStable(b);
            AssertSettled(b);
        }

        [Test]
        public void CascadeIsDeterministicWithASeed()
        {
            int[] Play(int seed)
            {
                var b = NewBoard(out _, seed);
                b.Generate();
                // perform every possible move one after the other, letting the board settle in between
                for (int n = 0; n < 6; n++)
                {
                    var move = FindMove(b);
                    if (move == null) break;
                    var (from, to) = move.Value;
                    b.lastCol = from.x;
                    b.lastRow = from.y;
                    b.Touch(BoardTouch.Began, Match3Board.CellX(from.x), Match3Board.CellY(from.y));
                    b.Touch(BoardTouch.Moved, Match3Board.CellX(to.x), Match3Board.CellY(to.y));
                    b.Touch(BoardTouch.Ended, Match3Board.CellX(to.x), Match3Board.CellY(to.y));
                    RunUntilStable(b);
                    AssertSettled(b);
                }
                return Types(b);
            }
            Assert.AreEqual(Play(99), Play(99));
        }

        [Test]
        public void SwapWithoutMatchSwapsBack()
        {
            var b = NewBoard(out var host);
            FillNoMatch(b);
            var before = Types(b);
            b.lastCol = 4;
            b.lastRow = 4;
            b.Touch(BoardTouch.Began, Match3Board.CellX(4), Match3Board.CellY(4));
            b.Touch(BoardTouch.Moved, Match3Board.CellX(4), Match3Board.CellY(5));
            Run(b, 0.25f);
            Assert.IsTrue(b.swapBack);
            Assert.IsTrue(b.isSwapping, "swapping back");
            Run(b, 0.25f);
            Assert.IsFalse(b.isSwapping);
            Assert.AreEqual(before, Types(b));
            Assert.AreEqual(0, host.matched);
            Assert.AreEqual(Match3Board.CellY(5), b.Tiles[4][5].y, 0.01f);
        }

        [Test]
        public void FrozenTileThawsInsteadOfBreaking()
        {
            var b = NewBoard(out var host, playedDay: 30);
            b.maxFrozenTile = 0; // no new ice from the refill
            FillNoMatch(b);
            b.Tiles[0][3].type = b.Tiles[1][3].type = b.Tiles[3][3].type = 4;
            b.FreezeTile(0, 3);
            Assert.IsTrue(b.Tiles[0][3].frozen);
            Assert.IsTrue(b.Frozen[0].visible);
            Assert.AreEqual(1, b.currentFrozenTile);
            b.SwapTile(b.Tiles[2][3], b.Tiles[3][3]);
            Run(b, 0.25f);
            var thawed = b.AllTiles.First(t => t.type == 4 && !t.removed && t.col == 0);
            Assert.IsFalse(thawed.frozen);
            Assert.IsFalse(b.Frozen[0].visible);
            Assert.AreEqual(0, b.currentFrozenTile);
            Assert.AreEqual(2, host.collected.Count, "the frozen piece stays on the board");
            Assert.IsTrue(host.sounds.Any(s => s.StartsWith("ice_break")));
            RunUntilStable(b);
            AssertSettled(b);
        }

        [Test]
        public void FrozenTileCannotBeSwappedAndShakes()
        {
            var b = NewBoard(out var host);
            FillNoMatch(b);
            b.FreezeTile(5, 5);
            b.lastCol = 4;
            b.lastRow = 5;
            b.Touch(BoardTouch.Began, Match3Board.CellX(4), Match3Board.CellY(5));
            b.Touch(BoardTouch.Moved, Match3Board.CellX(5), Match3Board.CellY(5));
            Assert.IsFalse(b.isSwapping);
            Assert.IsTrue(host.sounds.Contains("ice_break"));
            Assert.IsTrue(b.Tweens.IsTweening(b.Frozen[0], BoardTweenProps.Position), "the ice shakes");
            Run(b, 0.5f);
            // (sic) the original's shake ends 3 px left of the tile
            Assert.AreEqual(Match3Board.CellX(5) - 3, b.Frozen[0].x, 0.01f);
        }

        [Test]
        public void TorchClearsItsRow()
        {
            var b = NewBoard(out var host, playedDay: 27);
            b.maxFrozenTile = 0;
            FillNoMatch(b);
            b.SetType(3, 4, TileTypes.Torch);
            Assert.IsTrue(b.LookForPossibles(), "a torch is always a possible move");
            b.SwapTile(b.Tiles[3][4], b.Tiles[3][5]);
            Run(b, 0.25f);
            Assert.AreEqual(1, host.torches);
            Assert.IsTrue(host.sounds.Contains("power_up"));
            Assert.AreEqual(8, host.collected.Count, "the whole row is collected");
            Assert.AreEqual(16, host.effects.Count, "fire and smoke on every piece");
            Assert.AreEqual(8, b.removedTileTorch, "torch counter from day 26");
            RunUntilStable(b);
            AssertSettled(b);
        }

        [Test]
        public void CookiesDoNotMatchAndAreGivenOnTap()
        {
            var b = NewBoard(out var host);
            FillNoMatch(b);
            for (int c = 2; c < 5; c++) b.SetType(c, 6, TileTypes.Cookie);
            Assert.AreEqual(3, b.cookiesCountOnBoard);
            Assert.AreEqual(0, b.LookForMatches().Count, "cookies never match");
            Assert.AreEqual(0, b.GetMatches(b.Tiles[3][6]).Count);
            b.lastCol = 3;
            b.lastRow = 6;
            b.Touch(BoardTouch.Began, Match3Board.CellX(3), Match3Board.CellY(6));
            b.Touch(BoardTouch.Ended, Match3Board.CellX(3), Match3Board.CellY(6));
            Assert.AreEqual(1, host.cookiesUsed);
            Assert.AreSame(b.Tiles[3][6], host.cookiesGiven.Single());
        }

        [Test]
        public void LongMatchesReportGoodMoves()
        {
            var b = NewBoard(out var host);
            FillNoMatch(b);
            // a line of four lettuce in row 2 after one swap: (0..2, 2) + (4, 2), swap (3,2) <-> (4,2)
            foreach (int c in new[] { 0, 1, 2, 4 }) b.Tiles[c][2].type = 3;
            b.SwapTile(b.Tiles[3][2], b.Tiles[4][2]);
            Run(b, 0.25f);
            CollectionAssert.AreEqual(new[] { 4 }, host.goodMoves);
            RunUntilStable(b);

            // an L of five: column 6 rows 0..2 and row 2 columns 4..6 after the swap
            var b2 = NewBoard(out var host2);
            FillNoMatch(b2);
            foreach (var p in new[] { (6, 0), (6, 1), (4, 2), (5, 2), (7, 3) }) b2.Tiles[p.Item1][p.Item2].type = 3;
            b2.SwapTile(b2.Tiles[6][2], b2.Tiles[7][2]);
            // (7,3) is not in line: move (7,3) into (7,2)'s place first
            b2 = NewBoard(out host2);
            FillNoMatch(b2);
            foreach (var p in new[] { (6, 0), (6, 1), (4, 2), (5, 2), (7, 2) }) b2.Tiles[p.Item1][p.Item2].type = 3;
            Assert.AreEqual(0, b2.LookForMatches().Count);
            b2.SwapTile(b2.Tiles[6][2], b2.Tiles[7][2]);
            Run(b2, 0.25f);
            Assert.AreEqual(1, host2.goodMoves.Count);
            Assert.AreEqual(5, host2.goodMoves[0], "an L / T of 3 + 3 counts as five");
            RunUntilStable(b2);
            AssertSettled(b2);
        }

        [Test]
        public void HintPulsesAMoveAndReportsItsTiles()
        {
            var b = NewBoard(out var host);
            FillNoMatch(b);
            b.Tiles[0][3].type = b.Tiles[1][3].type = b.Tiles[3][3].type = 4;
            b.dispatchHighlightPosition = 3;
            b.HighlightPossible();
            Assert.AreEqual(3, host.highlights.Count);
            CollectionAssert.AreEquivalent(new[] { new Vector2Int(0, 3), new Vector2Int(1, 3), new Vector2Int(3, 3) }, host.highlights);
            Run(b, 0.4f);
            Assert.AreEqual(1.2f, b.Tiles[0][3].scale, 0.02f);
            Run(b, 0.45f);
            Assert.AreEqual(1f, b.Tiles[0][3].scale, 0.05f);
            // a touch stops the pulse
            b.Touch(BoardTouch.Began, Match3Board.CellX(6), Match3Board.CellY(6));
            Assert.IsFalse(b.Tweens.IsTweening(b.Tiles[0][3], BoardTweenProps.Scale));
            Assert.AreEqual(1, b.Tiles[0][3].scale);
        }

        [Test]
        public void IdleHintAfterTenSeconds()
        {
            var b = NewBoard(out var host);
            b.Generate();
            for (int i = 0; i < Match3Board.IdleFramesForHint; i++) b.TickIdle();
            Assert.IsFalse(b.highlightPossibleTile);
            b.TickIdle();
            Assert.IsTrue(b.highlightPossibleTile, "frame 301 shows the hint");
            Assert.IsTrue(b.AllTiles.Any(t => b.Tweens.IsTweening(t, BoardTweenProps.Scale)));
        }

        [Test]
        public void NoPossibleMovesReshufflesTheBoard()
        {
            var b = NewBoard(out var host);
            // 2 x 2 blocks of four types: no line and no move anywhere
            int[] types = { 1, 9, 7, 2 };
            for (int c = 0; c < 8; c++)
                for (int r = 0; r < 8; r++)
                    b.Tiles[c][r].type = types[(c % 2) * 2 + (r % 2)];
            Assert.AreEqual(0, b.LookForMatches().Count);
            Assert.IsFalse(b.LookForPossibles());
            b.FindAndRemoveOneMatch();
            Assert.AreEqual(1, host.noMoves);
            Assert.IsTrue(b.noMoreMoves);
            Assert.IsTrue(host.blackScreen);
            RunUntilStable(b, 10);
            Assert.IsFalse(b.noMoreMoves);
            Assert.IsFalse(host.blackScreen);
            Assert.IsTrue(b.LookForPossibles());
            AssertSettled(b);
        }

        /// <summary>First adjacent swap that makes a match (types only).</summary>
        internal static (Vector2Int, Vector2Int)? FindMove(Match3Board b)
        {
            var types = new int[8, 8];
            for (int c = 0; c < 8; c++)
                for (int r = 0; r < 8; r++)
                    types[c, r] = b.Tiles[c][r].type;
            bool Line(int c, int r)
            {
                int t = types[c, r];
                if (t == TileTypes.Cookie || t == TileTypes.Torch) return false;
                int h = 1;
                for (int k = c - 1; k >= 0 && types[k, r] == t; k--) h++;
                for (int k = c + 1; k < 8 && types[k, r] == t; k++) h++;
                int v = 1;
                for (int k = r - 1; k >= 0 && types[c, k] == t; k--) v++;
                for (int k = r + 1; k < 8 && types[c, k] == t; k++) v++;
                return h >= 3 || v >= 3;
            }
            for (int c = 0; c < 8; c++)
                for (int r = 0; r < 8; r++)
                    foreach (var d in new[] { new Vector2Int(1, 0), new Vector2Int(0, 1) })
                    {
                        int c2 = c + d.x, r2 = r + d.y;
                        if (c2 > 7 || r2 > 7) continue;
                        if (b.Tiles[c][r].frozen || b.Tiles[c2][r2].frozen) continue;
                        (types[c, r], types[c2, r2]) = (types[c2, r2], types[c, r]);
                        bool ok = Line(c, r) || Line(c2, r2);
                        (types[c, r], types[c2, r2]) = (types[c2, r2], types[c, r]);
                        if (ok) return (new Vector2Int(c, r), new Vector2Int(c2, r2));
                    }
            return null;
        }

        // ================================================================== scene

        Match3Controller game;
        string saveFolder;

        static GameSession Session => GameManager.Instance.Session;
        static PlayerProfile Player => GameManager.Player;

        /// <summary>What the kitchen unlocks before a day (Kitchen.initialize).</summary>
        static void KitchenUnlocks(PlayerProfile p)
        {
            if (p.passedDays >= GameRules.UnlockChineseDish && p.chineseDishLevel[0] == 0) p.chineseDishLevel[0] = p.chineseDishLevel[1] = 1;
            if (p.passedDays >= GameRules.UnlockMidEastDish && p.mideastDishLevel[0] == 0) p.mideastDishLevel[0] = p.mideastDishLevel[1] = 1;
            if (p.passedDays >= GameRules.UnlockBeverage && p.hardwareLevel[GameRules.Juicer] == 0) p.hardwareLevel[GameRules.Juicer] = 1;
            if (p.passedDays >= GameRules.UnlockFries && p.hardwareLevel[GameRules.Fryer] == 0) p.hardwareLevel[GameRules.Fryer] = 1;
        }

        /// <summary>Loads the Match3 scene for day <paramref name="day"/> (0-based) as the first play of that day.</summary>
        IEnumerator StartDay(int day, bool tutorials = false, Action<PlayerProfile> setup = null, int seed = 4242)
        {
            TutorialManager.ResetRuntimeOverrides();
            var gm = GameManager.Instance;
            saveFolder = TestUtil.TempSaveFolder();
            gm.UseSaveFolder(saveFolder);
            var p = gm.Saves.CreatePlayer(0, "Tester");
            p.passedDays = day;
            p.lastCustIdx = Mathf.Max(2, Mathf.Min(11, day / 3));
            KitchenUnlocks(p);
            if (!tutorials) for (int i = 0; i < p.tutorialShowed.Count; i++) p.tutorialShowed[i] = true;
            setup?.Invoke(p);
            gm.Saves.Save();
            gm.Audio.MuteSound = false;
            gm.Audio.MuteMusic = false;
            gm.Session.SelectDay(day);
            gm.Session.extrasMode = false;
            gm.Session.isPlayingSpecialLevel = GameDatabase.Instance.levels.days[day].gameType == GameType.Match3Special;
            Match3Rng.SeedOverride = seed;
            SceneManager.LoadScene(GameScene.Match3.ToString());
            yield return null;
            yield return null;
            game = Match3Controller.Current;
            Assert.IsNotNull(game, "Match3Controller missing");
            Assert.IsTrue(game.Ready);
        }

        static PopupManager Popups => PopupManager.Current;

        /// <summary>Clicks "Continue" through the day windows.</summary>
        static IEnumerator CloseDayWindows()
        {
            for (int i = 0; i < 10 && Popups.HasOpenPopup; i++)
            {
                Assert.IsInstanceOf<DayEventPopup>(Popups.Open[Popups.Open.Count - 1]);
                TestUtil.Click("UniButton");
                yield return TestUtil.Frames(3);
            }
            Assert.IsFalse(Popups.HasOpenPopup);
        }

        IEnumerator WaitForCustomers(int count = 3, float timeout = 6)
        {
            float end = Time.realtimeSinceStartup + timeout;
            while (Time.realtimeSinceStartup < end && game.Customers.Take(count).Any(c => c.onTween || c.X < 0)) yield return null;
            foreach (var c in game.Customers.Take(count))
            {
                Assert.IsFalse(c.onTween, $"customer {c.idx} still walking");
                Assert.AreEqual(0, c.X, 0.01f);
            }
        }

        IEnumerator Wait(Func<bool> condition, float timeout, string what)
        {
            float end = Time.realtimeSinceStartup + timeout;
            while (!condition() && Time.realtimeSinceStartup < end) yield return null;
            Assert.IsTrue(condition(), "timed out waiting for " + what);
        }

        /// <summary>Does one possible move through the board's touch input; false when there is none.</summary>
        bool DoMove()
        {
            var b = game.Board;
            var move = FindMove(b);
            if (move == null) return false;
            var (from, to) = move.Value;
            b.Touch(BoardTouch.Hover, Match3Board.CellX(from.x), Match3Board.CellY(from.y));
            b.Touch(BoardTouch.Began, Match3Board.CellX(from.x), Match3Board.CellY(from.y));
            b.Touch(BoardTouch.Moved, Match3Board.CellX(to.x), Match3Board.CellY(to.y));
            b.Touch(BoardTouch.Ended, Match3Board.CellX(to.x), Match3Board.CellY(to.y));
            return true;
        }

        IEnumerator BoardSettles(float timeout = 10)
        {
            var b = game.Board;
            yield return Wait(() => !b.moveTile && !b.isSwapping && b.Tweens.Count == 0 && b.AllTiles.All(t => !t.removed && !t.onTween),
                timeout, "the board to settle");
        }

        [UnityTest]
        public IEnumerator SceneLoadsForDays([Values(0, 1, 6, 15, 25, 29)] int day)
        {
            yield return StartDay(day, tutorials: true);
            yield return TestUtil.Frames(30);
            var b = game.Board;
            Assert.AreEqual(64, game.BoardView.Tiles.Count);
            Assert.AreEqual(0, b.LookForMatches().Count);
            Assert.IsTrue(b.LookForPossibles());
            var dayData = GameDatabase.Instance.levels.days[day];
            Assert.AreEqual(dayData.goal, game.goalTarget);
            Assert.AreEqual(dayData.expert, game.expertTarget);
            Assert.AreEqual((int)(dayData.time * 30), game.totalTime);
            if (day == 6)
            {
                Assert.IsTrue(Session.isPlayingSpecialLevel, "every 7th day is the food critic day");
                Assert.AreEqual("0 / 1 food critic", game.TargetInfoText.text);
                Assert.AreEqual("upbeat-feeling_loop", GameManager.Instance.Audio.CurrentMusic);
            }
            else
            {
                Assert.AreEqual("0/" + dayData.goal, game.TargetInfoText.text);
                Assert.IsTrue(GameManager.Instance.Audio.IsPlayingMusic("fun-time", "sunny-advert"));
            }
            Assert.AreEqual(Player.hardwareLevel[GameRules.Juicer] > 0, game.Juicer.Machine.gameObject.activeSelf);
            Assert.AreEqual(Player.hardwareLevel[GameRules.Fryer] > 0, game.Fryer.Machine.gameObject.activeSelf);
            var tut = TutorialManager.Current;
            int expectedTutorial = day switch
            {
                0 => TutorialIds.LEVEL_1,
                6 => TutorialIds.LEVEL_7,
                15 => TutorialIds.LEVEL_16,
                25 => TutorialIds.LEVEL_26,
                _ => TutorialIds.None,
            };
            Assert.AreEqual(expectedTutorial, tut.CurrentDialogueId);
            if (day == 25)
            {
                Assert.AreEqual(1, b.currentFrozenTile, "LEVEL_26 freezes the hinted tile");
                Assert.AreEqual(1, b.AllTiles.Count(t => t.frozen));
            }
            TestUtil.Capture($"match3_day{day + 1}_open");
        }

        [UnityTest]
        public IEnumerator DayStartsAfterTheWindows()
        {
            yield return StartDay(1);
            Assert.IsTrue(game.hasWindow);
            Assert.IsInstanceOf<DayEventPopup>(Popups.Open.Last());
            Assert.AreEqual("Today's Preparation", ((DayEventPopup)Popups.Open.Last()).title.text);
            int timer = game.timer;
            yield return TestUtil.Frames(20);
            Assert.AreEqual(timer, game.timer, "nothing runs behind the day window");
            yield return CloseDayWindows();
            Assert.IsFalse(game.hasWindow);
            yield return WaitForCustomers();
            Assert.Less(game.timer, timer);
            Assert.AreEqual(3, game.CustomerOnQueue.Count);
            Assert.AreEqual(3, Session.currentCust);
            Assert.AreEqual(1, Session.customerFocus);
            Assert.IsTrue(game.Customers[0].PriorityVisible, "the first customer has the priority");
            var c0 = game.Customers[0];
            Assert.Greater(c0.ingredientType.Count, 0);
            Assert.IsTrue(c0.IngredientTexts[0].gameObject.activeSelf);
            Assert.AreEqual("x" + c0.ingredientTarget[0], c0.IngredientTexts[0].text);
            TestUtil.Capture("match3_start");
        }

        [UnityTest]
        public IEnumerator MovesPlayAndCapture()
        {
            yield return StartDay(1);
            yield return CloseDayWindows();
            yield return WaitForCustomers();
            int moves = 0;
            for (int i = 0; i < 4; i++)
            {
                if (!DoMove()) break;
                moves++;
                yield return TestUtil.Frames(2);
                Assert.IsTrue(game.Board.isSwapping || game.Board.matched);
                yield return BoardSettles();
            }
            Assert.Greater(moves, 0);
            yield return null;
            TestUtil.Capture("match3_mid");
        }

        /// <summary>Feeds customer <paramref name="slot"/>'s whole order through the board's collect path.</summary>
        IEnumerator CompleteOrder(int slot)
        {
            var c = game.Customers[slot];
            var b = game.Board;
            Session.customerFocus = c.queueIdx; // the pieces go to the focused customer first
            for (int k = 0; k < c.ingredientType.Count; k++)
            {
                int need = c.ingredientTarget[k];
                for (int n = 0; n < need; n++)
                {
                    var t = b.Tiles[k % 8][7];
                    t.type = c.ingredientType[k];
                    t.used = false;
                    ((IMatch3BoardHost)game).OnTileCollected(k % 8, 7);
                    t.used = false;
                }
            }
            yield return Wait(() => game.Coins[slot].gameObject.activeSelf, 3, "the dish to be ready");
        }

        [UnityTest]
        public IEnumerator ServingACustomerCompletesTheDishAndPays()
        {
            yield return StartDay(1);
            yield return CloseDayWindows();
            yield return WaitForCustomers();
            var c = game.Customers[0];
            int price = c.money + c.tipsBonus;
            yield return CompleteOrder(0);
            Assert.IsTrue(c.completed);
            Assert.IsTrue(c.ReadyText.gameObject.activeSelf, "READY!");
            Assert.IsTrue(game.Dishes[0].gameObject.activeSelf);
            Assert.AreEqual(DishPlanner.DishSprite(c.currentDish), game.Dishes[0].sprite.name);
            Assert.IsTrue(c.CheckMarks.Take(c.ingredientType.Count).All(m => !m.gameObject.activeSelf), "the order is cleared");
            yield return TestUtil.Frames(10);
            TestUtil.Capture("match3_dish_ready");
            int total = game.Score.Total;
            game.TouchCoins(0);
            Assert.AreEqual(total + price, game.Score.Total, "price plus tip (no chain bonus on the first coin)");
            Assert.AreEqual(1, game.servedCustomerCount);
            Assert.AreEqual("1/" + game.goalTarget, game.TargetInfoText.text);
            Assert.IsTrue(c.onTween, "the customer leaves");
            Assert.IsFalse(game.Coins[0].gameObject.activeSelf);
            Assert.AreEqual(2, game.CustomerOnQueue.Count);
            yield return Wait(() => game.Score.Shown == game.Score.Total, 3, "the score counter");
            Assert.AreEqual("$ " + game.Score.Total, game.ScoreText.text);
            // the next customer walks in
            yield return Wait(() => !c.onTween && c.X == 0, 4, "the next customer");
            Assert.AreEqual(4, Session.currentCust);
            Assert.AreEqual(4, c.queueIdx);
        }

        [UnityTest]
        public IEnumerator TimerEndingShowsTheResultAndSaves()
        {
            yield return StartDay(1);
            yield return CloseDayWindows();
            yield return WaitForCustomers();
            // a served day: the goal number of customers and some earnings
            game.servedCustomerCount = game.goalTarget;
            game.Score.Add(150);
            game.timer = 2;
            yield return Wait(() => game.timesUp, 1, "time up");
            Assert.AreEqual(0, game.timer);
            Assert.AreEqual(0, game.TimerFill.fillAmount, 0.001f);
            Assert.IsFalse(Popups.HasOpenPopup);
            // the waiting customers give up
            for (int i = 0; i < 3; i++) game.TweenDishAlpha(i, Match3Customer.Leave);
            Assert.IsTrue(game.calledShowWindow, "the last customer left: the summary is coming");
            yield return Wait(() => Popups.HasOpenPopup, 4, "the summary");
            var w = Popups.Open.Last() as ResultPopup;
            Assert.IsNotNull(w, "a result window");
            Assert.AreEqual("Congratulations!", w.title.text);
            Assert.AreEqual(2, Player.passedDays);
            Assert.AreEqual(150, Player.daysEarning[1]);
            Assert.AreEqual(3, Session.coinsGot, "goal reached, expert missed: 3 coins");
            Assert.AreEqual(3, Player.money);
            Assert.AreEqual(1, Player.achievementCounter[Achievements.Achievements.QualifiedChef]);
            Assert.AreEqual(150, Player.achievementCounter[Achievements.Achievements.BigBusiness]);
            var onDisk = new SaveSystem(saveFolder);
            onDisk.LoadAll();
            Assert.AreEqual(2, onDisk.GetSlot(0).passedDays, "saved");
            Assert.AreEqual(150, onDisk.GetSlot(0).daysEarning[1]);
            yield return TestUtil.Frames(30);
            TestUtil.Capture("match3_result");
        }

        [UnityTest]
        public IEnumerator FailedDayShowsLevelFailed()
        {
            yield return StartDay(2);
            yield return CloseDayWindows();
            yield return WaitForCustomers();
            game.timer = 1;
            yield return Wait(() => game.timesUp, 1, "time up");
            for (int i = 0; i < 3; i++) game.TweenDishAlpha(i, Match3Customer.Leave);
            yield return Wait(() => Popups.HasOpenPopup, 4, "the summary");
            var w = (ResultPopup)Popups.Open.Last();
            Assert.AreEqual("Level Failed!", w.title.text);
            Assert.IsTrue(game.Board.gameover);
            Assert.AreEqual(2, Player.passedDays, "not passed");
            Assert.AreEqual(0, Player.daysEarning[2]);
            yield return TestUtil.Frames(20);
            TestUtil.Capture("match3_failed");
        }

        [UnityTest]
        public IEnumerator PauseOpensAndStopsTheDay()
        {
            yield return StartDay(1);
            yield return CloseDayWindows();
            yield return WaitForCustomers();
            TestUtil.Click("PauseButton");
            yield return null;
            Assert.IsTrue(game.paused);
            Assert.IsInstanceOf<PausePopup>(Popups.Open.Last());
            int timer = game.timer;
            int patience = game.Customers[0].patience;
            yield return TestUtil.Frames(20);
            Assert.AreEqual(timer, game.timer, "the timer stops");
            Assert.AreEqual(patience, game.Customers[0].patience, "patience stops");
            TestUtil.Capture("match3_pause");
            TestUtil.Click("RightButton"); // Resume
            yield return TestUtil.Frames(5);
            Assert.IsFalse(game.paused);
            Assert.Less(game.timer, timer);
        }

        [UnityTest]
        public IEnumerator FoodCriticDay()
        {
            yield return StartDay(6);
            Assert.IsTrue(Session.isPlayingSpecialLevel);
            yield return CloseDayWindows();
            yield return WaitForCustomers();
            // queue numbers 1..3 are normal customers; the 6th customer of the day is the critic
            Assert.IsTrue(game.Customers.All(c => c.customerAvaID != Match3Customer.Critic));
            for (int served = 0; served < 3; served++)
            {
                int slot = game.Customers.ToList().FindIndex(c => !c.onTween && !c.completed);
                yield return CompleteOrder(slot);
                game.TouchCoins(slot);
                yield return TestUtil.Frames(2);
            }
            yield return Wait(() => game.Customers.Any(c => c.customerAvaID == Match3Customer.Critic && !c.onTween), 5, "the critic");
            var critic = game.Customers.First(c => c.customerAvaID == Match3Customer.Critic);
            Assert.IsTrue(critic.asVIP);
            Assert.AreEqual(6, critic.queueIdx);
            var dish = GameDatabase.Instance.dishes.dishes[critic.currentDish];
            CollectionAssert.AreEqual(dish.defaultTarget.Select(t => t * 2), critic.ingredientTarget, "the critic orders double");
            Assert.AreEqual("0 / 1 food critic", game.TargetInfoText.text);
            yield return CompleteOrder(critic.idx);
            game.TouchCoins(critic.idx);
            Assert.AreEqual(2, game.goalState, "serving the critic reaches the goal");
            Assert.IsTrue(Player.goalAchieved[6]);
            Assert.AreEqual(1.5f, Session.vipServedRating, "served with more than 75% patience");
            TestUtil.Capture("match3_critic");
        }

        [UnityTest]
        public IEnumerator Day1TutorialStartsTheLevel()
        {
            yield return StartDay(0, tutorials: true);
            var tut = TutorialManager.Current;
            Assert.AreEqual(TutorialIds.LEVEL_1, tut.CurrentDialogueId);
            Assert.IsTrue(tut.IsButtonShowing);
            // day 1 shows its windows anyway (alwaysShow); closing them does not start the level while the bubble waits
            Assert.IsTrue(Popups.HasOpenPopup);
            yield return CloseDayWindows();
            Assert.IsTrue(game.hasWindow);
            Assert.IsTrue(game.Customers.All(c => c.X < 0));
            Assert.AreEqual(0, tut.DialogueCounter);
            Assert.AreEqual(Session.currentPlayedLevel, 0);
            TestUtil.Capture("match3_tutorial_0");
            TestUtil.Click("NextButton"); // OK!
            yield return null;
            Assert.AreEqual(1, tut.DialogueCounter);
            TestUtil.Click("NextButton"); // Continue: (start level) with one customer
            yield return null;
            Assert.IsFalse(game.hasWindow, "the level started");
            Assert.IsFalse(tut.IsShowing, "blockNext line: the bubble waits for the customer");
            yield return Wait(() => tut.DialogueCounter == 2, 4, "customer_in");
            Assert.AreEqual(0, game.Customers[0].X, 0.01f);
            Assert.IsTrue(game.Customers[1].X < 0 && game.Customers[2].X < 0, "only one customer at first");
            TestUtil.Click("NextButton"); // Continue: the hint
            yield return null;
            Assert.AreEqual(3, tut.DialogueCounter, "highlight -> next line");
            Assert.AreNotEqual(0, tut.HighlightX);
            Assert.AreNotEqual(0, tut.HighlightY);
            Assert.IsTrue(game.Board.highlightPossibleTile);
            // only bread / lettuce moves are hinted at LEVEL_1
            var hinted = game.Board.AllTiles.Where(t => game.Board.Tweens.IsTweening(t, BoardTweenProps.Scale)).ToList();
            Assert.AreEqual(3, hinted.Count);
            Assert.IsTrue(hinted.All(t => t.type == 1 || t.type == 2));
            yield return TestUtil.Frames(10);
            TestUtil.Capture("match3_tutorial_3");
            // do the hinted move: "matched" -> line 4
            Assert.IsTrue(DoMove());
            yield return Wait(() => tut.DialogueCounter == 4, 3, "matched");
        }

        [UnityTest]
        public IEnumerator BeverageTutorialStartLevelLine()
        {
            yield return StartDay(11, tutorials: true);
            var tut = TutorialManager.Current;
            Assert.AreEqual(TutorialIds.LEVEL_12, tut.CurrentDialogueId);
            Assert.AreEqual(Player.hardwareLevel[GameRules.Juicer], 1);
            // line 0 is "(start level)": the level started right away, the juicer is 3 tiles from ready
            Assert.AreEqual(game.Juicer.target - 3, game.Juicer.removed);
            Assert.AreEqual((int)((game.Juicer.target - 3) * 100.0 / game.Juicer.target) + "%", game.JuicePercentageText);
            Assert.IsFalse(Popups.HasOpenPopup);
            yield return Wait(() => tut.DialogueCounter == 1, 4, "customer_in");
            Assert.IsFalse(game.hasWindow);
            Assert.IsTrue(tut.IsShowing);
            TestUtil.Click("NextButton");
            yield return null;
            Assert.AreEqual(2, tut.DialogueCounter);
            StringAssert.Contains(game.JuicePercentageText, TestTextOfBubble());
            TestUtil.Capture("match3_tutorial_juice");
        }

        static string TestTextOfBubble() =>
            TutorialManager.Current.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true).First(t => t.name == "Text").text;

        [UnityTest]
        public IEnumerator JuiceIsDraggedToACustomer()
        {
            yield return StartDay(12);
            yield return CloseDayWindows();
            yield return WaitForCustomers();
            var hw = game.Juicer;
            Assert.IsTrue(hw.Layer.gameObject.activeSelf);
            hw.removed = hw.target - 1;
            ((IMatch3BoardHost)game).OnTileCollected(0, 0);
            ((IMatch3BoardHost)game).AfterTilesCollected();
            Assert.IsTrue(hw.Product.gameObject.activeSelf, "the beverage is ready");
            Assert.AreEqual("Drag to Customer", hw.Percentage.text);
            var cust = game.Customers[1];
            var start = hw.Home;
            var end = new Vector2(cust.X + 200, cust.Y + 50);
            TestUtil.Press(hw.Machine.gameObject, start, Vector2.Lerp(start, end, 0.5f), end);
            yield return null;
            Assert.IsTrue(cust.hasJuice);
            Assert.IsTrue(cust.Juice.gameObject.activeSelf);
            Assert.IsFalse(hw.Product.gameObject.activeSelf);
            Assert.IsTrue(hw.Layer.gameObject.activeSelf, "the machine refills");
            Assert.AreEqual("0%", hw.Percentage.text);
        }
    }
}
