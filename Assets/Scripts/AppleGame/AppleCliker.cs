using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class AppleClicker : MonoBehaviour
{
    public int bitesPerApple = 5;
    public Sprite[] biteStages;
    public SpriteRenderer spriteRenderer;
    public AppleGameManager gameManager;
    private AudioSource audioSource;

    private int bites;
    private Collider2D col; 

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        ResetApple();
    }

    void Update()
{
    if (gameManager == null || !gameManager.IsRunning) return;

    Vector2? screenPos = GetClickScreenPosition();
    if (screenPos == null) return;

    var cam = Camera.main;
    if (cam == null) return;

    Vector3 sp = new Vector3(screenPos.Value.x, screenPos.Value.y, -cam.transform.position.z);
    Vector3 world = cam.ScreenToWorldPoint(sp);

    if (col != null && col.OverlapPoint(world))
        Bite();
}


    private Vector2? GetClickScreenPosition()
    {
#if ENABLE_INPUT_SYSTEM
        // Nouveau Input System
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return Mouse.current.position.ReadValue();

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return Touchscreen.current.primaryTouch.position.ReadValue();
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        // Ancien input
        if (Input.GetMouseButtonDown(0))
            return Input.mousePosition;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            return Input.GetTouch(0).position;
#endif
        return null;
    }

    private void Bite()
    {
        if (audioSource != null)
        audioSource.Play();

        bites++;
        UpdateStageSprite();

        if (bites >= bitesPerApple)
        {
            if (gameManager != null) gameManager.EatApple();
            ResetApple();
        }
    }

    private void ResetApple()
    {
        bites = 0;
        UpdateStageSprite();
    }

    private void UpdateStageSprite()
    {
        if (biteStages == null || biteStages.Length == 0 || spriteRenderer == null) return;

        float t = (bitesPerApple <= 1) ? 1f : (float)bites / (bitesPerApple - 1);
        int idx = Mathf.Clamp(Mathf.RoundToInt(t * (biteStages.Length - 1)), 0, biteStages.Length - 1);
        spriteRenderer.sprite = biteStages[idx];
    }
}
