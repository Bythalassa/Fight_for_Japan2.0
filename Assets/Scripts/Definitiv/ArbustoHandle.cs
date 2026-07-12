using UnityEngine;

public class ArbustoHandle : MonoBehaviour
{
    [Header("Spawner")]
    public SpawnerxTrigger enemySpawner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger.");

            if (enemySpawner != null)
            {
                enemySpawner.SpawnFromTrigger();
            }
        }
    }
}
