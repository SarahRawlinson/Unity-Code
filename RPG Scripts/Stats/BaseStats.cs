using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RPG.Attributes;
using RPG.Core;
using RPG.Saving;
using System;
//using RPG.Movement;



namespace RPG.Stats
{
    //[ExecuteAlways]
    [DisallowMultipleComponent]
    //[RequireComponent(typeof(Health))]
    //[RequireComponent(typeof(Mover))]
    public class BaseStats : MonoBehaviour, ISaveable, IModifierProvider
    {
        [SerializeField] string charactorName;
        [SerializeField] Progression progression = null;
        [SerializeField] Levels.CharactorLevels startingLevel;
        [SerializeField] CharactorType charactor = new CharactorType();
        [SerializeField] BaseCharator startStats;
        [SerializeField] bool baseStatOveride = false;
        
        public CharactorType Charactor { get => charactor; set => charactor = value; }
        public string CharactorName { get => charactorName; set => charactorName = value; }
        public Levels.CharactorLevels CurrentLevel { get => currentLevel; }

        private BaseCharator baseStats;
        private Levels.CharactorLevels currentLevel;
        public event Action onLoad;
        public event Action onLevelUp;
        public event Action onModifierUpdate;
        private Dictionary<Stats, float> addativeStats;
        private Dictionary<Stats, float> PercentageStats;

        public void Awake()
        {
            UpdateBaseStates();
            currentLevel = startingLevel;
            CreateStatDictionary();
        }

        public void Start()
        {
            UpdateBaseStates();
            onLoad();            
        }

        private void CreateStatDictionary()
        {
            addativeStats = new Dictionary<Stats, float>();
            PercentageStats = new Dictionary<Stats, float>();
            foreach (Stats stat in (Stats[])Enum.GetValues(typeof(Stats)))
            {
                addativeStats.Add(stat, 0f);
                PercentageStats.Add(stat, 0f);
            }
        }

        //public void Start()
        //{
        //    Experience experience = GetComponent<Experience>();
        //    if (experience != null)
        //    {
        //        experience.onExperienceGained += TestEvent;
        //    }
        //}

        //public void TestEvent()
        //{
        //    Debug.Log("Event!");
        //}

        public void UpdateBaseStates()
        {
            if (progression != null && !baseStatOveride)
            {
                baseStats = progression.GetBaseCharactor(currentLevel, Charactor);

                if (baseStats == null)
                {
                    baseStats = startStats;
                }
            }
            else baseStats = startStats;
        }        

        public bool LevelUp(bool setAction)
        {
            Levels.CharactorLevels next = progression.GetNextLevel(currentLevel, Charactor);
            if (next == currentLevel) return false;
            currentLevel = next;
            UpdateBaseStates();
            if (setAction) onLevelUp();
            return true;
        }

        public float NextLevelXP()
        {
            return progression.GetXPLevelUp(currentLevel, Charactor);
        }


        public float GetStatFloat(Stats stat)
        {
            UpdateBaseStates();
            return baseStats.GetStatFloat(stat) + GetAdditiveModifier(stat) * (GetPercentageModifier(stat) / 100);
        }

        private float GetPercentageModifier(Stats stat)
        {
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {                
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    //Debug.Log($"percentage mod for {stat.ToString()} = {modifier}");
                    total += modifier;
                }
            }
            return total;
        }

        public object CaptureState()
        {
            //return baseStats;
            return currentLevel;
        }

        public void RestoreState(object state)
        {
            //baseStats = (BaseCharator)state;
            startingLevel = (Levels.CharactorLevels)state;
            currentLevel = (Levels.CharactorLevels)state;
            //if (gameObject.tag == "Player") Debug.Log(startingLevel.ToString());
            baseStats = new BaseCharator();
            //onLoad();
        }

        private float GetAdditiveModifier(Stats stat)
        {
            float total = 0;
            foreach(IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        public IEnumerator TimedStatEffectPercentage(Stats stat, float value, float time)
        {
            PercentageStats[stat] += value;
            //Debug.Log($"value given = {value}, total = {PercentageStats[stat]}");
            onModifierUpdate();
            Debug.Log("Effect Percentage On");
            yield return new WaitForSeconds(time);
            Debug.Log("Effect Percentage Off");
            PercentageStats[stat] -= value;
            onModifierUpdate();
        }

        public IEnumerator TimedStatEffectAddative(Stats stat, float value, float time)
        {
            addativeStats[stat] += value;
            onModifierUpdate();
            Debug.Log("Effect Addative On");
            yield return new WaitForSeconds(time);
            Debug.Log("Effect Addative Off");
            addativeStats[stat] -= value;
            onModifierUpdate();
        }

        public IEnumerable<float> GetAdditiveModifiers(Stats stat)
        {
            float mod = 0f;
            try
            {
                mod = addativeStats[stat];
            }
            catch(Exception e)
            {
                Debug.Log($"issue accessing dictionary {e.Message}");
                CreateStatDictionary();
            }
            yield return mod;
        }

        public IEnumerable<float> GetPercentageModifiers(Stats stat)
        {
            float mod = 0f;
            try
            {
                mod = PercentageStats[stat];
            }
            catch (Exception e)
            {
                Debug.Log($"issue accessing dictionary {e.Message}");
                CreateStatDictionary();
            }
            yield return mod;
        }

        //#if UNITY_EDITOR
        //        private void Update()
        //        {
        //            //reset name, will maybe create a random name genorator
        //            charactorName = "Bob";
        //        }

        //#endif
    }
}
