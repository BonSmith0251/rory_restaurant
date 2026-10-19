package {
class Tutorial {
    static const TUTORIAL_COUNT:int = 30;
    static const OPENING_STORY:int = 0;
    static const LEVEL_1:int = 1;
    static const AFTER_LEVEL_1:int = 2;
    static const AFTER_LEVEL_2:int = 3;
    static const LEVEL_4:int = 4;
    static const LEVEL_5:int = 5;
    static const LEVEL_7:int = 6;
    static const AFTER_LEVEL_7:int = 7;
    static const LEVEL_8:int = 8;
    static const AFTER_LEVEL_8:int = 9;
    static const LEVEL_11:int = 10;
    static const LEVEL_12:int = 11;
    static const LEVEL_13:int = 12;
    static const LEVEL_14:int = 13;
    static const LEVEL_16:int = 14;
    static const LEVEL_17:int = 15;
    static const LEVEL_18:int = 16;
    static const LEVEL_19:int = 17;
    static const LEVEL_21:int = 18;
    static const LEVEL_22:int = 19;
    static const LEVEL_25:int = 20;
    static const LEVEL_26:int = 21;
    static const LEVEL_27:int = 22;
    static const LEVEL_28:int = 23;
    static const LEVEL_31:int = 24;
    static const LEVEL_32:int = 25;
    static const AFTER_LEVEL_34:int = 26;
    var _global:Global;
    var currentDialogueID:int;
    var dialogueCounter:int;
    var atlas:TextureAtlas;
    var textBox:cSprite;
    var ava:cSprite;
    var arrow:cSprite;
    var button:TNonStateButton;
    var text:ExtendTextField;
    var blockCustomerIn:Boolean;
    var highlightedAr:Array;
    var highlightedAr2:Array;

    // static initializer
    static {
        TUTORIAL_COUNT = 30;
        OPENING_STORY = 0;
        LEVEL_1 = 1;
        AFTER_LEVEL_1 = 2;
        AFTER_LEVEL_2 = 3;
        LEVEL_4 = 4;
        LEVEL_5 = 5;
        LEVEL_7 = 6;
        AFTER_LEVEL_7 = 7;
        LEVEL_8 = 8;
        AFTER_LEVEL_8 = 9;
        LEVEL_11 = 10;
        LEVEL_12 = 11;
        LEVEL_13 = 12;
        LEVEL_14 = 13;
        LEVEL_16 = 14;
        LEVEL_17 = 15;
        LEVEL_18 = 16;
        LEVEL_19 = 17;
        LEVEL_21 = 18;
        LEVEL_22 = 19;
        LEVEL_25 = 20;
        LEVEL_26 = 21;
        LEVEL_27 = 22;
        LEVEL_28 = 23;
        LEVEL_31 = 24;
        LEVEL_32 = 25;
        AFTER_LEVEL_34 = 26;
    }

    function Tutorial():* {
        this.highlightedAr = [];
        this.highlightedAr2 = [];
        super();
    }

    function init():void {
        this._global = Global.getInstance();
        this.atlas = TGlobalClass._windowDisplay.windowsAtlas;
        this.textBox = new cSprite(this.atlas.getTexture("textbox"));
        this.textBox.pivotX = this.textBox.width / 2;
        this.textBox.pivotY = this.textBox.height / 2;
        this.textBox.scaleX = 1.2;
        this.textBox.visible = false;
        this.textBox.touchable = false;
        this._global._gameS.addChild(this.textBox);
        this.ava = new cSprite(this.atlas.getTexture("rory"));
        this.ava.pivotY = this.ava.height;
        this.ava.visible = false;
        this.ava.touchable = false;
        this._global._gameS.addChild(this.ava);
        this.arrow = new cSprite(this.atlas.getTexture("arrow"));
        this.arrow.pivotX = this.arrow.width / 2;
        this.arrow.pivotY = this.arrow.height / 2;
        this.arrow.visible = false;
        this.arrow.touchable = false;
        this._global._gameS.addChild(this.arrow);
        this.button = new TNonStateButton(this.atlas.getTexture("small_btn_blue"), "Next", "bromine", 20, 0);
        this.button.pivotX = this.button.width / 2;
        this.button.pivotY = this.button.height / 2;
        this.button.visible = false;
        this._global._gameS.addChild(this.button);
        this.button.addEventListener(TouchEvent.TOUCH, this.touchButton);
        this.text = new ExtendTextField(this.textBox.width - 135, this.textBox.height - 10, "", "pfennig", 20, 0);
        this.text.pivotX = this.textBox.width / 2 - 115;
        this.text.pivotY = this.textBox.height / 2 - 5;
        this.text.visible = false;
        this.text.touchable = false;
        this.text.hAlign = "center";
        this.text.vAlign = "center";
        this._global._gameS.addChild(this.text);
        this._global._gameS.addEventListener(TutorialEvent.TRIGGER_NEXT, this.onTriggerNext);
    }

    function bringTutorialToTop():void {
        this._global._gameS.setChildIndex(this.textBox, this._global._gameS.numChildren - 1);
        this._global._gameS.setChildIndex(this.ava, this._global._gameS.numChildren - 1);
        this._global._gameS.setChildIndex(this.arrow, this._global._gameS.numChildren - 1);
        this._global._gameS.setChildIndex(this.button, this._global._gameS.numChildren - 1);
        this._global._gameS.setChildIndex(this.text, this._global._gameS.numChildren - 1);
    }

    function onTriggerNext(arg1:TutorialEvent):void {
        var _local2 = 0;
        var _local3 = 0;
        var _local4 = 0;
        var _local5 = 0;
        if (this.currentDialogueID != -1) goto L27;
        return;
      L27:
        if (arg1.actionName != "highlight") goto L941;
        _local2 = int(arg1.infos.col);
        _local3 = int(arg1.infos.row);
        if (!(this.highlightedAr2.length >= 3)) goto L75;
        this.highlightedAr2 = [];
      L75:
        this.highlightedAr2.push({"col": _local2, "row": _local3});
        if (!(this.highlightedAr2.length >= 3)) goto L941;
        var $t0 = this.highlightedAr2[0].row == this.highlightedAr2[1].row;
        $s0 = $t0;
        if (!$t0) goto L162;
        $s0 = this.highlightedAr2[1].row == this.highlightedAr2[2].row;
      L162:
        if (!$s0) goto L338;
        if (Math.abs(this.highlightedAr2[0].col - this.highlightedAr2[1].col) != 1) goto L236;
        _local2 = int(this.highlightedAr2[2].col);
        _local3 = int(this.highlightedAr2[2].row);
        goto L334;
      L236:
        if (Math.abs(this.highlightedAr2[0].col - this.highlightedAr2[2].col) != 1) goto L306;
        _local2 = int(this.highlightedAr2[1].col);
        _local3 = int(this.highlightedAr2[1].row);
        goto L334;
      L306:
        _local2 = int(this.highlightedAr2[0].col);
        _local3 = int(this.highlightedAr2[0].row);
      L334:
        goto L874;
      L338:
        var $t1 = this.highlightedAr2[0].col == this.highlightedAr2[1].col;
        $s0 = $t1;
        if (!$t1) goto L394;
        $s0 = this.highlightedAr2[1].col == this.highlightedAr2[2].col;
      L394:
        if (!$s0) goto L570;
        if (Math.abs(this.highlightedAr2[0].row - this.highlightedAr2[1].row) != 1) goto L468;
        _local2 = int(this.highlightedAr2[2].col);
        _local3 = int(this.highlightedAr2[2].row);
        goto L566;
      L468:
        if (Math.abs(this.highlightedAr2[0].row - this.highlightedAr2[2].row) != 1) goto L538;
        _local2 = int(this.highlightedAr2[1].col);
        _local3 = int(this.highlightedAr2[1].row);
        goto L566;
      L538:
        _local2 = int(this.highlightedAr2[0].col);
        _local3 = int(this.highlightedAr2[0].row);
      L566:
        goto L874;
      L570:
        var $t2 = this.highlightedAr2[0].row == this.highlightedAr2[1].row;
        $s0 = $t2;
        if ($t2) goto L626;
        $s0 = this.highlightedAr2[0].row == this.highlightedAr2[2].row;
      L626:
        if ($s0) goto L657;
        $s0 = this.highlightedAr2[0].col == this.highlightedAr2[1].col;
      L657:
        if ($s0) goto L688;
        $s0 = this.highlightedAr2[0].col == this.highlightedAr2[2].col;
      L688:
        if ($s0) goto L724;
        _local2 = int(this.highlightedAr2[0].col);
        _local3 = int(this.highlightedAr2[0].row);
        goto L874;
      L724:
        var $t3 = this.highlightedAr2[1].row == this.highlightedAr2[0].row;
        $s0 = $t3;
        if ($t3) goto L780;
        $s0 = this.highlightedAr2[1].row == this.highlightedAr2[2].row;
      L780:
        if ($s0) goto L811;
        $s0 = this.highlightedAr2[1].col == this.highlightedAr2[0].col;
      L811:
        if ($s0) goto L842;
        $s0 = this.highlightedAr2[1].col == this.highlightedAr2[2].col;
      L842:
        if ($s0) goto L874;
        _local2 = int(this.highlightedAr2[1].col);
        _local3 = int(this.highlightedAr2[1].row);
      L874:
        this._global.gameScreen.highlightX = this._global.gameScreen.tileAr[_local2][0].x + 40;
        this._global.gameScreen.highlightY = this._global.gameScreen.tileAr[0][_local3].y;
      L941:
        if (this.currentDialogueID != LEVEL_1) goto L1399;
        var $t4 = arg1.actionName == "clicked_ok";
        $s0 = $t4;
        if (!$t4) goto L973;
        $s0 = this.dialogueCounter == 1;
      L973:
        if (!$s0) goto L994;
        this._global.gameScreen.startLevel(1);
        goto L1395;
      L994:
        var $t5 = arg1.actionName == "customer_in";
        $s0 = $t5;
        if (!$t5) goto L1015;
        $s0 = this.dialogueCounter == 1;
      L1015:
        if (!$s0) goto L1028;
        this.nextTutorial();
        goto L1395;
      L1028:
        var $t6 = arg1.actionName == "clicked_ok";
        $s0 = $t6;
        if (!$t6) goto L1049;
        $s0 = this.dialogueCounter == 2;
      L1049:
        if (!$s0) goto L1091;
        this._global.gameScreen.dispatchHighlightPosition = 3;
        this._global.gameScreen.highlightPossible();
        this._global.gameScreen.highlightPossibleTile = true;
        goto L1395;
      L1091:
        var $t7 = arg1.actionName == "matched";
        $s0 = $t7;
        if (!$t7) goto L1112;
        $s0 = this.dialogueCounter == 3;
      L1112:
        if (!$s0) goto L1125;
        this.nextTutorial();
        goto L1395;
      L1125:
        var $t8 = arg1.actionName == "dish_completed";
        $s0 = $t8;
        if (!$t8) goto L1146;
        $s0 = this.dialogueCounter == 4;
      L1146:
        if (!$s0) goto L1175;
        if (!this.textBox.visible) goto L1166;
        this.hideTutorial();
      L1166:
        this.nextTutorial();
        goto L1395;
      L1175:
        var $t9 = arg1.actionName == "clicked_coin";
        $s0 = $t9;
        if (!$t9) goto L1196;
        $s0 = this.dialogueCounter == 5;
      L1196:
        if (!$s0) goto L1214;
        this.blockCustomerIn = true;
        this.nextTutorial();
        goto L1395;
      L1214:
        var $t10 = arg1.actionName == "clicked_ok";
        $s0 = $t10;
        if (!$t10) goto L1235;
        $s0 = this.dialogueCounter == 6;
      L1235:
        if (!$s0) goto L1315;
        this.blockCustomerIn = false;
        this._global.gameScreen.hasWindow = false;
        _local4 = 0;
        goto L1303;
      L1263:
        this._global.gameScreen.customer[_local4].tweenOut(this._global.gameScreen.customer[_local4].CUST_SATISFIED);
        _local4++;
      L1303:
        if (_local4 < 3) goto L1263;
        goto L1395;
      L1315:
        var $t11 = arg1.actionName == "customer_in";
        $s0 = $t11;
        if (!$t11) goto L1339;
        $s0 = arg1.infos.position == 2;
      L1339:
        if (!$s0) goto L1352;
        $s0 = this.dialogueCounter == 6;
      L1352:
        if (!$s0) goto L1365;
        this.nextTutorial();
        goto L1395;
      L1365:
        var $t12 = arg1.actionName == "changed_priority";
        $s0 = $t12;
        if (!$t12) goto L1386;
        $s0 = this.dialogueCounter == 7;
      L1386:
        if (!$s0) goto L1395;
        this.nextTutorial();
      L1395:
        goto L4416;
      L1399:
        if (this.currentDialogueID != AFTER_LEVEL_2) goto L1512;
        var $t13 = arg1.actionName == "clicked_recipe_shop";
        $s0 = $t13;
        if (!$t13) goto L1431;
        $s0 = this.dialogueCounter == 1;
      L1431:
        if (!$s0) goto L1444;
        this.nextTutorial();
        goto L1508;
      L1444:
        var $t14 = arg1.actionName == "right_button";
        $s0 = $t14;
        if (!$t14) goto L1465;
        $s0 = this.dialogueCounter == 3;
      L1465:
        if (!$s0) goto L1478;
        this.nextTutorial();
        goto L1508;
      L1478:
        var $t15 = arg1.actionName == "buy_item";
        $s0 = $t15;
        if (!$t15) goto L1499;
        $s0 = this.dialogueCounter == 4;
      L1499:
        if (!$s0) goto L1508;
        this.nextTutorial();
      L1508:
        goto L4416;
      L1512:
        if (this.currentDialogueID != LEVEL_4) goto L1625;
        var $t16 = arg1.actionName == "get_correct_ho_item";
        $s0 = $t16;
        if (!$t16) goto L1544;
        $s0 = this.dialogueCounter == 4;
      L1544:
        if (!$s0) goto L1557;
        this.nextTutorial();
        goto L1621;
      L1557:
        var $t17 = arg1.actionName == "used_hint";
        $s0 = $t17;
        if (!$t17) goto L1578;
        $s0 = this.dialogueCounter == 5;
      L1578:
        if (!$s0) goto L1591;
        this.hideTutorial();
        goto L1621;
      L1591:
        var $t18 = arg1.actionName == "get_correct_ho_item";
        $s0 = $t18;
        if (!$t18) goto L1612;
        $s0 = this.dialogueCounter == 5;
      L1612:
        if (!$s0) goto L1621;
        this.nextTutorial();
      L1621:
        goto L4416;
      L1625:
        if (this.currentDialogueID != LEVEL_5) goto L1676;
        var $t19 = arg1.actionName == "clicked_ok";
        $s0 = $t19;
        if (!$t19) goto L1657;
        $s0 = this.dialogueCounter == 2;
      L1657:
        if (!$s0) goto L1672;
        this._global.gameScreen.startLevel();
      L1672:
        goto L4416;
      L1676:
        if (this.currentDialogueID != LEVEL_7) goto L1800;
        var $t20 = arg1.actionName == "clicked_ok";
        $s0 = $t20;
        if (!$t20) goto L1708;
        $s0 = this.dialogueCounter == 2;
      L1708:
        if (!$s0) goto L1727;
        this._global.gameScreen.startLevel();
        goto L1796;
      L1727:
        var $t21 = arg1.actionName == "special_customer_in";
        $s0 = $t21;
        if (!$t21) goto L1748;
        $s0 = this.dialogueCounter == 2;
      L1748:
        if (!$s0) goto L1796;
        TGlobalClass.dialogueXML.dialogue[LEVEL_7].text[3].@arrowY = 120 + arg1.infos.position * 118;
        this.nextTutorial();
      L1796:
        goto L4416;
      L1800:
        if (this.currentDialogueID != LEVEL_8) goto L1851;
        var $t22 = arg1.actionName == "clicked_ok";
        $s0 = $t22;
        if (!$t22) goto L1832;
        $s0 = this.dialogueCounter == 1;
      L1832:
        if (!$s0) goto L1847;
        this._global.gameScreen.startLevel();
      L1847:
        goto L4416;
      L1851:
        if (this.currentDialogueID != AFTER_LEVEL_8) goto L1930;
        var $t23 = arg1.actionName == "clicked_ingredients";
        $s0 = $t23;
        if (!$t23) goto L1883;
        $s0 = this.dialogueCounter == 2;
      L1883:
        if (!$s0) goto L1896;
        this.nextTutorial();
        goto L1926;
      L1896:
        var $t24 = arg1.actionName == "buy_item";
        $s0 = $t24;
        if (!$t24) goto L1917;
        $s0 = this.dialogueCounter == 8;
      L1917:
        if (!$s0) goto L1926;
        this.nextTutorial();
      L1926:
        goto L4416;
      L1930:
        if (this.currentDialogueID != LEVEL_11) goto L2341;
        var $t25 = arg1.actionName == "clicked_ok";
        $s0 = $t25;
        if (!$t25) goto L1962;
        $s0 = this.dialogueCounter == 4;
      L1962:
        if (!$s0) goto L1981;
        this._global._minigameDessert.startLevel();
        goto L2337;
      L1981:
        var $t26 = arg1.actionName == "check_times_up";
        $s0 = $t26;
        if (!$t26) goto L2002;
        $s0 = this.dialogueCounter == 4;
      L2002:
        if (!$s0) goto L2021;
        this._global.tutorial.nextTutorial();
        goto L2337;
      L2021:
        var $t27 = arg1.actionName == "touch_ingredient";
        $s0 = $t27;
        if (!$t27) goto L2042;
        $s0 = this.dialogueCounter == 7;
      L2042:
        if (!$s0) goto L2058;
        $s0 = arg1.infos.id == 0;
      L2058:
        if (!$s0) goto L2077;
        this._global.tutorial.nextTutorial();
        goto L2337;
      L2077:
        var $t28 = arg1.actionName == "touch_ingredient";
        $s0 = $t28;
        if (!$t28) goto L2098;
        $s0 = this.dialogueCounter == 8;
      L2098:
        if (!$s0) goto L2114;
        $s0 = arg1.infos.id == 1;
      L2114:
        if (!$s0) goto L2133;
        this._global.tutorial.nextTutorial();
        goto L2337;
      L2133:
        var $t29 = arg1.actionName == "touch_ingredient";
        $s0 = $t29;
        if (!$t29) goto L2154;
        $s0 = this.dialogueCounter == 9;
      L2154:
        if (!$s0) goto L2170;
        $s0 = arg1.infos.id == 3;
      L2170:
        if (!$s0) goto L2189;
        this._global.tutorial.nextTutorial();
        goto L2337;
      L2189:
        var $t30 = arg1.actionName == "touch_ingredient";
        $s0 = $t30;
        if (!$t30) goto L2210;
        $s0 = this.dialogueCounter == 10;
      L2210:
        if (!$s0) goto L2226;
        $s0 = arg1.infos.id == 4;
      L2226:
        if (!$s0) goto L2245;
        this._global.tutorial.nextTutorial();
        goto L2337;
      L2245:
        var $t31 = arg1.actionName == "touch_ingredient";
        $s0 = $t31;
        if (!$t31) goto L2266;
        $s0 = this.dialogueCounter == 11;
      L2266:
        if (!$s0) goto L2282;
        $s0 = arg1.infos.id == 6;
      L2282:
        if (!$s0) goto L2301;
        this._global.tutorial.hideTutorial();
        goto L2337;
      L2301:
        var $t32 = arg1.actionName == "check_times_up";
        $s0 = $t32;
        if (!$t32) goto L2322;
        $s0 = this.dialogueCounter == 11;
      L2322:
        if (!$s0) goto L2337;
        this._global.tutorial.nextTutorial();
      L2337:
        goto L4416;
      L2341:
        if (this.currentDialogueID != LEVEL_12) goto L2562;
        var $t33 = arg1.actionName == "customer_in";
        $s0 = $t33;
        if (!$t33) goto L2373;
        $s0 = this.dialogueCounter == 0;
      L2373:
        if (!$s0) goto L2397;
        this._global.gameScreen.hasWindow = false;
        this.nextTutorial();
        goto L2558;
      L2397:
        var $t34 = arg1.actionName == "clicked_ok";
        $s0 = $t34;
        if (!$t34) goto L2418;
        $s0 = this.dialogueCounter == 2;
      L2418:
        if (!$s0) goto L2460;
        this._global.gameScreen.dispatchHighlightPosition = 3;
        this._global.gameScreen.highlightPossible();
        this._global.gameScreen.highlightPossibleTile = true;
        goto L2558;
      L2460:
        var $t35 = arg1.actionName == "matched";
        $s0 = $t35;
        if (!$t35) goto L2481;
        $s0 = this.dialogueCounter == 3;
      L2481:
        if (!$s0) goto L2494;
        this.nextTutorial();
        goto L2558;
      L2494:
        var $t36 = arg1.actionName == "clicked_beverage";
        $s0 = $t36;
        if (!$t36) goto L2515;
        $s0 = this.dialogueCounter == 4;
      L2515:
        if (!$s0) goto L2528;
        this.nextTutorial();
        goto L2558;
      L2528:
        var $t37 = arg1.actionName == "offered_beverage";
        $s0 = $t37;
        if (!$t37) goto L2549;
        $s0 = this.dialogueCounter == 5;
      L2549:
        if (!$s0) goto L2558;
        this.nextTutorial();
      L2558:
        goto L4416;
      L2562:
        if (this.currentDialogueID != LEVEL_13) goto L2613;
        var $t38 = arg1.actionName == "clicked_ok";
        $s0 = $t38;
        if (!$t38) goto L2594;
        $s0 = this.dialogueCounter == 2;
      L2594:
        if (!$s0) goto L2609;
        this._global.gameScreen.startLevel();
      L2609:
        goto L4416;
      L2613:
        if (this.currentDialogueID != LEVEL_14) goto L2664;
        var $t39 = arg1.actionName == "clicked_ok";
        $s0 = $t39;
        if (!$t39) goto L2645;
        $s0 = this.dialogueCounter == 0;
      L2645:
        if (!$s0) goto L2660;
        this._global.gameScreen.startLevel();
      L2660:
        goto L4416;
      L2664:
        if (this.currentDialogueID != LEVEL_16) goto L2715;
        var $t40 = arg1.actionName == "clicked_ok";
        $s0 = $t40;
        if (!$t40) goto L2696;
        $s0 = this.dialogueCounter == 2;
      L2696:
        if (!$s0) goto L2711;
        this._global.gameScreen.startLevel();
      L2711:
        goto L4416;
      L2715:
        if (this.currentDialogueID != LEVEL_17) goto L2936;
        var $t41 = arg1.actionName == "customer_in";
        $s0 = $t41;
        if (!$t41) goto L2747;
        $s0 = this.dialogueCounter == 0;
      L2747:
        if (!$s0) goto L2771;
        this._global.gameScreen.hasWindow = false;
        this.nextTutorial();
        goto L2932;
      L2771:
        var $t42 = arg1.actionName == "clicked_ok";
        $s0 = $t42;
        if (!$t42) goto L2792;
        $s0 = this.dialogueCounter == 2;
      L2792:
        if (!$s0) goto L2834;
        this._global.gameScreen.dispatchHighlightPosition = 3;
        this._global.gameScreen.highlightPossible();
        this._global.gameScreen.highlightPossibleTile = true;
        goto L2932;
      L2834:
        var $t43 = arg1.actionName == "matched";
        $s0 = $t43;
        if (!$t43) goto L2855;
        $s0 = this.dialogueCounter == 3;
      L2855:
        if (!$s0) goto L2868;
        this.nextTutorial();
        goto L2932;
      L2868:
        var $t44 = arg1.actionName == "clicked_fries";
        $s0 = $t44;
        if (!$t44) goto L2889;
        $s0 = this.dialogueCounter == 4;
      L2889:
        if (!$s0) goto L2902;
        this.nextTutorial();
        goto L2932;
      L2902:
        var $t45 = arg1.actionName == "offered_fries";
        $s0 = $t45;
        if (!$t45) goto L2923;
        $s0 = this.dialogueCounter == 5;
      L2923:
        if (!$s0) goto L2932;
        this.nextTutorial();
      L2932:
        goto L4416;
      L2936:
        if (this.currentDialogueID != LEVEL_19) goto L2987;
        var $t46 = arg1.actionName == "clicked_ok";
        $s0 = $t46;
        if (!$t46) goto L2968;
        $s0 = this.dialogueCounter == 0;
      L2968:
        if (!$s0) goto L2983;
        this._global.gameScreen.startLevel();
      L2983:
        goto L4416;
      L2987:
        if (this.currentDialogueID != LEVEL_21) goto L3038;
        var $t47 = arg1.actionName == "clicked_ok";
        $s0 = $t47;
        if (!$t47) goto L3019;
        $s0 = this.dialogueCounter == 0;
      L3019:
        if (!$s0) goto L3034;
        this._global.gameScreen.startLevel();
      L3034:
        goto L4416;
      L3038:
        if (this.currentDialogueID != LEVEL_22) goto L3089;
        var $t48 = arg1.actionName == "clicked_ok";
        $s0 = $t48;
        if (!$t48) goto L3070;
        $s0 = this.dialogueCounter == 1;
      L3070:
        if (!$s0) goto L3085;
        this._global.gameScreen.startLevel();
      L3085:
        goto L4416;
      L3089:
        if (this.currentDialogueID != LEVEL_25) goto L3140;
        var $t49 = arg1.actionName == "clicked_ok";
        $s0 = $t49;
        if (!$t49) goto L3121;
        $s0 = this.dialogueCounter == 0;
      L3121:
        if (!$s0) goto L3136;
        this._global._minigameDessert.startLevel();
      L3136:
        goto L4416;
      L3140:
        if (this.currentDialogueID != LEVEL_26) goto L4144;
        var $t50 = arg1.actionName == "customer_in";
        $s0 = $t50;
        if (!$t50) goto L3172;
        $s0 = this.dialogueCounter == 0;
      L3172:
        if (!$s0) goto L3196;
        this._global.gameScreen.hasWindow = false;
        this.nextTutorial();
        goto L4140;
      L3196:
        var $t51 = arg1.actionName == "highlight";
        $s0 = $t51;
        if (!$t51) goto L3217;
        $s0 = this.dialogueCounter == 0;
      L3217:
        if (!$s0) goto L4047;
        _local2 = int(arg1.infos.col);
        _local3 = int(arg1.infos.row);
        if (!(this.highlightedAr.length >= 3)) goto L3258;
        this.highlightedAr = [];
      L3258:
        this.highlightedAr.push({"col": _local2, "row": _local3});
        if (!(this.highlightedAr.length < 3)) goto L3290;
        return;
      L3290:
        var $t52 = this.highlightedAr[0].row == this.highlightedAr[1].row;
        $s0 = $t52;
        if (!$t52) goto L3346;
        $s0 = this.highlightedAr[1].row == this.highlightedAr[2].row;
      L3346:
        if (!$s0) goto L3452;
        if (Math.abs(this.highlightedAr[0].col - this.highlightedAr[1].col) != 1) goto L3420;
        _local2 = int(this.highlightedAr[0].col);
        _local3 = int(this.highlightedAr[0].row);
        goto L3448;
      L3420:
        _local2 = int(this.highlightedAr[2].col);
        _local3 = int(this.highlightedAr[2].row);
      L3448:
        goto L3764;
      L3452:
        var $t53 = this.highlightedAr[0].col == this.highlightedAr[1].col;
        $s0 = $t53;
        if (!$t53) goto L3508;
        $s0 = this.highlightedAr[1].col == this.highlightedAr[2].col;
      L3508:
        if (!$s0) goto L3614;
        if (Math.abs(this.highlightedAr[0].row - this.highlightedAr[1].row) != 1) goto L3582;
        _local2 = int(this.highlightedAr[0].col);
        _local3 = int(this.highlightedAr[0].row);
        goto L3610;
      L3582:
        _local2 = int(this.highlightedAr[2].col);
        _local3 = int(this.highlightedAr[2].row);
      L3610:
        goto L3764;
      L3614:
        var $t54 = this.highlightedAr[2].row == this.highlightedAr[0].row;
        $s0 = $t54;
        if ($t54) goto L3670;
        $s0 = this.highlightedAr[2].row == this.highlightedAr[1].row;
      L3670:
        if ($s0) goto L3701;
        $s0 = this.highlightedAr[2].col == this.highlightedAr[0].col;
      L3701:
        if ($s0) goto L3732;
        $s0 = this.highlightedAr[2].col == this.highlightedAr[1].col;
      L3732:
        if ($s0) goto L3764;
        _local2 = int(this.highlightedAr[0].col);
        _local3 = int(this.highlightedAr[0].row);
      L3764:
        this._global.gameScreen.tileAr[_local2][_local3].frozen = true;
        _local5 = 0;
        goto L3997;
      L3794:
        if (this._global.gameScreen.frozenTileAr[_local5].visible) goto L3995;
        this._global.gameScreen.frozenTileAr[_local5].visible = true;
        this._global.gameScreen.frozenTileAr[_local5].x = this._global.gameScreen.tileAr[_local2][_local3].x;
        this._global.gameScreen.frozenTileAr[_local5].y = this._global.gameScreen.tileAr[_local2][_local3].y;
        this._global.gameScreen.frozenTileAr[_local5].attachedCol = _local2;
        this._global.gameScreen.frozenTileAr[_local5].attachedRow = _local3;
        this._global.gameScreen.tileAr[_local2][_local3].frozenTileIdx = this._global.gameScreen.frozenTileAr[_local5].idx;
        goto L4016;
      L3995:
        _local5++;
      L3997:
        if (_local5 < this._global.gameScreen.frozenTileAr.length) goto L3794;
      L4016:
        var $t55 = this._global.gameScreen;
        var _local6 = $t55;
        var _local7 = $t55.currentFrozenTile + 1;
        _local6.currentFrozenTile = _local7;
        goto L4140;
      L4047:
        var $t56 = arg1.actionName == "clicked_ok";
        $s0 = $t56;
        if (!$t56) goto L4068;
        $s0 = this.dialogueCounter == 2;
      L4068:
        if (!$s0) goto L4110;
        this._global.gameScreen.dispatchHighlightPosition = 3;
        this._global.gameScreen.highlightPossible();
        this._global.gameScreen.highlightPossibleTile = true;
        goto L4140;
      L4110:
        var $t57 = arg1.actionName == "matched";
        $s0 = $t57;
        if (!$t57) goto L4131;
        $s0 = this.dialogueCounter == 3;
      L4131:
        if (!$s0) goto L4140;
        this.nextTutorial();
      L4140:
        goto L4416;
      L4144:
        if (this.currentDialogueID != LEVEL_27) goto L4234;
        var $t58 = arg1.actionName == "customer_in";
        $s0 = $t58;
        if (!$t58) goto L4176;
        $s0 = this.dialogueCounter == 0;
      L4176:
        if (!$s0) goto L4189;
        this.nextTutorial();
        goto L4230;
      L4189:
        var $t59 = arg1.actionName == "start_torch";
        $s0 = $t59;
        if (!$t59) goto L4210;
        $s0 = this.dialogueCounter == 2;
      L4210:
        if (!$s0) goto L4230;
        this.nextTutorial();
        this._global.gameScreen.hasWindow = false;
      L4230:
        goto L4416;
      L4234:
        if (this.currentDialogueID != LEVEL_28) goto L4285;
        var $t60 = arg1.actionName == "clicked_ok";
        $s0 = $t60;
        if (!$t60) goto L4266;
        $s0 = this.dialogueCounter == 0;
      L4266:
        if (!$s0) goto L4281;
        this._global.gameScreen.startLevel();
      L4281:
        goto L4416;
      L4285:
        if (this.currentDialogueID != LEVEL_31) goto L4375;
        var $t61 = arg1.actionName == "customer_in";
        $s0 = $t61;
        if (!$t61) goto L4317;
        $s0 = this.dialogueCounter == 0;
      L4317:
        if (!$s0) goto L4341;
        this.nextTutorial();
        this._global.gameScreen.hasWindow = false;
        goto L4371;
      L4341:
        var $t62 = arg1.actionName == "give_cookies";
        $s0 = $t62;
        if (!$t62) goto L4362;
        $s0 = this.dialogueCounter == 2;
      L4362:
        if (!$s0) goto L4371;
        this.nextTutorial();
      L4371:
        goto L4416;
      L4375:
        if (this.currentDialogueID != AFTER_LEVEL_34) goto L4416;
        var $t63 = arg1.actionName == "clicked_hardware";
        $s0 = $t63;
        if (!$t63) goto L4407;
        $s0 = this.dialogueCounter == 1;
      L4407:
        if (!$s0) goto L4416;
        this.nextTutorial();
      L4416:
    }

    function canClickThisButton(arg1:int):Boolean {
        if (this.currentDialogueID != AFTER_LEVEL_2) goto L101;
        var $t0 = this.dialogueCounter == 1;
        $s0 = $t0;
        if (!$t0) goto L35;
        $s0 = !(arg1 == Kitchen.RECIPE_ID);
      L35:
        if (!$s0) goto L41;
        return false;
      L41:
        var $t1 = this.dialogueCounter == 3;
        $s0 = $t1;
        if (!$t1) goto L63;
        $s0 = !(arg1 == Kitchen.RIGHT_BUTTON);
      L63:
        if (!$s0) goto L69;
        return false;
      L69:
        var $t2 = this.dialogueCounter == 4;
        $s0 = $t2;
        if (!$t2) goto L91;
        $s0 = !(arg1 == Kitchen.BUY_BUTTON);
      L91:
        if (!$s0) goto L97;
        return false;
      L97:
        goto L346;
      L101:
        if (this.currentDialogueID != AFTER_LEVEL_8) goto L172;
        var $t3 = this.dialogueCounter == 2;
        $s0 = $t3;
        if (!$t3) goto L134;
        $s0 = !(arg1 == Kitchen.SHOP_ID);
      L134:
        if (!$s0) goto L140;
        return false;
      L140:
        var $t4 = this.dialogueCounter == 8;
        $s0 = $t4;
        if (!$t4) goto L162;
        $s0 = !(arg1 == Kitchen.BUY_BUTTON);
      L162:
        if (!$s0) goto L168;
        return false;
      L168:
        goto L346;
      L172:
        if (this.currentDialogueID != LEVEL_11) goto L307;
        var $t5 = this.dialogueCounter == 7;
        $s0 = $t5;
        if (!$t5) goto L201;
        $s0 = !(arg1 == 0);
      L201:
        if (!$s0) goto L207;
        return false;
      L207:
        var $t6 = this.dialogueCounter == 8;
        $s0 = $t6;
        if (!$t6) goto L225;
        $s0 = !(arg1 == 1);
      L225:
        if (!$s0) goto L231;
        return false;
      L231:
        var $t7 = this.dialogueCounter == 9;
        $s0 = $t7;
        if (!$t7) goto L249;
        $s0 = !(arg1 == 3);
      L249:
        if (!$s0) goto L255;
        return false;
      L255:
        var $t8 = this.dialogueCounter == 10;
        $s0 = $t8;
        if (!$t8) goto L273;
        $s0 = !(arg1 == 4);
      L273:
        if (!$s0) goto L279;
        return false;
      L279:
        var $t9 = this.dialogueCounter == 11;
        $s0 = $t9;
        if (!$t9) goto L297;
        $s0 = !(arg1 == 6);
      L297:
        if (!$s0) goto L303;
        return false;
      L303:
        goto L346;
      L307:
        if (this.currentDialogueID != AFTER_LEVEL_34) goto L346;
        var $t10 = this.dialogueCounter == 1;
        $s0 = $t10;
        if (!$t10) goto L340;
        $s0 = !(arg1 == Kitchen.HARDWARE_ID);
      L340:
        if (!$s0) goto L346;
        return false;
      L346:
        return true;
    }

    function checkTut(arg1:int, arg2:int, arg3:Boolean = false):Boolean {
        var $t0 = TSavegame.currentPlayer.passedDays == arg2;
        $s0 = $t0;
        if (!$t0) goto L48;
        var $t1 = this._global.currentSceneID == this._global.KITCHEN_ID;
        $s0 = $t1;
        if ($t1) goto L48;
        $s0 = TGlobalClass.currentPlayedLevel == arg2;
      L48:
        if (!$s0) goto L76;
        var $t2 = !TSavegame.currentPlayer.tutorialShowed[arg1];
        $s0 = $t2;
        if ($t2) goto L76;
        $s0 = Boolean(arg3);
      L76:
        if (!$s0) goto L102;
        this.showTutorial(arg1);
        TSavegame.currentPlayer.tutorialShowed[arg1] = true;
        return true;
      L102:
        return false;
    }

    function touchButton(arg1:TouchEvent):void {
        if (!arg1.getTouch(this._global._game, TouchPhase.BEGAN)) goto L101;
        this._global._gameS.dispatchEvent(new TutorialEvent(TutorialEvent.TRIGGER_NEXT, "clicked_ok"));
        if (TGlobalClass.dialogueXML.dialogue[this.currentDialogueID].text[this.dialogueCounter].@blockNext == "true") goto L96;
        this.nextTutorial();
        goto L101;
      L96:
        this.hideTutorial();
      L101:
    }

    function showTutorial(arg1:int, arg2:int = 0):void {
        this.currentDialogueID = arg1;
        this.dialogueCounter = arg2;
        this.hideTutorial();
        var _local3 = TGlobalClass.dialogueXML.dialogue[arg1].text[arg2];
        if (_local3.toString() != "(start level)") goto L64;
        this._global.gameScreen.startLevel();
        return;
      L64:
        this.textBox.visible = true;
        this.ava.visible = true;
        if (_local3.attribute("arrowY") == "") goto L102;
        this.arrow.visible = true;
      L102:
        if (_local3.attribute("proceedType") != "button") goto L125;
        this.button.visible = true;
      L125:
        this.text.visible = true;
        this.textBox.x = _local3.attribute("posX");
        this.textBox.y = _local3.attribute("posY");
        this.arrow.x = this.processArrowPosition(_local3.attribute("arrowX"));
        this.arrow.y = this.processArrowPosition(_local3.attribute("arrowY"));
        this.button.x = this.textBox.x;
        this.button.y = this.textBox.y + 110;
        this.ava.x = this.textBox.x - this.textBox.pivotX * this.textBox.scaleX + 5;
        this.ava.y = this.textBox.y + this.textBox.pivotY - 5;
        this.text.x = this.textBox.x;
        this.text.y = this.textBox.y;
        this.arrow.scaleX = int(_local3.attribute("arrowDirection"));
        $s0 = this;
        if (!(_local3.attribute("arrowDirection") == 1)) goto L363;
        $s1 = false;
        goto L364;
      L363:
        $s1 = true;
      L364:
        $s0.tweenArrow($s1);
        this.text.text = _local3.toString();
        this.text.text = this.text.text.replace("(player)", TSavegame.currentPlayer.name);
        if (this._global.currentSceneID != this._global.GAMESCREEN_ID) goto L462;
        this.text.text = this.text.text.replace("(juice percentage)", this._global.gameScreen.juicerPercentage.text);
      L462:
        this.button._text.text = _local3.attribute("buttonText").toString();
    }

    function processArrowPosition(arg1:String):int {
        var _local2 = 0;
        var _local3 = 0;
        if (arg1 != "HOITEMX") goto L199;
        _local2 = int(this._global._minigameHO._HOClass.itemPosXAr[this._global._minigameHO._HOClass.listedItemAr[0]] - 40);
        _local3 = int(this._global._minigameHO._HOClass.itemPosYAr[this._global._minigameHO._HOClass.listedItemAr[0]]);
        if (!(_local3 > 300)) goto L113;
        this.textBox.y = 250;
        goto L123;
      L113:
        this.textBox.y = 500;
      L123:
        if (!(_local2 > 512)) goto L145;
        this.textBox.x = 335;
        goto L193;
      L145:
        this.textBox.x = 670;
        TGlobalClass.dialogueXML.dialogue[Tutorial.LEVEL_4].text[4].@arrowDirection = "-1";
        _local2 = int(_local2 + 100);
      L193:
        return _local2;
      L199:
        if (arg1 != "HOITEMY") goto L283;
        return this._global._minigameHO._HOClass.itemPosYAr[this._global._minigameHO._HOClass.listedItemAr[0]] + this._global._minigameHO._HOClass.itemDisplayAr[this._global._minigameHO._HOClass.listedItemAr[0]].height / 2;
      L283:
        if (arg1 != "TILEX") goto L302;
        return this._global.gameScreen.highlightX;
      L302:
        if (arg1 != "TILEY") goto L366;
        if (!(this._global.gameScreen.highlightY > 300)) goto L341;
        this.textBox.y = 250;
        goto L351;
      L341:
        this.textBox.y = 500;
      L351:
        return this._global.gameScreen.highlightY;
      L366:
        if (arg1 != "TORCHX") goto L401;
        return this._global.gameScreen.tileAr[1][1].x + 40;
      L401:
        if (arg1 != "TORCHY") goto L433;
        return this._global.gameScreen.tileAr[1][1].y;
      L433:
        if (arg1 != "COOKIEX") goto L468;
        return this._global.gameScreen.tileAr[1][1].x + 40;
      L468:
        if (arg1 != "COOKIEY") goto L500;
        return this._global.gameScreen.tileAr[1][1].y;
      L500:
        return int(arg1);
    }

    function tweenArrow(arg1:Boolean):void {
        reverse = arg1;
        $s0 = TweenLite;
        $s1 = this.arrow;
        $s2 = 0.6;
        $s3 = "scaleX";
        $s4 = 0.8;
        if (!reverse) goto L40;
        $s5 = -1;
        goto L43;
      L40:
        $s5 = 1;
      L43:
        $s4 = $s4 * $s5;
        $s5 = "scaleY";
        $s6 = 0.95;
        $s7 = "x";
        $s8 = this.arrow.x;
        $s9 = 40;
        if (!reverse) goto L76;
        $s10 = -1;
        goto L79;
      L76:
        $s10 = 1;
      L79:
        $s0.to($s1, $s2, {$s3: $s4, $s5: $s6, $s7: $s8 - $s9 * $s10, "onComplete": function():void {
            $s0 = TweenLite;
            $s1 = arrow;
            $s2 = 0.5;
            $s3 = "scaleX";
            $s4 = 1;
            if (!reverse) goto L31;
            $s5 = -1;
            goto L34;
          L31:
            $s5 = 1;
          L34:
            $s4 = $s4 * $s5;
            $s5 = "scaleY";
            $s6 = 1;
            $s7 = "x";
            $s8 = arrow.x;
            $s9 = 40;
            if (!reverse) goto L65;
            $s10 = -1;
            goto L68;
          L65:
            $s10 = 1;
          L68:
            $s0.to($s1, $s2, {$s3: $s4, $s5: $s6, $s7: $s8 + $s9 * $s10, "onComplete": function():void {
                tweenArrow(reverse);
            }});
        }});
    }

    function nextTutorial():void {
        var _local1 = this;
        var _local2 = this.dialogueCounter + 1;
        _local1.dialogueCounter = _local2;
        if (!TGlobalClass.dialogueXML.dialogue[this.currentDialogueID].text[this.dialogueCounter]) goto L66;
        this.showTutorial(this.currentDialogueID, this.dialogueCounter);
        goto L77;
      L66:
        this.currentDialogueID = -1;
        this.hideTutorial();
      L77:
    }

    function hideTutorial():void {
        this.textBox.visible = false;
        this.ava.visible = false;
        this.arrow.visible = false;
        this.button.visible = false;
        this.text.visible = false;
        TweenLite.killTweensOf(this.arrow);
    }

    function kill():void {
        this.textBox.removeEventListener(TutorialEvent.TRIGGER_NEXT, this.onTriggerNext);
        this.textBox.removeFromParent(true);
        this.textBox = null;
        this.ava.removeFromParent(true);
        this.ava = null;
        TweenLite.killTweensOf(this.arrow);
        this.arrow.removeFromParent(true);
        this.arrow = null;
        this.text.removeFromParent(true);
        this.text = null;
    }
}
}
