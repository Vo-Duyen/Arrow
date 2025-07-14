using System;
using System.Collections;
using System.Collections.Generic;
using DesignPattern.Observer;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace _.Scripts.Player
{
    public class PlayerCombat : MonoBehaviour, IDamageable
    {
        private static readonly  int          IsWalking = Animator.StringToHash("IsWalking");
        private static readonly  int          Attack1   = Animator.StringToHash("Attack");
        private static readonly  int          Die       = Animator.StringToHash("Die");

        private Action<object> _playerGetHit;
        
        [SerializeField] private PlayerData playerData;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator     animator;
        
        [Header("Enemy Stats")]
        [SerializeField] private float maxHealth;
        [SerializeField] private float health;
        [SerializeField] private float   defense;
        [SerializeField] private float healthRegenRate;

        [Header("Attack Configurations")]
        [SerializeField] private float attackDamage;
        [SerializeField] private float attackRange;
        [SerializeField] private float attackCooldown;

        private Coroutine _startAttack;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            
            ApplyConfig();

            StartCoroutine(HealthRegenerate());
        }

        private void OnEnable()
        {
            _playerGetHit = param => GetHit((float)param);
            
            ObserverManager<EventID>.Instance.RegisterEvent(EventID.PlayerGetHit, _playerGetHit);
        }

        private void OnDisable()
        {
            ObserverManager<EventID>.Instance.RemoveEvent(EventID.PlayerGetHit, _playerGetHit);
        }

        private void ApplyConfig()
        {
            maxHealth = playerData.maxHealth;
            health          = playerData.maxHealth;
            defense         = playerData.defense;
            healthRegenRate = playerData.healthRegenRate;
            attackDamage    = playerData.attackDamage;
            attackRange     = playerData.attackRange;
            attackCooldown  = playerData.attackCooldown;
            
            agent.updatePosition        = true;
            agent.updateRotation        = true;
            agent.acceleration          = playerData.acceleration;
            agent.angularSpeed          = playerData.angularSpeed;
            agent.areaMask              = playerData.areaMask;
            agent.avoidancePriority     = playerData.avoidancePriority;
            agent.baseOffset            = playerData.baseOffset;
            agent.height                = playerData.height;
            agent.obstacleAvoidanceType = playerData.obstacleAvoidanceType;
            agent.radius                = playerData.radius;
            agent.speed                 = playerData.speed;
            agent.stoppingDistance      = playerData.stoppingDistance;
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
                // transform.LookAt(player);
                // animator.SetTrigger(Attack1);
                // yield return wait;
                // if (Vector3.Distance(transform.position, player.position) > attackRange)
                // {
                //     agent.isStopped = false;
                //     _startAttack    = null;
                //     break;
                // }
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