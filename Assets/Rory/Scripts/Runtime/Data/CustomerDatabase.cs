using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rory.Data
{
    [CreateAssetMenu(menuName = "Rory/Customer Database")]
    public class CustomerDatabase : ScriptableObject
    {
        public List<CustomerData> customers = new List<CustomerData>();
    }
}
