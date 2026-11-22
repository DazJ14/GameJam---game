
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class StickCollisionDetector : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Transform stickTip;
    [SerializeField] private float detectionRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    public bool IsTouchingGround { get; private set; }

    private void FixedUpdate()
    {
        CheckCollision();
    }

    private void CheckCollision()
    {
        IsTouchingGround = Physics2D.OverlapCircle(
            stickTip.position,
            detectionRadius,
            groundLayer
        ) != null;
    }

    private void OnDrawGizmos()
    {
        if (stickTip == null)
        {
            return;
        }

        Gizmos.color = IsTouchingGround
            ? Color.green
            : Color.red;

        Gizmos.DrawWireSphere(
            stickTip.position,
            detectionRadius
        );
    }
}
