namespace Rory.Tutorials
{
    /// <summary>
    /// Dialogue ids (Tutorial.as constants) = index into <c>GameDatabase.dialogues.dialogues</c> and into
    /// <c>PlayerProfile.tutorialShowed</c>. The day numbers in the comments are the original 0-based
    /// <c>checkTut(id, day)</c> arguments (= <c>PlayerProfile.passedDays</c> when the tutorial shows).
    /// </summary>
    public static class TutorialIds
    {
        public const int None = -1;
        public const int TUTORIAL_COUNT = 30;

        public const int OPENING_STORY = 0;   // kitchen, day 0
        public const int LEVEL_1 = 1;         // match-3, day 0
        public const int AFTER_LEVEL_1 = 2;   // kitchen, day 1
        public const int AFTER_LEVEL_2 = 3;   // kitchen, day 2 (recipe shop)
        public const int LEVEL_4 = 4;         // hidden object, day 3 (forced)
        public const int LEVEL_5 = 5;         // match-3, day 4
        public const int LEVEL_7 = 6;         // match-3, day 6 (food critic)
        public const int AFTER_LEVEL_7 = 7;   // kitchen, day 7
        public const int LEVEL_8 = 8;         // match-3, day 7
        public const int AFTER_LEVEL_8 = 9;   // kitchen, day 8 (ingredient shop)
        public const int LEVEL_11 = 10;       // dessert, day 10
        public const int LEVEL_12 = 11;       // match-3, day 11 (beverage)
        public const int LEVEL_13 = 12;       // match-3, day 12
        public const int LEVEL_14 = 13;       // match-3, day 13
        public const int LEVEL_16 = 14;       // match-3, day 15
        public const int LEVEL_17 = 15;       // match-3, day 16 (fries)
        public const int LEVEL_18 = 16;       // hidden object, day 17
        public const int LEVEL_19 = 17;       // match-3, day 18
        public const int LEVEL_21 = 18;       // match-3, day 20
        public const int LEVEL_22 = 19;       // match-3, day 21
        public const int LEVEL_25 = 20;       // dessert, day 24 (forced)
        public const int LEVEL_26 = 21;       // match-3, day 25 (frozen tiles)
        public const int LEVEL_27 = 22;       // match-3, day 26 (chef's torch)
        public const int LEVEL_28 = 23;       // match-3 (no checkTut call in the original)
        public const int LEVEL_31 = 24;       // match-3, day 30 (cookies)
        public const int LEVEL_32 = 25;       // hidden object, day 31
        public const int AFTER_LEVEL_34 = 26; // kitchen, day 34 (hardware shop)
    }

    /// <summary>
    /// Button / item ids that <see cref="TutorialManager.CanClickThisButton"/> compares against, with the original values.
    /// Kitchen ids are Kitchen.as constants (the quads' names / bubble idx); dessert ids are the ingredient numbers of
    /// MiniGameMakeDessert (the touched quad's name) used by the LEVEL_11 rules.
    /// </summary>
    public static class TutorialButtons
    {
        /// <summary>Kitchen.swipe passes -1 (no button): blocked while a rule restricts the clickable button.</summary>
        public const int NONE = -1;

        // Kitchen (Kitchen.as)
        public const int PLAY_ID = 1;
        public const int SHOP_ID = 2;
        public const int ACHIEVEMENT_ID = 3;
        public const int RECIPE_ID = 4;
        public const int BACK_BUTTON = 5;
        public const int WESTERN_BUTTON = 6;
        public const int CHINESE_BUTTON = 7;
        public const int BUY_BUTTON = 8;
        public const int LEFT_BUTTON = 9;
        public const int RIGHT_BUTTON = 10;
        public const int HARDWARE_ID = 11;
        public const int MID_EAST_BUTTON = 12;
        public const int FREE_COINS_BUTTON = 13;

        // Dessert ingredient ids allowed by LEVEL_11 lines 7..11 (the tutorial order: cup, yogurt, syrup, topping, beverage)
        public const int DESSERT_CUP = 0;      // line 7
        public const int DESSERT_YOGURT = 1;   // line 8
        public const int DESSERT_SYRUP = 3;    // line 9
        public const int DESSERT_TOPPING = 4;  // line 10
        public const int DESSERT_BEVERAGE = 6; // line 11
    }

    /// <summary>
    /// TutorialEvent.TRIGGER_NEXT action names sent by the screens to <see cref="TutorialManager.Trigger"/>, and the
    /// keys of their info dictionaries. See the table at the top of TutorialManager.cs for who sends what.
    /// </summary>
    public static class TutorialActions
    {
        // sent by the tutorial itself (Next button)
        public const string ClickedOk = "clicked_ok";

        // match-3 (GameScreen / TCustomer)
        public const string CustomerIn = "customer_in";               // info: position (customer slot 0..2)
        public const string SpecialCustomerIn = "special_customer_in"; // info: position (food critic, customer 13)
        public const string Highlight = "highlight";                 // info: col, row
        public const string Matched = "matched";
        public const string DishCompleted = "dish_completed";
        public const string ClickedCoin = "clicked_coin";
        public const string ChangedPriority = "changed_priority";
        public const string ClickedBeverage = "clicked_beverage";
        public const string OfferedBeverage = "offered_beverage";
        public const string ClickedFries = "clicked_fries";
        public const string OfferedFries = "offered_fries";
        public const string StartTorch = "start_torch";
        public const string GiveCookies = "give_cookies";

        // kitchen
        public const string ClickedRecipeShop = "clicked_recipe_shop";
        public const string ClickedIngredients = "clicked_ingredients";
        public const string ClickedHardware = "clicked_hardware";
        public const string RightButton = "right_button";
        public const string BuyItem = "buy_item";

        // hidden object (HOClass)
        public const string GetCorrectHoItem = "get_correct_ho_item";
        public const string UsedHint = "used_hint";

        // dessert (MiniGameMakeDessert)
        public const string TouchIngredient = "touch_ingredient";     // info: id (ingredient number)
        public const string CheckTimesUp = "check_times_up";

        // info keys
        public const string KeyPosition = "position";
        public const string KeyCol = "col";
        public const string KeyRow = "row";
        public const string KeyId = "id";
    }
}
