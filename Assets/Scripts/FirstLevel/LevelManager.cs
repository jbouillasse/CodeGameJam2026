using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Liens")]
    public PlayerHands scriptMainJoueur;
    public Enemy scriptEnnemi;
    public UIManager scriptUI;

    [Header("Paramètres")]
    public int vieMax = 3;
    private int vieJoueur;
    private int vieEnnemi;
    private bool jeuFini = false;

    [Header("Background")]
    public SpriteRenderer backgroundSprite;

    [Header("Audio")]
    public AudioSource musiqueDeFond;
    public AudioSource bruitagesSource;
    public AudioClip sonVictoire;
    public AudioClip sonDefaite;

    [Header("UI Tuto")]
    public GameObject panelTuto;

    void Start()
    {
        vieJoueur = vieMax;
        vieEnnemi = vieMax;
        scriptUI.UpdateCoeurs(vieJoueur, vieEnnemi);
        
        StartCoroutine(BoucleDeJeu());
    }

    IEnumerator BoucleDeJeu()
    {
        if (panelTuto != null)
        {
            panelTuto.SetActive(true);

            while (!Input.GetMouseButtonDown(0))
            {
                yield return null;
            }

            panelTuto.SetActive(false);
        }

        if (backgroundSprite != null)
        {
            backgroundSprite.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        }

        yield return new WaitForSeconds(0.5f);
        
        scriptUI.AfficherImageChiffre(0);
        yield return new WaitForSeconds(1f);
        
        scriptUI.AfficherImageChiffre(1);
        yield return new WaitForSeconds(1f);
        
        scriptUI.AfficherImageChiffre(2);
        yield return new WaitForSeconds(1f);
        
        scriptUI.AfficherImageChiffre(3);
        yield return new WaitForSeconds(1.5f);
        
        scriptUI.CacherImageChiffre();

        while (!jeuFini)
        {
            scriptMainJoueur.ResetState();
            scriptEnnemi.MettreEnAttente();

            scriptUI.MontrerSpamIcon();

            scriptMainJoueur.peutSpammer = true;
            yield return new WaitForSeconds(3.5f);
            
            scriptMainJoueur.peutSpammer = false;

            scriptUI.CacherSpamIcon();

            if (scriptMainJoueur.EstChargeAFond())
            {
                vieEnnemi--;
                scriptUI.AfficherImpact();

                scriptMainJoueur.LancerAnimationClaque();
                CameraShake.Instance.Secouer(0.2f, 0.5f);

                scriptEnnemi.PrendreUneClaque();
                scriptMainJoueur.AugmenterRound();
            }
            else
            {
                scriptUI.AfficherFeedbackEchec();
            }

            scriptUI.UpdateCoeurs(vieJoueur, vieEnnemi);

            if (vieEnnemi <= 0) { Victoire(); break; }

            yield return new WaitForSeconds(2f);
            scriptMainJoueur.ResetState();

            scriptUI.MontrerDodgeIcon();
            float attenteAleatoire = Random.Range(2f, 5f);
            yield return new WaitForSeconds(attenteAleatoire);

            scriptEnnemi.PreparerAttaque();

            scriptUI.CacherDodgeIcon();

            scriptUI.MontrerDodgeAction();

            scriptMainJoueur.peutEsquiver = true;
            yield return new WaitForSeconds(0.7f); 
            scriptMainJoueur.peutEsquiver = false;

            scriptUI.CacherDodgeAction();

            if (scriptMainJoueur.aEsquive)
            {
                scriptUI.AfficherFeedbackEsquive();
            }
            else
            {
                vieJoueur--;
                scriptUI.AfficherFeedbackMiss();
                CameraShake.Instance.Secouer(0.3f, 0.5f);
            }

            scriptEnnemi.MettreEnAttente();
            scriptUI.UpdateCoeurs(vieJoueur, vieEnnemi);

            if (vieJoueur <= 0) { Defaite(); break; }

            yield return new WaitForSeconds(1.5f);
        }
    }

    void Victoire()
    {
        jeuFini = true;
        
        if (musiqueDeFond != null) musiqueDeFond.Stop();

        scriptEnnemi.Mourir();

        if (bruitagesSource != null && sonVictoire != null)
        {
            bruitagesSource.PlayOneShot(sonVictoire);
        }

        scriptUI.AfficherVictoire();
        Debug.Log("GAGNÉ");
        
        // Compléter le niveau
        CompleteCurrentLevel();
        
        // Retour à la map après 3 secondes
        Invoke("RetourMap", 3f);
    }

    void Defaite()
    {
        jeuFini = true;
        
        if (musiqueDeFond != null) musiqueDeFond.Stop();

        if (bruitagesSource != null && sonDefaite != null)
        {
            bruitagesSource.PlayOneShot(sonDefaite);
        }

        scriptUI.AfficherDefaite();
        Debug.Log("PERDU");
        
        // Retour à la map après 3 secondes (sans compléter)
        Invoke("RetourMap", 3f);
    }

    void CompleteCurrentLevel()
    {
        string currentID = PlayerPrefs.GetString("CurrentLevelID", "");
        int currentIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 0);
        
        Debug.Log($"=== VICTOIRE niveau {currentIndex} (ID: {currentID}) ===");
        
        // Marquer comme complété (2 = LevelState.Completed)
        if (!string.IsNullOrEmpty(currentID))
        {
            PlayerPrefs.SetInt($"Level_{currentID}", 2);
        }
        
        // Sauvegarder pour débloquer le niveau suivant au retour
        PlayerPrefs.SetInt("JustCompletedLevel", currentIndex);
        PlayerPrefs.Save();
        
        Debug.Log($"Niveau {currentIndex} complété ! Le niveau {currentIndex + 1} sera débloqué.");
    }

    void RetourMap()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelector");
    }
}