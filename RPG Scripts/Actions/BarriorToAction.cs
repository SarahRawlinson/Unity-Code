using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Core.UI;
using UnityEngine;

namespace RPG.Actions
{
    public class BarriorToAction : MonoBehaviour, IActionable
    {
        public PlayerActions GetAction()
        {
            return PlayerActions.None;
        }

        public CursorUI GetCursor()
        {
            return CursorUI.Dialogue;
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
