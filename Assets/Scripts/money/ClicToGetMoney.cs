using UnityEngine;
using UnityEngine.InputSystem;

public class ClicToGetMoney : MonoBehaviour
{
    [Header("Réglages")]
    [SerializeField] private int moneyPerClick = 1;

    [Header("Audio")]
    [SerializeField] private AudioClip soundEffect;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (MoneyManager.Instance != null)
            {
                MoneyManager.Instance.AddMoney(moneyPerClick);
                if (soundEffect != null)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(soundEffect);
                }
            }
        }
    }
}