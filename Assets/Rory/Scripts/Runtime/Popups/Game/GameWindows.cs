using System;
using System.Collections.Generic;
using Rory.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Rory.Localization;

namespace Rory.Popups.Game
{
    /// <summary>
    /// <para>
    /// The shared in-game windows of the original TWindowDisplay, for the match-3, hidden-object (HO), dessert and
    /// kitchen screens. Every method opens a prefab from <see cref="GameWindowsLibrary"/> (Resources/GameWindows) in
    /// the current scene's <see cref="PopupManager"/> (the 70% black backdrop comes with it) and returns the popup.
    /// Windows are positioned on the original widget_BG / small_widget_BG at stage (512, 300).
    /// </para>
    /// <para><b>Conventions</b></para>
    /// <list type="bullet">
    /// <item><c>Session.currentPlayedLevel</c> is 0-based like the original TGlobalClass.currentPlayedLevel (index into
    /// LevelDatabase.days, PlayerProfile.daysEarning / goalAchieved, LevelEventDatabase.days); the original arithmetic is kept.
    /// Day <i>indices</i> taken as parameters (<c>dayIndex</c>) are 0-based, like LevelEvent/LevelSet data.</item>
    /// <item>Buttons that leave the screen (Kitchen, Main Menu, Huraay, Next Level) keep the window open during the fade.
    /// Kitchen always does <c>Session.coinsGot = 0</c> first (TNonStateButton.KITCHEN).</item>
    /// <item>The game screens read nothing from these windows; anything the original read from a screen
    /// (served customers, targets, earnings, ...) is passed in (<see cref="StoryDayResult"/>, <see cref="HiddenObjectResult"/>,
    /// <see cref="DessertResult"/>, <see cref="PauseOptions"/>, <see cref="BeginningWindowOptions"/>), and calls back into
    /// a screen (resume, start level, next HO level) are <see cref="Action"/> callbacks.</item>
    /// <item>Dropped (not ported): EXIT_WINDOW (the main menu has its own), the player windows (main menu),
    /// UNLOCK_FULL_VERSION, SHOW_FREE_COINS, VIDEO_ADS_NOT_AVAILABLE, COINS_REWARDED, the Leaderboard / Submit Score buttons
    /// of the summaries, analytics (GALib / ReportGameState).</item>
    /// </list>
    ///
    /// <para><b>Pause</b> (<see cref="ShowPause"/>, TWindowState.PAUSE_GAME). Call when the screen's pause button is
    /// clicked (match-3: not while the day-end summary is pending). Set your own paused flag first.
    /// Buttons: SOUND / MUSIC / FULL SCREEN toggles (ON green / OFF red; stay open, call <see cref="PauseOptions.onMuteChanged"/>);
    /// left = "Kitchen" in story mode -> <see cref="ShowConfirmKitchen"/> on top of the pause, or "Main Menu" in
    /// extras mode -> straight to the main menu; right = "Resume" -> closes, <see cref="PauseOptions.onResume"/>.</para>
    /// <para><b>Confirmations</b> (<see cref="ShowConfirmKitchen"/> / <see cref="ShowConfirmMainMenu"/>, CONFIRM_KITCHEN /
    /// CONFIRM_MAINMENU). Small window over the pause. "No" closes it (the pause stays). "Yes": if
    /// <c>resetGoalOnExit</c> (match-3), not extras mode, <c>Player.daysEarning[day] == 0</c> and <c>day == Player.passedDays</c>
    /// (day = Session.currentPlayedLevel, i.e. the first play of a new day) then <c>Player.goalAchieved[day] = false</c>
    /// (not saved, as in the original); then Kitchen (coinsGot = 0) or Main Menu.</para>
    ///
    /// <para><b>Summaries</b> (one centred button each; values are shown as given):</para>
    /// <list type="table">
    /// <item><term><see cref="ShowStoryLevelComplete"/></term><description>match-3 goal reached. Goal / expert badge, congrats
    /// particles, tip. "Kitchen".</description></item>
    /// <item><term><see cref="ShowStoryLevelFailed"/></term><description>match-3 goal missed. Coins tip. "Kitchen".</description></item>
    /// <item><term><see cref="ShowStoryCompleteAll"/></term><description>last story day done (passedDays == TotalDays).
    /// Badge, particles. "Huraay!!" -> Ending if !Player.endingViewed, else Kitchen.</description></item>
    /// <item><term><see cref="ShowHOLevelComplete"/> / <see cref="ShowHOLevelFailed"/></term><description>story HO day
    /// (found >= goal or not). "Kitchen". Complete shows the badge (expert = itemsLeft == 0); both play the congrats
    /// particles (the original also did on failure).</description></item>
    /// <item><term><see cref="ShowHOExtrasComplete"/></term><description>extras HO level done: "Next Level" -> your
    /// callback (original: currentLevel++ and reload the HO screen). Particles.</description></item>
    /// <item><term><see cref="ShowHOExtrasFailed"/> / <see cref="ShowHOExtrasCompleteAll"/></term><description>extras HO
    /// failed / last level done. "Main Menu" (<see cref="MainMenuButton"/>). CompleteAll has particles.</description></item>
    /// <item><term><see cref="ShowDessertLevelComplete"/> / <see cref="ShowDessertLevelFailed"/></term><description>story
    /// dessert day. "Kitchen". Complete has the badge (expert = earning >= expertTarget) and particles.</description></item>
    /// <item><term><see cref="ShowDessertExtrasComplete"/> / <see cref="ShowDessertExtrasFailed"/></term><description>extras
    /// dessert game. "Main Menu". Complete has particles.</description></item>
    /// </list>
    /// <para>The badge (<see cref="ResultPopup.ShowGoalIcon"/>) plays goal_reached / expert_reached. Apply the day's rewards
    /// (Player.money, Session.coinsGot, daysEarning, passedDays, achievements, save) before opening a summary: "Total Coins"
    /// shows <c>Player.money (+coinsGot)</c>.</para>
    ///
    /// <para><b>Locked recipe</b> (<see cref="ShowExpertNeededInfo"/>, SHOW_EXPERT_NEEDED_INFO): kitchen "Info" button of a
    /// recipe that needs expert badges. "OK" closes.</para>
    ///
    /// <para><b>Day windows</b> (<see cref="ShowBeginningWindows"/>, showBeginningWindow + NEXT_WINDOW): the LevelEvent.xml
    /// windows before a match-3 / HO / dessert day. Each "Continue" (uni button, NEXT_WINDOW) opens the next window of the
    /// day while <c>Player.passedDays &lt; 60</c> and one exists, otherwise closes and calls
    /// <see cref="BeginningWindowOptions.onAllClosed"/> (original: match-3 paused = false and startLevel() unless the
    /// tutorial bubble's button is visible; HO startLevel(); dessert startLevel() unless the tutorial button is visible).
    /// Big windows have widget_BG at scaleY 0.7. A "General Tips" window becomes "Today's Preparation" with the goal /
    /// expert targets and today's dish previews. A "char_N" picture (new customer, N &lt; 13, not on the first day) sets
    /// <c>Player.lastCustIdx</c> (saved) and <c>Session.tempCustUse</c>. <c>Session.currentNotifWindow</c> tracks the shown index.
    /// When to call (screen's job): match-3 if the day has windows and (no tutorial dialogue is running or the day is
    /// alwaysShow), start index 0 when Player.passedDays == day index, else the last window; HO / dessert (story mode only)
    /// if the day has windows and no tutorial text box is shown, start index 0. With no windows, start the level yourself.</para>
    /// <para><b>Kitchen windows</b> (<see cref="ShowKitchenWindows"/>, showKitchenWindow): same chaining for kitchen
    /// notification windows. LevelEvent.xml has none left (all &lt;kitchen&gt; entries are commented out), so the data
    /// model has no list for them; the method takes the windows as a parameter.</para>
    ///
    /// <para><b>HUD badge</b> (<see cref="ShowGoalIconSmall(GoalIconPlace, bool)"/>, showGoalIconSmall): call when the goal
    /// or the expert target is reached during play (match-3: score passes goal / expert; HO: the goal-th object found;
    /// dessert: earnings pass goal / expert); expert = match-3 score >= expertTarget, HO itemFoundCount + 1 >= expertTarget,
    /// dessert earning >= expertTarget. Plays no sound (the screens played goal_reached / expert_reached themselves).</para>
    /// </summary>
    public static class GameWindows
    {
        /// <summary>gameScreen.customerInUse: customer texture numbers in the order they are introduced.</summary>
        public static readonly IReadOnlyList<int> CustomerInUse = new[] { 7, 1, 4, 3, 8, 10, 5, 11, 6, 12, 2, 9 };

