using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private int gameScore;

    public UnLokcableArea[] unLockAreas;
    public int unLockedAreaCount = 0;
    public int scorePerSecond;
    private float scoreBuffer;

    public event Action OnGameScoreAdd;
    
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
    }

    void Update()
    {
        scoreBuffer += scorePerSecond * Time.deltaTime;
        if (scoreBuffer >= 1f)
        {
            int increase = Mathf.FloorToInt(scoreBuffer);
            AddGameScore(increase);
            scoreBuffer -= increase;
        }
    }
    
    public void AddGameScore(int score)
    {
        gameScore += score;
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
