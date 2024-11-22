using MapGenerator;
using UnityEngine;

public class WalkableAStar : AStar
{
    internal WalkableAStar(Grid<PathNode> grid)
    {
        Grid = grid;
    }

    /// <summary>
    /// Overrides the default walkability check to consider only walkable nodes.
    /// </summary>
    /// <param name="node">The node to check.</param>
    /// <returns>True if the node is walkable, otherwise false.</returns>
    internal override bool IsNodeWalkable(PathNode node)
    {
        return node?.NodeType == NodeType.Room || node?.NodeType == NodeType.Hallway;
    }

    internal override int CalculateDistanceCost(PathNode startNode, PathNode endNode)
    {
        int xDistance = Mathf.Abs(startNode.X - endNode.X);
        int yDistance = Mathf.Abs(startNode.Y - endNode.Y);

        int straightCost = 10;
        int diagonalCost = 20;

        int remaining = Mathf.Abs(xDistance - yDistance);
        return diagonalCost * Mathf.Min(xDistance, yDistance) + straightCost * remaining;
    }
}
