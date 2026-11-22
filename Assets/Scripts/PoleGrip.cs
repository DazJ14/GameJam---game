
using UnityEngine;

public class PoleGrip : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D playerBody;

    [Header("Grip Movement")]
    [SerializeField] private float gripSensitivity = 1f;
    [SerializeField] private float maxMouseStep = 0.24f;
    [SerializeField] private float maxGripSpeed = 9f;
    [SerializeField] private float minGripLength = 0.3f;
    [SerializeField] private float maxGripLength = 1.5f;

    [Header("Release")]
    [SerializeField] private float releaseMomentum = 0.8f;
    [SerializeField] private float maxReleaseSpeed = 7f;

    [Header("Launch Feel")]
    [SerializeField, Range(0f, 1f)]
    private float velocitySmoothing = 0.5f;

    [SerializeField] private float minLaunchSpeed = 0.6f;

    public bool IsGripping { get; private set; }
    public Vector2 GripPoint { get; private set; }

    private float originalGravity;
    private float gripMinimumDistance;

    private Vector2 lastGripBodyPosition;
    private Vector2 smoothedGripVelocity;
    private bool hasGripVelocitySample;

    private void Awake()
    {
        if (playerBody != null)
            originalGravity = playerBody.gravityScale;
    }

    public void BeginGrip(Vector2 point)
    {
        if (IsGripping || playerBody == null)
            return;

        GripPoint = point;
        IsGripping = true;

        float distance = Vector2.Distance(
            playerBody.position,
            GripPoint
        );

        gripMinimumDistance = Mathf.Min(
            minGripLength,
            distance
        );

        lastGripBodyPosition = playerBody.position;

        // Reset launch smoothing for this grip.
        smoothedGripVelocity = Vector2.zero;
        hasGripVelocitySample = false;

        // The mouse controls the player while attached.
        playerBody.gravityScale = 0f;
        playerBody.linearVelocity = Vector2.zero;
    }

    public void UpdateGrip(Vector2 mouseDelta)
    {
        if (!IsGripping || playerBody == null)
            return;

        // Measure actual movement from the previous
        // physics step, not the requested movement.
        Vector2 measuredVelocity =
            (playerBody.position - lastGripBodyPosition)
            / Time.fixedDeltaTime;

        if (!hasGripVelocitySample)
        {
            smoothedGripVelocity = measuredVelocity;
            hasGripVelocitySample = true;
        }
        else
        {
            smoothedGripVelocity = Vector2.Lerp(
                smoothedGripVelocity,
                measuredVelocity,
                velocitySmoothing
            );
        }

        mouseDelta = Vector2.ClampMagnitude(
            mouseDelta,
            maxMouseStep
        );

        // Player moves opposite to mouse movement.
        Vector2 requestedPosition =
            playerBody.position -
            mouseDelta * gripSensitivity;

        Vector2 fromGrip =
            requestedPosition - GripPoint;

        if (fromGrip.sqrMagnitude > 0.000001f)
        {
            float length = Mathf.Clamp(
                fromGrip.magnitude,
                gripMinimumDistance,
                maxGripLength
            );

            requestedPosition =
                GripPoint +
                fromGrip.normalized * length;
        }

        Vector2 nextPosition = Vector2.MoveTowards(
            playerBody.position,
            requestedPosition,
            maxGripSpeed * Time.fixedDeltaTime
        );

        lastGripBodyPosition = playerBody.position;

        playerBody.linearVelocity = Vector2.zero;
        playerBody.MovePosition(nextPosition);
    }

    public void ReleaseGrip()
    {
        if (!IsGripping || playerBody == null)
            return;

        // Actual velocity over the last physics step.
        Vector2 currentVelocity =
            (playerBody.position - lastGripBodyPosition)
            / Time.fixedDeltaTime;

        Vector2 launchVelocity = Vector2.zero;

        if (currentVelocity.magnitude >= minLaunchSpeed)
        {
            Vector2 combinedVelocity =
                hasGripVelocitySample
                    ? Vector2.Lerp(
                        smoothedGripVelocity,
                        currentVelocity,
                        0.7f
                    )
                    : currentVelocity;

            launchVelocity = Vector2.ClampMagnitude(
                combinedVelocity * releaseMomentum,
                maxReleaseSpeed
            );
        }

        IsGripping = false;
        playerBody.gravityScale = originalGravity;

        // The only pole-generated launch.
        playerBody.linearVelocity = launchVelocity;
    }

    public void CancelGrip()
    {
        if (!IsGripping || playerBody == null)
            return;

        IsGripping = false;
        playerBody.gravityScale = originalGravity;
        playerBody.linearVelocity = Vector2.zero;
    }

    private void OnDisable()
    {
        CancelGrip();
    }
}
