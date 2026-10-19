namespace Rory.Screens.Match3
{
    /// <summary>
    /// One board piece (Match3Object): its ingredient type, logical cell, position in stage pixels (centre pivot) and
    /// the state flags the original kept on the display object. Views read it every frame (<see cref="TileView"/>).
    /// Objects keep their identity while they move through <see cref="Match3Board.Tiles"/>, exactly like the original
    /// display objects did, so draw order follows creation order.
    /// </summary>
    public class BoardTile : IBoardTweenTarget
    {
        /// <summary>Creation index (draw order) - not the original idx, see <see cref="idx"/>.</summary>
        public readonly int id;

        /// <summary>Match id (Match3Object.idx): col * 8 + row after each settle (setIdx), unique on the board.</summary>
        public int idx;
        public int type;
        public int col;
        public int row;
        public float x;
        public float y;
        public float vy;
        public float scale = 1;

        public bool removed;
        public bool isDropping;
        public bool isSwapping;
        public bool onTween;
        public bool used;
        public bool aktif;
        public bool frozen;
        public int frozenTileIdx;
        public bool asMatch3Part, asMatch4Part, asMatch5Part, asMatchMorePart;
        /// <summary>Hover glow (filter = blurFilter in the original).</summary>
        public bool glow;
        public bool hover;

        public BoardTile(int id) => this.id = id;

        /// <summary>Not touchable for matching: dropping, swapping, tweening or removed.</summary>
        public bool Busy => isDropping || onTween || removed || isSwapping;

        float IBoardTweenTarget.X { get => x; set => x = value; }
        float IBoardTweenTarget.Y { get => y; set => y = value; }
        float IBoardTweenTarget.Scale { get => scale; set => scale = value; }

        public override string ToString() => $"tile#{id}(type {type} at {col},{row}{(removed ? " removed" : "")}{(frozen ? " frozen" : "")})";
    }

    /// <summary>The ice overlay of a frozen tile (FrozenTile). 20 exist; a visible one follows its tile while it drops.</summary>
    public class FrozenOverlay : IBoardTweenTarget
    {
        public readonly int idx;
        public bool visible;
        public float x = -100;
        public float y;
        public int attachedCol;
        public int attachedRow;

        public FrozenOverlay(int idx) => this.idx = idx;

        float IBoardTweenTarget.X { get => x; set => x = value; }
        float IBoardTweenTarget.Y { get => y; set => y = value; }
        float IBoardTweenTarget.Scale { get => 1; set { } }
    }
}
