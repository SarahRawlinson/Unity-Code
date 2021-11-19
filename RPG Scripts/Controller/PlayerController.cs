using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Stats;
using RPG.States;
using RPG.Actions;
using RPG.Core.UI;
using UnityEngine.EventSystems;
using RPG.Movement;
using System;


namespace RPG.Control
{
    
    [RequireComponent(typeof(Experience))]
    
    [DisallowMultipleComponent]
    public class PlayerController : RPGCharactorController
    {
        private StatsUI ui;
        private GameObject[] enemies;
        private Experience experience;
        private Camera[] cameras;
        [SerializeField] float maxNavMeshDistance = 1f;
        [SerializeField] float maxPathDistance = 40f;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorUI type;
            public Texture2D texture;
            public Vector2 hotspot;
        }
        [SerializeField] CursorMapping[] cursorMappings = null;
        private CursorUI chosenCursor = CursorUI.None;
        [SerializeField] GameObject main;
        private CursorUI lastCursor = CursorUI.None;

        private void Start()
        {
            //Debug.Log(FindObjectsOfType<PlayerController>().Length);
            //gameObject.transform.position = startTransform.position;
            cameras = FindObjectsOfType<Camera>();
            GetComponentObjects();
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            onBeingAttacked += AlertActive;
        }     
        
        private void AlertActive(Transform transform)
        {
            //ShowText($"you have been attached by {transform.gameObject.name}");
            //Invoke("ClearText", 5f);
        }
        private void ClearText()
        {
            ShowText("");
        }

        private void ShowText(string text)
        {
            ui.DisplayMainText(text);
        }

