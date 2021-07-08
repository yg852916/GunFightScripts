using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerThrowSystem : MonoBehaviour
{
    public Animator rigAnim;

    public Transform throwWeaponSlot;

    public GameObject grenadePrefab;

    GameObject throwingWeapon;
    WeaponAnimationEvent weaponAnimationEvent;

    public float throwForce = 25;
    public bool isEquipThrowing;
    int currentType;

    bool canClick = true;

    LineRenderer linePath;

    PhotonView pv;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    void Start()
    {
        linePath = GetComponent<LineRenderer>();
        linePath.positionCount = 10;
        linePath.enabled = false;
        weaponAnimationEvent = rigAnim.GetComponent<WeaponAnimationEvent>();
        weaponAnimationEvent.throwWeaponEvent.AddListener(StartThrow);
        if (pv.IsMine)
        {
            EventCenter.instance.equipThrowingWeapon.AddListener(EquipThrowingWeapon);
            EventCenter.instance.cancelHandAction.AddListener(CancelHandActionEvent);
            EventCenter.instance.openMenu.AddListener(ClickControl);
        }
        else
        {
            enabled = false;
        }

    }
    void ClickControl(bool value)
    {
        canClick = value;
    }
    private void Update()
    {
        if (isEquipThrowing)
        {
            DrawPath();
            if (Input.GetMouseButtonDown(0) && canClick)
            {
                linePath.enabled = false;
                pv.RPC("RPC_StartThrow", RpcTarget.All);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                pv.RPC("RPC_CancelEquip", RpcTarget.All);
            }
        }
        //equip throwing weapon
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //the parameter is whether to switch other throwing weapon.
            InventoryManager.instance.GetThrowingWeapon(isEquipThrowing);
        }
    }
    void DrawPath()
    {
        linePath.enabled = true;
        Vector3 startPos = throwWeaponSlot.position + Vector3.up * 0.3f;
        Vector3 velocity = Camera.main.transform.forward * throwForce;
        for (int i = 0; i < 10; i++)
        {
            float time = i * 0.05f;
            Vector3 nextPos = startPos + (0.5f * Physics.gravity * time * time + velocity * time);
            linePath.SetPosition(i, nextPos);
        }
    }

    void EquipThrowingWeapon(int type)
    {
        EventCenter.instance.cancelHandAction.Invoke("All");
        pv.RPC("RPC_EquipThrowingWeapon", RpcTarget.All, type);
    }
    [PunRPC]
    void RPC_EquipThrowingWeapon(int type)
    {
        if (isEquipThrowing)
        {
            if (currentType == type) return;
            Destroy(throwingWeapon);
            rigAnim.Play("EquipThrowWeapon", 0, 0);
        }
        if (type == (int)Item.ItemType.grenade)
        {

            isEquipThrowing = true;
            rigAnim.SetBool("isEquipThrowing", true);
            throwingWeapon = Instantiate(grenadePrefab, throwWeaponSlot);
            throwingWeapon.transform.localPosition = Vector3.zero;
            throwingWeapon.transform.localRotation = Quaternion.identity;
        }
        currentType = type;
    }
    [PunRPC]
    void RPC_StartThrow() 
    {
        rigAnim.SetTrigger("throwing");
        isEquipThrowing = false;
        rigAnim.SetBool("isEquipThrowing", false);
    }
    void CancelHandActionEvent(string actionName)
    {
        if (actionName == "All")
        {
            pv.RPC("RPC_CancelEquip", RpcTarget.All);
        }
        else if (actionName == "grenade")
        {
            pv.RPC("RPC_CancelEquip", RpcTarget.All);
        }
    }
    [PunRPC]
    void RPC_CancelEquip()
    {
        if (!isEquipThrowing) return;
        isEquipThrowing = false;
        rigAnim.SetBool("isEquipThrowing", false);
        linePath.enabled = false;
        Destroy(throwingWeapon);
    }
    void StartThrow(string eventName)//Animation event,grenade leave hand.
    {
        if (eventName == "throw")
        {
            Destroy(throwingWeapon);
            if (pv.IsMine)
            {
                throwingWeapon = PhotonNetwork.Instantiate(System.IO.Path.Combine("ThrowWeapon/", "RGD-5"), throwWeaponSlot.position, throwWeaponSlot.rotation);
                throwingWeapon.GetComponent<ThrowWeapon>().holderPlayer = pv.Controller;
                throwingWeapon.GetComponent<Rigidbody>().velocity = Camera.main.transform.forward * throwForce;
                InventoryManager.instance.RemoveItem((int)Item.ItemType.grenade, 1);
            }
        }
    }
    private void OnDestroy()
    {
        if (pv.IsMine)
        {
            EventCenter.instance.equipThrowingWeapon.RemoveListener(EquipThrowingWeapon);
            EventCenter.instance.cancelHandAction.RemoveListener(CancelHandActionEvent);
            weaponAnimationEvent.throwWeaponEvent.RemoveListener(StartThrow);
        }
    }
}
