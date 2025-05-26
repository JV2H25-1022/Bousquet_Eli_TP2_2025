using UnityEngine;
using System.Collections.Generic;

public class ObjectScatterer : MonoBehaviour
{
    [Header("Prefabs to Scatter")]
    public List<GameObject> scatterPrefabs;

    [Header("Floors to Scatter On (assign manually or via tag)")]
    public List<GameObject> floorSurfaces;

    [Header("Options")]
    public int objectsPerFloor = 5;
    public float scatterRadius = 2f;
    public float verticalOffset = 0.1f;

    void Start()
    {
        ScatterObjects();
    }

    void ScatterObjects()
    {
        if (scatterPrefabs.Count == 0 || floorSurfaces.Count == 0)
        {
            Debug.LogWarning("Missing scatterPrefabs or floorSurfaces.");
            return;
        }

        foreach (GameObject floor in floorSurfaces)
        {
            Bounds bounds = GetMeshOrColliderBounds(floor);

            for (int i = 0; i < objectsPerFloor; i++)
            {
                Vector3 randomPos = GetRandomPointOnFloor(bounds);
                GameObject prefab = scatterPrefabs[Random.Range(0, scatterPrefabs.Count)];

                Instantiate(prefab, randomPos + Vector3.up * verticalOffset, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            }
        }
    }

    Vector3 GetRandomPointOnFloor(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        float y = bounds.max.y; // Top of the floor

        return new Vector3(x, y, z);
    }

    Bounds GetMeshOrColliderBounds(GameObject obj)
    {
        if (obj.TryGetComponent(out MeshRenderer mesh))
            return mesh.bounds;
        else if (obj.TryGetComponent(out Collider col))
            return col.bounds;
        else
            return new Bounds(obj.transform.position, Vector3.one); // fallback
    }
}

