using UnityEngine;
using UnityEngine.SceneManagement;

public class WheelManager : MonoBehaviour
{
    [Header("UI Fin de Jeu")]
    public UIManager scriptUI;      // Pour les cœurs
    public GameObject ecranVictoire; // Image "PASS"
    public GameObject ecranDefaite;  // Image "X"

    [Header("Audio")]
    public AudioSource musiqueDeFond;
    public AudioSource bruitagesSource;
    public AudioClip sonVictoire;
    public AudioClip sonDefaite;

    [Header("Navigation")]
    public string nomSceneMap = "LevelSelector"; // Le nom exact de ta scène Map

    private bool jeuFini = false;
    private int vieJoueur;

    void Start()
    {
        Time.timeScale = 1f; // On s'assure que le temps n'est pas figé

        // 1. Charger la vie actuelle
        vieJoueur = PlayerPrefs.GetInt("VieJoueur", 3);

        // 2. Mettre à jour l'affichage des cœurs
        if (scriptUI != null)
        {
            scriptUI.UpdateCoeurs(vieJoueur, 0);
        }

        // Cacher les écrans de fin
        if (ecranVictoire != null) ecranVictoire.SetActive(false);
        if (ecranDefaite != null) ecranDefaite.SetActive(false);
    }

    // Appelé par la Roue quand c'est VERT
    public void Victoire()
    {
        if (jeuFini) return;
        jeuFini = true;

        Debug.Log("GAGNÉ (PASS)");

        // 1. Audio
        if (musiqueDeFond != null) musiqueDeFond.Stop();
        if (bruitagesSource != null && sonVictoire != null)
        {
            bruitagesSource.PlayOneShot(sonVictoire);
        }

        // 2. Afficher "PASS"
        if (ecranVictoire != null) ecranVictoire.SetActive(true);

        // 3. SAUVEGARDE DE LA PROGRESSION (Ta logique)
        CompleteCurrentLevel();

        // 4. Retour à la map après 3 secondes
        Invoke("RetourMap", 3f);
    }

    // Appelé par la Roue quand c'est ROUGE
    public void Defaite()
    {
        if (jeuFini) return;
        jeuFini = true;

        Debug.Log("PERDU (X) - 1 VIE");

        // 1. PERTE DE VIE (Important !)
        vieJoueur--;
        PlayerPrefs.SetInt("VieJoueur", vieJoueur);
        PlayerPrefs.Save(); // On sauvegarde la blessure

        // Mise à jour visuelle
        if (scriptUI != null) scriptUI.UpdateCoeurs(vieJoueur, 0);

        // 2. Audio
        if (musiqueDeFond != null) musiqueDeFond.Stop();
        if (bruitagesSource != null && sonDefaite != null)
        {
            bruitagesSource.PlayOneShot(sonDefaite);
        }

        // 3. Afficher "X"
        if (ecranDefaite != null) ecranDefaite.SetActive(true);

        // 4. Gestion Game Over ou Retour Map
        if (vieJoueur <= 0)
        {
            Debug.Log("PLUS DE VIE -> GAME OVER");
            // Ici tu pourrais rediriger vers une scène Game Over
            // SceneManager.LoadScene("GameOver");
            Invoke("RetourMap", 3f); // Pour l'instant on retourne à la map
        }
        else
        {
            // Retour à la map sans valider le niveau (pour réessayer plus tard)
            Invoke("RetourMap", 3f);
        }
    }

    // TA FONCTION DE VALIDATION
    void CompleteCurrentLevel()
    {
        // Récupère les infos stockées par le Map Manager avant d'entrer dans le niveau
        string currentID = PlayerPrefs.GetString("CurrentLevelID", "");
        int currentIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 0);

        Debug.Log($"=== VICTOIRE niveau {currentIndex} (ID: {currentID}) ===");

        // Marquer comme complété (2 = LevelState.Completed)
        if (!string.IsNullOrEmpty(currentID))
        {
            PlayerPrefs.SetInt($"Level_{currentID}", 2);
        }

        // Sauvegarder pour dire à la Map : "Hey, je viens de finir celui-là, débloque le suivant !"
        PlayerPrefs.SetInt("JustCompletedLevel", currentIndex);
        PlayerPrefs.Save();

        Debug.Log($"Niveau {currentIndex} complété !");
    }

    void RetourMap()
    {
        SceneManager.LoadScene(nomSceneMap);
    }
}