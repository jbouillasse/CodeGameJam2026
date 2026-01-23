using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

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

    void Start()
    {
        vieJoueur = vieMax;
        vieEnnemi = vieMax;
        scriptUI.UpdateCoeurs(vieJoueur, vieEnnemi);
        
        StartCoroutine(BoucleDeJeu());
    }

    IEnumerator BoucleDeJeu()
    {
        if(backgroundSprite != null)
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
                scriptUI.AfficherMessage("BAM ! -1 PV", Color.cyan);

                // --- AJOUTE CES 2 LIGNES ICI ---
                scriptMainJoueur.LancerAnimationClaque(); // Lance le mouvement de la main
                CameraShake.Instance.Secouer(0.2f, 0.5f); // Fait trembler l'écran
                // -------------------------------

                scriptEnnemi.PrendreUneClaque();
                scriptMainJoueur.AugmenterRound();
            }
            else
            {
                scriptUI.AfficherMessage("TROP FAIBLE...", Color.gray);
            }

            scriptUI.UpdateCoeurs(vieJoueur, vieEnnemi);

            if (vieEnnemi <= 0) { Victoire(); break; }

            yield return new WaitForSeconds(2f);
            Debug.Log("--- TOUR ENNEMI ---");
            scriptMainJoueur.ResetState();

            scriptUI.AfficherMessage("ATTENTION...", Color.yellow, 0); // On laisse le texte affiché
            
            float attenteAleatoire = Random.Range(2f, 5f);
            yield return new WaitForSeconds(attenteAleatoire);

            scriptEnnemi.PreparerAttaque();
            scriptUI.AfficherMessage("CLIQUE !!!", Color.red, 0.5f);

            scriptMainJoueur.peutEsquiver = true;
            yield return new WaitForSeconds(0.5f); 
            scriptMainJoueur.peutEsquiver = false; // Trop tard

            // RESOLUTION DEFENSE
            if (scriptMainJoueur.aEsquive)
            {
                scriptUI.AfficherMessage("ESQUIVÉ !", Color.green);
                // Animation esquive visuelle (Camera ou autre) ici si tu veux
            }
            else
            {
                vieJoueur--;
                scriptUI.AfficherMessage("AÏE ! -1 PV", Color.red);
                // Feedback écran rouge ou tremblement ici
            }

            scriptEnnemi.MettreEnAttente();
            scriptUI.UpdateCoeurs(vieJoueur, vieEnnemi);

            // Vérifier Défaite
            if (vieJoueur <= 0) { Defaite(); break; }

            yield return new WaitForSeconds(1.5f);
        }
    }

    void Victoire()
    {
        scriptUI.AfficherMessage("VICTOIRE !", Color.yellow, 0);
        Debug.Log("GAGNÉ");
    }

    void Defaite()
    {
        scriptUI.AfficherMessage("K.O.", Color.red, 0);
        Debug.Log("PERDU");
    }
}