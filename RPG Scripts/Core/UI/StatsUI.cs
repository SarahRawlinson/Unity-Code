using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Core.UI
{
    public class StatsUI : MonoBehaviour
    {

        public Text kill_text;
        public Text killXP_text;
        public Text main_text;
        public Text damage_text;
        public Text level_text;

        public void Start()
        {
            kill_text.text = "Kills: 0";
            killXP_text.text = "XP: 0";
            damage_text.text = "DMG: 0";
            level_text.text = "Level: 0";
        }

        public void Kills(long killCount, float XP, float DMG, string level)
        {
            kill_text.text = "Kills: " + killCount;
            killXP_text.text = "XP: " + XP;
            damage_text.text = "DMG: " + DMG;
            level_text.text = "Level: " + level;
        }

        public void DisplayMainText(string displayText)
        {
            main_text.text = displayText;
        }


    }
}
