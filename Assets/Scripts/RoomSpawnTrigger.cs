using UnityEngine;

public class RoomSpawnTrigger : MonoBehaviour
{
    [Tooltip("Prefab to spawn at each SpawnPoint.")]
    public GameObject prefabToSpawn;

    [Tooltip("Should we only trigger once?")]
    public bool triggerOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnce) return;

        if (other.CompareTag("Player"))
        {
            Transform roomRoot = transform.parent;

            if (roomRoot == null)
            {
                Debug.LogWarning("RoomSpawnTrigger: No parent room found.");
                return;
            }

            foreach (Transform child in roomRoot.GetComponentsInChildren<Transform>())
            {
                if (child.name == "SpawnPoint")
                {
                    Instantiate(prefabToSpawn, child.position, Quaternion.identity);
                }
            }

            hasTriggered = true;
        }
    }
}
