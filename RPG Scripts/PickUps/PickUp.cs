using System.Collections;
using UnityEngine;
using RPG.Core;
using RPG.Control;
using RPG.Stats;
using RPG.Core.UI;
using RPG.Actions;

namespace RPG.PickUp
{
    public class PickUp : MonoBehaviour, IActionable
    {
        public delegate void SetActive(IActivate pickUp);
        private bool isActive = false;
        public bool Active { get => isActive; }
        private bool pickUpComplete = false;
        public bool PickUpComplete { get => pickUpComplete; }
        public GameObject Player { get => player;}
        [SerializeField] float timeBeforeActive = 5;
        public bool timePassed = false;
        [SerializeField] CursorUI CursorType;

        private GameObject player;

        private void Awake()
        {
            StartCoroutine(DeactivateForSeconds(timeBeforeActive));
        }

        private void OnTriggerEnter(Collider other)
        {
            //PlayerController
            //Debug.Log(other.attachedRigidbody.gameObject.name);
            if (isActive) return;
            try
            {
                GameObject collector = other.attachedRigidbody.gameObject;
                if (collector.GetComponent<PlayerController>() != null)
                {                    
                    if (collector.GetComponent<PlayerController>().IsDead()) return;
                    Debug.Log("Pick Up Activated");
                    player = collector;
                    isActive = true;
                }
            }
            catch
            {
                //isActive = false;
                //Debug.Log("no CharacterController");
            }            
        }

        public void Deactivated(IActivate PickUp)
        {
            pickUpComplete = true;
            PickUp.Deactivate();
        }
        public void Activated(IActivate PickUp)
        {
            PickUp.Activate();
        }

        public IEnumerator DeactivateForSeconds(float time)
        {
            //Debug.Log($"{gameObject.name} Deactivate for {time}");
            ActivatePickups(false);
            yield return new WaitForSeconds(time);
            //Debug.Log($"{gameObject.name} Now Active after {time}");
            ActivatePickups(true);
            isActive = false;
            pickUpComplete = false;
            player = null;
        }

        internal void AddativeStat(Stats.Stats stat, float value, float time)
        {
            StartCoroutine(player.GetComponent<BaseStats>().TimedStatEffectAddative(stat, value, time));
        }

        internal void PercentageStat(Stats.Stats stat, float value, float time)
        {
            StartCoroutine(player.GetComponent<BaseStats>().TimedStatEffectPercentage(stat, value, time));
        }

        public void ActivatePickups(bool active)
        {
            GetComponent<SphereCollider>().enabled = active;
            timePassed = active;
            ShowPickUp(active);
        }

        private void ShowPickUp(bool active)
        {
            Transform[] allChildren = GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                //Debug.Log(child.gameObject.name);
                MeshRenderer render = child.GetComponent<MeshRenderer>();
                if (render != null)
                {
                    render.enabled = active;
                    //Debug.Log($"renderer is {active}");
                }
            }

        }

        bool IActionable.HandleRayCast(RPGCharactorController controller)
        {
            return !pickUpComplete;
        }

        CursorUI IActionable.GetCursor()
        {
            return CursorType;
        }

        public PlayerActions GetAction()
        {
            return PlayerActions.PickUp;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}