        /// <summary>Dish sprite names ("game" atlas) by global dish index: western 0-5, chinese 6-11, middle east 12-17.</summary>
        public static readonly IReadOnlyList<string> DishSprites = new[]
        {
            "veggie_croissant", "deluxe_chicken_croissant", "french_BBQ_combo", "beef_steak", "smokey_salmon", "premium_salmon_steak",
            "asian_fried_rice", "thai_fried_rice", "kalasan_fried_chicken", "chinese_pork_special", "fine_imperial_cuisine", "deluxe_BBQ_combo",
            "doner_kebab", "gyro_style", "tomato_domas", "tuscan_humus", "baklava", "crown_lamb_roast",
        };

        public static readonly Vector2 GoalIconSmallMatch3 = new Vector2(363, 25);
        public static readonly Vector2 GoalIconSmallHiddenObject = new Vector2(90, 95);
        public static readonly Vector2 GoalIconSmallDessert = new Vector2(810, 25);

        /// <summary>
        /// The pause window's info text when <see cref="PauseOptions.info"/> is null. The original pause showed the shared
        /// widgetInfo field, i.e. the text of the last day window ("Goal : serve 5 customer(s)\nExpert : earn $220").
        /// Set by <see cref="ShowBeginningWindows"/>; the match-3 screen set "Goal   : N\nExpert : M" before the day windows,
        /// the HO screen "" (assign it like that if no day window is shown).
        /// </summary>
        public static string LastDayInfo { get; set; } = "";

