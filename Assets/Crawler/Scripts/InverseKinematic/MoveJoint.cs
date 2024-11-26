using UnityEngine;

public class SpiderLegMovement : MonoBehaviour
{
    public Transform cubeA;      // Reference to Cube A
    public Transform cubeB;      // Reference to Cube B
    public float maxDistance = 5.0f;  // Distance threshold to start moving
    public float moveSpeed = 2.0f;    // Horizontal speed towards Cube B
    public float liftHeight = 2.0f;   // Height to lift Cube A before moving
    public float liftSpeed = 1.0f;    // Speed of lifting up and down

    private enum MovementState { Idle, Lifting, Moving, Lowering }
    private MovementState state = MovementState.Idle;

    void Update()
    {
        // Calculate distance between Cube A and Cube B
        float distance = Vector3.Distance(new Vector3(cubeA.position.x, 0, cubeA.position.z), new Vector3(cubeB.position.x, 0, cubeB.position.z));

        switch (state)
        {
            case MovementState.Idle:
                // Start moving if distance exceeds threshold
                if (distance > maxDistance)
                {
                    state = MovementState.Lifting;
                }
                break;

            case MovementState.Lifting:
                // Lift Cube A up to the specified lift height
                cubeA.position = Vector3.MoveTowards(cubeA.position, new Vector3(cubeA.position.x, liftHeight, cubeA.position.z), liftSpeed * Time.deltaTime);

                // Transition to Moving state when lift height is reached
                if (Mathf.Abs(cubeA.position.y - liftHeight) < 0.01f)
                {
                    state = MovementState.Moving;
                }
                break;

            case MovementState.Moving:
                // Move Cube A horizontally towards Cube B
                Vector3 targetPosition = new Vector3(cubeB.position.x, cubeA.position.y, cubeB.position.z);
                cubeA.position = Vector3.MoveTowards(cubeA.position, targetPosition, moveSpeed * Time.deltaTime);

                // Transition to Lowering state when close to Cube B horizontally
                if (Vector3.Distance(new Vector3(cubeA.position.x, 0, cubeA.position.z), new Vector3(cubeB.position.x, 0, cubeB.position.z)) < 0.1f)
                {
                    state = MovementState.Lowering;
                }
                break;

            case MovementState.Lowering:
                // Lower Cube A to Cube B's height
                cubeA.position = Vector3.MoveTowards(cubeA.position, new Vector3(cubeA.position.x, cubeB.position.y, cubeA.position.z), liftSpeed * Time.deltaTime);

                // Return to Idle state once at Cube B's height
                if (Mathf.Abs(cubeA.position.y - cubeB.position.y) < 0.01f)
                {
                    state = MovementState.Idle;
                }
                break;
        }
    }
}
