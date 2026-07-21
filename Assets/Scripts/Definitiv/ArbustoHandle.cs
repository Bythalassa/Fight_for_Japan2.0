using UnityEngine;

public class ArbustoHandle : MonoBehaviour
{
    [Header("Spawner")]
    public SpawnerxTrigger enemySpawner;
    public Health scriptHealth;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        scriptHealth.destroyOnDeath = true;
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
