using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using UnityEngine.AI;
using RPG.DialogueInteraction;


namespace RPG.States
{
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class Death : MonoBehaviour, IAction
    {
        public bool isDead = false;

        // Update is called once per frame
        void Update()
        {

        }
        public void OnDeath(bool dead)
        {
            DialogueTriggerManager dialogue = GetComponent<DialogueTriggerManager>();
            if (dialogue != null) dialogue.Death();
            GetComponent<ActionScheduler>().StartAction(this);
            if (dead) GetComponent<Animator>().SetTrigger("die");
            else GetComponent<Animator>().ResetTrigger("die");
            GetComponent<Animator>().SetBool("dead", dead);
            GetComponent<NavMeshAgent>().enabled = !dead;
            //Debug.Log(this.name + " has died.");
            isDead = dead;
        }
        public void CancelAction()
        {
            isDead = false;
        }
    }
}
