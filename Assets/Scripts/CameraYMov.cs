using UnityEngine;

public class CameraYMov : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate player based on mouse input
        float mouseY = Input.GetAxis("Mouse Y");
        Vector3 rotation = new Vector3(mouseY * -1, 0f, 0f);
        transform.Rotate(rotation);
    }
}
