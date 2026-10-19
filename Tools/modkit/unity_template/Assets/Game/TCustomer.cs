// Ported from TCustomer.as (decompiled).
using System;
using System.Collections.Generic;
using Starlite;

public class TCustomer : Sprite
{
    public const int MAIN_CHAR = 0;
    public const int LITTLE_BOY = 1;
    public const int ACTOR = 2;
    public const int BUSINESS_WOMAN = 3;
    public const int BUSINESS_MAN = 4;
    public const int GRANDMA = 5;
    public const int HIPHOP_MAN = 6;
    public const int LITTLE_GIRL = 7;
    public const int GANGSTER = 8;
    public const int RICH_WOMAN = 9;
    public const int RICH_MAN = 10;
    public const int NURSE = 11;
    public const int SPORTSMAN = 12;
    public const int CRITICS = 13;
    public int idx;
    public cSprite heartBG;
    public cSprite heart;
    public cSprite patienceBar;
    public Sprite patienceSprite;
    public cSprite patienceGlass;
    public cSprite customerAva;
    public float intervalForDecreaseMoney;
    public int customerAvaID;
    public List<cSprite> ingredientList;
    public List<ExtendTextField> ingredientTargetPreview;
    public List<cSprite> ingredientCheckMark;
    public List<int> ingredientType;
    public List<int> ingredientTarget;
    public Quad clickQuad;
    public cSprite juice;
    public cSprite fries;
    public cSprite queueNumber;
    public ExtendTextField moneyDisplay;
    public ExtendTextField readyText;
    public List<bool> ingredientFulfilled;
    public bool onTween;
    public int queueIdx;
    public int currentDish;
    public int totalPatience;
    public int patience;
    public int money;
    public int totalMoney;
    public int tipsBonus;
    public bool completed;
    public int worth;
    public bool hasJuice;
    public bool hasFries;
    public bool tweeningPatience;
    public Quad displayQuad;
    public cSprite queueNumberTransparan;
    public bool asVIP;
    // AS3 instance consts (GameScreen reads them as customer[i].CUST_SATISFIED): readonly fields.
    public readonly int CUST_SATISFIED = 1;
    public readonly int CUST_LEAVE = 2;

