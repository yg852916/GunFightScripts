using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
public class NetworkState : MonoBehaviour
{
    [SerializeField] TMP_Text pingText;
    [SerializeField] TMP_Text peopleCountText;
    public static NetworkState instance;
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(transform.parent.gameObject);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log(PhotonNetwork.GetPing());
            Debug.Log((int)PhotonNetwork.CountOfPlayers);
        }
    }
    private void FixedUpdate()
    {
        if (PhotonNetwork.IsConnected)
        {
            pingText.text = "ping:" + PhotonNetwork.GetPing() + "ms";
            peopleCountText.text = "people:" + (int)PhotonNetwork.CountOfPlayers;
        }
        else
        {
            pingText.text = "disconnect";
            peopleCountText.text = "";
        }

    }
}