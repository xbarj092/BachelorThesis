using MapGenerator;
using System.Collections.Generic;
using UnityEngine;

public class WalkableAStar : AStar
{
    internal WalkableAStar(Grid<PathNode> grid)
    {
        Grid = grid;
    }

    internal override bool IsNodeWalkable(PathNode node)
    {
        return node?.NodeType == NodeType.Room || node?.NodeType == NodeType.Hallway;
    }

    internal override List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new();

        if (currentNode.X - 1 >= 0 && !currentNode.Walls.Contains(WallDirection.West))
        {
            PathNode leftNode = GetNode(currentNode.X - 1, currentNode.Y);
            if (!leftNode.Walls.Contains(WallDirection.East)) neighbourList.Add(leftNode);
        }
        if (currentNode.X + 1 < Grid.GetWidth() && !currentNode.Walls.Contains(WallDirection.East))
        {
            PathNode rightNode = GetNode(currentNode.X + 1, currentNode.Y);
            if (!rightNode.Walls.Contains(WallDirection.West)) neighbourList.Add(rightNode);
        }
        if (currentNode.Y - 1 >= 0 && !currentNode.Walls.Contains(WallDirection.South))
        {
            PathNode bottomNode = GetNode(currentNode.X, currentNode.Y - 1);
            if (!bottomNode.Walls.Contains(WallDirection.North)) neighbourList.Add(bottomNode);
        }
        if (currentNode.Y + 1 < Grid.GetHeight() && !currentNode.Walls.Contains(WallDirection.North))
        {
            PathNode topNode = GetNode(currentNode.X, currentNode.Y + 1);
            if (!topNode.Walls.Contains(WallDirection.South)) neighbourList.Add(topNode);
        }

        return neighbourList;
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
