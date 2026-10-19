// Ported from GameScreen.as (decompiled): the match-3 gameplay screen.
using System;
using System.Collections.Generic;
using Starlite;
using Mathf = UnityEngine.Mathf;

public class GameScreen : Screen
{
    // AS3 instance constants: kept as instance fields (same convention as Global)
    public readonly int PAUSE_BUTTON = 1;
    public readonly int SOUND_BUTTON = 2;
    public readonly int JUICER_ID = 3;
    public readonly int FRYER_ID = 4;
    public readonly int MUSIC_BUTTON = 5;
    public readonly int PLAY_STAR = 1;
    public readonly int PLAY_ASAP = 2;
    public readonly int PLAY_API = 3;
    public bool _udaLoad;
    public Global _global;
    public List<object> loadedFiles;
    public List<string> loadQueue;
    public List<int> loadQueueType;
    public int loadedCount;
    public bool initVector;
    public bool onLoadProgress;
    public bool paused;
    public bool hasWindow;
    public TextureAtlas gameAtlas;
    public cSprite _background;
    public List<TNonStateButton> allButtonAr;
    public cSprite targetBoard;
    public TNonStateButton pauseButton;
    public TNonStateButton soundButton;
    public TNonStateButton musicButton;
    public cSprite mute_icon;
    public cSprite mute_icon_music;
    public cSprite timerBG;
    public cSprite timerFill;
    public cSprite timerGlass;
    public Sprite timerSprite;
    public List<TCustomer> customer;
    public TNonStateButton drinkMachine;
    public cSprite drink;
    public TNonStateButton friesMachine;
    public cSprite fries;
    public List<int> availableTypeAr;
    public bool touchFirstTile;
    public List<int> tileTypeAr;
    public bool moveTile;
    public bool noMoreMoves;
    public readonly float gravity = 0.2f;
    public int chainCombo;
    public int cookiesCountOnBoard;
    public MovieClip efekAsap;
    public MovieClip efekStar;
    public MovieClip efekApi;
    public List<MovieClip> efekAsapAr;
    public List<MovieClip> efekStarAr;
    public List<MovieClip> efekApiAr;
    public int removedTileCountCookies;
    public int removedTileCountJuice;
    public int removedTileCountFries;
    public int removedTileTorch;
    public int cookiesTarget;
    public int cookiesEffect;
    public int juiceTarget;
    public int juiceEffect;
    public int friesTarget;
    public int friesEffect;
    public int torchTarget;
    public bool levelComplete;
    public bool isSwapping;
    public bool gameover;
    public bool matched;
    public bool swapBack;
    public bool highlightPossibleTile;
    public Match3Object firstTile;
    public Match3Object lastTile;
    public int idleDuration;
    public int maxFrozenTile;
    public int currentFrozenTile;
    public ExtendTextField noPossibleMoveText;
    public ExtendTextField outOfMoveText;
    public ExtendTextField levelCompleteText;
    public ExtendTextField goalExpertText;
    public Quad _blackScreen;
    public cSprite juicerLayer;
    public cSprite fryerLayer;
    public ExtendTextField juicerPercentage;
    public ExtendTextField fryerPercentage;
    public ExtendTextField targetInfoText;
    public ExtendTextField targetStateText;
    public bool touchCookies;
    public bool choosenJuice;
    public bool choosenFries;
    public FrozenTile frozenTile;
    public List<FrozenTile> frozenTileAr;
    public List<List<Match3Object>> tileAr;
    public List<Match3Object> tweenTileAr;
    public List<int> customerInUse;
    public List<int> customerOnQueue;
    public int servedCustomerCount;
    public int missedCustomerCount;
    public int timer;
    public int totalTime;
    public int goalTarget;
    public int expertTarget;
    public int VIPServeBonus;
    public ExtendTextField earnedMoneyText;
    public ExtendTextField chainBonusText;
    public List<ExtendTextField> chainBonusTextAr;
    public List<ExtendTextField> earnedMoneyTextAr;
    public cSprite coins;
    public List<cSprite> coinsAr;
    public cSprite dishes;
    public List<cSprite> dishesAr;
    public ExtendTextField largeText;
    public List<string> westernDishName;
    public List<string> chineseDishName;
    public List<string> mideastDishName;
    public Quad touchQuad;
    public bool timesUp;
    public bool cheatActive;
    public cSprite tmpDrink;
    public cSprite tmpFries;
    public BlurFilter blurFilter;
    public BlurFilter coinFilter;
    public int escCounter;
    public int lastCol;
    public int lastRow;
    public bool calledShowWindow;
    public bool firstCust;
    public int dispatchHighlightPosition;
    public int highlightX;
    public int highlightY;

    public GameScreen()
    {
        this.loadedFiles = new List<object>();
        this.loadQueue = new List<string>();
        this.loadQueueType = new List<int>();
        this.availableTypeAr = new List<int>();
    }

    public override void Start()
    {
        int i = 0;
        base.Start();
        assetPath.start(this._global.graphicsPath, this._global.soundsPath);
        this._global.currentSceneID = this._global.GAMESCREEN_ID;
        if (!this.initVector)
        {
            this.allButtonAr = new List<TNonStateButton>();
            this.customer = new List<TCustomer>();
            this.tileTypeAr = new List<int>();
            this.tileAr = new List<List<Match3Object>>();
            this.tweenTileAr = new List<Match3Object>();
            this.earnedMoneyTextAr = new List<ExtendTextField>();
            this.chainBonusTextAr = new List<ExtendTextField>();
            this.frozenTileAr = new List<FrozenTile>();
            this.efekApiAr = new List<MovieClip>();
            this.efekAsapAr = new List<MovieClip>();
            this.efekStarAr = new List<MovieClip>();
            this.customerInUse = new List<int> { 7, 1, 4, 3, 8, 10, 5, 11, 6, 12, 2, 9 };
            this.customerOnQueue = new List<int>();
            this.coinsAr = new List<cSprite>();
            this.dishesAr = new List<cSprite>();
            this.westernDishName = new List<string> { "veggie_croissant", "deluxe_chicken_croissant", "french_BBQ_combo", "beef_steak", "smokey_salmon", "premium_salmon_steak" };
            this.chineseDishName = new List<string> { "asian_fried_rice", "thai_fried_rice", "kalasan_fried_chicken", "chinese_pork_special", "fine_imperial_cuisine", "deluxe_BBQ_combo" };
            this.mideastDishName = new List<string> { "doner_kebab", "gyro_style", "tomato_domas", "tuscan_humus", "baklava", "crown_lamb_roast" };
            i = 0;
            while (i < 8)
            {
                this.tileAr.Add(new List<Match3Object>());
                i = i + 1;
            }
            this.initVector = true;
        }
        this._global.assetManager.enqueue(this._global.graphicsPath + "game.ean", this._global.graphicsPath + "game.xml");
        this._global.assetManager.enqueue(this._global.graphicsPath + "match3_BG.eag");
        this._global.assetManager.loadQueue((float f1_arg1) =>
        {
            if (f1_arg1 == 1)
            {
                this.initialize();
            }
        });
    }

    public override void Update()
    {
        if (!ready)
        {
            return;
        }
        if (this.hasWindow)
        {
            return;
        }
        ScoreManager.updateDisplayedScore();
        if (this.paused)
        {
            return;
        }
        if (this.gameover)
        {
            return;
        }
        if (this._global.tutorial.button.visible)
        {
            return;
        }
        if (!this.noMoreMoves)
        {
            this.movePieces();
        }
        for (int c = 0; c < 3; c++)
        {
            if (!this.customer[c].onTween && this.customer[c].x > -10)
            {
                this.customer[c].dePatience();
                if (this.customer[c].patienceSprite.clipRect.x > 0)
                {
                    SetClipRectX(this.customer[c].patienceSprite, 0);
                }
            }
        }
        if (!this.timesUp && !this._global.tutorial.textBox.visible)
        {
            this.timer = this.timer - 1;
            if (this.timer <= 0)
            {
                this.timesUp = true;
            }
            SetClipRectX(this.timerSprite, (float)this.timer / this.totalTime * this.timerSprite.clipRect.width - this.timerSprite.clipRect.width);
        }
        if (!this.highlightPossibleTile)
        {
            this.idleDuration = this.idleDuration + 1;
            if (this.idleDuration > 300)
            {
                this.highlightPossible();
                this.highlightPossibleTile = true;
                this.idleDuration = 0;
            }
        }
    }

