// Author: DanlangA

using System;
using UnityEngine;
using UnityEngine.AI;

namespace _.Scripts.Player
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerMovement : MonoBehaviour
    {
        [HideInInspector]
        public NavMeshAgent agent;

        private RaycastHit[] hits = new RaycastHit[1];
        
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.RaycastNonAlloc(ray, hits) > 0)
                {
                    agent.SetDestination(hits[0].point);
                }
            }
        }
    }
}