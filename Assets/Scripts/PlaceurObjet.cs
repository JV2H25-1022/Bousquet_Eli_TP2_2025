using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject lampTorchPrefab;
    public GameObject maskPrefab;

    public string shelfTag = "Shelf";
    public string deskTag = "Desk";

    void Start()
    {
        StartCoroutine(DelayedSpawn());
    }

    IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(0.5f); // wait for rooms/furniture to spawn
        SpawnCollectibles();
    }

    void SpawnCollectibles()
    {
        GameObject[] shelves = GameObject.FindGameObjectsWithTag(shelfTag);
        GameObject[] desks = GameObject.FindGameObjectsWithTag(deskTag);

        if (shelves.Length == 0 || desks.Length == 0)
        {
            Debug.LogWarning("No shelves or desks found in the scene!");
            return;
        }

        GameObject selectedShelf = shelves[Random.Range(0, shelves.Length)];
        GameObject selectedDesk = desks[Random.Range(0, desks.Length)];

        // Random assignment
        bool lampOnShelf = Random.value > 0.5f;

        if (lampOnShelf)
        {
            SpawnAtPoint(lampTorchPrefab, selectedShelf.transform);
            SpawnAtPoint(maskPrefab, selectedDesk.transform);
        }
        else
        {
            SpawnAtPoint(maskPrefab, selectedShelf.transform);
            SpawnAtPoint(lampTorchPrefab, selectedDesk.transform);
        }

        Debug.Log("Spawned lamp and mask.");
    }

    void SpawnAtPoint(GameObject prefab, Transform furnitureTransform)
    {
        Transform spawnPoint = furnitureTransform.Find("SpawnPoint");

        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : furnitureTransform.position;
        Quaternion spawnRot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        Instantiate(prefab, spawnPos, spawnRot);
    }
}

