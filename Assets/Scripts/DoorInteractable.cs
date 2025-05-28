using System.Collections;
using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable, IHasViewSource
{
    public bool isOpen = false;
    private Quaternion closedRotation;
    public Quaternion openRotation;
    public float openSpeed = 2f;

    public GameObject floatingPromptPrefab;

    private GameObject floatingPromptInstance;
    private Transform viewSource;

    [SerializeField] private Vector3 promptOffset = new Vector3(0.09f, 0, -1.09f); // Local position

    private void Start()
    {
        closedRotation = transform.rotation;
        if (floatingPromptPrefab != null)
        {
            floatingPromptInstance = Instantiate(floatingPromptPrefab);
            floatingPromptInstance.SetActive(false);

            // Immediately position the prompt near this object
            if (floatingPromptInstance && floatingPromptInstance.activeSelf && viewSource != null)
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
            Vector3 doorPosition = transform.position;

            // Direction from door to player
            Vector3 toPlayer = (viewSource.position - doorPosition).normalized;

            Vector3 promptPosition = doorPosition + promptOffset;
            floatingPromptInstance.transform.position = promptPosition;

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
        }
    }

    public void Interact()
    {
        isOpen = !isOpen;
        StopAllCoroutines();
        StartCoroutine(RotateDoor(isOpen ? openRotation : closedRotation));
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 toPlayer = (viewSource.position - transform.position).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, toPlayer).normalized;

        Vector3 offset =
            toPlayer * promptOffset.z +
            Vector3.up * promptOffset.y +
            right * promptOffset.x;

        floatingPromptInstance.transform.position = transform.position + offset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(promptOffset), 0.1f);
    }
}
