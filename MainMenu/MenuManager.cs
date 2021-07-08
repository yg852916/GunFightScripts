using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instanse;
    [SerializeField] Menu[] menus;
    private void Awake()
    {
        Instanse = this;
    }
    public void OpenMenu(string menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName)
            {
                menus[i].Open();
            }
            else if (menus[i].open)
            {
                menus[i].Close();
            }
        }
    }
    public void OpenMenu(Menu menu) 
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].open)
            {
                menus[i].Close();
            }
        }
        menu.Open();
        if (menu.menuName == "PlayerNameInputMenu")
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
        }
    } 
}
