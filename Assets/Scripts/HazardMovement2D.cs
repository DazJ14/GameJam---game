
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HazardMovement2D : MonoBehaviour
{
    public enum MovementAxis
    {
        Horizontal,
        Vertical
    }

    [Header("Movement")]
    [SerializeField] private MovementAxis axis =
        MovementAxis.Vertical;

    [SerializeField, Min(0f)]
    private float distanceNegative = 2f;

    [SerializeField, Min(0f)]
    private float distancePositive = 2f;

    [SerializeField, Min(0.01f)]
    private float speed = 2f;

    [SerializeField]
    private bool startAtPositiveEnd = false;

    [Header("Path Line")]
    [SerializeField] private LineRenderer pathLine;
    [SerializeField] private bool showPathLine = false;
    [SerializeField] private float lineWidth = 0.06f;
    [SerializeField] private Color lineColor =
        new Color(0.6f, 0.7f, 0.8f, 0.65f);

    private Rigidbody2D body;
    private Vector2 center;
    private Vector2 negativeEnd;
    private Vector2 positiveEnd;

    private Vector2 destination;
    private bool initialized;

    private Vector2 AxisDirection =>
        axis == MovementAxis.Horizontal
            ? Vector2.right
            : Vector2.up;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;

        // Starting transform is the path center.
        center = transform.position;

        CalculateEndpoints();

        Vector2 start = startAtPositiveEnd
            ? positiveEnd
            : negativeEnd;

        destination = startAtPositiveEnd
            ? negativeEnd
            : positiveEnd;

        // Set starting position before physics begins.
        body.position = start;
        initialized = true;

        SetupLine();
    }

    private void FixedUpdate()
    {
        if (!initialized)
            return;

        Vector2 nextPosition = Vector2.MoveTowards(
            body.position,
            destination,
            speed * Time.fixedDeltaTime
        );

        body.MovePosition(nextPosition);

        if (Vector2.Distance(
            nextPosition,
            destination
        ) <= 0.001f)
        {
            destination =
                destination == positiveEnd
                    ? negativeEnd
                    : positiveEnd;
        }
    }

    private void CalculateEndpoints()
    {
        Vector2 direction = AxisDirection;

        negativeEnd =
            center - direction * distanceNegative;

        positiveEnd =
            center + direction * distancePositive;
    }

    private void SetupLine()
    {
        if (pathLine == null)
            return;

        pathLine.enabled = showPathLine;

        if (!showPathLine)
            return;

        pathLine.useWorldSpace = true;
        pathLine.positionCount = 2;
        pathLine.startWidth = lineWidth;
        pathLine.endWidth = lineWidth;

        pathLine.startColor = lineColor;
        pathLine.endColor = lineColor;

        pathLine.SetPosition(
            0,
            new Vector3(
                negativeEnd.x,
                negativeEnd.y,
                0f
            )
        );

        pathLine.SetPosition(
            1,
            new Vector3(
                positiveEnd.x,
                positiveEnd.y,
                0f
            )
        );
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 origin = Application.isPlaying &&
                         initialized
            ? center
            : (Vector2)transform.position;

        Vector2 direction = AxisDirection;

        Vector2 a = origin -
            direction * distanceNegative;

        Vector2 b = origin +
            direction * distancePositive;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(a, b);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(a, 0.15f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(b, 0.15f);
    }
}
