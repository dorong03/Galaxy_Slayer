using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public int playerMaxHealth = 100;
    public int playerCurrentHealth;
    public Image playerHealthBar;

    public event Action OnPlayerDamaged;

    void Start()
    {
        playerCurrentHealth = playerMaxHealth;
    }

    public void TakeDamage(int damage)
    {
        playerCurrentHealth -= damage;
        playerHealthBar.fillAmount = (float)playerCurrentHealth / playerMaxHealth;
        Debug.Log(playerCurrentHealth);
        OnPlayerDamaged?.Invoke();
        if (playerCurrentHealth <= 0)
        {
            GetComponent<PlayerController>().Die();
        }
    }
}
