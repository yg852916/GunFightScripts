using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class PlayerSkill : MonoBehaviour
{
    public static PlayerSkill instance;


    Animator anim;
    [SerializeField] Animator rigAnim;
    PhotonView pv;


    //need initialization
    Rigidbody playerRb;
    public Transform emissionPoint;


    public Transform skillParent;
    //only one skill
    GameObject skill;


    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        if (pv.IsMine)
        {
            EventCenter.instance.activateSkill.AddListener(ActivateSkills);
        }
    }
    void ActivateSkills(string name, GameObject button, bool isActivate)
    {
        if (name == "hook")
        {
            if (isActivate)
            {
                HookSkillInitialize(name, button);
            }
            else
            {
                DestroySkill();
            }
        }
    }
    void DestroySkill()
    {
        PhotonNetwork.Destroy(skill);
    }
    void HookSkillInitialize(string name, GameObject button)
    {
        skill = PhotonNetwork.Instantiate(System.IO.Path.Combine("Skills", "hookSkillPrefab"), Vector3.zero, Quaternion.identity);
        //GameObject skill = Instantiate(hookSkillPrefab, Vector3.zero, Quaternion.identity);
        skill.transform.SetParent(skillParent);
        HookSkill hookSkill = skill.GetComponent<HookSkill>();
        hookSkill.emissionPoint = emissionPoint;
        hookSkill.playerController = GetComponent<PlayerController>();
        hookSkill.button = button.GetComponent<Button>();
        hookSkill.secondText = button.GetComponentInChildren<TMP_Text>();
        hookSkill.anim = anim;
        hookSkill.rigAnim = rigAnim;
        pv.RPC("RPC_HookSkillInitialize", RpcTarget.Others, skill.GetComponent<PhotonView>().ViewID);
    }
    [PunRPC]
    void RPC_HookSkillInitialize(int pvid)
    {
        skill = PhotonView.Find(pvid).gameObject;
        skill.transform.SetParent(skillParent);
        HookSkill hookSkill = skill.GetComponent<HookSkill>();
        hookSkill.emissionPoint = emissionPoint;
    }
    
}
