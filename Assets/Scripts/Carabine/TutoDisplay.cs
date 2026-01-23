using System.Collections;
using UnityEngine;

public class TutoDisplay : MonoBehaviour
{
    public float displayTime = 10f;

    void Start()
    {
        StartCoroutine(ShowAndPause());
    }

    IEnumerator ShowAndPause()
    {
        Time.timeScale = 0f; 

        yield return new WaitForSecondsRealtime(displayTime); 

        Time.timeScale = 1f; 
        gameObject.SetActive(false); 
    }

    void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
