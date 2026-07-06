using UnityEngine;

//no funciona perfecto, pero a veces si funciona pedir auxilio a neyra                          
public class EnemySpawner : MonoBehaviour
{
    [Header("Tipos de enemigos posibles")]
    public GameObject[] enemyPrefabs;

    [Header("Puntos de spawn")]  
    public Transform[] spawnPoints;

    [Header("Cantidad de enemigos por oleada")]
    [Range(1, 3)]
    public int[] enemiesPerWave;

    // Spawner Time Manager tiene q manejarse publico xd
    [Header("Time Manager")]
    public float[] spawnTimerSequence;
    private int sequenceIndex = 0;
    private float spawnTimer;

    // Variables that cut Timer of Respawn
    private int spawnRounds = 0;
    private int maxSpawnRounds = 2; 
    private bool canSpawn = true;

    void Start()
    {
        spawnTimer = spawnTimerSequence[0];
        
        /*Debug.Log($"[{gameObject.name}] spawnTimerSequence.Length={spawnTimerSequence.Length}, enemiesPerWave.Length={enemiesPerWave.Length}");
        for (int i = 0; i < spawnTimerSequence.Length; i++)
        {
            Debug.Log($"[{gameObject.name}] Index {i}: time={spawnTimerSequence[i]} | enemies={(i < enemiesPerWave.Length ? enemiesPerWave[i].ToString() : "N/A")}");
        }*/
    }

    void Update()
    {
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
           // Debug.Log($"[{gameObject.name}] Disparando oleada. sequenceIndex={sequenceIndex}, spawnRounds antes={spawnRounds}");


            SpawnEnemy();
            spawnRounds++;

            //Debug.Log($"[{gameObject.name}] spawnRounds despues={spawnRounds}, maxSpawnRounds={maxSpawnRounds}");

            if (spawnRounds >= maxSpawnRounds)
            {
                canSpawn = false;
               // Debug.Log($"[{gameObject.name}] canSpawn puesto en false. Se corta el spawn.");
            }

            sequenceIndex = (sequenceIndex + 1) % spawnTimerSequence.Length;
            spawnTimer = spawnTimerSequence[sequenceIndex];
        }
    }

    private void SpawnEnemy()
    {


        if (spawnPoints.Length == 0 || enemyPrefabs.Length == 0)
        {
            //Debug.LogWarning($"[{gameObject.name}] Faltan spawnPoints o enemyPrefabs");
            return;
        }

        if (enemiesPerWave.Length == 0)
        {
           // Debug.LogWarning($"[{gameObject.name}] Falta configurar enemiesPerWave");
            return;
        }

        int amount = enemiesPerWave[sequenceIndex % enemiesPerWave.Length];
        //Debug.Log($"[{gameObject.name}] Spawneando {amount} enemigos (sequenceIndex={sequenceIndex})");


        for (int i = 0; i < amount; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(prefab, point.position, Quaternion.identity);
        }
    }
}
