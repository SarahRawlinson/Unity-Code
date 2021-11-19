using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using System;
using RPG.Core;
using RPG.Control;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] bool deleteOnLoad = false;
        [SerializeField] float loadTime = .2f;
        const string defaultSaveFile = "Save";
        public string filePath;
        private bool loadComplete = false;
        //public event Action onLoaded;

        public bool LoadComplete { get => loadComplete; set => loadComplete = value; }
        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }

        IEnumerator LoadLastScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeImmediate(1);
            if (deleteOnLoad) { Delete(); yield return fader.Fade(loadTime, 0f); }
            else
            {
                //DestroyAllGameObjects();
                
                yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            }
            //if (!loadComplete) onLoaded();
            Load();
            loadComplete = true;
            Debug.Log("Load complete");
            Save();
            yield return fader.Fade(loadTime, 0f);
        }


        public void DestroyAllGameObjects()
        {
            GameObject[] GameObjects = (UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects() as GameObject[]);

            for (int i = 0; i < GameObjects.Length; i++)
            {
                //Debug.Log(GameObjects[i].name);
                if (GameObjects[i] != transform.root) Destroy(GameObjects[i]);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.End))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.Home))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
            Debug.Log("Save Deleted");
        }

        public void Save()
        {
            try
            {
                GetComponent<SavingSystem>().Save(defaultSaveFile);
                Debug.Log("Game Saved");
            }
            catch (Exception E)
            {
                Debug.LogError("Caught Error: " + "Game could not be Saved : " 
                    + E.Message + " - " + E.StackTrace);
            }
        }

        public void Load()
        {            
            loadComplete = false;
            Debug.Log("Load called");
            try
            {
                GetComponent<SavingSystem>().Load(defaultSaveFile);
                Debug.Log("Game Loaded");
            }
            catch (Exception E)
            {
                Debug.LogError("Caught Error: " + "Game Could not be loaded : " 
                    + E.Message + " - " + E.StackTrace);
            }
            loadComplete = true;
            //onLoaded();
        }

        private void OnDestroy()
        {
            //Debug.Log("Saving Wrapper has been destroyed");
        }
    }
}
