using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Screamer : MonoBehaviour
{
    public GameObject screamerImage;
    public AudioSource screamSound;
    public GameObject buttonObject;
    public float screamerDuration = 2f;

    public void PlayScreamer()
    {
        StartCoroutine(ScreamerSequence());
    }

    IEnumerator ScreamerSequence()
    {
        buttonObject.SetActive(false);

        screamerImage.SetActive(true);

        screamSound.Play();

        yield return new WaitForSeconds(screamerDuration);

        screamerImage.SetActive(false);

        // ✅ NOUVEAU : Compléter le niveau
        CompleteCurrentLevel();

        // ✅ Retour à la map après 1 seconde
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("LevelSelector");
    }

    // ✅ NOUVELLE MÉTHODE : Compléter le niveau
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
}