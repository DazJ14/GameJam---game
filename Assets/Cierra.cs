using UnityEngine;

public class Cierra : Trap
{
    [Header("Animación de Sierra")]
    public float rotateSpeed = 300f;

    private void Update()
    {
        // Hace girar la sierra sobre su propio eje
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
}