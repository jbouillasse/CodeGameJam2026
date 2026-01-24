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
    }

    public void SetState(HammerGameManager.HeatState state)
    {
        if (spriteRenderer == null) return;

        switch (state)
        {
            case HammerGameManager.HeatState.Normal:
                spriteRenderer.sprite = normalSprite;
                break;

            case HammerGameManager.HeatState.Hot:
                spriteRenderer.sprite = hotSprite;
                break;

            case HammerGameManager.HeatState.Flaming:
                spriteRenderer.sprite = flamingSprite;
                break;
        }

        // petit feedback visuel
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
