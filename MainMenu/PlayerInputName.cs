using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
public class PlayerInputName : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] Button confirmButton;
    const string playerNameKey = "PlayerName";
    void Start()
    {
        if (PlayerPrefs.HasKey(playerNameKey))
        { 
            string defaultName = PlayerPrefs.GetString("PlayerName");
            nameInputField.text = defaultName;
            ChangedInputField(defaultName);
        }
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log(PhotonNetwork.IsConnected);
        }
    }
    public void ChangedInputField(string name)
    {
        confirmButton.interactable = !string.IsNullOrEmpty(name);
    }
    public void ClickConfirmButton()
    {
        string playerName = nameInputField.text;
        PhotonNetwork.NickName = playerName;
        PlayerPrefs.SetString(playerNameKey, playerName);
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        MenuManager.Instanse.OpenMenu("LoadingMenu");
    }
}
