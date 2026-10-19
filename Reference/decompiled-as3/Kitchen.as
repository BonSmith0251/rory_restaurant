package {
class Kitchen extends Screen {
    static const PLAY_ID:int = 1;
    static const SHOP_ID:int = 2;
    static const ACHIEVEMENT_ID:int = 3;
    static const RECIPE_ID:int = 4;
    static const BACK_BUTTON:int = 5;
    static const WESTERN_BUTTON:int = 6;
    static const CHINESE_BUTTON:int = 7;
    static const BUY_BUTTON:int = 8;
    static const LEFT_BUTTON:int = 9;
    static const RIGHT_BUTTON:int = 10;
    static const HARDWARE_ID:int = 11;
    static const MID_EAST_BUTTON:int = 12;
    static const FREE_COINS_BUTTON:int = 13;
    const INGREDIENT_SHOP:int = 1;
    const RECIPE_SHOP:int = 2;
    const HARDWARE_SHOP:int = 3;
    const WESTERN:int = 1;
    const CHINESE:int = 2;
    const MID_EAST:int = 3;
    var _global:Global;
    var initVector:Boolean;
    var kitchenAtlas:TextureAtlas;
    var _background:cSprite;
    var shopBackground:cSprite;
    var shopCoin:cSprite;
    var coinPreview:cSprite;
    var playBubble:cSprite;
    var achievementBubble:cSprite;
    var recipeBubble:cSprite;
    var shopBubble:cSprite;
    var hardwareBubble:cSprite;
    var playQuad:cQuad;
    var achievementQuad:cQuad;
    var recipeQuad:cQuad;
    var shopQuad:cQuad;
    var hardwareQuad:cQuad;
    var playGlow:cSprite;
    var achievementGlow:cSprite;
    var recipeGlow:cSprite;
    var shopGlow:cSprite;
    var hardwareGlow:cSprite;
    var backBtn:TNonStateButton;
    var FreeCoinsBtn:TNonStateButton;
    var buttonAr:Vector.<cSprite>;
    var quadButtonAr:Vector.<DisplayObject>;
    var hardwareName:Vector.<String>;
    var ingredientName:Vector.<String>;
    var westernDishName:Vector.<String>;
    var chineseDishName:Vector.<String>;
    var mideastDishName:Vector.<String>;
    var shopName:ExtendTextField;
    var upgradeButton:TNonStateButton;
    var westernButton:TNonStateButton;
    var chineseButton:TNonStateButton;
    var midEastButton:TNonStateButton;
    var buyButton:TNonStateButton;
    var leftButton:TNonStateButton;
    var rightButton:TNonStateButton;
    var moneyPlaceBG:cSprite;
    var moneyPreview:ExtendTextField;
    var moneyPreviewMinus:ExtendTextField;
    var ingredientPreview:cSprite;
    var ingredientPreviewAr:Vector.<cSprite>;
    var ingredientCount:ExtendTextField;
    var ingredientCountAr:Vector.<ExtendTextField>;
    var shopItemContainer:Vector.<Sprite>;
    var plateAr:Vector.<cSprite>;
    var itemAr:Vector.<cSprite>;
    var itemName:ExtendTextField;
    var starAr:Vector.<cSprite>;
    var upgradeInfo:ExtendTextField;
    var upgradeCost:ExtendTextField;
    var currentRecipeRegion:int;
    var currentFocus:int;
    var currentShop:int;
    var startPointLooping:int;
    var timeline:TimelineLite;
    var startTouchX:Number = 0;
    var itemIsTweening:Boolean;
    var touchedBG:Boolean;
    var _bolehKlik:Boolean;
    var posYDif_Chinese:Vector.<Number>;
    var posXDif_Chinese:Vector.<Number>;
    var posYDif_Western:Vector.<Number>;
    var posXDif_Western:Vector.<Number>;
    var posXDif_MidEast:Vector.<Number>;
    var posYDif_MidEast:Vector.<Number>;

    // static initializer
    static {
        PLAY_ID = 1;
        SHOP_ID = 2;
        ACHIEVEMENT_ID = 3;
        RECIPE_ID = 4;
        BACK_BUTTON = 5;
        WESTERN_BUTTON = 6;
        CHINESE_BUTTON = 7;
        BUY_BUTTON = 8;
        LEFT_BUTTON = 9;
        RIGHT_BUTTON = 10;
        HARDWARE_ID = 11;
        MID_EAST_BUTTON = 12;
        FREE_COINS_BUTTON = 13;
    }

    function Kitchen():* {
        var $t0 = new Vector.<Number>(6);
        $t0[0] = 67.5;
        $t0[1] = 72.5;
        $t0[2] = 81.5;
        $t0[3] = 78.5;
        $t0[4] = 52.5;
        $t0[5] = 77.5;
        this.posYDif_Chinese = $t0;
        var $t1 = new Vector.<Number>(6);
        $t1[0] = 130;
        $t1[1] = 141;
        $t1[2] = 137;
        $t1[3] = 127;
        $t1[4] = 152;
        $t1[5] = 131.5;
        this.posXDif_Chinese = $t1;
        var $t2 = new Vector.<Number>(6);
        $t2[0] = 60.5;
        $t2[1] = 84.5;
        $t2[2] = 84.5;
        $t2[3] = 68.5;
        $t2[4] = 75;
        $t2[5] = 70;
        this.posYDif_Western = $t2;
        var $t3 = new Vector.<Number>(6);
        $t3[0] = 133.5;
        $t3[1] = 134.5;
        $t3[2] = 135;
        $t3[3] = 129.5;
        $t3[4] = 138.5;
        $t3[5] = 138;
        this.posXDif_Western = $t3;
        var $t4 = new Vector.<Number>(6);
        $t4[0] = 134;
        $t4[1] = 131.5;
        $t4[2] = 133;
        $t4[3] = 128;
        $t4[4] = 128.5;
        $t4[5] = 131;
        this.posXDif_MidEast = $t4;
        var $t5 = new Vector.<Number>(6);
        $t5[0] = 81;
        $t5[1] = 75.5;
        $t5[2] = 71.5;
        $t5[3] = 84.5;
        $t5[4] = 63.5;
        $t5[5] = 47.5;
        this.posYDif_MidEast = $t5;
        super();
    }

    override function Start():void {
        super.Start();
        this._global.currentSceneID = this._global.KITCHEN_ID;
        if (this.initVector) goto L542;
        this.buttonAr = new Vector.<cSprite>();
        this.quadButtonAr = new Vector.<DisplayObject>();
        this.plateAr = new Vector.<cSprite>();
        this.starAr = new Vector.<cSprite>();
        this.itemAr = new Vector.<cSprite>();
        this.shopItemContainer = new Vector.<Sprite>();
        this.ingredientPreviewAr = new Vector.<cSprite>();
        this.ingredientCountAr = new Vector.<ExtendTextField>();
        var $t0 = new Vector.<String>(3);
        $t0[0] = "juicer";
        $t0[1] = "fryer";
        $t0[2] = "cookies";
        this.hardwareName = $t0;
        var $t1 = new Vector.<String>(12);
        $t1[0] = "croissant";
        $t1[1] = "selada";
        $t1[2] = "beef";
        $t1[3] = "chicken";
        $t1[4] = "rice";
        $t1[5] = "bacon";
        $t1[6] = "salmon";
        $t1[7] = "seafood";
        $t1[8] = "cheese";
        $t1[9] = "pita_bread";
        $t1[10] = "lamb";
        $t1[11] = "tomato";
        this.ingredientName = $t1;
        var $t2 = new Vector.<String>(6);
        $t2[0] = "veggie_croissant";
        $t2[1] = "deluxe_chicken_croissant";
        $t2[2] = "french_BBQ_combo";
        $t2[3] = "beef_steak";
        $t2[4] = "smokey_salmon";
        $t2[5] = "premium_salmon_steak";
        this.westernDishName = $t2;
        var $t3 = new Vector.<String>(6);
        $t3[0] = "asian_fried_rice";
        $t3[1] = "thai_fried_rice";
        $t3[2] = "kalasan_fried_chicken";
        $t3[3] = "chinese_pork_special";
        $t3[4] = "fine_imperial_cuisine";
        $t3[5] = "deluxe_BBQ_combo";
        this.chineseDishName = $t3;
        var $t4 = new Vector.<String>(6);
        $t4[0] = "doner_kebab";
        $t4[1] = "gyro_style";
        $t4[2] = "tomato_domas";
        $t4[3] = "tuscan_humus";
        $t4[4] = "baklava";
        $t4[5] = "crown_lamb_roast";
        this.mideastDishName = $t4;
        this.initVector = true;
      L542:
        this._global.assetManager.enqueue(this._global.graphicsPath + "kitchen.ean", this._global.graphicsPath + "kitchen.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + "kitchen_BG.eag", this._global.graphicsPath + "shop_table_BG.eag");
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
        if (!this.timeline) goto L30;
        this.timeline.kill();
        this.timeline = null;
      L30:
        _local2 = uint(this.buttonAr.length);
        _local1 = 0;
        goto L108;
      L46:
        TweenLite.killTweensOf(this.buttonAr[_local1]);
        this.buttonAr[_local1].removeEventListener(TouchEvent.TOUCH, this.touchButton);
        this.buttonAr[_local1].removeFromParent(true);
        this.buttonAr[_local1] = null;
        _local1++;
      L108:
        if (_local1 < _local2) goto L46;
        this.buttonAr.length = 0;
        _local2 = uint(this.quadButtonAr.length);
        _local1 = 0;
        goto L186;
      L139:
        this.quadButtonAr[_local1].removeEventListener(TouchEvent.TOUCH, this.touchQuad);
        this.quadButtonAr[_local1].removeFromParent(true);
        this.quadButtonAr[_local1] = null;
        _local1++;
      L186:
        if (_local1 < _local2) goto L139;
        this.quadButtonAr.length = 0;
        _local2 = uint(this.shopItemContainer.length);
        _local1 = 0;
        goto L242;
      L217:
        this.shopItemContainer[_local1].removeFromParent(true);
        this.shopItemContainer[_local1] = null;
        _local1++;
      L242:
        if (_local1 < _local2) goto L217;
        this.shopItemContainer.length = 0;
        _local2 = uint(this.starAr.length);
        _local1 = 0;
        goto L298;
      L273:
        this.starAr[_local1].removeFromParent(true);
        this.starAr[_local1] = null;
        _local1++;
      L298:
        if (_local1 < 10) goto L273;
        this.starAr.length = 0;
        _local1 = 0;
        goto L412;
      L321:
        this.itemAr[_local1].removeFromParent(true);
        this.itemAr[_local1] = null;
        this.plateAr[_local1].removeFromParent(true);
        this.plateAr[_local1] = null;
        this.ingredientPreviewAr[_local1].removeFromParent(true);
        this.ingredientPreviewAr[_local1] = null;
        this.ingredientCountAr[_local1].removeFromParent(true);
        this.ingredientCountAr[_local1] = null;
        _local1++;
      L412:
        if (_local1 < 5) goto L321;
        this.itemAr.length = 0;
        this.plateAr.length = 0;
        this.ingredientPreviewAr.length = 0;
        this.ingredientCountAr.length = 0;
        this._background.removeFromParent(true);
        this._background = null;
        this.playBubble.removeFromParent(true);
        this.playBubble = null;
        this.playQuad.removeFromParent(true);
        this.playQuad = null;
        this.shopBubble.removeFromParent(true);
        this.shopBubble = null;
        this.hardwareBubble.removeFromParent(true);
        this.hardwareBubble = null;
        this.shopQuad.removeFromParent(true);
        this.shopQuad = null;
        this.hardwareQuad.removeFromParent(true);
        this.hardwareQuad = null;
        this.achievementBubble.removeFromParent(true);
        this.achievementBubble = null;
        this.achievementQuad.removeFromParent(true);
        this.achievementQuad = null;
        this.recipeBubble.removeFromParent(true);
        this.recipeBubble = null;
        this.recipeQuad.removeFromParent(true);
        this.recipeQuad = null;
        this.shopBackground.removeFromParent(true);
        this.shopBackground = null;
        this.shopCoin.removeFromParent(true);
        this.shopCoin = null;
        this.coinPreview.removeFromParent(true);
        this.coinPreview = null;
        this.shopName.removeFromParent(true);
        this.shopName = null;
        this.backBtn.removeFromParent(true);
        this.backBtn = null;
        this.buyButton.removeFromParent(true);
        this.buyButton = null;
        this.itemName.removeFromParent(true);
        this.itemName = null;
        this.upgradeInfo.removeFromParent(true);
        this.upgradeInfo = null;
        this.upgradeCost.removeFromParent(true);
        this.upgradeCost = null;
        this.moneyPlaceBG.removeFromParent(true);
        this.moneyPlaceBG = null;
        this.moneyPreview.removeFromParent(true);
        this.moneyPreview = null;
        this.moneyPreviewMinus.removeFromParent(true);
        this.moneyPreviewMinus = null;
        this.westernButton.removeFromParent(true);
        this.westernButton = null;
        this.chineseButton.removeFromParent(true);
        this.chineseButton = null;
        this.midEastButton.removeFromParent(true);
        this.midEastButton = null;
        this.leftButton.removeFromParent(true);
        this.leftButton = null;
        this.rightButton.removeFromParent(true);
        this.rightButton = null;
        this.ingredientPreview.removeFromParent(true);
        this.ingredientPreview = null;
        this.ingredientCount.removeFromParent(true);
        this.ingredientCount = null;
        this.FreeCoinsBtn.removeFromParent(true);
        this.FreeCoinsBtn = null;
        this.playGlow.removeFromParent(true);
        this.playGlow = null;
        this.recipeGlow.removeFromParent(true);
        this.recipeGlow = null;
        this.achievementGlow.removeFromParent(true);
        this.achievementGlow = null;
        this.shopGlow.removeFromParent(true);
        this.shopGlow = null;
        this.hardwareGlow.removeFromParent(true);
        this.hardwareGlow = null;
        this.kitchenAtlas.dispose();
        this.kitchenAtlas = null;
        this._global.assetManager.purge();
        TGlobalClass._windowDisplay.hideAllWindows(this._global.currentSceneID);
        TGlobalClass._windowDisplay.kill(this._global.currentSceneID);
        ready = false;
    }

    function initialize():void {
        i = 0;
        plate = null;
        item = null;
        container = null;
        star = null;
        System.pauseForGCIfCollectionImminent(0);
        System.gc();
        if (TGlobalClass.musicPlayingIdx == 1) goto L93;
        this._global._soundAsset.playMusic("feeling-positive");
        TGlobalClass.musicPlayingIdx = 1;
      L93:
        this.kitchenAtlas = this._global.assetManager.getTextureAtlas("kitchen");
        this._background = new cSprite(this._global.assetManager.getTexture("kitchen_BG"));
        this._background.blendMode = BlendMode.NONE;
        this._background.touchable = false;
        this._global._gameS.addChild(this._background);
        this.playBubble = new cSprite(this.kitchenAtlas.getTexture("play_bubble"));
        this.playBubble.x = 468;
        this.playBubble.y = 388;
        this.playBubble.idx = PLAY_ID;
        this._global._gameS.addChild(this.playBubble);
        this.buttonAr.push(this.playBubble);
        this.playQuad = new cQuad(this.playBubble.width + 20, this.playBubble.height + 10);
        this.playQuad.x = this.playBubble.x - 20;
        this.playQuad.y = this.playBubble.y + 50;
        this.playQuad.alpha = 0;
        this.playQuad.name = "" + PLAY_ID;
        this._global._gameS.addChild(this.playQuad);
        this.quadButtonAr.push(this.playQuad);
        this.shopBubble = new cSprite(this.kitchenAtlas.getTexture("shop_bubble"));
        this.shopBubble.x = 616;
        this.shopBubble.y = 340;
        this.shopBubble.visible = false;
        this.shopBubble.idx = SHOP_ID;
        this._global._gameS.addChild(this.shopBubble);
        this.buttonAr.push(this.shopBubble);
        this.shopQuad = new cQuad(this.shopBubble.width - 20, this.shopBubble.height + 10);
        this.shopQuad.x = this.shopBubble.x + 20;
        this.shopQuad.y = this.shopBubble.y + 50;
        this.shopQuad.alpha = 0;
        this.shopQuad.visible = false;
        this.shopQuad.name = "" + SHOP_ID;
        this._global._gameS.addChild(this.shopQuad);
        this.quadButtonAr.push(this.shopQuad);
        this.hardwareBubble = new cSprite(this.kitchenAtlas.getTexture("convenience_bubble"));
        this.hardwareBubble.x = 20;
        this.hardwareBubble.y = 270;
        this.hardwareBubble.visible = false;
        this.hardwareBubble.idx = HARDWARE_ID;
        this._global._gameS.addChild(this.hardwareBubble);
        this.buttonAr.push(this.hardwareBubble);
        this.hardwareQuad = new cQuad(100, 100);
        this.hardwareQuad.x = 120;
        this.hardwareQuad.y = 315;
        this.hardwareQuad.alpha = 0;
        this.hardwareQuad.visible = false;
        this.hardwareQuad.name = "" + HARDWARE_ID;
        this._global._gameS.addChild(this.hardwareQuad);
        this.quadButtonAr.push(this.hardwareQuad);
        this.achievementBubble = new cSprite(this.kitchenAtlas.getTexture("achievement_bubble"));
        this.achievementBubble.x = 669;
        this.achievementBubble.y = 152;
        this.achievementBubble.idx = ACHIEVEMENT_ID;
        this._global._gameS.addChild(this.achievementBubble);
        this.buttonAr.push(this.achievementBubble);
        this.achievementQuad = new cQuad(70, 70);
        this.achievementQuad.x = this.achievementBubble.x + 100;
        this.achievementQuad.y = this.achievementBubble.y + 55;
        this.achievementQuad.alpha = 0;
        this.achievementQuad.name = "" + ACHIEVEMENT_ID;
        this._global._gameS.addChild(this.achievementQuad);
        this.quadButtonAr.push(this.achievementQuad);
        this.recipeBubble = new cSprite(this.kitchenAtlas.getTexture("recipe_bubble"));
        this.recipeBubble.x = 224;
        this.recipeBubble.y = 143;
        this.recipeBubble.visible = false;
        this.recipeBubble.idx = RECIPE_ID;
        this._global._gameS.addChild(this.recipeBubble);
        this.buttonAr.push(this.recipeBubble);
        this.recipeQuad = new cQuad(70, 70);
        this.recipeQuad.x = this.recipeBubble.x + 50;
        this.recipeQuad.y = this.recipeBubble.y + 55;
        this.recipeQuad.alpha = 0;
        this.recipeQuad.visible = false;
        this.recipeQuad.name = "" + RECIPE_ID;
        this._global._gameS.addChild(this.recipeQuad);
        this.quadButtonAr.push(this.recipeQuad);
        this.playGlow = new cSprite(this.kitchenAtlas.getTexture("bookGlow"));
        this.playGlow.alignPivot();
        this.playGlow.x = 523.5;
        this.playGlow.y = 477.5;
        this.playGlow.touchable = false;
        this._global._gameS.addChild(this.playGlow);
        this.playGlow.alpha = 0;
        this.shopGlow = new cSprite(this.kitchenAtlas.getTexture("shopGlow"));
        this.shopGlow.alignPivot();
        this.shopGlow.x = 698.5;
        this.shopGlow.y = 436;
        this.shopGlow.touchable = false;
        this._global._gameS.addChild(this.shopGlow);
        this.shopGlow.alpha = 0;
        this.achievementGlow = new cSprite(this.kitchenAtlas.getTexture("achievementGlow"));
        this.achievementGlow.alignPivot();
        this.achievementGlow.x = 810.5;
        this.achievementGlow.y = 246.5;
        this.achievementGlow.touchable = false;
        this._global._gameS.addChild(this.achievementGlow);
        this.achievementGlow.alpha = 0;
        this.recipeGlow = new cSprite(this.kitchenAtlas.getTexture("recipeGlow"));
        this.recipeGlow.alignPivot();
        this.recipeGlow.x = 304.5;
        this.recipeGlow.y = 241.5;
        this.recipeGlow.touchable = false;
        this._global._gameS.addChild(this.recipeGlow);
        this.recipeGlow.alpha = 0;
        this.hardwareGlow = new cSprite(this.kitchenAtlas.getTexture("hardwareGlow"));
        this.hardwareGlow.alignPivot();
        this.hardwareGlow.x = 164;
        this.hardwareGlow.y = 371.5;
        this.hardwareGlow.touchable = false;
        this._global._gameS.addChild(this.hardwareGlow);
        this.hardwareGlow.alpha = 0;
        if (!(TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_RECIPE_SHOP)) goto L1555;
        this.recipeBubble.visible = true;
        this.recipeQuad.visible = true;
      L1555:
        if (!(TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_INGREDIENT_SHOP)) goto L1590;
        this.shopBubble.visible = true;
        this.shopQuad.visible = true;
      L1590:
        if (!(TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_HARDWARE_SHOP)) goto L1625;
        this.hardwareBubble.visible = true;
        this.hardwareQuad.visible = true;
      L1625:
        this.FreeCoinsBtn = new TNonStateButton(this.kitchenAtlas.getTexture("small_btn_blue"), "Free Coins", "bromine", 25, 16776960);
        this.FreeCoinsBtn.alignPivot();
        this.FreeCoinsBtn.x = 60 + this.FreeCoinsBtn.pivotX;
        this.FreeCoinsBtn.y = 20 + this.FreeCoinsBtn.pivotY;
        this.FreeCoinsBtn._image.name = "" + FREE_COINS_BUTTON;
        this.FreeCoinsBtn._text.touchable = false;
        this._global._gameS.addChild(this.FreeCoinsBtn);
        this.quadButtonAr.push(this.FreeCoinsBtn._image);
        this.FreeCoinsBtn.visible = TGlobalClass.EnableFreeCoins;
        if (!TGlobalClass.EnableFreeCoins) goto L1871;
        this.timeline = new TimelineLite();
        this.timeline.append(new TweenLite(this.FreeCoinsBtn, 1, {"scaleX": 1.1, "scaleY": 1.1, "ease": Expo.easeOut}));
        this.timeline.append(new TweenLite(this.FreeCoinsBtn, 0.5, {"scaleX": 1, "scaleY": 1, "onComplete": function():* {
            timeline.restart();
        }}));
      L1871:
        this.shopBackground = new cSprite(this._global.assetManager.getTexture("shop_table_BG"));
        this.shopBackground.blendMode = BlendMode.NONE;
        this.shopBackground.visible = false;
        this.shopBackground.addEventListener(TouchEvent.TOUCH, this.swipe);
        this._global._gameS.addChild(this.shopBackground);
        this.shopCoin = new cSprite(TGlobalClass._windowDisplay.windowsAtlas.getTexture("coin"));
        this.shopCoin.visible = false;
        this.shopCoin.x = 565;
        this.shopCoin.y = 470;
        this.shopCoin.touchable = false;
        var _local2 = 0.5;
        this.shopCoin.scaleY = 0.5;
        this.shopCoin.scaleX = _local2;
        this._global._gameS.addChild(this.shopCoin);
        this.shopName = new ExtendTextField(600, 100, "MILLY'S INGREDIENT MARKET", "bromine", 35, 16777215);
        this.shopName.pivotX = 300;
        this.shopName.pivotY = 50;
        this.shopName.x = 512;
        this.shopName.y = 20;
        this.shopName.touchable = false;
        this.shopName.visible = false;
        this._global._gameS.addChild(this.shopName);
        this.westernButton = new TNonStateButton(this.kitchenAtlas.getTexture("small_btn_blue"), "Western", "bromine", 25, 0);
        _local2 = 0.7;
        this.westernButton.scaleY = 0.7;
        this.westernButton.scaleX = _local2;
        this.westernButton.x = 700;
        this.westernButton.y = 70;
        this.westernButton._image.name = "" + WESTERN_BUTTON;
        this.westernButton._text.touchable = false;
        this.westernButton.visible = false;
        this._global._gameS.addChild(this.westernButton);
        this.quadButtonAr.push(this.westernButton._image);
        this.chineseButton = new TNonStateButton(this.kitchenAtlas.getTexture("small_btn_blue"), "Asian", "bromine", 25, 0);
        _local2 = 0.7;
        this.chineseButton.scaleY = 0.7;
        this.chineseButton.scaleX = _local2;
        this.chineseButton.x = 860;
        this.chineseButton.y = 70;
        this.chineseButton.visible = false;
        this.chineseButton._text.touchable = false;
        this.chineseButton._image.name = "" + CHINESE_BUTTON;
        this._global._gameS.addChild(this.chineseButton);
        this.quadButtonAr.push(this.chineseButton._image);
        this.midEastButton = new TNonStateButton(this.kitchenAtlas.getTexture("small_btn_blue"), "Middle East", "bromine", 25, 0);
        _local2 = 0.7;
        this.midEastButton.scaleY = 0.7;
        this.midEastButton.scaleX = _local2;
        this.midEastButton.x = 860;
        this.midEastButton.y = 120;
        this.midEastButton.visible = false;
        this.midEastButton._text.touchable = false;
        this.midEastButton._image.name = "" + MID_EAST_BUTTON;
        this._global._gameS.addChild(this.midEastButton);
        this.quadButtonAr.push(this.midEastButton._image);
        this.moneyPlaceBG = new cSprite(this.kitchenAtlas.getTexture("coin_BG"));
        this.moneyPlaceBG.x = 30;
        this.moneyPlaceBG.y = 70;
        this.moneyPlaceBG.touchable = false;
        this.moneyPlaceBG.visible = false;
        this._global._gameS.addChild(this.moneyPlaceBG);
        this.moneyPreview = new ExtendTextField(this.moneyPlaceBG.width - 20, this.moneyPlaceBG.height, "0", "bromine", 25);
        this.moneyPreview.x = this.moneyPlaceBG.x + 5;
        this.moneyPreview.y = this.moneyPlaceBG.y;
        this.moneyPreview.hAlign = "right";
        this.moneyPreview.visible = false;
        this.moneyPreview.touchable = false;
        this._global._gameS.addChild(this.moneyPreview);
        this.coinPreview = new cSprite(TGlobalClass._windowDisplay.windowsAtlas.getTexture("coin"));
        this.coinPreview.visible = false;
        this.coinPreview.x = this.moneyPlaceBG.x + 25;
        this.coinPreview.y = this.moneyPlaceBG.y + 5;
        this.coinPreview.touchable = false;
        _local2 = 0.7;
        this.coinPreview.scaleY = 0.7;
        this.coinPreview.scaleX = _local2;
        this._global._gameS.addChild(this.coinPreview);
        this.moneyPreviewMinus = new ExtendTextField(this.moneyPlaceBG.width - 20, this.moneyPlaceBG.height, "0", "bromine", 25, 16711680);
        this.moneyPreviewMinus.x = this.moneyPlaceBG.x + 5;
        this.moneyPreviewMinus.y = this.moneyPlaceBG.y;
        this.moneyPreviewMinus.hAlign = "right";
        this.moneyPreviewMinus.visible = false;
        this.moneyPreviewMinus.touchable = false;
        this._global._gameS.addChild(this.moneyPreviewMinus);
        i = 0;
        goto L3540;
      L2967:
        container = new Sprite();
        this._global._gameS.addChild(container);
        plate = new cSprite(this.kitchenAtlas.getTexture("plate"));
        plate.pivotX = plate.width / 2;
        plate.pivotY = plate.height / 2;
        plate.x = plate.pivotX;
        plate.y = plate.pivotY;
        plate.touchable = false;
        plate.visible = false;
        container.addChild(plate);
        this.plateAr.push(plate);
        container.pivotX = container.width / 2;
        container.pivotY = container.height / 2;
        container.x = 512 + i * 550;
        container.y = 280;
        container.touchable = false;
        this.shopItemContainer.push(container);
        item = new cSprite(this.kitchenAtlas.getTexture("fine_imperial_cuisine"));
        item.pivotX = item.width / 2;
        item.pivotY = item.height / 2;
        item.x = item.pivotX;
        item.y = item.pivotY;
        item.itemIdx = i;
        item.visible = false;
        item.touchable = false;
        container.addChild(item);
        this.itemAr.push(item);
        this.ingredientPreview = new cSprite(this.kitchenAtlas.getTexture("chicken_small"));
        this.ingredientPreview.x = 30 + i * 70;
        this.ingredientPreview.y = 520;
        this.ingredientPreview.touchable = false;
        this.ingredientPreview.visible = false;
        this._global._gameS.addChild(this.ingredientPreview);
        this.ingredientPreviewAr.push(this.ingredientPreview);
        this.ingredientCount = new ExtendTextField(50, 50, "x6", "KomikandyWhite", 25, 16777215);
        this.ingredientCount.x = 30 + i * 70;
        this.ingredientCount.y = 540;
        this.ingredientCount.touchable = false;
        this.ingredientCount.visible = false;
        this._global._gameS.addChild(this.ingredientCount);
        this.ingredientCountAr.push(this.ingredientCount);
        i = i + 1;
      L3540:
        if (i < 5) goto L2967;
        i = 0;
        goto L3686;
      L3560:
        star = new cSprite(this.kitchenAtlas.getTexture("star_empty"));
        star.pivotX = star.width / 2;
        star.pivotY = star.height / 2;
        star.x = 4294967146;
        star.touchable = false;
        star.visible = false;
        this.starAr.push(star);
        this._global._gameS.addChild(star);
        i = i + 1;
      L3686:
        if (i < 10) goto L3560;
        this.itemName = new ExtendTextField(400, 150, "", "Pfennig", 35, 16777215);
        this.itemName.pivotX = 200;
        this.itemName.pivotY = 75;
        this.itemName.x = 512;
        this.itemName.y = 120;
        this.itemName.touchable = false;
        this.itemName.visible = false;
        this._global._gameS.addChild(this.itemName);
        this.upgradeInfo = new ExtendTextField(600, 70, "Current price : $45 ($10 increase)", "bromine", 25, 16777215);
        this.upgradeInfo.pivotX = 300;
        this.upgradeInfo.pivotY = 35;
        this.upgradeInfo.x = 512;
        this.upgradeInfo.y = 450;
        this.upgradeInfo.touchable = false;
        this.upgradeInfo.visible = false;
        this._global._gameS.addChild(this.upgradeInfo);
        this.upgradeCost = new ExtendTextField(400, 70, "Cost : 250", "bromine", 25, 16777215);
        this.upgradeCost.pivotX = 200;
        this.upgradeCost.pivotY = 35;
        this.upgradeCost.x = 512;
        this.upgradeCost.y = 490;
        this.upgradeCost.touchable = false;
        this.upgradeCost.visible = false;
        this._global._gameS.addChild(this.upgradeCost);
        this.rightButton = new TNonStateButton(this.kitchenAtlas.getTexture("arrow"), "", "bromine", 2, 0);
        this.rightButton.pivotX = this.rightButton.width / 2;
        this.rightButton.pivotY = this.rightButton.height / 2;
        this.rightButton.x = 712;
        this.rightButton.y = 300;
        this.rightButton.scaleX = -1;
        this.rightButton.visible = false;
        this.rightButton._text.touchable = false;
        this.rightButton._image.name = "" + RIGHT_BUTTON;
        this._global._gameS.addChild(this.rightButton);
        this.quadButtonAr.push(this.rightButton._image);
        this.leftButton = new TNonStateButton(this.kitchenAtlas.getTexture("arrow"), "", "bromine", 2, 0);
        this.leftButton.pivotX = this.leftButton.width / 2;
        this.leftButton.pivotY = this.leftButton.height / 2;
        this.leftButton.x = 312;
        this.leftButton.y = 300;
        this.leftButton.visible = false;
        this.leftButton._text.touchable = false;
        this.leftButton._image.name = "" + LEFT_BUTTON;
        this._global._gameS.addChild(this.leftButton);
        this.quadButtonAr.push(this.leftButton._image);
        this.buyButton = new TNonStateButton(this.kitchenAtlas.getTexture("small_btn_green"), "Buy", "bromine", 25, 0);
        this.buyButton.x = 580;
        this.buyButton.y = 535;
        this.buyButton._image.name = "" + BUY_BUTTON;
        this.buyButton._text.touchable = false;
        this.buyButton._text.width = this.buyButton._text.width - 15;
        this.buyButton._text.x = 7;
        this.buyButton.visible = false;
        this._global._gameS.addChild(this.buyButton);
        this.quadButtonAr.push(this.buyButton._image);
        this.backBtn = new TNonStateButton(this.kitchenAtlas.getTexture("small_btn_red"), "Back", "bromine", 25, 0);
        this.backBtn.x = 60;
        this.backBtn.y = 535;
        this.backBtn._image.name = "" + BACK_BUTTON;
        this.backBtn._text.touchable = false;
        this._global._gameS.addChild(this.backBtn);
        this.quadButtonAr.push(this.backBtn._image);
        this.moneyPreview.text = this._global.addCommas(this._global._maxCrypto.STI(TSavegame.currentPlayer.money));
        i = 0;
        goto L4657;
      L4597:
        this.buttonAr[i].useHandCursor = true;
        this.buttonAr[i].addEventListener(TouchEvent.TOUCH, this.touchButton);
        this.tweenBubbleText(i);
        i = i + 1;
      L4657:
        if (i < this.buttonAr.length) goto L4597;
        i = 0;
        goto L4733;
      L4682:
        this.quadButtonAr[i].useHandCursor = true;
        this.quadButtonAr[i].addEventListener(TouchEvent.TOUCH, this.touchQuad);
        i = i + 1;
      L4733:
        if (i < this.quadButtonAr.length) goto L4682;
        if (!(TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_CHINESE_DISH_AT)) goto L4864;
        if (TSavegame.currentPlayer.chineseDishLevel[0] != 0) goto L4864;
        TSavegame.currentPlayer.chineseDishLevel[0] = 1;
        TSavegame.currentPlayer.chineseDishLevel[1] = 1;
        TSavegame.currentPlayer.achievementCounter[Achievement.ASIAN_RECIPE] = TSavegame.currentPlayer.achievementCounter[Achievement.ASIAN_RECIPE] + 2;
        TSavegame.save();
      L4864:
        if (!(TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_MID_EAST_DISH_AT)) goto L4980;
        if (TSavegame.currentPlayer.mideastDishLevel[0] != 0) goto L4980;
        TSavegame.currentPlayer.mideastDishLevel[0] = 1;
        TSavegame.currentPlayer.mideastDishLevel[1] = 1;
        TSavegame.currentPlayer.achievementCounter[Achievement.MIDDLE_EAST_RECIPE] = TSavegame.currentPlayer.achievementCounter[Achievement.MIDDLE_EAST_RECIPE] + 2;
        TSavegame.save();
      L4980:
        if (!(TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_BEVERAGE_AT)) goto L5050;
        if (TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] != 0) goto L5050;
        TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] = 1;
        TSavegame.save();
      L5050:
        if (!(TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_FRIES_AT)) goto L5120;
        if (TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] != 0) goto L5120;
        TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] = 1;
        TSavegame.save();
      L5120:
        TGlobalClass._windowDisplay.initializeWindow(this._global.currentSceneID);
        TGlobalClass.currentNotifWindow = 0;
        var $t0 = TSavegame.currentPlayer.passedDays < 60;
        $s0 = $t0;
        if (!$t0) goto L5194;
        $s0 = TGlobalClass.levelEventXML.day[TSavegame.currentPlayer.passedDays].kitchen.length() > 0;
      L5194:
        if (!$s0) goto L5220;
        $s0 = TSavegame.currentPlayer.lastDayKitchen == TSavegame.currentPlayer.passedDays - 1;
      L5220:
        if (!$s0) goto L5249;
        TGlobalClass._windowDisplay.showKitchenWindow(TSavegame.currentPlayer.passedDays, TGlobalClass.currentNotifWindow);
      L5249:
        this._global.tutorial.checkTut(Tutorial.OPENING_STORY, 0);
        this._global.tutorial.checkTut(Tutorial.AFTER_LEVEL_1, 1);
        this._global.tutorial.checkTut(Tutorial.AFTER_LEVEL_2, 2);
        this._global.tutorial.checkTut(Tutorial.AFTER_LEVEL_7, 7);
        this._global.tutorial.checkTut(Tutorial.AFTER_LEVEL_8, 8);
        this._global.tutorial.checkTut(Tutorial.AFTER_LEVEL_34, 34);
        if (this._global.tutorial.currentDialogueID != Tutorial.OPENING_STORY) goto L5399;
        this.achievementBubble.visible = false;
        this.achievementQuad.visible = false;
      L5399:
        TSavegame.currentPlayer.lastDayKitchen = TSavegame.currentPlayer.passedDays;
        TSavegame.save();
        this.currentRecipeRegion = this.WESTERN;
        this._bolehKlik = true;
        ready = true;
    }

    function dialogueCondition(arg1:int, arg2:int):void {
    }

    function tweenBubbleText(arg1:int):void {
        idx = arg1;
        TweenLite.to(this.buttonAr[idx], 0.2, {"y": this.buttonAr[idx].y - 25, "delay": 0.5, "ease": Linear.easeNone, "onComplete": function(f1_arg1:int):void {
            TweenLite.to(buttonAr[f1_arg1], 0.4, {"y": buttonAr[f1_arg1].y + 25, "ease": Bounce.easeOut, "onComplete": tweenBubbleText, "onCompleteParams": [f1_arg1]});
        }, "onCompleteParams": [idx]});
    }

    function swipe(arg1:TouchEvent):void {
        if (!this._global.tutorial.button.visible) goto L20;
        return;
      L20:
        if (this._global.tutorial.canClickThisButton(-1)) goto L38;
        return;
      L38:
        var _local2 = arg1.getTouch(this._global._game);
        if (!arg1.getTouch(this._global._game, TouchPhase.BEGAN)) goto L96;
        this.startTouchX = _local2.globalX;
        if (this.itemIsTweening) goto L96;
        this.touchedBG = true;
      L96:
        if (!arg1.getTouch(this._global._game, TouchPhase.MOVED)) goto L172;
        if (!this.touchedBG) goto L172;
        if (!(_local2.globalX < this.startTouchX - 70)) goto L151;
        this.tweenItem(true);
        goto L172;
      L151:
        if (!(_local2.globalX > this.startTouchX + 70)) goto L172;
        this.tweenItem(false);
      L172:
    }

    function tweenItem(arg1:Boolean):void {
        i = 0;
        reverse = arg1;
        if (!reverse) goto L128;
        var $t0 = this.currentShop == this.INGREDIENT_SHOP;
        $s0 = $t0;
        if (!$t0) goto L53;
        $s0 = this.currentFocus == this.ingredientName.length - 1;
      L53:
        if (!$s0) goto L58;
        return;
      L58:
        var $t1 = this.currentShop == this.RECIPE_SHOP;
        $s0 = $t1;
        if (!$t1) goto L86;
        $s0 = this.currentFocus == this.westernDishName.length - 1;
      L86:
        if (!$s0) goto L91;
        return;
      L91:
        var $t2 = this.currentShop == this.HARDWARE_SHOP;
        $s0 = $t2;
        if (!$t2) goto L119;
        $s0 = this.currentFocus == this.hardwareName.length - 1;
      L119:
        if (!$s0) goto L124;
        return;
      L124:
        goto L139;
      L128:
        if (this.currentFocus != 0) goto L139;
        return;
      L139:
        this.touchedBG = false;
        this.itemIsTweening = true;
        $s0 = this;
        $s1 = this.currentFocus;
        if (!(reverse == true)) goto L171;
        $s2 = 1;
        goto L174;
      L171:
        $s2 = -1;
      L174:
        $s0.currentFocus = $s1 + $s2;
        _scaleX = Number(0);
        _scaleY = Number(0);
        alpha = Number(0);
        i = 0;
        goto L546;
      L209:
        if (!reverse) goto L436;
        if (!(this.shopItemContainer[i].x <= 512 - 2 * 550)) goto L432;
        if (this.currentShop != this.INGREDIENT_SHOP) goto L341;
        if (!(this.startPointLooping + 4 < this.ingredientName.length)) goto L337;
        var _local3 = this;
        var _local4 = this.startPointLooping + 1;
        _local3.startPointLooping = _local4;
        this.shopItemContainer[i].x = 512 + 1100;
        this.itemAr[i].itemIdx = this.startPointLooping + 3;
      L337:
        goto L432;
      L341:
        if (this.currentShop != this.RECIPE_SHOP) goto L432;
        if (!(this.startPointLooping + 4 < this.chineseDishName.length)) goto L432;
        _local3 = this;
        _local4 = this.startPointLooping + 1;
        _local3.startPointLooping = _local4;
        this.shopItemContainer[i].x = 512 + 1100;
        this.itemAr[i].itemIdx = this.startPointLooping + 3;
      L432:
        goto L536;
      L436:
        var $t3 = this.shopItemContainer[i].x >= 512 + 2 * 550;
        $s0 = $t3;
        if (!$t3) goto L474;
        $s0 = this.startPointLooping > 0;
      L474:
        if (!$s0) goto L536;
        this.shopItemContainer[i].x = 512 - 1100;
        _local3 = this;
        _local4 = this.startPointLooping - 1;
        _local3.startPointLooping = _local4;
        this.itemAr[i].itemIdx = this.startPointLooping;
      L536:
        i = i + 1;
      L546:
        if (i < 4) goto L209;
        i = 0;
        goto L738;
      L566:
        if (i != this.currentFocus % 4) goto L607;
        _scaleX = Number(1);
        _scaleY = Number(1);
        alpha = Number(1);
        goto L628;
      L607:
        _scaleX = Number(0.7);
        _scaleY = Number(0.7);
        alpha = Number(0.7);
      L628:
        $s0 = TweenLite;
        $s1 = this.shopItemContainer[i];
        $s2 = 0.4;
        $s3 = "x";
        $s4 = this.shopItemContainer[i].x;
        $s5 = 550;
        if (!(reverse == true)) goto L681;
        $s6 = -1;
        goto L684;
      L681:
        $s6 = 1;
      L684:
        $s0.to($s1, $s2, {$s3: $s4 + $s5 * $s6, "scaleX": _scaleX, "scaleY": _scaleY, "alpha": alpha, "ease": Expo.easeOut, "onComplete": function():void {
            itemIsTweening = false;
        }});
        i = i + 1;
      L738:
        if (i < 4) goto L566;
        this.showShop(this.currentShop);
    }

    function showShop(arg1:int):void {
        var _local2 = 0;
        var _local3 = 0;
        var _local4 = 0;
        var _local5 = 0;
        var _local6 = null;
        var _local7 = null;
        var _local8 = null;
        var _local9 = null;
        var _local10 = 0;
        var _local11 = null;
        var _local12 = 0;
        var _local13 = 0;
        this.currentShop = arg1;
        this.shopBackground.visible = true;
        this.shopName.visible = true;
        this.itemName.visible = true;
        this.upgradeCost.visible = true;
        this.upgradeInfo.visible = true;
        this.buyButton.visible = true;
        this.moneyPlaceBG.visible = true;
        this.coinPreview.visible = true;
        this.moneyPreview.visible = true;
        this.leftButton.visible = true;
        this.rightButton.visible = true;
        this.shopCoin.visible = true;
        this.backBtn.x = 800;
        if (arg1 != this.INGREDIENT_SHOP) goto L1634;
        this.shopName.text = "MILLY'S INGREDIENT MARKET";
        _local2 = 0;
        goto L359;
      L189:
        this.itemAr[_local2].visible = true;
        this.plateAr[_local2].visible = true;
        this.itemAr[_local2].texture = this.kitchenAtlas.getTexture(this.ingredientName[this.itemAr[_local2].itemIdx]);
        this.itemAr[_local2].readjustSize();
        this.itemAr[_local2].pivotX = this.itemAr[_local2].width / 2;
        this.itemAr[_local2].pivotY = this.itemAr[_local2].height / 2;
        this.itemAr[_local2].x = this.plateAr[_local2].x;
        this.itemAr[_local2].y = this.plateAr[_local2].y;
        _local2++;
      L359:
        if (_local2 < 5) goto L189;
        this.itemName.text = this.ingredientName[this.currentFocus].replace("_", " ");
        if (this.itemName.text != "selada") goto L418;
        this.itemName.text = "vegetables";
      L418:
        _local9 = [];
        _local9 = this.itemName.text.split(" ");
        _local2 = 0;
        goto L487;
      L449:
        _local9[_local2] = _local9[_local2].charAt(0).toUpperCase() + _local9[_local2].substr(1);
        _local2++;
      L487:
        if (_local2 < _local9.length) goto L449;
        this.itemName.text = _local9.join(" ");
        if (!(this.currentFocus >= this.ingredientName.length - 1)) goto L541;
        this.rightButton.visible = false;
        goto L559;
      L541:
        if (!(this.currentFocus <= 0)) goto L559;
        this.leftButton.visible = false;
      L559:
        _local5 = int(TGlobalClass.ingredientCost[this.currentFocus][TSavegame.currentPlayer.ingredientLevel[this.currentFocus]]);
        if (!(TSavegame.currentPlayer.ingredientLevel[this.currentFocus] > 0)) goto L791;
        _local2 = 1;
        goto L780;
      L623:
        var $t0 = TSavegame.currentPlayer.ingredientLevel[this.currentFocus] > 0;
        $s0 = $t0;
        if (!$t0) goto L667;
        $s0 = _local2 < TSavegame.currentPlayer.ingredientLevel[this.currentFocus];
      L667:
        if (!$s0) goto L698;
        this.starAr[_local2 - 1].texture = this.kitchenAtlas.getTexture("star");
        goto L721;
      L698:
        this.starAr[_local2 - 1].texture = this.kitchenAtlas.getTexture("star_empty");
      L721:
        this.starAr[_local2 - 1].visible = true;
        this.starAr[_local2 - 1].x = 512 - 4 * 60 + _local2 * 60 - 90;
        this.starAr[_local2 - 1].y = 410;
        _local2++;
      L780:
        if (_local2 < TGlobalClass.MAX_INGREDIENT_LEVEL) goto L623;
      L791:
        _local2 = 0;
        goto L1074;
      L798:
        var $t1 = _local2 < TGlobalClass.TOTAL_WESTERN_DISH;
        $s0 = $t1;
        if (!$t1) goto L829;
        $s0 = TSavegame.currentPlayer.westernDishLevel[_local2] == 0;
      L829:
        if (!$s0) goto L837;
        goto L1072;
      L837:
        var $t2 = _local2 >= TGlobalClass.TOTAL_WESTERN_DISH;
        $s0 = $t2;
        if (!$t2) goto L866;
        $s0 = _local2 < TGlobalClass.TOTAL_CHINESE_DISH + TGlobalClass.TOTAL_WESTERN_DISH;
      L866:
        if (!$s0) goto L913;
        var $t3 = TSavegame.currentPlayer.chineseDishLevel[_local2 - TGlobalClass.TOTAL_WESTERN_DISH] == 0;
        $s0 = $t3;
        if ($t3) goto L913;
        $s0 = TSavegame.currentPlayer.passedDays < 10;
      L913:
        if (!$s0) goto L921;
        goto L1072;
      L921:
        var $t4 = _local2 >= TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH;
        $s0 = $t4;
        if (!$t4) goto L990;
        var $t5 = TSavegame.currentPlayer.mideastDishLevel[_local2 - TGlobalClass.TOTAL_WESTERN_DISH - TGlobalClass.TOTAL_CHINESE_DISH] == 0;
        $s0 = $t5;
        if ($t5) goto L990;
        $s0 = TSavegame.currentPlayer.passedDays < 20;
      L990:
        if (!$s0) goto L998;
        goto L1072;
      L998:
        _local11 = this._global.dish.getDishIngredientType(_local2);
        _local3 = 0;
        goto L1062;
      L1021:
        $s0 = this.currentFocus;
        $s1 = _local11[_local3];
        if (!(this.currentFocus > 8)) goto L1050;
        $s2 = 2;
        goto L1053;
      L1050:
        $s2 = 1;
      L1053:
        if ($s0 != $s1 - $s2) goto L1060;
        _local10++;
      L1060:
        _local3++;
      L1062:
        if (_local3 < _local11.length) goto L1021;
      L1072:
        _local2++;
      L1074:
        if (_local2 < TGlobalClass.TOTAL_CHINESE_DISH + TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_MIDEAST_DISH) goto L798;
        if (!(_local10 > 0)) goto L1549;
        $s0 = this.upgradeInfo;
        $s1 = "Upgrading this will increase " + _local10;
        if (!(_local10 > 1)) goto L1134;
        $s2 = " dishes";
        goto L1138;
      L1134:
        $s2 = " dish";
      L1138:
        $s0.text = $s1 + $s2 + " by $1";
        var $t6 = this._global._maxCrypto.STI(TSavegame.currentPlayer.money) < _local5;
        $s0 = $t6;
        if (!$t6) goto L1198;
        $s0 = TSavegame.currentPlayer.ingredientLevel[this.currentFocus] < TGlobalClass.MAX_INGREDIENT_LEVEL;
      L1198:
        if (!$s0) goto L1269;
        this.buyButton._text.text = "Can't Afford";
        this.upgradeCost.text = "Cost : " + this._global.addCommas(_local5);
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_red");
        this.buyButton.touchable = false;
        goto L1545;
      L1269:
        if (TSavegame.currentPlayer.ingredientLevel[this.currentFocus] != 0) goto L1358;
        this.buyButton._text.text = "Buy";
        this.upgradeCost.text = "Cost : " + this._global.addCommas(_local5);
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_green");
        this.buyButton.touchable = true;
        goto L1545;
      L1358:
        var $t7 = TSavegame.currentPlayer.ingredientLevel[this.currentFocus] > 0;
        $s0 = $t7;
        if (!$t7) goto L1406;
        $s0 = TSavegame.currentPlayer.ingredientLevel[this.currentFocus] < TGlobalClass.MAX_INGREDIENT_LEVEL;
      L1406:
        if (!$s0) goto L1477;
        this.buyButton._text.text = "Upgrade";
        this.upgradeCost.text = "Cost : " + this._global.addCommas(_local5);
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
        this.buyButton.touchable = true;
        goto L1545;
      L1477:
        this.upgradeInfo.text = "You've maxed this ingredient quality.";
        this.buyButton._text.text = "Maxed";
        this.upgradeCost.visible = false;
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_green");
        this.buyButton.touchable = false;
        this.shopCoin.visible = false;
      L1545:
        goto L1630;
      L1549:
        if (!(_local10 <= 0)) goto L1630;
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_red");
        this.buyButton._text.text = "Sold Out!";
        this.upgradeCost.text = "Cost : " + this._global.addCommas(_local5);
        this.buyButton.touchable = false;
        this.upgradeInfo.text = "No recipe with this ingredient yet.";
      L1630:
        goto L8150;
      L1634:
        if (arg1 != this.RECIPE_SHOP) goto L6915;
        this.westernButton.visible = TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_CHINESE_DISH_AT;
        this.chineseButton.visible = TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_CHINESE_DISH_AT;
        this.midEastButton.visible = TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_MID_EAST_DISH_AT;
        this.shopName.text = "EMMA'S RECIPES";
        if (this.currentRecipeRegion != this.WESTERN) goto L3166;
        this.westernButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue_dark");
        this.chineseButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
        this.midEastButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
        if (!(this.currentFocus >= this.westernDishName.length - 1)) goto L1825;
        this.rightButton.visible = false;
        goto L1843;
      L1825:
        if (!(this.currentFocus <= 0)) goto L1843;
        this.leftButton.visible = false;
      L1843:
        this.itemName.text = this.westernDishName[this.currentFocus];
        goto L1890;
      L1865:
        this.itemName.text = this.itemName.text.replace("_", " ");
      L1890:
        if (this.itemName.text.indexOf("_") > 0) goto L1865;
        _local9 = [];
        _local9 = this.itemName.text.split(" ");
        _local2 = 0;
        goto L1979;
      L1941:
        _local9[_local2] = _local9[_local2].charAt(0).toUpperCase() + _local9[_local2].substr(1);
        _local2++;
      L1979:
        if (_local2 < _local9.length) goto L1941;
        this.itemName.text = _local9.join(" ");
        _local4 = 0;
        _local6 = this._global.dish.getDishIngredientType(this.currentFocus);
        _local7 = this._global.dish.getDishTarget(this.currentFocus);
        _local3 = 0;
        goto L2102;
      L2054:
        _local4 = int(_local4 + (TSavegame.currentPlayer.ingredientLevel[_local6[_local3] - 1] - 1) * this._global.dish.getIngredientPriceIncrease(_local6[_local3] - 1));
        _local3++;
      L2102:
        if (_local3 < _local6.length) goto L2054;
        if (!(TSavegame.currentPlayer.westernDishLevel[this.currentFocus] > 0)) goto L2234;
        _local4 = int(_local4 + (this._global.dish.getDishDefaultPrice(this.currentFocus) + (TSavegame.currentPlayer.westernDishLevel[this.currentFocus] - 1) * this._global.dish.getPriceIncrease(this.currentFocus)));
        this.upgradeInfo.text = "Current price : $" + _local4 + " ($" + this._global.dish.getPriceIncrease(this.currentFocus) + " increase)";
        _local8 = "";
        goto L2305;
      L2234:
        _local12 = int(_local4);
        _local4 = int(_local4 + this._global.dish.getDishDefaultPrice(this.currentFocus));
        $s0 = this.upgradeInfo;
        $s1 = "Dish price : $" + _local4;
        if (!(_local12 > 0)) goto L2294;
        $s2 = " ($" + _local12 + " ingredient bonus)";
        goto L2297;
      L2294:
        $s2 = "";
      L2297:
        $s0.text = $s1 + $s2;
        _local8 = "";
      L2305:
        _local5 = int(TGlobalClass.westernUpgradeCost[this.currentFocus][TSavegame.currentPlayer.westernDishLevel[this.currentFocus]]);
        if (!(TSavegame.currentPlayer.westernDishLevel[this.currentFocus] > 0)) goto L2534;
        _local3 = 1;
        goto L2519;
      L2369:
        var $t8 = TSavegame.currentPlayer.westernDishLevel[this.currentFocus] > 1;
        $s0 = $t8;
        if (!$t8) goto L2414;
        $s0 = _local3 <= TSavegame.currentPlayer.westernDishLevel[this.currentFocus] - 1;
      L2414:
        if (!$s0) goto L2444;
        this.starAr[_local3].texture = this.kitchenAtlas.getTexture("star");
        goto L2466;
      L2444:
        this.starAr[_local3].texture = this.kitchenAtlas.getTexture("star_empty");
      L2466:
        this.starAr[_local3].visible = true;
        this.starAr[_local3].x = 512 - 2 * 60 + _local3 * 60;
        this.starAr[_local3].y = 410;
        _local3++;
      L2519:
        if (_local3 < TGlobalClass.MAX_DISH_LEVEL) goto L2369;
        goto L2567;
      L2534:
        _local3 = 1;
        goto L2556;
      L2541:
        this.starAr[_local3].visible = false;
        _local3++;
      L2556:
        if (_local3 < TGlobalClass.MAX_DISH_LEVEL) goto L2541;
      L2567:
        var $t9 = this.currentFocus == 5;
        $s0 = $t9;
        if (!$t9) goto L2629;
        $s0 = TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] < int(this._global.dish.dishXML.dish[this.currentFocus].@expertNeeded);
      L2629:
        if (!$s0) goto L2740;
        this.buyButton._text.text = "Info";
        this.buyButton.idx = int(this._global.dish.dishXML.dish[this.currentFocus].@expertNeeded);
        this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_red");
        this.buyButton.touchable = true;
        goto L3151;
      L2740:
        var $t10 = this._global._maxCrypto.STI(TSavegame.currentPlayer.money) < _local5;
        $s0 = $t10;
        if (!$t10) goto L2792;
        $s0 = TSavegame.currentPlayer.westernDishLevel[this.currentFocus] < TGlobalClass.MAX_DISH_LEVEL;
      L2792:
        if (!$s0) goto L2866;
        this.buyButton._text.text = "Can't Afford";
        this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_red");
        this.buyButton.touchable = false;
        goto L3151;
      L2866:
        if (TSavegame.currentPlayer.westernDishLevel[this.currentFocus] != 0) goto L2958;
        this.buyButton._text.text = "Buy";
        this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_green");
        this.buyButton.touchable = true;
        goto L3151;
      L2958:
        var $t11 = TSavegame.currentPlayer.westernDishLevel[this.currentFocus] > 0;
        $s0 = $t11;
        if (!$t11) goto L3006;
        $s0 = TSavegame.currentPlayer.westernDishLevel[this.currentFocus] < TGlobalClass.MAX_DISH_LEVEL;
      L3006:
        if (!$s0) goto L3080;
        this.buyButton._text.text = "Upgrade";
        this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
        this.buyButton.touchable = true;
        goto L3151;
      L3080:
        this.upgradeInfo.text = "Current price : $" + _local4;
        this.buyButton._text.text = "Maxed";
        this.upgradeCost.visible = false;
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_green");
        this.buyButton.touchable = false;
        this.shopCoin.visible = false;
      L3151:
        this.buyButton._text.autoScale = true;
        goto L6188;
      L3166:
        if (this.currentRecipeRegion != this.CHINESE) goto L4658;
        this.chineseButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue_dark");
        this.westernButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
        this.midEastButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
        if (!(this.currentFocus >= this.chineseDishName.length - 1)) goto L3269;
        this.rightButton.visible = false;
        goto L3287;
      L3269:
        if (!(this.currentFocus <= 0)) goto L3287;
        this.leftButton.visible = false;
      L3287:
        this.itemName.text = this.chineseDishName[this.currentFocus];
        goto L3334;
      L3309:
        this.itemName.text = this.itemName.text.replace("_", " ");
      L3334:
        if (this.itemName.text.indexOf("_") > 0) goto L3309;
        _local9 = [];
        _local9 = this.itemName.text.split(" ");
        _local2 = 0;
        goto L3423;
      L3385:
        _local9[_local2] = _local9[_local2].charAt(0).toUpperCase() + _local9[_local2].substr(1);
        _local2++;
      L3423:
        if (_local2 < _local9.length) goto L3385;
        this.itemName.text = _local9.join(" ");
        _local4 = 0;
        _local6 = this._global.dish.getDishIngredientType(this.currentFocus + TGlobalClass.TOTAL_WESTERN_DISH);
        _local7 = this._global.dish.getDishTarget(this.currentFocus + TGlobalClass.TOTAL_WESTERN_DISH);
        _local3 = 0;
        goto L3560;
      L3512:
        _local4 = int(_local4 + (TSavegame.currentPlayer.ingredientLevel[_local6[_local3] - 1] - 1) * this._global.dish.getIngredientPriceIncrease(_local6[_local3] - 1));
        _local3++;
      L3560:
        if (_local3 < _local6.length) goto L3512;
        if (!(TSavegame.currentPlayer.chineseDishLevel[this.currentFocus] > 0)) goto L3713;
        _local4 = int(_local4 + (this._global.dish.getDishDefaultPrice(this.currentFocus + TGlobalClass.TOTAL_WESTERN_DISH) + (TSavegame.currentPlayer.chineseDishLevel[this.currentFocus] - 1) * this._global.dish.getPriceIncrease(this.currentFocus + TGlobalClass.TOTAL_WESTERN_DISH)));
        this.upgradeInfo.text = "Current price : $" + _local4 + " ($" + this._global.dish.getPriceIncrease(this.currentFocus + TGlobalClass.TOTAL_WESTERN_DISH) + " increase)";
        _local8 = "";
        goto L3791;
      L3713:
        _local12 = int(_local4);
        _local4 = int(_local4 + this._global.dish.getDishDefaultPrice(this.currentFocus + TGlobalClass.TOTAL_WESTERN_DISH));
        $s0 = this.upgradeInfo;
        $s1 = "Dish price : $" + _local4;
        if (!(_local12 > 0)) goto L3780;
        $s2 = " ($" + _local12 + " ingredient bonus)";
        goto L3783;
      L3780:
        $s2 = "";
      L3783:
        $s0.text = $s1 + $s2;
        _local8 = "";
      L3791:
        _local5 = int(TGlobalClass.chineseUpgradeCost[this.currentFocus][TSavegame.currentPlayer.chineseDishLevel[this.currentFocus]]);
        if (!(TSavegame.currentPlayer.chineseDishLevel[this.currentFocus] > 0)) goto L4020;
        _local3 = 1;
        goto L4005;
      L3855:
        var $t12 = TSavegame.currentPlayer.chineseDishLevel[this.currentFocus] > 1;
        $s0 = $t12;
        if (!$t12) goto L3900;
        $s0 = _local3 <= TSavegame.currentPlayer.chineseDishLevel[this.currentFocus] - 1;
      L3900:
        if (!$s0) goto L3930;
        this.starAr[_local3].texture = this.kitchenAtlas.getTexture("star");
        goto L3952;
      L3930:
        this.starAr[_local3].texture = this.kitchenAtlas.getTexture("star_empty");
      L3952:
        this.starAr[_local3].visible = true;
        this.starAr[_local3].x = 512 - 2 * 60 + _local3 * 60;
        this.starAr[_local3].y = 410;
        _local3++;
      L4005:
        if (_local3 < TGlobalClass.MAX_DISH_LEVEL) goto L3855;
        goto L4053;
      L4020:
        _local3 = 1;
        goto L4042;
      L4027:
        this.starAr[_local3].visible = false;
        _local3++;
      L4042:
        if (_local3 < TGlobalClass.MAX_DISH_LEVEL) goto L4027;
      L4053:
        var $t13 = this.currentFocus == 5;
        $s0 = $t13;
        if (!$t13) goto L4118;
        $s0 = TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] < int(this._global.dish.dishXML.dish[6 + this.currentFocus].@expertNeeded);
      L4118:
        if (!$s0) goto L4232;
        this.buyButton._text.text = "Info";
        this.buyButton.idx = int(this._global.dish.dishXML.dish[6 + this.currentFocus].@expertNeeded);
        this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_red");
        this.buyButton.touchable = true;
        goto L4643;
      L4232:
        var $t14 = this._global._maxCrypto.STI(TSavegame.currentPlayer.money) < _local5;
        $s0 = $t14;
        if (!$t14) goto L4284;
        $s0 = TSavegame.currentPlayer.chineseDishLevel[this.currentFocus] < TGlobalClass.MAX_DISH_LEVEL;
      L4284:
        if (!$s0) goto L4358;
        this.buyButton._text.text = "Can't Afford";
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_red");
        this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
        this.buyButton.touchable = false;
        goto L4643;
      L4358:
        if (TSavegame.currentPlayer.chineseDishLevel[this.currentFocus] != 0) goto L4450;
        this.buyButton._text.text = "Buy";
        this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_green");
        this.buyButton.touchable = true;
        goto L4643;
      L4450:
        var $t15 = TSavegame.currentPlayer.chineseDishLevel[this.currentFocus] > 0;
        $s0 = $t15;
        if (!$t15) goto L4498;
        $s0 = TSavegame.currentPlayer.chineseDishLevel[this.currentFocus] < TGlobalClass.MAX_DISH_LEVEL;
      L4498:
        if (!$s0) goto L4572;
        this.buyButton._text.text = "Upgrade";
        this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
        this.buyButton.touchable = true;
        goto L4643;
      L4572:
        this.upgradeInfo.text = "Current price : $" + _local4;
        this.buyButton._text.text = "Maxed";
        this.upgradeCost.visible = false;
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_green");
        this.buyButton.touchable = false;
        this.shopCoin.visible = false;
      L4643:
        this.buyButton._text.autoScale = true;
        goto L6188;
      L4658:
        if (this.currentRecipeRegion != this.MID_EAST) goto L6188;
        this.midEastButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue_dark");
        this.chineseButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
        this.westernButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
        if (!(this.currentFocus >= this.mideastDishName.length - 1)) goto L4761;
        this.rightButton.visible = false;
        goto L4779;
      L4761:
        if (!(this.currentFocus <= 0)) goto L4779;
        this.leftButton.visible = false;
      L4779:
        this.itemName.text = this.mideastDishName[this.currentFocus];
        goto L4826;
      L4801:
        this.itemName.text = this.itemName.text.replace("_", " ");
      L4826:
        if (this.itemName.text.indexOf("_") > 0) goto L4801;
        _local9 = [];
        _local9 = this.itemName.text.split(" ");
        _local2 = 0;
        goto L4915;
      L4877:
        _local9[_local2] = _local9[_local2].charAt(0).toUpperCase() + _local9[_local2].substr(1);
        _local2++;
      L4915:
        if (_local2 < _local9.length) goto L4877;
        this.itemName.text = _local9.join(" ");
        _local4 = 0;
        _local6 = this._global.dish.getDishIngredientType(this.currentFocus + TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH);
        _local7 = this._global.dish.getDishTarget(this.currentFocus + TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH);
        _local3 = 0;
        goto L5066;
      L5018:
        _local4 = int(_local4 + (TSavegame.currentPlayer.ingredientLevel[_local6[_local3] - 1] - 1) * this._global.dish.getIngredientPriceIncrease(_local6[_local3] - 1));
        _local3++;
      L5066:
        if (_local3 < _local6.length) goto L5018;
        if (!(TSavegame.currentPlayer.mideastDishLevel[this.currentFocus] > 0)) goto L5240;
        _local4 = int(_local4 + (this._global.dish.getDishDefaultPrice(this.currentFocus + TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH) + (TSavegame.currentPlayer.mideastDishLevel[this.currentFocus] - 1) * this._global.dish.getPriceIncrease(this.currentFocus + TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH)));
        this.upgradeInfo.text = "Current price : $" + _local4 + " ($" + this._global.dish.getPriceIncrease(this.currentFocus + TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH) + " increase)";
        _local8 = "";
        goto L5325;
      L5240:
        _local12 = int(_local4);
        _local4 = int(_local4 + this._global.dish.getDishDefaultPrice(this.currentFocus + TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH));
        $s0 = this.upgradeInfo;
        $s1 = "Dish price : $" + _local4;
        if (!(_local12 > 0)) goto L5314;
        $s2 = " ($" + _local12 + " ingredient bonus)";
        goto L5317;
      L5314:
        $s2 = "";
      L5317:
        $s0.text = $s1 + $s2;
        _local8 = "";
      L5325:
        _local5 = int(TGlobalClass.mideastUpgradeCost[this.currentFocus][TSavegame.currentPlayer.mideastDishLevel[this.currentFocus]]);
        if (!(TSavegame.currentPlayer.mideastDishLevel[this.currentFocus] > 0)) goto L5554;
        _local3 = 1;
        goto L5539;
      L5389:
        var $t16 = TSavegame.currentPlayer.mideastDishLevel[this.currentFocus] > 1;
        $s0 = $t16;
        if (!$t16) goto L5434;
        $s0 = _local3 <= TSavegame.currentPlayer.mideastDishLevel[this.currentFocus] - 1;
      L5434:
        if (!$s0) goto L5464;
        this.starAr[_local3].texture = this.kitchenAtlas.getTexture("star");
        goto L5486;
      L5464:
        this.starAr[_local3].texture = this.kitchenAtlas.getTexture("star_empty");
      L5486:
        this.starAr[_local3].visible = true;
        this.starAr[_local3].x = 512 - 2 * 60 + _local3 * 60;
        this.starAr[_local3].y = 410;
        _local3++;
      L5539:
        if (_local3 < TGlobalClass.MAX_DISH_LEVEL) goto L5389;
        goto L5587;
      L5554:
        _local3 = 1;
        goto L5576;
      L5561:
        this.starAr[_local3].visible = false;
        _local3++;
      L5576:
        if (_local3 < TGlobalClass.MAX_DISH_LEVEL) goto L5561;
      L5587:
        var $t17 = this.currentFocus == 5;
        $s0 = $t17;
        if (!$t17) goto L5652;
        $s0 = TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] < int(this._global.dish.dishXML.dish[12 + this.currentFocus].@expertNeeded);
      L5652:
        if (!$s0) goto L5766;
        this.buyButton._text.text = "Info";
        this.buyButton.idx = int(this._global.dish.dishXML.dish[12 + this.currentFocus].@expertNeeded);
        this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_red");
        this.buyButton.touchable = true;
        goto L6177;
      L5766:
        var $t18 = this._global._maxCrypto.STI(TSavegame.currentPlayer.money) < _local5;
        $s0 = $t18;
        if (!$t18) goto L5818;
        $s0 = TSavegame.currentPlayer.mideastDishLevel[this.currentFocus] < TGlobalClass.MAX_DISH_LEVEL;
      L5818:
        if (!$s0) goto L5892;
        this.buyButton._text.text = "Can't Afford";
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_red");
        this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
        this.buyButton.touchable = false;
        goto L6177;
      L5892:
        if (TSavegame.currentPlayer.mideastDishLevel[this.currentFocus] != 0) goto L5984;
        this.buyButton._text.text = "Buy";
        this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_green");
        this.buyButton.touchable = true;
        goto L6177;
      L5984:
        var $t19 = TSavegame.currentPlayer.mideastDishLevel[this.currentFocus] > 0;
        $s0 = $t19;
        if (!$t19) goto L6032;
        $s0 = TSavegame.currentPlayer.mideastDishLevel[this.currentFocus] < TGlobalClass.MAX_DISH_LEVEL;
      L6032:
        if (!$s0) goto L6106;
        this.buyButton._text.text = "Upgrade";
        this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
        this.buyButton.touchable = true;
        goto L6177;
      L6106:
        this.upgradeInfo.text = "Current price : $" + _local4;
        this.buyButton._text.text = "Maxed";
        this.upgradeCost.visible = false;
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_green");
        this.buyButton.touchable = false;
        this.shopCoin.visible = false;
      L6177:
        this.buyButton._text.autoScale = true;
      L6188:
        _local2 = 0;
        goto L6761;
      L6195:
        this.itemAr[_local2].visible = true;
        this.plateAr[_local2].visible = true;
        this.ingredientCountAr[_local2].visible = false;
        this.ingredientPreviewAr[_local2].visible = false;
        if (this.currentRecipeRegion != this.WESTERN) goto L6417;
        this.itemAr[_local2].texture = this.kitchenAtlas.getTexture(this.westernDishName[this.itemAr[_local2].itemIdx]);
        this.itemAr[_local2].readjustSize();
        this.itemAr[_local2].pivotX = this.itemAr[_local2].width / 2;
        this.itemAr[_local2].pivotY = this.itemAr[_local2].height / 2;
        this.itemAr[_local2].x = this.posXDif_Western[this.itemAr[_local2].itemIdx];
        this.itemAr[_local2].y = this.posYDif_Western[this.itemAr[_local2].itemIdx];
        goto L6759;
      L6417:
        if (this.currentRecipeRegion != this.CHINESE) goto L6590;
        this.itemAr[_local2].texture = this.kitchenAtlas.getTexture(this.chineseDishName[this.itemAr[_local2].itemIdx]);
        this.itemAr[_local2].readjustSize();
        this.itemAr[_local2].pivotX = this.itemAr[_local2].width / 2;
        this.itemAr[_local2].pivotY = this.itemAr[_local2].height / 2;
        this.itemAr[_local2].x = this.posXDif_Chinese[this.itemAr[_local2].itemIdx];
        this.itemAr[_local2].y = this.posYDif_Chinese[this.itemAr[_local2].itemIdx];
        goto L6759;
      L6590:
        if (this.currentRecipeRegion != this.MID_EAST) goto L6759;
        this.itemAr[_local2].texture = this.kitchenAtlas.getTexture(this.mideastDishName[this.itemAr[_local2].itemIdx]);
        this.itemAr[_local2].readjustSize();
        this.itemAr[_local2].pivotX = this.itemAr[_local2].width / 2;
        this.itemAr[_local2].pivotY = this.itemAr[_local2].height / 2;
        this.itemAr[_local2].x = this.posXDif_MidEast[this.itemAr[_local2].itemIdx];
        this.itemAr[_local2].y = this.posYDif_MidEast[this.itemAr[_local2].itemIdx];
      L6759:
        _local2++;
      L6761:
        if (_local2 < 5) goto L6195;
        _local2 = 0;
        goto L6901;
      L6775:
        var $t20 = int(_local6[_local2]);
        _local13 = $t20;
        if (!($t20 < 10)) goto L6802;
        _local13 = int(_local13 - 1);
        goto L6810;
      L6802:
        _local13 = int(_local13 - 2);
      L6810:
        this.ingredientPreviewAr[_local2].texture = this.kitchenAtlas.getTexture(this.ingredientName[_local13] + "_small");
        this.ingredientPreviewAr[_local2].readjustSize();
        this.ingredientPreviewAr[_local2].visible = true;
        this.ingredientCountAr[_local2].visible = true;
        this.ingredientCountAr[_local2].text = "x" + _local7[_local2];
        _local2++;
      L6901:
        if (_local2 < _local6.length) goto L6775;
        goto L8150;
      L6915:
        if (arg1 != this.HARDWARE_SHOP) goto L8150;
        this.shopName.text = "HYPERMART";
        if (!(this.currentFocus >= this.hardwareName.length - 1)) goto L6962;
        this.rightButton.visible = false;
        goto L6980;
      L6962:
        if (!(this.currentFocus <= 0)) goto L6980;
        this.leftButton.visible = false;
      L6980:
        this.itemName.text = this.hardwareName[this.currentFocus];
        _local9 = [];
        _local9 = this.itemName.text.split(" ");
        _local2 = 0;
        goto L7067;
      L7029:
        _local9[_local2] = _local9[_local2].charAt(0).toUpperCase() + _local9[_local2].substr(1);
        _local2++;
      L7067:
        if (_local2 < _local9.length) goto L7029;
        this.itemName.text = _local9.join(" ");
        _local3 = 1;
        goto L7250;
      L7100:
        var $t21 = TSavegame.currentPlayer.hardwareLevel[this.currentFocus] > 1;
        $s0 = $t21;
        if (!$t21) goto L7145;
        $s0 = _local3 <= TSavegame.currentPlayer.hardwareLevel[this.currentFocus] - 1;
      L7145:
        if (!$s0) goto L7175;
        this.starAr[_local3].texture = this.kitchenAtlas.getTexture("star");
        goto L7197;
      L7175:
        this.starAr[_local3].texture = this.kitchenAtlas.getTexture("star_empty");
      L7197:
        this.starAr[_local3].visible = true;
        this.starAr[_local3].x = 512 - 3 * 60 + _local3 * 60;
        this.starAr[_local3].y = 410;
        _local3++;
      L7250:
        if (_local3 <= 5) goto L7100;
        var $t22 = TSavegame.currentPlayer.hardwareLevel[this.currentFocus] > 0;
        $s0 = $t22;
        if (!$t22) goto L7305;
        $s0 = TSavegame.currentPlayer.hardwareLevel[this.currentFocus] < TGlobalClass.MAX_HARDWARE_LEVEL;
      L7305:
        if (!$s0) goto L7453;
        _local4 = int(this._global.dish.getHardwareDefaultPrice(this.currentFocus) + (TSavegame.currentPlayer.hardwareLevel[this.currentFocus] - 1) * this._global.dish.getHardwarePriceIncrease(this.currentFocus));
        if (this.currentFocus == 2) goto L7412;
        this.upgradeInfo.text = "Current price : $" + _local4 + " ($" + this._global.dish.getHardwarePriceIncrease(this.currentFocus) + " increase)";
        goto L7449;
      L7412:
        this.upgradeInfo.text = "Current Effect : " + _local4 + "% (" + this._global.dish.getHardwarePriceIncrease(this.currentFocus) + "% increase)";
      L7449:
        goto L7549;
      L7453:
        _local4 = int(this._global.dish.getHardwareDefaultPrice(this.currentFocus) + (TSavegame.currentPlayer.hardwareLevel[this.currentFocus] - 1) * this._global.dish.getHardwarePriceIncrease(this.currentFocus));
        if (this.currentFocus == 2) goto L7532;
        this.upgradeInfo.text = "Current Price : $" + _local4;
        goto L7549;
      L7532:
        this.upgradeInfo.text = "Current Effect : " + _local4 + "%";
      L7549:
        _local5 = int(this._global.dish.getHardwareUpgradecost(this.currentFocus)[TSavegame.currentPlayer.hardwareLevel[this.currentFocus]]);
        var $t23 = this._global._maxCrypto.STI(TSavegame.currentPlayer.money) < _local5;
        $s0 = $t23;
        if (!$t23) goto L7638;
        $s0 = TSavegame.currentPlayer.hardwareLevel[this.currentFocus] < TGlobalClass.MAX_HARDWARE_LEVEL;
      L7638:
        if (!$s0) goto L7717;
        this.buyButton._text.text = "Can't Afford";
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_red");
        this.upgradeInfo.visible = true;
        this.upgradeCost.text = "Cost : " + this._global.addCommas(_local5);
        this.buyButton.touchable = false;
        goto L7967;
      L7717:
        if (TSavegame.currentPlayer.hardwareLevel[this.currentFocus] != 0) goto L7782;
        this.buyButton._text.text = "Buy";
        this.buyButton.visible = false;
        this.upgradeInfo.visible = false;
        this.upgradeCost.text = "NOT AVAILABLE";
        goto L7967;
      L7782:
        var $t24 = TSavegame.currentPlayer.hardwareLevel[this.currentFocus] > 0;
        $s0 = $t24;
        if (!$t24) goto L7830;
        $s0 = TSavegame.currentPlayer.hardwareLevel[this.currentFocus] < TGlobalClass.MAX_HARDWARE_LEVEL;
      L7830:
        if (!$s0) goto L7909;
        this.buyButton._text.text = "Upgrade";
        this.upgradeInfo.visible = true;
        this.upgradeCost.text = "Cost : " + this._global.addCommas(_local5);
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
        this.buyButton.touchable = true;
        goto L7967;
      L7909:
        this.buyButton._text.text = "Maxed";
        this.upgradeCost.visible = false;
        this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_green");
        this.buyButton.touchable = false;
        this.shopCoin.visible = false;
      L7967:
        this.buyButton._text.autoScale = true;
        _local2 = 0;
        goto L8143;
      L7985:
        this.itemAr[_local2].visible = true;
        this.itemAr[_local2].texture = this.kitchenAtlas.getTexture(this.hardwareName[this.itemAr[_local2].itemIdx]);
        this.itemAr[_local2].readjustSize();
        this.itemAr[_local2].pivotX = this.itemAr[_local2].width / 2;
        this.itemAr[_local2].pivotY = this.itemAr[_local2].height / 2;
        this.itemAr[_local2].x = this.plateAr[_local2].x;
        this.itemAr[_local2].y = this.plateAr[_local2].y;
        _local2++;
      L8143:
        if (_local2 < 3) goto L7985;
      L8150:
        this.moneyPreview.text = this._global.addCommas(this._global._maxCrypto.STI(TSavegame.currentPlayer.money));
    }

    function touchQuad(arg1:TouchEvent):void {
        var _local4 = 0;
        var _local5 = 0;
        if (!this._global.tutorial.button.visible) goto L28;
        return;
      L28:
        var _local2 = arg1.currentTarget as cQuad;
        var _local3 = arg1.getTouch(this._global._game, "hover");
        if (!_local2) goto L274;
        if (!_local3) goto L216;
        if (_local2.hover) goto L212;
        if (_local2.name != "" + PLAY_ID) goto L103;
        this.playGlow.alpha = 1;
        goto L207;
      L103:
        if (_local2.name != "" + SHOP_ID) goto L130;
        this.shopGlow.alpha = 1;
        goto L207;
      L130:
        if (_local2.name != "" + RECIPE_ID) goto L157;
        this.recipeGlow.alpha = 1;
        goto L207;
      L157:
        if (_local2.name != "" + ACHIEVEMENT_ID) goto L184;
        this.achievementGlow.alpha = 1;
        goto L207;
      L184:
        if (_local2.name != "" + HARDWARE_ID) goto L207;
        this.hardwareGlow.alpha = 1;
      L207:
        _local2.hover = true;
      L212:
        goto L274;
      L216:
        if (!_local2.hover) goto L274;
        this.playGlow.alpha = 0;
        this.shopGlow.alpha = 0;
        this.recipeGlow.alpha = 0;
        this.achievementGlow.alpha = 0;
        this.hardwareGlow.alpha = 0;
        _local2.hover = false;
      L274:
        if (!arg1.getTouch(this._global._game, TouchPhase.BEGAN)) goto L1234;
        if (this._bolehKlik) goto L305;
        return;
      L305:
        _local4 = int((arg1.currentTarget as Quad).name);
        if (this._global.tutorial.canClickThisButton(_local4)) goto L344;
        return;
      L344:
        if (_local4 != PLAY_ID) goto L391;
        this._global._game.fadeToScreen(this._global._levelSelectScreen);
        if (this._global._onFade) goto L387;
        this._bolehKlik = false;
      L387:
        goto L1234;
      L391:
        if (_local4 != SHOP_ID) goto L446;
        this.resetItem();
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_ingredients", true));
        this.showShop(this.INGREDIENT_SHOP);
        goto L1234;
      L446:
        if (_local4 != HARDWARE_ID) goto L501;
        this.resetItem();
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_hardware", true));
        this.showShop(this.HARDWARE_SHOP);
        goto L1234;
      L501:
        if (_local4 != ACHIEVEMENT_ID) goto L559;
        this._global._achievementScreen.backToMainMenu = false;
        this._global._game.fadeToScreen(this._global._achievementScreen);
        if (this._global._onFade) goto L555;
        this._bolehKlik = false;
      L555:
        goto L1234;
      L559:
        if (_local4 != RECIPE_ID) goto L614;
        this.resetItem();
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_recipe_shop", true));
        this.showShop(this.RECIPE_SHOP);
        goto L1234;
      L614:
        if (_local4 != BACK_BUTTON) goto L934;
        if (this.itemIsTweening) goto L930;
        if (!this.shopBackground.visible) goto L896;
        this.westernButton.visible = false;
        this.chineseButton.visible = false;
        this.midEastButton.visible = false;
        this.shopBackground.visible = false;
        this.shopName.visible = false;
        this.itemName.visible = false;
        this.upgradeCost.visible = false;
        this.upgradeInfo.visible = false;
        this.buyButton.visible = false;
        this.moneyPlaceBG.visible = false;
        this.coinPreview.visible = false;
        this.moneyPreview.visible = false;
        this.leftButton.visible = false;
        this.rightButton.visible = false;
        this.shopCoin.visible = false;
        this.backBtn.x = 20;
        _local5 = 0;
        goto L834;
      L779:
        this.plateAr[_local5].visible = false;
        this.itemAr[_local5].visible = false;
        this.ingredientCountAr[_local5].visible = false;
        this.ingredientPreviewAr[_local5].visible = false;
        _local5++;
      L834:
        if (_local5 < 5) goto L779;
        _local5 = 0;
        goto L884;
      L850:
        this.starAr[_local5].visible = false;
        this.starAr[_local5].x = 4294967146;
        _local5++;
      L884:
        if (_local5 < 10) goto L850;
        goto L930;
      L896:
        this._global._game.fadeToScreen(this._global._mainMenuScreen);
        if (this._global._onFade) goto L930;
        this._bolehKlik = false;
      L930:
        goto L1234;
      L934:
        if (_local4 != WESTERN_BUTTON) goto L978;
        if (!this.itemIsTweening) goto L952;
        return;
      L952:
        this.currentRecipeRegion = this.WESTERN;
        this.resetItem();
        this.showShop(this.RECIPE_SHOP);
        goto L1234;
      L978:
        if (_local4 != CHINESE_BUTTON) goto L1022;
        if (!this.itemIsTweening) goto L996;
        return;
      L996:
        this.currentRecipeRegion = this.CHINESE;
        this.resetItem();
        this.showShop(this.RECIPE_SHOP);
        goto L1234;
      L1022:
        if (_local4 != MID_EAST_BUTTON) goto L1066;
        if (!this.itemIsTweening) goto L1040;
        return;
      L1040:
        this.currentRecipeRegion = this.MID_EAST;
        this.resetItem();
        this.showShop(this.RECIPE_SHOP);
        goto L1234;
      L1066:
        if (_local4 != BUY_BUTTON) goto L1084;
        this.buyItem();
        goto L1234;
      L1084:
        if (_local4 != LEFT_BUTTON) goto L1112;
        if (!this.itemIsTweening) goto L1102;
        return;
      L1102:
        this.tweenItem(false);
        goto L1234;
      L1112:
        if (_local4 != RIGHT_BUTTON) goto L1209;
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "right_button", true));
        var $t0 = this._global.tutorial.currentDialogueID == Tutorial.AFTER_LEVEL_2;
        $s0 = $t0;
        if (!$t0) goto L1185;
        $s0 = this._global.tutorial.dialogueCounter == 2;
      L1185:
        if (!$s0) goto L1190;
        return;
      L1190:
        if (!this.itemIsTweening) goto L1199;
        return;
      L1199:
        this.tweenItem(true);
        goto L1234;
      L1209:
        if (_local4 != FREE_COINS_BUTTON) goto L1234;
        TGlobalClass._windowDisplay.showWindow(TWindowState.SHOW_FREE_COINS);
      L1234:
    }

    function buyItem():void {
        cost = 0;
        if (this.buyButton._text.text != "Info") goto L46;
        TGlobalClass._windowDisplay.showWindow(TWindowState.SHOW_EXPERT_NEEDED_INFO);
        return;
      L46:
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "buy_item", true));
        this._global._soundAsset.playSFX("money_collected");
        if (this.currentShop != this.INGREDIENT_SHOP) goto L491;
        GALib.TrackEvent(GALib.BUY_OR_UPGRADE_RECIPE, TSavegame.currentPlayer.ingredientLevel[this.currentFocus], this.ingredientName[this.currentFocus]);
        cost = int(TGlobalClass.ingredientCost[this.currentFocus][TSavegame.currentPlayer.ingredientLevel[this.currentFocus]]);
        var $t0 = TSavegame.currentPlayer.ingredientLevel;
        var _local2 = $t0;
        var $t1 = this.currentFocus;
        var _local3 = $t1;
        var _local4 = $t0[$t1] + 1;
        _local2[_local3] = _local4;
        TSavegame.currentPlayer.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(TSavegame.currentPlayer.money) - cost);
        if (TSavegame.currentPlayer.unlocked[Achievement.SECRET_INGREDIENT]) goto L478;
        if (!(TSavegame.currentPlayer.ingredientLevel[this.currentFocus] > TSavegame.currentPlayer.achievementCounter[Achievement.SECRET_INGREDIENT])) goto L352;
        TSavegame.currentPlayer.achievementCounter[Achievement.SECRET_INGREDIENT] = TSavegame.currentPlayer.ingredientLevel[this.currentFocus];
      L352:
        if (TSavegame.currentPlayer.achievementCounter[Achievement.SECRET_INGREDIENT] != Achievement.ALL_TARGET[Achievement.SECRET_INGREDIENT] / 2) goto L409;
        Achievement.tweenAchievementProgress(Achievement.SECRET_INGREDIENT);
        goto L478;
      L409:
        if (!(TSavegame.currentPlayer.achievementCounter[Achievement.SECRET_INGREDIENT] >= Achievement.ALL_TARGET[Achievement.SECRET_INGREDIENT])) goto L478;
        TSavegame.currentPlayer.unlocked[Achievement.SECRET_INGREDIENT] = true;
        Achievement.tweenAchievementNotif(Achievement.SECRET_INGREDIENT);
      L478:
        this.showShop(this.INGREDIENT_SHOP);
        goto L1749;
      L491:
        if (this.currentShop != this.RECIPE_SHOP) goto L1568;
        if (this.currentRecipeRegion != this.WESTERN) goto L840;
        GALib.TrackEvent(GALib.BUY_OR_UPGRADE_RECIPE, TSavegame.currentPlayer.westernDishLevel[this.currentFocus], this.westernDishName[this.currentFocus]);
        cost = int(TGlobalClass.westernUpgradeCost[this.currentFocus][TSavegame.currentPlayer.westernDishLevel[this.currentFocus]]);
        var $t2 = TSavegame.currentPlayer.westernDishLevel;
        _local2 = $t2;
        var $t3 = this.currentFocus;
        _local3 = $t3;
        _local4 = $t2[$t3] + 1;
        _local2[_local3] = _local4;
        if (TSavegame.currentPlayer.westernDishLevel[this.currentFocus] != 1) goto L836;
        if (TSavegame.currentPlayer.unlocked[Achievement.WESTERN_RECIPE]) goto L836;
        var $t4 = TSavegame.currentPlayer.achievementCounter;
        _local2 = $t4;
        var $t5 = Achievement.WESTERN_RECIPE;
        _local3 = $t5;
        _local4 = $t4[$t5] + 1;
        _local2[_local3] = _local4;
        if (TSavegame.currentPlayer.achievementCounter[Achievement.WESTERN_RECIPE] != Achievement.ALL_TARGET[Achievement.WESTERN_RECIPE] / 2) goto L767;
        Achievement.tweenAchievementProgress(Achievement.WESTERN_RECIPE);
        goto L836;
      L767:
        if (!(TSavegame.currentPlayer.achievementCounter[Achievement.WESTERN_RECIPE] >= Achievement.ALL_TARGET[Achievement.WESTERN_RECIPE])) goto L836;
        TSavegame.currentPlayer.unlocked[Achievement.WESTERN_RECIPE] = true;
        Achievement.tweenAchievementNotif(Achievement.WESTERN_RECIPE);
      L836:
        goto L1510;
      L840:
        if (this.currentRecipeRegion != this.CHINESE) goto L1177;
        GALib.TrackEvent(GALib.BUY_OR_UPGRADE_RECIPE, TSavegame.currentPlayer.chineseDishLevel[this.currentFocus], this.chineseDishName[this.currentFocus]);
        cost = int(TGlobalClass.chineseUpgradeCost[this.currentFocus][TSavegame.currentPlayer.chineseDishLevel[this.currentFocus]]);
        var $t6 = TSavegame.currentPlayer.chineseDishLevel;
        _local2 = $t6;
        var $t7 = this.currentFocus;
        _local3 = $t7;
        _local4 = $t6[$t7] + 1;
        _local2[_local3] = _local4;
        if (TSavegame.currentPlayer.chineseDishLevel[this.currentFocus] != 1) goto L1173;
        if (TSavegame.currentPlayer.unlocked[Achievement.ASIAN_RECIPE]) goto L1173;
        var $t8 = TSavegame.currentPlayer.achievementCounter;
        _local2 = $t8;
        var $t9 = Achievement.ASIAN_RECIPE;
        _local3 = $t9;
        _local4 = $t8[$t9] + 1;
        _local2[_local3] = _local4;
        if (TSavegame.currentPlayer.achievementCounter[Achievement.ASIAN_RECIPE] != Achievement.ALL_TARGET[Achievement.ASIAN_RECIPE] / 2) goto L1104;
        Achievement.tweenAchievementProgress(Achievement.ASIAN_RECIPE);
        goto L1173;
      L1104:
        if (!(TSavegame.currentPlayer.achievementCounter[Achievement.ASIAN_RECIPE] >= Achievement.ALL_TARGET[Achievement.ASIAN_RECIPE])) goto L1173;
        TSavegame.currentPlayer.unlocked[Achievement.ASIAN_RECIPE] = true;
        Achievement.tweenAchievementNotif(Achievement.ASIAN_RECIPE);
      L1173:
        goto L1510;
      L1177:
        if (this.currentRecipeRegion != this.MID_EAST) goto L1510;
        GALib.TrackEvent(GALib.BUY_OR_UPGRADE_RECIPE, TSavegame.currentPlayer.mideastDishLevel[this.currentFocus], this.mideastDishName[this.currentFocus]);
        cost = int(TGlobalClass.mideastUpgradeCost[this.currentFocus][TSavegame.currentPlayer.mideastDishLevel[this.currentFocus]]);
        var $t10 = TSavegame.currentPlayer.mideastDishLevel;
        _local2 = $t10;
        var $t11 = this.currentFocus;
        _local3 = $t11;
        _local4 = $t10[$t11] + 1;
        _local2[_local3] = _local4;
        if (TSavegame.currentPlayer.mideastDishLevel[this.currentFocus] != 1) goto L1510;
        if (TSavegame.currentPlayer.unlocked[Achievement.MIDDLE_EAST_RECIPE]) goto L1510;
        var $t12 = TSavegame.currentPlayer.achievementCounter;
        _local2 = $t12;
        var $t13 = Achievement.MIDDLE_EAST_RECIPE;
        _local3 = $t13;
        _local4 = $t12[$t13] + 1;
        _local2[_local3] = _local4;
        if (TSavegame.currentPlayer.achievementCounter[Achievement.MIDDLE_EAST_RECIPE] != Achievement.ALL_TARGET[Achievement.MIDDLE_EAST_RECIPE] / 2) goto L1441;
        Achievement.tweenAchievementProgress(Achievement.MIDDLE_EAST_RECIPE);
        goto L1510;
      L1441:
        if (!(TSavegame.currentPlayer.achievementCounter[Achievement.MIDDLE_EAST_RECIPE] >= Achievement.ALL_TARGET[Achievement.MIDDLE_EAST_RECIPE])) goto L1510;
        TSavegame.currentPlayer.unlocked[Achievement.MIDDLE_EAST_RECIPE] = true;
        Achievement.tweenAchievementNotif(Achievement.MIDDLE_EAST_RECIPE);
      L1510:
        TSavegame.currentPlayer.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(TSavegame.currentPlayer.money) - cost);
        this.showShop(this.RECIPE_SHOP);
        goto L1749;
      L1568:
        if (this.currentShop != this.HARDWARE_SHOP) goto L1749;
        GALib.TrackEvent(GALib.BUY_OR_UPGRADE_RECIPE, TSavegame.currentPlayer.hardwareLevel[this.currentFocus], this.hardwareName[this.currentFocus]);
        cost = int(this._global.dish.getHardwareUpgradecost(this.currentFocus)[TSavegame.currentPlayer.hardwareLevel[this.currentFocus]]);
        var $t14 = TSavegame.currentPlayer.hardwareLevel;
        _local2 = $t14;
        var $t15 = this.currentFocus;
        _local3 = $t15;
        _local4 = $t14[$t15] + 1;
        _local2[_local3] = _local4;
        TSavegame.currentPlayer.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(TSavegame.currentPlayer.money) - cost);
        this.showShop(this.HARDWARE_SHOP);
      L1749:
        this.moneyPreview.text = this._global.addCommas(this._global._maxCrypto.STI(TSavegame.currentPlayer.money));
        this.moneyPreviewMinus.text = "-" + cost;
        this.moneyPreviewMinus.visible = true;
        TweenLite.killTweensOf(this.moneyPreviewMinus);
        TweenLite.killDelayedCallsTo(this.hideMoneyMinusText);
        this.moneyPreviewMinus.y = this.moneyPlaceBG.y;
        TweenLite.to(this.moneyPreviewMinus, 0.5, {"y": this.moneyPreviewMinus.y + 50, "onComplete": function():void {
            TweenLite.delayedCall(0.5, hideMoneyMinusText);
        }});
        TSavegame.save();
    }

    function hideMoneyMinusText():void {
        this.moneyPreviewMinus.visible = false;
        this.moneyPreviewMinus.y = this.moneyPreviewMinus.y - 50;
    }

    function touchButton(arg1:TouchEvent):void {
        var _local2 = 0;
        var _local3 = 0;
        if (!this._global.tutorial.button.visible) goto L26;
        return;
      L26:
        if (!arg1.getTouch(this._global._game, TouchPhase.HOVER)) goto L166;
        _local2 = int((arg1.currentTarget as cSprite).idx);
        if (_local2 != PLAY_ID) goto L82;
        this.playGlow.alpha = 1;
        goto L162;
      L82:
        if (_local2 != SHOP_ID) goto L103;
        this.shopGlow.alpha = 1;
        goto L162;
      L103:
        if (_local2 != ACHIEVEMENT_ID) goto L124;
        this.achievementGlow.alpha = 1;
        goto L162;
      L124:
        if (_local2 != RECIPE_ID) goto L145;
        this.recipeGlow.alpha = 1;
        goto L162;
      L145:
        if (_local2 != HARDWARE_ID) goto L162;
        this.hardwareGlow.alpha = 1;
      L162:
        goto L211;
      L166:
        this.playGlow.alpha = 0;
        this.achievementGlow.alpha = 0;
        this.hardwareGlow.alpha = 0;
        this.recipeGlow.alpha = 0;
        this.shopGlow.alpha = 0;
      L211:
        if (!arg1.getTouch(this._global._game, TouchPhase.BEGAN)) goto L494;
        _local3 = int((arg1.currentTarget as cSprite).idx);
        if (this._bolehKlik) goto L255;
        return;
      L255:
        if (this._global.tutorial.canClickThisButton(_local3)) goto L272;
        return;
      L272:
        if (_local3 != PLAY_ID) goto L318;
        this._global._game.fadeToScreen(this._global._levelSelectScreen);
        if (this._global._onFade) goto L314;
        this._bolehKlik = false;
      L314:
        goto L494;
      L318:
        if (_local3 != SHOP_ID) goto L372;
        this.resetItem();
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_ingredients", true));
        this.showShop(this.INGREDIENT_SHOP);
        goto L494;
      L372:
        if (_local3 != ACHIEVEMENT_ID) goto L418;
        this._global._game.fadeToScreen(this._global._achievementScreen);
        if (this._global._onFade) goto L414;
        this._bolehKlik = false;
      L414:
        goto L494;
      L418:
        if (_local3 != RECIPE_ID) goto L472;
        this.resetItem();
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_recipe_shop", true));
        this.showShop(this.RECIPE_SHOP);
        goto L494;
      L472:
        if (_local3 != HARDWARE_ID) goto L494;
        this.resetItem();
        this.showShop(this.HARDWARE_SHOP);
      L494:
    }

    function resetItem():void {
        this.startPointLooping = 0;
        this.currentFocus = 0;
        var _local1 = 0;
        goto L150;
      L21:
        this.itemAr[_local1].itemIdx = _local1;
        this.shopItemContainer[_local1].x = 512 + _local1 * 550;
        if (_local1 != this.currentFocus % 4) goto L109;
        this.shopItemContainer[_local1].scaleX = 1;
        this.shopItemContainer[_local1].scaleY = 1;
        this.shopItemContainer[_local1].alpha = 1;
        goto L148;
      L109:
        this.shopItemContainer[_local1].scaleX = 0.7;
        this.shopItemContainer[_local1].scaleY = 0.7;
        this.shopItemContainer[_local1].alpha = 0.7;
      L148:
        _local1++;
      L150:
        if (_local1 < 4) goto L21;
    }
}
}