        public bool InteractWithWorldComponent()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            hits = RaycaseAllSorted(hits);
            foreach (RaycastHit hit in hits)
            {
                Rigidbody body = hit.collider.GetComponent<Rigidbody>();
                if (body != null && body.gameObject == gameObject) continue;
                IActionable[] rayCastables = hit.transform.GetComponents<IActionable>();
                if (CheckIActionables(hit, rayCastables)) return true;
            }
            return false;
        }

        private bool CheckIActionables(RaycastHit hit, IActionable[] rayCastables)
        {
            foreach (IActionable rayCastable in rayCastables)
            {
                Vector3 target = new Vector3();
                bool canMoveTo = Mover.CanMoveTo(out target, hit.point, maxNavMeshDistance, maxPathDistance, transform);
                bool Visable = Observation.Observer.CheckCamerasForOnScreen(hit.point, cameras);
                //Debug.Log("Has RayCastable");
                if (rayCastable.HandleRayCast(this))
                {
                    bool conditionsMet = false;
                    switch (rayCastable.GetAction())
                    {
                        case PlayerActions.None:
                            conditionsMet = true;
                            //Debug.Log("None Condition Met");
                            break;
                        case PlayerActions.Attack:
                            if (Visable)
                            {
                                InteractWithCombat(rayCastable.GetGameObject());
                                conditionsMet = true;
                            }                                
                            break;
                        case PlayerActions.PickUp:
                            if (canMoveTo && Visable)
                            {
                                conditionsMet = true;
                                InteractWithMovement(rayCastable.GetGameObject().transform.position);
                            }
                            break;
                        case PlayerActions.Movement:
                            if (canMoveTo && Visable)
                            {
                                conditionsMet = true;
                                InteractWithMovement(target);
                            }
                            break;
                        default:
                            break;
                    }                    
                    if (conditionsMet)
                    {
                        chosenCursor = rayCastable.GetCursor();
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycaseAllSorted(RaycastHit[] hits)
        {
            float[] distance = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++) distance[i] = hits[i].distance;
            Array.Sort(distance, hits);
            return hits;
        }

        private bool InteractWithUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        private bool CheckEnemyStates()
        {
            foreach (GameObject enemy in enemies)
            {
                if (!enemy.GetComponent<Death>().isDead)
                {
                    return false;
                }                
            }
            return true;
        }


        private void GetComponentObjects()
        {
            
            ui = FindObjectOfType<StatsUI>();
            experience = GetComponent<Experience>();
        }

        private CursorMapping GetCursorMapping(CursorUI type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type) return mapping;
            }
            return cursorMappings[0];
        }

        private void SetCursorType(CursorUI cursor)
        {
            if (lastCursor == cursor) return;
            lastCursor = cursor;
            CursorMapping mapping = GetCursorMapping(cursor);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private void Update()
        {
            chosenCursor = CursorUI.None;            
            if (IsDead())
            {
                SetCursorType(chosenCursor);
                return;
            }
            RPGCharactorUpdate();
            CheckDropWeapon();
            
            //if (!Loaded()) return;
            // update the canvas text with kill count

            try
            {
                ui.Kills(experience.GetKillCount(), experience.GetXP(), GetDamage(), level.ToString());
            }
            catch
            {

            }
            if (InteractWithUI())
            {
                chosenCursor = CursorUI.UI;
                SetCursorType(chosenCursor);
                return;
            }
            if (InteractWithWorldComponent())
            {
                SetCursorType(chosenCursor);
                return;
            }
            // Check for weapon enabled, needs some work to iron out            

            // check for combat
            //if (InteractWithCombat())
            //{
            //    SetCursorType(chosenCursor);
            //    return;
            //}
            // needs work doesnt seam to attack if in range, mouse button needs to be activated again

            // check if should move
            //if (InteractWithMovement())
            //{
            //    SetCursorType(chosenCursor);
            //    return;
            //}
            if (CheckStates()) return;
            // if none of the above goto idle
            SetCursorType(chosenCursor);
            Idle(); 
        }
        
        private bool CheckStates()
        {
            if (FighterTarget()) return true;
            return CheckForOverRideMovement();
        }

        public void CheckDropWeapon()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                DropWeapon();
            }
        }

        private bool InteractWithCombat(GameObject targetGameObject)
        {
            CombatTarget target = targetGameObject.GetComponent<CombatTarget>();
            //CombatTarget target = CheckRayCastHits(mouseRay);
            if (Input.GetMouseButtonDown(0))
            {                
                if (target == null) return false;
                CombatTarget lastTarget = target;
                Fighting(target);
            }            
            return FighterTarget();
        }

        private CombatTarget CheckRayCastHits(Ray ray)            
        {
            RaycastHit[] hits = Physics.RaycastAll(ray);
            //Debug.Log("Check For Hits");
            //int i = 1;
            foreach (RaycastHit hit in hits)
            {
                //Debug.Log($"checking hit {i}");
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                BaseStats charactor = hit.transform.GetComponent<BaseStats>();
                if (target == null || TargetCharactor(charactor))
                {
                    
                    //Debug.Log("Player No Hit!");
                    continue;
                }
                //chosenCursor = CursorUI.Combat;
                //Debug.Log("Player has Hit!");
                return target;                
            }
            return null;
        }

        public bool CheckForOverRideMovement()
        {
            Vector3 desiredPosition = transform.position;
            if (Input.GetKey(KeyCode.W))
            {
                desiredPosition = transform.forward;
                MovingControl(desiredPosition);
            }
            if (Input.GetKey(KeyCode.S))
            {
                desiredPosition = -transform.forward;
                MovingControl(desiredPosition);

            }
            if (Input.GetKey(KeyCode.A))
            {
                desiredPosition = -transform.right;
                MovingControl(desiredPosition);

            }
            if (Input.GetKey(KeyCode.D))
            {
                desiredPosition = transform.right;
                MovingControl(desiredPosition);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                Vector3 newRotation = new Vector3(0, -0.2f, 0);
                transform.eulerAngles += newRotation;

            }
            if (Input.GetKey(KeyCode.E))
            {
                Vector3 newRotation = new Vector3(0, 0.2f, 0);
                transform.eulerAngles += newRotation;
            }
            return IsMoving();
        }

        private bool InteractWithMovement(Vector3 target)
        {
            //var (hasHit, hit) = HasHit(mouseRay);
            //if (hasHit && chosenCursor == CursorUI.None) chosenCursor = CursorUI.Movement;
            //    else SetCursorType(CursorUI.None);
            // hasHit bool, hit RaycastHit, getmousehit            
            if (Input.GetMouseButton(0))
            {
                Moving(target);
                //if (hasHit) return Moving(hit.point);                   
            }            
            return IsMoving();
        }   

    }
}
