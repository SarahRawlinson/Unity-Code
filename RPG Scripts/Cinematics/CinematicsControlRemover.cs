using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicsControlRemover : MonoBehaviour
    {
        private bool InControl = false;
        private GameObject player;
        private void Start()
        {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
            player = GameObject.FindWithTag("Player");
        }

        private void DisableControl(PlayableDirector notNeededInfunction)
        {
            InControl = true;
            player.GetComponent<ActionScheduler>().StopActions();
            player.GetComponent<PlayerController>().enabled = false;
            Debug.Log("Control Disabled");
        }
        private void EnableControl(PlayableDirector notNeededInfunction)
        {
            InControl = false;
            player.GetComponent<PlayerController>().enabled = true;
            Debug.Log("Control Enabled");
        }
        public bool Playing()
        {
            return InControl;
        }

    }
}
