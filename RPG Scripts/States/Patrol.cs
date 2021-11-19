using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Movement;
//using RPG.Control;

namespace RPG.States
{
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Mover))]
    [DisallowMultipleComponent]
    public class Patrol : MonoBehaviour, IAction
    {
        [Range(0,1)]
        [SerializeField] float patrolSpeed = 0.5f;
        public bool isPatroling = false;
        public bool isGuarding = false;
        private int lastWaypoint = 0;
        //private AIController controller;
        private Mover mover;
        private float timeSinceLastMove = 0;
        private float patrolTime = 0;
        private PatrolPath patrolPath = null;
        public Transform currentTarget;

        // Start is called before the first frame update

        void Awake()
        {
            //controller = GetComponent<AIController>();
            //patrolPath = controller.GetPatrolPath();
            mover = GetComponent<Mover>();
        }

        private void ClosestPosition()
        {
            if (patrolPath == null) return;            
            //timeSinceLastMove = controller.GetPatrolTime();
            lastWaypoint = Random.Range(0, patrolPath.GetNumberOfWaypoints() - 1);
            for (int i = 0; i < patrolPath.GetNumberOfWaypoints(); i++)
            {
                if (WaypointDistance(i) < WaypointDistance(lastWaypoint))
                {
                    lastWaypoint = i;
                }
            }
        }


        public void SetPatrolPath(PatrolPath path, float waitTime)
        {
            patrolPath = path;
            patrolTime = waitTime;
            ClosestPosition();
        }

        private float WaypointDistance(int child)
        {
            return Vector3.Distance(patrolPath.ChildPosition(child).position, transform.position);
        }

        public void Update()
        {
            //PatrolUpdate();
        }

        public void PatrolUpdate()
        {
            timeSinceLastMove += Time.deltaTime;
            if (isGuarding) return;
            if (isPatroling) PatrolPath();
        }

        public Transform NextWaypointTransform()
        {            
            return patrolPath.ChildPosition(lastWaypoint);
        }

        public void ForceRandomWayPoint()
        {
            lastWaypoint = Random.Range(0, patrolPath.GetNumberOfWaypoints() - 1);
        }

        public bool OnPatrol()
        {
            if (patrolPath == null)
            {
                return false;
            }
            //else Debug.Log(patrolPath.gameObject.name);
            //if (gameObject.name == "Test AI Enemy Variant Test Code") Debug.Log($"Old Patrolling Path {patrolPath.gameObject.name} Last waypoint {lastWaypoint}");
            //patrolPath = path;
            //patrolTime = waitTime;
            isPatroling = true;
            GetComponent<ActionScheduler>().StartAction(this);
            //lastWaypoint = patrolPath.NextWayPoint(lastWaypoint);
            Transform desiredTransform = patrolPath.ChildPosition(lastWaypoint);
            MoveToNextDestination(desiredTransform);
            PatrolPath();
            //if (gameObject.name == "Test AI Enemy Variant Test Code") Debug.Log($"New Patrolling Path {patrolPath.gameObject.name} New waypoint {lastWaypoint}");
            return true;
        }
        public bool OnPatrol(Vector3 target)
        {
            patrolTime = 0;            
            //Debug.Log("Guarding");
            isGuarding = true;            
            MoveToNextDestination(target);
            if (WithinDistance())
            {                
                return false;
            }
            GetComponent<ActionScheduler>().StartAction(this);
            return true;
        }

        private void PatrolPath()
        {
            if (WithinDistance())
            {
                //Debug.Log($"within distance T{timeSinceLastMove} PT {patrolTime}");
                //if (timeSinceLastMove < controller.GetPatrolTime()) return;
                if (timeSinceLastMove <= patrolTime) return;
                lastWaypoint = patrolPath.NextWayPoint(lastWaypoint);
                //Debug.Log(lastWaypoint);
                timeSinceLastMove = 0;
                //OnPatrol(/*patrolPath, patrolTime*/);
            }
            else { timeSinceLastMove = 0; }

        }

        private bool WithinDistance()
        {
            return mover.HasReachedDestination() || !mover.isMoving;
        }

        private void MoveToNextDestination(Transform destination)
        {
            //timeSinceLastMove = 0;
            currentTarget = destination;
            float y = destination.position.y;
            //if (Terrain.activeTerrain != null) y = Terrain.activeTerrain.SampleHeight(destination.position);
            //Debug.Log("patrol set move");
            GetComponent<Mover>().StartMoveAction(new Vector3(destination.position.x, y, destination.position.z), patrolSpeed, false, "Patrol");
        }

        private void MoveToNextDestination(Vector3 destination)
        {
            //timeSinceLastMove = 0;
            currentTarget = null;
            float y = destination.y;
            //if (Terrain.activeTerrain != null) y = Terrain.activeTerrain.SampleHeight(destination);
            GetComponent<Mover>().StartMoveAction(new Vector3(destination.x, y, destination.z), patrolSpeed, false, "Patrol");
        }


        public void CancelAction()
        {
            //if (gameObject.name == "Test AI Enemy Variant Test Code") Debug.Log($"Patrol has now been cancled");
            isPatroling = false;
            isGuarding = false;
        }
    }
}
