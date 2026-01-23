using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleter : MonoBehaviour
{
    [Header("Scène de la map")]
    public string mapSceneName = "LevelSelectMap";
    
    public void WinLevel()
    {
        CompleteCurrentLevel();
        
        // Retourner à la map
        SceneManager.LoadScene(mapSceneName);
    }

    /// <summary>
    /// Compléter le niveau et débloquer le suivant
    /// </summary>
    private void CompleteCurrentLevel()
    {
        // Récupérer l'ID du niveau actuel
        string currentID = PlayerPrefs.GetString("CurrentLevelID", "");
        int currentIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 0);

        Debug.Log($"=== VICTOIRE niveau {currentIndex} (ID: {currentID}) ===");

        // Marquer comme complété (2 = LevelState.Completed)
        if (!string.IsNullOrEmpty(currentID))
        {
            PlayerPrefs.SetInt($"Level_{currentID}", 2);
        }

        // Débloquer le niveau suivant
        int nextLevel = currentIndex + 1;
        string nextID = $"level_{nextLevel}";
        int nextState = PlayerPrefs.GetInt($"Level_{nextID}", 0);
        
        // Si le niveau suivant est verrouillé (0), le débloquer (1)
        if (nextState == 0)
        {
            PlayerPrefs.SetInt($"Level_{nextID}", 1);
            Debug.Log($"Niveau {nextLevel} (ID: {nextID}) débloqué !");
        }

        // Sauvegarder l'index pour que LevelMapManager repositionne au retour
        PlayerPrefs.SetInt("JustCompletedLevel", currentIndex);
        PlayerPrefs.Save();

        Debug.Log($"Niveau {currentIndex} complété ! Le niveau {nextLevel} sera débloqué.");
    }
    
    public void ReturnToMap()
    {
        SceneManager.LoadScene(mapSceneName);
    }
}