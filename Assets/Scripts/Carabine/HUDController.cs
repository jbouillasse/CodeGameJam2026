using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text killText;

    private int kills;

    public int Kills => kills;

    void Start()
    {
        kills = 0;
        RefreshUI();
    }

    void Update()
    {
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
        // ✅ Lit le temps depuis GameManager
        float timeLeft = 0f;
        
        if (GameManager.Instance != null)
        {
            timeLeft = GameManager.Instance.GetTimeLeft();
        }

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