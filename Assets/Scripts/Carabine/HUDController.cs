using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text killText;

    [Header("Timer")]
    [SerializeField] private float gameDuration = 30f; // durée partie en secondes

    private float timeLeft;
    private int kills;

    public float TimeLeft => timeLeft;
    public int Kills => kills;

    void Start()
    {
        timeLeft = gameDuration;
        kills = 0;
        RefreshUI();
    }

    void Update()
    {
        if (timeLeft <= 0f) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;

        RefreshTimer();

    }

    public void AddKill(int amount = 1)
    {
        kills += amount;
        RefreshKills();
    }

    private void RefreshUI()
    {
        RefreshTimer();
        RefreshKills();
    }

    private void RefreshTimer()
    {
        // format mm:ss
        int totalSeconds = Mathf.CeilToInt(timeLeft);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void RefreshKills()
    {
        killText.text = kills.ToString();
    }
}
