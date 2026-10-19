package rudi {
class cSprite extends Image {
    var aktif:Boolean;
    var idx:int;
    var onCenter:Boolean;
    var itemIdx:int;
    var type:int;
    var ingredientName:String;
    var _paddingX:Number = 0;
    var _paddingY:Number = 0;
    var hover:Boolean;
    var xDif:Number;
    var yDif:Number;

    function cSprite(arg1:Texture):* {
        super(arg1);
    }

    function get paddingX():Number {
        return this._paddingX;
    }

    function set paddingX(arg1:Number):void {
        this._paddingX = arg1;
    }

    function get paddingY():Number {
        return this._paddingY;
    }

    function set paddingY(arg1:Number):void {
        this._paddingY = arg1;
    }

    override function hitTest(arg1:Point, arg2:Boolean = false):DisplayObject {
        var $t0 = Boolean(arg2);
        $s0 = $t0;
        if (!$t0) goto L24;
        var $t1 = !visible;
        $s0 = $t1;
        if ($t1) goto L24;
        $s0 = !touchable;
      L24:
        if (!$s0) goto L30;
        return null;
      L30:
        var _local3 = getBounds(this);
        _local3.inflate(this._paddingX, this._paddingY);
        if (!_local3.containsPoint(arg1)) goto L66;
        return this;
      L66:
        return null;
    }
}
}
