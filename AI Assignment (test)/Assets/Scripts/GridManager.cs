using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    // https://pavcreations.com/pathfinding-with-a-star-algorithm-in-unity-small-game-project/

    // References to grid and tilemaps
    public Grid grid;
    public Tilemap roadTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap intersectionTilemap;

    // Stores nodes and neighbors
    private Dictionary<Vector2, List<Vector2>> gridNodes;

    void Start()
    {
        InitializeGrid();
    }

    void OnDrawGizmos()
    {
        DrawGizmos();
    }

    //Draw nodes to the scene
    void DrawGizmos()
    {
        if (gridNodes == null)
            return;

        Gizmos.color = Color.blue;

        foreach (var node in gridNodes)
        {
            Gizmos.DrawSphere(node.Key, 0.1f);
        }
    }

    void InitializeGrid()
    {
        gridNodes = new Dictionary<Vector2, List<Vector2>>();

        ProcessTilemap(roadTilemap);
       // ProcessTilemap(intersectionTilemap);
    }

    void ProcessTilemap(Tilemap tilemap)
    {
        // Get the bounds of the tilemap
        BoundsInt bounds = tilemap.cellBounds;

        // Loop through all the positions within the bounds
        foreach (Vector3Int position in bounds.allPositionsWithin)
        {
            // Get world position of the cell
            Vector2 worldPosition = grid.GetCellCenterWorld(position);

            // Check if the tile is not an obstacle
            if (IsTileWalkable(worldPosition, tilemap))
            {
                // Get neighbors for current node and add it to gridNodes
                List<Vector2> neighbors = GetNeighbors(worldPosition);
                gridNodes.Add(worldPosition, neighbors);
            }
        }
    }

    public List<Vector2> GetNeighbors(Vector2 position)
    {
        // Create a list to store neighbor nodes
        List<Vector2> neighbors = new List<Vector2>();

        // Get the cell pos of the current world pos
        Vector3Int cellPosition = grid.WorldToCell(position);

        // Check for neighbours in cardinal directions
        Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        // Loop through the directions to find neighbors
        foreach (Vector3Int dir in directions)
        {
            // Calculate the cell position of the neighbor
            Vector3Int neighborCell = cellPosition + dir;

            // Get the world position of the neighbor
            Vector2 neighborPos = grid.GetCellCenterWorld(neighborCell);

            // If the tile is walkable, add to list
            if (IsTileWalkable(neighborPos, roadTilemap) || IsTileWalkable(neighborPos, intersectionTilemap))
            {
                neighbors.Add(neighborPos);
            }
        }

        // Return the list of neighbors
        return neighbors;
    }

    bool IsTileWalkable(Vector2 position, Tilemap tilemap) 
    {
        // Get the tile at the given position
        TileBase tile = tilemap.GetTile(tilemap.WorldToCell(position));

        // Return true if the tile is walkable
        return tile != null;
    }

    public Vector2 GetNearestNode(Vector2 position)
    {
        // Find the cell position of the given world position
        Vector3Int cellPosition = grid.WorldToCell(position);

        // Return the world position
        return grid.GetCellCenterWorld(cellPosition);
    }




    //////////////////////// A* Algorithm to find a path from the start to target position ////////////////////////
    public List<Vector2> FindPath(Vector2 startPosition, Vector2 targetPosition)
    {
        // Find the nearest nodes for start and target position
        Vector2 startNode = GetNearestNode(startPosition);
        Vector2 targetNode = GetNearestNode(targetPosition);

        // Initialize data structures
        HashSet<Vector2> closedSet = new HashSet<Vector2>();
        Dictionary<Vector2, float> gScore = new Dictionary<Vector2, float>();
        Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();

        PriorityQueue<Vector2> openSet = new PriorityQueue<Vector2>();
        openSet.Enqueue(startNode, 0);

        gScore[startNode] = 0;

        // Main A* loop
        while (openSet.Count > 0)
        {
            Vector2 current = openSet.Dequeue();

            if (current == targetNode)
            {
                // If the target is reached, reconstruct and return the path
                return ReconstructPath(cameFrom, startNode, targetNode);
            }

            // Add current node to the closed set, as it has been explored
            closedSet.Add(current);

            // Explore neighbors of the current node
            foreach (Vector2 neighbor in gridNodes[current]) // Need to look at again
            {
                // Skip neighbors in the closed set
                if (closedSet.Contains(neighbor))
                    continue;

                // Calculate the G score from the start node to this neighbor
                float tentativeGScore = gScore[current] + Vector2.Distance(current, neighbor);

                // Check if this G score is better than the previous G score of the neighbor
                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    // Update the G score
                    gScore[neighbor] = tentativeGScore;
                    cameFrom[neighbor] = current;

                    // Calculate the heuristic value from this neighbor to the target node
                    float heuristic = Vector2.Distance(neighbor, targetNode);

                    // Calculate the priority for the neighbor based on the G score and heuristic value
                    float priority = tentativeGScore + heuristic;

                    // Check if the neighbor is already in the open set
                    if (openSet.Contains(neighbor))
                        // If yes, update its priority
                        openSet.UpdatePriority(neighbor, priority);
                    else
                        // If not, enqueue the neighbor with its priority into the open set
                        openSet.Enqueue(neighbor, priority);
                }
            }
        }

        // No path found
        return null;
    }

    // Reconstruct the path from start to finish using the "cameFrom" dictionary
    private List<Vector2> ReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 start, Vector2 end)
    {
        List<Vector2> path = new List<Vector2>();
        Vector2 current = end;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }
}

// Priority Queue Implementation
public class PriorityQueue<T>
{
    private List<T> elements = new List<T>();
    private Dictionary<T, float> priorities = new Dictionary<T, float>();

    // Get the count of elements in the priority queue
    public int Count { get { return elements.Count; } }

    // Enqueue an element with a specified priority
    public void Enqueue(T element, float priority)
    {
        // Add the element to the list
        elements.Add(element);

        // Add it to the dictionary
        priorities[element] = priority;

        // Sort the elements by their priorities
        elements.Sort((a, b) => priorities[a].CompareTo(priorities[b]));
    }

    // Dequeue the element with the highest priority
    public T Dequeue()
    {
        // Get the element with the highest priority
        T element = elements[0];

        // Remove it from the list
        elements.RemoveAt(0);

        // Remove it from the dictionary
        priorities.Remove(element);

        // Return the dequeued element
        return element;
    }

    // Check if the queue contains a specific element
    public bool Contains(T element)
    {
        // Check if it is in the dictionary
        return priorities.ContainsKey(element);
    }

    // Update the priority of an element in the queue
    public void UpdatePriority(T element, float priority)
    {
        // Update it in the dictionary
        priorities[element] = priority;

        // Sort the elements based on their priorities
        elements.Sort((a, b) => priorities[a].CompareTo(priorities[b]));
    }
}