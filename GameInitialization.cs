using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class GameInitialization : MonoBehaviourPunCallbacks
{
    public byte maxPlayersPerRoom;
    [SerializeField] GameObject loadingPanel;
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster...");
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("OnJoinedRoom...");
        PhotonNetwork.Instantiate("PlayerManager", Vector3.zero, Quaternion.identity);
        if (PhotonNetwork.IsMasterClient)
        {
            CreateAllItem();
            loadingPanel.SetActive(false);
        }

    }
    void CreateAllItem()
    {

        Transform[] allPoint = ItemRebirthPoint.instance.allPoint;
        for (int i = 0; i < allPoint.Length; i++)
        {
            PhotonNetwork.InstantiateRoomObject("RGD-5", allPoint[i].position, Quaternion.identity);
            int randomNumGun = Random.Range(1, 6);
            int randomNumItem = Random.Range(1, 11);
            //Debug.Log(randomNumItem + "  " + randomNumGun);
            if (randomNumItem > 3)
            {
                PhotonNetwork.InstantiateRoomObject("PotionHealth", allPoint[i].position, Quaternion.identity);
            }
            if (randomNumItem > 6)
            {
                PhotonNetwork.InstantiateRoomObject("hookSkillItem", allPoint[i].position, Quaternion.identity);
            }
            if (randomNumGun == 1)
            {
                PhotonNetwork.InstantiateRoomObject("AK47", allPoint[i].position, Quaternion.identity);
            }
            else if (randomNumGun == 2)
            {
                PhotonNetwork.InstantiateRoomObject("M1911", allPoint[i].position, Quaternion.identity);
            }
            else if (randomNumGun == 3)
            {
                PhotonNetwork.InstantiateRoomObject("M4_8", allPoint[i].position, Quaternion.identity);
            }
            else if (randomNumGun == 4)
            {
                PhotonNetwork.InstantiateRoomObject("M107", allPoint[i].position, Quaternion.identity);
            }
            else if (randomNumGun == 5)
            {
                PhotonNetwork.InstantiateRoomObject("M249", allPoint[i].position, Quaternion.identity);
            }
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed...");
        PhotonNetwork.CreateRoom(null, new RoomOptions
        {
            MaxPlayers = maxPlayersPerRoom
        });
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("OnCreatedRoom...");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("OnCreateRoomFailed...");
    }
    public override void OnLeftRoom()
    {
        //base.OnLeftRoom();
        Debug.Log("OnLeftRoom...");
        SceneManager.LoadScene(0);
    }
}
