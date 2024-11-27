using UnityEngine;

public class SpiderLegMovement : MonoBehaviour
{
    public Transform cubeA;     
    public Transform cubeB;     
    public float maxDistance = 5.0f;
    public float moveSpeed = 2.0f;
    public float liftHeight = 2.0f;
    public float liftSpeed = 1.0f;

    private enum MovementState { Idle, Lifting, Moving, Lowering }
    private MovementState state = MovementState.Idle;

    void Update()
    {
        float distance = Vector3.Distance(new Vector3(cubeA.position.x, 0, cubeA.position.z), new Vector3(cubeB.position.x, 0, cubeB.position.z));

        switch (state)
        {
            case MovementState.Idle:
                if (distance > maxDistance)
                {
                    state = MovementState.Lifting;
                }
                break;

            case MovementState.Lifting:
                cubeA.position = Vector3.MoveTowards(cubeA.position, new Vector3(cubeA.position.x, liftHeight, cubeA.position.z), liftSpeed * Time.deltaTime);

                if (Mathf.Abs(cubeA.position.y - liftHeight) < 0.01f)
                {
                    state = MovementState.Moving;
                }
                break;

            case MovementState.Moving:
                Vector3 targetPosition = new Vector3(cubeB.position.x, cubeA.position.y, cubeB.position.z);
                cubeA.position = Vector3.MoveTowards(cubeA.position, targetPosition, moveSpeed * Time.deltaTime);

                if (Vector3.Distance(new Vector3(cubeA.position.x, 0, cubeA.position.z), new Vector3(cubeB.position.x, 0, cubeB.position.z)) < 0.1f)
                {
                    state = MovementState.Lowering;
                }
                break;

            case MovementState.Lowering:
                cubeA.position = Vector3.MoveTowards(cubeA.position, new Vector3(cubeA.position.x, cubeB.position.y, cubeA.position.z), liftSpeed * Time.deltaTime);

                if (Mathf.Abs(cubeA.position.y - cubeB.position.y) < 0.01f)
                {
                    state = MovementState.Idle;
                }
                break;
        }
    }
}
