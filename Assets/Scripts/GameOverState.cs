using UnityEngine;

public class GameOverState : GameState
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.SetState(GameManager.EGameState.Title);
        }
    }
}
