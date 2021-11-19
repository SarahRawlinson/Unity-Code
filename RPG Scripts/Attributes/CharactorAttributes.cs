using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    [CreateAssetMenu(fileName = "CharactorAttributes", menuName = "RPG Project/Charactor/New Attributes", order = 0)]
    public class CharactorAttributes : ScriptableObject
    {
        [SerializeField] string charactorName;
        [SerializeField] CharactorType charactor = new CharactorType();
        [SerializeField] CharactorType[] enemyCharactors;
        [SerializeField] GameObject charactorPrefab;
        
        public enum Emotions { Happy, Sad, Curious, Annoyed, Angery, Rage, Fear }
        public enum AgressionState { Passitive, Provoked, Kill, Fear, Defence }
        public enum Controller { Player, AI }
    }
}
