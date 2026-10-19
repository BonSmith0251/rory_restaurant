package {
class TWindowDisplay extends TWindow {
    var inputName:TextField;
    var inputTextFormat:TextFormat;
    var Create:Boolean;
    var buttonAr:Vector.<TNonStateButton>;
    var widgetBG:cSprite;
    var widgetMenu:ExtendTextField;
    var widgetInfo:ExtendTextField;
    var buttonLeft:TNonStateButton;
    var buttonRight:TNonStateButton;
    var uniButton:TNonStateButton;
    var smallWidgetBG:cSprite;
    var smallWidgetMenu:ExtendTextField;
    var smallWidgetInfo:ExtendTextField;
    var smallButtonLeft:TNonStateButton;
    var smallButtonRight:TNonStateButton;
    var smallUniButton:TNonStateButton;
    var playerNameText:Vector.<ExtendTextField>;
    var underLine:Vector.<Quad>;
    var soundText:ExtendTextField;
    var musicText:ExtendTextField;
    var fullScreenText:ExtendTextField;
    var soundButton:TNonStateButton;
    var musicButton:TNonStateButton;
    var fullScreenButton:TNonStateButton;
    var summaryText:ExtendTextField;
    var summaryStats:ExtendTextField;
    var summaryTips:ExtendTextField;
    var achievementNotif:cSprite;
    var achievementNotifTitle:ExtendTextField;
    var achievementNotifInfo:ExtendTextField;
    var submitScoreBtn:TNonStateButton;
    var leaderboardBtn:TNonStateButton;
    var unlockedItem:cSprite;
    var goalIcon:cSprite;
    var expertBlink:cSprite;
    var activeState:int;
    var choosenPlayerIdx:int;
    var dishPreviewAr:Vector.<cSprite>;

    function TWindowDisplay():* {
        super();
        super.startLoad();
        this.buttonAr = new Vector.<TNonStateButton>();
        this.dishPreviewAr = new Vector.<cSprite>(4, true);
        this.playerNameText = new Vector.<ExtendTextField>(5, true);
        this.underLine = new Vector.<Quad>(5, true);
    }

    function reload():void {
        super.startLoad();
    }

    function setText(arg1:ExtendTextField):void {
        var _local2 = arg1.text;
        arg1.text = "";
        arg1.text = _local2;
    }

    function setTexture():void {
    }

    function kill(arg1:int):void {
        var _local2 = 0;
        if (arg1 != _global.MAINMENU_ID) goto L158;
        _local2 = 0;
        goto L92;
      L23:
        this.playerNameText[_local2].removeEventListener(TouchEvent.TOUCH, this.choosePlayer);
        this.playerNameText[_local2].removeFromParent(true);
        this.playerNameText[_local2] = null;
        this.underLine[_local2].removeFromParent(true);
        this.underLine[_local2] = null;
        _local2++;
      L92:
        if (_local2 < 5) goto L23;
        this.inputName.removeEventListener(KeyboardEvent.KEY_UP, this.onKeyUp);
        this.inputName.removeEventListener(KeyboardEvent.KEY_DOWN, this.onKeyDown);
        _global._stage.removeChild(this.inputName);
        this.inputName = null;
        goto L449;
      L158:
        var $t0 = arg1 == _global.GAMESCREEN_ID;
        $s0 = $t0;
        if ($t0) goto L180;
        $s0 = arg1 == _global.HIDDEN_OBJECT_ID;
      L180:
        if ($s0) goto L194;
        $s0 = arg1 == _global.MAKE_DESSERT_ID;
      L194:
        if (!$s0) goto L449;
        this.soundButton.removeFromParent(true);
        this.soundButton = null;
        this.soundText.removeFromParent(true);
        this.soundText = null;
        this.musicButton.removeFromParent(true);
        this.musicButton = null;
        this.musicText.removeFromParent(true);
        this.musicText = null;
        this.fullScreenButton.removeFromParent(true);
        this.fullScreenButton = null;
        this.fullScreenText.removeFromParent(true);
        this.fullScreenText = null;
        this.summaryStats.removeFromParent(true);
        this.summaryStats = null;
        this.summaryTips.removeFromParent(true);
        this.summaryTips = null;
        this.summaryText.removeFromParent(true);
        this.summaryText = null;
        this.goalIcon.removeFromParent(true);
        this.goalIcon = null;
        TweenLite.killTweensOf(this.expertBlink);
        this.expertBlink.removeFromParent(true);
        this.expertBlink = null;
        this.leaderboardBtn.removeFromParent(true);
        this.leaderboardBtn = null;
        this.submitScoreBtn.removeFromParent(true);
        this.submitScoreBtn = null;
        _local2 = 0;
        goto L437;
      L398:
        if (this.buttonAr[_local2].idx != TNonStateButton.SOUND_BUTTON) goto L435;
        this.buttonAr.splice(_local2, 1);
        goto L449;
      L435:
        _local2++;
      L437:
        if (_local2 < this.buttonAr.length) goto L398;
      L449:
        var $t1 = arg1 == _global.GAMESCREEN_ID;
        $s0 = $t1;
        if ($t1) goto L471;
        $s0 = arg1 == _global.HIDDEN_OBJECT_ID;
      L471:
        if ($s0) goto L485;
        $s0 = arg1 == _global.MAKE_DESSERT_ID;
      L485:
        if ($s0) goto L499;
        $s0 = arg1 == _global.KITCHEN_ID;
      L499:
        if (!$s0) goto L517;
        this.unlockedItem.removeFromParent(true);
        this.unlockedItem = null;
      L517:
        if (arg1 != _global.GAMESCREEN_ID) goto L567;
        _local2 = 0;
        goto L560;
      L535:
        this.dishPreviewAr[_local2].removeFromParent(true);
        this.dishPreviewAr[_local2] = null;
        _local2++;
      L560:
        if (_local2 < 4) goto L535;
      L567:
        _global.tutorial.hideTutorial();
        _global.tutorial.currentDialogueID = -1;
    }

    function onKeyDown(arg1:KeyboardEvent):void {
        var _local2 = 0;
        var _local3 = 0;
        var _local4 = uint(0);
        var _local5 = null;
        if (this.activeState != TWindowState.CREATE_PLAYER) goto L419;
        if (!(this.inputName.length <= 0)) goto L67;
        if (arg1.keyCode != Keyboard.SPACE) goto L63;
        arg1.preventDefault();
      L63:
        goto L419;
      L67:
        if (arg1.keyCode != Keyboard.ENTER) goto L419;
        this.smallButtonLeft.visible = false;
        this.smallButtonRight.visible = false;
        this.smallWidgetBG.visible = false;
        this.smallWidgetInfo.visible = false;
        this.smallWidgetMenu.visible = false;
        this.inputName.visible = false;
        _local2 = 0;
        goto L172;
      L136:
        if (this.inputName.text != TSavegame.playerName[_local2]) goto L170;
        this.showWindow(TWindowState.SAME_NAME_WARNING);
        return;
      L170:
        _local2++;
      L172:
        if (_local2 < 5) goto L136;
        _local3 = -1;
        _local5 = this.inputName.text;
        _local2 = 0;
        goto L247;
      L199:
        if (_local5.charAt(_local2) != " ") goto L222;
        _local4 = uint(_local2);
        goto L245;
      L222:
        if (_local3 != -1) goto L232;
        _local3 = int(_local2);
      L232:
        _local4 = uint(this.inputName.text.length);
      L245:
        _local2++;
      L247:
        if (_local2 < this.inputName.text.length) goto L199;
        this.inputName.text = _local5.substring(_local3, _local4);
        _local2 = 0;
        goto L375;
      L285:
        if (TSavegame.usedSlot[_local2]) goto L373;
        TSavegame.usedSlot[_local2] = true;
        TSavegame.playerName[_local2] = this.inputName.text;
        TSavegame.lastPlayer = _local2 + 1;
        TSavegame._saveGame.data.lastPlayer = _local2 + 1;
        TSavegame.loadPlayer(_local2);
        TSavegame.save();
        goto L382;
      L373:
        _local2++;
      L375:
        if (_local2 < 5) goto L285;
      L382:
        this.choosenPlayerIdx = _local2;
        this.playerNameText[_local2].color = 16711680;
        this.buttonLeft.enabled = false;
        this.showWindow(TWindowState.CHANGE_PLAYER);
      L419:
    }

    function onKeyUp(arg1:KeyboardEvent):void {
        var _local2 = 0;
        if (this.activeState != TWindowState.CREATE_PLAYER) goto L105;
        if (!(this.inputName.text.length > 0)) goto L97;
        _local2 = 0;
        goto L73;
      L42:
        if (this.inputName.text.charAt(_local2) == " ") goto L71;
        this.smallButtonRight.enabled = true;
        return;
      L71:
        _local2++;
      L73:
        if (_local2 < this.inputName.length) goto L42;
        this.smallButtonRight.enabled = false;
        goto L105;
      L97:
        this.smallButtonRight.enabled = false;
      L105:
    }

    function initializeWindow(arg1:int):void {
        var _local2 = 0;
        var _local3 = NaN;
        var _local4 = NaN;
        if (_global.tutorial) goto L47;
        _global.tutorial = new Tutorial();
        _global.tutorial.init();
        goto L57;
      L47:
        _global.tutorial.bringTutorialToTop();
      L57:
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.numChildren - 1);
        if (this.Create) goto L908;
        this.widgetBG = new cSprite(windowsAtlas.getTexture("widget_BG"));
        this.widgetBG.pivotX = this.widgetBG.width / 2;
        this.widgetBG.pivotY = this.widgetBG.height / 2;
        this.widgetBG.x = 512;
        this.widgetBG.y = 300;
        this.widgetBG.visible = false;
        _global._gameS.addChild(this.widgetBG);
        this.widgetMenu = new ExtendTextField(this.widgetBG.width - 100, 100, "SUMMARY", "jenna_sue", 60, 0, true);
        this.widgetMenu.pivotX = this.widgetMenu.width / 2;
        this.widgetMenu.pivotY = 50;
        this.widgetMenu.x = 512;
        this.widgetMenu.y = this.widgetBG.y - 220;
        this.widgetMenu.visible = false;
        _global._gameS.addChild(this.widgetMenu);
        this.widgetInfo = new ExtendTextField(this.widgetBG.width - 100, 300, "", "bromine", 25, 0);
        this.widgetInfo.pivotX = this.widgetInfo.width / 2;
        this.widgetInfo.pivotY = 150;
        this.widgetInfo.x = 512;
        this.widgetInfo.y = this.widgetBG.y - 30;
        this.widgetInfo.visible = false;
        _global._gameS.addChild(this.widgetInfo);
        this.buttonLeft = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "Test", "bromine", 20, 0);
        this.buttonLeft.pivotX = this.buttonLeft.width / 2;
        this.buttonLeft.pivotY = this.buttonLeft.height / 2;
        var _local5 = 0.8;
        this.buttonLeft.scaleY = 0.8;
        this.buttonLeft.scaleX = _local5;
        this.buttonLeft.textureName = "small_btn_blue";
        this.buttonLeft.x = this.widgetBG.x - 170;
        this.buttonLeft.y = this.widgetBG.y + 200;
        this.buttonLeft.visible = false;
        _global._gameS.addChild(this.buttonLeft);
        this.buttonAr.push(this.buttonLeft);
        this.buttonRight = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "Test", "bromine", 20, 0);
        this.buttonRight.pivotX = this.buttonRight.width / 2;
        this.buttonRight.pivotY = this.buttonRight.height / 2;
        _local5 = 0.8;
        this.buttonRight.scaleY = 0.8;
        this.buttonRight.scaleX = _local5;
        this.buttonRight.textureName = "small_btn_blue";
        this.buttonRight.x = this.widgetBG.x + 170;
        this.buttonRight.y = this.widgetBG.y + 200;
        this.buttonRight.visible = false;
        _global._gameS.addChild(this.buttonRight);
        this.buttonAr.push(this.buttonRight);
        this.uniButton = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "New Game", "bromine", 20, 0);
        this.uniButton.pivotX = this.uniButton.width / 2;
        this.uniButton.pivotY = this.uniButton.height / 2;
        _local5 = 0.8;
        this.uniButton.scaleY = 0.8;
        this.uniButton.scaleX = _local5;
        this.uniButton.textureName = "small_btn_blue";
        this.uniButton.x = this.widgetBG.x;
        this.uniButton.y = this.widgetBG.y + 200;
        this.uniButton.visible = false;
        _global._gameS.addChild(this.uniButton);
        this.buttonAr.push(this.uniButton);
        goto L1052;
      L908:
        _global._gameS.setChildIndex(this.widgetBG, _global._gameS.numChildren - 1);
        _global._gameS.setChildIndex(this.widgetMenu, _global._gameS.numChildren - 1);
        _global._gameS.setChildIndex(this.widgetInfo, _global._gameS.numChildren - 1);
        _global._gameS.setChildIndex(this.buttonLeft, _global._gameS.numChildren - 1);
        _global._gameS.setChildIndex(this.buttonRight, _global._gameS.numChildren - 1);
        _global._gameS.setChildIndex(this.uniButton, _global._gameS.numChildren - 1);
      L1052:
        if (arg1 != _global.MAINMENU_ID) goto L1410;
        _local2 = 0;
        goto L1403;
      L1070:
        this.playerNameText[_local2] = new ExtendTextField(380, 70, "Empty", "jenna_sue", 30, 0);
        this.playerNameText[_local2].pivotX = this.playerNameText[_local2].width / 2;
        this.playerNameText[_local2].pivotY = this.playerNameText[_local2].height / 2;
        this.playerNameText[_local2].x = 512;
        this.playerNameText[_local2].y = _local2 * 70 + 145;
        this.playerNameText[_local2].visible = false;
        this.playerNameText[_local2].idx = _local2;
        _global._gameS.addChild(this.playerNameText[_local2]);
        this.playerNameText[_local2].addEventListener(TouchEvent.TOUCH, this.choosePlayer);
        this.underLine[_local2] = new Quad(380, 5, 0);
        this.underLine[_local2].pivotX = this.underLine[_local2].width / 2;
        this.underLine[_local2].pivotY = this.underLine[_local2].height / 2;
        this.underLine[_local2].x = 512;
        this.underLine[_local2].y = this.playerNameText[_local2].y + 15;
        this.underLine[_local2].visible = false;
        this.underLine[_local2].touchable = false;
        _global._gameS.addChild(this.underLine[_local2]);
        _local2++;
      L1403:
        if (_local2 < 5) goto L1070;
      L1410:
        var $t0 = arg1 == _global.GAMESCREEN_ID;
        $s0 = $t0;
        if ($t0) goto L1432;
        $s0 = arg1 == _global.HIDDEN_OBJECT_ID;
      L1432:
        if ($s0) goto L1446;
        $s0 = arg1 == _global.MAKE_DESSERT_ID;
      L1446:
        if (!$s0) goto L2965;
        this.soundText = new ExtendTextField(150, 50, "SOUND : ", "bromine", 25, 0);
        this.soundText.touchable = false;
        this.soundText.x = this.smallWidgetBG.x - 100;
        this.soundText.y = this.smallWidgetBG.y - 30;
        this.soundText.visible = false;
        _global._gameS.addChild(this.soundText);
        this.soundButton = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "ON", "bromine", 20);
        this.soundButton.x = this.smallWidgetBG.x + 100;
        this.soundButton.y = this.soundText.y;
        this.soundButton.visible = false;
        this.soundButton.idx = TNonStateButton.SOUND_BUTTON;
        _global._gameS.addChild(this.soundButton);
        this.buttonAr.push(this.soundButton);
        this.musicText = new ExtendTextField(150, 50, "MUSIC : ", "bromine", 25, 0);
        this.musicText.touchable = false;
        this.musicText.x = this.smallWidgetBG.x - 100;
        this.musicText.y = this.smallWidgetBG.y + 40;
        this.musicText.visible = false;
        _global._gameS.addChild(this.musicText);
        this.musicButton = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "ON", "bromine", 20);
        this.musicButton.x = this.smallWidgetBG.x + 100;
        this.musicButton.y = this.musicText.y;
        this.musicButton.visible = false;
        this.musicButton.idx = TNonStateButton.MUSIC_BUTTON;
        _global._gameS.addChild(this.musicButton);
        this.buttonAr.push(this.musicButton);
        this.fullScreenText = new ExtendTextField(250, 50, "FULL SCREEN : ", "bromine", 25, 0);
        this.fullScreenText.touchable = false;
        this.fullScreenText.x = this.smallWidgetBG.x - 100;
        this.fullScreenText.y = this.smallWidgetBG.y + 80;
        this.fullScreenText.visible = false;
        _global._gameS.addChild(this.fullScreenText);
        this.fullScreenButton = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "ON", "bromine", 20);
        this.fullScreenButton.x = this.smallWidgetBG.x + 100;
        this.fullScreenButton.y = this.fullScreenText.y;
        this.fullScreenButton.visible = false;
        this.fullScreenButton.idx = TNonStateButton.FULLSCREEN_BUTTON;
        _global._gameS.addChild(this.fullScreenButton);
        this.buttonAr.push(this.fullScreenButton);
        this.summaryText = new ExtendTextField(this.widgetBG.width / 2, 350, "Customers Served\nBeverage Served\nFrench Fries Served\n\nToday’s Earnings\nTotal Earning", "bromine", 20);
        this.summaryText.pivotX = this.summaryText.width / 2;
        this.summaryText.pivotY = this.summaryText.height / 2;
        this.summaryText.hAlign = "left";
        this.summaryText.x = this.widgetBG.x - 100;
        this.summaryText.y = this.widgetBG.y;
        this.summaryText.touchable = false;
        this.summaryText.visible = false;
        _global._gameS.addChild(this.summaryText);
        this.summaryStats = new ExtendTextField(this.widgetBG.width / 2, 350, "", "bromine", 20);
        this.summaryStats.pivotX = this.summaryStats.width / 2;
        this.summaryStats.pivotY = this.summaryStats.height / 2;
        this.summaryStats.hAlign = "right";
        this.summaryStats.x = this.widgetBG.x + 80;
        this.summaryStats.y = this.widgetBG.y;
        this.summaryStats.touchable = false;
        this.summaryText.visible = false;
        _global._gameS.addChild(this.summaryStats);
        this.summaryTips = new ExtendTextField(this.widgetBG.width / 2 + 40, 100, "", "bromine", 20);
        this.summaryTips.pivotX = this.summaryTips.width / 2;
        this.summaryTips.pivotY = this.summaryTips.height / 2;
        this.summaryTips.hAlign = "center";
        this.summaryTips.x = this.widgetBG.x;
        this.summaryTips.y = this.widgetBG.y + 125;
        this.summaryTips.touchable = false;
        this.summaryTips.visible = false;
        _global._gameS.addChild(this.summaryTips);
        this.goalIcon = new cSprite(windowsAtlas.getTexture("goal"));
        this.goalIcon.pivotX = this.goalIcon.width / 2;
        this.goalIcon.pivotY = this.goalIcon.height / 2;
        this.goalIcon.x = 780;
        this.goalIcon.y = 500;
        this.goalIcon.visible = false;
        _global._gameS.addChild(this.goalIcon);
        this.expertBlink = new cSprite(windowsAtlas.getTexture("expert_blink"));
        this.expertBlink.pivotX = this.expertBlink.width / 2;
        this.expertBlink.pivotY = this.expertBlink.height / 2;
        this.expertBlink.x = this.goalIcon.x;
        this.expertBlink.y = this.goalIcon.y + 10;
        this.expertBlink.visible = false;
        _global._gameS.addChild(this.expertBlink);
        this.leaderboardBtn = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "Leaderboard", "bromine", 20, 0);
        this.leaderboardBtn.pivotX = this.leaderboardBtn.width / 2;
        this.leaderboardBtn.pivotY = this.leaderboardBtn.height / 2;
        _local5 = 0.8;
        this.leaderboardBtn.scaleY = 0.8;
        this.leaderboardBtn.scaleX = _local5;
        this.leaderboardBtn.x = 640;
        this.leaderboardBtn.y = 550;
        this.leaderboardBtn.visible = false;
        this.leaderboardBtn.idx = TNonStateButton.LEADERBOARD_BUTTON;
        _global._gameS.addChild(this.leaderboardBtn);
        this.buttonAr.push(this.leaderboardBtn);
        this.submitScoreBtn = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "Submit Score", "bromine", 20, 0);
        this.submitScoreBtn.pivotX = this.submitScoreBtn.width / 2;
        this.submitScoreBtn.pivotY = this.submitScoreBtn.height / 2;
        _local5 = 0.8;
        this.submitScoreBtn.scaleY = 0.8;
        this.submitScoreBtn.scaleX = _local5;
        this.submitScoreBtn.x = 375;
        this.submitScoreBtn.y = 550;
        this.submitScoreBtn.visible = false;
        this.submitScoreBtn.idx = TNonStateButton.SUBMIT_BUTTON;
        _global._gameS.addChild(this.submitScoreBtn);
        this.buttonAr.push(this.submitScoreBtn);
      L2965:
        var $t1 = arg1 == _global.GAMESCREEN_ID;
        $s0 = $t1;
        if ($t1) goto L2987;
        $s0 = arg1 == _global.HIDDEN_OBJECT_ID;
      L2987:
        if ($s0) goto L3001;
        $s0 = arg1 == _global.MAKE_DESSERT_ID;
      L3001:
        if ($s0) goto L3015;
        $s0 = arg1 == _global.KITCHEN_ID;
      L3015:
        if (!$s0) goto L3135;
        this.unlockedItem = new cSprite(windowsAtlas.getTexture("small_btn_blue"));
        this.unlockedItem.pivotX = this.unlockedItem.width / 2;
        this.unlockedItem.pivotY = this.unlockedItem.height / 2;
        this.unlockedItem.x = this.widgetBG.x;
        this.unlockedItem.y = this.widgetBG.y - 30;
        this.unlockedItem.touchable = false;
        this.unlockedItem.visible = false;
        _global._gameS.addChild(this.unlockedItem);
      L3135:
        if (arg1 != _global.GAMESCREEN_ID) goto L3284;
        _local2 = 0;
        goto L3277;
      L3153:
        this.dishPreviewAr[_local2] = new cSprite(_global.gameScreen.gameAtlas.getTexture("veggie_croissant"));
        this.dishPreviewAr[_local2].x = 512 - (_local2 - 2) * (this.dishPreviewAr[_local2].width - 10);
        this.dishPreviewAr[_local2].y = 300;
        this.dishPreviewAr[_local2].touchable = false;
        this.dishPreviewAr[_local2].visible = false;
        _global._gameS.addChild(this.dishPreviewAr[_local2]);
        _local2++;
      L3277:
        if (_local2 < 4) goto L3153;
      L3284:
        if (this.Create) goto L4102;
        this.smallWidgetBG = new cSprite(windowsAtlas.getTexture("small_widget_BG"));
        this.smallWidgetBG.pivotX = this.smallWidgetBG.width / 2;
        this.smallWidgetBG.pivotY = this.smallWidgetBG.height / 2;
        this.smallWidgetBG.x = 512;
        this.smallWidgetBG.y = 300;
        this.smallWidgetBG.visible = false;
        _global._gameS.addChild(this.smallWidgetBG);
        this.smallWidgetMenu = new ExtendTextField(this.smallWidgetBG.width - 80, 70, "Exit?", "jenna_sue", 60, 0);
        this.smallWidgetMenu.pivotX = this.smallWidgetMenu.width / 2;
        this.smallWidgetMenu.pivotY = this.smallWidgetMenu.height / 2;
        this.smallWidgetMenu.x = 525;
        this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
        this.smallWidgetMenu.visible = false;
        _global._gameS.addChild(this.smallWidgetMenu);
        this.smallWidgetInfo = new ExtendTextField(this.smallWidgetBG.width - 80, 250, "Are You Sure Want To Exit?", "jenna_sue", 40, 0);
        this.smallWidgetInfo.pivotX = this.smallWidgetInfo.width / 2;
        this.smallWidgetInfo.pivotY = this.smallWidgetInfo.height / 2;
        this.smallWidgetInfo.x = 525;
        this.smallWidgetInfo.y = this.smallWidgetBG.y - 30;
        this.smallWidgetInfo.visible = false;
        _global._gameS.addChild(this.smallWidgetInfo);
        this.smallButtonLeft = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "Test", "bromine", 20, 0);
        this.smallButtonLeft.textureName = "small_btn_blue";
        this.smallButtonLeft.pivotX = this.smallButtonLeft.width / 2;
        this.smallButtonLeft.pivotY = this.smallButtonLeft.height / 2;
        this.smallButtonLeft.x = this.smallWidgetBG.x - 100;
        this.smallButtonLeft.y = this.smallWidgetBG.y + 130;
        this.smallButtonLeft.scaleX = 0.9;
        this.smallButtonLeft.scaleY = 0.9;
        this.smallButtonLeft.visible = false;
        _global._gameS.addChild(this.smallButtonLeft);
        this.buttonAr.push(this.smallButtonLeft);
        this.smallButtonRight = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "Test", "bromine", 20, 0);
        this.smallButtonRight.textureName = "small_btn_blue";
        this.smallButtonRight.pivotX = this.smallButtonRight.width / 2;
        this.smallButtonRight.pivotY = this.smallButtonRight.height / 2;
        this.smallButtonRight.x = this.smallWidgetBG.x + 100;
        this.smallButtonRight.y = this.smallWidgetBG.y + 130;
        this.smallButtonRight.visible = false;
        this.smallButtonRight.scaleX = 0.9;
        this.smallButtonRight.scaleY = 0.9;
        _global._gameS.addChild(this.smallButtonRight);
        this.buttonAr.push(this.smallButtonRight);
        this.smallUniButton = new TNonStateButton(windowsAtlas.getTexture("small_btn_blue"), "Test", "bromine", 20, 0);
        this.smallUniButton.textureName = "small_btn_blue";
        this.smallUniButton.pivotX = this.smallUniButton.width / 2;
        this.smallUniButton.pivotY = this.smallUniButton.height / 2;
        this.smallUniButton._image.visible = false;
        this.smallUniButton.x = this.smallWidgetBG.x;
        this.smallUniButton.y = this.smallWidgetBG.y + 130;
        this.smallUniButton.visible = false;
        _global._gameS.addChild(this.smallUniButton);
        this.buttonAr.push(this.smallUniButton);
        goto L4246;
      L4102:
        _global._gameS.setChildIndex(this.smallWidgetBG, _global._gameS.numChildren - 1);
        _global._gameS.setChildIndex(this.smallWidgetMenu, _global._gameS.numChildren - 1);
        _global._gameS.setChildIndex(this.smallWidgetInfo, _global._gameS.numChildren - 1);
        _global._gameS.setChildIndex(this.smallButtonLeft, _global._gameS.numChildren - 1);
        _global._gameS.setChildIndex(this.smallButtonRight, _global._gameS.numChildren - 1);
        _global._gameS.setChildIndex(this.smallUniButton, _global._gameS.numChildren - 1);
      L4246:
        if (arg1 != _global.MAINMENU_ID) goto L4543;
        this.inputTextFormat = new TextFormat("jenna_sue", 30, 0);
        this.inputTextFormat.align = "center";
        this.inputName = new TextField();
        this.inputName.width = this.smallWidgetBG.width - 100;
        this.inputName.height = 40;
        this.inputName.maxChars = 12;
        this.inputName.defaultTextFormat = this.inputTextFormat;
        this.inputName.type = TextFieldType.INPUT;
        this.inputName.needsSoftKeyboard = true;
        this.inputName.border = true;
        this.inputName.embedFonts = true;
        this.inputName.restrict = "a-zA-Z ";
        this.inputName.visible = false;
        _local3 = Number(_global._stage.stageWidth / 1024);
        _local4 = Number(_global._stage.stageHeight / 600);
        this.inputName.scaleX = _local3;
        this.inputName.scaleY = _local4;
        this.inputName.x = (_global._stage.stageWidth / 2 - this.inputName.width / 2) * _local3;
        this.inputName.y = this.smallWidgetBG.y * _local4;
        _global._stage.addChild(this.inputName);
        this.inputName.addEventListener(KeyboardEvent.KEY_UP, this.onKeyUp);
        this.inputName.addEventListener(KeyboardEvent.KEY_DOWN, this.onKeyDown);
      L4543:
        if (this.Create) goto L4937;
        this.achievementNotif = new cSprite(windowsAtlas.getTexture("widget_BG"));
        this.achievementNotif.pivotX = this.achievementNotif.width / 2;
        this.achievementNotif.pivotY = this.achievementNotif.height / 2;
        this.achievementNotif.x = 1500;
        this.achievementNotif.y = 200;
        this.achievementNotif.scaleX = 0.5;
        this.achievementNotif.scaleY = 0.2;
        this.achievementNotif.touchable = false;
        _global._gameS.addChild(this.achievementNotif);
        this.achievementNotifTitle = new ExtendTextField(this.achievementNotif.width - 30, this.achievementNotif.height / 2 - 20, "MASTER", "bromine", 20, Color.RED);
        this.achievementNotifTitle.pivotX = this.achievementNotifTitle.width / 2;
        this.achievementNotifTitle.pivotY = this.achievementNotifTitle.height / 2;
        this.achievementNotifTitle.x = this.achievementNotif.x;
        this.achievementNotifTitle.y = this.achievementNotif.y - 30;
        this.achievementNotifTitle.touchable = false;
        _global._gameS.addChild(this.achievementNotifTitle);
        this.achievementNotifInfo = new ExtendTextField(this.achievementNotif.width - 30, this.achievementNotif.height - 10, "You've served 100 fully satisfied customers!", "bromine", 15, Color.BLACK);
        this.achievementNotifInfo.pivotX = this.achievementNotifInfo.width / 2;
        this.achievementNotifInfo.pivotY = this.achievementNotifInfo.height / 2;
        this.achievementNotifInfo.x = this.achievementNotif.x;
        this.achievementNotifInfo.y = this.achievementNotif.y + 10;
        this.achievementNotifInfo.touchable = false;
        _global._gameS.addChild(this.achievementNotifInfo);
        goto L5009;
      L4937:
        _global._gameS.setChildIndex(this.achievementNotif, _global._gameS.numChildren - 1);
        _global._gameS.setChildIndex(this.achievementNotifTitle, _global._gameS.numChildren - 1);
        _global._gameS.setChildIndex(this.achievementNotifInfo, _global._gameS.numChildren - 1);
      L5009:
        _local2 = 0;
        goto L5041;
      L5016:
        this.buttonAr[_local2].addEventListener(TouchEvent.TOUCH, this.touchButton);
        _local2++;
      L5041:
        if (_local2 < this.buttonAr.length) goto L5016;
        this.Create = true;
    }

    function choosePlayer(arg1:TouchEvent):void {
        var _local2 = 0;
        if (!arg1.getTouch(_global._game, TouchPhase.BEGAN)) goto L176;
        this.choosenPlayerIdx = (arg1.currentTarget as ExtendTextField).idx;
        _local2 = 0;
        goto L64;
      L48:
        this.playerNameText[_local2].color = 0;
        _local2++;
      L64:
        if (_local2 < 5) goto L48;
        if (!TSavegame.usedSlot[this.choosenPlayerIdx]) goto L142;
        this.playerNameText[this.choosenPlayerIdx].color = 16711680;
        this.buttonLeft.enabled = true;
        this.uniButton.idx = TNonStateButton.SELECT_BUTTON;
        this.uniButton._text.text = "Select";
        goto L176;
      L142:
        this.uniButton._text.text = "Close";
        this.uniButton.idx = TNonStateButton.CANCEL_BUTTON;
        this.buttonLeft.enabled = false;
      L176:
    }

    function touchButton(arg1:TouchEvent):void {
        var _local2 = 0;
        var _local3 = 0;
        var _local4 = 0;
        var _local5 = uint(0);
        var _local6 = null;
        if (!arg1.getTouch(_global._game, TouchPhase.BEGAN)) goto L4017;
        _local3 = int((arg1.currentTarget as TNonStateButton).idx);
        if (_local3 != TNonStateButton.YES_BUTTON) goto L366;
        if (this.activeState != TWindowState.EXIT_WINDOW) goto L94;
        NativeApplication.nativeApplication.exit();
        goto L362;
      L94:
        if (this.activeState != TWindowState.CONFIRM_MAINMENU) goto L226;
        var $t0 = _global.currentSceneID == _global.GAMESCREEN_ID;
        $s0 = $t0;
        if (!$t0) goto L134;
        $s0 = !TGlobalClass.extrasMode;
      L134:
        if (!$s0) goto L161;
        $s0 = TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] == 0;
      L161:
        if (!$s0) goto L183;
        $s0 = TGlobalClass.currentPlayedLevel == TSavegame.currentPlayer.passedDays;
      L183:
        if (!$s0) goto L206;
        TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] = false;
      L206:
        _global._game.fadeToScreen(_global._mainMenuScreen);
        goto L362;
      L226:
        if (this.activeState != TWindowState.CONFIRM_KITCHEN) goto L362;
        var $t1 = _global.currentSceneID == _global.GAMESCREEN_ID;
        $s0 = $t1;
        if (!$t1) goto L266;
        $s0 = !TGlobalClass.extrasMode;
      L266:
        if (!$s0) goto L293;
        $s0 = TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] == 0;
      L293:
        if (!$s0) goto L315;
        $s0 = TGlobalClass.currentPlayedLevel == TSavegame.currentPlayer.passedDays;
      L315:
        if (!$s0) goto L338;
        TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] = false;
      L338:
        TGlobalClass.coinsGot = 0;
        _global._game.fadeToScreen(_global._kitchen);
      L362:
        goto L4017;
      L366:
        if (_local3 != TNonStateButton.NO_BUTTON) goto L541;
        if (this.activeState != TWindowState.EXIT_WINDOW) goto L446;
        _blackScreen.visible = false;
        this.smallWidgetBG.visible = false;
        this.smallWidgetMenu.visible = false;
        this.smallWidgetInfo.visible = false;
        this.smallUniButton.visible = false;
        this.smallButtonLeft.visible = false;
        this.smallButtonRight.visible = false;
      L446:
        var $t2 = this.activeState == TWindowState.CONFIRM_MAINMENU;
        $s0 = $t2;
        if ($t2) goto L474;
        $s0 = this.activeState == TWindowState.CONFIRM_KITCHEN;
      L474:
        if (!$s0) goto L537;
        this.smallWidgetBG.visible = false;
        this.smallWidgetMenu.visible = false;
        this.smallWidgetInfo.visible = false;
        this.smallUniButton.visible = false;
        this.smallButtonLeft.visible = false;
        this.smallButtonRight.visible = false;
        this.showWindow(TWindowState.PAUSE_GAME);
      L537:
        goto L4017;
      L541:
        if (_local3 != TNonStateButton.DELETE_BUTTON) goto L733;
        if (this.activeState != TWindowState.CHANGE_PLAYER) goto L581;
        this.showWindow(TWindowState.DELETE_PLAYER);
        goto L729;
      L581:
        if (this.activeState != TWindowState.DELETE_PLAYER) goto L729;
        _blackScreen.visible = false;
        this.smallWidgetBG.visible = false;
        this.smallButtonLeft.visible = false;
        this.smallButtonRight.visible = false;
        this.smallWidgetMenu.visible = false;
        this.smallWidgetInfo.visible = false;
        this.buttonLeft.enabled = false;
        this.playerNameText[_local2].color = 0;
        this.playerNameText[_local2].text = "Empty";
        if (this.choosenPlayerIdx != TSavegame.lastPlayer - 1) goto L707;
        _global._mainMenuScreen.playerNameText.text = "Guest";
      L707:
        TSavegame.clear(this.choosenPlayerIdx);
        this.showWindow(TWindowState.CHANGE_PLAYER);
      L729:
        goto L4017;
      L733:
        if (_local3 != TNonStateButton.CREATE_BUTOTN) goto L759;
        this.showWindow(TWindowState.CREATE_PLAYER);
        goto L4017;
      L759:
        if (_local3 != TNonStateButton.CANCEL_BUTTON) goto L1287;
        if (this.activeState != TWindowState.CHANGE_PLAYER) goto L884;
        this.widgetBG.visible = false;
        _blackScreen.visible = false;
        this.widgetMenu.visible = false;
        this.widgetInfo.visible = false;
        this.buttonLeft.visible = false;
        this.buttonRight.visible = false;
        this.uniButton.visible = false;
        _local2 = 0;
        goto L873;
      L846:
        this.playerNameText[_local2].visible = false;
        this.underLine[_local2].visible = false;
        _local2++;
      L873:
        if (_local2 < 5) goto L846;
        goto L1283;
      L884:
        if (this.activeState != TWindowState.CREATE_PLAYER) goto L969;
        this.smallButtonLeft.visible = false;
        this.smallButtonRight.visible = false;
        this.smallWidgetBG.visible = false;
        this.smallWidgetInfo.visible = false;
        this.smallWidgetMenu.visible = false;
        this.inputName.visible = false;
        this.smallButtonRight.enabled = true;
        this.showWindow(TWindowState.CHANGE_PLAYER);
        goto L1283;
      L969:
        if (this.activeState != TWindowState.DELETE_PLAYER) goto L1038;
        this.smallButtonLeft.visible = false;
        this.smallButtonRight.visible = false;
        this.smallWidgetBG.visible = false;
        this.smallWidgetInfo.visible = false;
        this.smallWidgetMenu.visible = false;
        this.showWindow(TWindowState.CHANGE_PLAYER);
        goto L1283;
      L1038:
        if (this.activeState != TWindowState.EXTRAS_MENU) goto L1157;
        _blackScreen.visible = false;
        this.widgetBG.visible = false;
        this.widgetMenu.visible = false;
        this.buttonLeft.visible = false;
        this.buttonRight.visible = false;
        this.uniButton.visible = false;
        this.buttonLeft.scaleX = 0.8;
        this.buttonLeft.scaleY = 0.8;
        this.buttonRight.scaleX = 0.8;
        this.buttonRight.scaleY = 0.8;
        this.uniButton.scaleX = 0.8;
        this.uniButton.scaleY = 0.8;
        goto L1283;
      L1157:
        if (this.activeState != TWindowState.UNLOCK_FULL_VERSION) goto L1222;
        _blackScreen.visible = false;
        this.smallButtonLeft.visible = false;
        this.smallButtonRight.visible = false;
        this.smallWidgetBG.visible = false;
        this.smallWidgetInfo.visible = false;
        this.smallWidgetMenu.visible = false;
        goto L1283;
      L1222:
        if (this.activeState != TWindowState.SHOW_FREE_COINS) goto L1283;
        _blackScreen.visible = false;
        this.widgetBG.visible = false;
        this.widgetMenu.visible = false;
        this.widgetInfo.visible = false;
        this.buttonLeft.visible = false;
        this.buttonRight.visible = false;
      L1283:
        goto L4017;
      L1287:
        if (_local3 != TNonStateButton.DONE_CREATE_PLAYER) goto L1644;
        this.smallButtonLeft.visible = false;
        this.smallButtonRight.visible = false;
        this.smallWidgetBG.visible = false;
        this.smallWidgetInfo.visible = false;
        this.smallWidgetMenu.visible = false;
        this.inputName.visible = false;
        _local2 = 0;
        goto L1389;
      L1353:
        if (this.inputName.text != TSavegame.playerName[_local2]) goto L1387;
        this.showWindow(TWindowState.SAME_NAME_WARNING);
        return;
      L1387:
        _local2++;
      L1389:
        if (_local2 < 5) goto L1353;
        _local4 = -1;
        _local6 = this.inputName.text;
        _local2 = 0;
        goto L1467;
      L1417:
        if (_local6.charAt(_local2) != " ") goto L1440;
        _local5 = uint(_local2);
        goto L1465;
      L1440:
        if (_local4 != -1) goto L1452;
        _local4 = int(_local2);
      L1452:
        _local5 = uint(this.inputName.text.length);
      L1465:
        _local2++;
      L1467:
        if (_local2 < this.inputName.text.length) goto L1417;
        this.inputName.text = _local6.substring(_local4, _local5);
        _local2 = 0;
        goto L1596;
      L1506:
        if (TSavegame.usedSlot[_local2]) goto L1594;
        TSavegame.usedSlot[_local2] = true;
        TSavegame.playerName[_local2] = this.inputName.text;
        TSavegame.lastPlayer = _local2 + 1;
        TSavegame._saveGame.data.lastPlayer = _local2 + 1;
        TSavegame.loadPlayer(_local2);
        TSavegame.save();
        goto L1603;
      L1594:
        _local2++;
      L1596:
        if (_local2 < 5) goto L1506;
      L1603:
        this.choosenPlayerIdx = _local2;
        this.playerNameText[_local2].color = 16711680;
        this.buttonLeft.enabled = false;
        this.showWindow(TWindowState.CHANGE_PLAYER);
        goto L4017;
      L1644:
        if (_local3 != TNonStateButton.OK_BUTTON) goto L1970;
        if (this.activeState != TWindowState.SAME_NAME_WARNING) goto L1716;
        this.smallWidgetBG.visible = false;
        this.smallUniButton.visible = false;
        this.smallWidgetInfo.visible = false;
        this.smallWidgetMenu.visible = false;
        this.showWindow(TWindowState.CREATE_PLAYER);
        goto L1966;
      L1716:
        if (this.activeState != TWindowState.SHOW_FREE_COINS) goto L1866;
        if (TGlobalClass.VideoReady != true) goto L1851;
        this.widgetBG.visible = false;
        _blackScreen.visible = false;
        this.widgetMenu.visible = false;
        this.widgetInfo.visible = false;
        this.buttonLeft.visible = false;
        this.buttonRight.visible = false;
        TSavegame.currentPlayer.money = _global._maxCrypto.ITS(_global._maxCrypto.STI(TSavegame.currentPlayer.money) + 3);
        TSavegame.save();
        this.showWindow(TWindowState.COINS_REWARDED);
        goto L1862;
      L1851:
        this.showWindow(TWindowState.VIDEO_ADS_NOT_AVAILABLE);
      L1862:
        goto L1966;
      L1866:
        if (this.activeState != TWindowState.VIDEO_ADS_NOT_AVAILABLE) goto L1927;
        this.smallWidgetBG.visible = false;
        this.smallUniButton.visible = false;
        this.smallWidgetInfo.visible = false;
        this.smallWidgetMenu.visible = false;
        this.showWindow(TWindowState.SHOW_FREE_COINS);
        goto L1966;
      L1927:
        _blackScreen.visible = false;
        this.smallWidgetBG.visible = false;
        this.smallUniButton.visible = false;
        this.smallWidgetInfo.visible = false;
        this.smallWidgetMenu.visible = false;
      L1966:
        goto L4017;
      L1970:
        if (_local3 != TNonStateButton.SELECT_BUTTON) goto L2138;
        this.buttonRight.enabled = true;
        this.widgetBG.visible = false;
        _blackScreen.visible = false;
        this.widgetMenu.visible = false;
        this.widgetInfo.visible = false;
        this.buttonLeft.visible = false;
        this.buttonRight.visible = false;
        this.uniButton.visible = false;
        _local2 = 0;
        goto L2078;
      L2051:
        this.playerNameText[_local2].visible = false;
        this.underLine[_local2].visible = false;
        _local2++;
      L2078:
        if (_local2 < 5) goto L2051;
        TSavegame.lastPlayer = this.choosenPlayerIdx + 1;
        _global._mainMenuScreen.playerNameText.text = TSavegame.playerName[this.choosenPlayerIdx];
        TSavegame.loadPlayer(this.choosenPlayerIdx);
        goto L4017;
      L2138:
        if (_local3 != TNonStateButton.RESUME_BUTTON) goto L2344;
        _blackScreen.visible = false;
        this.widgetBG.visible = false;
        this.widgetInfo.visible = false;
        this.widgetMenu.visible = false;
        this.buttonLeft.visible = false;
        this.buttonRight.visible = false;
        this.soundText.visible = false;
        this.musicText.visible = false;
        this.fullScreenText.visible = false;
        this.soundButton.visible = false;
        this.musicButton.visible = false;
        this.fullScreenButton.visible = false;
        if (_global.currentSceneID != _global.GAMESCREEN_ID) goto L2274;
        _global.gameScreen.paused = false;
        goto L2340;
      L2274:
        if (_global.currentSceneID != _global.HIDDEN_OBJECT_ID) goto L2304;
        _global._minigameHO.paused = false;
        goto L2340;
      L2304:
        if (_global.currentSceneID != _global.MAKE_DESSERT_ID) goto L2340;
        _global._minigameDessert.paused = false;
        _global._minigameDessert.Resume();
      L2340:
        goto L4017;
      L2344:
        if (_local3 != TNonStateButton.MAINMENU_BUTTON) goto L2400;
        if (!TGlobalClass.extrasMode) goto L2385;
        _global._game.fadeToScreen(_global._mainMenuScreen);
        goto L2396;
      L2385:
        this.showWindow(TWindowState.CONFIRM_MAINMENU);
      L2396:
        goto L4017;
      L2400:
        if (_local3 != TNonStateButton.SOUND_BUTTON) goto L2618;
        _global.MuteSound = !_global.MuteSound;
        if (!_global.MuteSound) goto L2471;
        this.soundButton._text.text = "OFF";
        this.soundButton._image.texture = windowsAtlas.getTexture("small_btn_red");
        goto L2504;
      L2471:
        this.soundButton._text.text = "ON";
        this.soundButton._image.texture = windowsAtlas.getTexture("small_btn_green");
      L2504:
        if (_global.currentSceneID != _global.GAMESCREEN_ID) goto L2542;
        _global.gameScreen.mute_icon.visible = _global.MuteSound;
        goto L2614;
      L2542:
        if (_global.currentSceneID != _global.MAKE_DESSERT_ID) goto L2580;
        _global._minigameDessert.mute.visible = _global.MuteSound;
        goto L2614;
      L2580:
        if (_global.currentSceneID != _global.HIDDEN_OBJECT_ID) goto L2614;
        _global._minigameHO.mute.visible = _global.MuteSound;
      L2614:
        goto L4017;
      L2618:
        if (_local3 != TNonStateButton.MUSIC_BUTTON) goto L2836;
        _global.MuteMusic = !_global.MuteMusic;
        if (!_global.MuteMusic) goto L2689;
        this.musicButton._text.text = "OFF";
        this.musicButton._image.texture = windowsAtlas.getTexture("small_btn_red");
        goto L2722;
      L2689:
        this.musicButton._text.text = "ON";
        this.musicButton._image.texture = windowsAtlas.getTexture("small_btn_green");
      L2722:
        if (_global.currentSceneID != _global.GAMESCREEN_ID) goto L2760;
        _global.gameScreen.mute_icon_music.visible = _global.MuteMusic;
        goto L2832;
      L2760:
        if (_global.currentSceneID != _global.MAKE_DESSERT_ID) goto L2798;
        _global._minigameDessert.muteMusic.visible = _global.MuteMusic;
        goto L2832;
      L2798:
        if (_global.currentSceneID != _global.HIDDEN_OBJECT_ID) goto L2832;
        _global._minigameHO.muteMusic.visible = _global.MuteMusic;
      L2832:
        goto L4017;
      L2836:
        if (_local3 != TNonStateButton.KITCHEN) goto L2875;
        TGlobalClass.coinsGot = 0;
        _global._game.fadeToScreen(_global._kitchen);
        goto L4017;
      L2875:
        if (_local3 != TNonStateButton.KITCHEN_WITH_CONFIRMATION) goto L2901;
        this.showWindow(TWindowState.CONFIRM_KITCHEN);
        goto L4017;
      L2901:
        if (_local3 != TNonStateButton.ENDING) goto L2932;
        _global._game.fadeToScreen(_global._ending);
        goto L4017;
      L2932:
        if (_local3 != TNonStateButton.NEXT_WINDOW) goto L3530;
        var _local7 = 1;
        this.widgetBG.scaleY = 1;
        this.widgetBG.scaleX = _local7;
        _local7 = TGlobalClass;
        var _local8 = TGlobalClass.currentNotifWindow + 1;
        _local7.currentNotifWindow = _local8;
        var $t3 = Boolean(TSavegame.currentPlayer.passedDays < 60);
        $s0 = $t3;
        if (!$t3) goto L3044;
        $s0 = Boolean(TGlobalClass.levelEventXML.day[TSavegame.currentPlayer.passedDays].kitchen[TGlobalClass.currentNotifWindow]);
      L3044:
        if (!$s0) goto L3063;
        $s0 = _global.currentSceneID == _global.KITCHEN_ID;
      L3063:
        if (!$s0) goto L3091;
        this.showKitchenWindow(TSavegame.currentPlayer.passedDays, TGlobalClass.currentNotifWindow);
        goto L3526;
      L3091:
        var $t4 = Boolean(TSavegame.currentPlayer.passedDays < 60);
        $s0 = $t4;
        if (!$t4) goto L3143;
        $s0 = Boolean(TGlobalClass.levelEventXML.day[TGlobalClass.currentPlayedLevel].window[TGlobalClass.currentNotifWindow]);
      L3143:
        if (!$s0) goto L3163;
        $s0 = !(_global.currentSceneID == _global.KITCHEN_ID);
      L3163:
        if (!$s0) goto L3188;
        this.showBeginningWindow(TGlobalClass.currentPlayedLevel, TGlobalClass.currentNotifWindow);
        goto L3526;
      L3188:
        if (_global.currentSceneID != _global.GAMESCREEN_ID) goto L3244;
        _global.gameScreen.paused = false;
        if (_global.tutorial.button.visible) goto L3240;
        _global.gameScreen.startLevel();
      L3240:
        goto L3316;
      L3244:
        if (_global.currentSceneID != _global.HIDDEN_OBJECT_ID) goto L3274;
        _global._minigameHO.startLevel();
        goto L3316;
      L3274:
        if (_global.currentSceneID != _global.MAKE_DESSERT_ID) goto L3316;
        if (_global.tutorial.button.visible) goto L3316;
        _global._minigameDessert.startLevel();
      L3316:
        if (!this.widgetBG.visible) goto L3431;
        _blackScreen.visible = false;
        this.widgetBG.visible = false;
        this.widgetInfo.visible = false;
        this.widgetMenu.visible = false;
        this.buttonLeft.visible = false;
        this.buttonRight.visible = false;
        this.uniButton.visible = false;
        if (_global.currentSceneID != _global.GAMESCREEN_ID) goto L3427;
        _local2 = 0;
        goto L3420;
      L3405:
        this.dishPreviewAr[_local2].visible = false;
        _local2++;
      L3420:
        if (_local2 < 4) goto L3405;
      L3427:
        goto L3526;
      L3431:
        if (!this.smallWidgetBG.visible) goto L3526;
        _blackScreen.visible = false;
        this.smallWidgetBG.visible = false;
        this.smallWidgetInfo.visible = false;
        this.smallWidgetMenu.visible = false;
        this.smallButtonLeft.visible = false;
        this.smallButtonRight.visible = false;
        this.smallUniButton.visible = false;
        _local2 = 0;
        goto L3519;
      L3504:
        this.dishPreviewAr[_local2].visible = false;
        _local2++;
      L3519:
        if (_local2 < 4) goto L3504;
      L3526:
        goto L4017;
      L3530:
        if (_local3 != TNonStateButton.HO_BUTTON) goto L3580;
        TGlobalClass.extrasMode = true;
        _global._minigameHO.bonusMoney = "a";
        _global._game.fadeToScreen(_global._minigameHO);
        goto L4017;
      L3580:
        if (_local3 != TNonStateButton.DM_BUTTON) goto L3618;
        TGlobalClass.extrasMode = true;
        _global._game.fadeToScreen(_global._minigameDessert);
        goto L4017;
      L3618:
        if (_local3 != TNonStateButton.NEXT_LEVEL) goto L3675;
        var $t5 = _global._minigameHO;
        _local7 = $t5;
        _local8 = $t5.currentLevel + 1;
        _local7.currentLevel = _local8;
        _global._game.fadeToScreen(_global._minigameHO);
        goto L4017;
      L3675:
        if (_local3 != TNonStateButton.SUBMIT_BUTTON) goto L3828;
        if (TGlobalClass.extrasMode) goto L3734;
        GALib.TrackEvent(GALib.EVENT_SUBMIT_SCORE, TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS], "STORY MODE");
        goto L3824;
      L3734:
        if (_global.currentSceneID != _global.MAKE_DESSERT_ID) goto L3789;
        GALib.TrackEvent(GALib.EVENT_SUBMIT_SCORE, _global._maxCrypto.STI(_global._minigameDessert.earning), "DESSERT MAKING MODE");
        goto L3824;
      L3789:
        GALib.TrackEvent(GALib.EVENT_SUBMIT_SCORE, _global._maxCrypto.STI(_global._minigameHO.bonusMoney), "HIDDEN OBJECTS MODE");
      L3824:
        goto L4017;
      L3828:
        if (_local3 != TNonStateButton.LEADERBOARD_BUTTON) goto L3843;
        goto L4017;
      L3843:
        if (_local3 != TNonStateButton.UNLOCK_BUTTON) goto L3858;
        goto L4017;
      L3858:
        if (_local3 != TNonStateButton.FULLSCREEN_BUTTON) goto L4017;
        TSavegame.fullScreen = !TSavegame.fullScreen;
        $s0 = _global._stage;
        if (!TSavegame.fullScreen) goto L3909;
        $s1 = StageDisplayState.FULL_SCREEN_INTERACTIVE;
        goto L3916;
      L3909:
        $s1 = StageDisplayState.NORMAL;
      L3916:
        $s0.displayState = $s1;
        TSavegame._saveGame.data.fullScreen = TSavegame.fullScreen;
        if (TSavegame.fullScreen) goto L3984;
        this.fullScreenButton._text.text = "OFF";
        this.fullScreenButton._image.texture = windowsAtlas.getTexture("small_btn_red");
        goto L4017;
      L3984:
        this.fullScreenButton._text.text = "ON";
        this.fullScreenButton._image.texture = windowsAtlas.getTexture("small_btn_green");
      L4017:
    }

    function hideAllWindows(arg1:int):void {
        var _local2 = 0;
        if (arg1 != _global.MAINMENU_ID) goto L65;
        _local2 = 0;
        goto L50;
      L23:
        this.playerNameText[_local2].visible = false;
        this.underLine[_local2].visible = false;
        _local2++;
      L50:
        if (_local2 < 5) goto L23;
        this.inputName.visible = false;
      L65:
        var $t0 = arg1 == _global.GAMESCREEN_ID;
        $s0 = $t0;
        if ($t0) goto L87;
        $s0 = arg1 == _global.HIDDEN_OBJECT_ID;
      L87:
        if ($s0) goto L101;
        $s0 = arg1 == _global.MAKE_DESSERT_ID;
      L101:
        if (!$s0) goto L193;
        this.soundButton.visible = false;
        this.soundText.visible = false;
        this.musicButton.visible = false;
        this.musicText.visible = false;
        this.fullScreenButton.visible = false;
        this.fullScreenText.visible = false;
        this.summaryStats.visible = false;
        this.summaryTips.visible = false;
        this.summaryText.visible = false;
        this.goalIcon.visible = false;
        this.expertBlink.visible = false;
      L193:
        this.widgetBG.visible = false;
        this.widgetInfo.visible = false;
        this.widgetMenu.visible = false;
        this.uniButton.visible = false;
        this.buttonLeft.visible = false;
        this.buttonRight.visible = false;
        this.smallButtonLeft.visible = false;
        this.smallButtonRight.visible = false;
        this.smallUniButton.visible = false;
        this.smallWidgetBG.visible = false;
        this.smallWidgetInfo.visible = false;
        this.smallWidgetMenu.visible = false;
        _blackScreen.visible = false;
    }

    function showWindow(arg1:int):void {
        var _local2 = 0;
        var _local3 = 0;
        var _local4 = 0;
        var _local5 = NaN;
        var _local6 = NaN;
        var _local7 = false;
        var _local8 = null;
        this.activeState = arg1;
        if (arg1 != TWindowState.EXIT_WINDOW) goto L397;
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
        this.smallWidgetBG.visible = true;
        this.smallWidgetMenu.visible = true;
        this.smallWidgetInfo.visible = true;
        this.smallUniButton.visible = true;
        this.smallButtonLeft.visible = true;
        this.smallButtonRight.visible = true;
        this.smallButtonLeft._text.text = "No";
        this.smallButtonRight._text.text = "Yes";
        this.smallUniButton._text.text = "More Games";
        this.smallWidgetMenu.text = "Exit?";
        this.smallWidgetInfo.text = "Are you sure want to exit game?";
        this.smallWidgetBG.x = 512;
        this.smallWidgetBG.y = 300;
        this.smallWidgetMenu.x = 525;
        this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
        this.smallWidgetInfo.x = 525;
        this.smallWidgetInfo.y = this.smallWidgetBG.y - 30;
        this.smallButtonLeft.x = this.smallWidgetBG.x - 100;
        this.smallButtonLeft.y = this.smallWidgetBG.y + 130;
        this.smallButtonRight.x = this.smallWidgetBG.x + 100;
        this.smallButtonRight.y = this.smallWidgetBG.y + 130;
        this.smallUniButton.x = 525;
        this.smallUniButton.y = this.smallWidgetBG.y + 80;
        this.smallUniButton.idx = TNonStateButton.MOREGAMES_BUTTON;
        this.smallButtonLeft.idx = TNonStateButton.NO_BUTTON;
        this.smallButtonRight.idx = TNonStateButton.YES_BUTTON;
        goto L11119;
      L397:
        if (arg1 != TWindowState.CHANGE_PLAYER) goto L931;
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetMenu.visible = true;
        this.buttonLeft.visible = true;
        this.buttonRight.visible = true;
        this.uniButton.visible = true;
        _local2 = 0;
        goto L593;
      L490:
        this.underLine[_local2].visible = true;
        this.playerNameText[_local2].visible = true;
        this.playerNameText[_local2].idx = _local2;
        this.playerNameText[_local2].text = "Empty";
        this.playerNameText[_local2].color = 0;
        if (!TSavegame.usedSlot[_local2]) goto L591;
        this.playerNameText[_local2].text = TSavegame.playerName[_local2];
        _local3++;
      L591:
        _local2++;
      L593:
        if (_local2 < 5) goto L490;
        this.buttonLeft.enabled = false;
        this.uniButton._text.text = "Close";
        this.uniButton.idx = TNonStateButton.CANCEL_BUTTON;
        if (!(_local3 >= 5)) goto L653;
        this.buttonRight.enabled = false;
        goto L661;
      L653:
        this.buttonRight.enabled = true;
      L661:
        var $t0 = int(TSavegame.lastPlayer);
        _local4 = $t0;
        if (!($t0 > 0)) goto L733;
        this.playerNameText[_local4 - 1].color = 16711680;
        this.choosenPlayerIdx = _local4 - 1;
        this.buttonLeft.enabled = true;
        this.uniButton._text.text = "Select";
        this.uniButton.idx = TNonStateButton.SELECT_BUTTON;
      L733:
        this.widgetMenu.text = "Select Player";
        this.buttonLeft._text.text = "Delete";
        this.buttonRight._text.text = "Create";
        this.widgetMenu.x = 512;
        this.widgetMenu.y = this.widgetBG.y - 220;
        this.buttonLeft.x = this.widgetBG.x - 170;
        this.buttonLeft.y = this.widgetBG.y + 200;
        this.buttonRight.x = this.widgetBG.x + 170;
        this.buttonRight.y = this.widgetBG.y + 200;
        this.uniButton.x = this.widgetBG.x;
        this.uniButton.y = this.widgetBG.y + 200;
        this.buttonLeft.idx = TNonStateButton.DELETE_BUTTON;
        this.buttonRight.idx = TNonStateButton.CREATE_BUTOTN;
        goto L11119;
      L931:
        if (arg1 != TWindowState.CREATE_PLAYER) goto L1378;
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
        this.smallWidgetBG.visible = true;
        this.smallButtonLeft.visible = true;
        this.smallButtonRight.visible = true;
        this.smallWidgetMenu.visible = true;
        this.smallWidgetInfo.visible = true;
        this.inputName.visible = true;
        this.smallWidgetMenu.text = "Create Player";
        this.smallWidgetInfo.text = "Enter Your Name :";
        this.smallButtonLeft._text.text = "Cancel";
        this.smallButtonRight._text.text = "Done";
        this.inputName.text = "";
        this.smallButtonRight.enabled = false;
        Starling.current.nativeStage.focus = this.inputName;
        this.smallWidgetBG.x = 512;
        this.smallWidgetBG.y = 300;
        this.smallWidgetMenu.x = 525;
        this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
        this.smallWidgetInfo.x = 525;
        this.smallWidgetInfo.y = this.smallWidgetBG.y - 30;
        this.smallButtonLeft.x = this.smallWidgetBG.x - 100;
        this.smallButtonLeft.y = this.smallWidgetBG.y + 130;
        this.smallButtonRight.x = this.smallWidgetBG.x + 100;
        this.smallButtonRight.y = this.smallWidgetBG.y + 130;
        _local5 = Number(_global._stage.stageWidth / 1024);
        _local6 = Number(_global._stage.stageHeight / 600);
        this.inputName.scaleX = _local5;
        this.inputName.scaleY = _local6;
        this.inputName.x = _global._stage.stageWidth / 2 - (this.inputName.width / 2 - 10);
        this.inputName.y = this.smallWidgetBG.y * _local6;
        this.smallButtonRight.idx = TNonStateButton.DONE_CREATE_PLAYER;
        this.smallButtonLeft.idx = TNonStateButton.CANCEL_BUTTON;
        goto L11119;
      L1378:
        if (arg1 != TWindowState.SAME_NAME_WARNING) goto L1612;
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
        this.smallWidgetBG.visible = true;
        this.smallUniButton.visible = true;
        this.smallWidgetMenu.visible = true;
        this.smallWidgetInfo.visible = true;
        this.smallWidgetMenu.text = "Try Again!";
        this.smallWidgetInfo.text = "Oops, the name you entered is already exist. Please try again!";
        this.smallUniButton._text.text = "OK";
        this.smallWidgetBG.x = 512;
        this.smallWidgetBG.y = 300;
        this.smallWidgetMenu.x = 525;
        this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
        this.smallWidgetInfo.x = 525;
        this.smallWidgetInfo.y = this.smallWidgetBG.y - 30;
        this.smallUniButton.x = this.smallWidgetBG.x;
        this.smallUniButton.y = this.smallWidgetBG.y + 130;
        this.smallUniButton.idx = TNonStateButton.OK_BUTTON;
        goto L11119;
      L1612:
        if (arg1 != TWindowState.DELETE_PLAYER) goto L1918;
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
        this.smallWidgetBG.visible = true;
        this.smallButtonLeft.visible = true;
        this.smallButtonRight.visible = true;
        this.smallWidgetMenu.visible = true;
        this.smallWidgetInfo.visible = true;
        this.smallWidgetMenu.text = "Delete Player?";
        this.smallWidgetInfo.text = "Are you sure want to delete this player? This process can't be undone.";
        this.smallButtonLeft._text.text = "Cancel";
        this.smallButtonRight._text.text = "Delete";
        this.smallWidgetBG.x = 512;
        this.smallWidgetBG.y = 300;
        this.smallWidgetMenu.x = 525;
        this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
        this.smallWidgetInfo.x = 525;
        this.smallWidgetInfo.y = this.smallWidgetBG.y - 30;
        this.smallButtonLeft.x = this.smallWidgetBG.x - 100;
        this.smallButtonLeft.y = this.smallWidgetBG.y + 130;
        this.smallButtonRight.x = this.smallWidgetBG.x + 100;
        this.smallButtonRight.y = this.smallWidgetBG.y + 130;
        this.smallButtonLeft.idx = TNonStateButton.CANCEL_BUTTON;
        this.smallButtonRight.idx = TNonStateButton.DELETE_BUTTON;
        goto L11119;
      L1918:
        if (arg1 != TWindowState.NEW_GAME) goto L1933;
        goto L11119;
      L1933:
        if (arg1 != TWindowState.PAUSE_GAME) goto L2979;
        if (!_global.gameScreen.tmpDrink) goto L1991;
        _global._gameS.setChildIndex(_global.gameScreen.tmpDrink, _global._gameS.getChildIndex(this.widgetBG) - 1);
      L1991:
        if (!_global.gameScreen.tmpFries) goto L2038;
        _global._gameS.setChildIndex(_global.gameScreen.tmpFries, _global._gameS.getChildIndex(this.widgetBG) - 1);
      L2038:
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetInfo.visible = true;
        this.widgetMenu.visible = true;
        this.buttonLeft.visible = true;
        this.buttonRight.visible = true;
        this.soundText.visible = true;
        this.soundButton.visible = true;
        this.musicText.visible = true;
        this.musicButton.visible = true;
        this.fullScreenText.visible = true;
        this.fullScreenButton.visible = true;
        if (!TGlobalClass.extrasMode) goto L2179;
        this.widgetInfo.visible = false;
      L2179:
        this.widgetMenu.text = "Paused";
        $s0 = this.buttonLeft._text;
        if (!TGlobalClass.extrasMode) goto L2213;
        $s1 = "Main Menu";
        goto L2216;
      L2213:
        $s1 = "Kitchen";
      L2216:
        $s0.text = $s1;
        this.buttonRight._text.text = "Resume";
        this.soundText.text = "SOUND :";
        if (!_global.MuteSound) goto L2289;
        this.soundButton._text.text = "OFF";
        this.soundButton._image.texture = windowsAtlas.getTexture("small_btn_red");
        goto L2322;
      L2289:
        this.soundButton._text.text = "ON";
        this.soundButton._image.texture = windowsAtlas.getTexture("small_btn_green");
      L2322:
        if (!_global.MuteMusic) goto L2369;
        this.musicButton._text.text = "OFF";
        this.musicButton._image.texture = windowsAtlas.getTexture("small_btn_red");
        goto L2402;
      L2369:
        this.musicButton._text.text = "ON";
        this.musicButton._image.texture = windowsAtlas.getTexture("small_btn_green");
      L2402:
        if (!_global.MuteMusic) goto L2449;
        this.musicButton._text.text = "OFF";
        this.musicButton._image.texture = windowsAtlas.getTexture("small_btn_red");
        goto L2482;
      L2449:
        this.musicButton._text.text = "ON";
        this.musicButton._image.texture = windowsAtlas.getTexture("small_btn_green");
      L2482:
        if (TSavegame.fullScreen) goto L2529;
        this.fullScreenButton._text.text = "OFF";
        this.fullScreenButton._image.texture = windowsAtlas.getTexture("small_btn_red");
        goto L2562;
      L2529:
        this.fullScreenButton._text.text = "ON";
        this.fullScreenButton._image.texture = windowsAtlas.getTexture("small_btn_green");
      L2562:
        this.soundButton.x = this.widgetBG.x;
        this.soundButton.y = this.widgetBG.y - 110;
        this.soundText.x = this.widgetBG.x - 200;
        this.soundText.y = this.widgetBG.y - 100;
        this.musicButton.x = this.widgetBG.x;
        this.musicButton.y = this.widgetBG.y - 40;
        this.musicText.x = this.widgetBG.x - 200;
        this.musicText.y = this.widgetBG.y - 40;
        this.fullScreenButton.x = this.widgetBG.x;
        this.fullScreenButton.y = this.widgetBG.y + 30;
        this.fullScreenText.x = this.widgetBG.x - 240;
        this.fullScreenText.y = this.widgetBG.y + 30;
        this.widgetMenu.x = 512;
        this.widgetMenu.y = this.widgetBG.y - 220;
        this.widgetInfo.x = 512;
        this.widgetInfo.y = this.widgetBG.y + 130;
        this.buttonLeft.x = this.widgetBG.x - 150;
        this.buttonLeft.y = this.widgetBG.y + 200;
        this.buttonRight.x = this.widgetBG.x + 140;
        this.buttonRight.y = this.widgetBG.y + 200;
        this.soundButton.idx = TNonStateButton.SOUND_BUTTON;
        this.musicButton.idx = TNonStateButton.MUSIC_BUTTON;
        this.fullScreenButton.idx = TNonStateButton.FULLSCREEN_BUTTON;
        $s0 = this.buttonLeft;
        if (!TGlobalClass.extrasMode) goto L2952;
        $s1 = uint(TNonStateButton.MAINMENU_BUTTON);
        goto L2959;
      L2952:
        $s1 = uint(TNonStateButton.KITCHEN_WITH_CONFIRMATION);
      L2959:
        $s0.idx = $s1;
        this.buttonRight.idx = TNonStateButton.RESUME_BUTTON;
        goto L11119;
      L2979:
        if (arg1 != TWindowState.STORY_LEVELCOMPLETE) goto L3757;
        _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
        GALib.TrackEvent(GALib.EVENT_COMPLETE_STORY, TGlobalClass.currentPlayedLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetInfo.visible = true;
        this.widgetMenu.visible = true;
        this.uniButton.visible = true;
        this.summaryStats.visible = true;
        this.summaryTips.visible = true;
        this.summaryText.visible = true;
        _local7 = Boolean(this.showGoalIcon());
        _local8 = String(ScoreManager.totalScore);
        if (!TGlobalClass.isPlayingSpecialLevel) goto L3353;
        this.summaryText.text = "Customers Served\t\t :\nBeverage Served\t\t :\nFrench Fries Served\t :\nVIP Rating Bonus\t\t :\n\nToday’s Earnings\t\t :\nTotal Coins\t\t\t :";
        $s0 = this.summaryStats;
        $s1 = _global.gameScreen.servedCustomerCount + "\n";
        if (!(TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] == 0)) goto L3223;
        $s2 = "-";
        goto L3230;
      L3223:
        $s2 = TGlobalClass.juiceServed;
      L3230:
        $s1 = $s1 + $s2 + "\n";
        if (!(TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] == 0)) goto L3268;
        $s2 = "-";
        goto L3275;
      L3268:
        $s2 = TGlobalClass.friesServed;
      L3275:
        $s0.text = $s1 + $s2 + "\n" + _global.gameScreen.VIPServeBonus + "\n\n$" + _global._maxCrypto.STI(_local8) + "\n" + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
        goto L3525;
      L3353:
        this.summaryText.text = "Customers Served\t\t :\nBeverage Served\t\t :\nFrench Fries Served\t :\n\nToday's Earnings\t\t :\nTotal Coins\t\t\t :";
        $s0 = this.summaryStats;
        $s1 = _global.gameScreen.servedCustomerCount + "\n";
        if (!(TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] == 0)) goto L3413;
        $s2 = "-";
        goto L3420;
      L3413:
        $s2 = TGlobalClass.juiceServed;
      L3420:
        $s1 = $s1 + $s2 + "\n";
        if (!(TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] == 0)) goto L3458;
        $s2 = "-";
        goto L3465;
      L3458:
        $s2 = TGlobalClass.friesServed;
      L3465:
        $s0.text = $s1 + $s2 + "\n\n$" + _global._maxCrypto.STI(_local8) + "\n" + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
      L3525:
        this.widgetMenu.text = "Congratulations!";
        this.widgetInfo.text = "You've reached today's goal!";
        this.uniButton._text.text = "Kitchen";
        $s0 = this.summaryTips;
        if (!_local7) goto L3575;
        $s1 = "Awesome! Couldn't have done it better myself!";
        goto L3578;
      L3575:
        $s1 = "Well done! Try to reach Expert target to earn even more coins!";
      L3578:
        $s0.text = $s1;
        this.widgetBG.x = 512;
        this.widgetBG.y = 300;
        this.widgetMenu.x = 512;
        this.widgetMenu.y = this.widgetBG.y - 220;
        this.widgetInfo.x = 512;
        this.widgetInfo.y = this.widgetBG.y - 170;
        this.uniButton.x = 512;
        this.uniButton.y = this.widgetBG.y + 200;
        this.uniButton.idx = TNonStateButton.KITCHEN;
        _global.psCongrats.start();
        TweenLite.delayedCall(6, _global.psCongrats.pause);
        var $t1 = TGlobalClass.EnableLeaderboardBtn;
        var _local9 = $t1;
        this.submitScoreBtn.visible = $t1;
        this.leaderboardBtn.visible = _local9;
        goto L11119;
      L3757:
        if (arg1 != TWindowState.STORY_LEVELFAILED) goto L4226;
        GALib.TrackEvent(GALib.EVENT_LOSE_STORY, TGlobalClass.currentPlayedLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
        _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetInfo.visible = true;
        this.widgetMenu.visible = true;
        this.uniButton.visible = true;
        this.summaryStats.visible = true;
        this.summaryTips.visible = true;
        this.summaryText.visible = true;
        this.summaryText.text = "Customers Served\t\t :\nBeverage Served\t\t :\nFrench Fries Served\t :\n\nTotal Coins\t\t\t :";
        $s0 = this.summaryStats;
        $s1 = _global.gameScreen.servedCustomerCount + "\n";
        if (!(TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] == 0)) goto L3967;
        $s2 = "-";
        goto L3974;
      L3967:
        $s2 = TGlobalClass.juiceServed;
      L3974:
        $s1 = $s1 + $s2 + "\n";
        if (!(TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] == 0)) goto L4012;
        $s2 = "-";
        goto L4019;
      L4012:
        $s2 = TGlobalClass.friesServed;
      L4019:
        $s0.text = $s1 + $s2 + "\n\n" + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
        this.widgetMenu.text = "Level Failed!";
        this.widgetInfo.text = "You've failed to meet today's goal!";
        this.uniButton._text.text = "Kitchen";
        this.summaryTips.text = "If you reach the normal Daily Goal you will receive 3 coins, but when you reach the Expert Goal you'll earn 6 coins.";
        this.widgetBG.x = 512;
        this.widgetBG.y = 300;
        this.widgetMenu.x = 512;
        this.widgetMenu.y = this.widgetBG.y - 220;
        this.widgetInfo.x = 512;
        this.widgetInfo.y = this.widgetBG.y - 180;
        this.uniButton.x = 512;
        this.uniButton.y = this.widgetBG.y + 200;
        this.uniButton.idx = TNonStateButton.KITCHEN;
        goto L11119;
      L4226:
        if (arg1 != TWindowState.STORY_COMPLETE_ALL) goto L4800;
        GALib.TrackEvent(GALib.EVENT_COMPLETE_STORY, TGlobalClass.currentPlayedLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
        _global.ReportGameState(_global.GAMESTATE_GAME_END);
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetInfo.visible = true;
        this.widgetMenu.visible = true;
        this.uniButton.visible = true;
        this.summaryText.visible = true;
        this.summaryStats.visible = true;
        this.showGoalIcon();
        _local8 = String(ScoreManager.totalScore);
        this.summaryText.text = "Customers Served\t\t :\nBeverage Served\t\t :\nFrench Fries Served\t :\n\nToday's Earnings\t\t :\nTotal Coins\t\t\t :";
        $s0 = this.summaryStats;
        $s1 = _global.gameScreen.servedCustomerCount + "\n";
        if (!(TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] == 0)) goto L4449;
        $s2 = "-";
        goto L4456;
      L4449:
        $s2 = TGlobalClass.juiceServed;
      L4456:
        $s1 = $s1 + $s2 + "\n";
        if (!(TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] == 0)) goto L4494;
        $s2 = "-";
        goto L4501;
      L4494:
        $s2 = TGlobalClass.friesServed;
      L4501:
        $s0.text = $s1 + $s2 + "\n\n$" + _global._maxCrypto.STI(_local8) + "\n" + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
        this.widgetMenu.text = "Congratulations!";
        this.widgetInfo.text = "You've reached today's goal and finished all levels!";
        this.uniButton._text.text = "Huraay!!";
        this.widgetBG.x = 512;
        this.widgetBG.y = 300;
        this.widgetMenu.x = 512;
        this.widgetMenu.y = this.widgetBG.y - 220;
        this.widgetInfo.x = 512;
        this.widgetInfo.y = this.widgetBG.y - 165;
        this.uniButton.x = 512;
        this.uniButton.y = this.widgetBG.y + 200;
        if (TSavegame.currentPlayer.endingViewed) goto L4728;
        this.uniButton.idx = TNonStateButton.ENDING;
        goto L4741;
      L4728:
        this.uniButton.idx = TNonStateButton.KITCHEN;
      L4741:
        _global.psCongrats.start();
        TweenLite.delayedCall(6, _global.psCongrats.pause);
        var $t2 = TGlobalClass.EnableLeaderboardBtn;
        _local9 = $t2;
        this.submitScoreBtn.visible = $t2;
        this.leaderboardBtn.visible = _local9;
        goto L11119;
      L4800:
        if (arg1 != TWindowState.HO_LEVEL_COMPLETE) goto L5287;
        GALib.TrackEvent(GALib.EVENT_COMPLETE_STORY, TGlobalClass.currentPlayedLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
        _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetInfo.visible = true;
        this.widgetMenu.visible = true;
        this.uniButton.visible = true;
        this.summaryStats.visible = true;
        this.summaryTips.visible = true;
        this.summaryText.visible = true;
        _local7 = Boolean(this.showGoalIcon());
        _local8 = _global._minigameHO.bonusMoney;
        this.summaryText.text = "Objects Found\t\t:\nToday's Earning\t:\n\nTotal Coins\t\t:";
        this.summaryStats.text = _global._minigameHO._HOClass.itemFoundCount + "\n$" + _global._maxCrypto.STI(_local8) + "\n\n" + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
        this.widgetMenu.text = "Summary";
        this.widgetInfo.text = "You've completed this level.";
        this.uniButton._text.text = "Kitchen";
        $s0 = this.summaryTips;
        if (!_local7) goto L5105;
        $s1 = "Awesome! Couldn't have done it better myself!";
        goto L5108;
      L5105:
        $s1 = "Well done! Try to reach Expert target to earn even more coins!";
      L5108:
        $s0.text = $s1;
        this.widgetBG.x = 512;
        this.widgetBG.y = 300;
        this.widgetMenu.x = 512;
        this.widgetMenu.y = this.widgetBG.y - 220;
        this.widgetInfo.x = 512;
        this.widgetInfo.y = this.widgetBG.y - 180;
        this.uniButton.x = 512;
        this.uniButton.y = this.widgetBG.y + 200;
        this.uniButton.idx = TNonStateButton.KITCHEN;
        _global.psCongrats.start();
        TweenLite.delayedCall(6, _global.psCongrats.pause);
        var $t3 = TGlobalClass.EnableLeaderboardBtn;
        _local9 = $t3;
        this.submitScoreBtn.visible = $t3;
        this.leaderboardBtn.visible = _local9;
        goto L11119;
      L5287:
        if (arg1 != TWindowState.HO_LEVEL_FAILED) goto L5726;
        GALib.TrackEvent(GALib.EVENT_LOSE_STORY, TGlobalClass.currentPlayedLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
        _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetInfo.visible = true;
        this.widgetMenu.visible = true;
        this.uniButton.visible = true;
        this.summaryStats.visible = true;
        this.summaryTips.visible = true;
        this.summaryText.visible = true;
        _local8 = _global._minigameHO.bonusMoney;
        this.summaryText.text = "Objects Found\t:\nToday's Earning\t:\n\nTotal Coins\t:";
        this.summaryStats.text = _global._minigameHO._HOClass.itemFoundCount + "\n$" + _global._maxCrypto.STI(_local8) + "\n\n" + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
        this.widgetMenu.text = "Level Failed";
        this.widgetInfo.text = "You've failed this level.";
        this.uniButton._text.text = "Kitchen";
        this.summaryTips.text = "If you reach the normal Daily Goal you will receive 3 coins, but when you reach the Expert Goal you'll earn 6 coins.";
        this.widgetBG.x = 512;
        this.widgetBG.y = 300;
        this.widgetMenu.x = 512;
        this.widgetMenu.y = this.widgetBG.y - 220;
        this.widgetInfo.x = 512;
        this.widgetInfo.y = this.widgetBG.y - 180;
        this.uniButton.x = 512;
        this.uniButton.y = this.widgetBG.y + 200;
        this.uniButton.idx = TNonStateButton.KITCHEN;
        _global.psCongrats.start();
        TweenLite.delayedCall(6, _global.psCongrats.pause);
        goto L11119;
      L5726:
        if (arg1 != TWindowState.HO_LEVEL_COMPLETE_EXTRAS) goto L6133;
        GALib.TrackEvent(GALib.EVENT_COMPLETE_EXTRAS, _global._minigameHO.currentLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
        _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetInfo.visible = true;
        this.widgetMenu.visible = true;
        this.uniButton.visible = true;
        this.summaryStats.visible = true;
        this.summaryText.visible = true;
        this.summaryText.text = "Objects Found\t:\nScore\t\t:";
        this.summaryStats.text = _global._minigameHO._HOClass.itemFoundCount + "\n" + _global._maxCrypto.STI(_global._minigameHO.bonusMoney);
        this.widgetMenu.text = "Summary";
        this.widgetInfo.text = "You've completed this level.";
        this.uniButton._text.text = "Next Level";
        this.widgetBG.x = 512;
        this.widgetBG.y = 300;
        this.widgetMenu.x = 512;
        this.widgetMenu.y = this.widgetBG.y - 220;
        this.widgetInfo.x = 512;
        this.widgetInfo.y = this.widgetBG.y - 180;
        this.uniButton.x = 512;
        this.uniButton.y = this.widgetBG.y + 200;
        this.uniButton.idx = TNonStateButton.NEXT_LEVEL;
        _global.psCongrats.start();
        TweenLite.delayedCall(6, _global.psCongrats.pause);
        var $t4 = TGlobalClass.EnableLeaderboardBtn;
        _local9 = $t4;
        this.submitScoreBtn.visible = $t4;
        this.leaderboardBtn.visible = _local9;
        goto L11119;
      L6133:
        if (arg1 != TWindowState.HO_LEVEL_FAILED_EXTRAS) goto L6485;
        GALib.TrackEvent(GALib.EVENT_LOSE_EXTRAS, _global._minigameHO.currentLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
        _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetInfo.visible = true;
        this.widgetMenu.visible = true;
        this.uniButton.visible = true;
        this.summaryStats.visible = true;
        this.summaryText.visible = true;
        this.summaryText.text = "Objects Found\t:\nScore\t\t:";
        this.summaryStats.text = _global._minigameHO._HOClass.itemFoundCount + "\n" + _global._maxCrypto.STI(_global._minigameHO.bonusMoney);
        this.widgetMenu.text = "Level Failed!";
        this.widgetInfo.text = "You've failed this level.";
        this.uniButton._text.text = "Main Menu";
        this.widgetBG.x = 512;
        this.widgetBG.y = 300;
        this.widgetMenu.x = 512;
        this.widgetMenu.y = this.widgetBG.y - 220;
        this.widgetInfo.x = 512;
        this.widgetInfo.y = this.widgetBG.y - 180;
        this.uniButton.x = 512;
        this.uniButton.y = this.widgetBG.y + 200;
        this.uniButton.idx = TNonStateButton.MAINMENU_BUTTON;
        goto L11119;
      L6485:
        if (arg1 != TWindowState.HO_LEVEL_COMPLETE_EXTRAS_ALL) goto L6892;
        GALib.TrackEvent(GALib.EVENT_COMPLETE_EXTRAS, _global._minigameHO.currentLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
        _global.ReportGameState(_global.GAMESTATE_GAME_END);
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetInfo.visible = true;
        this.widgetMenu.visible = true;
        this.uniButton.visible = true;
        this.summaryStats.visible = true;
        this.summaryText.visible = true;
        this.summaryText.text = "Objects Found\t:\nScore\t\t:";
        this.summaryStats.text = _global._minigameHO._HOClass.itemFoundCount + "\n" + _global._maxCrypto.STI(_global._minigameHO.bonusMoney);
        this.widgetMenu.text = "Congratulations!";
        this.widgetInfo.text = "You've completed all levels.";
        this.uniButton._text.text = "Main Menu";
        this.widgetBG.x = 512;
        this.widgetBG.y = 300;
        this.widgetMenu.x = 512;
        this.widgetMenu.y = this.widgetBG.y - 220;
        this.widgetInfo.x = 512;
        this.widgetInfo.y = this.widgetBG.y - 180;
        this.uniButton.x = 512;
        this.uniButton.y = this.widgetBG.y + 200;
        this.uniButton.idx = TNonStateButton.MAINMENU_BUTTON;
        _global.psCongrats.start();
        TweenLite.delayedCall(6, _global.psCongrats.pause);
        var $t5 = TGlobalClass.EnableLeaderboardBtn;
        _local9 = $t5;
        this.submitScoreBtn.visible = $t5;
        this.leaderboardBtn.visible = _local9;
        goto L11119;
      L6892:
        if (arg1 != TWindowState.DESSERT_LEVEL_COMPLETE) goto L7376;
        GALib.TrackEvent(GALib.EVENT_COMPLETE_STORY, TGlobalClass.currentPlayedLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
        _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetInfo.visible = true;
        this.widgetMenu.visible = true;
        this.uniButton.visible = true;
        this.summaryStats.visible = true;
        this.summaryText.visible = true;
        this.summaryTips.visible = true;
        _local7 = Boolean(this.showGoalIcon());
        _local8 = _global._minigameDessert.earning;
        this.summaryText.text = "Froyo Served\t\t:\nToday's Earning\t:\n\nTotal Coins\t\t:";
        this.summaryStats.text = _global._minigameDessert.froyoMadeCount + "\n$" + _global._maxCrypto.STI(_local8) + "\n\n" + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
        this.widgetMenu.text = "Level Complete!";
        this.widgetInfo.text = "You've completed this level.";
        $s0 = this.summaryTips;
        if (!_local7) goto L7181;
        $s1 = "Awesome! Couldn't have done it better myself!";
        goto L7184;
      L7181:
        $s1 = "Well done! Try to reach Expert target to earn even more coins!";
      L7184:
        $s0.text = $s1;
        this.widgetBG.x = 512;
        this.widgetBG.y = 300;
        this.widgetMenu.x = 512;
        this.widgetMenu.y = this.widgetBG.y - 220;
        this.widgetInfo.x = 512;
        this.widgetInfo.y = this.widgetBG.y - 180;
        this.uniButton.x = 512;
        this.uniButton.y = this.widgetBG.y + 200;
        this.uniButton._text.text = "Kitchen";
        this.uniButton.idx = TNonStateButton.KITCHEN;
        _global.psCongrats.start();
        TweenLite.delayedCall(6, _global.psCongrats.pause);
        var $t6 = TGlobalClass.EnableLeaderboardBtn;
        _local9 = $t6;
        this.submitScoreBtn.visible = $t6;
        this.leaderboardBtn.visible = _local9;
        goto L11119;
      L7376:
        if (arg1 != TWindowState.DESSERT_LEVEL_COMPLETE_EXTRAS) goto L7768;
        GALib.TrackEvent(GALib.EVENT_COMPLETE_EXTRAS, _global._minigameDessert.level, _global.getCurrentSceneNameByID(_global.currentSceneID));
        _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetInfo.visible = true;
        this.widgetMenu.visible = true;
        this.uniButton.visible = true;
        this.summaryStats.visible = true;
        this.summaryText.visible = true;
        _local8 = _global._minigameDessert.earning;
        this.summaryText.text = "Froyo Served\t\t:";
        this.summaryStats.text = _global._minigameDessert.froyoMadeCount + "";
        this.widgetMenu.text = "Congratulations!";
        this.widgetInfo.text = "You've completed this game.";
        this.widgetBG.x = 512;
        this.widgetBG.y = 300;
        this.widgetMenu.x = 512;
        this.widgetMenu.y = this.widgetBG.y - 220;
        this.widgetInfo.x = 512;
        this.widgetInfo.y = this.widgetBG.y - 180;
        this.uniButton.x = 512;
        this.uniButton.y = this.widgetBG.y + 200;
        this.uniButton._text.text = "Main Menu";
        this.uniButton.idx = TNonStateButton.MAINMENU_BUTTON;
        _global.psCongrats.start();
        TweenLite.delayedCall(6, _global.psCongrats.pause);
        var $t7 = TGlobalClass.EnableLeaderboardBtn;
        _local9 = $t7;
        this.submitScoreBtn.visible = $t7;
        this.leaderboardBtn.visible = _local9;
        goto L11119;
      L7768:
        if (arg1 != TWindowState.DESSERT_LEVEL_FAILED) goto L8171;
        GALib.TrackEvent(GALib.EVENT_LOSE_STORY, TGlobalClass.currentPlayedLevel + 1, _global.getCurrentSceneNameByID(_global.currentSceneID));
        _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetInfo.visible = true;
        this.widgetMenu.visible = true;
        this.uniButton.visible = true;
        this.summaryStats.visible = true;
        this.summaryTips.visible = true;
        this.summaryText.visible = true;
        this.summaryText.text = "Froyo Served\t:\nToday's Earning\t:\n\nTotal Coins\t:";
        this.summaryStats.text = _global._minigameDessert.froyoMadeCount + "\n$" + _global._maxCrypto.STI(_global._minigameDessert.earning) + "\n\n" + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + " (+" + TGlobalClass.coinsGot + ")";
        this.widgetMenu.text = "Level Failed!";
        this.widgetInfo.text = "You've failed this level.";
        this.summaryTips.text = "If you reach the normal Daily Goal you will receive 3 coins, but when you reach the Expert Goal you'll earn 6 coins.";
        this.widgetBG.x = 512;
        this.widgetBG.y = 300;
        this.widgetMenu.x = 512;
        this.widgetMenu.y = this.widgetBG.y - 220;
        this.widgetInfo.x = 512;
        this.widgetInfo.y = this.widgetBG.y - 180;
        this.uniButton.x = 512;
        this.uniButton.y = this.widgetBG.y + 200;
        this.uniButton._text.text = "Kitchen";
        this.uniButton.idx = TNonStateButton.KITCHEN;
        goto L11119;
      L8171:
        if (arg1 != TWindowState.DESSERT_LEVEL_FAILED_EXTRAS) goto L8496;
        GALib.TrackEvent(GALib.EVENT_LOSE_EXTRAS, _global._minigameDessert.level, _global.getCurrentSceneNameByID(_global.currentSceneID));
        _global.ReportGameState(_global.GAMESTATE_LEVEL_END);
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetInfo.visible = true;
        this.widgetMenu.visible = true;
        this.uniButton.visible = true;
        this.summaryStats.visible = true;
        this.summaryText.visible = true;
        this.summaryText.text = "Froyo Served\t:";
        this.summaryStats.text = _global._minigameDessert.froyoMadeCount + "";
        this.widgetMenu.text = "You Failed!";
        this.widgetInfo.text = "You've failed this game.";
        this.widgetBG.x = 512;
        this.widgetBG.y = 300;
        this.widgetMenu.x = 512;
        this.widgetMenu.y = this.widgetBG.y - 220;
        this.widgetInfo.x = 512;
        this.widgetInfo.y = this.widgetBG.y - 180;
        this.uniButton.x = 512;
        this.uniButton.y = this.widgetBG.y + 200;
        this.uniButton._text.text = "Main Menu";
        this.uniButton.idx = TNonStateButton.MAINMENU_BUTTON;
        goto L11119;
      L8496:
        if (arg1 != TWindowState.CONFIRM_MAINMENU) goto L8864;
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
        this.smallWidgetBG.visible = true;
        this.smallWidgetMenu.visible = true;
        this.smallWidgetInfo.visible = true;
        this.smallButtonLeft.visible = true;
        this.smallButtonRight.visible = true;
        this.smallButtonLeft._text.text = "No";
        this.smallButtonLeft._image.visible = true;
        this.smallButtonRight._image.visible = true;
        this.smallButtonLeft._image.texture = windowsAtlas.getTexture("small_btn_blue");
        this.smallButtonRight._image.texture = windowsAtlas.getTexture("small_btn_blue");
        this.smallButtonRight._text.text = "Yes";
        this.smallWidgetMenu.text = "Main Menu?";
        this.smallWidgetInfo.text = "Are you sure want to exit to the Main Menu? Your progress in this level will not be saved.";
        this.smallWidgetBG.x = 512;
        this.smallWidgetBG.y = 300;
        this.smallWidgetMenu.x = 525;
        this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
        this.smallWidgetInfo.x = 525;
        this.smallWidgetInfo.y = this.smallWidgetBG.y - 15;
        this.smallButtonLeft.x = this.smallWidgetBG.x - 100;
        this.smallButtonLeft.y = this.smallWidgetBG.y + 130;
        this.smallButtonRight.x = this.smallWidgetBG.x + 100;
        this.smallButtonRight.y = this.smallWidgetBG.y + 130;
        this.smallButtonLeft.idx = TNonStateButton.NO_BUTTON;
        this.smallButtonRight.idx = TNonStateButton.YES_BUTTON;
        goto L11119;
      L8864:
        if (arg1 != TWindowState.CONFIRM_KITCHEN) goto L9232;
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
        this.smallWidgetBG.visible = true;
        this.smallWidgetMenu.visible = true;
        this.smallWidgetInfo.visible = true;
        this.smallButtonLeft.visible = true;
        this.smallButtonRight.visible = true;
        this.smallButtonLeft._text.text = "No";
        this.smallButtonLeft._image.visible = true;
        this.smallButtonRight._image.visible = true;
        this.smallButtonLeft._image.texture = windowsAtlas.getTexture("small_btn_blue");
        this.smallButtonRight._image.texture = windowsAtlas.getTexture("small_btn_blue");
        this.smallButtonRight._text.text = "Yes";
        this.smallWidgetMenu.text = "Go to kitchen?";
        this.smallWidgetInfo.text = "Are you sure want to exit to the Kitchen? Your progress in this level will not be saved.";
        this.smallWidgetBG.x = 512;
        this.smallWidgetBG.y = 300;
        this.smallWidgetMenu.x = 525;
        this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
        this.smallWidgetInfo.x = 525;
        this.smallWidgetInfo.y = this.smallWidgetBG.y - 15;
        this.smallButtonLeft.x = this.smallWidgetBG.x - 100;
        this.smallButtonLeft.y = this.smallWidgetBG.y + 130;
        this.smallButtonRight.x = this.smallWidgetBG.x + 100;
        this.smallButtonRight.y = this.smallWidgetBG.y + 130;
        this.smallButtonLeft.idx = TNonStateButton.NO_BUTTON;
        this.smallButtonRight.idx = TNonStateButton.YES_BUTTON;
        goto L11119;
      L9232:
        if (arg1 != TWindowState.EXTRAS_MENU) goto L9627;
        _global.ReportGameState(_global.GAMESTATE_LEVEL_SELECT);
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.buttonRight.visible = true;
        this.buttonLeft.visible = true;
        this.widgetMenu.visible = true;
        this.uniButton.visible = true;
        this.buttonLeft.enabled = true;
        this.buttonLeft.scaleX = 1.2;
        this.buttonLeft.scaleY = 1.2;
        this.buttonRight.scaleX = 1.2;
        this.buttonRight.scaleY = 1.2;
        this.uniButton.scaleX = 1.2;
        this.uniButton.scaleY = 1.2;
        this.widgetMenu.text = "Select Game";
        this.buttonLeft._text.text = "Hidden Object";
        this.buttonRight._text.text = "Dessert Making";
        this.uniButton._text.text = "Cancel";
        this.widgetBG.x = 512;
        this.widgetBG.y = 300;
        this.widgetMenu.x = 525;
        this.widgetMenu.y = this.widgetBG.y - 200;
        this.buttonLeft.x = this.widgetBG.x;
        this.buttonLeft.y = this.widgetBG.y - 50;
        this.buttonRight.x = this.widgetBG.x;
        this.buttonRight.y = this.widgetBG.y + 40;
        this.uniButton.x = this.widgetBG.x;
        this.uniButton.y = this.widgetBG.y + 130;
        this.buttonLeft.idx = TNonStateButton.HO_BUTTON;
        this.buttonRight.idx = TNonStateButton.DM_BUTTON;
        this.uniButton.idx = TNonStateButton.CANCEL_BUTTON;
        goto L11119;
      L9627:
        if (arg1 != TWindowState.SHOW_EXPERT_NEEDED_INFO) goto L9897;
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
        this.smallWidgetBG.visible = true;
        this.smallUniButton.visible = true;
        this.smallWidgetMenu.visible = true;
        this.smallWidgetInfo.visible = true;
        this.smallWidgetMenu.text = "Locked!";
        this.smallWidgetInfo.text = "To unlock this recipe, you need to collect " + _global._kitchen.buyButton.idx + " expert badges.\nCurrent Expert Badges: " + TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF];
        this.smallUniButton._text.text = "OK";
        this.smallWidgetBG.x = 512;
        this.smallWidgetBG.y = 300;
        this.smallWidgetMenu.x = 525;
        this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
        this.smallWidgetInfo.x = 525;
        this.smallWidgetInfo.y = this.smallWidgetBG.y + 10;
        this.smallUniButton.x = this.smallWidgetBG.x;
        this.smallUniButton.y = this.smallWidgetBG.y + 130;
        this.smallUniButton.idx = TNonStateButton.OK_BUTTON;
        goto L11119;
      L9897:
        if (arg1 != TWindowState.UNLOCK_FULL_VERSION) goto L10262;
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
        this.smallWidgetBG.visible = true;
        this.smallWidgetMenu.visible = true;
        this.smallWidgetInfo.visible = true;
        this.smallButtonLeft.visible = true;
        this.smallButtonRight.visible = true;
        this.smallButtonLeft._text.text = "Cancel";
        this.smallButtonLeft._image.visible = true;
        this.smallButtonRight._image.visible = true;
        this.smallButtonLeft._image.texture = windowsAtlas.getTexture("small_btn_blue");
        this.smallButtonRight._image.texture = windowsAtlas.getTexture("small_btn_blue");
        this.smallButtonRight._text.text = "Unlock";
        this.smallWidgetMenu.text = "Unlock Full Version";
        this.smallWidgetInfo.text = "Get Premium access to all Extra minigames, remove Ads and support development for this game!";
        this.smallWidgetBG.x = 512;
        this.smallWidgetBG.y = 300;
        this.smallWidgetMenu.x = 525;
        this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
        this.smallWidgetInfo.x = 525;
        this.smallWidgetInfo.y = this.smallWidgetBG.y;
        this.smallButtonLeft.x = this.smallWidgetBG.x - 100;
        this.smallButtonLeft.y = this.smallWidgetBG.y + 130;
        this.smallButtonRight.x = this.smallWidgetBG.x + 100;
        this.smallButtonRight.y = this.smallWidgetBG.y + 130;
        this.smallButtonLeft.idx = TNonStateButton.CANCEL_BUTTON;
        this.smallButtonRight.idx = TNonStateButton.UNLOCK_BUTTON;
        goto L11119;
      L10262:
        if (arg1 != TWindowState.SHOW_FREE_COINS) goto L10655;
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetMenu.visible = true;
        this.widgetInfo.visible = true;
        this.buttonLeft.visible = true;
        this.buttonRight.visible = true;
        this.buttonLeft._text.text = "OK!";
        this.buttonLeft._image.visible = true;
        this.buttonRight._image.visible = true;
        this.buttonLeft._image.texture = windowsAtlas.getTexture("small_btn_blue");
        this.buttonRight._image.texture = windowsAtlas.getTexture("small_btn_blue");
        this.buttonRight._text.text = "Cancel";
        this.widgetMenu.text = "Free Coins";
        this.widgetInfo.text = "Your Coins: " + _global._maxCrypto.STI(TSavegame.currentPlayer.money) + "\n\n\nIt's raining coins! Watch an interesting Ad and you'll get rewarded with 3 coins afterwards!\n(Please note that you must watch the whole video ad and there may not be any Ad available at all times)";
        this.widgetBG.x = 512;
        this.widgetBG.y = 300;
        this.widgetMenu.x = 525;
        this.widgetMenu.y = this.widgetBG.y - 200;
        this.widgetInfo.x = 525;
        this.widgetInfo.y = this.widgetBG.y - 20;
        this.buttonLeft.x = this.widgetBG.x - 100;
        this.buttonLeft.y = this.widgetBG.y + 160;
        this.buttonRight.x = this.widgetBG.x + 100;
        this.buttonRight.y = this.widgetBG.y + 160;
        this.buttonLeft.idx = TNonStateButton.OK_BUTTON;
        this.buttonRight.idx = TNonStateButton.CANCEL_BUTTON;
        goto L11119;
      L10655:
        if (arg1 != TWindowState.VIDEO_ADS_NOT_AVAILABLE) goto L10889;
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
        this.smallWidgetBG.visible = true;
        this.smallUniButton.visible = true;
        this.smallWidgetMenu.visible = true;
        this.smallWidgetInfo.visible = true;
        this.smallWidgetMenu.text = "Free Coins";
        this.smallWidgetInfo.text = "Sorry, no video ad is available at this moment. Please try again in a moment.";
        this.smallUniButton._text.text = "OK";
        this.smallWidgetBG.x = 512;
        this.smallWidgetBG.y = 300;
        this.smallWidgetMenu.x = 525;
        this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
        this.smallWidgetInfo.x = 525;
        this.smallWidgetInfo.y = this.smallWidgetBG.y + 10;
        this.smallUniButton.x = this.smallWidgetBG.x;
        this.smallUniButton.y = this.smallWidgetBG.y + 130;
        this.smallUniButton.idx = TNonStateButton.OK_BUTTON;
        goto L11119;
      L10889:
        if (arg1 != TWindowState.COINS_REWARDED) goto L11119;
        _blackScreen.visible = true;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
        this.smallWidgetBG.visible = true;
        this.smallUniButton.visible = true;
        this.smallWidgetMenu.visible = true;
        this.smallWidgetInfo.visible = true;
        this.smallWidgetMenu.text = "Free Coins";
        this.smallWidgetInfo.text = "We have awarded you 3 coins.";
        this.smallUniButton._text.text = "OK";
        this.smallWidgetBG.x = 512;
        this.smallWidgetBG.y = 300;
        this.smallWidgetMenu.x = 525;
        this.smallWidgetMenu.y = this.smallWidgetBG.y - 110;
        this.smallWidgetInfo.x = 525;
        this.smallWidgetInfo.y = this.smallWidgetBG.y + 10;
        this.smallUniButton.x = this.smallWidgetBG.x;
        this.smallUniButton.y = this.smallWidgetBG.y + 130;
        this.smallUniButton.idx = TNonStateButton.OK_BUTTON;
      L11119:
    }

    function showGoalIconSmall():void {
        expertTarget = 0;
        this.goalIcon.visible = true;
        if (_global.currentSceneID != _global.GAMESCREEN_ID) goto L54;
        expertTarget = int(_global.gameScreen.expertTarget);
        goto L102;
      L54:
        if (_global.currentSceneID != _global.MAKE_DESSERT_ID) goto L88;
        expertTarget = int(_global._minigameDessert.expertTarget);
        goto L102;
      L88:
        expertTarget = int(_global._minigameHO.expertTarget);
      L102:
        if (_global.currentSceneID != _global.GAMESCREEN_ID) goto L204;
        if (!(_global._maxCrypto.STI(ScoreManager.totalScore) >= expertTarget)) goto L177;
        this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
        this.goalIcon.readjustSize();
        goto L200;
      L177:
        this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal");
      L200:
        goto L406;
      L204:
        if (_global.currentSceneID != _global.MAKE_DESSERT_ID) goto L309;
        if (!(_global._maxCrypto.STI(_global._minigameDessert.earning) >= expertTarget)) goto L282;
        this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
        this.goalIcon.readjustSize();
        goto L305;
      L282:
        this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal");
      L305:
        goto L406;
      L309:
        if (_global.currentSceneID != _global.HIDDEN_OBJECT_ID) goto L406;
        if (!(_global._minigameHO._HOClass.itemFoundCount + 1 >= expertTarget)) goto L383;
        this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
        this.goalIcon.readjustSize();
        goto L406;
      L383:
        this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal");
      L406:
        var _local2 = 0.6;
        this.goalIcon.scaleY = 0.6;
        this.goalIcon.scaleX = _local2;
        this.goalIcon.alpha = 0.5;
        this.goalIcon.x = 363;
        this.goalIcon.y = 25;
        if (_global.currentSceneID != _global.HIDDEN_OBJECT_ID) goto L493;
        this.goalIcon.x = 90;
        this.goalIcon.y = 95;
        goto L528;
      L493:
        if (_global.currentSceneID != _global.MAKE_DESSERT_ID) goto L528;
        this.goalIcon.x = 810;
        this.goalIcon.y = 25;
      L528:
        TweenLite.to(this.goalIcon, 0.8, {"alpha": 1, "scaleX": 0.35, "scaleY": 0.35, "ease": Back.easeIn, "onComplete": function():void {
            TweenLite.delayedCall(1, function():void {
                if (!(goalIcon.scaleX < 1)) goto L19;
                goalIcon.visible = false;
              L19:
            });
        }});
    }

    function showGoalIcon():Boolean {
        expert = false;
        expertTarget = 0;
        this.goalIcon.visible = true;
        if (_global.currentSceneID != _global.GAMESCREEN_ID) goto L59;
        expertTarget = int(_global.gameScreen.expertTarget);
        goto L107;
      L59:
        if (_global.currentSceneID != _global.MAKE_DESSERT_ID) goto L93;
        expertTarget = int(_global._minigameDessert.expertTarget);
        goto L107;
      L93:
        expertTarget = int(_global._minigameHO.expertTarget);
      L107:
        var $t0 = _global.currentSceneID == _global.GAMESCREEN_ID;
        $s0 = $t0;
        if (!$t0) goto L147;
        $s0 = _global._maxCrypto.STI(ScoreManager.totalScore) >= expertTarget;
      L147:
        if (!$s0) goto L204;
        expert = true;
        _global._soundAsset.playSFX("expert_reached");
        this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
        this.goalIcon.readjustSize();
        goto L431;
      L204:
        var $t1 = _global.currentSceneID == _global.MAKE_DESSERT_ID;
        $s0 = $t1;
        if (!$t1) goto L247;
        $s0 = _global._maxCrypto.STI(_global._minigameDessert.earning) >= expertTarget;
      L247:
        if (!$s0) goto L304;
        expert = true;
        _global._soundAsset.playSFX("expert_reached");
        this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
        this.goalIcon.readjustSize();
        goto L431;
      L304:
        var $t2 = _global.currentSceneID == _global.HIDDEN_OBJECT_ID;
        $s0 = $t2;
        if (!$t2) goto L338;
        $s0 = _global._minigameHO._HOClass.totalItems == 0;
      L338:
        if (!$s0) goto L395;
        expert = true;
        _global._soundAsset.playSFX("expert_reached");
        this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
        this.goalIcon.readjustSize();
        goto L431;
      L395:
        _global._soundAsset.playSFX("goal_reached");
        this.goalIcon.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal");
      L431:
        var _local2 = 2;
        this.goalIcon.scaleY = 2;
        this.goalIcon.scaleX = _local2;
        this.goalIcon.alpha = 0.5;
        this.goalIcon.x = 780;
        this.goalIcon.y = 500;
        this.expertBlink.x = this.goalIcon.x;
        this.expertBlink.y = this.goalIcon.y + 10;
        TweenLite.to(this.goalIcon, 0.8, {"alpha": 1, "scaleX": 1, "scaleY": 1, "ease": Back.easeIn, "onComplete": function(f1_arg1:Boolean):void {
            var $t0 = Boolean(f1_arg1);
            $s0 = $t0;
            if (!$t0) goto L12;
            $s0 = Boolean(expertBlink);
          L12:
            if (!$s0) goto L30;
            expertBlink.visible = true;
            tweenExpertBlink();
          L30:
        }, "onCompleteParams": [expert]});
        return expert;
    }

    function tweenExpertBlink():void {
        TweenLite.to(this.expertBlink, 0.4, {"alpha": 0, "onComplete": function():void {
            TweenLite.to(expertBlink, 0.4, {"alpha": 1, "onComplete": tweenExpertBlink});
        }});
    }

    function showKitchenWindow(arg1:int, arg2:int):void {
        var _local3 = null;
        _local3 = TGlobalClass.levelEventXML.day[arg1].kitchen[arg2];
        _blackScreen.visible = true;
        if (_local3.attribute("size") != "big") goto L752;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetInfo.visible = _local3.info.attribute("visible").toString() == "true";
        this.widgetMenu.visible = _local3.title.attribute("visible").toString() == "true";
        this.buttonLeft.visible = _local3.leftButton.attribute("visible").toString() == "true";
        this.buttonRight.visible = _local3.rightButton.attribute("visible").toString() == "true";
        this.uniButton.visible = _local3.uniButton.attribute("visible").toString() == "true";
        if (_local3.texture.toString() == "") goto L313;
        this.unlockedItem.visible = true;
        this.unlockedItem.texture = _global.gameScreen.gameAtlas.getTexture(_local3.texture.toString());
        this.unlockedItem.readjustSize();
        this.unlockedItem.pivotX = this.unlockedItem.width / 2;
        this.unlockedItem.pivotY = this.unlockedItem.height / 2;
        goto L321;
      L313:
        this.unlockedItem.visible = false;
      L321:
        this.widgetInfo.text = _local3.info.toString();
        this.widgetMenu.text = _local3.title.toString();
        this.buttonLeft._text.text = _local3.leftButton.toString();
        this.buttonRight._text.text = _local3.rightButton.toString();
        this.uniButton._text.text = _local3.uniButton.toString();
        this.widgetInfo.x = this.widgetBG.x + int(_local3.info.attribute("offsetX"));
        this.widgetMenu.x = this.widgetBG.x + int(_local3.title.attribute("offsetX"));
        this.buttonLeft.x = this.widgetBG.x + int(_local3.leftButton.attribute("offsetX"));
        this.buttonRight.x = this.widgetBG.x + int(_local3.rightButton.attribute("offsetX"));
        this.uniButton.x = this.widgetBG.x + int(_local3.uniButton.attribute("offsetX"));
        this.widgetInfo.y = this.widgetBG.y + int(_local3.info.attribute("offsetY"));
        this.widgetMenu.y = this.widgetBG.y + int(_local3.title.attribute("offsetY"));
        this.buttonLeft.y = this.widgetBG.y + int(_local3.leftButton.attribute("offsetY"));
        this.buttonRight.y = this.widgetBG.y + int(_local3.rightButton.attribute("offsetY"));
        this.uniButton.y = this.widgetBG.y + int(_local3.uniButton.attribute("offsetY"));
        this.uniButton.idx = TNonStateButton.NEXT_WINDOW;
        goto L1464;
      L752:
        if (_local3.attribute("size") != "small") goto L1464;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
        this.smallWidgetBG.visible = true;
        this.smallWidgetInfo.visible = _local3.info.attribute("visible").toString() == "true";
        this.smallWidgetMenu.visible = _local3.title.attribute("visible").toString() == "true";
        this.smallButtonLeft.visible = _local3.leftButton.attribute("visible").toString() == "true";
        this.smallButtonRight.visible = _local3.rightButton.attribute("visible").toString() == "true";
        this.smallUniButton.visible = _local3.uniButton.attribute("visible").toString() == "true";
        if (_local3.texture.toString() == "") goto L1029;
        this.unlockedItem.visible = true;
        this.unlockedItem.texture = _global.gameScreen.gameAtlas.getTexture(_local3.texture.toString());
        this.unlockedItem.readjustSize();
        this.unlockedItem.pivotX = this.unlockedItem.width / 2;
        this.unlockedItem.pivotY = this.unlockedItem.height / 2;
        goto L1037;
      L1029:
        this.unlockedItem.visible = false;
      L1037:
        this.smallWidgetInfo.text = _local3.info.toString();
        this.smallWidgetMenu.text = _local3.title.toString();
        this.smallButtonLeft._text.text = _local3.leftButton.toString();
        this.smallButtonRight._text.text = _local3.rightButton.toString();
        this.smallUniButton._text.text = _local3.uniButton.toString();
        this.smallWidgetInfo.x = this.smallWidgetBG.x + int(_local3.info.attribute("offsetX"));
        this.smallWidgetMenu.x = this.smallWidgetBG.x + int(_local3.title.attribute("offsetX"));
        this.smallButtonLeft.x = this.smallWidgetBG.x + int(_local3.leftButton.attribute("offsetX"));
        this.smallButtonRight.x = this.smallWidgetBG.x + int(_local3.rightButton.attribute("offsetX"));
        this.smallUniButton.x = this.smallWidgetBG.x + int(_local3.uniButton.attribute("offsetX"));
        this.smallWidgetInfo.y = this.smallWidgetBG.y + int(_local3.info.attribute("offsetY"));
        this.smallWidgetMenu.y = this.smallWidgetBG.y + int(_local3.title.attribute("offsetY"));
        this.smallButtonLeft.y = this.smallWidgetBG.y + int(_local3.leftButton.attribute("offsetY"));
        this.smallButtonRight.y = this.smallWidgetBG.y + int(_local3.rightButton.attribute("offsetY"));
        this.smallUniButton.y = this.smallWidgetBG.y + int(_local3.uniButton.attribute("offsetY"));
        this.smallUniButton.idx = TNonStateButton.NEXT_WINDOW;
      L1464:
    }

    function showBeginningWindow(arg1:int, arg2:int):void {
        var _local3 = null;
        var _local4 = null;
        var _local5 = null;
        var _local6 = 0;
        var _local7 = 0;
        var _local8 = 0;
        var _local9 = null;
        var _local10 = 0;
        _local3 = TGlobalClass.levelEventXML.day[arg1].window[arg2];
        _blackScreen.visible = true;
        if (_local3.attribute("size") != "big") goto L1746;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.widgetBG) - 1);
        this.widgetBG.visible = true;
        this.widgetBG.scaleY = 0.7;
        this.widgetInfo.visible = _local3.info.attribute("visible").toString() == "true";
        this.widgetMenu.visible = _local3.title.attribute("visible").toString() == "true";
        this.buttonLeft.visible = _local3.leftButton.attribute("visible").toString() == "true";
        this.buttonRight.visible = _local3.rightButton.attribute("visible").toString() == "true";
        this.uniButton.visible = _local3.uniButton.attribute("visible").toString() == "true";
        if (_local3.texture.toString() == "") goto L518;
        this.unlockedItem.visible = true;
        this.unlockedItem.texture = _global.gameScreen.gameAtlas.getTexture(_local3.texture.toString());
        this.unlockedItem.readjustSize();
        this.unlockedItem.pivotX = this.unlockedItem.width / 2;
        this.unlockedItem.pivotY = this.unlockedItem.height / 2;
        this.unlockedItem.scaleX = 1.2;
        this.unlockedItem.scaleY = 1.2;
        var $t0 = _local3.texture.toString().indexOf("char_") > -1;
        $s0 = $t0;
        if (!$t0) goto L399;
        $s0 = TGlobalClass.currentPlayedLevel > 0;
      L399:
        if (!$s0) goto L514;
        _local5 = _local3.texture.toString().split("_");
        var $t1 = int(_local5[_local5.length - 1]);
        _local6 = $t1;
        if (!($t1 < 13)) goto L514;
        TSavegame.currentPlayer.lastCustIdx = _global.gameScreen.customerInUse.indexOf(_local6);
        TSavegame.save();
        TGlobalClass.tempCustUse = _global.gameScreen.customerInUse.slice(0, TSavegame.currentPlayer.lastCustIdx + 1);
      L514:
        goto L526;
      L518:
        this.unlockedItem.visible = false;
      L526:
        this.widgetInfo.text = _local3.info.toString();
        this.widgetMenu.text = _local3.title.toString();
        this.buttonLeft._text.text = _local3.leftButton.toString();
        this.buttonRight._text.text = _local3.rightButton.toString();
        this.uniButton._text.text = _local3.uniButton.toString();
        this.widgetInfo.x = this.widgetBG.x + int(_local3.info.attribute("offsetX"));
        this.widgetMenu.x = this.widgetBG.x + int(_local3.title.attribute("offsetX"));
        this.buttonLeft.x = this.widgetBG.x + int(_local3.leftButton.attribute("offsetX"));
        this.buttonRight.x = this.widgetBG.x + int(_local3.rightButton.attribute("offsetX"));
        this.uniButton.x = this.widgetBG.x + int(_local3.uniButton.attribute("offsetX"));
        this.widgetInfo.y = this.widgetBG.y + int(_local3.info.attribute("offsetY"));
        this.widgetMenu.y = this.widgetBG.y + int(_local3.title.attribute("offsetY"));
        this.buttonLeft.y = this.widgetBG.y + int(_local3.leftButton.attribute("offsetY"));
        this.buttonRight.y = this.widgetBG.y + int(_local3.rightButton.attribute("offsetY"));
        this.uniButton.y = this.widgetBG.y + int(_local3.uniButton.attribute("offsetY"));
        _local4 = " customer(s)";
        if (!TGlobalClass.isPlayingSpecialLevel) goto L960;
        _local4 = " food critic";
      L960:
        if (this.widgetMenu.text != "General Tips") goto L1729;
        this.widgetMenu.text = "Today's Preparation";
        this.widgetInfo.text = "Goal : serve " + _global.gameScreen.goalTarget + _local4 + "\nExpert : earn $" + _global.gameScreen.expertTarget;
        _local9 = new Vector.<int>();
        if (TGlobalClass.currentRegionDish != ConstClass.WESTERN_DISH) goto L1067;
        _local7 = int(TGlobalClass.dishRegionWestern.length);
        goto L1127;
      L1067:
        if (TGlobalClass.currentRegionDish != ConstClass.CHINESE_DISH) goto L1099;
        _local7 = int(TGlobalClass.dishRegionChinese.length);
        goto L1127;
      L1099:
        if (TGlobalClass.currentRegionDish != ConstClass.MID_EAST_DISH) goto L1127;
        _local7 = int(TGlobalClass.dishRegionMidEast.length);
      L1127:
        _local8 = 0;
        goto L1584;
      L1135:
        if (TGlobalClass.currentRegionDish != ConstClass.WESTERN_DISH) goto L1248;
        if (!(_local9.indexOf(TGlobalClass.dishRegionWestern[_local8]) > -1)) goto L1179;
        goto L1582;
      L1179:
        this.dishPreviewAr[_local10].texture = _global.gameScreen.gameAtlas.getTexture(_global.gameScreen.westernDishName[TGlobalClass.dishRegionWestern[_local8]]);
        _local9.push(TGlobalClass.dishRegionWestern[_local8]);
        goto L1489;
      L1248:
        if (TGlobalClass.currentRegionDish != ConstClass.CHINESE_DISH) goto L1367;
        if (!(_local9.indexOf(TGlobalClass.dishRegionChinese[_local8]) > -1)) goto L1291;
        goto L1582;
      L1291:
        this.dishPreviewAr[_local10].texture = _global.gameScreen.gameAtlas.getTexture(_global.gameScreen.chineseDishName[TGlobalClass.dishRegionChinese[_local8] - TGlobalClass.TOTAL_WESTERN_DISH]);
        _local9.push(TGlobalClass.dishRegionChinese[_local8]);
        goto L1489;
      L1367:
        if (TGlobalClass.currentRegionDish != ConstClass.MID_EAST_DISH) goto L1489;
        if (!(_local9.indexOf(TGlobalClass.dishRegionMidEast[_local8]) > -1)) goto L1410;
        goto L1582;
      L1410:
        this.dishPreviewAr[_local10].texture = _global.gameScreen.gameAtlas.getTexture(_global.gameScreen.mideastDishName[TGlobalClass.dishRegionMidEast[_local8] - TGlobalClass.TOTAL_WESTERN_DISH - TGlobalClass.TOTAL_CHINESE_DISH]);
        _local9.push(TGlobalClass.dishRegionMidEast[_local8]);
      L1489:
        this.dishPreviewAr[_local10].readjustSize();
        this.dishPreviewAr[_local10].scaleX = 1.2;
        this.dishPreviewAr[_local10].scaleY = 1.2;
        this.dishPreviewAr[_local10].pivotY = this.dishPreviewAr[_local10].height / this.dishPreviewAr[_local10].scaleY;
        this.dishPreviewAr[_local10].visible = true;
        _local10++;
      L1582:
        _local8++;
      L1584:
        if (_local8 < _local7) goto L1135;
        _local8 = 0;
        goto L1721;
      L1600:
        if (_local10 % 2 != 0) goto L1656;
        this.dishPreviewAr[_local8].x = 512 + (_local8 - _local10 / 2) * (this.dishPreviewAr[_local8].width + 10);
        goto L1719;
      L1656:
        this.dishPreviewAr[_local8].x = 512 + (_local8 - Math.floor(_local10 / 2)) * (this.dishPreviewAr[_local8].width + 10) - this.dishPreviewAr[_local8].width / 2;
      L1719:
        _local8++;
      L1721:
        if (_local8 < _local10) goto L1600;
      L1729:
        this.uniButton.idx = TNonStateButton.NEXT_WINDOW;
        goto L2458;
      L1746:
        if (_local3.attribute("size") != "small") goto L2458;
        _global._gameS.setChildIndex(_blackScreen, _global._gameS.getChildIndex(this.smallWidgetBG) - 1);
        this.smallWidgetBG.visible = true;
        this.smallWidgetInfo.visible = _local3.info.attribute("visible").toString() == "true";
        this.smallWidgetMenu.visible = _local3.title.attribute("visible").toString() == "true";
        this.smallButtonLeft.visible = _local3.leftButton.attribute("visible").toString() == "true";
        this.smallButtonRight.visible = _local3.rightButton.attribute("visible").toString() == "true";
        this.smallUniButton.visible = _local3.uniButton.attribute("visible").toString() == "true";
        if (_local3.texture.toString() == "") goto L2023;
        this.unlockedItem.visible = true;
        this.unlockedItem.texture = _global.gameScreen.gameAtlas.getTexture(_local3.texture.toString());
        this.unlockedItem.readjustSize();
        this.unlockedItem.pivotX = this.unlockedItem.width / 2;
        this.unlockedItem.pivotY = this.unlockedItem.height / 2;
        goto L2031;
      L2023:
        this.unlockedItem.visible = false;
      L2031:
        this.smallWidgetInfo.text = _local3.info.toString();
        this.smallWidgetMenu.text = _local3.title.toString();
        this.smallButtonLeft._text.text = _local3.leftButton.toString();
        this.smallButtonRight._text.text = _local3.rightButton.toString();
        this.smallUniButton._text.text = _local3.uniButton.toString();
        this.smallWidgetInfo.x = this.smallWidgetBG.x + int(_local3.info.attribute("offsetX"));
        this.smallWidgetMenu.x = this.smallWidgetBG.x + int(_local3.title.attribute("offsetX"));
        this.smallButtonLeft.x = this.smallWidgetBG.x + int(_local3.leftButton.attribute("offsetX"));
        this.smallButtonRight.x = this.smallWidgetBG.x + int(_local3.rightButton.attribute("offsetX"));
        this.smallUniButton.x = this.smallWidgetBG.x + int(_local3.uniButton.attribute("offsetX"));
        this.smallWidgetInfo.y = this.smallWidgetBG.y + int(_local3.info.attribute("offsetY"));
        this.smallWidgetMenu.y = this.smallWidgetBG.y + int(_local3.title.attribute("offsetY"));
        this.smallButtonLeft.y = this.smallWidgetBG.y + int(_local3.leftButton.attribute("offsetY"));
        this.smallButtonRight.y = this.smallWidgetBG.y + int(_local3.rightButton.attribute("offsetY"));
        this.smallUniButton.y = this.smallWidgetBG.y + int(_local3.uniButton.attribute("offsetY"));
        this.smallUniButton.idx = TNonStateButton.NEXT_WINDOW;
      L2458:
    }
}
}
