
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private string menuScene = "MainMenu";

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(menuScene);
    }
}
