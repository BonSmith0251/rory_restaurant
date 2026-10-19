using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rory.Data
{
    [CreateAssetMenu(menuName = "Rory/Dessert Database")]
    public class DessertDatabase : ScriptableObject
    {
        public List<DessertItemData> items = new List<DessertItemData>();
        public List<DessertSetData> sets = new List<DessertSetData>();
    }
}
