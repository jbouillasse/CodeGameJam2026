using UnityEngine;

public class BallonScript : MonoBehaviour
{
    [Header("Score")]
    public int points = 10;

    [Header("Pop-up (sort du trou)")]
    public float popUpHeight = 1.2f;
    public float popUpTime = 0.25f;

    [Header("Temps avant disparition")]
    public float lifeTime = 3f;

    private SpawnPoint myHole;

    private Vector3 startPos;
    private Vector3 endPos;
    private float t;
    private float life;

    // Appelé par le spawner
    public void SetSpawnPoint(SpawnPoint hole)
    {
        myHole = hole;
    }

    void Start()
    {
        // Position du trou
        Vector3 holePos = new Vector3(transform.position.x, transform.position.y, 0f);

        // Commence caché dans le trou
        startPos = holePos + Vector3.down * 0.6f;

        // Position finale visible
        endPos = holePos + Vector3.up * popUpHeight;

        transform.position = startPos;
    }

    void Update()
    {
        // Animation d'entrée
        if (t < 1f)
        {
            t += Time.deltaTime / popUpTime;
            transform.position = Vector3.Lerp(startPos, endPos, Mathf.Clamp01(t));
            return;
        }

        // Temps de vie
        life += Time.deltaTime;
        if (life >= lifeTime)
        {
            Despawn();
        }
    }

public void Pop()
{
    Debug.Log("Ballon touché !");

    if (GameManager.Instance != null)
    {
        GameManager.Instance.AddScore(points);
        GameManager.Instance.AddKill();
    }

    var hud = FindFirstObjectByType<HUDController>();
    if (hud != null) hud.AddKill();

    Despawn();
}


void Despawn()
{
    if (myHole != null)
        myHole.occupied = false;

    Destroy(gameObject);
}
}
