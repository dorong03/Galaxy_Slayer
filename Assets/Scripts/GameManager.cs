using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    public int gameScore;

    public int survivalTime;
    public UnLokcableArea[] unLockAreas;
    public int unLockedAreaCount;
    public int scorePerSecond;
    private float scoreBuffer;

    public event Action OnGameScoreAdd;
    float timer = 0;
    int interval = 1;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        OnGameScoreAdd += CheckUnlockAreas;
    }

    void Start()
    {
        scoreBuffer = 0;
        OnGameScoreAdd += DebugScore;
        survivalTime = 0;
        unLockedAreaCount = 0;
    }

    void Update()
    {
        if (!UIManager.Instance.FadeOutEnd) return;
        
        scoreBuffer += scorePerSecond * Time.deltaTime;
        if (scoreBuffer >= 1f)
        {
            int increase = Mathf.FloorToInt(scoreBuffer);
            AddGameScore(increase);
            scoreBuffer -= increase;
        }
        
        timer += Time.deltaTime;
        if (timer > interval)
        {
            survivalTime++;
            timer -= interval;
            Debug.Log(survivalTime);
        }
    }
    
    public void AddGameScore(int score)
    {
        gameScore += score;
        UIManager.Instance.UpdateScoreText(gameScore);
        OnGameScoreAdd?.Invoke();
    }
    
    public void DebugScore()
    {

    }

    private void CheckUnlockAreas()
    {
        foreach (var area in unLockAreas)
        {
            if (!area.isUnLocked && gameScore >= area.requiredScore)
            {
                area.UnLock();
                unLockedAreaCount++;
            }
        }
    }
}
