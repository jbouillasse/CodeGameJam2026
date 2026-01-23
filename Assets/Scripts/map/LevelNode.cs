using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum LevelState
{
    Locked,
    Unlocked,
    Completed
}

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class LevelNode : MonoBehaviour
{
    [Header("Configuration")]
    public int levelIndex;
    public string sceneName;
    
    [Header("Mode Test (pas de mini-jeu)")]
    public bool simulateWin = false;
    
    [Header("Sprites")]
    public Sprite lockedSprite;
    public Sprite unlockedSprite;
    
    [Header("Taille des sprites")]
    public float lockedSpriteScale = 1f;
    public float unlockedSpriteScale = 4f;
    
    [Header("Animation")]
    public bool animateChange = true;
    public float animDuration = 0.4f;
    public float bounceScale = 1.5f;
    
    [Header("Events")]
    public UnityEvent onClicked;
    
    private SpriteRenderer sr;
    private LevelState state = LevelState.Locked;
    private Vector3 baseScale;
    private Vector3 currentTargetScale;
    private Camera mainCam;
    private bool isHovered = false;
    
    public string UniqueID => $"{levelIndex}_{gameObject.name}";
    
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
        mainCam = Camera.main;
        
        if (GetComponent<Collider2D>() == null)
            gameObject.AddComponent<CircleCollider2D>();
    }
    
    void Start()
    {
        LoadState();
        ApplyVisual(false);
    }
    
    void Update()
    {
        HandleInput();
    }
    
    private void HandleInput()
    {
        if (mainCam == null) return;
        
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = mainCam.ScreenToWorldPoint(mousePos);
        
        Collider2D col = GetComponent<Collider2D>();
        bool wasHovered = isHovered;
        isHovered = col.OverlapPoint(worldPos);
        
        // Hover effect seulement si jouable (Unlocked ou Completed)
        if (isHovered && !wasHovered)
        {
            if (state != LevelState.Locked)
                transform.localScale = currentTargetScale * 1.15f;
        }
        
        if (!isHovered && wasHovered)
        {
            transform.localScale = currentTargetScale;
        }
        
        if (isHovered && Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnClick();
        }
    }
    
    private void OnClick()
    {
        if (state == LevelState.Locked)
        {
            Debug.Log($"Niveau {levelIndex} verrouillé !");
            StartCoroutine(ShakeAnimation());
            return;
        }
        
        // Unlocked OU Completed = on peut jouer
        if (MapCharacter.Instance != null)
        {
            MapCharacter.Instance.MoveToLevel(this);
        }
        else
        {
            LaunchLevel();
        }
    }
    
    public void LaunchLevel()
    {
        Debug.Log($"Lancement niveau {levelIndex}");
        onClicked?.Invoke();
        
        PlayerPrefs.SetString("CurrentLevelID", UniqueID);
        PlayerPrefs.SetInt("CurrentLevelIndex", levelIndex);
        PlayerPrefs.Save();
        
        if (simulateWin || string.IsNullOrEmpty(sceneName))
        {
            Debug.Log($"=== SIMULATION VICTOIRE niveau {levelIndex} ===");
            LevelMapManager.Instance?.CompleteLevel(levelIndex);
            return;
        }
        
        Debug.Log($"=== CHARGEMENT SCENE: {sceneName} ===");
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    
    public void SetState(LevelState newState, bool animate = true)
    {
        if (state == newState) return;
        
        state = newState;
        SaveState();
        ApplyVisual(animate);
    }
    
    public void Unlock()
    {
        if (state == LevelState.Locked)
            SetState(LevelState.Unlocked, true);
    }
    
    public void Complete()
    {
        SetState(LevelState.Completed, false);
    }
    
    private void ApplyVisual(bool animate)
    {
        switch (state)
        {
            case LevelState.Locked:
                sr.sprite = lockedSprite;
                sr.enabled = true;
                SetAlpha(1f);
                currentTargetScale = baseScale * lockedSpriteScale;
                transform.localScale = currentTargetScale;
                break;
                
            case LevelState.Unlocked:
                sr.sprite = unlockedSprite;
                sr.enabled = true;
                SetAlpha(1f);
                currentTargetScale = baseScale * unlockedSpriteScale;
                if (animate && animateChange)
                    StartCoroutine(BounceAnimation());
                else
                    transform.localScale = currentTargetScale;
                break;
                
            case LevelState.Completed:
                // ✅ Sprite CACHÉ mais collider reste actif !
                sr.enabled = false;
                currentTargetScale = baseScale * unlockedSpriteScale;
                transform.localScale = currentTargetScale;
                // Le collider reste actif donc on peut toujours cliquer
                break;
        }
    }
    
    private void SetAlpha(float alpha)
    {
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }
    
    private System.Collections.IEnumerator BounceAnimation()
    {
        float elapsed = 0f;
        float half = animDuration / 2f;
        
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / half;
            transform.localScale = currentTargetScale * Mathf.Lerp(1f, bounceScale, t);
            yield return null;
        }
        
        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / half;
            transform.localScale = currentTargetScale * Mathf.Lerp(bounceScale, 1f, t);
            yield return null;
        }
        
        transform.localScale = currentTargetScale;
    }
    
    private System.Collections.IEnumerator ShakeAnimation()
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;
        float shakeDuration = 0.3f;
        float shakeAmount = 0.1f;
        
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float x = originalPos.x + Random.Range(-shakeAmount, shakeAmount);
            float y = originalPos.y + Random.Range(-shakeAmount, shakeAmount);
            transform.position = new Vector3(x, y, originalPos.z);
            yield return null;
        }
        
        transform.position = originalPos;
    }
    
    private void SaveState()
    {
        PlayerPrefs.SetInt($"Level_{UniqueID}", (int)state);
        PlayerPrefs.Save();
    }
    
    private void LoadState()
    {
        int saved = PlayerPrefs.GetInt($"Level_{UniqueID}", 0);
        state = (LevelState)saved;
        
        if (levelIndex == 0 && state == LevelState.Locked)
            state = LevelState.Unlocked;
    }
    
    public LevelState GetState() => state;
    public bool IsPlayable() => state != LevelState.Locked;
}