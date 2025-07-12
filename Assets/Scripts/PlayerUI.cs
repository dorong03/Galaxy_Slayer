using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public PlayerManager playerManager;
    void Start()
    {
        playerManager = gameObject.GetComponent<PlayerManager>();
        playerManager.OnPlayerDamaged += PlayerUIUpdate;
    }
    
    void PlayerUIUpdate()
    {
        Debug.Log(playerManager.playerCurrentHealth);
    }
}
