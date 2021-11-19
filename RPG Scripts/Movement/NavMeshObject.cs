using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Actions;
using RPG.Control;
using RPG.Core.UI;

namespace RPG.Movement
{
    public class NavMeshObject : MonoBehaviour, IActionable
    {
        public PlayerActions GetAction()
        {
            return PlayerActions.Movement;
        }

        public CursorUI GetCursor()
        {
            return CursorUI.Movement;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public bool HandleRayCast(RPGCharactorController controller)
        {
            return true;
        }
        

    }
}
