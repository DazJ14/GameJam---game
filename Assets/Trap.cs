using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Configuración de Trampa")]
    public float bounceForce = 10f;
    public int damage = 1;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerBounce(collision.gameObject);
        }
    }

    protected virtual void HandlePlayerBounce(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Reinicia la velocidad vertical antes de aplicar el impulso
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            // Aplica la fuerza de rebote hacia arriba
            rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
        }
    }
}