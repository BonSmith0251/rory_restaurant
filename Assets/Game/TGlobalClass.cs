// Ported from TGlobalClass and ConstClass (global game state and constants).
using System;
using System.Collections.Generic;
using Starlite;
using Mathf = UnityEngine.Mathf;

public class TGlobalClass
{
    public static TWindow _window;
    public static TWindowDisplay _windowDisplay;
    public static int totalCust;
    public static int currentCust;
    public static int customerFocus;
    public static int daysEarningPrev;
    public static int coinsGot;
    public static int gameMode;
    public static bool loadGame;
    public static bool extrasMode;
    public static bool startedStrategy;
    public static bool startedClassic;
    public static bool startedIngredient;
    public static bool startedEndless;
    public static bool startedTarget;
    public static string gameModeName;
    public static int musicPlayingIdx;
    public static bool isPlayingSpecialLevel;
    public static bool isPlayingSFX;
    public static float VIPServedRating;
    public static int currentPlayedLevel;
    public static bool lastKitchenWindows;
    public static string currentRegionDish;
    public static List<int> dishRegionWestern;
    public static List<int> dishRegionChinese;
    public static List<int> dishRegionMidEast;
    public static List<List<int>> westernUpgradeCost;
    public static List<List<int>> chineseUpgradeCost;
    public static List<List<int>> mideastUpgradeCost;
    public static List<List<int>> ingredientCost;
    public static List<int> tempCustUse;
    public static int MAX_DISH_LEVEL = 4;
    public static int MAX_HARDWARE_LEVEL = 6;
    public static int MAX_INGREDIENT_LEVEL = 11;
    public static int TOTAL_INGREDIENT = 13;
    public static int TOTAL_HARDWARE = 3;
    public static int TOTAL_WESTERN_DISH = 6;
    public static int TOTAL_CHINESE_DISH = 6;
    public static int TOTAL_MIDEAST_DISH = 6;
    public static int TOTAL_DAYS = 91;
    public static int TILE_NEEDED_FOR_TORCH = 50;
    public const int UNLOCK_TORCH_AT = 26;
    public const int UNLOCK_FROZEN_AT = 25;
    public const int UNLOCK_COOKIES_AT = 30;
    public const int UNLOCK_CHINESE_DISH_AT = 7;
    public const int UNLOCK_MID_EAST_DISH_AT = 21;
    public const int UNLOCK_RECIPE_SHOP = 2;
    public const int UNLOCK_INGREDIENT_SHOP = 8;
    public const int UNLOCK_HARDWARE_SHOP = 34;
    public const int UNLOCK_MATCH_BONUS = 15;
    public const int UNLOCK_BEVERAGE_AT = 11;
    public const int UNLOCK_FRIES_AT = 16;
    public const int MATCH_3_SPECIAL_EVERY = 7;
    public static bool showLevelSelect;
    public static XML customerXML;
    public static XML levelSetXML;
    public static XML levelEventXML;
    public static XML dialogueXML;
    public static int currentNotifWindow;
    public static int cookiesUsed;
    public static int juiceServed;
    public static int friesServed;
    public static string PublisherName = "";
    public static bool FreeVersion = false;
    public static bool EnableLeaderboardBtn = false;
    public static bool EnableQuitButton = true;
    public static bool EnableMoreGames = false;
    public static bool EnableFoundItemParticle;
    public static bool EnableHintParticle;
    public static bool EnableCrossPromo = false;
    public static bool EnableFreeCoins = false;
    public static bool VideoReady = true;
    public static bool deactivate;
    public static bool dekstopVersion = true;
    public static bool minimized;

    public static int RandomRange(int arg1, int arg2) => (int)Mathf.Floor(AS3.random() * (1 + arg2 - arg1)) + arg1;

    /// <summary>Formats a number with thousands separators ("12,345", decimals kept with 2 digits).</summary>
    public static string addCommas(float arg1)
    {
        string sign = "";
        if (arg1 < 0)
        {
            sign = "-";
            arg1 = Math.Abs(arg1);
        }
        string s = AS3.str(arg1);
        List<string> parts = s.split(".");
        s = parts[0];
        if (s.Length > 3)
        {
            int first = s.Length % 3;
            string res = s.substr(0, first);
            for (int i = first; i < s.Length; i += 3)
                res = res + ((first == 0 && i == 0) ? "" : ",") + s.substr(i, 3);
            if (parts.Count > 1)
                return parts[1].Length == 1 ? sign + res + "." + parts[1] + "0" : sign + res + "." + parts[1];
            return sign + res;
        }
        if (parts.Count > 1)
            return parts[1].Length == 1 ? sign + s + "." + parts[1] + "0" : sign + s + "." + parts[1];
        return sign + s;
    }
}

public class ConstClass
{
    public const string WESTERN_DISH = "W";
    public const string CHINESE_DISH = "C";
    public const string MID_EAST_DISH = "ME";
    public const int WESTERN_DISH_COUNT = 6;
    public const int CHINESE_DISH_COUNT = 6;
    public const int MID_EAST_DISH_COUNT = 6;
    public const int INGREDIENT_COUNT = 13;
    public const string GAME_TYPE_M3 = "M3";
    public const string GAME_TYPE_HO = "HO";
    public const string GAME_TYPE_SPECIAL_M3 = "M3S";
    public const string GAME_TYPE_DESSERT_MAKING = "DM";
}
