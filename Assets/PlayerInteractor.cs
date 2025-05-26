using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public float interactRange = 3f;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask interactLayer;
    public Transform cameraTransform;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
}

