using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Stats;
using RPG.Attributes;
using System;

//using System.Collections.Generic;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "RPG Project/Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        
        [System.Serializable]
        public class CharactorLevel
        {
            [SerializeField] Levels.CharactorLevels level;
            [SerializeField] float levelXP;
            [SerializeField] Levels.CharactorLevels nextLevel;
            [SerializeField] BaseCharator baseCharators;

            public Levels.CharactorLevels Level { get => level; set => level = value; }
            public BaseCharator BaseCharator { get => baseCharators; set => baseCharators = value; }
            public float LevelXP { get => levelXP; set => levelXP = value; }
            public Levels.CharactorLevels NextLevel { get => nextLevel; set => nextLevel = value; }
        }
        [System.Serializable]
        public class Charactor
        {
            [SerializeField] CharactorType charactorType;
            [SerializeField] List<CharactorLevel> charatorLevels;

            public List<CharactorLevel> CharatorLevels { get => charatorLevels; set => charatorLevels = value; }
            public CharactorType CharactorClass { get => charactorType; set => charactorType = value; }
        }
        [SerializeField] List<Charactor> charators;
        public enum charactorDict { Stat, XPRequired, NextLevel }
        private Dictionary<string, Dictionary<charactorDict, Dictionary<int, object>>> charactorDictionary = null;
        

        public float ReturnStatFloat(Stats stat, Levels.CharactorLevels charactorLevel, CharactorType charactorClass)
        {
            return GetBaseCharactor(charactorLevel, charactorClass).GetStatFloat(stat);
        }

        public BaseCharator GetBaseCharactor(Levels.CharactorLevels charactorLevel, CharactorType charactorType)
        {
            BuildDictionary();
            if (!LevelsDictionary(charactorType))
            {
                //Debug.LogError("No level for " + charactorType.ToString() + " default BaseCharator created");
                return new BaseCharator();
            }
            try
            {
                return (BaseCharator)charactorDictionary[charactorType.ToString()][charactorDict.Stat][Convert.ToInt32(charactorLevel)];
            }
            catch
            {
                //Debug.LogError("No BaseCharator for " + charactorLevel.ToString());
                if (charactorLevel > 0) try
                {
                    Levels.CharactorLevels defaultLevel = charactorLevel - 1;
                    Debug.Log($"No BaseCharator for {charactorLevel.ToString()} try existing {defaultLevel.ToString()}");
                    return (BaseCharator)charactorDictionary[charactorType.ToString()][charactorDict.Stat][Convert.ToInt32(defaultLevel)];
                }
                catch { Debug.LogError($"No BaseCharator for {charactorLevel.ToString()} try existing couldnt work out existing"); }                
                try
                {
                    Debug.LogError($"No BaseCharator for {charactorLevel.ToString()} default used");
                    return (BaseCharator)charactorDictionary[charactorType.ToString()][charactorDict.Stat][0];
                }
                catch
                {
                    Debug.LogError("No starting level for " + charactorType.ToString() + " default BaseCharator created");
                    return new BaseCharator();
                }
            }
        }

        private bool LevelsDictionary(CharactorType charactorType)
        {
            try
            {
                object levels = charactorDictionary[charactorType.ToString()];
                return true;
            }
            catch
            {
                return false;
            }
        }

        public float GetXPLevelUp(Levels.CharactorLevels charactorLevel, CharactorType charactorType)
        {
            BuildDictionary();
            if (!LevelsDictionary(charactorType))
            {
                Debug.LogError("No Level for " + charactorType.ToString() + " default 0");
                return 0;
            }            
            try
            {
                object returnValue = charactorDictionary[charactorType.ToString()][charactorDict.XPRequired][Convert.ToInt32(charactorLevel)];
                return float.Parse(returnValue.ToString());
            }
            catch
            {
                Debug.LogError("No LevelUp for " + charactorType.ToString() + " default 0");
                return 0;
            }
        }

        public Levels.CharactorLevels GetNextLevel(Levels.CharactorLevels charactorLevel, CharactorType charactorType)
        {
            BuildDictionary();
            if (!LevelsDictionary(charactorType))
            {
                Debug.LogError("No Level for " + charactorType.ToString() + " default 0");
                return charactorLevel;
            }
            try
            {
                object returnValue = charactorDictionary[charactorType.ToString()][charactorDict.NextLevel][Convert.ToInt32(charactorLevel)];
                return (Levels.CharactorLevels)returnValue;
            }
            catch
            {
                Debug.LogError("No LevelUp for " + charactorType.ToString() + " default 0");
                return charactorLevel;
            }
        }

        private void BuildDictionary()
        {
            if (charactorDictionary != null) return;
            charactorDictionary = new Dictionary<string, Dictionary<charactorDict, Dictionary<int, object>>>();            
            foreach (Charactor charactor in charators)
            {
                Dictionary<charactorDict, Dictionary<int, object>> objectDictionary = new Dictionary<charactorDict, Dictionary<int, object>>();
                Dictionary<int, object> stats = new Dictionary<int, object>();
                Dictionary<int, object> levelUp = new Dictionary<int, object>();
                Dictionary<int, object> nextLevel = new Dictionary<int, object>();

                foreach (CharactorLevel level in charactor.CharatorLevels)
                {
                    stats[Convert.ToInt32(level.Level)] = level.BaseCharator;
                    levelUp[Convert.ToInt32(level.Level)] = level.LevelXP;
                    nextLevel[Convert.ToInt32(level.Level)] = level.NextLevel;
                }

                objectDictionary[charactorDict.Stat] = stats;
                objectDictionary[charactorDict.XPRequired] = levelUp;
                objectDictionary[charactorDict.NextLevel] = nextLevel;

                charactorDictionary[charactor.CharactorClass.ToString()] = objectDictionary;
                //Debug.Log("Build Dictionary: " + charactorDictionary.ToString());
            }

        }

    }
}
