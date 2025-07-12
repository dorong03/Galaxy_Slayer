using UnityEngine;
using System.Collections;

public class ErrorSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform player;
    public float safeDistanceFromPlayer = 3f;
    public float checkRadius = 0.5f;
    public LayerMask obstacleMask;
    public int maxAttempts = 10;

    public SpawnerConfig[] configs;

    private Coroutine spawnCoroutine;

    private void Start()
    {
        Invoke("StartSpawnLoop",1.5f);
    }

    void StartSpawnLoop()
    {
        StartCoroutine(SpawnLoop());
    }
    
    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            int unlocked = GameManager.Instance.unLockedAreaCount;
            float interval = Mathf.Clamp(8f - unlocked, 5f, 8f);
            if (UIManager.Instance.FadeOutEnd)
            {
                SpawnEnemies();
            }
            yield return new WaitForSeconds(interval);
        }
    }

    public void SpawnEnemies()
    {
        gameObject.transform.position = configs[GameManager.Instance.unLockedAreaCount].spawnerPosition;
        SpawnerConfig config = configs[GameManager.Instance.unLockedAreaCount];
        int spawnCount = Random.Range(config.minSpawn, config.maxSpawn + 1);

        int spawned = 0;
        int attempts = 0;

        while (spawned < spawnCount && attempts < maxAttempts * spawnCount)
        {
            Vector3 spawnPosition = GetRandomPositionInArea(config);

            if (Vector2.Distance(spawnPosition, player.position) < safeDistanceFromPlayer ||
                Physics2D.OverlapCircle(spawnPosition, checkRadius, obstacleMask))
            {
                attempts++;
                continue;
            }

            GameObject prefabToSpawn = GetRandomPrefabFromConfig(config);
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

            spawned++;
        }
    }

    private Vector3 GetRandomPositionInArea(SpawnerConfig config)
    {
        float halfX = config.spawnerSize.x / 2f;
        float halfY = config.spawnerSize.y / 2f;

        float x = Random.Range(-halfX, halfX);
        float y = Random.Range(-halfY, halfY);

        Vector3 localPos = new Vector3(x, y, 0);
        return transform.position + localPos;
    }

    private GameObject GetRandomPrefabFromConfig(SpawnerConfig config)
    {
        float rand = Random.value;
        float cumulative = 0f;

        foreach (var unit in config.unitList)
        {
            cumulative += unit.spawnChance;
            if (rand <= cumulative)
                return unit.prefab;
        }

        return config.unitList[config.unitList.Count - 1].prefab;
    }

    private void OnDrawGizmosSelected()
    {
        if (GameManager.Instance == null || configs == null || configs.Length == 0)
            return;

        int index = Mathf.Clamp(GameManager.Instance.unLockedAreaCount, 0, configs.Length - 1);

        if (configs[index] == null)
            return;

        SpawnerConfig config = configs[index];

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(config.spawnerPosition, new Vector3(config.spawnerSize.x, config.spawnerSize.y, 0f));
    }

}
