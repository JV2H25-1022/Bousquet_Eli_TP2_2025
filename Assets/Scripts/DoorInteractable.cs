using System.Collections;
using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    private bool isOpen = false;
    private Quaternion closedRotation;
    public Quaternion openRotation;
    public float openSpeed = 2f;

    public GameObject floatingPromptPrefab;

    private GameObject floatingPromptInstance;
    private Transform playerView;

    [SerializeField] private Vector3 promptLocalOffset = new Vector3(0, 0, 0); // Local position above center

    private void Start()
    {
        closedRotation = transform.rotation;

        if (floatingPromptPrefab != null)
        {
            floatingPromptInstance = Instantiate(floatingPromptPrefab);
            floatingPromptInstance.SetActive(false);
        }
    }

private void Update()
{
    if (floatingPromptInstance && floatingPromptInstance.activeSelf && playerView != null)
    {
        Vector3 doorPosition = transform.position;

        // Direction from door to player
        Vector3 toPlayer = (playerView.position - doorPosition).normalized;

            // Offset: in front of the door toward the player, plus a bit above the door

        Vector3 promptPosition = doorPosition + promptLocalOffset;
        floatingPromptInstance.transform.position = promptPosition;

        // Rotate prompt to always face the player
        floatingPromptInstance.transform.rotation = Quaternion.LookRotation(toPlayer*-1);
    }
}


    public void SetViewSource(Transform playerViewSource)
    {
        playerView = playerViewSource;
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(promptLocalOffset), 0.1f);
    }
}
