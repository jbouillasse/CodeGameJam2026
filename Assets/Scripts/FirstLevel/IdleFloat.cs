using UnityEngine;

public class IdleFloat : MonoBehaviour
{
    public float vitesse = 2f;
    public float amplitude = 0.1f;
    private Vector3 posDepart;

    void Start()
    {
        posDepart = transform.position;
    }

    void Update()
    {
        float newY = posDepart.y + Mathf.Sin(Time.time * vitesse) * amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}