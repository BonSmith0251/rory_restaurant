package {
class TNonStateButton extends ButtonWithSound {
    static const CONFIRM_EXIT:uint = 1;
    static const CANCEL_BUTTON:uint = 2;
    static const MOREGAMES_BUTTON:uint = 3;
    static const NEWGAME_BUTTON:uint = 4;
    static const CONTINUEGAME_BUTTON:uint = 5;
    static const CONFIRM_BUTTON:uint = 6;
    static const SUBMIT_BUTTON:uint = 7;
    static const LEADERBOARD_BUTTON:uint = 8;
    static const REPLAY_BUTTON:uint = 9;
    static const MAINMENU_BUTTON:uint = 10;
    static const LEFT_ARROW:uint = 11;
    static const RIGHT_ARROW:uint = 12;
    static const PLAY_BUTTON:uint = 13;
    static const EXTRAS_BUTTON:uint = 14;
    static const NEXT_LEVEL:uint = 15;
    static const TRY_AGAIN:uint = 16;
    static const YES_BUTTON:uint = 17;
    static const NO_BUTTON:uint = 18;
    static const OK_BUTTON:uint = 19;
    static const HELP_BUTTON:uint = 20;
    static const DELETE_BUTTON:uint = 21;
    static const CREATE_BUTOTN:uint = 22;
    static const SELECT_BUTTON:uint = 23;
    static const DONE_CREATE_PLAYER:uint = 24;
    static const SOUND_BUTTON:uint = 25;
    static const MUSIC_BUTTON:uint = 26;
    static const RESUME_BUTTON:uint = 27;
    static const KITCHEN:uint = 28;
    static const KITCHEN_WITH_CONFIRMATION:uint = 29;
    static const NEXT_WINDOW:uint = 30;
    static const ENDING:uint = 31;
    static const HO_BUTTON:uint = 32;
    static const DM_BUTTON:uint = 33;
    static const UNLOCK_BUTTON:uint = 34;
    static const FULLSCREEN_BUTTON:uint = 35;
    var _text:ExtendTextField;
    var _image:cSprite;
    var textureName:String;
    var idx:uint;
    var hover:Boolean;
    var _enabled:Boolean;
    var initPosX:Boolean;
    var initPosY:Boolean;
    var isPivotSetX:Boolean;
    var isPivotSetY:Boolean;

    // static initializer
    static {
        CONFIRM_EXIT = 1;
        CANCEL_BUTTON = 2;
        MOREGAMES_BUTTON = 3;
        NEWGAME_BUTTON = 4;
        CONTINUEGAME_BUTTON = 5;
        CONFIRM_BUTTON = 6;
        SUBMIT_BUTTON = 7;
        LEADERBOARD_BUTTON = 8;
        REPLAY_BUTTON = 9;
        MAINMENU_BUTTON = 10;
        LEFT_ARROW = 11;
        RIGHT_ARROW = 12;
        PLAY_BUTTON = 13;
        EXTRAS_BUTTON = 14;
        NEXT_LEVEL = 15;
        TRY_AGAIN = 16;
        YES_BUTTON = 17;
        NO_BUTTON = 18;
        OK_BUTTON = 19;
        HELP_BUTTON = 20;
        DELETE_BUTTON = 21;
        CREATE_BUTOTN = 22;
        SELECT_BUTTON = 23;
        DONE_CREATE_PLAYER = 24;
        SOUND_BUTTON = 25;
        MUSIC_BUTTON = 26;
        RESUME_BUTTON = 27;
        KITCHEN = 28;
        KITCHEN_WITH_CONFIRMATION = 29;
        NEXT_WINDOW = 30;
        ENDING = 31;
        HO_BUTTON = 32;
        DM_BUTTON = 33;
        UNLOCK_BUTTON = 34;
        FULLSCREEN_BUTTON = 35;
    }

    function TNonStateButton(arg1:Texture, arg2:String = "", arg3:String = "", arg4:uint = 20, arg5:uint = 0):* {
        super(arg1, arg2);
        this.fontName = arg3;
        this.fontSize = arg4;
        this.fontColor = arg5;
        this._image = new cSprite(arg1);
        this.addChild(this._image);
        if (this.text != "Test") goto L62;
        this.text = "";
      L62:
        this._text = new ExtendTextField(this._image.width, this._image.height, arg2, arg3, arg4, arg5);
        this._text.y = 0;
        this._text.vAlign = "center";
        this._text.hAlign = "center";
        this.addChild(this._text);
        this.addEventListener(TouchEvent.TOUCH, this.touchButton);
    }

    function touchButton(arg1:TouchEvent):void {
        var _local2 = arg1.currentTarget as TNonStateButton;
        var _local3 = arg1.getTouch(_local2, "hover");
        if (!_local3) goto L140;
        if (_local2.hover) goto L136;
        var $t0 = _local2._image.name == "" + Kitchen.RIGHT_BUTTON;
        $s0 = $t0;
        if ($t0) goto L79;
        $s0 = _local2._image.name == "" + Kitchen.LEFT_BUTTON;
      L79:
        if (!$s0) goto L109;
        _local2.scaleX = _local2.scaleX * 1.2;
        _local2.scaleY = _local2.scaleY * 1.2;
        goto L131;
      L109:
        _local2.scaleX = _local2.scaleX * 1.05;
        _local2.scaleY = _local2.scaleY * 1.05;
      L131:
        _local2.hover = true;
      L136:
        goto L257;
      L140:
        if (!_local2.hover) goto L257;
        var $t1 = _local2._image.name == "" + Kitchen.RIGHT_BUTTON;
        $s0 = $t1;
        if ($t1) goto L188;
        $s0 = _local2._image.name == "" + Kitchen.LEFT_BUTTON;
      L188:
        if (!$s0) goto L224;
        _local2.scaleX = _local2.scaleX * (10 / 12);
        _local2.scaleY = _local2.scaleY * (10 / 12);
        goto L252;
      L224:
        _local2.scaleX = _local2.scaleX * (10 / 10.5);
        _local2.scaleY = _local2.scaleY * (10 / 10.5);
      L252:
        _local2.hover = false;
      L257:
    }

    override function get x():Number {
        return super.x;
    }

    override function set x(arg1:Number):void {
        if (this.initPosX) goto L69;
        if (this.pivotX != 0) goto L50;
        this.pivotX = this.width / 2;
        super.x = arg1 + this.pivotX;
        this.isPivotSetX = false;
        goto L60;
      L50:
        this.isPivotSetX = true;
        super.x = arg1;
      L60:
        this.initPosX = true;
        goto L96;
      L69:
        if (this.isPivotSetX) goto L91;
        super.x = arg1 + this.pivotX;
        goto L96;
      L91:
        super.x = arg1;
      L96:
    }

    override function get y():Number {
        return super.y;
    }

    override function set y(arg1:Number):void {
        if (this.initPosY) goto L69;
        if (this.pivotY != 0) goto L50;
        this.pivotY = this.height / 2;
        super.y = arg1 + this.pivotY;
        this.isPivotSetY = false;
        goto L60;
      L50:
        this.isPivotSetY = true;
        super.y = arg1;
      L60:
        this.initPosY = true;
        goto L96;
      L69:
        if (this.isPivotSetY) goto L91;
        super.y = arg1 + this.pivotY;
        goto L96;
      L91:
        super.y = arg1;
      L96:
    }

    override function dispose():void {
        super.dispose();
        if (!this._image) goto L24;
        this._image.removeFromParent(true);
      L24:
        this._image = null;
        if (!this._text) goto L46;
        this._text.removeFromParent(true);
      L46:
        this._text = null;
    }

    override function get enabled():Boolean {
        return this._enabled;
    }

    override function set enabled(arg1:Boolean):void {
        if (!arg1) goto L22;
        this.alpha = 1;
        this.touchable = true;
        goto L33;
      L22:
        this.alpha = 0.5;
        this.touchable = false;
      L33:
        this._enabled = arg1;
    }
}
}
