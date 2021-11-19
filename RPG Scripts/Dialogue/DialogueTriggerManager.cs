using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;
using RPG.Saving;


namespace RPG.DialogueInteraction
{
    public class DialogueTriggerManager : MonoBehaviour, ISaveable
    {
        [SerializeField] List<DialogueTrigger> triggers;
        [SerializeField] DialogueManager dialogueManager = null;
        [Range(0, 100)]
        [SerializeField] int chanceOfAttackDialogue = 100;
        [Range(0, 100)]
        [SerializeField] int chanceOfDeathDialogue = 100;
        [Range(0, 100)]
        [SerializeField] int chanceOfIdleDialogue = 100;
        [Range(0, 100)]
        [SerializeField] int chanceOfMovingDialogue = 100;
        [Range(0, 100)]
        [SerializeField] int chanceOfAlertDialogue = 100;

        private void Awake()
        {
            RPGCharactorController charactorController = null;
            charactorController = GetComponent<RPGCharactorController>();
            if (charactorController != null)
            {
                charactorController.onFighting += Attack;
                charactorController.onDeath += Death;
                charactorController.onMoving += Move;
                charactorController.onIdle += Idle;
                charactorController.onAlert += Alert;
            }
        }

        public void Alert()
        {
            //Debug.Log("Alert has been triggered in dialogue");
            TriggerDialogueByType(DialogueTrigger.ScriptsTypes.Alert, chanceOfAlertDialogue);
        }

        public void Move()
        {
            TriggerDialogueByType(DialogueTrigger.ScriptsTypes.Moving, chanceOfMovingDialogue);
        }

        public void Idle()
        {
            TriggerDialogueByType(DialogueTrigger.ScriptsTypes.Idle, chanceOfIdleDialogue);
        }

        public void Attack()
        {
            //Debug.Log("Dialogue Attack!");
            TriggerDialogueByType(DialogueTrigger.ScriptsTypes.Attack, chanceOfAttackDialogue);
        } 

        public void Death()
        {
            TriggerDialogueByType(DialogueTrigger.ScriptsTypes.Death, chanceOfDeathDialogue);
        }

        private void TriggerDialogueByType(DialogueTrigger.ScriptsTypes type, int chance)
        {
            DialogueTrigger trigger = FindTriggerType(type);
            if (trigger != null) RandomChanceTriggerDialogue(chance, trigger);
        }

        private DialogueTrigger FindTriggerType(DialogueTrigger.ScriptsTypes triggerType)
        {
            foreach (DialogueTrigger trigger in triggers)
            {
                if (trigger.ScriptType == triggerType)
                {
                    return trigger;
                }
            }
            return null;
        }

        private void RandomChanceTriggerDialogue(int chance, DialogueTrigger trigger)
        {
            int ranChance = Random.Range(1, 101);
            if (ranChance <= chance) TriggerDialogue(trigger.Dialogues);
        }

        public void TriggerDialogue(List<Dialogue> dialogues)
        {
            if (dialogueManager == null) dialogueManager = GetComponent<DialogueManager>();
            Dialogue dialogue = dialogues[Random.Range(0, dialogues.Count)];
            dialogueManager.StartDialogue(dialogue);
        }

        public object CaptureState()
        {
            //Debug.Log("Capture Dialogue");
            string[] triggerList = null;
            try
            {
                triggerList = new string[triggers.Count];
                for (int i = 0; i > triggers.Count; i++)
                {
                    DialogueTrigger d = triggers[i];
                    triggerList[i] = d.SaveState();
                    Debug.Log(d.SaveState());

                }
            }
            catch (System.Exception e)
            {
                Debug.Log($"could not run capture Dialogue : {e}");
            }
            
            return triggerList;
        }

        public void RestoreState(object state)
        {
            //Debug.Log("Restore Dialogue");
            try
            {
                string[] triggerList = (string[])state;
                triggers = new List<DialogueTrigger>();

                foreach (string d in triggerList)
                {
                    DialogueTrigger dialogue = new DialogueTrigger(d);
                    triggers.Add(dialogue);
                    Debug.Log("trigger added");
                }
            }
            catch
            {

            }       
        }
    }
}
