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
                coeursEnnemi[i].color = new Color(0.3f, 0.3f, 0.3f, 1f);
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
        if(imageCompteARebours != null && index < spritesChiffres.Length)
        {
            imageCompteARebours.sprite = spritesChiffres[index];

            imageCompteARebours.enabled = true;
            
            imageCompteARebours.transform.localScale = Vector3.one; 
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
}