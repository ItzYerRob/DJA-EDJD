using UnityEngine;

public class callCards : MonoBehaviour
{
    public RoomManager roomManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roomManager.RoomCleared();
    }

}
