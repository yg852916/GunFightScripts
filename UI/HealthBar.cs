using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    [SerializeField] Image redHealthBar;
    [SerializeField] Image greenHealthBar;
    [SerializeField] Image whiteHealthBar;
    [SerializeField] float declineSpeed;
    bool isPrevious = false;
    float previousHealthValue;
    private void Start()
    {
        EventCenter.instance.healthControl.AddListener(HealthControl);
    }
    private void Update()
    {
        if (redHealthBar.fillAmount > whiteHealthBar.fillAmount)
        {
            redHealthBar.fillAmount -= Time.deltaTime * declineSpeed;
        }
    }
    void HealthControl(string eventName, float value)
    {
        float currentHealth = PlayerHealth.instance.currentHealth;
        float maxHealth = PlayerHealth.instance.maxHealth;
        if (eventName == "ChangedHealth")
        {
            whiteHealthBar.fillAmount = currentHealth / maxHealth;
            if (redHealthBar.fillAmount < whiteHealthBar.fillAmount)
            {
                redHealthBar.fillAmount = whiteHealthBar.fillAmount;
            }
            if (isPrevious)
            {
                greenHealthBar.fillAmount = (currentHealth + previousHealthValue) / maxHealth;
            }
            else
            {
                greenHealthBar.fillAmount = whiteHealthBar.fillAmount;
            }
        }
        else if (eventName == "PreviousHealth")
        {
            if (isPrevious)
            {
                previousHealthValue += value;
            }
            else
            {
                previousHealthValue = value;
            }
            isPrevious = true;

            greenHealthBar.fillAmount = (currentHealth + previousHealthValue) / maxHealth;
        }
        else if (eventName == "CancelPreviousHealth")
        {
            isPrevious = false;
            greenHealthBar.fillAmount = currentHealth / maxHealth;
        }
    }

    public void ChangeHealth(object info)//only controll self take damage
    {
        PlayerHealth playerHealth = (info as PlayerHealth);
        whiteHealthBar.fillAmount = playerHealth.currentHealth / playerHealth.maxHealth;
    }

    private void OnDestroy()
    {
        EventCenter.instance.healthControl.RemoveListener(HealthControl);
    }
}
