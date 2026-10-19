// Ported from Kitchen.as (decompiled).
using System;
using System.Collections.Generic;
using Starlite;

public class Kitchen : Screen
{
    public const int PLAY_ID = 1;
    public const int SHOP_ID = 2;
    public const int ACHIEVEMENT_ID = 3;
    public const int RECIPE_ID = 4;
    public const int BACK_BUTTON = 5;
    public const int WESTERN_BUTTON = 6;
    public const int CHINESE_BUTTON = 7;
    public const int BUY_BUTTON = 8;
    public const int LEFT_BUTTON = 9;
    public const int RIGHT_BUTTON = 10;
    public const int HARDWARE_ID = 11;
    public const int MID_EAST_BUTTON = 12;
    public const int FREE_COINS_BUTTON = 13;
    // AS3 instance consts (accessed through instances): readonly fields.
    public readonly int INGREDIENT_SHOP = 1;
    public readonly int RECIPE_SHOP = 2;
    public readonly int HARDWARE_SHOP = 3;
    public readonly int WESTERN = 1;
    public readonly int CHINESE = 2;
    public readonly int MID_EAST = 3;

    public Global _global;
    public bool initVector;
    public TextureAtlas kitchenAtlas;
    public cSprite _background;
    public cSprite shopBackground;
    public cSprite shopCoin;
    public cSprite coinPreview;
    public cSprite playBubble;
    public cSprite achievementBubble;
    public cSprite recipeBubble;
    public cSprite shopBubble;
    public cSprite hardwareBubble;
    public cQuad playQuad;
    public cQuad achievementQuad;
    public cQuad recipeQuad;
    public cQuad shopQuad;
    public cQuad hardwareQuad;
    public cSprite playGlow;
    public cSprite achievementGlow;
    public cSprite recipeGlow;
    public cSprite shopGlow;
    public cSprite hardwareGlow;
    public TNonStateButton backBtn;
    public TNonStateButton FreeCoinsBtn;
    public List<cSprite> buttonAr;
    public List<DisplayObject> quadButtonAr;
    public List<string> hardwareName;
    public List<string> ingredientName;
    public List<string> westernDishName;
    public List<string> chineseDishName;
    public List<string> mideastDishName;
    public ExtendTextField shopName;
    public TNonStateButton upgradeButton;
    public TNonStateButton westernButton;
    public TNonStateButton chineseButton;
    public TNonStateButton midEastButton;
    public TNonStateButton buyButton;
    public TNonStateButton leftButton;
    public TNonStateButton rightButton;
    public cSprite moneyPlaceBG;
    public ExtendTextField moneyPreview;
    public ExtendTextField moneyPreviewMinus;
    public cSprite ingredientPreview;
    public List<cSprite> ingredientPreviewAr;
    public ExtendTextField ingredientCount;
    public List<ExtendTextField> ingredientCountAr;
    public List<Sprite> shopItemContainer;
    public List<cSprite> plateAr;
    public List<cSprite> itemAr;
    public ExtendTextField itemName;
    public List<cSprite> starAr;
    public ExtendTextField upgradeInfo;
    public ExtendTextField upgradeCost;
    public int currentRecipeRegion;
    public int currentFocus;
    public int currentShop;
    public int startPointLooping;
    public TimelineLite timeline;
    public float startTouchX = 0;
    public bool itemIsTweening;
    public bool touchedBG;
    public bool _bolehKlik;
    public List<float> posYDif_Chinese;
    public List<float> posXDif_Chinese;
    public List<float> posYDif_Western;
    public List<float> posXDif_Western;
    public List<float> posXDif_MidEast;
    public List<float> posYDif_MidEast;

    public Kitchen()
    {
        this.posYDif_Chinese = new List<float> { 67.5f, 72.5f, 81.5f, 78.5f, 52.5f, 77.5f };
        this.posXDif_Chinese = new List<float> { 130, 141, 137, 127, 152, 131.5f };
        this.posYDif_Western = new List<float> { 60.5f, 84.5f, 84.5f, 68.5f, 75, 70 };
        this.posXDif_Western = new List<float> { 133.5f, 134.5f, 135, 129.5f, 138.5f, 138 };
        this.posXDif_MidEast = new List<float> { 134, 131.5f, 133, 128, 128.5f, 131 };
        this.posYDif_MidEast = new List<float> { 81, 75.5f, 71.5f, 84.5f, 63.5f, 47.5f };
    }

