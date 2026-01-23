using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    // On garde juste la position locale originale (normalement 0,0,0)
    private Vector3 originalLocalPos;

    void Awake() { Instance = this; }

    void Start()
    {
        originalLocalPos = transform.localPosition;
    }

    public void Secouer(float duree, float puissance)
    {
        // Si une secousse est déjà en cours, on l'arrête pour éviter les bugs
        StopAllCoroutines();
        StartCoroutine(ShakeCo(duree, puissance));
    }

    IEnumerator ShakeCo(float duree, float puissance)
    {
        float tempsEcoule = 0f;

        while (tempsEcoule < duree)
        {
            // On génère un décalage aléatoire
            float x = Random.Range(-1f, 1f) * puissance;
            float y = Random.Range(-1f, 1f) * puissance;

            // IMPORTANT : On touche au localPosition, pas au position
            transform.localPosition = originalLocalPos + new Vector3(x, y, 0);

            tempsEcoule += Time.deltaTime;
            yield return null;
        }

        // On remet la caméra à sa place locale exacte (0,0,0)
        transform.localPosition = originalLocalPos;
    }
}