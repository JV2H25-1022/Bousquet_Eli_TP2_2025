using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectible
{
    void OnPickup(Transform holdParent);
    void OnDrop();
    void OnThrow(Vector3 force);
}

