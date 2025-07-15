using System;
using System.Collections;
using DesignPattern.ObjectPool;
using UnityEngine;
using UnityEngine.AI;

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
            agent = enemy.GetComponent<NavMeshAgent>();
            _triangulation = NavMesh.CalculateTriangulation();

            StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            WaitForSeconds wait = new WaitForSeconds(timeDelay);
            int cntEnemy = 0;
            while (cntEnemy < numberOfEnemiesToSpawn)
            {
                int        vertexIndex = UnityEngine.Random.Range(0, _triangulation.vertices.Length);
                if (NavMesh.SamplePosition(_triangulation.vertices[vertexIndex], out NavMeshHit hit, 1f,
                                           NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                }
                GameObject enemyClone  = PoolingManager.Spawn(enemy, transform.position, Quaternion.identity);
                enemyClone.transform.SetParent(transform);
                ++cntEnemy;
                yield return wait;
            }
        }
    }
}