using UnityEngine;
using UnityEngine.UI;

public class Clignotement : MonoBehaviour
{
    public float vitesse = 2f;
    private Image imageComposant;

    void Start()
    {
        imageComposant = GetComponent<Image>();
    }

    void Update()
    {
        if (imageComposant != null)
        {
            float alpha = (Mathf.Sin(Time.time * vitesse) + 1f) / 2f;

            Color c = imageComposant.color;
            imageComposant.color = new Color(c.r, c.g, c.b, alpha);
        }
    }
}