using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    

    [Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> _poolDictionary;
    
    public static ObjectPooler Instance;
    private void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        _poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            _poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string pooltag, Vector3 position, Quaternion rotation)
    {
        if (!_poolDictionary.ContainsKey(pooltag))
        {
            Debug.LogWarning($"Pool with tag {pooltag} doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = _poolDictionary[pooltag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        _poolDictionary[pooltag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}