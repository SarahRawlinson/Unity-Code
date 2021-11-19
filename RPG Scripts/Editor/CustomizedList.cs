using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UsingEditor
{
    public class CustomizedList : MonoBehaviour
    {
        // we do this beacause we call base.OnInspectorGUI();
        // And we don't want that to draw the list aswell.
        [HideInInspector] 
        public List<ICustomizedList> list;
    }
}
