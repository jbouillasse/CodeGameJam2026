using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game")]
    public float gameDuration = 30f;
    public bool IsRunning { get; private set; }

    [Header("Score")]
    public int Score { get; private set; }

    [Header("Stats")]
    public int Shots { get; private set; }
    public int Kills { get; private set; }

    [Header("Result")]
    public bool victoire = false;

    [Header("Result UI")]
    public Image resultImage;
    public Image resultBack;  
    public Sprite victoireSprite;
    public Sprite defaiteSprite;

    float timeLeft;
    bool ended = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        StartGame();
    }

void ShowResult(Sprite sprite)
{
    if (resultBack != null) resultBack.gameObject.SetActive(true);

    resultImage.sprite = sprite;
    resultImage.gameObject.SetActive(true);
}



    public void StartGame()
    {
        Score = 0;
        Shots = 0;
        Kills = 0;
        victoire = false;

        timeLeft = gameDuration;
        IsRunning = true;
        ended = false;

        Debug.Log("GO !");
    }

    void Update()
    {
        if (!IsRunning) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            IsRunning = false;

            if (!ended)
            {
                ended = true;
                EndGame();
            }
        }
    }

void EndGame()
{
    float accuracy = GetAccuracy();
    int minShots = 20;

    Debug.Log($"FIN ! Score = {Score} | Accuracy = {accuracy:0.0}% ({Kills}/{Shots})");

    if (Shots >= minShots && accuracy >= 90f)
    {
        victoire = true;
        ShowResult(victoireSprite);
        // charge la scene shun
    }
    else
    {
        victoire = false;
        ShowResult(defaiteSprite);
        Invoke(nameof(ReloadScene), 2.5f);
    }
}


    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void AddScore(int amount)
    {
        if (!IsRunning) return;
        Score += amount;
        Debug.Log("Score: " + Score);
    }

    public void AddShot()
    {
        if (!IsRunning) return;
        Shots++;
    }

    public void AddKill()
    {
        if (!IsRunning) return;
        Kills++;
    }

    public float GetAccuracy()
    {
        if (Shots <= 0) return 0f;
        return ((float)Kills / Shots) * 100f;
    }

    public float GetTimeLeft()
    {
        return timeLeft;
    }
}
