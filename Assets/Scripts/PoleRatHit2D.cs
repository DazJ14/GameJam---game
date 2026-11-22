
using UnityEngine;

public class PoleRatHit2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PoleTipCollision poleTip;

    [Header("Hit Detection")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float hitRadius = 0.15f;
    [SerializeField] private float minMovement = 0.03f;
    [SerializeField] private float maxSampleDistance = 2.5f;

    private Vector2 previousTip;
    private bool ready;

    private void OnEnable()
    {
        ready = false;
    }

    private void FixedUpdate()
    {
        if (poleTip == null)
            return;

        Vector2 currentTip = poleTip.TipWorld;

        if (!ready)
        {
            previousTip = currentTip;
            ready = true;
            return;
        }

        Vector2 delta = currentTip - previousTip;
        float distance = delta.magnitude;

        previousTip = currentTip;

        // Skip tiny movements and large teleports.
        if (distance < minMovement ||
            distance > maxSampleDistance)
            return;

        RaycastHit2D hit = Physics2D.CircleCast(
            currentTip - delta,
            hitRadius,
            delta.normalized,
            distance,
            enemyLayer
        );

        if (hit.collider == null)
            return;

        RatPatrol2D rat =
            hit.collider.GetComponentInParent<RatPatrol2D>();

        if (rat != null)
            rat.HitByPole();
    }
}
