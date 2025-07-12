using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Text ComboText;
    public Text ScoreText;
    public Text TimeText;
    
    public void LoadStartScene()
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.gameOverRetry);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        Destroy(GameManager.Instance.gameObject);
        Destroy(UIManager.Instance.gameObject);
    }

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.Log("2번째 씬부터 시작해");
            return;
        }
        ComboText.text = GameManager.Instance.maxCombo.ToString();
        ScoreText.text = GameManager.Instance.gameScore.ToString();
        TimeSpan ts = TimeSpan.FromSeconds(GameManager.Instance.survivalTime);
        TimeText.text = ts.ToString(@"mm\:ss");
    }
}
