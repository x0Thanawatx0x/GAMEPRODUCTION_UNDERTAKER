using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LavaDropSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject lavaDropPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints; // จุดที่ lava จะเกิด

    [Header("Spawn Settings")]
    public float spawnInterval = 2f;
    public float randomDelay = 1f;

    [Header("Spawn Count")]
    public int minSpawnCount = 1;
    public int maxSpawnCount = 3;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float delay = spawnInterval + Random.Range(-randomDelay, randomDelay);
            yield return new WaitForSeconds(delay);

            SpawnLava();
        }
    }

    void SpawnLava()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        // สุ่มจำนวนที่จะ spawn
        int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);

        // กันไม่ให้เกินจำนวนจุดจริง
        spawnCount = Mathf.Min(spawnCount, spawnPoints.Length);

        List<int> usedIndex = new List<int>();

        for (int i = 0; i < spawnCount; i++)
        {
            int index;

            // สุ่มแบบไม่ให้ซ้ำ
            do
            {
                index = Random.Range(0, spawnPoints.Length);
            }
            while (usedIndex.Contains(index));

            usedIndex.Add(index);

            Transform spawnPoint = spawnPoints[index];
            Instantiate(lavaDropPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}