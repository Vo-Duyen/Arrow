using System;
using System.Collections;
using System.Collections.Generic;
using DesignPattern.ObjectPool;
using DesignPattern.Observer;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace _.Scripts.Player
{
    public class PlayerCombat : MonoBehaviour, IDamageable
    {
        private Queue<Tween> _arrTween = new Queue<Tween>();
        
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

        [SerializeField] private Transform  arrowParent;
        [SerializeField] private GameObject arrow;
        [SerializeField] private Vector3    arrowOffset = new Vector3(0f, 0.9f, 0f);
        
        [SerializeField] private Transform target;
        public                   bool      isAttackDone;

        [SerializeField] private Transform objAttackRange;
        private readonly Vector3 _scaleDefault = new Vector3(10f, 0f, 10f);
        
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            
            ApplyConfig();

            StartCoroutine(HealthRegenerate());
            
            // Create attack range and find enemy to attack
            objAttackRange            = transform.GetChild(0);
            objAttackRange.localScale = _scaleDefault * attackRange;
            
            arrow.GetComponent<ArrowController>().attackDamage = attackDamage;
        }

        private void OnEnable()
        {
            _playerGetHit = param => GetHit((float)param);
            
            ObserverManager<EventID>.Instance.RegisterEvent(EventID.PlayerGetHit, _playerGetHit);
        }

        private void OnDisable()
        {
            ObserverManager<EventID>.Instance.RemoveEvent(EventID.PlayerGetHit, _playerGetHit);

            while (_arrTween.Count > 0)
            {
                _arrTween.Dequeue()?.Kill();
            }
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

        public void StartAttack(Transform enemy)
        {
            target = enemy;
            Attack();
        }
        public void Attack()
        {
            StartCoroutine(LookAtTarget());
            animator.SetTrigger(Attack1);
            GameObject arrowClone = PoolingManager.Spawn(arrow, transform.position + arrowOffset, Quaternion.identity);
            arrowClone.transform.SetParent(arrowParent);
            arrowClone.transform.LookAt(target);
            _arrTween.Enqueue(arrowClone.transform.DOMove(target.position + Vector3.up, 1f).OnComplete(() =>
            {
                if (arrowClone.activeSelf)
                {
                    _arrTween.Enqueue(arrowClone.transform.DOMove(arrowClone.transform.position - Vector3.up, 0.5f).OnComplete(() =>
                    {
                        if (arrowClone.activeSelf)
                        {
                            PoolingManager.Despawn(arrowClone);
                        }
                    }));
                }
            }));
        }

        private IEnumerator LookAtTarget()
        {
            WaitForSeconds wait =  new WaitForSeconds(1f / 600);
            while (!isAttackDone)
            {
                transform.LookAt(target);
                transform.Rotate(0f, 90f, 0f);
                yield return wait;
            }

            isAttackDone = false;
        }

        public void GetHit(float amount)
        {
            if (health <= amount - defense)
            {
                health = 0;
                Time.timeScale = 0;
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