    public TCustomer()
    {
        int i;
        this.ingredientList = AS3Extensions.filled<cSprite>(5, k => null);
        this.ingredientTargetPreview = AS3Extensions.filled<ExtendTextField>(5, k => null);
        this.ingredientCheckMark = AS3Extensions.filled<cSprite>(5, k => null);
        this.ingredientType = new List<int>();
        this.ingredientTarget = new List<int>();
        this.ingredientFulfilled = new List<bool>();
        Global g = Global.getInstance();
        TextureAtlas atlas = g.gameScreen.gameAtlas;
        this.displayQuad = new Quad(150, 50);
        this.displayQuad.alignPivot();
        this.displayQuad.x = 150;
        this.displayQuad.y = 45;
        this.displayQuad.alpha = 0;
        this.displayQuad.touchable = false;
        this.addChild(this.displayQuad);
        this.customerAva = new cSprite(atlas.getTexture("char_1"));
        this.customerAva.pivotX = this.customerAva.width;
        this.customerAva.pivotY = this.customerAva.height;
        this.customerAva.x = 70;
        this.customerAva.y = 100;
        this.customerAva.touchable = false;
        this.addChild(this.customerAva);
        this.heartBG = new cSprite(atlas.getTexture("cust_bar_BG"));
        this.heartBG.x = 95;
        this.heartBG.y = 68;
        this.heartBG.touchable = false;
        this.addChild(this.heartBG);
        this.heart = new cSprite(atlas.getTexture("heart_cust"));
        this.heart.pivotX = this.heart.width / 2;
        this.heart.pivotY = this.heart.height / 2;
        this.heart.x = this.heartBG.x + 19;
        this.heart.y = this.heartBG.y + 17;
        this.heart.touchable = false;
        this.addChild(this.heart);
        this.patienceBar = new cSprite(atlas.getTexture("cust_fill"));
        this.patienceSprite = new Sprite();
        this.addChild(this.patienceSprite);
        this.patienceSprite.addChild(this.patienceBar);
        this.patienceSprite.x = this.heartBG.x + 36;
        this.patienceSprite.y = this.heartBG.y + 11;
        this.patienceSprite.clipRect = new Rectangle(0, 0, this.patienceBar.width, this.patienceBar.height);
        this.patienceGlass = new cSprite(atlas.getTexture("cust_glass"));
        this.patienceGlass.x = this.patienceSprite.x;
        this.patienceGlass.y = this.patienceSprite.y;
        this.addChild(this.patienceGlass);
        for (i = 0; i < 5; i++)
        {
            this.ingredientList[i] = new cSprite(atlas.getTexture("keju"));
            this.ingredientList[i].pivotX = this.ingredientList[i].width / 2;
            this.ingredientList[i].pivotY = this.ingredientList[i].height / 2;
            this.ingredientList[i].scaleX = 0.8f;
            this.ingredientList[i].scaleY = 0.8f;
            this.ingredientList[i].x = i * 51 + 95;
            this.ingredientList[i].y = 40;
            this.ingredientList[i].touchable = false;
            this.addChild(this.ingredientList[i]);
            this.ingredientTargetPreview[i] = new ExtendTextField(50, 40, "x99", "KomikandyWhite", 20, 16777215);
            this.ingredientTargetPreview[i].x = this.ingredientList[i].x - 10;
            this.ingredientTargetPreview[i].y = this.ingredientList[i].y - 5;
            this.ingredientTargetPreview[i].touchable = false;
            this.addChild(this.ingredientTargetPreview[i]);
            this.ingredientCheckMark[i] = new cSprite(atlas.getTexture("check"));
            this.ingredientCheckMark[i].x = this.ingredientList[i].x - 10;
            this.ingredientCheckMark[i].y = this.ingredientList[i].y - 5;
            this.ingredientCheckMark[i].touchable = false;
            this.ingredientCheckMark[i].visible = false;
            this.addChild(this.ingredientCheckMark[i]);
        }
        this.juice = new cSprite(atlas.getTexture("juice"));
        this.juice.x = 330;
        this.juice.y = 47;
        this.juice.visible = false;
        this.juice.touchable = false;
        this.addChild(this.juice);
        this.fries = new cSprite(atlas.getTexture("fries"));
        this.fries.x = 380;
        this.fries.y = 42;
        this.fries.visible = false;
        this.fries.touchable = false;
        this.addChild(this.fries);
        this.queueNumberTransparan = new cSprite(atlas.getTexture("priority"));
        this.queueNumberTransparan.pivotX = this.queueNumberTransparan.width / 2;
        this.queueNumberTransparan.pivotY = this.queueNumberTransparan.height / 2;
        this.queueNumberTransparan.x = 70;
        this.queueNumberTransparan.y = 80;
        this.queueNumberTransparan.alpha = 0.5f;
        this.queueNumberTransparan.touchable = false;
        this.queueNumberTransparan.visible = false;
        this.addChild(this.queueNumberTransparan);
        this.queueNumber = new cSprite(atlas.getTexture("priority"));
        this.queueNumber.pivotX = this.queueNumber.width / 2;
        this.queueNumber.pivotY = this.queueNumber.height / 2;
        this.queueNumber.x = 70;
        this.queueNumber.y = 80;
        this.queueNumber.touchable = false;
        this.queueNumber.visible = false;
        this.addChild(this.queueNumber);
        this.moneyDisplay = new ExtendTextField(100, 50, "$", "KomikandyGold", 25, 16777215);
        this.moneyDisplay.x = 323;
        this.moneyDisplay.y = 65;
        this.moneyDisplay.touchable = false;
        this.addChild(this.moneyDisplay);
        this.readyText = new ExtendTextField(150, 100, "READY!", "KomikandyGold", 32, 16777215);
        this.readyText.pivotX = 75;
        this.readyText.pivotY = 50;
        this.readyText.x = 265;
        this.readyText.y = 60;
        this.readyText.scaleY = 0;
        this.readyText.scaleX = 0;
        this.readyText.visible = false;
        this.readyText.touchable = false;
        this.addChild(this.readyText);
        this.clickQuad = new Quad(this.width, this.height);
        this.clickQuad.alignPivot();
        this.clickQuad.x = this.clickQuad.x + this.clickQuad.pivotX;
        this.clickQuad.y = this.clickQuad.y + this.clickQuad.pivotY;
        this.clickQuad.alpha = 0;
        this.clickQuad.addEventListener(TouchEvent.TOUCH, this.touchCust);
        this.addChild(this.clickQuad);
        this.x = -this.width;
        this.completed = true;
        this.useHandCursor = true;
        g.dish.getDishIngredientType(0);
    }

