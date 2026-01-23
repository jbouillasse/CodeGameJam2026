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
        
        // R = reset tout (y compris position personnage)
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            debugIndex = 0;
            PlayerPrefs.DeleteKey("CurrentPathNodeID");
            LevelMapManager.Instance?.ResetProgress();
        }
    }
#endif
}