using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;

namespace RPG.Movement
{

    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float stoppingRange = 1f;
        [Range(0,1)]
        [SerializeField] float speedFraction = 1f;
        [SerializeField] float maxMovementSpeed = 5f;
        [SerializeField] float turnSpeed = 5f;

        public bool isMoving = false;
        private bool destinationReached = true;
        public bool onCamera = true;
        private Animator animator;
        private float currentStoppingRange;

        NavMeshAgent navMeshAgent;
        private Vector3 currentDestination;

        public float StoppingRange { get => stoppingRange;}
        public float MaxMovementSpeed { get => maxMovementSpeed; set => maxMovementSpeed = value; }
        public Vector3 CurrentDestination { get => currentDestination; set => currentDestination = value; }
        public float TurnSpeed { get => turnSpeed; set => turnSpeed = value; }


        public static bool CanMoveTo(out Vector3 target, Vector3 hit, float maxNavTargetDistance, float maxPathDistance, Transform transform)
        {
            target = new Vector3();
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit, out navMeshHit, maxNavTargetDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;
            target = navMeshHit.position;
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            if (!hasPath || path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxPathDistance) return false;
            return true;
        }

        public static float GetPathLength(NavMeshPath path)
        {
            float distance = 0f;
            if (path.corners.Length < 2) return distance;
            Vector3[] corners = path.corners;
            Vector3 startCorner = corners[0];
            for (int i = 1; i < corners.Length; i++)
            {
                distance += Vector3.Distance(startCorner, corners[i]);
                startCorner = corners[i];
            }
            return distance;
        }

        // Update is called once per frame
        void Update()
        {
            //UpdateMover();
        }

        public void UpdateMover()
        {
            UpdateAnimation();
        }

        public void Awake()
        {
            currentStoppingRange = stoppingRange;
            navMeshAgent = GetComponent<NavMeshAgent>();
            TryWarp(transform.position);
            animator = GetComponent<Animator>();
        }

        private void TryWarp(Vector3 point)
        {
            bool isWalkable;
            NavMeshHit hit;
            FindWalkable(point, out isWalkable, out hit);
            if (!isWalkable || !GetComponent<NavMeshAgent>().Warp(hit.position))
            {
                Debug.LogError($"{gameObject.name} mover could not be placed on nav mesh");
                if (GetComponent<NavMeshAgent>().Warp(point)) Debug.LogError($"{gameObject.name} 2nd attempet mover placed on nav mesh");
                else Debug.LogError($"{gameObject.name} 2nd attempet mover failed to place on nav mesh");
            }

        }

        private static void FindWalkable(Vector3 point, out bool isWalkable, out NavMeshHit hit)
        {
            float y = point.y;
            if (Terrain.activeTerrain != null)
            {
                y = Terrain.activeTerrain.SampleHeight(point);

            }
            isWalkable = NavMesh.SamplePosition(new Vector3(point.x, y, point.z), out hit, 1f, NavMesh.AllAreas);
            int Attempts = 50;
            if (!isWalkable)
            {
                y = point.y;
                while (!isWalkable && Attempts > 0)
                {
                    y -= .1f;
                    isWalkable = NavMesh.SamplePosition(new Vector3(point.x, y, point.z), out hit, 1f, NavMesh.AllAreas);
                    Attempts -= 1;
                }
            }
        }

        public float GetStoppingRange()
        {
            return stoppingRange;
        }

