using System.Collections.Generic;
using Rory.Data;

namespace Rory
{
    /// <summary>
    /// State of the current play session that is not saved (originally the mutable statics of TGlobalClass).
    /// Lives on <see cref="GameManager.Session"/>.
    /// </summary>
    public class GameSession
    {
        /// <summary>
        /// Day being played, 0-based like the original TGlobalClass.currentPlayedLevel (index into
        /// LevelDatabase.days and the per-day lists of PlayerProfile; the next story day is Player.passedDays).
        /// </summary>
        public int currentPlayedLevel;
        /// <summary>Region of the day being played: "W", "C" or "ME".</summary>
        public string currentRegionDish = GameRules.Western;
        /// <summary>True when the day is replayed from the level select (extras / replay mode).</summary>
        public bool extrasMode;
        /// <summary>The kitchen should open the level select instead of the kitchen view.</summary>
        public bool showLevelSelect;
        public bool isPlayingSpecialLevel;
        public bool lastKitchenWindows;
        public int currentNotifWindow;
        /// <summary>Developer cheat unlocked on the title screen (GameScreen.cheatActive).</summary>
        public bool cheatActive;

        // Match-3 day results, read by the result popup and the kitchen.
        public int totalCust;
        public int currentCust;
        public int customerFocus;
        public int daysEarningPrev;
        public int coinsGot;
        public float vipServedRating;
        public int cookiesUsed;
        public int juiceServed;
        public int friesServed;

        /// <summary>Customer indices already used this day (TGlobalClass.tempCustUse).</summary>
        public List<int> tempCustUse = new List<int>();

        // ---- Match-3 (rework/match3)
        /// <summary>
        /// TGlobalClass.dishRegionWestern / dishRegionChinese / dishRegionMidEast: today's dishes per region as global dish
        /// indices (one entry per recipe level). Null until the match-3 screen first fills them with every dish of the
        /// region (Dish.dishLoadComplete); GameScreen.checkDishPriceAndAvaiblity then narrows them and keeps them across
        /// days, like the original statics (it skips the update while a list already has the wanted length).
        /// </summary>
        public List<int> dishRegionWestern, dishRegionChinese, dishRegionMidEast;

        // ---- Achievements screen
        /// <summary>Back on the achievements screen goes to the main menu (true) or the kitchen (false)
        /// (AchievementScreen.backToMainMenu: the main menu sets it to true before opening the screen, the kitchen to false).</summary>
        public bool achievementsBackToMainMenu;

        public DayData CurrentDay
        {
            get
            {
                var days = GameDatabase.Instance.levels.days;
                int i = currentPlayedLevel;
                return i >= 0 && i < days.Count ? days[i] : null;
            }
        }

        /// <summary>Selects a day (0-based) to play and its region (as the level select / kitchen did before starting a day).</summary>
        public void SelectDay(int day)
        {
            currentPlayedLevel = day;
            var d = CurrentDay;
            if (d != null && !string.IsNullOrEmpty(d.region)) currentRegionDish = d.region;
        }

        public void ResetDayStats()
        {
            totalCust = currentCust = customerFocus = coinsGot = 0;
            cookiesUsed = juiceServed = friesServed = 0;
            vipServedRating = 0;
            tempCustUse.Clear();
        }
    }
}
