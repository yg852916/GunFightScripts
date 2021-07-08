using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class HitBox : MonoBehaviour
{
    public int index;

    [HideInInspector] public PhotonView pv;
    private void Awake()
    {
        pv = GetComponent<PhotonTransformView>().photonView;
    }
    public void TakeDamage(float value, Player enemy)
    {
        pv.RPC("RPC_TakeDamage", pv.Controller, value, enemy);
    }
    public void ImpulseBody(Vector3 dir, Vector3 point)
    {
        pv.RPC("RPC_ImpulseBody", pv.Controller, dir, point, index);
    }

}
