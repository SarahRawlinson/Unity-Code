using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.States
{
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class Idle : MonoBehaviour, IAction
    {
        [SerializeField] bool stopIdle = false;
        private Animator animator;
        // for testing
        public bool isIdle = false;
        // Start is called before the first frame update
        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void OnIdle()
        {
            if (stopIdle) return;
            animator.SetTrigger("idle");
            GetComponent<ActionScheduler>().StartAction(this);
            isIdle = true;
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void CancelAction()
        {
            animator.ResetTrigger("idle");
            isIdle = false;
        }
    }
}
