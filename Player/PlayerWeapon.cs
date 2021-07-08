using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using Cinemachine;

public class PlayerWeapon : MonoBehaviourPunCallbacks
{
    //Animation Event
    public WeaponAnimationEvent weaponAnimationEvent;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip pickUpClip;

    PhotonView pv;


    public Animator rigAnim;
    public Transform[] weaponSlotParent;
    public WeaponItem[] weaponItem = new WeaponItem[2];

    //I need to record the player property
    [SerializeField] int activeWeaponIndex = -1;

    //Drop
    public float dropWeaponForce;
    public float dropWeaponTorque;

    //View
    public Transform crossHairTarget;


    PlayerController playerController;
    //hold gun state
    public bool isHolster = true;
    public bool isChangingWeapon = false;
    public bool isFiring = false;
    public bool isReloading = false;
    bool canClick = true;

    //view zoom in
    float originalFieldOfView;
    float gunFieldOfView;
    float gunZoomSpeed;
    public bool isZoomIn = false;
    [SerializeField] CinemachineVirtualCamera cvc;


    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    void Start()
    {

        weaponAnimationEvent.weaponEvent.AddListener(OnWeaponEvent);
        audioSource = GetComponent<AudioSource>();

        //initialization weapon 
        //WeaponItem weaponItem = GetComponentInChildren<WeaponItem>();
        //if (weaponItem) PickUp(weaponItem.gameObject.GetComponent<PhotonView>().ViewID);
        if (pv.IsMine)
        {
            playerController = GetComponent<PlayerController>();
            //EventCenter.instance.holsterWeapon.AddListener(HolsterWeapon);
            EventCenter.instance.cancelHandAction.AddListener(CancelHandAction);
            //EventCenter.instance.equipWeapon.AddListener(EquipWeapon);
            //EventCenter.instance.AddEventListener("PlayerDie", AllDrop);
            EventCenter.instance.itemAllDrop.AddListener(AllDrop);
            EventCenter.instance.openMenu.AddListener(ClickControl);
            originalFieldOfView = cvc.m_Lens.FieldOfView;
        }
        InitProperties();
    }
    void ClickControl(bool value)
    {
        canClick = value;
    }
    void OnWeaponEvent(string eventName)
    {
        if (weaponItem[activeWeaponIndex] == null) return;
        if (eventName == "DetachMagazine")
        {
            weaponItem[activeWeaponIndex].DetachMagazine();
        }
        else if (eventName == "AttachMagazine")
        {
            weaponItem[activeWeaponIndex].AttachMagazine();
        }
        else if (eventName == "RecoilAction")
        {
            weaponItem[activeWeaponIndex].RecoilAction();
        }
    }
    void Update()
    {
        //Photon Process
        if (!pv.IsMine) return;
        if (activeWeaponIndex == -1) return;
        //weapon process
        isFiring = false;
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isChangingWeapon)
        {
            //if isHolster or different weaponType can equip
            if (isHolster)
            {
                EquipWeapon((int)WeaponItem.WeaponType.primary);
            }
            else if (activeWeaponIndex != (int)WeaponItem.WeaponType.primary)
            {
                EquipWeapon((int)WeaponItem.WeaponType.primary);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && !isChangingWeapon)
        {
            if (isHolster)
            {
                EquipWeapon((int)WeaponItem.WeaponType.secondary);
            }
            else if (activeWeaponIndex != (int)WeaponItem.WeaponType.secondary)
            {
                EquipWeapon((int)WeaponItem.WeaponType.secondary);
            }
        }
        //view zoom in control
        if (isZoomIn && !canZoomIn())
        {
            ZoomIn(false);
        }
        else if (Input.GetMouseButtonDown(1) && canClick)
        {
            if (isZoomIn)
            {
                ZoomIn(false);
            }
            else if(canZoomIn())
            {
                ZoomIn(true);
            }
        }
        if (weaponItem[activeWeaponIndex])
        {
            //----------------weaponItem is not null
            if (!isChangingWeapon)
            {
                if (!isHolster && !isReloading)
                {
                    if (weaponItem[activeWeaponIndex].allowHoldShoot)
                    {
                        if (Input.GetMouseButton(0) && canClick)
                        {
                            isFiring = true;
                            if (!playerController.isSprinting)
                            {
                                weaponItem[activeWeaponIndex].StartFire(crossHairTarget.position);
                                EventCenter.instance.EventTrigger("UpdateWeaponDisplay", this);
                            }
                        }
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0) && canClick)
                        {
                            isFiring = true;
                            if (!playerController.isSprinting)
                            {
                                weaponItem[activeWeaponIndex].StartFire(crossHairTarget.position);
                                EventCenter.instance.EventTrigger("UpdateWeaponDisplay", this);
                            }
                        }
                    }

                    if ((weaponItem[activeWeaponIndex].ammoCount <= 0 || Input.GetKeyDown(KeyCode.R)) && weaponItem[activeWeaponIndex].CanReload() && !isHolster && !weaponItem[activeWeaponIndex].recoil.isRecoil)
                    {
                        StartCoroutine(IEnum_Reload());
                    }
                }

                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    HolsterWeapon();
                }
            }
            if (!isChangingWeapon && !isHolster)
            {
                //-----------------weaponItem is null
                if (Input.GetKeyDown(KeyCode.G))
                {
                    //pv.RPC("RPC_Drop", RpcTarget.All, crossHairTarget.position);
                    Drop(activeWeaponIndex, crossHairTarget.position);
                }
            }
        }
    }
    bool canZoomIn()
    {
        if (!weaponItem[activeWeaponIndex] || isHolster || isReloading || isChangingWeapon)
            return false;
        return true;
    }
    void ZoomIn(bool value)
    {
        StopCoroutine("IEnum_ZoomIn");
        StartCoroutine(IEnum_ZoomIn(value));
        
    }
    IEnumerator IEnum_ZoomIn(bool value)
    {
       
        if (value)
        {
            do
            {
                cvc.m_Lens.FieldOfView -= gunZoomSpeed;
                yield return new WaitForEndOfFrame();
            } while (cvc.m_Lens.FieldOfView > gunFieldOfView);
            cvc.m_Lens.FieldOfView = gunFieldOfView;
        }
        else
        {
            do
            {
                cvc.m_Lens.FieldOfView += gunZoomSpeed;
                yield return new WaitForEndOfFrame();
            } while (cvc.m_Lens.FieldOfView < originalFieldOfView);
            cvc.m_Lens.FieldOfView = originalFieldOfView;
        }
        isZoomIn = value;
    }

    void CancelHandAction(string eventName)
    {
        if (eventName == "All")
        {
            //StopAllCoroutines();
            //SetupHolsterState();
            StopAllCoroutines();
            SetupHolsterState();

            //HolsterWeapon();
        }
    }
    public void HolsterWeapon()
    {
        if (activeWeaponIndex == -1) return;
        if (weaponItem[activeWeaponIndex])
        {
            SetupHolsterState();
            StopAllCoroutines();
            StartCoroutine("IEnum_HolsterWeapon");
        }
    }
    public void EquipWeapon(int index)
    {
        if (weaponItem[index])
        {
            EventCenter.instance.cancelHandAction.Invoke("All");
            rigAnim.SetBool("isSprinting", false);
            activeWeaponIndex = index;
            PhotonNetwork.LocalPlayer.CustomProperties[PropertisKey.instance.activeWeaponIndex] = index;
            StopAllCoroutines();
            StartCoroutine("IEnum_EquipWeapon");
            EventCenter.instance.EventTrigger("PickUpWeapon", this);
            //UpdateDisplayWeaponInfo();
            EventCenter.instance.EventTrigger("UpdateWeaponDisplay", this);
            //EventCenter.instance.equipWeapon.Invoke();
 
        }
    }
    IEnumerator IEnum_Reload()
    {
        isReloading = true;
        rigAnim.SetBool("isReloading", true);
        yield return new WaitForSeconds(weaponItem[activeWeaponIndex].reloadDuration);
        weaponItem[activeWeaponIndex].Reload();
        rigAnim.SetBool("isReloading", false);
        isReloading = false;
        //UpdateDisplayWeaponInfo();
        EventCenter.instance.EventTrigger("UpdateWeaponDisplay", this);
    }
    IEnumerator IEnum_HolsterWeapon()
    {
        isChangingWeapon = true;
        //wait sprint end
        do
        {
            yield return new WaitForEndOfFrame();
        } while (!rigAnim.GetCurrentAnimatorStateInfo(2).IsName("NotSprint"));
        
        rigAnim.SetBool("isHolster", true);

        yield return new WaitForSeconds(weaponItem[activeWeaponIndex].holsterDuration);

        isHolster = true;
        isChangingWeapon = false;
    }
    IEnumerator IEnum_EquipWeapon()
    {
        gunFieldOfView = weaponItem[activeWeaponIndex].fieldOfView;
        gunZoomSpeed = weaponItem[activeWeaponIndex].zoomSpeed;
        isChangingWeapon = true;
        rigAnim.SetBool("isHolster", false);
        rigAnim.SetInteger("weaponNumber", weaponItem[activeWeaponIndex].weaponNumber);
        rigAnim.Play(weaponItem[activeWeaponIndex].weaponName + "_equip", 0, 0);
        yield return new WaitForSeconds(weaponItem[activeWeaponIndex].holsterDuration);

        isHolster = false;
        isChangingWeapon = false;
    }

    public WeaponItem GetActiveWeapon()
    {
        return weaponItem[activeWeaponIndex];
    }
    void InitProperties()
    {
        Hashtable hash = pv.Owner.CustomProperties;
        if (hash.ContainsKey(PropertisKey.instance.holderWeapons[0]))
        {
            if ((int)hash[PropertisKey.instance.holderWeapons[0]] != -1)
            {
                //self do
                RPC_PickUp((int)hash[PropertisKey.instance.holderWeapons[0]]);
            }
        }
        if (hash.ContainsKey(PropertisKey.instance.holderWeapons[1]))
        {
            if ((int)hash[PropertisKey.instance.holderWeapons[1]] != -1)
            {
                //self do
                RPC_PickUp((int)hash[PropertisKey.instance.holderWeapons[1]]);
            }
        }
        if (hash.ContainsKey(PropertisKey.instance.activeWeaponIndex))
        {
            if ((int)hash[PropertisKey.instance.activeWeaponIndex] != -1)
            {
                activeWeaponIndex = (int)hash[PropertisKey.instance.activeWeaponIndex];
            }
        }
    }
    public void PickUp(int _id)
    {
        pv.RPC("RPC_PickUp", RpcTarget.All, _id);
    }
    [PunRPC]
    void RPC_PickUp(int _id)
    {
        audioSource.PlayOneShot(pickUpClip);

        //no hold gun no pick
        //if (isHolster) return;
        if (_id == -1) return;
        //new weapon
        if (!PhotonView.Find(_id)) return;
        WeaponItem newWeaponItem = PhotonView.Find(_id).gameObject.GetComponent<WeaponItem>();
        //drop gun
        if (weaponItem[(int)newWeaponItem.weaponType] != null)
        {
            RPC_Drop((int)newWeaponItem.weaponType, crossHairTarget.position);
        }

        activeWeaponIndex = (int)newWeaponItem.weaponType;


        weaponItem[activeWeaponIndex] = newWeaponItem;
        weaponItem[activeWeaponIndex].holderPlayer = pv.Controller;
        weaponItem[activeWeaponIndex].pv.TransferOwnership(pv.Owner);
        weaponItem[activeWeaponIndex].SetHand();
        weaponItem[activeWeaponIndex].transform.SetParent(weaponSlotParent[activeWeaponIndex]);
        weaponItem[activeWeaponIndex].transform.localPosition = Vector3.zero;
        weaponItem[activeWeaponIndex].transform.localRotation = Quaternion.identity;
        weaponItem[activeWeaponIndex].recoil.rigAnim = rigAnim;
        if (pv.IsMine)
        {
            weaponItem[activeWeaponIndex].recoil.playerCamera = GetComponent<PlayerCamera>();
            EquipWeapon(activeWeaponIndex);
        }
        

        //set up player properties
        if (pv.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add(PropertisKey.instance.holderWeapons[activeWeaponIndex], weaponItem[activeWeaponIndex].pv.ViewID);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            //UpdateDisplayWeaponInfo();
            EventCenter.instance.EventTrigger("UpdateWeaponDisplay", this);
        }
    }

    void AllDrop()
    {
        pv.RPC("RPC_Drop", RpcTarget.All, 0, transform.forward);
        pv.RPC("RPC_Drop", RpcTarget.All, 1, transform.forward);
    }
    public void Drop(int slotIndex,Vector3 targetPos)
    {
        pv.RPC("RPC_Drop", RpcTarget.All, slotIndex, targetPos);
    }
    [PunRPC]
    void RPC_Drop(int slotIndex, Vector3 targetPos)
    {
        ResetAllState();
        if (!weaponItem[slotIndex]) return;
        Vector3 dir = (targetPos - weaponItem[slotIndex].firePoint.position).normalized;
        if (pv.IsMine)
        {
            StopAllCoroutines();
        }

        //weapon set ground
        weaponItem[slotIndex].SetGround();
        weaponItem[slotIndex].transform.SetParent(null);

        //throw
        if (pv.IsMine)
        {
            weaponItem[slotIndex].rb.AddForce((dir + Vector3.up / 5) * dropWeaponForce, ForceMode.Impulse);
            float f = Random.Range(-90, 90);
            weaponItem[slotIndex].rb.AddTorque(new Vector3(f, f, f) * dropWeaponTorque);
            weaponItem[slotIndex].SetCustomProperties();

            //set player properties
            Hashtable hash = new Hashtable();
            hash.Add(PropertisKey.instance.holderWeapons[slotIndex], -1);;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            EventCenter.instance.EventTrigger("DropWeapon", this);
        }

        //pointer to null
        weaponItem[slotIndex] = null;

    }
    void SetupHolsterState()
    {
        isFiring = false;
        isChangingWeapon = false;
        isHolster = true;
        isReloading = false;

        rigAnim.SetBool("isHolster", isHolster);
        rigAnim.SetBool("isReloading", isReloading);
    }
    void ResetAllState()
    {
        isFiring = false;
        isChangingWeapon = false;
        isHolster = false;
        isReloading = false;
        rigAnim.SetInteger("weaponNumber", -1);
        rigAnim.SetBool("isHolster", isHolster);
        rigAnim.SetBool("isReloading", isReloading);
    }
    private void OnDestroy()
    {
        if (pv.IsMine)
        {
            EventCenter.instance.cancelHandAction.RemoveListener(CancelHandAction);
            //EventCenter.instance.holsterWeapon.RemoveListener(HolsterWeapon);
            EventCenter.instance.openMenu.RemoveListener(ClickControl);
            EventCenter.instance.itemAllDrop.RemoveListener(AllDrop);
            weaponAnimationEvent.weaponEvent.RemoveListener(OnWeaponEvent);
            
        }
    }
}
