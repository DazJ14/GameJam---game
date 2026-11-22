
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FloatingKey2D : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private float orbitRadius = 0.65f;
    [SerializeField] private float orbitSpeed = 2f;
    [SerializeField] private float followSpeed = 12f;

    private Transform owner;
    private Collider2D pickupCollider;
    private float orbitAngle;
    private bool collected;

    private void Awake()
    {
        pickupCollider = GetComponent<Collider2D>();
        pickupCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
            return;

        PlayerKeyHolder holder =
            other.GetComponentInParent<PlayerKeyHolder>();

        if (holder == null)
            return;

        Rigidbody2D playerBody =
            holder.GetComponent<Rigidbody2D>();

        if (other.attachedRigidbody != playerBody)
            return;

        if (!holder.AcquireKey(this))
            return;

        collected = true;
        owner = holder.transform;
        pickupCollider.enabled = false;
    }

    private void LateUpdate()
    {
        if (!collected || owner == null)
            return;

        orbitAngle += orbitSpeed * Time.deltaTime;

        Vector3 offset = new Vector3(
            Mathf.Cos(orbitAngle),
            Mathf.Sin(orbitAngle),
            0f
        ) * orbitRadius;

        Vector3 target = owner.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            target,
            followSpeed * Time.deltaTime
        );
    }
}
