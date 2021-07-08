using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField roomNameInputField;

    [SerializeField] TMP_Text errorText;

    //player list content
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrototype;
    [SerializeField] GameObject StartGameBtn;

    //room info
    public override void OnJoinedRoom()
    {
        roomNameText.text = "Room Name : " + PhotonNetwork.CurrentRoom.Name;
        MenuManager.Instanse.OpenMenu("RoomMenu");
        Player[] players = PhotonNetwork.PlayerList;

        PlayerListItem[] allPlayerContent = playerListContent.GetComponentsInChildren<PlayerListItem>();
        for (int i = 0; i < allPlayerContent.Length; i++)
        {
            Destroy(allPlayerContent[i].gameObject);
        }
        for (int i = 0; i < players.Length; i++)
        {
            GameObject playerListItem = Instantiate(playerListItemPrototype, playerListContent);
            playerListItem.GetComponent<PlayerListItem>().SetUp(players[i]);
            playerListItem.SetActive(true);
        }
        StartGameBtn.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject playerListItem = Instantiate(playerListItemPrototype, playerListContent);
        playerListItem.GetComponent<PlayerListItem>().SetUp(newPlayer);
        playerListItem.SetActive(true);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        StartGameBtn.SetActive(PhotonNetwork.IsMasterClient);
    }

    //Room list content
    Dictionary<string, RoomListItem> roomListContent = new Dictionary<string, RoomListItem>();
    [SerializeField] private RoomListItem roomListItemPrototype;
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        Debug.Log("OnRoomListUpdate...");
        foreach (RoomInfo entry in roomList)
        {
            if (roomListContent.ContainsKey(entry.Name))
            {
                if (entry.RemovedFromList)
                {
                    roomListContent[entry.Name].Remove();
                    roomListContent.Remove(entry.Name);
                }
                else
                {
                    roomListContent[entry.Name].SetUp(entry);
                }
            }
            else
            {
                if (!entry.RemovedFromList)
                {
                    roomListContent.Add(entry.Name, Instantiate(roomListItemPrototype));
                    RoomListItem roomListItem = roomListContent[entry.Name];
                    roomListItem.SetUp(entry);
                    roomListItem.transform.SetParent(roomListItemPrototype.transform.parent);
                    roomListItem.gameObject.SetActive(true);
                }
            }
        }
    }


    //--------------

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {

    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster...");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("OnDisconnected...");
        //SceneManager.LoadScene(0);
    }
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("OnJoinedLobby...");
        MenuManager.Instanse.OpenMenu("LobbyMenu");
    }

    public void StartGame()
    {
        MenuManager.Instanse.OpenMenu("GameLoadingMenu");
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(1);
    }
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instanse.OpenMenu("LoadingMenu");
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoomFailed...");
        errorText.text = "Room creation Failed: " + message;
        MenuManager.Instanse.OpenMenu("ErrorMenu");
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instanse.OpenMenu("LoadingMenu");
    }
    public void JoinRoom(RoomInfo roomInfo)
    {
        PhotonNetwork.JoinRoom(roomInfo.Name);
        MenuManager.Instanse.OpenMenu("LoadingMenu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom...");
    }
}
