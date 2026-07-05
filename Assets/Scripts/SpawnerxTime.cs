using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Tipos de enemigos posibles (arrastrá los 3 prefabs)")]
    public GameObject[] enemyPrefabs;

    [Header("Puntos de spawn (las 2 posiciones superpuestas de esta esquina)")]
    public Transform[] spawnPoints;

    [Header("Cantidad de enemigos por oleada")]
    [Range(1, 3)]
    public int enemiesPerSpawn = 1;

    // Spawner Time Manager tiene q manejarse publico xd
    [Header("Time Manager")]
    public float[] spawnTimerSequence;

    private int sequenceIndex = 0;
    private float spawnTimer;

    // Variables that cut Timer of Respawn
    private float elapsedTime = 0f;
    private float stopSpawningAfter = 21f;
    private bool canSpawn = true;

    void Start()
    {
        spawnTimer = spawnTimerSequence[0];
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= stopSpawningAfter)
        {
            canSpawn = false;
        }

        if (canSpawn)
        {
            TimerMechanic();
        }
    }
    private void TimerMechanic()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer < 0)
        {
            SpawnEnemy();

            sequenceIndex = (sequenceIndex + 1) % spawnTimerSequence.Length;
            spawnTimer = spawnTimerSequence[sequenceIndex];
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("Faltan spawnPoints o enemyPrefabs en " + gameObject.name);
            return;
        }

        for (int i = 0; i < enemiesPerSpawn; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(prefab, point.position, Quaternion.identity);
        }
    }
}
