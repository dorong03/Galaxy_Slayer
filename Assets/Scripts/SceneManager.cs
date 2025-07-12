using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    private bool firstTime = true;
    public SpriteRenderer rb_1;
    public SpriteRenderer rb_2;
    public Image panel;
    
    public SpriteRenderer blink;
    private float minAlpha = 50f / 255f;
    private float maxAlpha = 150f / 255f;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && firstTime)
        {
            firstTime = false;
            TitleStart();
        }

        float alpha = minAlpha + Mathf.PingPong(Time.time * 0.5f, maxAlpha - minAlpha);

        Color color = blink.color;
        color.a = alpha;
        blink.color = color;
    }

    void TitleStart()
    {
        StartCoroutine(FadeInPanel());
        SoundManager.Instance.PlaySFX(SoundManager.Instance.titleStart2);
    }

    void EndFade()
    {
        StartCoroutine(FadeOutPanel());
    }
    
    IEnumerator FadeOutPanel()
    {
        float duration = 0.2f;
        float elapsed = 0f;
        Color startColor = panel.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            panel.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        panel.color = new Color(startColor.r, startColor.g, startColor.b, 0f); 
        Invoke("InGameStart", 1.5f);
    }
    
    IEnumerator FadeInPanel()
    {
        float duration = 0.15f;
        float elapsed = 0f;
        Color startColor = panel.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            panel.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb_1.gameObject.SetActive(false);
        rb_2.gameObject.SetActive(true);
        panel.color = new Color(startColor.r, startColor.g, startColor.b, 1f); 
        Invoke("EndFade",0.5f);
    }

    void InGameStart()
    {
        StartCoroutine(InGameFadeInCoroutine());
    }
    
    IEnumerator InGameFadeInCoroutine()
    {
        float duration = 1.5f;
        float elapsed = 0f;
        Color startColor = Color.black;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            panel.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        panel.color = new Color(startColor.r, startColor.g, startColor.b, 1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

}