        /// <summary>LevelEvent.xml title (data, not display text) of the window that becomes "Today's Preparation".</summary>
        const string GeneralTipsTitle = "General Tips";

        const string ExpertTip = "Awesome! Couldn't have done it better myself!";
        const string GoalTip = "Well done! Try to reach Expert target to earn even more coins!";
        const string FailTip = "If you reach the normal Daily Goal you will receive 3 coins, but when you reach the Expert Goal you'll earn 6 coins.";

        static GameWindowsLibrary Lib => GameWindowsLibrary.Instance;
        static GameSession Session => GameManager.Instance.Session;
        static Save.PlayerProfile Player => GameManager.Player;

        static T Open<T>(T prefab) where T : Popup
        {
            var popups = PopupManager.Current;
            if (popups == null || prefab == null)
            {
                Debug.LogError($"[Rory] GameWindows: no PopupManager in the scene or missing prefab ({typeof(T).Name})");
                return null;
            }
            return popups.Show(prefab);
        }

        // ================================================================== pause

        /// <summary>Opens the "Paused" window (PAUSE_GAME). See the class summary for the buttons.</summary>
        public static PausePopup ShowPause(PauseOptions options)
        {
            var w = Open(Lib.pause);
            if (w != null) w.Setup(options);
            return w;
        }

        /// <summary>"Go to kitchen?" (CONFIRM_KITCHEN) over the pause. Yes: optional goal reset, coinsGot = 0, Kitchen.</summary>
        public static WindowPopup ShowConfirmKitchen(bool resetGoalOnExit) =>
            Confirm("Go to kitchen?",
                "Are you sure want to exit to the Kitchen? Your progress in this level will not be saved.",
                () =>
                {
                    ResetUnfinishedGoal(resetGoalOnExit);
                    GoToKitchen();
                });

        /// <summary>"Main Menu?" (CONFIRM_MAINMENU). Yes: optional goal reset, Main Menu.</summary>
        public static WindowPopup ShowConfirmMainMenu(bool resetGoalOnExit) =>
            Confirm("Main Menu?",
                "Are you sure want to exit to the Main Menu? Your progress in this level will not be saved.",
                () =>
                {
                    ResetUnfinishedGoal(resetGoalOnExit);
                    GameManager.Instance.Flow.Go(GameScene.MainMenu);
                });

