using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Realtime;
using Photon.Pun;
using System;
//Event args

public class InventoryManager : MonoBehaviour
{
    PhotonView pv;

    bool isDisplay = false;

    public List<InventorySlot> container = new List<InventorySlot>();

    public static InventoryManager instance;

    public Transform InventoryContent;

    AudioSource audioSource;
    [SerializeField] AudioClip pickUpClip;
    //record throwimg weapon order
    List<Item.ItemType> throwingWeapons = new List<Item.ItemType>();
    int throwingWeaponIndex = 0;
    private void Awake()
    {
        instance = this;
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        EventCenter.instance.AddEventListener("PickUpItem", PickUpItem);
        EventCenter.instance.removeItem.AddListener(RemoveItem);
        //EventCenter.instance.AddEventListener("PlayerDie", AllDrop);
        EventCenter.instance.itemAllDrop.AddListener(AllDrop);
        audioSource = GetComponent<AudioSource>();
    }
    void PickUpItem(object info)
    {

        Item item = (info as Item);
        AddItem(item, item.amount);
        item.pv.RPC("DestroySelf", RpcTarget.MasterClient);
    }
    private void Update()
    {
        DisplayInventory();
    }
    [PunRPC]
    void RPC_CreateRoomObject(string path,Vector3 pos,Quaternion rot,Vector3 forceVelocity)
    {
        GameObject itemObject = PhotonNetwork.InstantiateRoomObject(path, pos, rot);
        itemObject.GetComponent<Rigidbody>().AddForce(forceVelocity, ForceMode.Impulse);
    }
    public void GetThrowingWeapon(bool isSwitching)
    {
        if (throwingWeapons.Count <= 0) return;
        if (isSwitching)
        {
            if (throwingWeapons.Count > 1)
                throwingWeaponIndex = (throwingWeaponIndex + 1) % throwingWeapons.Count;
            else
                return;
        }
        else
        {
            throwingWeaponIndex = 0;
        }
        for (int i = 0; i < container.Count; i++)
        {
            if (container[i].item.type == throwingWeapons[throwingWeaponIndex])
            {
                container[i].buttonObj.GetComponent<ButtonEvent>().useEvent.Invoke();
                break;
            }
        }
    }
    void DisplayInventory()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isDisplay == true)
            {
                EventCenter.instance.openMenu.Invoke(true);
                isDisplay = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                EventCenter.instance.openMenu.Invoke(false);
                isDisplay = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            InventoryContent.gameObject.SetActive(isDisplay);
        }
    }
    //every type click function
    void OnItemSelected(Item item)
    {
        if (item.type == Item.ItemType.healthPotion)
        {
            //EventCenter.instance.holsterWeapon.Invoke();
            EventCenter.instance.potionEvents.Invoke((item as PotionHealthItem).restoreHealthValue);
        }
        else if (item.type == Item.ItemType.grenade)
        {
            //EventCenter.instance.holsterWeapon.Invoke();
            EventCenter.instance.equipThrowingWeapon.Invoke((int)item.type);
        }
    }
    void OnItemDropped(Item item)
    {
        //if (!PlayerController.instance) return;
        if(!audioSource.isPlaying)
            audioSource.PlayOneShot(pickUpClip);
        if (item.type == Item.ItemType.healthPotion)
        {
            pv.RPC("RPC_CreateRoomObject", RpcTarget.MasterClient, "PotionHealth", PlayerController.instance.transform.position + Vector3.up, Quaternion.identity, PlayerController.instance.transform.forward * 5);
            RemoveItem((int)item.type, 1);
            EventCenter.instance.cancelHandAction.Invoke("PlayerPotionSystem");
        }
        else if (item.type == Item.ItemType.grenade)
        {
            pv.RPC("RPC_CreateRoomObject", RpcTarget.MasterClient, "RGD-5", PlayerController.instance.transform.position + Vector3.up, Quaternion.identity, PlayerController.instance.transform.forward * 5);
            RemoveItem((int)item.type, 1);
            EventCenter.instance.cancelHandAction.Invoke("grenade");
        }
    }
    void AllDrop()
    {
        while (container.Count > 0)
        {
            OnItemDropped(container[0].item);
        }
    }
    void AllDrop(object useless)
    {
        while (container.Count > 0)
        {
            OnItemDropped(container[0].item);
        }
    }
    public void AddItem(Item _item, int _amount)
    {
        audioSource.PlayOneShot(pickUpClip);
        bool hasItem = false;
        for (int i = 0; i < container.Count; i++)
        {
            if (container[i].item.type == _item.type)
            {
                hasItem = true;
                container[i].AddAmount(_amount);
                UpdateItemDisplay(container[i]);
                break;
            }
        }
        if (!hasItem)
        {
            if (_item.type == Item.ItemType.grenade)
            {
                throwingWeapons.Add(_item.type);
            }
            InventorySlot newInventorySlot = new InventorySlot(_item, _amount);
            newInventorySlot.buttonObj = Instantiate(_item.prefab, InventoryContent);
            //newInventorySlot.buttonObj.GetComponent<Button>().onClick.AddListener(() => OnItemSelected(_item));
            ButtonEvent buttonEvent = newInventorySlot.buttonObj.GetComponent<ButtonEvent>();
            buttonEvent.useEvent.AddListener(() => OnItemSelected(_item));
            buttonEvent.dropEvent.AddListener(() => OnItemDropped(_item));
            container.Add(newInventorySlot);
            UpdateItemDisplay(newInventorySlot);
        }   
    }
    public void RemoveItem(int type, int _amount)
    {
        for (int i = 0; i < container.Count; i++)
        {
            if ((int)container[i].item.type == type)
            {
                container[i].amount -= _amount;
                UpdateItemDisplay(container[i]);
                break;
            }
        }
    }
    void UpdateItemDisplay(InventorySlot slot)
    {
        if (slot.amount <= 0)
        {
            Destroy(slot.buttonObj);
            container.Remove(slot);
            if (slot.item.type == Item.ItemType.grenade)
            {
                throwingWeapons.Remove(slot.item.type);
            }
        }
        slot.buttonObj.GetComponentInChildren<TMPro.TMP_Text>().text = slot.amount.ToString();
    }
    private void OnDestroy()
    {
        EventCenter.instance.RemoveEventListener("PickUpItem", PickUpItem);
        EventCenter.instance.removeItem.RemoveListener(RemoveItem);
        EventCenter.instance.itemAllDrop.RemoveListener(AllDrop);
    }
}
public class InventorySlot
{
    public Item item;
    public int amount;
    public GameObject buttonObj;
    public InventorySlot(Item _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}