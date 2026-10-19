package {
class HOObjects {
    var display:Sprite;
    var image:PixelPerfectObject;
    var itemName:String;
    var itemIdx:int;
    var itemType:uint;
    var shouldBeRemoved:Boolean;
    const AS_HO_ITEM:uint = 1;
    const AS_BG:uint = 2;
    const AS_BG2:uint = 7;
    var listed:Boolean;
    var found:Boolean;
    var onTween:Boolean;
    var isCoin:Boolean;
    var isLogo:Boolean;
    var displayed:Boolean;
    var itemIsFound:Boolean;
    var placeNumber:int;
    var path:BezierPath;
    var isMultiple:Boolean;

    function HOObjects(arg1:Texture, arg2:String, arg3:uint, arg4:int, arg5:Number, arg6:Number, arg7:PixelHitArea):* {
        super();
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
        if (arg3 != this.AS_HO_ITEM) goto L170;
        this.image.hitArea = null;
        goto L195;
      L170:
        if (arg3 != this.AS_BG2) goto L195;
        this.image.hitArea = null;
        this.image.touchable = false;
      L195:
    }

    function Kill():void {
        this.image.removeFromParent(true);
        this.image = null;
        this.path.kill();
        this.path.removeFromParent(true);
        this.path = null;
    }

    function setTexture(arg1:Texture):void {
        this.image.texture = arg1;
    }
}
}
