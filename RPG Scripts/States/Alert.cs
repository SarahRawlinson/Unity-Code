using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Control;
using RPG.Movement;
using System;

namespace RPG.States
{
    [RequireComponent(typeof(RPGCharactorController))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Mover))]

    [DisallowMultipleComponent]
    public class Alert : MonoBehaviour, IAction
    {
        [Range(0, 1)]
        [SerializeField] float speed = 0.5f;
        [SerializeField] float min = -100f;
        [SerializeField] float max = 100f;
        [SerializeField] float coolDownTime = 5f;
        private Mover move;
        private bool isAlert = false;
        private bool hasTarget = false;
        private List<GameObject> targets = new List<GameObject>();
        private bool isObserving = true;
        private Transform targetTransform;

        public bool IsAlert { get => isAlert;}
        public bool HasTarget { get => hasTarget;}

        private void Start()
        {
            move = GetComponent<Mover>();
            //foreach (RPGCharactorController controller in FindObjectsOfType<RPGCharactorController>()) targets.Add(controller.gameObject);
        }

        public void AlertEnable(bool on, Transform target)
        {
            targetTransform = target;
            if (on && !IsAlert && isObserving)
            {
                Invoke("TryCancel", coolDownTime);
            }
            isAlert = on;
        }

        //public void AlertEnable()
        //{
        //    Debug.Log("Alert");
        //    float distance = -1f;
        //    Transform target = transform;
        //    foreach (RPGCharactorController controller in FindObjectsOfType<RPGCharactorController>())
        //    {
        //        if (!(distance != -1f && Vector3.Distance(controller.transform.position, transform.position) > distance))
        //        {
        //            target = controller.transform;
        //            distance = Vector3.Distance(controller.transform.position, transform.position);
        //        }
        //    }
        //    targetTransform = target;
        //    AlertEnable(true, target);
        //}

        public void ActivateAlert()
        {
            //Debug.Log("check Alert?");
            Vector3 targetVector;
            if (targetTransform != null && isAlert && !hasTarget)
            {
                Debug.Log("is Alert?");
                hasTarget = true;
                GetComponent<ActionScheduler>().StartAction(this);
                targetVector = FindPointOfInterest(targetTransform);
                transform.LookAt(targetVector);
                move.StartMoveAction(targetVector, speed, false, "Alert");                
                return;
            }            
            foreach (GameObject target in targets)
            {
                Transform area = target.transform;
                if (isAlert && !hasTarget)
                {
                    hasTarget = true;
                    //Debug.Log("Alert!");
                    GetComponent<ActionScheduler>().StartAction(this);
                    targetVector = FindPointOfInterest(area);
                    transform.LookAt(targetVector);
                    move.StartMoveAction(targetVector, speed, false, "Alert");                    
                    break;
                }
            }
        }

        public void Deactivate()
        {
            isObserving = false;
            CancelAction();
        }

        public Vector3 FindPointOfInterest(Transform areaTarget)
        {
            bool canMove = false;
            int attempts = 50;
            Vector3 newVector3 = transform.position;
            while (!canMove && attempts > 0)
            {
                Vector3 pos = areaTarget.position;
                float x = pos.x - UnityEngine.Random.Range(min, max);
                float y = pos.y;
                float z = pos.z - UnityEngine.Random.Range(min, max);
                //Debug.Log($"targeting position x = {x}, y = {y}, z = {z}, origional = x = {pos.x}, y = {pos.y}, z = {pos.z}, " +
                    //$"Current = {transform.position.x}, y = {transform.position.y}, z = {transform.position.z}");
                newVector3 = new Vector3(x, y, z);
                canMove = Mover.CanMoveTo(out newVector3, newVector3, 10f, 100f, transform);
                attempts -= 1;
            }
            Debug.Log($"can Move = {canMove}");
            return newVector3;
        }

        public void TryCancel()
        {
            if (!GetComponent<RPGCharactorController>().Aware(targets.ToArray()) || !isObserving)
            {
                CancelAction();
                return;
            }
            if (move.HasReachedDestination())
            {
                //Debug.Log($"{name} Target Reached");
                hasTarget = false;
            }
            Invoke("TryCancel", coolDownTime);
        }

        public void CancelAction()
        {
            targetTransform = null;
            //Debug.Log("Not Alert!");
            isAlert = false;
            hasTarget = false;
        }
        
    }
}
