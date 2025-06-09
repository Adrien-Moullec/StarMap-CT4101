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

            pool.poolParent = Instantiate(new GameObject());
            pool.poolParent.name = pool.tag;
            pool.poolParent.transform.parent = transform;

            for (int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.parent = pool.poolParent.transform;
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public bool TrySpawnFromPool<T>(string poolId, out T poolItem) {

        if (!poolDictionary.ContainsKey(poolId)) {
            poolItem = default;
            Debug.LogError("No dictionary with this ID");
            return false;
        }        

        GameObject obj = SpawnFromPool(poolId);

        if (!obj.TryGetComponent<T>(out poolItem)) {
            poolItem = default;
            Debug.LogError("Object does not have this component");
            return false;
        }

        poolItem = obj.GetComponent<T>();
        return true;
    }

    public GameObject SpawnFromPool(string poolId) {

        if (!poolDictionary.ContainsKey(poolId)) {
            Debug.LogError("No dictionary with this ID");
            return null;
        }

        GameObject obj = poolDictionary[poolId].Dequeue();
        obj.SetActive(true);
        poolDictionary[poolId].Enqueue(obj);
        return obj;
    }
    
    public void DespawnByTag(string poolId) {
        if (!poolDictionary.ContainsKey(poolId)) {
            Debug.LogError("No dictionary with this ID");
            return;
        }

        foreach (GameObject i in poolDictionary[poolId]) {
            i.SetActive(false);
        }
    }
}

[System.Serializable]
public class Pool {
    [HideInInspector] public GameObject poolParent;
    public string tag;
    public GameObject prefab;
    public int size;
}