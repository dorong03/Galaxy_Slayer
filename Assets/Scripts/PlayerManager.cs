using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public int playerMaxHealth = 100;
    public int playerCurrentHealth;
    public Image playerHealthBar;

    public int attackCombo;
    public float comboDuration = 13f;
    public float comboTimer = 0;
    public bool isCombo = false;

    public PostProcessVolume volume;
    private Vignette vignette;

    public PlayerController _playerController;
    
    public event Action OnPlayerDamaged;
    public float minIntensity = 0.59f;
    public float maxIntensity = 0.7f;
    
    void Start()
    {
        attackCombo = 1;
        playerCurrentHealth = playerMaxHealth;
        _playerController = GetComponent<PlayerController>();
        volume.profile.TryGetSettings(out vignette);
        vignette.intensity.Override(minIntensity);
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
            _playerController.arrowMoveSpeed = 2f;
        }
    }

    public void UpdateComboTimer()
    {
        if (_playerController.playerFlySpeed < 25)
        {
            _playerController.playerFlySpeed += 2f;
        }
        isCombo = true;
        attackCombo++;
        if (attackCombo > GameManager.Instance.maxCombo) GameManager.Instance.maxCombo = attackCombo;
        if(attackCombo >= 6) _playerController.arrowMoveSpeed = 3f;
        comboDuration = comboDuration - GameManager.Instance.survivalTime % 10f;
        comboDuration = Mathf.Clamp(comboDuration, 9f, comboDuration);
        comboTimer = comboDuration;
        Debug.Log(attackCombo);
    }

    public void TakeDamage(int damage)
    {
        playerCurrentHealth -= damage;
        playerHealthBar.fillAmount = (float)playerCurrentHealth / playerMaxHealth;
        Debug.Log(playerCurrentHealth);
        OnPlayerDamaged?.Invoke();
        _playerController.OnPlayerHit();
        if (playerCurrentHealth <= 0)
        {
            GetComponent<PlayerController>().Die();
        }
    }
}
