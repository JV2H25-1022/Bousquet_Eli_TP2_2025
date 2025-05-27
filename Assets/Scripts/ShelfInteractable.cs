using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ShelfInteractable : MonoBehaviour, IInteractable
{
    private bool isOpen = false;
    private Quaternion closedRotation;
    public Quaternion openRotation;
    public float openSpeed = 2f;

    public GameObject floatingPromptPrefab;
    private GameObject floatingPromptInstance;
    private Transform playerView;

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
            Vector3 toPlayer = (playerView.position - transform.position).normalized;
            Vector3 promptPosition = transform.position + toPlayer * 0.4f + Vector3.up * 1.2f;

            floatingPromptInstance.transform.position = promptPosition;
            floatingPromptInstance.transform.rotation = Quaternion.LookRotation(toPlayer);
        }
    }

    public void SetViewSource(Transform viewSource)
    {
        playerView = viewSource;
    }

    public void ShowPrompt(bool show)
    {
        if (floatingPromptInstance != null)
            floatingPromptInstance.SetActive(show);
    }

    public void Interact()
    {
        isOpen = !isOpen;
        StopAllCoroutines();
        StartCoroutine(RotateShelf(isOpen ? openRotation : closedRotation));
    }

    private IEnumerator RotateShelf(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}

