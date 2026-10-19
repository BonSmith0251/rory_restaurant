using System.Collections.Generic;
using Rory.Tweening;

namespace Rory.Screens.Match3
{
    /// <summary>
    /// The 8x8 match-3 board of GameScreen as a plain model: generation, touch swapping, matching, gravity, refill,
    /// frozen tiles, chef's torch, cookies, the hint and the no-moves reshuffle. Ported line by line, quirks included
    /// (see the comments marked "(sic)"); everything outside the board goes through <see cref="IMatch3BoardHost"/>.
    /// <para>Timing: <see cref="Tick"/> is one frame of the original's 30 fps Update (movePieces + the idle hint
    /// counter), <see cref="StepTweens"/> advances the board tweens by real time.</para>
    /// <para><see cref="Tiles"/>[col][row] is tileAr. Positions are stage pixels of the tile centre.</para>
    /// </summary>
    public class Match3Board
    {
        public const int Size = 8;
        public const float OriginX = 494, OriginY = 73.5f, Cell = 68;
        /// <summary>touchQuad: 544 x 544 at (457, 40).</summary>
        public const float TouchX = 457, TouchY = 40, TouchSize = 544;
        public const float Gravity = 0.2f;
        /// <summary>Frames without a move before the hint (idleDuration &gt; 300).</summary>
        public const int IdleFramesForHint = 300;
        public const int FrozenOverlayCount = 20;

        public static float CellX(int col) => col * Cell + OriginX;
        public static float CellY(int row) => row * Cell + OriginY;

        readonly IMatch3BoardHost host;
        readonly Match3Rng rng;

        public readonly BoardTile[][] Tiles = new BoardTile[Size][];
        /// <summary>The 64 tile objects in creation order (draw order).</summary>
        public readonly List<BoardTile> AllTiles = new List<BoardTile>();
        public readonly FrozenOverlay[] Frozen = new FrozenOverlay[FrozenOverlayCount];
        public readonly BoardTweens Tweens = new BoardTweens();

        /// <summary>availableTypeAr: the ingredient types that drop on this day.</summary>
        public List<int> AvailableTypes = new List<int>();
        /// <summary>TGlobalClass.currentPlayedLevel (0-based) - frozen / torch / cookie counters.</summary>
        public int PlayedDay;

        public bool moveTile, isSwapping, matched, swapBack, noMoreMoves, highlightPossibleTile;
        public bool touchFirstTile, touchCookies;
        public bool gameover, levelComplete;
        public int idleDuration;
        public int dispatchHighlightPosition;
        public int maxFrozenTile, currentFrozenTile;
        public int cookiesCountOnBoard;
        public int removedTileCountCookies;
        public int removedTileTorch;
        public int torchTarget = GameRules.TileNeededForTorch;
        public int lastCol, lastRow;
        public BoardTile firstTile, lastTile;

        static readonly string[] TextureNames =
            { "bread", "selada", "beef", "ayam", "nasi", "bacon", "salmon", "ikan", "keju", "cookies", "pita_bread", "lamb", "tomato" };

        /// <summary>Sprite name ("game" atlas) of a tile type: 1..13 ingredients (10 = cookies), 15 = chef's torch.</summary>
        public static string TextureName(int type) =>
            type == TileTypes.Torch ? "chef_torch" : type >= 1 && type <= TextureNames.Length ? TextureNames[type - 1] : "keju";

        public Match3Board(IMatch3BoardHost host, Match3Rng rng)
        {
            this.host = host;
            this.rng = rng;
            int n = 0;
            for (int col = 0; col < Size; col++)
            {
                Tiles[col] = new BoardTile[Size];
                for (int row = 0; row < Size; row++)
                {
                    var t = new BoardTile(n) { x = CellX(col), y = CellY(row), col = col, row = row, idx = n, type = 9 };
                    Tiles[col][row] = t;
                    AllTiles.Add(t);
                    n++;
                }
            }
            for (int i = 0; i < FrozenOverlayCount; i++) Frozen[i] = new FrozenOverlay(i);
        }

        /// <summary>maxFrozenTile of the played day (initialize()).</summary>
        public static int MaxFrozenFor(int playedDay) =>
            playedDay < 30 ? 2 : playedDay < 35 ? 3 : playedDay < 40 ? 4 : playedDay < 45 ? 5 : 6;

        /// <summary>initialize(): reshuffle until the board has no ready-made match and at least one possible move.</summary>
        public void Generate()
        {
            do
            {
                ShuffleTile();
            }
            while (LookForMatches().Count > 0 || !LookForPossibles());
        }

        int RandomAvailable() => AvailableTypes[rng.Range(0, AvailableTypes.Count - 1)];

