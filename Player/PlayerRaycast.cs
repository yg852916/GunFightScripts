using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerRaycast : MonoBehaviour
{
    [SerializeField] float distance;
    PhotonView pv;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (!pv.IsMine) enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        DoorControl();
    }
    void DoorControl()
    {
       
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, distance, LayerManager.instance.playerRaycastLayer))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                DoorControl doorControl = hit.transform.parent.GetComponent<DoorControl>();
                if (doorControl.openWay != 0)
                {
                    doorControl.CloseDoor();
                }
                else
                {
                    Vector3 doorPos = hit.transform.forward;
                    Vector3 playerPos = transform.forward;
                    if (Vector3.Dot(doorPos, playerPos) > 0)
                    {
                        doorControl.OpenDoor(1);
                    }
                    else
                    {
                        doorControl.OpenDoor(-1);
                    }
                }
            }
            
        }
    }
}
