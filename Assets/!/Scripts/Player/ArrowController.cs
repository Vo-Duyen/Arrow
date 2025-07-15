using System;
using DesignPattern.ObjectPool;
using DesignPattern.Observer;
using UnityEngine;

namespace _.Scripts.Player
{
    public class ArrowController : MonoBehaviour
    {
        public float attackDamage;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                PoolingManager.Despawn(gameObject);
                ObserverManager<EventID>.Instance.PostEvent(EventID.EnemyGetHit, (other.transform, attackDamage));
            }
        }
    }
}