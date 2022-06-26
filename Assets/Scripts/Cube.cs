using UnityEngine;
using UnityEngine.Events;
using static Health;

public interface IWorthPoints
{
    int PointValue { get; }
}

public class Cube : CubeBase, IWorthPoints
{
    public Collider collider;

    public Health health;
    public int PointValue => health.StartHealth;

    public static UnityEvent<int> OnDeathEvent = new UnityEvent<int>();
    public EHealthState State => health.State;

    private void Awake()
    {
        health.OnDeath.AddListener(OnDeath);
    }

    public void Resurrect()
    {
        health.Resurrect();
        collider.enabled = true;
    }

    public void Kill()
    {
        health.Kill(false);
    }

    private void OnDeath()
    {
        Cube.OnDeathEvent.Invoke(PointValue);
        collider.enabled = false;
    }

    private void OnDestroy()
    {
        health.OnDeath.RemoveListener(OnDeath);
    }
}