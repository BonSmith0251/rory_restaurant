namespace Rory.Screens.Match3
{
    /// <summary>The three board animations of GameScreen.playBoardEffect (MovieClips "star_", "asap_", "api_").</summary>
    public enum BoardEffectKind
    {
        Star = 1,
        Smoke = 2,
        Fire = 3,
    }

    /// <summary>What the board's touch area reports (Starling touch phases; Exit = no touch on the quad any more).</summary>
    public enum BoardTouch
    {
        Began,
        Moved,
        Ended,
        Hover,
        Exit,
    }

    /// <summary>
    /// What <see cref="Match3Board"/> needs from the screen: sounds, effects, the customers (orders, cookies), the
    /// hardware counters, the tutorial checks of touchTile / swapTile / highlightPossible and the texts it starts.
    /// Implemented by <see cref="Match3Controller"/>; tests use a fake.
    /// </summary>
    public interface IMatch3BoardHost
    {
        void PlaySfx(string name);
        void PlayEffect(BoardEffectKind kind, float x, float y);

        // ---- tutorial
        /// <summary>touchTile's early returns: the tutorial's Next button is shown, or LEVEL_1 dc 1/7, LEVEL_12 dc 4/5,
        /// LEVEL_17 dc 4/5.</summary>
        bool TouchBlockedByTutorial { get; }
        /// <summary>touchTile MOVED: LEVEL_26 dc &lt; 3 (or dc 3 and the dragged tile is not highlighted), LEVEL_27 dc 0
        /// (or dc 2 unless the torch is dragged upwards), LEVEL_31 dc &lt;= 2.</summary>
        bool DragBlockedByTutorial(int lastCol, int lastRow, int col, int row, int lastTileType);
        /// <summary>swapTile at LEVEL_26 dc 3: a match only counts when it frees a frozen tile.</summary>
        bool MatchMustFreeFrozenTile { get; }
        /// <summary>highlightPossible at LEVEL_1: only bread and lettuce moves are hinted.</summary>
        bool HintOnlyBreadAndLettuce { get; }
        /// <summary>tweenHighlighted while dispatchHighlightPosition &gt; 0: Trigger("highlight", col, row).</summary>
        void OnHighlightDispatched(int col, int row);
        /// <summary>swapTile found a match ("matched").</summary>
        void OnMatched();
        /// <summary>A torch was swapped ("start_torch").</summary>
        void OnStartTorch();

        // ---- gameplay
        /// <summary>touchTile BEGAN, before the board resets its hint: chosen juice/fries are dropped, chain combo = 0.</summary>
        void OnTouchBegan();
        /// <summary>
        /// addNewPieces, for every removed tile in the order the original visits them: hardware counters and
        /// percentages, then checkCustOrderPrior / checkCustOrder.
        /// </summary>
        void OnTileCollected(int col, int row);
        /// <summary>End of addNewPieces: fries / juice ready checks.</summary>
        void AfterTilesCollected();
        /// <summary>startTorchEffect for an ingredient: checkCustOrderPrior + checkCustOrder.</summary>
        void CheckCustomerOrders(int col, int row);
        /// <summary>getWantedIngredient(): an ingredient the focused customer still needs, or 0.</summary>
        int GetWantedIngredient();
        /// <summary>A cookie was used (TGlobalClass.cookiesUsed++).</summary>
        void OnCookieUsed();
        /// <summary>giveTheCookies(tile, cookiesEffect).</summary>
        void GiveCookies(BoardTile tile);
        /// <summary>findAndRemoveOneMatch: a move of 4+ tiles (showGoodMovesText, then the match bonus).</summary>
        void OnGoodMove(int length);
        /// <summary>No possible move left: black screen + "NO POSSIBLE MOVES!" flying in; call
        /// <see cref="Match3Board.OnNoMovesTextArrived"/> when it arrived (1.5 s), then fly it out.</summary>
        void ShowNoPossibleMoves();
        /// <summary>The 50% black screen of tweenAllDown.</summary>
        void SetBlackScreen(bool visible);
    }
}
