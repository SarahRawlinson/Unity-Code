using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement
{
    [DisallowMultipleComponent]
    public class PatrolPath : MonoBehaviour
    {
        [SerializeField] Color pathColour = Color.white;
        [SerializeField] float waypointSize = 1f;
        
        
        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int nextChild = 0;
                if (i + 1 < transform.childCount)
                {
                    nextChild = i + 1;
                }
                Gizmos.color = pathColour;
                Gizmos.DrawSphere(ChildPosition(i).position, waypointSize);
                Gizmos.DrawLine(ChildPosition(i).position, ChildPosition(nextChild).position);
            }
        }

        public int GetNumberOfWaypoints()
        {
            //Debug.Log("Child number " + transform.childCount);
            return transform.childCount - 1;
        }

        public int NextWayPoint(int i)
        {
            if (i + 1 <= transform.childCount - 1) return i + 1;
            return 0;
        }

        public Transform ChildPosition(int i)
        {
            //Debug.Log("Child number " + i);
            if (i <= transform.childCount - 1)
            {
                //Debug.Log("Child number " + i);
                return transform.GetChild(i).transform;                
            }
            else
            {
                //Debug.Log("Child number " + 0);
                return transform.GetChild(0).transform;
            }    
        }
    }
}
