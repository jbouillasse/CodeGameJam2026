using UnityEngine;
using System.Collections;

public class RoueController : MonoBehaviour
{
    [Header("Réglages")]
    public float vitesseMax = 800f;
    public float dureeFreinage = 3f;
    public AnimationCurve courbeFreinage;

    [Header("Juice (Optionnel)")]
    public Transform flecheTransform;
    public AudioSource sourceAudio;
    public AudioClip sonTick;

    [Header("Lien OBLIGATOIRE vers le Manager")]
    public WheelManager scriptWheelManager; // <--- C'est ici qu'on relie les deux !

    private bool estEnRotation = true;
    private bool boutonClique = false;
    private float tempsFreinageActuel = 0f;
    private float vitesseActuelle;
    private int dernierIndexSegment = -1;

    void Start()
    {
        vitesseActuelle = vitesseMax;
        if (courbeFreinage.length == 0)
            courbeFreinage = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
    }

    void Update()
    {
        // Clic pour freiner
        if (Input.GetMouseButtonDown(0) && !boutonClique) boutonClique = true;

        if (estEnRotation)
        {
            transform.Rotate(0, 0, -vitesseActuelle * Time.deltaTime);

            GererLeTick(); // Le bruit tic-tic

            if (boutonClique)
            {
                tempsFreinageActuel += Time.deltaTime;
                float pourcentage = tempsFreinageActuel / dureeFreinage;
                vitesseActuelle = vitesseMax * courbeFreinage.Evaluate(pourcentage);

                if (vitesseActuelle <= 10f || pourcentage >= 1f)
                {
                    vitesseActuelle = 0;
                    estEnRotation = false;
                    CalculerResultat();
                }
            }
        }
    }

    void GererLeTick()
    {
        float angleActuel = transform.eulerAngles.z % 360;
        int indexActuel = Mathf.FloorToInt((angleActuel + 22.5f) / 45f);

        if (indexActuel != dernierIndexSegment)
        {
            dernierIndexSegment = indexActuel;
            if (sourceAudio != null && sonTick != null) sourceAudio.PlayOneShot(sonTick);
            if (flecheTransform != null) StartCoroutine(AnimFleche());
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

        if (flecheTransform != null) flecheTransform.localRotation = Quaternion.identity;
    }

    void CalculerResultat()
    {
        float angleFinal = transform.eulerAngles.z % 360;
        float angleCorrige = (angleFinal + 22.5f) % 360;
        int segmentIndex = Mathf.FloorToInt(angleCorrige / 45f);

        bool estGagne = (segmentIndex % 2 == 0);

        if (scriptWheelManager != null)
        {
            if (estGagne) scriptWheelManager.Victoire();
            else scriptWheelManager.Defaite();
        }
        else
        {
            Debug.LogError("OUBLI : Tu n'as pas mis le GAME MANAGER dans la case du script de la Roue !");
        }
    }
}