
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RoomCamera2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Rooms")]
    [SerializeField] private float roomHeight = 12f;
    [SerializeField] private int roomCount = 3;

    public int CurrentRoom { get; private set; } = -1;

    private void Awake()
    {
        Camera cam = GetComponent<Camera>();

        cam.orthographic = true;
        cam.orthographicSize = 6f;
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning(
                "RoomCamera2D: Player reference is missing.",
                this
            );
            return;
        }

        int roomIndex = Mathf.FloorToInt(
            (player.position.y + roomHeight * 0.5f)
            / roomHeight
        );

        roomIndex = Mathf.Clamp(
            roomIndex,
            0,
            roomCount - 1
        );

        if (roomIndex != CurrentRoom)
        {
            CurrentRoom = roomIndex;

            Debug.Log(
                $"Camera switched to Level {CurrentRoom + 1}. " +
                $"Player Y: {player.position.y:F2}",
                this
            );
        }

        transform.position = new Vector3(
            0f,
            CurrentRoom * roomHeight,
            -10f
        );
    }

    public void ShowRoom(int roomIndex)
    {
        // GameProgress can still request an immediate
        // camera position during respawn.
        CurrentRoom = Mathf.Clamp(
            roomIndex,
            0,
            roomCount - 1
        );

        transform.position = new Vector3(
            0f,
            CurrentRoom * roomHeight,
            -10f
        );
    }
}
