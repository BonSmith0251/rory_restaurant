// Ported from HOObjects.as (decompiled).
using Starlite;

public class HOObjects
{
    public Sprite display;
    public PixelPerfectObject image;
    public string itemName;
    public int itemIdx;
    public int itemType;
    public bool shouldBeRemoved;
    public const int AS_HO_ITEM = 1;
    public const int AS_BG = 2;
    public const int AS_BG2 = 7;
    public bool listed;
    public bool found;
    public bool onTween;
    public bool isCoin;
    public bool isLogo;
    public bool displayed;
    public bool itemIsFound;
    public int placeNumber;
    public BezierPath path;
    public bool isMultiple;

    public HOObjects(Texture arg1, string arg2, int arg3, int arg4, float arg5, float arg6, PixelHitArea arg7)
    {
        this.display = new Sprite();
        this.image = new PixelPerfectObject(arg1, arg7);
        this.display.addChild(this.image);
        this.display.pivotX = this.display.width / 2;
        this.display.pivotY = this.display.height / 2;
        this.display.x = arg5 + this.display.pivotX;
        this.display.y = arg6 + this.display.pivotY;
        this.display.name = "" + arg4;
        this.itemName = arg2;
        this.itemIdx = arg4;
        this.itemType = arg3;
        this.path = new BezierPath();
        if (arg3 == AS_HO_ITEM)
        {
            this.image.hitArea = null;
        }
        else if (arg3 == AS_BG2)
        {
            this.image.hitArea = null;
            this.image.touchable = false;
        }
    }

    public void Kill()
    {
        this.image.removeFromParent(true);
        this.image = null;
        this.path.kill();
        this.path.removeFromParent(true);
        this.path = null;
    }

    public void setTexture(Texture arg1)
    {
        this.image.texture = arg1;
    }
}
