using UnityEngine;

public class SpawnerxTrigger : MonoBehaviour

{
    [Header("Tipos de enemigos posibles")]
    public GameObject[] enemyPrefabs;

    [Header("Puntos de spawn")]
    public Transform[] spawnPoints;

    [Header("Cantidad de enemigos por oleada")]
    [Range(1, 3)]
    public int[] enemiesPerWave;
    
    // Variables que cortan el spawn por cantidad de colisiones
    private int sequenceIndex = 0;
    public void SpawnFromTrigger()
    {
        SpawnEnemy();
        sequenceIndex = (sequenceIndex + 1) % enemiesPerWave.Length;
    }

    private void SpawnEnemy()
    {

        //if (amount < 1) amount = 1
        if (spawnPoints.Length == 0 || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] Faltan spawnPoints o enemyPrefabs");
            return;
        }

        if (enemiesPerWave.Length == 0)
        {
            Debug.LogWarning($"[{gameObject.name}] Falta configurar enemiesPerWave");
            return;
        }
        
        int amount = enemiesPerWave[sequenceIndex % enemiesPerWave.Length];
        if (amount < 1) amount = 1;

        Debug.Log($"[{gameObject.name}] Spawned {amount} enemies from trigger.");

        for (int i = 0; i < amount; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(prefab, point.position, Quaternion.identity);
        }
    }
}


