package {
class LevelSelectScreen extends Screen {
    const PLAY_BUTTON:uint = 1;
    const BACK_BUTTON:uint = 2;
    const RIGHT_BUTTON:uint = 3;
    const LEFT_BUTTON:uint = 4;
    var _global:Global;
    var initVector:Boolean;
    var levelSelectAtlas:TextureAtlas;
    var mondayQuad:ExtendTextField;
    var tuesdayQuad:ExtendTextField;
    var wednesdayQuad:ExtendTextField;
    var thursdayQuad:ExtendTextField;
    var fridayQuad:ExtendTextField;
    var saturdayQuad:ExtendTextField;
    var sundayQuad:ExtendTextField;
    var currentPage:int;
    var selectedDays:int;
    var buttonAr:Vector.<DisplayObject>;
    var daysAr:Vector.<ExtendTextField>;
    var _background:cSprite;
    var playButton:TNonStateButton;
    var backButton:TNonStateButton;
    var leftButton:TNonStateButton;
    var rightButton:TNonStateButton;
    var noteTitle:ExtendTextField;
    var noteDescription:ExtendTextField;
    var noteDescription2:ExtendTextField;
    var checkMark1:cSprite;
    var checkMark2:cSprite;
    var badgeIcon:Vector.<cSprite>;
    var badgeIconBig:cSprite;
    var badgePosX:Vector.<int>;
    var badgePosY:Vector.<int>;

    function LevelSelectScreen():* {
        var $t0 = new Vector.<int>(7);
        $t0[0] = 150;
        $t0[1] = 386;
        $t0[2] = 573;
        $t0[3] = 188;
        $t0[4] = 445;
        $t0[5] = 651;
        $t0[6] = 251;
        this.badgePosX = $t0;
        var $t1 = new Vector.<int>(7);
        $t1[0] = 174;
        $t1[1] = 121;
        $t1[2] = 150;
        $t1[3] = 332;
        $t1[4] = 290;
        $t1[5] = 307;
        $t1[6] = 529;
        this.badgePosY = $t1;
        super();
    }

    override function Start():void {
        this._global.currentSceneID = this._global.LEVEL_SELECT_ID;
        if (this.initVector) goto L75;
        this.buttonAr = new Vector.<DisplayObject>();
        this.daysAr = new Vector.<ExtendTextField>();
        this.badgeIcon = new Vector.<cSprite>();
        this.initVector = true;
      L75:
        this._global.assetManager.enqueue(this._global.graphicsPath + "level_select.ean", this._global.graphicsPath + "level_select.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + "level_select_BG.eag");
        this._global.assetManager.loadQueue(function(f1_arg1:Number):void {
            if (f1_arg1 != 1) goto L14;
            initialize();
          L14:
        });
    }

    override function Update():void {
        if (ready) goto L10;
        return;
      L10:
    }

    override function Kill():void {
        var _local1 = 0;
        var _local2 = uint(0);
        _local2 = uint(this.buttonAr.length);
        _local1 = 0;
        goto L72;
      L25:
        this.buttonAr[_local1].removeEventListener(TouchEvent.TOUCH, this.touchButton);
        this.buttonAr[_local1].removeFromParent(true);
        this.buttonAr[_local1] = null;
        _local1++;
      L72:
        if (_local1 < _local2) goto L25;
        this.buttonAr.length = 0;
        _local1 = 0;
        goto L141;
      L94:
        this.daysAr[_local1].removeEventListener(TouchEvent.TOUCH, this.selectDay);
        this.daysAr[_local1].removeFromParent(true);
        this.daysAr[_local1] = null;
        _local1++;
      L141:
        if (_local1 < this.daysAr.length) goto L94;
        this.daysAr.length = 0;
        _local1 = 0;
        goto L194;
      L169:
        this.badgeIcon[_local1].removeFromParent(true);
        this.badgeIcon[_local1] = null;
        _local1++;
      L194:
        if (_local1 < this.badgeIcon.length) goto L169;
        this.badgeIcon.length = 0;
        this._background.removeFromParent(true);
        this._background = null;
        this.playButton.removeFromParent(true);
        this.playButton = null;
        this.backButton.removeFromParent(true);
        this.backButton = null;
        this.rightButton.removeFromParent(true);
        this.rightButton = null;
        this.leftButton.removeFromParent(true);
        this.leftButton = null;
        this.noteDescription.removeFromParent(true);
        this.noteDescription = null;
        this.noteDescription2.removeFromParent(true);
        this.noteDescription2 = null;
        this.mondayQuad.removeFromParent(true);
        this.mondayQuad = null;
        this.tuesdayQuad.removeFromParent(true);
        this.tuesdayQuad = null;
        this.wednesdayQuad.removeFromParent(true);
        this.wednesdayQuad = null;
        this.thursdayQuad.removeFromParent(true);
        this.thursdayQuad = null;
        this.fridayQuad.removeFromParent(true);
        this.fridayQuad = null;
        this.saturdayQuad.removeFromParent(true);
        this.saturdayQuad = null;
        this.sundayQuad.removeFromParent(true);
        this.sundayQuad = null;
        this.checkMark1.removeFromParent(true);
        this.checkMark1 = null;
        this.checkMark2.removeFromParent(true);
        this.checkMark2 = null;
        this.badgeIconBig.removeFromParent(true);
        this.badgeIconBig = null;
        this.levelSelectAtlas.dispose();
        this.levelSelectAtlas = null;
        this._global.assetManager.purge();
        ready = false;
    }

    function initialize():void {
        var _local2 = 0;
        var _local3 = null;
        this._global.ReportGameState(this._global.GAMESTATE_LEVEL_SELECT);
        System.pauseForGCIfCollectionImminent(0);
        System.gc();
        TGlobalClass.isPlayingSpecialLevel = false;
        this.levelSelectAtlas = this._global.assetManager.getTextureAtlas("level_select");
        this._background = new cSprite(this._global.assetManager.getTexture("level_select_BG"));
        this._background.blendMode = BlendMode.NONE;
        this._background.touchable = false;
        this._global._gameS.addChild(this._background);
        var _local1 = Number(Math.PI / 180);
        this.mondayQuad = new ExtendTextField(200, 150, "Day\n1", "bromine", 40, 0, true);
        this.mondayQuad.x = 80;
        this.mondayQuad.y = 70;
        this.mondayQuad.rotation = -20 * _local1;
        this._global._gameS.addChild(this.mondayQuad);
        this.daysAr.push(this.mondayQuad);
        this.tuesdayQuad = new ExtendTextField(170, 110, "Day\n2", "bromine", 40, 0, true);
        this.tuesdayQuad.x = 340;
        this.tuesdayQuad.y = 40;
        this.tuesdayQuad.rotation = -6 * _local1;
        this.tuesdayQuad.skewY = 0.35;
        this.tuesdayQuad.skewX = -0.2;
        this._global._gameS.addChild(this.tuesdayQuad);
        this.daysAr.push(this.tuesdayQuad);
        this.wednesdayQuad = new ExtendTextField(150, 100, "Day\n3", "bromine", 35, 0, true);
        this.wednesdayQuad.x = 540;
        this.wednesdayQuad.y = 97;
        this.wednesdayQuad.rotation = -10 * _local1;
        this.wednesdayQuad.skewX = -0.17;
        this._global._gameS.addChild(this.wednesdayQuad);
        this.daysAr.push(this.wednesdayQuad);
        this.thursdayQuad = new ExtendTextField(200, 150, "Day\n4", "bromine", 40, 0, true);
        this.thursdayQuad.x = 130;
        this.thursdayQuad.y = 240;
        this.thursdayQuad.rotation = -20 * _local1;
        this._global._gameS.addChild(this.thursdayQuad);
        this.daysAr.push(this.thursdayQuad);
        this.fridayQuad = new ExtendTextField(180, 110, "Day\n5", "bromine", 40, 0, true);
        this.fridayQuad.x = 390;
        this.fridayQuad.y = 200;
        this.fridayQuad.rotation = -6 * _local1;
        this.fridayQuad.skewY = 0.29;
        this.fridayQuad.skewX = -0.2;
        this._global._gameS.addChild(this.fridayQuad);
        this.daysAr.push(this.fridayQuad);
        this.saturdayQuad = new ExtendTextField(160, 120, "Day\n6", "bromine", 40, 0, true);
        this.saturdayQuad.x = 590;
        this.saturdayQuad.y = 230;
        this.saturdayQuad.rotation = -11 * _local1;
        this.saturdayQuad.skewX = -0.17;
        this._global._gameS.addChild(this.saturdayQuad);
        this.daysAr.push(this.saturdayQuad);
        this.sundayQuad = new ExtendTextField(200, 160, "Day\n7", "bromine", 40, 0, true);
        this.sundayQuad.x = 200;
        this.sundayQuad.y = 420;
        this.sundayQuad.rotation = -15 * _local1;
        this._global._gameS.addChild(this.sundayQuad);
        this.daysAr.push(this.sundayQuad);
        _local2 = 0;
        goto L896;
      L794:
        _local3 = new cSprite(TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal"));
        _local3.alignPivot();
        var _local4 = 0.3;
        _local3.scaleY = 0.3;
        _local3.scaleX = _local4;
        _local3.x = this.badgePosX[_local2];
        _local3.y = this.badgePosY[_local2];
        _local3.touchable = false;
        this._global._gameS.addChild(_local3);
        this.badgeIcon.push(_local3);
        _local2++;
      L896:
        if (_local2 < this.daysAr.length) goto L794;
        this.badgeIconBig = new cSprite(TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal"));
        this.badgeIconBig.x = 670;
        this.badgeIconBig.y = 380;
        this.badgeIconBig.touchable = false;
        _local4 = 0.75;
        this.badgeIconBig.scaleY = 0.75;
        this.badgeIconBig.scaleX = _local4;
        this._global._gameS.addChild(this.badgeIconBig);
        this.noteDescription = new ExtendTextField(350, 150, "", "jenna_sue", 30);
        this.noteDescription.hAlign = "left";
        this.noteDescription.vAlign = "top";
        this.noteDescription.touchable = false;
        this.noteDescription.x = 430;
        this.noteDescription.y = 370;
        this.noteDescription.rotation = -5 * Math.PI / 180;
        this.noteDescription.skewY = 0.18;
        this.noteDescription.skewX = -0.3;
        this._global._gameS.addChild(this.noteDescription);
        this.noteDescription2 = new ExtendTextField(150, 150, "", "jenna_sue", 30);
        this.noteDescription2.hAlign = "left";
        this.noteDescription2.vAlign = "top";
        this.noteDescription2.touchable = false;
        this.noteDescription2.x = 683;
        this.noteDescription2.y = 393;
        this.noteDescription2.rotation = -5 * Math.PI / 180;
        this.noteDescription2.skewY = -0.1;
        this.noteDescription2.skewX = -0.3;
        this._global._gameS.addChild(this.noteDescription2);
        this.checkMark1 = new cSprite(TGlobalClass._windowDisplay.windowsAtlas.getTexture("check_mark"));
        this.checkMark1.x = 635;
        this.checkMark1.y = 413;
        _local4 = 0.5;
        this.checkMark1.scaleY = 0.5;
        this.checkMark1.scaleX = _local4;
        this._global._gameS.addChild(this.checkMark1);
        this.checkMark2 = new cSprite(TGlobalClass._windowDisplay.windowsAtlas.getTexture("check_mark"));
        this.checkMark2.x = 622;
        this.checkMark2.y = 446;
        _local4 = 0.5;
        this.checkMark2.scaleY = 0.5;
        this.checkMark2.scaleX = _local4;
        this._global._gameS.addChild(this.checkMark2);
        this.playButton = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_green"), "PLAY", "bromine", 30, 16777215);
        this.playButton.pivotX = this.playButton.width / 2;
        this.playButton.pivotY = this.playButton.height / 2;
        this.playButton._text.touchable = false;
        this.playButton.x = 512;
        this.playButton.y = 550;
        this.playButton._image.name = String(this.PLAY_BUTTON);
        this._global._gameS.addChild(this.playButton);
        this.buttonAr.push(this.playButton._image);
        this.backButton = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("small_btn_blue"), "Back", "bromine", 30, 16777215);
        this.backButton.pivotX = this.backButton.width / 2;
        this.backButton.pivotY = this.backButton.height / 2;
        this.backButton._text.touchable = false;
        this.backButton.x = 880;
        this.backButton.y = 50;
        this.backButton._image.name = String(this.BACK_BUTTON);
        this._global._gameS.addChild(this.backButton);
        this.buttonAr.push(this.backButton._image);
        this.rightButton = new TNonStateButton(this.levelSelectAtlas.getTexture("arrow_button"));
        this.rightButton.pivotX = this.rightButton.width / 2;
        this.rightButton.pivotY = this.rightButton.height / 2;
        this.rightButton.x = 920;
        this.rightButton.y = 400;
        this.rightButton.name = String(this.RIGHT_BUTTON);
        this._global._gameS.addChild(this.rightButton);
        this.buttonAr.push(this.rightButton);
        this.leftButton = new TNonStateButton(this.levelSelectAtlas.getTexture("arrow_button"));
        this.leftButton.pivotX = this.leftButton.width / 2;
        this.leftButton.pivotY = this.leftButton.height / 2;
        this.leftButton.x = 80;
        this.leftButton.y = 400;
        this.leftButton.scaleX = -1;
        this.leftButton.name = String(this.LEFT_BUTTON);
        this._global._gameS.addChild(this.leftButton);
        this.buttonAr.push(this.leftButton);
        _local2 = 0;
        goto L2004;
      L1979:
        this.buttonAr[_local2].addEventListener(TouchEvent.TOUCH, this.touchButton);
        _local2++;
      L2004:
        if (_local2 < this.buttonAr.length) goto L1979;
        _local2 = 0;
        goto L2060;
      L2023:
        this.daysAr[_local2].useHandCursor = true;
        this.daysAr[_local2].addEventListener(TouchEvent.TOUCH, this.selectDay);
        _local2++;
      L2060:
        if (_local2 < this.daysAr.length) goto L2023;
        TGlobalClass.currentPlayedLevel = TSavegame.currentPlayer.passedDays;
        var $t0 = !(TGlobalClass.currentPlayedLevel == TGlobalClass.TOTAL_DAYS);
        $s0 = $t0;
        if (!$t0) goto L2127;
        $s0 = TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] == true;
      L2127:
        if (!$s0) goto L2150;
        TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] = false;
      L2150:
        this.currentPage = Math.floor(TSavegame.currentPlayer.passedDays / 7);
        if (!(this.currentPage >= TGlobalClass.TOTAL_DAYS / 7)) goto L2204;
        this.currentPage = TGlobalClass.TOTAL_DAYS / 7 - 1;
      L2204:
        this.setAvailableDays();
        ready = true;
    }

    function setAvailableDays():void {
        var _local1 = 0;
        var _local2 = 0;
        _local1 = 0;
        goto L278;
      L15:
        _local2 = int(this.currentPage * 7 + _local1);
        if (!(_local2 > TSavegame.currentPlayer.passedDays)) goto L82;
        this.daysAr[_local1].text = "";
        this.daysAr[_local1].touchable = false;
        this.badgeIcon[_local1].visible = false;
        goto L276;
      L82:
        this.daysAr[_local1].text = "Day\n" + (_local2 + 1);
        this.daysAr[_local1].name = "" + _local2;
        this.daysAr[_local1].touchable = true;
        this.selectedDays = _local2;
        if (!TSavegame.currentPlayer.goalAchieved[_local2]) goto L264;
        if (!(TSavegame.currentPlayer.daysEarning[_local2] >= int(TGlobalClass.levelSetXML.day[_local2].expert))) goto L221;
        this.badgeIcon[_local1].texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
        goto L248;
      L221:
        this.badgeIcon[_local1].texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal");
      L248:
        this.badgeIcon[_local1].visible = true;
        goto L276;
      L264:
        this.badgeIcon[_local1].visible = false;
      L276:
        _local1++;
      L278:
        if (_local1 < 7) goto L15;
        if (!(this.currentPage >= Math.floor(TSavegame.currentPlayer.passedDays / 7))) goto L324;
        this.rightButton.visible = false;
        goto L332;
      L324:
        this.rightButton.visible = true;
      L332:
        if (!(this.currentPage <= 0)) goto L354;
        this.leftButton.visible = false;
        goto L362;
      L354:
        this.leftButton.visible = true;
      L362:
        var $t0 = TSavegame.currentPlayer.passedDays == TGlobalClass.TOTAL_DAYS;
        $s0 = $t0;
        if (!$t0) goto L411;
        $s0 = this.currentPage + 1 >= Math.floor(TSavegame.currentPlayer.passedDays / 7);
      L411:
        if (!$s0) goto L423;
        this.rightButton.visible = false;
      L423:
        if (!(this.selectedDays > TGlobalClass.TOTAL_DAYS - 1)) goto L449;
        this.selectedDays = TGlobalClass.TOTAL_DAYS - 1;
      L449:
        if (!(this.selectedDays < int(TSavegame.currentPlayer.passedDays / 7) * 7)) goto L490;
        this.selectedDays = this.selectedDays - 6;
      L490:
        var $t1 = TSavegame.currentPlayer.passedDays == TGlobalClass.TOTAL_DAYS;
        $s0 = $t1;
        if (!$t1) goto L527;
        $s0 = this.currentPage == TGlobalClass.TOTAL_DAYS / 7 - 1;
      L527:
        if (!$s0) goto L542;
        this.selectedDays = TGlobalClass.TOTAL_DAYS - 1;
      L542:
        this.daysAr[this.selectedDays % 7].color = 16711680;
        this.showDescription();
    }

    function selectDay(arg1:TouchEvent):void {
        var _local2 = 0;
        if (!arg1.getTouch(this._global._game, TouchPhase.BEGAN)) goto L188;
        _local2 = int((arg1.currentTarget as ExtendTextField).name);
        this.daysAr[this.selectedDays % 7].color = 0;
        this.selectedDays = _local2;
        this.daysAr[this.selectedDays % 7].color = 16711680;
        this.showDescription();
        var _local3 = false;
        this.checkMark2.visible = false;
        this.checkMark1.visible = _local3;
        if (!TSavegame.currentPlayer.goalAchieved[_local2]) goto L188;
        if (!(TSavegame.currentPlayer.daysEarning[_local2] >= int(TGlobalClass.levelSetXML.day[_local2].expert))) goto L180;
        this.checkMark2.visible = true;
      L180:
        this.checkMark1.visible = true;
      L188:
    }

    function showDescription():void {
        var _local1 = null;
        var _local2 = null;
        var _local6 = false;
        this.checkMark2.visible = false;
        this.checkMark1.visible = _local6;
        this.badgeIconBig.visible = false;
        if (!TSavegame.currentPlayer.goalAchieved[this.selectedDays]) goto L224;
        if (!(TSavegame.currentPlayer.daysEarning[this.selectedDays] >= int(TGlobalClass.levelSetXML.day[this.selectedDays].expert))) goto L112;
        this.checkMark2.visible = true;
      L112:
        this.checkMark1.visible = true;
        if (!(TSavegame.currentPlayer.daysEarning[this.selectedDays] >= int(TGlobalClass.levelSetXML.day[this.selectedDays].expert))) goto L193;
        this.badgeIconBig.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("expert");
        goto L216;
      L193:
        this.badgeIconBig.texture = TGlobalClass._windowDisplay.windowsAtlas.getTexture("goal");
      L216:
        this.badgeIconBig.visible = true;
      L224:
        var _local3 = TGlobalClass.levelSetXML.day[this.selectedDays];
        var _local4 = _local3.goal;
        var _local5 = _local3.expert;
        goto L288;
      L261:
        _local1 = "Western Cuisine";
        goto L356;
      L270:
        _local1 = "Asian Cuisine";
        goto L356;
      L279:
        _local1 = "Middle East Cuisine";
        goto L356;
      L288:
        _local6 = _local3.region.toString();
        if ("W" !== _local6) goto L313;
        $s0 = 0;
        goto L340;
      L313:
        if ("C" !== _local6) goto L328;
        $s0 = 1;
        goto L340;
      L328:
        goto L338;
      L338:
        $s0 = 2;
      L340:
        switch ($s0) { case 0: goto L261; case 1: goto L270; case 2: goto L279; default: goto L279; }
      L356:
        goto L481;
      L360:
        _local2 = "Hidden Objects";
        _local1 = "Cleaning up";
        _local4 = "find " + _local4 + " items";
        _local5 = _local5 + " items";
        goto L549;
      L393:
        _local2 = "Minigame";
        _local1 = "Froyo Business";
        _local4 = "earn $" + _local4;
        _local5 = "earn $" + _local5;
        goto L549;
      L424:
        _local2 = "Match 3";
        $s0 = "serve " + _local4;
        if (!((this.selectedDays + 1) % 7 == 0)) goto L460;
        $s1 = " food critic";
        goto L464;
      L460:
        $s1 = " customer(s)";
      L464:
        _local4 = $s0 + $s1;
        _local5 = "earn $ " + _local5;
        goto L549;
      L481:
        _local6 = _local3.gameType.toString();
        if ("HO" !== _local6) goto L506;
        $s0 = 0;
        goto L533;
      L506:
        if ("DM" !== _local6) goto L521;
        $s0 = 1;
        goto L533;
      L521:
        goto L531;
      L531:
        $s0 = 2;
      L533:
        switch ($s0) { case 0: goto L360; case 1: goto L393; case 2: goto L424; default: goto L424; }
      L549:
        if (_local4 != "") goto L604;
        _local4 = String(int(this._global._minigameHO.totalItemPerLevel[int(this.selectedDays / 14)] * 0.75));
      L604:
        if (_local5 != "") goto L649;
        _local5 = String(this._global._minigameHO.totalItemPerLevel[int(this.selectedDays / 14)]);
      L649:
        this.noteDescription.text = _local1 + " (" + _local2 + ")\nGoal: " + _local4 + "\nExpert: " + _local5 + "\nYour Best Score: " + TSavegame.currentPlayer.daysEarning[this.selectedDays];
    }

    function touchButton(arg1:TouchEvent):void {
        var _local2 = 0;
        var _local3 = null;
        if (!arg1.getTouch(this._global._game, TouchPhase.BEGAN)) goto L403;
        _local2 = int((arg1.currentTarget as DisplayObject).name);
        if (_local2 != this.PLAY_BUTTON) goto L258;
        _local3 = TGlobalClass.levelSetXML.day[this.selectedDays].gameType;
        TGlobalClass.currentRegionDish = TGlobalClass.levelSetXML.day[this.selectedDays].region.toString();
        if (_local3 != ConstClass.GAME_TYPE_M3) goto L142;
        this._global._game.fadeToScreen(this._global.gameScreen);
        goto L244;
      L142:
        if (_local3 != ConstClass.GAME_TYPE_HO) goto L175;
        this._global._game.fadeToScreen(this._global._minigameHO);
        goto L244;
      L175:
        if (_local3 != ConstClass.GAME_TYPE_SPECIAL_M3) goto L215;
        TGlobalClass.isPlayingSpecialLevel = true;
        this._global._game.fadeToScreen(this._global.gameScreen);
        goto L244;
      L215:
        if (_local3 != ConstClass.GAME_TYPE_DESSERT_MAKING) goto L244;
        this._global._game.fadeToScreen(this._global._minigameDessert);
      L244:
        TGlobalClass.currentPlayedLevel = this.selectedDays;
        goto L403;
      L258:
        if (_local2 != this.BACK_BUTTON) goto L289;
        this._global._game.fadeToScreen(this._global._kitchen);
        goto L403;
      L289:
        if (_local2 != this.RIGHT_BUTTON) goto L349;
        this.daysAr[this.selectedDays % 7].color = 0 + 0;
        var _local4 = this;
        var _local5 = this.currentPage + 1;
        _local4.currentPage = _local5;
        this.setAvailableDays();
        goto L403;
      L349:
        if (_local2 != this.LEFT_BUTTON) goto L403;
        this.daysAr[this.selectedDays % 7].color = 0;
        _local4 = this;
        _local5 = this.currentPage - 1;
        _local4.currentPage = _local5;
        this.setAvailableDays();
      L403:
    }
}
}
