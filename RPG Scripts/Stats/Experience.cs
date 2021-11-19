using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    [DisallowMultipleComponent]
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] GameObject effect;
        public float XP;
        public long killCount;
        private enum saveDataVar { KillXP, KillCount, NextLevel }
        private float nextLevelXP = 0;
        private BaseStats baseStats;
        public bool loaded = false;
        private bool levelCapReached = false;
        //public delegate void ExperienceGainedDelegate();
        //public event Action onExperienceGained;


        public void Awake() //possable race condition
        {
            baseStats = GetComponent<BaseStats>();
            baseStats.onLoad += StartAwake;
        }

        private void Start()
        {
            loaded = true;
        }

        private void StartAwake()
        {
            try
            {
                float xp = baseStats.NextLevelXP();
                if (nextLevelXP < xp) nextLevelXP = xp;
                else nextLevelXP += xp;
            }
            catch (Exception e)
            {
                Debug.Log($"{gameObject.name} could not find Next Level: {e.Message}");
            }
        }


        public void AwardFightXP(float xp, bool kill)
        {
            XP += xp;
            if (kill) killCount += 1;
            if (levelCapReached) return;
            IfLevelUp();
        }

        public long GetKillCount()
        {
            return killCount;
        }

        public float GetXP()
        {
            return XP;
        }

        private bool IfLevelUp()
        {
            
            if (XP > nextLevelXP && nextLevelXP != 0)
            {
                //onExperienceGained();
                LevelUp();
                return true;
            }
            return false;
        }

        private void LevelUp()
        {            
            levelCapReached = !baseStats.LevelUp(loaded);
            nextLevelXP = baseStats.NextLevelXP();
            HealthFX();
            //Debug.Log("Level Up");
        }

        public void HealthFX()
        {
            GameObject fx = Instantiate(effect, gameObject.transform);
        }

        public object CaptureState()
        {
            
            Dictionary<saveDataVar, object> data = new Dictionary<saveDataVar, object>
            {
                [saveDataVar.KillXP] = XP,
                [saveDataVar.KillCount] = killCount,
                [saveDataVar.NextLevel] = nextLevelXP
            };
            return data;
        }

        public void RestoreState(object state)
        {
            loaded = false;
            Dictionary<saveDataVar, object> data = (Dictionary<saveDataVar, object>)state;
            XP = (float)data[saveDataVar.KillXP];
            killCount = (long)data[saveDataVar.KillCount];
            nextLevelXP = (float)data[saveDataVar.NextLevel];
            //while (IfLevelUp())
            //{
            //}
            loaded = true;
        }
    }

}