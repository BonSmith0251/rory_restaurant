using System.Collections.Generic;
using Rory.Data;
using Rory.Save;

namespace Rory.Screens.Match3
{
    /// <summary>
    /// Dish.as lookups and the dish choices of a match-3 day: GameScreen.checkDishPriceAndAvaiblity (today's dishes per
    /// region, kept in <see cref="GameSession"/> across days like the TGlobalClass.dishRegion* statics), the tile types
    /// of the day (initialize()) and dish prices. Global dish indices: western 0-5, chinese 6-11, middle east 12-17.
    /// </summary>
    public class DishPlanner
    {
        public const int WesternCount = 6, ChineseCount = 6;

        readonly DishDatabase db;
        readonly PlayerProfile player;
        readonly GameSession session;

        public DishPlanner(DishDatabase db, PlayerProfile player, GameSession session)
        {
            this.db = db;
            this.player = player;
            this.session = session;
            EnsureRegionLists(db, session);
        }

        /// <summary>Dish.dishLoadComplete: every dish index of its region (the start value of the statics).</summary>
        public static void EnsureRegionLists(DishDatabase db, GameSession session)
        {
            if (session.dishRegionWestern != null) return;
            session.dishRegionWestern = new List<int>();
            session.dishRegionChinese = new List<int>();
            session.dishRegionMidEast = new List<int>();
            for (int i = 0; i < db.dishes.Count; i++)
            {
                string region = db.dishes[i].region;
                if (region == "western") session.dishRegionWestern.Add(i);
                else if (region == "chinese") session.dishRegionChinese.Add(i);
                else session.dishRegionMidEast.Add(i);
            }
        }

        // ------------------------------------------------------------------ Dish.as

        public List<int> IngredientTypes(int dish) => new List<int>(db.dishes[dish].ingredients);
        public List<int> Targets(int dish) => new List<int>(db.dishes[dish].defaultTarget);
        public int DefaultPrice(int dish) => db.dishes[dish].defaultPrice;
        public int PriceIncrease(int dish) => db.dishes[dish].priceIncrease;
        public int IngredientPriceIncrease(int ingredient) => db.ingredients[ingredient].priceIncrease;
        public int HardwareDefaultPrice(int hw) => db.hardware[hw].defaultPrice;
        public int HardwarePriceIncrease(int hw) => db.hardware[hw].priceIncrease;
        public int HardwareDefaultTarget(int hw) => db.hardware[hw].startingTarget;
        public int HardwareReduction(int hw) => db.hardware[hw].reduction;

        /// <summary>Tiles needed to fill a machine: startingTarget - (level - 1) * Reduction.</summary>
        public int HardwareTarget(int hw) => HardwareDefaultTarget(hw) - (player.hardwareLevel[hw] - 1) * HardwareReduction(hw);

        /// <summary>Bonus of a machine (juice / fries money, cookie patience %): defaultPrice + (level - 1) * priceIncrease.</summary>
        public int HardwareEffect(int hw) => HardwareDefaultPrice(hw) + (player.hardwareLevel[hw] - 1) * HardwarePriceIncrease(hw);

        // ------------------------------------------------------------------ today's dishes

        /// <summary>TGlobalClass.dishRegion* of the current region (Session.currentRegionDish).</summary>
        public List<int> RegionDishes => RegionList(session.currentRegionDish);

        List<int> RegionList(string region) => region switch
        {
            GameRules.Chinese => session.dishRegionChinese,
            GameRules.MidEast => session.dishRegionMidEast,
            GameRules.Western => session.dishRegionWestern,
            _ => null,
        };

