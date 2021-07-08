using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRebirthPoint : MonoBehaviour
{
    public static ItemRebirthPoint instance;

    [HideInInspector] public Transform[] allPoint;

    private void Awake()
    {
        instance = this;
        allPoint = GetComponentsInChildren<Transform>();
    }

}
