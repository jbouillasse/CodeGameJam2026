using System.Collections;
using UnityEngine;

public class HammerVisual : MonoBehaviour
{
    [Header("Renderer")]
    public SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite hotSprite;
    public Sprite flamingSprite;

    [Header("Pop Animation")]
    public float popScale = 1.08f;
    public float popTime = 0.12f;

    private Vector3 baseScale;
    private Coroutine popCo;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        baseScale = transform.localScale;
        
        // ✅ FIX : Afficher le sprite normal au démarrage
        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    void Start()
    {
        // ✅ FIX : Double vérification au Start
        if (spriteRenderer != null && normalSprite != null && spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    public void SetState(HammerGameManager.HeatState state)
    {
        if (spriteRenderer == null) return;

        switch (state)
        {
            case HammerGameManager.HeatState.Normal:
                if (normalSprite != null) spriteRenderer.sprite = normalSprite;
                break;

            case HammerGameManager.HeatState.Hot:
                if (hotSprite != null) spriteRenderer.sprite = hotSprite;
                break;

            case HammerGameManager.HeatState.Flaming:
                if (flamingSprite != null) spriteRenderer.sprite = flamingSprite;
                break;
        }

        if (popCo != null) StopCoroutine(popCo);
        popCo = StartCoroutine(Pop());
    }

    private IEnumerator Pop()
    {
        transform.localScale = baseScale * popScale;
        yield return new WaitForSeconds(popTime * 0.5f);
        transform.localScale = baseScale;
    }
}