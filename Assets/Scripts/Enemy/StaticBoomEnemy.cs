using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StaticBoomEnemy : BaseEnemy
{
    [SerializeField]
    private float explodeTime = 5f;
    [SerializeField]
    private float blinkDuration = 3f;
    [SerializeField]
    private float blinkSpeed = 0.75f;

    private SpriteRenderer sp;

    public override void Start()
    {
        base.Start();
        sp = GetComponent<SpriteRenderer>();
        Invoke("EnemyExplode", explodeTime);
        Invoke("EnemyBlink", explodeTime - blinkDuration);
    }

    private void EnemyExplode()
    {
        if (damaged) return;
        PlayerManager player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        SoundManager.Instance.PlaySFX(SoundManager.Instance.monsterExplosion);
        player.TakeDamage(attackDamage);
        GameObject.Destroy(gameObject);
    }

    private void EnemyBlink()
    {
        StartCoroutine(CorutineBlink());
    }

    private IEnumerator CorutineBlink()
    {
        float timer = 0;
        Color orginColor = sp.color;
        float minAlpha = 0.3f;
        float maxAlpha = 1f;

        while (timer < blinkDuration)
        {
            float t = Mathf.PingPong(Time.time * (1f / blinkSpeed), 1f);
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
            SoundManager.Instance.PlaySFX(SoundManager.Instance.monsterWarning);
            Color c = sp.color;
            c.a = alpha;
            sp.color = c;

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