    public void movePieces()
    {
        bool moving = false;
        if (!this.moveTile)
        {
            return;
        }
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Match3Object t = this.tileAr[col][row];
                if (t.removed != false)
                {
                    continue;
                }
                if (t.y < t.row * 68 + 73.5f)
                {
                    if (!t.isSwapping)
                    {
                        t.isDropping = true;
                    }
                    if (t.y + t.vy * 2 < t.row * 68 + 73.5f)
                    {
                        t.vy = t.vy + this.gravity * 2;
                        t.y = t.y + t.vy * 2;
                    }
                    else
                    {
                        t.isDropping = false;
                        t.y = t.row * 68 + 73.5f;
                    }
                    if (this.frozenTileAr[t.frozenTileIdx].visible
                        && this.frozenTileAr[t.frozenTileIdx].attachedCol == col
                        && this.frozenTileAr[t.frozenTileIdx].attachedRow == row)
                    {
                        this.frozenTileAr[t.frozenTileIdx].y = t.y;
                    }
                    moving = true;
                }
                else
                {
                    t.vy = 0;
                    t.isDropping = false;
                }
            }
        }
        if (!moving && this.moveTile)
        {
            this.moveTile = false;
            this.isSwapping = false;
            this.findAndRemoveOneMatch();
            this.setIdx();
        }
    }

    public void setIdx()
    {
        int n = 0;
        for (int col = 0; col < 8; col++)
        {
            for (int row = 0; row < 8; row++)
            {
                this.tileAr[col][row].idx = n;
                this.tileAr[col][row].col = col;
                this.tileAr[col][row].row = row;
                n++;
            }
        }
    }

    public override void Kill()
    {
        int i = 0;
        int count = 0;
        assetPath.reset();
        this.blurFilter.dispose();
        this.blurFilter = null;
        this.coinFilter.dispose();
        this.coinFilter = null;
        this.touchQuad.removeEventListener(TouchEvent.TOUCH, this.touchTile);
        this.touchQuad.removeFromParent(true);
        this.touchQuad = null;
        count = this.customer.Count;
        for (i = 0; i < count; i++)
        {
            this.customer[i].Kill();
            this.customer[i].removeFromParent(true);
            this.customer[i] = null;
            this.coinsAr[i].removeEventListener(TouchEvent.TOUCH, this.touchCoins);
            this.coinsAr[i].removeFromParent(true);
            this.coinsAr[i] = null;
            TweenLite.killTweensOf(this.dishesAr[i]);
            this.dishesAr[i].removeFromParent(true);
            this.dishesAr[i] = null;
            TweenLite.killTweensOf(this.earnedMoneyTextAr[i]);
            this.earnedMoneyTextAr[i].removeFromParent(true);
            this.earnedMoneyTextAr[i] = null;
            TweenLite.killTweensOf(this.chainBonusTextAr[i]);
            this.chainBonusTextAr[i].removeFromParent(true);
            this.chainBonusTextAr[i] = null;
        }
        this.coinsAr.Clear();
        this.dishesAr.Clear();
        this.earnedMoneyTextAr.Clear();
        this.chainBonusTextAr.Clear();
        for (int col = 0; col < 8; col++)
        {
            for (int row = 0; row < 8; row++)
            {
                TweenLite.killTweensOf(this.tileAr[col][row]);
                this.tileAr[col][row].removeFromParent(true);
                this.tileAr[col][row] = null;
            }
            this.tileAr[col].Clear();
        }
        for (i = 0; i < this.tweenTileAr.Count; i++)
        {
            TweenLite.killTweensOf(this.tweenTileAr[i]);
            this.tweenTileAr[i].removeFromParent(true);
            this.tweenTileAr[i] = null;
        }
        this.tweenTileAr.Clear();
        count = this.frozenTileAr.Count;
        for (i = 0; i < count; i++)
        {
            this.frozenTileAr[i].removeFromParent(true);
            this.frozenTileAr[i] = null;
        }
        this.frozenTileAr.Clear();
        count = this.efekApiAr.Count;
        for (i = 0; i < count; i++)
        {
            Starling.juggler.remove(this.efekApiAr[i]);
            Starling.juggler.remove(this.efekAsapAr[i]);
            Starling.juggler.remove(this.efekStarAr[i]);
            this.efekApiAr[i].removeFromParent(true);
            this.efekAsapAr[i].removeFromParent(true);
            this.efekStarAr[i].removeFromParent(true);
            this.efekApiAr[i] = null;
            this.efekAsapAr[i] = null;
            this.efekStarAr[i] = null;
        }
        this.efekApiAr.Clear();
        this.efekAsapAr.Clear();
        this.efekStarAr.Clear();
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
        // (the original ends with an empty loop over _global._gameS.numChildren)
        ready = false;
    }

    public void initialize()
    {
        int i = 0;
        int k = 0;
        int idxCounter = 0;
        Match3Object tile = null;
        List<int> ingredients = null;
        this._global.ReportGameState(this._global.GAMESTATE_PLAYING);
        List<int> usedIngredients = new List<int>();
        this.blurFilter = BlurFilter.createGlow(15988227, 1, 0.5f, 0.2f);
        this.coinFilter = BlurFilter.createGlow(65322);
        this.timesUp = false;
        if ((TGlobalClass.currentPlayedLevel + 1) % TGlobalClass.MATCH_3_SPECIAL_EVERY == 0)
        {
            TGlobalClass.isPlayingSpecialLevel = true;
        }
        GALib.TrackEvent(GALib.EVENT_START_LEVEL_STORY, TGlobalClass.currentPlayedLevel + 1, "MATCH 3" + (TGlobalClass.isPlayingSpecialLevel ? " SPECIAL" : ""));
        if (!TGlobalClass.isPlayingSpecialLevel)
        {
            if (TGlobalClass.musicPlayingIdx != 2)
            {
                this._global._soundAsset.playMusic(TGlobalClass.RandomRange(1, 2) == 1 ? "fun-time" : "sunny-advert");
                TGlobalClass.musicPlayingIdx = 2;
            }
        }
        else if (TGlobalClass.musicPlayingIdx != 4)
        {
            this._global._soundAsset.playMusic("upbeat-feeling_loop");
            TGlobalClass.musicPlayingIdx = 4;
        }
        TGlobalClass.customerFocus = 0;
        this.gameAtlas = this._global.assetManager.getTextureAtlas("game");
        this.checkDishPriceAndAvaiblity(4);
        this.availableTypeAr = new List<int>();
        if (TGlobalClass.currentRegionDish == ConstClass.WESTERN_DISH)
        {
            this.availableTypeAr.push(1, 9, 7, 2, 4, 3);
        }
        else if (TGlobalClass.currentRegionDish == ConstClass.CHINESE_DISH)
        {
            this.availableTypeAr.push(5, 6, 8, 2, 4, 3);
        }
        else if (TGlobalClass.currentRegionDish == ConstClass.MID_EAST_DISH)
        {
            this.availableTypeAr.push(11, 12, 13, 2, 4, 5);
        }
        if (TSavegame.currentPlayer.passedDays <= 1)
        {
            this.availableTypeAr = new List<int>();
            this.availableTypeAr.push(1, 9, 7, 2);
        }
        if (TSavegame.currentPlayer.passedDays > 1 && TSavegame.currentPlayer.passedDays <= 5)
        {
            if (TGlobalClass.currentRegionDish == ConstClass.WESTERN_DISH)
            {
                for (i = 0; i < TGlobalClass.dishRegionWestern.Count; i++)
                {
                    ingredients = this._global.dish.getDishIngredientType(TGlobalClass.dishRegionWestern[i]);
                    for (k = 0; k < ingredients.Count; k++)
                    {
                        if (usedIngredients.indexOf(ingredients[k]) == -1)
                        {
                            usedIngredients.push(ingredients[k]);
                        }
                    }
                }
            }
            else if (TGlobalClass.currentRegionDish == ConstClass.CHINESE_DISH)
            {
                for (i = 0; i < TGlobalClass.dishRegionChinese.Count; i++)
                {
                    ingredients = this._global.dish.getDishIngredientType(TGlobalClass.dishRegionChinese[i]);
                    for (k = 0; k < ingredients.Count; k++)
                    {
                        if (usedIngredients.indexOf(ingredients[k]) == -1)
                        {
                            usedIngredients.push(ingredients[k]);
                        }
                    }
                }
            }
            else if (TGlobalClass.currentRegionDish == ConstClass.MID_EAST_DISH)
            {
                for (i = 0; i < TGlobalClass.dishRegionMidEast.Count; i++)
                {
                    ingredients = this._global.dish.getDishIngredientType(TGlobalClass.dishRegionMidEast[i]);
                    for (k = 0; k < ingredients.Count; k++)
                    {
                        if (usedIngredients.indexOf(ingredients[k]) == -1)
                        {
                            usedIngredients.push(ingredients[k]);
                        }
                    }
                }
            }
            for (i = 0; i < this.availableTypeAr.Count; i++)
            {
                // (sic) no i-- after splice, as in the original
                if (usedIngredients.indexOf(this.availableTypeAr[i]) == -1 && this.availableTypeAr.Count > 4)
                {
                    this.availableTypeAr.splice(i, 1);
                }
            }
        }
        if (TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_COOKIES_AT)
        {
            if (TSavegame.currentPlayer.hardwareLevel[Dish.COOKIES] == 0)
            {
                TSavegame.currentPlayer.hardwareLevel[Dish.COOKIES] = 1;
            }
        }
        this.customerInUse = new List<int> { 7, 1, 4, 3, 8, 10, 5, 11, 6, 12, 2, 9 };
        TGlobalClass.tempCustUse = new List<int>();
        TGlobalClass.tempCustUse = this.customerInUse.slice(0, TSavegame.currentPlayer.lastCustIdx + 1);
        this.customerOnQueue.Clear();
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
        if (TGlobalClass.currentPlayedLevel < 30)
        {
            this.maxFrozenTile = 2;
        }
        else if (TGlobalClass.currentPlayedLevel < 35)
        {
            this.maxFrozenTile = 3;
        }
        else if (TGlobalClass.currentPlayedLevel < 40)
        {
            this.maxFrozenTile = 4;
        }
        else if (TGlobalClass.currentPlayedLevel < 45)
        {
            this.maxFrozenTile = 5;
        }
        else
        {
            this.maxFrozenTile = 6;
        }
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
        XML dayXML = TGlobalClass.levelSetXML["day"][TGlobalClass.currentPlayedLevel];
        this.timer = AS3.toInt(AS3.toNumber(dayXML["time"]) * 30.0);
        this.totalTime = this.timer;
        this.goalTarget = dayXML["goal"].toInt();
        this.expertTarget = dayXML["expert"].toInt();
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
        this.pauseButton.y = 0;
        this.pauseButton.x = 0;
        this.pauseButton.idx = this.PAUSE_BUTTON;
        this.pauseButton.scaleX = 0.7f;
        this.pauseButton.scaleY = 0.7f;
        this._global._gameS.addChild(this.pauseButton);
        this.allButtonAr.push(this.pauseButton);
        this.soundButton = new TNonStateButton(this.gameAtlas.getTexture("sound_button"));
        this.soundButton.x = 45;
        this.soundButton.y = 0;
        this.soundButton.idx = this.SOUND_BUTTON;
        this.soundButton.scaleX = 0.7f;
        this.soundButton.scaleY = 0.7f;
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
        this.musicButton.scaleX = 0.7f;
        this.musicButton.scaleY = 0.7f;
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
        this.timerSprite.y = 11.2f;
        this.timerSprite.clipRect = new Rectangle(0, 0, this.timerFill.width, this.timerFill.height);
        this.timerGlass = new cSprite(this.gameAtlas.getTexture("timer_glass"));
        this.timerGlass.x = 600;
        this.timerGlass.touchable = false;
        this._global._gameS.addChild(this.timerGlass);
        for (i = 0; i < 3; i++)
        {
            this.dishes = new cSprite(this.gameAtlas.getTexture("veggie_croissant"));
            this.dishes.pivotY = this.dishes.height;
            this.dishes.pivotX = this.dishes.width / 2;
            this.dishes.x = 375;
            this.dishes.y = i * 117 + 140;
            this.dishes.visible = false;
            this._global._gameS.addChild(this.dishes);
            this.dishesAr.push(this.dishes);
            this.customer.set(i, new TCustomer());
            this.customer[i].y = i * 118 + 63;
            this.customer[i].idx = i;
            this._global._gameS.addChild(this.customer[i]);
            this.coins = new cSprite(this.gameAtlas.getTexture("coins"));
            this.coins.pivotX = this.coins.width / 2;
            this.coins.pivotY = this.coins.height / 2;
            this.coins.x = 120 + this.coins.pivotX;
            this.coins.y = i * 117 + 90 + this.coins.pivotY;
            this.coins.scaleY = 0;
            this.coins.scaleX = 0;
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
        }
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
        if (TGlobalClass.isPlayingSpecialLevel)
        {
            this.targetStateText.text = "";
            this.targetInfoText.text = "0 / 1 food critic";
        }
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
        if (TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] == 0)
        {
            this.juicerLayer.visible = false;
            this.juicerPercentage.visible = false;
            this.drinkMachine.visible = false;
            this.drink.visible = false;
        }
        if (TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] == 0)
        {
            this.fryerLayer.visible = false;
            this.fryerPercentage.visible = false;
            this.fries.visible = false;
            this.friesMachine.visible = false;
        }
        for (int col = 0; col < 8; col++)
        {
            for (int row = 0; row < 8; row++)
            {
                tile = new Match3Object(this.gameAtlas.getTexture("keju"));
                tile.pivotX = tile.width / 2;
                tile.pivotY = tile.height / 2;
                tile.x = col * 68 + 494;
                tile.y = row * 68 + 73.5f;
                tile.col = col;
                tile.row = row;
                tile.idx = idxCounter;
                this.tileAr[col].push(tile);
                tile.touchable = false;
                this._global._gameS.addChild(tile);
                idxCounter++;
            }
        }
        this.touchQuad = new Quad(544, 544);
        this.touchQuad.x = 457;
        this.touchQuad.y = 40;
        this.touchQuad.alpha = 0;
        this._global._gameS.addChild(this.touchQuad);
        this.touchQuad.useHandCursor = true;
        this.touchQuad.addEventListener(TouchEvent.TOUCH, this.touchTile);
        for (i = 0; i < 20; i++)
        {
            this.frozenTile = new FrozenTile(this.gameAtlas.getTexture("ice"));
            this.frozenTile.name = "frozen tile";
            this.frozenTile.alignPivot();
            this.frozenTile.x = -100;
            this.frozenTile.visible = false;
            this.frozenTile.touchable = false;
            this.frozenTile.idx = i;
            this._global._gameS.addChild(this.frozenTile);
            this.frozenTileAr.push(this.frozenTile);
        }
        for (i = 0; i < 20; i++)
        {
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
        }
        for (i = 0; i < 25; i++)
        {
            tile = new Match3Object(this.gameAtlas.getTexture("keju"));
            tile.pivotX = tile.width / 2;
            tile.pivotY = tile.height / 2;
            tile.x = -150;
            tile.y = -150;
            tile.touchable = false;
            tile.visible = false;
            this.tweenTileAr.push(tile);
            this._global._gameS.addChild(tile);
        }
        // reshuffle until the board has no ready-made match and at least one possible move
        do
        {
            this.shuffleTile();
        }
        while (this.lookForMatches().Count > 0 || !this.lookForPossibles());
        this.goalExpertText = new ExtendTextField(450, 150, "Goal!!!", "komikandy", 60, 65280);
        this.goalExpertText.visible = false;
        this.goalExpertText.touchable = false;
        this.goalExpertText.pivotX = 225;
        this.goalExpertText.pivotY = 75;
        this.goalExpertText.x = 512;
        this.goalExpertText.y = 300;
        this.goalExpertText.idx = 1;
        this._global._gameS.addChild(this.goalExpertText);
        if (TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel])
        {
            this.targetStateText.text = "Expert:";
            this.targetInfoText.text = "$" + this.expertTarget;
            this.goalExpertText.idx = 2;
        }
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
        this._blackScreen.alpha = 0.5f;
        this._blackScreen.visible = false;
        this._global._gameS.addChild(this._blackScreen);
        this.outOfMoveText = new ExtendTextField(500, 200, "OUT OF MOVES!", "komikandy", 80, 16777215);
        this.outOfMoveText.pivotX = 250;
        this.outOfMoveText.pivotY = 100;
        this.outOfMoveText.x = -500;
        this.outOfMoveText.y = 450;
        this.outOfMoveText.hAlign = "center";
        this.outOfMoveText.vAlign = "center";
        this.outOfMoveText.touchable = false;
        this.outOfMoveText.visible = false;
        this._global._gameS.addChild(this.outOfMoveText);
        this.noPossibleMoveText = new ExtendTextField(500, 200, "NO POSSIBLE MOVES!", "komikandy", 50, 16777215);
        this.noPossibleMoveText.pivotX = 250;
        this.noPossibleMoveText.pivotY = 100;
        this.noPossibleMoveText.x = -500;
        this.noPossibleMoveText.y = 300;
        this.noPossibleMoveText.hAlign = "center";
        this.noPossibleMoveText.vAlign = "center";
        this.noPossibleMoveText.touchable = false;
        this.noPossibleMoveText.visible = false;
        this._global._gameS.addChild(this.noPossibleMoveText);
        this.levelCompleteText = new ExtendTextField(500, 200, "LEVEL COMPLETE!", "komikandy", 80, 16777215);
        this.levelCompleteText.pivotX = 250;
        this.levelCompleteText.pivotY = 100;
        this.levelCompleteText.x = -500;
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
        for (i = 0; i < this.allButtonAr.Count; i++)
        {
            this.allButtonAr[i].addEventListener(Event.TRIGGERED, this.touchButton);
        }
        if (TSavegame.currentPlayer.passedDays == TGlobalClass.currentPlayedLevel)
        {
            TGlobalClass.currentNotifWindow = 0;
        }
        else
        {
            TGlobalClass.currentNotifWindow = TGlobalClass.levelEventXML["day"][TGlobalClass.currentPlayedLevel]["window"].length() - 1;
        }
        // checkTut() has side effects: only the first matching tutorial is triggered
        if (this._global.tutorial.checkTut(Tutorial.LEVEL_1, 0)) { }
        else if (this._global.tutorial.checkTut(Tutorial.LEVEL_5, 4)) { }
        else if (this._global.tutorial.checkTut(Tutorial.LEVEL_7, 6)) { }
        else if (this._global.tutorial.checkTut(Tutorial.LEVEL_8, 7)) { }
        else if (this._global.tutorial.checkTut(Tutorial.LEVEL_12, 11))
        {
            this.removedTileCountJuice = this.juiceTarget - 3;
            this.juicerPercentage.text = (int)((double)this.removedTileCountJuice / this.juiceTarget * 100) + "%";
        }
        else if (this._global.tutorial.checkTut(Tutorial.LEVEL_13, 12)) { }
        else if (this._global.tutorial.checkTut(Tutorial.LEVEL_14, 13)) { }
        else if (this._global.tutorial.checkTut(Tutorial.LEVEL_16, 15)) { }
        else if (this._global.tutorial.checkTut(Tutorial.LEVEL_17, 16))
        {
            this.removedTileCountFries = this.friesTarget - 3;
            this.fryerPercentage.text = (int)((double)this.removedTileCountFries / this.friesTarget * 100) + "%";
        }
        else if (this._global.tutorial.checkTut(Tutorial.LEVEL_19, 18)) { }
        else if (this._global.tutorial.checkTut(Tutorial.LEVEL_21, 20)) { }
        else if (this._global.tutorial.checkTut(Tutorial.LEVEL_22, 21)) { }
        else if (this._global.tutorial.checkTut(Tutorial.LEVEL_26, 25))
        {
            this.dispatchHighlightPosition = 3;
            this.highlightPossible();
            this.highlightPossibleTile = true;
            for (int c = 0; c < 8; c++)
            {
                for (int r = 0; r < 8; r++)
                {
                    if (!this.tileAr[c][r].onTween)
                    {
                        TweenLite.killTweensOf(this.tileAr[c][r], false, new Vars { ["scaleX"] = true, ["scaleY"] = true });
                        this.tileAr[c][r].scaleX = 1;
                        this.tileAr[c][r].scaleY = 1;
                    }
                }
            }
        }
        else if (this._global.tutorial.checkTut(Tutorial.LEVEL_27, 26))
        {
            this.tileAr[0][0].frozen = true;
            for (int f = 0; f < this.frozenTileAr.Count; f++)
            {
                if (!this.frozenTileAr[f].visible)
                {
                    this.frozenTileAr[f].visible = true;
                    this.frozenTileAr[f].x = this.tileAr[0][0].x;
                    this.frozenTileAr[f].y = this.tileAr[0][0].y;
                    this.frozenTileAr[f].attachedCol = 0;
                    this.frozenTileAr[f].attachedRow = 0;
                    this.tileAr[0][0].frozenTileIdx = this.frozenTileAr[f].idx;
                    break;
                }
            }
            this.currentFrozenTile = this.currentFrozenTile + 1;
            this.tileAr[1][1].type = 15;
            this.tileAr[1][1].texture = this.gameAtlas.getTexture("chef_torch");
            this.tileAr[1][1].readjustSize();
            this.tileAr[1][1].pivotX = this.tileAr[1][1].width / 2;
            this.tileAr[1][1].pivotY = this.tileAr[1][1].height / 2;
        }
        else if (this._global.tutorial.checkTut(Tutorial.LEVEL_31, 30))
        {
            this.tileAr[1][1].type = 10;
            this.tileAr[1][1].texture = this.gameAtlas.getTexture("cookies");
            this.tileAr[1][1].readjustSize();
            this.tileAr[1][1].pivotX = this.tileAr[1][1].width / 2;
            this.tileAr[1][1].pivotY = this.tileAr[1][1].height / 2;
            this.cookiesCountOnBoard = this.cookiesCountOnBoard + 1;
        }
        TGlobalClass._windowDisplay.widgetInfo.text = "Goal   : " + this.goalTarget + "\nExpert : " + this.expertTarget;
        XML eventDay = TGlobalClass.levelEventXML["day"][TGlobalClass.currentPlayedLevel];
        if (eventDay["window"].length() > 0
            && (!(this._global.tutorial.currentDialogueID >= 0) || (string)eventDay["@alwaysShow"] == "true"))
        {
            TGlobalClass._windowDisplay.showBeginningWindow(TGlobalClass.currentPlayedLevel, TGlobalClass.currentNotifWindow);
        }
        else if (eventDay["window"].length() == 0)
        {
            this.startLevel();
        }
        this._global._stage.addEventListener(KeyboardEvent.KEY_DOWN, this.KeyDown);
        this.escCounter = 0;
        this.firstCust = true;
        this.calledShowWindow = false;
        this.hasWindow = true;
        this.paused = false;
        ready = true;
    }

    public void moveFries(Event arg1)
    {
        TouchEvent e = (TouchEvent)arg1;
        int i = 0;
        Rectangle friesBounds = null;
        Rectangle custBounds = null;
        Touch touch = e.getTouch(this._global._game);
        if (this.fries.visible)
        {
            this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_fries", true));
            this.tmpFries.visible = true;
        }
        if (e.getTouch(this._global._game, "moved") != null)
        {
            this._global._gameS.setChildIndex(this.tmpFries, this._global._gameS.numChildren - 1);
            this.fries.visible = false;
            this.choosenJuice = false;
            this.choosenFries = true;
            this.tmpFries.x = touch.globalX;
            this.tmpFries.y = touch.globalY;
        }
        if (e.getTouch(this._global._game, "ended") != null)
        {
            for (i = 0; i < this.customer.Count; i++)
            {
                if (!this.customer[i].hasFries && this.choosenFries)
                {
                    friesBounds = this.tmpFries.bounds;
                    custBounds = this.customer[i].clickQuad.bounds;
                    custBounds.x = this.customer[i].clickQuad.localToGlobal(new Point(0, 0)).x;
                    custBounds.y = this.customer[i].clickQuad.localToGlobal(new Point(0, 0)).y;
                    if (friesBounds.intersects(custBounds))
                    {
                        this._global._soundAsset.playSFX("put_beverage_fries_on_tray" + (TGlobalClass.RandomRange(1, 2) == 1 ? "" : "_v2"));
                        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "offered_fries", true));
                        this.choosenFries = false;
                        this.customer[i].hasFries = true;
                        this.customer[i].fries.visible = true;
                        this.tmpFries.visible = false;
                        this.tmpFries.x = 216.5f;
                        this.tmpFries.y = 522;
                        this.remakeFries();
                        break;
                    }
                }
            }
            if (i > 2)
            {
                this.tmpFries.x = 216.5f;
                this.tmpFries.y = 522;
                this._global._gameS.setChildIndex(this.tmpFries, this._global._gameS.getChildIndex(this.fries) - 1);
            }
        }
    }

    public void moveDrink(Event arg1)
    {
        TouchEvent e = (TouchEvent)arg1;
        int i = 0;
        Rectangle drinkBounds = null;
        Rectangle custBounds = null;
        Touch touch = e.getTouch(this._global._game);
        if (this.drink.visible)
        {
            this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_beverage", true));
            this.tmpDrink.visible = true;
        }
        if (e.getTouch(this._global._game, "moved") != null)
        {
            this._global._gameS.setChildIndex(this.tmpDrink, this._global._gameS.numChildren - 1);
            this.drink.visible = false;
            this.choosenJuice = true;
            this.choosenFries = false;
            this.tmpDrink.x = touch.globalX;
            this.tmpDrink.y = touch.globalY;
        }
        if (e.getTouch(this._global._game, "ended") != null)
        {
            for (i = 0; i < this.customer.Count; i++)
            {
                if (!this.customer[i].hasJuice && this.choosenJuice)
                {
                    drinkBounds = this.tmpDrink.bounds;
                    custBounds = this.customer[i].clickQuad.bounds;
                    custBounds.x = this.customer[i].clickQuad.localToGlobal(new Point(0, 0)).x;
                    custBounds.y = this.customer[i].clickQuad.localToGlobal(new Point(0, 0)).y;
                    if (drinkBounds.intersects(custBounds))
                    {
                        this._global._soundAsset.playSFX("put_beverage_fries_on_tray" + (TGlobalClass.RandomRange(1, 2) == 1 ? "" : "_v2"));
                        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "offered_beverage", true));
                        this.choosenJuice = false;
                        this.customer[i].hasJuice = true;
                        this.customer[i].juice.visible = true;
                        this.tmpDrink.visible = false;
                        this.tmpDrink.x = 108.5f;
                        this.tmpDrink.y = 527.5f;
                        this.remakeJuice();
                        break;
                    }
                }
            }
            if (i > 2)
            {
                this.tmpDrink.x = 108.5f;
                this.tmpDrink.y = 527.5f;
                this._global._gameS.setChildIndex(this.tmpDrink, this._global._gameS.getChildIndex(this.drink) - 1);
            }
        }
    }

    public bool collide(object arg1, object arg2)
    {
        var a = (DisplayObject)arg1;
        var b = (DisplayObject)arg2;
        if (((a.x >= b.x && a.x <= b.x + b.width / 2) || (b.x >= a.x && b.x <= a.x + a.width / 2))
            && ((a.y >= b.y && a.y <= b.y + b.height / 2) || (b.y >= a.y && b.y <= a.y + a.height / 2)))
        {
            return true;
        }
        return false;
    }

    public void KeyDown(Event arg1)
    {
        KeyboardEvent e = (KeyboardEvent)arg1;
        if (TSavegame.currentPlayer.name == "OrIgAmInglOl" && this._global.gameScreen.cheatActive)
        {
            TSavegame.currentPlayer.money = "faaaa";
            if (e.keyCode == Keyboard.ESCAPE)
            {
                this.escCounter = this.escCounter + 1;
                if (this.escCounter > 2)
                {
                    this.servedCustomerCount = this.goalTarget;
                    ScoreManager.totalScore = this._global._maxCrypto.ITS(this.expertTarget);
                    TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] = true;
                    this.showWindow();
                }
            }
        }
    }

    public void startLevel(int arg1 = 3)
    {
        this.hasWindow = false;
        for (int i = 0; i < arg1; i++)
        {
            this.customer[i].tweenOut(this.customer[i].CUST_SATISFIED);
        }
    }

    public void dialogueCondition(int arg1, int arg2)
    {
    }

    public void touchCoins(Event arg1)
    {
        int targetID = 0;
        int juiceBonus = 0;
        int friesBonus = 0;
        int chainBonus = 0;
        TouchEvent e = (TouchEvent)arg1;
        Touch touchHover = e.getTouch(e.currentTarget as cSprite, TouchPhase.HOVER);
        int targetIDHover = (e.currentTarget as cSprite).idx;
        if (touchHover != null)
        {
            this.coinsAr[targetIDHover].filter = this.coinFilter;
        }
        else
        {
            this.coinsAr[targetIDHover].filter = null;
        }
        if (e.getTouch(this._global._game, TouchPhase.BEGAN) == null)
        {
            return;
        }
        targetID = (e.currentTarget as cSprite).idx;
        if ((this.choosenJuice && !this.customer[targetID].hasJuice) || (this.choosenFries && !this.customer[targetID].hasFries))
        {
            this.customer[targetID].selectPriorCust();
            return;
        }
        this._global._soundAsset.playSFX("money_collected" + (TGlobalClass.RandomRange(1, 2) == 1 ? "" : "_v2"));
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_coin"));
        this._global._soundAsset.playSFX("serve");
        this.earnedMoneyTextAr[targetID].visible = true;
        this.earnedMoneyTextAr[targetID].text = "$" + (this.customer[targetID].money + this.customer[targetID].tipsBonus);
        if (this.customer[targetID].hasJuice)
        {
            TGlobalClass.juiceServed = TGlobalClass.juiceServed + 1;
            juiceBonus = this._global.dish.getHardwareDefaultPrice(Dish.JUICER) + (TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] - 1) * this._global.dish.getHardwarePriceIncrease(Dish.JUICER);
            this.earnedMoneyTextAr[targetID].text = "$" + (this.customer[targetID].money + this.customer[targetID].tipsBonus) + "\n+ $" + juiceBonus + " Juice Bonus!";
        }
        if (this.customer[targetID].hasFries)
        {
            TGlobalClass.friesServed = TGlobalClass.friesServed + 1;
            friesBonus = this._global.dish.getHardwareDefaultPrice(Dish.FRYER) + (TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] - 1) * this._global.dish.getHardwarePriceIncrease(Dish.FRYER);
            this.earnedMoneyTextAr[targetID].text = "$" + (this.customer[targetID].money + this.customer[targetID].tipsBonus) + "\n+ $" + friesBonus + " Fries Bonus!";
        }
        if (this.customer[targetID].hasJuice && this.customer[targetID].hasFries)
        {
            this.earnedMoneyTextAr[targetID].text = "$" + (this.customer[targetID].money + this.customer[targetID].tipsBonus) + "\n+ $" + (friesBonus + juiceBonus) + " Juice + Fries Bonus!";
        }
        chainBonus = (int)(this.chainCombo * 0.2 * this.customer[targetID].money);
        this.chainCombo = this.chainCombo + 1;
        TweenLite.to(this.coinsAr[targetID], 0.4f, new Vars { ["scaleX"] = 0, ["scaleY"] = 0, ["ease"] = Back.easeIn });
        ScoreManager.updateScore(this.customer[targetID].money + this.customer[targetID].tipsBonus + juiceBonus + friesBonus + chainBonus);
        if (TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel]
            && this._global._maxCrypto.STI(ScoreManager.totalScore) >= this.expertTarget
            && this.goalExpertText.idx == 2)
        {
            this.goalExpertText.idx = 3;
            TGlobalClass._windowDisplay.showGoalIconSmall();
            this._global._soundAsset.playSFX("expert_reached");
        }
        if (chainBonus > 0)
        {
            this.chainBonusTextAr[targetID].text = "Chain Bonus $" + chainBonus;
            this.chainBonusTextAr[targetID].visible = true;
            TweenLite.to(this.chainBonusTextAr[targetID], 0.8f, new Vars
            {
                ["y"] = this.chainBonusTextAr[targetID].y - 65,
                ["onComplete"] = (Action<int>)((int f1_arg1) =>
                {
                    this.chainBonusTextAr[f1_arg1].x = this.coinsAr[f1_arg1].x;
                    this.chainBonusTextAr[f1_arg1].y = this.coinsAr[f1_arg1].y;
                    this.chainBonusTextAr[f1_arg1].visible = false;
                }),
                ["onCompleteParams"] = new object[] { targetID },
            });
        }
        TweenLite.to(this.earnedMoneyTextAr[targetID], 1, new Vars
        {
            ["y"] = this.earnedMoneyTextAr[targetID].y - 65,
            ["onComplete"] = (Action<int>)((int f1_arg1) =>
            {
                this.earnedMoneyTextAr[f1_arg1].x = this.coinsAr[f1_arg1].x;
                this.earnedMoneyTextAr[f1_arg1].y = this.coinsAr[f1_arg1].y;
                this.earnedMoneyTextAr[f1_arg1].visible = false;
            }),
            ["onCompleteParams"] = new object[] { targetID },
        });
        this.servedCustomerCount = this.servedCustomerCount + 1;
        if (!TGlobalClass.isPlayingSpecialLevel)
        {
            if (this.goalExpertText.idx == 1)
            {
                this.targetInfoText.text = this.servedCustomerCount + "/" + this.goalTarget;
            }
            if (this.servedCustomerCount == this.goalTarget && this.goalExpertText.idx == 1)
            {
                TGlobalClass._windowDisplay.showGoalIconSmall();
                this.goalExpertText.idx = 2;
                this.targetInfoText.text = "$" + this.expertTarget;
                this.targetStateText.text = "Expert:";
                this._global._soundAsset.playSFX("goal_reached");
                TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] = true;
            }
        }
        else if (this.customer[targetID].asVIP && this.goalExpertText.idx == 1)
        {
            TGlobalClass._windowDisplay.showGoalIconSmall();
            this.goalExpertText.idx = 2;
            this.targetInfoText.text = "$" + this.expertTarget;
            this.targetStateText.text = "Expert:";
            this._global._soundAsset.playSFX("goal_reached");
            TSavegame.currentPlayer.goalAchieved[TGlobalClass.currentPlayedLevel] = true;
        }
        this.tweenDishAlpha(targetID, this.customer[targetID].CUST_SATISFIED);
    }

    public void touchTile(Event arg1)
    {
        TouchEvent e = (TouchEvent)arg1;
        int col = 0;
        int row = 0;
        Match3Object tile = null;
        Touch touch = e.getTouch(this.touchQuad);
        Touch hover = e.getTouch(this.touchQuad, TouchPhase.HOVER);
        if (touch != null)
        {
            col = (int)((touch.globalX - 457) / 68);
            row = (int)((touch.globalY - 40) / 68);
            if (col < 0 || row < 0 || col > 7 || row > 7)
            {
                return;
            }
            tile = this.tileAr[col][row];
            if (this.levelComplete) return;
            if (this.isSwapping) return;
            if (this.gameover) return;
            if (tile.onTween) return;
            if (tile.removed) return;
            if (tile.isSwapping) return;
            if (tile.isDropping) return;
            if (this._global.tutorial.button.visible) return;
            if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_1 && this._global.tutorial.dialogueCounter == 1) return;
            if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_1 && this._global.tutorial.dialogueCounter == 7) return;
            if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_12 && this._global.tutorial.dialogueCounter == 4) return;
            if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_12 && this._global.tutorial.dialogueCounter == 5) return;
            if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_17 && this._global.tutorial.dialogueCounter == 4) return;
            if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_17 && this._global.tutorial.dialogueCounter == 5) return;
            if (touch.phase == TouchPhase.BEGAN)
            {
                int tileIdx = tile.idx;
                this.matched = false;
                this.swapBack = false;
                this.choosenFries = false;
                this.choosenJuice = false;
                if (this.juicerPercentage.text == "Select Customer")
                {
                    this.juicerPercentage.text = "READY!!";
                }
                if (this.fryerPercentage.text == "Select Customer")
                {
                    this.fryerPercentage.text = "READY!!";
                }
                this.highlightPossibleTile = false;
                this.idleDuration = 0;
                this.chainCombo = 0;
                for (int c = 0; c < 8; c++)
                {
                    for (int r = 0; r < 8; r++)
                    {
                        if (!this.tileAr[c][r].onTween)
                        {
                            TweenLite.killTweensOf(this.tileAr[c][r], false, new Vars { ["scaleX"] = true, ["scaleY"] = true });
                            this.tileAr[c][r].scaleX = 1;
                            this.tileAr[c][r].scaleY = 1;
                        }
                    }
                }
                if (tile.type == 10)
                {
                    this.touchCookies = true;
                }
                TweenLite.killTweensOf(tile, false, new Vars { ["scaleX"] = true, ["scaleY"] = true });
                tile.scaleX = 1;
                tile.scaleY = 1;
                if (!this.touchFirstTile)
                {
                    this.touchFirstTile = true;
                }
            }
            else if (touch.phase == TouchPhase.MOVED)
            {
                if (this.lastCol == col && this.lastRow == row)
                {
                    return;
                }
                if (!this.touchFirstTile)
                {
                    return;
                }
                if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_26)
                {
                    if (this._global.tutorial.dialogueCounter < 3)
                    {
                        return;
                    }
                    if (this._global.tutorial.dialogueCounter == 3)
                    {
                        int h = 0;
                        for (h = 0; h < 3; h++)
                        {
                            if (AS3.toInt(this._global.tutorial.highlightedAr[h]["col"]) == this.lastCol && AS3.toInt(this._global.tutorial.highlightedAr[h]["row"]) == this.lastRow)
                            {
                                break;
                            }
                        }
                        if (h == 3)
                        {
                            return;
                        }
                    }
                }
                else if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_27)
                {
                    if (this._global.tutorial.dialogueCounter == 0)
                    {
                        return;
                    }
                    if (this._global.tutorial.dialogueCounter == 2 && (!(this.tileAr[this.lastCol][this.lastRow].type == 15) || row >= this.lastRow))
                    {
                        return;
                    }
                }
                else if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_31 && this._global.tutorial.dialogueCounter <= 2)
                {
                    return;
                }
                if (this.lastRow == row && this.lastCol < col)
                {
                    if (!this._swapWithNeighbour(tile, this.lastCol + 1, this.lastRow, 1, 0)) return;
                }
                else if (this.lastRow == row && this.lastCol > col)
                {
                    if (!this._swapWithNeighbour(tile, this.lastCol - 1, this.lastRow, 1, 0)) return;
                }
                else if (this.lastRow < row && this.lastCol == col)
                {
                    if (!this._swapWithNeighbour(tile, this.lastCol, this.lastRow + 1, 0, 1)) return;
                }
                else if (this.lastRow > row && this.lastCol == col)
                {
                    if (!this._swapWithNeighbour(tile, this.lastCol, this.lastRow - 1, 0, 1)) return;
                }
            }
            else if (touch.phase == TouchPhase.ENDED)
            {
                if (this.touchFirstTile)
                {
                    this.touchFirstTile = false;
                }
                if (this.touchCookies)
                {
                    this.touchCookies = false;
                    TGlobalClass.cookiesUsed = TGlobalClass.cookiesUsed + 1;
                    this.giveTheCookies(tile, this.cookiesEffect);
                }
            }
        }
        if (hover != null)
        {
            tile.filter = this.blurFilter;
            tile.hover = true;
            if (!(this.lastCol == col) || !(this.lastRow == row))
            {
                this.tileAr[this.lastCol][this.lastRow].filter = null;
            }
        }
        else
        {
            this.tileAr[this.lastCol][this.lastRow].filter = null;
        }
        this.lastCol = col;
        this.lastRow = row;
    }

    /// <summary>
    /// One drag direction of touchTile (the original repeats this block four times).
    /// Returns false where the original returns from touchTile, true after a swap was started.
    /// </summary>
    bool _swapWithNeighbour(Match3Object tile, int nCol, int nRow, int shakeX, int shakeY)
    {
        if (this.tileAr[nCol][nRow].isDropping) return false;
        if (this.tileAr[nCol][nRow].onTween) return false;
        if (this.tileAr[nCol][nRow].isSwapping) return false;
        if (this.tileAr[nCol][nRow].removed) return false;
        this.touchCookies = false;
        if (this.tileAr[nCol][nRow].frozen || this.tileAr[this.lastCol][this.lastRow].frozen)
        {
            if (this.tileAr[nCol][nRow].frozen)
            {
                this.tweenShake(this.frozenTileAr[this.tileAr[nCol][nRow].frozenTileIdx], shakeX, shakeY);
            }
            if (this.tileAr[this.lastCol][this.lastRow].frozen)
            {
                this.tweenShake(this.frozenTileAr[this.tileAr[this.lastCol][this.lastRow].frozenTileIdx], shakeX, shakeY);
            }
            return false;
        }
        tile.scaleX = 1;
        tile.scaleY = 1;
        TweenLite.killTweensOf(tile, false, new Vars { ["scaleX"] = true, ["scaleY"] = true });
        this.touchFirstTile = false;
        this.firstTile = this.tileAr[this.lastCol][this.lastRow];
        this.lastTile = this.tileAr[nCol][nRow];
        this.swapTile(this.firstTile, this.lastTile);
        return true;
    }

    public void giveTheCookies(Match3Object arg1 = null, int arg2 = 0)
    {
        Match3Object tweenTarget = arg1;
        int effectedPatience = arg2;
        this._global._soundAsset.playSFX("cookie" + (TGlobalClass.RandomRange(1, 2) == 1 ? "" : "_v2"));
        bool setLowestPatience = false;
        int lowestPatience = 0;
        int lowestPatienceIdx = 0;
        List<string> textureName = new List<string> { "bread", "selada", "beef", "ayam", "nasi", "bacon", "salmon", "ikan", "keju", "cookies" };
        int i = 0;
        for (i = 0; i < 3; i++)
        {
            if (this.customer[i].patienceSprite.clipRect.x < lowestPatience && !this.customer[i].onTween)
            {
                lowestPatience = (int)this.customer[i].patienceSprite.clipRect.x;
                lowestPatienceIdx = i;
                setLowestPatience = true;
            }
        }
        if (tweenTarget == null || !setLowestPatience)
        {
            return;
        }
        for (i = 0; i < this.tweenTileAr.Count; i++)
        {
            if (this.tweenTileAr[i].onTween)
            {
                continue;
            }
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
            this.cookiesCountOnBoard = this.cookiesCountOnBoard - 1;
            TweenLite.to(this.tweenTileAr[i], 0.5f + tweenTarget.col * 0.05f, new Vars
            {
                ["x"] = this.customer[lowestPatienceIdx].x + 15,
                ["y"] = this.customer[lowestPatienceIdx].y + 25,
                ["ease"] = Linear.easeNone,
                ["onComplete"] = (Action<int, int>)((int f1_arg1, int f1_arg2) =>
                {
                    int idx = f1_arg1;
                    int tweenTileIdx = f1_arg2;
                    this.tweenTileAr[tweenTileIdx].visible = false;
                    this.tweenTileAr[tweenTileIdx].onTween = false;
                    double toadd = effectedPatience / 100.0 * this.customer[idx].totalPatience;
                    if (this.customer[idx].patience + toadd > this.customer[idx].totalPatience)
                    {
                        toadd = this.customer[idx].totalPatience - this.customer[idx].patience;
                    }
                    this.customer[idx].tweeningPatience = true;
                    TweenLite.to(this.customer[idx], 0.3f, new Vars
                    {
                        ["patience"] = this.customer[idx].patience + toadd,
                        ["onComplete"] = (Action<int>)((int f2_arg1) =>
                        {
                            this.customer[f2_arg1].tweeningPatience = false;
                            this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "give_cookies"));
                        }),
                        ["onCompleteParams"] = new object[] { idx },
                    });
                    double halfPrice = 0.5 * this.customer[idx].totalMoney;
                    double patienceDivided = (this.customer[idx].patience + toadd) / this.customer[idx].totalPatience;
                    if (this.customer[idx].patience >= 0.8 * this.customer[idx].totalPatience)
                    {
                        this.customer[idx].money = AS3.toInt(halfPrice * 2);
                        this.customer[idx].moneyDisplay.text = "$" + this.customer[idx].money;
                    }
                    else
                    {
                        this.customer[idx].money = AS3.toInt(halfPrice + halfPrice * patienceDivided);
                        this.customer[idx].moneyDisplay.text = "$" + this.customer[idx].money;
                    }
                }),
                ["onCompleteParams"] = new object[] { lowestPatienceIdx, i },
            });
            break;
        }
    }

    public void swapTile(Match3Object arg1, Match3Object arg2)
    {
        Match3Object firstTile = arg1;
        Match3Object lastTile = arg2;
        this.tileAr[firstTile.col][firstTile.row].isSwapping = true;
        this.tileAr[lastTile.col][lastTile.row].isSwapping = true;
        this.isSwapping = true;
        int tempCol = lastTile.col;
        int tempRow = lastTile.row;
        lastTile.filter = null;
        firstTile.filter = null;
        lastTile.col = firstTile.col;
        lastTile.row = firstTile.row;
        firstTile.col = tempCol;
        firstTile.row = tempRow;
        this.tileAr[firstTile.col][firstTile.row] = firstTile;
        this.tileAr[lastTile.col][lastTile.row] = lastTile;
        TweenLite.to(firstTile, 0.2f, new Vars { ["x"] = lastTile.x, ["y"] = lastTile.y, ["ease"] = Linear.easeNone });
        TweenLite.to(lastTile, 0.2f, new Vars
        {
            ["x"] = firstTile.x,
            ["y"] = firstTile.y,
            ["ease"] = Linear.easeNone,
            ["onComplete"] = (Action)(() =>
            {
                this.tileAr[firstTile.col][firstTile.row].isSwapping = false;
                this.tileAr[lastTile.col][lastTile.row].isSwapping = false;
                this.isSwapping = false;
                List<List<int>> m1 = this.getMatches(this.tileAr[firstTile.col][firstTile.row]);
                List<List<int>> m2 = this.getMatches(this.tileAr[lastTile.col][lastTile.row]);
                if (firstTile.type == 15 || lastTile.type == 15)
                {
                    this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "start_torch", false));
                    if (firstTile.type == 15)
                    {
                        this.startTorchEffect(firstTile);
                    }
                    if (lastTile.type == 15)
                    {
                        this.startTorchEffect(lastTile);
                    }
                }
                else if (m1.Count == 0 && m2.Count == 0 && !this.swapBack)
                {
                    this.isSwapping = true;
                    this.swapBack = true;
                    this.swapTile(firstTile, lastTile);
                }
                if (m1.Count > 0 || m2.Count > 0)
                {
                    if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_26 && this._global.tutorial.dialogueCounter == 3)
                    {
                        bool frozenFound = this._matchesContainFrozen(m1);
                        if (!frozenFound)
                        {
                            frozenFound = this._matchesContainFrozen(m2);
                        }
                        if (!frozenFound)
                        {
                            this.isSwapping = true;
                            this.swapBack = true;
                            this.swapTile(firstTile, lastTile);
                            return;
                        }
                    }
                    this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "matched", false));
                    this.matched = true;
                    this.findAndRemoveOneMatch();
                }
            }),
        });
    }

    /// <summary>Inner search of swapTile's tutorial check: does any tile of these matches sit on a frozen tile?</summary>
    bool _matchesContainFrozen(List<List<int>> matches)
    {
        for (int a = 0; a < matches.Count; a++)
        {
            for (int b = 0; b < matches[a].Count; b++)
            {
                bool found = false;
                for (int c = 0; c < 8; c++)
                {
                    int d;
                    for (d = 0; d < 8; d++)
                    {
                        if (this.tileAr[c][d].idx == matches[a][b])
                        {
                            if (this.tileAr[c][d].frozen)
                            {
                                found = true;
                            }
                            break;
                        }
                    }
                    if (d != 8)
                    {
                        break;
                    }
                }
                if (found)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void startTorchEffect(Match3Object arg1)
    {
        bool doRemove = false;
        this._global._soundAsset.playSFX("power_up");
        for (int c = 0; c < 8; c++)
        {
            if (!this.tileAr[c][arg1.row].isDropping && !this.tileAr[c][arg1.row].onTween && !this.tileAr[c][arg1.row].removed && !this.tileAr[c][arg1.row].isSwapping)
            {
                doRemove = true;
                if (this.tileAr[c][arg1.row].frozen)
                {
                    this.tileAr[c][arg1.row].frozen = false;
                    this.frozenTileAr[this.tileAr[c][arg1.row].frozenTileIdx].visible = false;
                }
                else if (this.tileAr[c][arg1.row].type == 10)
                {
                    doRemove = false;
                    TGlobalClass.cookiesUsed = TGlobalClass.cookiesUsed + 1;
                    this.playBoardEffect(this.PLAY_API, c, arg1.row);
                    this.playBoardEffect(this.PLAY_ASAP, c, arg1.row);
                    this.giveTheCookies(this.tileAr[c][arg1.row], this.cookiesEffect);
                }
                else if (!(this.tileAr[c][arg1.row].type == 10) && !(this.tileAr[c][arg1.row].type == 15))
                {
                    this.checkCustOrderPrior(c, arg1.row);
                    this.checkCustOrder(c, arg1.row);
                }
                if (doRemove)
                {
                    this.tileAr[c][arg1.row].removed = true;
                    this.playBoardEffect(this.PLAY_API, c, arg1.row);
                    this.playBoardEffect(this.PLAY_ASAP, c, arg1.row);
                    this.affectAbove(this.tileAr[c][arg1.row]);
                    this.addNewPieces();
                }
            }
        }
    }

    public List<List<int>> getMatches(Match3Object arg1)
    {
        List<List<int>> result = new List<List<int>>();
        List<int> horiz = new List<int>();
        horiz.push(this.tileAr[arg1.col][arg1.row].idx);
        for (int k = 1; arg1.col + k < 8; k++)
        {
            Match3Object n = this.tileAr[arg1.col + k][arg1.row];
            if (!(n.type == arg1.type && !(n.type == 10) && !(arg1.type == 10) && !(arg1.type == 15))) break;
            if (!(!n.isDropping && !n.onTween && !n.removed && !n.isSwapping)) break;
            horiz.push(n.idx);
        }
        for (int k = 1; arg1.col - k >= 0; k++)
        {
            Match3Object n = this.tileAr[arg1.col - k][arg1.row];
            if (!(n.type == arg1.type && !(n.type == 10) && !(arg1.type == 10) && !(arg1.type == 15))) break;
            if (!(!n.isDropping && !n.onTween && !n.removed && !n.isSwapping)) break;
            horiz.push(n.idx);
        }
        if (horiz.Count > 2)
        {
            result.push(horiz);
        }
        List<int> vert = new List<int>();
        vert.push(this.tileAr[arg1.col][arg1.row].idx);
        for (int k = 1; arg1.row + k < 8; k++)
        {
            Match3Object n = this.tileAr[arg1.col][arg1.row + k];
            if (!(n.type == arg1.type && !(n.type == 10) && !(arg1.type == 10) && !(arg1.type == 15))) break;
            if (!(!n.isDropping && !n.onTween && !n.removed && !n.isSwapping)) break;
            vert.push(n.idx);
        }
        for (int k = 1; arg1.row - k >= 0; k++)
        {
            Match3Object n = this.tileAr[arg1.col][arg1.row - k];
            if (!(n.type == arg1.type && !(n.type == 10) && !(arg1.type == 10) && !(arg1.type == 15))) break;
            if (!(!n.isDropping && !n.onTween && !n.removed && !n.isSwapping)) break;
            vert.push(n.idx);
        }
        if (vert.Count > 2)
        {
            result.push(vert);
        }
        return result;
    }

    public void shuffleTile()
    {
        for (int c = 0; c < 8; c++)
        {
            for (int r = 0; r < 8; r++)
            {
                if (!(this.tileAr[c][r].type == 10) && !(this.tileAr[c][r].type == 15))
                {
                    this.tileAr[c][r].type = this.availableTypeAr[this._global._maxCrypto.RandomRange(0, this.availableTypeAr.Count - 1)];
                }
                string tex = _tileTextureName(this.tileAr[c][r].type);
                if (tex != null)
                {
                    this.tileAr[c][r].texture = this.gameAtlas.getTexture(tex);
                }
                this.tileAr[c][r].readjustSize();
                this.tileAr[c][r].pivotX = this.tileAr[c][r].width / 2;
                this.tileAr[c][r].pivotY = this.tileAr[c][r].height / 2;
            }
        }
    }

    /// <summary>The original's per-type texture switch (types 1..13; others keep their texture).</summary>
    static string _tileTextureName(int type)
    {
        switch (type)
        {
            case 1: return "bread";
            case 2: return "selada";
            case 3: return "beef";
            case 4: return "ayam";
            case 5: return "nasi";
            case 6: return "bacon";
            case 7: return "salmon";
            case 8: return "ikan";
            case 9: return "keju";
            case 10: return "cookies";
            case 11: return "pita_bread";
            case 12: return "lamb";
            case 13: return "tomato";
            default: return null;
        }
    }

    public void findAndRemoveOneMatch()
    {
        List<List<int>> matches = null;
        int tempType = 0;
        int i = 0;
        int j = 0;
        int horzCount = 0;
        int vertCount = 0;
        int col = 0;
        bool getMatch5 = false;
        bool getMatch6orMore = false;
        int n = 0;
        int row = 0;
        int m = 0;
        List<int> temp = new List<int>();
        List<Match3Object> tempTorL = new List<Match3Object>();
        matches = this.lookForMatches();
        int length = matches.Count;
        if (length > 0)
        {
            this.highlightPossibleTile = false;
            this.idleDuration = 0;
        }
        int goodMovesLength = 3;
        for (i = 0; i < matches.Count; i++)
        {
            if (matches[i].Count >= 4)
            {
                goodMovesLength = matches[i].Count;
            }
            for (j = 0; j < matches[i].Count; j++)
            {
                horzCount = 1;
                vertCount = 1;
                for (col = 0; col < 8; col++)
                {
                    for (row = 0; row < 8; row++)
                    {
                        if (this.tileAr[col][row].idx != matches[i][j])
                        {
                            continue;
                        }
                        tempType = this.tileAr[col][row].type;
                        if (this.tileAr[col][row].removed != true)
                        {
                            this.tileAr[col][row].removed = true;
                            this.tileAr[col][row].filter = null;
                            if (matches[i].Count == 3)
                            {
                                this.tileAr[col][row].asMatch3Part = true;
                            }
                            else if (matches[i].Count == 4)
                            {
                                this.tileAr[col][row].asMatch4Part = true;
                            }
                            else if (matches[i].Count == 5)
                            {
                                this.tileAr[col][row].asMatch5Part = true;
                            }
                            else if (matches[i].Count > 5)
                            {
                                this.tileAr[col][row].asMatchMorePart = true;
                            }
                            temp.push(this.tileAr[col][row].idx);
                            continue;
                        }
                        // tile already removed by a previous match: T or L shape
                        tempTorL.push(this.tileAr[col][row]);
                        for (m = 1; row - m >= 0; m++)
                        {
                            if (this.tileAr[col][row].type != this.tileAr[col][row - m].type) break;
                            vertCount = vertCount + 1;
                            tempTorL.push(this.tileAr[col][row - m]);
                        }
                        for (m = 1; row + m < 8; m++)
                        {
                            if (this.tileAr[col][row].type != this.tileAr[col][row + m].type) break;
                            vertCount = vertCount + 1;
                            tempTorL.push(this.tileAr[col][row + m]);
                        }
                        for (m = 1; col - m >= 0; m++)
                        {
                            if (this.tileAr[col][row].type != this.tileAr[col - m][row].type) break;
                            horzCount = horzCount + 1;
                            tempTorL.push(this.tileAr[col - m][row]);
                        }
                        for (m = 1; col + m < 8; m++)
                        {
                            if (this.tileAr[col][row].type != this.tileAr[col + m][row].type) break;
                            horzCount = horzCount + 1;
                            tempTorL.push(this.tileAr[col + m][row]);
                        }
                    }
                }
                for (n = 0; n < tempTorL.Count; n++)
                {
                    Match3Object t = this.tileAr[tempTorL[n].col][tempTorL[n].row];
                    if (horzCount == 3 && vertCount == 3)
                    {
                        t.asMatch3Part = false;
                        t.asMatch4Part = false;
                        t.asMatchMorePart = false;
                        t.asMatch5Part = true;
                        getMatch5 = true;
                        if (goodMovesLength < 5)
                        {
                            goodMovesLength = 5;
                        }
                    }
                    else if (horzCount > 3 || vertCount > 3)
                    {
                        t.asMatch3Part = false;
                        t.asMatch4Part = false;
                        t.asMatch5Part = false;
                        t.asMatchMorePart = true;
                        getMatch6orMore = true;
                        if (goodMovesLength < horzCount + vertCount)
                        {
                            goodMovesLength = horzCount + vertCount - 1;
                        }
                    }
                }
                tempTorL.Clear();
            }
            if (tempType == 3)
            {
                this._global._soundAsset.playSFX("beef_sizzle");
            }
            else if (tempType == 4)
            {
                this._global._soundAsset.playSFX("chicken_sizzle");
            }
            else if (tempType == 7)
            {
                this._global._soundAsset.playSFX("fish_sizzle");
            }
            else if (tempType == 6)
            {
                this._global._soundAsset.playSFX("pork_sizzle");
            }
            else
            {
                this._global._soundAsset.playSFX("other_ingredient_matched_v" + TGlobalClass.RandomRange(1, 2));
            }
        }
        if (goodMovesLength >= 4)
        {
            this.showGoodMovesText(goodMovesLength);
        }
        if (TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_MATCH_BONUS)
        {
            if (goodMovesLength >= 5)
            {
                ScoreManager.updateScore(goodMovesLength - 4);
                if (this._global._maxCrypto.STI(ScoreManager.totalScore) >= this.expertTarget && this.goalExpertText.idx == 2)
                {
                    this.goalExpertText.idx = 3;
                    TGlobalClass._windowDisplay.showGoalIconSmall();
                    this._global._soundAsset.playSFX("expert_reached");
                }
            }
        }
        bool hasFrozenRemoved = false;
        for (col = 0; col < 8; col++)
        {
            for (row = 0; row < 8; row++)
            {
                for (i = 0; i < temp.Count; i++)
                {
                    if (this.tileAr[col][row].idx != temp[i])
                    {
                        continue;
                    }
                    if (this.tileAr[col][row].frozen)
                    {
                        this._global._soundAsset.playSFX("ice_break" + (TGlobalClass.RandomRange(1, 2) == 1 ? "" : "_v2"));
                        this.tileAr[col][row].frozen = false;
                        this.tileAr[col][row].removed = false;
                        this.frozenTileAr[this.tileAr[col][row].frozenTileIdx].visible = false;
                        this.frozenTileAr[this.tileAr[col][row].frozenTileIdx].x = -100;
                        this.tileAr[col][row].frozenTileIdx = 0;
                        this.currentFrozenTile = this.currentFrozenTile - 1;
                        hasFrozenRemoved = true;
                    }
                    else
                    {
                        int type = this.tileAr[col][row].type;
                        if (type == 2 || type == 3 || type == 6 || type == 7 || type == 8 || type == 12)
                        {
                            this.playBoardEffect(this.PLAY_ASAP, col, row);
                        }
                        else
                        {
                            this.playBoardEffect(this.PLAY_STAR, col, row);
                        }
                        this.affectAbove(this.tileAr[col][row]);
                    }
                    break;
                }
            }
        }
        temp.Clear();
        this.addNewPieces();
        if (hasFrozenRemoved)
        {
            this.findAndRemoveOneMatch();
        }
        if (matches.Count <= 0 && !this.moveTile)
        {
            if (this.gameover)
            {
            }
            else if (this.levelComplete)
            {
                this.levelComplete = false;
            }
            else if (this.lookForPossibles() != true)
            {
                this._blackScreen.visible = true;
                this.noPossibleMoveText.visible = true;
                TweenLite.to(this.noPossibleMoveText, 1.5f, new Vars
                {
                    ["x"] = 512,
                    ["ease"] = Expo.easeOut,
                    ["onComplete"] = (Action)(() =>
                    {
                        this.noMoreMoves = true;
                        this.tweenAllDown();
                        TweenLite.to(this.noPossibleMoveText, 1, new Vars
                        {
                            ["x"] = 1524,
                            ["ease"] = Expo.easeIn,
                            ["onComplete"] = (Action)(() =>
                            {
                                this.noPossibleMoveText.x = -500;
                            }),
                        });
                    }),
                });
            }
        }
    }

    public void playBoardEffect(int arg1, int arg2, int arg3)
    {
        for (int k = 0; k < 20; k++)
        {
            if (arg1 == this.PLAY_API)
            {
                if (!this.efekApiAr[k].visible)
                {
                    this.efekApiAr[k].visible = true;
                    this.efekApiAr[k].x = this.tileAr[arg2][arg3].x;
                    this.efekApiAr[k].y = this.tileAr[arg2][arg3].y;
                    Starling.juggler.add(this.efekApiAr[k]);
                    this.efekApiAr[k].currentFrame = 0;
                    this.efekApiAr[k].play();
                    this.efekApiAr[k].addEventListener(Event.COMPLETE, this.onEffectComplete);
                    return;
                }
            }
            else if (arg1 == this.PLAY_ASAP)
            {
                if (!this.efekAsapAr[k].visible)
                {
                    this.efekAsapAr[k].visible = true;
                    this.efekAsapAr[k].x = this.tileAr[arg2][arg3].x;
                    this.efekAsapAr[k].y = this.tileAr[arg2][arg3].y - 10;
                    Starling.juggler.add(this.efekAsapAr[k]);
                    this.efekAsapAr[k].currentFrame = 0;
                    this.efekAsapAr[k].play();
                    this.efekAsapAr[k].addEventListener(Event.COMPLETE, this.onEffectComplete);
                    return;
                }
            }
            else if (arg1 == this.PLAY_STAR)
            {
                if (!this.efekStarAr[k].visible)
                {
                    this.efekStarAr[k].visible = true;
                    this.efekStarAr[k].x = this.tileAr[arg2][arg3].x;
                    this.efekStarAr[k].y = this.tileAr[arg2][arg3].y;
                    Starling.juggler.add(this.efekStarAr[k]);
                    this.efekStarAr[k].currentFrame = 0;
                    this.efekStarAr[k].play();
                    this.efekStarAr[k].addEventListener(Event.COMPLETE, this.onEffectComplete);
                    return;
                }
            }
        }
    }

    public void onEffectComplete(Event arg1)
    {
        MovieClip mc = arg1.currentTarget as MovieClip;
        mc.visible = false;
        mc.removeEventListener(Event.COMPLETE, this.onEffectComplete);
        mc.x = -100;
        mc.currentFrame = 0;
        Starling.juggler.remove(mc);
    }

    public void tweenAllDown()
    {
        int count = 0;
        bool levelTargetCompleted = this.levelComplete;
        this._blackScreen.visible = true;
        for (int col = 0; col < 8; col++)
        {
            for (int row = 0; row < 8; row++)
            {
                Match3Object t = this.tileAr[col][row];
                t.removed = false;
                t.aktif = false;
                TweenLite.killTweensOf(t, false, new Vars { ["scaleX"] = true, ["scaleY"] = true });
                t.asMatch3Part = false;
                t.asMatch4Part = false;
                t.asMatch5Part = false;
                t.asMatchMorePart = false;
                t.isDropping = false;
                t.onTween = true;
                t.isSwapping = false;
                TweenLite.to(t, 1 - t.row * 0.05f, new Vars
                {
                    ["y"] = 700,
                    ["delay"] = 0.08f * (9 - t.row),
                    ["ease"] = Linear.easeNone,
                    ["onComplete"] = (Action<int, int>)((int f1_arg1, int f1_arg2) =>
                    {
                        count = count + 1;
                        this.tileAr[f1_arg1][f1_arg2].onTween = false;
                        this.tileAr[f1_arg1][f1_arg2].y = -80;
                        if (count >= 64)
                        {
                            do
                            {
                                this.shuffleTile();
                            }
                            while (this.lookForMatches().Count != 0 || !this.lookForPossibles());
                            this.moveTile = true;
                            this._blackScreen.visible = false;
                            this.noMoreMoves = false;
                        }
                    }),
                    ["onCompleteParams"] = new object[] { col, row },
                });
            }
        }
        for (int i = 0; i < 20; i++)
        {
            if (this.frozenTileAr[i].visible)
            {
                TweenLite.to(this.frozenTileAr[i], 1 - this.frozenTileAr[i].attachedRow * 0.05f, new Vars
                {
                    ["y"] = 700,
                    ["delay"] = 0.08f * (9 - this.frozenTileAr[i].attachedRow),
                    ["ease"] = Linear.easeNone,
                });
            }
        }
    }

    public void affectAbove(Match3Object arg1)
    {
        Match3Object tmp = null;
        for (int r = arg1.row - 1; r >= 0; r--)
        {
            if (this.tileAr[arg1.col][r].removed != false)
            {
                continue;
            }
            if (this.tileAr[arg1.col][r].frozen)
            {
                this.frozenTileAr[this.tileAr[arg1.col][r].frozenTileIdx].attachedRow = this.frozenTileAr[this.tileAr[arg1.col][r].frozenTileIdx].attachedRow + 1;
            }
            tmp = this.tileAr[arg1.col][r + 1];
            this.tileAr[arg1.col][r].row = this.tileAr[arg1.col][r].row + 1;
            this.tileAr[arg1.col][r + 1] = this.tileAr[arg1.col][r];
            this.tileAr[arg1.col][r] = tmp;
            this.tileAr[arg1.col][r].removed = true;
            this.tileAr[arg1.col][r + 1].removed = false;
            this.tileAr[arg1.col][r + 1].isDropping = true;
        }
    }

    public void addNewPieces()
    {
        int missingPieces = 0;
        bool notComplete = false;
        for (int col = 0; col < 8; col++)
        {
            missingPieces = 0;
            for (int row = 7; row >= 0; row--)
            {
                if (this.tileAr[col][row].removed != true)
                {
                    continue;
                }
                if (this.tileAr[col][row].onTween)
                {
                    continue;
                }
                if (TGlobalClass.currentPlayedLevel >= TGlobalClass.UNLOCK_COOKIES_AT)
                {
                    this.removedTileCountCookies = this.removedTileCountCookies + 1;
                }
                if (this.fryerLayer.visible)
                {
                    this.removedTileCountFries = this.removedTileCountFries + 1;
                }
                if (this.juicerLayer.visible)
                {
                    this.removedTileCountJuice = this.removedTileCountJuice + 1;
                }
                if (TGlobalClass.currentPlayedLevel >= TGlobalClass.UNLOCK_TORCH_AT)
                {
                    this.removedTileTorch = this.removedTileTorch + 1;
                }
                if (this.tileAr[col][row].type == 10)
                {
                    this._global._soundAsset.playSFX("heartburst");
                }
                if (this.fryerLayer.visible)
                {
                    this.fryerPercentage.text = AS3.toInt((double)this.removedTileCountFries / this.friesTarget * 100) + "%";
                }
                if (this.juicerLayer.visible)
                {
                    this.juicerPercentage.text = AS3.toInt((double)this.removedTileCountJuice / this.juiceTarget * 100) + "%";
                }
                this.checkCustOrderPrior(col, row);
                this.checkCustOrder(col, row);
                missingPieces = missingPieces + 1;
                this.tileAr[col][row].onTween = true;
                TweenLite.to(this.tileAr[col][row], 0.1f, new Vars
                {
                    ["scaleX"] = 1.2f,
                    ["scaleY"] = 1.2f,
                    ["ease"] = Linear.easeNone,
                    ["onComplete"] = (Action<int, int, int>)((int f1_arg1, int f1_arg2, int f1_arg3) =>
                    {
                        int column = f1_arg1;
                        int rows = f1_arg2;
                        int missPiece = f1_arg3;
                        TweenLite.to(this.tileAr[column][rows], 0.1f, new Vars
                        {
                            ["scaleX"] = 0,
                            ["scaleY"] = 0,
                            ["ease"] = Linear.easeNone,
                            ["onComplete"] = (Action<int, int, int>)this._onNewPieceShrunk,
                            ["onCompleteParams"] = new object[] { column, rows, missPiece },
                        });
                    }),
                    ["onCompleteParams"] = new object[] { col, row, missingPieces },
                });
            }
        }
        if (this.removedTileCountFries >= this.friesTarget)
        {
            this._global._soundAsset.playSFX("fries_machine_ready");
            this.removedTileCountFries = 0;
            this.fryerPercentage.text = "Drag to Customer";
            this.fryerLayer.visible = false;
            this.fries.visible = true;
        }
        if (this.removedTileCountJuice >= this.juiceTarget)
        {
            this._global._soundAsset.playSFX("beverage_machine_ready");
            this.removedTileCountJuice = 0;
            this.juicerPercentage.text = "Drag to Customer";
            this.juicerLayer.visible = false;
            this.drink.visible = true;
        }
        this.tileTypeAr.Clear();
        for (int col = 0; col < 8; col++)
        {
            for (int row = 0; row < 8; row++)
            {
                this.tileTypeAr.push(this.tileAr[col][row].type);
            }
        }
    }

    /// <summary>Inner onComplete closure of addNewPieces: respawns the shrunk tile above the board as a new random piece.</summary>
    void _onNewPieceShrunk(int f2_arg1, int f2_arg2, int f2_arg3)
    {
        Match3Object t = this.tileAr[f2_arg1][f2_arg2];
        t.removed = false;
        t.aktif = false;
        t.asMatch3Part = false;
        t.asMatch4Part = false;
        t.asMatch5Part = false;
        t.asMatchMorePart = false;
        t.onTween = false;
        t.frozen = false;
        t.used = false;
        t.y = 60 - 70 - 69 * f2_arg3;
        t.scaleX = 1;
        t.scaleY = 1;
        t.alpha = 1;
        t.isDropping = true;
        t.col = f2_arg1;
        t.row = f2_arg2;
        int rnd = this._global._maxCrypto.RandomRange(1, 100);
        if (rnd <= 25)
        {
            t.type = this.getWantedIngredient();
            if (t.type == 0)
            {
                t.type = this.availableTypeAr[this._global._maxCrypto.RandomRange(0, this.availableTypeAr.Count - 1)];
            }
        }
        else
        {
            t.type = this.availableTypeAr[this._global._maxCrypto.RandomRange(0, this.availableTypeAr.Count - 1)];
        }
        string tex = _tileTextureName(t.type);
        if (tex != null)
        {
            t.texture = this.gameAtlas.getTexture(tex);
        }
        t.readjustSize();
        t.pivotX = t.width / 2;
        t.pivotY = t.height / 2;
        if (TGlobalClass.currentPlayedLevel >= TGlobalClass.UNLOCK_FROZEN_AT)
        {
            if (this.currentFrozenTile < this.maxFrozenTile)
            {
                int rndFrozen = TGlobalClass.RandomRange(0, 100);
                if (rndFrozen < 5)
                {
                    t.frozen = true;
                    for (int f = 0; f < this.frozenTileAr.Count; f++)
                    {
                        if (!this.frozenTileAr[f].visible)
                        {
                            this.frozenTileAr[f].visible = true;
                            this.frozenTileAr[f].x = t.x;
                            this.frozenTileAr[f].y = t.y;
                            this.frozenTileAr[f].attachedCol = f2_arg1;
                            this.frozenTileAr[f].attachedRow = f2_arg2;
                            t.frozenTileIdx = this.frozenTileAr[f].idx;
                            break;
                        }
                    }
                    this.currentFrozenTile = this.currentFrozenTile + 1;
                }
            }
        }
        if (this.removedTileTorch >= this.torchTarget)
        {
            if (!t.frozen)
            {
                t.type = 15;
                t.texture = this.gameAtlas.getTexture("chef_torch");
                t.readjustSize();
                t.pivotX = t.width / 2;
                t.pivotY = t.height / 2;
                this.removedTileTorch = 0;
            }
        }
        this.moveTile = true;
    }

    public void tweenShake(FrozenTile arg1, int arg2 = 1, int arg3 = 0)
    {
        this._global._soundAsset.playSFX("ice_break");
        TimelineLite tl = new TimelineLite();
        arg1.x = this.tileAr[arg1.attachedCol][arg1.attachedRow].x;
        arg1.y = this.tileAr[arg1.attachedCol][arg1.attachedRow].y;
        int s = 6;
        tl.to(arg1, 0.1f, new Vars { ["x"] = arg1.x + s / 2f * arg2, ["y"] = arg1.y + s / 2f * arg3, ["ease"] = Linear.easeNone });
        tl.to(arg1, 0.1f, new Vars { ["x"] = arg1.x - s * arg2, ["y"] = arg1.y - s * arg3, ["ease"] = Linear.easeNone });
        tl.to(arg1, 0.1f, new Vars { ["x"] = arg1.x + s * arg2, ["y"] = arg1.y + s * arg3, ["ease"] = Linear.easeNone });
        tl.to(arg1, 0.1f, new Vars { ["x"] = arg1.x - s / 2f * arg2, ["y"] = arg1.y - s / 2f * arg3, ["ease"] = Linear.easeNone });
    }

    public int getWantedIngredient()
    {
        int result = 0;
        List<int> wanted = new List<int>();
        int focus = TGlobalClass.customerFocus;
        for (int c = 0; c < 3; c++)
        {
            if (!this.customer[c].onTween && !this.customer[c].completed && this.customer[c].queueIdx == focus)
            {
                for (int k = 0; k < this.customer[c].ingredientTarget.Count; k++)
                {
                    if (this.customer[c].ingredientTarget[k] > 0)
                    {
                        wanted.push(this.customer[c].ingredientType[k]);
                    }
                }
                break;
            }
        }
        if (wanted.Count > 0)
        {
            result = wanted[TGlobalClass.RandomRange(0, wanted.Count - 1)];
        }
        else
        {
            result = 0;
        }
        return result;
    }

    public void remakeJuice()
    {
        this.drink.visible = false;
        this.juicerLayer.visible = true;
        this.juicerPercentage.text = "0%";
    }

    public void remakeFries()
    {
        this.fries.visible = false;
        this.fryerLayer.visible = true;
        this.fryerPercentage.text = "0%";
    }

    public void popCookiesOnBoard()
    {
        int pc = 0;
        int pr = 0;
        if (this.cookiesCountOnBoard >= 5)
        {
            return;
        }
        List<int> cols = new List<int>();
        List<int> rows = new List<int>();
        for (int c = 0; c < 8; c++)
        {
            for (int r = 0; r < 8; r++)
            {
                if (!this.tileAr[c][r].removed && !this.tileAr[c][r].onTween && !this.tileAr[c][r].frozen)
                {
                    cols.push(c);
                    rows.push(r);
                }
            }
        }
        do
        {
            pc = cols[TGlobalClass.RandomRange(0, cols.Count - 1)];
            pr = rows[TGlobalClass.RandomRange(0, rows.Count - 1)];
        }
        while (this.tileAr[pc][pr].frozen || this.tileAr[pc][pr].type == 10 || this.tileAr[pc][pr].type == 15);
        this.tileAr[pc][pr].type = 10;
        this.tileAr[pc][pr].texture = this.gameAtlas.getTexture("cookies");
        this.tileAr[pc][pr].readjustSize();
        this.tileAr[pc][pr].pivotX = this.tileAr[pc][pr].width / 2;
        this.tileAr[pc][pr].pivotY = this.tileAr[pc][pr].height / 2;
        this.cookiesCountOnBoard = this.cookiesCountOnBoard + 1;
    }

    public void checkCustOrderPrior(int arg1, int arg2)
    {
        int countFulfilled = 0;
        int col = arg1;
        int row = arg2;
        int customerFocus = TGlobalClass.customerFocus;
        for (int i = 0; i < 3; i++)
        {
            if (!this.customer[i].onTween && !this.customer[i].completed && this.customer[i].queueIdx == customerFocus)
            {
                for (int j = 0; j < this.customer[i].ingredientType.Count; j++)
                {
                    if (this.tileAr[col][row].type == this.customer[i].ingredientType[j] && !this.tileAr[col][row].used)
                    {
                        if (this.customer[i].ingredientTarget[j] > 0)
                        {
                            this._takeIngredient(col, row, i, j);
                        }
                    }
                }
            }
        }
    }

    /// <summary>Shared body of checkCustOrderPrior/checkCustOrder: send the tile to the customer's order and pulse the counter.</summary>
    void _takeIngredient(int col, int row, int i, int j)
    {
        this.startTweenFromBoard(col, row, i, j);
        this.tileAr[col][row].used = true;
        int left = this.customer[i].ingredientTarget[j] - 1;
        this.customer[i].ingredientTarget[j] = left;
        this.customer[i].ingredientTargetPreview[j].text = "x" + left;
        TweenLite.to(this.customer[i].ingredientTargetPreview[j], 0.2f, new Vars
        {
            ["scaleX"] = 1.2f,
            ["scaleY"] = 1.2f,
            ["ease"] = Linear.easeNone,
            ["onComplete"] = (Action<int, int>)((int f1_arg1, int f1_arg2) =>
            {
                TweenLite.to(this.customer[f1_arg1].ingredientTargetPreview[f1_arg2], 0.2f, new Vars { ["scaleX"] = 1, ["scaleY"] = 1, ["ease"] = Linear.easeNone });
            }),
            ["onCompleteParams"] = new object[] { i, j },
        });
    }

    public void startTweenFromBoard(int arg1, int arg2, int arg3, int arg4)
    {
        List<string> textureName = null;
        int countFulfilled = 0;
        Point loc2Glob = null;
        int col = arg1;
        int row = arg2;
        int custIdx = arg3;
        int ingIdx = arg4;
        for (int i = 0; i < 25; i++)
        {
            if (this.tweenTileAr[i].onTween)
            {
                continue;
            }
            this.tweenTileAr[i].visible = true;
            this.tweenTileAr[i].x = this.tileAr[col][row].x;
            this.tweenTileAr[i].y = this.tileAr[col][row].y;
            this.tweenTileAr[i].onTween = true;
            textureName = new List<string> { "bread", "selada", "beef", "ayam", "nasi", "bacon", "salmon", "ikan", "keju", "cookies", "pita_bread", "lamb", "tomato" };
            this.tweenTileAr[i].texture = this.gameAtlas.getTexture(textureName[this.tileAr[col][row].type - 1]);
            this.tweenTileAr[i].readjustSize();
            this.tweenTileAr[i].pivotX = this.tweenTileAr[i].width / 2;
            this.tweenTileAr[i].pivotY = this.tweenTileAr[i].height / 2;
            countFulfilled = 0;
            cSprite ing = this.customer[custIdx].ingredientList[ingIdx];
            loc2Glob = ing.parent.localToGlobal(new Point(ing.x, ing.y));
            TweenLite.to(this.tweenTileAr[i], 0.5f + col * 0.05f, new Vars
            {
                ["x"] = loc2Glob.x,
                ["y"] = loc2Glob.y,
                ["ease"] = Linear.easeNone,
                ["onComplete"] = (Action<int, int, int>)((int f1_arg1, int f1_arg2, int f1_arg3) =>
                {
                    this.tweenTileAr[f1_arg1].visible = false;
                    this.tweenTileAr[f1_arg1].x = -150;
                    this.tweenTileAr[f1_arg1].y = -150;
                    this.tweenTileAr[f1_arg1].onTween = false;
                    TCustomer cust = this.customer[f1_arg2];
                    if (!cust.onTween && !cust.completed)
                    {
                        for (int k = 0; k < cust.ingredientType.Count; k++)
                        {
                            if (cust.ingredientTarget[k] <= 0)
                            {
                                countFulfilled = countFulfilled + 1;
                                cust.ingredientFulfilled[k] = true;
                                cust.ingredientCheckMark[k].visible = true;
                                cust.ingredientTargetPreview[k].visible = false;
                            }
                        }
                    }
                    if (countFulfilled == cust.ingredientType.Count && !cust.onTween && !cust.completed)
                    {
                        this._global._soundAsset.playSFX("money_drop_v" + TGlobalClass.RandomRange(1, 2));
                        cust.readyText.visible = true;
                        TweenLite.to(cust.readyText, 0.4f, new Vars { ["scaleX"] = 1, ["scaleY"] = 1, ["ease"] = Back.easeOut });
                        cust.tweenReadyText();
                        TweenLite.to(this.coinsAr[f1_arg2], 0.4f, new Vars { ["scaleX"] = 1, ["scaleY"] = 1, ["ease"] = Back.easeOut });
                        this.coinsAr[f1_arg2].visible = true;
                        this.coinsAr[f1_arg2].idx = f1_arg2;
                        this.dishesAr[f1_arg2].visible = true;
                        if (TGlobalClass.currentRegionDish == ConstClass.WESTERN_DISH)
                        {
                            this.dishesAr[f1_arg2].texture = this.gameAtlas.getTexture(this.westernDishName[cust.currentDish]);
                        }
                        else if (TGlobalClass.currentRegionDish == ConstClass.CHINESE_DISH)
                        {
                            this.dishesAr[f1_arg2].texture = this.gameAtlas.getTexture(this.chineseDishName[cust.currentDish - TGlobalClass.TOTAL_WESTERN_DISH]);
                        }
                        else if (TGlobalClass.currentRegionDish == ConstClass.MID_EAST_DISH)
                        {
                            this.dishesAr[f1_arg2].texture = this.gameAtlas.getTexture(this.mideastDishName[cust.currentDish - TGlobalClass.TOTAL_WESTERN_DISH - TGlobalClass.TOTAL_CHINESE_DISH]);
                        }
                        this.dishesAr[f1_arg2].readjustSize();
                        this.dishesAr[f1_arg2].alignPivot("center", "bottom");
                        cust.setDishCompleteState();
                        this.checkCustomerLeft();
                        this.setNextPriorCust();
                    }
                }),
                ["onCompleteParams"] = new object[] { i, custIdx, ingIdx },
            });
            break;
        }
    }

    public bool checkCustomerLeft()
    {
        int left = 0;
        for (int c = 0; c < 3; c++)
        {
            if (this.customer[c].completed && !this.coinsAr[c].visible)
            {
                left++;
            }
        }
        if (this.timesUp && left >= 3)
        {
            this.showWindow();
            return true;
        }
        return false;
    }

    public void showWindow()
    {
        if (this.calledShowWindow)
        {
            return;
        }
        this.calledShowWindow = true;
        if (TGlobalClass.isPlayingSpecialLevel)
        {
            if (TGlobalClass.VIPServedRating > 0)
            {
                this.VIPServeBonus = AS3.toInt((double)TGlobalClass.VIPServedRating * this._global._maxCrypto.STI(ScoreManager.totalScore) - this._global._maxCrypto.STI(ScoreManager.totalScore));
            }
            else
            {
                this.VIPServeBonus = 0;
            }
        }
        // (sic) string concatenation on the encrypted score string, as in the original
        ScoreManager.totalScore = ScoreManager.totalScore + this.VIPServeBonus;
        TweenLite.delayedCall(2.5f, (Action)(() =>
        {
            int earned = 0;
            int prevBigBusiness = 0;
            int notifCount = 0;
            this.calledShowWindow = false;
            TPlayer player = TSavegame.currentPlayer;
            int lvl = TGlobalClass.currentPlayedLevel;
            if (player.daysEarning[lvl] == 0 && TGlobalClass.currentPlayedLevel == player.passedDays)
            {
                if (this.servedCustomerCount >= this.goalTarget)
                {
                    player.daysEarning[lvl] = this._global._maxCrypto.STI(ScoreManager.totalScore);
                    player.passedDays = player.passedDays + 1;
                    earned = this._global._maxCrypto.STI(ScoreManager.totalScore);
                    if (!player.unlocked[Achievement.QUALIFIED_CHEF])
                    {
                        player.achievementCounter[Achievement.QUALIFIED_CHEF] = player.achievementCounter[Achievement.QUALIFIED_CHEF] + 1;
                        if (player.achievementCounter[Achievement.QUALIFIED_CHEF] >= Achievement.ALL_TARGET[Achievement.QUALIFIED_CHEF])
                        {
                            player.unlocked[Achievement.QUALIFIED_CHEF] = true;
                            Achievement.tweenAchievementNotif(Achievement.QUALIFIED_CHEF);
                            notifCount++;
                        }
                        else if (player.achievementCounter[Achievement.QUALIFIED_CHEF] % 15 == 0)
                        {
                            Achievement.tweenAchievementProgress(Achievement.QUALIFIED_CHEF);
                            notifCount++;
                        }
                    }
                    if (this._global._maxCrypto.STI(ScoreManager.totalScore) >= this.expertTarget)
                    {
                        if (!player.unlocked[Achievement.INTERNATIONAL_CHEF])
                        {
                            player.achievementCounter[Achievement.INTERNATIONAL_CHEF] = player.achievementCounter[Achievement.INTERNATIONAL_CHEF] + 1;
                            if (player.achievementCounter[Achievement.INTERNATIONAL_CHEF] >= Achievement.ALL_TARGET[Achievement.INTERNATIONAL_CHEF])
                            {
                                player.unlocked[Achievement.INTERNATIONAL_CHEF] = true;
                                TweenLite.delayedCall(notifCount * 6, (Action<int>)Achievement.tweenAchievementNotif, new object[] { Achievement.INTERNATIONAL_CHEF });
                            }
                            else if (player.achievementCounter[Achievement.INTERNATIONAL_CHEF] % 15 == 0)
                            {
                                TweenLite.delayedCall(notifCount * 6, (Action<int>)Achievement.tweenAchievementProgress, new object[] { Achievement.INTERNATIONAL_CHEF });
                            }
                        }
                        player.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(player.money) + 6);
                        TGlobalClass.coinsGot = 6;
                    }
                    else
                    {
                        player.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(player.money) + 3);
                        TGlobalClass.coinsGot = 3;
                    }
                    TSavegame.save();
                    if (player.passedDays == TGlobalClass.TOTAL_DAYS)
                    {
                        TGlobalClass._windowDisplay.showWindow(TWindowState.STORY_COMPLETE_ALL);
                    }
                    else
                    {
                        TGlobalClass._windowDisplay.showWindow(TWindowState.STORY_LEVELCOMPLETE);
                    }
                    TGlobalClass.isPlayingSpecialLevel = false;
                }
                else
                {
                    this.gameover = true;
                    TGlobalClass._windowDisplay.showWindow(TWindowState.STORY_LEVELFAILED);
                    TGlobalClass.isPlayingSpecialLevel = false;
                }
            }
            else if (this.servedCustomerCount >= this.goalTarget)
            {
                TGlobalClass.daysEarningPrev = player.daysEarning[lvl];
                if (player.daysEarning[lvl] < this._global._maxCrypto.STI(ScoreManager.totalScore))
                {
                    if (this._global._maxCrypto.STI(ScoreManager.totalScore) >= this.expertTarget && player.daysEarning[lvl] < this.expertTarget)
                    {
                        if (!player.unlocked[Achievement.INTERNATIONAL_CHEF])
                        {
                            player.achievementCounter[Achievement.INTERNATIONAL_CHEF] = player.achievementCounter[Achievement.INTERNATIONAL_CHEF] + 1;
                            if (player.achievementCounter[Achievement.INTERNATIONAL_CHEF] >= Achievement.ALL_TARGET[Achievement.INTERNATIONAL_CHEF])
                            {
                                player.unlocked[Achievement.INTERNATIONAL_CHEF] = true;
                                Achievement.tweenAchievementNotif(Achievement.INTERNATIONAL_CHEF);
                            }
                            else if (player.achievementCounter[Achievement.INTERNATIONAL_CHEF] % 15 == 0)
                            {
                                Achievement.tweenAchievementProgress(Achievement.INTERNATIONAL_CHEF);
                            }
                        }
                        player.money = this._global._maxCrypto.ITS(this._global._maxCrypto.STI(player.money) + 3);
                        TGlobalClass.coinsGot = 3;
                    }
                    earned = this._global._maxCrypto.STI(ScoreManager.totalScore) - player.daysEarning[lvl];
                    player.daysEarning[lvl] = this._global._maxCrypto.STI(ScoreManager.totalScore);
                    TSavegame.save();
                }
                TGlobalClass._windowDisplay.showWindow(TWindowState.STORY_LEVELCOMPLETE);
                TGlobalClass.isPlayingSpecialLevel = false;
            }
            else
            {
                this.gameover = true;
                TGlobalClass._windowDisplay.showWindow(TWindowState.STORY_LEVELFAILED);
                TGlobalClass.isPlayingSpecialLevel = false;
            }
            if (!player.unlocked[Achievement.BIG_BUSINESS])
            {
                prevBigBusiness = player.achievementCounter[Achievement.BIG_BUSINESS];
                player.achievementCounter[Achievement.BIG_BUSINESS] = player.achievementCounter[Achievement.BIG_BUSINESS] + earned;
                if (player.achievementCounter[Achievement.BIG_BUSINESS] >= Achievement.ALL_TARGET[Achievement.BIG_BUSINESS])
                {
                    player.unlocked[Achievement.BIG_BUSINESS] = true;
                    Achievement.tweenAchievementNotif(Achievement.BIG_BUSINESS);
                }
                else if (prevBigBusiness % 20000 > player.achievementCounter[Achievement.BIG_BUSINESS] % 20000)
                {
                    Achievement.tweenAchievementProgress(Achievement.BIG_BUSINESS);
                }
            }
            TSavegame.save();
        }));
    }

    public void autoCollect()
    {
        int juiceBonus = 0;
        int friesBonus = 0;
        for (int i = 0; i < 3; i++)
        {
            if (!this.coinsAr[i].visible)
            {
                continue;
            }
            TweenLite.to(this.coinsAr[i], 0.4f, new Vars { ["scaleX"] = 0, ["scaleY"] = 0, ["ease"] = Back.easeIn });
            this.earnedMoneyTextAr[i].visible = true;
            this.earnedMoneyTextAr[i].text = "$" + (this.customer[i].money + this.customer[i].tipsBonus);
            if (this.customer[i].hasJuice)
            {
                TGlobalClass.juiceServed = TGlobalClass.juiceServed + 1;
                juiceBonus = this._global.dish.getHardwareDefaultPrice(Dish.JUICER) + (TSavegame.currentPlayer.hardwareLevel[Dish.JUICER] - 1) * this._global.dish.getHardwarePriceIncrease(Dish.JUICER);
                this.earnedMoneyTextAr[i].text = "$" + (this.customer[i].money + this.customer[i].tipsBonus) + " + $" + juiceBonus + " Juice Bonus!";
            }
            if (this.customer[i].hasFries)
            {
                TGlobalClass.friesServed = TGlobalClass.friesServed + 1;
                friesBonus = this._global.dish.getHardwareDefaultPrice(Dish.FRYER) + (TSavegame.currentPlayer.hardwareLevel[Dish.FRYER] - 1) * this._global.dish.getHardwarePriceIncrease(Dish.FRYER);
                this.earnedMoneyTextAr[i].text = "$" + (this.customer[i].money + this.customer[i].tipsBonus) + " + $" + friesBonus + " Fries Bonus!";
            }
            if (this.customer[i].hasJuice && this.customer[i].hasFries)
            {
                this.earnedMoneyTextAr[i].text = "$" + (this.customer[i].money + this.customer[i].tipsBonus) + " + $" + (friesBonus + juiceBonus) + " Juice + Fries Bonus!";
            }
            ScoreManager.updateScore(this.customer[i].money + this.customer[i].tipsBonus + juiceBonus + friesBonus);
            if (this._global._maxCrypto.STI(ScoreManager.totalScore) >= this.goalTarget && this.goalExpertText.idx == 1)
            {
                TGlobalClass._windowDisplay.showGoalIconSmall();
                this.goalExpertText.idx = 2;
                this.targetInfoText.text = "$" + this.expertTarget;
                this.targetStateText.text = "Expert:";
                this._global._soundAsset.playSFX("goal_reached");
            }
            else if (this._global._maxCrypto.STI(ScoreManager.totalScore) >= this.expertTarget && this.goalExpertText.idx == 2)
            {
                this.goalExpertText.idx = 3;
                TGlobalClass._windowDisplay.showGoalIconSmall();
                this._global._soundAsset.playSFX("expert_reached");
            }
            TweenLite.to(this.earnedMoneyTextAr[i], 0.5f, new Vars
            {
                ["y"] = this.earnedMoneyTextAr[i].y - 65,
                ["onComplete"] = (Action<int>)((int f1_arg1) =>
                {
                    this.earnedMoneyTextAr[f1_arg1].x = this.coinsAr[f1_arg1].x;
                    this.earnedMoneyTextAr[f1_arg1].y = this.coinsAr[f1_arg1].y;
                    this.earnedMoneyTextAr[f1_arg1].visible = false;
                }),
                ["onCompleteParams"] = new object[] { i },
            });
            this.tweenDishAlpha(i, this.customer[i].CUST_SATISFIED);
        }
    }

    public void tweenDishAlpha(int arg1, int arg2)
    {
        int patienceLeft = 0;
        int custID = arg1;
        int custState = arg2;
        this.customer[custID].onTween = true;
        this.coinsAr[custID].visible = false;
        if (this.customer[custID].asVIP)
        {
            if (custState == this.customer[custID].CUST_SATISFIED)
            {
                patienceLeft = AS3.toInt((double)this.customer[custID].patience / this.customer[custID].totalPatience * 100);
                if (patienceLeft > 75)
                {
                    TGlobalClass.VIPServedRating = 1.5f;
                }
                else
                {
                    TGlobalClass.VIPServedRating = 1.25f;
                }
            }
            else if (custState == this.customer[custID].CUST_LEAVE)
            {
                TGlobalClass.VIPServedRating = 0;
            }
        }
        TweenLite.to(this.customer[custID].fries, 0.3f, new Vars { ["alpha"] = 0 });
        TweenLite.to(this.customer[custID].juice, 0.3f, new Vars { ["alpha"] = 0 });
        TweenLite.to(this.dishesAr[custID], 0.3f, new Vars
        {
            ["alpha"] = 0,
            ["ease"] = Linear.easeNone,
            ["onComplete"] = (Action<int>)((int f1_arg1) =>
            {
                this.dishesAr[f1_arg1].visible = false;
                this.dishesAr[f1_arg1].alpha = 1;
                this.customer[f1_arg1].fries.visible = false;
                this.customer[f1_arg1].juice.visible = false;
                this.customer[f1_arg1].fries.alpha = 1;
                this.customer[f1_arg1].juice.alpha = 1;
            }),
            ["onCompleteParams"] = new object[] { custID },
        });
        int onQueueRemove = this.customerOnQueue.indexOf(this.customer[custID].customerAvaID);
        this.customerOnQueue.splice(onQueueRemove, 1);
        this.customer[custID].tweenOut(custState, !this._global.tutorial.blockCustomerIn);
    }

    public void checkCustOrder(int arg1, int arg2)
    {
        int countFulfilled = 0;
        int col = arg1;
        int row = arg2;
        if (this.tileAr[col][row].used)
        {
            return;
        }
        List<int> tempVec = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            tempVec.push(this.customer[i].queueIdx);
        }
        tempVec.Sort();
        for (int k = 0; k < 3; k++)
        {
            for (int i = 0; i < 3; i++)
            {
                countFulfilled = 0;
                if (this.customer[i].queueIdx != tempVec[k])
                {
                    continue;
                }
                if (!this.customer[i].onTween && !this.customer[i].completed)
                {
                    for (int j = 0; j < this.customer[i].ingredientType.Count; j++)
                    {
                        if (this.tileAr[col][row].type == this.customer[i].ingredientType[j] && !this.tileAr[col][row].used)
                        {
                            if (this.customer[i].ingredientTarget[j] > 0)
                            {
                                this._takeIngredient(col, row, i, j);
                            }
                        }
                    }
                }
            }
        }
    }

    public void setNextPriorCust()
    {
        int c = 0;
        for (c = 0; c < 3; c++)
        {
            if (this.customer[c].queueIdx == TGlobalClass.customerFocus && !this.customer[c].completed)
            {
                return;
            }
        }
        List<int> waiting = new List<int>();
        for (c = 0; c < 3; c++)
        {
            if (!this.coinsAr[c].visible)
            {
                waiting.push(this.customer[c].queueIdx);
            }
        }
        if (this.firstCust)
        {
            this.firstCust = false;
        }
        if (waiting.Count <= 0)
        {
            for (c = 0; c < 3; c++)
            {
                this.customer[c].hidePriority();
            }
            return;
        }
        waiting.Sort();
        TGlobalClass.customerFocus = waiting[0];
        for (c = 0; c < 3; c++)
        {
            if (this.customer[c].queueIdx == TGlobalClass.customerFocus)
            {
                if (!this.customer[c].queueNumber.visible)
                {
                    this.customer[c].showPriority();
                }
            }
            else
            {
                this.customer[c].hidePriority();
            }
        }
    }

    public List<List<int>> lookForMatches()
    {
        List<List<int>> result = new List<List<int>>();
        List<int> horiz = new List<int>();
        List<int> vert = new List<int>();
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 6; col++)
            {
                Match3Object t = this.tileAr[col][row];
                if (!t.isDropping && !t.isSwapping && !t.onTween && !t.removed)
                {
                    horiz = this.getMatchHoriz(col, row);
                    if (horiz.Count > 2)
                    {
                        result.push(horiz);
                        col = col + (horiz.Count - 1);
                    }
                }
            }
        }
        for (int col = 0; col < 8; col++)
        {
            for (int row = 0; row < 6; row++)
            {
                Match3Object t = this.tileAr[col][row];
                if (!t.isDropping && !t.isSwapping && !t.onTween && !t.removed)
                {
                    vert = this.getMatchVert(col, row);
                    if (vert.Count > 2)
                    {
                        result.push(vert);
                        row = row + (vert.Count - 1);
                    }
                }
            }
        }
        return result;
    }

    public List<int> getMatchHoriz(int arg1, int arg2)
    {
        List<int> result = new List<int>();
        Match3Object t = this.tileAr[arg1][arg2];
        if (!t.isDropping && !t.onTween && !t.removed && !t.isSwapping)
        {
            result.push(t.idx);
        }
        for (int k = 1; arg1 + k < 8; k++)
        {
            if (!(!(t.type == 10) && !(t.type == 15)))
            {
                continue;
            }
            Match3Object n = this.tileAr[arg1 + k][arg2];
            if (t.type != n.type)
            {
                return result;
            }
            if (!n.isDropping && !n.onTween && !n.removed && !n.isSwapping)
            {
                result.push(n.idx);
            }
            else
            {
                return result;
            }
        }
        return result;
    }

    public List<int> getMatchVert(int arg1, int arg2)
    {
        List<int> result = new List<int>();
        Match3Object t = this.tileAr[arg1][arg2];
        if (!t.isDropping && !t.onTween && !t.removed && !t.isSwapping)
        {
            result.push(t.idx);
        }
        for (int k = 1; arg2 + k < 8; k++)
        {
            if (!(!(t.type == 10) && !(t.type == 15)))
            {
                continue;
            }
            Match3Object n = this.tileAr[arg1][arg2 + k];
            if (t.type != n.type)
            {
                return result;
            }
            if (!n.isDropping && !n.onTween && !n.removed && !n.isSwapping)
            {
                result.push(n.idx);
            }
            else
            {
                return result;
            }
        }
        return result;
    }

    // ---- possible-move patterns (AS3 nested array literals): offsets of the required same-type tile,
    //      candidate tiles that would complete the line, and the tile each candidate swaps with
    static List<List<int>> P(params int[][] a)
    {
        var l = new List<List<int>>();
        foreach (var x in a) l.Add(new List<int>(x));
        return l;
    }
    static readonly List<List<int>> PAT1_NEED = P(new[] { 1, 0 });
    static readonly List<List<int>> PAT1_MATCH = P(new[] { -2, 0 }, new[] { -1, -1 }, new[] { -1, 1 }, new[] { 2, -1 }, new[] { 2, 1 }, new[] { 3, 0 });
    static readonly List<List<int>> PAT1_SWAP = P(new[] { -1, 0 }, new[] { -1, 0 }, new[] { -1, 0 }, new[] { 2, 0 }, new[] { 2, 0 }, new[] { 2, 0 });
    static readonly List<List<int>> PAT2_NEED = P(new[] { 2, 0 });
    static readonly List<List<int>> PAT2_MATCH = P(new[] { 1, -1 }, new[] { 1, 1 });
    static readonly List<List<int>> PAT2_SWAP = P(new[] { 1, 0 }, new[] { 1, 0 });
    static readonly List<List<int>> PAT3_NEED = P(new[] { 0, 1 });
    static readonly List<List<int>> PAT3_MATCH = P(new[] { 0, -2 }, new[] { -1, -1 }, new[] { 1, -1 }, new[] { -1, 2 }, new[] { 1, 2 }, new[] { 0, 3 });
    static readonly List<List<int>> PAT3_SWAP = P(new[] { 0, -1 }, new[] { 0, -1 }, new[] { 0, -1 }, new[] { 0, 2 }, new[] { 0, 2 }, new[] { 0, 2 });
    static readonly List<List<int>> PAT4_NEED = P(new[] { 0, 2 });
    static readonly List<List<int>> PAT4_MATCH = P(new[] { -1, 1 }, new[] { 1, 1 });
    static readonly List<List<int>> PAT4_SWAP = P(new[] { 0, 1 }, new[] { 0, 1 });

    public bool lookForPossibles()
    {
        for (int col = 0; col < 8; col++)
        {
            for (int row = 0; row < 8; row++)
            {
                if (this.matchPattern(col, row, PAT1_NEED, PAT1_MATCH, PAT1_SWAP)) return true;
                if (this.matchPattern(col, row, PAT2_NEED, PAT2_MATCH, PAT2_SWAP)) return true;
                if (this.matchPattern(col, row, PAT3_NEED, PAT3_MATCH, PAT3_SWAP)) return true;
                if (this.matchPattern(col, row, PAT4_NEED, PAT4_MATCH, PAT4_SWAP)) return true;
                if (this.tileAr[col][row].type == 15) return true;
            }
        }
        return false;
    }

    public bool matchPattern(int arg1, int arg2, List<List<int>> arg3, List<List<int>> arg4, List<List<int>> arg5)
    {
        int type = this.tileAr[arg1][arg2].type;
        if (type == 10 && type == 15)
        {
            // (sic) never true in the original either
            return false;
        }
        for (int k = 0; k < arg3.Count; k++)
        {
            if (!this.matchType(arg1 + arg3[k][0], arg2 + arg3[k][1], type))
            {
                return false;
            }
        }
        for (int k = 0; k < arg4.Count; k++)
        {
            if (this.matchType(arg1 + arg4[k][0], arg2 + arg4[k][1], type) && !this.tileAr[arg1 + arg4[k][0]][arg2 + arg4[k][1]].frozen)
            {
                if (!this.tileAr[arg1 + arg5[k][0]][arg2 + arg5[k][1]].frozen)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool matchType(int arg1, int arg2, int arg3)
    {
        if (arg1 < 0 || arg1 > 7 || arg2 < 0 || arg2 > 7)
        {
            return false;
        }
        return this.tileAr[arg1][arg2].type == arg3;
    }

    public void highlightPossible()
    {
        for (int col = 0; col < 8; col++)
        {
            for (int row = 0; row < 8; row++)
            {
                if (this._global.tutorial.currentDialogueID == Tutorial.LEVEL_1 && !(this.matchType(col, row, 1) || this.matchType(col, row, 2)))
                {
                    continue;
                }
                if (this.matchPattern(col, row, PAT1_NEED, PAT1_MATCH, PAT1_SWAP))
                {
                    this.highlight(col, row, PAT1_NEED, PAT1_MATCH, PAT1_SWAP);
                    return;
                }
                if (this.matchPattern(col, row, PAT2_NEED, PAT2_MATCH, PAT2_SWAP))
                {
                    this.highlight(col, row, PAT2_NEED, PAT2_MATCH, PAT2_SWAP);
                    return;
                }
                if (this.matchPattern(col, row, PAT3_NEED, PAT3_MATCH, PAT3_SWAP))
                {
                    this.highlight(col, row, PAT3_NEED, PAT3_MATCH, PAT3_SWAP);
                    return;
                }
                if (this.matchPattern(col, row, PAT4_NEED, PAT4_MATCH, PAT4_SWAP))
                {
                    this.highlight(col, row, PAT4_NEED, PAT4_MATCH, PAT4_SWAP);
                    return;
                }
            }
        }
    }

    public void highlight(int arg1, int arg2, List<List<int>> arg3, List<List<int>> arg4, List<List<int>> arg5)
    {
        int type = this.tileAr[arg1][arg2].type;
        this.tweenHighlighted(this.tileAr[arg1][arg2].col, this.tileAr[arg1][arg2].row);
        for (int k = 0; k < arg3.Count; k++)
        {
            if (this.matchType(arg1 + arg3[k][0], arg2 + arg3[k][1], type))
            {
                this.tweenHighlighted(this.tileAr[arg1 + arg3[k][0]][arg2 + arg3[k][1]].col, this.tileAr[arg1 + arg3[k][0]][arg2 + arg3[k][1]].row);
            }
        }
        for (int k = 0; k < arg4.Count; k++)
        {
            if (this.matchType(arg1 + arg4[k][0], arg2 + arg4[k][1], type))
            {
                if (!this.tileAr[arg1 + arg5[k][0]][arg2 + arg5[k][1]].frozen)
                {
                    this.tweenHighlighted(this.tileAr[arg1 + arg4[k][0]][arg2 + arg4[k][1]].col, this.tileAr[arg1 + arg4[k][0]][arg2 + arg4[k][1]].row);
                    return;
                }
            }
        }
    }

    public void tweenHighlighted(int arg1, int arg2)
    {
        int col = arg1;
        int row = arg2;
        if (this.dispatchHighlightPosition > 0)
        {
            this.dispatchHighlightPosition = this.dispatchHighlightPosition - 1;
            this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "highlight", true, false, new Dictionary<string, object> { ["row"] = row, ["col"] = col }));
        }
        TweenLite.to(this.tileAr[col][row], 0.4f, new Vars
        {
            ["scaleX"] = 1.2f,
            ["scaleY"] = 1.2f,
            ["ease"] = Linear.easeNone,
            ["onComplete"] = (Action)(() =>
            {
                TweenLite.to(this.tileAr[col][row], 0.4f, new Vars
                {
                    ["scaleX"] = 1,
                    ["scaleY"] = 1,
                    ["ease"] = Linear.easeNone,
                    ["onComplete"] = (Action<int, int>)this.tweenHighlighted,
                    ["onCompleteParams"] = new object[] { col, row },
                });
            }),
        });
    }

    public void checkDishPriceAndAvaiblity(int arg1)
    {
        int i = 0;
        int k = 0;
        int price = 0;          // (sic) accumulated over all dishes, never reset
        int extra = 0;
        int count = 0;
        List<int> ingredients = null;
        Global g = null;
        List<int> westernPrices = null;
        List<int> chinesePrices = null;
        List<int> mideastPrices = null;
        int minPrice = 0;
        int minIdx = 0;
        if (TGlobalClass.currentRegionDish == ConstClass.WESTERN_DISH && TGlobalClass.dishRegionWestern.Count == arg1)
        {
            return;
        }
        if (TGlobalClass.currentRegionDish == ConstClass.CHINESE_DISH && TGlobalClass.dishRegionChinese.Count == arg1)
        {
            return;
        }
        if (TGlobalClass.currentRegionDish == ConstClass.MID_EAST_DISH && TGlobalClass.dishRegionMidEast.Count == arg1)
        {
            return;
        }
        ingredients = new List<int>();
        g = Global.getInstance();
        westernPrices = new List<int>();
        chinesePrices = new List<int>();
        mideastPrices = new List<int>();
        TGlobalClass.dishRegionChinese.Clear();
        TGlobalClass.dishRegionWestern.Clear();
        TGlobalClass.dishRegionMidEast.Clear();
        TPlayer p = TSavegame.currentPlayer;
        if (TGlobalClass.currentRegionDish == ConstClass.WESTERN_DISH)
        {
            for (i = 0; i < p.westernDishLevel.Count; i++)
            {
                if (p.westernDishLevel[i] > 0)
                {
                    ingredients = g.dish.getDishIngredientType(i);
                    for (k = 0; k < ingredients.Count; k++)
                    {
                        price = price + (p.ingredientLevel[ingredients[k] - 1] - 1) * g.dish.getIngredientPriceIncrease(ingredients[k] - 1);
                    }
                    price = price + (g.dish.getDishDefaultPrice(i) + (p.westernDishLevel[i] - 1) * g.dish.getPriceIncrease(i));
                    westernPrices.push(price);
                    TGlobalClass.dishRegionWestern.push(i);
                }
            }
            while (westernPrices.Count > arg1)
            {
                // (sic) minIdx is not reset for the western region
                minPrice = 1000;
                for (i = 0; i < westernPrices.Count; i++)
                {
                    if (minPrice > westernPrices[i])
                    {
                        minPrice = westernPrices[i];
                        minIdx = i;
                    }
                }
                westernPrices.splice(minIdx, 1);
                TGlobalClass.dishRegionWestern.splice(minIdx, 1);
            }
            count = TGlobalClass.dishRegionWestern.Count;
            for (i = 0; i < count; i++)
            {
                if (p.westernDishLevel[TGlobalClass.dishRegionWestern[i]] > 1)
                {
                    for (extra = p.westernDishLevel[TGlobalClass.dishRegionWestern[i]] - 1; extra > 0; extra--)
                    {
                        TGlobalClass.dishRegionWestern.push(TGlobalClass.dishRegionWestern[i]);
                    }
                }
            }
        }
        else if (TGlobalClass.currentRegionDish == ConstClass.CHINESE_DISH)
        {
            for (i = 0; i < p.chineseDishLevel.Count; i++)
            {
                if (p.chineseDishLevel[i] > 0)
                {
                    ingredients = g.dish.getDishIngredientType(i + TGlobalClass.TOTAL_WESTERN_DISH);
                    for (k = 0; k < ingredients.Count; k++)
                    {
                        price = price + (p.ingredientLevel[ingredients[k] - 1] - 1) * g.dish.getIngredientPriceIncrease(ingredients[k] - 1);
                    }
                    price = price + (g.dish.getDishDefaultPrice(TGlobalClass.TOTAL_WESTERN_DISH + i) + (p.chineseDishLevel[i] - 1) * g.dish.getPriceIncrease(TGlobalClass.TOTAL_WESTERN_DISH + i));
                    chinesePrices.push(price);
                    TGlobalClass.dishRegionChinese.push(TGlobalClass.TOTAL_WESTERN_DISH + i);
                }
            }
            while (chinesePrices.Count > arg1)
            {
                minPrice = 1000;
                minIdx = 0;
                for (i = 0; i < chinesePrices.Count; i++)
                {
                    if (minPrice > chinesePrices[i])
                    {
                        minPrice = chinesePrices[i];
                        minIdx = i;
                    }
                }
                chinesePrices.splice(minIdx, 1);
                TGlobalClass.dishRegionChinese.splice(minIdx, 1);
            }
            count = TGlobalClass.dishRegionChinese.Count;
            for (i = 0; i < count; i++)
            {
                if (p.chineseDishLevel[TGlobalClass.dishRegionChinese[i] - TGlobalClass.TOTAL_WESTERN_DISH] > 1)
                {
                    for (extra = p.chineseDishLevel[TGlobalClass.dishRegionChinese[i] - TGlobalClass.TOTAL_WESTERN_DISH] - 1; extra > 0; extra--)
                    {
                        TGlobalClass.dishRegionChinese.push(TGlobalClass.dishRegionChinese[i]);
                    }
                }
            }
        }
        else if (TGlobalClass.currentRegionDish == ConstClass.MID_EAST_DISH)
        {
            for (i = 0; i < p.mideastDishLevel.Count; i++)
            {
                if (p.mideastDishLevel[i] > 0)
                {
                    ingredients = g.dish.getDishIngredientType(i + TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH);
                    for (k = 0; k < ingredients.Count; k++)
                    {
                        price = price + (p.ingredientLevel[ingredients[k] - 1] - 1) * g.dish.getIngredientPriceIncrease(ingredients[k] - 1);
                    }
                    price = price + (g.dish.getDishDefaultPrice(TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH + i) + (p.mideastDishLevel[i] - 1) * g.dish.getPriceIncrease(TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH + i));
                    mideastPrices.push(price);
                    TGlobalClass.dishRegionMidEast.push(TGlobalClass.TOTAL_WESTERN_DISH + TGlobalClass.TOTAL_CHINESE_DISH + i);
                }
            }
            while (mideastPrices.Count > arg1)
            {
                minPrice = 1000;
                minIdx = 0;
                for (i = 0; i < mideastPrices.Count; i++)
                {
                    if (minPrice > mideastPrices[i])
                    {
                        minPrice = mideastPrices[i];
                        minIdx = i;
                    }
                }
                mideastPrices.splice(minIdx, 1);
                TGlobalClass.dishRegionMidEast.splice(minIdx, 1);
            }
            count = TGlobalClass.dishRegionMidEast.Count;
            for (i = 0; i < count; i++)
            {
                if (p.mideastDishLevel[TGlobalClass.dishRegionMidEast[i] - TGlobalClass.TOTAL_WESTERN_DISH - TGlobalClass.TOTAL_CHINESE_DISH] > 1)
                {
                    for (extra = p.mideastDishLevel[TGlobalClass.dishRegionMidEast[i] - TGlobalClass.TOTAL_WESTERN_DISH - TGlobalClass.TOTAL_CHINESE_DISH] - 1; extra > 0; extra--)
                    {
                        TGlobalClass.dishRegionMidEast.push(TGlobalClass.dishRegionMidEast[i]);
                    }
                }
            }
        }
    }

    public void showGoodMovesText(int arg1)
    {
        if (arg1 == 4)
        {
            this.largeText.text = "Good!";
        }
        else if (arg1 == 5)
        {
            this.largeText.text = "Nice!";
        }
        else if (arg1 == 6)
        {
            this.largeText.text = "Great!";
        }
        else if (arg1 == 7)
        {
            this.largeText.text = "Awesome!";
        }
        else if (arg1 >= 8)
        {
            this.largeText.text = "WOW!";
        }
        if (TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_MATCH_BONUS && arg1 > 4)
        {
            this.largeText.text = this.largeText.text + ("\n+ $" + (arg1 - 4));
        }
        if (TSavegame.currentPlayer.passedDays >= TGlobalClass.UNLOCK_COOKIES_AT && arg1 > 4)
        {
            this.popCookiesOnBoard();
        }
        TweenLite.killTweensOf(this.largeText);
        TweenLite.killDelayedCallsTo((Action)this.hideGoodMovesText);
        this.largeText.visible = true;
        this.largeText.alpha = 0.5f;
        this.largeText.scaleY = 0.8f;
        this.largeText.scaleX = 0.8f;
        this.largeText.y = 250;
        TweenLite.to(this.largeText, 1, new Vars { ["alpha"] = 1, ["scaleX"] = 1, ["scaleY"] = 1, ["y"] = 300, ["ease"] = Expo.easeOut });
        TweenLite.delayedCall(2, (Action)this.hideGoodMovesText);
    }

    public void hideGoodMovesText()
    {
        if (this.largeText != null)
        {
            this.largeText.visible = false;
        }
    }

    public void touchButton(Event arg1)
    {
        int id = (arg1.currentTarget as TNonStateButton).idx;
        if (id == this.PAUSE_BUTTON)
        {
            if (this.calledShowWindow)
            {
                return;
            }
            this.paused = true;
            TGlobalClass._windowDisplay.showWindow(TWindowState.PAUSE_GAME);
        }
        else if (id == this.SOUND_BUTTON)
        {
            this._global.MuteSound = !this._global.MuteSound;
            this.mute_icon.visible = !this.mute_icon.visible;
        }
        else if (id == this.MUSIC_BUTTON)
        {
            this._global.MuteMusic = !this._global.MuteMusic;
            this.mute_icon_music.visible = !this.mute_icon_music.visible;
        }
    }

    public void pauseGame()
    {
        if (!TGlobalClass._windowDisplay._blackScreen.visible)
        {
            if (this.calledShowWindow)
            {
                return;
            }
            this.paused = true;
            TGlobalClass._windowDisplay.showWindow(TWindowState.PAUSE_GAME);
        }
    }

    public void resumeGame()
    {
        if (!TGlobalClass._windowDisplay._blackScreen.visible)
        {
            this.paused = false;
        }
    }

    /// <summary>
    /// Starling reads a mutated clipRect every frame; Starlite only applies it in the setter,
    /// so "sprite.clipRect.x = v" is done by re-assigning the rectangle.
    /// </summary>
    static void SetClipRectX(Sprite s, float x)
    {
        Rectangle r = s.clipRect;
        r.x = x;
        s.clipRect = r;
    }
}
