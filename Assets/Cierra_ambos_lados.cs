using UnityEngine;

public class Cierra_ambos_lados : Trap
{
    [Header("Recorrido en L (Adelante y Arriba)")]
    public float moveSpeed = 3f;            // Velocidad constante del recorrido
    public float horizontalDistance = 3f;   // Distancia hacia adelante (+ derecha, - izquierda)
    public float verticalDistance = 2f;     // Distancia hacia arriba (+ arriba, - abajo)

    [Header("Animación de Sierra")]
    public float rotateSpeed = 300f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float distX = Mathf.Abs(horizontalDistance);
        float distY = Mathf.Abs(verticalDistance);
        float totalDistance = distX + distY;

        if (totalDistance > 0f)
        {
            // Calcula cuánto ha avanzado en el ciclo de ida y vuelta
            float traveled = Mathf.PingPong(Time.time * moveSpeed, totalDistance);

            float offsetX = 0f;
            float offsetY = 0f;

            if (traveled <= distX)
            {
                // Primer tramo: movimiento horizontal
                offsetX = traveled * Mathf.Sign(horizontalDistance);
            }
            else
            {
                // Segundo tramo: mantiene el avance horizontal y sube/baja
                offsetX = horizontalDistance;
                offsetY = (traveled - distX) * Mathf.Sign(verticalDistance);
            }

            transform.position = startPosition + new Vector3(offsetX, offsetY, 0f);
        }

        // Gira la sierra constantemente
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    // Dibuja la ruta amarilla en el editor cuando seleccionas la sierra
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = Application.isPlaying ? startPosition : transform.position;
        Vector3 corner = origin + new Vector3(horizontalDistance, 0f, 0f);
        Vector3 end = corner + new Vector3(0f, verticalDistance, 0f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, corner);
        Gizmos.DrawLine(corner, end);
    }
}