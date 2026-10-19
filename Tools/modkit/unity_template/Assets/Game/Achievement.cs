// Ported from Achievement.as (decompiled).
using System;
using System.Collections.Generic;
using Starlite;

public class Achievement
{
    public const int SERVICE_EXELLENCE = 0;
    public const int BIG_BUSINESS = 1;
    public const int WESTERN_RECIPE = 2;
    public const int ASIAN_RECIPE = 3;
    public const int MIDDLE_EAST_RECIPE = 4;
    public const int SECRET_INGREDIENT = 5;
    public const int HAWKEYE = 6;
    public const int FROYO_MASTER = 7;
    public const int QUALIFIED_CHEF = 8;
    public const int INTERNATIONAL_CHEF = 9;

    public static readonly List<int> ALL_TARGET = new List<int> { 1000, 100000, 6, 6, 6, 10, 200, 200, 91, 91 };

    public static readonly List<string> title = new List<string>
    {
        "Service Excellence",
        "Big Business",
        "Western Recipes Book",
        "Asian Recipes Book",
        "Middle East Recipes Book",
        "Secret Ingredient",
        "Hawk Eye",
        "Froyo Master",
        "Qualified Chef",
        "International Chef",
    };

    public static readonly List<string> info = new List<string>
    {
        "You've served 1000 perfectly happy customers\n(80% patience or above)!",
        "You've earned $100.000!",
        "You've bought all Western recipe!",
        "You've bought all Asian recipe!",
        "You've bought all Middle Eastern recipe!",
        "You've upgraded 1 ingredient to MAX Level",
        "You've found 200 Hidden Objects!",
        "You've made 200 Frozen Yogurts!",
        "You've finished 91 levels in Story Mode!",
        "You've achieved Expert scores in 91 levels in Story Mode!",
    };

    public static readonly List<string> infoProgress = new List<string>
    {
        "You've served VALUE/1000 perfectly happy customers\n(80% patience or above)!",
        "You've earned $VALUE/$100.000",
        "You've bought VALUE/6 Western recipe!",
        "You've bought VALUE/6 Asian recipe!",
        "You've bought VALUE/6 Middle Eastern recipe!",
        "You've upgraded an ingredient to VALUE/10 level!",
        "You've found VALUE/200 Hidden Objects!",
        "You've made VALUE/200 Frozen Yogurts!",
        "You've completed VALUE/91 levels in Story Mode!",
        "You've achieved Expert scores in VALUE/91 levels in Story Mode!",
    };

    /// <summary>Not declared in the decompiled source (dynamic/static slot used by the tween functions).</summary>
    public static int achievementID;

    public Achievement()
    {
    }

    public static void tweenAchievementProgress(int arg1)
    {
        achievementID = arg1;
        TGlobalClass._windowDisplay.achievementNotifInfo.text = infoProgress[achievementID];
        TGlobalClass._windowDisplay.achievementNotifInfo.text = TGlobalClass._windowDisplay.achievementNotifInfo.text.replace("VALUE",
            TGlobalClass.addCommas(TSavegame.currentPlayer.achievementCounter[achievementID]));
        TGlobalClass._windowDisplay.achievementNotifTitle.text = title[achievementID];
        tweenNotif();
    }

    public static void tweenAchievementNotif(int arg1)
    {
        achievementID = arg1;
        GALib.TrackEvent(GALib.COMPLETED_AN_ACHIEVEMENT, 0, title[achievementID]);
        TGlobalClass._windowDisplay.achievementNotifInfo.text = info[achievementID];
        TGlobalClass._windowDisplay.achievementNotifTitle.text = title[achievementID];
        tweenNotif();
    }

    /// <summary>Slide-in / hold / slide-out of the notification widget (same closure code in both AS3 functions).</summary>
    static void tweenNotif()
    {
        TweenLite.to(TGlobalClass._windowDisplay.achievementNotif, 1, new Vars
        {
            ["x"] = 850,
            ["ease"] = Expo.easeOut,
            ["onUpdate"] = (Action)followNotif,
            ["onComplete"] = (Action)(() =>
            {
                TweenLite.to(TGlobalClass._windowDisplay.achievementNotif, 1, new Vars
                {
                    ["x"] = 1500,
                    ["delay"] = 1,
                    ["ease"] = Expo.easeIn,
                    ["onUpdate"] = (Action)followNotif,
                });
            }),
        });
    }

    static void followNotif()
    {
        TGlobalClass._windowDisplay.achievementNotifInfo.x = TGlobalClass._windowDisplay.achievementNotif.x;
        TGlobalClass._windowDisplay.achievementNotifTitle.x = TGlobalClass._windowDisplay.achievementNotif.x;
    }
}
