using UnityEngine;
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

        buttonObject.SetActive(true);
    }
}