        // ================================================================== frame / tweens

        public void StepTweens(float dt) => Tweens.Step(dt);

        /// <summary>One original frame of board work while the game runs (Update: movePieces unless noMoreMoves).</summary>
        public void Tick()
        {
            if (!noMoreMoves) MovePieces();
        }

        /// <summary>The idle hint of Update (after the customers and the timer).</summary>
        public void TickIdle()
        {
            if (highlightPossibleTile) return;
            idleDuration++;
            if (idleDuration > IdleFramesForHint)
            {
                HighlightPossible();
                highlightPossibleTile = true;
                idleDuration = 0;
            }
        }

        public void MovePieces()
        {
            bool moving = false;
            if (!moveTile) return;
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    var t = Tiles[col][row];
                    if (t.removed) continue;
                    float target = t.row * Cell + OriginY;
                    if (t.y < target)
                    {
                        if (!t.isSwapping) t.isDropping = true;
                        if (t.y + t.vy * 2 < target)
                        {
                            t.vy = t.vy + Gravity * 2;
                            t.y = t.y + t.vy * 2;
                        }
                        else
                        {
                            t.isDropping = false;
                            t.y = target;
                        }
                        var f = Frozen[t.frozenTileIdx];
                        if (f.visible && f.attachedCol == col && f.attachedRow == row) f.y = t.y;
                        moving = true;
                    }
                    else
                    {
                        t.vy = 0;
                        t.isDropping = false;
                    }
                }
            }
            if (!moving && moveTile)
            {
                moveTile = false;
                isSwapping = false;
                FindAndRemoveOneMatch();
                SetIdx();
            }
        }

        public void SetIdx()
        {
            int n = 0;
            for (int col = 0; col < Size; col++)
                for (int row = 0; row < Size; row++)
                {
                    var t = Tiles[col][row];
                    t.idx = n;
                    t.col = col;
                    t.row = row;
                    n++;
                }
        }

        // ================================================================== touch

        /// <summary>touchTile: a touch on the board quad at stage (x, y).</summary>
        public void Touch(BoardTouch phase, float x, float y)
        {
            int col = 0, row = 0;
            BoardTile tile = null;
            if (phase != BoardTouch.Exit)
            {
                // AS3 int(): truncation, so up to one cell left of / above the quad still reads as column / row 0
                col = (int)((x - TouchX) / Cell);
                row = (int)((y - TouchY) / Cell);
                if (col < 0 || row < 0 || col > 7 || row > 7) return;
                tile = Tiles[col][row];
                if (levelComplete || isSwapping || gameover) return;
                if (tile.onTween || tile.removed || tile.isSwapping || tile.isDropping) return;
                if (host.TouchBlockedByTutorial) return;
                if (phase == BoardTouch.Began)
                {
                    matched = false;
                    swapBack = false;
                    host.OnTouchBegan();
                    highlightPossibleTile = false;
                    idleDuration = 0;
                    ResetHintScale();
                    if (tile.type == TileTypes.Cookie) touchCookies = true;
                    Tweens.KillProps(tile, BoardTweenProps.Scale);
                    tile.scale = 1;
                    if (!touchFirstTile) touchFirstTile = true;
                }
                else if (phase == BoardTouch.Moved)
                {
                    if (lastCol == col && lastRow == row) return;
                    if (!touchFirstTile) return;
                    if (host.DragBlockedByTutorial(lastCol, lastRow, col, row, Tiles[lastCol][lastRow].type)) return;
                    if (lastRow == row && lastCol < col)
                    {
                        if (!SwapWithNeighbour(tile, lastCol + 1, lastRow, 1, 0)) return;
                    }
                    else if (lastRow == row && lastCol > col)
                    {
                        if (!SwapWithNeighbour(tile, lastCol - 1, lastRow, 1, 0)) return;
                    }
                    else if (lastRow < row && lastCol == col)
                    {
                        if (!SwapWithNeighbour(tile, lastCol, lastRow + 1, 0, 1)) return;
                    }
                    else if (lastRow > row && lastCol == col)
                    {
                        if (!SwapWithNeighbour(tile, lastCol, lastRow - 1, 0, 1)) return;
                    }
                }
                else if (phase == BoardTouch.Ended)
                {
                    if (touchFirstTile) touchFirstTile = false;
                    if (touchCookies)
                    {
                        touchCookies = false;
                        host.OnCookieUsed();
                        host.GiveCookies(tile);
                    }
                }
            }
            if (phase == BoardTouch.Hover)
            {
                tile.glow = true;
                tile.hover = true;
                if (lastCol != col || lastRow != row) Tiles[lastCol][lastRow].glow = false;
            }
            else
            {
                Tiles[lastCol][lastRow].glow = false;
            }
            lastCol = col;
            lastRow = row;
        }

        /// <summary>The hint pulse stops: scale tweens of every tile that is not being removed are killed.</summary>
        public void ResetHintScale()
        {
            for (int c = 0; c < Size; c++)
                for (int r = 0; r < Size; r++)
                {
                    var t = Tiles[c][r];
                    if (t.onTween) continue;
                    Tweens.KillProps(t, BoardTweenProps.Scale);
                    t.scale = 1;
                }
        }

        /// <summary>One drag direction of touchTile; false where the original returned from touchTile.</summary>
        bool SwapWithNeighbour(BoardTile tile, int nCol, int nRow, int shakeX, int shakeY)
        {
            var n = Tiles[nCol][nRow];
            if (n.isDropping || n.onTween || n.isSwapping || n.removed) return false;
            touchCookies = false;
            var last = Tiles[lastCol][lastRow];
            if (n.frozen || last.frozen)
            {
                if (n.frozen) TweenShake(Frozen[n.frozenTileIdx], shakeX, shakeY);
                if (last.frozen) TweenShake(Frozen[last.frozenTileIdx], shakeX, shakeY);
                return false;
            }
            tile.scale = 1;
            Tweens.KillProps(tile, BoardTweenProps.Scale);
            touchFirstTile = false;
            firstTile = last;
            lastTile = n;
            SwapTile(firstTile, lastTile);
            return true;
        }

        // ================================================================== swap

        public void SwapTile(BoardTile first, BoardTile last)
        {
            Tiles[first.col][first.row].isSwapping = true;
            Tiles[last.col][last.row].isSwapping = true;
            isSwapping = true;
            int tempCol = last.col, tempRow = last.row;
            last.glow = false;
            first.glow = false;
            last.col = first.col;
            last.row = first.row;
            first.col = tempCol;
            first.row = tempRow;
            Tiles[first.col][first.row] = first;
            Tiles[last.col][last.row] = last;
            Tweens.To(first, 0.2f, BoardTweenProps.Position, last.x, last.y, ease: Ease.Linear);
            Tweens.To(last, 0.2f, BoardTweenProps.Position, first.x, first.y, ease: Ease.Linear, onComplete: () =>
            {
                Tiles[first.col][first.row].isSwapping = false;
                Tiles[last.col][last.row].isSwapping = false;
                isSwapping = false;
                var m1 = GetMatches(Tiles[first.col][first.row]);
                var m2 = GetMatches(Tiles[last.col][last.row]);
                if (first.type == TileTypes.Torch || last.type == TileTypes.Torch)
                {
                    host.OnStartTorch();
                    if (first.type == TileTypes.Torch) StartTorchEffect(first);
                    if (last.type == TileTypes.Torch) StartTorchEffect(last);
                }
                else if (m1.Count == 0 && m2.Count == 0 && !swapBack)
                {
                    isSwapping = true;
                    swapBack = true;
                    SwapTile(first, last);
                }
                if (m1.Count > 0 || m2.Count > 0)
                {
                    if (host.MatchMustFreeFrozenTile)
                    {
                        bool frozenFound = MatchesContainFrozen(m1) || MatchesContainFrozen(m2);
                        if (!frozenFound)
                        {
                            isSwapping = true;
                            swapBack = true;
                            SwapTile(first, last);
                            return;
                        }
                    }
                    host.OnMatched();
                    matched = true;
                    FindAndRemoveOneMatch();
                }
            });
        }

        /// <summary>Does any tile of these matches sit on a frozen tile? (swapTile's LEVEL_26 check)</summary>
        bool MatchesContainFrozen(List<List<int>> matches)
        {
            foreach (var m in matches)
                foreach (int id in m)
                {
                    var t = FindByIdx(id);
                    if (t != null && t.frozen) return true;
                }
            return false;
        }

        BoardTile FindByIdx(int id)
        {
            for (int c = 0; c < Size; c++)
                for (int r = 0; r < Size; r++)
                    if (Tiles[c][r].idx == id) return Tiles[c][r];
            return null;
        }

        public void StartTorchEffect(BoardTile torch)
        {
            host.PlaySfx("power_up");
            int row = torch.row;
            for (int c = 0; c < Size; c++)
            {
                var t = Tiles[c][row];
                if (t.isDropping || t.onTween || t.removed || t.isSwapping) continue;
                bool doRemove = true;
                if (t.frozen)
                {
                    // (sic) currentFrozenTile is not decreased and the overlay keeps its index
                    t.frozen = false;
                    Frozen[t.frozenTileIdx].visible = false;
                }
                else if (t.type == TileTypes.Cookie)
                {
                    doRemove = false;
                    host.OnCookieUsed();
                    PlayEffect(BoardEffectKind.Fire, c, row);
                    PlayEffect(BoardEffectKind.Smoke, c, row);
                    host.GiveCookies(Tiles[c][row]);
                }
                else if (t.type != TileTypes.Cookie && t.type != TileTypes.Torch)
                {
                    host.CheckCustomerOrders(c, row);
                }
                if (doRemove)
                {
                    Tiles[c][row].removed = true;
                    PlayEffect(BoardEffectKind.Fire, c, row);
                    PlayEffect(BoardEffectKind.Smoke, c, row);
                    AffectAbove(Tiles[c][row]);
                    AddNewPieces();
                }
            }
        }

        // ================================================================== matching

        static bool SameForMatch(BoardTile n, BoardTile t) =>
            n.type == t.type && n.type != TileTypes.Cookie && t.type != TileTypes.Cookie && t.type != TileTypes.Torch;

        /// <summary>getMatches(tile): the horizontal and vertical lines of 3+ through the tile.</summary>
        public List<List<int>> GetMatches(BoardTile t)
        {
            var result = new List<List<int>>();
            var horiz = new List<int> { Tiles[t.col][t.row].idx };
            for (int k = 1; t.col + k < Size; k++)
            {
                var n = Tiles[t.col + k][t.row];
                if (!SameForMatch(n, t) || n.Busy) break;
                horiz.Add(n.idx);
            }
            for (int k = 1; t.col - k >= 0; k++)
            {
                var n = Tiles[t.col - k][t.row];
                if (!SameForMatch(n, t) || n.Busy) break;
                horiz.Add(n.idx);
            }
            if (horiz.Count > 2) result.Add(horiz);
            var vert = new List<int> { Tiles[t.col][t.row].idx };
            for (int k = 1; t.row + k < Size; k++)
            {
                var n = Tiles[t.col][t.row + k];
                if (!SameForMatch(n, t) || n.Busy) break;
                vert.Add(n.idx);
            }
            for (int k = 1; t.row - k >= 0; k++)
            {
                var n = Tiles[t.col][t.row - k];
                if (!SameForMatch(n, t) || n.Busy) break;
                vert.Add(n.idx);
            }
            if (vert.Count > 2) result.Add(vert);
            return result;
        }

        public void ShuffleTile()
        {
            for (int c = 0; c < Size; c++)
                for (int r = 0; r < Size; r++)
                {
                    var t = Tiles[c][r];
                    if (t.type != TileTypes.Cookie && t.type != TileTypes.Torch) t.type = RandomAvailable();
                }
        }

        public void FindAndRemoveOneMatch()
        {
            var temp = new List<int>();
            var tempTorL = new List<BoardTile>();
            var matches = LookForMatches();
            if (matches.Count > 0)
            {
                highlightPossibleTile = false;
                idleDuration = 0;
            }
            int goodMovesLength = 3;
            int tempType = 0;
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].Count >= 4) goodMovesLength = matches[i].Count;
                for (int j = 0; j < matches[i].Count; j++)
                {
                    int horzCount = 1, vertCount = 1;
                    for (int col = 0; col < Size; col++)
                    {
                        for (int row = 0; row < Size; row++)
                        {
                            var tile = Tiles[col][row];
                            if (tile.idx != matches[i][j]) continue;
                            tempType = tile.type;
                            if (!tile.removed)
                            {
                                tile.removed = true;
                                tile.glow = false;
                                int count = matches[i].Count;
                                if (count == 3) tile.asMatch3Part = true;
                                else if (count == 4) tile.asMatch4Part = true;
                                else if (count == 5) tile.asMatch5Part = true;
                                else if (count > 5) tile.asMatchMorePart = true;
                                temp.Add(tile.idx);
                                continue;
                            }
                            // tile already removed by a previous match: T or L shape
                            tempTorL.Add(tile);
                            for (int m = 1; row - m >= 0; m++)
                            {
                                if (tile.type != Tiles[col][row - m].type) break;
                                vertCount++;
                                tempTorL.Add(Tiles[col][row - m]);
                            }
                            for (int m = 1; row + m < Size; m++)
                            {
                                if (tile.type != Tiles[col][row + m].type) break;
                                vertCount++;
                                tempTorL.Add(Tiles[col][row + m]);
                            }
                            for (int m = 1; col - m >= 0; m++)
                            {
                                if (tile.type != Tiles[col - m][row].type) break;
                                horzCount++;
                                tempTorL.Add(Tiles[col - m][row]);
                            }
                            for (int m = 1; col + m < Size; m++)
                            {
                                if (tile.type != Tiles[col + m][row].type) break;
                                horzCount++;
                                tempTorL.Add(Tiles[col + m][row]);
                            }
                        }
                    }
                    foreach (var part in tempTorL)
                    {
                        var t = Tiles[part.col][part.row];
                        if (horzCount == 3 && vertCount == 3)
                        {
                            t.asMatch3Part = t.asMatch4Part = t.asMatchMorePart = false;
                            t.asMatch5Part = true;
                            if (goodMovesLength < 5) goodMovesLength = 5;
                        }
                        else if (horzCount > 3 || vertCount > 3)
                        {
                            t.asMatch3Part = t.asMatch4Part = t.asMatch5Part = false;
                            t.asMatchMorePart = true;
                            // (sic) compares with h + v but assigns h + v - 1
                            if (goodMovesLength < horzCount + vertCount) goodMovesLength = horzCount + vertCount - 1;
                        }
                    }
                    tempTorL.Clear();
                }
                if (tempType == 3) host.PlaySfx("beef_sizzle");
                else if (tempType == 4) host.PlaySfx("chicken_sizzle");
                else if (tempType == 7) host.PlaySfx("fish_sizzle");
                else if (tempType == 6) host.PlaySfx("pork_sizzle");
                else host.PlaySfx("other_ingredient_matched_v" + rng.Range(1, 2));
            }
            if (goodMovesLength >= 4) host.OnGoodMove(goodMovesLength);

            bool hasFrozenRemoved = false;
            for (int col = 0; col < Size; col++)
            {
                for (int row = 0; row < Size; row++)
                {
                    for (int i = 0; i < temp.Count; i++)
                    {
                        var t = Tiles[col][row];
                        if (t.idx != temp[i]) continue;
                        if (t.frozen)
                        {
                            host.PlaySfx("ice_break" + (rng.Range(1, 2) == 1 ? "" : "_v2"));
                            t.frozen = false;
                            t.removed = false;
                            Frozen[t.frozenTileIdx].visible = false;
                            Frozen[t.frozenTileIdx].x = -100;
                            t.frozenTileIdx = 0;
                            currentFrozenTile--;
                            hasFrozenRemoved = true;
                        }
                        else
                        {
                            int type = t.type;
                            if (type == 2 || type == 3 || type == 6 || type == 7 || type == 8 || type == 12)
                                PlayEffect(BoardEffectKind.Smoke, col, row);
                            else
                                PlayEffect(BoardEffectKind.Star, col, row);
                            AffectAbove(Tiles[col][row]);
                        }
                        break;
                    }
                }
            }
            temp.Clear();
            AddNewPieces();
            if (hasFrozenRemoved) FindAndRemoveOneMatch();
            if (matches.Count <= 0 && !moveTile)
            {
                if (gameover)
                {
                }
                else if (levelComplete)
                {
                    levelComplete = false;
                }
                else if (!LookForPossibles())
                {
                    host.ShowNoPossibleMoves();
                }
            }
        }

        /// <summary>playBoardEffect: the effect at the current position of tileAr[col][row] (smoke 10 px higher).</summary>
        void PlayEffect(BoardEffectKind kind, int col, int row)
        {
            var t = Tiles[col][row];
            host.PlayEffect(kind, t.x, kind == BoardEffectKind.Smoke ? t.y - 10 : t.y);
        }

        /// <summary>"NO POSSIBLE MOVES!" reached the centre: the board stops and every tile falls out (tweenAllDown).</summary>
        public void OnNoMovesTextArrived()
        {
            noMoreMoves = true;
            TweenAllDown();
        }

        public void TweenAllDown()
        {
            int count = 0;
            host.SetBlackScreen(true);
            for (int col = 0; col < Size; col++)
            {
                for (int row = 0; row < Size; row++)
                {
                    var t = Tiles[col][row];
                    t.removed = false;
                    t.aktif = false;
                    Tweens.KillProps(t, BoardTweenProps.Scale);
                    t.asMatch3Part = t.asMatch4Part = t.asMatch5Part = t.asMatchMorePart = false;
                    t.isDropping = false;
                    t.onTween = true;
                    t.isSwapping = false;
                    int c = col, r = row;
                    Tweens.To(t, 1 - t.row * 0.05f, BoardTweenProps.Y, y: 700, delay: 0.08f * (9 - t.row), ease: Ease.Linear,
                        onComplete: () =>
                        {
                            count++;
                            Tiles[c][r].onTween = false;
                            Tiles[c][r].y = -80;
                            if (count >= 64)
                            {
                                do
                                {
                                    ShuffleTile();
                                }
                                while (LookForMatches().Count != 0 || !LookForPossibles());
                                moveTile = true;
                                host.SetBlackScreen(false);
                                noMoreMoves = false;
                            }
                        });
                }
            }
            for (int i = 0; i < FrozenOverlayCount; i++)
            {
                var f = Frozen[i];
                if (f.visible)
                    Tweens.To(f, 1 - f.attachedRow * 0.05f, BoardTweenProps.Y, y: 700, delay: 0.08f * (9 - f.attachedRow), ease: Ease.Linear);
            }
        }

        // ================================================================== gravity and refill

        public void AffectAbove(BoardTile tile)
        {
            int col = tile.col;
            for (int r = tile.row - 1; r >= 0; r--)
            {
                var above = Tiles[col][r];
                if (above.removed) continue;
                if (above.frozen) Frozen[above.frozenTileIdx].attachedRow++;
                var tmp = Tiles[col][r + 1];
                above.row++;
                Tiles[col][r + 1] = above;
                Tiles[col][r] = tmp;
                Tiles[col][r].removed = true;
                Tiles[col][r + 1].removed = false;
                Tiles[col][r + 1].isDropping = true;
            }
        }

        public void AddNewPieces()
        {
            for (int col = 0; col < Size; col++)
            {
                int missingPieces = 0;
                for (int row = 7; row >= 0; row--)
                {
                    var t = Tiles[col][row];
                    if (!t.removed) continue;
                    if (t.onTween) continue;
                    if (PlayedDay >= GameRules.UnlockCookies) removedTileCountCookies++;
                    if (PlayedDay >= GameRules.UnlockTorch) removedTileTorch++;
                    // (the original played "heartburst" for a cookie here; the sound does not exist in the game data)
                    host.OnTileCollected(col, row);
                    missingPieces++;
                    t.onTween = true;
                    int c = col, r = row, miss = missingPieces;
                    // both steps look the tile up by position when they finish, like the original closures
                    Tweens.To(t, 0.1f, BoardTweenProps.Scale, scale: 1.2f, ease: Ease.Linear, onComplete: () =>
                        Tweens.To(Tiles[c][r], 0.1f, BoardTweenProps.Scale, scale: 0, ease: Ease.Linear,
                            onComplete: () => OnNewPieceShrunk(c, r, miss)));
                }
            }
            host.AfterTilesCollected();
        }

        /// <summary>Respawns the shrunk tile above its column as a new random piece (maybe frozen or a torch).</summary>
        void OnNewPieceShrunk(int col, int row, int missPiece)
        {
            var t = Tiles[col][row];
            t.removed = false;
            t.aktif = false;
            t.asMatch3Part = t.asMatch4Part = t.asMatch5Part = t.asMatchMorePart = false;
            t.onTween = false;
            t.frozen = false;
            t.used = false;
            t.y = 60 - 70 - 69 * missPiece;
            t.scale = 1;
            t.isDropping = true;
            t.col = col;
            t.row = row;
            int rnd = rng.Range(1, 100);
            if (rnd <= 25)
            {
                t.type = host.GetWantedIngredient();
                if (t.type == 0) t.type = RandomAvailable();
            }
            else
            {
                t.type = RandomAvailable();
            }
            if (PlayedDay >= GameRules.UnlockFrozen && currentFrozenTile < maxFrozenTile)
            {
                int rndFrozen = rng.Range(0, 100);
                if (rndFrozen < 5)
                {
                    t.frozen = true;
                    AttachFrozenOverlay(t, col, row);
                    currentFrozenTile++;
                }
            }
            if (removedTileTorch >= torchTarget && !t.frozen)
            {
                t.type = TileTypes.Torch;
                removedTileTorch = 0;
            }
            moveTile = true;
        }

        void AttachFrozenOverlay(BoardTile t, int col, int row)
        {
            foreach (var f in Frozen)
            {
                if (f.visible) continue;
                f.visible = true;
                f.x = t.x;
                f.y = t.y;
                f.attachedCol = col;
                f.attachedRow = row;
                t.frozenTileIdx = f.idx;
                break;
            }
        }

        /// <summary>Freezes tileAr[col][row] (tutorial LEVEL_26 / LEVEL_27): first free overlay, currentFrozenTile++.</summary>
        public void FreezeTile(int col, int row)
        {
            var t = Tiles[col][row];
            t.frozen = true;
            AttachFrozenOverlay(t, col, row);
            currentFrozenTile++;
        }

        /// <summary>Turns tileAr[col][row] into a special piece (tutorial: torch / cookie at [1][1]).</summary>
        public void SetType(int col, int row, int type)
        {
            Tiles[col][row].type = type;
            if (type == TileTypes.Cookie) cookiesCountOnBoard++;
        }

        public void TweenShake(FrozenOverlay f, int dx = 1, int dy = 0)
        {
            host.PlaySfx("ice_break");
            var t = Tiles[f.attachedCol][f.attachedRow];
            f.x = t.x;
            f.y = t.y;
            const float s = 6;
            // TimelineLite: four 0.1 s steps; every target is computed from the position now, (sic) ending 3 px off
            float x = f.x, y = f.y;
            Tweens.To(f, 0.1f, BoardTweenProps.Position, x + s / 2f * dx, y + s / 2f * dy, ease: Ease.Linear);
            Tweens.To(f, 0.1f, BoardTweenProps.Position, x - s * dx, y - s * dy, ease: Ease.Linear, delay: 0.1f);
            Tweens.To(f, 0.1f, BoardTweenProps.Position, x + s * dx, y + s * dy, ease: Ease.Linear, delay: 0.2f);
            Tweens.To(f, 0.1f, BoardTweenProps.Position, x - s / 2f * dx, y - s / 2f * dy, ease: Ease.Linear, delay: 0.3f);
        }

        /// <summary>popCookiesOnBoard: a random free piece becomes a cookie (at most 5 on the board).</summary>
        public void PopCookiesOnBoard()
        {
            if (cookiesCountOnBoard >= 5) return;
            var cols = new List<int>();
            var rows = new List<int>();
            for (int c = 0; c < Size; c++)
                for (int r = 0; r < Size; r++)
                {
                    var t = Tiles[c][r];
                    if (!t.removed && !t.onTween && !t.frozen)
                    {
                        cols.Add(c);
                        rows.Add(r);
                    }
                }
            if (cols.Count == 0) return;
            int pc, pr;
            int guard = 0;
            do
            {
                // (sic) column and row are drawn independently from the two lists
                pc = cols[rng.Range(0, cols.Count - 1)];
                pr = rows[rng.Range(0, rows.Count - 1)];
            }
            while ((Tiles[pc][pr].frozen || Tiles[pc][pr].type == TileTypes.Cookie || Tiles[pc][pr].type == TileTypes.Torch) && ++guard < 10000);
            if (guard >= 10000) return;
            Tiles[pc][pr].type = TileTypes.Cookie;
            cookiesCountOnBoard++;
        }

        // ================================================================== scanning

        public List<List<int>> LookForMatches()
        {
            var result = new List<List<int>>();
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < 6; col++)
                {
                    var t = Tiles[col][row];
                    if (t.isDropping || t.isSwapping || t.onTween || t.removed) continue;
                    var horiz = GetMatchHoriz(col, row);
                    if (horiz.Count > 2)
                    {
                        result.Add(horiz);
                        col += horiz.Count - 1;
                    }
                }
            }
            for (int col = 0; col < Size; col++)
            {
                for (int row = 0; row < 6; row++)
                {
                    var t = Tiles[col][row];
                    if (t.isDropping || t.isSwapping || t.onTween || t.removed) continue;
                    var vert = GetMatchVert(col, row);
                    if (vert.Count > 2)
                    {
                        result.Add(vert);
                        row += vert.Count - 1;
                    }
                }
            }
            return result;
        }

        public List<int> GetMatchHoriz(int col, int row)
        {
            var result = new List<int>();
            var t = Tiles[col][row];
            if (!t.Busy) result.Add(t.idx);
            for (int k = 1; col + k < Size; k++)
            {
                if (t.type == TileTypes.Cookie || t.type == TileTypes.Torch) continue;
                var n = Tiles[col + k][row];
                if (t.type != n.type) return result;
                if (n.Busy) return result;
                result.Add(n.idx);
            }
            return result;
        }

        public List<int> GetMatchVert(int col, int row)
        {
            var result = new List<int>();
            var t = Tiles[col][row];
            if (!t.Busy) result.Add(t.idx);
            for (int k = 1; row + k < Size; k++)
            {
                if (t.type == TileTypes.Cookie || t.type == TileTypes.Torch) continue;
                var n = Tiles[col][row + k];
                if (t.type != n.type) return result;
                if (n.Busy) return result;
                result.Add(n.idx);
            }
            return result;
        }

        // Possible-move patterns: offsets of the required same-type tile, the candidates that would complete the
        // line, and the tile each candidate swaps with.
        static readonly int[][] Pat1Need = { new[] { 1, 0 } };
        static readonly int[][] Pat1Match = { new[] { -2, 0 }, new[] { -1, -1 }, new[] { -1, 1 }, new[] { 2, -1 }, new[] { 2, 1 }, new[] { 3, 0 } };
        static readonly int[][] Pat1Swap = { new[] { -1, 0 }, new[] { -1, 0 }, new[] { -1, 0 }, new[] { 2, 0 }, new[] { 2, 0 }, new[] { 2, 0 } };
        static readonly int[][] Pat2Need = { new[] { 2, 0 } };
        static readonly int[][] Pat2Match = { new[] { 1, -1 }, new[] { 1, 1 } };
        static readonly int[][] Pat2Swap = { new[] { 1, 0 }, new[] { 1, 0 } };
        static readonly int[][] Pat3Need = { new[] { 0, 1 } };
        static readonly int[][] Pat3Match = { new[] { 0, -2 }, new[] { -1, -1 }, new[] { 1, -1 }, new[] { -1, 2 }, new[] { 1, 2 }, new[] { 0, 3 } };
        static readonly int[][] Pat3Swap = { new[] { 0, -1 }, new[] { 0, -1 }, new[] { 0, -1 }, new[] { 0, 2 }, new[] { 0, 2 }, new[] { 0, 2 } };
        static readonly int[][] Pat4Need = { new[] { 0, 2 } };
        static readonly int[][] Pat4Match = { new[] { -1, 1 }, new[] { 1, 1 } };
        static readonly int[][] Pat4Swap = { new[] { 0, 1 }, new[] { 0, 1 } };

        static readonly int[][][][] Patterns =
        {
            new[] { Pat1Need, Pat1Match, Pat1Swap },
            new[] { Pat2Need, Pat2Match, Pat2Swap },
            new[] { Pat3Need, Pat3Match, Pat3Swap },
            new[] { Pat4Need, Pat4Match, Pat4Swap },
        };

        public bool LookForPossibles()
        {
            for (int col = 0; col < Size; col++)
                for (int row = 0; row < Size; row++)
                {
                    foreach (var p in Patterns)
                        if (MatchPattern(col, row, p[0], p[1], p[2])) return true;
                    if (Tiles[col][row].type == TileTypes.Torch) return true;
                }
            return false;
        }

        bool MatchPattern(int col, int row, int[][] need, int[][] match, int[][] swap)
        {
            int type = Tiles[col][row].type;
            foreach (var n in need)
                if (!MatchType(col + n[0], row + n[1], type)) return false;
            for (int k = 0; k < match.Length; k++)
            {
                if (MatchType(col + match[k][0], row + match[k][1], type) && !Tiles[col + match[k][0]][row + match[k][1]].frozen)
                {
                    if (!Tiles[col + swap[k][0]][row + swap[k][1]].frozen) return true;
                }
            }
            return false;
        }

        public bool MatchType(int col, int row, int type)
        {
            if (col < 0 || col > 7 || row < 0 || row > 7) return false;
            return Tiles[col][row].type == type;
        }

        // ================================================================== hint

        public void HighlightPossible()
        {
            for (int col = 0; col < Size; col++)
                for (int row = 0; row < Size; row++)
                {
                    if (host.HintOnlyBreadAndLettuce && !(MatchType(col, row, 1) || MatchType(col, row, 2))) continue;
                    foreach (var p in Patterns)
                    {
                        if (MatchPattern(col, row, p[0], p[1], p[2]))
                        {
                            Highlight(col, row, p[0], p[1], p[2]);
                            return;
                        }
                    }
                }
        }

        void Highlight(int col, int row, int[][] need, int[][] match, int[][] swap)
        {
            int type = Tiles[col][row].type;
            TweenHighlighted(Tiles[col][row].col, Tiles[col][row].row);
            foreach (var n in need)
                if (MatchType(col + n[0], row + n[1], type))
                {
                    var t = Tiles[col + n[0]][row + n[1]];
                    TweenHighlighted(t.col, t.row);
                }
            for (int k = 0; k < match.Length; k++)
            {
                if (MatchType(col + match[k][0], row + match[k][1], type) && !Tiles[col + swap[k][0]][row + swap[k][1]].frozen)
                {
                    var t = Tiles[col + match[k][0]][row + match[k][1]];
                    TweenHighlighted(t.col, t.row);
                    return;
                }
            }
        }

        /// <summary>The endless 0.4 s pulse of a hinted cell (whatever tile is at the cell when each step starts).</summary>
        public void TweenHighlighted(int col, int row)
        {
            if (dispatchHighlightPosition > 0)
            {
                dispatchHighlightPosition--;
                host.OnHighlightDispatched(col, row);
            }
            Tweens.To(Tiles[col][row], 0.4f, BoardTweenProps.Scale, scale: 1.2f, ease: Ease.Linear, onComplete: () =>
                Tweens.To(Tiles[col][row], 0.4f, BoardTweenProps.Scale, scale: 1, ease: Ease.Linear,
                    onComplete: () => TweenHighlighted(col, row)));
        }
    }

    /// <summary>Special tile types.</summary>
    public static class TileTypes
    {
        public const int Cookie = 10;
        public const int Torch = 15;
    }
}
