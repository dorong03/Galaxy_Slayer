using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Aiming,
    Flying,
    Attack,
    Die
}

public class PlayerController : MonoBehaviour
{
    public Transform arrowTransform;
    public float arrowMoveSpeed = 2.0f;
    public float arrowMoveAngle = 45.0f;

    public float playerFlySpeed = 13f;
    [SerializeField] 
    private float playerMaxFlySpeed = 20f;
    public int playerAttackDamage = 1;

    private PlayerState playerState;
    public Rigidbody2D rb;
    public Animator animator;
    private float angleTimer = 0f;
    private int arrowDirectionSign = 1;
    private Vector2 flyDirection;

    public SpriteRenderer visualSprite;
    public Sprite deadSprite;
    private PlayerState prevState;

    void Start()
    {
        playerState = PlayerState.Aiming;
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void Update()
    {
        if (!UIManager.Instance.FadeOutEnd) return;
        switch (playerState)
        {
            case PlayerState.Aiming:
                AimArrow();
                if (Input.GetMouseButtonDown(0))
                {
                    rb.constraints = RigidbodyConstraints2D.None;
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    PlayerFlying();
                }
                break;
            case PlayerState.Flying:
                animator.SetTrigger("Fly");
                rb.velocity = flyDirection * playerFlySpeed;
                break;

            case PlayerState.Attack:
                animator.SetTrigger("Attack");
                rb.velocity = flyDirection * playerFlySpeed;
                playerState = prevState;
                break;
        }
    }

    void AimArrow()
    {
        angleTimer += Time.deltaTime * arrowMoveSpeed * arrowDirectionSign;
        float angle = MathF.Sin(angleTimer) * arrowMoveAngle;
        arrowTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void PlayerFlying()
    {
        if (playerState == PlayerState.Aiming)
        {
            playerState = PlayerState.Flying;
            flyDirection = arrowTransform.up.normalized;
            arrowTransform.localRotation = Quaternion.Euler(0, 0, 0);
            arrowTransform.gameObject.SetActive(false);

            UpdateVisualRotation(flyDirection);
        }
    }

    void UpdateVisualRotation(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 왼쪽 기준 → 오른쪽 이동 시 flipX true
        visualSprite.flipX = direction.x > 0;

        // visualSprite 회전 초기화 (flip만 하려면 이 라인만 써도 됨)
        visualSprite.transform.rotation = Quaternion.identity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            prevState = playerState;
            playerState = PlayerState.Attack;
            if (collision.collider.GetComponent<BaseEnemy>().GetHealth() - playerAttackDamage <= 0)
            {
                UIManager.Instance.SpawnComboText(collision.collider.gameObject.transform.Find("HeadPosition").transform, GetComponent<PlayerManager>().attackCombo);
            }
            collision.collider.GetComponent<BaseEnemy>().TakeDamage(playerAttackDamage);
            
        }
        
        if (playerState != PlayerState.Flying) return;

        if (collision.collider.CompareTag("Obstacle"))
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            animator.ResetTrigger("Fly");
            animator.SetTrigger("Land");
            ContactPoint2D contact = collision.contacts[0];
            Vector2 normal = contact.normal;
            float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            arrowTransform.gameObject.SetActive(true);
            rb.constraints = RigidbodyConstraints2D.None;
            playerState = PlayerState.Aiming;
            
            visualSprite.transform.rotation = transform.rotation;
            visualSprite.flipX = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            prevState = playerState;
            playerState = PlayerState.Attack;
            if (other.GetComponent<BaseEnemy>().GetHealth() - playerAttackDamage <= 0)
            {
                UIManager.Instance.SpawnComboText(other.gameObject.transform.Find("HeadPosition").transform, GetComponent<PlayerManager>().attackCombo);
            }
            other.GetComponent<BaseEnemy>().TakeDamage(playerAttackDamage);
            other.gameObject.GetComponent<BaseEnemy>().TakeDamage(playerAttackDamage);
        }
    }

    public void Die()
    {
        // animator.gameObject.SetActive(false);
        playerState = PlayerState.Die;
        visualSprite.sprite = deadSprite;
        Vector3 vec = visualSprite.gameObject.transform.position;
        vec.y = -0.5f;
        visualSprite.gameObject.transform.position = vec;
    }

    /*
    public void ReflectArrow()
    {
        if (playerState == PlayerState.Aiming)
        {
            arrowDirectionSign *= -1;
            float currentPhase = angleTimer % (2 * Mathf.PI);
            angleTimer = (Mathf.PI - currentPhase) + (angleTimer - currentPhase);
        }
    }*/
}
