using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform ballHolder;
    private Ball ball;

    private void Start()
    {
        GameManager.Instance.GameState.SpawnBallEvent.AddListener(OnSpawnBall);
        GameManager.Instance.GameState.DestroyBallsEvent.AddListener(OnDestroyBall);
    }

    private void OnDestroy()
    {
        GameManager.Instance?.GameState?.SpawnBallEvent.RemoveListener(OnSpawnBall);
        GameManager.Instance?.GameState?.DestroyBallsEvent.RemoveListener(OnDestroyBall);
    }

    private void OnSpawnBall()
    {
        Spawn();
    }

    private void OnDestroyBall()
    {
        if (ball)
        {
            Destroy(ball.gameObject);
        }
        ball = null;
    }

    public void Spawn()
    {
        if (ball != null)
            return;

        var go = Instantiate(ballPrefab, ballHolder);
        go.SetActive(true);
        go.transform.position = ballHolder.position;
        go.transform.rotation = Quaternion.identity;
        ball = go.GetComponent<Ball>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ball)
            {
                Vector3 randomVector = GetComponent<PaddleController>().GetRandomReflectionVector();
                ball.transform.SetParent(null);
                ball.Send(randomVector);
                ball = null;
            }
        }
    }
}
