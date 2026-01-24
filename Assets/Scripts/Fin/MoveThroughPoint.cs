using UnityEngine;

public class MoveThroughPoint : MonoBehaviour
{
    public Transform pointA;
    public Transform pointC;
    public Transform pointB;
    public float speed = 3.0f;

    private int step = 0; // 0 = rien, 1 = vers C, 2 = vers B

    void Start()
    {
        // Place le personnage sur A au démarrage
        if (pointA != null)
            transform.position = pointA.position;
        step = 1; // Commence le mouvement vers C directement
    }

    void Update()
    {
        if (step == 1 && pointC != null)
        {
            // Avance vers C
            transform.position = Vector3.MoveTowards(transform.position, pointC.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, pointC.position) < 0.01f)
            {
                transform.position = pointC.position;
                step = 2; // Prochaine étape: aller vers B
            }
        }
        else if (step == 2 && pointB != null)
        {
            // Avance vers B
            transform.position = Vector3.MoveTowards(transform.position, pointB.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, pointB.position) < 0.01f)
            {
                transform.position = pointB.position;
                step = 0; // Mouvement terminé
            }
        }
    }
}