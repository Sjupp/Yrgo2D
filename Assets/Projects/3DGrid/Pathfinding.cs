using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Node
{
    public Vector3 worldPosition;
    public int gridX;
    public int gridZ;

    public int gCost; // Distance from start node
    public int hCost; // Distance to end node
    public Node parent;

    public int fCost { get { return gCost + hCost; } }

    public Node(Vector3 _worldPos, int _gridX, int _gridZ)
    {
        worldPosition = _worldPos;
        gridX = _gridX;
        gridZ = _gridZ;
    }
}

public class Pathfinding : MonoBehaviour
{
    public bool allowDiagonal = true;  // Set this in inspector

    public GameObject[] gridObjects; // Assign all grid objects in inspector
    private Node[,] grid;

    private int gridSizeX;
    private int gridSizeZ;

    public Transform player;
    public float moveSpeed = 5f;

    private List<Node> path;

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        // Assuming gridObjects are ordered row-wise or column-wise (you can customize)
        // For simplicity, assuming square grid (gridSizeX x gridSizeZ)
        gridSizeX = Mathf.RoundToInt(Mathf.Sqrt(gridObjects.Length));
        gridSizeZ = gridSizeX;

        grid = new Node[gridSizeX, gridSizeZ];

        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeZ; j++)
            {
                int index = i * gridSizeZ + j;
                grid[i, j] = new Node(gridObjects[index].transform.position, i, j);
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is in gridObjects array
                foreach (var gridObj in gridObjects)
                {
                    if (hit.collider.gameObject == gridObj)
                    {
                        // Move player to the clicked position on the grid
                        MoveTo(gridObj.transform.position);
                        break;
                    }
                }
            }
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        Node startNode = GetNodeFromWorldPoint(player.position);
        Node targetNode = GetNodeFromWorldPoint(targetPosition);

        path = FindPath(startNode, targetNode);
        if (path != null && path.Count > 0)
        {
            StopAllCoroutines();
            StartCoroutine(FollowPath());
        }
    }

    Node GetNodeFromWorldPoint(Vector3 worldPos)
    {
        // Find closest node in the grid
        Node closestNode = null;
        float shortestDist = Mathf.Infinity;

        foreach (var node in grid)
        {
            float dist = Vector3.Distance(worldPos, node.worldPosition);
            if (dist < shortestDist)
            {
                shortestDist = dist;
                closestNode = node;
            }
        }

        return closestNode;
    }

    List<Node> FindPath(Node startNode, Node targetNode)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

        // Straight movement cost is 10, diagonal is 14 (if needed)
        if (dstX > dstZ)
            return 14 * dstZ + 10 * (dstX - dstZ);
        return 14 * dstX + 10 * (dstZ - dstX);
    }

    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0)
                    continue;

                if (!allowDiagonal && Mathf.Abs(x) + Mathf.Abs(z) > 1)
                    continue; // Skip diagonals if not allowed

                int checkX = node.gridX + x;
                int checkZ = node.gridZ + z;

                if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ)
                {
                    neighbors.Add(grid[checkX, checkZ]);
                }
            }
        }

        return neighbors;
    }

    IEnumerator FollowPath()
    {
        foreach (Node node in path)
        {
            while (Vector3.Distance(player.position, node.worldPosition) > 0.1f)
            {
                player.position = Vector3.MoveTowards(player.position, node.worldPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
