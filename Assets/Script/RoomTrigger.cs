using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public RoomManager roomManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            roomManager.RoomCleared();
        }
    }
}
