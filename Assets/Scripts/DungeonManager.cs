using UnityEngine;
using System.Collections.Generic;
using Unity.AI.Navigation;

public class DungeonManager : MonoBehaviour
{
    public List<GameObject> roomPrefabs;

    [Tooltip("Offset to apply between connected rooms.")]
    public Vector3 roomOffset = Vector3.zero;

    [Tooltip("NavMeshSurface that should bake the entire dungeon.")]
    public NavMeshSurface navMeshSurface;

    private Transform lastExitPoint;

    void Start()
    {
        //Spawn the first room at origin
        SpawnRoom(roomPrefabs[0], Vector3.zero);

        //Spawn the rest, each aligned to the previous ExitPoint
        for (int i = 1; i < roomPrefabs.Count; i++)
        {
            Vector3 spawnPos = lastExitPoint.position - GetEntryPoint(roomPrefabs[i]).localPosition + roomOffset;
            SpawnRoom(roomPrefabs[i], spawnPos);
        }
        
        navMeshSurface.layerMask = 1 << LayerMask.NameToLayer("Obstacles");
    }

    private void SpawnRoom(GameObject prefab, Vector3 worldPosition)
    {
        GameObject room = Instantiate(prefab, worldPosition, Quaternion.identity, transform);

        //Find exit & entry in the just‐spawned room
        Transform entry = room.transform.Find("EntryPoint");
        Transform exit = room.transform.Find("ExitPoint");
        if (entry == null || exit == null)
        {
            Debug.LogError($"Room '{prefab.name}' is missing EntryPoint or ExitPoint child.");
            return;
        }

        //If we have a lastExitPoint, hook its TeleportTrigger to this room's entry
        if (lastExitPoint != null)
        {
            var trigger = lastExitPoint.gameObject.AddComponent<TeleportTrigger>();
            trigger.target = entry;
        }

        lastExitPoint = exit;
        
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
    }

    //Get the EntryPoint transform *in* the prefab asset
    private Transform GetEntryPoint(GameObject prefab)
    {
        var e = prefab.transform.Find("EntryPoint");
        if (e == null) throw new System.Exception($"Prefab {prefab.name} missing EntryPoint");
        return e;
    }
}
