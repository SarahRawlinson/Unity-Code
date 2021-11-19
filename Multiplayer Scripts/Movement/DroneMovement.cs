using System;
using System.Collections;
using System.Collections.Generic;
using MultiplayerRTS.WorldObjects;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace MultiplayerRTS.Movement
{
    public class DroneMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 1.0f;
        private Vector3 target = new Vector3(0,0,0);
        private Vector3 baseTarget = new Vector3(0,0,0);
        private bool hasTarget = false;
        private Vector3 currentTargetPos = new Vector3();
        private enum Targets
        {
            MoveToTarget,
            MoveToBase
        }
        private Targets currentTarget = Targets.MoveToTarget;
        
        // Start is called before the first frame update
        void Awake()
        {
            var position = transform.position;
            baseTarget = position;
            //TODO probably need to change this in future
            FindPlanetTarget(position);
        }

        private void FindPlanetTarget(Vector3 position)
        {
            Planet targetPlanet = FindObjectOfType<Planet>();
            foreach (Planet planet in FindObjectsOfType<Planet>())
            {
                if (planet.isSun) continue;
                if (Vector3.Distance(planet.transform.position, position) < 
                    Vector3.Distance(targetPlanet.transform.position, position) || targetPlanet.isSun)
                {
                    targetPlanet = planet;
                }
            }

            target = targetPlanet.transform.position;
            currentTargetPos = target;
            hasTarget = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!hasTarget) return;
            switch (currentTarget)
            {
                case Targets.MoveToBase:
                    if (Vector3.Distance(transform.position, baseTarget) < .5f)
                    {
                        currentTarget = Targets.MoveToTarget;
                        currentTargetPos = target;
                        return;
                    }
                    break;
                case  Targets.MoveToTarget:
                    if (Vector3.Distance(transform.position, target) < .5f)
                    {
                        currentTargetPos = baseTarget;
                        currentTarget = Targets.MoveToBase;
                        return;
                    }
                    
                    break;
                default:
                    return;
                    
            }
            MoveTowards();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentTargetPos, 100);
            var position = transform.position;
            Gizmos.DrawLine(position, currentTargetPos);
        }

        private void MoveTowards()
        {
            transform.LookAt(currentTargetPos);
            //Debug.Log("Drone Move Towards");
            Vector3 aim = (currentTargetPos - transform.position).normalized;
            transform.position = transform.position + aim * speed * Time.deltaTime;
        }

        public void SetTarget(Vector3 nextPosition)
        {
            hasTarget = true;
            target = nextPosition;
        }

        public void StopTargeting()
        {
            hasTarget = false;
            target = new Vector3(0, 0, 0);
        }
    }
}
