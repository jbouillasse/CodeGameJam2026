using UnityEngine;
using System.Collections;
using TMPro; // Si tu l'utilises sur du texte

public class JuicyEffect : MonoBehaviour
{
    [Header("Réglages")]
    public float vitessePop = 10f;
    public float tailleMax = 1.5f; // Il grossit jusqu'à 150% puis revient

    // S'active automatiquement quand l'objet apparaît (SetActive true)
    void OnEnable()
    {
        StartCoroutine(AnimPop());
    }

    IEnumerator AnimPop()
    {
        // 1. On commence tout petit (invisible)
        transform.localScale = Vector3.zero;
        float timer = 0f;

        // 2. On grossit très vite (Effet Elastique)
        while (timer < 1f)
        {
            timer += Time.deltaTime * vitessePop;
            // Courbe mathématique pour faire un rebond (Overshoot)
            float scale = Mathf.Sin(timer * Mathf.PI) * (tailleMax - 1f) + 1f;

            // On applique la taille, mais on bloque à 1 à la fin
            if (timer >= 1f) scale = 1f;

            transform.localScale = Vector3.one * scale;
            yield return null;
        }
        transform.localScale = Vector3.one;
    }
}