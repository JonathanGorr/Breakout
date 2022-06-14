using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : UnityEditor.Editor
{
    private GameManager t;

    private void OnEnable()
    {
        t = target as GameManager;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Title"))
        {
            t.SetState(GameManager.EGameState.Title);
        }
        else if (GUILayout.Button("Game Loop"))
        {
            t.SetState(GameManager.EGameState.GameLoop);
        }
        else if (GUILayout.Button("Game Over"))
        {
            t.SetState(GameManager.EGameState.GameOver);
        }
        else if (GUILayout.Button("Game Complete"))
        {
            t.SetState(GameManager.EGameState.GameCompleted);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif

public class GameManager : MonoBehaviour<GameManager>
{
    public enum EGameState
    {
        Title,
        GameLoop,
        GameOver,
        GameCompleted
    }

    public StateMachine<GameState> stateMachine = new StateMachine<GameState>();

    [SerializeField] private GameState titleState;
    [SerializeField] private GameState gameState;
    [SerializeField] private GameState gameOverState;
    [SerializeField] private GameState gameCompleteState;
    public TitleState TitleState => titleState as TitleState;
    public GameLoopState GameState => gameState as GameLoopState;
    public GameOverState GameOverState => gameOverState as GameOverState;
    //public gameCompleteState GameCompleteState => gameCompleteState;

    private Dictionary<EGameState, GameState> Map = new Dictionary<EGameState, GameState>();
    public UnityEvent<GameState> OnStateChangedEvent = new UnityEvent<GameState>();
    public GameState CurrentState => stateMachine.CurrentState;

    protected override void Awake()
    {
        base.Awake();
        MapStates();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        SetState(TitleState);
    }

    private void MapStates()
    {
        Map.Add(EGameState.Title, titleState);
        Map.Add(EGameState.GameLoop, gameState);
        Map.Add(EGameState.GameOver, gameOverState);
        Map.Add(EGameState.GameCompleted, gameCompleteState);
    }

    public void SetState(EGameState newState)
    {
        if (Map.ContainsKey(newState))
        {
            if(Map[newState] != null)
                SetState(Map[newState]);
        }
    }

    private void SetState(GameState state)
    {
        if (!stateMachine.TrySetState(state))
        {
            Debug.LogError($"Failed to set state.");
        }
        else
        {
            OnStateChangedEvent.Invoke(state);
        }
    }
}
