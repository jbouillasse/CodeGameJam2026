using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // Pour changer de scène

public class MoveAfterDelay : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 3.0f;
    public Image endImage; // Image à afficher à la fin

    private bool isMoving = false;
    private bool hasArrived = false;

    void Start()
    {
        // Place l'objet sur A au départ
        if (pointA != null)
            transform.position = pointA.position;

        // Cache l’image au départ
        if (endImage != null)
            endImage.gameObject.SetActive(false);

        StartCoroutine(WaitAndStartMoving());
    }

    System.Collections.IEnumerator WaitAndStartMoving()
    {
        yield return new WaitForSeconds(5.0f);
        isMoving = true;
    }

    void Update()
    {
        if (isMoving && pointB != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, pointB.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, pointB.position) < 0.01f && !hasArrived)
            {
                transform.position = pointB.position;
                isMoving = false;
                hasArrived = true;
                ShowEndImage();
            }
        }
    }

    private void ShowEndImage()
    {
        if (endImage != null)
            endImage.gameObject.SetActive(true);
        // Démarre le chargement de la scène crédits dans 5 secondes
        Invoke("LoadCreditsScene", 5f);
    }

    private void LoadCreditsScene()
    {
        SceneManager.LoadScene("Crédits"); // Mets ici le nom EXACT de ta scène crédits ("Credits" ou "credits", etc.)
    }
}