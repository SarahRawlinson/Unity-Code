using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Stats;
using RPG.Attributes;
using RPG.Saving;
using System;
using RPG.Control;
using RPG.DialogueInteraction;


namespace RPG.Combat
{

    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Health))]
    [DisallowMultipleComponent]
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {        
        [SerializeField] public List<CharactorType.CharactorState> attackClasses;
        [SerializeField] public List<CharactorType.CharactorState> allyClasses;
        [SerializeField] public CharactorType.CharactorState thisClass;
        [SerializeField] Transform rightHandTransform;
        [SerializeField] Transform leftHandTransform;
        [SerializeField] string noWeaponWeapon = "Unarmed001";
        [SerializeField] string defaultWeapon = "Unarmed001";
        [SerializeField] bool drawGimbles = false;
        [SerializeField] Color GimbleWeaponRangeColour = Color.white;

        public bool isFighting = false;
        public float WeaponRange { get => weaponRange; }
        public Transform Target { get => targetTransform; }
        public bool IsDead { get => isDead; set => isDead = value; }
        public WeaponHandler WeaponController { get => weaponController;}

        private bool isDead = false;
        private WeaponHandler weaponController = null;
        private float weaponRange;
        private float timeBetweenAttack;
        private float attackSpeed;
        private float attackDammage;
        private Weapon activeWeapon = null;
        private Animator animator;
        private Mover mover;
        private Health health;             
        private Transform targetTransform;
        private float timeSinceLastAttack = 0;        
        private CombatTarget combatTarget;
        private GameObject targetGameObject;

        public void SetWeaponAttributes(Weapon weapon)
        {
            weaponRange = weapon.GetRange();
            timeBetweenAttack = weapon.GetTimeBetweenAttack();
            attackSpeed = weapon.GetAttackSpeedFraction();
            attackDammage = weapon.GetDammage();
        }

        private void Awake()
        {
            //StartFighter();
            GetComponent<BaseStats>().onLevelUp += UpdateDamage;
            GetComponent<BaseStats>().onModifierUpdate += UpdateDamage;
            GetComponent<BaseStats>().onLoad += StartFighter;
            if (activeWeapon == null) SetDefaultWeapon();
        }

        //to do amend later really dont like this, will probably drop getting weapon damage from weapon controller 
        //just to send it back accumulated with other values i think the weapon controller can manage this alone
        //i think this would be useful for other modifiers but not the weapon damage its self
        public IEnumerable<float> GetAdditiveModifiers(Stats.Stats stat)
        {
            if (weaponController != null && stat == Stats.Stats.Damage)
            {
                yield return weaponController.WeaponDamage;
            }
        }
        public IEnumerable<float> GetPercentageModifiers(Stats.Stats stat)
        {
            if (stat == Stats.Stats.Damage)
            {
                yield return 100;
            }
        }

        private void UpdateDamage()
        {
            //Debug.Log("Update Damage");
            weaponController.DamageAdditional = GetComponent<BaseStats>().GetStatFloat(Stats.Stats.Damage);
        }

        public void CancelAction()
        {
            TriggerAttack("stopAttack", "attack");
            weaponController.CancelAttack();
            targetTransform = null;
            isFighting = false;
            targetGameObject = null;
        }      

        public void StartFighter()
        {
            //Debug.Log($"Fighter started created for {gameObject.name}");
            health = GetComponent<Health>();
            isDead = health.IsDead();
            animator = GetComponent<Animator>();
            mover = GetComponent<Mover>();
            SetUpWeapon();
        }

        private void SetUpWeapon()
        {
            if (weaponController == null)
            {
                weaponController = new WeaponHandler(rightHandTransform, leftHandTransform, animator, gameObject);
            }
            else
            {
                weaponController.ResetHandler();
            }
            if (activeWeapon == null) SetDefaultWeapon();
            EquipWeapon(activeWeapon, "Figher");
        }

        private void SetDefaultWeapon()
        {
            Weapon weapon = Resources.Load<Weapon>(defaultWeapon);
            activeWeapon = weapon;
        }

        public void EquipWeapon(Weapon weapon, string caller)
        {
            if (weaponController == null)
            {
                Debug.Log($"{this.name} has no controller");
                return;
            }
            if (weapon == null)
            {
                Debug.LogError(this.name + " has no weapon to create");
                return;
            }         
            if (isDead)
            {
                //Debug.Log($"{this.name} is dead no weapon loaded");
                try
                {
                    weaponController.DropWeapon();
                }
                catch (Exception E)
                {
                    Debug.LogWarning($"no weapon controller {E.Message}");
                }
                return;
            }
            activeWeapon = weapon;            
            StartCoroutine(weaponController.NewWeapon(weapon, caller));
            SetWeaponAttributes(weapon);
            UpdateDamage(); 
        }        

        public void DropWeapon(string caller)
        {
            defaultWeapon = noWeaponWeapon;
            Weapon weapon = Resources.Load<Weapon>(noWeaponWeapon);            
            EquipWeapon(weapon, caller);
        }

        public Transform TargetTransform()
        {
            return targetGameObject.transform;
        }

        //private void Update()
        //{
        //    FighterUpdate();
        //    Debug.Log($"{gameObject.name} is waiting to attack");
        //}

        public void FighterUpdate()
        {
            isDead = health.IsDead();
            if (isDead) return;
            timeSinceLastAttack += Time.deltaTime;
            //Debug.Log($"Time since last attack = {timeSinceLastAttack} target game object = {targetGameObject.name}");
            if (targetGameObject == null) return;
            if (!IsTarget())
            {
                targetTransform = null; return;
            }
            if (!GetIsInRange())
            {
                //Debug.Log($"{gameObject.name} is moving tward target");
                mover.MoveTo(targetGameObject.transform.position, attackSpeed, weaponRange, "Fighter");
                if (!mover.HasReachedDestination()) return;
            }
            if (timeSinceLastAttack >= timeBetweenAttack)
            {
                //Debug.Log($"{gameObject.name} is fighting");
                Health enemyHealth = targetGameObject.GetComponent<Health>();
                if (enemyHealth.IsDead()) CancelAction();
                Fight();
            }
        }

        private void Fight()
        {
            //Debug.Log($"{gameObject.name} set to fight!");
            transform.LookAt(targetTransform);
            TriggerAttack("attack", "stopAttack");
            isFighting = true;
            weaponController.Activate(combatTarget);
            timeSinceLastAttack = 0;
        }

        public void SetAttackTarget(CombatTarget target)
        {
            combatTarget = target;
            targetGameObject = target.gameObject;
        }

        public void Attack(CombatTarget target)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            combatTarget = target;
            targetGameObject = target.gameObject;   
            targetTransform = target.Colliders.GetTargetOfType(ColliderHandler.BodyPartType.Chest);
        }

        private void TriggerAttack(string start, string stop)
        {
            DialogueTriggerManager dialogue = GetComponent<DialogueTriggerManager>();
            if(dialogue != null) dialogue.Attack();
            animator.ResetTrigger(stop);
            animator.SetTrigger(start);
        }

        private bool GetIsInRange()
        {
            return GetIsInRange(targetTransform);
        }

        public bool GetIsInRange(Transform newTargetTransform)
        {
            return Vector3.Distance(transform.position, newTargetTransform.position) <= weaponRange + (weaponRange / 100 * 5);
        }

        private bool IsTarget()
        {
            CharactorType.CharactorState state = targetGameObject.GetComponent<BaseStats>().Charactor.charactorState;
            //Debug.Log($"Is Target? charactorState = {state.ToString()}");
            //foreach (CharactorType.CharactorState s in attackClasses) Debug.Log(s.ToString());
            return attackClasses.Contains(state);
        }

        // animation event
        public void Hit()
        {
        }
        // animation event
        public void Fire()
        {
        }

        public object CaptureState()
        {
            if (activeWeapon == null) return null;
            return activeWeapon.name;
        }

        public void RestoreState(object state)
        {
            if ((string)state == null)
            {
                Debug.Log($"no weapon saved for {gameObject.name}");
                return;
            }
            activeWeapon = Resources.Load<Weapon>((string)state);
            //StartFighter();
        }

        private void OnDrawGizmos()
        {
            if (drawGimbles)
            {
                Gizmos.color = GimbleWeaponRangeColour;
                Gizmos.DrawWireSphere(transform.position, weaponRange);
            }
        }        
    }
}
