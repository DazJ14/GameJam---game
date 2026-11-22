
using UnityEngine;
using UnityEngine.InputSystem;

public class StickController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        float distanceToPlane =
            transform.position.z - mainCamera.transform.position.z;

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(
                new Vector3(
                    mouseScreenPosition.x,
                    mouseScreenPosition.y,
                    distanceToPlane
                )
            );

        Vector2 direction =
            mouseWorldPosition - transform.position;

        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        float angle = Mathf.Atan2(
            direction.y,
            direction.x
        ) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }
}
