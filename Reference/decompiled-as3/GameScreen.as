package {
class GameScreen extends Screen {
    const PAUSE_BUTTON:int = 1;
    const SOUND_BUTTON:int = 2;
    const JUICER_ID:int = 3;
    const FRYER_ID:int = 4;
    const MUSIC_BUTTON:int = 5;
    const PLAY_STAR:int = 1;
    const PLAY_ASAP:int = 2;
    const PLAY_API:int = 3;
    var _udaLoad:Boolean;
    var _global:Global;
    var loadedFiles:Array;
    var loadQueue:Vector.<String>;
    var loadQueueType:Vector.<int>;
    var loadedCount:int;
    var initVector:Boolean;
    var onLoadProgress:Boolean;
    var paused:Boolean;
    var hasWindow:Boolean;
    var gameAtlas:TextureAtlas;
    var _background:cSprite;
    var allButtonAr:Vector.<TNonStateButton>;
    var targetBoard:cSprite;
    var pauseButton:TNonStateButton;
    var soundButton:TNonStateButton;
    var musicButton:TNonStateButton;
    var mute_icon:cSprite;
    var mute_icon_music:cSprite;
    var timerBG:cSprite;
    var timerFill:cSprite;
    var timerGlass:cSprite;
    var timerSprite:Sprite;
    var customer:Vector.<TCustomer>;
    var drinkMachine:TNonStateButton;
    var drink:cSprite;
    var friesMachine:TNonStateButton;
    var fries:cSprite;
    var availableTypeAr:Array;
    var touchFirstTile:Boolean;
    var tileTypeAr:Vector.<int>;
    var moveTile:Boolean;
    var noMoreMoves:Boolean;
    const gravity:Number = 0.2;
    var chainCombo:int;
    var cookiesCountOnBoard:int;
    var efekAsap:MovieClip;
    var efekStar:MovieClip;
    var efekApi:MovieClip;
    var efekAsapAr:Vector.<MovieClip>;
    var efekStarAr:Vector.<MovieClip>;
    var efekApiAr:Vector.<MovieClip>;
    var removedTileCountCookies:int;
    var removedTileCountJuice:int;
    var removedTileCountFries:int;
    var removedTileTorch:int;
    var cookiesTarget:int;
    var cookiesEffect:int;
    var juiceTarget:int;
    var juiceEffect:int;
    var friesTarget:int;
    var friesEffect:int;
    var torchTarget:int;
    var levelComplete:Boolean;
    var isSwapping:Boolean;
    var gameover:Boolean;
    var matched:Boolean;
    var swapBack:Boolean;
    var highlightPossibleTile:Boolean;
    var firstTile:Match3Object;
    var lastTile:Match3Object;
    var idleDuration:int;
    var maxFrozenTile:int;
    var currentFrozenTile:int;
    var noPossibleMoveText:ExtendTextField;
    var outOfMoveText:ExtendTextField;
    var levelCompleteText:ExtendTextField;
    var goalExpertText:ExtendTextField;
    var _blackScreen:Quad;
    var juicerLayer:cSprite;
    var fryerLayer:cSprite;
    var juicerPercentage:ExtendTextField;
    var fryerPercentage:ExtendTextField;
    var targetInfoText:ExtendTextField;
    var targetStateText:ExtendTextField;
    var touchCookies:Boolean;
    var choosenJuice:Boolean;
    var choosenFries:Boolean;
    var frozenTile:FrozenTile;
    var frozenTileAr:Vector.<FrozenTile>;
    var tileAr:Vector.<Vector.<Match3Object>>;
    var tweenTileAr:Vector.<Match3Object>;
    var customerInUse:Vector.<int>;
    var customerOnQueue:Vector.<int>;
    var servedCustomerCount:int;
    var missedCustomerCount:int;
    var timer:int;
    var totalTime:int;
    var goalTarget:int;
    var expertTarget:int;
    var VIPServeBonus:int;
    var earnedMoneyText:ExtendTextField;
    var chainBonusText:ExtendTextField;
    var chainBonusTextAr:Vector.<ExtendTextField>;
    var earnedMoneyTextAr:Vector.<ExtendTextField>;
    var coins:cSprite;
    var coinsAr:Vector.<cSprite>;
    var dishes:cSprite;
    var dishesAr:Vector.<cSprite>;
    var largeText:ExtendTextField;
    var westernDishName:Vector.<String>;
    var chineseDishName:Vector.<String>;
    var mideastDishName:Vector.<String>;
    var touchQuad:Quad;
    var timesUp:Boolean;
    var cheatActive:Boolean;
    var tmpDrink:cSprite;
    var tmpFries:cSprite;
    var blurFilter:BlurFilter;
    var coinFilter:BlurFilter;
    var escCounter:int;
    var lastCol:int;
    var lastRow:int;
    var calledShowWindow:Boolean;
    var firstCust:Boolean;
    var dispatchHighlightPosition:int;
    var highlightX:int;
    var highlightY:int;

    function GameScreen():* {
        this.loadedFiles = [];
        this.loadQueue = new Vector.<String>();
        this.loadQueueType = new Vector.<int>();
        this.availableTypeAr = [];
        super();
    }

    override function Start():void {
        i = 0;
        super.Start();
        assetPath.start(this._global.graphicsPath, this._global.soundsPath);
        this._global.currentSceneID = this._global.GAMESCREEN_ID;
        if (this.initVector) goto L647;
        this.allButtonAr = new Vector.<TNonStateButton>();
        this.customer = new Vector.<TCustomer>();
        this.tileTypeAr = new Vector.<int>();
        this.tileAr = new Vector.<Vector.<Match3Object>>();
        this.tweenTileAr = new Vector.<Match3Object>();
        this.earnedMoneyTextAr = new Vector.<ExtendTextField>();
        this.chainBonusTextAr = new Vector.<ExtendTextField>();
        this.frozenTileAr = new Vector.<FrozenTile>();
        this.efekApiAr = new Vector.<MovieClip>();
        this.efekAsapAr = new Vector.<MovieClip>();
        this.efekStarAr = new Vector.<MovieClip>();
        var $t0 = new Vector.<int>(12);
        $t0[0] = 7;
        $t0[1] = 1;
        $t0[2] = 4;
        $t0[3] = 3;
        $t0[4] = 8;
        $t0[5] = 10;
        $t0[6] = 5;
        $t0[7] = 11;
        $t0[8] = 6;
        $t0[9] = 12;
        $t0[10] = 2;
        $t0[11] = 9;
        this.customerInUse = $t0;
        this.customerOnQueue = new Vector.<int>();
        this.coinsAr = new Vector.<cSprite>();
        this.dishesAr = new Vector.<cSprite>();
        var $t1 = new Vector.<String>(6);
        $t1[0] = "veggie_croissant";
        $t1[1] = "deluxe_chicken_croissant";
        $t1[2] = "french_BBQ_combo";
        $t1[3] = "beef_steak";
        $t1[4] = "smokey_salmon";
        $t1[5] = "premium_salmon_steak";
        this.westernDishName = $t1;
        var $t2 = new Vector.<String>(6);
        $t2[0] = "asian_fried_rice";
        $t2[1] = "thai_fried_rice";
        $t2[2] = "kalasan_fried_chicken";
        $t2[3] = "chinese_pork_special";
        $t2[4] = "fine_imperial_cuisine";
        $t2[5] = "deluxe_BBQ_combo";
        this.chineseDishName = $t2;
        var $t3 = new Vector.<String>(6);
        $t3[0] = "doner_kebab";
        $t3[1] = "gyro_style";
        $t3[2] = "tomato_domas";
        $t3[3] = "tuscan_humus";
        $t3[4] = "baklava";
        $t3[5] = "crown_lamb_roast";
        this.mideastDishName = $t3;
        i = 0;
        goto L632;
      L603:
        this.tileAr.push(new Vector.<Match3Object>());
        i = i + 1;
      L632:
        if (i < 8) goto L603;
        this.initVector = true;
      L647:
        this._global.assetManager.enqueue(this._global.graphicsPath + "game.ean", this._global.graphicsPath + "game.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + "match3_BG.eag");
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
        if (!this.hasWindow) goto L19;
        return;
      L19:
        ScoreManager.updateDisplayedScore();
        if (!this.paused) goto L35;
        return;
      L35:
        if (!this.gameover) goto L44;
        return;
      L44:
        if (!this._global.tutorial.button.visible) goto L62;
        return;
      L62:
        if (this.noMoreMoves) goto L75;
        this.movePieces();
      L75:
        var _local1 = 0;
        goto L171;
      L82:
        if (this.customer[_local1].onTween) goto L169;
        if (!(this.customer[_local1].x > -10)) goto L169;
        this.customer[_local1].dePatience();
        if (!(this.customer[_local1].patienceSprite.clipRect.x > 0)) goto L169;
        this.customer[_local1].patienceSprite.clipRect.x = 0;
      L169:
        _local1++;
      L171:
        if (_local1 < 3) goto L82;
        var $t0 = !this.timesUp;
        $s0 = $t0;
        if (!$t0) goto L203;
        $s0 = !this._global.tutorial.textBox.visible;
      L203:
        if (!$s0) goto L280;
        var _local2 = this;
        var _local3 = this.timer - 1;
        _local2.timer = _local3;
        if (!(this.timer <= 0)) goto L239;
        this.timesUp = true;
      L239:
        this.timerSprite.clipRect.x = this.timer / this.totalTime * this.timerSprite.clipRect.width - this.timerSprite.clipRect.width;
      L280:
        if (this.highlightPossibleTile) goto L332;
        _local2 = this;
        _local3 = this.idleDuration + 1;
        _local2.idleDuration = _local3;
        if (!(this.idleDuration > 300)) goto L332;
        this.highlightPossible();
        this.highlightPossibleTile = true;
        this.idleDuration = 0;
      L332:
    }

    function movePieces():void {
        var _local3 = 0;
        var _local1 = false;
        if (this.moveTile) goto L16;
        return;
      L16:
        var _local2 = 0;
        goto L512;
      L23:
        _local3 = 0;
        goto L503;
      L31:
        if (this.tileAr[_local3][_local2].removed != false) goto L501;
        if (!(this.tileAr[_local3][_local2].y < this.tileAr[_local3][_local2].row * 68 + 73.5)) goto L468;
        if (this.tileAr[_local3][_local2].isSwapping) goto L127;
        this.tileAr[_local3][_local2].isDropping = true;
      L127:
        if (!(this.tileAr[_local3][_local2].y + this.tileAr[_local3][_local2].vy * 2 < this.tileAr[_local3][_local2].row * 68 + 73.5)) goto L277;
        this.tileAr[_local3][_local2].vy = this.tileAr[_local3][_local2].vy + this.gravity * 2;
        this.tileAr[_local3][_local2].y = this.tileAr[_local3][_local2].y + this.tileAr[_local3][_local2].vy * 2;
        goto L329;
      L277:
        this.tileAr[_local3][_local2].isDropping = false;
        this.tileAr[_local3][_local2].y = this.tileAr[_local3][_local2].row * 68 + 73.5;
      L329:
        if (!this.frozenTileAr[this.tileAr[_local3][_local2].frozenTileIdx].visible) goto L462;
        var $t0 = this.frozenTileAr[this.tileAr[_local3][_local2].frozenTileIdx].attachedCol == _local3;
        $s0 = $t0;
        if (!$t0) goto L418;
        $s0 = this.frozenTileAr[this.tileAr[_local3][_local2].frozenTileIdx].attachedRow == _local2;
      L418:
        if (!$s0) goto L462;
        this.frozenTileAr[this.tileAr[_local3][_local2].frozenTileIdx].y = this.tileAr[_local3][_local2].y;
      L462:
        _local1 = true;
        goto L501;
      L468:
        this.tileAr[_local3][_local2].vy = 0;
        this.tileAr[_local3][_local2].isDropping = false;
      L501:
        _local3++;
      L503:
        if (_local3 < 8) goto L31;
        _local2++;
      L512:
        if (_local2 < 8) goto L23;
        var $t1 = !_local1;
        $s0 = $t1;
        if (!$t1) goto L532;
        $s0 = Boolean(this.moveTile);
      L532:
        if (!$s0) goto L556;
        this.moveTile = false;
        this.isSwapping = false;
        this.findAndRemoveOneMatch();
        this.setIdx();
      L556:
    }

    function setIdx():void {
        var _local1 = 0;
        var _local3 = 0;
        var _local2 = 0;
        goto L85;
      L15:
        _local3 = 0;
        goto L76;
      L23:
        this.tileAr[_local2][_local3].idx = _local1;
        this.tileAr[_local2][_local3].col = _local2;
        this.tileAr[_local2][_local3].row = _local3;
        _local1++;
        _local3++;
      L76:
        if (_local3 < 8) goto L23;
        _local2++;
      L85:
        if (_local2 < 8) goto L15;
    }

    override function Kill():void {
        var _local1 = 0;
        var _local2 = 0;
        var _local4 = 0;
        assetPath.reset();
        this.blurFilter.dispose();
        this.blurFilter = null;
        this.coinFilter.dispose();
        this.coinFilter = null;
        this.touchQuad.removeEventListener(TouchEvent.TOUCH, this.touchTile);
        this.touchQuad.removeFromParent(true);
        this.touchQuad = null;
        _local2 = int(this.customer.length);
        _local1 = 0;
        goto L285;
      L93:
        this.customer[_local1].Kill();
        this.customer[_local1].removeFromParent(true);
        this.customer[_local1] = null;
        this.coinsAr[_local1].removeEventListener(TouchEvent.TOUCH, this.touchCoins);
        this.coinsAr[_local1].removeFromParent(true);
        this.coinsAr[_local1] = null;
        TweenLite.killTweensOf(this.dishesAr[_local1]);
        this.dishesAr[_local1].removeFromParent(true);
        this.dishesAr[_local1] = null;
        TweenLite.killTweensOf(this.earnedMoneyTextAr[_local1]);
        this.earnedMoneyTextAr[_local1].removeFromParent(true);
        this.earnedMoneyTextAr[_local1] = null;
        TweenLite.killTweensOf(this.chainBonusTextAr[_local1]);
        this.chainBonusTextAr[_local1].removeFromParent(true);
        this.chainBonusTextAr[_local1] = null;
        _local1++;
      L285:
        if (_local1 < _local2) goto L93;
        this.coinsAr.length = 0;
        this.dishesAr.length = 0;
        this.earnedMoneyTextAr.length = 0;
        this.chainBonusTextAr.length = 0;
        var _local3 = 0;
        goto L421;
      L334:
        _local4 = 0;
        goto L398;
      L343:
        TweenLite.killTweensOf(this.tileAr[_local3][_local4]);
        this.tileAr[_local3][_local4].removeFromParent(true);
        this.tileAr[_local3][_local4] = null;
        _local4++;
      L398:
        if (_local4 < 8) goto L343;
        this.tileAr[_local3].length = 0;
        _local3++;
      L421:
        if (_local3 < 8) goto L334;
        _local1 = 0;
        goto L475;
      L435:
        TweenLite.killTweensOf(this.tweenTileAr[_local1]);
        this.tweenTileAr[_local1].removeFromParent(true);
        this.tweenTileAr[_local1] = null;
        _local1++;
      L475:
        if (_local1 < this.tweenTileAr.length) goto L435;
        this.tweenTileAr.length = 0;
        _local2 = int(this.frozenTileAr.length);
        _local1 = 0;
        goto L537;
      L512:
        this.frozenTileAr[_local1].removeFromParent(true);
        this.frozenTileAr[_local1] = null;
        _local1++;
      L537:
        if (_local1 < _local2) goto L512;
        this.frozenTileAr.length = 0;
        _local2 = int(this.efekApiAr.length);
        _local1 = 0;
        goto L691;
      L568:
        Starling.juggler.remove(this.efekApiAr[_local1]);
        Starling.juggler.remove(this.efekAsapAr[_local1]);
        Starling.juggler.remove(this.efekStarAr[_local1]);
        this.efekApiAr[_local1].removeFromParent(true);
        this.efekAsapAr[_local1].removeFromParent(true);
        this.efekStarAr[_local1].removeFromParent(true);
        this.efekApiAr[_local1] = null;
        this.efekAsapAr[_local1] = null;
        this.efekStarAr[_local1] = null;
        _local1++;
      L691:
        if (_local1 < _local2) goto L568;
        this.efekApiAr.length = 0;
        this.efekAsapAr.length = 0;
        this.efekStarAr.length = 0;
        this._background.removeFromParent(true);
        this._background = null;
        this._blackScreen.removeFromParent(true);
        this._blackScreen = null;
        this.targetBoard.removeFromParent(true);
        this.targetBoard = null;
        this.pauseButton.removeFromParent(true);
        this.pauseButton = null;
        this.soundButton.removeFromParent(true);
        this.soundButton = null;
        this.musicButton.removeFromParent(true);
        this.musicButton = null;
        this.mute_icon.removeFromParent(true);
        this.mute_icon = null;
        this.mute_icon_music.removeFromParent(true);
        this.mute_icon_music = null;
        this.timerBG.removeFromParent(true);
        this.timerBG = null;
        this.timerFill.removeFromParent(true);
        this.timerFill = null;
        this.timerSprite.removeFromParent(true);
        this.timerSprite = null;
        this.timerGlass.removeFromParent(true);
        this.timerGlass = null;
        this.drinkMachine.removeFromParent(true);
        this.drinkMachine = null;
        this.drink.removeFromParent(true);
        this.drink = null;
        this.tmpDrink.removeFromParent(true);
        this.tmpDrink = null;
        this.friesMachine.removeFromParent(true);
        this.friesMachine = null;
        this.fries.removeFromParent(true);
        this.fries = null;
        this.tmpFries.removeFromParent(true);
        this.tmpFries = null;
        this.noPossibleMoveText.removeFromParent(true);
        this.noPossibleMoveText = null;
        this.outOfMoveText.removeFromParent(true);
        this.outOfMoveText = null;
        this.levelCompleteText.removeFromParent(true);
        this.levelCompleteText = null;
        this.coins.removeFromParent(true);
        this.coins = null;
        this.dishes.removeFromParent(true);
        this.dishes = null;
        this.earnedMoneyText.removeFromParent(true);
        this.earnedMoneyText = null;
        this.goalExpertText.removeFromParent(true);
        this.goalExpertText = null;
        this.juicerLayer.removeFromParent(true);
        this.juicerLayer = null;
        this.juicerPercentage.removeFromParent(true);
        this.juicerPercentage = null;
        this.fryerLayer.removeFromParent(true);
        this.fryerLayer = null;
        this.fryerPercentage.removeFromParent(true);
        this.fryerPercentage = null;
        this.targetInfoText.removeFromParent(true);
        this.targetInfoText = null;
        this.targetStateText.removeFromParent(true);
        this.targetStateText = null;
        this.frozenTile.removeFromParent(true);
        this.frozenTile = null;
        this.efekApi.removeFromParent(true);
        this.efekApi = null;
        this.efekAsap.removeFromParent(true);
        this.efekAsap = null;
        this.efekStar.removeFromParent(true);
        this.efekStar = null;
        this.largeText.removeFromParent(true);
        this.largeText = null;
        this.gameAtlas.dispose();
        this.gameAtlas = null;
        ScoreManager.Kill();
        TGlobalClass._windowDisplay.hideAllWindows(this._global.GAMESCREEN_ID);
        TGlobalClass._windowDisplay.kill(this._global.GAMESCREEN_ID);
        this._global.assetManager.purge();
        _local1 = 0;
        goto L1303;
      L1300:
        _local1++;
      L1303:
        if (_local1 < this._global._gameS.numChildren) goto L1300;
        ready = false;
    }

    function initialize():void {
        var _local2 = 0;
        var _local3 = 0;
        var _local4 = 0;
        var _local5 = null;
        var _local7 = null;
        var _local8 = 0;
        var _local9 = 0;
        var _local10 = 0;
        var _local11 = 0;
        this._global.ReportGameState(this._global.GAMESTATE_PLAYING);
        var _local1 = new Vector.<int>();
        System.pauseForGCIfCollectionImminent(0);
        System.gc();
        this.blurFilter = BlurFilter.createGlow(15988227, 1, 0.5, 0.2);
        this.coinFilter = BlurFilter.createGlow(65322);
        this.timesUp = false;
        if ((TGlobalClass.currentPlayedLevel + 1) % TGlobalClass.MATCH_3_SPECIAL_EVERY != 0) goto L148;
        TGlobalClass.isPlayingSpecialLevel = true;
      L148:
        $s0 = GALib;
        $s1 = GALib.EVENT_START_LEVEL_STORY;
        $s2 = TGlobalClass.currentPlayedLevel + 1;
        $s3 = "MATCH 3";
        if (!TGlobalClass.isPlayingSpecialLevel) goto L187;
        $s4 = " SPECIAL";
        goto L190;
      L187:
        $s4 = "";
      L190:
        $s0.TrackEvent($s1, $s2, $s3 + $s4);
        if (TGlobalClass.isPlayingSpecialLevel) goto L268;
        if (TGlobalClass.musicPlayingIdx == 2) goto L264;
        $s0 = this._global._soundAsset;
        if (!(TGlobalClass.RandomRange(1, 2) == 1)) goto L249;
        $s1 = "fun-time";
        goto L252;
      L249:
        $s1 = "sunny-advert";
      L252:
        $s0.playMusic($s1);
        TGlobalClass.musicPlayingIdx = 2;
      L264:
        goto L302;
      L268:
        if (TGlobalClass.musicPlayingIdx == 4) goto L302;
        this._global._soundAsset.playMusic("upbeat-feeling_loop");
        TGlobalClass.musicPlayingIdx = 4;
      L302:
        TGlobalClass.customerFocus = 0;
        this.gameAtlas = this._global.assetManager.getTextureAtlas("game");
        this.checkDishPriceAndAvaiblity(4);
        this.availableTypeAr = [];
        if (TGlobalClass.currentRegionDish != ConstClass.WESTERN_DISH) goto L381;
        this.availableTypeAr.push(1, 9, 7, 2, 4, 3);
        goto L457;
      L381:
        if (TGlobalClass.currentRegionDish != ConstClass.CHINESE_DISH) goto L421;
        this.availableTypeAr.push(5, 6, 8, 2, 4, 3);
        goto L457;
      L421:
        if (TGlobalClass.currentRegionDish != ConstClass.MID_EAST_DISH) goto L457;
        this.availableTypeAr.push(11, 12, 13, 2, 4, 5);
      L457:
        if (!(TSavegame.currentPlayer.passedDays <= 1)) goto L494;
        this.availableTypeAr = [];
        this.availableTypeAr.push(1, 9, 7, 2);
      L494:
        var $t0 = TSavegame.currentPlayer.passedDays > 1;
        $s0 = $t0;
        if (!$t0) goto L524;
        $s0 = TSavegame.currentPlayer.passedDays <= 5;
      L524:
        if (!$s0) goto L940;
        if (TGlobalClass.currentRegionDish != ConstClass.WESTERN_DISH) goto L645;
        _local2 = 0;
        goto L627;
      L551:
        _local7 = this._global.dish.getDishIngredientType(TGlobalClass.dishRegionWestern[_local2]);
        _local3 = 0;
        goto L615;
      L584:
        if (_local1.indexOf(_local7[_local3]) != -1) goto L613;
        _local1.push(_local7[_local3]);
      L613:
        _local3++;
      L615:
        if (_local3 < _local7.length) goto L584;
        _local2++;
      L627:
        if (_local2 < TGlobalClass.dishRegionWestern.length) goto L551;
        goto L875;
      L645:
        if (TGlobalClass.currentRegionDish != ConstClass.CHINESE_DISH) goto L762;
        _local2 = 0;
        goto L744;
      L668:
        _local7 = this._global.dish.getDishIngredientType(TGlobalClass.dishRegionChinese[_local2]);
        _local3 = 0;
        goto L732;
      L701:
        if (_local1.indexOf(_local7[_local3]) != -1) goto L730;
        _local1.push(_local7[_local3]);
      L730:
        _local3++;
      L732:
        if (_local3 < _local7.length) goto L701;
        _local2++;
      L744:
        if (_local2 < TGlobalClass.dishRegionChinese.length) goto L668;
        goto L875;
      L762:
        if (TGlobalClass.currentRegionDish != ConstClass.MID_EAST_DISH) goto L875;
        _local2 = 0;
        goto L861;
      L785:
        _local7 = this._global.dish.getDishIngredientType(TGlobalClass.dishRegionMidEast[_local2]);
        _local3 = 0;
        goto L849;
      L818:
        if (_local1.indexOf(_local7[_local3]) != -1) goto L847;
        _local1.push(_local7[_local3]);
      L847:
        _local3++;
      L849:
        if (_local3 < _local7.length) goto L818;
        _local2++;
      L861:
        if (_local2 < TGlobalClass.dishRegionMidEast.length) goto L785;
      L875:
        _local2 = 0;
        goto L928;
      L882:
        if (_local1.indexOf(this.availableTypeAr[_local2]) != -1) goto L926;
        if (!(this.availableTypeAr.length > 4)) goto L926;
        this.availableTypeAr.splice(_local2, 1);
      L926:
        _local2++;
      L928:
        if (_local2 < this.availableTypeAr.length) goto L882;
      L940:
        if (!(TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_COOKIES_AT)) goto L1003;
        if (TSavegame.currentPlayer.hardwareLevel[Dish.COOKIES] != 0) goto L1003;
        TSavegame.currentPlayer.hardwareLevel[Dish.COOKIES] = 1;
      L1003:
        var $t1 = new Vector.<int>(12);
        $t1[0] = 7;
        $t1[1] = 1;
        $t1[2] = 4;
        $t1[3] = 3;
        $t1[4] = 8;
        $t1[5] = 10;
        $t1[6] = 5;
        $t1[7] = 11;
        $t1[8] = 6;
        $t1[9] = 12;
        $t1[10] = 2;
        $t1[11] = 9;
        this.customerInUse = $t1;
        TGlobalClass.tempCustUse = new Vector.<int>();
        TGlobalClass.tempCustUse = this.customerInUse.slice(0, TSavegame.currentPlayer.lastCustIdx + 1);
        this.customerOnQueue.length = 0;
        TGlobalClass.totalCust = 12;
        TGlobalClass.currentCust = 0;
        TGlobalClass.VIPServedRating = 0;
        TGlobalClass.juiceServed = 0;
        TGlobalClass.friesServed = 0;
        TGlobalClass.cookiesUsed = 0;
        ScoreManager.score = "a";
        ScoreManager.totalScore = "a";
        this.servedCustomerCount = 0;
        this.missedCustomerCount = 0;
        this.VIPServeBonus = 0;
        if (!(TGlobalClass.currentPlayedLevel < 30)) goto L1275;
        this.maxFrozenTile = 2;
        goto L1347;
      L1275:
        if (!(TGlobalClass.currentPlayedLevel < 35)) goto L1297;
        this.maxFrozenTile = 3;
        goto L1347;
      L1297:
        if (!(TGlobalClass.currentPlayedLevel < 40)) goto L1319;
        this.maxFrozenTile = 4;
        goto L1347;
      L1319:
        if (!(TGlobalClass.currentPlayedLevel < 45)) goto L1341;
        this.maxFrozenTile = 5;
        goto L1347;
      L1341:
        this.maxFrozenTile = 6;
      L1347:
        this.currentFrozenTile = 0;
        this.cookiesTarget = this._global.dish.getHardwareDefaultTarget(Dish.COOKIES) - (TSavegame.currentPlayer.hardwareLevel[Dish.COOKIES] - 1) * this._global.dish.getHardwareReduction(Dish.COOKIES);
        this.juiceTarget = this._global.dish.getHardwareDefaultTarget(Dish.JUICER) - (TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] - 1) * this._global.dish.getHardwareReduction(Dish.JUICER);
        this.friesTarget = this._global.dish.getHardwareDefaultTarget(Dish.FRYER) - (TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] - 1) * this._global.dish.getHardwareReduction(Dish.FRYER);
        this.cookiesEffect = this._global.dish.getHardwareDefaultPrice(Dish.COOKIES) + (TSavegame.currentPlayer.hardwareLevel[Dish.COOKIES] - 1) * this._global.dish.getHardwarePriceIncrease(Dish.COOKIES);
        this.juiceEffect = this._global.dish.getHardwareDefaultPrice(Dish.JUICER) + (TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] - 1) * this._global.dish.getHardwarePriceIncrease(Dish.JUICER);
        this.friesEffect = this._global.dish.getHardwareDefaultPrice(Dish.FRYER) + (TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] - 1) * this._global.dish.getHardwarePriceIncrease(Dish.FRYER);
        this.removedTileCountCookies = 0;
        this.removedTileCountFries = 0;
        this.removedTileCountJuice = 0;
        this.removedTileTorch = 0;
        this.torchTarget = TGlobalClass.TILE_NEEDED_FOR_TORCH;
        this.timer = TGlobalClass.levelSetXML.day[TGlobalClass.currentPlayedLevel].time * 30;
        this.totalTime = this.timer;
        this.goalTarget = TGlobalClass.levelSetXML.day[TGlobalClass.currentPlayedLevel].goal;
        this.expertTarget = TGlobalClass.levelSetXML.day[TGlobalClass.currentPlayedLevel].expert;
        this.gameover = false;
        this.levelComplete = false;
        this._background = new cSprite(this._global.assetManager.getTexture("match3_BG"));
        this._background.blendMode = BlendMode.NONE;
        this._background.touchable = false;
        this._global._gameS.addChild(this._background);
        this.targetBoard = new cSprite(this.gameAtlas.getTexture("score_board"));
        this.targetBoard.x = 140;
        this.targetBoard.touchable = false;
        this._global._gameS.addChild(this.targetBoard);
        this.pauseButton = new TNonStateButton(this.gameAtlas.getTexture("pause_button"));
        var _local12 = 0;
        this.pauseButton.y = 0;
        this.pauseButton.x = _local12;
        this.pauseButton.idx = this.PAUSE_BUTTON;
        this.pauseButton.scaleX = 0.7;
        this.pauseButton.scaleY = 0.7;
        this._global._gameS.addChild(this.pauseButton);
        this.allButtonAr.push(this.pauseButton);
        this.soundButton = new TNonStateButton(this.gameAtlas.getTexture("sound_button"));
        this.soundButton.x = 45;
        this.soundButton.y = 0;
        this.soundButton.idx = this.SOUND_BUTTON;
        this.soundButton.scaleX = 0.7;
        this.soundButton.scaleY = 0.7;
        this._global._gameS.addChild(this.soundButton);
        this.allButtonAr.push(this.soundButton);
        this.mute_icon = new cSprite(this.gameAtlas.getTexture("mute_icon"));
        this.mute_icon.x = this.soundButton.x + 18 - this.soundButton.pivotX;
        this.mute_icon.y = this.soundButton.y + 14 - this.soundButton.pivotY;
        this.mute_icon.touchable = false;
        this._global._gameS.addChild(this.mute_icon);
        this.mute_icon.visible = this._global.MuteSound;
        this.musicButton = new TNonStateButton(TGlobalClass._windowDisplay.windowsAtlas.getTexture("music_icon"));
        this.musicButton.x = 90;
        this.musicButton.y = 0;
        this.musicButton.idx = this.MUSIC_BUTTON;
        this.musicButton.scaleX = 0.7;
        this.musicButton.scaleY = 0.7;
        this._global._gameS.addChild(this.musicButton);
        this.allButtonAr.push(this.musicButton);
        this.mute_icon_music = new cSprite(this.gameAtlas.getTexture("mute_icon"));
        this.mute_icon_music.x = this.musicButton.x - this.mute_icon_music.pivotX - 10;
        this.mute_icon_music.y = this.musicButton.y - this.mute_icon_music.pivotY - 14;
        this.mute_icon_music.touchable = false;
        this._global._gameS.addChild(this.mute_icon_music);
        this.mute_icon_music.visible = this._global.MuteMusic;
        this.timerBG = new cSprite(this.gameAtlas.getTexture("timer_BG"));
        this.timerBG.x = 550;
        this.timerBG.y = 7;
        this.timerBG.touchable = false;
        this._global._gameS.addChild(this.timerBG);
        this.timerFill = new cSprite(this.gameAtlas.getTexture("timer_fill"));
        this.timerSprite = new Sprite();
        this._global._gameS.addChild(this.timerSprite);
        this.timerSprite.addChild(this.timerFill);
        this.timerSprite.touchable = false;
        this.timerSprite.x = 623;
        this.timerSprite.y = 11.2;
        this.timerSprite.clipRect = new Rectangle(0, 0, this.timerFill.width, this.timerFill.height);
        this.timerGlass = new cSprite(this.gameAtlas.getTexture("timer_glass"));
        this.timerGlass.x = 600;
        this.timerGlass.touchable = false;
        this._global._gameS.addChild(this.timerGlass);
        _local2 = 0;
        goto L3318;
      L2713:
        this.dishes = new cSprite(this.gameAtlas.getTexture("veggie_croissant"));
        this.dishes.pivotY = this.dishes.height;
        this.dishes.pivotX = this.dishes.width / 2;
        this.dishes.x = 375;
        this.dishes.y = _local2 * 117 + 140;
        this.dishes.visible = false;
        this._global._gameS.addChild(this.dishes);
        this.dishesAr.push(this.dishes);
        this.customer[_local2] = new TCustomer();
        this.customer[_local2].y = _local2 * 118 + 63;
        this.customer[_local2].idx = _local2;
        this._global._gameS.addChild(this.customer[_local2]);
        this.coins = new cSprite(this.gameAtlas.getTexture("coins"));
        this.coins.pivotX = this.coins.width / 2;
        this.coins.pivotY = this.coins.height / 2;
        this.coins.x = 120 + this.coins.pivotX;
        this.coins.y = _local2 * 117 + 90 + this.coins.pivotY;
        _local12 = 0;
        this.coins.scaleY = 0;
        this.coins.scaleX = _local12;
        this.coins.paddingX = 80;
        this.coins.paddingY = 20;
        this.coins.visible = false;
        this.coins.useHandCursor = true;
        this.coins.addEventListener(TouchEvent.TOUCH, this.touchCoins);
        this._global._gameS.addChild(this.coins);
        this.coinsAr.push(this.coins);
        this.earnedMoneyText = new ExtendTextField(250, 70, "", "KomikandyGold", 15, 16777215);
        this.earnedMoneyText.pivotX = 75;
        this.earnedMoneyText.pivotY = 35;
        this.earnedMoneyText.touchable = false;
        this.earnedMoneyText.visible = false;
        this.earnedMoneyText.x = this.coins.x;
        this.earnedMoneyText.y = this.coins.y;
        this._global._gameS.addChild(this.earnedMoneyText);
        this.earnedMoneyTextAr.push(this.earnedMoneyText);
        this.chainBonusText = new ExtendTextField(150, 50, "", "KomikandyGold", 15, 16777215);
        this.chainBonusText.pivotX = 75;
        this.chainBonusText.pivotY = 25;
        this.chainBonusText.touchable = false;
        this.chainBonusText.visible = false;
        this.chainBonusText.x = this.coins.x;
        this.chainBonusText.y = this.coins.y;
        this._global._gameS.addChild(this.chainBonusText);
        this.chainBonusTextAr.push(this.chainBonusText);
        _local2++;
      L3318:
        if (_local2 < 3) goto L2713;
        ScoreManager.initScore(255, 0);
        this._global._gameS.addChild(ScoreManager.scoreDisplayText);
        this.targetInfoText = new ExtendTextField(150, 50, "0/" + this.goalTarget, "jenna_sue", 35, 16777215);
        this.targetInfoText.x = 138;
        this.targetInfoText.hAlign = "right";
        this.targetInfoText.touchable = false;
        this._global._gameS.addChild(this.targetInfoText);
        this.targetStateText = new ExtendTextField(150, 50, "Goal:", "jenna_sue", 30, 16777215);
        this.targetStateText.x = 145;
        this.targetStateText.hAlign = "left";
        this.targetStateText.touchable = false;
        this._global._gameS.addChild(this.targetStateText);
        if (!TGlobalClass.isPlayingSpecialLevel) goto L3525;
        this.targetStateText.text = "";
        this.targetInfoText.text = "0 / 1 food critic";
      L3525:
        ScoreManager.scoreDisplayText.text = "$ 0";
        this.drinkMachine = new TNonStateButton(this.gameAtlas.getTexture("juicer"));
        this.drinkMachine.x = 0;
        this.drinkMachine.y = 400;
        this.drinkMachine.idx = this.JUICER_ID;
        this._global._gameS.addChild(this.drinkMachine);
        this.allButtonAr.push(this.drinkMachine);
        this.drinkMachine.addEventListener(TouchEvent.TOUCH, this.moveDrink);
        this.juicerLayer = new cSprite(this.gameAtlas.getTexture("juicer_layer"));
        this.juicerLayer.x = 0;
        this.juicerLayer.y = 395;
        this._global._gameS.addChild(this.juicerLayer);
        this.drink = new cSprite(this.gameAtlas.getTexture("juice"));
        this.drink.alignPivot();
        this.drink.x = 80 + this.drink.pivotX;
        this.drink.y = 500 + this.drink.pivotY;
        this.drink.visible = false;
        this.drink.touchable = false;
        this._global._gameS.addChild(this.drink);
        this.tmpDrink = new cSprite(this.gameAtlas.getTexture("juice"));
        this.tmpDrink.alignPivot();
        this.tmpDrink.x = this.drink.x;
        this.tmpDrink.y = this.drink.y;
        this.tmpDrink.visible = false;
        this._global._gameS.addChild(this.tmpDrink);
        this.tmpDrink.addEventListener(TouchEvent.TOUCH, this.moveDrink);
        this.juicerPercentage = new ExtendTextField(150, 100, "0%", "KomikandyWhite", 25, 16777215);
        this.juicerPercentage.x = 30;
        this.juicerPercentage.y = 510;
        this.juicerPercentage.touchable = false;
        this._global._gameS.addChild(this.juicerPercentage);
        this.friesMachine = new TNonStateButton(this.gameAtlas.getTexture("fryer"));
        this.friesMachine.x = 160;
        this.friesMachine.y = 440;
        this.friesMachine.idx = this.FRYER_ID;
        this._global._gameS.addChild(this.friesMachine);
        this.allButtonAr.push(this.friesMachine);
        this.friesMachine.addEventListener(TouchEvent.TOUCH, this.moveFries);
        this.fryerLayer = new cSprite(this.gameAtlas.getTexture("fryer_layer"));
        this.fryerLayer.x = 160;
        this.fryerLayer.y = 440;
        this._global._gameS.addChild(this.fryerLayer);
        this.fries = new cSprite(this.gameAtlas.getTexture("fries"));
        this.fries.alignPivot();
        this.fries.x = 193 + this.fries.pivotX;
        this.fries.y = 490 + this.fries.pivotY;
        this.fries.visible = false;
        this.fries.touchable = false;
        this._global._gameS.addChild(this.fries);
        this.tmpFries = new cSprite(this.gameAtlas.getTexture("fries"));
        this.tmpFries.alignPivot();
        this.tmpFries.x = this.fries.x;
        this.tmpFries.y = this.fries.y;
        this.tmpFries.visible = false;
        this._global._gameS.addChild(this.tmpFries);
        this.tmpFries.addEventListener(TouchEvent.TOUCH, this.moveFries);
        this.fryerPercentage = new ExtendTextField(150, 100, "0%", "KomikandyWhite", 25, 16777215);
        this.fryerPercentage.x = 200;
        this.fryerPercentage.y = 510;
        this.fryerPercentage.touchable = false;
        this._global._gameS.addChild(this.fryerPercentage);
        if (TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] != 0) goto L4429;
        this.juicerLayer.visible = false;
        this.juicerPercentage.visible = false;
        this.drinkMachine.visible = false;
        this.drink.visible = false;
      L4429:
        if (TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] != 0) goto L4485;
        this.fryerLayer.visible = false;
        this.fryerPercentage.visible = false;
        this.fries.visible = false;
        this.friesMachine.visible = false;
      L4485:
        var _local6 = 0;
        goto L4645;
      L4493:
        _local8 = 0;
        goto L4635;
      L4502:
        var $t2 = new Match3Object(this.gameAtlas.getTexture("keju"));
        _local5 = $t2;
        $t2.pivotX = $t2.width / 2;
        _local5.pivotY = _local5.height / 2;
        _local5.x = _local6 * 68 + 494;
        _local5.y = _local8 * 68 + 73.5;
        _local5.col = _local6;
        _local5.row = _local8;
        _local5.idx = _local4;
        this.tileAr[_local6].push(_local5);
        _local5.touchable = false;
        this._global._gameS.addChild(_local5);
        _local4++;
        _local8++;
      L4635:
        if (_local8 < 8) goto L4502;
        _local6++;
      L4645:
        if (_local6 < 8) goto L4493;
        this.touchQuad = new Quad(544, 544);
        this.touchQuad.x = 457;
        this.touchQuad.y = 40;
        this.touchQuad.alpha = 0;
        this._global._gameS.addChild(this.touchQuad);
        this.touchQuad.useHandCursor = true;
        this.touchQuad.addEventListener(TouchEvent.TOUCH, this.touchTile);
        _local2 = 0;
        goto L4849;
      L4746:
        this.frozenTile = new FrozenTile(this.gameAtlas.getTexture("ice"));
        this.frozenTile.name = "frozen tile";
        this.frozenTile.alignPivot();
        this.frozenTile.x = -100;
        this.frozenTile.visible = false;
        this.frozenTile.touchable = false;
        this.frozenTile.idx = _local2;
        this._global._gameS.addChild(this.frozenTile);
        this.frozenTileAr.push(this.frozenTile);
        _local2++;
      L4849:
        if (_local2 < 20) goto L4746;
        _local2 = 0;
        goto L5118;
      L4863:
        this.efekApi = new MovieClip(this.gameAtlas.getTextures("api_"), 18);
        this.efekApi.alignPivot();
        this.efekApi.x = -100;
        this.efekApi.visible = false;
        this.efekApi.touchable = false;
        this._global._gameS.addChild(this.efekApi);
        this.efekApiAr.push(this.efekApi);
        this.efekAsap = new MovieClip(this.gameAtlas.getTextures("asap_"), 18);
        this.efekAsap.alignPivot();
        this.efekAsap.x = -100;
        this.efekAsap.visible = false;
        this.efekAsap.touchable = false;
        this._global._gameS.addChild(this.efekAsap);
        this.efekAsapAr.push(this.efekAsap);
        this.efekStar = new MovieClip(this.gameAtlas.getTextures("star_"), 18);
        this.efekStar.alignPivot();
        this.efekStar.x = -100;
        this.efekStar.visible = false;
        this.efekStar.touchable = false;
        this._global._gameS.addChild(this.efekStar);
        this.efekStarAr.push(this.efekStar);
        _local2++;
      L5118:
        if (_local2 < 20) goto L4863;
        _local2 = 0;
        goto L5238;
      L5132:
        var $t3 = new Match3Object(this.gameAtlas.getTexture("keju"));
        _local5 = $t3;
        $t3.pivotX = $t3.width / 2;
        _local5.pivotY = _local5.height / 2;
        _local5.x = 4294967146;
        _local5.y = 4294967146;
        _local5.touchable = false;
        _local5.visible = false;
        this.tweenTileAr.push(_local5);
        this._global._gameS.addChild(_local5);
        _local2++;
      L5238:
        if (_local2 < 25) goto L5132;
        goto L5291;
      L5249:
        this.shuffleTile();
        if (!(this.lookForMatches().length > 0)) goto L5273;
        goto L5291;
      L5273:
        if (this.lookForPossibles() != false) goto L5287;
        goto L5291;
      L5287:
        goto L5295;
      L5291:
        goto L5249;
      L5295:
        this.goalExpertText = new ExtendTextField(450, 150, "Goal!!!", "komikandy", 60, 65280);
        this.goalExpertText.visible = false;
        this.goalExpertText.touchable = false;
        this.goalExpertText.pivotX = 225;
        this.goalExpertText.pivotY = 75;
        this.goalExpertText.x = 512;
        this.goalExpertText.y = 300;
        this.goalExpertText.idx = 1;
        this._global._gameS.addChild(this.goalExpertText);
        if (!TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel]) goto L5457;
        this.targetStateText.text = "Expert:";
        this.targetInfoText.text = "$" + this.expertTarget;
        this.goalExpertText.idx = 2;
      L5457:
        this.largeText = new ExtendTextField(500, 400, "Awesome!", "KomikandyLarge", 50, 16776960);
        this.largeText.pivotX = 250;
        this.largeText.pivotY = 200;
        this.largeText.x = 730;
        this.largeText.y = 300;
        this.largeText.hAlign = "center";
        this.largeText.vAlign = "center";
        this.largeText.touchable = false;
        this.largeText.visible = false;
        this._global._gameS.addChild(this.largeText);
        this._blackScreen = new Quad(1024, 600, 0);
        this._blackScreen.alpha = 0.5;
        this._blackScreen.visible = false;
        this._global._gameS.addChild(this._blackScreen);
        this.outOfMoveText = new ExtendTextField(500, 200, "OUT OF MOVES!", "komikandy", 80, 16777215);
        this.outOfMoveText.pivotX = 250;
        this.outOfMoveText.pivotY = 100;
        this.outOfMoveText.x = 4294966796;
        this.outOfMoveText.y = 450;
        this.outOfMoveText.hAlign = "center";
        this.outOfMoveText.vAlign = "center";
        this.outOfMoveText.touchable = false;
        this.outOfMoveText.visible = false;
        this._global._gameS.addChild(this.outOfMoveText);
        this.noPossibleMoveText = new ExtendTextField(500, 200, "NO POSSIBLE MOVES!", "komikandy", 50, 16777215);
        this.noPossibleMoveText.pivotX = 250;
        this.noPossibleMoveText.pivotY = 100;
        this.noPossibleMoveText.x = 4294966796;
        this.noPossibleMoveText.y = 300;
        this.noPossibleMoveText.hAlign = "center";
        this.noPossibleMoveText.vAlign = "center";
        this.noPossibleMoveText.touchable = false;
        this.noPossibleMoveText.visible = false;
        this._global._gameS.addChild(this.noPossibleMoveText);
        this.levelCompleteText = new ExtendTextField(500, 200, "LEVEL COMPLETE!", "komikandy", 80, 16777215);
        this.levelCompleteText.pivotX = 250;
        this.levelCompleteText.pivotY = 100;
        this.levelCompleteText.x = 4294966796;
        this.levelCompleteText.y = 450;
        this.levelCompleteText.hAlign = "center";
        this.levelCompleteText.vAlign = "center";
        this.levelCompleteText.touchable = false;
        this.levelCompleteText.visible = false;
        this._global._gameS.addChild(this.levelCompleteText);
        TGlobalClass._windowDisplay.initializeWindow(this._global.GAMESCREEN_ID);
        this._global._gameS.setChildIndex(this._global.psCongrats, this._global._gameS.numChildren - 1);
        this._global.psCongrats.stop();
        Starling.juggler.add(this._global.psCongrats);
        _local2 = 0;
        goto L6083;
      L6059:
        this.allButtonAr[_local2].addEventListener(Event.TRIGGERED, this.touchButton);
        _local2++;
      L6083:
        if (_local2 < this.allButtonAr.length) goto L6059;
        if (TSavegame.currentPlayer.passedDays != TGlobalClass.currentPlayedLevel) goto L6126;
        TGlobalClass.currentNotifWindow = 0;
        goto L6158;
      L6126:
        TGlobalClass.currentNotifWindow = TGlobalClass.levelEventXML.day[TGlobalClass.currentPlayedLevel].window.length() - 1;
      L6158:
        if (!this._global.tutorial.checkTut(Tutorial.LEVEL_1, 0)) goto L6185;
        goto L7295;
      L6185:
        if (!this._global.tutorial.checkTut(Tutorial.LEVEL_5, 4)) goto L6212;
        goto L7295;
      L6212:
        if (!this._global.tutorial.checkTut(Tutorial.LEVEL_7, 6)) goto L6239;
        goto L7295;
      L6239:
        if (!this._global.tutorial.checkTut(Tutorial.LEVEL_8, 7)) goto L6266;
        goto L7295;
      L6266:
        if (!this._global.tutorial.checkTut(Tutorial.LEVEL_12, 11)) goto L6334;
        this.removedTileCountJuice = this.juiceTarget - 3;
        this.juicerPercentage.text = int(this.removedTileCountJuice / this.juiceTarget * 100) + "%";
        goto L7295;
      L6334:
        if (!this._global.tutorial.checkTut(Tutorial.LEVEL_13, 12)) goto L6361;
        goto L7295;
      L6361:
        if (!this._global.tutorial.checkTut(Tutorial.LEVEL_14, 13)) goto L6388;
        goto L7295;
      L6388:
        if (!this._global.tutorial.checkTut(Tutorial.LEVEL_16, 15)) goto L6415;
        goto L7295;
      L6415:
        if (!this._global.tutorial.checkTut(Tutorial.LEVEL_17, 16)) goto L6483;
        this.removedTileCountFries = this.friesTarget - 3;
        this.fryerPercentage.text = int(this.removedTileCountFries / this.friesTarget * 100) + "%";
        goto L7295;
      L6483:
        if (!this._global.tutorial.checkTut(Tutorial.LEVEL_19, 18)) goto L6510;
        goto L7295;
      L6510:
        if (!this._global.tutorial.checkTut(Tutorial.LEVEL_21, 20)) goto L6537;
        goto L7295;
      L6537:
        if (!this._global.tutorial.checkTut(Tutorial.LEVEL_22, 21)) goto L6564;
        goto L7295;
      L6564:
        if (!this._global.tutorial.checkTut(Tutorial.LEVEL_26, 25)) goto L6735;
        this.dispatchHighlightPosition = 3;
        this.highlightPossible();
        this.highlightPossibleTile = true;
        _local9 = 0;
        goto L6723;
      L6611:
        _local10 = 0;
        goto L6713;
      L6620:
        if (this.tileAr[_local9][_local10].onTween) goto L6711;
        TweenLite.killTweensOf(this.tileAr[_local9][_local10], {"scaleX": true, "scaleY": true});
        this.tileAr[_local9][_local10].scaleX = 1;
        this.tileAr[_local9][_local10].scaleY = 1;
      L6711:
        _local10++;
      L6713:
        if (_local10 < 8) goto L6620;
        _local9++;
      L6723:
        if (_local9 < 8) goto L6611;
        goto L7295;
      L6735:
        if (!this._global.tutorial.checkTut(Tutorial.LEVEL_27, 26)) goto L7112;
        this.tileAr[0][0].frozen = true;
        _local11 = 0;
        goto L6935;
      L6784:
        if (this.frozenTileAr[_local11].visible) goto L6933;
        this.frozenTileAr[_local11].visible = true;
        this.frozenTileAr[_local11].x = this.tileAr[0][0].x;
        this.frozenTileAr[_local11].y = this.tileAr[0][0].y;
        this.frozenTileAr[_local11].attachedCol = 0;
        this.frozenTileAr[_local11].attachedRow = 0;
        this.tileAr[0][0].frozenTileIdx = this.frozenTileAr[_local11].idx;
        goto L6948;
      L6933:
        _local11++;
      L6935:
        if (_local11 < this.frozenTileAr.length) goto L6784;
      L6948:
        _local12 = this;
        var _local13 = this.currentFrozenTile + 1;
        _local12.currentFrozenTile = _local13;
        this.tileAr[1][1].type = 15;
        this.tileAr[1][1].texture = this.gameAtlas.getTexture("chef_torch");
        this.tileAr[1][1].readjustSize();
        this.tileAr[1][1].pivotX = this.tileAr[1][1].width / 2;
        this.tileAr[1][1].pivotY = this.tileAr[1][1].height / 2;
        goto L7295;
      L7112:
        if (!this._global.tutorial.checkTut(Tutorial.LEVEL_31, 30)) goto L7295;
        this.tileAr[1][1].type = 10;
        this.tileAr[1][1].texture = this.gameAtlas.getTexture("cookies");
        this.tileAr[1][1].readjustSize();
        this.tileAr[1][1].pivotX = this.tileAr[1][1].width / 2;
        this.tileAr[1][1].pivotY = this.tileAr[1][1].height / 2;
        _local12 = this;
        _local13 = this.cookiesCountOnBoard + 1;
        _local12.cookiesCountOnBoard = _local13;
      L7295:
        TGlobalClass._windowDisplay.widgetInfo.text = "Goal   : " + this.goalTarget + "\nExpert : " + this.expertTarget;
        var $t4 = TGlobalClass.levelEventXML.day[TGlobalClass.currentPlayedLevel].window.length() > 0;
        $s0 = $t4;
        if (!$t4) goto L7403;
        var $t5 = !(this._global.tutorial.currentDialogueID >= 0);
        $s0 = $t5;
        if ($t5) goto L7403;
        $s0 = TGlobalClass.levelEventXML.day[TGlobalClass.currentPlayedLevel].@alwaysShow == "true";
      L7403:
        if (!$s0) goto L7433;
        TGlobalClass._windowDisplay.showBeginningWindow(TGlobalClass.currentPlayedLevel, TGlobalClass.currentNotifWindow);
        goto L7469;
      L7433:
        if (TGlobalClass.levelEventXML.day[TGlobalClass.currentPlayedLevel].window.length() != 0) goto L7469;
        this.startLevel();
      L7469:
        this._global._stage.addEventListener(KeyboardEvent.KEY_DOWN, this.KeyDown);
        this.escCounter = 0;
        this.firstCust = true;
        this.calledShowWindow = false;
        this.hasWindow = true;
        this.paused = false;
        ready = true;
    }

    function moveFries(arg1:TouchEvent):void {
        var _local3 = 0;
        var _local4 = null;
        var _local5 = null;
        var _local2 = arg1.getTouch(this._global._game);
        if (!this.fries.visible) goto L77;
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_fries", true));
        this.tmpFries.visible = true;
      L77:
        if (!arg1.getTouch(this._global._game, "moved")) goto L162;
        this._global._gameS.setChildIndex(this.tmpFries, this._global._gameS.numChildren - 1);
        this.fries.visible = false;
        this.choosenJuice = false;
        this.choosenFries = true;
        this.tmpFries.x = _local2.globalX;
        this.tmpFries.y = _local2.globalY;
      L162:
        if (!arg1.getTouch(this._global._game, "ended")) goto L533;
        _local3 = 0;
        goto L464;
      L188:
        var $t0 = !this.customer[_local3].hasFries;
        $s0 = $t0;
        if (!$t0) goto L212;
        $s0 = Boolean(this.choosenFries);
      L212:
        if (!$s0) goto L462;
        _local4 = this.tmpFries.bounds;
        var $t1 = this.customer[_local3].clickQuad.bounds;
        _local5 = $t1;
        $t1.x = this.customer[_local3].clickQuad.localToGlobal(new Point(0, 0)).x;
        _local5.y = this.customer[_local3].clickQuad.localToGlobal(new Point(0, 0)).y;
        if (!_local4.intersects(_local5)) goto L462;
        $s0 = this._global._soundAsset;
        $s1 = "put_beverage_fries_on_tray";
        if (!(TGlobalClass.RandomRange(1, 2) == 1)) goto L357;
        $s2 = "";
        goto L361;
      L357:
        $s2 = "_v2";
      L361:
        $s0.playSFX($s1 + $s2);
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "offered_fries", true));
        this.choosenFries = false;
        this.customer[_local3].hasFries = true;
        this.customer[_local3].fries.visible = true;
        this.tmpFries.visible = false;
        this.tmpFries.x = 216.5;
        this.tmpFries.y = 522;
        this.remakeFries();
        goto L476;
      L462:
        _local3++;
      L464:
        if (_local3 < this.customer.length) goto L188;
      L476:
        if (!(_local3 > 2)) goto L533;
        this.tmpFries.x = 216.5;
        this.tmpFries.y = 522;
        this._global._gameS.setChildIndex(this.tmpFries, this._global._gameS.getChildIndex(this.fries) - 1);
      L533:
    }

    function moveDrink(arg1:TouchEvent):void {
        var _local3 = 0;
        var _local4 = null;
        var _local5 = null;
        var _local2 = arg1.getTouch(this._global._game);
        if (!this.drink.visible) goto L77;
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_beverage", true));
        this.tmpDrink.visible = true;
      L77:
        if (!arg1.getTouch(this._global._game, "moved")) goto L162;
        this._global._gameS.setChildIndex(this.tmpDrink, this._global._gameS.numChildren - 1);
        this.drink.visible = false;
        this.choosenJuice = true;
        this.choosenFries = false;
        this.tmpDrink.x = _local2.globalX;
        this.tmpDrink.y = _local2.globalY;
      L162:
        if (!arg1.getTouch(this._global._game, "ended")) goto L531;
        _local3 = 0;
        goto L463;
      L188:
        var $t0 = !this.customer[_local3].hasJuice;
        $s0 = $t0;
        if (!$t0) goto L212;
        $s0 = Boolean(this.choosenJuice);
      L212:
        if (!$s0) goto L461;
        _local4 = this.tmpDrink.bounds;
        var $t1 = this.customer[_local3].clickQuad.bounds;
        _local5 = $t1;
        $t1.x = this.customer[_local3].clickQuad.localToGlobal(new Point(0, 0)).x;
        _local5.y = this.customer[_local3].clickQuad.localToGlobal(new Point(0, 0)).y;
        if (!_local4.intersects(_local5)) goto L461;
        $s0 = this._global._soundAsset;
        $s1 = "put_beverage_fries_on_tray";
        if (!(TGlobalClass.RandomRange(1, 2) == 1)) goto L357;
        $s2 = "";
        goto L361;
      L357:
        $s2 = "_v2";
      L361:
        $s0.playSFX($s1 + $s2);
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "offered_beverage", true));
        this.choosenJuice = false;
        this.customer[_local3].hasJuice = true;
        this.customer[_local3].juice.visible = true;
        this.tmpDrink.visible = false;
        this.tmpDrink.x = 108.5;
        this.tmpDrink.y = 527.5;
        this.remakeJuice();
        goto L475;
      L461:
        _local3++;
      L463:
        if (_local3 < this.customer.length) goto L188;
      L475:
        if (!(_local3 > 2)) goto L531;
        this.tmpDrink.x = 108.5;
        this.tmpDrink.y = 527.5;
        this._global._gameS.setChildIndex(this.tmpDrink, this._global._gameS.getChildIndex(this.drink) - 1);
      L531:
    }

    function collide(arg1:*, arg2:*):Boolean {
        var $t0 = arg1.x >= arg2.x;
        $s0 = $t0;
        if (!$t0) goto L34;
        $s0 = arg1.x <= arg2.x + arg2.width / 2;
      L34:
        if ($s0) goto L72;
        var $t1 = arg2.x >= arg1.x;
        $s0 = $t1;
        if (!$t1) goto L72;
        $s0 = arg2.x <= arg1.x + arg1.width / 2;
      L72:
        if (!$s0) goto L152;
        var $t2 = arg1.y >= arg2.y;
        $s0 = $t2;
        if (!$t2) goto L108;
        $s0 = arg1.y <= arg2.y + arg2.height / 2;
      L108:
        if ($s0) goto L146;
        var $t3 = arg2.y >= arg1.y;
        $s0 = $t3;
        if (!$t3) goto L146;
        $s0 = arg2.y <= arg1.y + arg1.height / 2;
      L146:
        if (!$s0) goto L152;
        return true;
      L152:
        return false;
    }

    function KeyDown(arg1:KeyboardEvent):void {
        var $t0 = TSavegame.currentPlayer.name == "OrIgAmInglOl";
        $s0 = $t0;
        if (!$t0) goto L32;
        $s0 = Boolean(this._global.gameScreen.cheatActive);
      L32:
        if (!$s0) goto L142;
        TSavegame.currentPlayer.money = "faaaa";
        if (arg1.keyCode != Keyboard.ESCAPE) goto L142;
        var _local2 = this;
        var _local3 = this.escCounter + 1;
        _local2.escCounter = _local3;
        if (!(this.escCounter > 2)) goto L142;
        this.servedCustomerCount = this.goalTarget;
        ScoreManager.totalScore = this._global._maxCrypto.ITS(this.expertTarget);
        TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] = true;
        this.showWindow();
      L142:
    }

    function startLevel(arg1:int = 3):void {
        this.hasWindow = false;
        var _local2 = 0;
        goto L40;
      L14:
        this.customer[_local2].tweenOut(this.customer[_local2].CUST_SATISFIED);
        _local2++;
      L40:
        if (_local2 < arg1) goto L14;
    }

    function dialogueCondition(arg1:int, arg2:int):void {
    }

    function touchCoins(arg1:TouchEvent):void {
        targetID = 0;
        juiceBonus = 0;
        friesBonus = 0;
        chainBonus = 0;
        e = arg1;
        touchHover = e.getTouch(e.currentTarget as cSprite, TouchPhase.HOVER);
        targetIDHover = int((e.currentTarget as cSprite).idx);
        if (!touchHover) goto L115;
        this.coinsAr[targetIDHover].filter = this.coinFilter;
        goto L130;
      L115:
        this.coinsAr[targetIDHover].filter = null;
      L130:
        if (!e.getTouch(this._global._game, TouchPhase.BEGAN)) goto L1528;
        targetID = int((e.currentTarget as cSprite).idx);
        var $t0 = Boolean(this.choosenJuice);
        $s0 = $t0;
        if (!$t0) goto L200;
        $s0 = !this.customer[targetID].hasJuice;
      L200:
        if ($s0) goto L232;
        var $t1 = Boolean(this.choosenFries);
        $s0 = $t1;
        if (!$t1) goto L232;
        $s0 = !this.customer[targetID].hasFries;
      L232:
        if (!$s0) goto L252;
        this.customer[targetID].selectPriorCust();
        return;
      L252:
        $s0 = this._global._soundAsset;
        $s1 = "money_collected";
        if (!(TGlobalClass.RandomRange(1, 2) == 1)) goto L287;
        $s2 = "";
        goto L291;
      L287:
        $s2 = "_v2";
      L291:
        $s0.playSFX($s1 + $s2);
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_coin"));
        this._global._soundAsset.playSFX("serve");
        this.earnedMoneyTextAr[targetID].visible = true;
        this.earnedMoneyTextAr[targetID].text = "$" + (this.customer[targetID].money + this.customer[targetID].tipsBonus);
        if (!this.customer[targetID].hasJuice) goto L558;
        var _local3 = TGlobalClass;
        var _local4 = TGlobalClass.juiceServed + 1;
        _local3.juiceServed = _local4;
        juiceBonus = int(this._global.dish.getHardwareDefaultPrice(Dish.JUICER) + (TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] - 1) * this._global.dish.getHardwarePriceIncrease(Dish.JUICER));
        this.earnedMoneyTextAr[targetID].text = "$" + (this.customer[targetID].money + this.customer[targetID].tipsBonus) + "\n+ $" + juiceBonus + " Juice Bonus!";
      L558:
        if (!this.customer[targetID].hasFries) goto L717;
        _local3 = TGlobalClass;
        _local4 = TGlobalClass.friesServed + 1;
        _local3.friesServed = _local4;
        friesBonus = int(this._global.dish.getHardwareDefaultPrice(Dish.FRYER) + (TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] - 1) * this._global.dish.getHardwarePriceIncrease(Dish.FRYER));
        this.earnedMoneyTextAr[targetID].text = "$" + (this.customer[targetID].money + this.customer[targetID].tipsBonus) + "\n+ $" + friesBonus + " Fries Bonus!";
      L717:
        var $t2 = Boolean(this.customer[targetID].hasJuice);
        $s0 = $t2;
        if (!$t2) goto L753;
        $s0 = Boolean(this.customer[targetID].hasFries);
      L753:
        if (!$s0) goto L822;
        this.earnedMoneyTextAr[targetID].text = "$" + (this.customer[targetID].money + this.customer[targetID].tipsBonus) + "\n+ $" + (friesBonus + juiceBonus) + " Juice + Fries Bonus!";
      L822:
        chainBonus = int(this.chainCombo * 0.2 * this.customer[targetID].money);
        _local3 = this;
        _local4 = this.chainCombo + 1;
        _local3.chainCombo = _local4;
        TweenLite.to(this.coinsAr[targetID], 0.4, {"scaleX": 0, "scaleY": 0, "ease": Back.easeIn});
        ScoreManager.updateScore(this.customer[targetID].money + this.customer[targetID].tipsBonus + juiceBonus + friesBonus + chainBonus);
        var $t3 = Boolean(TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel]);
        $s0 = $t3;
        if (!$t3) goto L1007;
        $s0 = this._global._maxCrypto.STI(ScoreManager.totalScore) >= this.expertTarget;
      L1007:
        if (!$s0) goto L1023;
        $s0 = this.goalExpertText.idx == 2;
      L1023:
        if (!$s0) goto L1060;
        this.goalExpertText.idx = 3;
        TGlobalClass._windowDisplay.showGoalIconSmall();
        this._global._soundAsset.playSFX("expert_reached");
      L1060:
        if (!(chainBonus > 0)) goto L1164;
        this.chainBonusTextAr[targetID].text = "Chain Bonus $" + chainBonus;
        this.chainBonusTextAr[targetID].visible = true;
        TweenLite.to(this.chainBonusTextAr[targetID], 0.8, {"y": this.chainBonusTextAr[targetID].y - 65, "onComplete": function(f1_arg1:int):void {
            chainBonusTextAr[f1_arg1].x = coinsAr[f1_arg1].x;
            chainBonusTextAr[f1_arg1].y = coinsAr[f1_arg1].y;
            chainBonusTextAr[f1_arg1].visible = false;
        }, "onCompleteParams": [targetID]});
      L1164:
        TweenLite.to(this.earnedMoneyTextAr[targetID], 1, {"y": this.earnedMoneyTextAr[targetID].y - 65, "onComplete": function(f1_arg1:int):void {
            earnedMoneyTextAr[f1_arg1].x = coinsAr[f1_arg1].x;
            earnedMoneyTextAr[f1_arg1].y = coinsAr[f1_arg1].y;
            earnedMoneyTextAr[f1_arg1].visible = false;
        }, "onCompleteParams": [targetID]});
        _local3 = this;
        _local4 = this.servedCustomerCount + 1;
        _local3.servedCustomerCount = _local4;
        if (TGlobalClass.isPlayingSpecialLevel) goto L1393;
        if (this.goalExpertText.idx != 1) goto L1283;
        this.targetInfoText.text = this.servedCustomerCount + "/" + this.goalTarget;
      L1283:
        var $t4 = this.servedCustomerCount == this.goalTarget;
        $s0 = $t4;
        if (!$t4) goto L1308;
        $s0 = this.goalExpertText.idx == 1;
      L1308:
        if (!$s0) goto L1389;
        TGlobalClass._windowDisplay.showGoalIconSmall();
        this.goalExpertText.idx = 2;
        this.targetInfoText.text = "$" + this.expertTarget;
        this.targetStateText.text = "Expert:";
        this._global._soundAsset.playSFX("goal_reached");
        TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] = true;
      L1389:
        goto L1505;
      L1393:
        var $t5 = Boolean(this.customer[targetID].asVIP);
        $s0 = $t5;
        if (!$t5) goto L1424;
        $s0 = this.goalExpertText.idx == 1;
      L1424:
        if (!$s0) goto L1505;
        TGlobalClass._windowDisplay.showGoalIconSmall();
        this.goalExpertText.idx = 2;
        this.targetInfoText.text = "$" + this.expertTarget;
        this.targetStateText.text = "Expert:";
        this._global._soundAsset.playSFX("goal_reached");
        TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] = true;
      L1505:
        this.tweenDishAlpha(targetID, this.customer[targetID].CUST_SATISFIED);
      L1528:
    }

    function touchTile(arg1:TouchEvent):void {
        var _local4 = 0;
        var _local5 = 0;
        var _local6 = null;
        var _local7 = 0;
        var _local8 = 0;
        var _local9 = 0;
        var _local10 = 0;
        var _local2 = arg1.getTouch(this.touchQuad);
        var _local3 = arg1.getTouch(this.touchQuad, TouchPhase.HOVER);
        if (!_local2) goto L2905;
        _local4 = int((_local2.globalX - 457) / 68);
        _local5 = int((_local2.globalY - 40) / 68);
        var $t0 = _local4 < 0;
        $s0 = $t0;
        if ($t0) goto L109;
        $s0 = _local5 < 0;
      L109:
        if ($s0) goto L120;
        $s0 = _local4 > 7;
      L120:
        if ($s0) goto L131;
        $s0 = _local5 > 7;
      L131:
        if (!$s0) goto L136;
        return;
      L136:
        _local6 = this.tileAr[_local4][_local5];
        if (!this.levelComplete) goto L163;
        return;
      L163:
        if (!this.isSwapping) goto L172;
        return;
      L172:
        if (!this.gameover) goto L181;
        return;
      L181:
        if (!_local6.onTween) goto L191;
        return;
      L191:
        if (!_local6.removed) goto L201;
        return;
      L201:
        if (!_local6.isSwapping) goto L211;
        return;
      L211:
        if (!_local6.isDropping) goto L221;
        return;
      L221:
        if (!this._global.tutorial.button.visible) goto L239;
        return;
      L239:
        var $t1 = this._global.tutorial.currentDialogueID == Tutorial.LEVEL_1;
        $s0 = $t1;
        if (!$t1) goto L275;
        $s0 = this._global.tutorial.dialogueCounter == 1;
      L275:
        if (!$s0) goto L280;
        return;
      L280:
        var $t2 = this._global.tutorial.currentDialogueID == Tutorial.LEVEL_1;
        $s0 = $t2;
        if (!$t2) goto L316;
        $s0 = this._global.tutorial.dialogueCounter == 7;
      L316:
        if (!$s0) goto L321;
        return;
      L321:
        var $t3 = this._global.tutorial.currentDialogueID == Tutorial.LEVEL_12;
        $s0 = $t3;
        if (!$t3) goto L357;
        $s0 = this._global.tutorial.dialogueCounter == 4;
      L357:
        if (!$s0) goto L362;
        return;
      L362:
        var $t4 = this._global.tutorial.currentDialogueID == Tutorial.LEVEL_12;
        $s0 = $t4;
        if (!$t4) goto L398;
        $s0 = this._global.tutorial.dialogueCounter == 5;
      L398:
        if (!$s0) goto L403;
        return;
      L403:
        var $t5 = this._global.tutorial.currentDialogueID == Tutorial.LEVEL_17;
        $s0 = $t5;
        if (!$t5) goto L439;
        $s0 = this._global.tutorial.dialogueCounter == 4;
      L439:
        if (!$s0) goto L444;
        return;
      L444:
        var $t6 = this._global.tutorial.currentDialogueID == Tutorial.LEVEL_17;
        $s0 = $t6;
        if (!$t6) goto L480;
        $s0 = this._global.tutorial.dialogueCounter == 5;
      L480:
        if (!$s0) goto L485;
        return;
      L485:
        if (_local2.phase != TouchPhase.BEGAN) goto L786;
        _local7 = int(_local6.idx);
        this.matched = false;
        this.swapBack = false;
        this.choosenFries = false;
        this.choosenJuice = false;
        if (this.juicerPercentage.text != "Select Customer") goto L551;
        this.juicerPercentage.text = "READY!!";
      L551:
        if (this.fryerPercentage.text != "Select Customer") goto L575;
        this.fryerPercentage.text = "READY!!";
      L575:
        this.highlightPossibleTile = false;
        this.idleDuration = 0;
        this.chainCombo = 0;
        _local8 = 0;
        goto L712;
      L600:
        _local9 = 0;
        goto L702;
      L609:
        if (this.tileAr[_local8][_local9].onTween) goto L700;
        TweenLite.killTweensOf(this.tileAr[_local8][_local9], {"scaleX": true, "scaleY": true});
        this.tileAr[_local8][_local9].scaleX = 1;
        this.tileAr[_local8][_local9].scaleY = 1;
      L700:
        _local9++;
      L702:
        if (_local9 < 8) goto L609;
        _local8++;
      L712:
        if (_local8 < 8) goto L600;
        if (_local6.type != 10) goto L736;
        this.touchCookies = true;
      L736:
        TweenLite.killTweensOf(_local6, {"scaleX": true, "scaleY": true});
        _local6.scaleX = 1;
        _local6.scaleY = 1;
        if (this.touchFirstTile) goto L782;
        this.touchFirstTile = true;
      L782:
        goto L2905;
      L786:
        if (_local2.phase != TouchPhase.MOVED) goto L2831;
        var $t7 = this.lastCol == _local4;
        $s0 = $t7;
        if (!$t7) goto L820;
        $s0 = this.lastRow == _local5;
      L820:
        if (!$s0) goto L825;
        return;
      L825:
        if (this.touchFirstTile) goto L834;
        return;
      L834:
        if (this._global.tutorial.currentDialogueID != Tutorial.LEVEL_26) goto L979;
        if (!(this._global.tutorial.dialogueCounter < 3)) goto L871;
        return;
      L871:
        if (this._global.tutorial.dialogueCounter != 3) goto L975;
        _local10 = 0;
        goto L958;
      L895:
        var $t8 = this._global.tutorial.highlightedAr[_local10].col == this.lastCol;
        $s0 = $t8;
        if (!$t8) goto L948;
        $s0 = this._global.tutorial.highlightedAr[_local10].row == this.lastRow;
      L948:
        if (!$s0) goto L956;
        goto L966;
      L956:
        _local10++;
      L958:
        if (_local10 < 3) goto L895;
      L966:
        if (_local10 != 3) goto L975;
        return;
      L975:
        goto L1123;
      L979:
        if (this._global.tutorial.currentDialogueID != Tutorial.LEVEL_27) goto L1082;
        if (this._global.tutorial.dialogueCounter != 0) goto L1016;
        return;
      L1016:
        var $t9 = this._global.tutorial.dialogueCounter == 2;
        $s0 = $t9;
        if (!$t9) goto L1073;
        var $t10 = !(this.tileAr[this.lastCol][this.lastRow].type == 15);
        $s0 = $t10;
        if ($t10) goto L1073;
        $s0 = _local5 >= this.lastRow;
      L1073:
        if (!$s0) goto L1078;
        return;
      L1078:
        goto L1123;
      L1082:
        var $t11 = this._global.tutorial.currentDialogueID == Tutorial.LEVEL_31;
        $s0 = $t11;
        if (!$t11) goto L1118;
        $s0 = this._global.tutorial.dialogueCounter <= 2;
      L1118:
        if (!$s0) goto L1123;
        return;
      L1123:
        var $t12 = this.lastRow == _local5;
        $s0 = $t12;
        if (!$t12) goto L1143;
        $s0 = this.lastCol < _local4;
      L1143:
        if (!$s0) goto L1558;
        if (!this.tileAr[this.lastCol + 1][this.lastRow].isDropping) goto L1176;
        return;
      L1176:
        if (!this.tileAr[this.lastCol + 1][this.lastRow].onTween) goto L1205;
        return;
      L1205:
        if (!this.tileAr[this.lastCol + 1][this.lastRow].isSwapping) goto L1234;
        return;
      L1234:
        if (!this.tileAr[this.lastCol + 1][this.lastRow].removed) goto L1263;
        return;
      L1263:
        this.touchCookies = false;
        var $t13 = Boolean(this.tileAr[this.lastCol + 1][this.lastRow].frozen);
        $s0 = $t13;
        if ($t13) goto L1321;
        $s0 = Boolean(this.tileAr[this.lastCol][this.lastRow].frozen);
      L1321:
        if (!$s0) goto L1456;
        if (!this.tileAr[this.lastCol + 1][this.lastRow].frozen) goto L1393;
        this.tweenShake(this.frozenTileAr[this.tileAr[this.lastCol + 1][this.lastRow].frozenTileIdx], 1, 0);
      L1393:
        if (!this.tileAr[this.lastCol][this.lastRow].frozen) goto L1455;
        this.tweenShake(this.frozenTileAr[this.tileAr[this.lastCol][this.lastRow].frozenTileIdx], 1, 0);
      L1455:
        return;
      L1456:
        _local6.scaleX = 1;
        _local6.scaleY = 1;
        TweenLite.killTweensOf(_local6, {"scaleX": true, "scaleY": true});
        this.touchFirstTile = false;
        this.firstTile = this.tileAr[this.lastCol][this.lastRow];
        this.lastTile = this.tileAr[this.lastCol + 1][this.lastRow];
        this.swapTile(this.firstTile, this.lastTile);
        goto L2827;
      L1558:
        var $t14 = this.lastRow == _local5;
        $s0 = $t14;
        if (!$t14) goto L1578;
        $s0 = this.lastCol > _local4;
      L1578:
        if (!$s0) goto L1977;
        if (!this.tileAr[this.lastCol - 1][this.lastRow].isDropping) goto L1609;
        return;
      L1609:
        if (!this.tileAr[this.lastCol - 1][this.lastRow].onTween) goto L1636;
        return;
      L1636:
        if (!this.tileAr[this.lastCol - 1][this.lastRow].isSwapping) goto L1663;
        return;
      L1663:
        if (!this.tileAr[this.lastCol - 1][this.lastRow].removed) goto L1690;
        return;
      L1690:
        this.touchCookies = false;
        var $t15 = Boolean(this.tileAr[this.lastCol - 1][this.lastRow].frozen);
        $s0 = $t15;
        if ($t15) goto L1746;
        $s0 = Boolean(this.tileAr[this.lastCol][this.lastRow].frozen);
      L1746:
        if (!$s0) goto L1877;
        if (!this.tileAr[this.lastCol - 1][this.lastRow].frozen) goto L1814;
        this.tweenShake(this.frozenTileAr[this.tileAr[this.lastCol - 1][this.lastRow].frozenTileIdx], 1, 0);
      L1814:
        if (!this.tileAr[this.lastCol][this.lastRow].frozen) goto L1876;
        this.tweenShake(this.frozenTileAr[this.tileAr[this.lastCol][this.lastRow].frozenTileIdx], 1, 0);
      L1876:
        return;
      L1877:
        _local6.scaleX = 1;
        _local6.scaleY = 1;
        TweenLite.killTweensOf(_local6, {"scaleX": true, "scaleY": true});
        this.touchFirstTile = false;
        this.firstTile = this.tileAr[this.lastCol][this.lastRow];
        this.lastTile = this.tileAr[this.lastCol - 1][this.lastRow];
        this.swapTile(this.firstTile, this.lastTile);
        goto L2827;
      L1977:
        var $t16 = this.lastRow < _local5;
        $s0 = $t16;
        if (!$t16) goto L1997;
        $s0 = this.lastCol == _local4;
      L1997:
        if (!$s0) goto L2412;
        if (!this.tileAr[this.lastCol][this.lastRow + 1].isDropping) goto L2030;
        return;
      L2030:
        if (!this.tileAr[this.lastCol][this.lastRow + 1].onTween) goto L2059;
        return;
      L2059:
        if (!this.tileAr[this.lastCol][this.lastRow + 1].isSwapping) goto L2088;
        return;
      L2088:
        if (!this.tileAr[this.lastCol][this.lastRow + 1].removed) goto L2117;
        return;
      L2117:
        this.touchCookies = false;
        var $t17 = Boolean(this.tileAr[this.lastCol][this.lastRow + 1].frozen);
        $s0 = $t17;
        if ($t17) goto L2175;
        $s0 = Boolean(this.tileAr[this.lastCol][this.lastRow].frozen);
      L2175:
        if (!$s0) goto L2310;
        if (!this.tileAr[this.lastCol][this.lastRow + 1].frozen) goto L2247;
        this.tweenShake(this.frozenTileAr[this.tileAr[this.lastCol][this.lastRow + 1].frozenTileIdx], 0, 1);
      L2247:
        if (!this.tileAr[this.lastCol][this.lastRow].frozen) goto L2309;
        this.tweenShake(this.frozenTileAr[this.tileAr[this.lastCol][this.lastRow].frozenTileIdx], 0, 1);
      L2309:
        return;
      L2310:
        _local6.scaleX = 1;
        _local6.scaleY = 1;
        TweenLite.killTweensOf(_local6, {"scaleX": true, "scaleY": true});
        this.touchFirstTile = false;
        this.firstTile = this.tileAr[this.lastCol][this.lastRow];
        this.lastTile = this.tileAr[this.lastCol][this.lastRow + 1];
        this.swapTile(this.firstTile, this.lastTile);
        goto L2827;
      L2412:
        var $t18 = this.lastRow > _local5;
        $s0 = $t18;
        if (!$t18) goto L2432;
        $s0 = this.lastCol == _local4;
      L2432:
        if (!$s0) goto L2827;
        if (!this.tileAr[this.lastCol][this.lastRow - 1].isDropping) goto L2463;
        return;
      L2463:
        if (!this.tileAr[this.lastCol][this.lastRow - 1].onTween) goto L2490;
        return;
      L2490:
        if (!this.tileAr[this.lastCol][this.lastRow - 1].isSwapping) goto L2517;
        return;
      L2517:
        if (!this.tileAr[this.lastCol][this.lastRow - 1].removed) goto L2544;
        return;
      L2544:
        this.touchCookies = false;
        var $t19 = Boolean(this.tileAr[this.lastCol][this.lastRow - 1].frozen);
        $s0 = $t19;
        if ($t19) goto L2600;
        $s0 = Boolean(this.tileAr[this.lastCol][this.lastRow].frozen);
      L2600:
        if (!$s0) goto L2731;
        if (!this.tileAr[this.lastCol][this.lastRow - 1].frozen) goto L2668;
        this.tweenShake(this.frozenTileAr[this.tileAr[this.lastCol][this.lastRow - 1].frozenTileIdx], 0, 1);
      L2668:
        if (!this.tileAr[this.lastCol][this.lastRow].frozen) goto L2730;
        this.tweenShake(this.frozenTileAr[this.tileAr[this.lastCol][this.lastRow].frozenTileIdx], 0, 1);
      L2730:
        return;
      L2731:
        _local6.scaleX = 1;
        _local6.scaleY = 1;
        TweenLite.killTweensOf(_local6, {"scaleX": true, "scaleY": true});
        this.touchFirstTile = false;
        this.firstTile = this.tileAr[this.lastCol][this.lastRow];
        this.lastTile = this.tileAr[this.lastCol][this.lastRow - 1];
        this.swapTile(this.firstTile, this.lastTile);
      L2827:
        goto L2905;
      L2831:
        if (_local2.phase != TouchPhase.ENDED) goto L2905;
        if (!this.touchFirstTile) goto L2858;
        this.touchFirstTile = false;
      L2858:
        if (!this.touchCookies) goto L2905;
        this.touchCookies = false;
        var _local11 = TGlobalClass;
        var _local12 = TGlobalClass.cookiesUsed + 1;
        _local11.cookiesUsed = _local12;
        this.giveTheCookies(_local6, this.cookiesEffect);
      L2905:
        if (!_local3) goto L2977;
        _local6.filter = this.blurFilter;
        _local6.hover = true;
        var $t20 = !(this.lastCol == _local4);
        $s0 = $t20;
        if ($t20) goto L2947;
        $s0 = !(this.lastRow == _local5);
      L2947:
        if (!$s0) goto L2973;
        this.tileAr[this.lastCol][this.lastRow].filter = null;
      L2973:
        goto L2999;
      L2977:
        this.tileAr[this.lastCol][this.lastRow].filter = null;
      L2999:
        this.lastCol = _local4;
        this.lastRow = _local5;
    }

    function giveTheCookies(arg1:Match3Object = null, arg2:int = 0):void {
        tweenTarget = arg1;
        effectedPatience = arg2;
        $s0 = this._global._soundAsset;
        $s1 = "cookie";
        if (!(TGlobalClass.RandomRange(1, 2) == 1)) goto L51;
        $s2 = "";
        goto L55;
      L51:
        $s2 = "_v2";
      L55:
        $s0.playSFX($s1 + $s2);
        setLowestPatience = false;
        lowestPatience = 0;
        lowestPatienceIdx = 0;
        var $t0 = new Vector.<String>(10);
        $t0[0] = "bread";
        $t0[1] = "selada";
        $t0[2] = "beef";
        $t0[3] = "ayam";
        $t0[4] = "nasi";
        $t0[5] = "bacon";
        $t0[6] = "salmon";
        $t0[7] = "ikan";
        $t0[8] = "keju";
        $t0[9] = "cookies";
        textureName = $t0;
        i = 0;
        goto L298;
      L198:
        var $t1 = this.customer[i].patienceSprite.clipRect.x < lowestPatience;
        $s0 = $t1;
        if (!$t1) goto L245;
        $s0 = !this.customer[i].onTween;
      L245:
        if (!$s0) goto L288;
        lowestPatience = int(this.customer[i].patienceSprite.clipRect.x);
        lowestPatienceIdx = int(i);
        setLowestPatience = true;
      L288:
        i = i + 1;
      L298:
        if (i < 3) goto L198;
        var $t2 = Boolean(tweenTarget);
        $s0 = $t2;
        if (!$t2) goto L324;
        $s0 = Boolean(setLowestPatience);
      L324:
        if (!$s0) goto L716;
        i = 0;
        goto L701;
      L338:
        if (this.tweenTileAr[i].onTween) goto L691;
        this.tweenTileAr[i].visible = true;
        this.tweenTileAr[i].onTween = true;
        this.tweenTileAr[i].x = tweenTarget.x;
        this.tweenTileAr[i].y = tweenTarget.y;
        this.tweenTileAr[i].texture = this.gameAtlas.getTexture(textureName[tweenTarget.type - 1]);
        this.tweenTileAr[i].readjustSize();
        this.tweenTileAr[i].pivotX = this.tweenTileAr[i].width / 2;
        this.tweenTileAr[i].pivotY = this.tweenTileAr[i].height / 2;
        tweenTarget.removed = true;
        this.affectAbove(tweenTarget);
        this.addNewPieces();
        var _local4 = this;
        var _local5 = this.cookiesCountOnBoard - 1;
        _local4.cookiesCountOnBoard = _local5;
        TweenLite.to(this.tweenTileAr[i], 0.5 + tweenTarget.col * 0.05, {"x": this.customer[lowestPatienceIdx].x + 15, "y": this.customer[lowestPatienceIdx].y + 25, "ease": Linear.easeNone, "onComplete": function(f1_arg1:int, f1_arg2:int):void {
            idx = f1_arg1;
            tweenTileIdx = f1_arg2;
            tweenTileAr[tweenTileIdx].visible = false;
            tweenTileAr[tweenTileIdx].onTween = false;
            toadd = effectedPatience / 100 * customer[idx].totalPatience;
            if (!(customer[idx].patience + toadd > customer[idx].totalPatience)) goto L134;
            toadd = customer[idx].totalPatience - customer[idx].patience;
          L134:
            customer[idx].tweeningPatience = true;
            TweenLite.to(customer[idx], 0.3, {"patience": customer[idx].patience + toadd, "onComplete": function(f2_arg1:int):void {
                customer[f2_arg1].tweeningPatience = false;
                _global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "give_cookies"));
            }, "onCompleteParams": [idx]});
            halfPrice = 0.5 * customer[idx].totalMoney;
            patienceDivided = (customer[idx].patience + toadd) / customer[idx].totalPatience;
            if (!(customer[idx].patience >= 0.8 * customer[idx].totalPatience)) goto L353;
            customer[idx].money = halfPrice * 2;
            customer[idx].moneyDisplay.text = "$" + customer[idx].money;
            goto L413;
          L353:
            customer[idx].money = halfPrice + halfPrice * patienceDivided;
            customer[idx].moneyDisplay.text = "$" + customer[idx].money;
          L413:
        }, "onCompleteParams": [lowestPatienceIdx, i]});
        goto L716;
      L691:
        i = i + 1;
      L701:
        if (i < this.tweenTileAr.length) goto L338;
      L716:
    }

    function swapTile(arg1:Match3Object, arg2:Match3Object):void {
        firstTile = arg1;
        lastTile = arg2;
        this.tileAr[firstTile.col][firstTile.row].isSwapping = true;
        this.tileAr[lastTile.col][lastTile.row].isSwapping = true;
        this.isSwapping = true;
        tempCol = int(lastTile.col);
        tempRow = int(lastTile.row);
        lastTile.filter = null;
        firstTile.filter = null;
        lastTile.col = firstTile.col;
        lastTile.row = firstTile.row;
        firstTile.col = tempCol;
        firstTile.row = tempRow;
        this.tileAr[firstTile.col][firstTile.row] = firstTile;
        this.tileAr[lastTile.col][lastTile.row] = lastTile;
        TweenLite.to(firstTile, 0.2, {"x": lastTile.x, "y": lastTile.y, "ease": Linear.easeNone});
        TweenLite.to(lastTile, 0.2, {"x": firstTile.x, "y": firstTile.y, "ease": Linear.easeNone, "onComplete": function():void {
            var _local3 = undefined;
            var _local4 = undefined;
            var _local5 = undefined;
            var _local6 = undefined;
            var _local7 = undefined;
            tileAr[firstTile.col][firstTile.row].isSwapping = false;
            tileAr[lastTile.col][lastTile.row].isSwapping = false;
            isSwapping = false;
            var _local1 = getMatches(tileAr[firstTile.col][firstTile.row]);
            var _local2 = getMatches(tileAr[lastTile.col][lastTile.row]);
            var $t0 = firstTile.type == 15;
            $s0 = $t0;
            if ($t0) goto L160;
            $s0 = lastTile.type == 15;
          L160:
            if (!$s0) goto L239;
            _global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "start_torch", false));
            if (firstTile.type != 15) goto L213;
            startTorchEffect(firstTile);
          L213:
            if (lastTile.type != 15) goto L235;
            startTorchEffect(lastTile);
          L235:
            goto L300;
          L239:
            var $t1 = _local1.length == 0;
            $s0 = $t1;
            if (!$t1) goto L259;
            $s0 = _local2.length == 0;
          L259:
            if (!$s0) goto L269;
            $s0 = !swapBack;
          L269:
            if (!$s0) goto L300;
            isSwapping = true;
            swapBack = true;
            swapTile(firstTile, lastTile);
          L300:
            var $t2 = _local1.length > 0;
            $s0 = $t2;
            if ($t2) goto L320;
            $s0 = _local2.length > 0;
          L320:
            if (!$s0) goto L830;
            var $t3 = _global.tutorial.currentDialogueID == Tutorial.LEVEL_26;
            $s0 = $t3;
            if (!$t3) goto L358;
            $s0 = _global.tutorial.dialogueCounter == 3;
          L358:
            if (!$s0) goto L789;
            _local3 = false;
            _local4 = 0;
            goto L548;
          L374:
            _local5 = 0;
            goto L518;
          L384:
            _local6 = 0;
            goto L495;
          L394:
            _local7 = 0;
            goto L469;
          L404:
            if (tileAr[_local6][_local7].idx != _local1[_local4][_local5]) goto L463;
            if (!tileAr[_local6][_local7].frozen) goto L459;
            _local3 = true;
          L459:
            goto L477;
          L463:
            _local7 = _local7 + 1;
          L469:
            if (_local7 < 8) goto L404;
          L477:
            if (_local7 == 8) goto L489;
            goto L503;
          L489:
            _local6 = _local6 + 1;
          L495:
            if (_local6 < 8) goto L394;
          L503:
            if (!_local3) goto L512;
            goto L533;
          L512:
            _local5 = _local5 + 1;
          L518:
            if (_local5 < _local1[_local4].length) goto L384;
          L533:
            if (!_local3) goto L542;
            goto L558;
          L542:
            _local4 = _local4 + 1;
          L548:
            if (_local4 < _local1.length) goto L374;
          L558:
            if (_local3) goto L756;
            _local4 = 0;
            goto L746;
          L572:
            _local5 = 0;
            goto L716;
          L582:
            _local6 = 0;
            goto L693;
          L592:
            _local7 = 0;
            goto L667;
          L602:
            if (tileAr[_local6][_local7].idx != _local2[_local4][_local5]) goto L661;
            if (!tileAr[_local6][_local7].frozen) goto L657;
            _local3 = true;
          L657:
            goto L675;
          L661:
            _local7 = _local7 + 1;
          L667:
            if (_local7 < 8) goto L602;
          L675:
            if (_local7 == 8) goto L687;
            goto L701;
          L687:
            _local6 = _local6 + 1;
          L693:
            if (_local6 < 8) goto L592;
          L701:
            if (!_local3) goto L710;
            goto L731;
          L710:
            _local5 = _local5 + 1;
          L716:
            if (_local5 < _local2[_local4].length) goto L582;
          L731:
            if (!_local3) goto L740;
            goto L756;
          L740:
            _local4 = _local4 + 1;
          L746:
            if (_local4 < _local2.length) goto L572;
          L756:
            if (_local3) goto L789;
            isSwapping = true;
            swapBack = true;
            swapTile(firstTile, lastTile);
            return;
          L789:
            _global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "matched", false));
            matched = true;
            findAndRemoveOneMatch();
          L830:
        }});
    }

    function startTorchEffect(arg1:Match3Object):void {
        var _local2 = false;
        this._global._soundAsset.playSFX("power_up");
        var _local3 = 0;
        goto L458;
      L25:
        var $t0 = !this.tileAr[_local3][arg1.row].isDropping;
        $s0 = $t0;
        if (!$t0) goto L70;
        $s0 = !this.tileAr[_local3][arg1.row].onTween;
      L70:
        if (!$s0) goto L95;
        $s0 = !this.tileAr[_local3][arg1.row].removed;
      L95:
        if (!$s0) goto L120;
        $s0 = !this.tileAr[_local3][arg1.row].isSwapping;
      L120:
        if (!$s0) goto L456;
        _local2 = true;
        if (!this.tileAr[_local3][arg1.row].frozen) goto L200;
        this.tileAr[_local3][arg1.row].frozen = false;
        this.frozenTileAr[this.tileAr[_local3][arg1.row].frozenTileIdx].visible = false;
        goto L379;
      L200:
        if (this.tileAr[_local3][arg1.row].type != 10) goto L305;
        _local2 = false;
        var _local4 = TGlobalClass;
        var _local5 = TGlobalClass.cookiesUsed + 1;
        _local4.cookiesUsed = _local5;
        this.playBoardEffect(this.PLAY_API, _local3, arg1.row);
        this.playBoardEffect(this.PLAY_ASAP, _local3, arg1.row);
        this.giveTheCookies(this.tileAr[_local3][arg1.row], this.cookiesEffect);
        goto L379;
      L305:
        var $t1 = !(this.tileAr[_local3][arg1.row].type == 10);
        $s0 = $t1;
        if (!$t1) goto L355;
        $s0 = !(this.tileAr[_local3][arg1.row].type == 15);
      L355:
        if (!$s0) goto L379;
        this.checkCustOrderPrior(_local3, arg1.row);
        this.checkCustOrder(_local3, arg1.row);
      L379:
        if (!_local2) goto L456;
        this.tileAr[_local3][arg1.row].removed = true;
        this.playBoardEffect(this.PLAY_API, _local3, arg1.row);
        this.playBoardEffect(this.PLAY_ASAP, _local3, arg1.row);
        this.affectAbove(this.tileAr[_local3][arg1.row]);
        this.addNewPieces();
      L456:
        _local3++;
      L458:
        if (_local3 < 8) goto L25;
    }

    function getMatches(arg1:Match3Object):Vector.<Vector.<int>> {
        var _local2 = new Vector.<Vector.<int>>();
        var _local3 = new Vector.<int>();
        _local3.push(this.tileAr[arg1.col][arg1.row].idx);
        var _local4 = 1;
        goto L332;
      L67:
        var $t0 = this.tileAr[arg1.col + _local4][arg1.row].type == arg1.type;
        $s0 = $t0;
        if (!$t0) goto L131;
        $s0 = !(this.tileAr[arg1.col + _local4][arg1.row].type == 10);
      L131:
        if (!$s0) goto L145;
        $s0 = !(arg1.type == 10);
      L145:
        if (!$s0) goto L159;
        $s0 = !(arg1.type == 15);
      L159:
        if (!$s0) goto L326;
        var $t1 = !this.tileAr[arg1.col + _local4][arg1.row].isDropping;
        $s0 = $t1;
        if (!$t1) goto L219;
        $s0 = !this.tileAr[arg1.col + _local4][arg1.row].onTween;
      L219:
        if (!$s0) goto L250;
        $s0 = !this.tileAr[arg1.col + _local4][arg1.row].removed;
      L250:
        if (!$s0) goto L281;
        $s0 = !this.tileAr[arg1.col + _local4][arg1.row].isSwapping;
      L281:
        if (!$s0) goto L318;
        _local3.push(this.tileAr[arg1.col + _local4][arg1.row].idx);
        goto L322;
      L318:
        goto L345;
      L322:
        goto L330;
      L326:
        goto L345;
      L330:
        _local4++;
      L332:
        if (arg1.col + _local4 < 8) goto L67;
      L345:
        _local4 = 1;
        goto L618;
      L353:
        var $t2 = this.tileAr[arg1.col - _local4][arg1.row].type == arg1.type;
        $s0 = $t2;
        if (!$t2) goto L417;
        $s0 = !(this.tileAr[arg1.col - _local4][arg1.row].type == 10);
      L417:
        if (!$s0) goto L431;
        $s0 = !(arg1.type == 10);
      L431:
        if (!$s0) goto L445;
        $s0 = !(arg1.type == 15);
      L445:
        if (!$s0) goto L612;
        var $t3 = !this.tileAr[arg1.col - _local4][arg1.row].isDropping;
        $s0 = $t3;
        if (!$t3) goto L505;
        $s0 = !this.tileAr[arg1.col - _local4][arg1.row].onTween;
      L505:
        if (!$s0) goto L536;
        $s0 = !this.tileAr[arg1.col - _local4][arg1.row].removed;
      L536:
        if (!$s0) goto L567;
        $s0 = !this.tileAr[arg1.col - _local4][arg1.row].isSwapping;
      L567:
        if (!$s0) goto L604;
        _local3.push(this.tileAr[arg1.col - _local4][arg1.row].idx);
        goto L608;
      L604:
        goto L631;
      L608:
        goto L616;
      L612:
        goto L631;
      L616:
        _local4++;
      L618:
        if (arg1.col - _local4 >= 0) goto L353;
      L631:
        if (!(_local3.length > 2)) goto L647;
        _local2.push(_local3);
      L647:
        var $t4 = new Vector.<int>();
        var _local5 = $t4;
        $t4.push(this.tileAr[arg1.col][arg1.row].idx);
        _local4 = 1;
        goto L961;
      L695:
        var $t5 = this.tileAr[arg1.col][arg1.row + _local4].type == arg1.type;
        $s0 = $t5;
        if (!$t5) goto L759;
        $s0 = !(this.tileAr[arg1.col][arg1.row + _local4].type == 10);
      L759:
        if (!$s0) goto L773;
        $s0 = !(arg1.type == 10);
      L773:
        if (!$s0) goto L787;
        $s0 = !(arg1.type == 15);
      L787:
        if (!$s0) goto L955;
        var $t6 = !this.tileAr[arg1.col][arg1.row + _local4].isDropping;
        $s0 = $t6;
        if (!$t6) goto L847;
        $s0 = !this.tileAr[arg1.col][arg1.row + _local4].onTween;
      L847:
        if (!$s0) goto L878;
        $s0 = !this.tileAr[arg1.col][arg1.row + _local4].removed;
      L878:
        if (!$s0) goto L909;
        $s0 = !this.tileAr[arg1.col][arg1.row + _local4].isSwapping;
      L909:
        if (!$s0) goto L947;
        _local5.push(this.tileAr[arg1.col][arg1.row + _local4].idx);
        goto L951;
      L947:
        goto L974;
      L951:
        goto L959;
      L955:
        goto L974;
      L959:
        _local4++;
      L961:
        if (arg1.row + _local4 < 8) goto L695;
      L974:
        _local4 = 1;
        goto L1248;
      L982:
        var $t7 = this.tileAr[arg1.col][arg1.row - _local4].type == arg1.type;
        $s0 = $t7;
        if (!$t7) goto L1046;
        $s0 = !(this.tileAr[arg1.col][arg1.row - _local4].type == 10);
      L1046:
        if (!$s0) goto L1060;
        $s0 = !(arg1.type == 10);
      L1060:
        if (!$s0) goto L1074;
        $s0 = !(arg1.type == 15);
      L1074:
        if (!$s0) goto L1242;
        var $t8 = !this.tileAr[arg1.col][arg1.row - _local4].isDropping;
        $s0 = $t8;
        if (!$t8) goto L1134;
        $s0 = !this.tileAr[arg1.col][arg1.row - _local4].onTween;
      L1134:
        if (!$s0) goto L1165;
        $s0 = !this.tileAr[arg1.col][arg1.row - _local4].removed;
      L1165:
        if (!$s0) goto L1196;
        $s0 = !this.tileAr[arg1.col][arg1.row - _local4].isSwapping;
      L1196:
        if (!$s0) goto L1234;
        _local5.push(this.tileAr[arg1.col][arg1.row - _local4].idx);
        goto L1238;
      L1234:
        goto L1261;
      L1238:
        goto L1246;
      L1242:
        goto L1261;
      L1246:
        _local4++;
      L1248:
        if (arg1.row - _local4 >= 0) goto L982;
      L1261:
        if (!(_local5.length > 2)) goto L1279;
        _local2.push(_local5);
      L1279:
        return _local2;
    }

    function shuffleTile():void {
        var _local2 = uint(0);
        var _local3 = uint(0);
        var _local1 = uint(0);
        goto L710;
      L18:
        _local2 = uint(0);
        goto L699;
      L27:
        var $t0 = !(this.tileAr[_local1][_local2].type == 10);
        $s0 = $t0;
        if (!$t0) goto L72;
        $s0 = !(this.tileAr[_local1][_local2].type == 15);
      L72:
        if (!$s0) goto L119;
        this.tileAr[_local1][_local2].type = this.availableTypeAr[this._global._maxCrypto.RandomRange(0, this.availableTypeAr.length - 1)];
      L119:
        _local3 = uint(this.tileAr[_local1][_local2].type);
        if (_local3 != 1) goto L173;
        this.tileAr[_local1][_local2].texture = this.gameAtlas.getTexture("bread");
        goto L613;
      L173:
        if (_local3 != 2) goto L210;
        this.tileAr[_local1][_local2].texture = this.gameAtlas.getTexture("selada");
        goto L613;
      L210:
        if (_local3 != 3) goto L247;
        this.tileAr[_local1][_local2].texture = this.gameAtlas.getTexture("beef");
        goto L613;
      L247:
        if (_local3 != 4) goto L284;
        this.tileAr[_local1][_local2].texture = this.gameAtlas.getTexture("ayam");
        goto L613;
      L284:
        if (_local3 != 5) goto L321;
        this.tileAr[_local1][_local2].texture = this.gameAtlas.getTexture("nasi");
        goto L613;
      L321:
        if (_local3 != 6) goto L358;
        this.tileAr[_local1][_local2].texture = this.gameAtlas.getTexture("bacon");
        goto L613;
      L358:
        if (_local3 != 7) goto L395;
        this.tileAr[_local1][_local2].texture = this.gameAtlas.getTexture("salmon");
        goto L613;
      L395:
        if (_local3 != 8) goto L432;
        this.tileAr[_local1][_local2].texture = this.gameAtlas.getTexture("ikan");
        goto L613;
      L432:
        if (_local3 != 9) goto L469;
        this.tileAr[_local1][_local2].texture = this.gameAtlas.getTexture("keju");
        goto L613;
      L469:
        if (_local3 != 10) goto L506;
        this.tileAr[_local1][_local2].texture = this.gameAtlas.getTexture("cookies");
        goto L613;
      L506:
        if (_local3 != 11) goto L543;
        this.tileAr[_local1][_local2].texture = this.gameAtlas.getTexture("pita_bread");
        goto L613;
      L543:
        if (_local3 != 12) goto L580;
        this.tileAr[_local1][_local2].texture = this.gameAtlas.getTexture("lamb");
        goto L613;
      L580:
        if (_local3 != 13) goto L613;
        this.tileAr[_local1][_local2].texture = this.gameAtlas.getTexture("tomato");
      L613:
        this.tileAr[_local1][_local2].readjustSize();
        this.tileAr[_local1][_local2].pivotX = this.tileAr[_local1][_local2].width / 2;
        this.tileAr[_local1][_local2].pivotY = this.tileAr[_local1][_local2].height / 2;
        _local2 = uint(_local2 + 1);
      L699:
        if (_local2 < 8) goto L27;
        _local1 = uint(_local1 + 1);
      L710:
        if (_local1 < 8) goto L18;
    }

    function findAndRemoveOneMatch():void {
        matches = null;
        tempType = 0;
        j = 0;
        horzCount = 0;
        vertCount = 0;
        col = 0;
        getMatch5 = false;
        getMatch6orMore = false;
        n = 0;
        row = 0;
        m = 0;
        temp = new Vector.<int>();
        tempTorL = new Vector.<Match3Object>();
        matches = this.lookForMatches();
        length = int(matches.length);
        if (!(length > 0)) goto L148;
        this.highlightPossibleTile = false;
        this.idleDuration = 0;
      L148:
        goodMovesLength = 3;
        i = 0;
        goto L1932;
      L164:
        if (!(matches[i].length >= 4)) goto L204;
        goodMovesLength = int(matches[i].length);
      L204:
        j = 0;
        goto L1762;
      L214:
        horzCount = 1;
        vertCount = 1;
        col = 0;
        goto L1201;
      L237:
        row = 0;
        goto L1181;
      L248:
        if (this.tileAr[col][row].idx != matches[i][j]) goto L1171;
        tempType = int(this.tileAr[col][row].type);
        if (this.tileAr[col][row].removed == true) goto L601;
        this.tileAr[col][row].removed = true;
        this.tileAr[col][row].filter = null;
        if (matches[i].length != 3) goto L434;
        this.tileAr[col][row].asMatch3Part = true;
        goto L568;
      L434:
        if (matches[i].length != 4) goto L480;
        this.tileAr[col][row].asMatch4Part = true;
        goto L568;
      L480:
        if (matches[i].length != 5) goto L526;
        this.tileAr[col][row].asMatch5Part = true;
        goto L568;
      L526:
        if (!(matches[i].length > 5)) goto L568;
        this.tileAr[col][row].asMatchMorePart = true;
      L568:
        temp.push(this.tileAr[col][row].idx);
        goto L1171;
      L601:
        tempTorL.push(this.tileAr[col][row]);
        m = 1;
        goto L748;
      L637:
        if (this.tileAr[col][row].type != this.tileAr[col][row - m].type) goto L734;
        vertCount = vertCount + 1;
        tempTorL.push(this.tileAr[col][row - m]);
        goto L738;
      L734:
        goto L763;
      L738:
        m = m + 1;
      L748:
        if (row - m >= 0) goto L637;
      L763:
        m = 1;
        goto L884;
      L773:
        if (this.tileAr[col][row].type != this.tileAr[col][row + m].type) goto L870;
        vertCount = vertCount + 1;
        tempTorL.push(this.tileAr[col][row + m]);
        goto L874;
      L870:
        goto L899;
      L874:
        m = m + 1;
      L884:
        if (row + m < 8) goto L773;
      L899:
        m = 1;
        goto L1020;
      L909:
        if (this.tileAr[col][row].type != this.tileAr[col - m][row].type) goto L1006;
        horzCount = horzCount + 1;
        tempTorL.push(this.tileAr[col - m][row]);
        goto L1010;
      L1006:
        goto L1035;
      L1010:
        m = m + 1;
      L1020:
        if (col - m >= 0) goto L909;
      L1035:
        m = 1;
        goto L1156;
      L1045:
        if (this.tileAr[col][row].type != this.tileAr[col + m][row].type) goto L1142;
        horzCount = horzCount + 1;
        tempTorL.push(this.tileAr[col + m][row]);
        goto L1146;
      L1142:
        goto L1171;
      L1146:
        m = m + 1;
      L1156:
        if (col + m < 8) goto L1045;
      L1171:
        row = row + 1;
      L1181:
        if (row < 8) goto L248;
        col = col + 1;
      L1201:
        if (col < 8) goto L237;
        var $t0 = horzCount == 3;
        $s0 = $t0;
        if (!$t0) goto L1231;
        $s0 = vertCount == 3;
      L1231:
        if ($s0) goto L1257;
        var $t1 = horzCount > 3;
        $s0 = $t1;
        if ($t1) goto L1257;
        $s0 = vertCount > 3;
      L1257:
        if (!$s0) goto L1261;
      L1261:
        n = 0;
        goto L1728;
      L1271:
        var $t2 = horzCount == 3;
        $s0 = $t2;
        if (!$t2) goto L1292;
        $s0 = vertCount == 3;
      L1292:
        if (!$s0) goto L1489;
        this.tileAr[tempTorL[n].col][tempTorL[n].row].asMatch3Part = false;
        this.tileAr[tempTorL[n].col][tempTorL[n].row].asMatch4Part = false;
        this.tileAr[tempTorL[n].col][tempTorL[n].row].asMatchMorePart = false;
        this.tileAr[tempTorL[n].col][tempTorL[n].row].asMatch5Part = true;
        getMatch5 = true;
        if (!(goodMovesLength < 5)) goto L1485;
        goodMovesLength = 5;
      L1485:
        goto L1718;
      L1489:
        var $t3 = horzCount > 3;
        $s0 = $t3;
        if ($t3) goto L1509;
        $s0 = vertCount > 3;
      L1509:
        if (!$s0) goto L1718;
        this.tileAr[tempTorL[n].col][tempTorL[n].row].asMatch3Part = false;
        this.tileAr[tempTorL[n].col][tempTorL[n].row].asMatch4Part = false;
        this.tileAr[tempTorL[n].col][tempTorL[n].row].asMatch5Part = false;
        this.tileAr[tempTorL[n].col][tempTorL[n].row].asMatchMorePart = true;
        getMatch6orMore = true;
        if (!(goodMovesLength < horzCount + vertCount)) goto L1718;
        goodMovesLength = int(horzCount + vertCount - 1);
      L1718:
        n = n + 1;
      L1728:
        if (n < tempTorL.length) goto L1271;
        tempTorL.length = 0;
        j = j + 1;
      L1762:
        if (j < matches[i].length) goto L214;
        if (tempType != 3) goto L1812;
        this._global._soundAsset.playSFX("beef_sizzle");
        goto L1922;
      L1812:
        if (tempType != 4) goto L1840;
        this._global._soundAsset.playSFX("chicken_sizzle");
        goto L1922;
      L1840:
        if (tempType != 7) goto L1868;
        this._global._soundAsset.playSFX("fish_sizzle");
        goto L1922;
      L1868:
        if (tempType != 6) goto L1896;
        this._global._soundAsset.playSFX("pork_sizzle");
        goto L1922;
      L1896:
        this._global._soundAsset.playSFX("other_ingredient_matched_v" + TGlobalClass.RandomRange(1, 2));
      L1922:
        i = i + 1;
      L1932:
        if (i < matches.length) goto L164;
        if (!(goodMovesLength >= 4)) goto L1966;
        this.showGoodMovesText(goodMovesLength);
      L1966:
        if (!(TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_MATCH_BONUS)) goto L2084;
        if (!(goodMovesLength >= 5)) goto L2084;
        ScoreManager.updateScore(goodMovesLength - 4);
        var $t4 = this._global._maxCrypto.STI(ScoreManager.totalScore) >= this.expertTarget;
        $s0 = $t4;
        if (!$t4) goto L2047;
        $s0 = this.goalExpertText.idx == 2;
      L2047:
        if (!$s0) goto L2084;
        this.goalExpertText.idx = 3;
        TGlobalClass._windowDisplay.showGoalIconSmall();
        this._global._soundAsset.playSFX("expert_reached");
      L2084:
        hasFrozenRemoved = false;
        col = 0;
        goto L2683;
      L2099:
        row = 0;
        goto L2663;
      L2110:
        i = 0;
        goto L2638;
      L2121:
        if (this.tileAr[col][row].idx != temp[i]) goto L2628;
        if (!this.tileAr[col][row].frozen) goto L2385;
        $s0 = this._global._soundAsset;
        $s1 = "ice_break";
        if (!(TGlobalClass.RandomRange(1, 2) == 1)) goto L2218;
        $s2 = "";
        goto L2222;
      L2218:
        $s2 = "_v2";
      L2222:
        $s0.playSFX($s1 + $s2);
        this.tileAr[col][row].frozen = false;
        this.tileAr[col][row].removed = false;
        this.frozenTileAr[this.tileAr[col][row].frozenTileIdx].visible = false;
        this.frozenTileAr[this.tileAr[col][row].frozenTileIdx].x = -100;
        this.tileAr[col][row].frozenTileIdx = 0;
        var _local2 = this;
        var _local3 = this.currentFrozenTile - 1;
        _local2.currentFrozenTile = _local3;
        hasFrozenRemoved = true;
        goto L2624;
      L2385:
        var $t5 = this.tileAr[col][row].type == 2;
        $s0 = $t5;
        if ($t5) goto L2439;
        $s0 = this.tileAr[col][row].type == 3;
      L2439:
        if ($s0) goto L2469;
        $s0 = this.tileAr[col][row].type == 6;
      L2469:
        if ($s0) goto L2499;
        $s0 = this.tileAr[col][row].type == 7;
      L2499:
        if ($s0) goto L2529;
        $s0 = this.tileAr[col][row].type == 8;
      L2529:
        if ($s0) goto L2559;
        $s0 = this.tileAr[col][row].type == 12;
      L2559:
        if (!$s0) goto L2584;
        this.playBoardEffect(this.PLAY_ASAP, col, row);
        goto L2601;
      L2584:
        this.playBoardEffect(this.PLAY_STAR, col, row);
      L2601:
        this.affectAbove(this.tileAr[col][row]);
      L2624:
        goto L2653;
      L2628:
        i = i + 1;
      L2638:
        if (i < temp.length) goto L2121;
      L2653:
        row = row + 1;
      L2663:
        if (row < 8) goto L2110;
        col = col + 1;
      L2683:
        if (col < 8) goto L2099;
        temp.length = 0;
        this.addNewPieces();
        if (!hasFrozenRemoved) goto L2720;
        this.findAndRemoveOneMatch();
      L2720:
        var $t6 = matches.length <= 0;
        $s0 = $t6;
        if (!$t6) goto L2741;
        $s0 = !this.moveTile;
      L2741:
        if (!$s0) goto L2836;
        if (!this.gameover) goto L2757;
        goto L2836;
      L2757:
        if (!this.levelComplete) goto L2774;
        this.levelComplete = false;
        goto L2836;
      L2774:
        if (this.lookForPossibles() == true) goto L2836;
        this._blackScreen.visible = true;
        this.noPossibleMoveText.visible = true;
        TweenLite.to(this.noPossibleMoveText, 1.5, {"x": 512, "ease": Expo.easeOut, "onComplete": function():void {
            noMoreMoves = true;
            tweenAllDown();
            TweenLite.to(noPossibleMoveText, 1, {"x": 1524, "ease": Expo.easeIn, "onComplete": function():void {
                noPossibleMoveText.x = 4294966796;
            }});
        }});
      L2836:
    }

    function playBoardEffect(arg1:int, arg2:int, arg3:int):void {
        var _local4 = 0;
        _local4 = 0;
        goto L520;
      L14:
        if (arg1 != this.PLAY_API) goto L183;
        if (this.efekApiAr[_local4].visible) goto L179;
        this.efekApiAr[_local4].visible = true;
        this.efekApiAr[_local4].x = this.tileAr[arg2][arg3].x;
        this.efekApiAr[_local4].y = this.tileAr[arg2][arg3].y;
        Starling.juggler.add(this.efekApiAr[_local4]);
        this.efekApiAr[_local4].currentFrame = 0;
        this.efekApiAr[_local4].play();
        this.efekApiAr[_local4].addEventListener(Event.COMPLETE, this.onEffectComplete);
        goto L528;
      L179:
        goto L518;
      L183:
        if (arg1 != this.PLAY_ASAP) goto L354;
        if (this.efekAsapAr[_local4].visible) goto L350;
        this.efekAsapAr[_local4].visible = true;
        this.efekAsapAr[_local4].x = this.tileAr[arg2][arg3].x;
        this.efekAsapAr[_local4].y = this.tileAr[arg2][arg3].y - 10;
        Starling.juggler.add(this.efekAsapAr[_local4]);
        this.efekAsapAr[_local4].currentFrame = 0;
        this.efekAsapAr[_local4].play();
        this.efekAsapAr[_local4].addEventListener(Event.COMPLETE, this.onEffectComplete);
        goto L528;
      L350:
        goto L518;
      L354:
        if (arg1 != this.PLAY_STAR) goto L518;
        if (this.efekStarAr[_local4].visible) goto L518;
        this.efekStarAr[_local4].visible = true;
        this.efekStarAr[_local4].x = this.tileAr[arg2][arg3].x;
        this.efekStarAr[_local4].y = this.tileAr[arg2][arg3].y;
        Starling.juggler.add(this.efekStarAr[_local4]);
        this.efekStarAr[_local4].currentFrame = 0;
        this.efekStarAr[_local4].play();
        this.efekStarAr[_local4].addEventListener(Event.COMPLETE, this.onEffectComplete);
        goto L528;
      L518:
        _local4++;
      L520:
        if (_local4 < 20) goto L14;
      L528:
    }

    function onEffectComplete(arg1:Event):void {
        var _local2 = arg1.currentTarget as MovieClip;
        _local2.visible = false;
        _local2.removeEventListener(Event.COMPLETE, this.onEffectComplete);
        _local2.x = -100;
        _local2.currentFrame = 0;
        Starling.juggler.remove(_local2);
    }

    function tweenAllDown():void {
        count = 0;
        row = 0;
        count = 0;
        levelTargetCompleted = Boolean(this.levelComplete);
        this._blackScreen.visible = true;
        col = 0;
        goto L444;
      L51:
        row = 0;
        goto L424;
      L62:
        this.tileAr[col][row].removed = false;
        this.tileAr[col][row].aktif = false;
        TweenLite.killTweensOf(this.tileAr[col][row], {"scaleX": true, "scaleY": true});
        this.tileAr[col][row].asMatch3Part = false;
        this.tileAr[col][row].asMatch4Part = false;
        this.tileAr[col][row].asMatch5Part = false;
        this.tileAr[col][row].asMatchMorePart = false;
        this.tileAr[col][row].isDropping = false;
        this.tileAr[col][row].onTween = true;
        this.tileAr[col][row].isSwapping = false;
        TweenLite.to(this.tileAr[col][row], 1 - this.tileAr[col][row].row * 0.05, {"y": 700, "delay": 0.08 * (9 - this.tileAr[col][row].row), "ease": Linear.easeNone, "onComplete": function(f1_arg1:int, f1_arg2:int):void {
            var _local3 = count;
            var _local4 = count + 1;
            _local3.count = _local4;
            tileAr[f1_arg1][f1_arg2].onTween = false;
            tileAr[f1_arg1][f1_arg2].y = -80;
            if (!(count >= 64)) goto L138;
            goto L113;
          L65:
            shuffleTile();
            if (lookForMatches().length == 0) goto L93;
            goto L113;
          L93:
            if (lookForPossibles() != false) goto L109;
            goto L113;
          L109:
            goto L117;
          L113:
            goto L65;
          L117:
            moveTile = true;
            _blackScreen.visible = false;
            noMoreMoves = false;
          L138:
        }, "onCompleteParams": [col, row]});
        row = row + 1;
      L424:
        if (row < 8) goto L62;
        col = col + 1;
      L444:
        if (col < 8) goto L51;
        i = 0;
        goto L571;
      L464:
        if (!this.frozenTileAr[i].visible) goto L561;
        TweenLite.to(this.frozenTileAr[i], 1 - this.frozenTileAr[i].attachedRow * 0.05, {"y": 700, "delay": 0.08 * (9 - this.frozenTileAr[i].attachedRow), "ease": Linear.easeNone});
      L561:
        i = i + 1;
      L571:
        if (i < 20) goto L464;
    }

    function affectAbove(arg1:Match3Object):void {
        var _local3 = null;
        var _local2 = int(arg1.row - 1);
        goto L279;
      L17:
        if (this.tileAr[arg1.col][_local2].removed != false) goto L276;
        if (!this.tileAr[arg1.col][_local2].frozen) goto L108;
        var $t0 = this.frozenTileAr[this.tileAr[arg1.col][_local2].frozenTileIdx];
        var _local4 = $t0;
        var _local5 = $t0.attachedRow + 1;
        _local4.attachedRow = _local5;
      L108:
        _local3 = this.tileAr[arg1.col][_local2 + 1];
        var $t1 = this.tileAr[arg1.col][_local2];
        _local4 = $t1;
        _local5 = $t1.row + 1;
        _local4.row = _local5;
        this.tileAr[arg1.col][_local2 + 1] = this.tileAr[arg1.col][_local2];
        this.tileAr[arg1.col][_local2] = _local3;
        this.tileAr[arg1.col][_local2].removed = true;
        this.tileAr[arg1.col][_local2 + 1].removed = false;
        this.tileAr[arg1.col][_local2 + 1].isDropping = true;
      L276:
        _local2 = _local2 - 1;
      L279:
        if (_local2 >= 0) goto L17;
    }

    function addNewPieces():void {
        missingPieces = 0;
        row = 0;
        notComplete = false;
        col = 0;
        goto L506;
      L33:
        missingPieces = 0;
        row = 7;
        goto L486;
      L50:
        if (this.tileAr[col][row].removed != true) goto L476;
        if (this.tileAr[col][row].onTween) goto L476;
        if (!(TGlobalClass.currentPlayedLevel >= TGlobalClass.UNLOCK_COOKIES_AT)) goto L135;
        var _local2 = this;
        var _local3 = this.removedTileCountCookies + 1;
        _local2.removedTileCountCookies = _local3;
      L135:
        if (!this.fryerLayer.visible) goto L163;
        _local2 = this;
        _local3 = this.removedTileCountFries + 1;
        _local2.removedTileCountFries = _local3;
      L163:
        if (!this.juicerLayer.visible) goto L191;
        _local2 = this;
        _local3 = this.removedTileCountJuice + 1;
        _local2.removedTileCountJuice = _local3;
      L191:
        if (!(TGlobalClass.currentPlayedLevel >= TGlobalClass.UNLOCK_TORCH_AT)) goto L224;
        _local2 = this;
        _local3 = this.removedTileTorch + 1;
        _local2.removedTileTorch = _local3;
      L224:
        if (this.tileAr[col][row].type != 10) goto L265;
        this._global._soundAsset.playSFX("heartburst");
      L265:
        if (!this.fryerLayer.visible) goto L306;
        this.fryerPercentage.text = int(this.removedTileCountFries / this.friesTarget * 100) + "%";
      L306:
        if (!this.juicerLayer.visible) goto L347;
        this.juicerPercentage.text = int(this.removedTileCountJuice / this.juiceTarget * 100) + "%";
      L347:
        this.checkCustOrderPrior(col, row);
        this.checkCustOrder(col, row);
        missingPieces = missingPieces + 1;
        this.tileAr[col][row].onTween = true;
        TweenLite.to(this.tileAr[col][row], 0.1, {"scaleX": 1.2, "scaleY": 1.2, "ease": Linear.easeNone, "onComplete": function(f1_arg1:int, f1_arg2:int, f1_arg3:int):void {
            column = f1_arg1;
            rows = f1_arg2;
            missPiece = f1_arg3;
            TweenLite.to(tileAr[column][rows], 0.1, {"scaleX": 0, "scaleY": 0, "ease": Linear.easeNone, "onComplete": function(f2_arg1:int, f2_arg2:int, f2_arg3:int):void {
                var _local6 = undefined;
                var _local7 = undefined;
                tileAr[f2_arg1][f2_arg2].removed = false;
                tileAr[f2_arg1][f2_arg2].aktif = false;
                tileAr[f2_arg1][f2_arg2].asMatch3Part = false;
                tileAr[f2_arg1][f2_arg2].asMatch4Part = false;
                tileAr[f2_arg1][f2_arg2].asMatch5Part = false;
                tileAr[f2_arg1][f2_arg2].asMatchMorePart = false;
                tileAr[f2_arg1][f2_arg2].onTween = false;
                tileAr[f2_arg1][f2_arg2].frozen = false;
                tileAr[f2_arg1][f2_arg2].used = false;
                tileAr[f2_arg1][f2_arg2].y = 60 - 70 - 69 * f2_arg3;
                if (!(tileAr[f2_arg1][f2_arg2].vy <= 0)) goto L187;
              L187:
                tileAr[f2_arg1][f2_arg2].scaleX = 1;
                tileAr[f2_arg1][f2_arg2].scaleY = 1;
                tileAr[f2_arg1][f2_arg2].alpha = 1;
                tileAr[f2_arg1][f2_arg2].isDropping = true;
                tileAr[f2_arg1][f2_arg2].col = f2_arg1;
                tileAr[f2_arg1][f2_arg2].row = f2_arg2;
                var $t0 = _global._maxCrypto.RandomRange(1, 100);
                var _local4 = $t0;
                if (!($t0 <= 25)) goto L388;
                tileAr[f2_arg1][f2_arg2].type = getWantedIngredient();
                if (tileAr[f2_arg1][f2_arg2].type != 0) goto L384;
                tileAr[f2_arg1][f2_arg2].type = availableTypeAr[_global._maxCrypto.RandomRange(0, availableTypeAr.length - 1)];
              L384:
                goto L427;
              L388:
                tileAr[f2_arg1][f2_arg2].type = availableTypeAr[_global._maxCrypto.RandomRange(0, availableTypeAr.length - 1)];
              L427:
                var $t1 = tileAr[f2_arg1][f2_arg2].type;
                var _local5 = $t1;
                if ($t1 != 1) goto L479;
                tileAr[f2_arg1][f2_arg2].texture = gameAtlas.getTexture("bread");
                goto L907;
              L479:
                if (_local5 != 2) goto L515;
                tileAr[f2_arg1][f2_arg2].texture = gameAtlas.getTexture("selada");
                goto L907;
              L515:
                if (_local5 != 3) goto L551;
                tileAr[f2_arg1][f2_arg2].texture = gameAtlas.getTexture("beef");
                goto L907;
              L551:
                if (_local5 != 4) goto L587;
                tileAr[f2_arg1][f2_arg2].texture = gameAtlas.getTexture("ayam");
                goto L907;
              L587:
                if (_local5 != 5) goto L623;
                tileAr[f2_arg1][f2_arg2].texture = gameAtlas.getTexture("nasi");
                goto L907;
              L623:
                if (_local5 != 6) goto L659;
                tileAr[f2_arg1][f2_arg2].texture = gameAtlas.getTexture("bacon");
                goto L907;
              L659:
                if (_local5 != 7) goto L695;
                tileAr[f2_arg1][f2_arg2].texture = gameAtlas.getTexture("salmon");
                goto L907;
              L695:
                if (_local5 != 8) goto L731;
                tileAr[f2_arg1][f2_arg2].texture = gameAtlas.getTexture("ikan");
                goto L907;
              L731:
                if (_local5 != 9) goto L767;
                tileAr[f2_arg1][f2_arg2].texture = gameAtlas.getTexture("keju");
                goto L907;
              L767:
                if (_local5 != 10) goto L803;
                tileAr[f2_arg1][f2_arg2].texture = gameAtlas.getTexture("cookies");
                goto L907;
              L803:
                if (_local5 != 11) goto L839;
                tileAr[f2_arg1][f2_arg2].texture = gameAtlas.getTexture("pita_bread");
                goto L907;
              L839:
                if (_local5 != 12) goto L875;
                tileAr[f2_arg1][f2_arg2].texture = gameAtlas.getTexture("lamb");
                goto L907;
              L875:
                if (_local5 != 13) goto L907;
                tileAr[f2_arg1][f2_arg2].texture = gameAtlas.getTexture("tomato");
              L907:
                tileAr[f2_arg1][f2_arg2].readjustSize();
                tileAr[f2_arg1][f2_arg2].pivotX = tileAr[f2_arg1][f2_arg2].width / 2;
                tileAr[f2_arg1][f2_arg2].pivotY = tileAr[f2_arg1][f2_arg2].height / 2;
                if (!(TGlobalClass.currentPlayedLevel >= TGlobalClass.UNLOCK_FROZEN_AT)) goto L1227;
                if (!(currentFrozenTile < maxFrozenTile)) goto L1227;
                var $t2 = TGlobalClass.RandomRange(0, 100);
                _local6 = $t2;
                if (!($t2 < 5)) goto L1227;
                tileAr[f2_arg1][f2_arg2].frozen = true;
                _local7 = 0;
                goto L1192;
              L1055:
                if (frozenTileAr[_local7].visible) goto L1186;
                frozenTileAr[_local7].visible = true;
                frozenTileAr[_local7].x = tileAr[f2_arg1][f2_arg2].x;
                frozenTileAr[_local7].y = tileAr[f2_arg1][f2_arg2].y;
                frozenTileAr[_local7].attachedCol = f2_arg1;
                frozenTileAr[_local7].attachedRow = f2_arg2;
                tileAr[f2_arg1][f2_arg2].frozenTileIdx = frozenTileAr[_local7].idx;
                goto L1204;
              L1186:
                _local7 = _local7 + 1;
              L1192:
                if (_local7 < frozenTileAr.length) goto L1055;
              L1204:
                var _local8 = currentFrozenTile;
                var _local9 = currentFrozenTile + 1;
                _local8.currentFrozenTile = _local9;
              L1227:
                if (!(removedTileTorch >= torchTarget)) goto L1380;
                if (tileAr[f2_arg1][f2_arg2].frozen) goto L1380;
                tileAr[f2_arg1][f2_arg2].type = 15;
                tileAr[f2_arg1][f2_arg2].texture = gameAtlas.getTexture("chef_torch");
                tileAr[f2_arg1][f2_arg2].readjustSize();
                tileAr[f2_arg1][f2_arg2].pivotX = tileAr[f2_arg1][f2_arg2].width / 2;
                tileAr[f2_arg1][f2_arg2].pivotY = tileAr[f2_arg1][f2_arg2].height / 2;
                removedTileTorch = 0;
              L1380:
                moveTile = true;
            }, "onCompleteParams": [column, rows, missPiece]});
        }, "onCompleteParams": [col, row, missingPieces]});
      L476:
        row = row - 1;
      L486:
        if (row >= 0) goto L50;
        col = col + 1;
      L506:
        if (col < 8) goto L33;
        if (!(this.removedTileCountFries >= this.friesTarget)) goto L574;
        this._global._soundAsset.playSFX("fries_machine_ready");
        this.removedTileCountFries = 0;
        this.fryerPercentage.text = "Drag to Customer";
        this.fryerLayer.visible = false;
        this.fries.visible = true;
      L574:
        if (!(this.removedTileCountJuice >= this.juiceTarget)) goto L632;
        this._global._soundAsset.playSFX("beverage_machine_ready");
        this.removedTileCountJuice = 0;
        this.juicerPercentage.text = "Drag to Customer";
        this.juicerLayer.visible = false;
        this.drink.visible = true;
      L632:
        this.tileTypeAr.length = 0;
        col = 0;
        goto L722;
      L651:
        row = 0;
        goto L702;
      L662:
        this.tileTypeAr.push(this.tileAr[col][row].type);
        row = row + 1;
      L702:
        if (row < 8) goto L662;
        col = col + 1;
      L722:
        if (col < 8) goto L651;
    }

    function tweenShake(arg1:FrozenTile, arg2:int = 1, arg3:int = 0):void {
        this._global._soundAsset.playSFX("ice_break");
        var _local4 = new TimelineLite();
        arg1.x = this.tileAr[arg1.attachedCol][arg1.attachedRow].x;
        arg1.y = this.tileAr[arg1.attachedCol][arg1.attachedRow].y;
        var _local5 = 6;
        _local4.to(arg1, 0.1, {"x": arg1.x + _local5 / 2 * arg2, "y": arg1.y + _local5 / 2 * arg3, "ease": Linear.easeNone});
        _local4.to(arg1, 0.1, {"x": arg1.x - _local5 * arg2, "y": arg1.y - _local5 * arg3, "ease": Linear.easeNone});
        _local4.to(arg1, 0.1, {"x": arg1.x + _local5 * arg2, "y": arg1.y + _local5 * arg3, "ease": Linear.easeNone});
        _local4.to(arg1, 0.1, {"x": arg1.x - _local5 / 2 * arg2, "y": arg1.y - _local5 / 2 * arg3, "ease": Linear.easeNone});
    }

    function getWantedIngredient():int {
        var _local3 = 0;
        var _local4 = 0;
        var _local5 = 0;
        var _local1 = new Vector.<int>();
        var _local2 = int(TGlobalClass.customerFocus);
        _local4 = 0;
        goto L182;
      L42:
        var $t0 = !this.customer[_local4].onTween;
        $s0 = $t0;
        if (!$t0) goto L75;
        $s0 = !this.customer[_local4].completed;
      L75:
        if (!$s0) goto L95;
        $s0 = this.customer[_local4].queueIdx == _local2;
      L95:
        if (!$s0) goto L180;
        _local5 = 0;
        goto L155;
      L107:
        if (!(this.customer[_local4].ingredientTarget[_local5] > 0)) goto L153;
        _local1.push(this.customer[_local4].ingredientType[_local5]);
      L153:
        _local5++;
      L155:
        if (_local5 < this.customer[_local4].ingredientTarget.length) goto L107;
        goto L190;
      L180:
        _local4++;
      L182:
        if (_local4 < 3) goto L42;
      L190:
        if (!(_local1.length > 0)) goto L224;
        _local3 = int(_local1[TGlobalClass.RandomRange(0, _local1.length - 1)]);
        goto L227;
      L224:
        _local3 = 0;
      L227:
        return _local3;
    }

    function remakeJuice():void {
        this.drink.visible = false;
        this.juicerLayer.visible = true;
        this.juicerPercentage.text = "0%";
    }

    function remakeFries():void {
        this.fries.visible = false;
        this.fryerLayer.visible = true;
        this.fryerPercentage.text = "0%";
    }

    function popCookiesOnBoard():void {
        var _local3 = 0;
        var _local4 = 0;
        var _local5 = 0;
        var _local6 = 0;
        if (!(this.cookiesCountOnBoard >= 5)) goto L28;
        return;
      L28:
        var _local1 = new Vector.<int>();
        var _local2 = new Vector.<int>();
        _local5 = 0;
        goto L168;
      L62:
        _local6 = 0;
        goto L158;
      L71:
        var $t0 = !this.tileAr[_local5][_local6].removed;
        $s0 = $t0;
        if (!$t0) goto L114;
        $s0 = !this.tileAr[_local5][_local6].onTween;
      L114:
        if (!$s0) goto L138;
        $s0 = !this.tileAr[_local5][_local6].frozen;
      L138:
        if (!$s0) goto L156;
        _local1.push(_local5);
        _local2.push(_local6);
      L156:
        _local6++;
      L158:
        if (_local6 < 8) goto L71;
        _local5++;
      L168:
        if (_local5 < 8) goto L62;
        goto L301;
      L180:
        _local3 = int(_local1[TGlobalClass.RandomRange(0, _local1.length - 1)]);
        _local4 = int(_local2[TGlobalClass.RandomRange(0, _local2.length - 1)]);
        var $t1 = Boolean(this.tileAr[_local3][_local4].frozen);
        $s0 = $t1;
        if ($t1) goto L264;
        $s0 = this.tileAr[_local3][_local4].type == 10;
      L264:
        if ($s0) goto L289;
        $s0 = this.tileAr[_local3][_local4].type == 15;
      L289:
        if (!$s0) goto L297;
        goto L301;
      L297:
        goto L305;
      L301:
        goto L180;
      L305:
        this.tileAr[_local3][_local4].type = 10;
        this.tileAr[_local3][_local4].texture = this.gameAtlas.getTexture("cookies");
        this.tileAr[_local3][_local4].readjustSize();
        this.tileAr[_local3][_local4].pivotX = this.tileAr[_local3][_local4].width / 2;
        this.tileAr[_local3][_local4].pivotY = this.tileAr[_local3][_local4].height / 2;
        var _local7 = this;
        var _local8 = this.cookiesCountOnBoard + 1;
        _local7.cookiesCountOnBoard = _local8;
    }

    function checkCustOrderPrior(arg1:int, arg2:int):void {
        j = 0;
        col = arg1;
        row = arg2;
        countFulfilled = 0;
        customerFocus = int(TGlobalClass.customerFocus);
        i = 0;
        goto L460;
      L49:
        var $t0 = !this.customer[i].onTween;
        $s0 = $t0;
        if (!$t0) goto L86;
        $s0 = !this.customer[i].completed;
      L86:
        if (!$s0) goto L111;
        $s0 = this.customer[i].queueIdx == customerFocus;
      L111:
        if (!$s0) goto L450;
        j = 0;
        goto L425;
      L125:
        var $t1 = this.tileAr[col][row].type == this.customer[i].ingredientType[j];
        $s0 = $t1;
        if (!$t1) goto L197;
        $s0 = !this.tileAr[col][row].used;
      L197:
        if (!$s0) goto L415;
        if (!(this.customer[i].ingredientTarget[j] > 0)) goto L415;
        this.startTweenFromBoard(col, row, i, j);
        this.tileAr[col][row].used = true;
        var $t2 = this.customer[i].ingredientTarget;
        var _local4 = $t2;
        var _local5 = j;
        var $t3 = $t2[j] - 1;
        var _local6 = $t3;
        _local4[_local5] = _local6;
        this.customer[i].ingredientTargetPreview[j].text = "x" + $t3;
        TweenLite.to(this.customer[i].ingredientTargetPreview[j], 0.2, {"scaleX": 1.2, "scaleY": 1.2, "ease": Linear.easeNone, "onComplete": function(f1_arg1:int, f1_arg2:int):void {
            TweenLite.to(customer[f1_arg1].ingredientTargetPreview[f1_arg2], 0.2, {"scaleX": 1, "scaleY": 1, "ease": Linear.easeNone});
        }, "onCompleteParams": [i, j]});
      L415:
        j = j + 1;
      L425:
        if (j < this.customer[i].ingredientType.length) goto L125;
      L450:
        i = i + 1;
      L460:
        if (i < 3) goto L49;
    }

    function startTweenFromBoard(arg1:int, arg2:int, arg3:int, arg4:int):void {
        textureName = null;
        countFulfilled = 0;
        loc2Glob = null;
        col = arg1;
        row = arg2;
        custIdx = arg3;
        ingIdx = arg4;
        i = 0;
        goto L634;
      L58:
        if (this.tweenTileAr[i].onTween) goto L624;
        this.tweenTileAr[i].visible = true;
        this.tweenTileAr[i].x = this.tileAr[col][row].x;
        this.tweenTileAr[i].y = this.tileAr[col][row].y;
        this.tweenTileAr[i].onTween = true;
        var $t0 = new Vector.<String>(13);
        $t0[0] = "bread";
        $t0[1] = "selada";
        $t0[2] = "beef";
        $t0[3] = "ayam";
        $t0[4] = "nasi";
        $t0[5] = "bacon";
        $t0[6] = "salmon";
        $t0[7] = "ikan";
        $t0[8] = "keju";
        $t0[9] = "cookies";
        $t0[10] = "pita_bread";
        $t0[11] = "lamb";
        $t0[12] = "tomato";
        textureName = $t0;
        this.tweenTileAr[i].texture = this.gameAtlas.getTexture(textureName[this.tileAr[col][row].type - 1]);
        this.tweenTileAr[i].readjustSize();
        this.tweenTileAr[i].pivotX = this.tweenTileAr[i].width / 2;
        this.tweenTileAr[i].pivotY = this.tweenTileAr[i].height / 2;
        countFulfilled = 0;
        loc2Glob = this.customer[custIdx].ingredientList[ingIdx].parent.localToGlobal(new Point(this.customer[custIdx].ingredientList[ingIdx].x, this.customer[custIdx].ingredientList[ingIdx].y));
        TweenLite.to(this.tweenTileAr[i], 0.5 + col * 0.05, {"x": loc2Glob.x, "y": loc2Glob.y, "ease": Linear.easeNone, "onComplete": function(f1_arg1:int, f1_arg2:int, f1_arg3:int):void {
            var _local4 = undefined;
            tweenTileAr[f1_arg1].visible = false;
            tweenTileAr[f1_arg1].x = 4294967146;
            tweenTileAr[f1_arg1].y = 4294967146;
            tweenTileAr[f1_arg1].onTween = false;
            var $t0 = !customer[f1_arg2].onTween;
            $s0 = $t0;
            if (!$t0) goto L86;
            $s0 = !customer[f1_arg2].completed;
          L86:
            if (!$s0) goto L223;
            _local4 = 0;
            goto L204;
          L99:
            if (!(customer[f1_arg2].ingredientTarget[_local4] <= 0)) goto L198;
            var _local5 = countFulfilled;
            var _local6 = countFulfilled + 1;
            _local5.countFulfilled = _local6;
            customer[f1_arg2].ingredientFulfilled[_local4] = true;
            customer[f1_arg2].ingredientCheckMark[_local4].visible = true;
            customer[f1_arg2].ingredientTargetPreview[_local4].visible = false;
          L198:
            _local4 = _local4 + 1;
          L204:
            if (_local4 < customer[f1_arg2].ingredientType.length) goto L99;
          L223:
            var $t1 = countFulfilled == customer[f1_arg2].ingredientType.length;
            $s0 = $t1;
            if (!$t1) goto L257;
            $s0 = !customer[f1_arg2].onTween;
          L257:
            if (!$s0) goto L274;
            $s0 = !customer[f1_arg2].completed;
          L274:
            if (!$s0) goto L666;
            _global._soundAsset.playSFX("money_drop_v" + TGlobalClass.RandomRange(1, 2));
            customer[f1_arg2].readyText.visible = true;
            TweenLite.to(customer[f1_arg2].readyText, 0.4, {"scaleX": 1, "scaleY": 1, "ease": Back.easeOut});
            customer[f1_arg2].tweenReadyText();
            TweenLite.to(coinsAr[f1_arg2], 0.4, {"scaleX": 1, "scaleY": 1, "ease": Back.easeOut});
            coinsAr[f1_arg2].visible = true;
            coinsAr[f1_arg2].idx = f1_arg2;
            dishesAr[f1_arg2].visible = true;
            if (TGlobalClass.currentRegionDish != ConstClass.WESTERN_DISH) goto L491;
            dishesAr[f1_arg2].texture = gameAtlas.getTexture(westernDishName[customer[f1_arg2].currentDish]);
            goto L614;
          L491:
            if (TGlobalClass.currentRegionDish != ConstClass.CHINESE_DISH) goto L551;
            dishesAr[f1_arg2].texture = gameAtlas.getTexture(chineseDishName[customer[f1_arg2].currentDish - TGlobalClass.TOTAL_WESTERN_DISH]);
            goto L614;
          L551:
            if (TGlobalClass.currentRegionDish != ConstClass.MID_EAST_DISH) goto L614;
            dishesAr[f1_arg2].texture = gameAtlas.getTexture(mideastDishName[customer[f1_arg2].currentDish - TGlobalClass.TOTAL_WESTERN_DISH - TGlobalClass.TOTAL_CHINESE_DISH]);
          L614:
            dishesAr[f1_arg2].readjustSize();
            dishesAr[f1_arg2].alignPivot("center", "bottom");
            customer[f1_arg2].setDishCompleteState();
            checkCustomerLeft();
            setNextPriorCust();
          L666:
        }, "onCompleteParams": [i, custIdx, ingIdx]});
        goto L644;
      L624:
        i = i + 1;
      L634:
        if (i < 25) goto L58;
      L644:
    }

    function checkCustomerLeft():Boolean {
        var _local1 = 0;
        var _local2 = 0;
        goto L51;
      L12:
        var $t0 = Boolean(this.customer[_local2].completed);
        $s0 = $t0;
        if (!$t0) goto L43;
        $s0 = !this.coinsAr[_local2].visible;
      L43:
        if (!$s0) goto L49;
        _local1++;
      L49:
        _local2++;
      L51:
        if (_local2 < 3) goto L12;
        var $t1 = Boolean(this.timesUp);
        $s0 = $t1;
        if (!$t1) goto L73;
        $s0 = _local1 >= 3;
      L73:
        if (!$s0) goto L84;
        this.showWindow();
        return true;
      L84:
        return false;
    }

    function showWindow():void {
        if (!this.calledShowWindow) goto L15;
        return;
      L15:
        this.calledShowWindow = true;
        if (!TGlobalClass.isPlayingSpecialLevel) goto L98;
        if (!(TGlobalClass.VIPServedRating > 0)) goto L92;
        this.VIPServeBonus = TGlobalClass.VIPServedRating * this._global._maxCrypto.STI(ScoreManager.totalScore) - this._global._maxCrypto.STI(ScoreManager.totalScore);
        goto L98;
      L92:
        this.VIPServeBonus = 0;
      L98:
        ScoreManager.totalScore = ScoreManager.totalScore + this.VIPServeBonus;
        TweenLite.delayedCall(2.5, function():void {
            var _local2 = 0;
            var _local3 = 0;
            var _local1 = 0;
            calledShowWindow = false;
            var $t0 = TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] == 0;
            $s0 = $t0;
            if (!$t0) goto L59;
            $s0 = TGlobalClass.currentPlayedLevel == TSavegame.currentPlayer.passedDays;
          L59:
            if (!$s0) goto L765;
            if (!(servedCustomerCount >= goalTarget)) goto L731;
            TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] = _global._maxCrypto.STI(ScoreManager.totalScore);
            var $t1 = TSavegame.currentPlayer;
            var _local4 = $t1;
            var _local5 = $t1.passedDays + 1;
            _local4.passedDays = _local5;
            _local2 = int(_global._maxCrypto.STI(ScoreManager.totalScore));
            if (TSavegame.currentPlayer.unlocked[Achievement.QUALIFIED_CHEF]) goto L332;
            var $t2 = TSavegame.currentPlayer.achievementCounter;
            _local4 = $t2;
            var $t3 = Achievement.QUALIFIED_CHEF;
            _local5 = $t3;
            var _local6 = $t2[$t3] + 1;
            _local4[_local5] = _local6;
            if (!(TSavegame.currentPlayer.achievementCounter[Achievement.QUALIFIED_CHEF] >= Achievement.ALL_TARGET[Achievement.QUALIFIED_CHEF])) goto L290;
            TSavegame.currentPlayer.unlocked[Achievement.QUALIFIED_CHEF] = true;
            Achievement.tweenAchievementNotif(Achievement.QUALIFIED_CHEF);
            _local1++;
            goto L332;
          L290:
            if (TSavegame.currentPlayer.achievementCounter[Achievement.QUALIFIED_CHEF] % 15 != 0) goto L332;
            Achievement.tweenAchievementProgress(Achievement.QUALIFIED_CHEF);
            _local1++;
          L332:
            if (!(_global._maxCrypto.STI(ScoreManager.totalScore) >= expertTarget)) goto L609;
            if (TSavegame.currentPlayer.unlocked[Achievement.INTERNATIONAL_CHEF]) goto L556;
            var $t4 = TSavegame.currentPlayer.achievementCounter;
            _local4 = $t4;
            var $t5 = Achievement.INTERNATIONAL_CHEF;
            _local5 = $t5;
            _local6 = $t4[$t5] + 1;
            _local4[_local5] = _local6;
            if (!(TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] >= Achievement.ALL_TARGET[Achievement.INTERNATIONAL_CHEF])) goto L504;
            TSavegame.currentPlayer.unlocked[Achievement.INTERNATIONAL_CHEF] = true;
            TweenLite.delayedCall(_local1 * 6, Achievement.tweenAchievementNotif, [Achievement.INTERNATIONAL_CHEF]);
            goto L556;
          L504:
            if (TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] % 15 != 0) goto L556;
            TweenLite.delayedCall(_local1 * 6, Achievement.tweenAchievementProgress, [Achievement.INTERNATIONAL_CHEF]);
          L556:
            TSavegame.currentPlayer.money = _global._maxCrypto.ITS(_global._maxCrypto.STI(TSavegame.currentPlayer.money) + 6);
            TGlobalClass.coinsGot = 6;
            goto L658;
          L609:
            TSavegame.currentPlayer.money = _global._maxCrypto.ITS(_global._maxCrypto.STI(TSavegame.currentPlayer.money) + 3);
            TGlobalClass.coinsGot = 3;
          L658:
            TSavegame.save();
            if (TSavegame.currentPlayer.passedDays != TGlobalClass.TOTAL_DAYS) goto L704;
            TGlobalClass._windowDisplay.showWindow(TWindowState.STORY_COMPLETE_ALL);
            goto L720;
          L704:
            TGlobalClass._windowDisplay.showWindow(TWindowState.STORY_LEVELCOMPLETE);
          L720:
            TGlobalClass.isPlayingSpecialLevel = false;
            goto L761;
          L731:
            gameover = true;
            TGlobalClass._windowDisplay.showWindow(TWindowState.STORY_LEVELFAILED);
            TGlobalClass.isPlayingSpecialLevel = false;
          L761:
            goto L1250;
          L765:
            if (!(servedCustomerCount >= goalTarget)) goto L1220;
            TGlobalClass.daysEarningPrev = TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel];
            if (!(TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] < _global._maxCrypto.STI(ScoreManager.totalScore))) goto L1193;
            var $t6 = _global._maxCrypto.STI(ScoreManager.totalScore) >= expertTarget;
            $s0 = $t6;
            if (!$t6) goto L885;
            $s0 = TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] < expertTarget;
          L885:
            if (!$s0) goto L1115;
            if (TSavegame.currentPlayer.unlocked[Achievement.INTERNATIONAL_CHEF]) goto L1066;
            var $t7 = TSavegame.currentPlayer.achievementCounter;
            _local4 = $t7;
            var $t8 = Achievement.INTERNATIONAL_CHEF;
            _local5 = $t8;
            _local6 = $t7[$t8] + 1;
            _local4[_local5] = _local6;
            if (!(TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] >= Achievement.ALL_TARGET[Achievement.INTERNATIONAL_CHEF])) goto L1026;
            TSavegame.currentPlayer.unlocked[Achievement.INTERNATIONAL_CHEF] = true;
            Achievement.tweenAchievementNotif(Achievement.INTERNATIONAL_CHEF);
            goto L1066;
          L1026:
            if (TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] % 15 != 0) goto L1066;
            Achievement.tweenAchievementProgress(Achievement.INTERNATIONAL_CHEF);
          L1066:
            TSavegame.currentPlayer.money = _global._maxCrypto.ITS(_global._maxCrypto.STI(TSavegame.currentPlayer.money) + 3);
            TGlobalClass.coinsGot = 3;
          L1115:
            _local2 = int(_global._maxCrypto.STI(ScoreManager.totalScore) - TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel]);
            TSavegame.currentPlayer.daysEarning[TGlobalClass.currentPlayedLevel] = _global._maxCrypto.STI(ScoreManager.totalScore);
            TSavegame.save();
          L1193:
            TGlobalClass._windowDisplay.showWindow(TWindowState.STORY_LEVELCOMPLETE);
            TGlobalClass.isPlayingSpecialLevel = false;
            goto L1250;
          L1220:
            gameover = true;
            TGlobalClass._windowDisplay.showWindow(TWindowState.STORY_LEVELFAILED);
            TGlobalClass.isPlayingSpecialLevel = false;
          L1250:
            if (TSavegame.currentPlayer.unlocked[Achievement.BIG_BUSINESS]) goto L1449;
            _local3 = int(TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS]);
            TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS] = TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS] + _local2;
            if (!(TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS] >= Achievement.ALL_TARGET[Achievement.BIG_BUSINESS])) goto L1403;
            TSavegame.currentPlayer.unlocked[Achievement.BIG_BUSINESS] = true;
            Achievement.tweenAchievementNotif(Achievement.BIG_BUSINESS);
            goto L1449;
          L1403:
            if (!(_local3 % 20000 > TSavegame.currentPlayer.achievementCounter[Achievement.BIG_BUSINESS] % 20000)) goto L1449;
            Achievement.tweenAchievementProgress(Achievement.BIG_BUSINESS);
          L1449:
            TSavegame.save();
        });
    }

    function autoCollect():void {
        juiceBonus = 0;
        friesBonus = 0;
        i = 0;
        goto L884;
      L28:
        if (!this.coinsAr[i].visible) goto L874;
        TweenLite.to(this.coinsAr[i], 0.4, {"scaleX": 0, "scaleY": 0, "ease": Back.easeIn});
        this.earnedMoneyTextAr[i].visible = true;
        this.earnedMoneyTextAr[i].text = "$" + (this.customer[i].money + this.customer[i].tipsBonus);
        if (!this.customer[i].hasJuice) goto L307;
        var _local2 = TGlobalClass;
        var _local3 = TGlobalClass.juiceServed + 1;
        _local2.juiceServed = _local3;
        juiceBonus = int(this._global.dish.getHardwareDefaultPrice(Dish.JUICER) + (TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] - 1) * this._global.dish.getHardwarePriceIncrease(Dish.JUICER));
        this.earnedMoneyTextAr[i].text = "$" + (this.customer[i].money + this.customer[i].tipsBonus) + " + $" + juiceBonus + " Juice Bonus!";
      L307:
        if (!this.customer[i].hasFries) goto L464;
        _local2 = TGlobalClass;
        _local3 = TGlobalClass.friesServed + 1;
        _local2.friesServed = _local3;
        friesBonus = int(this._global.dish.getHardwareDefaultPrice(Dish.FRYER) + (TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] - 1) * this._global.dish.getHardwarePriceIncrease(Dish.FRYER));
        this.earnedMoneyTextAr[i].text = "$" + (this.customer[i].money + this.customer[i].tipsBonus) + " + $" + friesBonus + " Fries Bonus!";
      L464:
        var $t0 = Boolean(this.customer[i].hasJuice);
        $s0 = $t0;
        if (!$t0) goto L500;
        $s0 = Boolean(this.customer[i].hasFries);
      L500:
        if (!$s0) goto L569;
        this.earnedMoneyTextAr[i].text = "$" + (this.customer[i].money + this.customer[i].tipsBonus) + " + $" + (friesBonus + juiceBonus) + " Juice + Fries Bonus!";
      L569:
        ScoreManager.updateScore(this.customer[i].money + this.customer[i].tipsBonus + juiceBonus + friesBonus);
        var $t1 = this._global._maxCrypto.STI(ScoreManager.totalScore) >= this.goalTarget;
        $s0 = $t1;
        if (!$t1) goto L653;
        $s0 = this.goalExpertText.idx == 1;
      L653:
        if (!$s0) goto L719;
        TGlobalClass._windowDisplay.showGoalIconSmall();
        this.goalExpertText.idx = 2;
        this.targetInfoText.text = "$" + this.expertTarget;
        this.targetStateText.text = "Expert:";
        this._global._soundAsset.playSFX("goal_reached");
        goto L794;
      L719:
        var $t2 = this._global._maxCrypto.STI(ScoreManager.totalScore) >= this.expertTarget;
        $s0 = $t2;
        if (!$t2) goto L757;
        $s0 = this.goalExpertText.idx == 2;
      L757:
        if (!$s0) goto L794;
        this.goalExpertText.idx = 3;
        TGlobalClass._windowDisplay.showGoalIconSmall();
        this._global._soundAsset.playSFX("expert_reached");
      L794:
        TweenLite.to(this.earnedMoneyTextAr[i], 0.5, {"y": this.earnedMoneyTextAr[i].y - 65, "onComplete": function(f1_arg1:int):void {
            earnedMoneyTextAr[f1_arg1].x = coinsAr[f1_arg1].x;
            earnedMoneyTextAr[f1_arg1].y = coinsAr[f1_arg1].y;
            earnedMoneyTextAr[f1_arg1].visible = false;
        }, "onCompleteParams": [i]});
        this.tweenDishAlpha(i, this.customer[i].CUST_SATISFIED);
      L874:
        i = i + 1;
      L884:
        if (i < 3) goto L28;
    }

    function tweenDishAlpha(arg1:int, arg2:int):void {
        patienceLeft = 0;
        custID = arg1;
        custState = arg2;
        this.customer[custID].onTween = true;
        this.coinsAr[custID].visible = false;
        if (!this.customer[custID].asVIP) goto L193;
        if (custState != this.customer[custID].CUST_SATISFIED) goto L163;
        patienceLeft = int(this.customer[custID].patience / this.customer[custID].totalPatience * 100);
        if (!(patienceLeft > 75)) goto L151;
        TGlobalClass.VIPServedRating = 1.5;
        goto L159;
      L151:
        TGlobalClass.VIPServedRating = 1.25;
      L159:
        goto L193;
      L163:
        if (custState != this.customer[custID].CUST_LEAVE) goto L193;
        TGlobalClass.VIPServedRating = 0;
      L193:
        TweenLite.to(this.customer[custID].fries, 0.3, {"alpha": 0});
        TweenLite.to(this.customer[custID].juice, 0.3, {"alpha": 0});
        TweenLite.to(this.dishesAr[custID], 0.3, {"alpha": 0, "ease": Linear.easeNone, "onComplete": function(f1_arg1:int):void {
            dishesAr[f1_arg1].visible = false;
            dishesAr[f1_arg1].alpha = 1;
            customer[f1_arg1].fries.visible = false;
            customer[f1_arg1].juice.visible = false;
            customer[f1_arg1].fries.alpha = 1;
            customer[f1_arg1].juice.alpha = 1;
        }, "onCompleteParams": [custID]});
        onQueueRemove = int(this.customerOnQueue.indexOf(this.customer[custID].customerAvaID));
        this.customerOnQueue.splice(onQueueRemove, 1);
        this.customer[custID].tweenOut(custState, !this._global.tutorial.blockCustomerIn);
    }

    function checkCustOrder(arg1:int, arg2:int):void {
        countFulfilled = 0;
        j = 0;
        col = arg1;
        row = arg2;
        if (!this.tileAr[col][row].used) goto L54;
        return;
      L54:
        tempVec = new Vector.<int>();
        i = 0;
        goto L113;
      L80:
        tempVec.push(this.customer[i].queueIdx);
        i = i + 1;
      L113:
        if (i < 3) goto L80;
        tempVec.sort(Array.NUMERIC);
        k = 0;
        goto L603;
      L147:
        i = 0;
        goto L583;
      L158:
        countFulfilled = 0;
        if (this.customer[i].queueIdx == tempVec[k]) goto L198;
        goto L573;
      L198:
        var $t0 = !this.customer[i].onTween;
        $s0 = $t0;
        if (!$t0) goto L234;
        $s0 = !this.customer[i].completed;
      L234:
        if (!$s0) goto L573;
        j = 0;
        goto L548;
      L248:
        var $t1 = this.tileAr[col][row].type == this.customer[i].ingredientType[j];
        $s0 = $t1;
        if (!$t1) goto L320;
        $s0 = !this.tileAr[col][row].used;
      L320:
        if (!$s0) goto L538;
        if (!(this.customer[i].ingredientTarget[j] > 0)) goto L538;
        this.startTweenFromBoard(col, row, i, j);
        this.tileAr[col][row].used = true;
        var $t2 = this.customer[i].ingredientTarget;
        var _local4 = $t2;
        var _local5 = j;
        var $t3 = $t2[j] - 1;
        var _local6 = $t3;
        _local4[_local5] = _local6;
        this.customer[i].ingredientTargetPreview[j].text = "x" + $t3;
        TweenLite.to(this.customer[i].ingredientTargetPreview[j], 0.2, {"scaleX": 1.2, "scaleY": 1.2, "ease": Linear.easeNone, "onComplete": function(f1_arg1:int, f1_arg2:int):void {
            TweenLite.to(customer[f1_arg1].ingredientTargetPreview[f1_arg2], 0.2, {"scaleX": 1, "scaleY": 1, "ease": Linear.easeNone});
        }, "onCompleteParams": [i, j]});
      L538:
        j = j + 1;
      L548:
        if (j < this.customer[i].ingredientType.length) goto L248;
      L573:
        i = i + 1;
      L583:
        if (i < 3) goto L158;
        k = k + 1;
      L603:
        if (k < 3) goto L147;
    }

    function setNextPriorCust():void {
        var _local1 = 0;
        goto L49;
      L9:
        if (this.customer[_local1].queueIdx != TGlobalClass.customerFocus) goto L47;
        if (this.customer[_local1].completed) goto L47;
        return;
      L47:
        _local1++;
      L49:
        if (_local1 < 3) goto L9;
        var _local2 = new Vector.<int>();
        _local1 = 0;
        goto L110;
      L76:
        if (this.coinsAr[_local1].visible) goto L108;
        _local2.push(this.customer[_local1].queueIdx);
      L108:
        _local1++;
      L110:
        if (_local1 < 3) goto L76;
        if (!this.firstCust) goto L130;
        this.firstCust = false;
      L130:
        if (!(_local2.length <= 0)) goto L173;
        _local1 = 0;
        goto L162;
      L147:
        this.customer[_local1].hidePriority();
        _local1++;
      L162:
        if (_local1 < 3) goto L147;
        goto L280;
      L173:
        _local2.sort(Array.NUMERIC);
        TGlobalClass.customerFocus = _local2[0];
        _local1 = 0;
        goto L273;
      L203:
        if (this.customer[_local1].queueIdx != TGlobalClass.customerFocus) goto L259;
        if (this.customer[_local1].queueNumber.visible) goto L255;
        this.customer[_local1].showPriority();
      L255:
        goto L271;
      L259:
        this.customer[_local1].hidePriority();
      L271:
        _local1++;
      L273:
        if (_local1 < 3) goto L203;
      L280:
    }

    function lookForMatches():Vector.<Vector.<int>> {
        var _local5 = 0;
        var _local1 = new Vector.<Vector.<int>>();
        var _local2 = new Vector.<int>();
        var _local3 = new Vector.<int>();
        var _local4 = 0;
        goto L213;
      L58:
        _local5 = 0;
        goto L203;
      L67:
        var $t0 = !this.tileAr[_local5][_local4].isDropping;
        $s0 = $t0;
        if (!$t0) goto L110;
        $s0 = !this.tileAr[_local5][_local4].isSwapping;
      L110:
        if (!$s0) goto L134;
        $s0 = !this.tileAr[_local5][_local4].onTween;
      L134:
        if (!$s0) goto L158;
        $s0 = !this.tileAr[_local5][_local4].removed;
      L158:
        if (!$s0) goto L201;
        _local2 = this.getMatchHoriz(_local5, _local4);
        if (!(_local2.length > 2)) goto L201;
        _local1.push(_local2);
        _local5 = int(_local5 + (_local2.length - 1));
      L201:
        _local5++;
      L203:
        if (_local5 < 6) goto L67;
        _local4++;
      L213:
        if (_local4 < 8) goto L58;
        _local5 = 0;
        goto L384;
      L229:
        _local4 = 0;
        goto L374;
      L238:
        var $t1 = !this.tileAr[_local5][_local4].isDropping;
        $s0 = $t1;
        if (!$t1) goto L281;
        $s0 = !this.tileAr[_local5][_local4].isSwapping;
      L281:
        if (!$s0) goto L305;
        $s0 = !this.tileAr[_local5][_local4].onTween;
      L305:
        if (!$s0) goto L329;
        $s0 = !this.tileAr[_local5][_local4].removed;
      L329:
        if (!$s0) goto L372;
        _local3 = this.getMatchVert(_local5, _local4);
        if (!(_local3.length > 2)) goto L372;
        _local1.push(_local3);
        _local4 = int(_local4 + (_local3.length - 1));
      L372:
        _local4++;
      L374:
        if (_local4 < 6) goto L238;
        _local5++;
      L384:
        if (_local5 < 8) goto L229;
        return _local1;
    }

    function getMatchHoriz(arg1:int, arg2:int):Vector.<int> {
        var _local3 = new Vector.<int>();
        var $t0 = !this.tileAr[arg1][arg2].isDropping;
        $s0 = $t0;
        if (!$t0) goto L53;
        $s0 = !this.tileAr[arg1][arg2].onTween;
      L53:
        if (!$s0) goto L75;
        $s0 = !this.tileAr[arg1][arg2].removed;
      L75:
        if (!$s0) goto L97;
        $s0 = !this.tileAr[arg1][arg2].isSwapping;
      L97:
        if (!$s0) goto L121;
        _local3.push(this.tileAr[arg1][arg2].idx);
      L121:
        var _local4 = 1;
        goto L350;
      L129:
        var $t1 = !(this.tileAr[arg1][arg2].type == 10);
        $s0 = $t1;
        if (!$t1) goto L174;
        $s0 = !(this.tileAr[arg1][arg2].type == 15);
      L174:
        if (!$s0) goto L348;
        if (this.tileAr[arg1][arg2].type != this.tileAr[arg1 + _local4][arg2].type) goto L346;
        var $t2 = !this.tileAr[arg1 + _local4][arg2].isDropping;
        $s0 = $t2;
        if (!$t2) goto L259;
        $s0 = !this.tileAr[arg1 + _local4][arg2].onTween;
      L259:
        if (!$s0) goto L284;
        $s0 = !this.tileAr[arg1 + _local4][arg2].removed;
      L284:
        if (!$s0) goto L309;
        $s0 = !this.tileAr[arg1 + _local4][arg2].isSwapping;
      L309:
        if (!$s0) goto L340;
        _local3.push(this.tileAr[arg1 + _local4][arg2].idx);
        goto L342;
      L340:
        return _local3;
      L342:
        goto L348;
      L346:
        return _local3;
      L348:
        _local4++;
      L350:
        if (arg1 + _local4 < 8) goto L129;
        return _local3;
    }

    function getMatchVert(arg1:int, arg2:int):Vector.<int> {
        var _local3 = new Vector.<int>();
        var $t0 = !this.tileAr[arg1][arg2].isDropping;
        $s0 = $t0;
        if (!$t0) goto L53;
        $s0 = !this.tileAr[arg1][arg2].onTween;
      L53:
        if (!$s0) goto L75;
        $s0 = !this.tileAr[arg1][arg2].removed;
      L75:
        if (!$s0) goto L97;
        $s0 = !this.tileAr[arg1][arg2].isSwapping;
      L97:
        if (!$s0) goto L121;
        _local3.push(this.tileAr[arg1][arg2].idx);
      L121:
        var _local4 = 1;
        goto L350;
      L129:
        var $t1 = !(this.tileAr[arg1][arg2].type == 10);
        $s0 = $t1;
        if (!$t1) goto L174;
        $s0 = !(this.tileAr[arg1][arg2].type == 15);
      L174:
        if (!$s0) goto L348;
        if (this.tileAr[arg1][arg2].type != this.tileAr[arg1][arg2 + _local4].type) goto L346;
        var $t2 = !this.tileAr[arg1][arg2 + _local4].isDropping;
        $s0 = $t2;
        if (!$t2) goto L259;
        $s0 = !this.tileAr[arg1][arg2 + _local4].onTween;
      L259:
        if (!$s0) goto L284;
        $s0 = !this.tileAr[arg1][arg2 + _local4].removed;
      L284:
        if (!$s0) goto L309;
        $s0 = !this.tileAr[arg1][arg2 + _local4].isSwapping;
      L309:
        if (!$s0) goto L340;
        _local3.push(this.tileAr[arg1][arg2 + _local4].idx);
        goto L342;
      L340:
        return _local3;
      L342:
        goto L348;
      L346:
        return _local3;
      L348:
        _local4++;
      L350:
        if (arg2 + _local4 < 8) goto L129;
        return _local3;
    }

    function lookForPossibles():Boolean {
        var _local2 = 0;
        var _local1 = 0;
        goto L345;
      L12:
        _local2 = 0;
        goto L336;
      L20:
        if (!this.matchPattern(_local1, _local2, [[1, 0]], [[-2, 0], [-1, -1], [-1, 1], [2, -1], [2, 1], [3, 0]], [[-1, 0], [-1, 0], [-1, 0], [2, 0], [2, 0], [2, 0]])) goto L118;
        return true;
      L118:
        if (!this.matchPattern(_local1, _local2, [[2, 0]], [[1, -1], [1, 1]], [[1, 0], [1, 0]])) goto L166;
        return true;
      L166:
        if (!this.matchPattern(_local1, _local2, [[0, 1]], [[0, -2], [-1, -1], [1, -1], [-1, 2], [1, 2], [0, 3]], [[0, -1], [0, -1], [0, -1], [0, 2], [0, 2], [0, 2]])) goto L263;
        return true;
      L263:
        if (!this.matchPattern(_local1, _local2, [[0, 2]], [[-1, 1], [1, 1]], [[0, 1], [0, 1]])) goto L311;
        return true;
      L311:
        if (this.tileAr[_local1][_local2].type != 15) goto L334;
        return true;
      L334:
        _local2++;
      L336:
        if (_local2 < 8) goto L20;
        _local1++;
      L345:
        if (_local1 < 8) goto L12;
        return false;
    }

    function matchPattern(arg1:uint, arg2:uint, arg3:Array, arg4:Array, arg5:Array):Boolean {
        var $t0 = int(this.tileAr[arg1][arg2].type);
        var _local6 = $t0;
        var $t1 = $t0 == 10;
        $s0 = $t1;
        if (!$t1) goto L35;
        $s0 = _local6 == 15;
      L35:
        if (!$s0) goto L41;
        return false;
      L41:
        var _local7 = 0;
        goto L91;
      L49:
        if (this.matchType(arg1 + arg3[_local7][0], arg2 + arg3[_local7][1], _local6)) goto L89;
        return false;
      L89:
        _local7++;
      L91:
        if (_local7 < arg3.length) goto L49;
        _local7 = 0;
        goto L247;
      L109:
        var $t2 = Boolean(this.matchType(arg1 + arg4[_local7][0], arg2 + arg4[_local7][1], _local6));
        $s0 = $t2;
        if (!$t2) goto L194;
        $s0 = !this.tileAr[arg1 + arg4[_local7][0]][arg2 + arg4[_local7][1]].frozen;
      L194:
        if (!$s0) goto L245;
        if (this.tileAr[arg1 + arg5[_local7][0]][arg2 + arg5[_local7][1]].frozen) goto L245;
        return true;
      L245:
        _local7++;
      L247:
        if (_local7 < arg4.length) goto L109;
        return false;
    }

    function matchType(arg1:int, arg2:int, arg3:int):Boolean {
        var $t0 = arg1 < 0;
        $s0 = $t0;
        if ($t0) goto L16;
        $s0 = arg1 > 7;
      L16:
        if ($s0) goto L26;
        $s0 = arg2 < 0;
      L26:
        if ($s0) goto L36;
        $s0 = arg2 > 7;
      L36:
        if (!$s0) goto L42;
        return false;
      L42:
        return this.tileAr[arg1][arg2].type == arg3;
    }

    function highlightPossible():void {
        var _local2 = 0;
        var _local1 = 0;
        goto L642;
      L12:
        _local2 = 0;
        goto L633;
      L20:
        var $t0 = this._global.tutorial.currentDialogueID == Tutorial.LEVEL_1;
        $s0 = $t0;
        if (!$t0) goto L71;
        var $t1 = Boolean(this.matchType(_local1, _local2, 1));
        $s0 = $t1;
        if ($t1) goto L70;
        $s0 = Boolean(this.matchType(_local1, _local2, 2));
      L70:
        $s0 = !$s0;
      L71:
        if (!$s0) goto L79;
        goto L631;
      L79:
        if (!this.matchPattern(_local1, _local2, [[1, 0]], [[-2, 0], [-1, -1], [-1, 1], [2, -1], [2, 1], [3, 0]], [[-1, 0], [-1, 0], [-1, 0], [2, 0], [2, 0], [2, 0]])) goto L266;
        this.highlight(_local1, _local2, [[1, 0]], [[-2, 0], [-1, -1], [-1, 1], [2, -1], [2, 1], [3, 0]], [[-1, 0], [-1, 0], [-1, 0], [2, 0], [2, 0], [2, 0]]);
        return;
      L266:
        if (!this.matchPattern(_local1, _local2, [[2, 0]], [[1, -1], [1, 1]], [[1, 0], [1, 0]])) goto L355;
        this.highlight(_local1, _local2, [[2, 0]], [[1, -1], [1, 1]], [[1, 0], [1, 0]]);
        return;
      L355:
        if (!this.matchPattern(_local1, _local2, [[0, 1]], [[0, -2], [-1, -1], [1, -1], [-1, 2], [1, 2], [0, 3]], [[0, -1], [0, -1], [0, -1], [0, 2], [0, 2], [0, 2]])) goto L542;
        this.highlight(_local1, _local2, [[0, 1]], [[0, -2], [-1, -1], [1, -1], [-1, 2], [1, 2], [0, 3]], [[0, -1], [0, -1], [0, -1], [0, 2], [0, 2], [0, 2]]);
        return;
      L542:
        if (!this.matchPattern(_local1, _local2, [[0, 2]], [[-1, 1], [1, 1]], [[0, 1], [0, 1]])) goto L631;
        this.highlight(_local1, _local2, [[0, 2]], [[-1, 1], [1, 1]], [[0, 1], [0, 1]]);
        return;
      L631:
        _local2++;
      L633:
        if (_local2 < 8) goto L20;
        _local1++;
      L642:
        if (_local1 < 8) goto L12;
    }

    function highlight(arg1:uint, arg2:uint, arg3:Array, arg4:Array, arg5:Array):void {
        var _local6 = int(this.tileAr[arg1][arg2].type);
        this.tweenHighlighted(this.tileAr[arg1][arg2].col, this.tileAr[arg1][arg2].row);
        var _local7 = 0;
        goto L186;
      L63:
        if (!this.matchType(arg1 + arg3[_local7][0], arg2 + arg3[_local7][1], _local6)) goto L184;
        this.tweenHighlighted(this.tileAr[arg1 + arg3[_local7][0]][arg2 + arg3[_local7][1]].col, this.tileAr[arg1 + arg3[_local7][0]][arg2 + arg3[_local7][1]].row);
      L184:
        _local7++;
      L186:
        if (_local7 < arg3.length) goto L63;
        _local7 = 0;
        goto L379;
      L204:
        if (!this.matchType(arg1 + arg4[_local7][0], arg2 + arg4[_local7][1], _local6)) goto L377;
        if (this.tileAr[arg1 + arg5[_local7][0]][arg2 + arg5[_local7][1]].frozen) goto L377;
        this.tweenHighlighted(this.tileAr[arg1 + arg4[_local7][0]][arg2 + arg4[_local7][1]].col, this.tileAr[arg1 + arg4[_local7][0]][arg2 + arg4[_local7][1]].row);
        return;
      L377:
        _local7++;
      L379:
        if (_local7 < arg4.length) goto L204;
    }

    function tweenHighlighted(arg1:int, arg2:int):void {
        col = arg1;
        row = arg2;
        if (!(this.dispatchHighlightPosition > 0)) goto L92;
        var _local4 = this;
        var _local5 = this.dispatchHighlightPosition - 1;
        _local4.dispatchHighlightPosition = _local5;
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "highlight", true, false, {"row": row, "col": col}));
      L92:
        TweenLite.to(this.tileAr[col][row], 0.4, {"scaleX": 1.2, "scaleY": 1.2, "ease": Linear.easeNone, "onComplete": function():void {
            TweenLite.to(tileAr[col][row], 0.4, {"scaleX": 1, "scaleY": 1, "ease": Linear.easeNone, "onComplete": tweenHighlighted, "onCompleteParams": [col, row]});
        }});
    }

    function checkDishPriceAndAvaiblity(arg1:int):void {
        var _local2 = 0;
        var _local3 = 0;
        var _local4 = 0;
        var _local5 = 0;
        var _local6 = 0;
        var _local7 = null;
        var _local8 = null;
        var _local9 = null;
        var _local10 = null;
        var _local11 = null;
        var _local12 = 0;
        var _local13 = 0;
        var $t0 = TGlobalClass.currentRegionDish == ConstClass.WESTERN_DISH;
        $s0 = $t0;
        if (!$t0) goto L83;
        $s0 = TGlobalClass.dishRegionWestern.length == arg1;
      L83:
        if (!$s0) goto L88;
        return;
      L88:
        var $t1 = TGlobalClass.currentRegionDish == ConstClass.CHINESE_DISH;
        $s0 = $t1;
        if (!$t1) goto L118;
        $s0 = TGlobalClass.dishRegionChinese.length == arg1;
      L118:
        if (!$s0) goto L123;
        return;
      L123:
        var $t2 = TGlobalClass.currentRegionDish == ConstClass.MID_EAST_DISH;
        $s0 = $t2;
        if (!$t2) goto L153;
        $s0 = TGlobalClass.dishRegionMidEast.length == arg1;
      L153:
        if (!$s0) goto L158;
        return;
      L158:
        _local7 = new Vector.<int>();
        _local8 = Global.getInstance();
        _local9 = new Vector.<int>();
        _local10 = new Vector.<int>();
        _local11 = new Vector.<int>();
        TGlobalClass.dishRegionChinese.length = 0;
        TGlobalClass.dishRegionWestern.length = 0;
        TGlobalClass.dishRegionMidEast.length = 0;
        if (TGlobalClass.currentRegionDish != ConstClass.WESTERN_DISH) goto L672;
        _local2 = 0;
        goto L441;
      L281:
        if (!(TSavegame.currentPlayer.westernDishLevel[_local2] > 0)) goto L439;
        _local7 = _local8.dish.getDishIngredientType(_local2);
        _local3 = 0;
        goto L368;
      L322:
        _local4 = int(_local4 + (TSavegame.currentPlayer.ingredientLevel[_local7[_local3] - 1] - 1) * _local8.dish.getIngredientPriceIncrease(_local7[_local3] - 1));
        _local3++;
      L368:
        if (_local3 < _local7.length) goto L322;
        _local4 = int(_local4 + (_local8.dish.getDishDefaultPrice(_local2) + (TSavegame.currentPlayer.westernDishLevel[_local2] - 1) * _local8.dish.getPriceIncrease(_local2)));
        _local9.push(_local4);
        TGlobalClass.dishRegionWestern.push(_local2);
      L439:
        _local2++;
      L441:
        if (_local2 < TSavegame.currentPlayer.westernDishLevel.length) goto L281;
        goto L537;
      L462:
        _local12 = 1000;
        _local2 = 0;
        goto L503;
      L475:
        if (!(_local12 > _local9[_local2])) goto L501;
        _local12 = int(_local9[_local2]);
        _local13 = int(_local2);
      L501:
        _local2++;
      L503:
        if (_local2 < _local9.length) goto L475;
        _local9.splice(_local13, 1);
        TGlobalClass.dishRegionWestern.splice(_local13, 1);
      L537:
        if (_local9.length > arg1) goto L462;
        _local6 = int(TGlobalClass.dishRegionWestern.length);
        _local2 = 0;
        goto L661;
      L566:
        if (!(TSavegame.currentPlayer.westernDishLevel[TGlobalClass.dishRegionWestern[_local2]] > 1)) goto L659;
        _local5 = int(TSavegame.currentPlayer.westernDishLevel[TGlobalClass.dishRegionWestern[_local2]] - 1);
        goto L651;
      L625:
        TGlobalClass.dishRegionWestern.push(TGlobalClass.dishRegionWestern[_local2]);
        _local5 = _local5 - 1;
      L651:
        if (_local5 > 0) goto L625;
      L659:
        _local2++;
      L661:
        if (_local2 < _local6) goto L566;
        goto L1630;
      L672:
        if (TGlobalClass.currentRegionDish != ConstClass.CHINESE_DISH) goto L1132;
        _local2 = 0;
        goto L883;
      L695:
        if (!(TSavegame.currentPlayer.chineseDishLevel[_local2] > 0)) goto L881;
        _local7 = _local8.dish.getDishIngredientType(_local2 + TGlobalClass.TOTAL_WESTERN_DISH);
        _local3 = 0;
        goto L789;
      L743:
        _local4 = int(_local4 + (TSavegame.currentPlayer.ingredientLevel[_local7[_local3] - 1] - 1) * _local8.dish.getIngredientPriceIncrease(_local7[_local3] - 1));
        _local3++;
      L789:
        if (_local3 < _local7.length) goto L743;
        _local4 = int(_local4 + (_local8.dish.getDishDefaultPrice(TGlobalClass.TOTAL_WESTERN_DISH + _local2) + (TSavegame.currentPlayer.chineseDishLevel[_local2] - 1) * _local8.dish.getPriceIncrease(TGlobalClass.TOTAL_WESTERN_DISH + _local2)));
        _local10.push(_local4);
        TGlobalClass.dishRegionChinese.push(TGlobalClass.TOTAL_WESTERN_DISH + _local2);
      L881:
        _local2++;
      L883:
        if (_local2 < TSavegame.currentPlayer.chineseDishLevel.length) goto L695;
        goto L983;
      L904:
        _local12 = 1000;
        _local13 = 0;
        _local2 = 0;
        goto L949;
      L921:
        if (!(_local12 > _local10[_local2])) goto L947;
        _local12 = int(_local10[_local2]);
        _local13 = int(_local2);
      L947:
        _local2++;
      L949:
        if (_local2 < _local10.length) goto L921;
        _local10.splice(_local13, 1);
        TGlobalClass.dishRegionChinese.splice(_local13, 1);
      L983:
        if (_local10.length > arg1) goto L904;
        _local6 = int(TGlobalClass.dishRegionChinese.length);
        _local2 = 0;
        goto L1121;
      L1012:
        if (!(TSavegame.currentPlayer.chineseDishLevel[TGlobalClass.dishRegionChinese[_local2] - TGlobalClass.TOTAL_WESTERN_DISH] > 1)) goto L1119;
        _local5 = int(TSavegame.currentPlayer.chineseDishLevel[TGlobalClass.dishRegionChinese[_local2] - TGlobalClass.TOTAL_WESTERN_DISH] - 1);
        goto L1111;
      L1085:
        TGlobalClass.dishRegionChinese.push(TGlobalClass.dishRegionChinese[_local2]);
        _local5 = _local5 - 1;
      L1111:
        if (_local5 > 0) goto L1085;
      L1119:
        _local2++;
      L1121:
        if (_local2 < _local6) goto L1012;
        goto L1630;
      L1132:
        if (TGlobalClass.currentRegionDish != ConstClass.MID_EAST_DISH) goto L1630;
        _local2 = 0;
        goto L1371;
      L1155:
        if (!(TSavegame.currentPlayer.mideastDishLevel[_local2] > 0)) goto L1369;
        _local7 = _local8.dish.getDishIngredientType(_local2 + TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH);
        _local3 = 0;
        goto L1256;
      L1210:
        _local4 = int(_local4 + (TSavegame.currentPlayer.ingredientLevel[_local7[_local3] - 1] - 1) * _local8.dish.getIngredientPriceIncrease(_local7[_local3] - 1));
        _local3++;
      L1256:
        if (_local3 < _local7.length) goto L1210;
        _local4 = int(_local4 + (_local8.dish.getDishDefaultPrice(TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH + _local2) + (TSavegame.currentPlayer.mideastDishLevel[_local2] - 1) * _local8.dish.getPriceIncrease(TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH + _local2)));
        _local11.push(_local4);
        TGlobalClass.dishRegionMidEast.push(TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH + _local2);
      L1369:
        _local2++;
      L1371:
        if (_local2 < TSavegame.currentPlayer.mideastDishLevel.length) goto L1155;
        goto L1471;
      L1392:
        _local12 = 1000;
        _local13 = 0;
        _local2 = 0;
        goto L1437;
      L1409:
        if (!(_local12 > _local11[_local2])) goto L1435;
        _local12 = int(_local11[_local2]);
        _local13 = int(_local2);
      L1435:
        _local2++;
      L1437:
        if (_local2 < _local11.length) goto L1409;
        _local11.splice(_local13, 1);
        TGlobalClass.dishRegionMidEast.splice(_local13, 1);
      L1471:
        if (_local11.length > arg1) goto L1392;
        _local6 = int(TGlobalClass.dishRegionMidEast.length);
        _local2 = 0;
        goto L1623;
      L1500:
        if (!(TSavegame.currentPlayer.mideastDishLevel[TGlobalClass.dishRegionMidEast[_local2] - TGlobalClass.TOTAL_WESTERN_DISH - TGlobalClass.TOTAL_CHINESE_DISH] > 1)) goto L1621;
        _local5 = int(TSavegame.currentPlayer.mideastDishLevel[TGlobalClass.dishRegionMidEast[_local2] - TGlobalClass.TOTAL_WESTERN_DISH - TGlobalClass.TOTAL_CHINESE_DISH] - 1);
        goto L1613;
      L1587:
        TGlobalClass.dishRegionMidEast.push(TGlobalClass.dishRegionMidEast[_local2]);
        _local5 = _local5 - 1;
      L1613:
        if (_local5 > 0) goto L1587;
      L1621:
        _local2++;
      L1623:
        if (_local2 < _local6) goto L1500;
      L1630:
    }

    function showGoodMovesText(arg1:int):void {
        if (arg1 != 4) goto L23;
        this.largeText.text = "Good!";
        goto L103;
      L23:
        if (arg1 != 5) goto L44;
        this.largeText.text = "Nice!";
        goto L103;
      L44:
        if (arg1 != 6) goto L65;
        this.largeText.text = "Great!";
        goto L103;
      L65:
        if (arg1 != 7) goto L86;
        this.largeText.text = "Awesome!";
        goto L103;
      L86:
        if (!(arg1 >= 8)) goto L103;
        this.largeText.text = "WOW!";
      L103:
        var $t0 = TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_MATCH_BONUS;
        $s0 = $t0;
        if (!$t0) goto L129;
        $s0 = arg1 > 4;
      L129:
        if (!$s0) goto L156;
        this.largeText.text = this.largeText.text + ("\n+ $" + (arg1 - 4));
      L156:
        var $t1 = TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_COOKIES_AT;
        $s0 = $t1;
        if (!$t1) goto L182;
        $s0 = arg1 > 4;
      L182:
        if (!$s0) goto L191;
        this.popCookiesOnBoard();
      L191:
        TweenLite.killTweensOf(this.largeText);
        TweenLite.killDelayedCallsTo(this.hideGoodMovesText);
        this.largeText.visible = true;
        this.largeText.alpha = 0.5;
        var _local2 = 0.8;
        this.largeText.scaleY = 0.8;
        this.largeText.scaleX = _local2;
        this.largeText.y = 250;
        TweenLite.to(this.largeText, 1, {"alpha": 1, "scaleX": 1, "scaleY": 1, "y": 300, "ease": Expo.easeOut});
        TweenLite.delayedCall(2, this.hideGoodMovesText);
    }

    function hideGoodMovesText():void {
        if (!this.largeText) goto L18;
        this.largeText.visible = false;
      L18:
    }

    function touchButton(arg1:Event):void {
        var _local2 = 0;
        _local2 = int((arg1.currentTarget as TNonStateButton).idx);
        if (_local2 != this.PAUSE_BUTTON) goto L61;
        if (!this.calledShowWindow) goto L36;
        return;
      L36:
        this.paused = true;
        TGlobalClass._windowDisplay.showWindow(TWindowState.PAUSE_GAME);
        goto L143;
      L61:
        if (_local2 != this.SOUND_BUTTON) goto L104;
        this._global.MuteSound = !this._global.MuteSound;
        this.mute_icon.visible = !this.mute_icon.visible;
        goto L143;
      L104:
        if (_local2 != this.MUSIC_BUTTON) goto L143;
        this._global.MuteMusic = !this._global.MuteMusic;
        this.mute_icon_music.visible = !this.mute_icon_music.visible;
      L143:
    }

    function pauseGame():void {
        if (TGlobalClass._windowDisplay._blackScreen.visible) goto L48;
        if (!this.calledShowWindow) goto L27;
        return;
      L27:
        this.paused = true;
        TGlobalClass._windowDisplay.showWindow(TWindowState.PAUSE_GAME);
      L48:
    }

    function resumeGame():void {
        if (TGlobalClass._windowDisplay._blackScreen.visible) goto L23;
        this.paused = false;
      L23:
    }
}
}
