using UnityEngine;

public class BodyPositionStabilizer : MonoBehaviour
{

    public LayerMask groundLayer; 
    public float hoverHeight = 1.0f; 
    public float heightThreshold = 0.05f; 
    public float rotationDampingFactor = 2.0f; 
    private float lastTargetHeight; 
    private bool initialized = false; 

    void Update()
    {
        Vector3 averagePosition = Vector3.zero;
        int validCorners = 0;

        Vector3 frontLeftCorner = transform.position + transform.rotation * new Vector3(-0.5f * transform.localScale.x, 0, 0.5f * transform.localScale.z);
        Vector3 frontRightCorner = transform.position + transform.rotation * new Vector3(0.5f * transform.localScale.x, 0, 0.5f * transform.localScale.z);
        Vector3 backLeftCorner = transform.position + transform.rotation * new Vector3(-0.5f * transform.localScale.x, 0, -0.5f * transform.localScale.z);
        Vector3 backRightCorner = transform.position + transform.rotation * new Vector3(0.5f * transform.localScale.x, 0, -0.5f * transform.localScale.z);

        Vector3[] corners = { frontLeftCorner, frontRightCorner, backLeftCorner, backRightCorner };
        foreach (Vector3 corner in corners)
        {
            RaycastHit hit;
            if (Physics.Raycast(corner, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                averagePosition += hit.point;
                validCorners++;
                Debug.DrawRay(corner, Vector3.down * 10, Color.green); 
            }
        }

        if (validCorners > 0)
        {
            averagePosition /= validCorners;
            float targetHeight = averagePosition.y + hoverHeight;

            if (!initialized)
            {
                lastTargetHeight = targetHeight;
                initialized = true;
            }

            float heightDifference = Mathf.Abs(targetHeight - lastTargetHeight);
            if (heightDifference > heightThreshold)
            {
                lastTargetHeight = targetHeight;
            }
            else
            {
                targetHeight = Mathf.Lerp(lastTargetHeight, targetHeight, heightDifference / heightThreshold);
            }

            Vector3 targetPosition = new Vector3(transform.position.x, targetHeight, transform.position.z);
            transform.position = targetPosition;

            Vector3 averageNormal = Vector3.zero;
            foreach (Vector3 corner in corners)
            {
                RaycastHit hit;
                if (Physics.Raycast(corner, Vector3.down, out hit, Mathf.Infinity, groundLayer))
                {
                    averageNormal += hit.normal;
                }
            }
            averageNormal.Normalize();

            Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, averageNormal).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(projectedForward, Vector3.up);

            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, transform.eulerAngles.y, targetRotation.eulerAngles.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationDampingFactor);
        }
    }
}