    public void touchCust(Event arg1)
    {
        var e = (TouchEvent)arg1;
        if (e.getTouch(this, TouchPhase.BEGAN) != null) this.selectPriorCust();
        if (e.getTouch(this, TouchPhase.HOVER) != null)
            this.queueNumberTransparan.visible = true;
        else
            this.queueNumberTransparan.visible = false;
    }

    public void selectPriorCust()
    {
        Global g = Global.getInstance();
        if (TGlobalClass.customerFocus == this.queueIdx) return;
        g._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "changed_priority", true));
        for (int i = 0; i < 3; i++)
        {
            if (!(g.gameScreen.customer[i].queueIdx == this.queueIdx) && g.gameScreen.customer[i].queueNumber.visible)
                g.gameScreen.customer[i].hidePriority();
        }
        TGlobalClass.customerFocus = this.queueIdx;
        this.showPriority();
    }

    public void dePatience()
    {
        double _local1 = 0.5 * this.totalMoney;
        double _local2 = (double)this.patience / this.totalPatience;
        if (this.patience > 0)
            this.patience = this.patience - 1;
        else
            Global.getInstance().gameScreen.tweenDishAlpha(this.idx, this.CUST_LEAVE);
        // Starling reads clipRect each frame; Starlite needs the rectangle re-assigned to apply the change.
        Rectangle clip = this.patienceSprite.clipRect;
        clip.x = (float)(_local2 * clip.width - clip.width);
        this.patienceSprite.clipRect = clip;
        if (this.patience < 0.8 * this.totalPatience && !this.tweeningPatience)
        {
            this.money = (int)(_local1 + _local1 * _local2);
            this.moneyDisplay.text = "$" + this.money;
        }
    }

    public void tweenCustHeart()
    {
        TweenLite.to(this.heart, 0.5f, new Vars
        {
            ["scaleX"] = 1.2f,
            ["scaleY"] = 1.2f,
            ["delay"] = this.patience * 1 / 6000f,
            ["ease"] = Linear.easeNone,
            ["onComplete"] = (Action)(() =>
            {
                TweenLite.to(this.heart, 0.5f, new Vars
                {
                    ["scaleX"] = 1, ["scaleY"] = 1, ["ease"] = Linear.easeNone,
                    ["onComplete"] = (Action)this.tweenCustHeart
                });
            })
        });
    }

    public void newCust(int arg1)
    {
        int k;
        string texName = null;
        int dishID = 0;
        int pick;
        bool alreadyQueued;
        int custID = 0;
        TweenLite.killTweensOf(this.heart);
        this.heart.scaleX = 1;
        this.heart.scaleY = 1;
        this.hasFries = false;
        this.hasJuice = false;
        int _local2 = this.queueIdx;
        Global g = Global.getInstance();
        TextureAtlas atlas = g.gameScreen.gameAtlas;
        while (true)
        {
            if (TGlobalClass.tempCustUse.Count == g.gameScreen.customerOnQueue.Count) return;
            alreadyQueued = false;
            pick = g._maxCrypto.RandomRange(0, TGlobalClass.tempCustUse.Count - 1);
            custID = TGlobalClass.tempCustUse[pick];
            for (k = 0; k < g.gameScreen.customerOnQueue.Count; k++)
            {
                if (custID == g.gameScreen.customerOnQueue[k])
                {
                    alreadyQueued = true;
                    break;
                }
            }
            if (alreadyQueued) continue;
            if (TGlobalClass.tempCustUse.Count > 3) TGlobalClass.tempCustUse.splice(pick, 1);
            break;
        }
        this.asVIP = false;
        if (TGlobalClass.isPlayingSpecialLevel && arg1 == 6)
        {
            custID = 13;
            this.asVIP = true;
        }
        if (custID == 13)
            g.gameScreen.checkDishPriceAndAvaiblity(2);
        else
            g.gameScreen.checkDishPriceAndAvaiblity(4);
        if (TGlobalClass.currentRegionDish == ConstClass.CHINESE_DISH)
        {
            if (TGlobalClass.isPlayingSpecialLevel && custID == 13)
            {
                if (TGlobalClass.dishRegionChinese.Count > 1)
                    dishID = TGlobalClass.dishRegionChinese[g._maxCrypto.RandomRange(0, 1)];
                else
                    dishID = TGlobalClass.dishRegionChinese[0];
            }
            else
            {
                dishID = TGlobalClass.dishRegionChinese[g._maxCrypto.RandomRange(0, TGlobalClass.dishRegionChinese.Count - 1)];
            }
        }
        else if (TGlobalClass.currentRegionDish == ConstClass.WESTERN_DISH)
        {
            if (TGlobalClass.isPlayingSpecialLevel && custID == 13)
            {
                if (TGlobalClass.dishRegionWestern.Count > 1)
                    dishID = TGlobalClass.dishRegionWestern[g._maxCrypto.RandomRange(0, 1)];
                else
                    dishID = TGlobalClass.dishRegionWestern[0];
            }
            else
            {
                dishID = TGlobalClass.dishRegionWestern[g._maxCrypto.RandomRange(0, TGlobalClass.dishRegionWestern.Count - 1)];
            }
        }
        else if (TGlobalClass.currentRegionDish == ConstClass.MID_EAST_DISH)
        {
            if (TGlobalClass.isPlayingSpecialLevel && custID == 13)
            {
                if (TGlobalClass.dishRegionMidEast.Count > 1)
                    dishID = TGlobalClass.dishRegionMidEast[g._maxCrypto.RandomRange(0, 1)];
                else
                    dishID = TGlobalClass.dishRegionMidEast[0];
            }
            else
            {
                dishID = TGlobalClass.dishRegionMidEast[g._maxCrypto.RandomRange(0, TGlobalClass.dishRegionMidEast.Count - 1)];
            }
        }
        this.currentDish = dishID;
        for (k = 0; k < 5; k++)
        {
            this.ingredientList[k].visible = false;
            this.ingredientTargetPreview[k].visible = false;
            this.ingredientCheckMark[k].visible = false;
        }
        g.gameScreen.customerOnQueue.push(custID);
        this.customerAva.texture = atlas.getTexture("char_" + custID);
        this.customerAvaID = custID;
        this.queueIdx = arg1;
        this.totalPatience = (int)(1800 * xmlNumber(TGlobalClass.customerXML["customer"][custID - 1].attribute("patience")));
        this.patience = this.totalPatience;
        if (g.tutorial.currentDialogueID == Tutorial.LEVEL_31 && g.tutorial.dialogueCounter == 0 && this.idx == 0)
            this.patience = (int)(this.patience / 2f);
        this.completed = false;
        this.customerAva.readjustSize();
        this.customerAva.pivotX = this.customerAva.width;
        this.customerAva.pivotY = this.customerAva.height;
        if (TGlobalClass.tempCustUse.Count <= 0)
            TGlobalClass.tempCustUse = g.gameScreen.customerInUse.slice(0, TSavegame.currentPlayer.lastCustIdx + 1);
        this.ingredientType.Clear();
        this.ingredientTarget.Clear();
        this.ingredientFulfilled.Clear();
        this.ingredientType = g.dish.getDishIngredientType(dishID);
        this.ingredientTarget = g.dish.getDishTarget(dishID);
        if (TGlobalClass.isPlayingSpecialLevel && custID == 13)
        {
            for (k = 0; k < this.ingredientTarget.Count; k++)
                this.ingredientTarget[k] = this.ingredientTarget[k] * 2;
        }
        if (TGlobalClass.currentRegionDish == ConstClass.WESTERN_DISH)
        {
            this.money = g.dish.getDishDefaultPrice(dishID) + (TSavegame.currentPlayer.westernDishLevel[dishID] - 1) * g.dish.getPriceIncrease(dishID);
        }
        else if (TGlobalClass.currentRegionDish == ConstClass.CHINESE_DISH)
        {
            this.money = g.dish.getDishDefaultPrice(dishID) + (TSavegame.currentPlayer.chineseDishLevel[dishID - TGlobalClass.TOTAL_WESTERN_DISH] - 1) * g.dish.getPriceIncrease(dishID);
        }
        else if (TGlobalClass.currentRegionDish == ConstClass.MID_EAST_DISH)
        {
            // NB: the original reads chineseDishLevel here (not mideastDishLevel); kept as-is.
            this.money = g.dish.getDishDefaultPrice(dishID) + (TSavegame.currentPlayer.chineseDishLevel[dishID - TGlobalClass.TOTAL_WESTERN_DISH - TGlobalClass.TOTAL_CHINESE_DISH] - 1) * g.dish.getPriceIncrease(dishID);
        }
        if (custID == 13) this.money = (int)(this.money * 1.5);
        // NB: uses the previous customer's totalMoney, as in the original.
        this.tipsBonus = (int)(g._maxCrypto.RandomRange(1, (int)(xmlNumber(TGlobalClass.customerXML["customer"][custID - 1].attribute("tipsBonus")) - 1)) * 0.1 * this.totalMoney);
        for (k = 0; k < this.ingredientType.Count; k++)
        {
            this.ingredientFulfilled.push(false);
            this.money = this.money + (TSavegame.currentPlayer.ingredientLevel[this.ingredientType[k] - 1] - 1) * g.dish.getIngredientPriceIncrease(this.ingredientType[k] - 1);
            switch (this.ingredientType[k])
            {
                case 1: texName = "bread"; break;
                case 2: texName = "selada"; break;
                case 3: texName = "beef"; break;
                case 4: texName = "ayam"; break;
                case 5: texName = "nasi"; break;
                case 6: texName = "bacon"; break;
                case 7: texName = "salmon"; break;
                case 8: texName = "ikan"; break;
                case 9: texName = "keju"; break;
                case 11: texName = "pita_bread"; break;
                case 12: texName = "lamb"; break;
                case 13: texName = "tomato"; break;
            }
            this.ingredientList[k].texture = g.gameScreen.gameAtlas.getTexture(texName);
            this.ingredientList[k].readjustSize();
            this.ingredientList[k].pivotX = this.ingredientList[k].width / 2;
            this.ingredientList[k].pivotY = this.ingredientList[k].height / 2;
            this.ingredientTargetPreview[k].text = "x" + this.ingredientTarget[k];
            this.ingredientList[k].visible = true;
            this.ingredientTargetPreview[k].visible = true;
        }
        this.totalMoney = this.money + this.tipsBonus;
        this.moneyDisplay.text = "$" + this.totalMoney;
        g.gameScreen.setNextPriorCust();
        this.tweenCustHeart();
    }

    /// <summary>AS3 Number(xmlAttr) in double precision (AS3.toNumber parses as float, which can truncate wrongly after scaling).</summary>
    static double xmlNumber(XMLList x)
    {
        string s = (x != null ? x.ToString() : "").Trim();
        if (s.Length == 0) return 0;
        return double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : double.NaN;
    }

    public void showPriority()
    {
        TweenLite.killTweensOf(this.queueNumber);
        this.queueNumber.visible = true;
        this.queueNumber.scaleY = 0;
        this.queueNumber.scaleX = 0;
        TweenLite.to(this.queueNumber, 0.5f, new Vars { ["scaleX"] = 1, ["scaleY"] = 1, ["ease"] = Back.easeOut });
    }

    public void hidePriority()
    {
        TweenLite.killTweensOf(this.queueNumber);
        this.queueNumber.visible = false;
    }

    public void setDishCompleteState()
    {
        Global.getInstance()._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "dish_completed"));
        this.completed = true;
        for (int i = 0; i < 5; i++)
        {
            this.ingredientCheckMark[i].visible = false;
            this.ingredientList[i].visible = false;
            this.ingredientTargetPreview[i].visible = false;
        }
    }

    public void tweenOut(int arg1, bool arg2 = true)
    {
        Global _global = null;
        bool nextCust = false;
        int customerState = arg1;
        bool nextCustomer = arg2;
        this.onTween = true;
        this.completed = true;
        _global = Global.getInstance();
        for (int i = 0; i < 5; i++)
        {
            this.ingredientList[i].visible = false;
            this.ingredientTargetPreview[i].visible = false;
            this.ingredientCheckMark[i].visible = false;
        }
        this.heart.visible = false;
        this.heartBG.visible = false;
        this.patienceGlass.visible = false;
        this.patienceSprite.visible = false;
        this.patienceBar.visible = false;
        this.queueNumber.visible = false;
        this.moneyDisplay.visible = false;
        TweenLite.killTweensOf(this.readyText);
        this.readyText.visible = false;
        if (!_global.gameScreen.timesUp)
        {
            TGlobalClass.currentCust = TGlobalClass.currentCust + 1;
            if (nextCustomer) nextCust = true;
        }
        else if (_global.gameScreen.timesUp)
        {
            nextCust = false;
            _global.gameScreen.checkCustomerLeft();
        }
        else
        {
            nextCust = false;
        }
        TweenLite.to(this, 1, new Vars
        {
            ["x"] = -this.width,
            ["ease"] = Linear.easeNone,
            ["onComplete"] = (Action<int, int, bool>)((int f1_arg1, int f1_arg2, bool f1_arg3) =>
            {
                if (f1_arg1 == this.CUST_SATISFIED)
                {
                    if (!TSavegame.currentPlayer.unlocked[Achievement.SERVICE_EXELLENCE] && (double)this.patience / this.totalPatience >= 0.8)
                    {
                        TSavegame.currentPlayer.achievementCounter[Achievement.SERVICE_EXELLENCE] = TSavegame.currentPlayer.achievementCounter[Achievement.SERVICE_EXELLENCE] + 1;
                        if (TSavegame.currentPlayer.achievementCounter[Achievement.SERVICE_EXELLENCE] >= Achievement.ALL_TARGET[Achievement.SERVICE_EXELLENCE])
                        {
                            TSavegame.currentPlayer.unlocked[Achievement.SERVICE_EXELLENCE] = true;
                            Achievement.tweenAchievementNotif(Achievement.SERVICE_EXELLENCE);
                        }
                        else if (TSavegame.currentPlayer.achievementCounter[Achievement.SERVICE_EXELLENCE] % 200 == 0)
                        {
                            Achievement.tweenAchievementProgress(Achievement.SERVICE_EXELLENCE);
                        }
                    }
                }
                else if (f1_arg1 == this.CUST_LEAVE)
                {
                    GameScreen gs = Global.getInstance().gameScreen;
                    gs.missedCustomerCount = gs.missedCustomerCount + 1;
                }
                if (f1_arg3)
                {
                    this.newCust(f1_arg2);
                    for (int j = 0; j < 5; j++)
                    {
                        this.ingredientList[j].visible = false;
                        this.ingredientTargetPreview[j].visible = false;
                        this.ingredientCheckMark[j].visible = false;
                    }
                    this.heart.visible = false;
                    this.heartBG.visible = false;
                    this.patienceGlass.visible = false;
                    this.patienceSprite.visible = false;
                    this.patienceBar.visible = false;
                    this.moneyDisplay.visible = false;
                    TweenLite.killTweensOf(this.readyText);
                    this.readyText.visible = false;
                    if (!_global.tutorial.blockCustomerIn) this.tweenIn();
                }
            }),
            ["onCompleteParams"] = new object[] { customerState, TGlobalClass.currentCust, nextCust }
        });
    }

    public void tweenReadyText()
    {
        TweenLite.to(this.readyText, 0.5f, new Vars
        {
            ["scaleX"] = 1.1f,
            ["scaleY"] = 1.1f,
            ["delay"] = 0.4f,
            ["onComplete"] = (Action)(() =>
            {
                TweenLite.to(this.readyText, 0.5f, new Vars { ["scaleX"] = 1, ["scaleY"] = 1, ["onComplete"] = (Action)this.tweenReadyText });
            })
        });
    }

    public void tweenIn()
    {
        int i = 0;
        if (Global.getInstance().gameScreen.checkCustomerLeft()) return;
        Global.getInstance()._soundAsset.playSFX("bell_ding-ding");
        // AS3: ingredientTarget[0] on an empty vector is undefined, and undefined != 0.
        if (this.ingredientTarget.Count > 0 && this.ingredientTarget[0] == 0)
        {
            this.ingredientTarget = Global.getInstance().dish.getDishTarget(TGlobalClass.dishRegionWestern[0]);
            for (i = 0; i < this.ingredientType.Count; i++)
                this.ingredientTargetPreview[i].text = "x" + this.ingredientTarget[i];
        }
        this.onTween = true;
        TweenLite.to(this, 1, new Vars
        {
            ["x"] = 0,
            ["ease"] = Linear.easeNone,
            ["onComplete"] = (Action<int>)((int f1_arg1) =>
            {
                if (!Global.getInstance().gameScreen.ready) return;
                Global.getInstance()._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "customer_in", true, false,
                    new Dictionary<string, object> { ["position"] = f1_arg1 }));
                this.onTween = false;
                for (int j = 0; j < this.ingredientType.Count; j++)
                {
                    this.ingredientList[j].visible = true;
                    this.ingredientTargetPreview[j].visible = true;
                }
                this.heart.visible = true;
                this.heartBG.visible = true;
                this.patienceGlass.visible = true;
                this.patienceSprite.visible = true;
                this.patienceBar.visible = true;
                this.moneyDisplay.visible = true;
                if (this.customerAvaID == 13)
                {
                    Global.getInstance()._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "special_customer_in", true, false,
                        new Dictionary<string, object> { ["position"] = f1_arg1 }));
                }
            }),
            ["onCompleteParams"] = new object[] { this.idx }
        });
    }

    public void Kill()
    {
        TweenLite.killTweensOf(this.readyText);
        base.dispose();
        for (int i = 0; i < 5; i++)
        {
            this.ingredientList[i].removeFromParent(true);
            this.ingredientList[i] = null;
            this.ingredientTargetPreview[i].removeFromParent(true);
            this.ingredientTargetPreview[i] = null;
        }
        this.customerAva.removeFromParent(true);
        this.customerAva = null;
        this.heartBG.removeFromParent(true);
        this.heartBG = null;
        TweenLite.killTweensOf(this.heart);
        this.heart.removeFromParent(true);
        this.heart = null;
        this.patienceBar.removeFromParent(true);
        this.patienceBar = null;
        this.patienceSprite.removeFromParent(true);
        this.patienceSprite = null;
        this.patienceGlass.removeFromParent(true);
        this.patienceGlass = null;
        this.moneyDisplay.removeFromParent(true);
        this.moneyDisplay = null;
        this.readyText.removeFromParent(true);
        this.readyText = null;
        TweenLite.killTweensOf(this.juice);
        this.juice.removeFromParent(true);
        this.juice = null;
        TweenLite.killTweensOf(this.fries);
        this.fries.removeFromParent(true);
        this.fries = null;
        this.queueNumberTransparan.removeFromParent(true);
        this.queueNumberTransparan = null;
    }
}
