
using UnityEngine;
using UnityEngine.InputSystem;

public class GameProgress : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private PoleController poleController;
    [SerializeField] private RoomCamera2D roomCamera;
    [SerializeField] private Transform initialSpawn;

    public Rigidbody2D PlayerBody => playerBody;
    public int CurrentRoom { get; private set; }

    private Vector2 respawnPosition;

    private void Start()
    {
        CurrentRoom = 0;

        respawnPosition = initialSpawn != null
            ? (Vector2)initialSpawn.position
            : playerBody.position;

        roomCamera.ShowRoom(0);
    }

    private void Update()
    {
        // Temporary testing key.
        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame)
        {
            Respawn();
        }
    }

    public void ActivateCheckpoint(
        int roomIndex,
        Vector2 newSpawnPosition)
    {
        // Only advance to a new room.
        if (roomIndex <= CurrentRoom)
            return;

        CurrentRoom = roomIndex;
        respawnPosition = newSpawnPosition;

        roomCamera.ShowRoom(CurrentRoom);

        Debug.Log(
            $"Checkpoint reached: Level {CurrentRoom + 1}"
        );
    }

    public void Respawn()
    {
        // Release the pole before moving the player.
        if (poleController != null)
            poleController.ResetAfterRespawn();

        playerBody.position = respawnPosition;
        playerBody.linearVelocity = Vector2.zero;
        playerBody.angularVelocity = 0f;

        roomCamera.ShowRoom(CurrentRoom);
    }

    // Hazards can call this method later.
    public void KillPlayer()
    {
        Respawn();
    }
}
