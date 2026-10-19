using System.Globalization;
using UnityEngine;

namespace Rory
{
    /// <summary>Fixed game rules (originally TGlobalClass / ConstClass constants).</summary>
    public static class GameRules
    {
        public const int SaveSlots = 5;
        public const int TotalDays = 91;
        public const int TutorialCount = 30;
        public const int AchievementCount = 10;

        public const int MaxDishLevel = 4;
        public const int MaxHardwareLevel = 6;
        public const int MaxIngredientLevel = 11;
        public const int IngredientCount = 13;
        public const int HardwareCount = 3;
        public const int DishesPerRegion = 6;

        public const int TileNeededForTorch = 50;

        // Day numbers (1-based, as in the original) at which features unlock.
        public const int UnlockRecipeShop = 2;
        public const int UnlockChineseDish = 7;
        public const int UnlockIngredientShop = 8;
        public const int UnlockBeverage = 11;
        public const int UnlockMatchBonus = 15;
        public const int UnlockFries = 16;
        public const int UnlockMidEastDish = 21;
        public const int UnlockFrozen = 25;
        public const int UnlockTorch = 26;
        public const int UnlockCookies = 30;
        public const int UnlockHardwareShop = 34;
        public const int Match3SpecialEvery = 7;

        // Region codes used by LevelDatabase.days[].region
        public const string Western = "W";
        public const string Chinese = "C";
        public const string MidEast = "ME";

        // Hardware indices (DishDatabase.hardware)
        public const int Juicer = 0;
        public const int Fryer = 1;
        public const int Cookies = 2;

        /// <summary>Inclusive integer random range, like TGlobalClass.RandomRange.</summary>
        public static int RandomRange(int min, int max) => Random.Range(min, max + 1);

        /// <summary>"12,345" style number formatting (TGlobalClass.addCommas).</summary>
        public static string AddCommas(float value)
        {
            bool whole = Mathf.Approximately(value, Mathf.Round(value));
            return value.ToString(whole ? "#,0" : "#,0.00", CultureInfo.InvariantCulture);
        }
    }
}
