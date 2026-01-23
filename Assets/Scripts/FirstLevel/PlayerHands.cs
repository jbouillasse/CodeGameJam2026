using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHands : MonoBehaviour
{
    [Header("Réglages")]
    [SerializeField] private int[] clicsNecessairesPerRounds = new int[] { 50, 100, 150 };
    [SerializeField] private float[] vitessePertePerRounds = new float[] { 5f, 10f, 15f };   

    [Header("Visuel")]
    [SerializeField] private SpriteRenderer monSpriteRenderer;
    [SerializeField] private Sprite[] etatsMain; 

    private float chargeActuelle = 0f;
    private int roundActuelle = 0; 

    void Update()
    {
        bool aClique = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        if (aClique) 
        {
            chargeActuelle += 1f; 
        }

        if (chargeActuelle > 0)
        {
            chargeActuelle -= vitessePertePerRounds[roundActuelle] * Time.deltaTime;
        }

        chargeActuelle = Mathf.Clamp(chargeActuelle, 0, clicsNecessairesPerRounds[roundActuelle]);
        MiseAJourVisuel();
    }

    void MiseAJourVisuel()
    {
        float maxPourCeRound = clicsNecessairesPerRounds[roundActuelle];
        float pourcentage = chargeActuelle / maxPourCeRound;
        
        int indexSprite = 0;
        if (pourcentage > 0.75f) indexSprite = 3;      
        else if (pourcentage > 0.50f) indexSprite = 2; 
        else if (pourcentage > 0.25f) indexSprite = 1; 
        else indexSprite = 0;                          

        if(monSpriteRenderer != null && indexSprite < etatsMain.Length)
        {
             monSpriteRenderer.sprite = etatsMain[indexSprite];
        }
    }
    
    public float GetForceDeLaClaque()
    {
        return chargeActuelle;
    }

    public void AugmenterDifficulte()
    {
        roundActuelle++; 

        if (roundActuelle >= clicsNecessairesPerRounds.Length)
        {
            roundActuelle = clicsNecessairesPerRounds.Length - 1; 
        }

        chargeActuelle = 0;
        MiseAJourVisuel();
    }
    public void ResetCharge()
    {
        chargeActuelle = 0;
        MiseAJourVisuel();
    }
}