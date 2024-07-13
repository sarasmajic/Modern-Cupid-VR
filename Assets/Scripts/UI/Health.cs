using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float maxHealth = 100.0f;
    private float currentHealth;
    
    public event Action<float> OnHealthPctChanged = delegate {  };

    public void Start() {
        currentHealth = maxHealth;
    }

    public void ModifyHealth(float amount) {
        currentHealth += amount;
        float currentHealthPct = currentHealth / maxHealth;
        OnHealthPctChanged(currentHealthPct);
    }

    public bool Dead() {
        return currentHealth <= 0;
    }

    public void SetStartHealth(float x) {
        currentHealth = x;
        maxHealth = x;
    }
}