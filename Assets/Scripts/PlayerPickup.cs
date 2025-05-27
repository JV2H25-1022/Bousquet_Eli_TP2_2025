using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float pickupRange = 3f;
    public Transform holdPoint; // empty child in front of player camera
    public LayerMask pickupLayer;

    public KeyCode pickupKey = KeyCode.E;
    public KeyCode throwKey = KeyCode.Mouse1;

    private ICollectible heldItem = null;

    void Update()
    {
        if (heldItem == null)
        {
            if (Input.GetKeyDown(pickupKey))
                TryPickup();
        }
        else
        {
            if (Input.GetKeyDown(pickupKey))
                Drop();
            else if (Input.GetKeyDown(throwKey))
                Throw();
        }
    }

    void TryPickup()
    {
        Vector3 rayOrigin = transform.position + Vector3.up/10; 
        Ray ray = new Ray(rayOrigin, transform.forward);

         Debug.DrawRay(rayOrigin, transform.forward, Color.green, 1f);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupLayer))
        {
            ICollectible collectible = hit.collider.GetComponent<ICollectible>();
            if (collectible != null)
            {
                heldItem = collectible;
                collectible.OnPickup(holdPoint);
            }
        }
    }

    void Drop()
    {
        if (heldItem != null)
        {
            heldItem.OnDrop();
            heldItem = null;
        }
    }

    void Throw()
    {
        if (heldItem != null)
        {
            Vector3 force = transform.forward * 8f + Vector3.up * 2f;
            heldItem.OnThrow(force);
            heldItem = null;
        }
    }
}

