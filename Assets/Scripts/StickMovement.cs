
using UnityEngine;

public class StickMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private Transform stickTip;
    [SerializeField] private StickCollisionDetector collisionDetector;

    [Header("Movement Settings")]
    [SerializeField] private float pushStrength = 7f;
    [SerializeField] private float maxTipSpeed = 14f;

    private Vector2 previousTipOffset;

    private void Start()
    {
        previousTipOffset = GetTipOffset();
    }

    private void FixedUpdate()
    {
        Vector2 currentTipOffset = GetTipOffset();

        Vector2 tipMovement = currentTipOffset - previousTipOffset;

        Vector2 tipVelocity = tipMovement / Time.fixedDeltaTime;

        // Limitar movimientos demasiado bruscos
        tipVelocity = Vector2.ClampMagnitude(
            tipVelocity,
            maxTipSpeed
        );

        previousTipOffset = currentTipOffset;

        if (!collisionDetector.IsTouchingGround)
        {
            return;
        }

        Vector2 pushForce =
            -tipVelocity * pushStrength;

        playerBody.AddForce(
            pushForce,
            ForceMode2D.Force
        );
    }

    private Vector2 GetTipOffset()
    {
        return stickTip.position - transform.position;
    }
}
