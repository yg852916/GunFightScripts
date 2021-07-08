using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text roomPeopleText;
    [SerializeField] TMP_Text roomStateText;
    RoomInfo roomInfo;
    public void SetUp(RoomInfo _roomInfo)
    {
        roomInfo = _roomInfo;
        roomNameText.text = _roomInfo.Name;
        roomPeopleText.text = _roomInfo.PlayerCount.ToString();
        if (_roomInfo.IsOpen)
        {
            roomStateText.text = "";
        }
        else
        {
            roomStateText.text = "started";
        }
        GetComponent<Button>().interactable = _roomInfo.IsOpen;
    }
    public void OnClick()
    {
        Launcher.Instance.JoinRoom(roomInfo);
    }
    public void Remove()
    {
        Destroy(gameObject);
    }

}
