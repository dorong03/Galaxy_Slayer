using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Text ScoreText;
    public GameObject ComboText;
    public Image FadeOutPanel;
    public bool FadeOutEnd = false;
    public Image ComboImage;

    public void UpdateScoreText(int score)
    {  
        ScoreText.text = "Score : " +  score;
    }

    void Start()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    public void FillComboImage(float combo)
    {
        ComboImage.fillAmount = combo;
    }
    
    IEnumerator FadeOutCoroutine()
    {
        float duration = 1.5f;
        float elapsed = 0f;
        Color startColor = FadeOutPanel.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            FadeOutPanel.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;  // ← 매 프레임 대기
        }

        FadeOutPanel.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        FadeOutEnd = true;
    }


    public void SpawnComboText(Transform headTrans, int combo)
    {
        if (headTrans == null) return;

        // 1. 캔버스를 찾음
        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        if (canvas == null) return;

        // 2. 화면 좌표로 변환
        Vector3 screenPos = Camera.main.WorldToScreenPoint(headTrans.position);

        // 3. UI 오브젝트 생성
        GameObject com = Instantiate(ComboText);
        com.transform.SetParent(canvas.transform, false);

        // 4. 화면 좌표를 RectTransform 위치로 변환
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransform comRect = com.GetComponent<RectTransform>();

        Vector2 uiPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main, out uiPos))
        {
            comRect.anchoredPosition = uiPos;
        }

        // 5. 텍스트 설정
        com.GetComponent<Text>().text = "Combo x " + combo;
        StartCoroutine(TextRemove(com.GetComponent<Text>()));
    }
        
        IEnumerator TextRemove(Text combo)
        {
            float duration = 1f;
            float elapsed = 0f;
            Color startColor = combo.color;

            while (elapsed < duration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                combo.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }

            combo.color = new Color(startColor.r, startColor.g, startColor.b, 0f); 
            Destroy(combo.gameObject);
        }
}
