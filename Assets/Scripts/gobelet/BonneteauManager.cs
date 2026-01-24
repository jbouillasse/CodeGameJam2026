using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class BonneteauManager : MonoBehaviour
{
    [Header("Objets de la scène")]
    public GameObject[] gobelets;
    public GameObject balle;
    public TextMeshProUGUI texteAffichage;

    [Header("Audio")]
    public AudioSource sourceAudio;
    public AudioClip sonVictoire;
    public AudioClip sonDefaite;
    public AudioClip musiqueFond;

    [Header("Réglages")]
    public int nombreMelanges = 5;
    public float vitesse = 0.5f;
    public int maxRounds = 3;
    public int roundsPourGagner = 2;

    [HideInInspector]
    public bool peutCliquer = false;

    private int indexGagnant;
    private int scoreJoueur = 0;
    private int roundActuel = 0;

    void Start()
    {
        if (sourceAudio != null && musiqueFond != null)
        {
            sourceAudio.clip = musiqueFond;
            sourceAudio.loop = true;
            sourceAudio.volume = 0.3f;
            sourceAudio.Play();
        }

        scoreJoueur = 0;
        roundActuel = 0;
        LancerRound();
    }

    void LancerRound()
    {
        roundActuel++;

        if (roundActuel <= maxRounds)
        {
            texteAffichage.text = "MANCHE " + roundActuel + " / " + maxRounds;
            StartCoroutine(BoucleDeJeu());
        }
        else
        {
            FinDePartie();
        }
    }

    IEnumerator BoucleDeJeu()
    {
        peutCliquer = false;

        indexGagnant = Random.Range(0, 3);

        balle.transform.SetParent(null);
        balle.transform.position = gobelets[indexGagnant].transform.position;
        balle.SetActive(true);

        texteAffichage.text = "OBSERVEZ BIEN LA BALLE";

        Vector3 posBas = gobelets[indexGagnant].transform.position;
        Vector3 posHaut = posBas + Vector3.up * 1.5f;

        float t = 0;
        while (t < 1) { t += Time.deltaTime * 2; gobelets[indexGagnant].transform.position = Vector3.Lerp(posBas, posHaut, t); yield return null; }

        yield return new WaitForSeconds(1.0f);

        t = 0;
        while (t < 1) { t += Time.deltaTime * 2; gobelets[indexGagnant].transform.position = Vector3.Lerp(posHaut, posBas, t); yield return null; }

        balle.transform.SetParent(gobelets[indexGagnant].transform);
        balle.SetActive(false);

        texteAffichage.text = "MELANGE EN COURS...";
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < nombreMelanges; i++)
        {
            int a = Random.Range(0, 3);
            int b = Random.Range(0, 3);

            Vector3 posA = gobelets[a].transform.position;
            Vector3 posB = gobelets[b].transform.position;

            float timer = 0f;
            while (timer < vitesse)
            {
                timer += Time.deltaTime;
                float progress = timer / vitesse;
                progress = progress * progress * (3f - 2f * progress);

                gobelets[a].transform.position = Vector3.Lerp(posA, posB, progress);
                gobelets[b].transform.position = Vector3.Lerp(posB, posA, progress);
                yield return null;
            }
            gobelets[a].transform.position = posB;
            gobelets[b].transform.position = posA;
        }

        peutCliquer = true;
        texteAffichage.text = "TROUVEZ LA BALLE !";
    }

    public void JoueurChoisitGobelet(int indexDuGobelet)
    {
        if (!peutCliquer) return;
        peutCliquer = false;

        bool estGagnant = false;
        if (indexDuGobelet == indexGagnant) estGagnant = true;
        if (balle.transform.parent == gobelets[indexDuGobelet].transform) estGagnant = true;

        balle.SetActive(true);
        balle.transform.SetParent(null);

        StartCoroutine(LeverGobeletFinal(indexDuGobelet, estGagnant));
    }

    IEnumerator LeverGobeletFinal(int indexChoisi, bool aGagne)
    {
        Vector3 posBas = gobelets[indexChoisi].transform.position;
        Vector3 posHaut = posBas + Vector3.up * 1.5f;

        float t = 0;
        while (t < 1) { t += Time.deltaTime * 3; gobelets[indexChoisi].transform.position = Vector3.Lerp(posBas, posHaut, t); yield return null; }

        if (aGagne)
        {
            scoreJoueur++;
            texteAffichage.text = "GAGNE !";
            texteAffichage.color = Color.green;
        }
        else
        {
            texteAffichage.text = "PERDU...";
            texteAffichage.color = Color.red;
            StartCoroutine(MontrerLaVraieSolution());
        }

        yield return new WaitForSeconds(2f);

        gobelets[indexChoisi].transform.position = posBas;
        texteAffichage.color = Color.white;
        LancerRound();
    }

    IEnumerator MontrerLaVraieSolution()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 posBas = gobelets[indexGagnant].transform.position;
        Vector3 posHaut = posBas + Vector3.up * 1.5f;

        balle.transform.position = posBas;
        balle.SetActive(true);

        float t = 0;
        while (t < 1) { t += Time.deltaTime * 5; gobelets[indexGagnant].transform.position = Vector3.Lerp(posBas, posHaut, t); yield return null; }
        yield return new WaitForSeconds(1f);
        t = 0;
        while (t < 1) { t += Time.deltaTime * 5; gobelets[indexGagnant].transform.position = Vector3.Lerp(posHaut, posBas, t); yield return null; }
    }

    void FinDePartie()
    {
        if (sourceAudio != null) sourceAudio.Stop();

        if (scoreJoueur >= roundsPourGagner)
        {
            texteAffichage.text = "VICTOIRE !";
            texteAffichage.color = Color.yellow;

            if (sourceAudio != null && sonVictoire != null)
            {
                sourceAudio.volume = 1.0f;
                sourceAudio.PlayOneShot(sonVictoire);
            }

            CompleteCurrentLevel();
        }
        else
        {
            texteAffichage.text = "DEFAITE...";
            texteAffichage.color = Color.grey;

            if (sourceAudio != null && sonDefaite != null)
            {
                sourceAudio.volume = 1.0f;
                sourceAudio.PlayOneShot(sonDefaite);
            }
        }

        Invoke("ChargerLevelSelector", 5f);
    }

    void CompleteCurrentLevel()
    {
        string currentID = PlayerPrefs.GetString("CurrentLevelID", "");
        int currentIndex = PlayerPrefs.GetInt("CurrentLevelIndex", 0);

        Debug.Log($"=== VICTOIRE niveau {currentIndex} (ID: {currentID}) ===");

        if (!string.IsNullOrEmpty(currentID))
        {
            PlayerPrefs.SetInt($"Level_{currentID}", 2);
        }

        PlayerPrefs.SetInt("JustCompletedLevel", currentIndex);
        PlayerPrefs.Save();

        Debug.Log($"Niveau {currentIndex} complété ! Le niveau {currentIndex + 1} sera débloqué.");
    }

    void ChargerLevelSelector()
    {
        SceneManager.LoadScene("LevelSelector");
    }
}