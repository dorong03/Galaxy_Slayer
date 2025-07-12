using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SpawnerConfig", menuName = "Spawning/Spawner Config")]
public class SpawnerConfig : ScriptableObject
{
    public Vector3 spawnerPosition;
    public Vector3 spawnerSize;
    public int minSpawn;
    public int maxSpawn;

    public List<UnitEntry> unitList;
}

[System.Serializable]
public class UnitEntry
{
    public GameObject prefab;
    [Range(0f, 1f)]
    public float spawnChance;
}