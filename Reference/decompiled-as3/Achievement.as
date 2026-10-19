package {
class Achievement {
    static const SERVICE_EXELLENCE:uint = 0;
    static const BIG_BUSINESS:uint = 1;
    static const WESTERN_RECIPE:uint = 2;
    static const ASIAN_RECIPE:uint = 3;
    static const MIDDLE_EAST_RECIPE:uint = 4;
    static const SECRET_INGREDIENT:uint = 5;
    static const HAWKEYE:uint = 6;
    static const FROYO_MASTER:uint = 7;
    static const QUALIFIED_CHEF:uint = 8;
    static const INTERNATIONAL_CHEF:uint = 9;
    static const ALL_TARGET:Vector.<int>;
    static const title:Vector.<String>;
    static const info:Vector.<String>;
    static const infoProgress:Vector.<String>;

    // static initializer
    static {
        SERVICE_EXELLENCE = 0;
        BIG_BUSINESS = 1;
        WESTERN_RECIPE = 2;
        ASIAN_RECIPE = 3;
        MIDDLE_EAST_RECIPE = 4;
        SECRET_INGREDIENT = 5;
        HAWKEYE = 6;
        FROYO_MASTER = 7;
        QUALIFIED_CHEF = 8;
        INTERNATIONAL_CHEF = 9;
        var $t0 = new Vector.<int>(10);
        $t0[0] = 1000;
        $t0[1] = 100000;
        $t0[2] = 6;
        $t0[3] = 6;
        $t0[4] = 6;
        $t0[5] = 10;
        $t0[6] = 200;
        $t0[7] = 200;
        $t0[8] = 91;
        $t0[9] = 91;
        ALL_TARGET = $t0;
        var $t1 = new Vector.<String>(10);
        $t1[0] = "Service Excellence";
        $t1[1] = "Big Business";
        $t1[2] = "Western Recipes Book";
        $t1[3] = "Asian Recipes Book";
        $t1[4] = "Middle East Recipes Book";
        $t1[5] = "Secret Ingredient";
        $t1[6] = "Hawk Eye";
        $t1[7] = "Froyo Master";
        $t1[8] = "Qualified Chef";
        $t1[9] = "International Chef";
        title = $t1;
        var $t2 = new Vector.<String>(10);
        $t2[0] = "You've served 1000 perfectly happy customers\n(80% patience or above)!";
        $t2[1] = "You've earned $100.000!";
        $t2[2] = "You've bought all Western recipe!";
        $t2[3] = "You've bought all Asian recipe!";
        $t2[4] = "You've bought all Middle Eastern recipe!";
        $t2[5] = "You've upgraded 1 ingredient to MAX Level";
        $t2[6] = "You've found 200 Hidden Objects!";
        $t2[7] = "You've made 200 Frozen Yogurts!";
        $t2[8] = "You've finished 91 levels in Story Mode!";
        $t2[9] = "You've achieved Expert scores in 91 levels in Story Mode!";
        info = $t2;
        var $t3 = new Vector.<String>(10);
        $t3[0] = "You've served VALUE/1000 perfectly happy customers\n(80% patience or above)!";
        $t3[1] = "You've earned $VALUE/$100.000";
        $t3[2] = "You've bought VALUE/6 Western recipe!";
        $t3[3] = "You've bought VALUE/6 Asian recipe!";
        $t3[4] = "You've bought VALUE/6 Middle Eastern recipe!";
        $t3[5] = "You've upgraded an ingredient to VALUE/10 level!";
        $t3[6] = "You've found VALUE/200 Hidden Objects!";
        $t3[7] = "You've made VALUE/200 Frozen Yogurts!";
        $t3[8] = "You've completed VALUE/91 levels in Story Mode!";
        $t3[9] = "You've achieved Expert scores in VALUE/91 levels in Story Mode!";
        infoProgress = $t3;
    }

    function Achievement():* {
        super();
    }

    static function tweenAchievementProgress(arg1:int):void {
        achievementID = arg1;
        TGlobalClass._windowDisplay.achievementNotifInfo.text = infoProgress[achievementID];
        TGlobalClass._windowDisplay.achievementNotifInfo.text = TGlobalClass._windowDisplay.achievementNotifInfo.text.replace("VALUE", TGlobalClass.addCommas(TSavegame.currentPlayer.achievementCounter[achievementID]));
        TGlobalClass._windowDisplay.achievementNotifTitle.text = title[achievementID];
        TweenLite.to(TGlobalClass._windowDisplay.achievementNotif, 1, {"x": 850, "ease": Expo.easeOut, "onUpdate": function():void {
            TGlobalClass._windowDisplay.achievementNotifInfo.x = TGlobalClass._windowDisplay.achievementNotif.x;
            TGlobalClass._windowDisplay.achievementNotifTitle.x = TGlobalClass._windowDisplay.achievementNotif.x;
        }, "onComplete": function():void {
            TweenLite.to(TGlobalClass._windowDisplay.achievementNotif, 1, {"x": 1500, "delay": 1, "ease": Expo.easeIn, "onUpdate": function():void {
                TGlobalClass._windowDisplay.achievementNotifInfo.x = TGlobalClass._windowDisplay.achievementNotif.x;
                TGlobalClass._windowDisplay.achievementNotifTitle.x = TGlobalClass._windowDisplay.achievementNotif.x;
            }});
        }});
    }

    static function tweenAchievementNotif(arg1:int):void {
        achievementID = arg1;
        GALib.TrackEvent(GALib.COMPLETED_AN_ACHIEVEMENT, 0, title[achievementID]);
        TGlobalClass._windowDisplay.achievementNotifInfo.text = info[achievementID];
        TGlobalClass._windowDisplay.achievementNotifTitle.text = title[achievementID];
        TweenLite.to(TGlobalClass._windowDisplay.achievementNotif, 1, {"x": 850, "ease": Expo.easeOut, "onUpdate": function():void {
            TGlobalClass._windowDisplay.achievementNotifInfo.x = TGlobalClass._windowDisplay.achievementNotif.x;
            TGlobalClass._windowDisplay.achievementNotifTitle.x = TGlobalClass._windowDisplay.achievementNotif.x;
        }, "onComplete": function():void {
            TweenLite.to(TGlobalClass._windowDisplay.achievementNotif, 1, {"x": 1500, "delay": 1, "ease": Expo.easeIn, "onUpdate": function():void {
                TGlobalClass._windowDisplay.achievementNotifInfo.x = TGlobalClass._windowDisplay.achievementNotif.x;
                TGlobalClass._windowDisplay.achievementNotifTitle.x = TGlobalClass._windowDisplay.achievementNotif.x;
            }});
        }});
    }
}
}
