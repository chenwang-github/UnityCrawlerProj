using UnityEngine;

public class InverseKinematics : MonoBehaviour
{
    public int ChainLength = 2;
    public Transform Target;
    public int Iterations = 10;
    public float Delta = 0.001f;
    [Range(0, 1)]
    public float SnapStrength = 1f;
    private float[] SegmentLengths;
    private float TotalLength;
    private Transform[] Joints;
    private Vector3[] Positions;
    private Vector3[] InitialDirections;
    private Quaternion[] InitialRotations;
    private Quaternion InitialTargetRotation;
    private Transform RootTransform;

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        Joints = new Transform[ChainLength + 1];
        Positions = new Vector3[ChainLength + 1];
        SegmentLengths = new float[ChainLength];
        InitialDirections = new Vector3[ChainLength + 1];
        InitialRotations = new Quaternion[ChainLength + 1];

        RootTransform = transform;
        for (int i = 0; i <= ChainLength; i++)
        {
            if (RootTransform == null)
                throw new UnityException("Chain length exceeds hierarchy!");
            RootTransform = RootTransform.parent;
        }

        if (Target == null)
        {
            Target = new GameObject(gameObject.name + " Target").transform;
            SetPositionRelativeToRoot(Target, GetPositionRelativeToRoot(transform));
        }
        InitialTargetRotation = GetRotationRelativeToRoot(Target);

        var currentTransform = transform;
        TotalLength = 0;
        for (int i = Joints.Length - 1; i >= 0; i--)
        {
            Joints[i] = currentTransform;
            InitialRotations[i] = GetRotationRelativeToRoot(currentTransform);

            if (i == Joints.Length - 1)
            {
                InitialDirections[i] = GetPositionRelativeToRoot(Target) - GetPositionRelativeToRoot(currentTransform);
            }
            else
            {
                InitialDirections[i] = GetPositionRelativeToRoot(Joints[i + 1]) - GetPositionRelativeToRoot(currentTransform);
                SegmentLengths[i] = InitialDirections[i].magnitude;
                TotalLength += SegmentLengths[i];
            }

            currentTransform = currentTransform.parent;
        }
    }

    void LateUpdate()
    {
        PerformIK();
    }

    private void PerformIK()
    {
        if (Target == null)
            return;

        if (SegmentLengths.Length != ChainLength)
            Initialize();

        for (int i = 0; i < Joints.Length; i++)
            Positions[i] = GetPositionRelativeToRoot(Joints[i]);

        var desiredPosition = GetPositionRelativeToRoot(Target);
        var desiredRotation = GetRotationRelativeToRoot(Target);

        if ((desiredPosition - GetPositionRelativeToRoot(Joints[0])).sqrMagnitude >= TotalLength * TotalLength)
        {
            var direction = (desiredPosition - Positions[0]).normalized;
            for (int i = 1; i < Positions.Length; i++)
                Positions[i] = Positions[i - 1] + direction * SegmentLengths[i - 1];
        }
        else
        {
            for (int i = 0; i < Positions.Length - 1; i++)
                Positions[i + 1] = Vector3.Lerp(Positions[i + 1], Positions[i] + InitialDirections[i], SnapStrength);

            for (int iter = 0; iter < Iterations; iter++)
            {
                for (int i = Positions.Length - 1; i > 0; i--)
                {
                    if (i == Positions.Length - 1)
                        Positions[i] = desiredPosition;
                    else
                        Positions[i] = Positions[i + 1] + (Positions[i] - Positions[i + 1]).normalized * SegmentLengths[i];
                }

                for (int i = 1; i < Positions.Length; i++)
                    Positions[i] = Positions[i - 1] + (Positions[i] - Positions[i - 1]).normalized * SegmentLengths[i - 1];

                if ((Positions[Positions.Length - 1] - desiredPosition).sqrMagnitude < Delta * Delta)
                    break;
            }
        }

        for (int i = 0; i < Positions.Length; i++)
        {
            if (i == Positions.Length - 1)
                SetRotationRelativeToRoot(Joints[i], Quaternion.Inverse(desiredRotation) * InitialTargetRotation * Quaternion.Inverse(InitialRotations[i]));
            else
                SetRotationRelativeToRoot(Joints[i], Quaternion.FromToRotation(InitialDirections[i], Positions[i + 1] - Positions[i]) * Quaternion.Inverse(InitialRotations[i]));
            SetPositionRelativeToRoot(Joints[i], Positions[i]);
        }
    }

    private Vector3 GetPositionRelativeToRoot(Transform current)
    {
        if (RootTransform == null)
            return current.position;
        else
            return Quaternion.Inverse(RootTransform.rotation) * (current.position - RootTransform.position);
    }

    private void SetPositionRelativeToRoot(Transform current, Vector3 position)
    {
        if (RootTransform == null)
            current.position = position;
        else
            current.position = RootTransform.rotation * position + RootTransform.position;
    }

    private Quaternion GetRotationRelativeToRoot(Transform current)
    {
        if (RootTransform == null)
            return current.rotation;
        else
            return Quaternion.Inverse(current.rotation) * RootTransform.rotation;
    }

    private void SetRotationRelativeToRoot(Transform current, Quaternion rotation)
    {
        if (RootTransform == null)
            current.rotation = rotation;
        else
            current.rotation = RootTransform.rotation * rotation;
    }
}
