using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerRagdoll : MonoBehaviour
{
    public GameObject hipsObj;
    Rigidbody[] rigidbodys;

    public float force = 1000;

    [HideInInspector] public PhotonView pv;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    void Start()
    {
        //get all ragdoll rigidbodys
        rigidbodys = hipsObj.GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < rigidbodys.Length; i++)
        {
            rigidbodys[i].GetComponent<HitBox>().index = i;
            if (pv.IsMine)
            {
                rigidbodys[i].gameObject.layer = LayerMask.NameToLayer("LocalPlayer");
            }
        }
        
        InactiveRagdoll();
    }
    void InactiveRagdoll()
    {
        for (int i = 0; i < rigidbodys.Length; i++)
        {
            rigidbodys[i].isKinematic = true;
            rigidbodys[i].gameObject.GetComponent<PhotonTransformView>().enabled = false;
            rigidbodys[i].gameObject.GetComponent<Collider>().isTrigger = true;
        }
    }
    public void ActiveRagdoll()
    {
        for (int i = 0; i < rigidbodys.Length; i++)
        {
            rigidbodys[i].isKinematic = false;
            rigidbodys[i].gameObject.GetComponent<PhotonTransformView>().enabled = true;
            //rigidbodys[i].gameObject.GetComponent<Collider>().enabled = true;
            rigidbodys[i].gameObject.GetComponent<Collider>().isTrigger = false;
            if (!pv.IsMine)
            {
                //synchronize transform only have one player rigidbody,ohterwise will shake
                Destroy(rigidbodys[i].GetComponent<CharacterJoint>());
                Destroy(rigidbodys[i]);
            }
        }
    }
    [PunRPC]
    public void RPC_ImpulseBody(Vector3 dir, Vector3 point, int index)
    {
        rigidbodys[index].AddForceAtPosition(dir * force, point);
    }
}
