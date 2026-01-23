using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AutoOrthoZoom : MonoBehaviour
{
    [Header("Référence (celle où ton cadrage est parfait)")]
    public float referenceWidth = 2560f;
    public float referenceHeight = 1440f;

    [Tooltip("Orthographic Size qui te donne le bon cadrage sur la résolution de référence.")]
    public float referenceOrthoSize = 5f;

    [Header("Remplissage")]
    [Tooltip("1 = fill strict. 0.98 = un poil moins zoomé (laisse une mini marge).")]
    [Range(0.85f, 1.05f)]
    public float fillFactor = 1.0f;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        Apply();
    }

    void OnValidate()
    {
        if (!Application.isPlaying) return;
        Apply();
    }

    void Apply()
    {
        float refAspect = referenceWidth / referenceHeight;
        float screenAspect = (float)Screen.width / Screen.height;

        // Mode FILL : on adapte la taille pour remplir l'écran
        float size = referenceOrthoSize;

        if (screenAspect > refAspect)
        {
            // écran plus large -> pour FILL on "zoome" un peu (size plus petit)
            size *= refAspect / screenAspect;
        }
        else
        {
            // écran plus étroit -> on garde le size (ou on peut zoomer différemment selon ton besoin)
            // ici on garde le size pour éviter trop de rognage vertical
        }

        cam.orthographicSize = size * fillFactor;
    }
}
