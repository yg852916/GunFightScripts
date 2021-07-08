using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    //Character view
    public Vector2 viewAngle = Vector2.zero;
    public Vector2 viewSpeed;
    public Transform followDiractionPoint;
    Photon.Pun.PhotonView pv;

    bool isDead;
    private void Awake()
    {
        pv = GetComponent<Photon.Pun.PhotonView>();
    }
    void Start()
    {
        if (!pv.IsMine)
        {
            enabled = false;
        }
        else 
        {
            EventCenter.instance.AddEventListener("PlayerDie", PlayerDie);
        }
    }

    // Update is called once per frame
    void Update()
    {
        viewAngle.x += Input.GetAxis("Mouse X") * viewSpeed.x;
        viewAngle.y += -Input.GetAxis("Mouse Y") * viewSpeed.y;
    }
    private void FixedUpdate()
    {
        if (isDead)
        {
            if (viewAngle.x > 360) viewAngle.x -= 360;
            else if (viewAngle.x < -360) viewAngle.x += 360;
            viewAngle.y = Mathf.Clamp(viewAngle.y, -80, 80);
            Vector3 rotation = new Vector3(viewAngle.y, viewAngle.x * viewSpeed.x, 0);
            followDiractionPoint.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        }
        else
        {
            CameraAim();
        }
    }
    void CameraAim()
    {
        if (viewAngle.x > 360) viewAngle.x -= 360;
        else if (viewAngle.x < -360) viewAngle.x += 360;
        viewAngle.y = Mathf.Clamp(viewAngle.y, -80, 80);
        Vector3 rotation = new Vector3(viewAngle.y, viewAngle.x * viewSpeed.x, 0);
        transform.rotation = Quaternion.Euler(0, rotation.y, 0);
        followDiractionPoint.rotation = Quaternion.Euler(rotation.x, followDiractionPoint.eulerAngles.y, 0);
    }
    void PlayerDie(object info)
    {
        isDead = true;  
    }
}
