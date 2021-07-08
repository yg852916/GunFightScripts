using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class ThrowWeapon : MonoBehaviour
{
    public Player holderPlayer;

    public float radiusEffect = 5;
 
    public float damage = 10;

    public ParticleSystem explosionEffect;

    AudioSource audioSource;

    public string throwWeaponName;

    bool isBoom = false;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {

        //StartCoroutine(IEnum_StartTiming());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isBoom) return;
        isBoom = true;
        GetComponent<Rigidbody>().isKinematic = true;
        if (throwWeaponName == "grenade")
        {
            explosionEffect.transform.eulerAngles = new Vector3(-90, 0, 0);
            explosionEffect.Play();
            audioSource.Play();
            GetComponent<MeshRenderer>().enabled = false;
            if (GetComponent<PhotonView>().IsMine)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, radiusEffect, LayerManager.instance.weaponImpactLayer);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].CompareTag("Player"))
                    {
                        colliders[i].GetComponent<HitBox>().TakeDamage(damage, holderPlayer);
                        EventCenter.instance.playerAttackDamage.Invoke(damage);
                    }
                }
            }
            Destroy(gameObject, 1);
        }
    }
}
