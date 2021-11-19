using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;
using RPG.States;
using RPG.Movement;
using RPG.Combat;
using System;
using RPG.Observation;
using RPG.Core;

namespace RPG.Control
{
    [RequireComponent(typeof(Fighter))]
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Death))]
    [RequireComponent(typeof(Idle))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Observer))]
    [RequireComponent(typeof(SoundAlerts))]
    [RequireComponent(typeof(ColliderHandler))]
    [DisallowMultipleComponent]
    public class RPGCharactorController : MonoBehaviour
    {
        [SerializeField] string[] targetTagsAll = new string[] { "Player", "Enemy", "Friendly" };
        internal Health health;
        private Fighter fighter;
        private Mover mover;
        private Idle idle;
        private Death death;
        private ColliderHandler colliderHandler;
        //internal SavingWrapper saver;
        internal Animator animator;
        internal bool loaded = false;
        public event Action onIdle;
        public event Action onMoving;
        public event Action onFighting;
        public event Action onDeath;
        public event Action onAlert;
        public event Action onTakingDamageFromAlly;
        public event Action onTakingDamageFromEnemy;
        public event Action onBumpingAlly;
        public event Action onBumpingEnemy;
        internal event Action<Transform> onBeingAttacked;
        public enum Actions { Idle, Move, Fight, Death, Alert }
        public Actions currentAction = Actions.Idle;
        public Observer observer;
        internal Ray mouseRay { get => GetMouseRay(); }
        internal List<GameObject> VisibleTargets { get => GetComponent<Observer>().VisibleTargets; }
        internal List<GameObject> AwareTargets { get => GetComponent<Observer>().AwareTargets; }
        internal List<GameObject> CloseRangeTargets { get => GetComponent<Observer>().CloseRangeTargets; }
        internal List<GameObject> HearRangeTargets { get => GetComponent<Observer>().HearRangeTargets; }
        internal List<GameObject> SoundRangeTargets { get => GetComponent<Observer>().SoundRangeTargets; }
        internal List<GameObject> Charactors { get => charactors; set => charactors = value; }
        internal List<GameObject> charactors = new List<GameObject>();
        internal Levels.CharactorLevels level { get => GetComponent<BaseStats>().CurrentLevel; }
        internal RPGCharactorController AllyDamaging;
        internal RPGCharactorController EnemyDamaging;
        internal RPGCharactorController AllyBumping;
        internal RPGCharactorController EnemyBumping;
        internal Transform BodyPartTarget(GameObject character, ColliderHandler.BodyPartType bodypart)
        {
            return character.GetComponent<ColliderHandler>().GetTargetOfType(bodypart);
        }
        internal void CollidersOn(bool on)
        {
            colliderHandler.CollidersEnabled(on);
        }
        internal void SetMoverOnCamera(bool on)
        {
            mover.onCamera = on;
        }
        private void Alert(Transform target)
        {
            onBeingAttacked(target);
        }

        internal void Idle()
        {
            if (currentAction != Actions.Idle)
            {
                currentAction = Actions.Idle;
                onIdle?.Invoke();
            }            
            idle.OnIdle();
        }

        public bool CanAttack(Transform attackTargetTransform)
        {
            return fighter.GetIsInRange(attackTargetTransform);
        }

        internal float GetDamage()
        {
            return fighter.WeaponController.DamageAdditional;
        }

        internal void RPGCharactorUpdate()
        {
            fighter.FighterUpdate();
            mover.UpdateMover();
        }

        internal void Alert()
        {
            if (currentAction != Actions.Alert)
            {
                currentAction = Actions.Alert;
                onAlert?.Invoke();
            }            
        }

        internal bool MovingControl(Vector3 target)
        {
            //mover.UpdateAnimation();
            if (currentAction != Actions.Move)
            {
                currentAction = Actions.Move;
                onMoving?.Invoke();
            }
            //onMoving();
            mover.MoveControl(target);
            return !mover.HasReachedDestination();
        }

        internal bool MovingOverride(Vector3 target)
        {
            mover.CancelAction();
            
            return Moving(target);
        }

        internal bool Moving(Vector3 target)
        {
            //mover.UpdateAnimation();
            if (currentAction != Actions.Move)
            {
                currentAction = Actions.Move;
                onMoving?.Invoke();
            }
            //onMoving();
            mover.StartMoveAction(target, "RPG Controller");
            return !mover.HasReachedDestination();
        }

        internal bool IsMoving()
        {
            return !mover.HasReachedDestination();
        }

        internal void Fighting(CombatTarget target)
        {
            //fighter.FighterUpdate();
            //Debug.Log($"{transform.parent.name} has targeted {target.transform.parent.name}");
            if (currentAction != Actions.Fight)
            {
                currentAction = Actions.Fight;
                onFighting?.Invoke();
                //Debug.Log($"{gameObject.name} fight!");
            }
            else
            {
                //Debug.Log($"{gameObject.name} already fighting!");
            }
            fighter.Attack(target);            
        }

        internal bool AwareAllyFighting()
        {
            foreach (GameObject gObject in AwareTargets)
            {
                RPGCharactorController controller = gObject.GetComponent<RPGCharactorController>();
                if (controller.fighter.isFighting && IsAlly(controller))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool IsAlly(RPGCharactorController controller)
        {
            return !controller.IsDead() && fighter.allyClasses.Contains(controller.fighter.thisClass);
        }

        internal bool IsEnemy(RPGCharactorController controller)
        {
            return GetComponent<Fighter>().attackClasses.Contains(controller.GetComponent<Fighter>().thisClass);
        }

        internal void Death()
        {
            if (currentAction != Actions.Death)
            {
                currentAction = Actions.Death;
                onDeath?.Invoke();
            }
            fighter.IsDead = true;
            //Debug.Log("Weapon to Drop");
            DropWeapon();
            //Debug.Log("Weapon dropped");
            death.OnDeath(true);
        }

        internal bool FighterTarget()
        {
            //Debug.Log($"{gameObject.name} has target = {fighter.Target != null}");
            return fighter.Target != null;
        }


        internal void GetBaseComponentObjects()
        {            
            health = GetComponent<Health>();            
            fighter = GetComponent<Fighter>();            
            mover = GetComponent<Mover>();            
            idle = GetComponent<Idle>();            
            death = GetComponent<Death>();
            animator = GetComponent<Animator>();
            observer = GetComponent<Observer>();
            colliderHandler = GetComponent<ColliderHandler>();
            colliderHandler.OnBumping += Bumping;
            colliderHandler.OnTakingDamage += TakingDamage;
            health.onBeingAttacked += Alert;
            foreach (string targetTag in targetTagsAll)
            {
                GameObject[] newCharactors = GameObject.FindGameObjectsWithTag(targetTag);
                foreach (GameObject charactor in newCharactors) charactors.Add(charactor);
                //Debug.Log("Charactors Added");
            }
        }

        private void Bumping()
        {
            RPGCharactorController bumped = colliderHandler.CharactorBumped.GetComponent<RPGCharactorController>();
            if (IsAlly(bumped))
            {
                AllyBumping = bumped;
                onBumpingAlly();
            }
            if (IsEnemy(bumped))
            {
                EnemyBumping = bumped;
                onBumpingEnemy();
            }
        }

        private void TakingDamage()
        {
            RPGCharactorController Damaging = colliderHandler.CharactorBumped.GetComponent<RPGCharactorController>();
            if (IsAlly(Damaging))
            {
                AllyDamaging = Damaging;
                onTakingDamageFromAlly();
            }
            if (IsEnemy(Damaging))
            {
                EnemyDamaging = Damaging;
                onTakingDamageFromEnemy();
            }
        }

        internal void DropWeapon()
        {
            fighter.DropWeapon("Charactor Controller");
        }

        internal void EnableBaseComponents(bool enable)
        {
            health.enabled = enable;
            fighter.enabled = enable;
            mover.enabled = enable;
            idle.enabled = enable;
            death.enabled = enable;
        }

        public void OnLoad()
        {
            GetBaseComponentObjects();
            loaded = true;
        }

        public void Awake()
        {
            OnLoad();
        }

        internal bool Spotted(GameObject[] targets)
        {
            //bool b = false;
            List<GameObject> gameObjects = VisibleTargets;
            foreach (GameObject target in targets)
            {
                //Debug.Log($"{target.name} has been spotted = {gameObjects.Contains(target)}");
                if (gameObjects.Contains(target)) return true;
                //if (Spotted(target)) b = true;
            }
            //return b;
            return false;
        }

        internal bool Spotted(GameObject target)
        {
            bool b = VisibleTargets.Contains(target) || CloseRange(target);
            //if (b) Debug.Log($"{gameObject.name} has spotted {target.transform.name}");
            return b;
        }

        internal bool Aware(GameObject[] targets)
        {
            //bool b = false;
            List<GameObject> gameObjects = new List<GameObject>();
            gameObjects.AddRange(AwareTargets);
            gameObjects.AddRange(HearRangeTargets);
            gameObjects.AddRange(VisibleTargets);
            gameObjects.AddRange(CloseRangeTargets);
            foreach (GameObject target in targets)
            {
                //if (Aware(target)) b = true;
                if (gameObjects.Contains(target))
                {
                    //FocusOnTarget(target);
                    return true;
                }
            }
            //return b;
            return false;
        }

        internal void FocusOnTarget(Vector3 target)
        {
            observer.SetFocus(target, GetComponent<Mover>().TurnSpeed);
        }

        internal bool Aware(GameObject target)
        {
            bool b = HearRange(target) || AwareTargets.Contains(target) || Spotted(target) || CloseRange(target);
            //if (b) Debug.Log($"{gameObject.name} is aware of {target.transform.name}");
            return b;            
        }

        internal bool CloseRange(GameObject target)
        {
            return CloseRangeTargets.Contains(target);
        }

        internal bool SoundRange(GameObject target)
        {
            bool b = SoundRangeTargets.Contains(target);
            //if (b) Debug.Log($"{gameObject.name} can Hear {target.name}");
            return b;
        }

        internal bool HearRange(GameObject target)
        {
            bool b = HearRangeTargets.Contains(target);
            //if (b) Debug.Log($"{gameObject.name} can Hear {target.name}");
            return b;
        }

        public bool WithinDistance(float distance, GameObject objectToFind)
        {
            bool b = Observer.WithinDistance(distance, objectToFind, transform);
            //Debug.Log($"{gameObject.name} within distance = {b}");
            return b;
        }

        public bool WithinDistance(float distance, GameObject objectToFind, Transform targetTransform)
        {
            bool b = Observer.WithinDistance(distance, objectToFind, targetTransform);
            //Debug.Log($"{gameObject.name} within distance = {b}");
            return b;
        }

        public (bool hasCharactor, List<GameObject> objects) CharactorInArea(Transform targetArea, float viewRadius)
        {
            List<GameObject> objects = Observer.ObjectsWithinRange(targetArea, viewRadius, observer, transform.GetComponent<ColliderHandler>(), observer.TagToLookFor);
            return (objects.Count > 0, objects);
        }

        public float BetweenDistance(Transform A, Transform B)
        {
            return Observer.BetweenDistance(A, B);
        }

        internal bool IsDead()
        {
            if (death.isDead) return true;
            if (health.IsDead())
            {
                Death();               
                return true;
            }
            return false;
        }

        internal bool TargetCharactor(BaseStats charactor)
        {
            return !fighter.attackClasses.Contains(charactor.Charactor.charactorState);
        }

        internal (bool hasHit, RaycastHit hit) HasHit(Ray ray)
        {
            return Observer.HasHit(ray);
        }

        internal Ray GetMouseRay()
        {
            return Observer.GetMouseRay();
        }
        
    }
}
