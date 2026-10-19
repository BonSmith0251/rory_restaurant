package {
class ButtonWithSound extends DisplayObjectContainer {
    static const MAX_DRAG_DIST:Number = 50;
    var _global:Global;
    var mUpState:Texture;
    var mDownState:Texture;
    var mContents:Sprite;
    var mBackground:Image;
    var mTextField:TextField;
    var mTextBounds:Rectangle;
    var mScaleWhenDown:Number;
    var mAlphaWhenDisabled:Number;
    var mEnabled:Boolean;
    var mIsDown:Boolean;
    var mUseHandCursor:Boolean;

    // static initializer
    static {
        MAX_DRAG_DIST = 50;
    }

    function ButtonWithSound(arg1:Texture, arg2:String = "", arg3:Texture = null):* {
        this._global = Global.getInstance();
        super();
        if (arg1 != null) goto L33;
        throw new ArgumentError("Texture cannot be null");
      L33:
        this.mUpState = arg1;
        $s0 = this;
        if (!arg3) goto L51;
        $s1 = arg3;
        goto L54;
      L51:
        $s1 = arg1;
      L54:
        $s0.mDownState = $s1;
        this.mBackground = new Image(arg1);
        $s0 = this;
        if (!arg3) goto L82;
        $s1 = Number(1);
        goto L85;
      L82:
        $s1 = Number(0.9);
      L85:
        $s0.mScaleWhenDown = $s1;
        this.mAlphaWhenDisabled = 0.5;
        this.mEnabled = true;
        this.mIsDown = false;
        this.mUseHandCursor = true;
        this.mTextBounds = new Rectangle(0, 0, arg1.width, arg1.height);
        this.mContents = new Sprite();
        this.mContents.addChild(this.mBackground);
        addChild(this.mContents);
        addEventListener(TouchEvent.TOUCH, this.onTouch);
        this.text = arg2;
    }

    function resetContents():void {
        this.mIsDown = false;
        this.mBackground.texture = this.mUpState;
        var _local1 = 0;
        this.mContents.y = 0;
        this.mContents.x = _local1;
        _local1 = 1;
        this.mContents.scaleY = 1;
        this.mContents.scaleX = _local1;
    }

    function createTextField():void {
        if (this.mTextField != null) goto L80;
        this.mTextField = new TextField(this.mTextBounds.width, this.mTextBounds.height, "");
        this.mTextField.vAlign = VAlign.CENTER;
        this.mTextField.hAlign = HAlign.CENTER;
        this.mTextField.touchable = false;
        this.mTextField.autoScale = true;
      L80:
        this.mTextField.width = this.mTextBounds.width;
        this.mTextField.height = this.mTextBounds.height;
        this.mTextField.x = this.mTextBounds.x;
        this.mTextField.y = this.mTextBounds.y;
    }

    function onTouch(arg1:TouchEvent):void {
        var _local3 = null;
        var $t0 = Boolean(this.mUseHandCursor);
        $s0 = Mouse;
        $s1 = $t0;
        if (!$t0) goto L25;
        $s1 = Boolean(this.mEnabled);
      L25:
        if (!$s1) goto L38;
        $s1 = Boolean(arg1.interactsWith(this));
      L38:
        if (!$s1) goto L53;
        $s1 = MouseCursor.BUTTON;
        goto L60;
      L53:
        $s1 = MouseCursor.AUTO;
      L60:
        $s0.cursor = $s1;
        var _local2 = arg1.getTouch(this);
        var $t1 = !this.mEnabled;
        $s0 = $t1;
        if ($t1) goto L86;
        $s0 = _local2 == null;
      L86:
        if (!$s0) goto L91;
        return;
      L91:
        var $t2 = _local2.phase == TouchPhase.BEGAN;
        $s0 = $t2;
        if (!$t2) goto L113;
        $s0 = !this.mIsDown;
      L113:
        if (!$s0) goto L212;
        this.mBackground.texture = this.mDownState;
        var $t3 = this.mScaleWhenDown;
        var _local4 = $t3;
        this.mContents.scaleY = $t3;
        this.mContents.scaleX = _local4;
        this.mContents.x = (1 - this.mScaleWhenDown) / 2 * this.mBackground.width;
        this.mContents.y = (1 - this.mScaleWhenDown) / 2 * this.mBackground.height;
        this.mIsDown = true;
        goto L403;
      L212:
        var $t4 = _local2.phase == TouchPhase.MOVED;
        $s0 = $t4;
        if (!$t4) goto L234;
        $s0 = Boolean(this.mIsDown);
      L234:
        if (!$s0) goto L344;
        _local3 = getBounds(stage);
        var $t5 = _local2.globalX < _local3.x - MAX_DRAG_DIST;
        $s0 = $t5;
        if ($t5) goto L283;
        $s0 = _local2.globalY < _local3.y - MAX_DRAG_DIST;
      L283:
        if ($s0) goto L307;
        $s0 = _local2.globalX > _local3.x + _local3.width + MAX_DRAG_DIST;
      L307:
        if ($s0) goto L331;
        $s0 = _local2.globalY > _local3.y + _local3.height + MAX_DRAG_DIST;
      L331:
        if (!$s0) goto L340;
        this.resetContents();
      L340:
        goto L403;
      L344:
        var $t6 = _local2.phase == TouchPhase.ENDED;
        $s0 = $t6;
        if (!$t6) goto L366;
        $s0 = Boolean(this.mIsDown);
      L366:
        if (!$s0) goto L403;
        this._global._soundAsset.playSFX("click_v1");
        this.resetContents();
        dispatchEventWith(Event.TRIGGERED, true);
      L403:
    }

    function get scaleWhenDown():Number {
        return this.mScaleWhenDown;
    }

    function set scaleWhenDown(arg1:Number):void {
        this.mScaleWhenDown = arg1;
    }

    function get alphaWhenDisabled():Number {
        return this.mAlphaWhenDisabled;
    }

    function set alphaWhenDisabled(arg1:Number):void {
        this.mAlphaWhenDisabled = arg1;
    }

    function get enabled():Boolean {
        return this.mEnabled;
    }

    function set enabled(arg1:Boolean):void {
        if (this.mEnabled == arg1) goto L45;
        this.mEnabled = arg1;
        $s0 = this.mContents;
        if (!arg1) goto L32;
        $s1 = Number(1);
        goto L37;
      L32:
        $s1 = Number(this.mAlphaWhenDisabled);
      L37:
        $s0.alpha = $s1;
        this.resetContents();
      L45:
    }

    function get text():String {
        if (!this.mTextField) goto L22;
        $s0 = this.mTextField.text;
        goto L24;
      L22:
        $s0 = "";
      L24:
        return $s0;
    }

    function set text(arg1:String):void {
        if (arg1.length != 0) goto L40;
        if (!this.mTextField) goto L36;
        this.mTextField.text = arg1;
        this.mTextField.removeFromParent();
      L36:
        goto L77;
      L40:
        this.createTextField();
        this.mTextField.text = arg1;
        if (this.mTextField.parent != null) goto L77;
        this.mContents.addChild(this.mTextField);
      L77:
    }

    function get fontName():String {
        if (!this.mTextField) goto L22;
        $s0 = this.mTextField.fontName;
        goto L24;
      L22:
        $s0 = "Verdana";
      L24:
        return $s0;
    }

    function set fontName(arg1:String):void {
        this.createTextField();
        this.mTextField.fontName = arg1;
    }

    function get fontSize():Number {
        if (!this.mTextField) goto L22;
        $s0 = Number(this.mTextField.fontSize);
        goto L25;
      L22:
        $s0 = Number(12);
      L25:
        return $s0;
    }

    function set fontSize(arg1:Number):void {
        this.createTextField();
        this.mTextField.fontSize = arg1;
    }

    function get fontColor():uint {
        if (!this.mTextField) goto L22;
        $s0 = uint(this.mTextField.color);
        goto L25;
      L22:
        $s0 = uint(0);
      L25:
        return $s0;
    }

    function set fontColor(arg1:uint):void {
        this.createTextField();
        this.mTextField.color = arg1;
    }

    function get fontBold():Boolean {
        if (!this.mTextField) goto L22;
        $s0 = Boolean(this.mTextField.bold);
        goto L23;
      L22:
        $s0 = false;
      L23:
        return $s0;
    }

    function set fontBold(arg1:Boolean):void {
        this.createTextField();
        this.mTextField.bold = arg1;
    }

    function get upState():Texture {
        return this.mUpState;
    }

    function set upState(arg1:Texture):void {
        if (this.mUpState == arg1) goto L32;
        this.mUpState = arg1;
        if (this.mIsDown) goto L32;
        this.mBackground.texture = arg1;
      L32:
    }

    function get downState():Texture {
        return this.mDownState;
    }

    function set downState(arg1:Texture):void {
        if (this.mDownState == arg1) goto L32;
        this.mDownState = arg1;
        if (!this.mIsDown) goto L32;
        this.mBackground.texture = arg1;
      L32:
    }

    function get textVAlign():String {
        return this.mTextField.vAlign;
    }

    function set textVAlign(arg1:String):void {
        this.createTextField();
        this.mTextField.vAlign = arg1;
    }

    function get textHAlign():String {
        return this.mTextField.hAlign;
    }

    function set textHAlign(arg1:String):void {
        this.createTextField();
        this.mTextField.hAlign = arg1;
    }

    function get textBounds():Rectangle {
        return this.mTextBounds.clone();
    }

    function set textBounds(arg1:Rectangle):void {
        this.mTextBounds = arg1.clone();
        this.createTextField();
    }

    override function get useHandCursor():Boolean {
        return this.mUseHandCursor;
    }

    override function set useHandCursor(arg1:Boolean):void {
        this.mUseHandCursor = arg1;
    }
}
}
