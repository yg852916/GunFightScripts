using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class DoorControl : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip openDoorClip;
    [SerializeField] AudioClip closeDoorClip;
    Animator anim;
    PhotonView pv;
    public int openWay = 0;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        pv = GetComponent<PhotonView>();
    }
    public void OpenDoor(int value)
    {
        pv.RPC("RPC_OpenDoor", RpcTarget.All, value);
    }
    [PunRPC]
    void RPC_OpenDoor(int value)
    {
        audioSource.PlayOneShot(openDoorClip);
        anim.SetInteger("openWay", value);
        openWay = value;
    }
    public void CloseDoor()
    {
        pv.RPC("RPC_CloseDoor", RpcTarget.All);
    }
    [PunRPC]
    void RPC_CloseDoor()
    {
        audioSource.PlayOneShot(closeDoorClip);
        anim.SetInteger("openWay",0);
        openWay = 0;
    }
}
