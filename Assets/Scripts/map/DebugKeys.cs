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
            // Simule qu'on a joué un niveau avec cet index
            var nodes = LevelMapManager.Instance?.GetNodesWithIndex(debugIndex);
            if (nodes != null && nodes.Count > 0)
            {
                // Prend le premier unlocked
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
        
        // R = reset tout
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            debugIndex = 0;
            LevelMapManager.Instance?.ResetProgress();
        }
    }
    #endif
}