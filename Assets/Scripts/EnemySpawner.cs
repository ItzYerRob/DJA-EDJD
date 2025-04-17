using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public Terrain terrain;                        //Also Optional
    public GameObject enemyPrefab;
    public Transform player;
    public int maxEnemies = 10;
    public float respawnInterval = 5f;

    [Header("Spawn Radius (for Terrain Spawning)")]
    public float minSpawnRadius = 10f;
    public float maxSpawnRadius = 30f;

    [Header("Optional Fixed Spawn Points")]
    public Transform[] spawnPoints;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= respawnInterval && spawnedEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
            timer = 0f;
        }

        spawnedEnemies.RemoveAll(enemy => enemy == null);
    }

    void SpawnEnemy()
    {
        Vector3 spawnPos;

        if (spawnPoints.Length > 0 && Random.value > 0.5f)
        {
            Transform chosenPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            spawnPos = chosenPoint.position;
        }
        else if (terrain != null)
        {
            spawnPos = GetTerrainSpawnPosition();
        }
        else if (spawnPoints.Length > 0)
        {
            Transform chosenPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            spawnPos = chosenPoint.position;
        }
        else
        {
            Debug.LogWarning("EnemySpawner has no valid spawn method!");
            return;
        }

        //Ensure spawnPos is on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 5f, NavMesh.AllAreas))
        {
            spawnPos = hit.position;
        }
        else
        {
            Debug.LogWarning($"No valid NavMesh near spawn position at {spawnPos}");
            return; //Skip spawn if no valid navmesh found
        }

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        newEnemy.GetComponent<EnemyAttack>().Init(player);

        spawnedEnemies.Add(newEnemy);
    }

    Vector3 GetTerrainSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minSpawnRadius, maxSpawnRadius);

        Vector3 spawnPos = new Vector3(
            player.position.x + randomCircle.x,
            0f,
            player.position.z + randomCircle.y
        );

        Vector3 terrainPos = terrain.transform.position;
        float terrainWidth = terrain.terrainData.size.x;
        float terrainLength = terrain.terrainData.size.z;

        spawnPos.x = Mathf.Clamp(spawnPos.x, terrainPos.x, terrainPos.x + terrainWidth);
        spawnPos.z = Mathf.Clamp(spawnPos.z, terrainPos.z, terrainPos.z + terrainLength);
        spawnPos.y = terrain.SampleHeight(spawnPos) + terrainPos.y;

        return spawnPos;
    }
}
