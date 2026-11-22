
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HazardRespawn2D : MonoBehaviour
{
    [SerializeField] private GameProgress gameProgress;

    private Collider2D hazardCollider;

    private void Awake()
    {
        hazardCollider = GetComponent<Collider2D>();
        hazardCollider.isTrigger = true;

        if (gameProgress == null)
            gameProgress = FindAnyObjectByType<GameProgress>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameProgress == null)
            return;

        Rigidbody2D body = other.attachedRigidbody;

        if (body == null)
            return;

        // Only damage the actual player.
        if (body != gameProgress.PlayerBody)
            return;

        gameProgress.KillPlayer();
    }
}
