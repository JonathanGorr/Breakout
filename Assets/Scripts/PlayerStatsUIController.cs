using System;
using UnityEngine;

public class PlayerStatsUIController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI[] scoresText;

    public TMPro.TextMeshProUGUI livesText;
    public TMPro.TextMeshProUGUI levelText;
    public TMPro.TextMeshProUGUI maxLevelText;

    public int multiplier = 16;

    private void Start()
    {
        GameManager.Instance.OnStateChangedEvent.AddListener(OnStateChange);
        GameManager.Instance.GameState.OnLifeChangedEvent.AddListener(OnLifeCountChanged);
        GameManager.Instance.GameState.OnLevelChangeEvent.AddListener(OnLevelChanged);
        GameManager.Instance.GameState.ScoreChangedEvent.AddListener(RepaintScore);
    }

    private void OnDestroy()
    {
        GameManager.Instance?.OnStateChangedEvent?.RemoveListener(OnStateChange);
        GameManager.Instance?.GameState?.OnLifeChangedEvent.RemoveListener(OnLifeCountChanged);
        GameManager.Instance?.GameState?.OnLevelChangeEvent.RemoveListener(OnLevelChanged);
        GameManager.Instance?.GameState?.ScoreChangedEvent.RemoveListener(RepaintScore);
    }

    private void OnLevelChanged(int lvl)
    {
        RepaintLevels();
    }

    private void OnStateChange(GameState newState)
    {
        switch (newState.State)
        {
            case GameManager.EGameState.GameLoop:
                Repaint();
                break;
        }
    }

    private void Repaint()
    {
        RepaintLives();
        RepaintScore();
        RepaintLevels();
    }

    private void OnLifeCountChanged()
    {
        RepaintLives();
    }

    private void RepaintLevels()
    {
        levelText.text = GameManager.Instance.GameState.Level.ToString();
        maxLevelText.text = GameManager.Instance.GameState.LevelCount.ToString();
    }

    private void RepaintLives()
    {
        livesText.text = GameManager.Instance.GameState.Lives.ToString();
    }

    public void RepaintScore()
    {
        foreach (var text in scoresText)
        {
            text.text = (GameManager.Instance.GameState.Score * multiplier).ToString();
        }
    }
}
