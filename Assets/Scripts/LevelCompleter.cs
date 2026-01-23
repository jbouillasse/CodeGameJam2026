using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleter : MonoBehaviour
{
    [Header("Scène de la map")]
    public string mapSceneName = "LevelSelectMap";
    
<<<<<<< HEAD
    /// <summary>
    /// Appeler quand le joueur gagne le niveau
    /// </summary>
    public void WinLevel()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        
        // Marquer comme complété
        PlayerPrefs.SetInt($"Level_{currentLevel}", (int)LevelState.Completed);
        
        // Débloquer le suivant
        int nextLevel = currentLevel + 1;
        int nextState = PlayerPrefs.GetInt($"Level_{nextLevel}", 0);
        if (nextState == (int)LevelState.Locked)
        {
            PlayerPrefs.SetInt($"Level_{nextLevel}", (int)LevelState.Unlocked);
        }
        
        PlayerPrefs.Save();
        
        // Retourner à la map
        SceneManager.LoadScene(mapSceneName);
    }
    
    /// <summary>
    /// Si le joueur perd ou quitte
    /// </summary>
=======
    public void WinLevel()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevelIndex", 0);
        LevelMapManager.Instance?.CompleteLevel(currentLevel);
        SceneManager.LoadScene(mapSceneName);
    }
    
>>>>>>> ab858e927267b82c30fb4bc30a8b89436febed95
    public void ReturnToMap()
    {
        SceneManager.LoadScene(mapSceneName);
    }
}