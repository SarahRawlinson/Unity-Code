using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.IO;



namespace RPG.DialogueInteraction
{
    [System.Serializable]
    public class DialogueTrigger
    {
        public enum ScriptsTypes { Attack, Greating, Converstation, Death, Moving, Idle, Alert}
        public ScriptsTypes scriptType;
        public List<Dialogue> dialogues;        
        public ScriptsTypes ScriptType { get => scriptType;}
        public List<Dialogue> Dialogues { get => dialogues;}

        public class SaveDialogue
        {
            public ScriptsTypes scriptType;
            private List<string> dialogueName;
            public SaveDialogue(DialogueTrigger d)
            {
                scriptType = d.scriptType;
                dialogueName = new List<string>();
                foreach (Dialogue dia in d.Dialogues)
                {
                    dialogueName.Add(dia.name);
                }
            }
            public SaveDialogue(string json)
            {
                JsonUtility.FromJsonOverwrite(json, this);
            }
            public List<Dialogue> GetDialogue()
            {
                List<Dialogue> l = new List<Dialogue>();
                foreach (string s in dialogueName)
                {
                    l.Add(Resources.Load<Dialogue>(s));
                }
                return l;
            }

        }

        public string SaveState()
        {
            string json = JsonUtility.ToJson(new SaveDialogue(this));            
            return json;
        } 
        public DialogueTrigger(string json)
        {
            SaveDialogue saveDialogue = new SaveDialogue(json);
            scriptType = saveDialogue.scriptType;
            dialogues = saveDialogue.GetDialogue();
        }
    }
}
