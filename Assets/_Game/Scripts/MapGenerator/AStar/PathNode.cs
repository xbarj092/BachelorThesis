using System;
using System.Collections.Generic;

namespace MapGenerator
{
    /// <summary>
    /// Enum representing the type of a node in a grid.
    /// </summary>
    internal enum NodeType
    {
        None = 0,
        Room = 1,
        Hallway = 2
    }

    internal enum WallDirection
    {
        North,
        East,
        South,
        West
    }

    /// <summary>
    /// Represents a node in a pathfinding grid.
    /// </summary>
    [Serializable]
    internal class PathNode
    {
        /// <summary>
        /// The type of the node.
        /// </summary>
        internal NodeType NodeType = NodeType.None;

        private Grid<PathNode> _grid;

        /// <summary>
        /// The X coordinate of the node in the grid.
        /// </summary>
        internal int X;

        /// <summary>
        /// The Y coordinate of the node in the grid.
        /// </summary>
        internal int Y;

        /// <summary>
        /// The cost of movement from the start node to this node.
        /// </summary>
        internal int GCost;

        /// <summary>
        /// The estimated cost of movement from this node to the target node.
        /// </summary>
        internal int HCost;

        /// <summary>
        /// The total estimated cost of the node (FCost = GCost + HCost).
        /// </summary>
        internal int FCost;

        /// <summary>
        /// The node from which this node was reached most efficiently.
        /// </summary>
        internal PathNode CameFromNode;

        internal List<WallDirection> Walls { get; private set; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="PathNode"/> class with the specified grid and coordinates.
        /// </summary>
        /// <param name="grid">The grid to which the node belongs.</param>
        /// <param name="x">The X coordinate of the node.</param>
        /// <param name="y">The Y coordinate of the node.</param>
        internal PathNode(Grid<PathNode> grid, int x, int y)
        {
            _grid = grid;
            X = x;
            Y = y;
        }

        public void AddWall(WallDirection direction)
        {
            if (!Walls.Contains(direction))
            {
                Walls.Add(direction);
            }
        }

        public void RemoveWall(WallDirection direction)
        {
            if (Walls.Contains(direction))
            {
                Walls.Remove(direction);
            }
        }

        /// <summary>
        /// Calculates the total estimated cost of the node (FCost = GCost + HCost).
        /// </summary>
        internal void CalculateFCost()
        {
            FCost = GCost + HCost;
        }
    }
}
