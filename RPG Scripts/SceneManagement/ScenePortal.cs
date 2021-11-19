using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.Control;
using UnityEngine.AI;
using RPG.Core;
using System;

namespace RPG.SceneManagement
{
    public class ScenePortal : MonoBehaviour
    {
        [SerializeField] Portal thisPortal = new Portal();
        [SerializeField] Portal targetPortal = new Portal();
        [SerializeField] Transform spawnPoint;
        [SerializeField] float timeFadeOut = 1f;
        [SerializeField] float timeFadeIn = 1f;
        [SerializeField] float timeFadeWait = .5f;
        private bool complete = false;
        private SavingWrapper wrapper;

        private void Start()
        {
            wrapper = FindObjectOfType<SavingWrapper>();
        }        

        private void OnTriggerEnter(Collider other)
        {
            if (complete) return;
            if (other.attachedRigidbody == null) return;
            if (other.attachedRigidbody.gameObject.tag == "Player")
            {
                complete = true;
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            GameObject.FindWithTag("Player").GetComponent<ActionScheduler>().StopActions();
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().enabled = false; 
            if (targetPortal.level < 0)
            {
                Debug.LogError("Load scene not set");
                yield break;
            }
            
            DontDestroyOnLoad(gameObject);
            Fader fader = FindObjectOfType<Fader>();            
            yield return fader.Fade(timeFadeOut, 1f);
            UpdatePlayer(this);
            wrapper.Save();
            yield return SceneManager.LoadSceneAsync((int)targetPortal.level);
            //Debug.Log("transport to level");
            wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Load();
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().enabled = false;
            yield return new WaitForSeconds(timeFadeWait);
            //Debug.Log("Loaded");            
            try
            {
                UpdatePlayer(GetTargetPortal());
            }
            catch (Exception e)
            {
                Debug.Log($"Nav Mesh Error {e.Message}");
            }
            //Debug.Log("player transported");
            wrapper.Save();
            fader.Fade(timeFadeIn, 0f);
            //Debug.Log("Faided In");
            //Debug.Log("game saved");
            //fader.FadeImmediate(0);
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().enabled = true;
            //Debug.Log("Load Complete");            
            Destroy(gameObject);
        }
        private void OnDestroy()
        {
            //Debug.Log("Scene Portal "+ thisPortal.level.ToString() +  " to " + targetPortal.level.ToString() + " Destroyed");
        }

        private void UpdatePlayer(ScenePortal targetPortal)
        {
            GameObject player =  GameObject.FindWithTag("Player");            
            player.transform.position = targetPortal.spawnPoint.position;
            player.GetComponent<NavMeshAgent>().Warp(targetPortal.spawnPoint.position);
            player.transform.rotation = targetPortal.spawnPoint.rotation;
        }

        private ScenePortal GetTargetPortal()
        {
            foreach (ScenePortal portal in FindObjectsOfType<ScenePortal>())
            {
                if (portal.thisPortal.level != targetPortal.level && portal != this) { Debug.Log("wrong level in target / portal"); }
                if (portal.thisPortal.portalType == targetPortal.portalType && portal != this)
                {
                    //Debug.Log("Target returned");
                    return portal;
                }                    
            }
            //Debug.Log("No target returned");
            return null;            
        }
    }
}
