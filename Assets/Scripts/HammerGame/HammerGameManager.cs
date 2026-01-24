using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HammerGameManager : MonoBehaviour
{
    public enum HeatState { Normal, Hot, Flaming }

    [Header("End Panels")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    [Header("Refs")]
    public HammerSwing hammerSwing;

    [Header("Rules")]
    public float roundTimeSeconds = 10f;

    [Header("Charge")]
    public float maxCharge = 100f;
    public float chargePerClick = 6f;
    public float decayPerSecond = 18f;

    [Header("Heat Thresholds")]
    public float hotThreshold = 40f;
    public float flameThreshold = 80f;

    [Header("Bell (Best score)")]
    [Range(0.0f, 1.0f)]
    public float bellThreshold01 = 0.95f;

    [Header("UI")]
    public TMP_Text timeText;

    [Header("Scene Objects")]
    public HammerVisual hammer;
    public MeterIndicator indicator;

    [Header("Flow")]
    public string levelSelectorSceneName = "LevelSelector";
    public float returnDelaySeconds = 5f;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip clickClip;
    public AudioClip hitClip;
    public AudioClip flameOnClip;  
    public AudioClip bellSound;  
    
    [Header("Ambience")]
    public AudioSource ambienceSource;


    private float timeLeft;
    private float charge;
    private HeatState currentState = HeatState.Normal;
    private bool hasHit;
    private bool bellRang;


    void Start()
    {
        bellRang = false;
        timeLeft = roundTimeSeconds;
        charge = 0f;
        hasHit = false;

        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);

        if (indicator != null)
        indicator.OnBell += OnBellRang; 

        if (hammer != null)
            hammer.SetState(HeatState.Normal);

        if (indicator != null)
            indicator.ResetToBottom();

        if (indicator != null)
            indicator.OnBell += PlayBell;


        RefreshUI();
    }

    void Update()
    {
        if (hasHit) return;

       
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            Hit();
            return;
        }

       
        if (charge > 0f)
        {
            charge -= decayPerSecond * Time.deltaTime;
            charge = Mathf.Clamp(charge, 0f, maxCharge);
            UpdateHeatState(); 
        }

        RefreshUI();
    }

    public void AddCharge(float amount)
    {
        if (hasHit) return;

        charge = Mathf.Clamp(charge + amount, 0f, maxCharge);
        UpdateHeatState();

        if (sfxSource != null && clickClip != null)
            sfxSource.PlayOneShot(clickClip);
    }

    private void UpdateHeatState()
    {
        HeatState newState;

        if (charge >= flameThreshold)
            newState = HeatState.Flaming;
        else if (charge >= hotThreshold)
            newState = HeatState.Hot;
        else
            newState = HeatState.Normal;

        if (newState != currentState)
        {
          
            bool enteredFlaming = (currentState != HeatState.Flaming && newState == HeatState.Flaming);

            currentState = newState;

            if (hammer != null)
                hammer.SetState(currentState);

            if (enteredFlaming)
            {
               
                if (sfxSource != null && flameOnClip != null)
                    sfxSource.PlayOneShot(flameOnClip);
            }
        }
    }

    private void Hit()
{
    hasHit = true;

    // son de frappe
    if (sfxSource != null && hitClip != null)
        sfxSource.PlayOneShot(hitClip);

    // animation marteau
    if (hammerSwing != null)
        hammerSwing.PlayHit();

    // calc puissance
    float power01 = (maxCharge <= 0f) ? 0f : (charge / maxCharge);
    power01 = Mathf.Clamp01(power01);

    // Si tu utilises le "snap" pour rendre la cloche atteignable :
    if (power01 >= 0.95f) power01 = 1f;

    // fait monter l'indicator (et d�clenchera OnBellRang si power01==1)
    if (indicator != null)
        indicator.ShowScore(power01);

    // stop ambiance si tu veux
    if (ambienceSource != null)
        ambienceSource.Stop();

    // D�cide victoire/d�faite apr�s que l�indicator ait eu le temps d�arriver en haut
    float wait = (indicator != null) ? indicator.moveTime : 0.25f;
    StartCoroutine(EndAfterIndicator(wait));
}

    private void PlayBell()
    {
        if (sfxSource != null && bellSound != null)
            sfxSource.PlayOneShot(bellSound);
    }

    private IEnumerator ReturnToMap()
    {
        yield return new WaitForSeconds(returnDelaySeconds);
        SceneManager.LoadScene(levelSelectorSceneName);
    }

    private void RefreshUI()
    {
        if (timeText != null)
            timeText.text = Mathf.CeilToInt(timeLeft).ToString();
    }

    private IEnumerator EndAfterIndicator(float wait)
{
    yield return new WaitForSeconds(wait + 0.05f);

    if (bellRang)
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }
    else
    {
        if (defeatPanel != null) defeatPanel.SetActive(true);
    }

    yield return new WaitForSeconds(returnDelaySeconds);
    SceneManager.LoadScene(levelSelectorSceneName);
}


    private void OnBellRang()
{
    bellRang = true;

    PlayBell();
}
}
