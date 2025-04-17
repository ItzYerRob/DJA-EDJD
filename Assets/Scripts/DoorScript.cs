using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    private TextMeshPro enemiesDefeatedText;

    private void Start()
    {
        enemiesDefeatedText = GetComponentInChildren<TextMeshPro>();
    }

    private void LateUpdate()
    {
        if (SingletonGameManager.Instance != null && enemiesDefeatedText != null)
        {
            enemiesDefeatedText.text = "Enemies Defeated: " + SingletonGameManager.Instance.enemiesDefeatedThisRound.ToString() + "/15";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && SingletonGameManager.Instance.enemiesDefeated >= 12)
        {
            SceneManager.LoadSceneAsync("Level2");
        }
    }
}
