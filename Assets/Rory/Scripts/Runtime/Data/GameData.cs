// Game design data as ScriptableObjects. Generated from the original XML by  Rory > Import Game Data
// (Assets/Rory/Data/Source/*.xml) and editable in the Inspector afterwards.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rory.Data
{
    public enum GameType { Match3, Match3Special, HiddenObject, DessertMaking }

    public static class GameTypes
    {
        public static GameType Parse(string s) => s switch
        {
            "M3S" => GameType.Match3Special,
            "HO" => GameType.HiddenObject,
            "DM" => GameType.DessertMaking,
            _ => GameType.Match3,
        };

        public static string ToCode(GameType t) => t switch
        {
            GameType.Match3Special => "M3S",
            GameType.HiddenObject => "HO",
            GameType.DessertMaking => "DM",
            _ => "M3",
        };
    }

    /// <summary>One day of the story (LevelSet.xml &lt;day&gt;).</summary>
    [Serializable]
    public class DayData
    {
        public GameType gameType;
        [Tooltip("Seconds")] public float time;
        [Tooltip("Customers to serve (M3), items to find (HO), money (DM)")] public int goal;
        public int expert;
        [Tooltip("W = Western, C = Chinese, ME = Middle East; empty for minigames")] public string region;
    }

    [Serializable]
    public class DessertLevelData
    {
        public int goal;
        [Tooltip("Seconds")] public float time;
    }

    [Serializable]
    public class DishData
    {
        [Tooltip("western / chinese / mid_east")] public string region;
        public string dishName;
        public int defaultPrice;
        public int priceIncrease;
        [Tooltip("Number of each ingredient needed, parallel to 'ingredients'")] public List<int> defaultTarget = new List<int>();
        [Tooltip("Ingredient type ids (see IngredientType)")] public List<int> ingredients = new List<int>();
        public List<int> upgradeCost = new List<int>();
        public int expertNeeded;
    }

    [Serializable]
    public class IngredientData
    {
        public List<int> upgradeCost = new List<int>();
        public int priceIncrease;
    }

    [Serializable]
    public class HardwareData
    {
        public string name;
        public List<int> upgradeCost = new List<int>();
        public int defaultPrice;
        public int priceIncrease;
        public int startingTarget;
        public int reduction;
    }

    /// <summary>Ingredient ids used by Dish.xml and the match-3 board.</summary>
    public static class IngredientType
    {
        public const int Bread = 1, Lettuce = 2, Beef = 3, Chicken = 4, Rice = 5, Bacon = 6, Salmon = 7, Seafood = 8,
            Cheese = 9, Cookies = 10, PitaBread = 11, Lamb = 12, Tomato = 13;
    }

    [Serializable]
    public class CustomerData
    {
        public string name;
        [Tooltip("Sprite name in the game atlas")] public string textureName;
        public float patience;
        public float tipsBonus;
    }

    [Serializable]
    public class WindowElement
    {
        public bool visible;
        public float offsetX, offsetY;
        [TextArea] public string text;
    }

    /// <summary>A story/notification window (LevelEvent.xml &lt;window&gt;).</summary>
    [Serializable]
    public class EventWindowData
    {
        public string size = "big";
        public bool alwaysShow;
        public WindowElement title = new WindowElement();
        public WindowElement info = new WindowElement();
        public WindowElement leftButton = new WindowElement();
        public WindowElement rightButton = new WindowElement();
        public WindowElement uniButton = new WindowElement();
        [Tooltip("Sprite name shown in the window (empty = none)")] public string texture;
    }

    [Serializable]
    public class DayEventsData
    {
        public bool alwaysShow;
        public List<EventWindowData> windows = new List<EventWindowData>();
    }

    [Serializable]
    public class DialogueLine
    {
        [Tooltip("button = wait for the button; action = wait for a game action")] public string proceedType;
        public string buttonText;
        public float posX, posY;
        [Tooltip("Arrow position: a number, or a token resolved by the screen (TILEX, HOITEMY, TORCHX, ...). Empty arrowY = no arrow.")]
        public string arrowX, arrowY;
        [Tooltip("-1 = points left (default), 1 = points right")] public int arrowDirection = -1;

        public bool HasArrow => !string.IsNullOrEmpty(arrowY);
        public bool blockNext;
        [TextArea] public string text;
    }

    [Serializable]
    public class DialogueData
    {
        public List<DialogueLine> lines = new List<DialogueLine>();
    }

    [Serializable]
    public class DessertItemData
    {
        public string name;
        [Tooltip("1 cup, 2 yogurt, 3 syrup, 4 topping, 5 beverage")] public int type;
        public float posX, posY;
        public int worth;
    }

    [Serializable]
    public class DessertSetData
    {
        public int level;
        public List<int> types = new List<int>();
    }

    [Serializable]
    public class HiddenObjectPlacement
    {
        [Tooltip("\"name,nm\" (normal) or \"name,bg\" (background part) like the original data")] public string name;
        public float locX, locY;
    }
}
