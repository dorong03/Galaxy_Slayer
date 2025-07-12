/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Aiming,
    Attack,
    Die
}

public class PlayerController : MonoBehaviour
{
    public Transform arrowTransform;
    public float arrowMoveSpeed = 2.0f;
    public float arrowMoveAngle = 45.0f;

    public float playerFlySpeed = 3.0f;

    private PlayerState playerState;
    public Rigidbody2D rb;
    public Animator animator;
    private float angleTimer = 0f;
    private Vector2 flyDirection;

    void Start()
    {
        playerState = PlayerState.Aiming;
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        switch (playerState)
        {
            case PlayerState.Aiming:
                AimArrow();
                if (Input.GetMouseButtonDown(0))
                {
                    PlayerAttack();
                }
                break;
            
            case PlayerState.Attack:
                rb.velocity = flyDirection * playerFlySpeed;
                break;
            
            case PlayerState.Die:
                Debug.Log("플레이어 사망 로직");
                break;
        }
    }

    void AimArrow()
    {
        angleTimer += Time.deltaTime * arrowMoveSpeed;
        float angle = MathF.Sin(angleTimer) * arrowMoveAngle;
        arrowTransform.localRotation = Quaternion.Euler(0,0, angle);
    }
    
    void PlayerAttack()
    {
        if (playerState == PlayerState.Aiming)
        {
            playerState = PlayerState.Attack;
            flyDirection = arrowTransform.up.normalized;
            Debug.Log(flyDirection);
            arrowTransform.localRotation = Quaternion.Euler(0, 0, 0);
            arrowTransform.gameObject.SetActive(false);
        }
    }

    /*
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerState != PlayerState.Attack) return;

        if (collision.collider.CompareTag("Obstacle"))
        {
            playerState = PlayerState.Attack;
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            Vector2 normal = collision.contacts[0].normal;
            float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
            
            transform.rotation = Quaternion.Euler(0, 0, angle-90);
            
            // 화살 초기화
            arrowTransform.gameObject.SetActive(true);
            rb.constraints = RigidbodyConstraints2D.None;
            playerState = PlayerState.Aiming;
        }
    } 
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerState != PlayerState.Attack) return;

        if (collision.collider.CompareTag("Obstacle"))
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            ContactPoint2D contact = collision.contacts[0];

            // 1. 회전 (법선 기준으로 회전)
            Vector2 normal = contact.normal;
            float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            // 2. 위치 보정 (플레이어 pivot이 발이 아니라면)
            Vector2 contactPoint = contact.point;
            Vector2 playerBottomOffset = new Vector2(0, -GetComponent<Collider2D>().bounds.extents.y);
            transform.position = contactPoint - (Vector2)(transform.rotation * playerBottomOffset);

            // 화살 다시 보이게
            arrowTransform.gameObject.SetActive(true);
            rb.constraints = RigidbodyConstraints2D.None;
            playerState = PlayerState.Aiming;
        }
    }
}
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Aiming,
    Attack,
    Die
}

public class PlayerController : MonoBehaviour
{
    public Transform arrowTransform;
    public float arrowMoveSpeed = 2.0f;
    public float arrowMoveAngle = 45.0f;

    public float playerFlySpeed = 10f;
    public int playerAttackDamage = 1;

    private PlayerState playerState;
    public Rigidbody2D rb;
    public Animator animator;
    private float angleTimer = 0f;
    private int arrowDirectionSign = 1;
    private Vector2 flyDirection;

    void Start()
    {
        playerState = PlayerState.Aiming;
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        switch (playerState)
        {
            case PlayerState.Aiming:
                AimArrow();
                if (Input.GetMouseButtonDown(0))
                {
                    PlayerAttack();
                }
                break;

            case PlayerState.Attack:
                rb.velocity = flyDirection * playerFlySpeed;
                break;

            case PlayerState.Die:
                Debug.Log("플레이어 사망 로직");
                break;
        }
    }

    void AimArrow()
    {
        angleTimer += Time.deltaTime * arrowMoveSpeed * arrowDirectionSign;
        float angle = MathF.Sin(angleTimer) * arrowMoveAngle;
        arrowTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void PlayerAttack()
    {
        if (playerState == PlayerState.Aiming)
        {
            playerState = PlayerState.Attack;
            flyDirection = arrowTransform.up.normalized;
            arrowTransform.localRotation = Quaternion.Euler(0, 0, 0);
            arrowTransform.gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (playerState != PlayerState.Attack) return;
        
        if (collision.collider.CompareTag("Obstacle"))
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            ContactPoint2D contact = collision.contacts[0];
            Vector2 normal = contact.normal;
            float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            arrowTransform.gameObject.SetActive(true);
            rb.constraints = RigidbodyConstraints2D.None;
            playerState = PlayerState.Aiming;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (playerState != PlayerState.Attack) return;
        
        if (other.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<BaseEnemy>().TakeDamage(playerAttackDamage);
        }
    }

    public void Die()
    {
        playerState = PlayerState.Die;
        GameObject.Destroy(gameObject);
        Debug.Log("Died");
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
    }
    */
}
