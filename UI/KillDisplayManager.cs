using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class KillDisplayManager : MonoBehaviour
{
    public GameObject killListCellPrototype;
    public Transform killListParent;


    public void DisplayKillList(string killer,string dead)
    {
        GameObject cell = Instantiate(killListCellPrototype, killListParent);
        TMP_Text[] text = cell.GetComponentsInChildren<TMP_Text>();
        text[0].text = killer;
        text[1].text = dead;
        cell.SetActive(true);
        Destroy(cell, 5);
    }
}
