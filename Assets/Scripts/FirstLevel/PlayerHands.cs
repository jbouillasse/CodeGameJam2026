using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // Indispensable pour l'animation

public class PlayerHands : MonoBehaviour
{
    [Header("Réglages")]
    [SerializeField] private int[] clicsNecessaires = new int[] { 15, 20, 25 };

    [Header("Visuel")]
    [SerializeField] private SpriteRenderer monSpriteRenderer;
    [SerializeField] private Sprite[] etatsMain;

    private float chargeActuelle = 0f;
    private int roundIndex = 0;

    public bool peutSpammer = false;
    public bool peutEsquiver = false;
    public bool aEsquive = false;

    // --- VARIABLES POUR L'ANIMATION (NOUVEAU) ---
    private Vector3 posInitiale;
    private Quaternion rotInitiale;

    void Start()
    {
        // On sauvegarde la position exacte de la main au lancement
        posInitiale = transform.localPosition;
        rotInitiale = transform.localRotation;
    }

    void Update()
    {
        bool clicGauche = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool clicDroit = Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;

        if (peutSpammer && clicGauche)
        {
            chargeActuelle += 1f;
            chargeActuelle = Mathf.Clamp(chargeActuelle, 0, GetMaxPourRound());
            MiseAJourVisuel();
        }

        if (peutEsquiver && clicDroit)
        {
            aEsquive = true;
            peutEsquiver = false;
            Debug.Log("ESQUIVE RÉUSSIE !");
        }
    }

    // --- FONCTION D'ANIMATION (NOUVEAU) ---
    public void LancerAnimationClaque()
    {
        StartCoroutine(AnimMouvementClaque());
    }

    IEnumerator AnimMouvementClaque()
    {
        float dureeElan = 0.1f;
        float dureeFrappe = 0.05f;
        float dureeRetour = 0.2f;

        // 1. ÉLAN (La main recule en bas à droite et se penche en arrière)
        float timer = 0;
        while (timer < dureeElan)
        {
            timer += Time.deltaTime;
            float t = timer / dureeElan;
            // Recule de (0.5, -0.5)
            transform.localPosition = Vector3.Lerp(posInitiale, posInitiale + new Vector3(0.5f, -0.5f, 0), t);
            // Pivote de -30 degrés (vers l'arrière)
            transform.localRotation = Quaternion.Lerp(rotInitiale, Quaternion.Euler(0, 0, -30), t);
            yield return null;
        }

        // 2. FRAPPE (La main fonce vers l'ennemi et pivote vers l'avant)
        timer = 0;
        // Cible : un peu à gauche et en haut
        Vector3 posFrappe = posInitiale + new Vector3(-1.5f, 1.0f, 0);
        // Rotation : 45 degrés vers l'avant (la baffe)
        Quaternion rotFrappe = Quaternion.Euler(0, 0, 45);

        while (timer < dureeFrappe)
        {
            timer += Time.deltaTime;
            float t = timer / dureeFrappe;
            transform.localPosition = Vector3.Lerp(transform.localPosition, posFrappe, t);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, rotFrappe, t);
            yield return null;
        }

        // IMPACT (Ici la main est sur la joue de l'ennemi)
        yield return new WaitForSeconds(0.1f);

        // 3. RETOUR (La main revient à sa place calmement)
        timer = 0;
        Vector3 posAvantRetour = transform.localPosition;
        Quaternion rotAvantRetour = transform.localRotation;

        while (timer < dureeRetour)
        {
            timer += Time.deltaTime;
            float t = timer / dureeRetour;
            transform.localPosition = Vector3.Lerp(posAvantRetour, posInitiale, t);
            transform.localRotation = Quaternion.Lerp(rotAvantRetour, rotInitiale, t);
            yield return null;
        }

        // Sécurité pour être sûr qu'elle est bien remise droite
        transform.localPosition = posInitiale;
        transform.localRotation = rotInitiale;
    }

    void MiseAJourVisuel()
    {
        float max = GetMaxPourRound();
        float pourcentage = chargeActuelle / max;

        int indexSprite = 0;
        if (pourcentage >= 0.95f) indexSprite = 3;
        else if (pourcentage > 0.60f) indexSprite = 2;
        else if (pourcentage > 0.30f) indexSprite = 1;
        else indexSprite = 0;

        if (monSpriteRenderer != null) monSpriteRenderer.sprite = etatsMain[indexSprite];
    }

    public bool EstChargeAFond()
    {
        return chargeActuelle >= GetMaxPourRound() * 0.9f;
    }

    public int GetMaxPourRound()
    {
        if (roundIndex >= clicsNecessaires.Length) return clicsNecessaires[clicsNecessaires.Length - 1];
        return clicsNecessaires[roundIndex];
    }

    public void ResetState()
    {
        chargeActuelle = 0;
        aEsquive = false;
        peutSpammer = false;
        peutEsquiver = false;
        MiseAJourVisuel();
    }

    public void AugmenterRound() { roundIndex++; }
}