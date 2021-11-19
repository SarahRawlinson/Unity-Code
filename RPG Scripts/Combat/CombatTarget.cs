using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;
using RPG.Attributes;
using RPG.Saving;
using RPG.Control;
using RPG.Core.UI;
using RPG.Actions;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(BaseStats))]
    [RequireComponent(typeof(ColliderHandler))]
    [DisallowMultipleComponent]
    public class CombatTarget : MonoBehaviour, /*ISaveable,*/ IActionable
    {
        ColliderHandler colliders;
        private Health health;

        public ColliderHandler Colliders { get => colliders; }

        public void Start()
        {
            EnableColliders();            
        }

        private void EnableColliders()
        {
            colliders = GetComponent<ColliderHandler>();
            health = GetComponent<Health>();
            colliders.CollidersEnabled(true);
        }

        public void Update()
        {
            if (health.IsDead())
            {
                colliders.CollidersEnabled(false);
            }
        }

        //public object CaptureState()
        //{
        //    return null;
        //}

        //public void RestoreState(object state)
        //{
        //    EnableColliders();
        //}

        public CursorUI GetCursor()
        {
            return CursorUI.Combat;
        }

        public bool HandleRayCast(RPGCharactorController controller)
        {
            return controller.IsEnemy(GetComponent<RPGCharactorController>()) && controller.CanAttack(transform);
        }

        public PlayerActions GetAction()
        {
            return PlayerActions.Attack;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}
