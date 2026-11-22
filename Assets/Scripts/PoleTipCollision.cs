
using UnityEngine;

public class PoleTipCollision : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float tipRadius = 0.12f;
    [SerializeField] private float contactSkin = 0.015f;

    public Vector2 TipWorld { get; private set; }
    public bool IsTouchingGround { get; private set; }
    public float TipRadius => tipRadius;

    public void Initialize(Vector2 startPosition)
    {
        TipWorld = startPosition;
        IsTouchingGround = false;
    }

    public bool MoveTip(
        Vector2 target,
        out Vector2 normal)
    {
        normal = Vector2.zero;

        Vector2 movement = target - TipWorld;

        if (movement.sqrMagnitude < 0.000001f)
        {
            IsTouchingGround =
                CheckNearbyContact(out normal);

            return IsTouchingGround;
        }

        RaycastHit2D hit = Physics2D.CircleCast(
            TipWorld,
            tipRadius,
            movement.normalized,
            movement.magnitude,
            groundLayer
        );

        if (hit.collider == null)
        {
            TipWorld = target;

            IsTouchingGround =
                CheckNearbyContact(out normal);

            return IsTouchingGround;
        }

        normal = hit.normal;

        // Stop at the obstacle.
        TipWorld =
            hit.centroid + normal * contactSkin;

        // Try sliding along the surface.
        Vector2 remaining = target - TipWorld;

        Vector2 tangentMovement =
            remaining -
            Vector2.Dot(remaining, normal) * normal;

        if (tangentMovement.sqrMagnitude > 0.000001f)
        {
            RaycastHit2D slideHit = Physics2D.CircleCast(
                TipWorld,
                tipRadius,
                tangentMovement.normalized,
                tangentMovement.magnitude,
                groundLayer
            );

            if (slideHit.collider == null)
            {
                TipWorld += tangentMovement;
            }
            else if (slideHit.distance > 0.0001f)
            {
                TipWorld =
                    slideHit.centroid +
                    slideHit.normal * contactSkin;
            }
        }

        IsTouchingGround = true;
        return true;
    }

    public void SetTipPosition(Vector2 position)
    {
        TipWorld = position;
        IsTouchingGround = false;
    }

    private bool CheckNearbyContact(out Vector2 normal)
    {
        normal = Vector2.zero;

        Collider2D surface = Physics2D.OverlapCircle(
            TipWorld,
            tipRadius + contactSkin + 0.005f,
            groundLayer
        );

        if (surface == null)
            return false;

        Vector2 closest = surface.ClosestPoint(TipWorld);
        Vector2 difference = TipWorld - closest;

        if (difference.sqrMagnitude > 0.000001f)
            normal = difference.normalized;

        return true;
    }
}
