using System;
using System.Collections.Generic;
using System.Linq;

namespace Rory.Save
{
    /// <summary>One save slot (originally TPlayer + the per-slot arrays of TSavegame). Serialized with JsonUtility.</summary>
    [Serializable]
    public class PlayerProfile
    {
        public string name = "";
        public int slot;
        public int money;
        /// <summary>Number of finished days; the next day to play is passedDays + 1.</summary>
        public int passedDays;

        /// <summary>Recipe levels per region: 0 = not bought, 1..MaxDishLevel = bought / upgraded.</summary>
        public List<int> westernDishLevel = new List<int> { 1, 0, 0, 0, 0, 0 };
        public List<int> chineseDishLevel = Filled(GameRules.DishesPerRegion, 0);
        public List<int> mideastDishLevel = Filled(GameRules.DishesPerRegion, 0);
        public List<int> ingredientLevel = Filled(GameRules.IngredientCount, 1);
        public List<int> hardwareLevel = Filled(GameRules.HardwareCount, 0);

        public List<int> daysEarning = Filled(GameRules.TotalDays, 0);
        public List<bool> goalAchieved = Filled(GameRules.TotalDays, false);
        public List<bool> expertAchieved = Filled(GameRules.TotalDays, false);

        public int lastCustIdx = 2;
        public int lastDayKitchen;
        public bool openingViewed;
        public bool endingViewed;

        public List<bool> unlocked = Filled(GameRules.AchievementCount, false);
        public List<int> achievementCounter = new List<int> { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
        public List<bool> tutorialShowed = Filled(GameRules.TutorialCount, false);

        public static List<T> Filled<T>(int count, T value) => Enumerable.Repeat(value, count).ToList();

        public List<int> DishLevels(string region) => region switch
        {
            GameRules.Chinese => chineseDishLevel,
            GameRules.MidEast => mideastDishLevel,
            _ => westernDishLevel,
        };

        /// <summary>Pads lists that are shorter than the current rules (older saves).</summary>
        public void Normalize()
        {
            Pad(westernDishLevel, GameRules.DishesPerRegion, 0);
            Pad(chineseDishLevel, GameRules.DishesPerRegion, 0);
            Pad(mideastDishLevel, GameRules.DishesPerRegion, 0);
            Pad(ingredientLevel, GameRules.IngredientCount, 1);
            Pad(hardwareLevel, GameRules.HardwareCount, 0);
            Pad(daysEarning, GameRules.TotalDays, 0);
            Pad(goalAchieved, GameRules.TotalDays, false);
            Pad(expertAchieved, GameRules.TotalDays, false);
            Pad(unlocked, GameRules.AchievementCount, false);
            Pad(achievementCounter, GameRules.AchievementCount, 0);
            Pad(tutorialShowed, GameRules.TutorialCount, false);
        }

        static void Pad<T>(List<T> list, int count, T value)
        {
            while (list.Count < count) list.Add(value);
        }
    }

    [Serializable]
    public class GameSettings
    {
        public bool muteMusic;
        public bool muteSound;
        public bool fullScreen = true;
        /// <summary>Slot of the last selected player, -1 = none.</summary>
        public int lastSlot = -1;
    }
}
