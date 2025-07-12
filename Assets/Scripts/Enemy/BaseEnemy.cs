using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    [SerializeField] 
    protected int maxHealth;
    [SerializeField]
    protected int currentHealth;
    [SerializeField] 
    protected int attackDamage;
    [SerializeField]
    protected int killingScore;

    public bool damaged = false;

    public GameObject deathPrefab;
    
    private PlayerManager _playerManager;
    
    public virtual void Start()
    {
        currentHealth = maxHealth;
        _playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual float GetHealth()
    {
        return currentHealth;
    }
    
    protected virtual void Die()
    {
        GameManager.Instance.AddGameScore(killingScore);
        Debug.Log("gdfhzb");
        _playerManager.UpdateComboTimer();
        Destroy(gameObject);
    }
}
