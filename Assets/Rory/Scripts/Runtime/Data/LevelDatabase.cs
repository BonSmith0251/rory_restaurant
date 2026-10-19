using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rory.Data
{
    [CreateAssetMenu(menuName = "Rory/Level Database")]
    public class LevelDatabase : ScriptableObject
    {
        public List<DayData> days = new List<DayData>();
        public List<DessertLevelData> dessertLevels = new List<DessertLevelData>();
        public int TotalDays => days.Count;
    }
}
