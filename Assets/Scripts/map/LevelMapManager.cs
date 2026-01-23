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
        var nodesToUnlock = nodes.Where(n => n.levelIndex == index && n.GetState() == LevelState.Locked).ToList();
        
        foreach (var node in nodesToUnlock)
        {
            yield return new WaitForSeconds(unlockDelay);
            node.Unlock();
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
        PlayerPrefs.DeleteKey("DebugLevel");
        PlayerPrefs.Save();
        
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.name);
    }
}