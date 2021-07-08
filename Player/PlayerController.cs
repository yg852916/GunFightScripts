using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    //Audio
    AudioSource bodyAudio;
    [SerializeField] AudioClip footStepClip;

    float eachStepDurationTimer;
    //Photon
    PhotonView pv;
    Animator anim;
    public Animator rigAnim;
    //Movement
    public bool isJumping = false;
    public bool inAir = false;
    public bool isSprinting = false;
    public bool isCrouching = false;
    public bool isSliding = false;
    public bool canMove = true;
    public bool thirdJump = true;

    public float jumpPower;
    public float thirJumpPower;
    public float moveSpeed;
    public float sprintSpeedScale;
    public float sprintDelay;
    public float crouchSpeedScale;

    //sliding
    [SerializeField] Vector3 slideVelocity;
    [SerializeField] float slideThreshold;
    [SerializeField] float slideSpeedScale = 1f;
    [SerializeField] float slideSpeedDecline = 0.3f;
    [SerializeField] Vector3 velocitySlide;
    [SerializeField] float maxSlideVelocity;

    [SerializeField] Vector3 moveDirection;
    [SerializeField] Vector3 horizontalVelocity;

    //Input
    Vector2 input;

    //PlayerWeapon
    PlayerWeapon playerWeapon;
    PlayerPotionSystem playerPotionSystem;

    //character controller 
    public Vector3 additionalVelocity;

    [SerializeField] float yVelocity = 0;
    public CharacterController cc;
    [SerializeField] float gravity = 9.8f;
    public bool useGravity = true;
    float crouchHeight = 1.2f;
    float standHeight;
    Vector3 crouchCenter;
    Vector3 standCenter;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            instance = this;
        }
    }
    void Start()
    {
        if (!pv.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().transform.parent.gameObject);
            enabled = false;
            return;
        }
        bodyAudio = GetComponent<AudioSource>();
        cc = GetComponent<CharacterController>();

        standHeight = cc.height;
        standCenter = cc.center;
        crouchCenter = new Vector3(cc.center.x, cc.center.y - (standHeight - crouchHeight) / 2, cc.center.z);
        InitialPlayerCustomProperties();
        playerWeapon = GetComponent<PlayerWeapon>();
        playerPotionSystem  = GetComponent<PlayerPotionSystem>();
        anim = GetComponent<Animator>();
        //mouse setup
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        EventCenter.instance.canMove.AddListener(CanMove);
    }
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0.2f;
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            Time.timeScale = 1f;
        }

        //input
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        moveDirection = (transform.forward * input.y + transform.right * input.x);


        anim.SetFloat("InputX", input.x);
        anim.SetFloat("InputY", input.y);

        //start the sprint
        if (Input.GetKeyDown(KeyCode.LeftShift) && CanSprint() && !isSprinting)
        {
            StopCoroutine(IEnum_EndSprint());
            isSprinting = true;
            anim.SetBool("isSprinting", true);
            rigAnim.SetBool("isSprinting", true);
        }
        //end the sprint
        else if (!CanSprint() && isSprinting && anim.GetBool("isSprinting"))
        {
            StartCoroutine(IEnum_EndSprint());
        }

        //Jumping
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetAxisRaw("Mouse ScrollWheel") < 0) && cc.isGrounded && canMove && !isJumping)
        {
            bodyAudio.PlayOneShot(footStepClip);
            yVelocity = jumpPower;
            isJumping = true;
            anim.SetBool("isJumping", isJumping);
        }
        else if (yVelocity <= 0 && isJumping == true && cc.isGrounded)
        {
            bodyAudio.PlayOneShot(footStepClip);
            isJumping = false;
            anim.SetBool("isJumping", isJumping);
            thirdJump = true;
        }

        if (cc.isGrounded)
        {
            inAir = false;
        }
        else
        {
            if (inAir == false && !isJumping)
            {
                yVelocity = 0;
                
            }
            inAir = true;
            if (useGravity)
            {
                yVelocity -= gravity * Time.deltaTime;
            }
            if(canMove && !isSliding && thirdJump && (Input.GetKeyDown(KeyCode.Space) || Input.GetAxisRaw("Mouse ScrollWheel") < 0))
            {
                bodyAudio.PlayOneShot(footStepClip);
                yVelocity = thirJumpPower;
                isJumping = true;
                anim.SetBool("isJumping", isJumping);
                thirdJump = false;
            }
        }
 

        //Crouching
        if (Input.GetKey(KeyCode.LeftControl) && !isSliding && !isCrouching)
        {
            Crouch(true);
        }
        else if (!Input.GetKey(KeyCode.LeftControl) && isCrouching == true && CanStandUp())
        {
            Crouch(false);
        }


        //Sliding if isSprint is true,if the trigger is only done once
        float horizontalVelocityMagnitude = new Vector3(cc.velocity.x, 0, cc.velocity.z).magnitude;
        if (Input.GetKey(KeyCode.LeftControl) && horizontalVelocityMagnitude > slideThreshold && cc.isGrounded && !isSliding)
        {
            slideVelocity = new Vector3(cc.velocity.x, 0, cc.velocity.z) * slideSpeedScale;
            slideVelocity = Vector3.ClampMagnitude(slideVelocity, maxSlideVelocity);

            anim.SetBool("isCrouching", false);
            anim.SetBool("isSliding", true);
            isSliding = true;
            cc.height = crouchHeight;
            cc.center = crouchCenter;
        }
        if (isSliding)
        {
            if (cc.isGrounded)
            {
                slideVelocity = Vector3.MoveTowards(slideVelocity, Vector3.zero, slideSpeedDecline);
            }


            if (isJumping)
            {
                float speed = slideVelocity.z / slideVelocity.normalized.z;
                slideVelocity = Vector3.RotateTowards(slideVelocity, transform.forward, 90, 90) * speed;
            }
            if (cc.velocity.magnitude < 0.5f && (input.y != 0 || input.x != 0))
            {
                isCrouching = true;
                anim.SetBool("isCrouching", true);
                isSliding = false;
                anim.SetBool("isSliding", false);
            }
            if (!Input.GetKey(KeyCode.LeftControl) && CanStandUp())
            {
                Crouch(false);
                isSliding = false;
                anim.SetBool("isSliding", false);
            }
        }
        if (isSliding)
        {
            horizontalVelocity = slideVelocity;
        }
        else if (isCrouching && cc.isGrounded)
        {
            horizontalVelocity = moveDirection * moveSpeed * crouchSpeedScale;
        }
        else if (isSprinting)
        {
            horizontalVelocity = moveDirection * moveSpeed * sprintSpeedScale;
        }
        else
        {
            horizontalVelocity = moveDirection * moveSpeed;
        }

        if (horizontalVelocity.magnitude > 2.5f && cc.isGrounded && !isSliding && !isCrouching)
        {
            eachStepDurationTimer += Time.deltaTime;
            if (eachStepDurationTimer >= 0.3f && isSprinting)
            {
                bodyAudio.PlayOneShot(footStepClip);
                eachStepDurationTimer = 0;
            }
            else if (eachStepDurationTimer >= 0.36f)
            {
                bodyAudio.PlayOneShot(footStepClip);
                eachStepDurationTimer = 0;
            }

        }

    }
    private void FixedUpdate()
    {
        if (cc.collisionFlags != CollisionFlags.None && useGravity)
        {
            additionalVelocity = Vector3.zero;
        }
        Movement();
    }
    void InitialPlayerCustomProperties()
    {
        //store player id
        Hashtable hash = new Hashtable();
        hash.Add(PropertisKey.instance.viewID, pv.ViewID);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
    // Update is called once per frame

    IEnumerator IEnum_EndSprint()
    {
        anim.SetBool("isSprinting", false);
        rigAnim.SetBool("isSprinting", false);
        yield return new WaitForSeconds(sprintDelay);
        isSprinting = false;
    }

    public void SetGravity(bool value)
    {
        useGravity = value;
        if (value == false)
        {
            yVelocity = 0;
        }
    }
    void Crouch(bool value)
    {
        if (value)
        {
            cc.height = crouchHeight;
            cc.center = crouchCenter;
            isCrouching = true;
            anim.SetBool("isCrouching", true);
        }
        else
        {
            cc.height = standHeight;
            cc.center = standCenter;
            isCrouching = false;
            anim.SetBool("isCrouching", false);
        }
    }
    bool CanStandUp()
    {
        if (Physics.Raycast(transform.position, transform.up, out RaycastHit hit, standHeight,LayerManager.instance.crouchDetectLayer))
        {
            return false;
        }
        return true;
    }
    public void SetVelocuty(Vector3 value)
    {
        horizontalVelocity = value;
    }
    void Movement()
    {

        if (!canMove)
            horizontalVelocity = Vector3.zero;
        cc.Move((horizontalVelocity + additionalVelocity + (Vector3.up * yVelocity)) * Time.fixedDeltaTime);
        //cc.SimpleMove((horizontalVelocity + additionalVelocity + (Vector3.up * yVelocity)));
    }
    void CanMove(bool value)
    {
        canMove = value;
    }
    bool CanSprint()
    {
        if (!playerWeapon.isChangingWeapon && !playerPotionSystem.isRestore && !playerWeapon.isFiring && !playerWeapon.isReloading && !isCrouching && input.y > 0 && !isSliding && !playerWeapon.isZoomIn)
            return true;
        return false;
    }
    private void OnDestroy()
    {
        EventCenter.instance.canMove.RemoveListener(CanMove);
    }

}
