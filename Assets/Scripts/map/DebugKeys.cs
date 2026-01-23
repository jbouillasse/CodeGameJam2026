using UnityEngine;
using UnityEngine.InputSystem;

public class DebugKeys : MonoBehaviour
{
#if UNITY_EDITOR
    private int debugIndex = 0;
    
    void Update()
    {
        // Espace = compléter le niveau actuel
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            var nodes = LevelMapManager.Instance?.GetNodesWithIndex(debugIndex);
            if (nodes != null && nodes.Count > 0)
            {
                var node = nodes.Find(n => n.GetState() == LevelState.Unlocked);
                if (node != null)
                {
                    PlayerPrefs.SetString("CurrentLevelID", node.UniqueID);
                    LevelMapManager.Instance.CompleteLevel(debugIndex);
                    Debug.Log($"Niveau index {debugIndex} complété !");
                    debugIndex++;
                }
            }
        }
        
        // R = reset tout (y compris position personnage, qui sera replacé sur le node 0)
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            debugIndex = 0;
            PlayerPrefs.DeleteKey("CurrentPathNodeID");
            
            // On force le personnage à réapparaître sur le node 0 au prochain lancement de la map
            PlayerPrefs.SetInt("LastPlayedLevelIndex", 0);         // ← Pour le placement par index
            PlayerPrefs.SetString("LastPlayedLevelUniqueID", "");  // ← (Optionnel) Pour forcer le fallback index si tu relances sans ID spécifique
            PlayerPrefs.Save();

            LevelMapManager.Instance?.ResetProgress();
        }
    }
#endif
}