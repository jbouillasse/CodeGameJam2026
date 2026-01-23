using UnityEngine;

public class DodgeCamera : MonoBehaviour
{
    public float sensibilite = 2f;
    public float distanceEsquive = 2f;
    public float vitesseRetour = 5f;

    private Vector3 positionInitiale;
    private float cibleX;

    void Start()
    {
        positionInitiale = transform.position;
    }

    void Update()
    {
        float mouvementSouris = Input.GetAxis("Mouse X");

        if (mouvementSouris < -sensibilite) 
            cibleX = positionInitiale.x - distanceEsquive; 
        else if (mouvementSouris > sensibilite) 
            cibleX = positionInitiale.x + distanceEsquive; 
        else
            cibleX = positionInitiale.x;

        float nouvellePositionX = Mathf.Lerp(transform.position.x, cibleX, Time.deltaTime * vitesseRetour);
        
        transform.position = new Vector3(nouvellePositionX, transform.position.y, transform.position.z);
    }
}