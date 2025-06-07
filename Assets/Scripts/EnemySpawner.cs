using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings (Terrain / Fixed)")]
    public Terrain terrain;
    public GameObject enemyPrefab;
    public Transform player;
    public int maxEnemies = 10;
    public float respawnInterval = 5f;

    [Header("Spawn Radius (for Terrain Spawning)")]
    public float minSpawnRadius = 10f;
    public float maxSpawnRadius = 30f;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private float timer = 0f;

    void Update()
    {
        //Keep cleaning up destroyed enemies
        spawnedEnemies.RemoveAll(e => e == null);

        timer += Time.deltaTime;
        if (timer >= respawnInterval && spawnedEnemies.Count < maxEnemies)
        {
            SpawnOutdoorOrFixed();
            timer = 0f;
        }
    }

    private void SpawnOutdoorOrFixed()
    {
        Vector3 spawnPos;
        //terrain‐spawn (if terrain is assigned)
        if (terrain != null)
        {
            spawnPos = GetTerrainSpawnPosition();
        }
        else
        {
            Debug.LogWarning("EnemySpawner: No fixed points or terrain to spawn outdoors.");
            return;
        }

        //Ensure that the chosen position is on the NavMesh
        if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            spawnPos = hit.position;
        }
        else
        {
            Debug.LogWarning($"EnemySpawner: Couldn't find NavMesh near {spawnPos}. Skipping spawn.");
            return;
        }

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        newEnemy.GetComponent<EnemyAttack>().Init(player);
        spawnedEnemies.Add(newEnemy);
    }

    private Vector3 GetTerrainSpawnPosition()
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