        /// <summary>
        /// checkDishPriceAndAvaiblity(count): keeps the <paramref name="count"/> most expensive owned dishes of the region,
        /// then adds one more copy of a dish per level above 1 (customers pick dishes from this list). Does nothing when
        /// the list already has exactly <paramref name="count"/> entries.
        /// </summary>
        public void CheckDishPriceAndAvailability(int count)
        {
            string region = session.currentRegionDish;
            if (region == GameRules.Western && session.dishRegionWestern.Count == count) return;
            if (region == GameRules.Chinese && session.dishRegionChinese.Count == count) return;
            if (region == GameRules.MidEast && session.dishRegionMidEast.Count == count) return;
            int price = 0; // (sic) accumulated over all dishes, never reset
            int minIdx = 0;
            session.dishRegionChinese.Clear();
            session.dishRegionWestern.Clear();
            session.dishRegionMidEast.Clear();
            int offset;
            List<int> levels, list;
            if (region == GameRules.Western) { offset = 0; levels = player.westernDishLevel; list = session.dishRegionWestern; }
            else if (region == GameRules.Chinese) { offset = WesternCount; levels = player.chineseDishLevel; list = session.dishRegionChinese; }
            else if (region == GameRules.MidEast) { offset = WesternCount + ChineseCount; levels = player.mideastDishLevel; list = session.dishRegionMidEast; }
            else return;

            var prices = new List<int>();
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i] <= 0) continue;
                foreach (int ing in IngredientTypes(i + offset))
                    price += (player.ingredientLevel[ing - 1] - 1) * IngredientPriceIncrease(ing - 1);
                price += DefaultPrice(offset + i) + (levels[i] - 1) * PriceIncrease(offset + i);
                prices.Add(price);
                list.Add(offset + i);
            }
            while (prices.Count > count)
            {
                int minPrice = 1000;
                if (region != GameRules.Western) minIdx = 0; // (sic) the western branch keeps the previous minIdx
                for (int i = 0; i < prices.Count; i++)
                {
                    if (minPrice > prices[i])
                    {
                        minPrice = prices[i];
                        minIdx = i;
                    }
                }
                prices.RemoveAt(minIdx);
                list.RemoveAt(minIdx);
            }
            int n = list.Count;
            for (int i = 0; i < n; i++)
            {
                int level = levels[list[i] - offset];
                for (int extra = level - 1; extra > 0; extra--) list.Add(list[i]);
            }
        }

        /// <summary>initialize(): the ingredient types dropping on the board today (availableTypeAr).</summary>
        public List<int> AvailableTypes()
        {
            var types = new List<int>();
            string region = session.currentRegionDish;
            if (region == GameRules.Western) types.AddRange(new[] { 1, 9, 7, 2, 4, 3 });
            else if (region == GameRules.Chinese) types.AddRange(new[] { 5, 6, 8, 2, 4, 3 });
            else if (region == GameRules.MidEast) types.AddRange(new[] { 11, 12, 13, 2, 4, 5 });
            if (player.passedDays <= 1) types = new List<int> { 1, 9, 7, 2 };
            if (player.passedDays > 1 && player.passedDays <= 5)
            {
                var used = new List<int>();
                var dishes = RegionDishes;
                if (dishes != null)
                    foreach (int d in dishes)
                        foreach (int ing in IngredientTypes(d))
                            if (!used.Contains(ing)) used.Add(ing);
                for (int i = 0; i < types.Count; i++)
                {
                    // (sic) no i-- after removing, so the next type is skipped
                    if (!used.Contains(types[i]) && types.Count > 4) types.RemoveAt(i);
                }
            }
            return types;
        }

        /// <summary>TCustomer.newCust: the dish's price for the player (recipe level), before ingredient upgrades.</summary>
        public int DishPrice(int dish)
        {
            string region = session.currentRegionDish;
            if (region == GameRules.Western)
                return DefaultPrice(dish) + (player.westernDishLevel[dish] - 1) * PriceIncrease(dish);
            if (region == GameRules.Chinese)
                return DefaultPrice(dish) + (player.chineseDishLevel[dish - WesternCount] - 1) * PriceIncrease(dish);
            if (region == GameRules.MidEast)
                // (sic) the original reads chineseDishLevel for middle east dishes
                return DefaultPrice(dish) + (player.chineseDishLevel[dish - WesternCount - ChineseCount] - 1) * PriceIncrease(dish);
            return 0;
        }

        /// <summary>The dish sprite ("game" atlas) of a global dish index (westernDishName / chineseDishName / mideastDishName).</summary>
        public static string DishSprite(int dish) =>
            dish >= 0 && dish < Popups.Game.GameWindows.DishSprites.Count ? Popups.Game.GameWindows.DishSprites[dish] : "veggie_croissant";
    }
}
