using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class RoomFurniturePlacer : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private Vector3 roomSize = new Vector3(3.4f, 3.6f, 3.4f);
    [SerializeField] private float wallThickness = 0.1f;
    [SerializeField] private float floorHeightOffset = 0.05f;

    [Header("Furniture Prefabs")]
    [SerializeField] private GameObject bedPrefab;
    [SerializeField] private GameObject nightstandPrefab;
    [SerializeField] private GameObject shelfPrefab;
    [SerializeField] private GameObject sinkPrefab;
    [SerializeField] private GameObject deskPrefab;

    private List<Bounds> occupiedSpaces = new List<Bounds>();

    private void Start()
    {
        PlaceFurniture();
    }

    private void PlaceFurniture()
    {
        Vector3 roomCenter = transform.position;

        // Place Bed in a random corner
        Vector3[] corners = new Vector3[]
        {
            new Vector3(-roomSize.x / 2 + 1f, 0, -roomSize.z / 2 + 1f),
            new Vector3(-roomSize.x / 2 + 1f, 0, roomSize.z / 2 - 1f),
            new Vector3(roomSize.x / 2 - 1f, 0, -roomSize.z / 2 + 1f),
            new Vector3(roomSize.x / 2 - 1f, 0, roomSize.z / 2 - 1f)
        };
        Vector3 bedPos = corners[Random.Range(0, corners.Length)] + roomCenter + Vector3.up * floorHeightOffset;
        Quaternion bedRot = Random.value > 0.5f ? Quaternion.Euler(0, 90, 0) : Quaternion.identity;
        GameObject bed = Instantiate(bedPrefab, bedPos, bedRot, transform);
        occupiedSpaces.Add(GetBounds(bed));

        // Nightstand beside the large side (left/right) of the bed
        Bounds bedBounds = GetBounds(bed);
        Vector3 offsetDir = bed.transform.right;
        float offsetDistance = bedBounds.extents.x + 0.1f;
        Vector3 nightstandPos = bed.transform.position + offsetDir * offsetDistance;
        nightstandPos.y = transform.position.y + floorHeightOffset;
        if (IsSpaceFree(nightstandPrefab, nightstandPos))
        {
            GameObject nightstand = Instantiate(nightstandPrefab, nightstandPos, Quaternion.identity, transform);
            occupiedSpaces.Add(GetBounds(nightstand));
        }

        // Place Shelf
        TryPlaceAgainstWall(shelfPrefab);

        // Place Sink (elevated)
        TryPlaceWallMounted(sinkPrefab, 0.75f);

        // Place Desk
        TryPlaceAgainstWall(deskPrefab, avoidAbove: sinkPrefab);
    }

    private void TryPlaceAgainstWall(GameObject prefab, GameObject avoidAbove = null)
    {
        for (int i = 0; i < 10; i++)
        {
            float x = Random.Range(-roomSize.x / 2 + 0.75f, roomSize.x / 2 - 0.75f);
            float z = Random.Range(-roomSize.z / 2 + 0.75f, roomSize.z / 2 - 0.75f);

            Vector3[] wallOffsets = {
                new Vector3(x, 0, -roomSize.z / 2 + 0.25f),
                new Vector3(x, 0, roomSize.z / 2 - 0.25f),
                new Vector3(-roomSize.x / 2 + 0.25f, 0, z),
                new Vector3(roomSize.x / 2 - 0.25f, 0, z)
            };

            Quaternion[] wallRotations = {
                Quaternion.Euler(0, 0, 0),
                Quaternion.Euler(0, 180, 0),
                Quaternion.Euler(0, 90, 0),
                Quaternion.Euler(0, -90, 0)
            };

            int index = Random.Range(0, wallOffsets.Length);
            Vector3 pos = transform.position + wallOffsets[index];
            pos.y = transform.position.y + floorHeightOffset;

            if (IsSpaceFree(prefab, pos))
            {
                if (avoidAbove != null && IsOverlapping(avoidAbove, pos)) continue;

                GameObject obj = Instantiate(prefab, pos, wallRotations[index], transform);
                occupiedSpaces.Add(GetBounds(obj));
                return;
            }
        }
    }

    private void TryPlaceWallMounted(GameObject prefab, float height)
    {
        for (int i = 0; i < 10; i++)
        {
            float x = Random.Range(-roomSize.x / 2 + 0.3f, roomSize.x / 2 - 0.3f);
            float z = Random.Range(-roomSize.z / 2 + 0.3f, roomSize.z / 2 - 0.3f);

            Vector3[] wallOffsets = {
                new Vector3(x, height, -roomSize.z / 2 + 0.05f),
                new Vector3(x, height, roomSize.z / 2 - 0.05f),
                new Vector3(-roomSize.x / 2 + 0.05f, height, z),
                new Vector3(roomSize.x / 2 - 0.05f, height, z)
            };

            Quaternion[] wallRotations = {
                Quaternion.Euler(0, 0, 0),
                Quaternion.Euler(0, 180, 0),
                Quaternion.Euler(0, 90, 0),
                Quaternion.Euler(0, -90, 0)
            };

            int index = Random.Range(0, wallOffsets.Length);
            Vector3 pos = transform.position + wallOffsets[index];

            if (IsSpaceFree(prefab, pos))
            {
                GameObject obj = Instantiate(prefab, pos, wallRotations[index], transform);
                occupiedSpaces.Add(GetBounds(obj));
                return;
            }
        }
    }

    private bool IsSpaceFree(GameObject prefab, Vector3 position)
    {
        Bounds newBounds = GetBounds(prefab, position);
        foreach (var b in occupiedSpaces)
        {
            if (b.Intersects(newBounds)) return false;
        }
        return true;
    }

    private bool IsOverlapping(GameObject reference, Vector3 testPosition)
    {
        Bounds refBounds = GetBounds(reference);
        Bounds testBounds = GetBounds(reference, testPosition);
        return refBounds.Intersects(testBounds);
    }

    private Bounds GetBounds(GameObject obj)
    {
        Collider col = obj.GetComponentInChildren<Collider>();
        if (col != null) return col.bounds;

        Renderer rend = obj.GetComponentInChildren<Renderer>();
        return rend != null ? rend.bounds : new Bounds(obj.transform.position, obj.transform.localScale);
    }

    private Bounds GetBounds(GameObject prefab, Vector3 position)
    {
        Renderer rend = prefab.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            return new Bounds(position, rend.bounds.size);
        }
        return new Bounds(position, prefab.transform.localScale);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, 0.5f, 0), new Vector3(roomSize.x, 1f, roomSize.z));
    }
}
