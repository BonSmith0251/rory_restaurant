using System.Collections.Generic;
using Rory.Localization;

namespace Rory.Screens.Kitchen
{
    /// <summary>
    /// The item lists of the kitchen shops (Kitchen.as: hardwareName, ingredientName, *DishName, posXDif / posYDif)
    /// and their display names. Names are sprite names of the "kitchen" atlas; list indices are the shop indices.
    /// </summary>
    public static class KitchenItems
    {
        public static readonly IReadOnlyList<string> Hardware = new[] { "juicer", "fryer", "cookies" };

        /// <summary>Ingredient shop order (12 items: the cookies ingredient type 10 is not sold). Shop index =
        /// ingredient type - 1 below type 10 and type - 2 above it.</summary>
        public static readonly IReadOnlyList<string> Ingredients = new[]
        {
            "croissant", "selada", "beef", "chicken", "rice", "bacon", "salmon", "seafood", "cheese", "pita_bread", "lamb", "tomato",
        };

        public static readonly IReadOnlyList<string> WesternDishes = new[]
        {
            "veggie_croissant", "deluxe_chicken_croissant", "french_BBQ_combo", "beef_steak", "smokey_salmon", "premium_salmon_steak",
        };

        public static readonly IReadOnlyList<string> ChineseDishes = new[]
        {
            "asian_fried_rice", "thai_fried_rice", "kalasan_fried_chicken", "chinese_pork_special", "fine_imperial_cuisine", "deluxe_BBQ_combo",
        };

        public static readonly IReadOnlyList<string> MidEastDishes = new[]
        {
            "doner_kebab", "gyro_style", "tomato_domas", "tuscan_humus", "baklava", "crown_lamb_roast",
        };

        // Dish picture centre on the plate container (container pixels, the plate's centre is its pivot), per dish.
        public static readonly IReadOnlyList<float> PosXWestern = new[] { 133.5f, 134.5f, 135, 129.5f, 138.5f, 138 };
        public static readonly IReadOnlyList<float> PosYWestern = new[] { 60.5f, 84.5f, 84.5f, 68.5f, 75, 70 };
        public static readonly IReadOnlyList<float> PosXChinese = new[] { 130, 141, 137, 127, 152, 131.5f };
        public static readonly IReadOnlyList<float> PosYChinese = new[] { 67.5f, 72.5f, 81.5f, 78.5f, 52.5f, 77.5f };
        public static readonly IReadOnlyList<float> PosXMidEast = new[] { 134, 131.5f, 133, 128, 128.5f, 131 };
        public static readonly IReadOnlyList<float> PosYMidEast = new[] { 81, 75.5f, 71.5f, 84.5f, 63.5f, 47.5f };

        /// <summary>"word word" to "Word Word" (the original split / charAt / toUpperCase / substr / join loop).</summary>
        public static string CapitalizeWords(string text)
        {
            var words = text.Split(' ');
            for (int k = 0; k < words.Length; k++)
                words[k] = words[k].Length > 0 ? char.ToUpperInvariant(words[k][0]) + words[k].Substring(1) : words[k];
            return string.Join(" ", words);
        }

        /// <summary>English name of an ingredient as the shop builds it: first "_" replaced by a space, "selada" is
        /// "vegetables", then every word capitalised.</summary>
        public static string IngredientEnglish(string sprite)
        {
            int i = sprite.IndexOf('_');
            string text = i >= 0 ? sprite.Substring(0, i) + " " + sprite.Substring(i + 1) : sprite;
            if (text == "selada") text = "vegetables";
            return CapitalizeWords(text);
        }

        /// <summary>English name of a dish: every "_" replaced (the original loops while indexOf("_") &gt; 0), capitalised.</summary>
        public static string DishEnglish(string sprite)
        {
            string text = sprite;
            while (text.IndexOf('_') > 0) text = ReplaceFirst(text, "_", " ");
            return CapitalizeWords(text);
        }

        /// <summary>English name of a hardware item (capitalised as is).</summary>
        public static string HardwareEnglish(string sprite) => CapitalizeWords(sprite);

        static string ReplaceFirst(string text, string a, string b)
        {
            int i = text.IndexOf(a, System.StringComparison.Ordinal);
            return i < 0 ? text : text.Substring(0, i) + b + text.Substring(i + a.Length);
        }

        /// <summary>
        /// The shown (translated) name for an English item name built by <see cref="IngredientEnglish"/>,
        /// <see cref="DishEnglish"/> or <see cref="HardwareEnglish"/>. The literals are listed so the string extractor
        /// finds them; KitchenTests checks that they match the generated names.
        /// </summary>
        public static string Translate(string english)
        {
            switch (english)
            {
                // ingredients
                case "Croissant": return Loc.T("Croissant");
                case "Vegetables": return Loc.T("Vegetables");
                case "Beef": return Loc.T("Beef");
                case "Chicken": return Loc.T("Chicken");
                case "Rice": return Loc.T("Rice");
                case "Bacon": return Loc.T("Bacon");
                case "Salmon": return Loc.T("Salmon");
                case "Seafood": return Loc.T("Seafood");
                case "Cheese": return Loc.T("Cheese");
                case "Pita Bread": return Loc.T("Pita Bread");
                case "Lamb": return Loc.T("Lamb");
                case "Tomato": return Loc.T("Tomato");
                // western
                case "Veggie Croissant": return Loc.T("Veggie Croissant");
                case "Deluxe Chicken Croissant": return Loc.T("Deluxe Chicken Croissant");
                case "French BBQ Combo": return Loc.T("French BBQ Combo");
                case "Beef Steak": return Loc.T("Beef Steak");
                case "Smokey Salmon": return Loc.T("Smokey Salmon");
                case "Premium Salmon Steak": return Loc.T("Premium Salmon Steak");
                // asian
                case "Asian Fried Rice": return Loc.T("Asian Fried Rice");
                case "Thai Fried Rice": return Loc.T("Thai Fried Rice");
                case "Kalasan Fried Chicken": return Loc.T("Kalasan Fried Chicken");
                case "Chinese Pork Special": return Loc.T("Chinese Pork Special");
                case "Fine Imperial Cuisine": return Loc.T("Fine Imperial Cuisine");
                case "Deluxe BBQ Combo": return Loc.T("Deluxe BBQ Combo");
                // middle east
                case "Doner Kebab": return Loc.T("Doner Kebab");
                case "Gyro Style": return Loc.T("Gyro Style");
                case "Tomato Domas": return Loc.T("Tomato Domas");
                case "Tuscan Humus": return Loc.T("Tuscan Humus");
                case "Baklava": return Loc.T("Baklava");
                case "Crown Lamb Roast": return Loc.T("Crown Lamb Roast");
                // hardware
                case "Juicer": return Loc.T("Juicer");
                case "Fryer": return Loc.T("Fryer");
                case "Cookies": return Loc.T("Cookies");
                default: return Loc.T(english);
            }
        }
    }
}
