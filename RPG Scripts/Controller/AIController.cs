using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Stats;
using RPG.States;
using System;

namespace RPG.Control
{

    [RequireComponent(typeof(Patrol))]
    [RequireComponent(typeof(Alert))]
    [DisallowMultipleComponent]
    public class AIController : RPGCharactorController
    {
        public enum Drirections { OffCamera, OnCamera, Passive }
        public enum AIAction { Alert, MoveToTartget, Combat, Patrol }
        //[SerializeField] float chaseDistance = 10f;
        //[SerializeField] float aleartDistance = 20f;
        [SerializeField] float attackDistance = 5f;
        [SerializeField] float keepDistanceFromOthers = 2f;
        //[SerializeField] Color chaseVisual = Color.blue;
        [SerializeField] Color keepDistanceVisual = Color.black;
        [SerializeField] Color attackVisual = Color.yellow;
        [SerializeField] float waitTimeAfterSpottedSeconds = 60f;
        [SerializeField] float waitTimePatrolSeconds = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] bool guardStartPosition = false;
        [SerializeField] bool findPath = false;
        [SerializeField] string[] targetTags = new string[] { "Player", "Enemy", "Friendly" };

        private Vector3 guardPosition;
        private List<GameObject> targets = new List<GameObject>();
        private float timeSinceTargetSpottedOrAlert = 0f;
        private Canvas canvas;  
        private Patrol patrol;
        private Alert alert;
        private bool Loaded = false;
        //private bool shouldFight = true;
        private int closeCallsWithAlly = 0;
        private bool onCamera = false;
        private bool LastUpdateOnCamera = true;
        [SerializeField] GameObject[] turnOffWhenNotOnCamera;
        [SerializeField] float timeFocusCoolDown = 5f;
        private float timeSinceLastFocus = 0f;
        private bool setUpForPatrol = false;
        private bool agrovated = false;
        private int agroCount = 0;
        [SerializeField] float agroCoolDown = 30f;
        private Transform agroTarget;
        [SerializeField] Transform chest;


        private void Start()
        {
            timeSinceTargetSpottedOrAlert = waitTimeAfterSpottedSeconds;
            guardPosition = transform.position;
            if (findPath) patrolPath = FindClosestPatrolPath();
            if (patrolPath == null) guardStartPosition = true;
            GetComponentObjects();
            SetComponentObjects();
            onDeath += alert.Deactivate;
            onTakingDamageFromEnemy += TargetEnemyDamaging;
            onBumpingEnemy += TargetEnemyBumping;
            onBumpingAlly += TargetAllyBumping;
            Loaded = true;
            onBeingAttacked += IsAlert;
            onBeingAttacked += Agrovate;
            onBeingAttacked += AgrovateNearByAllies;
        }

        public void Agrovate(Transform target)
        {
            agrovated = true;
            agroCount += 1;
            Invoke("Calm", agroCoolDown);
        }

        private void AgrovateNearByAllies(Transform target)
        {
            AIController[] charactors = FindObjectsOfType<AIController>();

            foreach (AIController controller in charactors)
            {
                if (IsAlly(controller))
                {
                    if (SoundRange(controller.gameObject))
                    {
                        controller.Agrovate(target);
                        //Debug.Log("AI Agrovate");
                    }
                }
            }
        }

        private void Calm()
        {
            agroCount -= 1;
            if (agroCount < 0)
            {
                agrovated = false;
            }            
        }

        private void IsAlert(Transform target)
        {
            Alert();
            alert.AlertEnable(true, target);
        }

        private void TargetEnemyDamaging()
        {
            FocusOnTargetHead(EnemyDamaging.gameObject);
        }
        private void TargetEnemyBumping()
        {
            FocusOnTargetHead(EnemyBumping.gameObject);
        }

