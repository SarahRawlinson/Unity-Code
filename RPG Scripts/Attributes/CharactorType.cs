using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Attributes
{
    [System.Serializable]
    public class CharactorType //: IEquatable<CharactorType>
    {
        public enum CharactorState { Enemy, Friendly, Player, God };
        //[SerializeField] public CharactorState charactorState;
        public CharactorState charactorState;
        public enum CharactorClass { Alian, Human, Robot};
        //[SerializeField] public CharactorClass charactorClass;
        public CharactorClass charactorClass;
        public enum Job { Guard, Patrol, Aimless, HelpPlayer, Trade, Inform, Mislead, Captin, Crew, Quest }
        public Job job;


        public override string ToString()
        {
            //Debug.Log(charactorClass.ToString() + charactorState.ToString());
            return charactorClass.ToString() + " " + charactorState.ToString();
        }
    }
}
