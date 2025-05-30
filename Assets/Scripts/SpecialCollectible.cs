using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCollectible : MonoBehaviour, IInteractable, ICollectible, IHasViewSource
{
    [Header("Prompt Settings")]
    [SerializeField] private GameObject floatingPromptPrefab;
    [SerializeField] private Vector3 promptOffset = new Vector3(0, 0, 0);

    private GameObject floatingPromptInstance;
    private Transform viewSource;
    private Transform holdPoint;

    private Collider col;

    private bool isPickedUp = false;

    private void Awake()
    {

        col = GetComponent<Collider>();

    }

    private void Start()
    {



        if (floatingPromptPrefab != null)
        {
            floatingPromptInstance = Instantiate(floatingPromptPrefab);
            floatingPromptInstance.SetActive(false);

            // Immediately position the prompt near this object
            if (viewSource != null)
            {
                Vector3 toPlayer = (viewSource.position - transform.position).normalized;
                Vector3 right = Vector3.Cross(Vector3.up, toPlayer).normalized;

                Vector3 offset =
                    toPlayer * promptOffset.z +
                    Vector3.up * promptOffset.y +
                    right * promptOffset.x;

                floatingPromptInstance.transform.position = transform.position + offset;
                floatingPromptInstance.transform.rotation = Quaternion.LookRotation(toPlayer);
            }
            else
            {
                // Default initial position — directly above the object until the viewSource is set
                floatingPromptInstance.transform.position = transform.position + Vector3.up * promptOffset.y;
            }
        }
    }

    private void Update()
    {
        if (floatingPromptInstance && floatingPromptInstance.activeSelf && viewSource != null)
        {
            Vector3 toPlayer = (viewSource.position - transform.position).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, toPlayer).normalized;

            Vector3 offset =
                toPlayer * promptOffset.z +
                Vector3.up * promptOffset.y +
                right * promptOffset.x;

            floatingPromptInstance.transform.position = transform.position + offset;

            // Rotate prompt to always face the player
            floatingPromptInstance.transform.rotation = Quaternion.LookRotation(toPlayer * -1);
        }
    }

    public void SetViewSource(Transform playerViewSource)
    {
        viewSource = playerViewSource;
    }

    public void ShowPrompt(bool show)
    {
        if (floatingPromptInstance != null)
        {
            floatingPromptInstance.SetActive(show);
            Debug.Log("Prompt showing2");
        }

    }

    public void Interact()
    {
        if (!isPickedUp && holdPoint != null)
        {
            StopAllCoroutines();

            StartCoroutine(PickupSmooth(holdPoint));

            Debug.Log("Interact");
        }
    }

    public void OnPickup(Transform holder)
    {
        if (holder == null)
        {
            Debug.LogWarning("Hold point is null!");
            return;
        }

        holdPoint = holder;

        Interact();
        Debug.Log("Pickup2");
    }

    private IEnumerator PickupSmooth(Transform target)
    {
        isPickedUp = true;
        float duration = 0.3f;
        float elapsed = 0f;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        col.enabled = false;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, target.position, elapsed / duration);
            transform.rotation = Quaternion.Slerp(startRot, target.rotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target.position;
        transform.localRotation = target.rotation;

        transform.SetParent(target);
        ShowPrompt(false);

    }


    public void OnDrop() { }  // Not applicable, kept for interface compliance
    public void OnThrow(Vector3 force) { } // Not applicable
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(promptOffset), 0.1f);
    }
}

