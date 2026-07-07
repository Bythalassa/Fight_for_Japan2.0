using UnityEngine;

public class SpawnerxEvento : MonoBehaviour
{
    [Header("Tipos de enemigos posibles")]
    public GameObject[] enemyPrefabs;

    [Header("Puntos de spawn")]
    public Transform[] spawnPoints;

    [Header("Cantidad maxima de rondas de spawn")]
    public int maxSpawnRounds = 2;

    private int spawnRounds = 0;
    private bool canSpawn = true;

    public float timeToRespawnSE = 5f;
    private float timeRespawnSE = 0f;

    void Update()
    {
        if (!canSpawn) return;

        // Cooldown: mientras no llegue a 0, no chequea nada más
        if (timeRespawnSE > 0)
        {
            timeRespawnSE -= Time.deltaTime;
            return;
        }

        HeallthManager[] enemies = FindObjectsByType<HeallthManager>(FindObjectsSortMode.None);

        foreach (HeallthManager enemy in enemies)
        {
            if (enemy.isDead)
            {
                enemy.isDead = false; // reset 
                spawnRounds++;
                SpawnEnemy();

                timeRespawnSE = timeToRespawnSE; // arranca el cooldown después d 1 Spawn

                if (spawnRounds >= maxSpawnRounds)
                {
                    canSpawn = false;
                }

                break; 
            }
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] Faltan spawnPoints o enemyPrefabs");
            return;
        }

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Instantiate(prefab, point.position, Quaternion.identity);
    }
}