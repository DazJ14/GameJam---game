
using UnityEngine;

public class ScreenCamera2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Level Settings")]
    [SerializeField] private float roomHeight = 12f;
    [SerializeField] private int roomCount = 3;

    [Header("Camera")]
    [SerializeField] private float cameraZ = -10f;

    private void LateUpdate()
    {
        if (player == null)
            return;

        // Determine which screen the player occupies.
        int roomIndex = Mathf.FloorToInt(
            (player.position.y + roomHeight / 2f)
            / roomHeight
        );

        roomIndex = Mathf.Clamp(
            roomIndex,
            0,
            roomCount - 1
        );

        // Keep the camera centered on the active room.
        transform.position = new Vector3(
            0f,
            roomIndex * roomHeight,
            cameraZ
        );
    }
}
