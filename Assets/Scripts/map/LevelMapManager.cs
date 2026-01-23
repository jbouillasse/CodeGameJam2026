using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelMapManager : MonoBehaviour
{
    public static LevelMapManager Instance { get; private set; }
    
    [Header("Tous les nodes (auto-détectés si vide)")]
    public List<LevelNode> nodes = new List<LevelNode>();
    
    [Header("Délai entre chaque déblocage")]
    public float unlockDelay = 0.3f;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        if (nodes.Count == 0)
        {
            nodes = FindObjectsByType<LevelNode>(FindObjectsSortMode.None).ToList();
        }
        
        // ✅ NOUVEAU : Vérifier si on revient d'un niveau complété
        CheckForCompletedLevel();
    }
    
    // ✅ NOUVELLE MÉTHODE
    private void CheckForCompletedLevel()
    {
        int justCompleted = PlayerPrefs.GetInt("JustCompletedLevel", -1);
        
        if (justCompleted >= 0)
        {
            Debug.Log($"Retour de victoire ! Déblocage du niveau {justCompleted + 1}");
            
            // Effacer le flag
            PlayerPrefs.DeleteKey("JustCompletedLevel");
            PlayerPrefs.Save();
            
            // Débloquer le niveau suivant
            StartCoroutine(UnlockAllWithIndex(justCompleted + 1));
        }
    }
    
    public void CompleteLevel(int levelIndex)
    {
        string currentID = PlayerPrefs.GetString("CurrentLevelID", "");
        
        foreach (var node in nodes)
        {
            if (node.UniqueID == currentID)
            {
                node.Complete();
                break;
            }
        }
        
        int nextIndex = levelIndex + 1;
        StartCoroutine(UnlockAllWithIndex(nextIndex));
    }
    
    private System.Collections.IEnumerator UnlockAllWithIndex(int index)
    {
        yield return new WaitForSeconds(0.5f); // Petit délai pour que tout soit chargé
        
        var nodesToUnlock = nodes.Where(n => n.levelIndex == index && n.GetState() == LevelState.Locked).ToList();
        
        Debug.Log($"Déblocage de {nodesToUnlock.Count} niveau(x) avec index {index}");
        
        foreach (var node in nodesToUnlock)
        {
            yield return new WaitForSeconds(unlockDelay);
            node.Unlock();
            Debug.Log($"Niveau débloqué : {node.name}");
        }
    }
    
    public List<LevelNode> GetNodesWithIndex(int index)
    {
        return nodes.Where(n => n.levelIndex == index).ToList();
    }
    
    [ContextMenu("Reset Progress")]
    public void ResetProgress()
    {
        foreach (var node in nodes)
            PlayerPrefs.DeleteKey($"Level_{node.UniqueID}");
        
        PlayerPrefs.DeleteKey("CurrentLevelID");
        PlayerPrefs.DeleteKey("CurrentLevelIndex");
        PlayerPrefs.DeleteKey("CurrentPathNodeID");
        PlayerPrefs.DeleteKey("JustCompletedLevel");
        PlayerPrefs.DeleteKey("DebugLevel");
        PlayerPrefs.Save();
        
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.name);
    }
}