using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Salud del Jugador")]
    public int maxHealth = 3;
    public int currentHealth;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log("Vida inicial: " + currentHealth);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detecta automáticamente CUALQUIER script que herede de Trap, esta creado para las trampas, y asi no batallen con el daño, ya que cada trampa puede tener un daño diferente o para que tengan variedades de trampas
        Trap trap = collision.GetComponent<Trap>();

        if (trap != null && trap.damage > 0)
        {
            TakeDamage(trap.damage);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("¡Daño recibido! Vida restante: " + currentHealth);

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = Color.white;
        }
    }

    private void Die()
    {
        Debug.Log("El jugador ha muerto");
        currentHealth = maxHealth;
        transform.position = new Vector3(-2f, 2f, 0f);
    }
}