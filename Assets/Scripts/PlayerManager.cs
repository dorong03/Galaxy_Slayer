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

    public int attackCombo;
    public float comboDuration = 10f;
    public float comboTimer = 0;
    public bool isCombo = false;

    public PlayerController _playerController;
    
    public event Action OnPlayerDamaged;

    void Start()
    {
        attackCombo = 1;
        playerCurrentHealth = playerMaxHealth;
        _playerController = GetComponent<PlayerController>();
    }
    
    void Update()
    {
        if (!isCombo) return;
        UIManager.Instance.FillComboImage(comboTimer / comboDuration);
        comboTimer -= Time.deltaTime;
        if (comboTimer <= 0)
        {
            isCombo = false;
            attackCombo = 1;
            _playerController.playerFlySpeed = 13;
        }
    }

    public void UpdateComboTimer()
    {
        if (_playerController.playerFlySpeed < 20)
        {
            _playerController.playerFlySpeed += 0.5f;
        }
        isCombo = true;
        attackCombo++;
        if (comboDuration - (GameManager.Instance.survivalTime % 10) > 6)
        {
            comboTimer = (comboDuration - GameManager.Instance.survivalTime % 10);
        }
        else
        {
            comboTimer = 6f;
        }
        Debug.Log(attackCombo);
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
