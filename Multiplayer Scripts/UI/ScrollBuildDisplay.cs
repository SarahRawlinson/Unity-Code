using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerRTS.UI
{
    public class ScrollBuildDisplay : MonoBehaviour
    {
        [SerializeField] ScrollRect scroll;
        [SerializeField] GameObject button;


        public void AddContent(GameObject button)
        {
            Instantiate(button, scroll.content);
        }
    }
}
