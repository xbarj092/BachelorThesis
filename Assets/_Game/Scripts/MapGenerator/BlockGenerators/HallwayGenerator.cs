using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapGenerator
{
    /// <summary>
    /// Generates hallways between rooms based on given triangles and placed rooms using A* pathfinding.
    /// </summary>
    internal class HallwayGenerator
    {
        private List<List<PathNode>> _paths = new();
        private List<PathNode> _edgeNodes = new();

        private GameObject _hallwayPrefab;
        private GameObject _hallwayFloorPrefab;

        private AStar _aStar;

        /// <summary>
        /// Generates hallways between rooms.
        /// </summary>
        /// <param name="triangles">List of triangles representing rooms.</param>
        /// <param name="placedRooms">List of placed rooms.</param>
        /// <param name="aStar">A* pathfinding instance.</param>
        /// <param name="primsAlg">Prim's algorithm instance.</param>
        /// <param name="hallwayPrefab">Prefab for hallway walls.</param>
        /// <param name="hallwayFloorPrefab">Prefab for hallway floors.</param>
        internal void GenerateHallways(List<Triangle> triangles, List<Room> placedRooms, AStar aStar,
            PrimsAlg primsAlg, GameObject hallwayPrefab, GameObject hallwayFloorPrefab)
        {
            _hallwayPrefab = hallwayPrefab;
            _hallwayFloorPrefab = hallwayFloorPrefab;

            _aStar = aStar;

            List<RoomConnection> hallways = primsAlg.CreateTriMesh(triangles, placedRooms);
            foreach (RoomConnection roomConnection in hallways)
            {
                GetPath(roomConnection);
            }

            BuildWalls(placedRooms);
        }

        /// <summary>
        /// Gets the path between two rooms.
        /// </summary>
        /// <param name="roomConnection">Connection between two rooms.</param>
        private void GetPath(RoomConnection roomConnection)
        {
            Vector2 startRoomPosition = roomConnection.StartRoom.transform.position;
            Vector2 startRoomScale = roomConnection.StartRoom.transform.localScale;
            Vector2 endRoomPosition = roomConnection.EndRoom.transform.position;
            Vector2 endRoomScale = roomConnection.EndRoom.transform.localScale;

            _aStar.GetGrid().GetXY(startRoomPosition, out int startX, out int startY);
            _aStar.GetGrid().GetXY(endRoomPosition, out int endX, out int endY);

            int minXStart = Mathf.FloorToInt(startX - (startRoomScale.x - 1) / 2);
            int maxXStart = Mathf.CeilToInt(startX + (startRoomScale.x - 1) / 2);
            int minYStart = Mathf.FloorToInt(startY - (startRoomScale.y - 1) / 2);
            int maxYStart = Mathf.CeilToInt(startY + (startRoomScale.y - 1) / 2);

            int minXEnd = Mathf.FloorToInt(endX - (endRoomScale.x - 1) / 2);
            int maxXEnd = Mathf.CeilToInt(endX + (endRoomScale.x - 1) / 2);
            int minYEnd = Mathf.FloorToInt(endY - (endRoomScale.y - 1) / 2);
            int maxYEnd = Mathf.CeilToInt(endY + (endRoomScale.y - 1) / 2);

            PathNode closestStartNode = null;
            PathNode closestEndNode = null;
            float minDistance = float.MaxValue;

            for (int xStart = minXStart; xStart <= maxXStart; xStart++)
            {
                for (int yStart = minYStart; yStart <= maxYStart; yStart++)
                {
                    PathNode startNode = _aStar.GetGrid().GetGridObject(xStart, yStart);

                    if (startNode != null)
                    {
                        for (int xEnd = minXEnd; xEnd <= maxXEnd; xEnd++)
                        {
                            for (int yEnd = minYEnd; yEnd <= maxYEnd; yEnd++)
                            {
                                PathNode endNode = _aStar.GetGrid().GetGridObject(xEnd, yEnd);

                                if (endNode != null)
                                {
                                    float currentDistance = Vector2.Distance(new Vector2(xStart, yStart), new Vector2(xEnd, yEnd));
                                    if (currentDistance <= minDistance)
                                    {
                                        closestStartNode = startNode;
                                        closestEndNode = endNode;
                                        minDistance = currentDistance;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            _paths.Add(_aStar.FindPath(closestStartNode.X, closestStartNode.Y, closestEndNode.X, closestEndNode.Y));
        }

        /// <summary>
        /// Builds hallway walls.
        /// </summary>
        /// <param name="placedRooms">List of placed rooms.</param>
        private void BuildWalls(List<Room> placedRooms)
        {
            foreach (List<PathNode> path in _paths)
            {
                _edgeNodes.AddRange(new List<PathNode>() { path.ElementAt(path.Count - 2), path.ElementAt(1) });

                foreach (PathNode pathNode in path)
                {
                    InstantiateHallwayWalls(pathNode);
                }

                foreach (Room room in placedRooms)
                {
                    Vector2 boxSize = new Vector2(room.transform.localScale.x, room.transform.localScale.y) / 1.1f;
                    Debug.DrawLine(room.transform.position - (Vector3)(boxSize / 2), room.transform.position + (Vector3)(boxSize / 2), Color.red, 50f);

                    foreach (Collider2D collider in Physics2D.OverlapBoxAll(room.transform.position, boxSize, 360))
                    {
                        if (collider.gameObject.CompareTag(GlobalConstants.Tags.Hallway.ToString()))
                        {
                            UnityEngine.Object.Destroy(collider.gameObject);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Instantiates hallway walls.
        /// </summary>
        /// <param name="pathNode">Current path node.</param>
        private void InstantiateHallwayWalls(PathNode pathNode)
        {
            PathNode leftNode = GetHallwayNeighbor(pathNode, pathNode.X - 1, pathNode.Y);
            PathNode rightNode = GetHallwayNeighbor(pathNode, pathNode.X + 1, pathNode.Y);
            PathNode bottomNode = GetHallwayNeighbor(pathNode, pathNode.X, pathNode.Y - 1);
            PathNode topNode = GetHallwayNeighbor(pathNode, pathNode.X, pathNode.Y + 1);

            if (topNode == null && bottomNode == null)
            {
                if (leftNode != null && rightNode == null)
                {
                    InstantiateHallway(_hallwayPrefab, pathNode, new Vector2(0.5f, 0), new Vector2(0.05f, 1));
                }
                else if (leftNode == null && rightNode != null)
                {
                    InstantiateHallway(_hallwayPrefab, pathNode, new Vector2(-0.5f, 0), new Vector2(0.05f, 1));
                }

                InstantiateHallway(_hallwayPrefab, pathNode, new Vector2(0, -0.5f), new Vector2(1, 0.05f));
                InstantiateHallway(_hallwayPrefab, pathNode, new Vector2(0, 0.5f), new Vector2(1, 0.05f));
            }
            else
            {
                if (topNode == null)
                {
                    InstantiateHallway(_hallwayPrefab, pathNode, new Vector2(0, 0.5f), new Vector2(1, 0.05f));
                }
                if (bottomNode == null)
                {
                    InstantiateHallway(_hallwayPrefab, pathNode, new Vector2(0, -0.5f), new Vector2(1, 0.05f));
                }

                InstantiateHallwayChunk(pathNode, leftNode, rightNode);
            }

            InstantiateHallway(_hallwayFloorPrefab, pathNode, Vector2.zero, new Vector2(1, 1));
        }

        /// <summary>
        /// Gets the neighboring hallway node.
        /// </summary>
        /// <param name="pathNode">Current path node.</param>
        /// <param name="x">X position of the neighboring node.</param>
        /// <param name="y">Y position of the neighboring node.</param>
        /// <returns>The neighboring hallway node if found, otherwise null.</returns>
        private PathNode GetHallwayNeighbor(PathNode pathNode, int x, int y)
        {
            return _aStar.GetNeighbourList(pathNode).FirstOrDefault(n => n.NodeType == NodeType.Hallway && n.X == x && n.Y == y);
        }

        /// <summary>
        /// Handles instantiation of hallway walls based on neighboring nodes.
        /// </summary>
        /// <param name="currentNode">Current path node.</param>
        /// <param name="firstNode">First neighboring node.</param>
        /// <param name="secondNode">Second neighboring node.</param>
        private void InstantiateHallwayChunk(PathNode currentNode, PathNode firstNode, PathNode secondNode)
        {
            if (firstNode == null && secondNode != null)
            {
                InstantiateHallway(_hallwayPrefab, currentNode, new Vector2(-0.5f, 0), new Vector2(0.05f, 1));
            }
            else if (firstNode != null && secondNode == null)
            {
                InstantiateHallway(_hallwayPrefab, currentNode, new Vector2(0.5f, 0), new Vector2(0.05f, 1));
            }
            else if (firstNode == null && secondNode == null)
            {
                InstantiateHallway(_hallwayPrefab, currentNode, new Vector2(0.5f, 0), new Vector2(0.05f, 1));
                InstantiateHallway(_hallwayPrefab, currentNode, new Vector2(-0.5f, 0), new Vector2(0.05f, 1));
            }
        }

        /// <summary>
        /// Creates entrances for rooms connected to hallways.
        /// </summary>
        internal void MakeRoomEntrances()
        {
            foreach (PathNode node in _edgeNodes)
            {
                Vector3 worldPos = _aStar.GetGrid().GetWorldPosition(node.X, node.Y);

                DestroyAdjacentRoom(worldPos, Vector2.left);
                DestroyAdjacentRoom(worldPos, Vector2.right);
                DestroyAdjacentRoom(worldPos, Vector2.up);
                DestroyAdjacentRoom(worldPos, Vector2.down);
            }
        }

        /// <summary>
        /// Destroys adjacent rooms based on hallway directions.
        /// </summary>
        /// <param name="position">Position of the hallway.</param>
        /// <param name="direction">Direction of the hallway.</param>
        private void DestroyAdjacentRoom(Vector2 position, Vector2 direction)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(position, direction, 0.5f);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.gameObject.CompareTag(GlobalConstants.Tags.Room.ToString()))
                {
                    UnityEngine.Object.Destroy(hit.collider.gameObject);
                }
            }
        }

        /// <summary>
        /// Instantiates a hallway wall.
        /// </summary>
        /// <param name="prefab">Prefab for the hallway wall.</param>
        /// <param name="pathNode">Current path node.</param>
        /// <param name="hallwayOffset">Offset for the hallway wall.</param>
        /// <param name="hallwayScale">Scale for the hallway wall.</param>
        private void InstantiateHallway(GameObject prefab, PathNode pathNode, Vector2 hallwayOffset, Vector2 hallwayScale)
        {
            Vector2 wallPosition = new Vector2(pathNode.X, pathNode.Y) + hallwayOffset;
            UnityEngine.Object.Instantiate(prefab, wallPosition, Quaternion.identity).transform.localScale = hallwayScale;
        }
    }
}
