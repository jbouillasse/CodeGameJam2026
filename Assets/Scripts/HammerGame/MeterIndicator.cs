using System;
using System.Collections;
using UnityEngine;

public class MeterIndicator : MonoBehaviour
{
    public Transform bottomPoint;
    public Transform topPoint;
    public float moveTime = 0.25f;

    // Appelé quand on atteint la cloche (top)
    public Action OnBell;

    private Coroutine co;

    public void ResetToBottom()
    {
        if (bottomPoint != null)
            transform.position = bottomPoint.position;
    }

    public void ShowScore(float t01)
    {
        t01 = Mathf.Clamp01(t01);

        if (bottomPoint == null || topPoint == null) return;

        if (co != null) StopCoroutine(co);
        co = StartCoroutine(MoveTo(t01));
    }

    private IEnumerator MoveTo(float t01)
    {
        Vector3 from = transform.position;
        Vector3 to = Vector3.Lerp(bottomPoint.position, topPoint.position, t01);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, moveTime);
            transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }

        transform.position = to;

        // Ding seulement si on est au top (t01 == 1)
        if (Mathf.Approximately(t01, 1f))
            OnBell?.Invoke();
    }
}
