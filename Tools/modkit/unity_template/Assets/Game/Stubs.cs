// Analytics (GameAnalytics) and ads/connectivity helpers from the AIR build. The Unity port keeps the
// call sites but does nothing: no network traffic, no ads.
public class GALib
{
    public static string buildName;
    public const string EVENT_SESSION_BEGIN = "Session:Begin";
    public const string EVENT_CHECK_CREDITS = "Game:CheckCredits";
    public const string EVENT_PRESS_HINT = "Game:PressHint";
    public const string EVENT_SUBMIT_SCORE = "Game:SubmitScore";
    public const string EVENT_START_LEVEL_STORY = "Game:StartLevelStory";
    public const string EVENT_COMPLETE_STORY = "Game:CompleteStory";
    public const string EVENT_LOSE_STORY = "Game:LoseStory";
    public const string EVENT_START_LEVEL_EXTRAS = "Game:StartLevelExtras";
    public const string EVENT_COMPLETE_EXTRAS = "Game:CompleteExtras";
    public const string EVENT_LOSE_EXTRAS = "Game:LoseExtras";
    public const string ENTERED_ENDING_SCREEN = "EnteredEndingScreen";
    public const string ENTERED_ACHIEVEMENT_ROOM = "EnteredAchievementRoom";
    public const string COMPLETED_AN_ACHIEVEMENT = "CompletedAnAchievement";
    public const string BUY_OR_UPGRADE_RECIPE = "BuyOrUpgradeRecipe";
    public const string EVENT_UNLOCK_FULL_VERSION = "Game:UnlockFullVersion";
    public static bool initGALib = false;

    public static void Init() { initGALib = true; }
    public static void TrackEvent(string arg1, int arg2 = 0, string arg3 = "") { }
}

public class AdsClass
{
    public static Global _global;
    public static bool onCheckConnection;
    public static int connectionProgressCount;
    public static bool displayingAds = false;
    public const string PING_WEBSITE_URL = "http://www.google.com";
    public const int PING_DURATION = 240;

    public static void initAds(int arg1, int arg2) { }
    public static void checkConnection() { }
    public static void checkTimeOut() { }
    public static void forceCancle() { }
    public static void onFailure(Starlite.Event arg1) { }
    public static void onSuccess(Starlite.Event arg1) { }
    public static void kill() { }
    public static void ShowAds() { }
    public static void RemoveAds() { }
}
