using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;



    [HideInInspector] public PhotonView pv;
    public float maxHealth;
    [HideInInspector] public float currentHealth;

    Animator anim;

    //hold potion

    public Player enemyPlayer;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
        //is not mine enable the script
        if (pv.IsMine)
        {
            instance = this;
        }
        else
        {
            //Destroy(this);
            enabled = false;
        }
    }
    private void Start()
    {
        if (pv.IsMine)
        {
            currentHealth = maxHealth;
            EventCenter.instance.healthControl.AddListener(HealthControl);
            EventCenter.instance.healthControl.Invoke("ChangedHealth", currentHealth);
        }
    }
    [PunRPC]
    void RPC_TakeDamage(float value, Player enemy)
    {
        if (currentHealth <= 0) return;

        currentHealth -= value;
        //lifeValueText.text = currentHealth.ToString("00");
        if (currentHealth <= 0)
        {

            //only self have rigidbody
            //GetComponent<CharacterController>().enabled = false;
            pv.RPC("RPC_FallDown", RpcTarget.All);

            //playerManager.Die(enemyName);
            this.enemyPlayer = enemy;
            EventCenter.instance.EventTrigger("PlayerDie", this);

            EventCenter.instance.itemAllDrop.Invoke();

        }
        //EventCenter.instance.EventTrigger("ChangeHealth", this);
        EventCenter.instance.healthControl.Invoke("ChangedHealth", value);
    }

    [PunRPC]
    void RPC_FallDown()
    {
        anim.enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<PlayerRagdoll>().ActiveRagdoll();
    }
    void HealthControl(string eventName,float value)
    {
        if (eventName == "RestoreHealth")
        {
            currentHealth += value;
            EventCenter.instance.healthControl.Invoke("ChangedHealth", value);
        }
    }

    private void OnDestroy()
    {

        EventCenter.instance.healthControl.RemoveListener(HealthControl);
    }
}
