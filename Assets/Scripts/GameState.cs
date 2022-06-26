using UnityEngine;
using UnityEngine.Events;
using static GameManager;

public abstract class GameState : StateBehaviour
{
    [SerializeField] private EGameState _state;
    public EGameState State => _state;

    public UnityEvent StateEnterEvent = new UnityEvent();
    public UnityEvent StateExitEvent = new UnityEvent();

    public override void OnEnterState()
    {
        base.OnEnterState();
        StateEnterEvent.Invoke();
    }

    public override void OnExitState()
    {
        base.OnExitState();
        StateExitEvent.Invoke();
    }
}