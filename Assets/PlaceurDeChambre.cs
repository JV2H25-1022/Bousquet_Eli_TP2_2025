using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomFurniturePlacer : MonoBehaviour
{
    public enum FurnitureType { Bed, Nightstand, Shelf, Sink, Desk }

    [System.Serializable]
    public struct FurniturePrefabEntry
    {
        public FurnitureType type;
        public GameObject prefab;
    }

    [Header("Room Settings")]
    [SerializeField] private Vector3 roomSize = new Vector3(3.6f, 3.6f, 3.6f);
    [SerializeField] private float wallThickness = 0.1f;
    [SerializeField] private float floorHeightOffset = 0.05f;

    [Header("Furniture Prefabs")]
    [SerializeField] private List<FurniturePrefabEntry> furniturePrefabs;

    private Dictionary<FurnitureType, GameObject> prefabDict;
    private List<Bounds> occupiedSpaces = new List<Bounds>();
    private Dictionary<FurnitureType, Bounds> prefabBoundsCache = new Dictionary<FurnitureType, Bounds>();
    private Dictionary<FurnitureType, List<Bounds>> placedFurnitureByType = new Dictionary<FurnitureType, List<Bounds>>();

    private void Awake()
    {
        prefabDict = new Dictionary<FurnitureType, GameObject>();
        foreach (var entry in furniturePrefabs)
        {
            if (!prefabDict.ContainsKey(entry.type))
            {
                prefabDict[entry.type] = entry.prefab;
                prefabBoundsCache[entry.type] = GetLocalBounds(entry.prefab);
                placedFurnitureByType[entry.type] = new List<Bounds>();
            }
        }
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
        temp.SetActive(false);
        Renderer[] renderers = temp.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Destroy(temp);
            return new Bounds(Vector3.zero, Vector3.zero);
        }

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        Destroy(temp);
        return bounds;
    }

    private void PlaceFurniture()
    {
        Vector3 roomCenter = transform.position;

        GameObject bedPrefab = GetPrefab(FurnitureType.Bed);
        Vector3[] corners = new Vector3[]
        {
            new Vector3(-roomSize.x / 2 + wallThickness, 0, -roomSize.z / 2 + wallThickness),
            new Vector3(-roomSize.x / 2 + wallThickness, 0, roomSize.z / 2 - wallThickness),
            new Vector3(roomSize.x / 2 - wallThickness, 0, -roomSize.z / 2 + wallThickness),
            new Vector3(roomSize.x / 2 - wallThickness, 0, roomSize.z / 2 - wallThickness)
        };

        int cornerIndex = Random.Range(0, corners.Length);
        Quaternion bedRot = (cornerIndex < 2) ? Quaternion.identity : Quaternion.Euler(0, 90, 0);
        GameObject bed = Instantiate(bedPrefab, Vector3.zero, bedRot, transform);
        Bounds bedBounds = GetBounds(bed);
        Vector3 offset = new Vector3(
            (cornerIndex >= 2 ? -1 : 1) * bedBounds.extents.x,
            0,
            (cornerIndex % 2 == 1 ? -1 : 1) * bedBounds.extents.z
        );

        bed.transform.position = transform.position + corners[cornerIndex] + offset + Vector3.up * (bedBounds.extents.y + floorHeightOffset);
        bedBounds = GetBounds(bed);
        occupiedSpaces.Add(bedBounds);
        placedFurnitureByType[FurnitureType.Bed].Add(bedBounds);

        // Place Nightstand beside Bed
        GameObject nightstandPrefab = GetPrefab(FurnitureType.Nightstand);
        Bounds nightstandBounds = prefabBoundsCache[FurnitureType.Nightstand];
        Vector3 longSideDir = bedBounds.size.x > bedBounds.size.z ? bed.transform.right : bed.transform.forward;
        float bedHalfLength = Mathf.Max(bedBounds.extents.x, bedBounds.extents.z);
        float nightstandOffset = nightstandBounds.extents.x + wallThickness;
        Vector3 nightstandPos = bed.transform.position + longSideDir * (bedHalfLength + nightstandOffset);
        nightstandPos.y = transform.position.y + floorHeightOffset + nightstandBounds.extents.y;

        if (IsSpaceFree(nightstandBounds, nightstandPos))
        {
            GameObject nightstand = Instantiate(nightstandPrefab, nightstandPos, bed.transform.rotation, transform);
            Bounds nsBounds = GetBounds(nightstand);
            occupiedSpaces.Add(nsBounds);
            placedFurnitureByType[FurnitureType.Nightstand].Add(nsBounds);
        }

        // Place Shelf
        TryPlaceAgainstWall(FurnitureType.Shelf);

        // Place Sink
        TryPlaceWallMounted(FurnitureType.Sink, 0.75f);

        // Place Desk (not under sink or above bed)
        TryPlaceAgainstWall(FurnitureType.Desk, avoidAbove: FurnitureType.Sink);
    }

    private void TryPlaceAgainstWall(FurnitureType type, FurnitureType? avoidAbove = null)
    {
        GameObject prefab = GetPrefab(type);
        Bounds prefabBounds = prefabBoundsCache[type];

        Vector3[] wallNormals = {
            new Vector3(0, 0, -1), new Vector3(0, 0, 1),
            new Vector3(-1, 0, 0), new Vector3(1, 0, 0)
        };

        Quaternion[] rotations = {
            Quaternion.identity, Quaternion.Euler(0, 180, 0),
            Quaternion.Euler(0, 90, 0), Quaternion.Euler(0, -90, 0)
        };

        for (int wall = 0; wall < 4; wall++)
        {
            for (int attempt = 0; attempt < 5; attempt++)
            {
                float posX = 0f, posZ = 0f;
                float halfWidth = prefabBounds.extents.x;
                float halfDepth = prefabBounds.extents.z;

                if (wall == 0 || wall == 1) // Back or Front wall
                {
                    posX = Random.Range(-roomSize.x / 2 + halfWidth, roomSize.x / 2 - halfWidth);
                    posZ = (wall == 0)
                        ? -roomSize.z / 2 + wallThickness + halfDepth
                        : roomSize.z / 2 - wallThickness - halfDepth;
                }
                else // Left or Right wall
                {
                    posZ = Random.Range(-roomSize.z / 2 + halfDepth, roomSize.z / 2 - halfDepth);
                    posX = (wall == 2)
                        ? -roomSize.x / 2 + wallThickness + halfWidth
                        : roomSize.x / 2 - wallThickness - halfWidth;
                }

                Vector3 pos = transform.position + new Vector3(posX, 0, posZ);
                pos.y = transform.position.y + floorHeightOffset + prefabBounds.extents.y;

                if (IsSpaceFree(prefabBounds, pos))
                {
                    if (avoidAbove.HasValue && IsOverlappingAny(avoidAbove.Value, prefabBounds, pos)) continue;
                    GameObject obj = Instantiate(prefab, pos, rotations[wall], transform);
                    Bounds placedBounds = GetBounds(obj);
                    occupiedSpaces.Add(placedBounds);
                    placedFurnitureByType[type].Add(placedBounds);
                    return;
                }
            }
        }
    }

    private void TryPlaceWallMounted(FurnitureType type, float height)
    {
        GameObject prefab = GetPrefab(type);
        Bounds prefabBounds = prefabBoundsCache[type];
        float halfWidth = prefabBounds.extents.x;
        float halfDepth = prefabBounds.extents.z;

        for (int i = 0; i < 10; i++)
        {
            float x = Random.Range(-roomSize.x / 2 + halfWidth, roomSize.x / 2);
            float z = Random.Range(-roomSize.z / 2 + halfDepth, roomSize.z / 2);

            Vector3[] wallOffsets = {
                new Vector3(x, height, -roomSize.z / 2 + wallThickness + halfDepth),
                new Vector3(x, height, roomSize.z / 2 - wallThickness - halfDepth),
                new Vector3(-roomSize.x / 2 + wallThickness + halfWidth, height, z),
                new Vector3(roomSize.x / 2 - wallThickness - halfWidth, height, z)
            };

            Quaternion[] rotations = {
                Quaternion.Euler(0, 90, 0),
                Quaternion.Euler(0, -90, 0),
                Quaternion.Euler(0, 180, 0),
                Quaternion.Euler(0, 0, 0)
            };

            int index = Random.Range(0, wallOffsets.Length);
            Vector3 pos = transform.position + wallOffsets[index];
            pos.y = transform.position.y + height + prefabBounds.extents.y;

            if (IsSpaceFree(prefabBounds, pos))
            {
                GameObject obj = Instantiate(prefab, pos, rotations[index], transform);
                Bounds bounds = GetBounds(obj);
                occupiedSpaces.Add(bounds);
                placedFurnitureByType[type].Add(bounds);
                return;
            }
        }
    }

    private bool IsSpaceFree(Bounds bounds, Vector3 position)
    {
        Bounds newBounds = new Bounds(position, bounds.size);
        foreach (var b in occupiedSpaces)
        {
            if (b.Intersects(newBounds)) return false;
        }
        return true;
    }

    private bool IsOverlappingAny(FurnitureType type, Bounds newBounds, Vector3 newPosition)
    {
        Bounds candidate = new Bounds(newPosition, newBounds.size);
        foreach (var placed in placedFurnitureByType[type])
        {
            if (placed.Intersects(candidate)) return true;
        }
        return false;
    }

    private Bounds GetBounds(GameObject obj)
    {
        Renderer rend = obj.GetComponentInChildren<Renderer>();
        return rend != null ? rend.bounds : new Bounds(obj.transform.position, obj.transform.localScale);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, roomSize.y / 2, 0), new Vector3(roomSize.x, roomSize.y, roomSize.z));
    }
}
