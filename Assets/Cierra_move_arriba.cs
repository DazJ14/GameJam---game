using UnityEngine;

public class Cierra_move_arriba : Trap
{
    [Header("Movimiento en Bucle")]
    public float moveSpeed = 2f;       // Velocidad a la que sube y baja
    public float moveDistance = 1.5f;  // Distancia del recorrido (usa negativo para que se esconda hacia abajo)

    [Header("Animación de Sierra")]
    public float rotateSpeed = 300f; 
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Calcula un movimiento continuo de ida y vuelta entre 0 y moveDistance
        float offset = Mathf.PingPong(Time.time * moveSpeed, Mathf.Abs(moveDistance));

        // Si moveDistance es negativo, se mueve hacia abajo; si es positivo, hacia arriba
        if (moveDistance < 0)
        {
            offset = -offset;
        }

        transform.position = new Vector3(startPosition.x, startPosition.y + offset, startPosition.z);

        // Hace girar la sierra sobre su propio eje
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
}