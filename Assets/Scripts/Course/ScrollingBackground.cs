using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [Header("Backgrounds - Glisse les 3 ici")]
    public SpriteRenderer[] backgrounds;
    
    [Header("Vitesse")]
    public float scrollSpeed = 3f;
    
    private float spriteWidth;
    
    void Start()
    {
        if (backgrounds == null || backgrounds.Length == 0)
        {
            Debug.LogError("Aucun background assigné !");
            return;
        }
        
        spriteWidth = backgrounds[0].bounds.size.x;
    }
    
    void Update()
    {
        if (backgrounds == null || backgrounds.Length == 0) return;
        
        float speed = scrollSpeed;
        if (RaceGameManager.Instance != null)
        {
            speed = RaceGameManager.Instance.GetPlayerSpeed();
        }
        
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i] == null) continue;
            
            // Déplacer vers la gauche
            Vector3 pos = backgrounds[i].transform.position;
            pos.x -= speed * Time.deltaTime;
            backgrounds[i].transform.position = pos;
            
            // Téléporter seulement quand le BORD DROIT du sprite est sorti
            // (position X + demi-largeur < bord gauche de l'écran)
            float rightEdge = pos.x + (spriteWidth / 2f);
            float screenLeftEdge = Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect);
            
            if (rightEdge < screenLeftEdge)
            {
                float rightmostX = GetRightmostX();
                // Coller juste à droite du dernier background
                float newX = rightmostX + spriteWidth;
                backgrounds[i].transform.position = new Vector3(newX, pos.y, pos.z);
            }
        }
    }
    
    private float GetRightmostX()
    {
        float max = float.MinValue;
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i] != null && backgrounds[i].transform.position.x > max)
            {
                max = backgrounds[i].transform.position.x;
            }
        }
        return max;
    }
}