using System;
using System.Collections;
using System.Collections.Generic;
using DesignPattern.ObjectPool;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _.Scripts.Enemy
{
    public class SpawnEnemy : MonoBehaviour
    {
        [SerializeField] private GameObject           enemy;
        [SerializeField] private NavMeshAgent         agent;
        private                  NavMeshTriangulation _triangulation;
        [SerializeField] private int                  numberOfEnemiesToSpawn;
        [SerializeField] private float                timeDelay;
        
        private void Awake()
        {
            agent          = enemy.GetComponent<NavMeshAgent>();
            _triangulation = NavMesh.CalculateTriangulation();

            StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            WaitForSeconds wait = new WaitForSeconds(timeDelay);
            int cntEnemy = 0;
            while (cntEnemy < numberOfEnemiesToSpawn)
            {
                int id = Random.Range(0, _triangulation.vertices.Length);
                if (NavMesh.SamplePosition(_triangulation.vertices[id], out NavMeshHit hit, 2f, -1))
                {
                    GameObject enemyClone  = PoolingManager.Spawn(enemy, transform.position, Quaternion.identity);
                    enemyClone.transform.SetParent(transform);
                    enemyClone.GetComponent<NavMeshAgent>().Warp(hit.position);
                    ++cntEnemy;
                }
                yield return wait;
            }
        }
    }
}