package {
class TSavegame {
    static var fullScreen:Boolean;
    static var usedSlot:Vector.<Boolean>;
    static var playerName:Vector.<String>;
    static var ingredientLevel:Vector.<Vector.<int>>;
    static var westernLevel:Vector.<Vector.<int>>;
    static var chineseLevel:Vector.<Vector.<int>>;
    static var mideastLevel:Vector.<Vector.<int>>;
    static var hardwareLevel:Vector.<Vector.<int>>;
    static var money:Vector.<String>;
    static var lastCustIdx:Vector.<int>;
    static var lastKitchenDay:Vector.<int>;
    static var openingViewed:Vector.<Boolean>;
    static var endingViewed:Vector.<Boolean>;
    static var passedDays:Vector.<int>;
    static var daysEarning:Vector.<Vector.<int>>;
    static var goalAchieved:Vector.<Vector.<Boolean>>;
    static var expertAchieved:Vector.<Vector.<Boolean>>;
    static var unlocked:Vector.<Vector.<Boolean>>;
    static var achievementCounter:Vector.<Vector.<int>>;
    static var tutorialShowed:Vector.<Vector.<Boolean>>;
    static var _saveGame:SharedObject;
    static var _saveGameSetting:SharedObject;
    static var lastPlayer:int;
    static var muteMusic:Boolean;
    static var muteSound:Boolean;
    static var currentPlayer:TPlayer;

    // static initializer
    static {
        usedSlot = new Vector.<Boolean>(5, true);
        playerName = new Vector.<String>(5, true);
        ingredientLevel = new Vector.<Vector.<int>>(5, true);
        westernLevel = new Vector.<Vector.<int>>(5, true);
        chineseLevel = new Vector.<Vector.<int>>(5, true);
        mideastLevel = new Vector.<Vector.<int>>(5, true);
        hardwareLevel = new Vector.<Vector.<int>>(5, true);
        money = new Vector.<String>(5, true);
        lastCustIdx = new Vector.<int>(5, true);
        lastKitchenDay = new Vector.<int>(5, true);
        openingViewed = new Vector.<Boolean>(5, true);
        endingViewed = new Vector.<Boolean>(5, true);
        passedDays = new Vector.<int>(5, true);
        daysEarning = new Vector.<Vector.<int>>(5, true);
        goalAchieved = new Vector.<Vector.<Boolean>>(5, true);
        expertAchieved = new Vector.<Vector.<Boolean>>(5, true);
        unlocked = new Vector.<Vector.<Boolean>>(5, true);
        achievementCounter = new Vector.<Vector.<int>>(5, true);
        tutorialShowed = new Vector.<Vector.<Boolean>>(5, true);
        _saveGame = SharedObject.getLocal("Match3Cooking");
        _saveGameSetting = SharedObject.getLocal("Match3CookingSetting");
    }

    function TSavegame():* {
        super();
    }

    static function init():void {
        currentPlayer = new TPlayer();
        lastPlayer = 0;
        muteMusic = false;
        muteSound = false;
        fullScreen = true;
        var _local1 = 0;
        goto L791;
      L51:
        usedSlot[_local1] = false;
        playerName[_local1] = "";
        money[_local1] = "a";
        lastCustIdx[_local1] = 2;
        lastKitchenDay[_local1] = 0;
        openingViewed[_local1] = false;
        endingViewed[_local1] = false;
        var $t0 = new Vector.<int>(13);
        $t0[0] = 1;
        $t0[1] = 1;
        $t0[2] = 1;
        $t0[3] = 1;
        $t0[4] = 1;
        $t0[5] = 1;
        $t0[6] = 1;
        $t0[7] = 1;
        $t0[8] = 1;
        $t0[9] = 1;
        $t0[10] = 1;
        $t0[11] = 1;
        $t0[12] = 1;
        ingredientLevel[_local1] = $t0;
        var $t1 = new Vector.<int>(6);
        $t1[0] = 1;
        $t1[1] = 0;
        $t1[2] = 0;
        $t1[3] = 0;
        $t1[4] = 0;
        $t1[5] = 0;
        westernLevel[_local1] = $t1;
        var $t2 = new Vector.<int>(6);
        $t2[0] = 0;
        $t2[1] = 0;
        $t2[2] = 0;
        $t2[3] = 0;
        $t2[4] = 0;
        $t2[5] = 0;
        chineseLevel[_local1] = $t2;
        var $t3 = new Vector.<int>(6);
        $t3[0] = 0;
        $t3[1] = 0;
        $t3[2] = 0;
        $t3[3] = 0;
        $t3[4] = 0;
        $t3[5] = 0;
        mideastLevel[_local1] = $t3;
        var $t4 = new Vector.<int>(3);
        $t4[0] = 0;
        $t4[1] = 0;
        $t4[2] = 0;
        hardwareLevel[_local1] = $t4;
        passedDays[_local1] = 0;
        daysEarning[_local1] = new Vector.<int>(TGlobalClass.TOTAL_DAYS, false);
        goalAchieved[_local1] = new Vector.<Boolean>(TGlobalClass.TOTAL_DAYS, false);
        expertAchieved[_local1] = new Vector.<Boolean>(TGlobalClass.TOTAL_DAYS, false);
        var $t5 = new Vector.<Boolean>(10);
        $t5[0] = false;
        $t5[1] = false;
        $t5[2] = false;
        $t5[3] = false;
        $t5[4] = false;
        $t5[5] = false;
        $t5[6] = false;
        $t5[7] = false;
        $t5[8] = false;
        $t5[9] = false;
        unlocked[_local1] = $t5;
        var $t6 = new Vector.<int>(10);
        $t6[0] = 0;
        $t6[1] = 0;
        $t6[2] = 1;
        $t6[3] = 0;
        $t6[4] = 0;
        $t6[5] = 0;
        $t6[6] = 0;
        $t6[7] = 0;
        $t6[8] = 0;
        $t6[9] = 0;
        achievementCounter[_local1] = $t6;
        tutorialShowed[_local1] = new Vector.<Boolean>(Tutorial.TUTORIAL_COUNT, false);
        _local1++;
      L791:
        if (_local1 < 5) goto L51;
    }

    static function loadAll():void {
        var _local1 = 0;
        var _local2 = 0;
        var _local3 = null;
        if (_saveGame.data.lastPlayer == null) goto L1279;
        usedSlot = Vector.<Boolean>(_saveGame.data.usedSlot);
        playerName = Vector.<String>(_saveGame.data.playerName);
        lastPlayer = _saveGame.data.lastPlayer;
        if (_saveGame.data.fullScreen == null) goto L122;
        fullScreen = _saveGame.data.fullScreen;
      L122:
        _local1 = 0;
        goto L818;
      L129:
        _local2 = 0;
        goto L362;
      L137:
        if (!(_local2 < TGlobalClass.TOTAL_CHINESE_DISH)) goto L184;
        chineseLevel[_local1].splice(_local2, 1, _saveGame.data.chineseLevel[_local1][_local2]);
        goto L212;
      L184:
        chineseLevel[_local1].push(_saveGame.data.chineseLevel[_local1][_local2]);
      L212:
        if (!(_local2 < TGlobalClass.TOTAL_WESTERN_DISH)) goto L258;
        westernLevel[_local1].splice(_local2, 1, _saveGame.data.westernLevel[_local1][_local2]);
        goto L286;
      L258:
        westernLevel[_local1].push(_saveGame.data.westernLevel[_local1][_local2]);
      L286:
        if (!(_local2 < TGlobalClass.TOTAL_MIDEAST_DISH)) goto L332;
        mideastLevel[_local1].splice(_local2, 1, _saveGame.data.mideastLevel[_local1][_local2]);
        goto L360;
      L332:
        mideastLevel[_local1].push(_saveGame.data.mideastLevel[_local1][_local2]);
      L360:
        _local2++;
      L362:
        if (_local2 < _saveGame.data.westernLevel[_local1].length) goto L137;
        _local2 = 0;
        goto L467;
      L390:
        if (!(_local2 < TGlobalClass.TOTAL_INGREDIENT)) goto L437;
        ingredientLevel[_local1].splice(_local2, 1, _saveGame.data.ingredientLevel[_local1][_local2]);
        goto L465;
      L437:
        ingredientLevel[_local1].push(_saveGame.data.ingredientLevel[_local1][_local2]);
      L465:
        _local2++;
      L467:
        if (_local2 < _saveGame.data.ingredientLevel[_local1].length) goto L390;
        _local2 = 0;
        goto L572;
      L495:
        if (!(_local2 < TGlobalClass.TOTAL_HARDWARE)) goto L542;
        hardwareLevel[_local1].splice(_local2, 1, _saveGame.data.hardwareLevel[_local1][_local2]);
        goto L570;
      L542:
        hardwareLevel[_local1].push(_saveGame.data.hardwareLevel[_local1][_local2]);
      L570:
        _local2++;
      L572:
        if (_local2 < _saveGame.data.hardwareLevel[_local1].length) goto L495;
        _local2 = 0;
        goto L795;
      L600:
        if (!(_local2 < TGlobalClass.TOTAL_DAYS)) goto L709;
        daysEarning[_local1].splice(_local2, 1, _saveGame.data.daysEarning[_local1][_local2]);
        goalAchieved[_local1].splice(_local2, 1, _saveGame.data.goalAchieved[_local1][_local2]);
        expertAchieved[_local1].splice(_local2, 1, _saveGame.data.expertAchieved[_local1][_local2]);
        goto L793;
      L709:
        daysEarning[_local1].push(_saveGame.data.daysEarning[_local1][_local2]);
        goalAchieved[_local1].push(_saveGame.data.goalAchieved[_local1][_local2]);
        expertAchieved[_local1].push(_saveGame.data.expertAchieved[_local1][_local2]);
      L793:
        _local2++;
      L795:
        if (_local2 < _saveGame.data.daysEarning[_local1].length) goto L600;
        _local1++;
      L818:
        if (_local1 < 5) goto L129;
        passedDays = _saveGame.data.passedDays;
        if (!(_saveGame.data.money[0] is int)) goto L944;
        _local3 = new Vector.<String>(5, true);
        _local1 = 0;
        goto L920;
      L885:
        _local3[_local1] = Global.getInstance()._maxCrypto.ITS(_saveGame.data.money[_local1]);
        _local1++;
      L920:
        if (_local1 < 5) goto L885;
        _saveGame.data.money = _local3;
        _saveGame.flush();
      L944:
        money = Vector.<String>(_saveGame.data.money);
        lastCustIdx = _saveGame.data.lastCustIdx;
        lastKitchenDay = _saveGame.data.lastKitchenDay;
        openingViewed = Vector.<Boolean>(_saveGame.data.openingViewed);
        endingViewed = Vector.<Boolean>(_saveGame.data.endingViewed);
        _local1 = 0;
        goto L1147;
      L1059:
        if (!_saveGame.data.achievementCounter[_local1]) goto L1097;
        achievementCounter[_local1] = _saveGame.data.achievementCounter[_local1];
      L1097:
        if (!_saveGame.data.unlocked[_local1]) goto L1145;
        unlocked[_local1] = Vector.<Boolean>(_saveGame.data.unlocked[_local1]);
      L1145:
        _local1++;
      L1147:
        if (_local1 < 5) goto L1059;
        _local1 = 0;
        goto L1252;
      L1161:
        _local2 = 0;
        goto L1229;
      L1169:
        if (!(_local2 < Tutorial.TUTORIAL_COUNT)) goto L1216;
        tutorialShowed[_local1].splice(_local2, 1, _saveGame.data.tutorialShowed[_local1][_local2]);
        goto L1227;
      L1216:
        tutorialShowed[_local1].pop();
      L1227:
        _local2++;
      L1229:
        if (_local2 < _saveGame.data.tutorialShowed[_local1].length) goto L1169;
        _local1++;
      L1252:
        if (_local1 < 5) goto L1161;
        if (!(lastPlayer > 0)) goto L1279;
        loadPlayer(lastPlayer - 1);
      L1279:
        muteMusic = _saveGameSetting.data.muteMusic;
        muteSound = _saveGameSetting.data.muteSound;
    }

    static function saveAll():void {
        _saveGame.data.usedSlot = usedSlot.slice();
        _saveGame.data.playerName = playerName.slice();
        _saveGame.data.ingredientLevel = ingredientLevel.slice();
        _saveGame.data.westernLevel = westernLevel.slice();
        _saveGame.data.chineseLevel = chineseLevel.slice();
        _saveGame.data.mideastLevel = mideastLevel.slice();
        _saveGame.data.hardwareLevel = hardwareLevel.slice();
        _saveGame.data.money = money.slice();
        _saveGame.data.lastCustIdx = lastCustIdx.slice();
        _saveGame.data.lastKitchenDay = lastKitchenDay.slice();
        _saveGame.data.openingViewed = openingViewed.slice();
        _saveGame.data.endingViewed = endingViewed.slice();
        _saveGame.data.passedDays = passedDays.slice();
        _saveGame.data.daysEarning = daysEarning.slice();
        _saveGame.data.goalAchieved = goalAchieved.slice();
        _saveGame.data.expertAchieved = expertAchieved.slice();
        _saveGame.data.unlocked = unlocked.slice();
        _saveGame.data.achievementCounter = achievementCounter.slice();
        _saveGame.data.tutorialShowed = tutorialShowed.slice();
        _saveGame.data.lastPlayer = lastPlayer;
        _saveGame.data.fullScreen = fullScreen;
    }

    static function saveSetting():void {
        _saveGameSetting.data.muteMusic = Global.getInstance().MuteMusic;
        _saveGameSetting.data.muteSound = Global.getInstance().MuteSound;
    }

    static function save(arg1:int = -1):void {
        if (arg1 != -1) goto L17;
        arg1 = int(currentPlayer.playerIdx);
      L17:
        playerName[arg1] = currentPlayer.name;
        ingredientLevel[arg1] = currentPlayer.ingredientLevel;
        westernLevel[arg1] = currentPlayer.westernDishLevel;
        chineseLevel[arg1] = currentPlayer.chineseDishLevel;
        mideastLevel[arg1] = currentPlayer.mideastDishLevel;
        hardwareLevel[arg1] = currentPlayer.hardwareLevel;
        passedDays[arg1] = currentPlayer.passedDays;
        daysEarning[arg1] = currentPlayer.daysEarning;
        goalAchieved[arg1] = currentPlayer.goalAchieved;
        expertAchieved[arg1] = currentPlayer.expertAchieved;
        money[arg1] = currentPlayer.money;
        lastCustIdx[arg1] = currentPlayer.lastCustIdx;
        lastKitchenDay[arg1] = currentPlayer.lastDayKitchen;
        openingViewed[arg1] = currentPlayer.openingViewed;
        endingViewed[arg1] = currentPlayer.endingViewed;
        unlocked[arg1] = currentPlayer.unlocked.slice();
        achievementCounter[arg1] = currentPlayer.achievementCounter.slice();
        tutorialShowed[arg1] = currentPlayer.tutorialShowed.slice();
        saveAll();
    }

    static function loadPlayer(arg1:int):void {
        currentPlayer.playerIdx = arg1;
        currentPlayer.name = playerName[arg1];
        currentPlayer.ingredientLevel = ingredientLevel[arg1];
        currentPlayer.westernDishLevel = westernLevel[arg1];
        currentPlayer.chineseDishLevel = chineseLevel[arg1];
        currentPlayer.mideastDishLevel = mideastLevel[arg1];
        currentPlayer.hardwareLevel = hardwareLevel[arg1];
        currentPlayer.passedDays = passedDays[arg1];
        currentPlayer.daysEarning = daysEarning[arg1];
        currentPlayer.goalAchieved = goalAchieved[arg1];
        currentPlayer.expertAchieved = expertAchieved[arg1];
        currentPlayer.money = money[arg1];
        currentPlayer.lastCustIdx = lastCustIdx[arg1];
        currentPlayer.lastDayKitchen = lastKitchenDay[arg1];
        currentPlayer.openingViewed = openingViewed[arg1];
        currentPlayer.endingViewed = endingViewed[arg1];
        currentPlayer.achievementCounter = achievementCounter[arg1].slice();
        currentPlayer.unlocked = unlocked[arg1].slice();
        currentPlayer.tutorialShowed = tutorialShowed[arg1].slice();
        lastPlayer = arg1 + 1;
        save(arg1);
    }

    static function clear(arg1:int):void {
        usedSlot[arg1] = false;
        playerName[arg1] = "";
        if (arg1 != lastPlayer - 1) goto L36;
        lastPlayer = 0;
      L36:
        money[arg1] = "a";
        lastCustIdx[arg1] = 2;
        lastKitchenDay[arg1] = 0;
        openingViewed[arg1] = false;
        endingViewed[arg1] = false;
        var $t0 = new Vector.<int>(13);
        $t0[0] = 1;
        $t0[1] = 1;
        $t0[2] = 1;
        $t0[3] = 1;
        $t0[4] = 1;
        $t0[5] = 1;
        $t0[6] = 1;
        $t0[7] = 1;
        $t0[8] = 1;
        $t0[9] = 1;
        $t0[10] = 1;
        $t0[11] = 1;
        $t0[12] = 1;
        ingredientLevel[arg1] = $t0;
        var $t1 = new Vector.<int>(6);
        $t1[0] = 1;
        $t1[1] = 0;
        $t1[2] = 0;
        $t1[3] = 0;
        $t1[4] = 0;
        $t1[5] = 0;
        westernLevel[arg1] = $t1;
        var $t2 = new Vector.<int>(6);
        $t2[0] = 0;
        $t2[1] = 0;
        $t2[2] = 0;
        $t2[3] = 0;
        $t2[4] = 0;
        $t2[5] = 0;
        chineseLevel[arg1] = $t2;
        var $t3 = new Vector.<int>(6);
        $t3[0] = 0;
        $t3[1] = 0;
        $t3[2] = 0;
        $t3[3] = 0;
        $t3[4] = 0;
        $t3[5] = 0;
        mideastLevel[arg1] = $t3;
        var $t4 = new Vector.<int>(3);
        $t4[0] = 0;
        $t4[1] = 0;
        $t4[2] = 0;
        hardwareLevel[arg1] = $t4;
        passedDays[arg1] = 0;
        daysEarning[arg1] = new Vector.<int>(TGlobalClass.TOTAL_DAYS, false);
        goalAchieved[arg1] = new Vector.<Boolean>(TGlobalClass.TOTAL_DAYS, false);
        expertAchieved[arg1] = new Vector.<Boolean>(TGlobalClass.TOTAL_DAYS, false);
        var $t5 = new Vector.<Boolean>(10);
        $t5[0] = false;
        $t5[1] = false;
        $t5[2] = false;
        $t5[3] = false;
        $t5[4] = false;
        $t5[5] = false;
        $t5[6] = false;
        $t5[7] = false;
        $t5[8] = false;
        $t5[9] = false;
        unlocked[arg1] = $t5;
        var $t6 = new Vector.<int>(10);
        $t6[0] = 0;
        $t6[1] = 0;
        $t6[2] = 1;
        $t6[3] = 0;
        $t6[4] = 0;
        $t6[5] = 0;
        $t6[6] = 0;
        $t6[7] = 0;
        $t6[8] = 0;
        $t6[9] = 0;
        achievementCounter[arg1] = $t6;
        tutorialShowed[arg1] = new Vector.<Boolean>(Tutorial.TUTORIAL_COUNT, false);
        saveAll();
    }

    static function saveAchievement():void {
        achievementCounter[currentPlayer.playerIdx] = currentPlayer.achievementCounter.slice();
        unlocked[currentPlayer.playerIdx] = currentPlayer.unlocked.slice();
        _saveGame.data._achievementCounter = achievementCounter.slice();
        _saveGame.data.unlocked = unlocked.slice();
    }
}
}
