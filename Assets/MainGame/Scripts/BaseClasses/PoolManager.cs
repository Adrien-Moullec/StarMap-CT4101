using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [SerializeField] public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake() {
        Instance = this;

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools) {

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public bool TrySpawnFromPool<T>(string poolPrefabId, out T poolItem) {

        if (!poolDictionary.ContainsKey(poolPrefabId)) {
            poolItem = default;
            Debug.LogError("No dictionary with this ID");
            return false;
        }        

        GameObject obj = poolDictionary[poolPrefabId].Dequeue();

        obj.SetActive(true);

        if (!obj.TryGetComponent<T>(out poolItem)) {
            poolItem = default;
            Debug.LogError("Object does not have this component");
            return false;
        }

        poolItem = obj.GetComponent<T>();
        return true;
    }
}

[System.Serializable]
public class Pool {
    public string tag;
    public GameObject prefab;
    public int size;
}