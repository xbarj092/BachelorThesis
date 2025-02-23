using DG.Tweening;
using System;
using System.Collections;
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
        private GameObject _emptySpaceFillPrefab;

        private AStar _aStar;

        private MapGenerator _mapGenerator;

        public HallwayGenerator(MapGenerator mapGenerator)
        {
            _mapGenerator = mapGenerator;
        }

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
            PrimsAlg primsAlg, GameObject hallwayPrefab, GameObject hallwayFloorPrefab, GameObject emptySpaceFillPrefab)
        {
            _hallwayPrefab = hallwayPrefab;
            _hallwayFloorPrefab = hallwayFloorPrefab;
            _emptySpaceFillPrefab = emptySpaceFillPrefab;

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

            List<PathNode> pathNodes = _aStar.FindPath(closestStartNode.X, closestStartNode.Y, closestEndNode.X, closestEndNode.Y);
            foreach (PathNode pathNode in pathNodes)
            {
                pathNode.NodeType = NodeType.Hallway;
            }
            _paths.Add(pathNodes);
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

                for (int i = 0; i < path.Count; i++)
                {
                    PathNode pathNode = path[i];
                    InstantiateHallwayWalls(pathNode);

                    foreach (Room room in placedRooms)
                    {
                        Vector2 roomPosition = room.transform.position;
                        Vector2 roomSize = room.transform.localScale / 2;

                        bool isAdjacent = Mathf.Abs(pathNode.X - roomPosition.x) <= roomSize.x + 1 &&
                                          Mathf.Abs(pathNode.Y - roomPosition.y) <= roomSize.y + 1 &&
                                          (Mathf.Abs(pathNode.X - roomPosition.x) > roomSize.x || Mathf.Abs(pathNode.Y - roomPosition.y) > roomSize.y);

                        bool previousNodeInRoom = false;
                        bool nextNodeInRoom = false;

                        if (i > 0)
                        {
                            PathNode prevNode = path[i - 1];
                            previousNodeInRoom = Mathf.Abs(prevNode.X - roomPosition.x) <= roomSize.x &&
                                                 Mathf.Abs(prevNode.Y - roomPosition.y) <= roomSize.y;
                        }

                        if (i < path.Count - 1)
                        {
                            PathNode nextNode = path[i + 1];
                            nextNodeInRoom = Mathf.Abs(nextNode.X - roomPosition.x) <= roomSize.x &&
                                             Mathf.Abs(nextNode.Y - roomPosition.y) <= roomSize.y;
                        }

                        if (isAdjacent && (previousNodeInRoom || nextNodeInRoom))
                        {
                            _edgeNodes.Add(pathNode);
                        }
                    }
                }
            }

            foreach (Room room in placedRooms)
            {
                Vector2 boxSize = new Vector2(room.transform.localScale.x, room.transform.localScale.y) / 1.1f;

                foreach (Collider2D collider in Physics2D.OverlapBoxAll(room.transform.position, boxSize, 360))
                {
                    if (collider.gameObject.CompareTag(GlobalConstants.Tags.Hallway.ToString()) ||
                        collider.gameObject.CompareTag(GlobalConstants.Tags.Shadowcaster.ToString()))
                    {
                        UnityEngine.Object.Destroy(collider.gameObject);
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
                    InstantiateHallway(_hallwayPrefab, pathNode, new Vector3(0.5f, 0), new Vector2(0.05f, 1f));
                    InstantiateShadowCaster(_emptySpaceFillPrefab, pathNode, new Vector2(0.5f, 0), new Vector2(0.005f, 1f));
                }
                else if (leftNode == null && rightNode != null)
                {
                    InstantiateHallway(_hallwayPrefab, pathNode, new Vector3(-0.5f, 0), new Vector2(0.05f, 1.05f));
                    InstantiateShadowCaster(_emptySpaceFillPrefab, pathNode, new Vector2(-0.5f, 0), new Vector2(0.005f, 1f));
                }

                InstantiateHallway(_hallwayPrefab, pathNode, new Vector3(0, -0.5f), new Vector2(1.05f, 0.05f));
                InstantiateShadowCaster(_emptySpaceFillPrefab, pathNode, new Vector2(0, -0.5f), new Vector2(1f, 0.005f));
                InstantiateHallway(_hallwayPrefab, pathNode, new Vector3(0, 0.5f), new Vector2(1.05f, 0.05f));
                InstantiateShadowCaster(_emptySpaceFillPrefab, pathNode, new Vector2(0, 0.5f), new Vector2(1f, 0.005f));
            }
            else
            {
                if (topNode == null)
                {
                    InstantiateHallway(_hallwayPrefab, pathNode, new Vector3(0, 0.5f), new Vector2(1.05f, 0.05f));
                    InstantiateShadowCaster(_emptySpaceFillPrefab, pathNode, new Vector2(0, 0.5f), new Vector2(1f, 0.005f));
                }
                if (bottomNode == null)
                {
                    InstantiateHallway(_hallwayPrefab, pathNode, new Vector3(0, -0.5f), new Vector2(1.05f, 0.05f));
                    InstantiateShadowCaster(_emptySpaceFillPrefab, pathNode, new Vector2(0, -0.5f), new Vector2(1f, 0.005f));
                }

                InstantiateHallwayChunk(pathNode, leftNode, rightNode);
            }

            InstantiateHallway(_hallwayFloorPrefab, pathNode, new Vector3(0, 0f, 0.05f), new Vector2(1, 1), true);
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
                InstantiateHallway(_hallwayPrefab, currentNode, new Vector3(-0.5f, 0), new Vector2(0.05f, 1.05f));
                InstantiateShadowCaster(_emptySpaceFillPrefab, currentNode, new Vector2(-0.5f, 0), new Vector2(0.005f, 1f));
            }
            else if (firstNode != null && secondNode == null)
            {
                InstantiateHallway(_hallwayPrefab, currentNode, new Vector3(0.5f, 0), new Vector2(0.05f, 1.05f));
                InstantiateShadowCaster(_emptySpaceFillPrefab, currentNode, new Vector2(0.5f, 0), new Vector2(0.005f, 1f));
            }
            else if (firstNode == null && secondNode == null)
            {
                InstantiateHallway(_hallwayPrefab, currentNode, new Vector3(0.5f, 0), new Vector2(0.05f, 1.05f));
                InstantiateShadowCaster(_emptySpaceFillPrefab, currentNode, new Vector2(0.5f, 0), new Vector2(0.005f, 1f));
                InstantiateHallway(_hallwayPrefab, currentNode, new Vector3(-0.5f, 0), new Vector2(0.05f, 1.05f));
                InstantiateShadowCaster(_emptySpaceFillPrefab, currentNode, new Vector2(0.5f, 0), new Vector2(0.005f, 1f));
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
                if (hit.collider != null && (hit.collider.gameObject.CompareTag(GlobalConstants.Tags.Room.ToString()) || 
                    hit.collider.gameObject.CompareTag(GlobalConstants.Tags.Shadowcaster.ToString())))
                {
                    PathNode node = _aStar.Grid.GetGridObject(position + direction);
                    if (node.NodeType != NodeType.None)
                    {
                        node.RemoveWall(GetOppositeDirection(direction));
                        UnityEngine.Object.Destroy(hit.collider.gameObject);
                    }
                }
            }
        }

        private WallDirection GetOppositeDirection(Vector2 direction)
        {
            if (direction == Vector2.up) return WallDirection.South;
            if (direction == Vector2.down) return WallDirection.North;
            if (direction == Vector2.left) return WallDirection.East;
            if (direction == Vector2.right) return WallDirection.West;
            throw new ArgumentException("Invalid direction: " + direction);
        }

        private WallDirection GetDirection(Vector2 direction)
        {
            if (direction == Vector2.up) return WallDirection.North;
            if (direction == Vector2.down) return WallDirection.South;
            if (direction == Vector2.left) return WallDirection.West;
            if (direction == Vector2.right) return WallDirection.East;
            throw new ArgumentException("Invalid direction: " + direction);
        }

        /// <summary>
        /// Instantiates a hallway wall.
        /// </summary>
        /// <param name="prefab">Prefab for the hallway wall.</param>
        /// <param name="pathNode">Current path node.</param>
        /// <param name="hallwayOffset">Offset for the hallway wall.</param>
        /// <param name="hallwayScale">Scale for the hallway wall.</param>
        private void InstantiateHallway(GameObject prefab, PathNode pathNode, Vector3 hallwayOffset, Vector2 hallwayScale, bool floor = false)
        {
            Vector3 wallPosition = new Vector3(pathNode.X, pathNode.Y) + hallwayOffset;
            UnityEngine.Object.Instantiate(prefab, wallPosition, Quaternion.identity, floor ? _mapGenerator.HallwayFloorLayoutSpawnTransform : _mapGenerator.HallwayLayoutSpawnTransform).transform.localScale = hallwayScale;
        }

        private void InstantiateShadowCaster(GameObject prefab, PathNode pathNode, Vector3 hallwayOffset, Vector3 hallwayScale)
        {
            Vector3 wallPosition = new Vector3(pathNode.X, pathNode.Y) + hallwayOffset;
            UnityEngine.Object.Instantiate(prefab, wallPosition, Quaternion.identity, _mapGenerator.FillSpawnTransform).transform.localScale = hallwayScale;
        }
    }
}
