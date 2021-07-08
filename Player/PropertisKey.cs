using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PropertisKey : MonoBehaviour
{
    public static PropertisKey instance;
    readonly public string[] holderWeapons = {"primaryWeapon","secondaryWeapon"};
    readonly public string activeWeaponIndex = "activeWeaponIndex";
    readonly public string killCount = "killCount";
    readonly public string deathCount = "deathCount";
    readonly public string viewID = "viewID";
    private void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        EventCenter.instance.AddEventListener("PlayerDie", ResetProperties);
    }
    void ResetProperties(object info)
    {
        PhotonNetwork.LocalPlayer.CustomProperties.Remove(viewID);
    }
    private void OnDestroy()
    {
        EventCenter.instance.RemoveEventListener("PlayerDie", ResetProperties);
    }
}
