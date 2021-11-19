using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [System.Serializable]
    public class BaseCharator
    {
        [SerializeField] string title;
        [SerializeField] float health = 1;
        [SerializeField] float strength = 1;
        [SerializeField] float speed = 1;
        [SerializeField] int inventarySlots = 1;
        [SerializeField] float experiencePoints = 1;
        [SerializeField] float additionalDamage = 0;

        public string Title { get => title; set => title = value; }
        public float Health { get => health; set => health = value; }
        public float Strength { get => strength; set => strength = value; }
        public float Speed { get => speed; set => speed = value; }
        public int InventarySlots { get => inventarySlots; set => inventarySlots = value; }
        public float ExperiencePoints { get => experiencePoints; set => experiencePoints = value; }
        public float AdditionalDamage { get => additionalDamage; set => additionalDamage = value; }

        public float GetStatFloat(Stats stat)
        {
            switch (stat)
            {
                case Stats.Experience:
                    return experiencePoints;
                case Stats.Health:
                    return health;
                case Stats.Speed:
                    return speed;
                case Stats.Strength:
                    return strength;
                case Stats.InventorySlots:
                    return inventarySlots;
                case Stats.Damage:
                    return additionalDamage;
                default:
                    return 0f;
            }
        }
    }
}
