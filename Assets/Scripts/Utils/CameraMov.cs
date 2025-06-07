using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0f, 2f, -4f);
    public float sensitivity = 5f;
    public float minY = -60f;
    public float maxY = 80f;
    public float smoothSpeed = 10f;
    public float collisionRadius = 0.3f;
    public float collisionBuffer = 0.2f;
    public LayerMask collisionLayers;

    private float currentRotationX = 0f;

    void LateUpdate()
    {
        //Handle vertical rotation (mouse Y)
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        currentRotationX -= mouseY;
        currentRotationX = Mathf.Clamp(currentRotationX, minY, maxY);

        //Calculate rotation and desired camera direction
        Quaternion rotation = Quaternion.Euler(currentRotationX, player.eulerAngles.y, 0f);
        Vector3 targetOffset = rotation * offset;
        Vector3 desiredPosition = player.position + targetOffset;

        //SphereCast from player to desired position to check for walls
        Vector3 direction = targetOffset.normalized;
        float targetDistance = targetOffset.magnitude;
        RaycastHit hit;

        float adjustedDistance = targetDistance;

        if (Physics.SphereCast(player.position, collisionRadius, direction, out hit, targetDistance + collisionBuffer, collisionLayers))
        {
            adjustedDistance = Mathf.Clamp(hit.distance - collisionBuffer, 0.5f, targetDistance);
        }

        Vector3 finalPosition = player.position + direction * adjustedDistance;

        //Smooth camera transition
        transform.position = Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * smoothSpeed);
        transform.rotation = rotation;
    }
}
