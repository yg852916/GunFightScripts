using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class AttackDamageDisplay : MonoBehaviour
{
    public TMP_Text damageText;

    float accumulatedDamage = 0;
    [SerializeField] float continueTime;
    float continueTimer;
    bool isDisplay;
    private void Start()
    {
        EventCenter.instance.playerAttackDamage.AddListener(AttackDamage);
    }
    private void Update()
    {
        if (continueTimer > 0)
        {
            isDisplay = true;
            continueTimer -= Time.deltaTime;
        }
        else if(isDisplay)
        {
            damageText.text = "";
            isDisplay = false;
        }
    }
    void AttackDamage(float value)
    {
        if (continueTimer > 0)
        {
            accumulatedDamage += value;
            continueTimer = continueTime;
        }
        else
        {
            accumulatedDamage = value;
            continueTimer = continueTime;
        }
        damageText.text = accumulatedDamage.ToString(); 
    }
}
