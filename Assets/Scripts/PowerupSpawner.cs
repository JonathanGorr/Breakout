using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    public GameObject powerupPrefab;

    public void SpawnPowerup()
    {
        var go = Instantiate(powerupPrefab);
        go.transform.position = transform.position;
    }
}
