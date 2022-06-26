using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;

public class UIGameStateController : MonoBehaviour
{
    public StateCanvas[] states;

    [System.Serializable]
    public class StateCanvas
    {
        public CanvasGroup cg;
        public EGameState gameState;
    }

    private Dictionary<EGameState, StateCanvas> map = new Dictionary<EGameState, StateCanvas>();

    private void Awake()
    {
        map = states.ToDictionary(x=>x.gameState, y=>y);
    }

    private void Start()
    {
        GameManager.Instance.OnStateChangedEvent.AddListener(OnStateChange);
        OnStateChange(GameManager.Instance.CurrentState);
    }

    private void OnDestroy()
    {
        GameManager.Instance?.OnStateChangedEvent.RemoveListener(OnStateChange);
    }

    private void OnStateChange(GameState newState)
    {
        foreach (KeyValuePair<EGameState, StateCanvas> state in map)
        {
            bool enable = newState.State == state.Key;
            state.Value.cg.alpha = enable ? 1 : 0;
            state.Value.cg.interactable = enable;
            state.Value.cg.blocksRaycasts = enable;
        }
    }
}
