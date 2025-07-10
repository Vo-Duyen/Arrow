using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DesignPattern.ObjectPool
{
    public static class PoolingManager
    {
        private static Dictionary<int, Pool> _pools;

        private static void Init(GameObject prefab = null)
        {
            if (_pools == null)
            {
                _pools = new Dictionary<int, Pool>();
            }
            if (prefab != null)
            {
                int idObject = prefab.GetInstanceID();
                if (!_pools.ContainsKey(idObject))
                {
                    _pools[idObject] = new Pool(prefab);
                }
            }
        }

        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion quaternion)
        {
            Init(prefab);
            return _pools[prefab.GetInstanceID()].Spawn(position, quaternion);
        }

        public static T Spawn<T>(T prefab, Vector3 positon, Quaternion quaternion) where T : Component
        {
            Init(prefab.gameObject);
            return _pools[prefab.GetInstanceID()].Spawn<T>(positon, quaternion);
        }

        public static void Despawn(GameObject gameObject, UnityAction callback = null)
        {
            Pool p = null;
            foreach (var pool in _pools.Values)
            {
                if (pool.ObjectIDs.Contains(gameObject.GetInstanceID()))
                {
                    p = pool;
                    break;
                }
            }

            if (p == null)
            {
                Object.Destroy(gameObject);
            }
            else
            {
                callback?.Invoke();
                p.Despawn(gameObject);
            }
        }
    }

    public class Pool
    {
        private readonly int _id = 1;
        private readonly Stack<GameObject> _deActiveStack;
        public readonly HashSet<int> ObjectIDs;
        private readonly GameObject _prefab;

        public Pool(GameObject prefab)
        {
            this._prefab = prefab;
            _deActiveStack = new Stack<GameObject>();
            ObjectIDs = new HashSet<int>();
        }

        public GameObject Spawn( Vector3 position, Quaternion quaternion)
        {
            while (true)
            {
                GameObject newObject;
                if (_deActiveStack.Count == 0)
                {
                    newObject = Object.Instantiate(_prefab);
                    newObject.name = _prefab.name + " " + _id;
                    ObjectIDs.Add(newObject.GetInstanceID());
                }
                else
                {
                    newObject = _deActiveStack.Pop();
                    if (!newObject)
                    {
                        continue;
                    }
                }
                newObject.SetActive(true);
                newObject.transform.SetPositionAndRotation(position,quaternion);
                return newObject;
            }
        }

        public T Spawn<T>(Vector3 position, Quaternion quaternion)
        {
            return Spawn(position, quaternion).GetComponent<T>();
        }

        public void Despawn(GameObject gameObject)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            gameObject.SetActive(false);
            _deActiveStack.Push(gameObject);
        }
    }
}