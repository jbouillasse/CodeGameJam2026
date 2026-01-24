using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Chamboule : MonoBehaviour
{
    [Header("UI Règles (Tuto)")]
    public GameObject panneauRegles;

    [Header("UI & Sons")]
    public TextMeshProUGUI texteAffichage;
    public TextMeshProUGUI endText;

    public AudioSource sourceAudio;
    public AudioClip musiqueDeFond;
    public AudioClip sonVictoire;
    public AudioClip sonDefaite;

    [Header("Réglages")]
    public string nomSceneMenu = "LevelSelector";
    public float tempsAttenteFin = 3f;

    [Header("Debug")]
    public int totalCanettes = 0;
    public int canettesTombees = 0;
    public bool partieFinie = false;

    private LanceurBoule leLanceur;

    void Start()
    {
        leLanceur = FindObjectOfType<LanceurBoule>();
        if (leLanceur != null)
        {
            leLanceur.enabled = false;
            leLanceur.GetComponent<Rigidbody2D>().simulated = false;
        }

        if (panneauRegles != null)
        {
            panneauRegles.SetActive(true);
        }

        if (sourceAudio != null && musiqueDeFond != null)
        {
            sourceAudio.clip = musiqueDeFond;
            sourceAudio.loop = true;
            sourceAudio.Play();
        }

        totalCanettes = GameObject.FindGameObjectsWithTag("Canette").Length;
        if (texteAffichage != null) texteAffichage.text = "Canettes : 0 / " + totalCanettes;
        if (endText != null) endText.gameObject.SetActive(false);
    }

    public void CommencerLeJeu()
    {
        if (panneauRegles != null) panneauRegles.SetActive(false);

        if (leLanceur != null)
        {
            leLanceur.enabled = true;
            leLanceur.GetComponent<Rigidbody2D>().simulated = true;
        }
    }

    void OnTriggerEnter2D(Collider2D autre)
    {
        if (autre.CompareTag("Canette") && !partieFinie)
        {
            canettesTombees++;
            if (texteAffichage != null) texteAffichage.text = "Canettes : " + canettesTombees + " / " + totalCanettes;
            autre.gameObject.SetActive(false);
            VerifierVictoire();
        }
    }

    void VerifierVictoire()
    {
        if (canettesTombees >= totalCanettes)
        {
            DeclencherFin(true);
        }
    }

    public void DeclencherFin(bool estVictoire)
    {
        if (partieFinie) return;
        partieFinie = true;

        if (leLanceur != null) leLanceur.gameObject.SetActive(false);

        if (sourceAudio != null) sourceAudio.Stop();

        if (endText != null)
        {
            endText.gameObject.SetActive(true);
            if (estVictoire)
            {
                endText.text = "VICTOIRE !";
                endText.color = Color.yellow;
                if (sourceAudio && sonVictoire) sourceAudio.PlayOneShot(sonVictoire);
            }
            else
            {
                endText.text = "PERDU...";
                endText.color = Color.red;
                if (sourceAudio && sonDefaite) sourceAudio.PlayOneShot(sonDefaite);
            }
        }
        StartCoroutine(RetourAuMenu());
    }

    IEnumerator RetourAuMenu()
    {
        yield return new WaitForSeconds(tempsAttenteFin);
        SceneManager.LoadScene(nomSceneMenu);
    }
}