        static WindowPopup Confirm(string titleText, string infoText, Action onYes)
        {
            var w = Open(Lib.confirm);
            if (w == null) return null;
            w.title.text = titleText;
            w.info.text = infoText;
            Bind(w.leftButton, "No", w.Close);
            Bind(w.rightButton, "Yes", onYes);
            Bind(w.uniButton, null, null);
            return w;
        }

        /// <summary>YES_BUTTON of CONFIRM_MAINMENU / CONFIRM_KITCHEN: forget the goal flag of an unfinished first play.</summary>
        static void ResetUnfinishedGoal(bool enabled)
        {
            var p = Player;
            if (!enabled || p == null || Session.extrasMode) return;
            int day = Session.currentPlayedLevel;
            if (day < 0 || day >= p.daysEarning.Count) return;
            if (p.daysEarning[day] == 0 && day == p.passedDays) p.goalAchieved[day] = false;
        }

        /// <summary>TNonStateButton.KITCHEN: Session.coinsGot = 0, then fade to the kitchen.</summary>
        public static void GoToKitchen()
        {
            Session.coinsGot = 0;
            GameManager.Instance.Flow.Go(GameScene.Kitchen);
        }

        /// <summary>TNonStateButton.MAINMENU_BUTTON: main menu in extras mode, otherwise the "Main Menu?" confirmation.</summary>
        public static void MainMenuButton(bool resetGoalOnExit = false)
        {
            if (Session.extrasMode) GameManager.Instance.Flow.Go(GameScene.MainMenu);
            else ShowConfirmMainMenu(resetGoalOnExit);
        }

        // ================================================================== summaries

        static string Dash(int hardware, int served)
        {
            var p = Player;
            return p != null && p.hardwareLevel[hardware] == 0 ? "-" : served.ToString();
        }

        static int Money => Player != null ? Player.money : 0;

        static ResultPopup Result(string titleText, string infoText, float infoY, string labels, string stats, string tips,
            string button, Action onButton)
        {
            var w = Open(Lib.result);
            if (w != null) w.Setup(titleText, infoText, infoY, labels, stats, tips, button, onButton);
            return w;
        }

        /// <summary>STORY_LEVELCOMPLETE: "Congratulations!" for a match-3 day whose goal was reached.</summary>
        public static ResultPopup ShowStoryLevelComplete(StoryDayResult r)
        {
            bool expert = r.earning >= r.expertTarget;
            string labels, stats;
            if (r.specialLevel)
            {
                labels = "Customers Served\t\t :\nBeverage Served\t\t :\nFrench Fries Served\t :\nVIP Rating Bonus\t\t :\n\nToday\u2019s Earnings\t\t :\nTotal Coins\t\t\t :";
                stats = Loc.F("{0}\n{1}\n{2}\n{3}\n\n${4}\n{5} (+{6})", r.servedCustomers, Dash(GameRules.Juicer, r.juiceServed),
                    Dash(GameRules.Fryer, r.friesServed), r.vipBonus, r.earning, Money, r.coinsGot);
            }
            else
            {
                labels = "Customers Served\t\t :\nBeverage Served\t\t :\nFrench Fries Served\t :\n\nToday's Earnings\t\t :\nTotal Coins\t\t\t :";
                stats = Loc.F("{0}\n{1}\n{2}\n\n${3}\n{4} (+{5})", r.servedCustomers, Dash(GameRules.Juicer, r.juiceServed),
                    Dash(GameRules.Fryer, r.friesServed), r.earning, Money, r.coinsGot);
            }
            var w = Result("Congratulations!", "You've reached today's goal!", -170, labels, stats, expert ? ExpertTip : GoalTip,
                "Kitchen", GoToKitchen);
            if (w == null) return null;
            w.ShowGoalIcon(expert);
            w.PlayCongrats();
            return w;
        }

        /// <summary>STORY_LEVELFAILED: "Level Failed!" for a match-3 day whose goal was missed.</summary>
        public static ResultPopup ShowStoryLevelFailed(StoryDayResult r) =>
            Result("Level Failed!", "You've failed to meet today's goal!", -180,
                "Customers Served\t\t :\nBeverage Served\t\t :\nFrench Fries Served\t :\n\nTotal Coins\t\t\t :",
                Loc.F("{0}\n{1}\n{2}\n\n{3} (+{4})", r.servedCustomers, Dash(GameRules.Juicer, r.juiceServed),
                    Dash(GameRules.Fryer, r.friesServed), Money, r.coinsGot),
                FailTip, "Kitchen", GoToKitchen);

