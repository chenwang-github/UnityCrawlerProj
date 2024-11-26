using UnityEngine;

public class AlignCubeWithSlope : MonoBehaviour
{
    public LayerMask groundLayer;
    public float hoverHeight = 1.0f;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            transform.position = hit.point + Vector3.up * hoverHeight;
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            transform.rotation = targetRotation;
        }
        Debug.DrawRay(transform.position, Vector3.down * 10, Color.red);
        Debug.DrawRay(hit.point, hit.normal * 2, Color.green);
    }
}
