using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class DynamicBoomEnemy : BaseEnemy
{
    [SerializeField] private float explodeTime = 5f;
    [SerializeField] private float blinkDuration = 3f;
    [SerializeField] private float blinkSpeed = 0.75f;
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody2D rb;
    private SpriteRenderer sp;

    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();

        // Rigidbody 설정 최적화
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // 시작 방향 무작위 설정
        Vector2 ranDir = Random.insideUnitCircle.normalized;
        rb.velocity = ranDir * moveSpeed;

        // 폭발 예약
        Invoke(nameof(EnemyExplode), explodeTime);
        Invoke(nameof(EnemyBlink), explodeTime - blinkDuration);
    }

    private void EnemyExplode()
    {
        if (damaged) return;
        PlayerManager player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        SoundManager.Instance.PlaySFX(SoundManager.Instance.monsterExplosion);
        player.TakeDamage(attackDamage);
        Destroy(gameObject);
    }

    private void EnemyBlink()
    {
        StartCoroutine(CoroutineBlink());
    }

    private IEnumerator CoroutineBlink()
    {
        float timer = 0f;
        float minColor = 100f / 255f;
        float maxColor = 200f / 255f;
        float soundTimer = 0f;

        while (timer < blinkDuration)
        {
            soundTimer += Time.deltaTime;
            if (soundTimer >= 1f)
            {
                SoundManager.Instance.PlaySFX(SoundManager.Instance.monsterWarning);
                soundTimer = 0f;
            }

            float t = Mathf.PingPong(Time.time * (1f / blinkSpeed), 1f);
            float gbValue = Mathf.Lerp(minColor, maxColor, t);

            Color c = sp.color;
            c.g = gbValue;
            c.b = gbValue;
            sp.color = c;

            timer += Time.deltaTime;
            yield return null;
        }
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.contacts.Length > 0)
        {
            ContactPoint2D contact = other.contacts[0];
            Vector2 reflectDir = Vector2.Reflect(rb.velocity.normalized, contact.normal);
            StartCoroutine(ApplyBounceNextFrame(reflectDir));
        }
    }

    private IEnumerator ApplyBounceNextFrame(Vector2 direction)
    {
        yield return new WaitForFixedUpdate();
        rb.velocity = direction * moveSpeed;
    }
}
