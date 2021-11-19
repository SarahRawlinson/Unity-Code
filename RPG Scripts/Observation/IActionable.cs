using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;
using RPG.Actions;
using RPG.Core.UI;

namespace RPG.Actions
{
    public interface IActionable
    {
        bool HandleRayCast(RPGCharactorController controller);
        CursorUI GetCursor();
        PlayerActions GetAction();
        GameObject GetGameObject();
    }
}
