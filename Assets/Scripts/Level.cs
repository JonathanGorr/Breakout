using UnityEngine;
using UnityEngine.Events;

public class Level : MonoBehaviour
{
    public GameObject root;
    private Cube[] cubes;

    public UnityEvent LevelCompletedEvent { get; } = new UnityEvent();

    public Color borderColor;
    public Color backgroundColor;
    public Color paddleColor;
    public Color cameraBackgroundColor;

    public SpriteRenderer backgroundRenderer;
    public MeshRenderer borderRenderer;
    public MeshRenderer paddleRenderer;

    private void Awake()
    {
        cubes = root.GetComponentsInChildren<Cube>(true);
        Disable();
    }

    public bool IsCompleted
    {
        get
        {
            for (int i = 0; i < cubes.Length; ++i)
            {
                if (cubes[i].State != Health.EHealthState.Dead)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public void Disable()
    {
        for (int i = 0; i < cubes.Length; ++i)
        {
            cubes[i].health.OnDeath.RemoveListener(OnDeath);
        }
        root.SetActive(false);
    }

    public void Enable()
    {
        backgroundRenderer.color = backgroundColor;
        borderRenderer.material.color = borderColor;
        Camera.main.backgroundColor = cameraBackgroundColor;
        paddleRenderer.material.color = paddleColor;

        root.SetActive(true);

        for (int i = 0; i < cubes.Length; ++i)
        {
            cubes[i].health.OnDeath.AddListener(OnDeath);
            cubes[i].Resurrect();
        }
    }

    private void OnDeath()
    {
        if (IsCompleted)
        {
            LevelCompletedEvent.Invoke();
        }
    }
}