        private void FocusOnTargetHead(GameObject gobj)
        {
            if (!onCamera || timeSinceLastFocus < timeFocusCoolDown) return;
            if (!Spotted(gobj))
            {
                timeSinceLastFocus = 0f;
                //Debug.Log("TargetToFocusOn");
                FocusOnTarget(BodyPartTarget(gobj.gameObject, ColliderHandler.BodyPartType.Head).position);
            }
        }

        private void TargetAllyBumping()
        {
            if (!onCamera) return;
            //Debug.Log("TargetToFocusOn");
            Vector3 target = transform.InverseTransformDirection(BodyPartTarget(AllyBumping.gameObject, ColliderHandler.BodyPartType.Head).position);
            FocusOnTarget(target);
            Moving(target);
        }

        private PatrolPath FindClosestPatrolPath()
        {
            PatrolPath closestPath = null;
            foreach (PatrolPath path in FindObjectsOfType<PatrolPath>())
            {
                if (closestPath == null) { closestPath = path; continue; }
                if (BetweenDistance(closestPath.transform,transform) > BetweenDistance(path.transform, transform))
                {
                    closestPath = path;
                }
            }
            return closestPath;
        }

        private PatrolPath FindRandomPatrolPath()
        {
            PatrolPath[] paths = FindObjectsOfType<PatrolPath>();
            return paths[UnityEngine.Random.Range(0, paths.Length)];
        }

        private void SetComponentObjects()
        {
            patrol.SetPatrolPath(patrolPath, waitTimePatrolSeconds);
            setUpForPatrol = true;
        }

        private void GetComponentObjects()
        {
            foreach (string targetTag in targetTags)
            {
                GameObject[] newCharactors = GameObject.FindGameObjectsWithTag(targetTag);
                foreach (GameObject charactor in newCharactors) targets.Add(charactor);
                //Debug.Log("Charactors Added");
            }
            canvas = FindObjectOfType<Canvas>();
            patrol = GetComponent<Patrol>();
            alert = GetComponent<Alert>();
        }

        public PatrolPath GetPatrolPath()
        {
            return patrolPath;
        }

        public float GetPatrolTime()
        {
            return waitTimePatrolSeconds;
        }

        private void Update()
        {
            timeSinceLastFocus += Time.deltaTime;
        }

        public void AIUpdate(Drirections drirection)
        {
            onCamera = drirection == Drirections.OnCamera;
            
            if ((LastUpdateOnCamera && !onCamera) || (!LastUpdateOnCamera && onCamera))
            {
                CollidersOn(onCamera);
                SetMoverOnCamera(onCamera);
                LastUpdateOnCamera = onCamera;
                GetComponent<Animator>().enabled = onCamera;
                //GetComponent<Rigidbody>().useGravity = onCamera;
                foreach (GameObject obj in turnOffWhenNotOnCamera) obj.SetActive(onCamera);
            }
            if (!Loaded) Start();
            if (IsDead()) return;
            RPGCharactorUpdate();
            //patrol.PatrolUpdate();
            timeSinceTargetSpottedOrAlert += Time.deltaTime;
            if (onCamera && InteractWithCombat()) { /*Debug.Log("In Combat")*/; return; }            
            if (InteractWithMovement()) { /*Debug.Log("In Movement")*/; return; }
            if (InteractWithAlerts()) return;
            Idle();
        }

        private bool InteractWithAlerts()
        {
            alert.ActivateAlert();
            if (alert.IsAlert && !alert.HasTarget)
            {
                Alert();
                Debug.Log("Alert");
            }
            return alert.IsAlert;
        }

