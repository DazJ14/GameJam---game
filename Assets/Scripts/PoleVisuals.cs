
using UnityEngine;

public class PoleVisual : MonoBehaviour
{
    [Header("Body")]
    [SerializeField] private Transform bodyRoot;

    [Header("Leg")]
    [SerializeField] private Transform stick;
    [SerializeField] private Transform stickTip;
    [SerializeField] private float legRotationOffset = 90f;

    [Header("Arms")]
    [SerializeField] private SpriteRenderer armBack;
    [SerializeField] private SpriteRenderer armFront;

    [Header("Arm Origin - Player Local Position")]
    [SerializeField] private Vector2 backArmOrigin =
        new Vector2(-0.12f, -0.05f);

    [SerializeField] private Vector2 frontArmOrigin =
        new Vector2(0.12f, -0.05f);

    [Header("Hand Position")]
    [SerializeField] private float gripDistanceFromTip = 0.55f;
    [SerializeField] private float minHandX = -0.1f;
    [SerializeField] private float handSeparation = 0.035f;

    [Header("Arm Appearance")]
    [SerializeField] private float armWidth = 0.06f;
    [SerializeField] private float minArmLength = 0.07f;

    private Vector3 originalLegScale;

    private void Awake()
    {
        if (bodyRoot == null)
            bodyRoot = transform.parent;

        if (stick != null)
            originalLegScale = stick.localScale;
    }

    public void UpdateVisual(Vector2 tipPosition)
    {
        if (stick == null || stickTip == null)
            return;

        Vector2 offset =
            tipPosition - (Vector2)transform.position;

        float length = offset.magnitude;

        if (length < 0.0001f)
            return;

        // Rotate the entire pole toward the physical tip.
        float angle = Mathf.Atan2(
            offset.y,
            offset.x
        ) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(
            0f, 0f, angle
        );

        // Position of the real tip in local coordinates.
        stickTip.localPosition = new Vector3(
            length, 0f, 0f
        );

        // The leg's sprite pivot is at the toes.
        // Therefore, its pivot follows StickTip.
        stick.localPosition = new Vector3(
            length, 0f, 0f
        );

        // Original leg artwork is vertical.
        stick.localRotation = Quaternion.Euler(
            0f, 0f, legRotationOffset
        );

        // Preserve the illustration's proportions.
        stick.localScale = originalLegScale;

        UpdateArms(length);
    }

    private void UpdateArms(float poleLength)
    {
        if (bodyRoot == null)
            return;

        // The hands hold a point along the leg,
        // measured backward from the toes.
        float handX = Mathf.Max(
            minHandX,
            poleLength - gripDistanceFromTip
        );

        Vector3 backHand = transform.TransformPoint(
            new Vector3(
                handX,
                -handSeparation,
                0f
            )
        );

        Vector3 frontHand = transform.TransformPoint(
            new Vector3(
                handX,
                handSeparation,
                0f
            )
        );

        Vector3 backStart = bodyRoot.TransformPoint(
            new Vector3(
                backArmOrigin.x,
                backArmOrigin.y,
                0f
            )
        );

        Vector3 frontStart = bodyRoot.TransformPoint(
            new Vector3(
                frontArmOrigin.x,
                frontArmOrigin.y,
                0f
            )
        );

        UpdateArm(armBack, backStart, backHand);
        UpdateArm(armFront, frontStart, frontHand);
    }

    private void UpdateArm(
        SpriteRenderer arm,
        Vector3 start,
        Vector3 end)
    {
        if (arm == null)
            return;

        Vector2 direction = end - start;
        float distance = direction.magnitude;

        if (distance < 0.0001f)
            return;

        // Center the arm between body and hand.
        Vector3 midpoint = (start + end) * 0.5f;
        midpoint.z = arm.transform.position.z;

        arm.transform.position = midpoint;

        // The arm sprite originally points upward.
        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        arm.transform.rotation = Quaternion.Euler(
            0f, 0f, angle - 90f
        );

        // Use SpriteRenderer Sliced mode to resize
        // the middle without distorting the ends.
        arm.size = new Vector2(
            armWidth,
            Mathf.Max(distance, minArmLength)
        );
    }
}
