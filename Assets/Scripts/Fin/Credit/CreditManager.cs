using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditManager : MonoBehaviour
{
    public float timeBeforeReturn = 10f;
    public string mainMenuScene = "MainMenu"; // Mets ici le nom EXACT de ta scène menu

    void Start()
    {
        Invoke(nameof(LoadMainMenu), timeBeforeReturn);
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}