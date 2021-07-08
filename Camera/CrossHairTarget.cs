using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class CrossHairTarget : MonoBehaviour
{
    public Camera thirdCamera;
    public Transform crosshairTarget;
    public float distance = 20;
    PhotonTransformView ptv;
    private void Awake()
    {
        ptv = GetComponent<PhotonTransformView>();
        if (!ptv.photonView.IsMine)
        {
            enabled = false;
        }
    }
    private void Update()
    {
        if (Physics.Raycast(thirdCamera.transform.position, thirdCamera.transform.forward, out RaycastHit hit, distance, LayerManager.instance.aimTargetLayer))
        {
            crosshairTarget.position = hit.point;
        }
        else
        {
            crosshairTarget.position = thirdCamera.transform.position + thirdCamera.transform.forward * distance * 2;
        }
    }
}
