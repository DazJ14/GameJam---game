using UnityEngine;

public class SpikeTramp_move_lados : Trap
{
    [Header("Movimiento Horizontal en Bucle")]
    public float moveSpeed = 2f;       // Velocidad a la que se mueve de lado a lado
    public float moveDistance = 2.5f;  // Distancia del recorrido (positivo = derecha, negativo = izquierda)

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

        // Mueve los pinchos únicamente en el eje X
        transform.position = new Vector3(startPosition.x + offset, startPosition.y, startPosition.z);
    }

    // Dibuja una línea amarilla en el editor para ver hasta dónde llegan los pinchos
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = Application.isPlaying ? startPosition : transform.position;
        Vector3 end = origin + new Vector3(moveDistance, 0f, 0f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, end);
    }
}