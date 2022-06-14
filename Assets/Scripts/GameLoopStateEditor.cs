using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GameLoopState))]
public class GameLoopStateEditor : UnityEditor.Editor
{
    private GameLoopState t;

    private void OnEnable()
    {
        t = target as GameLoopState;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Complete Level"))
        {
            t.SkipLevel();
        }
    }
}
#endif

public class GameLoopState : GameState
{
    public int StartLives = 10;
    private int lives;
    public int Lives => lives;
    public int Level => levelIndex+1;
    private uint score;
    public uint Score => score;

    public UnityEvent SpawnBallEvent { get; } = new UnityEvent();
    public UnityEvent DestroyBallsEvent { get; } = new UnityEvent();
    public UnityEvent OnLifeChangedEvent { get; } = new UnityEvent();
    public UnityEvent<int> OnLevelChangeEvent { get; } = new UnityEvent<int>();
    public UnityEvent ScoreChangedEvent { get; } = new UnityEvent();

    [SerializeField] private Level[] levels;
    public int LevelCount => levels.Length;
    private int levelIndex = 0;

    public UnityEvent LastLevelEvent = new UnityEvent();

    private void OnCubeDestroyed(int value)
    {
        score += (uint)value;
        ScoreChangedEvent.Invoke();
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        Reset();
        SetLevel(levelIndex);

        Cube.OnDeathEvent.AddListener(OnCubeDestroyed);
        Ball.OnDeathEvent.AddListener(OnBallDeath);
    }

    public override void OnExitState()
    {
        base.OnExitState();
        Cube.OnDeathEvent.RemoveListener(OnCubeDestroyed);
        Ball.OnDeathEvent.RemoveListener(OnBallDeath);
        levels[levelIndex].Disable();
        DestroyAllBalls();
    }

    private void SetLevel(int newIndex)
    {
        levels[levelIndex].LevelCompletedEvent.RemoveListener(OnLevelCompleted);
        if (newIndex == levels.Length-1)
        {
            LastLevelEvent.Invoke();
        }
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].Disable();
        }
        levelIndex = newIndex;
        levels[levelIndex].Enable();
        levels[levelIndex].LevelCompletedEvent.AddListener(OnLevelCompleted);
        OnLevelChangeEvent.Invoke(Level);
        Invoke(nameof(SpawnBall), 0.5f);
    }

    public void SkipLevel()
    {
        OnLevelCompleted();
    }

    private void OnLevelCompleted()
    {
        levels[levelIndex].LevelCompletedEvent.RemoveListener(OnLevelCompleted);
        DestroyAllBalls();
        int nextLevel = levelIndex + 1;
        if (nextLevel > levels.Length-1)
        {
            GameManager.Instance.SetState(GameManager.EGameState.GameCompleted);
            return;
        }
        SetLevel(levelIndex + 1);
    }

    private void Reset()
    {
        score = 0;
        levelIndex = 0;
        lives = StartLives;
    }

    private void DestroyAllBalls()
    {
        DestroyBallsEvent.Invoke();
    }

    private void OnBallDeath()
    {
        //only penalize the player when their final ball
        //dies
        if (Ball.BallCount < 1)
        {
            --lives;
            OnLifeChangedEvent.Invoke();
            if (lives <= 0)
            {
                GameManager.Instance.SetState(GameManager.EGameState.GameOver);
                return;
            }
            Invoke(nameof(SpawnBall), 2f);
        }
    }

    private void SpawnBall()
    {
        if (Ball.BallCount < 1)
        {
            SpawnBallEvent.Invoke();
        }
    }
}
