
using UnityEngine;
using UnityEngine.InputSystem;

public class Door2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerKeyHolder player;
    [SerializeField] private SpriteRenderer doorRenderer;
    [SerializeField] private Collider2D blockingCollider;

    [Header("Sprites")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    [Header("Interaction")]
    [SerializeField, Min(0.1f)]
    private float interactionRange = 1.5f;

    public bool IsOpen { get; private set; }

    private void Start()
    {
        if (doorRenderer != null)
            doorRenderer.sprite = closedSprite;
    }

    private void Update()
    {
        if (IsOpen || player == null ||
            Keyboard.current == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.transform.position
        );

        if (distance > interactionRange)
            return;

        if (!Keyboard.current.eKey.wasPressedThisFrame)
            return;

        if (!player.UseKey())
            return;

        Open();
    }

    private void Open()
    {
        IsOpen = true;

        if (doorRenderer != null)
            doorRenderer.sprite = openSprite;

        if (blockingCollider != null)
            blockingCollider.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            transform.position,
            interactionRange
        );
    }
}
