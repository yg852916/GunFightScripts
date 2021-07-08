using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class MyKillState : MonoBehaviour
{
    public TMP_Text killCountText;
    public TMP_Text deathCountText;
    public int killCount = 0;
    public int deathCount = 0;


    public void UpdateKill()
    {
        killCount++;
        killCountText.text = killCount.ToString();
    }
    public void UpdateDeath()
    {
        deathCount++;
        deathCountText.text = deathCount.ToString();
    }

}