        /// <summary>STORY_COMPLETE_ALL: the last story day was completed. "Huraay!!" opens the ending the first time.</summary>
        public static ResultPopup ShowStoryCompleteAll(StoryDayResult r)
        {
            bool expert = r.earning >= r.expertTarget;
            var w = Result("Congratulations!", "You've reached today's goal and finished all levels!", -165,
                "Customers Served\t\t :\nBeverage Served\t\t :\nFrench Fries Served\t :\n\nToday's Earnings\t\t :\nTotal Coins\t\t\t :",
                Loc.F("{0}\n{1}\n{2}\n\n${3}\n{4} (+{5})", r.servedCustomers, Dash(GameRules.Juicer, r.juiceServed),
                    Dash(GameRules.Fryer, r.friesServed), r.earning, Money, r.coinsGot),
                null, "Huraay!!", () =>
                {
                    if (Player != null && !Player.endingViewed) GameManager.Instance.Flow.Go(GameScene.Ending);
                    else GoToKitchen();
                });
            if (w == null) return null;
            w.ShowGoalIcon(expert);
            w.PlayCongrats();
            return w;
        }

        /// <summary>HO_LEVEL_COMPLETE: story hidden-object day, goal reached. Expert badge when every object was found.</summary>
        public static ResultPopup ShowHOLevelComplete(HiddenObjectResult r)
        {
            bool expert = r.itemsLeft == 0;
            var w = Result("Summary", "You've completed this level.", -180,
                "Objects Found\t\t:\nToday's Earning\t:\n\nTotal Coins\t\t:",
                Loc.F("{0}\n${1}\n\n{2} (+{3})", r.itemsFound, r.earning, Money, r.coinsGot),
                expert ? ExpertTip : GoalTip, "Kitchen", GoToKitchen);
            if (w == null) return null;
            w.ShowGoalIcon(expert);
            w.PlayCongrats();
            return w;
        }

        /// <summary>HO_LEVEL_FAILED: story hidden-object day, goal missed (the original plays the congrats particles here too).</summary>
        public static ResultPopup ShowHOLevelFailed(HiddenObjectResult r)
        {
            var w = Result("Level Failed", "You've failed this level.", -180,
                "Objects Found\t:\nToday's Earning\t:\n\nTotal Coins\t:",
                Loc.F("{0}\n${1}\n\n{2} (+{3})", r.itemsFound, r.earning, Money, r.coinsGot),
                FailTip, "Kitchen", GoToKitchen);
            if (w != null) w.PlayCongrats();
            return w;
        }

        /// <summary>HO_LEVEL_COMPLETE_EXTRAS: extras hidden-object level done. "Next Level" calls <paramref name="onNextLevel"/>
        /// (original: _minigameHO.currentLevel + 1 and reload the HO screen).</summary>
        public static ResultPopup ShowHOExtrasComplete(HiddenObjectResult r, Action onNextLevel)
        {
            var w = Result("Summary", "You've completed this level.", -180,
                "Objects Found\t:\nScore\t\t:", Loc.F("{0}\n{1}", r.itemsFound, r.earning), null, "Next Level", onNextLevel);
            if (w != null) w.PlayCongrats();
            return w;
        }

        /// <summary>HO_LEVEL_FAILED_EXTRAS: extras hidden-object level failed. "Main Menu".</summary>
        public static ResultPopup ShowHOExtrasFailed(HiddenObjectResult r) =>
            Result("Level Failed!", "You've failed this level.", -180,
                "Objects Found\t:\nScore\t\t:", Loc.F("{0}\n{1}", r.itemsFound, r.earning), null, "Main Menu", () => MainMenuButton());

