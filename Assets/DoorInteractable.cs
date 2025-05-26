using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    private bool isOpen = false;
    private Quaternion closedRotation;
    public Quaternion openRotation;
    public float openSpeed = 2f;

    private void Start()
    {
        closedRotation = transform.rotation;
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
}
