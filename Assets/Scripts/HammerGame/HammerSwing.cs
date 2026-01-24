using System.Collections;
using UnityEngine;

public class HammerSwing : MonoBehaviour
{
    [Header("Points (place-les dans la même hiérarchie que le marteau)")]
    public Transform restPoint;
    public Transform hitPoint;
    public Transform recoilPoint; // optionnel

    [Header("Timing")]
    public float downTime = 0.12f;
    public float recoilTime = 0.08f;
    public float returnTime = 0.14f;

    [Header("Rotation (en Z)")]
    public float hitRotationZ = -35f;

    private bool isPlaying;

    private Vector3 restLocalPos;
    private Quaternion restLocalRot;

    void Awake()
    {
        // On mémorise la position/rotation de repos réelle du marteau (celle dans la scène)
        restLocalPos = transform.localPosition;
        restLocalRot = transform.localRotation;
    }

    public void PlayHit()
    {
        if (isPlaying) return;
        StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        isPlaying = true;

        // Si tu as un restPoint, on l’utilise, sinon on revient à la pose mémorisée
        Vector3 restPos = (restPoint != null) ? restPoint.localPosition : restLocalPos;
        Quaternion restRot = restLocalRot;

        // Points en LOCAL (très important)
        Vector3 hitPos = (hitPoint != null) ? hitPoint.localPosition : restPos;
        Vector3 recoilPos = (recoilPoint != null) ? recoilPoint.localPosition : restPos;

        Quaternion hitRot = Quaternion.Euler(0, 0, hitRotationZ);

        // Assure le repos au début
        transform.localPosition = restPos;
        transform.localRotation = restRot;

        // Descente
        yield return MoveAndRotate(restPos, hitPos, restRot, hitRot, downTime);

        // Recoil (bounce)
        if (recoilPoint != null)
            yield return MoveAndRotate(hitPos, recoilPos, hitRot, restRot, recoilTime);

        // Retour
        Vector3 fromReturn = (recoilPoint != null) ? recoilPos : hitPos;
        yield return MoveAndRotate(fromReturn, restPos, restRot, restRot, returnTime);

        // Force la pose finale exacte (évite les petites erreurs)
        transform.localPosition = restPos;
        transform.localRotation = restRot;

        isPlaying = false;
    }

    private IEnumerator MoveAndRotate(Vector3 fromPos, Vector3 toPos, Quaternion fromRot, Quaternion toRot, float duration)
    {
        float t = 0f;
        float d = Mathf.Max(0.0001f, duration);

        while (t < 1f)
        {
            t += Time.deltaTime / d;
            float eased = EaseOutCubic(t);

            transform.localPosition = Vector3.Lerp(fromPos, toPos, eased);
            transform.localRotation = Quaternion.Slerp(fromRot, toRot, eased);

            yield return null;
        }

        transform.localPosition = toPos;
        transform.localRotation = toRot;
    }

    private float EaseOutCubic(float t)
    {
        t = Mathf.Clamp01(t);
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}
