using UnityEngine;

public class Cierra_move_lados : Trap
{
    [Header("Movimiento Horizontal en Bucle")]
    public float moveSpeed = 2f;       // Velocidad a la que va de lado a lado
    public float moveDistance = 2.5f;  // Distancia del recorrido (positivo = derecha, negativo = izquierda)

    [Header("Animación de Sierra")]
    public float rotateSpeed = 300f;   // Velocidad de giro (pon 0 si no quieres que gire)

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Calcula el desplazamiento horizontal de ida y vuelta
        float offset = Mathf.PingPong(Time.time * moveSpeed, Mathf.Abs(moveDistance));

        if (moveDistance < 0)
        {
            offset = -offset;
        }

        // Aplica el movimiento únicamente en el eje X
        transform.position = new Vector3(startPosition.x + offset, startPosition.y, startPosition.z);

        // Hace girar la sierra sobre su propio eje
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
}