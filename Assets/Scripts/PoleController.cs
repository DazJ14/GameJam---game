
using UnityEngine;
using UnityEngine.InputSystem;

public class PoleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private PoleTipCollision tipCollision;
    [SerializeField] private PoleGrip poleGrip;
    [SerializeField] private PoleVisual poleVisual;
    [SerializeField] private Camera mainCamera;

    [Header("Pole")]
    [SerializeField] private float minLength = 0.15f;
    [SerializeField] private float maxLength = 1.5f;

    [Header("Player Movement")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundBraking = 4f;
    [SerializeField] private float maxHorizontalSpeed = 8f;
    [SerializeField] private float maxRiseSpeed = 9f;
    [SerializeField] private float maxFallSpeed = 12f;

    private CircleCollider2D playerCollider;

    private Vector2 mouseScreen;
    private Vector2 previousMouseScreen;

    private bool mouseReady;
    private bool initialized;
    private bool gripHeld;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (playerBody != null)
        {
            playerCollider =
                playerBody.GetComponent<CircleCollider2D>();
        }
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        mouseScreen =
            Mouse.current.position.ReadValue();

        gripHeld =
            Mouse.current.leftButton.isPressed;

        mouseReady = true;
    }

    private Vector2 ScreenToWorld(Vector2 screen)
    {
        float cameraDistance =
            transform.position.z -
            mainCamera.transform.position.z;

        return mainCamera.ScreenToWorldPoint(
            new Vector3(
                screen.x,
                screen.y,
                cameraDistance
            )
        );
    }

    private void FixedUpdate()
    {
        if (!mouseReady ||
            mainCamera == null ||
            playerBody == null ||
            tipCollision == null ||
            poleGrip == null ||
            poleVisual == null)
        {
            return;
        }

        Vector2 currentMouse =
            ScreenToWorld(mouseScreen);

        Vector2 previousMouse =
            ScreenToWorld(previousMouseScreen);

        if (!initialized)
        {
            InitializePole(currentMouse);

            previousMouseScreen = mouseScreen;
            initialized = true;
            return;
        }

        Vector2 mouseDelta =
            currentMouse - previousMouse;

        if (poleGrip.IsGripping)
        {
            if (gripHeld)
            {
                poleGrip.UpdateGrip(mouseDelta);
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
            UpdateFreePole(currentMouse);
        }

        previousMouseScreen = mouseScreen;

        if (!poleGrip.IsGripping)
            LimitPlayerSpeed();
    }

    private void InitializePole(Vector2 mousePosition)
    {
        Vector2 origin = playerBody.position;

        tipCollision.Initialize(origin);

        Vector2 initialTarget =
            origin + ClampPole(
                mousePosition - origin
            );

        tipCollision.MoveTip(
            initialTarget,
            out _
        );
    }

    private void UpdateFreePole(Vector2 mousePosition)
    {
        Vector2 origin = playerBody.position;

        Vector2 offset = ClampPole(
            mousePosition - origin
        );

        Vector2 targetTip = origin + offset;

        // Only the tip detects terrain.
        bool contact = tipCollision.MoveTip(
            targetTip,
            out _
        );

        // Click + contact: enter grip mode.
        if (gripHeld && contact)
        {
            poleGrip.BeginGrip(
                tipCollision.TipWorld
            );

            return;
        }

        // No pushing while free.
        // Only gravity and existing velocity apply.

        // Reduce sliding while grounded.
        if (IsPlayerGrounded())
        {
            Vector2 velocity =
                playerBody.linearVelocity;

            velocity.x = Mathf.MoveTowards(
                velocity.x,
                0f,
                groundBraking * Time.fixedDeltaTime
            );

            playerBody.linearVelocity = velocity;
        }
    }

    private void ReleaseGrip()
    {
        Vector2 lastGripPoint =
            poleGrip.GripPoint;

        // Launch calculation belongs to PoleGrip.
        poleGrip.ReleaseGrip();

        // Continue from the tip's last valid position.
        tipCollision.SetTipPosition(lastGripPoint);
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

    private void LimitPlayerSpeed()
    {
        Vector2 velocity =
            playerBody.linearVelocity;

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

        float length = Mathf.Clamp(
            offset.magnitude,
            minLength,
            maxLength
        );

        return offset.normalized * length;
    }

    private void LateUpdate()
    {
        if (!initialized ||
            poleVisual == null ||
            poleGrip == null ||
            tipCollision == null)
        {
            return;
        }

        Vector2 target = poleGrip.IsGripping
            ? poleGrip.GripPoint
            : tipCollision.TipWorld;

        poleVisual.UpdateVisual(target);
    }

    private void OnDisable()
    {
        if (poleGrip != null)
            poleGrip.CancelGrip();
    }
}
