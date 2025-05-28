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
            if (heldItem != null)
            {
                Drop(); // if holding something already
            }
            else if (currentInteractable != null)
            {
                if (currentInteractable is IHasViewSource viewSource)
                {
                    viewSource.SetViewSource(cameraTransform);
                }

                if (currentInteractable is ICollectible collectible)
                {
                    heldItem = collectible;

                    // Decide correct hold point (optional automation)
                    Transform point = holdPoint;

                    var interactableComponent = currentInteractable as Component;

                    if (interactableComponent != null)
                    {
                        if (interactableComponent.CompareTag("Flashlight"))
                        {
                            point = flashlightHoldPoint;
                        }
                        else if (interactableComponent.CompareTag("Mask"))
                        {
                            point = maskHoldPoint;
                        }
                         if (interactableComponent.CompareTag("Bin"))
                        {
                            point = binHoldPoint;
                        }
                    
                    }

                    collectible.OnPickup(point);
                }
                else
                {
                    currentInteractable.Interact();
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
                {
                    currentInteractable.ShowPrompt(false);
                }

                currentInteractable = interactable;
                currentInteractable.ShowPrompt(true);
            }

            if (interactable is IHasViewSource viewSource)
            {
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
    }

    void Throw()
    {
        Vector3 force = cameraTransform.forward * throwForce + Vector3.up * 3f;
        heldItem?.OnThrow(force);
        heldItem = null;
    }
}