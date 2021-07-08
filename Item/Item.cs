using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Item : MonoBehaviour
{
    public enum ItemType
    {
        skill,
        healthPotion,
        energyPotion,
        grenade,
    }

    [HideInInspector] public PhotonView pv;
    public GameObject prefab;
    public ItemType type;
    public int amount = 1;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    [PunRPC]
    public void DestroySelf()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
