using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Liens")]
    public PlayerHands scriptMainJoueur;
    public DodgeCamera scriptCamera;
    public Enemy scriptEnnemi;

    [Header("Equilibrage")]
    public int pointsDeVieEnnemi = 100;
    public float dureeCharge = 10f;
    public float dureeReflexe = 1f;

    private bool combatEnCours = true;

    void Start()
    {
        StartCoroutine(BoucleDeCombat());
    }

    IEnumerator BoucleDeCombat()
    {
        while (combatEnCours && pointsDeVieEnnemi > 0)
        {
            Debug.Log("--- CHARGEZ !!! ---");
            scriptEnnemi.MettreEnAttente();
            yield return new WaitForSeconds(dureeCharge);

            Debug.Log("--- ATTENTION ESQUIVE !!! ---");
            scriptEnnemi.PreparerAttaque();
            yield return new WaitForSeconds(dureeReflexe);
            
            bool esquiveReussie = Mathf.Abs(scriptCamera.transform.localPosition.x) > 0.5f;

            if (esquiveReussie)
            {
                float degats = scriptMainJoueur.GetForceDeLaClaque();
                pointsDeVieEnnemi -= (int)degats;
                Debug.Log("BAM ! Dégâts : " + degats + ". Vie restante : " + pointsDeVieEnnemi);
                
                scriptEnnemi.PrendreUneClaque();

                scriptMainJoueur.AugmenterDifficulte();
            }
            else
            {
                Debug.Log("AIE ! Tu t'es pris une gifle !");
                scriptMainJoueur.ResetCharge();
            }

            yield return new WaitForSeconds(1f);
        }

        Debug.Log("COMBAT TERMINE !");
    }
}