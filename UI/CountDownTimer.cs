using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountDownTimer : MonoBehaviour
{
    [SerializeField] float totalSeconds;
    [SerializeField] TMP_Text timeText;
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (totalSeconds > 0)
        {
            totalSeconds -= Time.fixedDeltaTime;
            if (totalSeconds < 0) totalSeconds = 0;
            float minute = Mathf.Floor(totalSeconds / 60);
            float second = Mathf.Floor(totalSeconds % 60);
            timeText.text = minute.ToString() + ":" + second.ToString();
        }
    }
}
