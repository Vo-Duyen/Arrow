using UnityEngine;
using UnityEngine.AI;

namespace _.Scripts.Enemy
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Data/New Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        // Enemy Stats
        public float   maxHealth       = 100f;
        public float   defense         = 5f;
        public float healthRegenRate = 0f;
        
        // Attack Configurations
        public float   attackDamage   = 10f;
        public float attackRange    = 2f;
        public float attackCooldown = 1f;
        
        // Sound & Effects
        public AudioClip attackSound;
        public AudioClip deathSound;
        public GameObject deathEffect;
        
        // NavMeshAgent Configs
        public float                 aiUpdateInterval      = 0.1f;
        public float                 acceleration          = 8f;
        public float                 angularSpeed          = 120f;
        public int                   areaMask              = -1;
        public int                   avoidancePriority     = 50;
        public float                 baseOffset            = 0f;
        public float                 height                = 2f;
        public ObstacleAvoidanceType obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        public float                 radius                = 0.5f;
        public float                 speed                 = 3f;
        public float                 stoppingDistance      = 0.5f;
    }
}