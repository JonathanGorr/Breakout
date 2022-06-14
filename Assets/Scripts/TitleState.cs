using UnityEngine;

public class TitleState : GameState
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.SetState(GameManager.EGameState.GameLoop);
        }
    }
}
