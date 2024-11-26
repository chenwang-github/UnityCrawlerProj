using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public Vector3 offset = new Vector3(0, 5, -15);
    public float smoothSpeed = 0.125f; 
    public float heightOffset = 3.0f;
    public float distance = 15.0f;
    public float angle = 30.0f;

    void LateUpdate()
    {
        Vector3 direction = new Vector3(0, heightOffset, -distance);
        Quaternion rotation = Quaternion.Euler(angle, 0, 0);
        Vector3 targetPosition = target.position + rotation * direction;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        transform.position = smoothedPosition;
        transform.LookAt(target.position + Vector3.up * heightOffset);
    }
}