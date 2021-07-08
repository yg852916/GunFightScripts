using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class HookSkill : MonoBehaviour
{

    [SerializeField] float coolDown;
    float coolDownTimer;

    [SerializeField] float maxDistance;
    [SerializeField] float emissionSpeed;
    [SerializeField] float springSpeed;
    [SerializeField] float maxVelocity;
    



    float accumulatedTime = 0;

    public LineRenderer hookPrefab;
    LineRenderer hook;
    //[SerializeField] bool isEmission;
    [SerializeField] bool isHit;
    [SerializeField] bool isPullHook;

    AudioSource audioSource;
    [SerializeField] AudioClip emissionClip;

    Vector3 destination;
    Vector3 originalVelocity;

    [SerializeField] float originalHorizontalVelocityImpact = 0.5f;
    [SerializeField] float originalVerticalVelocityImpact = 2f;
    Vector3 lastdirection = Vector3.zero;

    //need initialization
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public Transform emissionPoint;
    //Canvas display
    [HideInInspector] public TMP_Text secondText;
    [HideInInspector] public Button button;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Animator rigAnim;
    //photon
    PhotonView pv;

    
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (pv.IsMine)
        {
            if (coolDownTimer > 0)
                coolDownTimer -= Time.deltaTime;
            UpdateSkillState();


            if (Input.GetKeyDown(KeyCode.Q) && !hook)
            {
                if (coolDownTimer <= 0)
                {
                    coolDownTimer = coolDown;
                    EmissionHook();
                }
            }
            if (hook)
            {
                UpdateHook();
            }
            if (isPullHook)
            {
                PullHook();
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    EndPullHook();
                }
            }

        }
        else
        {
            if (hook)
            {
                Vector3 startPos = emissionPoint.position;
                Vector3 nextPos;
                accumulatedTime += Time.deltaTime;
                Vector3 direction = (destination - startPos).normalized;
                nextPos = startPos + emissionSpeed * accumulatedTime * direction;
                if (isPullHook)
                {
                    nextPos = destination;
                }
                hook.SetPosition(0, startPos);
                hook.SetPosition(1, nextPos);
            }
        }

    }
    void UpdateSkillState()
    {
        if (coolDownTimer > 0)
        {
            secondText.text = coolDownTimer.ToString("0.0");
            button.interactable = false;
        }
        else if(!button.interactable)
        {
            secondText.text = "";
            button.interactable = true;
        }

    }

    void EmissionHook()
    {
        audioSource.PlayOneShot(emissionClip);
        anim.SetInteger("skillNumber", 1);
        rigAnim.SetInteger("skillNumber", 1);
        //hook = (PhotonNetwork.Instantiate("hookLinePrefab", Vector3.zero, Quaternion.identity)).GetComponent<LineRenderer>();
        hook = Instantiate(hookPrefab, Vector3.zero, Quaternion.identity);

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, maxDistance, LayerManager.instance.hookSkillLayer))
        {
            lastdirection = Camera.main.transform.forward;
            isHit = true;
            destination = hit.point;
        }
        else
        {
            isHit = false;
            destination = Camera.main.transform.position + Camera.main.transform.forward * maxDistance;
        }
        EventCenter.instance.cancelHandAction.Invoke("Item");
        pv.RPC("RPC_EmissionHook", RpcTarget.Others, destination);
    }
    [PunRPC]
    void RPC_EmissionHook(Vector3 des)
    {
        audioSource.PlayOneShot(emissionClip);
        hook = Instantiate(hookPrefab, Vector3.zero, Quaternion.identity);
        destination = des;
        accumulatedTime = 0;
    }

    void UpdateHook()
    {
        Vector3 startPos = emissionPoint.position;
        Vector3 nextPos;

        if (!isPullHook)
        {
            accumulatedTime += Time.deltaTime;
            Vector3 direction = (destination - startPos).normalized;
            nextPos = startPos + emissionSpeed * accumulatedTime * direction;
        }
        else
        {
            nextPos = destination;
        }

        hook.SetPosition(0, startPos);
        hook.SetPosition(1, nextPos);

        //is end
        if (Vector3.Distance(nextPos, destination) <= 1 && !isPullHook)
        {
            accumulatedTime = 0;
            //start pull
            if (isHit)
            {
                isPullHook = true;
                coolDownTimer = coolDown;
                playerController.SetGravity(false);

                originalVelocity = playerController.cc.velocity;

                float angle = Vector3.Angle(originalVelocity, lastdirection);

                originalVelocity = new Vector3(originalVelocity.x * ((180 - angle) / 180) * originalHorizontalVelocityImpact, originalVelocity.y * originalVerticalVelocityImpact * ((180 - angle) / 180), originalVelocity.z * ((180 - angle) / 180) * originalHorizontalVelocityImpact);

                EventCenter.instance.canMove.Invoke(false);
                pv.RPC("RPC_PullHook", RpcTarget.Others);
            }
            else
            {
                coolDownTimer = 0.5f;
                EndPullHook();
            }
        }
    }
    void PullHook()
    {
        anim.SetInteger("skillStep", 1);
        rigAnim.SetInteger("skillStep", 1);
        Vector3 dir = (destination - playerController.cc.transform.position).normalized;

        if(Vector3.Angle(dir,lastdirection) < 50)
        {
           
            Vector3 velocity = dir * springSpeed + originalVelocity;
            float velocityY = velocity.y;
            velocity.y = 0;
            velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
            velocity.y = velocityY;
            playerController.additionalVelocity = velocity;
        }
        else
        {
            EndPullHook();
        }
    }

    [PunRPC]
    void RPC_PullHook()
    {
        isPullHook = true;
    }
    void EndPullHook()
    {
        EventCenter.instance.canMove.Invoke(true);
        anim.SetInteger("skillNumber", 0);
        anim.SetInteger("skillStep", 0);
        rigAnim.SetInteger("skillNumber", 0);
        rigAnim.SetInteger("skillStep", 0);
        lastdirection = Vector3.zero;
        isPullHook = false;
        playerController.SetGravity(true);
        pv.RPC("RPC_EndPullHook", RpcTarget.Others);
        if (hook)
            Destroy(hook.gameObject);
        StopAllCoroutines();
    }
    [PunRPC]
    void RPC_EndPullHook()
    {
        isPullHook = false;
        if (hook)
            Destroy(hook.gameObject);
    }
    private void OnDestroy()
    {
        if (pv.IsMine)
        {
            anim.SetInteger("skillNumber", 0);
            anim.SetInteger("skillStep", 0);
            rigAnim.SetInteger("skillNumber", 0);
            rigAnim.SetInteger("skillStep", 0);
            StopAllCoroutines();
        }
        if (hook)
            Destroy(hook.gameObject);
    }
}
