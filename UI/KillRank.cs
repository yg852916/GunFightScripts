using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using UnityEngine.UI;
public class KillRank : MonoBehaviour
{
    [SerializeField] Transform rankParent;
    [SerializeField] GameObject rankCellPrototype;

    GameObject[] killRanks=new GameObject[3];
    public void UpdateRank()
    {
        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++)
        {
            for (int j = 0; j < players.Length - 1 - i; j++)
            {
                if ((int)players[j].CustomProperties["killCount"] < (int)players[j + 1].CustomProperties["killCount"])
                {
                    Debug.Log(j);
                    Player temp = players[j];
                    players[j] = players[j + 1];
                    players[j + 1] = temp;
                }
            }
        }

        Image[] childs = rankParent.GetComponentsInChildren<Image>();
        for (int i = 0; i < childs.Length; i++)
        {
            Destroy(childs[i].gameObject);
        }
        for (int i = 0; i < players.Length && i < 3; i++)
        {
            if ((int)players[i].CustomProperties["killCount"] <= 0) return;
            GameObject info =  Instantiate(rankCellPrototype, rankParent);
            killRanks[i] = info;
            info.SetActive(true);
            info.GetComponentsInChildren<TMP_Text>()[0].text = players[i].NickName;
            info.GetComponentsInChildren<TMP_Text>()[1].text = players[i].CustomProperties["killCount"].ToString();
        }
    }
}
