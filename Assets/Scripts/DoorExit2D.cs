
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class DoorExit2D : MonoBehaviour
{
    [SerializeField] private Door2D door;
    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private string winScene = "Win";

    private bool finishing;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryFinish(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryFinish(other);
    }

    private void TryFinish(Collider2D other)
    {
        if (finishing || door == null || !door.IsOpen)
            return;

        if (other.attachedRigidbody != playerBody)
            return;

        finishing = true;
        SceneManager.LoadScene(winScene);
    }
}
