using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    [HideInInspector]
    public float maxHealth = 100f;
    [HideInInspector]
    public float maxMana = 100f;
    [HideInInspector]
    public float maxFuryEnergy = 100f;
    [HideInInspector]
    public float maxTime = 100f;

    // [HideInInspector]
    public float currentHealth;
  //  [HideInInspector]
    public float currentMana;
    //[HideInInspector]
    public float currentFuryEnergy;

    public float currentTimeSlow;

    public Slider energySlider;
    public Slider healthSlider;
    public Slider furySlider;
    public Slider timeSlider;
    public Character p;

    private void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentFuryEnergy = 0f;
        currentTimeSlow = 0f;
    }

    private void Update()
    {
        UpdateSlider(currentMana, maxMana, energySlider);
        UpdateSlider(currentHealth, maxHealth, healthSlider);
        UpdateSlider(currentFuryEnergy, maxFuryEnergy, furySlider);
        UpdateSlider(currentTimeSlow, maxTime, timeSlider);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    public void UseMana(float amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            currentMana = Mathf.Ceil(currentMana);
        }
        else
        {
            Debug.Log("Mana insuficiente!");
        }    
    }

    public void UseTime(float amount)
    {
        currentTimeSlow += amount;
        currentTimeSlow = Mathf.Clamp(currentTimeSlow, 0f, maxTime);    
    }


    public void RechargeMana(float amount)
    {
        currentMana += amount;

        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
    }

    public void GainFuryEnergy(float amount)
    {
        currentFuryEnergy += amount;

        if (currentFuryEnergy > maxFuryEnergy)
        {
            currentFuryEnergy = maxFuryEnergy;
        }
    }

    public void UseFuryAttack(float furyCost)
    {
        if (currentFuryEnergy >= furyCost)
        {
            currentFuryEnergy -= furyCost;
        }
        else
        {
            Debug.Log("Energia de fúria insuficiente!");
        }
    }

    public void RechargeHealth(float amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    private void Die()
    {
        Debug.Log("Morreu!");
        p.ISDEAD = true;
        p.canAttack = false;

        Invoke("RestartScene", 3.0f);
    }


    private void UpdateSlider(float currentValue, float maxValue, Slider slider)
    {
        if (slider != null)
        {
            float clampedValue = Mathf.Clamp(currentValue, 0f, maxValue);
            slider.value = clampedValue / maxValue;
            int roundedValue = Mathf.RoundToInt(currentValue);
            slider.value = roundedValue / maxValue;
        }
    }

}


