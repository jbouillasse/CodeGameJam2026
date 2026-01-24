using UnityEngine;
using UnityEngine.EventSystems;

public class ChargeClickArea : MonoBehaviour, IPointerDownHandler
{
    public HammerGameManager game;
    public float clickGain = 6f;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (game != null)
            game.AddCharge(clickGain);
    }
}