        /// <summary>HO_LEVEL_COMPLETE_EXTRAS_ALL: the last extras hidden-object level was completed. "Main Menu".</summary>
        public static ResultPopup ShowHOExtrasCompleteAll(HiddenObjectResult r)
        {
            var w = Result("Congratulations!", "You've completed all levels.", -180,
                "Objects Found\t:\nScore\t\t:", Loc.F("{0}\n{1}", r.itemsFound, r.earning), null, "Main Menu", () => MainMenuButton());
            if (w != null) w.PlayCongrats();
            return w;
        }

        /// <summary>DESSERT_LEVEL_COMPLETE: story dessert day, goal reached.</summary>
        public static ResultPopup ShowDessertLevelComplete(DessertResult r)
        {
            bool expert = r.earning >= r.expertTarget;
            var w = Result("Level Complete!", "You've completed this level.", -180,
                "Froyo Served\t\t:\nToday's Earning\t:\n\nTotal Coins\t\t:",
                Loc.F("{0}\n${1}\n\n{2} (+{3})", r.froyoServed, r.earning, Money, r.coinsGot),
                expert ? ExpertTip : GoalTip, "Kitchen", GoToKitchen);
            if (w == null) return null;
            w.ShowGoalIcon(expert);
            w.PlayCongrats();
            return w;
        }

        /// <summary>DESSERT_LEVEL_FAILED: story dessert day, goal missed.</summary>
        public static ResultPopup ShowDessertLevelFailed(DessertResult r) =>
            Result("Level Failed!", "You've failed this level.", -180,
                "Froyo Served\t:\nToday's Earning\t:\n\nTotal Coins\t:",
                Loc.F("{0}\n${1}\n\n{2} (+{3})", r.froyoServed, r.earning, Money, r.coinsGot),
                FailTip, "Kitchen", GoToKitchen);

        /// <summary>DESSERT_LEVEL_COMPLETE_EXTRAS: extras dessert game won. "Main Menu".</summary>
        public static ResultPopup ShowDessertExtrasComplete(DessertResult r)
        {
            var w = Result("Congratulations!", "You've completed this game.", -180,
                "Froyo Served\t\t:", r.froyoServed.ToString(), null, "Main Menu", () => MainMenuButton());
            if (w != null) w.PlayCongrats();
            return w;
        }

        /// <summary>DESSERT_LEVEL_FAILED_EXTRAS: extras dessert game lost. "Main Menu".</summary>
        public static ResultPopup ShowDessertExtrasFailed(DessertResult r) =>
            Result("You Failed!", "You've failed this game.", -180,
                "Froyo Served\t:", r.froyoServed.ToString(), null, "Main Menu", () => MainMenuButton());

        // ================================================================== kitchen info

        /// <summary>
        /// SHOW_EXPERT_NEEDED_INFO: "Locked!" - the recipe needs <paramref name="expertNeeded"/> expert badges
        /// (Dish.xml expertNeeded; the original read it from the kitchen's buyButton.idx). Shows the player's
        /// International Chef counter as the current badges. "OK" closes.
        /// </summary>
        public static WindowPopup ShowExpertNeededInfo(int expertNeeded)
        {
            var w = Open(Lib.info);
            if (w == null) return null;
            int badges = Player != null ? Player.achievementCounter[Achievements.Achievements.InternationalChef] : 0;
            w.title.text = "Locked!";
            w.info.text = Loc.F("To unlock this recipe, you need to collect {0} expert badges.\nCurrent Expert Badges: {1}", expertNeeded, badges);
            Bind(w.leftButton, null, null);
            Bind(w.rightButton, null, null);
            Bind(w.uniButton, "OK", w.Close);
            return w;
        }

        // ================================================================== day windows

        /// <summary>
        /// Opens LevelEvent window <see cref="BeginningWindowOptions.startIndex"/> of day <paramref name="dayIndex"/>
        /// (0-based, normally Session.currentPlayedLevel) and chains the following ones (NEXT_WINDOW). Returns false (and
        /// calls nothing) when that window does not exist.
        /// </summary>
        public static bool ShowBeginningWindows(int dayIndex, BeginningWindowOptions options)
        {
            var w = GetDayWindow(dayIndex, options.startIndex);
            if (w == null) return false;
            Session.currentNotifWindow = options.startIndex;
            ShowBeginningWindow(dayIndex, options.startIndex, w, options);
            return true;
        }

