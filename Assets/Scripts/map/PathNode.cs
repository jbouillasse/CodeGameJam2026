using UnityEngine;
using System.Collections.Generic;

public class PathNode : MonoBehaviour
{
    [Header("Connexions")]
    public List<PathNode> connectedNodes = new List<PathNode>();
    
    [Header("Niveau associé (optionnel)")]
    public LevelNode associatedLevel;
    
    [Header("Gizmos")]
    public Color gizmoColor = Color.yellow;
    public float gizmoRadius = 0.3f;
    
    public string NodeID => $"PathNode_{gameObject.name}";
    
    void OnDrawGizmos()
    {
        if (associatedLevel != null)
            Gizmos.color = Color.green;
        else
            Gizmos.color = gizmoColor;
        
        Gizmos.DrawSphere(transform.position, gizmoRadius);
        
        Gizmos.color = Color.cyan;
        foreach (var node in connectedNodes)
        {
            if (node != null)
            {
                Gizmos.DrawLine(transform.position, node.transform.position);
            }
        }
    }
}