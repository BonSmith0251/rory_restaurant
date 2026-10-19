// Ported from Match3Object.as (decompiled).
using Starlite;

public class Match3Object : Image
{
    public int idx;
    public bool aktif;
    public bool used;
    public int type;
    public int col;
    public int row;
    public float vy = 0;
    public bool removed;
    public bool isDropping;
    public bool isSwapping;
    public bool @double;
    public bool onTween;
    public bool asMatch3Part;
    public bool asMatch4Part;
    public bool asMatch5Part;
    public bool asMatchMorePart;
    public bool asBomb;
    public bool asVertical;
    public bool asHorizontal;
    public bool asColor;
    public bool asSpecial;
    public bool toBombTile;
    public bool toColorTile;
    public bool specialPossible;
    public bool powerActivated;
    public bool frozen;
    public int frozenTileIdx;
    public bool hover;

    public Match3Object(Texture arg1) : base(arg1)
    {
    }
}
