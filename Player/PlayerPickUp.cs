using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerPickUp : MonoBehaviour
{

    PhotonView pv;

    [SerializeField] float radius = 3;
    [SerializeField] float distance = 4;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    void Start()
    {
        //Photon Process
        if (!pv.IsMine)
        {
            enabled = false;
            return;
        } 
        //End PhotonProcess
        
    }

    void Update()
    {
        CheckPickUp();
    }

    void CheckPickUp()
    {
        

        //if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance, LayerManager.instance.pickUpLayer))
        if(Physics.SphereCast(Camera.main.transform.position, radius, Camera.main.transform.forward, out RaycastHit hit, distance, LayerManager.instance.pickUpLayer))
        {
            if (hit.transform.GetComponent<Outline>())
            {
                hit.transform.GetComponent<Outline>().DisplayOutline();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (hit.transform.gameObject.tag == "Weapon")
                {
                    PlayerWeapon playerWeapon = gameObject.GetComponent<PlayerWeapon>();
                    playerWeapon.PickUp(hit.transform.gameObject.GetComponent<PhotonView>().ViewID);
                }
                else if (hit.transform.gameObject.tag == "Item")
                {
                    Item item = hit.transform.gameObject.GetComponent<Item>();
                    if (item.type == Item.ItemType.skill)
                    {
                        EventCenter.instance.EventTrigger("PickUpSkill", item);
                    }
                    else
                    {
                        EventCenter.instance.EventTrigger("PickUpItem", item);
                    }
                }
            }
        }
    }
    [PunRPC]
    void RPC_DestroyObject(int id)
    {
        PhotonNetwork.Destroy(PhotonView.Find(id).gameObject);
    }

}
