using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("Textes Combat")]
    public TextMeshProUGUI texteCentral;

    [Header("Compte à Rebours (Images)")]
    public Image imageCompteARebours;
    public Sprite[] spritesChiffres; 

    [Header("Cœurs Joueur")]
    public Image[] coeursJoueur;
    public Sprite coeurJoueurPlein;
    public Sprite coeurJoueurVide;

    [Header("coeurs Ennemi")]
    public Image[] coeursEnnemi;
    public Sprite coeurEnnemiPlein;
    public Sprite coeurEnnemiVide;

    [Header("Animation Spam")]
    public GameObject spamIconObject;

    [Header("Animation Dodge")]
    public GameObject dodgeIconObject;
    public GameObject dodgeActionObject;

    [Header("Feedback Esquive")]
    public GameObject dodgeSuccessObject;
    public GameObject dodgeMissObject;

    [Header("Feedback Echec")]
    public GameObject failIconObject;

    [Header("Feedback Touché")]
    public GameObject hitImpactObject;

    [Header("Ecrans Fin de Jeu")]
    public GameObject victoryObject;
    public GameObject gameOverObject;

    public void UpdateCoeurs(int vieJoueur, int vieEnnemi)
    {
        for (int i = 0; i < coeursJoueur.Length; i++)
        {
            if (i < vieJoueur) coeursJoueur[i].sprite = coeurJoueurPlein;
            else coeursJoueur[i].sprite = coeurJoueurVide;
        }

        for (int i = 0; i < coeursEnnemi.Length; i++)
        {
            if (i < vieEnnemi) coeursEnnemi[i].sprite = coeurEnnemiPlein;
            else {
                coeursEnnemi[i].sprite = coeurEnnemiVide;
            }
        }
    }

    public void AfficherMessage(string message, Color couleur, float duree = 1f)
    {
        if (texteCentral != null)
        {
            texteCentral.text = message;
            texteCentral.color = couleur;
            texteCentral.gameObject.SetActive(true);
            StartCoroutine(AnimScale(texteCentral.transform));
            
            if (duree > 0) Invoke("CacherMessage", duree);
        }
    }

    void CacherMessage()
    {
        if(texteCentral != null) texteCentral.text = "";
    }

    public void AfficherImageChiffre(int index)
    {
        if (imageCompteARebours != null && index < spritesChiffres.Length)
        {
            // On éteint l'objet une fraction de seconde pour reset l'anim
            imageCompteARebours.gameObject.SetActive(false);

            imageCompteARebours.sprite = spritesChiffres[index];

            // On rallume -> Le script JuicyEffect se lance tout seul !
            imageCompteARebours.gameObject.SetActive(true);
            imageCompteARebours.color = Color.white;
        }
    }

    public void CacherImageChiffre()
    {
        if(imageCompteARebours != null) imageCompteARebours.gameObject.SetActive(false);
    }

    IEnumerator AnimScale(Transform target)
    {
        float timer = 0;
        target.localScale = Vector3.zero;
        while(timer < 0.2f)
        {
            timer += Time.deltaTime;
            float scale = Mathf.Lerp(0f, 1.2f, timer * 5);
            target.localScale = Vector3.one * scale;
            yield return null;
        }
        target.localScale = Vector3.one;
    }

    IEnumerator AnimFeedbackEsquive()
    {
        if (dodgeSuccessObject != null)
        {
            dodgeSuccessObject.SetActive(true);

            yield return new WaitForSeconds(1f);

            dodgeSuccessObject.SetActive(false);
        }
    }

    public void MontrerSpamIcon()
    {
        // On cache le texte s'il y en avait un
        if (texteCentral != null) texteCentral.text = "";

        // On active l'objet animé. L'Animator lancera l'anim tout seul.
        if (spamIconObject != null) spamIconObject.SetActive(true);
    }

    public void CacherSpamIcon()
    {
        // On désactive l'objet
        if (spamIconObject != null) spamIconObject.SetActive(false);
    }

    public void MontrerDodgeIcon() // Affiche le bouclier
    {
        if (texteCentral != null) texteCentral.text = "";
        if (dodgeIconObject != null) dodgeIconObject.SetActive(true);
    }

    public void CacherDodgeIcon() // Cache le bouclier
    {
        if (dodgeIconObject != null) dodgeIconObject.SetActive(false);
    }

    // --- AJOUTE CES DEUX FONCTIONS ---
    public void MontrerDodgeAction() // Affiche la souris
    {
        if (dodgeActionObject != null) dodgeActionObject.SetActive(true);
    }

    public void CacherDodgeAction() // Cache la souris
    {
        if (dodgeActionObject != null) dodgeActionObject.SetActive(false);
    }

    public void AfficherFeedbackEsquive()
    {
        StartCoroutine(AnimFeedbackEsquive());
    }

    // Miss
    IEnumerator AnimFeedbackMiss()
    {
        if (dodgeMissObject != null)
        {
            dodgeMissObject.SetActive(true);

            yield return new WaitForSeconds(1f);

            dodgeMissObject.SetActive(false);
        }
    }

    public void AfficherFeedbackMiss()
    {
        StartCoroutine(AnimFeedbackMiss());
    }

    // Feedback Miss Hit
    public void AfficherFeedbackEchec()
    {
        StartCoroutine(AnimFeedbackEchec());
    }

    IEnumerator AnimFeedbackEchec()
    {
        if (failIconObject != null)
        {
            failIconObject.SetActive(true);

            yield return new WaitForSeconds(1.5f);

            failIconObject.SetActive(false);
        }
    }

    // HIT
    public void AfficherImpact()
    {
        StartCoroutine(AnimImpact());
    }

    IEnumerator AnimImpact()
    {
        if (hitImpactObject != null)
        {
            float decalageX = Random.Range(-20f, 20f);
            float decalageY = Random.Range(-20f, 20f);
            hitImpactObject.transform.localPosition = new Vector3(decalageX, decalageY, 0);

            hitImpactObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            hitImpactObject.SetActive(false);
        }
    }

    // WIN / LOSS
    public void AfficherVictoire()
    {
        if (texteCentral != null) texteCentral.text = "";

        if (victoryObject != null) victoryObject.SetActive(true);
    }

    public void AfficherDefaite()
    {
        if (texteCentral != null) texteCentral.text = "";

        if (gameOverObject != null) gameOverObject.SetActive(true);
    }
}