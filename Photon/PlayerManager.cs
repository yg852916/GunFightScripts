using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerManager : MonoBehaviour
{
    PhotonView pv;
    GameObject player;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    void Start()
    {
        if (pv.IsMine)
        {
            CreatePlayer();
            EventCenter.instance.AddEventListener("PlayerDie", PlayerDie);
        }
    }
    void CreatePlayer()
    {
        player = PhotonNetwork.Instantiate("character", new Vector3(Random.Range(-21,27),20,Random.Range(-28,17)), Quaternion.identity,0,new object[] { pv.ViewID });
    }
    public void PlayerDie(object info)
    {
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<PlayerPickUp>().enabled = false;
        StartCoroutine(IEnum_Die());
    }

    IEnumerator IEnum_Die()
    {
        yield return new WaitForSeconds(5);
        PhotonNetwork.Destroy(player);
        CreatePlayer();
    }
    private void OnDestroy()
    {
        EventCenter.instance.RemoveEventListener("PlayerDie", PlayerDie);
    }
}
