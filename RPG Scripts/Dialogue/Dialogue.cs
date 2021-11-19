using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.DialogueInteraction
{
    [CreateAssetMenu(fileName = "DialogueScript", menuName = "RPG Project/Dialogue/New DialogueScript", order = 0)]
    public class Dialogue : ScriptableObject
    {
        [TextArea(3, 10)]
        public string[] sentences;
    }
}
