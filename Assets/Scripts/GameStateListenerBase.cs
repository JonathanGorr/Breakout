using UnityEngine;
using UnityEngine.Events;
using static GameManager;

public abstract class GameStateListenerBase : MonoBehaviour
{
    private void Start()
    {
		GameManager.Instance.OnStateChangedEvent.AddListener(OnGameStateChanged);
		OnGameStateChanged(GameManager.Instance.CurrentState);
	}

    private void OnDestroy()
	{
		GameManager.Instance.OnStateChangedEvent.RemoveListener(OnGameStateChanged);
	}

	protected abstract void OnGameStateChanged(GameState currentState);
}

public class GameStateListener : GameStateListenerBase
{
	[SerializeField] private EGameState _actionState = EGameState.Title;
	private EGameState _lastState = EGameState.Title;

	[SerializeField] private UnityEvent OnStateEnter = new UnityEvent();
	[SerializeField] private UnityEvent OnStateExit = new UnityEvent();

	private void Awake()
	{
		_lastState = _actionState;
	}

	protected override void OnGameStateChanged(GameState currentState)
	{
		//if we entered our action state
		if (_actionState == currentState.State)
		{
			OnStateEnter.Invoke();
		}
		else
		{
			//if we left our action state
			if (_lastState == _actionState)
			{
				OnStateExit.Invoke();
			}
		}

		//update
		_lastState = currentState.State;
	}
}
