using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    public class ObjectPooler : MonoBehaviour
    {
        [Serializable]
        public class Pool
        {
            public GameObject prefab;
            private List<GameObject> _pool = new List<GameObject>();
            public GameObject GetPooledObject(Transform parent)
            {
                foreach (var obj in _pool)
                {
                    if (!obj.activeSelf)
                    {
                        obj.SetActive(true);
                        return obj;
                    }
                }
            
                var newObject = Instantiate(prefab, parent);
                _pool.Add(newObject);
                return newObject;
            }
        }
    
        private readonly Dictionary<Type, Pool> _poolDictionary = new Dictionary<Type, Pool>();
        public static ObjectPooler Instance;
        private void Awake()
        {
            Instance = this;
        }

        public void RegisterPool<T>(GameObject prefab)
        {
            _poolDictionary[typeof(T)] = new Pool()
            {
                prefab = prefab,
            };
        }

        public T SpawnFromPool<T>(Vector3 position, Quaternion rotation)
        {
            if (!_poolDictionary.TryGetValue(typeof(T), out var pool))
            {
                Debug.LogError("No pool registered for type: " + typeof(T));
                return default;
            }
        
            GameObject objectToSpawn = pool.GetPooledObject(transform);

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            return objectToSpawn.GetComponent<T>();
        }
    }
}