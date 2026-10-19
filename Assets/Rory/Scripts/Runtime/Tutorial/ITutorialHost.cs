using UnityEngine;

namespace Rory.Tutorials
{
    /// <summary>
    /// The screen the tutorial is shown on. The scene's controller registers itself with
    /// <c>TutorialManager.Current.Host = this</c> (Awake/Start) so the tutorial can read screen data and ask the
    /// screen to do what the original Tutorial.as did directly to <c>gameScreen</c> / <c>_minigameDessert</c>.
    /// Screens without anything to offer may leave the host unset (kitchen: implement it to return IsKitchen = true).
    /// </summary>
    public interface ITutorialHost
    {
        /// <summary><c>_global.currentSceneID == KITCHEN_ID</c>: checkTut then ignores the played day.</summary>
        bool IsKitchen { get; }

        /// <summary>
        /// Text that replaces "(juice percentage)" (<c>gameScreen.juicerPercentage.text</c>, e.g. "75%").
        /// Return null on screens other than match-3: the original only replaced it on the game screen.
        /// </summary>
        string JuicePercentageText { get; }

        /// <summary>
        /// Original position (x, y-down stage pixels, the tile's centre pivot) of match-3 tile
        /// <c>tileAr[col][row]</c>. Used for the "highlight" rule and the TORCH/COOKIE arrow tokens.
        /// Return false when there is no board.
        /// </summary>
        bool TryGetTilePosition(int col, int row, out Vector2 position);

        /// <summary>
        /// Hidden object screen: original position of the first listed item
        /// (<c>itemPosXAr/itemPosYAr[listedItemAr[0]]</c>) and its display height
        /// (<c>itemDisplayAr[listedItemAr[0]].height</c>), for the HOITEMX/HOITEMY arrow tokens. False elsewhere.
        /// </summary>
        bool TryGetFirstListedItem(out Vector2 position, out float height);

        /// <summary>
        /// Performs a screen action requested by the rule table or by a "(start level)" line. Must run synchronously:
        /// e.g. <see cref="TutorialCommandType.HighlightPossibleMove"/> has to call
        /// <c>Trigger("highlight", col/row)</c> for the first three highlighted tiles before it returns, because the
        /// next dialogue line reads the result (TILEX/TILEY). Ignore commands that don't apply to the screen.
        /// </summary>
        void OnTutorialCommand(TutorialCommand command);
    }

    public enum TutorialCommandType
    {
        /// <summary><c>gameScreen.startLevel(Customers)</c> (match-3; Customers is 3 unless LEVEL_1 asks for 1).
        /// Also sent for a "(start level)" line (<see cref="TutorialCommand.FromStartLevelLine"/>).</summary>
        StartLevel,
        /// <summary><c>_minigameDessert.startLevel()</c> (LEVEL_11 / LEVEL_25 "clicked_ok").</summary>
        StartDessertLevel,
        /// <summary><c>dispatchHighlightPosition = 3; highlightPossible(); highlightPossibleTile = true;</c>
        /// The screen sends "highlight" (col, row) for each of the first 3 tiles it highlights, synchronously.</summary>
        HighlightPossibleMove,
        /// <summary><c>gameScreen.hasWindow = false</c> (the board starts updating).</summary>
        ClearHasWindow,
        /// <summary>LEVEL_1 line 6: <c>customer[0..2].tweenOut(CUST_SATISFIED)</c>.</summary>
        SendCustomersOutSatisfied,
        /// <summary>LEVEL_26: freeze tile (Col, Row): <c>tileAr[c][r].frozen = true</c>, attach the first invisible
        /// frozen overlay to it, <c>currentFrozenTile++</c>.</summary>
        FreezeTile,
    }

    /// <summary>A screen action requested by the tutorial (see <see cref="ITutorialHost.OnTutorialCommand"/>).</summary>
    public readonly struct TutorialCommand
    {
        public readonly TutorialCommandType Type;
        /// <summary>StartLevel: number of customers (gameScreen.startLevel argument, default 3).</summary>
        public readonly int Customers;
        /// <summary>FreezeTile: tile column / row.</summary>
        public readonly int Col, Row;
        /// <summary>StartLevel sent because a "(start level)" dialogue line was reached.</summary>
        public readonly bool FromStartLevelLine;

        public TutorialCommand(TutorialCommandType type, int customers = 3, int col = 0, int row = 0, bool fromStartLevelLine = false)
        {
            Type = type;
            Customers = customers;
            Col = col;
            Row = row;
            FromStartLevelLine = fromStartLevelLine;
        }

        public override string ToString() => Type switch
        {
            TutorialCommandType.StartLevel => $"StartLevel({Customers}{(FromStartLevelLine ? ", line" : "")})",
            TutorialCommandType.FreezeTile => $"FreezeTile({Col},{Row})",
            _ => Type.ToString(),
        };
    }
}
