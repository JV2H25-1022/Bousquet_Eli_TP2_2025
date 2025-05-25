
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
        yield return new WaitForSeconds(0.5f); 
        SpawnCollectibles();
    }


    void SpawnCollectibles()
    {
        GameObject[] shelves = GameObject.FindGameObjectsWithTag(shelfTag);
        GameObject[] desks = GameObject.FindGameObjectsWithTag(deskTag);

        List<GameObject> validShelves = new List<GameObject>(shelves);
        List<GameObject> validDesks = new List<GameObject>(desks);

        if (validShelves.Count == 0 || validDesks.Count == 0)
        {
            Debug.LogWarning("No shelves or desks found in the scene!");
            return;
        }

        // Randomly decide which item goes where
        bool lampOnShelf = Random.value > 0.5f;

        // Choose one random shelf and desk
        GameObject selectedShelf = validShelves[Random.Range(0, validShelves.Count)];
        GameObject selectedDesk = validDesks[Random.Range(0, validDesks.Count)];

        // Spawn lamp and mask at predefined child position (placeholder for now)
        if (lampOnShelf)
        {
            Instantiate(lampTorchPrefab, selectedShelf.transform.position + Vector3.up * -0.5f, Quaternion.identity);
            Instantiate(maskPrefab, selectedDesk.transform.position + Vector3.up * 0.41f, Quaternion.identity);
        }
        else
        {
            Instantiate(maskPrefab, selectedShelf.transform.position + Vector3.up * -0.5f, Quaternion.identity);
            Instantiate(lampTorchPrefab, selectedDesk.transform.position + Vector3.up * 0.425f, Quaternion.identity);
        }
    }
}

