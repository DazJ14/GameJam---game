
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RatPatrol2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject keyPrefab;

    [Header("Detection")]
    [SerializeField, Min(0f)]
    private float detectionRange = 3f;

    [Header("Route")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField, Min(0.01f)]
    private float speed = 2f;
    [SerializeField, Min(0f)]
    private float waitAtEnd = 2f;

    private Rigidbody2D body;
    private Vector2 startPosition;

    private bool activated;
    private bool dead;
    private bool returning;
    private int waypointIndex;
    private float waitTimer;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;

        startPosition = body.position;
    }

    private void Update()
    {
        if (dead || activated || player == null)
            return;

        if (Vector2.Distance(
            transform.position,
            player.position
        ) <= detectionRange)
        {
            activated = true;
        }
    }

    private void FixedUpdate()
    {
        if (!activated || dead ||
            waypoints == null || waypoints.Length == 0)
            return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector2 target = returning
            ? (waypointIndex < 0
                ? startPosition
                : (Vector2)waypoints[waypointIndex].position)
            : (Vector2)waypoints[waypointIndex].position;

        Vector2 next = Vector2.MoveTowards(
            body.position,
            target,
            speed * Time.fixedDeltaTime
        );

        body.MovePosition(next);

        if (Vector2.Distance(next, target) > 0.001f)
            return;

        if (!returning)
        {
            waypointIndex++;

            if (waypointIndex >= waypoints.Length)
            {
                returning = true;
                waypointIndex = waypoints.Length - 2;
                waitTimer = waitAtEnd;
            }
        }
        else
        {
            waypointIndex--;

            if (waypointIndex < -1)
            {
                returning = false;
                waypointIndex = 0;
            }
        }
    }

    public void HitByPole()
    {
        if (dead)
            return;

        dead = true;

        if (keyPrefab != null)
            Instantiate(
                keyPrefab,
                transform.position,
                Quaternion.identity
            );

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 previous = transform.position;

        if (waypoints != null)
        {
            foreach (Transform point in waypoints)
            {
                if (point == null)
                    continue;

                Gizmos.DrawLine(previous, point.position);
                Gizmos.DrawWireSphere(point.position, 0.12f);
                previous = point.position;
            }
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );
    }
}
