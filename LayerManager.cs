using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public static LayerManager instance;

    public LayerMask weaponImpactLayer;
    public LayerMask playerBodyLayer;
    public LayerMask pickUpLayer;
    public LayerMask aimTargetLayer;
    public LayerMask playerRaycastLayer;
    public LayerMask hookSkillLayer;
    public LayerMask crouchDetectLayer;
    private void Awake()
    {
        instance = this;
    }
}
