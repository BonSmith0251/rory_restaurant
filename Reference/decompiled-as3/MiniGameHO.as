package {
class MiniGameHO extends Screen {
    static const FireConfig:Class;
    static const FireParticle:Class;
    var _global:Global;
    var initVector:Boolean;
    var _background:cSprite;
    var bitmapData:BitmapData;
    var paused:Boolean;
    var hasWindow:Boolean;
    var uiAtlas:TextureAtlas;
    var tweenAr:Vector.<TweenLite>;
    const HINT_RECOVER_TIME:int = 450;
    const PAUSE_BUTTON_IDX:int = 1;
    const SOUND_BUTTON_IDX:int = 2;
    const HINT_BUTTON_IDX:int = 3;
    const MUSIC_BUTTON_IDX:int = 4;
    var uiBG:cSprite;
    var soundBtn:TNonStateButton;
    var musicBtn:TNonStateButton;
    var pauseBtn:TNonStateButton;
    var mute:cSprite;
    var muteMusic:cSprite;
    var hintFill:cSprite;
    var hintBG:cSprite;
    var timeBG:cSprite;
    var timeFill:cSprite;
    var timeGlass:cSprite;
    var timeFillSprite:Sprite;
    var hintFillSprite:Sprite;
    var hintText:TNonStateButton;
    var moneyText:ExtendTextField;
    var buttonAr:Vector.<DisplayObject>;
    var _HOClass:HOClass;
    var zoomContainer:Sprite;
    var touchDragged:Boolean;
    var hasHint:Boolean;
    var hintTimer:int;
    var timer:int;
    var HINT_HEIGHT:int;
    var TIMER_WIDTH:int;
    var TOTAL_TIME:int;
    var gameover:Boolean;
    var zoomContainerHeight:Number;
    var zoomContainerWidth:Number;
    var _backgroundWidth:Number;
    var bottomDifference:Number;
    var beganLocX:Number;
    var beganLocY:Number;
    var goalTarget:int;
    var expertTarget:int;
    var bonusMoney:String;
    var currentLevel:int;
    var totalItemPerLevel:Vector.<int>;
    var levelSet:Vector.<String>;
    var touchedBackground:Boolean;
    var escCounter:int;

    // static initializer
    static {
        FireConfig = MiniGameHO_FireConfig;
        FireParticle = MiniGameHO_FireParticle;
    }

    function MiniGameHO():* {
        var $t0 = new Vector.<int>(7);
        $t0[0] = 21;
        $t0[1] = 24;
        $t0[2] = 27;
        $t0[3] = 30;
        $t0[4] = 33;
        $t0[5] = 33;
        $t0[6] = 33;
        this.totalItemPerLevel = $t0;
        var $t1 = new Vector.<String>(7);
        $t1[0] = "HO1_A";
        $t1[1] = "HO1_B";
        $t1[2] = "HO1_C";
        $t1[3] = "HO2_A";
        $t1[4] = "HO2_B";
        $t1[5] = "HO2_C";
        $t1[6] = "HO1_A";
        this.levelSet = $t1;
        super();
    }

    override function Start():void {
        super.Start();
        this._global.currentSceneID = this._global.HIDDEN_OBJECT_ID;
        if (this.initVector) goto L66;
        this.tweenAr = new Vector.<TweenLite>();
        this.buttonAr = new Vector.<DisplayObject>();
        this.initVector = true;
      L66:
        if (TGlobalClass.extrasMode) goto L140;
        this.currentLevel = Math.floor(TGlobalClass.currentPlayedLevel / 14);
        this.goalTarget = int(this.totalItemPerLevel[this.currentLevel] * 0.75);
        this.expertTarget = this.totalItemPerLevel[this.currentLevel];
        goto L146;
      L140:
        this.currentLevel = 0;
      L146:
        this._global.assetManager.enqueue(this._global.graphicsPath + "Hidden_Object.ean", this._global.graphicsPath + "Hidden_Object.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + this.levelSet[this.currentLevel] + "_BG.eag");
        this._global.assetManager.enqueue(this._global.graphicsPath + this.levelSet[this.currentLevel] + ".ean", this._global.graphicsPath + this.levelSet[this.currentLevel] + ".xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + this.levelSet[this.currentLevel] + "_data.xml");
        this._global.assetManager.loadQueue(function(f1_arg1:Number):void {
            if (f1_arg1 != 1) goto L14;
            initialize();
          L14:
        });
    }

    override function Kill():void {
        var _local1 = 0;
        var _local2 = 0;
        _local1 = 0;
        goto L30;
      L15:
        this.tweenAr[_local1].kill();
        _local1++;
      L30:
        if (_local1 < this.tweenAr.length) goto L15;
        goto L55;
      L46:
        this.tweenAr.pop();
      L55:
        if (this.tweenAr.length) goto L46;
        _local1 = 0;
        goto L98;
      L73:
        this.buttonAr[_local1].removeFromParent(true);
        this.buttonAr[_local1] = null;
        _local1++;
      L98:
        if (_local1 < _local2) goto L73;
        this.buttonAr.length = 0;
        this._background.removeFromParent(true);
        this._background = null;
        this.bitmapData.dispose();
        this.bitmapData = null;
        Starling.juggler.remove(this._global.ps);
        Starling.juggler.remove(this._global.psFound);
        this._global.ps.stop();
        this._global.ps.removeFromParent(true);
        this._global.ps = null;
        this._global.psFound.stop();
        this._global.psFound.removeFromParent(true);
        this._global.psFound = null;
        this.uiBG.removeFromParent(true);
        this.soundBtn.removeFromParent(true);
        this.musicBtn.removeFromParent(true);
        this.pauseBtn.removeFromParent(true);
        this.mute.removeFromParent(true);
        this.muteMusic.removeFromParent(true);
        this.hintFill.removeFromParent(true);
        this.hintBG.removeFromParent(true);
        this.timeBG.removeFromParent(true);
        this.timeFill.removeFromParent(true);
        this.timeGlass.removeFromParent(true);
        this.hintFillSprite.removeFromParent(true);
        this.timeFillSprite.removeFromParent(true);
        this.hintText.removeFromParent(true);
        this.moneyText.removeFromParent(true);
        this.zoomContainer.removeFromParent(true);
        this.uiBG = null;
        this.soundBtn = null;
        this.musicBtn = null;
        this.pauseBtn = null;
        this.mute = null;
        this.muteMusic = null;
        this.hintFill = null;
        this.hintBG = null;
        this.timeBG = null;
        this.timeFill = null;
        this.timeGlass = null;
        this.hintFillSprite = null;
        this.timeFillSprite = null;
        this.hintText = null;
        this.moneyText = null;
        this.zoomContainer = null;
        this._HOClass.Kill();
        this._HOClass = null;
        this.uiAtlas.dispose();
        this.uiAtlas = null;
        this._global.assetManager.purge();
        TGlobalClass._windowDisplay.hideAllWindows(this._global.currentSceneID);
        TGlobalClass._windowDisplay.kill(this._global.currentSceneID);
        ready = false;
    }

    override function Update():void {
        if (ready) goto L10;
        return;
      L10:
        if (!this.hasWindow) goto L19;
        return;
      L19:
        if (!this.paused) goto L28;
        return;
      L28:
        if (!this.gameover) goto L37;
        return;
      L37:
        if (!(this.timer > 0)) goto L134;
        var $t0 = !TGlobalClass._windowDisplay._blackScreen.visible;
        $s0 = $t0;
        if (!$t0) goto L80;
        $s0 = !this._global.tutorial.button.visible;
      L80:
        if (!$s0) goto L130;
        var _local1 = this;
        var _local2 = this.timer - 1;
        _local1.timer = _local2;
        this.timeFillSprite.clipRect.x = this.timer / this.TOTAL_TIME * this.TIMER_WIDTH - this.TIMER_WIDTH;
      L130:
        goto L147;
      L134:
        this.gameover = true;
        this._HOClass.onLevelComplete();
      L147:
        if (this.hasHint) goto L226;
        _local1 = this;
        _local2 = this.hintTimer + 1;
        _local1.hintTimer = _local2;
        this.hintFillSprite.clipRect.y = this.HINT_HEIGHT - this.hintTimer / this.HINT_RECOVER_TIME * this.HINT_HEIGHT;
        if (!(this.hintTimer >= this.HINT_RECOVER_TIME)) goto L226;
        this.hasHint = true;
        this.hintText.visible = true;
      L226:
    }

    function initialize():void {
        var _local1 = 0;
        var _local3 = null;
        var _local4 = null;
        this._global.ReportGameState(this._global.GAMESTATE_PLAYING);
        if (TGlobalClass.musicPlayingIdx == 3) goto L63;
        this._global._soundAsset.playMusic("sunny-bliss");
        TGlobalClass.musicPlayingIdx = 3;
      L63:
        if (TGlobalClass.extrasMode) goto L102;
        GALib.TrackEvent(GALib.EVENT_START_LEVEL_STORY, TGlobalClass.currentPlayedLevel + 1, "HIDDEN OBJECTS");
        goto L125;
      L102:
        GALib.TrackEvent(GALib.EVENT_START_LEVEL_EXTRAS, this.currentLevel + 1, "HIDDEN OBJECTS");
      L125:
        System.pauseForGCIfCollectionImminent(0);
        System.gc();
        if (TGlobalClass.extrasMode) goto L197;
        this.TOTAL_TIME = int(TGlobalClass.levelSetXML.day[TGlobalClass.currentPlayedLevel].time) * 30;
        this.bonusMoney = "a";
        goto L206;
      L197:
        this.TOTAL_TIME = 120 * 30;
      L206:
        this.timer = this.TOTAL_TIME;
        this.uiAtlas = this._global.assetManager.getTextureAtlas("Hidden_Object");
        this.zoomContainer = new Sprite();
        this._global._gameS.addChild(this.zoomContainer);
        this.zoomContainer.addEventListener(TouchEvent.TOUCH, this.touchContainer);
        this._background = new cSprite(this._global.assetManager.getTexture(this.levelSet[this.currentLevel] + "_BG"));
        this._background.blendMode = BlendMode.NONE;
        this._backgroundWidth = this._background.width;
        this._background.addEventListener(TouchEvent.TOUCH, this.touchBackground);
        this.zoomContainer.addChild(this._background);
        this.bitmapData = this._global.assetManager.getBitmapData(this.levelSet[this.currentLevel]);
        var _local2 = new Bitmap(this.bitmapData);
        this._HOClass = new HOClass();
        this._HOClass.init(this._global.assetManager.getXml(this.levelSet[this.currentLevel] + "_data"));
        this._HOClass.buildPixelHitArea(_local2);
        this._HOClass.initHOItems(1, this.totalItemPerLevel[this.currentLevel], this._global.assetManager.getTextureAtlas(this.levelSet[this.currentLevel]));
        this.uiBG = new cSprite(this.uiAtlas.getTexture("UI_bg"));
        this.uiBG.touchable = false;
        this._global._gameS.addChild(this.uiBG);
        this.moneyText = new ExtendTextField(150, 80, "Goal: 0/" + this.goalTarget, "jenna_sue", 30, 16777215);
        $s0 = this.moneyText;
        if (!TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel]) goto L614;
        $s1 = "Exp.: ";
        goto L618;
      L614:
        $s1 = "Goal: ";
      L618:
        $s1 = $s1 + this._HOClass.itemFoundCount + "/";
        if (!TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel]) goto L661;
        $s2 = this.expertTarget;
        goto L666;
      L661:
        $s2 = this.goalTarget;
      L666:
        $s0.text = $s1 + $s2;
        if (!TGlobalClass.extrasMode) goto L706;
        this.moneyText.text = "$ " + this._global._maxCrypto.STI(this.bonusMoney);
      L706:
        this.moneyText.touchable = false;
        this.moneyText.hAlign = "left";
        this.moneyText.x = 30;
        this.moneyText.y = 58;
        this._global._gameS.addChild(this.moneyText);
        this.hintFill = new cSprite(this.uiAtlas.getTexture("hint_fill"));
        this.HINT_HEIGHT = this.hintFill.height;
        this.hintFill.touchable = false;
        this.hintFillSprite = new Sprite();
        this.hintFillSprite.addChild(this.hintFill);
        this.hintFillSprite.x = 887;
        this.hintFillSprite.y = 25;
        this.hintFillSprite.clipRect = new Rectangle(0, 0, this.hintFill.width, this.hintFill.height);
        this._global._gameS.addChild(this.hintFillSprite);
        this.hintBG = new cSprite(this.uiAtlas.getTexture("hint_bg"));
        this.hintBG.x = 862;
        this._global._gameS.addChild(this.hintBG);
        this.hintText = new TNonStateButton(Texture.empty(this.hintBG.width, this.hintBG.height), "HINT!", "bromine", 25, 0);
        this.hintText.x = this.hintBG.x + 5;
        this.hintText.y = this.hintBG.y + 20;
        this.hintText.idx = this.HINT_BUTTON_IDX;
        this.buttonAr.push(this.hintText);
        this._global._gameS.addChild(this.hintText);
        this.timeBG = new cSprite(this.uiAtlas.getTexture("timer_BG"));
        this.timeBG.x = 299;
        this.timeBG.y = 103;
        this.timeBG.touchable = false;
        this._global._gameS.addChild(this.timeBG);
        this.timeFill = new cSprite(this.uiAtlas.getTexture("timer_fill"));
        this.TIMER_WIDTH = this.timeFill.width;
        this.timeFill.touchable = false;
        this.timeFillSprite = new Sprite();
        this.timeFillSprite.addChild(this.timeFill);
        this.timeFillSprite.x = 371;
        this.timeFillSprite.y = 108;
        this._global._gameS.addChild(this.timeFillSprite);
        this.timeFillSprite.clipRect = new Rectangle(0, 0, this.timeFill.width, this.timeFill.height);
        this.timeGlass = new cSprite(this.uiAtlas.getTexture("timer_glass"));
        this.timeGlass.x = 347;
        this.timeGlass.y = 98;
        this.timeGlass.touchable = false;
        this._global._gameS.addChild(this.timeGlass);
        this.pauseBtn = new TNonStateButton(this.uiAtlas.getTexture("pause_btn"));
        this.pauseBtn.pivotX = this.pauseBtn.width / 2;
        this.pauseBtn.pivotY = this.pauseBtn.height / 2;
        var _local5 = 0.9;
        this.pauseBtn.scaleY = 0.9;
        this.pauseBtn.scaleX = _local5;
        this.pauseBtn.x = 30;
        this.pauseBtn.y = 35;
        this.pauseBtn.idx = this.PAUSE_BUTTON_IDX;
        this._global._gameS.addChild(this.pauseBtn);
        this.buttonAr.push(this.pauseBtn);
        this.soundBtn = new TNonStateButton(this.uiAtlas.getTexture("sound_btn"));
        this.soundBtn.pivotX = this.soundBtn.width / 2;
        this.soundBtn.pivotY = this.soundBtn.height / 2;
        _local5 = 0.9;
        this.soundBtn.scaleY = 0.9;
        this.soundBtn.scaleX = _local5;
        this.soundBtn.x = 85;
        this.soundBtn.y = 35;
        this.soundBtn.idx = this.SOUND_BUTTON_IDX;
        this._global._gameS.addChild(this.soundBtn);
        this.buttonAr.push(this.soundBtn);
        this.mute = new cSprite(this.uiAtlas.getTexture("mute"));
        this.mute.pivotX = this.mute.width / 2;
        this.mute.pivotY = this.mute.height / 2;
        this.mute.x = this.soundBtn.x;
        this.mute.y = this.soundBtn.y;
        this.mute.touchable = false;
        this.mute.visible = this._global.MuteSound;
        this._global._gameS.addChild(this.mute);
        this.musicBtn = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("music_icon"));
        this.musicBtn.pivotX = this.musicBtn.width / 2;
        this.musicBtn.pivotY = this.musicBtn.height / 2;
        _local5 = 0.9;
        this.musicBtn.scaleY = 0.9;
        this.musicBtn.scaleX = _local5;
        this.musicBtn.x = 140;
        this.musicBtn.y = 35;
        this.musicBtn.idx = this.MUSIC_BUTTON_IDX;
        this._global._gameS.addChild(this.musicBtn);
        this.buttonAr.push(this.musicBtn);
        this.muteMusic = new cSprite(this.uiAtlas.getTexture("mute"));
        this.muteMusic.pivotX = this.mute.width / 2;
        this.muteMusic.pivotY = this.mute.height / 2;
        this.muteMusic.x = this.musicBtn.x;
        this.muteMusic.y = this.musicBtn.y;
        this.muteMusic.touchable = false;
        this.muteMusic.visible = this._global.MuteMusic;
        this._global._gameS.addChild(this.muteMusic);
        this._HOClass.initUIObjects(this._global.assetManager.getTextureAtlas(this.levelSet[this.currentLevel]));
        if (this._global.ps) goto L2216;
        System.pauseForGCIfCollectionImminent(0);
        System.gc();
        _local3 = XML(new FireConfig());
        _local4 = Texture.fromBitmap(new FireParticle());
        this._global.ps = new PDParticleSystem(_local3, _local4);
        Starling.juggler.add(this._global.ps);
        this._global.ps.emitterX = 20;
        this._global.ps.emitterY = 40;
        this._global._gameS.addChild(this._global.ps);
        this._global.psFound = new PDParticleSystem(_local3, _local4);
        Starling.juggler.add(this._global.psFound);
        this._global.psFound.emitAngleVariance = 3;
        this._global.psFound.speed = 70;
        this._global.psFound.startColorVariance.fromRgb(16777215);
        this._global._gameS.addChild(this._global.psFound);
      L2216:
        TGlobalClass._windowDisplay.initializeWindow(this._global.HIDDEN_OBJECT_ID);
        this._global._gameS.setChildIndex(this._global.psCongrats, this._global._gameS.numChildren - 1);
        this._global.psCongrats.stop();
        Starling.juggler.add(this._global.psCongrats);
        TGlobalClass.currentNotifWindow = 0;
        TGlobalClass._windowDisplay.widgetInfo.text = "";
        var $t0 = !TGlobalClass.extrasMode;
        $s0 = $t0;
        if (!$t0) goto L2353;
        $s0 = TGlobalClass.levelEventXML.day[TGlobalClass.currentPlayedLevel].window.length() > 0;
      L2353:
        if (!$s0) goto L2373;
        $s0 = !this._global.tutorial.textBox.visible;
      L2373:
        if (!$s0) goto L2399;
        TGlobalClass._windowDisplay.showBeginningWindow(TGlobalClass.currentPlayedLevel, TGlobalClass.currentNotifWindow);
      L2399:
        _local1 = 0;
        goto L2431;
      L2406:
        this.buttonAr[_local1].addEventListener(Event.TRIGGERED, this.touchButton);
        _local1++;
      L2431:
        if (_local1 < this.buttonAr.length) goto L2406;
        Starling.current.simulateMultitouch = true;
        TGlobalClass.EnableHintParticle = true;
        this._global.tutorial.checkTut(Tutorial.LEVEL_4, 3, true);
        this._global.tutorial.checkTut(Tutorial.LEVEL_18, 17);
        this._global.tutorial.checkTut(Tutorial.LEVEL_32, 31);
        this.hasHint = true;
        this.gameover = false;
        this.paused = false;
        ready = true;
        this._global._stage.addEventListener(KeyboardEvent.KEY_DOWN, this.KeyDown);
        this.escCounter = 0;
    }

    function KeyDown(arg1:KeyboardEvent):void {
        if (arg1.keyCode != Keyboard.ESCAPE) goto L60;
        var _local2 = this;
        var _local3 = this.escCounter + 1;
        _local2.escCounter = _local3;
        if (!(this.escCounter > 2)) goto L60;
        if (!this._HOClass) goto L60;
        this._HOClass.totalItems = 0;
      L60:
    }

    function touchBackground(arg1:TouchEvent):void {
        if (!arg1.getTouch(this._global._game, TouchPhase.ENDED)) goto L37;
        if (this.touchedBackground) goto L37;
        this.onMissClick();
      L37:
    }

    function touchButton(arg1:Event):void {
        var _local2 = int((arg1.currentTarget as TNonStateButton).idx);
        if (_local2 != this.PAUSE_BUTTON_IDX) goto L49;
        this.paused = true;
        TGlobalClass._windowDisplay.showWindow(TWindowState.PAUSE_GAME);
        goto L191;
      L49:
        if (_local2 != this.SOUND_BUTTON_IDX) goto L91;
        this._global.MuteSound = !this._global.MuteSound;
        this.mute.visible = this._global.MuteSound;
        goto L191;
      L91:
        if (_local2 != this.MUSIC_BUTTON_IDX) goto L133;
        this._global.MuteMusic = !this._global.MuteMusic;
        this.muteMusic.visible = this._global.MuteMusic;
        goto L191;
      L133:
        if (_local2 != this.HINT_BUTTON_IDX) goto L191;
        var $t0 = this._global.tutorial.currentDialogueID == Tutorial.LEVEL_5;
        $s0 = $t0;
        if (!$t0) goto L178;
        $s0 = this._global.tutorial.dialogueCounter < 5;
      L178:
        if (!$s0) goto L183;
        return;
      L183:
        this._HOClass.revealItem();
      L191:
    }

    function startLevel():void {
        this.hasWindow = false;
    }

    function dialogueCondition(arg1:int, arg2:int):void {
    }

    function touchContainer(arg1:TouchEvent):void {
        var _local4 = null;
        var _local5 = null;
        var _local6 = null;
        var _local7 = null;
        var _local8 = null;
        var _local9 = null;
        var _local10 = null;
        var _local11 = null;
        var _local12 = NaN;
        var _local13 = null;
        var _local14 = null;
        var _local15 = NaN;
        var _local16 = NaN;
        var _local17 = NaN;
        var _local18 = NaN;
        var _local19 = NaN;
        var _local20 = NaN;
        var _local21 = NaN;
        var _local22 = NaN;
        var _local23 = NaN;
        var _local24 = NaN;
        var _local25 = NaN;
        var _local26 = NaN;
        var _local27 = null;
        var _local2 = arg1.getTouches(this.zoomContainer, TouchPhase.BEGAN);
        if (_local2.length != 2) goto L208;
        this.touchDragged = false;
        _local4 = _local2[0];
        _local5 = _local2[1];
        this.zoomContainerWidth = this.zoomContainer.width;
        this.zoomContainerHeight = this.zoomContainer.height;
        this.bottomDifference = 1024 - this._backgroundWidth - (this.zoomContainer.height - this._backgroundWidth * this.zoomContainer.scaleX);
        goto L304;
      L208:
        if (_local2.length != 1) goto L304;
        this.beganLocX = _local2[0].globalX;
        this.beganLocY = _local2[0].globalY;
        this.touchDragged = false;
        this.zoomContainerWidth = this.zoomContainer.width;
        this.zoomContainerHeight = this.zoomContainer.height;
        this.bottomDifference = 1024 - this._backgroundWidth - (this.zoomContainer.height - this._backgroundWidth * this.zoomContainer.scaleX);
      L304:
        var _local3 = arg1.getTouches(this.zoomContainer, TouchPhase.MOVED);
        if (_local3.length != 2) goto L1042;
        this.touchDragged = true;
        if (!this._HOClass.onScaleBack) goto L349;
        return;
      L349:
        _local4 = _local3[0];
        _local5 = _local3[1];
        _local6 = _local4.getLocation(this.zoomContainer.parent);
        _local7 = _local4.getPreviousLocation(this.zoomContainer.parent);
        _local8 = _local5.getLocation(this.zoomContainer.parent);
        _local9 = _local5.getPreviousLocation(this.zoomContainer.parent);
        _local10 = _local6.subtract(_local8);
        _local11 = _local7.subtract(_local9);
        _local12 = Number(_local10.length / _local11.length);
        this.zoomContainer.scaleX = this.zoomContainer.scaleX * _local12;
        this.zoomContainer.scaleY = this.zoomContainer.scaleY * _local12;
        if (!(this.zoomContainer.scaleX < 1)) goto L540;
        this.zoomContainer.scaleX = 1;
        this.zoomContainer.scaleY = 1;
      L540:
        if (!(this.zoomContainer.scaleX > 2)) goto L571;
        this.zoomContainer.scaleX = 2;
        this.zoomContainer.scaleY = 2;
      L571:
        _local13 = _local4.getPreviousLocation(this.zoomContainer);
        _local14 = _local5.getPreviousLocation(this.zoomContainer);
        _local15 = Number(this.zoomContainer.pivotX);
        _local16 = Number(this.zoomContainer.pivotY);
        this.zoomContainer.pivotX = (_local13.x + _local14.x) * 0.5;
        this.zoomContainer.pivotY = (_local13.y + _local14.y) * 0.5;
        _local17 = Number(this.zoomContainer.x);
        _local18 = Number(this.zoomContainer.y);
        this.zoomContainer.x = (_local6.x + _local8.x) * 0.5;
        this.zoomContainer.y = (_local6.y + _local8.y) * 0.5;
        this.zoomContainerWidth = this.zoomContainer.width;
        this.zoomContainerHeight = this.zoomContainer.height;
        this.bottomDifference = 1024 - this._backgroundWidth - (this.zoomContainer.height - this._backgroundWidth * this.zoomContainer.scaleX);
        _local19 = Number(600 - this.zoomContainerWidth);
        _local20 = Number(0);
        _local21 = Number(this._background.parent.localToGlobal(this._background.bounds.topLeft).x);
        _local22 = Number(this._background.parent.localToGlobal(this._background.bounds.bottomRight).x);
        _local23 = Number(this._background.parent.localToGlobal(this._background.bounds.topLeft).y);
        _local24 = Number(this._background.parent.localToGlobal(this._background.bounds.bottomRight).y);
        if (!(_local21 > 1024 - this._backgroundWidth)) goto L950;
        this.zoomContainer.pivotX = 1024 - this._backgroundWidth;
        this.zoomContainer.x = 1024 - this._backgroundWidth;
        goto L979;
      L950:
        if (!(_local22 < 1024)) goto L979;
        this.zoomContainer.pivotX = 1024;
        this.zoomContainer.x = 1024;
      L979:
        if (!(_local23 > 0)) goto L1009;
        this.zoomContainer.pivotY = 0;
        this.zoomContainer.y = 0;
        goto L1038;
      L1009:
        if (!(_local24 < 600)) goto L1038;
        this.zoomContainer.pivotY = 600;
        this.zoomContainer.y = 600;
      L1038:
        goto L1434;
      L1042:
        if (_local3.length != 1) goto L1434;
        _local25 = Number(_local3[0].globalX);
        _local26 = Number(_local3[0].globalY);
        var $t0 = Math.abs(_local25 - this.beganLocX) > 25;
        $s0 = $t0;
        if ($t0) goto L1116;
        $s0 = Math.abs(_local26 - this.beganLocY) > 25;
      L1116:
        if (!$s0) goto L1125;
        this.touchDragged = true;
      L1125:
        if (!this._HOClass.onScaleBack) goto L1137;
        return;
      L1137:
        _local27 = _local3[0].getMovement(this.zoomContainer.parent);
        this.zoomContainer.x = this.zoomContainer.x + _local27.x;
        this.zoomContainer.y = this.zoomContainer.y + _local27.y;
        _local21 = Number(this._background.parent.localToGlobal(this._background.bounds.topLeft).x);
        _local22 = Number(this._background.parent.localToGlobal(this._background.bounds.bottomRight).x);
        _local23 = Number(this._background.parent.localToGlobal(this._background.bounds.topLeft).y);
        _local24 = Number(this._background.parent.localToGlobal(this._background.bounds.bottomRight).y);
        if (!(_local21 > 1024 - this._backgroundWidth)) goto L1344;
        this.zoomContainer.x = this.zoomContainer.x - _local27.x;
        goto L1373;
      L1344:
        if (!(_local22 < 1024)) goto L1373;
        this.zoomContainer.x = this.zoomContainer.x - _local27.x;
      L1373:
        if (!(_local23 > 0)) goto L1405;
        this.zoomContainer.y = this.zoomContainer.y - _local27.y;
        goto L1434;
      L1405:
        if (!(_local24 < 600)) goto L1434;
        this.zoomContainer.y = this.zoomContainer.y - _local27.y;
      L1434:
    }

    function onMissClick():void {
        this._global._soundAsset.playSFX("SFX_wrong_click");
        this._HOClass.soundCombo = 0;
        var $t0 = this._HOClass;
        var _local1 = $t0;
        var _local2 = $t0.missClickCount + 1;
        _local1.missClickCount = _local2;
        if (!(this._HOClass.missClickCount > this._HOClass.allowedMissClick)) goto L80;
        this._HOClass.missClick = false;
        this._HOClass.missClickCount = 0;
      L80:
    }

    function useHint():void {
        this.hasHint = false;
        this.hintTimer = 0;
        this.hintFillSprite.clipRect.y = this.HINT_HEIGHT;
        this.hintText.visible = false;
    }

    function updateMoney():void {
        this.bonusMoney = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(this.bonusMoney) + 5 * (this.currentLevel + 1) + 5);
        if (!TGlobalClass.extrasMode) goto L86;
        this.moneyText.text = "$ " + this._global._maxCrypto.STI(this.bonusMoney);
        goto L318;
      L86:
        $s0 = this.moneyText;
        if (!TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel]) goto L120;
        $s1 = "Exp.: ";
        goto L124;
      L120:
        $s1 = "Goal: ";
      L124:
        $s1 = $s1 + (this._HOClass.itemFoundCount + 1) + "/";
        if (!TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel]) goto L170;
        $s2 = this.expertTarget;
        goto L175;
      L170:
        $s2 = this.goalTarget;
      L175:
        $s0.text = $s1 + $s2;
        var $t0 = !TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel];
        $s0 = $t0;
        if (!$t0) goto L219;
        $s0 = this._HOClass.itemFoundCount + 1 == this.goalTarget;
      L219:
        if (!$s0) goto L300;
        TGlobalClass._windowDisplay.showGoalIconSmall();
        this._global._soundAsset.playSFX("goal_reached");
        this.moneyText.text = "Exp.: " + (this._HOClass.itemFoundCount + 1) + "/" + this.expertTarget;
        TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] = true;
        goto L318;
      L300:
        if (this._HOClass.itemFoundCount + 1 != this.expertTarget) goto L318;
      L318:
    }

    override function Resume():void {
        if (TGlobalClass._windowDisplay._blackScreen.visible) goto L23;
        this.paused = false;
      L23:
    }

    override function Pause():void {
        if (TGlobalClass._windowDisplay._blackScreen.visible) goto L39;
        this.paused = true;
        TGlobalClass._windowDisplay.showWindow(TWindowState.PAUSE_GAME);
      L39:
    }

    function getTween():int {
        var _local1 = 0;
        goto L43;
      L9:
        if (this.tweenAr[_local1]._time != this.tweenAr[_local1].duration()) goto L41;
        goto L55;
      L41:
        _local1++;
      L43:
        if (_local1 < this.tweenAr.length) goto L9;
      L55:
        return _local1;
    }
}
}
