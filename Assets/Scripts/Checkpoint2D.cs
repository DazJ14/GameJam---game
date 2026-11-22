
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Checkpoint2D : MonoBehaviour
{
    [SerializeField] private GameProgress gameProgress;

    [Header("Checkpoint")]
    [SerializeField] private int targetRoom = 1;
    [SerializeField] private Transform respawnPoint;

    [Tooltip("Player must cross this world Y position.")]
    [SerializeField] private float minimumPlayerY = 6.1f;

    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryActivate(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryActivate(other);
    }

    private void TryActivate(Collider2D other)
    {
        if (gameProgress == null || respawnPoint == null)
            return;

        Rigidbody2D body = other.attachedRigidbody;

        if (body == null ||
            body != gameProgress.PlayerBody)
        {
            return;
        }

        // Activate only after crossing the boundary.
        if (body.position.y < minimumPlayerY)
            return;

        if (body.linearVelocity.y < 0f)
            return;

        gameProgress.ActivateCheckpoint(
            targetRoom,
            respawnPoint.position
        );
    }
}
