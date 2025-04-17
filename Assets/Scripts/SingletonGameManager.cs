using UnityEngine;

public class SingletonGameManager : MonoBehaviour
{
    public static SingletonGameManager Instance;

    public int enemiesDefeated = 0, enemiesDefeatedThisRound = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //Persist across scenes
        }
        else
        {
            Destroy(gameObject); //Prevent duplicates
        }
    }

    public void EnemyDefeated()
    {
        enemiesDefeated++;
        enemiesDefeatedThisRound++;
        // Debug.Log("Enemies defeated: " + enemiesDefeated);
    }

    public void clearRoundStats() {
        enemiesDefeatedThisRound = 0;
    }
}
