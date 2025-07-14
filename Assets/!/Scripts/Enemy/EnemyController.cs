using System;
using System.Collections;
using DesignPattern.Observer;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace _.Scripts.Enemy
{
    public class EnemyController : MonoBehaviour, IDamageable
    {
        private static readonly  int          IsWalking = Animator.StringToHash("IsWalking");
        private static readonly  int          Attack1   = Animator.StringToHash("Attack");
        private static readonly  int          Die       = Animator.StringToHash("Die");
        [SerializeField] private EnemyData    enemyData;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator     animator;
        [SerializeField] private bool         isFollow;
        [SerializeField] private Transform    player;
        
        [Header("Enemy Stats")]
        [SerializeField] private float maxHealth;
        [SerializeField] private float health;
        [SerializeField] private float defense;
        [SerializeField] private float healthRegenRate;

        [Header("Attack Configurations")]
        [SerializeField] private float attackDamage;
        [SerializeField] private float       attackRange;
        [SerializeField] private float       attackCooldown;
        private                  Coroutine _startAttack;

        private void Awake()
        {
            agent  = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            player = GameManager.Instance.player.transform;
            
            ApplyConfig();

            StartCoroutine(HealthRegenerate());
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                if (isFollow)
                {
                    isFollow        = false;
                    animator.SetBool(IsWalking, false);
                }
                agent.isStopped = true;
                Attack();
            }
        }

        private void FixedUpdate()
        {
            if (isFollow)
            {
                agent.SetDestination(player.position);
                animator.SetBool(IsWalking, true);
            }
        }

        private void ApplyConfig()
        {
            maxHealth       = enemyData.maxHealth;
            health          = enemyData.maxHealth;
            defense         = enemyData.defense;
            healthRegenRate = enemyData.healthRegenRate;
            attackDamage    = enemyData.attackDamage;
            attackRange     = enemyData.attackRange;
            attackCooldown  = enemyData.attackCooldown;
            
            agent.updatePosition        = true;
            agent.updateRotation        = true;
            agent.acceleration          = enemyData.acceleration;
            agent.angularSpeed          = enemyData.angularSpeed;
            agent.areaMask              = enemyData.areaMask;
            agent.avoidancePriority     = enemyData.avoidancePriority;
            agent.baseOffset            = enemyData.baseOffset;
            agent.height                = enemyData.height;
            agent.obstacleAvoidanceType = enemyData.obstacleAvoidanceType;
            agent.radius                = enemyData.radius;
            agent.speed                 = enemyData.speed;
            agent.stoppingDistance      = enemyData.stoppingDistance;
        }

        public void Attack()
        {
            _startAttack ??= StartCoroutine(AttackDamage());
        }

        private IEnumerator AttackDamage()
        {
            WaitForSeconds wait = new WaitForSeconds(attackCooldown);
            while (true)
            {
                transform.LookAt(player);
                animator.SetTrigger(Attack1);
                yield return wait;
                if (Vector3.Distance(transform.position, player.position) > attackRange)
                {
                    isFollow        = true;
                    agent.isStopped = false;
                    _startAttack    = null;
                    break;
                }
            }
        }

        public void CheckAttackPlayer()
        {
            if (Vector3.Distance(transform.position, player.position) <= attackRange + 0.5f && Vector3.Angle(transform.forward, player.position - transform.position) <= 15f)
            {
                print("Player is get hit");
                ObserverManager<EventID>.Instance.PostEvent(EventID.PlayerGetHit, attackDamage);
            }
        }

        public void GetHit(float amount)
        {
            if (health <= amount - defense)
            {
                health = 0;
                animator.SetBool(Die, true);
                return;
            }
            health -= amount - defense;
        }

        private IEnumerator HealthRegenerate()
        {
            WaitForSeconds wait = new WaitForSeconds(1f);
            while (true)
            {
                if (health + healthRegenRate <= maxHealth)
                {
                    health += healthRegenRate;
                }
                else if (health < maxHealth)
                {
                    health = maxHealth;
                }
                yield return wait;
            }

        }
    }
}