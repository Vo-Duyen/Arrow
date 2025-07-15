using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace _.Scripts.Player
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class FindEnemy : MonoBehaviour
    {
        [SerializeField] private List<Transform> enemiesInRange = new List<Transform>();
        [SerializeField] private PlayerCombat playerCombat;
        private                  Coroutine      _findEnemy;

        [SerializeField] private bool isAttacking;

        private void Awake()
        {
            playerCombat = transform.GetComponentInParent<PlayerCombat>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                enemiesInRange.Add(other.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            enemiesInRange.Remove(other.transform);
        }

        private void Update()
        {
            if (enemiesInRange.Count > 0)
            {
                if (_findEnemy == null)
                {
                    isAttacking = true;
                    _findEnemy  = StartCoroutine(StartFindEnemy());
                }
            }
            else
            {
                isAttacking = false;
                _findEnemy  = null;
            }
        }

        private IEnumerator StartFindEnemy()
        {
            WaitForSeconds wait = new WaitForSeconds(1f);
            while (isAttacking)
            {
                Transform target = enemiesInRange[0];
                float distanceEnemy = Vector3.Distance(transform.position, target.position);
                foreach (var trans in enemiesInRange)
                {
                    float tmp = Vector3.Distance(trans.position, transform.position);
                    if (distanceEnemy > tmp)
                    {
                        distanceEnemy = tmp;
                        target        = trans;
                    }
                }
                playerCombat.StartAttack(target);
                yield return wait;
            }
        }
    }
}