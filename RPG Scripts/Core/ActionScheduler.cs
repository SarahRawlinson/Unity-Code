using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    [DisallowMultipleComponent]
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction;
        public void StartAction(IAction action)
        {
            if (currentAction == action) return;
            if(currentAction != null)
            {
                currentAction.CancelAction();
            }
            
            currentAction = action;
        }
        public void StopActions()
        {
            currentAction.CancelAction();
        }
    }
}
