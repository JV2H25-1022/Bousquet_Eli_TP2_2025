using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform holdPoint;

    [SerializeField] private Transform flashlightHoldPoint;
    [SerializeField] private Transform maskHoldPoint;
    [SerializeField] private Transform binHoldPoint;

    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Pickup Settings")]
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private KeyCode throwKey = KeyCode.Mouse1;
    [SerializeField] private float throwForce = 8f;

    private IInteractable currentInteractable;
    private ICollectible heldItem;

    void Update()
    {
        HandleLookInteraction();

        if (Input.GetKeyDown(interactKey))
        {
            if (currentInteractable != null)
            {
                var asCollectible = currentInteractable as ICollectible;

                if (heldItem != null)
                {
                    // Drop current held item
                    Drop();

                    // If the target is *not* a collectible, still allow interacting with it
                    if (asCollectible == null)
                        currentInteractable.Interact();
                }
                else
                {
                    if (currentInteractable is IHasViewSource viewSource)
                        viewSource.SetViewSource(cameraTransform);

                    if (asCollectible != null)
                    {
                        heldItem = asCollectible;

                        Transform point = holdPoint;
                        var go = (currentInteractable as MonoBehaviour).gameObject;

                        if (go != null)
                        {
                            switch (go.tag)
                            {
                                case "Flashlight": point = flashlightHoldPoint; break;
                                case "Mask": point = maskHoldPoint; break;
                                case "Bin": point = binHoldPoint; break;
                            }

                            GameUIManager.Instance.ShowIconForTag(go.tag);
                        }

                        asCollectible.OnPickup(point);
                    }
                    else
                    {
                        currentInteractable.Interact();
                    }
                }
            }
        }

        if (Input.GetKeyDown(throwKey) && heldItem != null)
        {
            Throw();
        }
    }

    void HandleLookInteraction()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactRange, Color.cyan);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (currentInteractable != null && currentInteractable != interactable)
                    currentInteractable.ShowPrompt(false);

                currentInteractable = interactable;
                currentInteractable.ShowPrompt(true);

                if (interactable is IHasViewSource viewSource)
                    viewSource.SetViewSource(cameraTransform);
            }
        }
        else
        {
            if (currentInteractable != null)
            {
                currentInteractable.ShowPrompt(false);
                currentInteractable = null;
            }
        }
    }

    void Drop()
    {
        heldItem?.OnDrop();
        heldItem = null;
        GameUIManager.Instance.HideAll();
    }

    void Throw()
    {
        Vector3 force = cameraTransform.forward * throwForce + Vector3.up * 3f;
        heldItem?.OnThrow(force);
        heldItem = null;
        GameUIManager.Instance.HideAll();
    }
}
