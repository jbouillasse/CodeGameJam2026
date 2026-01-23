using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class RaceGameManager : MonoBehaviour
{
    public static RaceGameManager Instance { get; private set; }
    
    [Header("Joueur")]
    public Transform player;
    public Animator playerAnimator;
    public float scorePerClick = 1f;
    
    [Header("Ennemi")]
    public Transform opponent;
    public Animator opponentAnimator;
    public int targetScore = 70;
    
    [Header("Difficulté")]
    [Range(1f, 1.5f)]
    public float opponentDifficulty = 1.2f;
    public float wrongClickPenalty = 2f;
    public float minTimeBetweenClicks = 0.1f;
    
    [Header("Positions X de référence")]
    public float behindPositionX = -4f;
    public float aheadPositionX = 4f;
    public float winExitPositionX = 12f;
    public float positionLerpSpeed = 5f;
    
    [Header("Course")]
    public float raceDuration = 8f;
    
    [Header("UI")]
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI targetScoreText;
    public Image clickIndicator;
    public Sprite souris1;
    public Sprite souris2;
    public TextMeshProUGUI comboText;
    public Slider progressBar;
    public GameObject victoryPanel;
    public GameObject defeatPanel;
    
    [Header("Tutorial")]
    public GameObject tutorialPanel;
    
    [Header("Paramètres")]
    public float countdownTime = 3f;
    public string mapSceneName = "LevelSelectMap";
    
    // État du jeu
    private bool raceStarted = false;
    private bool raceEnded = false;
    private bool expectLeftClick = true;
    private float playerScore = 0f;
    private float timeRemaining;
    private float lastClickTime = 0f;
    
    // Combo
    private int combo = 0;
    
    // Positions X cibles
    private float playerTargetX;
    private float opponentTargetX;
    
    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        // Pause le jeu
        Time.timeScale = 0f;
        
        // Affiche le tutoriel
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }
        
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
        if (comboText != null) comboText.text = "";
        
        timeRemaining = raceDuration;
        
        playerTargetX = behindPositionX;
        opponentTargetX = aheadPositionX;
        
        if (player != null)
            player.position = new Vector3(behindPositionX, player.position.y, player.position.z);
        if (opponent != null)
            opponent.position = new Vector3(aheadPositionX, opponent.position.y, opponent.position.z);
        
        if (playerAnimator != null) playerAnimator.speed = 0f;
        if (opponentAnimator != null) opponentAnimator.speed = 0f;
        
        UpdateUI();
        UpdateClickIndicator();
    }
    
    public void CloseTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
        
        // Relance le temps
        Time.timeScale = 1f;
        
        // Lance le countdown
        StartCoroutine(Countdown());
    }
    
    void Update()
    {
        if (!raceStarted || raceEnded) return;
        
        HandleInput();
        UpdateTimer();
        UpdateLeaderPositions();
        UpdatePositions();
        UpdateUI();
        CheckRaceEnd();
    }
    
    private void HandleInput()
    {
        if (Time.time - lastClickTime < minTimeBetweenClicks) return;
        
        bool leftClicked = Mouse.current.leftButton.wasPressedThisFrame;
        bool rightClicked = Mouse.current.rightButton.wasPressedThisFrame;
        
        if (expectLeftClick && leftClicked)
        {
            combo++;
            float comboBonus = 1f + (combo * 0.05f);
            playerScore += scorePerClick * comboBonus;
            expectLeftClick = false;
            lastClickTime = Time.time;
            UpdateClickIndicator();
            UpdateComboText();
        }
        else if (!expectLeftClick && rightClicked)
        {
            combo++;
            float comboBonus = 1f + (combo * 0.05f);
            playerScore += scorePerClick * comboBonus;
            expectLeftClick = true;
            lastClickTime = Time.time;
            UpdateClickIndicator();
            UpdateComboText();
        }
        else if ((leftClicked && !expectLeftClick) || (rightClicked && expectLeftClick))
        {
            combo = 0;
            playerScore = Mathf.Max(0, playerScore - scorePerClick * wrongClickPenalty);
            lastClickTime = Time.time;
            UpdateComboText();
        }
    }
    
    private void UpdateComboText()
    {
        if (comboText != null)
        {
            if (combo >= 5)
            {
                comboText.text = $"COMBO x{combo}!";
                comboText.color = Color.yellow;
            }
            else if (combo > 0)
            {
                comboText.text = $"x{combo}";
                comboText.color = Color.white;
            }
            else
            {
                comboText.text = "RATÉ!";
                comboText.color = Color.red;
            }
        }
    }
    
    private void UpdateLeaderPositions()
    {
        float playerProgress = playerScore / targetScore;
        float opponentProgress = GetOpponentProgress();
        
        if (playerProgress > opponentProgress)
        {
            playerTargetX = aheadPositionX;
            opponentTargetX = behindPositionX;
        }
        else
        {
            playerTargetX = behindPositionX;
            opponentTargetX = aheadPositionX;
        }
    }
    
    private float GetOpponentProgress()
    {
        float elapsedTime = raceDuration - timeRemaining;
        float opponentFinalScore = targetScore * opponentDifficulty;
        float opponentScore = (elapsedTime / raceDuration) * opponentFinalScore;
        return opponentScore / targetScore;
    }
    
    private void UpdatePositions()
    {
        if (player != null)
        {
            float newX = Mathf.Lerp(
                player.position.x,
                playerTargetX,
                positionLerpSpeed * Time.deltaTime
            );
            player.position = new Vector3(newX, player.position.y, player.position.z);
        }
        
        if (opponent != null)
        {
            float newX = Mathf.Lerp(
                opponent.position.x,
                opponentTargetX,
                positionLerpSpeed * Time.deltaTime
            );
            opponent.position = new Vector3(newX, opponent.position.y, opponent.position.z);
        }
    }
    
    private void UpdateTimer()
    {
        timeRemaining -= Time.deltaTime;
        
        if (timerText != null)
        {
            timerText.text = Mathf.Ceil(timeRemaining).ToString("0");
            
            if (timeRemaining <= 3f)
                timerText.color = Color.red;
            else
                timerText.color = Color.white;
        }
    }
    
    private void UpdateUI()
    {
        if (playerScoreText != null)
            playerScoreText.text = $"{Mathf.Floor(playerScore)}";
        
        if (targetScoreText != null)
            targetScoreText.text = $"/ {targetScore}";
        
        if (progressBar != null)
            progressBar.value = playerScore / targetScore;
    }
    
    private void UpdateClickIndicator()
    {
        if (clickIndicator != null)
        {
            if (expectLeftClick)
            {
                clickIndicator.sprite = souris1;
            }
            else
            {
                clickIndicator.sprite = souris2;
            }
        }
    }
    
    private void CheckRaceEnd()
    {
        if (timeRemaining <= 0)
        {
            bool playerWon = playerScore >= targetScore;
            StartCoroutine(EndRaceSequence(playerWon));
        }
    }
    
    private IEnumerator EndRaceSequence(bool playerWon)
    {
        raceEnded = true;
        
        Transform winner = playerWon ? player : opponent;
        Transform loser = playerWon ? opponent : player;
        
        Animator loserAnimator = playerWon ? opponentAnimator : playerAnimator;
        if (loserAnimator != null) loserAnimator.speed = 0f;
        
        float winnerTargetX = winExitPositionX;
        float loserTargetX = behindPositionX;
        
        float exitDuration = 1.5f;
        float elapsed = 0f;
        
        float winnerStartX = winner.position.x;
        float loserStartX = loser.position.x;
        
        while (elapsed < exitDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / exitDuration;
            
            float easedT = 1f - Mathf.Pow(1f - t, 2f);
            
            float winnerNewX = Mathf.Lerp(winnerStartX, winnerTargetX, easedT);
            float loserNewX = Mathf.Lerp(loserStartX, loserTargetX, t * 0.5f);
            
            winner.position = new Vector3(winnerNewX, winner.position.y, winner.position.z);
            loser.position = new Vector3(loserNewX, loser.position.y, loser.position.z);
            
            yield return null;
        }
        
        Animator winnerAnimator = playerWon ? playerAnimator : opponentAnimator;
        if (winnerAnimator != null) winnerAnimator.speed = 0f;
        
        if (playerWon)
        {
            if (victoryPanel != null) victoryPanel.SetActive(true);
            SaveVictory();
        }
        else
        {
            if (defeatPanel != null) defeatPanel.SetActive(true);
        }
        
        yield return new WaitForSeconds(3f);
        ReturnToMap();
    }
    
    private void SaveVictory()
    {
        string levelID = PlayerPrefs.GetString("CurrentLevelID", "");
        if (!string.IsNullOrEmpty(levelID))
        {
            PlayerPrefs.SetInt($"Level_{levelID}", 2);
            int levelIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 0);
            PlayerPrefs.SetInt("LastCompletedIndex", levelIndex);
            PlayerPrefs.Save();
        }
    }
    
    private IEnumerator Countdown()
    {
        float timer = countdownTime;
        
        while (timer > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = Mathf.Ceil(timer).ToString();
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        
        if (countdownText != null)
        {
            countdownText.text = "GO!";
            yield return new WaitForSeconds(0.5f);
            countdownText.gameObject.SetActive(false);
        }
        
        if (playerAnimator != null) playerAnimator.speed = 1f;
        if (opponentAnimator != null) opponentAnimator.speed = 1f;
        
        raceStarted = true;
    }
    
    public void RetryRace()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void ReturnToMap()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mapSceneName);
    }
    
    public float GetPlayerSpeed()
    {
        if (!raceStarted || raceEnded) return 0f;
        return 3f;
    }
}