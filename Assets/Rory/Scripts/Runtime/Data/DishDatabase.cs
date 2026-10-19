using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rory.Data
{
    [CreateAssetMenu(menuName = "Rory/Dish Database")]
    public class DishDatabase : ScriptableObject
    {
        public List<DishData> dishes = new List<DishData>();
        public List<IngredientData> ingredients = new List<IngredientData>();
        public List<HardwareData> hardware = new List<HardwareData>();
    }
}
