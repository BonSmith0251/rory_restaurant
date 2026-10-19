package {
class TCustomer extends Sprite {
    static const MAIN_CHAR:int = 0;
    static const LITTLE_BOY:int = 1;
    static const ACTOR:int = 2;
    static const BUSINESS_WOMAN:int = 3;
    static const BUSINESS_MAN:int = 4;
    static const GRANDMA:int = 5;
    static const HIPHOP_MAN:int = 6;
    static const LITTLE_GIRL:int = 7;
    static const GANGSTER:int = 8;
    static const RICH_WOMAN:int = 9;
    static const RICH_MAN:int = 10;
    static const NURSE:int = 11;
    static const SPORTSMAN:int = 12;
    static const CRITICS:int = 13;
    var idx:int;
    var heartBG:cSprite;
    var heart:cSprite;
    var patienceBar:cSprite;
    var patienceSprite:Sprite;
    var patienceGlass:cSprite;
    var customerAva:cSprite;
    var intervalForDecreaseMoney:Number;
    var customerAvaID:int;
    var ingredientList:Vector.<cSprite>;
    var ingredientTargetPreview:Vector.<ExtendTextField>;
    var ingredientCheckMark:Vector.<cSprite>;
    var ingredientType:Vector.<int>;
    var ingredientTarget:Vector.<int>;
    var clickQuad:Quad;
    var juice:cSprite;
    var fries:cSprite;
    var queueNumber:cSprite;
    var moneyDisplay:ExtendTextField;
    var readyText:ExtendTextField;
    var ingredientFulfilled:Vector.<Boolean>;
    var onTween:Boolean;
    var queueIdx:int;
    var currentDish:int;
    var totalPatience:int;
    var patience:int;
    var money:int;
    var totalMoney:int;
    var tipsBonus:int;
    var completed:Boolean;
    var worth:int;
    var hasJuice:Boolean;
    var hasFries:Boolean;
    var tweeningPatience:Boolean;
    var displayQuad:Quad;
    var queueNumberTransparan:cSprite;
    var asVIP:Boolean;
    const CUST_SATISFIED:int = 1;
    const CUST_LEAVE:int = 2;

    // static initializer
    static {
        MAIN_CHAR = 0;
        LITTLE_BOY = 1;
        ACTOR = 2;
        BUSINESS_WOMAN = 3;
        BUSINESS_MAN = 4;
        GRANDMA = 5;
        HIPHOP_MAN = 6;
        LITTLE_GIRL = 7;
        GANGSTER = 8;
        RICH_WOMAN = 9;
        RICH_MAN = 10;
        NURSE = 11;
        SPORTSMAN = 12;
        CRITICS = 13;
    }

    function TCustomer():* {
        var _local1 = 0;
        this.ingredientList = new Vector.<cSprite>(5, true);
        this.ingredientTargetPreview = new Vector.<ExtendTextField>(5, true);
        this.ingredientCheckMark = new Vector.<cSprite>(5, true);
        this.ingredientType = new Vector.<int>();
        this.ingredientTarget = new Vector.<int>();
        this.ingredientFulfilled = new Vector.<Boolean>();
        super();
        var _local2 = Global.getInstance();
        var _local3 = _local2.gameScreen.gameAtlas;
        this.displayQuad = new Quad(150, 50);
        this.displayQuad.alignPivot();
        this.displayQuad.x = 150;
        this.displayQuad.y = 45;
        this.displayQuad.alpha = 0;
        this.displayQuad.touchable = false;
        this.addChild(this.displayQuad);
        this.customerAva = new cSprite(_local3.getTexture("char_1"));
        this.customerAva.pivotX = this.customerAva.width;
        this.customerAva.pivotY = this.customerAva.height;
        this.customerAva.x = 70;
        this.customerAva.y = 100;
        this.customerAva.touchable = false;
        this.addChild(this.customerAva);
        this.heartBG = new cSprite(_local3.getTexture("cust_bar_BG"));
        this.heartBG.x = 95;
        this.heartBG.y = 68;
        this.heartBG.touchable = false;
        this.addChild(this.heartBG);
        this.heart = new cSprite(_local3.getTexture("heart_cust"));
        this.heart.pivotX = this.heart.width / 2;
        this.heart.pivotY = this.heart.height / 2;
        this.heart.x = this.heartBG.x + 19;
        this.heart.y = this.heartBG.y + 17;
        this.heart.touchable = false;
        this.addChild(this.heart);
        this.patienceBar = new cSprite(_local3.getTexture("cust_fill"));
        this.patienceSprite = new Sprite();
        this.addChild(this.patienceSprite);
        this.patienceSprite.addChild(this.patienceBar);
        this.patienceSprite.x = this.heartBG.x + 36;
        this.patienceSprite.y = this.heartBG.y + 11;
        this.patienceSprite.clipRect = new Rectangle(0, 0, this.patienceBar.width, this.patienceBar.height);
        this.patienceGlass = new cSprite(_local3.getTexture("cust_glass"));
        this.patienceGlass.x = this.patienceSprite.x;
        this.patienceGlass.y = this.patienceSprite.y;
        this.addChild(this.patienceGlass);
        _local1 = 0;
        goto L981;
      L609:
        this.ingredientList[_local1] = new cSprite(_local3.getTexture("keju"));
        this.ingredientList[_local1].pivotX = this.ingredientList[_local1].width / 2;
        this.ingredientList[_local1].pivotY = this.ingredientList[_local1].height / 2;
        this.ingredientList[_local1].scaleX = 0.8;
        this.ingredientList[_local1].scaleY = 0.8;
        this.ingredientList[_local1].x = _local1 * 51 + 95;
        this.ingredientList[_local1].y = 40;
        this.ingredientList[_local1].touchable = false;
        this.addChild(this.ingredientList[_local1]);
        this.ingredientTargetPreview[_local1] = new ExtendTextField(50, 40, "x99", "KomikandyWhite", 20, 16777215);
        this.ingredientTargetPreview[_local1].x = this.ingredientList[_local1].x - 10;
        this.ingredientTargetPreview[_local1].y = this.ingredientList[_local1].y - 5;
        this.ingredientTargetPreview[_local1].touchable = false;
        this.addChild(this.ingredientTargetPreview[_local1]);
        this.ingredientCheckMark[_local1] = new cSprite(_local3.getTexture("check"));
        this.ingredientCheckMark[_local1].x = this.ingredientList[_local1].x - 10;
        this.ingredientCheckMark[_local1].y = this.ingredientList[_local1].y - 5;
        this.ingredientCheckMark[_local1].touchable = false;
        this.ingredientCheckMark[_local1].visible = false;
        this.addChild(this.ingredientCheckMark[_local1]);
        _local1++;
      L981:
        if (_local1 < 5) goto L609;
        this.juice = new cSprite(_local3.getTexture("juice"));
        this.juice.x = 330;
        this.juice.y = 47;
        this.juice.visible = false;
        this.juice.touchable = false;
        this.addChild(this.juice);
        this.fries = new cSprite(_local3.getTexture("fries"));
        this.fries.x = 380;
        this.fries.y = 42;
        this.fries.visible = false;
        this.fries.touchable = false;
        this.addChild(this.fries);
        this.queueNumberTransparan = new cSprite(_local3.getTexture("priority"));
        this.queueNumberTransparan.pivotX = this.queueNumberTransparan.width / 2;
        this.queueNumberTransparan.pivotY = this.queueNumberTransparan.height / 2;
        this.queueNumberTransparan.x = 70;
        this.queueNumberTransparan.y = 80;
        this.queueNumberTransparan.alpha = 0.5;
        this.queueNumberTransparan.touchable = false;
        this.queueNumberTransparan.visible = false;
        this.addChild(this.queueNumberTransparan);
        this.queueNumber = new cSprite(_local3.getTexture("priority"));
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
        var _local4 = 0;
        this.readyText.scaleY = 0;
        this.readyText.scaleX = _local4;
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
        _local2.dish.getDishIngredientType(0);
    }

    function touchCust(arg1:TouchEvent):void {
        if (!arg1.getTouch(this, TouchPhase.BEGAN)) goto L23;
        this.selectPriorCust();
      L23:
        if (!arg1.getTouch(this, TouchPhase.HOVER)) goto L51;
        this.queueNumberTransparan.visible = true;
        goto L59;
      L51:
        this.queueNumberTransparan.visible = false;
      L59:
    }

    function selectPriorCust():void {
        var _local2 = 0;
        var _local1 = Global.getInstance();
        if (TGlobalClass.customerFocus == this.queueIdx) goto L149;
        _local1._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "changed_priority", true));
        _local2 = 0;
        goto L127;
      L61:
        var $t0 = !(_local1.gameScreen.customer[_local2].queueIdx == this.queueIdx);
        $s0 = $t0;
        if (!$t0) goto L106;
        $s0 = Boolean(_local1.gameScreen.customer[_local2].queueNumber.visible);
      L106:
        if (!$s0) goto L125;
        _local1.gameScreen.customer[_local2].hidePriority();
      L125:
        _local2++;
      L127:
        if (_local2 < 3) goto L61;
        TGlobalClass.customerFocus = this.queueIdx;
        this.showPriority();
      L149:
    }

    function dePatience():void {
        var _local1 = Number(0.5 * this.totalMoney);
        var _local2 = Number(this.patience / this.totalPatience);
        if (!(this.patience > 0)) goto L55;
        var _local3 = this;
        var _local4 = this.patience - 1;
        _local3.patience = _local4;
        goto L77;
      L55:
        Global.getInstance().gameScreen.tweenDishAlpha(this.idx, this.CUST_LEAVE);
      L77:
        this.patienceSprite.clipRect.x = _local2 * this.patienceSprite.clipRect.width - this.patienceSprite.clipRect.width;
        var $t0 = this.patience < 0.8 * this.totalPatience;
        $s0 = $t0;
        if (!$t0) goto L133;
        $s0 = !this.tweeningPatience;
      L133:
        if (!$s0) goto L161;
        this.money = _local1 + _local1 * _local2;
        this.moneyDisplay.text = "$" + this.money;
      L161:
    }

    function tweenCustHeart():void {
        TweenLite.to(this.heart, 0.5, {"scaleX": 1.2, "scaleY": 1.2, "delay": this.patience * 1 / 6000, "ease": Linear.easeNone, "onComplete": function():void {
            TweenLite.to(heart, 0.5, {"scaleX": 1, "scaleY": 1, "ease": Linear.easeNone, "onComplete": tweenCustHeart});
        }});
    }

    function newCust(arg1:int):void {
        var _local3 = 0;
        var _local5 = null;
        var _local6 = 0;
        var _local8 = 0;
        var _local9 = false;
        var _local10 = 0;
        TweenLite.killTweensOf(this.heart);
        this.heart.scaleX = 1;
        this.heart.scaleY = 1;
        this.hasFries = false;
        this.hasJuice = false;
        var _local2 = int(this.queueIdx);
        var $t0 = Global.getInstance();
        var _local4 = $t0;
        var _local7 = $t0.gameScreen.gameAtlas;
        goto L256;
      L95:
        if (TGlobalClass.tempCustUse.length != _local4.gameScreen.customerOnQueue.length) goto L121;
        return;
      L121:
        _local9 = false;
        _local8 = int(_local4._maxCrypto.RandomRange(0, TGlobalClass.tempCustUse.length - 1));
        _local10 = int(TGlobalClass.tempCustUse[_local8]);
        _local3 = 0;
        goto L197;
      L169:
        if (_local10 != _local4.gameScreen.customerOnQueue[_local3]) goto L195;
        _local9 = true;
        goto L213;
      L195:
        _local3++;
      L197:
        if (_local3 < _local4.gameScreen.customerOnQueue.length) goto L169;
      L213:
        if (!_local9) goto L223;
        goto L256;
      L223:
        if (!(TGlobalClass.tempCustUse.length > 3)) goto L252;
        TGlobalClass.tempCustUse.splice(_local8, 1);
      L252:
        goto L260;
      L256:
        goto L95;
      L260:
        this.asVIP = false;
        var $t1 = Boolean(TGlobalClass.isPlayingSpecialLevel);
        $s0 = $t1;
        if (!$t1) goto L282;
        $s0 = arg1 == 6;
      L282:
        if (!$s0) goto L295;
        _local10 = 13;
        this.asVIP = true;
      L295:
        if (_local10 != 13) goto L318;
        _local4.gameScreen.checkDishPriceAndAvaiblity(2);
        goto L329;
      L318:
        _local4.gameScreen.checkDishPriceAndAvaiblity(4);
      L329:
        if (TGlobalClass.currentRegionDish != ConstClass.CHINESE_DISH) goto L466;
        var $t2 = Boolean(TGlobalClass.isPlayingSpecialLevel);
        $s0 = $t2;
        if (!$t2) goto L363;
        $s0 = _local10 == 13;
      L363:
        if (!$s0) goto L429;
        if (!(TGlobalClass.dishRegionChinese.length > 1)) goto L411;
        _local6 = int(TGlobalClass.dishRegionChinese[_local4._maxCrypto.RandomRange(0, 1)]);
        goto L425;
      L411:
        _local6 = int(TGlobalClass.dishRegionChinese[0]);
      L425:
        goto L462;
      L429:
        _local6 = int(TGlobalClass.dishRegionChinese[_local4._maxCrypto.RandomRange(0, TGlobalClass.dishRegionChinese.length - 1)]);
      L462:
        goto L736;
      L466:
        if (TGlobalClass.currentRegionDish != ConstClass.WESTERN_DISH) goto L603;
        var $t3 = Boolean(TGlobalClass.isPlayingSpecialLevel);
        $s0 = $t3;
        if (!$t3) goto L500;
        $s0 = _local10 == 13;
      L500:
        if (!$s0) goto L566;
        if (!(TGlobalClass.dishRegionWestern.length > 1)) goto L548;
        _local6 = int(TGlobalClass.dishRegionWestern[_local4._maxCrypto.RandomRange(0, 1)]);
        goto L562;
      L548:
        _local6 = int(TGlobalClass.dishRegionWestern[0]);
      L562:
        goto L599;
      L566:
        _local6 = int(TGlobalClass.dishRegionWestern[_local4._maxCrypto.RandomRange(0, TGlobalClass.dishRegionWestern.length - 1)]);
      L599:
        goto L736;
      L603:
        if (TGlobalClass.currentRegionDish != ConstClass.MID_EAST_DISH) goto L736;
        var $t4 = Boolean(TGlobalClass.isPlayingSpecialLevel);
        $s0 = $t4;
        if (!$t4) goto L637;
        $s0 = _local10 == 13;
      L637:
        if (!$s0) goto L703;
        if (!(TGlobalClass.dishRegionMidEast.length > 1)) goto L685;
        _local6 = int(TGlobalClass.dishRegionMidEast[_local4._maxCrypto.RandomRange(0, 1)]);
        goto L699;
      L685:
        _local6 = int(TGlobalClass.dishRegionMidEast[0]);
      L699:
        goto L736;
      L703:
        _local6 = int(TGlobalClass.dishRegionMidEast[_local4._maxCrypto.RandomRange(0, TGlobalClass.dishRegionMidEast.length - 1)]);
      L736:
        this.currentDish = _local6;
        _local3 = 0;
        goto L788;
      L749:
        this.ingredientList[_local3].visible = false;
        this.ingredientTargetPreview[_local3].visible = false;
        this.ingredientCheckMark[_local3].visible = false;
        _local3++;
      L788:
        if (_local3 < 5) goto L749;
        _local4.gameScreen.customerOnQueue.push(_local10);
        this.customerAva.texture = _local7.getTexture("char_" + _local10);
        this.customerAvaID = _local10;
        this.queueIdx = arg1;
        this.totalPatience = 1800 * TGlobalClass.customerXML.customer[_local10 - 1].attribute("patience");
        this.patience = this.totalPatience;
        var $t5 = _local4.tutorial.currentDialogueID == Tutorial.LEVEL_31;
        $s0 = $t5;
        if (!$t5) goto L909;
        $s0 = _local4.tutorial.dialogueCounter == 0;
      L909:
        if (!$s0) goto L922;
        $s0 = this.idx == 0;
      L922:
        if (!$s0) goto L937;
        this.patience = this.patience / 2;
      L937:
        this.completed = false;
        this.customerAva.readjustSize();
        this.customerAva.pivotX = this.customerAva.width;
        this.customerAva.pivotY = this.customerAva.height;
        if (!(TGlobalClass.tempCustUse.length <= 0)) goto L1025;
        TGlobalClass.tempCustUse = _local4.gameScreen.customerInUse.slice(0, TSavegame.currentPlayer.lastCustIdx + 1);
      L1025:
        this.ingredientType.length = 0;
        this.ingredientTarget.length = 0;
        this.ingredientFulfilled.length = 0;
        this.ingredientType = _local4.dish.getDishIngredientType(_local6);
        this.ingredientTarget = _local4.dish.getDishTarget(_local6);
        var $t6 = Boolean(TGlobalClass.isPlayingSpecialLevel);
        $s0 = $t6;
        if (!$t6) goto L1100;
        $s0 = _local10 == 13;
      L1100:
        if (!$s0) goto L1145;
        _local3 = 0;
        goto L1133;
      L1111:
        this.ingredientTarget[_local3] = this.ingredientTarget[_local3] * 2;
        _local3++;
      L1133:
        if (_local3 < this.ingredientTarget.length) goto L1111;
      L1145:
        if (TGlobalClass.currentRegionDish != ConstClass.WESTERN_DISH) goto L1208;
        this.money = _local4.dish.getDishDefaultPrice(_local6) + (TSavegame.currentPlayer.westernDishLevel[_local6] - 1) * _local4.dish.getPriceIncrease(_local6);
        goto L1351;
      L1208:
        if (TGlobalClass.currentRegionDish != ConstClass.CHINESE_DISH) goto L1278;
        this.money = _local4.dish.getDishDefaultPrice(_local6) + (TSavegame.currentPlayer.chineseDishLevel[_local6 - TGlobalClass.TOTAL_WESTERN_DISH] - 1) * _local4.dish.getPriceIncrease(_local6);
        goto L1351;
      L1278:
        if (TGlobalClass.currentRegionDish != ConstClass.MID_EAST_DISH) goto L1351;
        this.money = _local4.dish.getDishDefaultPrice(_local6) + (TSavegame.currentPlayer.chineseDishLevel[_local6 - TGlobalClass.TOTAL_WESTERN_DISH - TGlobalClass.TOTAL_CHINESE_DISH] - 1) * _local4.dish.getPriceIncrease(_local6);
      L1351:
        if (_local10 != 13) goto L1370;
        this.money = this.money * 1.5;
      L1370:
        this.tipsBonus = _local4._maxCrypto.RandomRange(1, TGlobalClass.customerXML.customer[_local10 - 1].attribute("tipsBonus") - 1) * 0.1 * this.totalMoney;
        _local3 = 0;
        goto L1891;
      L1423:
        this.ingredientFulfilled.push(false);
        this.money = this.money + (TSavegame.currentPlayer.ingredientLevel[this.ingredientType[_local3] - 1] - 1) * _local4.dish.getIngredientPriceIncrease(this.ingredientType[_local3] - 1);
        if (this.ingredientType[_local3] != 1) goto L1506;
        _local5 = "bread";
        goto L1755;
      L1506:
        if (this.ingredientType[_local3] != 2) goto L1529;
        _local5 = "selada";
        goto L1755;
      L1529:
        if (this.ingredientType[_local3] != 3) goto L1552;
        _local5 = "beef";
        goto L1755;
      L1552:
        if (this.ingredientType[_local3] != 4) goto L1575;
        _local5 = "ayam";
        goto L1755;
      L1575:
        if (this.ingredientType[_local3] != 5) goto L1598;
        _local5 = "nasi";
        goto L1755;
      L1598:
        if (this.ingredientType[_local3] != 6) goto L1621;
        _local5 = "bacon";
        goto L1755;
      L1621:
        if (this.ingredientType[_local3] != 7) goto L1644;
        _local5 = "salmon";
        goto L1755;
      L1644:
        if (this.ingredientType[_local3] != 8) goto L1667;
        _local5 = "ikan";
        goto L1755;
      L1667:
        if (this.ingredientType[_local3] != 9) goto L1690;
        _local5 = "keju";
        goto L1755;
      L1690:
        if (this.ingredientType[_local3] != 11) goto L1713;
        _local5 = "pita_bread";
        goto L1755;
      L1713:
        if (this.ingredientType[_local3] != 12) goto L1736;
        _local5 = "lamb";
        goto L1755;
      L1736:
        if (this.ingredientType[_local3] != 13) goto L1755;
        _local5 = "tomato";
      L1755:
        this.ingredientList[_local3].texture = _local4.gameScreen.gameAtlas.getTexture(_local5);
        this.ingredientList[_local3].readjustSize();
        this.ingredientList[_local3].pivotX = this.ingredientList[_local3].width / 2;
        this.ingredientList[_local3].pivotY = this.ingredientList[_local3].height / 2;
        this.ingredientTargetPreview[_local3].text = "x" + this.ingredientTarget[_local3];
        this.ingredientList[_local3].visible = true;
        this.ingredientTargetPreview[_local3].visible = true;
        _local3++;
      L1891:
        if (_local3 < this.ingredientType.length) goto L1423;
        this.totalMoney = this.money + this.tipsBonus;
        this.moneyDisplay.text = "$" + this.totalMoney;
        _local4.gameScreen.setNextPriorCust();
        this.tweenCustHeart();
    }

    function showPriority():void {
        TweenLite.killTweensOf(this.queueNumber);
        this.queueNumber.visible = true;
        var _local1 = 0;
        this.queueNumber.scaleY = 0;
        this.queueNumber.scaleX = _local1;
        TweenLite.to(this.queueNumber, 0.5, {"scaleX": 1, "scaleY": 1, "ease": Back.easeOut});
    }

    function hidePriority():void {
        TweenLite.killTweensOf(this.queueNumber);
        this.queueNumber.visible = false;
    }

    function setDishCompleteState():void {
        Global.getInstance()._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "dish_completed"));
        this.completed = true;
        var _local1 = 0;
        goto L83;
      L44:
        this.ingredientCheckMark[_local1].visible = false;
        this.ingredientList[_local1].visible = false;
        this.ingredientTargetPreview[_local1].visible = false;
        _local1++;
      L83:
        if (_local1 < 5) goto L44;
    }

    function tweenOut(arg1:int, arg2:Boolean = true):void {
        _global = null;
        nextCust = false;
        customerState = arg1;
        nextCustomer = arg2;
        this.onTween = true;
        this.completed = true;
        _global = Global.getInstance();
        i = 0;
        goto L117;
      L61:
        this.ingredientList[i].visible = false;
        this.ingredientTargetPreview[i].visible = false;
        this.ingredientCheckMark[i].visible = false;
        i = i + 1;
      L117:
        if (i < 5) goto L61;
        this.heart.visible = false;
        this.heartBG.visible = false;
        this.patienceGlass.visible = false;
        this.patienceSprite.visible = false;
        this.patienceBar.visible = false;
        this.queueNumber.visible = false;
        this.moneyDisplay.visible = false;
        TweenLite.killTweensOf(this.readyText);
        this.readyText.visible = false;
        if (_global.gameScreen.timesUp) goto L256;
        var _local4 = TGlobalClass;
        var _local5 = TGlobalClass.currentCust + 1;
        _local4.currentCust = _local5;
        if (!nextCustomer) goto L252;
        nextCust = true;
      L252:
        goto L295;
      L256:
        if (!_global.gameScreen.timesUp) goto L290;
        nextCust = false;
        _global.gameScreen.checkCustomerLeft();
        goto L295;
      L290:
        nextCust = false;
      L295:
        TweenLite.to(this, 1, {"x": -this.width, "ease": Linear.easeNone, "onComplete": function(f1_arg1:int, f1_arg2:int, f1_arg3:Boolean):void {
            var _local4 = undefined;
            if (f1_arg1 != CUST_SATISFIED) goto L207;
            if (TSavegame.currentPlayer.unlocked[Achievement.SERVICE_EXELLENCE]) goto L203;
            if (!(patience / totalPatience >= 0.8)) goto L203;
            var $t0 = TSavegame.currentPlayer.achievementCounter;
            var _local5 = $t0;
            var $t1 = Achievement.SERVICE_EXELLENCE;
            var _local6 = $t1;
            var _local7 = $t0[$t1] + 1;
            _local5[_local6] = _local7;
            if (!(TSavegame.currentPlayer.achievementCounter[Achievement.SERVICE_EXELLENCE] >= Achievement.ALL_TARGET[Achievement.SERVICE_EXELLENCE])) goto L162;
            TSavegame.currentPlayer.unlocked[Achievement.SERVICE_EXELLENCE] = true;
            Achievement.tweenAchievementNotif(Achievement.SERVICE_EXELLENCE);
            goto L203;
          L162:
            if (TSavegame.currentPlayer.achievementCounter[Achievement.SERVICE_EXELLENCE] % 200 != 0) goto L203;
            Achievement.tweenAchievementProgress(Achievement.SERVICE_EXELLENCE);
          L203:
            goto L245;
          L207:
            if (f1_arg1 != CUST_LEAVE) goto L245;
            var $t2 = Global.getInstance().gameScreen;
            _local5 = $t2;
            _local6 = $t2.missedCustomerCount + 1;
            _local5.missedCustomerCount = _local6;
          L245:
            if (!f1_arg3) goto L397;
            newCust(f1_arg2);
            _local4 = 0;
            goto L310;
          L267:
            ingredientList[_local4].visible = false;
            ingredientTargetPreview[_local4].visible = false;
            ingredientCheckMark[_local4].visible = false;
            _local4 = _local4 + 1;
          L310:
            if (_local4 < 5) goto L267;
            heart.visible = false;
            heartBG.visible = false;
            patienceGlass.visible = false;
            patienceSprite.visible = false;
            patienceBar.visible = false;
            moneyDisplay.visible = false;
            TweenLite.killTweensOf(readyText);
            readyText.visible = false;
            if (_global.tutorial.blockCustomerIn) goto L397;
            tweenIn();
          L397:
        }, "onCompleteParams": [customerState, TGlobalClass.currentCust, nextCust]});
    }

    function tweenReadyText():void {
        TweenLite.to(this.readyText, 0.5, {"scaleX": 1.1, "scaleY": 1.1, "delay": 0.4, "onComplete": function():void {
            TweenLite.to(readyText, 0.5, {"scaleX": 1, "scaleY": 1, "onComplete": tweenReadyText});
        }});
    }

    function tweenIn():void {
        i = 0;
        if (!Global.getInstance().gameScreen.checkCustomerLeft()) goto L31;
        return;
      L31:
        Global.getInstance()._soundAsset.playSFX("bell_ding-ding");
        if (this.ingredientTarget[0] != 0) goto L157;
        this.ingredientTarget = Global.getInstance().dish.getDishTarget(TGlobalClass.dishRegionWestern[0]);
        i = 0;
        goto L142;
      L102:
        this.ingredientTargetPreview[i].text = "x" + this.ingredientTarget[i];
        i = i + 1;
      L142:
        if (i < this.ingredientType.length) goto L102;
      L157:
        this.onTween = true;
        TweenLite.to(this, 1, {"x": 0, "ease": Linear.easeNone, "onComplete": function(f1_arg1:int):void {
            if (Global.getInstance().gameScreen.ready) goto L18;
            return;
          L18:
            Global.getInstance()._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "customer_in", true, false, {"position": f1_arg1}));
            onTween = false;
            var _local2 = 0;
            goto L98;
          L71:
            ingredientList[_local2].visible = true;
            ingredientTargetPreview[_local2].visible = true;
            _local2 = _local2 + 1;
          L98:
            if (_local2 < ingredientType.length) goto L71;
            heart.visible = true;
            heartBG.visible = true;
            patienceGlass.visible = true;
            patienceSprite.visible = true;
            patienceBar.visible = true;
            moneyDisplay.visible = true;
            if (customerAvaID != 13) goto L198;
            Global.getInstance()._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "special_customer_in", true, false, {"position": f1_arg1}));
          L198:
        }, "onCompleteParams": [this.idx]});
    }

    function Kill():void {
        var _local1 = 0;
        TweenLite.killTweensOf(this.readyText);
        super.dispose();
        _local1 = 0;
        goto L75;
      L28:
        this.ingredientList[_local1].removeFromParent(true);
        this.ingredientList[_local1] = null;
        this.ingredientTargetPreview[_local1].removeFromParent(true);
        this.ingredientTargetPreview[_local1] = null;
        _local1++;
      L75:
        if (_local1 < 5) goto L28;
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
}
