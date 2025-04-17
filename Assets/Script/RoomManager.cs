using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public delegate void RoomClearedDelegate();
    public event RoomClearedDelegate OnRoomCleared;
    
    public void RoomCleared()
    {
        OnRoomCleared?.Invoke();
    }
}
