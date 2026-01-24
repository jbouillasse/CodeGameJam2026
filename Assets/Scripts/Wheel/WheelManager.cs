using UnityEngine;
using UnityEngine.SceneManagement;

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

    private int vieJoueur;
    private bool jeuFini = false;
    public bool estEnTuto = false;

    void Start()
    {
        Time.timeScale = 1f;

        // Charger vie et UI
        vieJoueur = PlayerPrefs.GetInt("VieJoueur", 3);
        if (scriptUI != null) scriptUI.UpdateCoeurs(vieJoueur, 0);

        // --- GESTION TUTO ---
        // Si tu veux revoir le tuto : Fais "Edit -> Clear All PlayerPrefs" dans Unity
        if (PlayerPrefs.GetInt("TutoRoueVu", 0) == 0)
        {
            estEnTuto = true;
            if (panelTuto != null) panelTuto.SetActive(true);
        }
        else
        {
            estEnTuto = false;
            if (panelTuto != null) panelTuto.SetActive(false);
        }

        if (ecranVictoire != null) ecranVictoire.SetActive(false);
        if (ecranDefaite != null) ecranDefaite.SetActive(false);
    }

    public void FermerTuto()
    {
        if (panelTuto != null) panelTuto.SetActive(false); // Cache le visuel

        PlayerPrefs.SetInt("TutoRoueVu", 1); // Sauvegarde pour ne plus l'afficher
        PlayerPrefs.Save();

        // ACTIVE LE BOUCLIER ICI
        if (scriptRoue != null)
        {
            scriptRoue.ActiverInputApresDelai(0.5f);
        }

        estEnTuto = false; // Le tuto est officiellement fini
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

        Invoke("RetourMap", 3f);
    }

    public void Defaite()
    {
        if (jeuFini) return;
        jeuFini = true;

        Debug.Log("PERDU !");

        // Perdre vie
        vieJoueur--;
        PlayerPrefs.SetInt("VieJoueur", vieJoueur);
        PlayerPrefs.Save();

        if (scriptUI != null) scriptUI.UpdateCoeurs(vieJoueur, 0);
        if (musiqueDeFond != null) musiqueDeFond.Stop(); // On coupe la musique un instant
        if (bruitagesSource != null && sonDefaite != null)
            bruitagesSource.PlayOneShot(sonDefaite);

        if (ecranDefaite != null) ecranDefaite.SetActive(true);

        // --- CORRECTION ICI ---
        if (vieJoueur <= 0)
        {
            // Plus de vie : On retourne à la map (Game Over)
            Invoke("RetourMap", 3f);
        }
        else
        {
            // Encore de la vie : On relance la roue après 2 secondes !
            Invoke("SoftReset", 2f);
        }
    }

    // NOUVELLE FONCTION POUR RELANCER SANS CHARGER LA SCENE
    void SoftReset()
    {
        jeuFini = false; // On débloque le jeu

        // On cache l'écran "X"
        if (ecranDefaite != null) ecranDefaite.SetActive(false);

        // On remet la musique
        if (musiqueDeFond != null) musiqueDeFond.Play();

        // On dit à la roue de tourner à nouveau
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

    void RetourMap()
    {
        SceneManager.LoadScene(nomSceneMap);
    }
}