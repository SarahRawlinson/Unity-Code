using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class NavMeshRayCastCameraMover : MonoBehaviour
    {
        NavMeshAgent navMeshAgent;
        private void Awake() => navMeshAgent = GetComponent<NavMeshAgent>();
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool hitSomething = Physics.Raycast(ray, out RaycastHit hit);
                if (hitSomething)
                {
                    Vector3 clickedWorldPoint = hit.point;
                    navMeshAgent.SetDestination(clickedWorldPoint);
                }
            }
        }
    }
}
