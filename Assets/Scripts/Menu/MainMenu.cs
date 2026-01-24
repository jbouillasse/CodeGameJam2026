using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        // Continuer la partie (sans reset)
        SceneManager.LoadScene(1);
    }

    public void NewGame()
    {
        // Nouvelle partie (reset tout)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        Debug.Log("=== NOUVELLE PARTIE ===");
        
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OpenGameJamWebsite()
    {
        Application.OpenURL("https://cgj.bpaul.fr/");
    }
}