using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppleGameManager : MonoBehaviour
{
    [Header("Rules")]
    public float roundTimeSeconds = 20f;
    public int applesToEat = 10;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip victoryClip;
    public AudioClip defeatClip;

    [Header("Instructions")]
    public GameObject instructionsText;
    public float instructionsDuration = 3f;

    [Header("UI")]
    public TMP_Text timeText;
    public TMP_Text scoreText;
    public GameObject victoryPanel;
    public GameObject defeatPanel; 

    [Header("Scene Objects")]
    public GameObject appleObject;   
    public GameObject coresPile; 

    [Header("Flow")]
    public float returnDelaySeconds = 10f;
    public string levelSelectorSceneName = "LevelSelector";

    public string unlockedKey = "UnlockedLevel";
    public int firstLevelValue = 1;

    public bool IsRunning { get; private set; } = true;

    private float timeLeft;
    private int applesEaten;

    void Start()
    {
        timeLeft = roundTimeSeconds;
        applesEaten = 0;
        IsRunning = false;
        
        if (sfxSource == null)
        sfxSource = GetComponent<AudioSource>();

        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
        if (coresPile != null) coresPile.SetActive(false);

        if (appleObject != null) appleObject.SetActive(true);

        if (instructionsText != null)
        instructionsText.SetActive(true);

        RefreshUI();

        Invoke(nameof(StartGame), instructionsDuration);
    }

    private void StartGame()
{
    IsRunning = true;

    if (instructionsText != null)
        instructionsText.SetActive(false);
}


    void Update()
    {
        if (!IsRunning) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndGame(win: false);
        }

        RefreshUI();
    }

    public void EatApple()
    {
        if (!IsRunning) return;

        applesEaten++;

        if (applesEaten >= applesToEat)
            EndGame(win: true);

        RefreshUI();
    }

    private void EndGame(bool win)
{
    if (!IsRunning) return;
    IsRunning = false;

    if (appleObject != null) appleObject.SetActive(false);

    if (sfxSource != null)
    {
        sfxSource.Stop();

        if (win && victoryClip != null)
            sfxSource.PlayOneShot(victoryClip);
        else if (!win && defeatClip != null)
            sfxSource.PlayOneShot(defeatClip);
    }

    if (win)
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
        if (coresPile != null) coresPile.SetActive(true);
        
        // ✅ NOUVEAU : Marquer le niveau comme complété
        CompleteCurrentLevel();
    }
    else
    {
        if (defeatPanel != null) defeatPanel.SetActive(true);
    }

    Invoke(nameof(ReturnToLevelSelector), returnDelaySeconds);
}

// ✅ NOUVELLE MÉTHODE : Compléter le niveau et débloquer le suivant
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
    // On sauvegarde l'index pour que LevelMapManager débloque au retour
    PlayerPrefs.SetInt("JustCompletedLevel", currentIndex);
    PlayerPrefs.Save();
    
    Debug.Log($"Niveau {currentIndex} complété ! Le niveau {currentIndex + 1} sera débloqué.");
}

    private void ReturnToLevelSelector()
    {
        SceneManager.LoadScene(levelSelectorSceneName);
    }

    private void RefreshUI()
    {
        if (timeText != null)
            timeText.text = Mathf.CeilToInt(timeLeft).ToString();

        if (scoreText != null)
            scoreText.text = $"{applesEaten} / {applesToEat}";
    }
}
