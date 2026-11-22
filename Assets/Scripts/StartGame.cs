using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [Header("Panel de Opciones")]
    public GameObject panelOpciones;

    public void Jugar()
    {
        int siguienteIndice = SceneManager.GetActiveScene().buildIndex + 1;

        if (siguienteIndice < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(siguienteIndice);
        }
        else
        {
            Debug.LogWarning("No hay escena en el índice " + siguienteIndice + ". Crea la escena y agrégala en File > Build Settings.");
        }
    }

    public void AlternarOpciones()
    {
        if (panelOpciones != null)
        {
            panelOpciones.SetActive(!panelOpciones.activeSelf);
        }
        else
        {
            Debug.LogWarning("Asigna 'Panel Opciones' en el Inspector antes de pulsar Opciones.");
        }
    }
}