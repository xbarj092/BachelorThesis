using MapGenerator;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class implementing the A* pathfinding algorithm.
/// </summary>
public class AStar
{
    internal Grid<PathNode> Grid;

    private List<PathNode> _openList;
    private List<PathNode> _closedList;

    private const int DUNGEON_SIZE_X = 50;
    private const int DUNGEON_SIZE_Y = 50;

    /// <summary>
    /// Initializes a new instance of the <see cref="AStar"/> class with the specified grid width and height.
    /// </summary>
    /// <param name="width">The width of the grid.</param>
    /// <param name="height">The height of the grid.</param>
    internal AStar()
    {
        Grid = new Grid<PathNode>(DUNGEON_SIZE_X + 10, DUNGEON_SIZE_Y + 10, 1, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }

    /// <summary>
    /// Gets the grid used by the A* algorithm.
    /// </summary>
    /// <returns>The grid used by the algorithm.</returns>
    internal Grid<PathNode> GetGrid()
    {
        return Grid;
    }

    /// <summary>
    /// Finds a shortest path from the start position to the end position using the A* algorithm.
    /// </summary>
    /// <param name="startX">The X coordinate of the start position.</param>
    /// <param name="startY">The Y coordinate of the start position.</param>
    /// <param name="endX">The X coordinate of the end position.</param>
    /// <param name="endY">The Y coordinate of the end position.</param>
    /// <returns>A list of path nodes representing the path from the start to the end position, or null if no path is found.</returns>
    internal List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = Grid.GetGridObject(startX, startY);
        PathNode endNode = Grid.GetGridObject(endX, endY);

        _openList = new List<PathNode> { startNode };
        _closedList = new List<PathNode>();

        for (int x = 0; x < Grid.GetWidth(); x++)
        {
            for (int y = 0; y < Grid.GetHeight(); y++)
            {
                PathNode pathNode = Grid.GetGridObject(x, y);
                pathNode.GCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.CameFromNode = null;
            }
        }

        startNode.GCost = 0;
        startNode.HCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (_openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(_openList);
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

            _openList.Remove(currentNode);
            _closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (!_closedList.Contains(neighbourNode) && IsNodeWalkable(neighbourNode))
                {
                    int tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost < neighbourNode.GCost)
                    {
                        neighbourNode.CameFromNode = currentNode;
                        neighbourNode.GCost = tentativeGCost;
                        neighbourNode.HCost = CalculateDistanceCost(neighbourNode, endNode);
                        neighbourNode.CalculateFCost();

                        if (!_openList.Contains(neighbourNode))
                        {
                            _openList.Add(neighbourNode);
                        }
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Determines if the given node is walkable.
    /// </summary>
    /// <param name="node">The node to check.</param>
    /// <returns>True if the node is walkable, otherwise false.</returns>
    internal virtual bool IsNodeWalkable(PathNode node)
    {
        return true;
    }

    /// <summary>
    /// Gets the list of neighboring nodes for the specified node.
    /// </summary>
    /// <param name="currentNode">The node for which to find neighbors.</param>
    /// <returns>The list of neighboring nodes.</returns>
    internal virtual List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new();
        if (currentNode.X - 1 >= 0) neighbourList.Add(GetNode(currentNode.X - 1, currentNode.Y));
        if (currentNode.X + 1 < Grid.GetWidth()) neighbourList.Add(GetNode(currentNode.X + 1, currentNode.Y));
        if (currentNode.Y - 1 >= 0) neighbourList.Add(GetNode(currentNode.X, currentNode.Y - 1));
        if (currentNode.Y + 1 < Grid.GetHeight()) neighbourList.Add(GetNode(currentNode.X, currentNode.Y + 1));
        return neighbourList;
    }

    /// <summary>
    /// Retrieves the path node at the specified coordinates from the grid.
    /// </summary>
    /// <param name="x">The X coordinate of the node.</param>
    /// <param name="y">The Y coordinate of the node.</param>
    /// <returns>The path node at the specified coordinates.</returns>
    internal PathNode GetNode(int x, int y)
    {
        return Grid.GetGridObject(x, y);
    }

    /// <summary>
    /// Calculates the path from the end node back to the start node.
    /// </summary>
    /// <param name="endNode">The end node of the path.</param>
    /// <returns>A list of path nodes representing the calculated path.</returns>
    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new() { endNode };
        PathNode currentNode = endNode;
        while (currentNode.CameFromNode != null)
        {
            path.Add(currentNode.CameFromNode);
            currentNode = currentNode.CameFromNode;
        }

        path.Reverse();
        return path;
    }

    /// <summary>
    /// Calculates the distance cost between two path nodes.
    /// </summary>
    /// <param name="startNode">The starting node.</param>
    /// <param name="endNode">The ending node.</param>
    /// <returns>The distance cost between the start and end nodes.</returns>
    internal virtual int CalculateDistanceCost(PathNode startNode, PathNode endNode)
    {
        int xDistance = Mathf.Abs(startNode.X - endNode.X);
        int yDistance = Mathf.Abs(startNode.Y - endNode.Y);

        int straightCost = 10;
        int diagonalCost = 21;

        int roomDistanceCost = 10;
        int hallwayDistanceCost = 1;
        int noneDistanceCost = 5;

        int additionalCost = 0;
        if (startNode.NodeType == NodeType.Room)
            additionalCost += roomDistanceCost;
        else if (startNode.NodeType == NodeType.Hallway)
            additionalCost += hallwayDistanceCost;
        else if (startNode.NodeType == NodeType.None)
            additionalCost += noneDistanceCost;

        if (endNode.NodeType == NodeType.Room)
            additionalCost += roomDistanceCost;
        else if (endNode.NodeType == NodeType.Hallway)
            additionalCost += hallwayDistanceCost;
        else if (endNode.NodeType == NodeType.None)
            additionalCost += noneDistanceCost;

        int remaining = Mathf.Abs(xDistance - yDistance);
        return (diagonalCost * Mathf.Min(xDistance, yDistance) + straightCost * remaining) + additionalCost;
    }

    /// <summary>
    /// Gets the node with the lowest F cost from the list of path nodes.
    /// </summary>
    /// <param name="pathNodes">The list of path nodes to search.</param>
    /// <returns>The path node with the lowest F cost.</returns>
    private PathNode GetLowestFCostNode(List<PathNode> pathNodes)
    {
        PathNode lowestFCostNode = pathNodes[0];
        for (int i = 0; i < pathNodes.Count; i++)
        {
            if (pathNodes[i].FCost < lowestFCostNode.FCost)
            {
                lowestFCostNode = pathNodes[i];
            }
        }
        return lowestFCostNode;
    }

    /// <summary>
    /// Gets all walkable nodes in the grid.
    /// </summary>
    /// <returns>A list of all walkable nodes in the grid.</returns>
    internal List<PathNode> GetAllWalkableNodes()
    {
        List<PathNode> walkableNodes = new();

        for (int x = 0; x < Grid.GetWidth(); x++)
        {
            for (int y = 0; y < Grid.GetHeight(); y++)
            {
                PathNode node = Grid.GetGridObject(x, y);
                if (node?.NodeType == NodeType.Room || node?.NodeType == NodeType.Hallway)
                {
                    walkableNodes.Add(node);
                }
            }
        }

        return walkableNodes;
    }
}
