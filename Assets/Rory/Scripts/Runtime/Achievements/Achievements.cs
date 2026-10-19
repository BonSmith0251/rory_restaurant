using Rory.Save;

namespace Rory.Achievements
{
    /// <summary>The ten achievements (Achievement.as): ids, targets and texts. Progress lives in PlayerProfile.</summary>
    public static class Achievements
    {
        public const int ServiceExcellence = 0;
        public const int BigBusiness = 1;
        public const int WesternRecipe = 2;
        public const int AsianRecipe = 3;
        public const int MiddleEastRecipe = 4;
        public const int SecretIngredient = 5;
        public const int HawkEye = 6;
        public const int FroyoMaster = 7;
        public const int QualifiedChef = 8;
        public const int InternationalChef = 9;

        public const int Count = 10;

        public static readonly int[] Target = { 1000, 100000, 6, 6, 6, 10, 200, 200, 91, 91 };

        public static readonly string[] Title =
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

        public static readonly string[] Info =
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

        /// <summary>Progress texts; VALUE is replaced by the counter (with thousands separators).</summary>
        public static readonly string[] InfoProgress =
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

        public static string ProgressText(PlayerProfile p, int id) =>
            InfoProgress[id].Replace("VALUE", GameRules.AddCommas(p.achievementCounter[id]));

        public enum Result { None, Progress, Unlocked }

        /// <summary>
        /// Adds <paramref name="amount"/> to an achievement counter (does nothing once unlocked). Returns Unlocked
        /// when the target is reached, Progress when the counter hits a multiple of <paramref name="progressEvery"/>
        /// (0 = never), else None. The caller shows the notification (see AchievementNotifier) and saves.
        /// </summary>
        public static Result Add(PlayerProfile p, int id, int amount = 1, int progressEvery = 0)
        {
            if (p == null || p.unlocked[id]) return Result.None;
            int before = p.achievementCounter[id];
            p.achievementCounter[id] = before + amount;
            if (p.achievementCounter[id] >= Target[id])
            {
                p.unlocked[id] = true;
                return Result.Unlocked;
            }
            if (progressEvery > 0 && before / progressEvery != p.achievementCounter[id] / progressEvery) return Result.Progress;
            return Result.None;
        }
    }
}
