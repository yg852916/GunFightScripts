using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerPotionSystem : MonoBehaviour
{
    PhotonView pv;
    //hold potion
    public GameObject healthPotionPrefab;
    public Transform rightHand;
    GameObject rightHandHealthPotion;
    float restoreValue;
    //animator
    Animator anim;

    [HideInInspector] public bool isRestore;
    AudioSource audioSource;
    [SerializeField] public AudioClip healClip;

    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        pv = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        if (pv.IsMine)
        {
            EventCenter.instance.potionEvents.AddListener(StartHealthPotion);
            EventCenter.instance.cancelHandAction.AddListener(CancelHandActionEvent);
        }
        else
        {
            enabled = false;
        }
    }
    private void Update()
    {
        //cancel restore
        if (isRestore == true && Input.GetMouseButtonDown(0))
        {
            CancelRestore();
        }
    }
    void CancelHandActionEvent(string actionName)
    {
        if (actionName == "All")
        {
            CancelRestore();
        }
        else if(actionName == "Item")
        {
            CancelRestore();
        }
    }
    void CancelRestore()
    {
        EventCenter.instance.healthControl.Invoke("CancelPreviousHealth", restoreValue);
        pv.RPC("RPC_CancelRestore", RpcTarget.All);
    }
    [PunRPC]
    void RPC_CancelRestore()
    {
        if (!isRestore) return;
        Destroy(rightHandHealthPotion);
        anim.SetBool("isRestore", false);
        isRestore = false;
    }
    void StartHealthPotion(float value)
    {
        EventCenter.instance.cancelHandAction.Invoke("All");
        EventCenter.instance.healthControl.Invoke("PreviousHealth", value);
        pv.RPC("RPC_StartHealthPotion", RpcTarget.All, value);
    }
    [PunRPC]
    void RPC_StartHealthPotion(float value)
    {
        restoreValue = value;
        isRestore = true;
        anim.SetBool("isRestore", true);
        rightHandHealthPotion = Instantiate(healthPotionPrefab, rightHand);
    }

    void EndHealthPotion()//this doesn't need rpc,because this is animation event,all player will execute
    {
        audioSource.PlayOneShot(healClip);
        isRestore = false;
        anim.SetBool("isRestore", false);
        Destroy(rightHandHealthPotion);
        if (pv.IsMine)
        {
            InventoryManager.instance.RemoveItem((int)Item.ItemType.healthPotion, 1);
            EventCenter.instance.healthControl.Invoke("RestoreHealth", restoreValue);
            EventCenter.instance.healthControl.Invoke("CancelPreviousHealth", restoreValue);

        }
    }
    private void OnDestroy()
    {
        if (pv.IsMine)
        {
            EventCenter.instance.potionEvents.RemoveListener(StartHealthPotion);
            EventCenter.instance.cancelHandAction.RemoveListener(CancelHandActionEvent);
        }
    }
}