        public void CancelAction()
        {
            currentStoppingRange = stoppingRange;
            try
            {
                GetComponent<NavMeshAgent>().isStopped = true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"{gameObject.name} could not be stoped on nav mesh {e.StackTrace}");
            }
            isMoving = false;
            //destinationReached = true;
            AnimatorBoolMovement(false);
        }

        public bool HasReachedDestination(float stopRange)
        {
            try
            {
                if (GetComponent<NavMeshAgent>().isActiveAndEnabled)
                return GetComponent<NavMeshAgent>().remainingDistance <= stopRange;
                return true;
            }
            catch(Exception E)
            {
                Debug.LogError($"player {name} Caught Error: {E.Message} : {E.StackTrace}");
                return true;
            }
        }

        public bool HasReachedDestination()
        {
            bool hasStopped = true;
            try
            {
                hasStopped = GetComponent<NavMeshAgent>().isStopped;
            }
            catch (Exception E)
            {
                Debug.LogError($"player {name} Caught Error: {E.Message} : {E.StackTrace}");
            }
            return HasReachedDestination(currentStoppingRange) || hasStopped;
        }

        
        public (bool hashit, RaycastHit hit) PathBlocked(Vector3 target)
        {
            RaycastHit hit;
            Debug.DrawRay(GetComponent<NavMeshAgent>().destination, target, Color.green);
            return (Physics.Linecast(transform.position + new Vector3(0, 1f), target + new Vector3(0,1f), out hit), hit);            
        }

        public void MoveTo(Vector3 destination, float speed, float stopRange, string caller)
        {
            if (caller == "Alert") Debug.Log($"mover has been called by {caller} to go to {destination.ToString()}");
            bool isWalkable;
            NavMeshHit hit;
            FindWalkable(destination, out isWalkable, out hit);
            destination = hit.position;
            if (caller == "Alert") Debug.Log($"mover has new position to go to {destination.ToString()}, is isWalkable = {isWalkable}");
            //var (hasHit, hit) = PathBlocked(destination);
            //if (hasHit) destination = hit.point;
            //if (PathBlocked(destination)) return;
            currentStoppingRange = stopRange;
            if (Vector3.Distance(transform.position, destination) <= currentStoppingRange)
            {
                if (caller == "Fighter") Debug.Log("within range mover has stopped");
                return;
            }
            AnimatorBoolMovement(true);
            //TriggerMovement("startMove", "stopMove");
            //destinationReached = false;
            currentDestination = destination;
            try
            {
                GetComponent<NavMeshAgent>().destination = destination;
                GetComponent<NavMeshAgent>().speed = maxMovementSpeed * Mathf.Clamp01(speed);
                GetComponent<NavMeshAgent>().isStopped = false;
            }
            catch (Exception E)
            {
                Debug.LogError($"player {name} Caught Error: {E.Message} : {E.StackTrace}");
            }
            //if (gameObject.name == "NewPlayer") Debug.Log(Vector3.Distance(destination, transform.position));
            destinationReached = HasReachedDestination();
            if (destinationReached) CancelAction();
        }

        //public void StartMoveAction(Vector3 destination, float speed)
        //{
        //    ActionStart();
        //    MoveTo(destination, speed, currentStoppingRange);
        //}

        public void StartMoveAction(Vector3 destination, float speed, bool keepAction, string actionFrom)
        {
            try
            {
                if (!keepAction) ActionStart();
                isMoving = true;
                MoveTo(destination, speed, currentStoppingRange, actionFrom);
            }
            catch
            {
                Debug.Log($"problem with mover from {actionFrom}");
            }
        }

        public void StartMoveAction(Vector3 destination, string actionFrom)
        {
            try
            { 
                ActionStart();            
                MoveTo(destination, speedFraction, currentStoppingRange, actionFrom);
            }
            catch
            {
                Debug.Log($"problem with mover from {actionFrom}");
            }
        }

        public void MoveControl(Vector3 moveTo)
        {
            ActionStart();
            float speed = (Time.deltaTime * maxMovementSpeed);
            try
            {
                navMeshAgent.ResetPath();
                navMeshAgent.Move(moveTo * speed);
                currentDestination = moveTo;
                navMeshAgent.destination = moveTo;
            }
            catch (Exception E)
            {
                Debug.LogError($"player {name} Caught Error: {E.Message} : {E.StackTrace}");
            }  
            //navMeshAgent.ResetPath();
            if (onCamera) animator.SetFloat("forwardSpeed", maxMovementSpeed);
        }

        private void ActionStart()
        {
            GetComponent<ActionScheduler>().StartAction(this);
            isMoving = true;
        }

        private void UpdateAnimation()
        {
            //float currectSpeed;
            Vector3 velocity = new Vector3();
            try
            {
                velocity = GetComponent<NavMeshAgent>().velocity;
            }
            catch (Exception E)
            {
                Debug.LogError($"player {name} Caught Error: {E.Message} : {E.StackTrace}");
            }            
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            if (onCamera) GetComponent<Animator>().SetFloat("forwardSpeed", speed);
            //if (onCamera) currectSpeed = animator.GetFloat("forwardSpeed");
            //if (currectSpeed == 0) destinationReached = true;
        }

        private void AnimatorBoolMovement(bool moving)
        {
            if (onCamera) animator.SetBool("moving", moving);
        }

        [Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        public object CaptureState()
        {
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData)state;
            transform.position = data.position.ToVector();
            transform.eulerAngles = data.rotation.ToVector();
            TryWarp(data.position.ToVector());
        }

        // animation event
        public void FootL()
        {

        }
        // animation event
        public void FootR()
        {

        }
    }
}
