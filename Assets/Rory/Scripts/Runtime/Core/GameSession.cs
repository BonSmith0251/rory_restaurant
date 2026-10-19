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
        /// <summary>Day being played, 1-based (TGlobalClass.currentPlayedLevel).</summary>
        public int currentPlayedLevel = 1;
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

        public DayData CurrentDay
        {
            get
            {
                var days = GameDatabase.Instance.levels.days;
                int i = currentPlayedLevel - 1;
                return i >= 0 && i < days.Count ? days[i] : null;
            }
        }

        /// <summary>Selects a day to play and its region (as the level select / kitchen did before starting a day).</summary>
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
