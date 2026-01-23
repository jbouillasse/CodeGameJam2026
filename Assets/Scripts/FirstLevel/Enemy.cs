using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Composants")]
    public SpriteRenderer monRenderer;
    public EnemyBreathing scriptRespiration;

    [Header("Sprites de la Piñata")]
    public Sprite spriteIdle;
    public Sprite spritePrepa;
    public Sprite spriteAttaque;
    public Sprite spriteMal;
    public Sprite spriteMort;

    public void MettreEnAttente()
    {
        monRenderer.sprite = spriteIdle;
        monRenderer.color = Color.white;
    }

    public void PreparerAttaque()
    {
        monRenderer.sprite = spritePrepa;
    }

    public void PrendreUneClaque()
    {
        monRenderer.sprite = spriteMal;
        transform.position += Vector3.right * 0.5f; 
        Invoke("ResetPosition", 0.2f);
    }

    void ResetPosition()
    {
        transform.position -= Vector3.right * 0.5f;
    }

    public void Mourir()
    {
        monRenderer.sprite = spriteMort;

        if (scriptRespiration != null) scriptRespiration.enabled = false;
    }
}