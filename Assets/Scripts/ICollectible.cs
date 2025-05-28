using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectible
{
    void OnPickup(Transform holdPoint);
    void OnDrop();
    void OnThrow(Vector3 force);
}


