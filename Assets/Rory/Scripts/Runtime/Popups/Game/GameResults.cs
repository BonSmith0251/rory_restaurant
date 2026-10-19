namespace Rory.Popups.Game
{
    /// <summary>
    /// What a match-3 day summary shows (STORY_LEVELCOMPLETE / STORY_LEVELFAILED / STORY_COMPLETE_ALL).
    /// Fill it after the day's rewards were applied (money / coinsGot), like GameScreen.showWindow() did before
    /// calling showWindow(...). "Total Coins" is read from the current player's money when the window opens.
    /// </summary>
    public struct StoryDayResult
    {
        /// <summary>gameScreen.servedCustomerCount</summary>
        public int servedCustomers;
        /// <summary>TGlobalClass.juiceServed (Session.juiceServed). Shown as "-" while the juicer is not owned.</summary>
        public int juiceServed;
        /// <summary>TGlobalClass.friesServed (Session.friesServed). Shown as "-" while the fryer is not owned.</summary>
        public int friesServed;
        /// <summary>ScoreManager.totalScore: the day's earnings including the VIP bonus. Also decides the expert
        /// badge (earning &gt;= expertTarget).</summary>
        public int earning;
        /// <summary>gameScreen.expertTarget</summary>
        public int expertTarget;
        /// <summary>TGlobalClass.isPlayingSpecialLevel (VIP / food critic day): adds the "VIP Rating Bonus" line to
        /// the level-complete summary. The original reset the flag only after opening the window.</summary>
        public bool specialLevel;
        /// <summary>gameScreen.VIPServeBonus</summary>
        public int vipBonus;
        /// <summary>TGlobalClass.coinsGot (Session.coinsGot): coins earned today, shown as "(+N)".</summary>
        public int coinsGot;
    }

    /// <summary>What a hidden-object summary shows (HO_LEVEL_* windows).</summary>
    public struct HiddenObjectResult
    {
        /// <summary>_HOClass.itemFoundCount</summary>
        public int itemsFound;
        /// <summary>_minigameHO.bonusMoney: "Today's Earning" in story mode, "Score" in extras.</summary>
        public int earning;
        /// <summary>_HOClass.totalItems (objects not found yet). 0 = everything found = expert badge.</summary>
        public int itemsLeft;
        /// <summary>TGlobalClass.coinsGot (Session.coinsGot)</summary>
        public int coinsGot;
    }

    /// <summary>What a dessert-making summary shows (DESSERT_LEVEL_* windows).</summary>
    public struct DessertResult
    {
        /// <summary>_minigameDessert.froyoMadeCount</summary>
        public int froyoServed;
        /// <summary>_minigameDessert.earning. Also decides the expert badge (earning &gt;= expertTarget).</summary>
        public int earning;
        /// <summary>_minigameDessert.expertTarget</summary>
        public int expertTarget;
        /// <summary>TGlobalClass.coinsGot (Session.coinsGot)</summary>
        public int coinsGot;
    }

    /// <summary>Where <see cref="GameWindows.ShowGoalIconSmall(GoalIconPlace, bool)"/> pops the badge.</summary>
    public enum GoalIconPlace
    {
        /// <summary>Stage (363, 25), next to the match-3 goal counter.</summary>
        Match3,
        /// <summary>Stage (90, 95), next to the hidden-object goal counter.</summary>
        HiddenObject,
        /// <summary>Stage (810, 25), next to the dessert earnings.</summary>
        Dessert,
    }
}
