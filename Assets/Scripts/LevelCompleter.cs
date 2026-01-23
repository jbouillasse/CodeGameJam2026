using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleter : MonoBehaviour
{
    [Header("Scène de la map")]
    public string mapSceneName = "LevelSelectMap";
    
    public void WinLevel()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevelIndex", 0);
        LevelMapManager.Instance?.CompleteLevel(currentLevel);
        SceneManager.LoadScene(mapSceneName);
    }
    
    public void ReturnToMap()
    {
        SceneManager.LoadScene(mapSceneName);
    }
}