using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rory.Data
{
    /// <summary>Root of all design data, loaded from Resources/GameDatabase.</summary>
    [CreateAssetMenu(menuName = "Rory/Game Database")]
    public class GameDatabase : ScriptableObject
    {
        public LevelDatabase levels;
        public DishDatabase dishes;
        public CustomerDatabase customers;
        public LevelEventDatabase levelEvents;
        public DialogueDatabase dialogues;
        public DessertDatabase desserts;
        public List<HiddenObjectLayout> hiddenObjectLayouts = new List<HiddenObjectLayout>();
        public SpriteCatalog sprites;
        public AudioLibrary audio;

        [Header("Shared UI prefabs")]
        public Popups.WindowPopup bigWindow;
        public Popups.WindowPopup smallWindow;

        static GameDatabase instance;

        public static GameDatabase Instance
        {
            get
            {
                if (instance == null) instance = Resources.Load<GameDatabase>("GameDatabase");
                return instance;
            }
        }

        public HiddenObjectLayout GetHiddenObjectLayout(string sceneId) =>
            hiddenObjectLayouts.Find(l => l != null && l.sceneId == sceneId);
    }
}
