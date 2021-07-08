using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class WeaponItem : MonoBehaviour
{
    //Photon
    [HideInInspector] public PhotonView pv;
    PhotonTransformView ptv;
    PhotonRigidbodyView prv;
    public Player holderPlayer;

    AudioSource audioSource;
    public AudioClip fireClip;
    public AudioClip detachMagazineClip;
    public AudioClip attachMagazineClip;
    public AudioClip recoilClip;


    public Sprite weaponImage; 


    public string weaponName;
    public enum WeaponType { 
        primary = 0,
        secondary = 1
    }
    public WeaponType weaponType;
    //weapon information
    public Transform firePoint;
    public float bulletDrop;
    public float bulletRate;
    public float bulletSpeed;
    public float bulletDamage;
    public float bulletMaxDistance;
    public float holsterDuration;
    public float reloadDuration;
    public float fieldOfView = 20;
    public float zoomSpeed = 1f;

    public int totalAmmoCount;
    public int ammoCount;
    public int clipSize;
    public int weaponNumber;
    public bool allowHoldShoot;
    [HideInInspector] public WeaponRecoil recoil;
    //bullet prefab
    public ParticleSystem hitPlayerEffect;
    public ParticleSystem hitEffect;
    public GameObject bulletPrefab;
    public ParticleSystem muzzleFlash;
    //public TrailRenderer trail;
    //Bullet date
    class Bullet
    {
        public GameObject bulletObj;
        public Vector3 currentPosition;
        public Vector3 currentVelocity;
        public float accumulatedDistance;
    }
    List<Bullet> bullets = new List<Bullet>();
    float accumulatedTime;
    bool canFire;



    //End weapon information



    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider coll;


    // Start is called before the first frame update
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        ptv = GetComponent<PhotonTransformView>();
        prv = GetComponent<PhotonRigidbodyView>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        recoil = GetComponent<WeaponRecoil>();
    }

    private void Update()
    {
        UpdateBullet(Time.deltaTime);
        //update can fire 
        if (pv.IsMine)
        {
            if (!canFire)
            {
                accumulatedTime += Time.deltaTime;
                if (accumulatedTime >= 1 / bulletRate)
                {
                    canFire = true;
                }
            }
        }
    }
    public bool CanReload()
    {
        if (totalAmmoCount > 0 && ammoCount != clipSize)
        {
            return true;
        }
        return false;
    }
    public void Reload()
    {
        //require ammo count
        int addCount = clipSize - ammoCount;
        if (totalAmmoCount > addCount)
        {
            ammoCount = clipSize;
            totalAmmoCount -= addCount;
        }
        else
        {
            ammoCount += totalAmmoCount;
            totalAmmoCount = 0;
        }
    }
    public void StartFire(Vector3 targetPos)
    {
        if (canFire && ammoCount > 0)
        {
            ammoCount--;
            canFire = false;
            accumulatedTime = 0;
            Vector3 dir;
            if (Vector3.Dot(firePoint.forward, (targetPos - firePoint.position).normalized) < 0)
            {
                dir = firePoint.forward;
            }
            else
            {
                dir = (targetPos - firePoint.position).normalized;
            }
            pv.RPC("CreateBullet", RpcTarget.All, dir);
        }
    }
    [PunRPC]
    void CreateBullet(Vector3 dir)
    {

        audioSource.PlayOneShot(fireClip);
        //create muzzle flash effect
        muzzleFlash.Play();
        //generate recoil
        //if (pv.IsMine)
        recoil.GenerateRecoil();
        //if ahead hava obstacle,direct hit trigger
        Vector3 startPos = transform.position;
        Vector3 displacement = (firePoint.position - startPos) / 10;
        Vector3 direction = firePoint.forward;
        float distance = displacement.magnitude;

        startPos -= displacement;
        for (int i = 0; i < 10; i++)
        {
            startPos += displacement;
            Debug.DrawLine(startPos, startPos + displacement, Color.black, 5);
            if (Physics.Raycast(startPos, firePoint.forward, out RaycastHit hit, distance, LayerManager.instance.weaponImpactLayer))
            {
               
                //determine if the hit is mine,skip it.
                if (hit.transform.CompareTag("Player"))
                {
                    HitBox hitBox = hit.transform.gameObject.GetComponent<HitBox>();
                    if (holderPlayer.Equals(hitBox.pv.Controller))
                    {
                        continue;
                    }
                }
                //judgment target
                if (hit.transform.CompareTag("Player"))
                {
                    if (pv.IsMine)
                    {
                        HitBox hitBox = hit.transform.gameObject.GetComponent<HitBox>();
                        if (hitBox)
                        {
                            //makesure hit is not mine
                            hitBox.TakeDamage(bulletDamage, holderPlayer);
                            hitBox.ImpulseBody(direction, hit.point);
                            EventCenter.instance.playerAttackDamage.Invoke(bulletDamage);
                        }
                    }
                    //hit player need follow player parent.
                    ParticleSystem bloodEffect = Instantiate(hitPlayerEffect, hit.transform);
                    bloodEffect.transform.position = hit.point;
                    bloodEffect.transform.right = hit.normal;
                    bloodEffect.Play();
                    Destroy(bloodEffect.gameObject, 5);
                    /*hitPlayerEffect.transform.position = hit.point;
                    hitPlayerEffect.transform.right = hit.normal;
                    hitPlayerEffect.Play();*/
                }
                else
                {
                    hitEffect.transform.position = hit.point;
                    hitEffect.transform.forward = hit.normal;
                    hitEffect.Emit(1);
                }
                return;
            }
        }
        //End if
        
        Bullet bullet = new Bullet();
        bullet.bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.currentPosition = firePoint.position;
        bullet.currentVelocity = dir * bulletSpeed;
        bullet.accumulatedDistance = 0;
        bullets.Add(bullet);

    }
    void UpdateBullet(float _deltaTime)
    {
        foreach (Bullet entry in bullets)
        {
            entry.currentVelocity -=  bulletDrop * _deltaTime * Vector3.up;
            Vector3 nextPosition = entry.currentPosition + (entry.currentVelocity) * _deltaTime;
            
            Vector3 direction = entry.currentVelocity.normalized;
            float distance = Vector3.Distance(entry.currentPosition, nextPosition);

            if (Physics.Raycast(entry.currentPosition, direction, out RaycastHit hit, distance, LayerManager.instance.weaponImpactLayer))
            {
                //determine if the hit is mine,skip it.
                if (hit.transform.CompareTag("Player"))
                {
                    HitBox hitBox = hit.transform.gameObject.GetComponent<HitBox>();
                    if (holderPlayer.Equals(hitBox.pv.Controller))
                    {
                        continue;
                    }
                }

                //judgment target
                if (hit.transform.CompareTag("Player"))
                {
                    if (pv.IsMine)
                    {
                        HitBox hitBox = hit.transform.gameObject.GetComponent<HitBox>();
                        if (hitBox)
                        {
                            //makesure hit is not mine
                            hitBox.TakeDamage(bulletDamage, holderPlayer);
                            hitBox.ImpulseBody(direction, hit.point);
                            EventCenter.instance.playerAttackDamage.Invoke(bulletDamage);
                        }
                    }
                    //hit player need follow player parent.
                    GameObject bloodEffect = Instantiate(hitPlayerEffect.gameObject, hit.transform);
                    bloodEffect.transform.position = hit.point;
                    bloodEffect.transform.right = hit.normal;
                    bloodEffect.GetComponent<ParticleSystem>().Play();
                    Destroy(bloodEffect, 5);
                    /*hitPlayerEffect.transform.position = hit.point;
                    hitPlayerEffect.transform.right = hit.normal;
                    hitPlayerEffect.Play();*/
                }
                else
                {
                    hitEffect.transform.position = hit.point;
                    hitEffect.transform.forward = hit.normal;
                    hitEffect.Emit(1);
                    /*hitPlayerEffect.transform.position = hit.point;
                    hitPlayerEffect.transform.right = hit.normal;
                    hitPlayerEffect.Emit(1);*/
                }


                //hitEffect.Play();
                Destroy(entry.bulletObj);
                bullets.Remove(entry);
                break;
            }




            entry.accumulatedDistance += distance;
            entry.currentPosition = nextPosition;
            entry.bulletObj.transform.position = nextPosition;
            //Determine if the distance is exceeded.
            if (entry.accumulatedDistance >= bulletMaxDistance)
            {
                Destroy(entry.bulletObj);
                bullets.Remove(entry);
                break;
            }
        }
    }

    public void SetHand()
    {
        pv.enabled = false;
        ptv.enabled = false;
        prv.enabled = false;
        rb.isKinematic = true;
        coll.enabled = false;
        GetCustomProperties();
    }
    public void SetGround()
    {
        pv.enabled = true;
        ptv.enabled = true;
        prv.enabled = true;
        rb.isKinematic = false;
        coll.enabled = true;
        //Update bullet information when dropped to the ground
        //SetCustomProperties();

    }
    void GetCustomProperties()
    {
        Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;
        if (hash.ContainsKey(pv.ViewID.ToString() + "ammoCount") && hash.ContainsKey(pv.ViewID.ToString() + "totalAmmoCount"))
        {
            ammoCount = (int)hash[pv.ViewID.ToString() + "ammoCount"];
            totalAmmoCount = (int)hash[pv.ViewID.ToString() + "totalAmmoCount"];
        }
    }
    public void SetCustomProperties()
    {
        Hashtable hash = new Hashtable();
        hash.Add(pv.ViewID.ToString() + "ammoCount", ammoCount);
        hash.Add(pv.ViewID.ToString() + "totalAmmoCount", totalAmmoCount);
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
    }

    //Audio Source
    public void DetachMagazine()
    {
        audioSource.PlayOneShot(detachMagazineClip);
    }
    public void AttachMagazine()
    {
        audioSource.PlayOneShot(attachMagazineClip);
    }
    public void RecoilAction()
    {
        if (recoilClip != null)
        {
            audioSource.PlayOneShot(recoilClip);
        }
    }
}