        static EventWindowData GetDayWindow(int day, int index)
        {
            var days = GameDatabase.Instance.levelEvents.days;
            if (day < 0 || day >= days.Count || index < 0 || index >= days[day].windows.Count) return null;
            return days[day].windows[index];
        }

        static void ShowBeginningWindow(int day, int index, EventWindowData data, BeginningWindowOptions o)
        {
            bool big = data.size != "small";
            var w = Open(big ? Lib.eventBig : Lib.eventSmall);
            if (w == null) return;
            w.Apply(data);
            if (big)
            {
                w.SetBackgroundScaleY(0.7f);
                if (!string.IsNullOrEmpty(data.texture))
                {
                    w.SetUnlockedItem(GameDatabase.Instance.sprites.Get("game", data.texture), 1.2f);
                    NewCustomerSideEffect(data.texture, day);
                }
                // compares the data string (LevelEvent.xml title), not the displayed label
                if (data.title.text == GeneralTipsTitle)
                {
                    w.title.text = "Today's Preparation";
                    w.info.text = Session.isPlayingSpecialLevel
                        ? Loc.F("Goal : serve {0} food critic\nExpert : earn ${1}", o.goalTarget, o.expertTarget)
                        : Loc.F("Goal : serve {0} customer(s)\nExpert : earn ${1}", o.goalTarget, o.expertTarget);
                    w.ShowDishes(DishPreviewSprites(o.todaysDishes));
                }
            }
            LastDayInfo = w.info.text;
            Bind(w.uniButton, null, () => NextBeginningWindow(w, day, o));
        }

        /// <summary>A "char_N" texture introduces customer N: the customer rotation restarts there.</summary>
        static void NewCustomerSideEffect(string texture, int day)
        {
            var p = Player;
            if (p == null || texture.IndexOf("char_", StringComparison.Ordinal) < 0 || day <= 0) return;
            var parts = texture.Split('_');
            if (!int.TryParse(parts[parts.Length - 1], out int custIdx) || custIdx >= 13) return;
            p.lastCustIdx = IndexOf(CustomerInUse, custIdx);
            GameManager.Instance.Saves.Save();
            Session.tempCustUse.Clear();
            for (int i = 0; i < p.lastCustIdx + 1 && i < CustomerInUse.Count; i++) Session.tempCustUse.Add(CustomerInUse[i]);
        }

        static int IndexOf(IReadOnlyList<int> list, int value)
        {
            for (int i = 0; i < list.Count; i++) if (list[i] == value) return i;
            return -1;
        }

        static List<Sprite> DishPreviewSprites(IList<int> dishes)
        {
            var result = new List<Sprite>();
            if (dishes == null) return result;
            var shown = new List<int>();
            foreach (var d in dishes)
            {
                if (shown.Contains(d) || d < 0 || d >= DishSprites.Count) continue;
                shown.Add(d);
                result.Add(GameDatabase.Instance.sprites.Get("game", DishSprites[d]));
            }
            return result;
        }

        static void NextBeginningWindow(DayEventPopup current, int day, BeginningWindowOptions o)
        {
            int next = Session.currentNotifWindow + 1;
            Session.currentNotifWindow = next;
            var data = Player != null && Player.passedDays < 60 ? GetDayWindow(day, next) : null;
            current.Close();
            if (data != null) ShowBeginningWindow(day, next, data, o);
            else o.onAllClosed?.Invoke();
        }

        /// <summary>
        /// showKitchenWindow + NEXT_WINDOW in the kitchen: shows <paramref name="windows"/>[<paramref name="startIndex"/>]
        /// and chains the following ones while Player.passedDays &lt; 60, then calls <paramref name="onAllClosed"/>.
        /// (Original: called on entering the kitchen when passedDays &lt; 60, the day has kitchen windows and
        /// Player.lastDayKitchen == passedDays - 1.) Returns false when there is no such window.
        /// </summary>
        public static bool ShowKitchenWindows(IList<EventWindowData> windows, int startIndex, Action onAllClosed)
        {
            if (windows == null || startIndex < 0 || startIndex >= windows.Count) return false;
            Session.currentNotifWindow = startIndex;
            ShowKitchenWindow(windows, startIndex, onAllClosed);
            return true;
        }

