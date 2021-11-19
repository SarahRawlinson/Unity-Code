using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Control;
using RPG.Saving;
using RPG.States;

namespace RPG.SceneManagement
{    
    public class SceneDirector : MonoBehaviour, ISaveable
    {
        //public event Action action;
        //private bool loaded = false;
        private AIController[] controllers;
        private Camera[] cameras;
        public class GameCharacter
        {
            private GameObject gameObject;    
            public GameCharacter (GameObject obj)
            {
                gameObject = obj;
            }
        }

        private void Start()
        {
            controllers = FindObjectsOfType<AIController>();
            cameras = FindObjectsOfType<Camera>();
        }

        private void Update()
        {
            UpdateAI();
        }

        private void UpdateAI()
        {
            foreach (AIController ai in controllers)
            {
                if (ai.IsDead()) continue;
                bool onScreen = false;
                onScreen = Observation.Observer.CheckCamerasForOnScreen(ai.transform.position, cameras);
                //Vector3 screenPoint = FindObjectOfType<Camera>().WorldToViewportPoint(ai.transform.position);
                //onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
                if ((onScreen)) ai.AIUpdate(AIController.Drirections.OnCamera);
                else ai.AIUpdate(AIController.Drirections.OffCamera);
                ai.GetComponent<Patrol>().PatrolUpdate();
            }
            //loaded = true;
        }

        


        private void CreateScene()
        {

        }

        private void CreateCharactors()
        {

        }

        private void BuildCharactorRelationships()
        {

        }

        public object CaptureState()
        {
            return "To Set Up?";
        }

        public void RestoreState(object state)
        {
            controllers = FindObjectsOfType<AIController>();
            cameras = FindObjectsOfType<Camera>();
            //foreach (AIController ai in controllers)
            //{
            //    ai.AIUpdate();
            //}
        }
    }
}
