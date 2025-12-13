using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectPoolManager : MonoBehaviour
{
    public static EnemyObjectPoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int size;
        [HideInInspector] public Queue<GameObject> queue; // 각 prefab 타입별로 풀을 관리할 큐
    }

    public List<Pool> pools; // 관리할 풀들의 목록 (인스펙터에서 설정)

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            pool.queue = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform); // 풀의 자식으로 생성
                obj.SetActive(false); // 비활성화
                pool.queue.Enqueue(obj);
            }
            poolDictionary.Add(pool.prefab, pool.queue);
        }
    }

    public GameObject SpawnFromPool(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning($"풀에 {prefab.name} 이 없음. 새로 생성.");
            // 풀에 없는 프리팹 요청 시, 새롭게 생성하여 반환
            GameObject newObj = Instantiate(prefab, transform);
            newObj.SetActive(true); // 활성화하여 반환
            return newObj;
        }

        // 풀이 비었을 경우 (재활용할 오브젝트가 없는 경우) 새로 생성
        if (poolDictionary[prefab].Count == 0)
        {
            GameObject newObj = Instantiate(prefab, transform);
            newObj.SetActive(true); // 활성화하여 반환
            return newObj;
        }

        GameObject objToSpawn = poolDictionary[prefab].Dequeue();
        objToSpawn.SetActive(true);
        return objToSpawn;
    }

    // 오브젝트를 풀로 반환
    public void ReturnToPool(GameObject obj, GameObject originalPrefab)
    {
        if (!poolDictionary.ContainsKey(originalPrefab))
        {
            Destroy(obj); // 풀에 없는 프리팹이라면 파괴
            return;
        }

        obj.SetActive(false); // 비활성화
        obj.transform.SetParent(this.transform);
        poolDictionary[originalPrefab].Enqueue(obj);
    }
}