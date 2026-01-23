using System.Collections.Generic;
using UnityEngine;

public class BalloonSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] balloonPrefabs;

    [Header("Spawn Points (holes)")]
    public SpawnPoint[] spawnPoints;

    [Header("Spawn")]
    public float spawnInterval = 1f;

    [Header("Sorting (sans TopTargets)")]
    public string balloonLayer = "Targets";
    public int normalOrder = 0;

    [Tooltip("Ordre pour les ballons des trous 1 à 4 (doit être > Foreground_Top).")]
    public int topOrder = 20;

    [Tooltip("Si true: hole_1 à hole_4 = trous du haut (devant Foreground_Top).")]
    public bool firstFourAreTopHoles = true;

    float timer;

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsRunning)
            return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            Spawn();
        }
    }

    void Spawn()
    {
        if (balloonPrefabs == null || balloonPrefabs.Length == 0)
        {
            Debug.LogError("BalloonSpawner: balloonPrefabs est vide !");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("BalloonSpawner: spawnPoints est vide !");
            return;
        }

        // Filtrer les prefabs non-null
        List<GameObject> validPrefabs = new List<GameObject>();
        foreach (var p in balloonPrefabs)
            if (p != null) validPrefabs.Add(p);

        if (validPrefabs.Count == 0)
        {
            Debug.LogError("BalloonSpawner: tous les prefabs sont NULL (None) dans l'Inspector.");
            return;
        }

        // Filtrer les trous libres et non-null
        List<SpawnPoint> free = new List<SpawnPoint>();
        foreach (var sp in spawnPoints)
        {
            if (sp == null) continue;
            if (!sp.occupied) free.Add(sp);
        }

        if (free.Count == 0)
            return;

        SpawnPoint hole = free[Random.Range(0, free.Count)];
        GameObject prefab = validPrefabs[Random.Range(0, validPrefabs.Count)];

        hole.occupied = true;

        Vector3 spawnPos = new Vector3(hole.transform.position.x, hole.transform.position.y, 0f);
        GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Associer le spawn point au ballon
        BallonScript b = go.GetComponent<BallonScript>();
        if (b != null) b.SetSpawnPoint(hole);

        // --- TRI D'AFFICHAGE selon le trou (hole_1..hole_4) ---
        bool isTopHole = IsTopHole(hole);
        int order = isTopHole ? topOrder : normalOrder;

        ApplySorting(go, balloonLayer, order);
    }

    bool IsTopHole(SpawnPoint hole)
    {
        if (!firstFourAreTopHoles) return false;

        // basé sur le nom: hole_1..hole_4
        string n = hole.gameObject.name; // ex: "hole_3"
        if (!n.StartsWith("hole_")) return false;

        string numStr = n.Substring(5);
        if (int.TryParse(numStr, out int num))
            return num >= 1 && num <= 4;

        return false;
    }

    void ApplySorting(GameObject root, string sortingLayerName, int orderInLayer)
    {
        // Change tous les SpriteRenderer du ballon (root + enfants)
        var renderers = root.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sr in renderers)
        {
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = orderInLayer;
        }
    }
}
