using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rory.Data
{
    /// <summary>Layout of one hidden-object scene (HO1_A_data.xml ...).</summary>
    [CreateAssetMenu(menuName = "Rory/Hidden Object Layout")]
    public class HiddenObjectLayout : ScriptableObject
    {
        public string sceneId;
        public List<HiddenObjectPlacement> items = new List<HiddenObjectPlacement>();
    }
}
