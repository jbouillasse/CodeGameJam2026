using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private float fireDuration = 0.08f;

    [SerializeField] private AudioSource shootSound;   // ← ICI

    private Image image;
    private Coroutine routine;

    void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = normalSprite;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(Fire());
        }
    }

    IEnumerator Fire()
    {
        shootSound.Play();              // ← LE SON PART ICI
        image.sprite = fireSprite;
        yield return new WaitForSeconds(fireDuration);
        image.sprite = normalSprite;
        routine = null;
    }
}
