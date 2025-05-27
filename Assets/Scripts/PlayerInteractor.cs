using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public float interactRange = 3f;
    public KeyCode interactKey = KeyCode.E;
    public LayerMask interactLayer;
    public Transform cameraTransform;

    private IInteractable currentTarget;

    void Update()
    {
        ShowPromptIfNeeded();

        if (Input.GetKeyDown(interactKey) && currentTarget != null)
        {
            currentTarget.Interact();
        }
    }

    void ShowPromptIfNeeded()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * interactRange, Color.red, 1f);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (currentTarget != null && currentTarget != interactable)
                {
                    currentTarget.ShowPrompt(false);
                }

                currentTarget = interactable;

                // Pass player camera to the door so it can aim the prompt
                if (currentTarget is DoorInteractable door)
                {
                    door.SetViewSource(cameraTransform);
                }

                currentTarget.ShowPrompt(true);
                return;
            }
        }

        if (currentTarget != null)
        {
            currentTarget.ShowPrompt(false);
            currentTarget = null;
        }


        
    }
}
