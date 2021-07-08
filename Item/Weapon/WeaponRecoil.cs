using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
public class WeaponRecoil : MonoBehaviour
{
    [HideInInspector] public PlayerCamera playerCamera;
    [HideInInspector] public Animator rigAnim;

    [SerializeField] AnimationClip recoilClip;
    float recoilDelay;
    public bool isRecoil = false;

    WeaponItem weaponItem;

    public float verticalRecoil;
    //public float duration;

    Cinemachine.CinemachineImpulseSource impulse;

    PhotonView pv;

    private void Awake()
    {
        weaponItem = GetComponent<WeaponItem>();
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        impulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
        recoilDelay = recoilClip.length;
    }
    public void GenerateRecoil()
    {
        if (pv.IsMine)
        {
            impulse.GenerateImpulse(playerCamera.followDiractionPoint.forward * verticalRecoil);
        }
 
        if (rigAnim)
        {
            rigAnim.Play(weaponItem.weaponName + "_recoil", 1, 0);
            StartCoroutine(Enum_RecoilDelay());
           
        }
    }
    IEnumerator Enum_RecoilDelay()
    {
        isRecoil = true;
        yield return new WaitForSeconds(recoilDelay);
        isRecoil = false;
    }
}