        private bool InteractWithMovement()
        {
            //patrol.CheckGuardPatrol();
            //Debug.Log($"{gameObject.name} is checking for aware of player");
            if (Aware(targets.ToArray()) || agrovated)
            {
                //Debug.Log($"{gameObject.name} is aware of {player.name}");
                if (Spotted(targets.ToArray()) || agrovated)
                {
                    timeSinceTargetSpottedOrAlert = 0;
                    //Debug.Log($"{gameObject.name} has spotted player");
                }
                bool allDead = true;
                bool shouldTarget = false;
                bool noTargets = true;
                foreach (GameObject player in targets)
                {
                    if (player.GetComponent<Health>().IsDead()) continue;
                    allDead = false;
                    if (agrovated && player.transform == agroTarget)
                    {
                        shouldTarget = true;
                    }
                    else
                    {
                        shouldTarget = TestForAlly(player.transform);
                    }                   
                    //alert.AlertEnable(true, player.transform);                    
                    if (shouldTarget)
                    {
                        noTargets = false;
                        FocusOnTargetHead(player.gameObject);
                        Moving(player.transform.position);
                        break;
                    }
                }
                if (allDead || noTargets) return CheckGuardPosition();
                           
            }
            if (IsMoving())
            {
                //Debug.Log("still moving");
                return true;
            }
            if (timeSinceTargetSpottedOrAlert >= waitTimeAfterSpottedSeconds)
            {
                if (InteractWithAlerts())
                {
                    Debug.Log("Alert");
                    return true;
                }
                return CheckGuardPosition();
            }
            return false;
            bool CheckGuardPosition()
            {
                if (guardStartPosition) { /*Debug.Log("In Guarding");*/ return GetComponent<Patrol>().OnPatrol(guardPosition); }
                if (!setUpForPatrol)
                {
                    SetComponentObjects();
                }
                if (TestForAlly(patrol.NextWaypointTransform()))
                {
                    //Debug.Log("On Patrol target");
                    return patrol.OnPatrol();
                }                
                if (UnityEngine.Random.Range(0, 2) > 0) patrol.ForceRandomWayPoint();
                //alert.AlertEnable(true, patrol.NextWaypointTransform());
                Debug.Log("On Patrol Random");
                if (closeCallsWithAlly > 5)
                {
                    closeCallsWithAlly = 0;
                    patrolPath = FindRandomPatrolPath();
                    patrol.SetPatrolPath(patrolPath, waitTimePatrolSeconds);
                }
                else
                {
                    closeCallsWithAlly += 1;
                }                
                return patrol.OnPatrol();
            }
        }

        private bool TestForAlly(Transform target)
        {
            bool shouldTarget = true;
            (bool hasCharctor, List<GameObject> gObjects) tpl = CharactorInArea(target, keepDistanceFromOthers);
            if (tpl.hasCharctor) foreach (GameObject gobj in tpl.gObjects)
            {
                if (IsAlly(gobj.GetComponent<RPGCharactorController>()))
                {
                    shouldTarget = false;
                }
            }
            return shouldTarget;
        }

        private bool InteractWithCombat()
        {            
            // need to be more spacific on who was spotted
            bool b = timeSinceTargetSpottedOrAlert < waitTimeAfterSpottedSeconds;
            if (!b) return false;
            foreach (GameObject player in targets) 
            {                
                if (player.GetComponent<Health>().IsDead()) continue;                
                //if (b) Debug.Log($"Time since last spotted < Wait Time ({waitTimeAfterSpottedSeconds}) = {b}");
                if (WithinDistance(attackDistance, player))
                {
                    FocusOnTargetHead(player);
                    alert.AlertEnable(true, player.transform);
                    //if (AwareAllyFighting())
                    //{
                    //    return false;
                    //}
                    if (!FighterTarget()) Fighting(player.GetComponent<CombatTarget>());                    
                    FighterTarget();
                    return true;
                }
            }
            return false;
        }        

        // draw gizmoz in editor in Unity
        private void OnDrawGizmosSelected()
        {
            //Gizmos.color = chaseVisual;
            //Gizmos.DrawWireSphere(transform.position, chaseDistance);
            Gizmos.color = attackVisual;
            Gizmos.DrawWireSphere(transform.position, attackDistance);
            Gizmos.color = keepDistanceVisual;
            Gizmos.DrawWireSphere(transform.position, keepDistanceFromOthers);
        }
    }
}