        static void ShowKitchenWindow(IList<EventWindowData> windows, int index, Action onAllClosed)
        {
            var data = windows[index];
            var w = Open(data.size == "small" ? Lib.eventSmall : Lib.eventBig);
            if (w == null) return;
            w.Apply(data);
            Bind(w.uniButton, null, () =>
            {
                int next = Session.currentNotifWindow + 1;
                Session.currentNotifWindow = next;
                w.Close();
                if (Player != null && Player.passedDays < 60 && next < windows.Count) ShowKitchenWindow(windows, next, onAllClosed);
                else onAllClosed?.Invoke();
            });
        }

        // ================================================================== HUD badge

        static GoalIconSmall smallIcon;

        /// <summary>showGoalIconSmall() at the original position of a screen.</summary>
        public static void ShowGoalIconSmall(GoalIconPlace place, bool expert) =>
            ShowGoalIconSmall(expert, place switch
            {
                GoalIconPlace.HiddenObject => GoalIconSmallHiddenObject,
                GoalIconPlace.Dessert => GoalIconSmallDessert,
                _ => GoalIconSmallMatch3,
            });

        /// <summary>
        /// Pops the goal (or expert) badge at stage position <paramref name="stagePosition"/> in the HUD (just below the
        /// popup layer): 0.6x / 50% alpha to 0.35x / opaque in 0.8 s, gone 1 s later. A new call replaces the previous badge.
        /// </summary>
        public static GoalIconSmall ShowGoalIconSmall(bool expert, Vector2 stagePosition)
        {
            var popups = PopupManager.Current;
            if (popups == null || Lib.goalIconSmall == null) return null;
            HideGoalIconSmall();
            var layer = popups.transform.parent;
            smallIcon = UnityEngine.Object.Instantiate(Lib.goalIconSmall, layer, false);
            smallIcon.name = Lib.goalIconSmall.name;
            smallIcon.transform.SetSiblingIndex(popups.transform.GetSiblingIndex());
            smallIcon.Play(expert, stagePosition);
            return smallIcon;
        }

        /// <summary>Removes the HUD badge (the original's summary badge reused the same image).</summary>
        public static void HideGoalIconSmall()
        {
            if (smallIcon != null) UnityEngine.Object.Destroy(smallIcon.gameObject);
            smallIcon = null;
        }

        // ================================================================== helpers

        /// <summary>
        /// Shows <paramref name="button"/> with <paramref name="label"/> (null = hide; the label is kept when only the
        /// action is given for an already visible button) and adds <paramref name="onClick"/>.
        /// Listeners are only added: the popups are fresh prefab instances, and RemoveAllListeners would also drop
        /// RoryButton's click sound.
        /// </summary>
        internal static void Bind(Button button, string label, Action onClick)
        {
            if (button == null) return;
            if (label == null && onClick == null)
            {
                button.gameObject.SetActive(false);
                return;
            }
            if (label != null)
            {
                button.gameObject.SetActive(true);
                var t = button.GetComponentInChildren<TextMeshProUGUI>(true);
                if (t != null) t.text = label;
            }
            if (onClick != null) button.onClick.AddListener(() => onClick());
        }
    }

    /// <summary>Parameters of <see cref="GameWindows.ShowBeginningWindows"/>.</summary>
    public struct BeginningWindowOptions
    {
        /// <summary>First window to show (TGlobalClass.currentNotifWindow). Match-3: 0 on the first play of the day
        /// (Player.passedDays == day index), else the last window of the day. HO / dessert: 0.</summary>
        public int startIndex;
        /// <summary>"Today's Preparation": gameScreen.goalTarget (customers to serve).</summary>
        public int goalTarget;
        /// <summary>"Today's Preparation": gameScreen.expertTarget (money).</summary>
        public int expertTarget;
        /// <summary>"Today's Preparation": today's dishes of the current region as global dish indices, duplicates allowed
        /// (TGlobalClass.dishRegionWestern / Chinese / MidEast). Distinct dishes are shown in order, at most four.
        /// Session.isPlayingSpecialLevel switches "customer(s)" to "food critic".</summary>
        public IList<int> todaysDishes;
        /// <summary>The last window was closed: start the level (see the class summary for what each screen did).</summary>
        public Action onAllClosed;
    }
}
