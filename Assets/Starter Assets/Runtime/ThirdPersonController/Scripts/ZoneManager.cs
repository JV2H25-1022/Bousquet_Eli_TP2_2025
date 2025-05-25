using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using StarterAssets;


namespace StarterAssets
{
    public class ZoneManager : MonoBehaviour
    {
        [SerializeField] private GameObject normalZone;
        [SerializeField] private GameObject crouchZone;

        public void SetCrouchState(bool isCrouching)
        {
            if (normalZone) normalZone.SetActive(!isCrouching);
            if (crouchZone) crouchZone.SetActive(isCrouching);
        }
    }
}

