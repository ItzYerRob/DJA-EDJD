using UnityEngine;

public class OnTriggerEnterCards : MonoBehaviour
{
    void OnTriggerEnter()
    {
        SingletonGameManager.Instance.roomManager.RoomCleared();
        Destroy(this);
    }
}
