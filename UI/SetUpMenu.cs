using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class SetUpMenu : MonoBehaviour
{
    bool isDisplay = false;
    [SerializeField] GameObject setUpMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isDisplay == true)
            {
                EventCenter.instance.openMenu.Invoke(true);
                isDisplay = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                setUpMenu.SetActive(false);
            }
            else
            {
                EventCenter.instance.openMenu.Invoke(false);
                isDisplay = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                setUpMenu.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    public void LeaveRoom()
    {
        EventCenter.instance.itemAllDrop.Invoke();
        PhotonNetwork.LeaveRoom();
        //PhotonNetwork.Disconnect();
        //PhotonNetwork.LoadLevel(0);
       
        //SceneManager.LoadScene(0);
    }
}
