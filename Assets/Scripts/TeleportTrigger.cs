using TMPro;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    [Tooltip("Set this to the EntryPoint Transform of the next room.")]
    public Transform target;
    private TextMeshPro enemiesDefeatedText;
    int spawnCount = 0;

    private void Start()
    {
        enemiesDefeatedText = GetComponentInChildren<TextMeshPro>();

        Transform roomTransform = transform.parent;
        if (roomTransform == null)
        {
            Debug.LogError("TeleportTrigger: Could not find parent room for ExitPoint.");
            return;
        }

        foreach (Transform t in roomTransform.GetComponentsInChildren<Transform>())
        {
            if (t.name == "SpawnPoint")
                spawnCount++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Only care about the player entering the exit‐trigger
        if (!other.CompareTag("Player"))
            return;

        //Compare against enemiesDefeatedThisRound
        int killedThisRound = SingletonGameManager.Instance.enemiesDefeatedThisRound;
        if (killedThisRound < spawnCount)
        {
            //Not enough enemies defeated yet
            Debug.Log($"You must defeat all {spawnCount} enemies before proceeding! (Defeated: {killedThisRound})");
            return;
        }

        other.transform.position = target.position;

        // SingletonGameManager.Instance.roomManager.RoomCleared();

        SingletonGameManager.Instance.clearRoundStats();
    }
    
    private void LateUpdate()
    {
        if (SingletonGameManager.Instance != null && enemiesDefeatedText != null)
        {
            enemiesDefeatedText.text = "Enemies Defeated: " + SingletonGameManager.Instance.enemiesDefeatedThisRound.ToString() + "/" + spawnCount;
        }
    }
}
