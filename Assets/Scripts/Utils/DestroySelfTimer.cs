using UnityEngine;

public class DestroySelfTimer : MonoBehaviour
{
    public float destroyTime = 1f;
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
