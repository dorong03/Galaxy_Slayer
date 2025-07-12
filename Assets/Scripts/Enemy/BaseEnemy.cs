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
    
    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    protected virtual void Die()
    {
        GameManager.Instance.AddGameScore(killingScore);
        Debug.Log("gdfhzb");
        Destroy(gameObject);
    }
}
