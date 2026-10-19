package {
class HOClass {
    var targetIDBegan:int;
    var _global:Global;
    var itemNameAr:Vector.<String>;
    var itemTypeAr:Vector.<uint>;
    var itemPosXAr:Vector.<Number>;
    var itemPosYAr:Vector.<Number>;
    var itemFileNameAr:Vector.<String>;
    var itemHOAr:Vector.<HOObjects>;
    var itemAsNormal:Vector.<int>;
    var itemCoins:Vector.<int>;
    var itemDisplayAr:Vector.<Sprite>;
    var itemMultiples:Vector.<Vector.<int>>;
    var itemLogoIdx:int;
    var itemTextUIAr:Vector.<ExtendTextField>;
    var itemUIAr:Vector.<HOObjects>;
    var listedItemsPlace:Quad;
    var HOItem:HOObjects;
    var HOText:ExtendTextField;
    var pixelHitArea:PixelHitArea;
    var itemsData:XML;
    var revealedItem:HOObjects;
    var missClick:Boolean;
    var missClickCount:int;
    var allowedMissClick:int;
    var itemCounting:int;
    var maxItems:int;
    var totalItems:int;
    var itemTweening:Boolean;
    var loadedPack:int;
    var getRevealedItem:Boolean;
    var itemFoundCount:int;
    var comboCount:int;
    var soundCombo:int;
    var revealedDisplayIdx:int;
    var onClick:Boolean;
    var itemIdx:int;
    var onScaleBack:Boolean;
    var listedItemAr:Vector.<int>;
    var itemFoundAr:Vector.<int>;
    var occupiedPlace:Array;
    var occupiedIdx:Vector.<int>;
    const MAX_DISPLAYED_ITEMS:uint = 3;
    const AS_HO_OBJECT_ID:uint = 1;
    const AS_BG_ID:uint = 2;
    const AS_BG2_ID:uint = 7;
    const AS_COIN_ID:uint = 4;
    const AS_LOGO_ID:uint = 5;
    const AS_MULTIPLE_ID:uint = 6;
    var multipleItemsTweening:int;
    const HINT_X:Number = 940;
    const HINT_Y:Number = 60;
    var revealedIdx:uint;
    var centerX:Number = 150;
    var centerY:Number = 150;
    var radiusX:Number = 80;
    var radiusY:Number = 80;
    var angleStep:Number = 0.02;
    var twoPI:Number = 6.283185307179586;
    var currentAngle:Number = 0.25;

    function HOClass():* {
        this._global = Global.getInstance();
        this.occupiedPlace = [false, false, false];
        this.occupiedIdx = new Vector.<int>(3, true);
        super();
    }

    function Kill():void {
        var _local1 = uint(this.itemDisplayAr.length);
        var _local2 = uint(0);
        goto L120;
      L19:
        TweenLite.killTweensOf(this.itemDisplayAr[_local2]);
        this.itemDisplayAr[_local2].removeEventListener(TouchEvent.TOUCH, this.touchHOObjects);
        this.itemDisplayAr[_local2].removeFromParent(true);
        this.itemDisplayAr[_local2] = null;
        this.itemHOAr[_local2].Kill();
        this.itemHOAr[_local2].display.removeFromParent(true);
        this.itemHOAr[_local2] = null;
        _local2 = uint(_local2 + 1);
      L120:
        if (_local2 < _local1) goto L19;
        this.itemDisplayAr.length = 0;
        this.itemHOAr.length = 0;
        _local1 = uint(this.itemTextUIAr.length);
        _local2 = uint(0);
        goto L188;
      L161:
        this.itemTextUIAr[_local2].removeFromParent(true);
        this.itemTextUIAr[_local2] = null;
        _local2 = uint(_local2 + 1);
      L188:
        if (_local2 < _local1) goto L161;
        this.itemTextUIAr.length = 0;
        this.itemFileNameAr.length = 0;
        this.itemNameAr.length = 0;
        this.itemPosXAr.length = 0;
        this.itemPosYAr.length = 0;
        this.itemTypeAr.length = 0;
        this.pixelHitArea.dispose();
        this.pixelHitArea = null;
        if (!this.HOText) goto L283;
        this.HOText.removeFromParent(true);
        this.HOText = null;
      L283:
        this.HOItem.display.filter = null;
        this.HOItem.display.removeFromParent(true);
        this.HOItem.display = null;
        this.HOItem = null;
        if (!this.HOText) goto L336;
        this.HOText.removeFromParent(true);
      L336:
        this.HOText = null;
        this.listedItemsPlace.removeFromParent(true);
        this.listedItemsPlace = null;
        this.itemsData = null;
        goto L373;
      L364:
        this.itemAsNormal.pop();
      L373:
        if (this.itemAsNormal.length > 0) goto L364;
        goto L399;
      L390:
        this.itemMultiples.pop();
      L399:
        if (this.itemMultiples.length > 0) goto L390;
    }

    function init(arg1:XML):void {
        this.itemFileNameAr = new Vector.<String>();
        this.itemNameAr = new Vector.<String>();
        this.itemDisplayAr = new Vector.<Sprite>();
        this.itemPosXAr = new Vector.<Number>();
        this.itemPosYAr = new Vector.<Number>();
        this.itemTypeAr = new Vector.<uint>();
        this.itemHOAr = new Vector.<HOObjects>();
        this.itemUIAr = new Vector.<HOObjects>();
        this.itemTextUIAr = new Vector.<ExtendTextField>();
        this.listedItemAr = new Vector.<int>();
        this.itemAsNormal = new Vector.<int>();
        this.itemFoundAr = new Vector.<int>();
        this.itemCoins = new Vector.<int>();
        this.itemMultiples = new Vector.<Vector.<int>>();
        this.itemLogoIdx = -1;
        this.readItemData(arg1);
    }

    function buildPixelHitArea(arg1:Bitmap):void {
        this.pixelHitArea = new PixelHitArea(arg1, 0.5);
    }

    function initHOItems(arg1:int, arg2:int, arg3:TextureAtlas):void {
        var _local6 = 0;
        var _local7 = 0;
        var _local8 = 0;
        this.listedItemAr.length = 0;
        this.loadedPack = arg1;
        this.maxItems = arg2;
        this.totalItems = arg2;
        var _local4 = uint(this.itemsData.children().length());
        var _local5 = uint(0);
        goto L205;
      L62:
        this.HOItem = new HOObjects(arg3.getTexture(this.itemFileNameAr[_local5]), this.itemNameAr[_local5], this.itemTypeAr[_local5], _local5, this.itemPosXAr[_local5], this.itemPosYAr[_local5], this.pixelHitArea);
        this._global._minigameHO.zoomContainer.addChild(this.HOItem.display);
        this._global._minigameHO.zoomContainer.addChild(this.HOItem.path);
        this.itemDisplayAr.push(this.HOItem.display);
        this.itemHOAr.push(this.HOItem);
        _local5 = uint(_local5 + 1);
      L205:
        if (_local5 < _local4) goto L62;
        if (TGlobalClass.loadGame) goto L304;
        goto L297;
      L227:
        _local7 = int(TGlobalClass.RandomRange(0, this.itemAsNormal.length - 1));
        _local8 = int(this.itemAsNormal[_local7]);
        this.itemAsNormal.splice(_local7, 1);
        this.listedItemAr.push(_local8);
        this.itemHOAr[_local8].listed = true;
        _local6++;
      L297:
        if (_local6 < arg2) goto L227;
      L304:
        _local4 = uint(this.itemDisplayAr.length);
        _local5 = uint(0);
        goto L353;
      L323:
        this.itemDisplayAr[_local5].addEventListener(TouchEvent.TOUCH, this.touchHOObjects);
        _local5 = uint(_local5 + 1);
      L353:
        if (_local5 < _local4) goto L323;
    }

    function initUIObjects(arg1:TextureAtlas):void {
        var _local6 = 0;
        var _local7 = 0;
        var _local8 = null;
        var _local9 = null;
        var _local10 = 0;
        var _local11 = null;
        var _local12 = 0;
        var _local13 = 0;
        var _local14 = 0;
        var _local15 = null;
        var _local16 = null;
        var _local17 = null;
        var _local2 = uint(this.listedItemAr.length);
        this.listedItemsPlace = new Quad(620, 100, 16777215);
        this.listedItemsPlace.x = 220;
        this.listedItemsPlace.alpha = 0.5;
        this.listedItemsPlace.touchable = false;
        this._global._gameS.addChild(this.listedItemsPlace);
        var _local3 = uint(this.listedItemsPlace.width);
        var _local4 = new Vector.<int>();
        var _local5 = 0;
        goto L900;
      L151:
        _local6 = int(this.listedItemAr[_local5]);
        _local7 = 0;
        goto L192;
      L172:
        if (_local6 != _local4[_local7]) goto L190;
        goto L203;
      L190:
        _local7++;
      L192:
        if (_local7 < _local4.length) goto L172;
      L203:
        if (_local7 == _local4.length) goto L218;
        goto L898;
      L218:
        _local8 = this.itemNameAr[_local6];
        if (this.itemTypeAr[_local6] != this.AS_MULTIPLE_ID) goto L472;
        if (!this.itemHOAr[_local6].found) goto L267;
        goto L898;
      L267:
        _local12 = 1;
        _local7 = int(_local5 + 1);
        goto L352;
      L283:
        var $t0 = this.itemNameAr[this.listedItemAr[_local7]] == _local8;
        $s0 = $t0;
        if (!$t0) goto L329;
        $s0 = !this.itemHOAr[this.listedItemAr[_local7]].found;
      L329:
        if (!$s0) goto L350;
        _local4.push(this.listedItemAr[_local7]);
        _local12++;
      L350:
        _local7++;
      L352:
        if (_local7 < _local2) goto L283;
        _local13 = 0;
        _local14 = 0;
        goto L393;
      L371:
        if (this.itemTypeAr[_local14] != this.AS_MULTIPLE_ID) goto L391;
        _local13++;
      L391:
        _local14++;
      L393:
        if (_local14 < _local6) goto L371;
        _local15 = this.itemsData.children().attribute("plural")[_local13];
        if (_local12 != 1) goto L460;
        _local8 = _local8.charAt(0).toUpperCase() + _local8.substring(1);
        goto L472;
      L460:
        _local8 = _local12 + " " + _local15;
      L472:
        this.HOText = new ExtendTextField(190, 130, _local8, "bromine", 25, 0);
        this.HOText.hAlign = "center";
        this.HOText.vAlign = "center";
        this.HOText.pivotX = this.HOText.width / 2;
        this.HOText.pivotY = this.HOText.height / 2;
        this.HOText.touchable = false;
        this.HOText.text = this.HOText.text.replace("_QUOTE_", "'");
        _local9 = this.HOText.text.charAt(0).toUpperCase();
        this.HOText.text = _local9 + this.HOText.text.substring(1);
        _local10 = 0;
        _local11 = " ";
        goto L724;
      L638:
        if (this.HOText.text.charCodeAt(_local10) != _local11.charCodeAt()) goto L722;
        var $t1 = this.HOText.text.split("");
        _local16 = $t1;
        $t1[_local10 + 1] = _local16[_local10 + 1].toUpperCase();
        _local17 = _local16.join("");
        this.HOText.text = _local17;
      L722:
        _local10++;
      L724:
        if (_local10 < this.HOText.text.length) goto L638;
        this.HOText.text = this.HOText.text.replace(" Of ", " of ");
        this.HOText.text = this.HOText.text.replace(" And ", " and ");
        this.HOText.idx = _local6;
        this.HOText.x = this.listedItemsPlace.x;
        this.HOText.y = 4294966996;
        this.itemTextUIAr.push(this.HOText);
        this._global._gameS.addChild(this.HOText);
        _local7 = 0;
        goto L885;
      L859:
        if (_local6 != this.itemFoundAr[_local7]) goto L883;
        this.HOText.itemIsFound = true;
      L883:
        _local7++;
      L885:
        if (_local7 < this.itemFoundAr.length) goto L859;
      L898:
        _local5++;
      L900:
        if (_local5 < _local2) goto L151;
        this.setListedItems();
    }

    function setListedItems():void {
        var _local2 = 0;
        var _local1 = int(this.listedItemAr.length);
        var _local3 = 0;
        goto L142;
      L21:
        if (this.itemTextUIAr[_local3].itemIsFound == true) goto L140;
        if (!(_local2 > 2)) goto L49;
        goto L148;
      L49:
        this.itemTextUIAr[_local3].y = this.listedItemsPlace.y + 50;
        this.itemTextUIAr[_local3].x = _local2 * 200 + 235 + this.itemTextUIAr[_local3].width / 2;
        this.itemTextUIAr[_local3].aktif = true;
        this.itemTextUIAr[_local3].placeNumber = _local2;
        this.occupiedPlace[_local2] = true;
        _local2++;
      L140:
        _local3++;
      L142:
        if (_local3 < _local1) goto L21;
      L148:
    }

    function removeItem(arg1:int, arg2:int, arg3:Boolean = true):void {
        this.itemTextUIAr[arg2].x = -100;
        this.itemTextUIAr[arg2].itemIsFound = true;
        this.occupiedPlace[this.itemTextUIAr[arg2].placeNumber] = false;
        this.itemDisplayAr[arg1].x = 4294967096;
        this.itemHOAr[arg1].onTween = false;
        var _local4 = this;
        var _local5 = this.totalItems - 1;
        _local4.totalItems = _local5;
        this.itemTweening = false;
        this.setDisplayedItems();
        if (!(this.totalItems <= 0)) goto L121;
        this.onLevelComplete();
      L121:
    }

    function onLevelComplete():void {
        var _local1 = 0;
        var _local2 = 0;
        if (TGlobalClass.extrasMode) goto L1328;
        var $t0 = TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] == 0;
        $s0 = $t0;
        if (!$t0) goto L61;
        $s0 = TGlobalClass.currentPlayedLevel == TSavegame.currentPlayer.passedDays;
      L61:
        if (!$s0) goto L638;
        if (!(this.itemFoundCount >= this._global._minigameHO.goalTarget)) goto L604;
        if (TSavegame.currentPlayer.unlocked[Achievement.QUALIFIED_CHEF]) goto L258;
        var $t1 = TSavegame.currentPlayer.achievementCounter;
        var _local3 = $t1;
        var $t2 = Achievement.QUALIFIED_CHEF;
        var _local4 = $t2;
        var _local5 = $t1[$t2] + 1;
        _local3[_local4] = _local5;
        if (!(TSavegame.currentPlayer.achievementCounter[Achievement.QUALIFIED_CHEF] >= Achievement.ALL_TARGET[Achievement.QUALIFIED_CHEF])) goto L218;
        TSavegame.currentPlayer.unlocked[Achievement.QUALIFIED_CHEF] = true;
        Achievement.tweenAchievementNotif(Achievement.QUALIFIED_CHEF);
        goto L258;
      L218:
        if (TSavegame.currentPlayer.achievementCounter[Achievement.QUALIFIED_CHEF] % 15 != 0) goto L258;
        Achievement.tweenAchievementProgress(Achievement.QUALIFIED_CHEF);
      L258:
        TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] = this._global._minigameHO._HOClass.itemFoundCount;
        var $t3 = TSavegame.currentPlayer;
        _local3 = $t3;
        _local4 = $t3.passedDays + 1;
        _local3.passedDays = _local4;
        if (this.totalItems != 0) goto L553;
        TSavegame.currentPlayer.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(TSavegame.currentPlayer.money) + 6);
        TGlobalClass.coinsGot = 6;
        if (TSavegame.currentPlayer.unlocked[Achievement.INTERNATIONAL_CHEF]) goto L549;
        var $t4 = TSavegame.currentPlayer.achievementCounter;
        _local3 = $t4;
        var $t5 = Achievement.INTERNATIONAL_CHEF;
        _local4 = $t5;
        _local5 = $t4[$t5] + 1;
        _local3[_local4] = _local5;
        if (!(TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] >= Achievement.ALL_TARGET[Achievement.INTERNATIONAL_CHEF])) goto L509;
        TSavegame.currentPlayer.unlocked[Achievement.INTERNATIONAL_CHEF] = true;
        Achievement.tweenAchievementNotif(Achievement.INTERNATIONAL_CHEF);
        goto L549;
      L509:
        if (TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] % 15 != 0) goto L549;
        Achievement.tweenAchievementProgress(Achievement.INTERNATIONAL_CHEF);
      L549:
        goto L604;
      L553:
        TSavegame.currentPlayer.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(TSavegame.currentPlayer.money) + 3);
        TGlobalClass.coinsGot = 3;
      L604:
        _local1 = int(this._global._maxCrypto.STI(this._global._minigameHO.bonusMoney));
        TSavegame.save();
        goto L1064;
      L638:
        var $t6 = this.itemFoundCount >= this._global._minigameHO.goalTarget;
        $s0 = $t6;
        if (!$t6) goto L699;
        $s0 = this._global._maxCrypto.STI(this._global._minigameHO.bonusMoney) > TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel];
      L699:
        if (!$s0) goto L1057;
        TGlobalClass.daysEarningPrev = TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel];
        var $t7 = this.totalItems == 0;
        $s0 = $t7;
        if (!$t7) goto L763;
        $s0 = this.itemFoundCount > TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel];
      L763:
        if (!$s0) goto L993;
        TSavegame.currentPlayer.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(TSavegame.currentPlayer.money) + 3);
        TGlobalClass.coinsGot = 3;
        if (TSavegame.currentPlayer.unlocked[Achievement.INTERNATIONAL_CHEF]) goto L993;
        var $t8 = TSavegame.currentPlayer.achievementCounter;
        _local3 = $t8;
        var $t9 = Achievement.INTERNATIONAL_CHEF;
        _local4 = $t9;
        _local5 = $t8[$t9] + 1;
        _local3[_local4] = _local5;
        if (!(TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] >= Achievement.ALL_TARGET[Achievement.INTERNATIONAL_CHEF])) goto L953;
        TSavegame.currentPlayer.unlocked[Achievement.INTERNATIONAL_CHEF] = true;
        Achievement.tweenAchievementNotif(Achievement.INTERNATIONAL_CHEF);
        goto L993;
      L953:
        if (TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] % 15 != 0) goto L993;
        Achievement.tweenAchievementProgress(Achievement.INTERNATIONAL_CHEF);
      L993:
        _local1 = int(this._global._maxCrypto.STI(this._global._minigameHO.bonusMoney) - TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel]);
        TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] = this.itemFoundCount;
      L1057:
        TSavegame.save();
      L1064:
        if (TSavegame.currentPlayer.unlocked[Achievement.BIG_BUSINESS]) goto L1263;
        _local2 = int(TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS]);
        TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS] = TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS] + _local1;
        if (!(TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS] >= Achievement.ALL_TARGET[Achievement.BIG_BUSINESS])) goto L1217;
        TSavegame.currentPlayer.unlocked[Achievement.BIG_BUSINESS] = true;
        Achievement.tweenAchievementNotif(Achievement.BIG_BUSINESS);
        goto L1263;
      L1217:
        if (!(_local2 % 20000 > TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS] % 20000)) goto L1263;
        Achievement.tweenAchievementProgress(Achievement.BIG_BUSINESS);
      L1263:
        TSavegame.save();
        if (!(this.itemFoundCount >= this._global._minigameHO.goalTarget)) goto L1308;
        TGlobalClass._windowDisplay.showWindow(TWindowState.HO_LEVEL_COMPLETE);
        goto L1324;
      L1308:
        TGlobalClass._windowDisplay.showWindow(TWindowState.HO_LEVEL_FAILED);
      L1324:
        goto L1440;
      L1328:
        this._global._soundAsset.playSFX("goal_reached");
        if (!(this._global._minigameHO.currentLevel >= this._global._minigameHO.levelSet.length - 1)) goto L1390;
        TGlobalClass._windowDisplay.showWindow(TWindowState.HO_LEVEL_COMPLETE_EXTRAS_ALL);
        goto L1440;
      L1390:
        if (!this._global._minigameHO.gameover) goto L1424;
        TGlobalClass._windowDisplay.showWindow(TWindowState.HO_LEVEL_FAILED_EXTRAS);
        goto L1440;
      L1424:
        TGlobalClass._windowDisplay.showWindow(TWindowState.HO_LEVEL_COMPLETE_EXTRAS);
      L1440:
    }

    function removeMultipleItem(arg1:int, arg2:int):void {
        var _local4 = this;
        var _local5 = this.multipleItemsTweening - 1;
        _local4.multipleItemsTweening = _local5;
        TweenLite.to(this.itemDisplayAr[arg1], 0.7, {"scaleX": 0, "scaleY": 0});
        this.itemTweening = false;
        var _local3 = int(this.itemTextUIAr[arg2].text.substring(0, this.itemTextUIAr[arg2].text.indexOf(" ")));
        if (_local3 != 0) goto L116;
        this.removeItem(arg1, arg2);
        return;
      L116:
        if (!(_local3 > 2)) goto L174;
        this.itemTextUIAr[arg2].text = _local3 - 1 + this.itemTextUIAr[arg2].text.substring(this.itemTextUIAr[arg2].text.indexOf(" "));
        goto L238;
      L174:
        this.itemTextUIAr[arg2].text = this.itemNameAr[this.itemTextUIAr[arg2].idx].charAt(0).toUpperCase() + this.itemNameAr[this.itemTextUIAr[arg2].idx].substring(1);
      L238:
    }

    function setDisplayedItems(arg1:int = -1):void {
        var _local5 = uint(0);
        var _local2 = int(this.itemTextUIAr.length);
        var _local3 = int(this.occupiedPlace.length);
        var _local4 = uint(0);
        goto L223;
      L34:
        _local5 = uint(0);
        goto L210;
      L44:
        var $t0 = !(this.itemTextUIAr[_local5].itemIsFound == true);
        $s0 = $t0;
        if (!$t0) goto L81;
        $s0 = !(this.itemTextUIAr[_local5].aktif == true);
      L81:
        if (!$s0) goto L99;
        $s0 = !(this.occupiedPlace[_local4] == true);
      L99:
        if (!$s0) goto L204;
        this.itemTextUIAr[_local5].y = this.listedItemsPlace.y + 50;
        this.itemTextUIAr[_local5].x = _local4 * 200 + 235 + this.itemTextUIAr[_local5].width / 2;
        this.itemTextUIAr[_local5].aktif = true;
        this.itemTextUIAr[_local5].placeNumber = _local4;
        this.occupiedPlace[_local4] = true;
        goto L217;
      L204:
        _local5 = uint(_local5 + 1);
      L210:
        if (_local5 < _local2) goto L44;
      L217:
        _local4 = uint(_local4 + 1);
      L223:
        if (_local4 < _local3) goto L34;
    }

    function readItemData(arg1:XML):void {
        var _local2 = null;
        var _local3 = uint(0);
        var _local6 = uint(0);
        var _local8 = null;
        var _local9 = 0;
        var _local10 = null;
        this.itemsData = arg1;
        var _local4 = uint(this.itemsData.children().length());
        var _local5 = this.itemsData.children();
        var _local7 = uint(0);
        goto L564;
      L68:
        _local2 = _local5.attribute("name")[_local7];
        _local3 = uint(_local2.length);
        _local8 = _local2.substring(_local2.indexOf(",") + 1, _local3);
        this.itemFileNameAr.push(_local2);
        this.itemPosXAr.push(_local5.attribute("locX")[_local7]);
        this.itemPosYAr.push(_local5.attribute("locY")[_local7]);
        if (_local8.match("nm,mul") == null) goto L342;
        this.itemTypeAr.push(this.AS_MULTIPLE_ID);
        this.itemNameAr.push(_local2.substring(0, _local2.indexOf("_")));
        _local9 = 0;
        goto L277;
      L221:
        if (this.itemNameAr[this.itemMultiples[_local9][0]] != this.itemNameAr[_local7]) goto L275;
        this.itemMultiples[_local9].push(_local7);
        goto L290;
      L275:
        _local9++;
      L277:
        if (_local9 < this.itemMultiples.length) goto L221;
      L290:
        if (_local9 == this.itemMultiples.length) goto L307;
        goto L558;
      L307:
        var $t0 = new Vector.<int>();
        _local10 = $t0;
        $t0.push(_local7);
        this.itemMultiples.push(_local10);
        goto L558;
      L342:
        this.itemNameAr.push(_local2.substring(0, _local2.indexOf(",")));
        if (_local8.match("nm,coin") == null) goto L405;
        this.itemTypeAr.push(this.AS_COIN_ID);
        this.itemCoins.push(_local7);
        goto L558;
      L405:
        if (_local8.match("nm,logo") == null) goto L441;
        this.itemTypeAr.push(this.AS_LOGO_ID);
        this.itemLogoIdx = _local7;
        goto L558;
      L441:
        if (_local8.match("nm") == null) goto L502;
        this.itemTypeAr.push(this.AS_HO_OBJECT_ID);
        this.itemAsNormal.push(_local7);
        var _local11 = this;
        var _local12 = this.itemCounting + 1;
        _local11.itemCounting = _local12;
        goto L558;
      L502:
        if (_local8.match("bg2") == null) goto L532;
        this.itemTypeAr.push(this.AS_BG2_ID);
        goto L558;
      L532:
        if (_local8.match("bg") == null) goto L558;
        this.itemTypeAr.push(this.AS_BG_ID);
      L558:
        _local7 = uint(_local7 + 1);
      L564:
        if (_local7 < _local4) goto L68;
    }

    function touchHOObjects(arg1:TouchEvent):void {
        var _local2 = 0;
        var _local3 = 0;
        var _local4 = 0;
        if (!arg1.getTouch(this._global._game, TouchPhase.BEGAN)) goto L56;
        this.targetIDBegan = int((arg1.currentTarget as Sprite).name);
      L56:
        if (!arg1.getTouch(this._global._game, TouchPhase.ENDED)) goto L705;
        var $t0 = this._global.tutorial.currentDialogueID == Tutorial.LEVEL_4;
        $s0 = $t0;
        if (!$t0) goto L114;
        $s0 = this._global.tutorial.dialogueCounter < 4;
      L114:
        if (!$s0) goto L119;
        return;
      L119:
        var $t1 = this.itemFoundCount >= this.maxItems;
        $s0 = $t1;
        if ($t1) goto L145;
        $s0 = Boolean(this._global._minigameHO.touchDragged);
      L145:
        if (!$s0) goto L150;
        return;
      L150:
        _local2 = int((arg1.currentTarget as Sprite).name);
        if (this.targetIDBegan == _local2) goto L180;
        return;
      L180:
        this.onClick = true;
        var $t2 = this.itemHOAr[_local2].listed == true;
        $s0 = $t2;
        if (!$t2) goto L217;
        $s0 = this.itemHOAr[_local2].found == false;
      L217:
        if (!$s0) goto L643;
        _local3 = int(this.itemTextUIAr.length);
        _local4 = 0;
        goto L632;
      L238:
        if (this.itemTypeAr[_local2] != this.AS_MULTIPLE_ID) goto L382;
        var $t3 = this.itemNameAr[_local2] == this.itemNameAr[this.itemTextUIAr[_local4].idx];
        $s0 = $t3;
        if (!$t3) goto L301;
        $s0 = !this.itemHOAr[_local2].found;
      L301:
        if (!$s0) goto L321;
        $s0 = this.itemTextUIAr[_local4].aktif == true;
      L321:
        if (!$s0) goto L378;
        this.getCorrectMultipleItem(_local2, _local4);
        $s0 = this._global._soundAsset;
        $s1 = "SFX_gameplay_click_0";
        if (!(this.soundCombo > 7)) goto L361;
        $s2 = 8;
        goto L369;
      L361:
        $s2 = this.soundCombo + 1;
      L369:
        $s0.playSFX($s1 + $s2);
        goto L639;
      L378:
        goto L630;
      L382:
        var $t4 = _local2 == this.itemTextUIAr[_local4].idx;
        $s0 = $t4;
        if (!$t4) goto L416;
        $s0 = this.itemTextUIAr[_local4].itemIsFound == false;
      L416:
        if (!$s0) goto L436;
        $s0 = this.itemTextUIAr[_local4].aktif == true;
      L436:
        if (!$s0) goto L522;
        this.getCorrectItem(_local2, _local4);
        $s0 = this._global._soundAsset;
        $s1 = "SFX_gameplay_click_0";
        if (!(this.soundCombo > 7)) goto L476;
        $s2 = 8;
        goto L484;
      L476:
        $s2 = this.soundCombo + 1;
      L484:
        $s0.playSFX($s1 + $s2);
        if (!this.revealedItem) goto L514;
        if (_local2 != this.revealedItem.itemIdx) goto L514;
        this.getRevealedItem = true;
      L514:
        goto L639;
      L522:
        var $t5 = _local2 == this.itemTextUIAr[_local4].idx;
        $s0 = $t5;
        if (!$t5) goto L556;
        $s0 = this.itemTextUIAr[_local4].itemIsFound == false;
      L556:
        if (!$s0) goto L576;
        $s0 = this.itemTextUIAr[_local4].aktif == false;
      L576:
        if (!$s0) goto L630;
        this._global._minigameHO.onMissClick();
        var _local5 = this;
        var _local6 = this.missClickCount + 1;
        _local5.missClickCount = _local6;
        this._global._soundAsset.playSFX("SFX_wrong_click");
        goto L639;
      L630:
        _local4++;
      L632:
        if (_local4 < _local3) goto L238;
      L639:
        goto L705;
      L643:
        if (this.itemHOAr[_local2].listed == true) goto L705;
        this._global._soundAsset.playSFX("SFX_wrong_click");
        this._global._minigameHO.onMissClick();
        _local5 = this;
        _local6 = this.missClickCount + 1;
        _local5.missClickCount = _local6;
      L705:
    }

    function getCorrectItem(arg1:int, arg2:int):void {
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "get_correct_ho_item"));
        this.addScore();
        if (TGlobalClass.extrasMode) goto L221;
        if (TSavegame.currentPlayer.unlocked[Achievement.HAWKEYE]) goto L221;
        var $t0 = TSavegame.currentPlayer.achievementCounter;
        var _local4 = $t0;
        var $t1 = Achievement.HAWKEYE;
        var _local5 = $t1;
        var _local6 = $t0[$t1] + 1;
        _local4[_local5] = _local6;
        if (!(TSavegame.currentPlayer.achievementCounter[Achievement.HAWKEYE] > Achievement.ALL_TARGET[Achievement.HAWKEYE])) goto L181;
        TSavegame.currentPlayer.unlocked[Achievement.HAWKEYE] = true;
        Achievement.tweenAchievementNotif(Achievement.HAWKEYE);
        goto L221;
      L181:
        if (TSavegame.currentPlayer.achievementCounter[Achievement.HAWKEYE] % 50 != 0) goto L221;
        Achievement.tweenAchievementProgress(Achievement.HAWKEYE);
      L221:
        var _local3 = this.itemDisplayAr[arg1].parent.localToGlobal(new Point(this.itemDisplayAr[arg1].x, this.itemDisplayAr[arg1].y));
        this._global._gameS.addChild(this.itemDisplayAr[arg1]);
        this._global._gameS.setChildIndex(this.itemDisplayAr[arg1], this._global._gameS.getChildIndex(TGlobalClass._windowDisplay._blackScreen) - 1);
        this.itemDisplayAr[arg1].x = _local3.x;
        this.itemDisplayAr[arg1].y = _local3.y;
        this.itemDisplayAr[arg1].scaleX = this._global._minigameHO.zoomContainer.scaleX;
        this.itemDisplayAr[arg1].scaleY = this._global._minigameHO.zoomContainer.scaleY;
        this.itemHOAr[arg1].found = true;
        this.itemFoundAr.push(arg1);
        this.missClickCount = 0;
        this.itemTextUIAr[arg2].itemIsFound = true;
        _local4 = this;
        _local5 = this.itemFoundCount + 1;
        _local4.itemFoundCount = _local5;
        _local4 = this;
        _local5 = this.comboCount + 1;
        _local4.comboCount = _local5;
        _local4 = this;
        _local5 = this.soundCombo + 1;
        _local4.soundCombo = _local5;
        if (this._global._minigameHO.hasHint) goto L548;
        var $t2 = this._global._minigameHO;
        _local4 = $t2;
        _local5 = $t2.hintTimer + 1;
        _local4.hintTimer = _local5;
      L548:
        this.tweenFoundItem(arg1, arg2, 0);
    }

    function getCorrectMultipleItem(arg1:int, arg2:int):void {
        this.addScore();
        var _local3 = int(this.itemTextUIAr[arg2].text.substring(0, this.itemTextUIAr[arg2].text.indexOf(" ")));
        _local3 = int(_local3 - this.multipleItemsTweening);
        if (!(_local3 <= 1)) goto L92;
        var $t0 = TSavegame.currentPlayer;
        var _local5 = $t0;
        var _local6 = $t0.totalItemsClassic - 1;
        _local5.totalItemsClassic = _local6;
      L92:
        _local5 = this;
        _local6 = this.multipleItemsTweening + 1;
        _local5.multipleItemsTweening = _local6;
        var _local4 = this.itemDisplayAr[arg1].parent.localToGlobal(new Point(this.itemDisplayAr[arg1].x, this.itemDisplayAr[arg1].y));
        this._global._gameS.addChild(this.itemDisplayAr[arg1]);
        this._global._gameS.setChildIndex(this.itemDisplayAr[arg1], this._global._gameS.getChildIndex(TGlobalClass._windowDisplay._blackScreen) - 1);
        this.itemDisplayAr[arg1].x = _local4.x;
        this.itemDisplayAr[arg1].y = _local4.y;
        this.itemDisplayAr[arg1].scaleX = this._global._minigameHO.zoomContainer.scaleX;
        this.itemDisplayAr[arg1].scaleY = this._global._minigameHO.zoomContainer.scaleY;
        this.itemHOAr[arg1].found = true;
        this.itemFoundAr.push(arg1);
        _local5 = this;
        _local6 = this.itemFoundCount + 1;
        _local5.itemFoundCount = _local6;
        this.missClickCount = 0;
        _local5 = this;
        _local6 = this.comboCount + 1;
        _local5.comboCount = _local6;
        _local5 = this;
        _local6 = this.soundCombo + 1;
        _local5.soundCombo = _local6;
        if (this._global._minigameHO.hasHint) goto L431;
        var $t1 = this._global._minigameHO;
        _local5 = $t1;
        _local6 = $t1.hintTimer + 1;
        _local5.hintTimer = _local6;
      L431:
        this.tweenFoundItem(arg1, arg2, 0);
    }

    function tweenFoundItem(arg1:int, arg2:int, arg3:int):void {
        randomFirstX = NaN;
        randomSecondX = NaN;
        selisih = NaN;
        points = null;
        jarak = NaN;
        multiplier = NaN;
        targetID = arg1;
        UIIdx = arg2;
        tweenCount = arg3;
        this.itemHOAr[targetID].onTween = true;
        this.itemTweening = true;
        TweenLite.killTweensOf(this.itemDisplayAr[targetID]);
        if (!(tweenCount < 1)) goto L381;
        tweenCount = tweenCount + 1;
        TweenLite.to(this.itemDisplayAr[targetID], 0.4, {"scaleX": this.itemDisplayAr[targetID].scaleX + 0.4, "scaleY": this.itemDisplayAr[targetID].scaleY + 0.4, "ease": Elastic.easeOut, "onComplete": this.tweenFoundItem, "onCompleteParams": [targetID, UIIdx, tweenCount]});
        if (targetID != this.revealedIdx) goto L257;
        this._global.ps.pause();
        this._global.ps.removeEventListener(Event.ENTER_FRAME, this.tweenCircle);
        this.currentAngle = 0.25;
      L257:
        if (!TGlobalClass.EnableFoundItemParticle) goto L377;
        this._global.psFound.start();
        this._global._gameS.setChildIndex(this._global.psFound, this._global._gameS.getChildIndex(TGlobalClass._windowDisplay._blackScreen) - 1);
        this._global.psFound.emitterX = this.itemDisplayAr[targetID].x;
        this._global.psFound.emitterY = this.itemDisplayAr[targetID].y;
        TweenLite.delayedCall(0.5, function():void {
            _global.psFound.pause();
        });
      L377:
        goto L1012;
      L381:
        randomFirstX = Number(TGlobalClass.RandomRange(1, 2));
        if (randomFirstX != 1) goto L418;
        randomFirstX = Number(-100);
        goto L425;
      L418:
        randomFirstX = Number(100);
      L425:
        randomSecondX = Number(TGlobalClass.RandomRange(30, 80));
        if (!(randomFirstX > 0)) goto L466;
        Math.abs(randomSecondX);
        goto L476;
      L466:
        randomSecondX = Number(-randomSecondX);
      L476:
        selisih = Number(this.itemDisplayAr[targetID].y - this.itemTextUIAr[UIIdx].y);
        points = [{"x": this.itemDisplayAr[targetID].x, "y": this.itemDisplayAr[targetID].y}, {"x": this.itemDisplayAr[targetID].x + randomFirstX, "y": this.itemDisplayAr[targetID].y + selisih / 3}, {"x": this.itemDisplayAr[targetID].x + randomSecondX, "y": this.itemDisplayAr[targetID].y - selisih / 3 * 4}, {"x": this.itemTextUIAr[UIIdx].x, "y": this.itemTextUIAr[UIIdx].y}];
        this.itemHOAr[targetID].path.follower = this.itemDisplayAr[targetID];
        this.itemHOAr[targetID].path.init(points);
        jarak = Number(Math.sqrt(Math.pow(this.itemDisplayAr[targetID].y - this.itemTextUIAr[UIIdx].y, 2) + Math.pow(this.itemDisplayAr[targetID].x - this.itemTextUIAr[UIIdx].x, 2)));
        $s0 = $activation;
        if (!(jarak > 500)) goto L851;
        $s1 = Number(0.001);
        goto L873;
      L851:
        if (!(jarak < 200)) goto L870;
        $s1 = Number(0.0025);
        goto L873;
      L870:
        $s1 = Number(0.0015);
      L873:
        $s0.slot9 = Number($s1);
        this.itemHOAr[targetID].path.start(0.3 + jarak * multiplier, targetID, UIIdx, Linear.easeNone);
        TweenLite.to(this.itemDisplayAr[targetID], 0.3 + jarak * multiplier, {"scaleX": this.itemTextUIAr[UIIdx].scaleX, "scaleY": this.itemTextUIAr[UIIdx].scaleY, "rotation": 360 * Math.PI / 180, "ease": Linear.easeNone});
      L1012:
    }

    function revealItem():void {
        getUIIdx = 0;
        j = 0;
        bPath = null;
        X = NaN;
        Y = NaN;
        jarak = NaN;
        if (this._global._minigameHO.hasHint) goto L56;
        return;
      L56:
        available = new Vector.<int>();
        i = 0;
        goto L359;
      L82:
        if (this.itemTypeAr[this.itemTextUIAr[i].idx] != this.AS_MULTIPLE_ID) goto L285;
        var $t0 = this.itemTextUIAr[i].aktif == true;
        $s0 = $t0;
        if (!$t0) goto L150;
        $s0 = this.itemTextUIAr[i].itemIsFound == false;
      L150:
        if (!$s0) goto L281;
        j = 0;
        goto L266;
      L164:
        var $t1 = this.itemNameAr[this.itemTextUIAr[i].idx] == this.itemNameAr[this.listedItemAr[j]];
        $s0 = $t1;
        if (!$t1) goto L233;
        $s0 = !this.itemHOAr[this.listedItemAr[j]].found;
      L233:
        if (!$s0) goto L256;
        available.push(this.listedItemAr[j]);
      L256:
        j = j + 1;
      L266:
        if (j < this.listedItemAr.length) goto L164;
      L281:
        goto L349;
      L285:
        var $t2 = this.itemTextUIAr[i].aktif == true;
        $s0 = $t2;
        if (!$t2) goto L323;
        $s0 = this.itemTextUIAr[i].itemIsFound == false;
      L323:
        if (!$s0) goto L349;
        available.push(this.itemTextUIAr[i].idx);
      L349:
        i = i + 1;
      L359:
        if (i < this.itemTextUIAr.length) goto L82;
        if (available.length != 0) goto L388;
        return;
      L388:
        this._global._soundAsset.playSFX("bell_dring");
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "used_hint"));
        if (!(this._global._minigameHO.zoomContainer.scaleX > 1)) goto L531;
        this.onScaleBack = true;
        TweenLite.to(this._global._minigameHO.zoomContainer, 0.2, {"scaleX": 1, "scaleY": 1, "x": this._global._minigameHO.zoomContainer.pivotX, "y": this._global._minigameHO.zoomContainer.pivotY, "ease": Linear.easeNone, "onComplete": function():void {
            onScaleBack = false;
        }});
      L531:
        this.revealedIdx = available[TGlobalClass.RandomRange(0, available.length - 1)];
        this.revealedItem = this.itemHOAr[this.revealedIdx];
        this._global._minigameHO.useHint();
        if (!TGlobalClass.EnableHintParticle) goto L933;
        this._global.ps.start();
        bPath = new BezierPath();
        X = Number(this.itemDisplayAr[this.revealedIdx].x);
        Y = Number(this.itemDisplayAr[this.revealedIdx].y);
        this._global.ps.emitterX = this.HINT_X;
        this._global.ps.emitterY = this.HINT_Y;
        this.radiusX = this.itemDisplayAr[this.revealedIdx].width;
        this.radiusY = this.itemDisplayAr[this.revealedIdx].height;
        bPath.follower = this._global.ps;
        bPath.init([{"x": this.HINT_X, "y": this.HINT_Y}, {"x": this.HINT_X, "y": this.HINT_Y - TGlobalClass.RandomRange(-100, 300)}, {"x": X - TGlobalClass.RandomRange(200, 400), "y": Y + this.radiusY}, {"x": X, "y": Y + this.radiusY}]);
        jarak = Number(Math.sqrt(Math.pow(X - this.HINT_X, 2) + Math.pow(Y - this.HINT_Y, 2)));
        bPath.startPS(0.003 * jarak, Linear.easeNone);
        this.centerX = X;
        this.centerY = Y;
      L933:
        GALib.TrackEvent(GALib.EVENT_PRESS_HINT, this._global._minigameHO.currentLevel + 1, this.itemNameAr[this.revealedIdx]);
    }

    function tweenCircle(arg1:EnterFrameEvent):void {
        if (!this._global._minigameHO.paused) goto L17;
        return;
      L17:
        this.currentAngle = this.currentAngle - this.angleStep;
        this._global.ps.emitterX = this.centerX + Math.cos(this.currentAngle * this.twoPI) * this.radiusX;
        this._global.ps.emitterY = this.centerY + Math.sin(this.currentAngle * this.twoPI) * this.radiusY;
        if (!(this.currentAngle < -1.75)) goto L150;
        this.currentAngle = 0.25;
        this._global.ps.pause();
        this._global.ps.removeEventListener(Event.ENTER_FRAME, this.tweenCircle);
      L150:
    }

    function addScore():void {
        this._global._minigameHO.updateMoney();
    }
}
}
