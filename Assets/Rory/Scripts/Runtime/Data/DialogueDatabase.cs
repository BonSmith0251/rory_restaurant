using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rory.Data
{
    [CreateAssetMenu(menuName = "Rory/Dialogue Database")]
    public class DialogueDatabase : ScriptableObject
    {
        public List<DialogueData> dialogues = new List<DialogueData>();
    }
}
