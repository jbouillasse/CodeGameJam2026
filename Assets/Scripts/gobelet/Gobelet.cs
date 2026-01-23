using UnityEngine;
using UnityEngine.EventSystems;

public class Gobelet : MonoBehaviour, IPointerDownHandler
{
    public BonneteauManager manager;
    public int monIndexID;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("🖱️ CLIC BOUTON REÇU SUR GOBELET " + monIndexID);

        if (manager.peutCliquer)
        {
            manager.JoueurChoisitGobelet(monIndexID);
        }
    }
}