    public override void Start()
    {
        base.Start();
        this._global.currentSceneID = this._global.KITCHEN_ID;
        if (!this.initVector)
        {
            this.buttonAr = new List<cSprite>();
            this.quadButtonAr = new List<DisplayObject>();
            this.plateAr = new List<cSprite>();
            this.starAr = new List<cSprite>();
            this.itemAr = new List<cSprite>();
            this.shopItemContainer = new List<Sprite>();
            this.ingredientPreviewAr = new List<cSprite>();
            this.ingredientCountAr = new List<ExtendTextField>();
            this.hardwareName = new List<string> { "juicer", "fryer", "cookies" };
            this.ingredientName = new List<string> { "croissant", "selada", "beef", "chicken", "rice", "bacon", "salmon", "seafood", "cheese", "pita_bread", "lamb", "tomato" };
            this.westernDishName = new List<string> { "veggie_croissant", "deluxe_chicken_croissant", "french_BBQ_combo", "beef_steak", "smokey_salmon", "premium_salmon_steak" };
            this.chineseDishName = new List<string> { "asian_fried_rice", "thai_fried_rice", "kalasan_fried_chicken", "chinese_pork_special", "fine_imperial_cuisine", "deluxe_BBQ_combo" };
            this.mideastDishName = new List<string> { "doner_kebab", "gyro_style", "tomato_domas", "tuscan_humus", "baklava", "crown_lamb_roast" };
            this.initVector = true;
        }
        this._global.assetManager.enqueue(this._global.graphicsPath + "kitchen.ean", this._global.graphicsPath + "kitchen.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + "kitchen_BG.eag", this._global.graphicsPath + "shop_table_BG.eag");
        this._global.assetManager.loadQueue((float f1_arg1) =>
        {
            if (f1_arg1 == 1) this.initialize();
        });
    }

    public override void Update()
    {
        if (!ready) return;
    }

    public override void Kill()
    {
        int i;
        int n;
        if (this.timeline != null)
        {
            this.timeline.kill();
            this.timeline = null;
        }
        n = this.buttonAr.Count;
        for (i = 0; i < n; i++)
        {
            TweenLite.killTweensOf(this.buttonAr[i]);
            this.buttonAr[i].removeEventListener(TouchEvent.TOUCH, this.touchButton);
            this.buttonAr[i].removeFromParent(true);
            this.buttonAr[i] = null;
        }
        this.buttonAr.Clear();
        n = this.quadButtonAr.Count;
        for (i = 0; i < n; i++)
        {
            this.quadButtonAr[i].removeEventListener(TouchEvent.TOUCH, this.touchQuad);
            this.quadButtonAr[i].removeFromParent(true);
            this.quadButtonAr[i] = null;
        }
        this.quadButtonAr.Clear();
        n = this.shopItemContainer.Count;
        for (i = 0; i < n; i++)
        {
            this.shopItemContainer[i].removeFromParent(true);
            this.shopItemContainer[i] = null;
        }
        this.shopItemContainer.Clear();
        for (i = 0; i < 10; i++)
        {
            this.starAr[i].removeFromParent(true);
            this.starAr[i] = null;
        }
        this.starAr.Clear();
        for (i = 0; i < 5; i++)
        {
            this.itemAr[i].removeFromParent(true);
            this.itemAr[i] = null;
            this.plateAr[i].removeFromParent(true);
            this.plateAr[i] = null;
            this.ingredientPreviewAr[i].removeFromParent(true);
            this.ingredientPreviewAr[i] = null;
            this.ingredientCountAr[i].removeFromParent(true);
            this.ingredientCountAr[i] = null;
        }
        this.itemAr.Clear();
        this.plateAr.Clear();
        this.ingredientPreviewAr.Clear();
        this.ingredientCountAr.Clear();
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

    public void initialize()
    {
        int i = 0;
        cSprite plate = null;
        cSprite item = null;
        Sprite container = null;
        cSprite star = null;
        if (TGlobalClass.musicPlayingIdx != 1)
        {
            this._global._soundAsset.playMusic("feeling-positive");
            TGlobalClass.musicPlayingIdx = 1;
        }
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
        this.playGlow.x = 523.5f;
        this.playGlow.y = 477.5f;
        this.playGlow.touchable = false;
        this._global._gameS.addChild(this.playGlow);
        this.playGlow.alpha = 0;
        this.shopGlow = new cSprite(this.kitchenAtlas.getTexture("shopGlow"));
        this.shopGlow.alignPivot();
        this.shopGlow.x = 698.5f;
        this.shopGlow.y = 436;
        this.shopGlow.touchable = false;
        this._global._gameS.addChild(this.shopGlow);
        this.shopGlow.alpha = 0;
        this.achievementGlow = new cSprite(this.kitchenAtlas.getTexture("achievementGlow"));
        this.achievementGlow.alignPivot();
        this.achievementGlow.x = 810.5f;
        this.achievementGlow.y = 246.5f;
        this.achievementGlow.touchable = false;
        this._global._gameS.addChild(this.achievementGlow);
        this.achievementGlow.alpha = 0;
        this.recipeGlow = new cSprite(this.kitchenAtlas.getTexture("recipeGlow"));
        this.recipeGlow.alignPivot();
        this.recipeGlow.x = 304.5f;
        this.recipeGlow.y = 241.5f;
        this.recipeGlow.touchable = false;
        this._global._gameS.addChild(this.recipeGlow);
        this.recipeGlow.alpha = 0;
        this.hardwareGlow = new cSprite(this.kitchenAtlas.getTexture("hardwareGlow"));
        this.hardwareGlow.alignPivot();
        this.hardwareGlow.x = 164;
        this.hardwareGlow.y = 371.5f;
        this.hardwareGlow.touchable = false;
        this._global._gameS.addChild(this.hardwareGlow);
        this.hardwareGlow.alpha = 0;
        if (TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_RECIPE_SHOP)
        {
            this.recipeBubble.visible = true;
            this.recipeQuad.visible = true;
        }
        if (TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_INGREDIENT_SHOP)
        {
            this.shopBubble.visible = true;
            this.shopQuad.visible = true;
        }
        if (TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_HARDWARE_SHOP)
        {
            this.hardwareBubble.visible = true;
            this.hardwareQuad.visible = true;
        }
        this.FreeCoinsBtn = new TNonStateButton(this.kitchenAtlas.getTexture("small_btn_blue"), "Free Coins", "bromine", 25, 16776960);
        this.FreeCoinsBtn.alignPivot();
        this.FreeCoinsBtn.x = 60 + this.FreeCoinsBtn.pivotX;
        this.FreeCoinsBtn.y = 20 + this.FreeCoinsBtn.pivotY;
        this.FreeCoinsBtn._image.name = "" + FREE_COINS_BUTTON;
        this.FreeCoinsBtn._text.touchable = false;
        this._global._gameS.addChild(this.FreeCoinsBtn);
        this.quadButtonAr.push(this.FreeCoinsBtn._image);
        this.FreeCoinsBtn.visible = TGlobalClass.EnableFreeCoins;
        if (TGlobalClass.EnableFreeCoins)
        {
            this.timeline = new TimelineLite();
            this.timeline.append(new TweenLite(this.FreeCoinsBtn, 1, new Vars { ["scaleX"] = 1.1f, ["scaleY"] = 1.1f, ["ease"] = Expo.easeOut }));
            this.timeline.append(new TweenLite(this.FreeCoinsBtn, 0.5f, new Vars
            {
                ["scaleX"] = 1, ["scaleY"] = 1,
                ["onComplete"] = (Action)(() => { this.timeline.restart(); })
            }));
        }
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
        this.shopCoin.scaleY = 0.5f;
        this.shopCoin.scaleX = 0.5f;
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
        this.westernButton.scaleY = 0.7f;
        this.westernButton.scaleX = 0.7f;
        this.westernButton.x = 700;
        this.westernButton.y = 70;
        this.westernButton._image.name = "" + WESTERN_BUTTON;
        this.westernButton._text.touchable = false;
        this.westernButton.visible = false;
        this._global._gameS.addChild(this.westernButton);
        this.quadButtonAr.push(this.westernButton._image);
        this.chineseButton = new TNonStateButton(this.kitchenAtlas.getTexture("small_btn_blue"), "Asian", "bromine", 25, 0);
        this.chineseButton.scaleY = 0.7f;
        this.chineseButton.scaleX = 0.7f;
        this.chineseButton.x = 860;
        this.chineseButton.y = 70;
        this.chineseButton.visible = false;
        this.chineseButton._text.touchable = false;
        this.chineseButton._image.name = "" + CHINESE_BUTTON;
        this._global._gameS.addChild(this.chineseButton);
        this.quadButtonAr.push(this.chineseButton._image);
        this.midEastButton = new TNonStateButton(this.kitchenAtlas.getTexture("small_btn_blue"), "Middle East", "bromine", 25, 0);
        this.midEastButton.scaleY = 0.7f;
        this.midEastButton.scaleX = 0.7f;
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
        this.moneyPreview = new ExtendTextField((int)(this.moneyPlaceBG.width - 20), (int)this.moneyPlaceBG.height, "0", "bromine", 25);
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
        this.coinPreview.scaleY = 0.7f;
        this.coinPreview.scaleX = 0.7f;
        this._global._gameS.addChild(this.coinPreview);
        this.moneyPreviewMinus = new ExtendTextField((int)(this.moneyPlaceBG.width - 20), (int)this.moneyPlaceBG.height, "0", "bromine", 25, 16711680);
        this.moneyPreviewMinus.x = this.moneyPlaceBG.x + 5;
        this.moneyPreviewMinus.y = this.moneyPlaceBG.y;
        this.moneyPreviewMinus.hAlign = "right";
        this.moneyPreviewMinus.visible = false;
        this.moneyPreviewMinus.touchable = false;
        this._global._gameS.addChild(this.moneyPreviewMinus);
        for (i = 0; i < 5; i++)
        {
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
        }
        for (i = 0; i < 10; i++)
        {
            star = new cSprite(this.kitchenAtlas.getTexture("star_empty"));
            star.pivotX = star.width / 2;
            star.pivotY = star.height / 2;
            star.x = -150;
            star.touchable = false;
            star.visible = false;
            this.starAr.push(star);
            this._global._gameS.addChild(star);
        }
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
        for (i = 0; i < this.buttonAr.Count; i++)
        {
            this.buttonAr[i].useHandCursor = true;
            this.buttonAr[i].addEventListener(TouchEvent.TOUCH, this.touchButton);
            this.tweenBubbleText(i);
        }
        for (i = 0; i < this.quadButtonAr.Count; i++)
        {
            this.quadButtonAr[i].useHandCursor = true;
            this.quadButtonAr[i].addEventListener(TouchEvent.TOUCH, this.touchQuad);
        }
        if (TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_CHINESE_DISH_AT && TSavegame.currentPlayer.chineseDishLevel[0] == 0)
        {
            TSavegame.currentPlayer.chineseDishLevel[0] = 1;
            TSavegame.currentPlayer.chineseDishLevel[1] = 1;
            TSavegame.currentPlayer.achievementCounter[Achievement.ASIAN_RECIPE] = TSavegame.currentPlayer.achievementCounter[Achievement.ASIAN_RECIPE] + 2;
            TSavegame.save();
        }
        if (TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_MID_EAST_DISH_AT && TSavegame.currentPlayer.mideastDishLevel[0] == 0)
        {
            TSavegame.currentPlayer.mideastDishLevel[0] = 1;
            TSavegame.currentPlayer.mideastDishLevel[1] = 1;
            TSavegame.currentPlayer.achievementCounter[Achievement.MIDDLE_EAST_RECIPE] = TSavegame.currentPlayer.achievementCounter[Achievement.MIDDLE_EAST_RECIPE] + 2;
            TSavegame.save();
        }
        if (TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_BEVERAGE_AT && TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] == 0)
        {
            TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] = 1;
            TSavegame.save();
        }
        if (TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_FRIES_AT && TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] == 0)
        {
            TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] = 1;
            TSavegame.save();
        }
        TGlobalClass._windowDisplay.initializeWindow(this._global.currentSceneID);
        TGlobalClass.currentNotifWindow = 0;
        if (TSavegame.currentPlayer.passedDays < 60
            && TGlobalClass.levelEventXML["day"][TSavegame.currentPlayer.passedDays]["kitchen"].length() > 0
            && TSavegame.currentPlayer.lastDayKitchen == TSavegame.currentPlayer.passedDays - 1)
        {
            TGlobalClass._windowDisplay.showKitchenWindow(TSavegame.currentPlayer.passedDays, TGlobalClass.currentNotifWindow);
        }
        this._global.tutorial.checkTut(Tutorial.OPENING_STORY, 0);
        this._global.tutorial.checkTut(Tutorial.AFTER_LEVEL_1, 1);
        this._global.tutorial.checkTut(Tutorial.AFTER_LEVEL_2, 2);
        this._global.tutorial.checkTut(Tutorial.AFTER_LEVEL_7, 7);
        this._global.tutorial.checkTut(Tutorial.AFTER_LEVEL_8, 8);
        this._global.tutorial.checkTut(Tutorial.AFTER_LEVEL_34, 34);
        if (this._global.tutorial.currentDialogueID == Tutorial.OPENING_STORY)
        {
            this.achievementBubble.visible = false;
            this.achievementQuad.visible = false;
        }
        TSavegame.currentPlayer.lastDayKitchen = TSavegame.currentPlayer.passedDays;
        TSavegame.save();
        this.currentRecipeRegion = this.WESTERN;
        this._bolehKlik = true;
        ready = true;
    }

    public void dialogueCondition(int arg1, int arg2)
    {
    }

    public void tweenBubbleText(int arg1)
    {
        int idx = arg1;
        TweenLite.to(this.buttonAr[idx], 0.2f, new Vars
        {
            ["y"] = this.buttonAr[idx].y - 25,
            ["delay"] = 0.5f,
            ["ease"] = Linear.easeNone,
            ["onComplete"] = (Action<int>)((int f1_arg1) =>
            {
                TweenLite.to(this.buttonAr[f1_arg1], 0.4f, new Vars
                {
                    ["y"] = this.buttonAr[f1_arg1].y + 25,
                    ["ease"] = Bounce.easeOut,
                    ["onComplete"] = (Action<int>)this.tweenBubbleText,
                    ["onCompleteParams"] = new object[] { f1_arg1 }
                });
            }),
            ["onCompleteParams"] = new object[] { idx }
        });
    }

    public void swipe(Event arg1)
    {
        var e = (TouchEvent)arg1;
        if (this._global.tutorial.button.visible) return;
        if (!this._global.tutorial.canClickThisButton(-1)) return;
        Touch touch = e.getTouch(this._global._game);
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) != null)
        {
            this.startTouchX = touch.globalX;
            if (!this.itemIsTweening) this.touchedBG = true;
        }
        if (e.getTouch(this._global._game, TouchPhase.MOVED) != null && this.touchedBG)
        {
            if (touch.globalX < this.startTouchX - 70)
                this.tweenItem(true);
            else if (touch.globalX > this.startTouchX + 70)
                this.tweenItem(false);
        }
    }

    public void tweenItem(bool arg1)
    {
        int i = 0;
        bool reverse = arg1;
        if (reverse)
        {
            if (this.currentShop == this.INGREDIENT_SHOP && this.currentFocus == this.ingredientName.Count - 1) return;
            if (this.currentShop == this.RECIPE_SHOP && this.currentFocus == this.westernDishName.Count - 1) return;
            if (this.currentShop == this.HARDWARE_SHOP && this.currentFocus == this.hardwareName.Count - 1) return;
        }
        else if (this.currentFocus == 0)
        {
            return;
        }
        this.touchedBG = false;
        this.itemIsTweening = true;
        this.currentFocus = this.currentFocus + (reverse == true ? 1 : -1);
        float _scaleX = 0;
        float _scaleY = 0;
        float alpha = 0;
        for (i = 0; i < 4; i++)
        {
            if (reverse)
            {
                if (this.shopItemContainer[i].x <= 512 - 2 * 550)
                {
                    if (this.currentShop == this.INGREDIENT_SHOP)
                    {
                        if (this.startPointLooping + 4 < this.ingredientName.Count)
                        {
                            this.startPointLooping = this.startPointLooping + 1;
                            this.shopItemContainer[i].x = 512 + 1100;
                            this.itemAr[i].itemIdx = this.startPointLooping + 3;
                        }
                    }
                    else if (this.currentShop == this.RECIPE_SHOP)
                    {
                        if (this.startPointLooping + 4 < this.chineseDishName.Count)
                        {
                            this.startPointLooping = this.startPointLooping + 1;
                            this.shopItemContainer[i].x = 512 + 1100;
                            this.itemAr[i].itemIdx = this.startPointLooping + 3;
                        }
                    }
                }
            }
            else if (this.shopItemContainer[i].x >= 512 + 2 * 550 && this.startPointLooping > 0)
            {
                this.shopItemContainer[i].x = 512 - 1100;
                this.startPointLooping = this.startPointLooping - 1;
                this.itemAr[i].itemIdx = this.startPointLooping;
            }
        }
        for (i = 0; i < 4; i++)
        {
            if (i == this.currentFocus % 4)
            {
                _scaleX = 1;
                _scaleY = 1;
                alpha = 1;
            }
            else
            {
                _scaleX = 0.7f;
                _scaleY = 0.7f;
                alpha = 0.7f;
            }
            TweenLite.to(this.shopItemContainer[i], 0.4f, new Vars
            {
                ["x"] = this.shopItemContainer[i].x + 550 * (reverse == true ? -1 : 1),
                ["scaleX"] = _scaleX,
                ["scaleY"] = _scaleY,
                ["alpha"] = alpha,
                ["ease"] = Expo.easeOut,
                ["onComplete"] = (Action)(() => { this.itemIsTweening = false; })
            });
        }
        this.showShop(this.currentShop);
    }

    /// <summary>"word word" -> "Word Word" (the decompiled split/charAt/toUpperCase/substr/join loop).</summary>
    static string capitalizeWords(string text)
    {
        List<string> words = text.split(" ");
        for (int k = 0; k < words.Count; k++)
            words[k] = words[k].charAt(0).toUpperCase() + words[k].substr(1);
        return words.join(" ");
    }

    public void showShop(int arg1)
    {
        int _local2 = 0;
        int _local3 = 0;
        int _local5 = 0;
        List<int> _local6 = null;
        List<int> _local7 = null;
        int _local10 = 0;
        List<int> _local11 = null;
        int _local13 = 0;
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
        if (arg1 == this.INGREDIENT_SHOP)
        {
            this.shopName.text = "MILLY'S INGREDIENT MARKET";
            for (_local2 = 0; _local2 < 5; _local2++)
            {
                this.itemAr[_local2].visible = true;
                this.plateAr[_local2].visible = true;
                this.itemAr[_local2].texture = this.kitchenAtlas.getTexture(this.ingredientName[this.itemAr[_local2].itemIdx]);
                this.itemAr[_local2].readjustSize();
                this.itemAr[_local2].pivotX = this.itemAr[_local2].width / 2;
                this.itemAr[_local2].pivotY = this.itemAr[_local2].height / 2;
                this.itemAr[_local2].x = this.plateAr[_local2].x;
                this.itemAr[_local2].y = this.plateAr[_local2].y;
            }
            this.itemName.text = this.ingredientName[this.currentFocus].replace("_", " ");
            if (this.itemName.text == "selada") this.itemName.text = "vegetables";
            this.itemName.text = capitalizeWords(this.itemName.text);
            if (this.currentFocus >= this.ingredientName.Count - 1)
                this.rightButton.visible = false;
            else if (this.currentFocus <= 0)
                this.leftButton.visible = false;
            List<int> ingredientLevel = TSavegame.currentPlayer.ingredientLevel;
            _local5 = TGlobalClass.ingredientCost[this.currentFocus].at(ingredientLevel[this.currentFocus]);
            if (ingredientLevel[this.currentFocus] > 0)
            {
                for (_local2 = 1; _local2 < TGlobalClass.MAX_INGREDIENT_LEVEL; _local2++)
                {
                    if (ingredientLevel[this.currentFocus] > 0 && _local2 < ingredientLevel[this.currentFocus])
                        this.starAr[_local2 - 1].texture = this.kitchenAtlas.getTexture("star");
                    else
                        this.starAr[_local2 - 1].texture = this.kitchenAtlas.getTexture("star_empty");
                    this.starAr[_local2 - 1].visible = true;
                    this.starAr[_local2 - 1].x = 512 - 4 * 60 + _local2 * 60 - 90;
                    this.starAr[_local2 - 1].y = 410;
                }
            }
            int totalDish = TGlobalClass.TOTAL_CHINESE_DISH + TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_MIDEAST_DISH;
            for (_local2 = 0; _local2 < totalDish; _local2++)
            {
                if (_local2 < TGlobalClass.TOTAL_WESTERN_DISH && TSavegame.currentPlayer.westernDishLevel[_local2] == 0)
                    continue;
                if (_local2 >= TGlobalClass.TOTAL_WESTERN_DISH && _local2 < TGlobalClass.TOTAL_CHINESE_DISH + TGlobalClass.TOTAL_WESTERN_DISH
                    && (TSavegame.currentPlayer.chineseDishLevel[_local2 - TGlobalClass.TOTAL_WESTERN_DISH] == 0 || TSavegame.currentPlayer.passedDays < 10))
                    continue;
                if (_local2 >= TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH
                    && (TSavegame.currentPlayer.mideastDishLevel[_local2 - TGlobalClass.TOTAL_WESTERN_DISH - TGlobalClass.TOTAL_CHINESE_DISH] == 0 || TSavegame.currentPlayer.passedDays < 20))
                    continue;
                _local11 = this._global.dish.getDishIngredientType(_local2);
                for (_local3 = 0; _local3 < _local11.Count; _local3++)
                {
                    if (this.currentFocus == _local11[_local3] - (this.currentFocus > 8 ? 2 : 1)) _local10++;
                }
            }
            if (_local10 > 0)
            {
                this.upgradeInfo.text = "Upgrading this will increase " + _local10 + (_local10 > 1 ? " dishes" : " dish") + " by $1";
                if (this._global._maxCrypto.STI(TSavegame.currentPlayer.money) < _local5 && ingredientLevel[this.currentFocus] < TGlobalClass.MAX_INGREDIENT_LEVEL)
                {
                    this.buyButton._text.text = "Can't Afford";
                    this.upgradeCost.text = "Cost : " + this._global.addCommas(_local5);
                    this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_red");
                    this.buyButton.touchable = false;
                }
                else if (ingredientLevel[this.currentFocus] == 0)
                {
                    this.buyButton._text.text = "Buy";
                    this.upgradeCost.text = "Cost : " + this._global.addCommas(_local5);
                    this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_green");
                    this.buyButton.touchable = true;
                }
                else if (ingredientLevel[this.currentFocus] > 0 && ingredientLevel[this.currentFocus] < TGlobalClass.MAX_INGREDIENT_LEVEL)
                {
                    this.buyButton._text.text = "Upgrade";
                    this.upgradeCost.text = "Cost : " + this._global.addCommas(_local5);
                    this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
                    this.buyButton.touchable = true;
                }
                else
                {
                    this.upgradeInfo.text = "You've maxed this ingredient quality.";
                    this.buyButton._text.text = "Maxed";
                    this.upgradeCost.visible = false;
                    this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_green");
                    this.buyButton.touchable = false;
                    this.shopCoin.visible = false;
                }
            }
            else if (_local10 <= 0)
            {
                this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_red");
                this.buyButton._text.text = "Sold Out!";
                this.upgradeCost.text = "Cost : " + this._global.addCommas(_local5);
                this.buyButton.touchable = false;
                this.upgradeInfo.text = "No recipe with this ingredient yet.";
            }
        }
        else if (arg1 == this.RECIPE_SHOP)
        {
            this.westernButton.visible = TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_CHINESE_DISH_AT;
            this.chineseButton.visible = TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_CHINESE_DISH_AT;
            this.midEastButton.visible = TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_MID_EAST_DISH_AT;
            this.shopName.text = "EMMA'S RECIPES";
            if (this.currentRecipeRegion == this.WESTERN)
            {
                this.westernButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue_dark");
                this.chineseButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
                this.midEastButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
                this.showRecipeRegion(this.westernDishName, TSavegame.currentPlayer.westernDishLevel, TGlobalClass.westernUpgradeCost, 0, 0, out _local6, out _local7);
            }
            else if (this.currentRecipeRegion == this.CHINESE)
            {
                this.chineseButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue_dark");
                this.westernButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
                this.midEastButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
                this.showRecipeRegion(this.chineseDishName, TSavegame.currentPlayer.chineseDishLevel, TGlobalClass.chineseUpgradeCost, TGlobalClass.TOTAL_WESTERN_DISH, 6, out _local6, out _local7);
            }
            else if (this.currentRecipeRegion == this.MID_EAST)
            {
                this.midEastButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue_dark");
                this.chineseButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
                this.westernButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
                this.showRecipeRegion(this.mideastDishName, TSavegame.currentPlayer.mideastDishLevel, TGlobalClass.mideastUpgradeCost, TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH, 12, out _local6, out _local7);
            }
            for (_local2 = 0; _local2 < 5; _local2++)
            {
                cSprite it = this.itemAr[_local2];
                it.visible = true;
                this.plateAr[_local2].visible = true;
                this.ingredientCountAr[_local2].visible = false;
                this.ingredientPreviewAr[_local2].visible = false;
                if (this.currentRecipeRegion == this.WESTERN)
                {
                    it.texture = this.kitchenAtlas.getTexture(this.westernDishName[it.itemIdx]);
                    it.readjustSize();
                    it.pivotX = it.width / 2;
                    it.pivotY = it.height / 2;
                    it.x = this.posXDif_Western[it.itemIdx];
                    it.y = this.posYDif_Western[it.itemIdx];
                }
                else if (this.currentRecipeRegion == this.CHINESE)
                {
                    it.texture = this.kitchenAtlas.getTexture(this.chineseDishName[it.itemIdx]);
                    it.readjustSize();
                    it.pivotX = it.width / 2;
                    it.pivotY = it.height / 2;
                    it.x = this.posXDif_Chinese[it.itemIdx];
                    it.y = this.posYDif_Chinese[it.itemIdx];
                }
                else if (this.currentRecipeRegion == this.MID_EAST)
                {
                    it.texture = this.kitchenAtlas.getTexture(this.mideastDishName[it.itemIdx]);
                    it.readjustSize();
                    it.pivotX = it.width / 2;
                    it.pivotY = it.height / 2;
                    it.x = this.posXDif_MidEast[it.itemIdx];
                    it.y = this.posYDif_MidEast[it.itemIdx];
                }
            }
            for (_local2 = 0; _local2 < _local6.Count; _local2++)
            {
                _local13 = _local6[_local2];
                if (_local13 < 10)
                    _local13 = _local13 - 1;
                else
                    _local13 = _local13 - 2;
                this.ingredientPreviewAr[_local2].texture = this.kitchenAtlas.getTexture(this.ingredientName[_local13] + "_small");
                this.ingredientPreviewAr[_local2].readjustSize();
                this.ingredientPreviewAr[_local2].visible = true;
                this.ingredientCountAr[_local2].visible = true;
                this.ingredientCountAr[_local2].text = "x" + _local7[_local2];
            }
        }
        else if (arg1 == this.HARDWARE_SHOP)
        {
            List<int> hardwareLevel = TSavegame.currentPlayer.hardwareLevel;
            int _local4;
            this.shopName.text = "HYPERMART";
            if (this.currentFocus >= this.hardwareName.Count - 1)
                this.rightButton.visible = false;
            else if (this.currentFocus <= 0)
                this.leftButton.visible = false;
            this.itemName.text = this.hardwareName[this.currentFocus];
            this.itemName.text = capitalizeWords(this.itemName.text);
            for (_local3 = 1; _local3 <= 5; _local3++)
            {
                if (hardwareLevel[this.currentFocus] > 1 && _local3 <= hardwareLevel[this.currentFocus] - 1)
                    this.starAr[_local3].texture = this.kitchenAtlas.getTexture("star");
                else
                    this.starAr[_local3].texture = this.kitchenAtlas.getTexture("star_empty");
                this.starAr[_local3].visible = true;
                this.starAr[_local3].x = 512 - 3 * 60 + _local3 * 60;
                this.starAr[_local3].y = 410;
            }
            if (hardwareLevel[this.currentFocus] > 0 && hardwareLevel[this.currentFocus] < TGlobalClass.MAX_HARDWARE_LEVEL)
            {
                _local4 = this._global.dish.getHardwareDefaultPrice(this.currentFocus) + (hardwareLevel[this.currentFocus] - 1) * this._global.dish.getHardwarePriceIncrease(this.currentFocus);
                if (this.currentFocus != 2)
                    this.upgradeInfo.text = "Current price : $" + _local4 + " ($" + this._global.dish.getHardwarePriceIncrease(this.currentFocus) + " increase)";
                else
                    this.upgradeInfo.text = "Current Effect : " + _local4 + "% (" + this._global.dish.getHardwarePriceIncrease(this.currentFocus) + "% increase)";
            }
            else
            {
                _local4 = this._global.dish.getHardwareDefaultPrice(this.currentFocus) + (hardwareLevel[this.currentFocus] - 1) * this._global.dish.getHardwarePriceIncrease(this.currentFocus);
                if (this.currentFocus != 2)
                    this.upgradeInfo.text = "Current Price : $" + _local4;
                else
                    this.upgradeInfo.text = "Current Effect : " + _local4 + "%";
            }
            _local5 = this._global.dish.getHardwareUpgradecost(this.currentFocus).at(hardwareLevel[this.currentFocus]);
            if (this._global._maxCrypto.STI(TSavegame.currentPlayer.money) < _local5 && hardwareLevel[this.currentFocus] < TGlobalClass.MAX_HARDWARE_LEVEL)
            {
                this.buyButton._text.text = "Can't Afford";
                this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_red");
                this.upgradeInfo.visible = true;
                this.upgradeCost.text = "Cost : " + this._global.addCommas(_local5);
                this.buyButton.touchable = false;
            }
            else if (hardwareLevel[this.currentFocus] == 0)
            {
                this.buyButton._text.text = "Buy";
                this.buyButton.visible = false;
                this.upgradeInfo.visible = false;
                this.upgradeCost.text = "NOT AVAILABLE";
            }
            else if (hardwareLevel[this.currentFocus] > 0 && hardwareLevel[this.currentFocus] < TGlobalClass.MAX_HARDWARE_LEVEL)
            {
                this.buyButton._text.text = "Upgrade";
                this.upgradeInfo.visible = true;
                this.upgradeCost.text = "Cost : " + this._global.addCommas(_local5);
                this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
                this.buyButton.touchable = true;
            }
            else
            {
                this.buyButton._text.text = "Maxed";
                this.upgradeCost.visible = false;
                this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_green");
                this.buyButton.touchable = false;
                this.shopCoin.visible = false;
            }
            this.buyButton._text.autoScale = true;
            for (_local2 = 0; _local2 < 3; _local2++)
            {
                this.itemAr[_local2].visible = true;
                this.itemAr[_local2].texture = this.kitchenAtlas.getTexture(this.hardwareName[this.itemAr[_local2].itemIdx]);
                this.itemAr[_local2].readjustSize();
                this.itemAr[_local2].pivotX = this.itemAr[_local2].width / 2;
                this.itemAr[_local2].pivotY = this.itemAr[_local2].height / 2;
                this.itemAr[_local2].x = this.plateAr[_local2].x;
                this.itemAr[_local2].y = this.plateAr[_local2].y;
            }
        }
        this.moneyPreview.text = this._global.addCommas(this._global._maxCrypto.STI(TSavegame.currentPlayer.money));
    }

    /// <summary>
    /// The recipe-shop block that the AS3 repeats verbatim for the western, chinese and middle-east regions
    /// (only the name list, level list, upgrade-cost table and dish index offset differ).
    /// </summary>
    void showRecipeRegion(List<string> dishNames, List<int> dishLevel, List<List<int>> upgradeCostTable, int dishOffset, int xmlOffset,
        out List<int> _local6, out List<int> _local7)
    {
        int _local3;
        int _local4;
        int _local5;
        string _local8;
        int _local12;
        int focus = this.currentFocus;
        if (focus >= dishNames.Count - 1)
            this.rightButton.visible = false;
        else if (focus <= 0)
            this.leftButton.visible = false;
        this.itemName.text = dishNames[focus];
        while (this.itemName.text.indexOf("_") > 0)
            this.itemName.text = this.itemName.text.replace("_", " ");
        this.itemName.text = capitalizeWords(this.itemName.text);
        _local4 = 0;
        _local6 = this._global.dish.getDishIngredientType(focus + dishOffset);
        _local7 = this._global.dish.getDishTarget(focus + dishOffset);
        for (_local3 = 0; _local3 < _local6.Count; _local3++)
        {
            _local4 = _local4 + (TSavegame.currentPlayer.ingredientLevel[_local6[_local3] - 1] - 1) * this._global.dish.getIngredientPriceIncrease(_local6[_local3] - 1);
        }
        if (dishLevel[focus] > 0)
        {
            _local4 = _local4 + (this._global.dish.getDishDefaultPrice(focus + dishOffset) + (dishLevel[focus] - 1) * this._global.dish.getPriceIncrease(focus + dishOffset));
            this.upgradeInfo.text = "Current price : $" + _local4 + " ($" + this._global.dish.getPriceIncrease(focus + dishOffset) + " increase)";
            _local8 = "";
        }
        else
        {
            _local12 = _local4;
            _local4 = _local4 + this._global.dish.getDishDefaultPrice(focus + dishOffset);
            this.upgradeInfo.text = "Dish price : $" + _local4 + (_local12 > 0 ? " ($" + _local12 + " ingredient bonus)" : "");
            _local8 = "";
        }
        _local5 = upgradeCostTable[focus].at(dishLevel[focus]);
        if (dishLevel[focus] > 0)
        {
            for (_local3 = 1; _local3 < TGlobalClass.MAX_DISH_LEVEL; _local3++)
            {
                if (dishLevel[focus] > 1 && _local3 <= dishLevel[focus] - 1)
                    this.starAr[_local3].texture = this.kitchenAtlas.getTexture("star");
                else
                    this.starAr[_local3].texture = this.kitchenAtlas.getTexture("star_empty");
                this.starAr[_local3].visible = true;
                this.starAr[_local3].x = 512 - 2 * 60 + _local3 * 60;
                this.starAr[_local3].y = 410;
            }
        }
        else
        {
            for (_local3 = 1; _local3 < TGlobalClass.MAX_DISH_LEVEL; _local3++)
                this.starAr[_local3].visible = false;
        }
        // dishXML.dish[focus] / dish[6 + focus] / dish[12 + focus] in the AS3 (literal offsets per region)
        int xmlIdx = xmlOffset + focus;
        if (focus == 5 && TSavegame.currentPlayer.achievementCounter[Achievement.INTERNATIONAL_CHEF] < this._global.dish.dishXML["dish"][xmlIdx]["@expertNeeded"].toInt())
        {
            this.buyButton._text.text = "Info";
            this.buyButton.idx = this._global.dish.dishXML["dish"][xmlIdx]["@expertNeeded"].toInt();
            this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
            this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_red");
            this.buyButton.touchable = true;
        }
        else if (this._global._maxCrypto.STI(TSavegame.currentPlayer.money) < _local5 && dishLevel[focus] < TGlobalClass.MAX_DISH_LEVEL)
        {
            this.buyButton._text.text = "Can't Afford";
            this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
            this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_red");
            this.buyButton.touchable = false;
        }
        else if (dishLevel[focus] == 0)
        {
            this.buyButton._text.text = "Buy";
            this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
            this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_green");
            this.buyButton.touchable = true;
        }
        else if (dishLevel[focus] > 0 && dishLevel[focus] < TGlobalClass.MAX_DISH_LEVEL)
        {
            this.buyButton._text.text = "Upgrade";
            this.upgradeCost.text = _local8 + " Cost : " + this._global.addCommas(_local5);
            this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_blue");
            this.buyButton.touchable = true;
        }
        else
        {
            this.upgradeInfo.text = "Current price : $" + _local4;
            this.buyButton._text.text = "Maxed";
            this.upgradeCost.visible = false;
            this.buyButton._image.texture = this.kitchenAtlas.getTexture("small_btn_green");
            this.buyButton.touchable = false;
            this.shopCoin.visible = false;
        }
        this.buyButton._text.autoScale = true;
    }

    public void touchQuad(Event arg1)
    {
        var e = (TouchEvent)arg1;
        int _local4 = 0;
        int _local5 = 0;
        if (this._global.tutorial.button.visible) return;
        cQuad _local2 = e.currentTarget as cQuad;
        Touch _local3 = e.getTouch(this._global._game, "hover");
        if (_local2 != null)
        {
            if (_local3 != null)
            {
                if (!_local2.hover)
                {
                    if (_local2.name == "" + PLAY_ID) this.playGlow.alpha = 1;
                    else if (_local2.name == "" + SHOP_ID) this.shopGlow.alpha = 1;
                    else if (_local2.name == "" + RECIPE_ID) this.recipeGlow.alpha = 1;
                    else if (_local2.name == "" + ACHIEVEMENT_ID) this.achievementGlow.alpha = 1;
                    else if (_local2.name == "" + HARDWARE_ID) this.hardwareGlow.alpha = 1;
                    _local2.hover = true;
                }
            }
            else if (_local2.hover)
            {
                this.playGlow.alpha = 0;
                this.shopGlow.alpha = 0;
                this.recipeGlow.alpha = 0;
                this.achievementGlow.alpha = 0;
                this.hardwareGlow.alpha = 0;
                _local2.hover = false;
            }
        }
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) == null) return;
        if (!this._bolehKlik) return;
        _local4 = AS3.toInt((e.currentTarget as Quad).name);
        if (!this._global.tutorial.canClickThisButton(_local4)) return;
        if (_local4 == PLAY_ID)
        {
            this._global._game.fadeToScreen(this._global._levelSelectScreen);
            if (!this._global._onFade) this._bolehKlik = false;
        }
        else if (_local4 == SHOP_ID)
        {
            this.resetItem();
            this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_ingredients", true));
            this.showShop(this.INGREDIENT_SHOP);
        }
        else if (_local4 == HARDWARE_ID)
        {
            this.resetItem();
            this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_hardware", true));
            this.showShop(this.HARDWARE_SHOP);
        }
        else if (_local4 == ACHIEVEMENT_ID)
        {
            this._global._achievementScreen.backToMainMenu = false;
            this._global._game.fadeToScreen(this._global._achievementScreen);
            if (!this._global._onFade) this._bolehKlik = false;
        }
        else if (_local4 == RECIPE_ID)
        {
            this.resetItem();
            this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_recipe_shop", true));
            this.showShop(this.RECIPE_SHOP);
        }
        else if (_local4 == BACK_BUTTON)
        {
            if (!this.itemIsTweening)
            {
                if (this.shopBackground.visible)
                {
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
                    for (_local5 = 0; _local5 < 5; _local5++)
                    {
                        this.plateAr[_local5].visible = false;
                        this.itemAr[_local5].visible = false;
                        this.ingredientCountAr[_local5].visible = false;
                        this.ingredientPreviewAr[_local5].visible = false;
                    }
                    for (_local5 = 0; _local5 < 10; _local5++)
                    {
                        this.starAr[_local5].visible = false;
                        this.starAr[_local5].x = -150;
                    }
                }
                else
                {
                    this._global._game.fadeToScreen(this._global._mainMenuScreen);
                    if (!this._global._onFade) this._bolehKlik = false;
                }
            }
        }
        else if (_local4 == WESTERN_BUTTON)
        {
            if (this.itemIsTweening) return;
            this.currentRecipeRegion = this.WESTERN;
            this.resetItem();
            this.showShop(this.RECIPE_SHOP);
        }
        else if (_local4 == CHINESE_BUTTON)
        {
            if (this.itemIsTweening) return;
            this.currentRecipeRegion = this.CHINESE;
            this.resetItem();
            this.showShop(this.RECIPE_SHOP);
        }
        else if (_local4 == MID_EAST_BUTTON)
        {
            if (this.itemIsTweening) return;
            this.currentRecipeRegion = this.MID_EAST;
            this.resetItem();
            this.showShop(this.RECIPE_SHOP);
        }
        else if (_local4 == BUY_BUTTON)
        {
            this.buyItem();
        }
        else if (_local4 == LEFT_BUTTON)
        {
            if (this.itemIsTweening) return;
            this.tweenItem(false);
        }
        else if (_local4 == RIGHT_BUTTON)
        {
            this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "right_button", true));
            if (this._global.tutorial.currentDialogueID == Tutorial.AFTER_LEVEL_2 && this._global.tutorial.dialogueCounter == 2) return;
            if (this.itemIsTweening) return;
            this.tweenItem(true);
        }
        else if (_local4 == FREE_COINS_BUTTON)
        {
            TGlobalClass._windowDisplay.showWindow(TWindowState.SHOW_FREE_COINS);
        }
    }

    public void buyItem()
    {
        int cost = 0;
        TPlayer player = TSavegame.currentPlayer;
        if (this.buyButton._text.text == "Info")
        {
            TGlobalClass._windowDisplay.showWindow(TWindowState.SHOW_EXPERT_NEEDED_INFO);
            return;
        }
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "buy_item", true));
        this._global._soundAsset.playSFX("money_collected");
        if (this.currentShop == this.INGREDIENT_SHOP)
        {
            GALib.TrackEvent(GALib.BUY_OR_UPGRADE_RECIPE, player.ingredientLevel[this.currentFocus], this.ingredientName[this.currentFocus]);
            cost = TGlobalClass.ingredientCost[this.currentFocus].at(player.ingredientLevel[this.currentFocus]);
            player.ingredientLevel[this.currentFocus] = player.ingredientLevel[this.currentFocus] + 1;
            player.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(player.money) - cost);
            if (!player.unlocked[Achievement.SECRET_INGREDIENT])
            {
                if (player.ingredientLevel[this.currentFocus] > player.achievementCounter[Achievement.SECRET_INGREDIENT])
                    player.achievementCounter[Achievement.SECRET_INGREDIENT] = player.ingredientLevel[this.currentFocus];
                if (player.achievementCounter[Achievement.SECRET_INGREDIENT] == Achievement.ALL_TARGET[Achievement.SECRET_INGREDIENT] / 2f)
                {
                    Achievement.tweenAchievementProgress(Achievement.SECRET_INGREDIENT);
                }
                else if (player.achievementCounter[Achievement.SECRET_INGREDIENT] >= Achievement.ALL_TARGET[Achievement.SECRET_INGREDIENT])
                {
                    player.unlocked[Achievement.SECRET_INGREDIENT] = true;
                    Achievement.tweenAchievementNotif(Achievement.SECRET_INGREDIENT);
                }
            }
            this.showShop(this.INGREDIENT_SHOP);
        }
        else if (this.currentShop == this.RECIPE_SHOP)
        {
            if (this.currentRecipeRegion == this.WESTERN)
            {
                GALib.TrackEvent(GALib.BUY_OR_UPGRADE_RECIPE, player.westernDishLevel[this.currentFocus], this.westernDishName[this.currentFocus]);
                cost = TGlobalClass.westernUpgradeCost[this.currentFocus].at(player.westernDishLevel[this.currentFocus]);
                this.levelUpDish(player.westernDishLevel, Achievement.WESTERN_RECIPE);
            }
            else if (this.currentRecipeRegion == this.CHINESE)
            {
                GALib.TrackEvent(GALib.BUY_OR_UPGRADE_RECIPE, player.chineseDishLevel[this.currentFocus], this.chineseDishName[this.currentFocus]);
                cost = TGlobalClass.chineseUpgradeCost[this.currentFocus].at(player.chineseDishLevel[this.currentFocus]);
                this.levelUpDish(player.chineseDishLevel, Achievement.ASIAN_RECIPE);
            }
            else if (this.currentRecipeRegion == this.MID_EAST)
            {
                GALib.TrackEvent(GALib.BUY_OR_UPGRADE_RECIPE, player.mideastDishLevel[this.currentFocus], this.mideastDishName[this.currentFocus]);
                cost = TGlobalClass.mideastUpgradeCost[this.currentFocus].at(player.mideastDishLevel[this.currentFocus]);
                this.levelUpDish(player.mideastDishLevel, Achievement.MIDDLE_EAST_RECIPE);
            }
            player.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(player.money) - cost);
            this.showShop(this.RECIPE_SHOP);
        }
        else if (this.currentShop == this.HARDWARE_SHOP)
        {
            GALib.TrackEvent(GALib.BUY_OR_UPGRADE_RECIPE, player.hardwareLevel[this.currentFocus], this.hardwareName[this.currentFocus]);
            cost = this._global.dish.getHardwareUpgradecost(this.currentFocus).at(player.hardwareLevel[this.currentFocus]);
            player.hardwareLevel[this.currentFocus] = player.hardwareLevel[this.currentFocus] + 1;
            player.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(player.money) - cost);
            this.showShop(this.HARDWARE_SHOP);
        }
        this.moneyPreview.text = this._global.addCommas(this._global._maxCrypto.STI(TSavegame.currentPlayer.money));
        this.moneyPreviewMinus.text = "-" + cost;
        this.moneyPreviewMinus.visible = true;
        TweenLite.killTweensOf(this.moneyPreviewMinus);
        TweenLite.killDelayedCallsTo((Action)this.hideMoneyMinusText);
        this.moneyPreviewMinus.y = this.moneyPlaceBG.y;
        TweenLite.to(this.moneyPreviewMinus, 0.5f, new Vars
        {
            ["y"] = this.moneyPreviewMinus.y + 50,
            ["onComplete"] = (Action)(() => { TweenLite.delayedCall(0.5f, (Action)this.hideMoneyMinusText); })
        });
        TSavegame.save();
    }

    /// <summary>Recipe-level increment + first-purchase achievement block (identical for the three regions in the AS3).</summary>
    void levelUpDish(List<int> dishLevel, int achievementID)
    {
        TPlayer player = TSavegame.currentPlayer;
        dishLevel[this.currentFocus] = dishLevel[this.currentFocus] + 1;
        if (dishLevel[this.currentFocus] == 1 && !player.unlocked[achievementID])
        {
            player.achievementCounter[achievementID] = player.achievementCounter[achievementID] + 1;
            if (player.achievementCounter[achievementID] == Achievement.ALL_TARGET[achievementID] / 2f)
            {
                Achievement.tweenAchievementProgress(achievementID);
            }
            else if (player.achievementCounter[achievementID] >= Achievement.ALL_TARGET[achievementID])
            {
                player.unlocked[achievementID] = true;
                Achievement.tweenAchievementNotif(achievementID);
            }
        }
    }

    public void hideMoneyMinusText()
    {
        this.moneyPreviewMinus.visible = false;
        this.moneyPreviewMinus.y = this.moneyPreviewMinus.y - 50;
    }

    public void touchButton(Event arg1)
    {
        var e = (TouchEvent)arg1;
        int _local2 = 0;
        int _local3 = 0;
        if (this._global.tutorial.button.visible) return;
        if (e.getTouch(this._global._game, TouchPhase.HOVER) != null)
        {
            _local2 = (e.currentTarget as cSprite).idx;
            if (_local2 == PLAY_ID) this.playGlow.alpha = 1;
            else if (_local2 == SHOP_ID) this.shopGlow.alpha = 1;
            else if (_local2 == ACHIEVEMENT_ID) this.achievementGlow.alpha = 1;
            else if (_local2 == RECIPE_ID) this.recipeGlow.alpha = 1;
            else if (_local2 == HARDWARE_ID) this.hardwareGlow.alpha = 1;
        }
        else
        {
            this.playGlow.alpha = 0;
            this.achievementGlow.alpha = 0;
            this.hardwareGlow.alpha = 0;
            this.recipeGlow.alpha = 0;
            this.shopGlow.alpha = 0;
        }
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) == null) return;
        _local3 = (e.currentTarget as cSprite).idx;
        if (!this._bolehKlik) return;
        if (!this._global.tutorial.canClickThisButton(_local3)) return;
        if (_local3 == PLAY_ID)
        {
            this._global._game.fadeToScreen(this._global._levelSelectScreen);
            if (!this._global._onFade) this._bolehKlik = false;
        }
        else if (_local3 == SHOP_ID)
        {
            this.resetItem();
            this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_ingredients", true));
            this.showShop(this.INGREDIENT_SHOP);
        }
        else if (_local3 == ACHIEVEMENT_ID)
        {
            this._global._game.fadeToScreen(this._global._achievementScreen);
            if (!this._global._onFade) this._bolehKlik = false;
        }
        else if (_local3 == RECIPE_ID)
        {
            this.resetItem();
            this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_recipe_shop", true));
            this.showShop(this.RECIPE_SHOP);
        }
        else if (_local3 == HARDWARE_ID)
        {
            this.resetItem();
            this.showShop(this.HARDWARE_SHOP);
        }
    }

    public void resetItem()
    {
        this.startPointLooping = 0;
        this.currentFocus = 0;
        for (int i = 0; i < 4; i++)
        {
            this.itemAr[i].itemIdx = i;
            this.shopItemContainer[i].x = 512 + i * 550;
            if (i == this.currentFocus % 4)
            {
                this.shopItemContainer[i].scaleX = 1;
                this.shopItemContainer[i].scaleY = 1;
                this.shopItemContainer[i].alpha = 1;
            }
            else
            {
                this.shopItemContainer[i].scaleX = 0.7f;
                this.shopItemContainer[i].scaleY = 0.7f;
                this.shopItemContainer[i].alpha = 0.7f;
            }
        }
    }
}
