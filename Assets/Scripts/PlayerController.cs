using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

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
    public float arrowMoveSpeed = 2f;
    public float arrowMoveAngle = 45f;

    public float playerFlySpeed = 13f;
    public int playerAttackDamage = 1;

    private PlayerState playerState;
    public Rigidbody2D rb;
    public Animator animator;
    private float angleTimer = 0f;
    private Vector2 flyDirection;
    private PlayerState prevState;

    public SpriteRenderer visualSprite;
    public Sprite deadSprite;

    public LayerMask enemyLayerMask;
    public GameObject hitEffectPrefab;
    private List<Collider2D> overlappedEnemies = new List<Collider2D>();

    public CameraShake cameraShake;

    public PostProcessVolume volume;

    private Vignette vignette;
    
    public float minIntensity = 0.6f;
    public float maxIntensity = 0.7f;
    public float period = 1.5f; // 한 사이클 주기 = 1.5초

    private bool isRunning = false;
    private float startTime;

    void Start()
    {
        playerState = PlayerState.Aiming;
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        cameraShake = Camera.main.GetComponent<CameraShake>();
        volume.profile.TryGetSettings(out vignette);
    }

    void Update()
    {
        if (isRunning)
        {
            float elapsed = Time.time - startTime;
            // 사인 함수로 0~1 사이 t 값 생성
            float t = (Mathf.Sin(elapsed * (2 * Mathf.PI / period)) * 0.5f + 0.5f);
            vignette.intensity.value = Mathf.Lerp(minIntensity, maxIntensity, t);
        }

        if (!UIManager.Instance.FadeOutEnd) return;

        switch (playerState)
        {
            case PlayerState.Aiming:
                AimArrow();
                if (Input.GetMouseButtonDown(0))
                {
                    rb.constraints = RigidbodyConstraints2D.None;
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    SoundManager.Instance.PlaySFX(SoundManager.Instance.playerAttack1);
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
        angleTimer += Time.deltaTime * arrowMoveSpeed;
        float angle = Mathf.Sin(angleTimer) * arrowMoveAngle;
        arrowTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void PlayerFlying()
    {
        playerState = PlayerState.Flying;
        flyDirection = arrowTransform.up.normalized;
        arrowTransform.localRotation = Quaternion.identity;
        arrowTransform.gameObject.SetActive(false);
        UpdateVisualRotation(flyDirection);
    }

    void UpdateVisualRotation(Vector2 dir)
    {
        visualSprite.flipX = dir.x > 0;
        visualSprite.transform.rotation = Quaternion.identity;
    }

    // --- 콜라이더 B: 적 트리거 감지용 ---
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.gameObject.layer == LayerMask.NameToLayer("DynamicEnemy"))
        {
            if (playerState != PlayerState.Aiming)
            {
                if (!overlappedEnemies.Contains(other))
                {
                    overlappedEnemies.Add(other);
                    SoundManager.Instance.PlaySFX(SoundManager.Instance.monsterHit2);
                    if (hitEffectPrefab != null)
                        Instantiate(hitEffectPrefab, other.transform.position, Quaternion.identity);
                    if (other.GetComponent<Rigidbody2D>() != null)
                    {
                        other.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        other.GetComponent<CapsuleCollider2D>().isTrigger = true;
                    }

                    other.gameObject.GetComponent<BaseEnemy>().damaged = true;
                }
            }
            else
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.monsterHit2);
                if (hitEffectPrefab != null)
                    Instantiate(hitEffectPrefab, other.transform.position, Quaternion.identity);
                other.GetComponent<BaseEnemy>().TakeDamage(playerAttackDamage);
            }
        }
    }

    // --- 콜라이더 A: 물리 충돌용 ---
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider != null && coll.collider.CompareTag("Enemy") && playerState == PlayerState.Aiming)
        {
            prevState = playerState;
            playerState = PlayerState.Attack;
            var e = coll.collider.GetComponent<BaseEnemy>();
            if (e != null)
            {
                if (e.GetHealth() - playerAttackDamage <= 0)
                {
                    UIManager.Instance.SpawnComboText(
                        coll.collider.transform.Find("HeadPosition"),
                        GetComponent<PlayerManager>().attackCombo);
                }
                e.TakeDamage(playerAttackDamage);
            }
        }

        if (playerState != PlayerState.Flying) return;

        if (coll.collider != null && coll.collider.CompareTag("Obstacle"))
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            animator.ResetTrigger("Fly");
            animator.SetTrigger("Land");

            // 착지 회전
            ContactPoint2D c = coll.contacts[0];
            float angle = Mathf.Atan2(c.normal.y, c.normal.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            // 겹친 적 처리
            if (overlappedEnemies.Count == 0)
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.playerLand1);
            }
            else
            {
                ProcessOverlaps();
                cameraShake.Shake();
            }
            
            overlappedEnemies.Clear();

            arrowTransform.gameObject.SetActive(true);
            rb.constraints = RigidbodyConstraints2D.None;
            playerState = PlayerState.Aiming;

            visualSprite.transform.rotation = transform.rotation;
            visualSprite.flipX = false;
        }
    }

    void ProcessOverlaps()
    {
        foreach (var col in overlappedEnemies)
        {
            if (col != null && col.gameObject != null)
            {
                var enemy = col.GetComponent<BaseEnemy>();
                if (enemy != null)
                {
                    UIManager.Instance.SpawnComboText(
                        col.transform.Find("HeadPosition"),
                        GetComponent<PlayerManager>().attackCombo);
                    enemy.TakeDamage(playerAttackDamage);
                    if (enemy.deathPrefab != null)
                    {
                        Debug.Log("DeathPRefab");
                        Instantiate(enemy.deathPrefab, enemy.transform.position, Quaternion.identity);
                        SoundManager.Instance.PlaySFX(SoundManager.Instance.playerLandAttack);
                    }
                }
            }
        }
    }
    
    // 호출 시 진동 시작
    public void OnPlayerHit()
    {
        isRunning = true;
        startTime = Time.time;
        if (!(GetComponent<PlayerManager>().playerCurrentHealth <= 20))
        {
            Invoke("StopOscillation", 1.2f);    
        }
    }

    // 호출 시 진동 종료
    public void StopOscillation()
    {
        isRunning = false;
        vignette.intensity.Override(minIntensity);
    }

    public void Die()
    {
        playerState = PlayerState.Die;
        visualSprite.sprite = deadSprite;
        Vector3 p = visualSprite.transform.position;
        p.y = -0.5f;
        visualSprite.transform.position = p;
        Destroy(UIManager.Instance.gameObject);
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }
}
