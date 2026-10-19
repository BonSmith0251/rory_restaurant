package {
class MiniGameMakeDessert extends Screen {
    var _global:Global;
    var hasWindow:Boolean;
    var paused:Boolean;
    var dessertMakingAtlas:TextureAtlas;
    var goalTarget:int;
    var expertTarget:int;
    var froyoMadeCount:int;
    const CUP:int = 1;
    const YOGURT:int = 2;
    const SYRUP:int = 3;
    const TOPPING:int = 4;
    const BEVERAGE:int = 5;
    const PAUSE_BUTTON:int = 1;
    const SOUND_BUTTON:int = 2;
    const MUSIC_BUTTON:int = 3;
    var initVector:Boolean;
    var _background:cSprite;
    var onPlateObj:cSprite;
    var tile:Quad;
    var preview:cSprite;
    var onPlateObjAr:Vector.<cSprite>;
    var tileAr:Vector.<Quad>;
    var previewAr:Vector.<cSprite>;
    var pan:cSprite;
    var previewPaper:cSprite;
    var customer:cSprite;
    var custBG:cSprite;
    var custGlass:cSprite;
    var custFill:cSprite;
    var custFillSprite:Sprite;
    var heart:cSprite;
    var pauseBtn:TNonStateButton;
    var soundBtn:TNonStateButton;
    var musicBtn:TNonStateButton;
    var mute:cSprite;
    var muteMusic:cSprite;
    var allButtonAr:Vector.<TNonStateButton>;
    var timeBG:cSprite;
    var timeFill:cSprite;
    var timeGlass:cSprite;
    var timeFillSprite:Sprite;
    var TIMER_WIDTH:Number;
    var CUST_FILL_WIDTH:Number;
    var scorePreview:cSprite;
    var scorePreviewText:ExtendTextField;
    var goalOrExpertPreviewText:ExtendTextField;
    var goalOrExpertText:ExtendTextField;
    var livesText:ExtendTextField;
    var bigInfoText:ExtendTextField;
    var orderInPlace:Boolean;
    var orderList:Vector.<int>;
    var choosenSet:Vector.<int>;
    var currentLayer:int;
    var lives:int;
    var level:int;
    var earning:String;
    var price:int;
    var totalTime:int;
    var time:int;
    var timesUp:Boolean;
    var patience:int;
    var totalPatience:int;
    var timeline:TimelineLite;
    var posX:Vector.<Number>;
    var posY:Vector.<Number>;
    var itemName:Vector.<String>;
    var type:Vector.<int>;
    var availableSet:Vector.<Vector.<int>>;
    var setsLevel:Vector.<int>;
    var itemPrice:Vector.<int>;

    function MiniGameMakeDessert():* {
        super();
    }

    override function Start():void {
        super.Start();
        this._global.currentSceneID = this._global.MAKE_DESSERT_ID;
        this.level = (TGlobalClass.currentPlayedLevel + 1) / 14 + 1;
        if (!TGlobalClass.extrasMode) goto L66;
        this.level = 1;
        this.lives = 3;
      L66:
        if (this.initVector) goto L252;
        this.onPlateObjAr = new Vector.<cSprite>();
        this.tileAr = new Vector.<Quad>();
        this.previewAr = new Vector.<cSprite>();
        this.posX = new Vector.<Number>();
        this.posY = new Vector.<Number>();
        this.itemName = new Vector.<String>();
        this.type = new Vector.<int>();
        this.orderList = new Vector.<int>();
        this.availableSet = new Vector.<Vector.<int>>();
        this.setsLevel = new Vector.<int>();
        this.itemPrice = new Vector.<int>();
        this.allButtonAr = new Vector.<TNonStateButton>();
        this.initVector = true;
      L252:
        this._global.assetManager.enqueue(this._global.graphicsPath + "make_dessert.ean", this._global.graphicsPath + "make_dessert.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + "Make_Dessert_BG.eag");
        this._global.assetManager.enqueue("data/MakeDessert.xml");
        this._global.assetManager.loadQueue(function(f1_arg1:Number):void {
            if (f1_arg1 != 1) goto L21;
            readXML();
            initialize();
          L21:
        });
    }

    function readXML():void {
        var _local2 = 0;
        var _local1 = this._global.assetManager.getXml("MakeDessert");
        if (!(this.itemName.length <= 0)) goto L173;
        _local2 = 0;
        goto L160;
      L42:
        this.posX.push(_local1.item[_local2].attribute("posX"));
        this.posY.push(_local1.item[_local2].attribute("posY"));
        this.itemName.push(_local1.item[_local2].attribute("name"));
        this.itemPrice.push(_local1.item[_local2].attribute("worth"));
        this.type.push(_local1.item[_local2].attribute("type"));
        _local2++;
      L160:
        if (_local2 < _local1.item.length()) goto L42;
      L173:
        this.setLevelSet();
    }

    function setLevelSet():void {
        var _local2 = null;
        var _local1 = this._global.assetManager.getXml("MakeDessert");
        goto L36;
      L27:
        this.availableSet.pop();
      L36:
        if (this.availableSet.length) goto L27;
        var _local3 = 0;
        goto L129;
      L54:
        if (!(_local1.sets[_local3].attribute("level") <= this.level)) goto L127;
        _local2 = Vector.<int>(_local1.sets[_local3].attribute("type").toString().split(","));
        this.availableSet.push(_local2);
      L127:
        _local3++;
      L129:
        if (_local3 < _local1.sets.length()) goto L54;
    }

    override function Kill():void {
        var _local1 = 0;
        this.timeline.clear();
        this.timeline = null;
        _local1 = 0;
        goto L50;
      L25:
        this.onPlateObjAr[_local1].removeFromParent(true);
        this.onPlateObjAr[_local1] = null;
        _local1++;
      L50:
        if (_local1 < this.onPlateObjAr.length) goto L25;
        this.onPlateObjAr.length = 0;
        _local1 = 0;
        goto L103;
      L78:
        this.tileAr[_local1].removeFromParent(true);
        this.tileAr[_local1] = null;
        _local1++;
      L103:
        if (_local1 < this.tileAr.length) goto L78;
        this.tileAr.length = 0;
        _local1 = 0;
        goto L178;
      L131:
        this.allButtonAr[_local1].removeEventListener(Event.TRIGGERED, this.touchButton);
        this.allButtonAr[_local1].removeFromParent(true);
        this.allButtonAr[_local1] = null;
        _local1++;
      L178:
        if (_local1 < this.allButtonAr.length) goto L131;
        this.allButtonAr.length = 0;
        _local1 = 0;
        goto L231;
      L206:
        this.previewAr[_local1].removeFromParent(true);
        this.previewAr[_local1] = null;
        _local1++;
      L231:
        if (_local1 < this.previewAr.length) goto L206;
        this.previewAr.length = 0;
        goto L265;
      L256:
        this.availableSet.pop();
      L265:
        if (this.availableSet.length) goto L256;
        this._background.removeFromParent(true);
        this._background = null;
        this.onPlateObj.removeFromParent(true);
        this.onPlateObj = null;
        this.tile.removeFromParent(true);
        this.tile = null;
        this.preview.removeFromParent(true);
        this.preview = null;
        this.previewPaper.removeFromParent(true);
        this.previewPaper = null;
        this.pan.removeFromParent(true);
        this.pan = null;
        this.customer.removeFromParent(true);
        this.customer = null;
        this.custBG.removeFromParent(true);
        this.custBG = null;
        this.custFill.removeFromParent(true);
        this.custFill = null;
        this.custGlass.removeFromParent(true);
        this.custGlass = null;
        this.custFillSprite.removeFromParent(true);
        this.custFillSprite = null;
        this.heart.removeFromParent(true);
        this.heart = null;
        this.pauseBtn.removeFromParent(true);
        this.pauseBtn = null;
        this.soundBtn.removeFromParent(true);
        this.soundBtn = null;
        this.mute.removeFromParent(true);
        this.mute = null;
        this.musicBtn.removeFromParent(true);
        this.musicBtn = null;
        this.muteMusic.removeFromParent(true);
        this.muteMusic = null;
        this.timeBG.removeFromParent(true);
        this.timeBG = null;
        this.timeFill.removeFromParent(true);
        this.timeFill = null;
        this.timeGlass.removeFromParent(true);
        this.timeGlass = null;
        this.timeFillSprite.removeFromParent(true);
        this.timeFillSprite = null;
        this.scorePreview.removeFromParent(true);
        this.scorePreview = null;
        this.scorePreviewText.removeFromParent(true);
        this.scorePreviewText = null;
        this.goalOrExpertPreviewText.removeFromParent(true);
        this.goalOrExpertPreviewText = null;
        this.goalOrExpertText.removeFromParent(true);
        this.goalOrExpertText = null;
        if (!this.livesText) goto L643;
        this.livesText.removeFromParent(true);
      L643:
        this.livesText = null;
        if (!this.bigInfoText) goto L665;
        this.bigInfoText.removeFromParent(true);
      L665:
        this.bigInfoText = null;
        this.dessertMakingAtlas.dispose();
        this.dessertMakingAtlas = null;
        this._global.assetManager.purge();
        TGlobalClass._windowDisplay.hideAllWindows(this._global.currentSceneID);
        TGlobalClass._windowDisplay.kill(this._global.currentSceneID);
        ready = false;
    }

    function dialogueCondition(arg1:int, arg2:int):void {
    }

    override function Update():void {
        i = 0;
        if (ready) goto L20;
        return;
      L20:
        if (!this.hasWindow) goto L29;
        return;
      L29:
        if (!this.paused) goto L38;
        return;
      L38:
        if (this._global.tutorial.currentDialogueID != Tutorial.LEVEL_11) goto L59;
        return;
      L59:
        var $t0 = !this.timesUp;
        $s0 = $t0;
        if (!$t0) goto L84;
        $s0 = !this._global.tutorial.textBox.visible;
      L84:
        if (!$s0) goto L149;
        var _local2 = this;
        var _local3 = this.time - 1;
        _local2.time = _local3;
        this.timeFillSprite.clipRect.x = this.time / this.totalTime * this.TIMER_WIDTH - this.TIMER_WIDTH;
        if (!(this.time <= 0)) goto L149;
        this.timesUp = true;
      L149:
        var $t1 = Boolean(this.orderInPlace);
        $s0 = $t1;
        if (!$t1) goto L174;
        $s0 = !this._global.tutorial.textBox.visible;
      L174:
        if (!$s0) goto L445;
        _local2 = this;
        _local3 = this.patience - 1;
        _local2.patience = _local3;
        this.custFillSprite.clipRect.x = this.patience / this.totalPatience * this.CUST_FILL_WIDTH - this.CUST_FILL_WIDTH;
        if (!(this.patience <= 0)) goto L445;
        if (!TGlobalClass.extrasMode) goto L352;
        _local2 = this;
        _local3 = this.lives - 1;
        _local2.lives = _local3;
        this.livesText.text = "LIVES x" + this.lives;
        _local2 = 0;
        this.livesText.scaleY = 0;
        this.livesText.scaleX = _local2;
        if (!(this.lives <= 0)) goto L312;
        this.timesUp = true;
      L312:
        TweenLite.to(this.livesText, 1, {"scaleX": 1, "scaleY": 1, "ease": Back.easeOut, "onComplete": function():void {
            if (!(lives <= 0)) goto L25;
            TGlobalClass._windowDisplay.showWindow(TWindowState.DESSERT_LEVEL_FAILED_EXTRAS);
          L25:
        }});
      L352:
        this._global._soundAsset.playSFX("bell_dring");
        this.orderInPlace = false;
        this.tweenOut();
        i = 0;
        goto L430;
      L386:
        if (!this.previewAr[i].visible) goto L420;
        this.previewAr[i].visible = false;
      L420:
        i = i + 1;
      L430:
        if (i < this.previewAr.length) goto L386;
      L445:
    }

    override function Resume():void {
        if (TGlobalClass._windowDisplay._blackScreen.visible) goto L44;
        this.paused = false;
        super.Resume();
        if (!this.timeline) goto L44;
        this.timeline.resume();
      L44:
    }

    override function Pause():void {
        if (TGlobalClass._windowDisplay._blackScreen.visible) goto L55;
        this.paused = true;
        if (!this.timeline) goto L39;
        this.timeline.pause();
      L39:
        TGlobalClass._windowDisplay.showWindow(TWindowState.PAUSE_GAME);
      L55:
    }

    function initialize():void {
        var _local2 = 0;
        var _local3 = 0;
        this._global.ReportGameState(this._global.GAMESTATE_PLAYING);
        if (TGlobalClass.musicPlayingIdx == 3) goto L57;
        this._global._soundAsset.playMusic("sunny-bliss");
        TGlobalClass.musicPlayingIdx = 3;
      L57:
        if (TGlobalClass.extrasMode) goto L96;
        GALib.TrackEvent(GALib.EVENT_START_LEVEL_STORY, TGlobalClass.currentPlayedLevel + 1, "DESSERT MAKING");
        goto L116;
      L96:
        GALib.TrackEvent(GALib.EVENT_START_LEVEL_EXTRAS, this.level, "DESSERT MAKING");
      L116:
        this.timeline = new TimelineLite();
        this.earning = "a";
        this.froyoMadeCount = 0;
        this.timesUp = false;
        if (TGlobalClass.extrasMode) goto L245;
        this.time = TGlobalClass.levelSetXML.day[TGlobalClass.currentPlayedLevel].time * 30;
        this.totalTime = this.time;
        this.goalTarget = TGlobalClass.levelSetXML.day[TGlobalClass.currentPlayedLevel].goal;
        this.expertTarget = TGlobalClass.levelSetXML.day[TGlobalClass.currentPlayedLevel].expert;
        goto L304;
      L245:
        this.time = TGlobalClass.levelSetXML.dessert[this.level - 1].time * 30;
        this.totalTime = this.time;
        this.goalTarget = TGlobalClass.levelSetXML.dessert[this.level - 1].goal;
      L304:
        this.dessertMakingAtlas = this._global.assetManager.getTextureAtlas("make_dessert");
        this._background = new cSprite(this._global.assetManager.getTexture("Make_Dessert_BG"));
        this._background.touchable = false;
        this._background.blendMode = BlendMode.NONE;
        this._global._gameS.addChild(this._background);
        this.pauseBtn = new TNonStateButton(this.dessertMakingAtlas.getTexture("pause_button"));
        var _local4 = 0.7;
        this.pauseBtn.scaleY = 0.7;
        this.pauseBtn.scaleX = _local4;
        this.pauseBtn.x = 5;
        this.pauseBtn.y = 5;
        this.pauseBtn.idx = this.PAUSE_BUTTON;
        this._global._gameS.addChild(this.pauseBtn);
        this.allButtonAr.push(this.pauseBtn);
        this.soundBtn = new TNonStateButton(this.dessertMakingAtlas.getTexture("sound_button"));
        _local4 = 0.7;
        this.soundBtn.scaleY = 0.7;
        this.soundBtn.scaleX = _local4;
        this.soundBtn.x = 48;
        this.soundBtn.y = 5;
        this.soundBtn.idx = this.SOUND_BUTTON;
        this._global._gameS.addChild(this.soundBtn);
        this.allButtonAr.push(this.soundBtn);
        this.mute = new cSprite(this.dessertMakingAtlas.getTexture("mute_icon"));
        this.mute.pivotX = this.mute.width / 2;
        this.mute.pivotY = this.mute.height / 2;
        this.mute.x = this.soundBtn.x + this.soundBtn.width / 2 - this.mute.pivotX;
        this.mute.y = this.soundBtn.y + this.soundBtn.height / 2 - this.mute.pivotY;
        this.mute.touchable = false;
        this.mute.visible = this._global.MuteSound;
        this._global._gameS.addChild(this.mute);
        this.musicBtn = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("music_icon"));
        _local4 = 0.7;
        this.musicBtn.scaleY = 0.7;
        this.musicBtn.scaleX = _local4;
        this.musicBtn.x = 91;
        this.musicBtn.y = 5;
        this.musicBtn.idx = this.MUSIC_BUTTON;
        this._global._gameS.addChild(this.musicBtn);
        this.allButtonAr.push(this.musicBtn);
        this.muteMusic = new cSprite(this.dessertMakingAtlas.getTexture("mute_icon"));
        this.muteMusic.pivotX = this.muteMusic.width / 2;
        this.muteMusic.pivotY = this.muteMusic.height / 2;
        this.muteMusic.x = this.musicBtn.x + this.musicBtn.width / 2 - this.muteMusic.pivotX;
        this.muteMusic.y = this.musicBtn.y + this.musicBtn.height / 2 - this.muteMusic.pivotY;
        this.muteMusic.touchable = false;
        this.muteMusic.visible = this._global.MuteMusic;
        this._global._gameS.addChild(this.muteMusic);
        this.scorePreview = new cSprite(this.dessertMakingAtlas.getTexture("score_board"));
        this.scorePreview.x = 730;
        this.scorePreview.touchable = false;
        this._global._gameS.addChild(this.scorePreview);
        this.scorePreviewText = new ExtendTextField(150, 50, "$ 0", "jenna_sue", 35);
        this.scorePreviewText.x = 870;
        this.scorePreviewText.touchable = false;
        this._global._gameS.addChild(this.scorePreviewText);
        this.goalOrExpertPreviewText = new ExtendTextField(150, 50, "$ " + this.goalTarget.toString(), "jenna_sue", 35, 16777215);
        this.goalOrExpertPreviewText.x = 725;
        this.goalOrExpertPreviewText.hAlign = "right";
        this.goalOrExpertPreviewText.touchable = false;
        this._global._gameS.addChild(this.goalOrExpertPreviewText);
        this.goalOrExpertText = new ExtendTextField(150, 50, "Goal:", "jenna_sue", 25, 16777215);
        this.goalOrExpertText.x = 740;
        this.goalOrExpertText.hAlign = "left";
        this.goalOrExpertText.touchable = false;
        this.goalOrExpertText.idx = 1;
        this._global._gameS.addChild(this.goalOrExpertText);
        var $t0 = !TGlobalClass.extrasMode;
        $s0 = $t0;
        if (!$t0) goto L1308;
        $s0 = Boolean(TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel]);
      L1308:
        if (!$s0) goto L1346;
        this.goalOrExpertPreviewText.text = "$ " + this.expertTarget;
        this.goalOrExpertText.text = "EXPERT:";
        this.goalOrExpertText.idx = 2;
      L1346:
        this.timeBG = new cSprite(this.dessertMakingAtlas.getTexture("timer_BG"));
        this.timeBG.x = 200;
        this.timeBG.y = 0;
        this.timeBG.touchable = false;
        this._global._gameS.addChild(this.timeBG);
        this.timeFill = new cSprite(this.dessertMakingAtlas.getTexture("timer_fill"));
        this.TIMER_WIDTH = this.timeFill.width;
        this.timeFill.touchable = false;
        this.timeFillSprite = new Sprite();
        this.timeFillSprite.addChild(this.timeFill);
        this.timeFillSprite.x = this.timeBG.x + 73;
        this.timeFillSprite.y = 4;
        this._global._gameS.addChild(this.timeFillSprite);
        this.timeFillSprite.clipRect = new Rectangle(0, 0, this.timeFill.width, this.timeFill.height);
        this.timeGlass = new cSprite(this.dessertMakingAtlas.getTexture("timer_glass"));
        this.timeGlass.x = this.timeBG.x + 49;
        this.timeGlass.y = -7;
        this.timeGlass.touchable = false;
        this._global._gameS.addChild(this.timeGlass);
        this.previewPaper = new cSprite(this.dessertMakingAtlas.getTexture("preview_list"));
        this.previewPaper.touchable = false;
        this.previewPaper.x = 1024 - this.previewPaper.width;
        this.previewPaper.y = 50;
        this._global._gameS.addChild(this.previewPaper);
        this.customer = new cSprite(this.dessertMakingAtlas.getTexture("char_1"));
        this.customer.touchable = false;
        this.customer.pivotY = this.customer.height;
        this.customer.y = 255;
        this.customer.x = 4294967046;
        this._global._gameS.addChild(this.customer);
        this.custBG = new cSprite(this.dessertMakingAtlas.getTexture("cust_bar_BG"));
        this.custBG.touchable = false;
        this.custBG.y = 240;
        this.custBG.x = 4294967046;
        this._global._gameS.addChild(this.custBG);
        this.heart = new cSprite(this.dessertMakingAtlas.getTexture("heart_cust"));
        this.heart.pivotX = this.heart.width / 2;
        this.heart.pivotY = this.heart.height / 2;
        this.heart.x = 4294967046;
        this.heart.y = this.custBG.y + 22;
        this.heart.touchable = false;
        this._global._gameS.addChild(this.heart);
        this.custFillSprite = new Sprite();
        this.custFill = new cSprite(this.dessertMakingAtlas.getTexture("cust_fill"));
        this.CUST_FILL_WIDTH = this.custFill.width;
        this.custFillSprite.addChild(this.custFill);
        this.custFillSprite.x = 4294967046;
        this.custFillSprite.y = this.custBG.y + 15;
        this._global._gameS.addChild(this.custFillSprite);
        this.custFillSprite.clipRect = new Rectangle(0, 0, this.custFill.width, this.custFill.height);
        this.custGlass = new cSprite(this.dessertMakingAtlas.getTexture("cust_glass"));
        this.custGlass.x = 4294967046;
        this.custGlass.y = this.custBG.y + 15;
        this.custGlass.touchable = false;
        this._global._gameS.addChild(this.custGlass);
        this.pan = new cSprite(this.dessertMakingAtlas.getTexture("pan"));
        this.pan.touchable = false;
        this.pan.pivotX = this.pan.width / 2;
        this.pan.pivotY = this.pan.height / 2;
        this.pan.x = 4294966296;
        this.pan.y = 300;
        this._global._gameS.addChild(this.pan);
        var _local1 = 0;
        goto L2371;
      L2264:
        this.onPlateObj = new cSprite(this.dessertMakingAtlas.getTexture("cup_paper"));
        this.onPlateObj.pivotX = this.onPlateObj.width / 2;
        this.onPlateObj.pivotY = this.onPlateObj.height / 2;
        this.onPlateObj.x = 4294967096;
        this.onPlateObj.touchable = false;
        this._global._gameS.addChild(this.onPlateObj);
        this.onPlateObjAr.push(this.onPlateObj);
        _local1++;
      L2371:
        if (_local1 < 15) goto L2264;
        _local1 = 0;
        goto L2535;
      L2385:
        this.tile = new Quad(100, 100);
        this.tile.pivotX = this.tile.width / 2;
        this.tile.pivotY = this.tile.height / 2;
        this.tile.x = 132 + _local2 * 126;
        this.tile.y = 426 + _local3 * 115;
        this.tile.alpha = 0;
        this.tile.name = "" + _local1;
        this.tile.useHandCursor = true;
        this._global._gameS.addChild(this.tile);
        this.tileAr.push(this.tile);
        _local2++;
        if (_local2 != 7) goto L2533;
        _local2 = 0;
        _local3++;
      L2533:
        _local1++;
      L2535:
        if (_local1 < 14) goto L2385;
        _local1 = 0;
        goto L2664;
      L2549:
        this.preview = new cSprite(this.dessertMakingAtlas.getTexture("cup_paper"));
        this.preview.pivotX = this.preview.width / 2;
        this.preview.pivotY = this.preview.height / 2;
        this.preview.x = 4294966996;
        this.preview.idx = _local1;
        this.preview.touchable = false;
        this.previewAr.push(this.preview);
        this._global._gameS.addChild(this.preview);
        _local1++;
      L2664:
        if (_local1 < 15) goto L2549;
        if (!TGlobalClass.extrasMode) goto L2764;
        this.livesText = new ExtendTextField(250, 70, "LIVES x" + this.lives, "KomikandyGold", 20, 16777215);
        this.livesText.pivotX = 125;
        this.livesText.pivotY = 35;
        this.livesText.x = 425;
        this.livesText.y = 65;
        this._global._gameS.addChild(this.livesText);
      L2764:
        TGlobalClass._windowDisplay.initializeWindow(this._global.MAKE_DESSERT_ID);
        if (!TGlobalClass.extrasMode) goto L2875;
        this.bigInfoText = new ExtendTextField(600, 300, "", "bromine", 70, 16777215);
        this.bigInfoText.pivotX = 300;
        this.bigInfoText.pivotY = 150;
        this.bigInfoText.x = 4294966896;
        this.bigInfoText.y = 300;
        this._global._gameS.addChild(this.bigInfoText);
      L2875:
        this._global._gameS.setChildIndex(this._global.psCongrats, this._global._gameS.numChildren - 1);
        this._global.psCongrats.stop();
        Starling.juggler.add(this._global.psCongrats);
        TGlobalClass.currentNotifWindow = 0;
        if (TGlobalClass.extrasMode) goto L2976;
        TGlobalClass._windowDisplay.showBeginningWindow(TGlobalClass.currentPlayedLevel, TGlobalClass.currentNotifWindow);
        goto L2981;
      L2976:
        this.showCurrentLevel();
      L2981:
        _local1 = 0;
        goto L3013;
      L2988:
        this.tileAr[_local1].addEventListener(TouchEvent.TOUCH, this.touchIngredient);
        _local1++;
      L3013:
        if (_local1 < this.tileAr.length) goto L2988;
        _local1 = 0;
        goto L3057;
      L3032:
        this.allButtonAr[_local1].addEventListener(Event.TRIGGERED, this.touchButton);
        _local1++;
      L3057:
        if (_local1 < this.allButtonAr.length) goto L3032;
        this.orderInPlace = false;
        if (TGlobalClass.extrasMode) goto L3123;
        this._global.tutorial.checkTut(Tutorial.LEVEL_11, 10);
        this._global.tutorial.checkTut(Tutorial.LEVEL_25, 24, true);
      L3123:
        this.hasWindow = true;
        this.paused = false;
        ready = true;
    }

    function touchButton(arg1:Event):void {
        var _local2 = int((arg1.currentTarget as TNonStateButton).idx);
        if (_local2 != this.PAUSE_BUTTON) goto L49;
        this.paused = true;
        TGlobalClass._windowDisplay.showWindow(TWindowState.PAUSE_GAME);
        goto L129;
      L49:
        if (_local2 != this.SOUND_BUTTON) goto L91;
        this._global.MuteSound = !this._global.MuteSound;
        this.mute.visible = this._global.MuteSound;
        goto L129;
      L91:
        if (_local2 != this.MUSIC_BUTTON) goto L129;
        this._global.MuteMusic = !this._global.MuteMusic;
        this.muteMusic.visible = this._global.MuteMusic;
      L129:
    }

    function startLevel():void {
        this.hasWindow = false;
        this.tweenIn();
        this.timeline.vars.onComplete = this.checkTimesUp;
    }

    function showCurrentLevel():void {
        this.bigInfoText.text = "LEVEL: " + this.level;
        TGlobalClass._windowDisplay._blackScreen.visible = true;
        this.bigInfoText.x = 4294966896;
        TweenLite.to(this.bigInfoText, 1, {"x": 512, "ease": Expo.easeOut, "onComplete": function():void {
            TweenLite.to(bigInfoText, 1, {"x": 1424, "ease": Expo.easeIn, "onComplete": function():void {
                TGlobalClass._windowDisplay._blackScreen.visible = false;
                if (!hasWindow) goto L34;
                startLevel();
                hasWindow = false;
              L34:
            }});
        }});
    }

    function setOrder():void {
        var _local1 = 0;
        var _local2 = 0;
        var _local3 = 0;
        var _local4 = false;
        var _local5 = null;
        var _local8 = 0;
        var _local9 = 0;
        var $t0 = new Vector.<int>(4);
        $t0[0] = 4;
        $t0[1] = 5;
        $t0[2] = 11;
        $t0[3] = 12;
        var _local6 = $t0;
        var $t1 = new Vector.<int>(2);
        $t1[0] = 6;
        $t1[1] = 13;
        var _local7 = $t1;
        this.orderList.length = 0;
        this.currentLayer = 0;
        this.price = 0;
        this.choosenSet = this.availableSet[TGlobalClass.RandomRange(0, this.availableSet.length - 1)];
        this.totalPatience = 2;
        this.custFillSprite.clipRect.x = 0;
        if (!(this.level >= 4)) goto L278;
        if (this.choosenSet.indexOf(5) != -1) goto L278;
        var $t2 = int(TGlobalClass.RandomRange(0, 100));
        _local9 = $t2;
        if (!($t2 <= 100)) goto L237;
        this.choosenSet.push(5);
      L237:
        if (!(this.level >= 5)) goto L278;
        var $t3 = int(TGlobalClass.RandomRange(0, 100));
        _local9 = $t3;
        if (!($t3 <= 100)) goto L278;
        this.choosenSet.push(5);
      L278:
        if (this._global.tutorial.currentDialogueID != Tutorial.LEVEL_11) goto L357;
        var $t4 = new Vector.<int>(5);
        $t4[0] = 1;
        $t4[1] = 2;
        $t4[2] = 3;
        $t4[3] = 4;
        $t4[4] = 5;
        this.choosenSet = $t4;
      L357:
        var _local10 = 0;
        goto L963;
      L365:
        if (this.choosenSet[_local10] != this.CUP) goto L473;
        var $t5 = new Vector.<int>(2);
        $t5[0] = 0;
        $t5[1] = 7;
        _local5 = $t5;
        if (!(this.level < 5)) goto L441;
        this.orderList.push(0);
        goto L469;
      L441:
        this.orderList.push(_local5[TGlobalClass.RandomRange(0, _local5.length - 1)]);
      L469:
        goto L950;
      L473:
        if (this.choosenSet[_local10] != this.YOGURT) goto L612;
        var $t6 = new Vector.<int>(3);
        $t6[0] = 1;
        $t6[1] = 2;
        $t6[2] = 8;
        _local5 = $t6;
        if (this._global.tutorial.currentDialogueID != Tutorial.LEVEL_11) goto L580;
        var $t7 = new Vector.<int>(1);
        $t7[0] = 1;
        _local5 = $t7;
      L580:
        this.orderList.push(_local5[TGlobalClass.RandomRange(0, _local5.length - 1)]);
        goto L950;
      L612:
        if (this.choosenSet[_local10] != this.SYRUP) goto L754;
        var $t8 = new Vector.<int>(3);
        $t8[0] = 3;
        $t8[1] = 9;
        $t8[2] = 10;
        _local5 = $t8;
        if (this._global.tutorial.currentDialogueID != Tutorial.LEVEL_11) goto L719;
        var $t9 = new Vector.<int>(1);
        $t9[0] = 3;
        _local5 = $t9;
      L719:
        this.orderList.push(_local5[TGlobalClass.RandomRange(0, _local5.length - 1)]);
        _local4 = true;
        goto L950;
      L754:
        if (this.choosenSet[_local10] != this.TOPPING) goto L854;
        _local8 = int(TGlobalClass.RandomRange(0, _local6.length - 1));
        if (this._global.tutorial.currentDialogueID != Tutorial.LEVEL_11) goto L823;
        this.orderList.push(4);
        goto L838;
      L823:
        this.orderList.push(_local6[_local8]);
      L838:
        _local6.splice(_local8, 1);
        _local1++;
        goto L950;
      L854:
        if (this.choosenSet[_local10] != this.BEVERAGE) goto L950;
        _local8 = int(TGlobalClass.RandomRange(0, _local7.length - 1));
        if (this._global.tutorial.currentDialogueID != Tutorial.LEVEL_11) goto L923;
        this.orderList.push(6);
        goto L938;
      L923:
        this.orderList.push(_local7[_local8]);
      L938:
        _local7.splice(_local8, 1);
        _local2++;
      L950:
        this.totalPatience = this.totalPatience + 1;
        _local10++;
      L963:
        if (_local10 < this.choosenSet.length) goto L365;
        this.totalPatience = this.totalPatience * 60;
        this.patience = this.totalPatience;
        _local3 = 0;
        var _local11 = int(this.previewPaper.x + this.previewPaper.width / 2 + 10);
        var _local12 = int(this.previewPaper.y + this.previewPaper.height / 2 + 10);
        if (this.type[this.orderList[0]] != this.CUP) goto L1126;
        var $t10 = _local2 > 0;
        $s0 = $t10;
        if (!$t10) goto L1084;
        $s0 = _local1 > 0;
      L1084:
        if (!$s0) goto L1100;
        _local11 = int(_local11 - 80);
        goto L1126;
      L1100:
        var $t11 = _local2 > 0;
        $s0 = $t11;
        if ($t11) goto L1114;
        $s0 = _local1 > 0;
      L1114:
        if (!$s0) goto L1126;
        _local11 = int(_local11 - 40);
      L1126:
        _local10 = 0;
        goto L1835;
      L1134:
        this.price = this.price + this.itemPrice[this.orderList[_local10]];
        this.previewAr[_local10].texture = this.dessertMakingAtlas.getTexture(this.itemName[this.orderList[_local10]] + "_preview");
        this.previewAr[_local10].readjustSize();
        this.previewAr[_local10].scaleX = 1;
        this.previewAr[_local10].scaleY = 1;
        this.previewAr[_local10].visible = true;
        this.previewAr[_local10].pivotX = this.previewAr[_local10].width / 2;
        this.previewAr[_local10].pivotY = this.previewAr[_local10].height / 2;
        this.previewAr[_local10].x = _local11;
        $s0 = this.previewAr[_local10];
        $s1 = this.previewPaper.y + this.previewAr[0].height;
        if (!(this.orderList[_local10] == 7)) goto L1374;
        $s2 = -10;
        goto L1377;
      L1374:
        $s2 = 10;
      L1377:
        $s0.y = $s1 + $s2;
        if (_local4) goto L1414;
        this.previewAr[_local10].y = this.previewAr[_local10].y + 40;
      L1414:
        if (this.type[this.orderList[0]] != this.CUP) goto L1790;
        if (this.type[this.orderList[_local10]] != this.YOGURT) goto L1520;
        $s0 = this.previewAr[_local10];
        $s1 = this.previewAr[_local10].y;
        $s2 = 55;
        if (!(this.orderList[_local10] == 7)) goto L1508;
        $s3 = 10;
        goto L1511;
      L1508:
        $s3 = 0;
      L1511:
        $s0.y = $s1 + ($s2 + $s3);
        goto L1786;
      L1520:
        if (this.type[this.orderList[_local10]] != this.SYRUP) goto L1602;
        $s0 = this.previewAr[_local10];
        $s1 = this.previewAr[_local10].y;
        $s2 = 120;
        if (!(this.orderList[_local10] == 7)) goto L1590;
        $s3 = 10;
        goto L1593;
      L1590:
        $s3 = 0;
      L1593:
        $s0.y = $s1 + ($s2 + $s3);
        goto L1786;
      L1602:
        if (this.type[this.orderList[_local10]] != this.TOPPING) goto L1686;
        this.previewAr[_local10].x = _local11 + 80;
        this.previewAr[_local10].y = _local12 + _local3 * 50 - (_local1 - 1) * 50 / 2;
        _local3++;
        if (!(_local3 >= _local1)) goto L1682;
        _local3 = 0;
      L1682:
        goto L1786;
      L1686:
        if (this.type[this.orderList[_local10]] != this.BEVERAGE) goto L1786;
        if (_local1 != 0) goto L1738;
        this.previewAr[_local10].x = _local11 + 80;
        goto L1756;
      L1738:
        this.previewAr[_local10].x = _local11 + 160;
      L1756:
        this.previewAr[_local10].y = _local3 * 95 + _local12 - (_local2 - 1) * 95 / 2;
        _local3++;
      L1786:
        goto L1820;
      L1790:
        this.previewAr[_local10].y = _local3 * 95 + _local12 - (_local2 - 1) * 95 / 2;
        _local3++;
      L1820:
        this.previewAr[_local10].visible = true;
        _local10++;
      L1835:
        if (_local10 < this.orderList.length) goto L1134;
        _local10 = 0;
        goto L1912;
      L1856:
        var _local13 = 0;
        this.previewAr[_local10].scaleY = 0;
        this.previewAr[_local10].scaleX = _local13;
        TweenLite.delayedCall(_local10 * 0.1, this.popOrder, [_local10]);
        _local10++;
      L1912:
        if (_local10 < this.orderList.length) goto L1856;
        this.orderInPlace = true;
    }

    function popOrder(arg1:int):void {
        this._global._soundAsset.playSFX("other_ingredient_matched_v3");
        TweenLite.to(this.previewAr[arg1], 0.2, {"scaleX": 1, "scaleY": 1, "ease": Back.easeOut});
    }

    function touchIngredient(arg1:TouchEvent):void {
        var _local2 = 0;
        var _local3 = 0;
        var _local4 = 0;
        if (!arg1.getTouch(this._global._game, TouchPhase.BEGAN)) goto L1338;
        if (this.orderInPlace) goto L43;
        return;
      L43:
        if (!this._global.tutorial.button.visible) goto L61;
        return;
      L61:
        _local2 = int((arg1.currentTarget as Quad).name);
        if (this._global.tutorial.canClickThisButton(_local2)) goto L98;
        return;
      L98:
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "touch_ingredient", true, false, {"id": _local2}));
        if (_local2 == this.orderList[this.currentLayer]) goto L204;
        this.currentLayer = 0;
        _local3 = 0;
        goto L177;
      L162:
        this.onPlateObjAr[_local3].visible = false;
        _local3++;
      L177:
        if (_local3 < this.onPlateObjAr.length) goto L162;
        this._global._soundAsset.playSFX("put_beverage_fries_on_tray_v2");
        return;
      L204:
        this._global._soundAsset.playSFX("other_ingredient_matched_v3");
        this.onPlateObjAr[_local2].texture = this.dessertMakingAtlas.getTexture(this.itemName[_local2]);
        this.onPlateObjAr[_local2].readjustSize();
        this.onPlateObjAr[_local2].visible = true;
        this.onPlateObjAr[_local2].pivotX = this.onPlateObjAr[_local2].width / 2;
        this.onPlateObjAr[_local2].pivotY = this.onPlateObjAr[_local2].height / 2;
        this.onPlateObjAr[_local2].x = this.posX[_local2];
        var $t0 = !(this.currentLayer == 0);
        $s0 = $t0;
        if (!$t0) goto L364;
        $s0 = this.orderList[0] == 7;
      L364:
        if (!$s0) goto L385;
        $s0 = !(this.orderList[this.currentLayer] == 6);
      L385:
        if (!$s0) goto L406;
        $s0 = !(this.orderList[this.currentLayer] == 13);
      L406:
        if (!$s0) goto L416;
        $s0 = 35;
        goto L418;
      L416:
        $s0 = 0;
      L418:
        _local4 = int($s0);
        this.onPlateObjAr[_local2].y = this.posY[_local2] - 40 - _local4;
        this.onPlateObjAr[_local2].scaleX = 0.9;
        this.onPlateObjAr[_local2].scaleY = 0.5;
        TweenLite.to(this.onPlateObjAr[_local2], 0.3, {"scaleX": 1, "scaleY": 1, "y": this.posY[_local2] - _local4, "ease": Back.easeOut});
        var $t1 = !(_local2 == 6);
        $s0 = $t1;
        if (!$t1) goto L540;
        $s0 = !(_local2 == 13);
      L540:
        if (!$s0) goto L584;
        this._global._gameS.setChildIndex(this.onPlateObjAr[_local2], this._global._gameS.getChildIndex(TGlobalClass._windowDisplay._blackScreen) - 1);
      L584:
        var _local5 = this;
        var _local6 = this.currentLayer + 1;
        _local5.currentLayer = _local6;
        if (!(this.currentLayer >= this.orderList.length)) goto L1338;
        this._global._soundAsset.playSFX("money_collected_v2");
        _local5 = this;
        _local6 = this.froyoMadeCount + 1;
        _local5.froyoMadeCount = _local6;
        this.earning = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(this.earning) + this.price);
        this.scorePreviewText.text = "$ " + this._global._maxCrypto.STI(this.earning);
        if (!(this._global._maxCrypto.STI(this.earning) >= this.goalTarget)) goto L1084;
        if (TGlobalClass.extrasMode) goto L927;
        if (this.goalOrExpertText.idx != 1) goto L854;
        TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] = true;
        this.goalOrExpertPreviewText.text = "$ " + this.expertTarget;
        this.goalOrExpertText.text = "EXPERT:";
        TGlobalClass._windowDisplay.showGoalIconSmall();
        this._global._soundAsset.playSFX("goal_reached");
        var $t2 = this.goalOrExpertText;
        _local5 = $t2;
        _local6 = $t2.idx + 1;
        _local5.idx = _local6;
      L854:
        if (this.goalOrExpertText.idx != 2) goto L923;
        if (!(this._global._maxCrypto.STI(this.earning) >= this.expertTarget)) goto L923;
        TGlobalClass._windowDisplay.showGoalIconSmall();
        this._global._soundAsset.playSFX("expert_reached");
        this.goalOrExpertText.idx = 3;
      L923:
        goto L1084;
      L927:
        _local5 = this;
        _local6 = this.level + 1;
        _local5.level = _local6;
        if (!(this.level <= 5)) goto L1063;
        this.showCurrentLevel();
        this.time = TGlobalClass.levelSetXML.dessert[this.level - 1].time * 30;
        this.totalTime = this.time;
        this.goalTarget = TGlobalClass.levelSetXML.dessert[this.level - 1].goal;
        this.goalOrExpertPreviewText.text = "$ " + this.goalTarget;
        this.earning = "a";
        this.scorePreviewText.text = "$ 0";
        this.setLevelSet();
        goto L1084;
      L1063:
        this.timesUp = true;
        TGlobalClass._windowDisplay.showWindow(TWindowState.DESSERT_LEVEL_COMPLETE_EXTRAS);
      L1084:
        this.orderInPlace = false;
        TweenLite.delayedCall(0.4, this.tweenOut);
        _local3 = 0;
        goto L1139;
      L1109:
        if (!this.previewAr[_local3].visible) goto L1137;
        this.previewAr[_local3].visible = false;
      L1137:
        _local3++;
      L1139:
        if (_local3 < this.previewAr.length) goto L1109;
        if (TGlobalClass.extrasMode) goto L1338;
        if (TSavegame.currentPlayer.unlocked[Achievement.FROYO_MASTER]) goto L1338;
        var $t3 = TSavegame.currentPlayer.achievementCounter;
        _local5 = $t3;
        var $t4 = Achievement.FROYO_MASTER;
        _local6 = $t4;
        var _local7 = $t3[$t4] + 1;
        _local5[_local6] = _local7;
        if (!(TSavegame.currentPlayer.achievementCounter[Achievement.FROYO_MASTER] > Achievement.ALL_TARGET[Achievement.FROYO_MASTER])) goto L1298;
        TSavegame.currentPlayer.unlocked[Achievement.FROYO_MASTER] = true;
        Achievement.tweenAchievementNotif(Achievement.FROYO_MASTER);
        goto L1338;
      L1298:
        if (TSavegame.currentPlayer.achievementCounter[Achievement.FROYO_MASTER] % 50 != 0) goto L1338;
        Achievement.tweenAchievementProgress(Achievement.FROYO_MASTER);
      L1338:
    }

    function tweenOut():void {
        this.timeline.restart();
        this.timeline.to(this.pan, 0.8, {"x": this.pan.x - 1000, "ease": Expo.easeIn}, 0);
        this.timeline.to(this.custBG, 0.8, {"x": 4294967046, "ease": Expo.easeIn}, 0);
        this.timeline.to(this.heart, 0.8, {"x": 4294967046, "ease": Expo.easeIn}, 0);
        this.timeline.to(this.custFillSprite, 0.8, {"x": 4294967046, "ease": Expo.easeIn}, 0);
        this.timeline.to(this.custGlass, 0.8, {"x": 4294967046, "ease": Expo.easeIn}, 0);
        i = 0;
        goto L293;
      L209:
        if (!this.onPlateObjAr[i].visible) goto L283;
        this.timeline.to(this.onPlateObjAr[i], 0.8, {"x": this.onPlateObjAr[i].x - 1000, "ease": Expo.easeIn}, 0);
      L283:
        i = i + 1;
      L293:
        if (i < this.onPlateObjAr.length) goto L209;
        this.timeline.to(this.customer, 1, {"x": 4294967046, "ease": Expo.easeIn, "onComplete": function():void {
            var _local1 = TGlobalClass.RandomRange(1, 12);
            var $t0 = _global.tutorial.currentDialogueID == Tutorial.LEVEL_11;
            $s0 = $t0;
            if (!$t0) goto L47;
            $s0 = _global.tutorial.dialogueCounter > 10;
          L47:
            if (!$s0) goto L55;
            _local1 = 2;
          L55:
            customer.texture = dessertMakingAtlas.getTexture("char_" + _local1);
            customer.readjustSize();
            custFillSprite.clipRect.x = 0;
        }}, 0);
        var $t0 = !this.timesUp;
        $s0 = $t0;
        if (!$t0) goto L368;
        $s0 = this.time > 30;
      L368:
        if (!$s0) goto L377;
        this.tweenIn();
      L377:
    }

    function tweenIn():void {
        this.timeline.to(this.pan, 0.8, {"x": 450, "ease": Expo.easeOut}, 1);
        this.timeline.to(this.custBG, 1, {"x": 0, "ease": Expo.easeOut}, 1);
        this.timeline.to(this.heart, 1, {"x": 25, "ease": Expo.easeOut}, 1);
        this.timeline.to(this.custFillSprite, 1, {"x": 47, "ease": Expo.easeOut}, 1);
        this.timeline.to(this.custGlass, 1, {"x": 47, "ease": Expo.easeOut}, 1);
        this.timeline.to(this.customer, 1, {"x": -20, "ease": Expo.easeOut}, 1);
    }

    function checkTimesUp():void {
        var _local1 = 0;
        var _local2 = 0;
        var _local3 = 0;
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "check_times_up"));
        if (this.timesUp) goto L55;
        this.setOrder();
        goto L1519;
      L55:
        if (TGlobalClass.extrasMode) goto L1483;
        _local2 = 0;
        var $t0 = TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] == 0;
        $s0 = $t0;
        if (!$t0) goto L111;
        $s0 = TGlobalClass.currentPlayedLevel == TSavegame.currentPlayer.passedDays;
      L111:
        if (!$s0) goto L772;
        if (!(this._global._maxCrypto.STI(this.earning) >= this.goalTarget)) goto L752;
        if (TSavegame.currentPlayer.unlocked[Achievement.QUALIFIED_CHEF]) goto L319;
        var $t1 = TSavegame.currentPlayer.achievementCounter;
        var _local4 = $t1;
        var $t2 = Achievement.QUALIFIED_CHEF;
        var _local5 = $t2;
        var _local6 = $t1[$t2] + 1;
        _local4[_local5] = _local6;
        if (!(TSavegame.currentPlayer.achievementCounter[Achievement.QUALIFIED_CHEF] >= Achievement.ALL_TARGET[Achievement.QUALIFIED_CHEF])) goto L277;
        TSavegame.currentPlayer.unlocked[Achievement.QUALIFIED_CHEF] = true;
        Achievement.tweenAchievementNotif(Achievement.QUALIFIED_CHEF);
        _local2++;
        goto L319;
      L277:
        if (TSavegame.currentPlayer.achievementCounter[Achievement.QUALIFIED_CHEF] % 15 != 0) goto L319;
        Achievement.tweenAchievementProgress(Achievement.QUALIFIED_CHEF);
        _local2++;
      L319:
        if (!(this._global._maxCrypto.STI(this.earning) >= this.expertTarget)) goto L598;
        if (TSavegame.currentPlayer.unlocked[Achievement.INTERNATIONAL_CHEF]) goto L543;
        var $t3 = TSavegame.currentPlayer.achievementCounter;
        _local4 = $t3;
        var $t4 = Achievement.INTERNATIONAL_CHEF;
        _local5 = $t4;
        _local6 = $t3[$t4] + 1;
        _local4[_local5] = _local6;
        if (!(TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] >= Achievement.ALL_TARGET[Achievement.INTERNATIONAL_CHEF])) goto L491;
        TSavegame.currentPlayer.unlocked[Achievement.INTERNATIONAL_CHEF] = true;
        TweenLite.delayedCall(_local2 * 6, Achievement.tweenAchievementNotif, [Achievement.INTERNATIONAL_CHEF]);
        goto L543;
      L491:
        if (TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] % 15 != 0) goto L543;
        TweenLite.delayedCall(_local2 * 6, Achievement.tweenAchievementProgress, [Achievement.INTERNATIONAL_CHEF]);
      L543:
        TSavegame.currentPlayer.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(TSavegame.currentPlayer.money) + 6);
        TGlobalClass.coinsGot = 6;
        goto L649;
      L598:
        TSavegame.currentPlayer.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(TSavegame.currentPlayer.money) + 3);
        TGlobalClass.coinsGot = 3;
      L649:
        TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] = this._global._maxCrypto.STI(this.earning);
        var $t5 = TSavegame.currentPlayer;
        _local4 = $t5;
        _local5 = $t5.passedDays + 1;
        _local4.passedDays = _local5;
        _local1 = int(this._global._maxCrypto.STI(this.earning));
        TSavegame.save();
        TGlobalClass._windowDisplay.showWindow(TWindowState.DESSERT_LEVEL_COMPLETE);
        goto L768;
      L752:
        TGlobalClass._windowDisplay.showWindow(TWindowState.DESSERT_LEVEL_FAILED);
      L768:
        goto L1273;
      L772:
        if (!(this._global._maxCrypto.STI(this.earning) >= this.goalTarget)) goto L1257;
        if (!(this._global._maxCrypto.STI(this.earning) > TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel])) goto L1237;
        var $t6 = this._global._maxCrypto.STI(this.earning) >= this.expertTarget;
        $s0 = $t6;
        if (!$t6) goto L881;
        $s0 = TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] < this.expertTarget;
      L881:
        if (!$s0) goto L1137;
        if (TSavegame.currentPlayer.unlocked[Achievement.INTERNATIONAL_CHEF]) goto L1086;
        var $t7 = TSavegame.currentPlayer.achievementCounter;
        _local4 = $t7;
        var $t8 = Achievement.INTERNATIONAL_CHEF;
        _local5 = $t8;
        _local6 = $t7[$t8] + 1;
        _local4[_local5] = _local6;
        if (!(TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] >= Achievement.ALL_TARGET[Achievement.INTERNATIONAL_CHEF])) goto L1034;
        TSavegame.currentPlayer.unlocked[Achievement.INTERNATIONAL_CHEF] = true;
        TweenLite.delayedCall(_local2 * 6, Achievement.tweenAchievementNotif, [Achievement.INTERNATIONAL_CHEF]);
        goto L1086;
      L1034:
        if (TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] % 15 != 0) goto L1086;
        TweenLite.delayedCall(_local2 * 6, Achievement.tweenAchievementProgress, [Achievement.INTERNATIONAL_CHEF]);
      L1086:
        TSavegame.currentPlayer.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(TSavegame.currentPlayer.money) + 3);
        TGlobalClass.coinsGot = 3;
      L1137:
        TGlobalClass.daysEarningPrev = TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel];
        _local1 = int(this._global._maxCrypto.STI(this.earning) - TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel]);
        TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] = this._global._maxCrypto.STI(this.earning);
        TSavegame.save();
      L1237:
        TGlobalClass._windowDisplay.showWindow(TWindowState.DESSERT_LEVEL_COMPLETE);
        goto L1273;
      L1257:
        TGlobalClass._windowDisplay.showWindow(TWindowState.DESSERT_LEVEL_FAILED);
      L1273:
        if (TSavegame.currentPlayer.unlocked[Achievement.BIG_BUSINESS]) goto L1472;
        _local3 = int(TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS]);
        TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS] = TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS] + _local1;
        if (!(TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS] >= Achievement.ALL_TARGET[Achievement.BIG_BUSINESS])) goto L1426;
        TSavegame.currentPlayer.unlocked[Achievement.BIG_BUSINESS] = true;
        Achievement.tweenAchievementNotif(Achievement.BIG_BUSINESS);
        goto L1472;
      L1426:
        if (!(_local3 % 20000 > TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS] % 20000)) goto L1472;
        Achievement.tweenAchievementProgress(Achievement.BIG_BUSINESS);
      L1472:
        TSavegame.save();
        goto L1519;
      L1483:
        if (!TGlobalClass.extrasMode) goto L1519;
        if (!(this.level < 6)) goto L1519;
        TGlobalClass._windowDisplay.showWindow(TWindowState.DESSERT_LEVEL_FAILED_EXTRAS);
      L1519:
        this.timeline.clear();
    }
}
}
