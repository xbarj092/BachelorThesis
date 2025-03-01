using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MapGenerator
{
    /// <summary>
    /// Represents a grid data structure.
    /// </summary>
    /// <typeparam name="TGridObject">The type of objects stored in the grid.</typeparam>
    internal class Grid<TGridObject>
    {
        private int _width;
        private int _height;
        private float _cellSize;
        private TGridObject[,] _gridArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid{TGridObject}"/> class with the specified dimensions and cell size.
        /// </summary>
        /// <param name="width">The width of the grid in cells.</param>
        /// <param name="height">The height of the grid in cells.</param>
        /// <param name="cellSize">The size of each cell in world units.</param>
        /// <param name="createGridObject">A function to create grid objects at each cell position.</param>
        internal Grid(int width, int height, float cellSize, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;

            _gridArray = new TGridObject[width, height];

            for (int x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < _gridArray.GetLength(1); y++)
                {
                    if (createGridObject != null)
                    {
                        _gridArray[x, y] = createGridObject(this, x, y);
                    }
                }
            }
        }

        /// <summary>
        /// Converts grid coordinates to world position.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell.</param>
        /// <param name="y">The y-coordinate of the cell.</param>
        /// <returns>The world position of the specified cell.</returns>
        internal Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector2(x, y) * _cellSize;
        }

        /// <summary>
        /// Converts world position to grid coordinates.
        /// </summary>
        /// <param name="worldPosition">The world position to convert.</param>
        /// <param name="x">The x-coordinate of the resulting grid position.</param>
        /// <param name="y">The y-coordinate of the resulting grid position.</param>
        internal void GetXY(Vector2 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt(worldPosition.x / _cellSize);
            y = Mathf.FloorToInt(worldPosition.y / _cellSize);
        }

        /// <summary>
        /// Sets the object at the specified grid coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell.</param>
        /// <param name="y">The y-coordinate of the cell.</param>
        /// <param name="value">The value to set at the specified cell.</param>
        internal void SetGridObject(int x, int y, TGridObject value)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                _gridArray[x, y] = value;
            }
        }

        /// <summary>
        /// Sets the object at the grid position corresponding to the specified world position.
        /// </summary>
        /// <param name="worldPosition">The world position of the cell.</param>
        /// <param name="value">The value to set at the specified cell.</param>
        internal void SetGridObject(Vector2 worldPosition, TGridObject value)
        {
            GetXY(worldPosition, out int x, out int y);
            SetGridObject(x, y, value);
        }

        /// <summary>
        /// Gets the object at the specified grid coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the cell.</param>
        /// <param name="y">The y-coordinate of the cell.</param>
        /// <returns>The object at the specified grid coordinates.</returns>
        internal TGridObject GetGridObject(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                return _gridArray[x, y];
            }

            return default(TGridObject);
        }

        /// <summary>
        /// Gets the object at the grid position corresponding to the specified world position.
        /// </summary>
        /// <param name="worldPosition">The world position of the cell.</param>
        /// <returns>The object at the specified grid coordinates.</returns>
        internal TGridObject GetGridObject(Vector2 worldPosition)
        {
            GetXY(worldPosition, out int x, out int y);
            return GetGridObject(x, y);
        }

        /// <summary>
        /// Gets the width of the grid.
        /// </summary>
        /// <returns>The width of the grid in cells.</returns>
        internal int GetWidth()
        {
            return _width;
        }

        /// <summary>
        /// Gets the height of the grid.
        /// </summary>
        /// <returns>The height of the grid in cells.</returns>
        internal int GetHeight()
        {
            return _height;
        }

        internal List<TGridObject> GetAllNodes()
        {
            List<TGridObject> nodes = new();

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    TGridObject pathNode = GetGridObject(x, y);
                    nodes.Add(pathNode);
                }
            }

            return nodes;
        }
    }
}
