
using UnityEngine;
using UnityEngine.InputSystem;

public class PoleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private Transform stick;
    [SerializeField] private Transform stickTip;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask groundLayer;

    [Header("Pole")]
    [SerializeField] private float minLength = 0.15f;
    [SerializeField] private float maxLength = 2.5f;
    [SerializeField] private float tipRadius = 0.12f;
    [SerializeField] private float contactSkin = 0.015f;

    [Header("Free Movement")]
    [SerializeField] private float minPushSpeed = 1.5f;
    [SerializeField] private float maxMouseStep = 0.24f;
    [SerializeField] private float pushStrength = 20f;
    [SerializeField] private float lateralGrip = 0.55f;
    [SerializeField] private float maxImpulsePerStep = 3.5f;

    [Header("Grip")]
    [SerializeField] private float gripSensitivity = 1f;
    [SerializeField] private float maxGripSpeed = 8f;
    [SerializeField] private float minGripLength = 0.3f;
    [SerializeField] private float releaseMomentum = 0.65f;
    [SerializeField] private float maxReleaseSpeed = 7f;

    [Header("Player Movement")]
    [SerializeField] private float groundBraking = 18f;
    [SerializeField] private float maxHorizontalSpeed = 6f;
    [SerializeField] private float maxRiseSpeed = 9f;
    [SerializeField] private float maxFallSpeed = 12f;

    [Header("Launch Feel")]
    [SerializeField, Range(0f, 1f)] private float velocitySmoothing = 0.5f;

    [SerializeField] private float minLaunchSpeed = 0.6f;

    private Vector2 smoothedGripVelocity;
    private bool hasGripVelocitySample;

    public bool IsGripping { get; private set; }
    public bool IsTouchingGround { get; private set; }

    private CircleCollider2D playerCollider;

    private Vector2 mouseScreen;
    private Vector2 previousMouseScreen;

    private Vector2 tipWorld;
    private Vector2 gripPoint;
    private Vector2 lastGripBodyPosition;

    private float originalGravity;
    private float gripMinimumDistance;

    private bool mouseReady;
    private bool initialized;
    private bool gripHeld;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (playerBody != null)
        {
            originalGravity = playerBody.gravityScale;
            playerCollider =
                playerBody.GetComponent<CircleCollider2D>();
        }
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        mouseScreen = Mouse.current.position.ReadValue();
        gripHeld = Mouse.current.leftButton.isPressed;
        mouseReady = true;
    }

    private Vector2 ScreenToWorld(Vector2 screen)
    {
        float distance =
            transform.position.z -
            mainCamera.transform.position.z;

        return mainCamera.ScreenToWorldPoint(
            new Vector3(screen.x, screen.y, distance)
        );
    }

    private void FixedUpdate()
    {
        if (!mouseReady ||
            mainCamera == null ||
            playerBody == null ||
            stick == null ||
            stickTip == null)
            return;

        Vector2 mouse = ScreenToWorld(mouseScreen);
        Vector2 previousMouse =
            ScreenToWorld(previousMouseScreen);

        if (!initialized)
        {
            tipWorld = playerBody.position;

            Vector2 initialTarget =
                playerBody.position +
                ClampPole(mouse - playerBody.position);

            MoveTip(initialTarget, out _);

            previousMouseScreen = mouseScreen;
            initialized = true;
            return;
        }

        if (IsGripping)
        {
            if (gripHeld)
            {
                UpdateGrip(mouse - previousMouse);
            }
            else
            {
                ReleaseGrip();
                previousMouseScreen = mouseScreen;
                return;
            }
        }
        else
        {
            UpdateFree(mouse, previousMouse);
        }

        previousMouseScreen = mouseScreen;
        LimitSpeed();
    }


    private void UpdateFree(
        Vector2 mouse,
        Vector2 previousMouse)
    {
        Vector2 origin = playerBody.position;

        Vector2 currentOffset = ClampPole(
            mouse - origin
        );

        Vector2 targetTip = origin + currentOffset;

        bool contact = MoveTip(
            targetTip,
            out Vector2 contactNormal
        );

        IsTouchingGround = contact;

        if (gripHeld && contact)
        {
            StartGrip();
            return;
        }

        if (IsPlayerGrounded())
        {
            Vector2 velocity = playerBody.linearVelocity;

            velocity.x = Mathf.MoveTowards(
                velocity.x,
                0f,
                groundBraking * Time.fixedDeltaTime
            );

            playerBody.linearVelocity = velocity;
        }
    }

    private bool MoveTip(
        Vector2 target,
        out Vector2 normal)
    {
        normal = Vector2.zero;

        Vector2 movement = target - tipWorld;

        if (movement.sqrMagnitude < 0.000001f)
            return CheckNearbyContact(out normal);

        RaycastHit2D hit = Physics2D.CircleCast(
            tipWorld,
            tipRadius,
            movement.normalized,
            movement.magnitude,
            groundLayer
        );

        if (hit.collider == null)
        {
            tipWorld = target;
            return CheckNearbyContact(out normal);
        }

        normal = hit.normal;

        tipWorld =
            hit.centroid + normal * contactSkin;

        Vector2 remaining = target - tipWorld;

        Vector2 tangentMovement =
            remaining -
            Vector2.Dot(remaining, normal) * normal;

        if (tangentMovement.sqrMagnitude > 0.000001f)
        {
            RaycastHit2D slideHit = Physics2D.CircleCast(
                tipWorld,
                tipRadius,
                tangentMovement.normalized,
                tangentMovement.magnitude,
                groundLayer
            );

            if (slideHit.collider == null)
            {
                tipWorld += tangentMovement;
            }
            else if (slideHit.distance > 0.0001f)
            {
                tipWorld =
                    slideHit.centroid +
                    slideHit.normal * contactSkin;
            }
        }

        return true;
    }

    private bool CheckNearbyContact(out Vector2 normal)
    {
        normal = Vector2.zero;

        Collider2D surface = Physics2D.OverlapCircle(
            tipWorld,
            tipRadius + contactSkin + 0.005f,
            groundLayer
        );

        if (surface == null)
            return false;

        Vector2 closest = surface.ClosestPoint(tipWorld);
        Vector2 difference = tipWorld - closest;

        if (difference.sqrMagnitude > 0.000001f)
            normal = difference.normalized;

        return true;
    }

    private bool ApplyPush(
        Vector2 stroke,
        Vector2 normal)
    {
        float inwardMovement =
            Mathf.Max(0f, -Vector2.Dot(stroke, normal));

        float inwardSpeed =
            inwardMovement / Time.fixedDeltaTime;

        if (inwardSpeed < minPushSpeed)
            return false;

        Vector2 tangentMovement =
            stroke - Vector2.Dot(stroke, normal) * normal;

        Vector2 impulse =
            (normal * inwardMovement -
             tangentMovement * lateralGrip) *
            pushStrength;

        impulse = Vector2.ClampMagnitude(
            impulse,
            maxImpulsePerStep
        );

        playerBody.AddForce(
            impulse,
            ForceMode2D.Impulse
        );

        return true;
    }

    private void StartGrip()
    {
        gripPoint = tipWorld;
        IsGripping = true;

        float distance = Vector2.Distance(
            playerBody.position,
            gripPoint
        );

        gripMinimumDistance =
            Mathf.Min(minGripLength, distance);

        lastGripBodyPosition = playerBody.position;
        smoothedGripVelocity = Vector2.zero;
        hasGripVelocitySample = false;

        playerBody.gravityScale = 0f;
        playerBody.linearVelocity = Vector2.zero;
    }

    private void UpdateGrip(Vector2 mouseDelta)
    {
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

        Vector2 targetPlayer =
            playerBody.position -
            mouseDelta * gripSensitivity;

        Vector2 fromGrip =
            targetPlayer - gripPoint;

        if (fromGrip.sqrMagnitude > 0.000001f)
        {
            float length = Mathf.Clamp(
                fromGrip.magnitude,
                gripMinimumDistance,
                maxLength
            );

            targetPlayer =
                gripPoint + fromGrip.normalized * length;
        }

        Vector2 nextPosition = Vector2.MoveTowards(
            playerBody.position,
            targetPlayer,
            maxGripSpeed * Time.fixedDeltaTime
        );

        lastGripBodyPosition = playerBody.position;

        playerBody.linearVelocity = Vector2.zero;
        playerBody.MovePosition(nextPosition);
    }



    private void ReleaseGrip()
    {
        Vector2 currentVelocity =
            (playerBody.position - lastGripBodyPosition)
            / Time.fixedDeltaTime;

        IsGripping = false;
        playerBody.gravityScale = originalGravity;

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

        playerBody.linearVelocity = launchVelocity;

        tipWorld = gripPoint;
    }

    private bool IsPlayerGrounded()
    {
        float distance = playerCollider != null
            ? playerCollider.bounds.extents.y + 0.1f
            : 0.6f;

        RaycastHit2D hit = Physics2D.Raycast(
            playerBody.position,
            Vector2.down,
            distance,
            groundLayer
        );

        return hit.collider != null &&
               hit.normal.y > 0.5f;
    }

    private void LimitSpeed()
    {
        if (IsGripping)
            return;

        Vector2 velocity = playerBody.linearVelocity;

        velocity.x = Mathf.Clamp(
            velocity.x,
            -maxHorizontalSpeed,
            maxHorizontalSpeed
        );

        velocity.y = Mathf.Clamp(
            velocity.y,
            -maxFallSpeed,
            maxRiseSpeed
        );

        playerBody.linearVelocity = velocity;
    }

    private Vector2 ClampPole(Vector2 offset)
    {
        if (offset.sqrMagnitude < 0.000001f)
            return Vector2.right * minLength;

        return offset.normalized * Mathf.Clamp(
            offset.magnitude,
            minLength,
            maxLength
        );
    }

    private void LateUpdate()
    {
        if (!initialized)
            return;

        Vector2 offset =
            (IsGripping ? gripPoint : tipWorld) -
            (Vector2)transform.position;

        if (offset.sqrMagnitude < 0.000001f)
            return;

        float length = offset.magnitude;

        float angle = Mathf.Atan2(
            offset.y,
            offset.x
        ) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);

        stick.localPosition =
            new Vector3(length / 2f, 0f, 0f);

        stick.localScale =
            new Vector3(length, 0.15f, 1f);

        stickTip.localPosition =
            new Vector3(length, 0f, 0f);
    }

    private void OnDisable()
    {
        if (playerBody != null)
            playerBody.gravityScale = originalGravity;

        IsGripping = false;
    }

    private void OnDrawGizmos()
    {
        if (!initialized)
            return;

        Gizmos.color = IsGripping
            ? Color.cyan
            : IsTouchingGround
                ? Color.green
                : Color.red;

        Gizmos.DrawWireSphere(
            IsGripping ? gripPoint : tipWorld,
            tipRadius
        );
    }
}
