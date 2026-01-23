using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void Play()
    {
        // Load the fist scene (the game scene)
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();

        // Test in editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OpenGameJamWebsite()
    {
        Application.OpenURL("https://cgj.bpaul.fr/");
    }
}
