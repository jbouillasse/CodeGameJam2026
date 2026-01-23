using UnityEngine;

public class EnemyBreathing : MonoBehaviour
{
    [Header("Réglages")]
    public float vitesse = 2f;
    public float force = 0.05f;

    private Vector3 scaleInitial;

    void Start()
    {
        scaleInitial = transform.localScale;
    }

    void Update()
    {
        float sin = Mathf.Sin(Time.time * vitesse);

        float scaleX = scaleInitial.x + (sin * force);
        float scaleY = scaleInitial.y - (sin * force);

        transform.localScale = new Vector3(scaleX, scaleY, scaleInitial.z);
    }
}