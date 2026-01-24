using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WheelManager : MonoBehaviour
{
    [Header("UI Tuto")]
    public GameObject panelTuto;

    [Header("Liens")]
    public RoueController scriptRoue;
    public UIManager scriptUI;
    public GameObject ecranVictoire;
    public GameObject ecranDefaite;

    [Header("Audio")]
    public AudioSource musiqueDeFond;
    public AudioSource bruitagesSource;
    public AudioClip sonVictoire;
    public AudioClip sonDefaite;

    [Header("Navigation")]
    public string nomSceneMap = "LevelSelector";

    [Header("Réglages")]
    public int viesDeDepart = 3;

    private int vieJoueur;
    private bool jeuFini = false;
    public bool estEnTuto = false;

    void Start()
    {
        Time.timeScale = 1f;

        // Reset les vies au lancement
        vieJoueur = viesDeDepart;
        PlayerPrefs.SetInt("VieJoueur", vieJoueur);
        PlayerPrefs.Save();

        if (scriptUI != null) scriptUI.UpdateCoeurs(vieJoueur, 0);

        // --- GESTION TUTO ---
        // ✅ TOUJOURS afficher le tuto (on ne vérifie plus TutoRoueVu)
        estEnTuto = true;
        if (panelTuto != null) 
        {
            panelTuto.SetActive(true);
            Debug.Log("TUTO AFFICHÉ");
        }
        else
        {
            Debug.LogError("panelTuto n'est pas assigné dans l'Inspector !");
            estEnTuto = false;
        }

        if (ecranVictoire != null) ecranVictoire.SetActive(false);
        if (ecranDefaite != null) ecranDefaite.SetActive(false);
    }

    public void FermerTuto()
    {
        if (panelTuto != null) panelTuto.SetActive(false);

        if (scriptRoue != null)
        {
            scriptRoue.ActiverInputApresDelai(0.5f);
        }

        estEnTuto = false;
        Debug.Log("TUTO FERMÉ - JEU LANCÉ");
    }

    public void Victoire()
    {
        if (jeuFini) return;
        jeuFini = true;

        Debug.Log("GAGNÉ !");

        if (musiqueDeFond != null) musiqueDeFond.Stop();
        if (bruitagesSource != null && sonVictoire != null)
            bruitagesSource.PlayOneShot(sonVictoire);

        CompleteCurrentLevel();

        if (ecranVictoire != null) ecranVictoire.SetActive(true);

        // ✅ Utilise une Coroutine au lieu de Invoke
        StartCoroutine(RetourMapDelai(3f));
    }

    public void Defaite()
    {
        if (jeuFini) return;
        jeuFini = true;

        Debug.Log("PERDU !");

        vieJoueur--;
        PlayerPrefs.SetInt("VieJoueur", vieJoueur);
        PlayerPrefs.Save();

        if (scriptUI != null) scriptUI.UpdateCoeurs(vieJoueur, 0);
        if (musiqueDeFond != null) musiqueDeFond.Stop();
        if (bruitagesSource != null && sonDefaite != null)
            bruitagesSource.PlayOneShot(sonDefaite);

        if (ecranDefaite != null) ecranDefaite.SetActive(true);

        if (vieJoueur <= 0)
        {
            // ✅ Utilise une Coroutine
            StartCoroutine(RetourMapDelai(3f));
        }
        else
        {
            StartCoroutine(SoftResetDelai(2f));
        }
    }

    // ✅ NOUVELLE COROUTINE pour le retour à la map
    IEnumerator RetourMapDelai(float delai)
    {
        Debug.Log($"Retour à la map dans {delai} secondes...");
        yield return new WaitForSeconds(delai);
        Debug.Log("Chargement de : " + nomSceneMap);
        SceneManager.LoadScene(nomSceneMap);
    }

    // ✅ NOUVELLE COROUTINE pour le soft reset
    IEnumerator SoftResetDelai(float delai)
    {
        yield return new WaitForSeconds(delai);
        SoftReset();
    }

    void SoftReset()
    {
        jeuFini = false;

        if (ecranDefaite != null) ecranDefaite.SetActive(false);

        if (musiqueDeFond != null) musiqueDeFond.Play();

        if (scriptRoue != null)
        {
            scriptRoue.RelancerLaRoue();
        }
    }

    void CompleteCurrentLevel()
    {
        string currentID = PlayerPrefs.GetString("CurrentLevelID", "");
        int currentIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 0);

        if (!string.IsNullOrEmpty(currentID))
        {
            PlayerPrefs.SetInt($"Level_{currentID}", 2);
        }

        PlayerPrefs.SetInt("JustCompletedLevel", currentIndex);
        PlayerPrefs.Save();

        Debug.Log($"Niveau {currentIndex} validé !");
    }
}