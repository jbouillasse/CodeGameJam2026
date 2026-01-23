using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    private static MoneyManager instance;

    [SerializeField] private TextMeshProUGUI moneyText;
    public int startingMoney = 10;

    public static MoneyManager Instance
    {
        get
        {
            if (instance == null)
            {
                SetupInstance();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private static void SetupInstance()
    {
        instance = FindFirstObjectByType<MoneyManager>();

        if (instance == null)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "MoneyManager";
            instance = gameObject.AddComponent<MoneyManager>();
        }
    }

    public void AddMoney(int amount)
    {
        startingMoney += amount;

        if (moneyText != null)
        {
            moneyText.text = startingMoney.ToString();
        }
        else
        {
            Debug.LogWarning("Attention : Pas de Texte assigné au MoneyManager !");
        }
    }
}