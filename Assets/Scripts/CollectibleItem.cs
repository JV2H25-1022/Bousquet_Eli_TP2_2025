using System.Collections;
using UnityEngine;

public class CollectibleItem : MonoBehaviour, IInteractable, ICollectible, IHasViewSource
{
    [Header("Prompt Settings")]
    [SerializeField] private GameObject floatingPromptPrefab;
    [SerializeField] private Vector3 promptOffset = Vector3.zero;

    private GameObject floatingPromptInstance;
    private Transform viewSource;
    private Transform holdPoint;
    private Rigidbody rb;
    private Collider col;
    private bool isPickedUp = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void Start()
    {
        if (floatingPromptPrefab != null)
        {
            floatingPromptInstance = Instantiate(floatingPromptPrefab);
            floatingPromptInstance.SetActive(false);

            floatingPromptInstance.transform.position = transform.position + Vector3.up * promptOffset.y;
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
            floatingPromptInstance.transform.rotation = Quaternion.LookRotation(toPlayer * -1f);
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
        }
    }

    public void Interact()
    {
        if (!isPickedUp && holdPoint != null)
        {
            StopAllCoroutines();
            StartCoroutine(PickupSmooth(holdPoint));
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
    }

    private IEnumerator PickupSmooth(Transform target)
    {
        isPickedUp = true;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero; // Prevent movement after pickup
        rb.angularVelocity = Vector3.zero;
        col.enabled = false;

        float duration = 0.3f;
        float elapsed = 0f;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, target.position, elapsed / duration);
            transform.rotation = Quaternion.Slerp(startRot, target.rotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target.position;
        transform.rotation = target.rotation;

        transform.SetParent(target);
        ShowPrompt(false);
    }

    public void OnDrop()
    {
        isPickedUp = false;
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        col.enabled = true;
    }

    public void OnThrow(Vector3 force)
    {
        OnDrop();
        rb.AddForce(force, ForceMode.VelocityChange);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(promptOffset), 0.1f);
    }
}
