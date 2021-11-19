using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerRTS.Control
{
    public class BaseObject : MonoBehaviour
    {
        [SerializeField] public float baseRange = 50f;

        public float BaseRange => baseRange;
    }
}
