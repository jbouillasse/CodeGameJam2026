using UnityEngine;
using System.Collections;

public class RoueController : MonoBehaviour
{
    [Header("Réglages")]
    public float vitesseMax = 800f;
    public float dureeFreinage = 3f;
    public AnimationCurve courbeFreinage;

    [Header("Physique (Le Scanner)")]
    public Transform pointDeDetection; // L'objet vide au bout de la flèche
    public LayerMask layerDesColliders; // Mettre sur "Default" ou "Everything"
    public float rayonDetection = 0.2f; // Taille de la zone de détection

    [Header("Juice")]
    public Transform flecheTransform;
    public AudioSource sourceAudio;
    public AudioClip sonTick;

    [Header("Lien Manager")]
    public WheelManager scriptWheelManager;

    private bool estEnRotation = true;
    private bool boutonClique = false;
    private float tempsFreinageActuel = 0f;
    private float vitesseActuelle;

    // SÉCURITÉ : Empêche de cliquer pendant le tuto ou juste après
    private bool inputAutorise = true;

    // Variables pour le son "Tick"
    private Collider2D dernierColliderTouche;

    void Start()
    {
        vitesseActuelle = vitesseMax;
        if (courbeFreinage.length == 0)
            courbeFreinage = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));

        // Si le tuto est actif au lancement, on interdit le clic
        if (scriptWheelManager != null && scriptWheelManager.estEnTuto)
        {
            inputAutorise = false;
        }
    }

    // Appelée par le WheelManager quand on ferme le tuto
    public void ActiverInputApresDelai(float delai)
    {
        StartCoroutine(DelaiInput(delai));
    }

    IEnumerator DelaiInput(float delai)
    {
        inputAutorise = false;
        yield return new WaitForSeconds(delai);
        inputAutorise = true; // On peut enfin cliquer !
    }

    void Update()
    {
        // On ne peut cliquer que si inputAutorise est VRAI
        if (Input.GetMouseButtonDown(0) && !boutonClique && inputAutorise)
        {
            boutonClique = true;
        }

        if (estEnRotation)
        {
            transform.Rotate(0, 0, -vitesseActuelle * Time.deltaTime);

            // --- GESTION DU SON VIA PHYSIQUE ---
            GererLeTickPhysique();
            // -----------------------------------

            if (boutonClique)
            {
                tempsFreinageActuel += Time.deltaTime;
                float pourcentage = tempsFreinageActuel / dureeFreinage;
                vitesseActuelle = vitesseMax * courbeFreinage.Evaluate(pourcentage);

                if (vitesseActuelle <= 10f || pourcentage >= 1f)
                {
                    vitesseActuelle = 0;
                    estEnRotation = false;
                    CalculerResultatPhysique(); // On lance la détection finale
                }
            }
        }
    }

    void GererLeTickPhysique()
    {
        // On utilise un CERCLE au lieu d'un point, c'est plus fiable
        Collider2D colliderSousLaFleche = Physics2D.OverlapCircle(pointDeDetection.position, rayonDetection, layerDesColliders);

        if (colliderSousLaFleche != dernierColliderTouche && colliderSousLaFleche != null)
        {
            dernierColliderTouche = colliderSousLaFleche;

            if (sourceAudio != null && sonTick != null) sourceAudio.PlayOneShot(sonTick);
            if (flecheTransform != null) StartCoroutine(AnimFleche());
        }
    }

    void CalculerResultatPhysique()
    {
        // Scan final
        Collider2D resultat = Physics2D.OverlapCircle(pointDeDetection.position, rayonDetection, layerDesColliders);

        if (resultat != null)
        {
            Debug.Log("La flèche touche : " + resultat.gameObject.tag);

            // ATTENTION AUX MAJUSCULES DANS TES TAGS UNITY !
            if (resultat.CompareTag("Pass"))
            {
                scriptWheelManager.Victoire();
            }
            else if (resultat.CompareTag("Fail"))
            {
                scriptWheelManager.Defaite();
            }
            else
            {
                Debug.LogWarning("Tag inconnu détecté : " + resultat.tag);
                scriptWheelManager.Defaite(); // Par sécurité
            }
        }
        else
        {
            Debug.LogError("La flèche ne touche rien !");
            scriptWheelManager.Defaite();
        }
    }

    IEnumerator AnimFleche()
    {
        flecheTransform.localRotation = Quaternion.Euler(0, 0, 25f);
        yield return new WaitForSeconds(0.05f);
        flecheTransform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void RelancerLaRoue()
    {
        vitesseActuelle = vitesseMax;
        estEnRotation = true;
        boutonClique = false;
        tempsFreinageActuel = 0f;
        inputAutorise = true;
    }

    // DESSIN DEBUG : Pour voir la zone de détection dans la scène
    void OnDrawGizmos()
    {
        if (pointDeDetection != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pointDeDetection.position, rayonDetection);
        }
    }
}