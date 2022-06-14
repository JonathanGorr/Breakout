using UnityEngine;

public interface IBackboard
{
}

public class Backboard : MonoBehaviour, IBackboard
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IHealth>(out IHealth health))
        {
            health.DealDamage(100);
        }
    }
}
