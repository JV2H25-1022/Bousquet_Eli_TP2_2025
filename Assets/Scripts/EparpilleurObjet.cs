using UnityEngine;
using System.Collections.Generic;

public class ObjectScattererWithPooling : MonoBehaviour
{
    [System.Serializable]
    public class ScatterObject
    {
        public GameObject prefab;
        public int poolSize = 10;
    }

    public List<ScatterObject> objectsToScatter;
    public List<Transform> floorAreas;
    public int objectsPerFloor = 5;
    public float scatterRadius = 2f;
    public float activationDistance = 10f;
    public Transform player;

    private Dictionary<GameObject, Queue<GameObject>> pools = new();
    private List<GameObject> allSpawnedObjects = new();

    void Start()
    {
        InitializePools();
        ScatterObjects();
    }

    void Update()
    {
        foreach (var obj in allSpawnedObjects)
        {
            float distance = Vector3.Distance(player.position, obj.transform.position);
            bool shouldBeActive = distance < activationDistance;

            if (obj.activeSelf != shouldBeActive)
            {
                obj.SetActive(shouldBeActive);
            }
        }
    }

    void InitializePools()
    {
        foreach (var item in objectsToScatter)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < item.poolSize; i++)
            {
                GameObject obj = Instantiate(item.prefab);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
            pools[item.prefab] = pool;
        }
    }

    void ScatterObjects()
    {
        foreach (var floor in floorAreas)
        {
            for (int i = 0; i < objectsPerFloor; i++)
            {
                var objType = objectsToScatter[Random.Range(0, objectsToScatter.Count)];
                var pool = pools[objType.prefab];

                if (pool.Count == 0) continue;

                GameObject obj = pool.Dequeue();
                Vector3 randomPos = floor.position + new Vector3(
                    Random.Range(-scatterRadius, scatterRadius),
                    0.1f,
                    Random.Range(-scatterRadius, scatterRadius)
                );
                Quaternion randomRot = Quaternion.Euler(
                   -90f,
                   Random.Range(0f, 359f),
                   0f
               );

                obj.transform.position = randomPos;
                obj.transform.rotation = randomRot;

                pool.Enqueue(obj); // Keep it in the cycle

                // Add to active list for distance checking
                allSpawnedObjects.Add(obj);

                pool.Enqueue(obj); // Optional: Keep it in cycle
            }
        }
    }
    
    void OnDrawGizmosSelected()
{
    if (player != null)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.position, activationDistance);
    }
}

}



