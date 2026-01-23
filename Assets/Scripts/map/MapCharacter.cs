using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapCharacter : MonoBehaviour
{
    public static MapCharacter Instance { get; private set; }
    
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float nodeReachedThreshold = 0.05f;
    
    [Header("Animation (optionnel)")]
    public Animator animator;
    public string walkingParam = "isWalking";
    public string dirXParam = "dirX";
    public string dirYParam = "dirY";
    
    [Header("Path Configuration")]
    public PathNode startNode;
    
    private PathNode currentNode;
    private bool isMoving = false;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        // NE RIEN FAIRE AU START - on garde la position de l'éditeur
        if (startNode != null)
        {
            currentNode = startNode;
            Debug.Log($"MapCharacter prêt sur {startNode.name}");
        }
    }
    
    public void MoveToLevel(LevelNode targetLevel)
    {
        if (isMoving) return;
        if (targetLevel.GetState() == LevelState.Locked) return;
        
        PathNode targetPathNode = FindPathNodeForLevel(targetLevel);
        
        if (targetPathNode == null)
        {
            targetLevel.LaunchLevel();
            return;
        }
        
        if (currentNode == targetPathNode)
        {
            targetLevel.LaunchLevel();
            return;
        }
        
        List<PathNode> path = FindPath(currentNode, targetPathNode);
        
        if (path != null && path.Count > 0)
        {
            StartCoroutine(FollowPath(path, targetLevel));
        }
        else
        {
            targetLevel.LaunchLevel();
        }
    }
    
    private PathNode FindPathNodeForLevel(LevelNode level)
    {
        var allPathNodes = FindObjectsByType<PathNode>(FindObjectsSortMode.None);
        return allPathNodes.FirstOrDefault(pn => pn.associatedLevel == level);
    }
    
    private IEnumerator FollowPath(List<PathNode> path, LevelNode destinationLevel)
    {
        isMoving = true;
        SetWalking(true);
        
        foreach (PathNode node in path)
        {
            yield return StartCoroutine(MoveToNode(node));
            currentNode = node;
        }
        
        SetWalking(false);
        isMoving = false;
        
        yield return new WaitForSeconds(0.2f);
        destinationLevel.LaunchLevel();
    }
    
    private IEnumerator MoveToNode(PathNode targetNode)
    {
        Vector3 targetPos = new Vector3(
            targetNode.transform.position.x,
            targetNode.transform.position.y,
            transform.position.z
        );
        
        Vector2 direction = (targetPos - transform.position).normalized;
        UpdateAnimationDirection(direction);
        
        while (Vector3.Distance(transform.position, targetPos) > nodeReachedThreshold)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
        
        transform.position = targetPos;
    }
    
    private void UpdateAnimationDirection(Vector2 direction)
    {
        if (animator == null) return;
        
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            animator.SetFloat(dirXParam, direction.x > 0 ? 1 : -1);
            animator.SetFloat(dirYParam, 0);
        }
        else
        {
            animator.SetFloat(dirXParam, 0);
            animator.SetFloat(dirYParam, direction.y > 0 ? 1 : -1);
        }
    }
    
    private void SetWalking(bool walking)
    {
        if (animator != null)
            animator.SetBool(walkingParam, walking);
    }
    
    #region Pathfinding
    
    private List<PathNode> FindPath(PathNode start, PathNode end)
    {
        if (start == null || end == null) return null;
        
        Queue<PathNode> queue = new Queue<PathNode>();
        Dictionary<PathNode, PathNode> cameFrom = new Dictionary<PathNode, PathNode>();
        HashSet<PathNode> visited = new HashSet<PathNode>();
        
        queue.Enqueue(start);
        visited.Add(start);
        cameFrom[start] = null;
        
        while (queue.Count > 0)
        {
            PathNode current = queue.Dequeue();
            
            if (current == end)
                return ReconstructPath(cameFrom, end);
            
            foreach (PathNode neighbor in current.connectedNodes)
            {
                if (neighbor != null && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }
        
        return null;
    }
    
    private List<PathNode> ReconstructPath(Dictionary<PathNode, PathNode> cameFrom, PathNode end)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode current = end;
        
        while (current != null)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        
        path.Reverse();
        if (path.Count > 0) path.RemoveAt(0);
        return path;
    }
    
    #endregion
    
    public bool IsMoving => isMoving;
}