using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Saving;

namespace RPG.DialogueInteraction
{
    [DisallowMultipleComponent]
    public class DialogueManager : MonoBehaviour, ISaveable
    {        
        [SerializeField] Text nameText;
        [SerializeField] Text dialogueText;
        [SerializeField] GameObject background;
        [SerializeField] float waitTime = 10;
        private bool dialogueComplete = true;
        private Queue<string> sentences;

        // Start is called before the first frame update
        void Start()
        {
            SetDialogueActive(false);
            sentences = new Queue<string>();
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void StartDialogue(Dialogue dialogue)
        {
            dialogueComplete = false;
            SetDialogueActive(true);
            //nameText.text = dialogue.name;
            sentences.Clear();
            foreach (string sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }
            while (!dialogueComplete)
            {
                StartCoroutine(DisplayCoroutine());
            }
        }

        private void SetDialogueActive(bool on)
        {
            dialogueText.text = "";
            background.SetActive(on);
        }

        IEnumerator DisplayCoroutine()
        {
            DisplayNextSentance();
            yield return new WaitForSeconds(waitTime);
        }

        private void DisplayNextSentance()
        {
            if (sentences.Count == 0)
            {
                StartCoroutine(EndDialogue());
                return;
            }
            string sentence = sentences.Dequeue();
            dialogueText.text = sentence;
        }

        IEnumerator EndDialogue()
        {
            dialogueComplete = true;
            yield return new WaitForSeconds(waitTime);
            SetDialogueActive(false);
        }

        public object CaptureState()
        {
            return null;
        }

        public void RestoreState(object state)
        {
            
        }
    }
}
