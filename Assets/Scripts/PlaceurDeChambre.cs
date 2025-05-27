using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RoomFurniturePlacer : MonoBehaviour
{
    public enum FurnitureType { Bed, Nightstand, Shelf, Sink, Desk, door }

    [System.Serializable]
    public struct FurniturePrefabEntry
    {
        public FurnitureType type;
        public GameObject prefab;
    }

    [Header("Room Settings")]
    [SerializeField] private Vector3 roomSize = new Vector3(3.6f, 3.6f, 3.6f);
    [SerializeField] private float wallThickness = 0.1f;
    
    [Header("Door Settings")]
    [SerializeField] private float doorWidth = 2.7f;
    [SerializeField] private float doorHeight = 2.0f;
    [SerializeField] private float doorBottomY = 0.0f;

    [Header("Furniture Prefabs")]
    [SerializeField] private List<FurniturePrefabEntry> furniturePrefabs;

    [Header("Furniture Layouts")]
    [SerializeField] private List<FurnitureLayout> layoutOptions;

    private Dictionary<FurnitureType, GameObject> prefabDict;
    private List<Bounds> occupiedSpaces;
    private Dictionary<FurnitureType, Bounds> prefabBoundsCache;
    private Dictionary<FurnitureType, List<Bounds>> placedFurnitureByType;

    private void Awake()
    {
        InitializePrefabData();
    }

    private void InitializePrefabData()
    {
        if (prefabDict != null && prefabDict.Count > 0)
            return;

        prefabDict = new Dictionary<FurnitureType, GameObject>();
        prefabBoundsCache = new Dictionary<FurnitureType, Bounds>();
        placedFurnitureByType = new Dictionary<FurnitureType, List<Bounds>>();
        occupiedSpaces = new List<Bounds>();

        foreach (var entry in furniturePrefabs)
        {
            if (!prefabDict.ContainsKey(entry.type))
            {
                prefabDict[entry.type] = entry.prefab;
                prefabBoundsCache[entry.type] = GetLocalBounds(entry.prefab);
                placedFurnitureByType[entry.type] = new List<Bounds>();
            }
        }

        AddDoorAreaToOccupied();
    }

    private void AddDoorAreaToOccupied()
    {
        float doorMinZ = -doorWidth / 2f;
        float doorMaxZ = doorWidth / 2f;
        Vector3 doorCenter = transform.position + new Vector3(-roomSize.x / 2 + wallThickness / 2, doorBottomY + doorHeight / 2, 0);
        Vector3 doorSize = new Vector3(wallThickness, doorHeight, doorWidth);
        Bounds doorBounds = new Bounds(doorCenter, doorSize);
        occupiedSpaces.Add(doorBounds);
    }

    private void Start()
    {
        PlaceFurniture();
    }

    private GameObject GetPrefab(FurnitureType type)
    {
        return prefabDict[type];
    }

    private Bounds GetLocalBounds(GameObject prefab)
    {
        GameObject temp = Instantiate(prefab);
        temp.hideFlags = HideFlags.HideAndDontSave;
        temp.SetActive(false);
        Renderer[] renderers = temp.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            DestroyImmediate(temp);
            return new Bounds(Vector3.zero, Vector3.zero);
        }

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        DestroyImmediate(temp);
        return bounds;
    }

    [ContextMenu("Place Furniture")]
    public void PlaceFurnitureInEditor()
    {
        ClearExistingFurniture();
        InitializePrefabData();
        PlaceFurniture();
    }

    private void ClearExistingFurniture()
    {
        var children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }
#if UNITY_EDITOR
        foreach (var child in children)
        {
            DestroyImmediate(child);
        }
#else
        foreach (var child in children)
        {
            Destroy(child);
        }
#endif
        occupiedSpaces?.Clear();
        placedFurnitureByType?.Clear();
    }

    private void PlaceFurniture()
    {
        if (layoutOptions == null || layoutOptions.Count == 0) return;

        FurnitureLayout layout = layoutOptions[Random.Range(0, layoutOptions.Count)];

        foreach (var item in layout.items)
        {
            if (!prefabDict.TryGetValue(item.type, out GameObject prefab)) continue;

            GameObject instance = Instantiate(prefab, transform);
            instance.transform.localPosition = item.position;
            instance.transform.localRotation = Quaternion.Euler(item.rotationEuler);

            Bounds localBounds = prefabBoundsCache[item.type];
            Bounds worldBounds = new Bounds(transform.TransformPoint(item.position), localBounds.size);

            if (!placedFurnitureByType.ContainsKey(item.type))
                placedFurnitureByType[item.type] = new List<Bounds>();

            placedFurnitureByType[item.type].Add(worldBounds);
            occupiedSpaces.Add(worldBounds);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, roomSize.y / 2, 0), new Vector3(roomSize.x, roomSize.y, roomSize.z));

        Gizmos.color = Color.red;
        Vector3 center = transform.position + new Vector3(-roomSize.x / 2 + wallThickness / 2, doorBottomY + doorHeight / 2, 0);
        Vector3 size = new Vector3(wallThickness, doorHeight, doorWidth);
        Gizmos.DrawCube(center, size);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            foreach (var kvp in placedFurnitureByType)
            {
                Gizmos.color = GetColorForType(kvp.Key);
                foreach (var b in kvp.Value)
                {
                    Gizmos.DrawWireCube(b.center, b.size);
                }
            }
        }
    }

    private Color GetColorForType(FurnitureType type)
    {
        switch (type)
        {
            case FurnitureType.Bed: return Color.red;
            case FurnitureType.Nightstand: return Color.yellow;
            case FurnitureType.Shelf: return Color.blue;
            case FurnitureType.Sink: return Color.cyan;
            case FurnitureType.Desk: return Color.magenta;
            default: return Color.white;
        }
    }
}
