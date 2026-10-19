using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rory.Data
{
    [CreateAssetMenu(menuName = "Rory/Level Event Database")]
    public class LevelEventDatabase : ScriptableObject
    {
        public List<DayEventsData> days = new List<DayEventsData>();
    }
}
