// Author: DanlangA

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace _.Scripts.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyMovement : MonoBehaviour
    {
        [HideInInspector]
        public NavMeshAgent agent;
        
        public Transform target;
        
        public float speed;
        public bool  isFollow;

        private void Awake()
        {
            target = GameManager.Instance.player.transform;
            agent  = GetComponent<NavMeshAgent>();
            StartCoroutine(FollowTarget(speed));
        }

        private IEnumerator FollowTarget(float moveSpeed)
        {
            WaitForSeconds wait = new WaitForSeconds(moveSpeed);
            while (isFollow)
            {
                agent.SetDestination(target.position);
                yield return wait;
            }
        }
    }
}