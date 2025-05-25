using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;

    public GameObject normalZone;
    public GameObject crouchZone;

    private void Awake()
    {
        Instance = this;
    }

    public void SetCrouchView(bool isCrouching)
    {
        normalZone.SetActive(!isCrouching);
        crouchZone.SetActive(isCrouching);
    }
}

