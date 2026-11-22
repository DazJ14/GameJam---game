
using UnityEngine;

public class SawRotation2D : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 240f;
    [SerializeField] private bool clockwise = true;

    private void Update()
    {
        float direction = clockwise ? -1f : 1f;

        transform.Rotate(
            0f,
            0f,
            rotationSpeed * direction * Time.deltaTime
        );
    }
}
