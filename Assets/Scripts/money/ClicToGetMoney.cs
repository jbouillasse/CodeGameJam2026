using UnityEngine;
using UnityEngine.InputSystem;

public class ClicToGetMoney : MonoBehaviour
{
    [SerializeField] private int moneyPerClick = 1;

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (MoneyManager.Instance != null)
            {
                MoneyManager.Instance.AddMoney(moneyPerClick);
            }
        }
    }
}