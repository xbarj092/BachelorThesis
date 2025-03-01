using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    /// <summary>
    /// Generates rooms for the dungeon.
    /// </summary>
    internal class RoomGenerator
    {
        private List<Room> _placedRooms = new List<Room>();
        private MapGenerator _mapGenerator;
        private GameObject _emptySpaceFillPrefab;

        public RoomGenerator(MapGenerator mapGenerator, GameObject emptySpaceFillPrefab)
        {
            _mapGenerator = mapGenerator;
            _emptySpaceFillPrefab = emptySpaceFillPrefab;
        }

        /// <summary>
        /// Gets the list of rooms placed in the dungeon.
        /// </summary>
        internal List<Room> PlacedRooms
        {
            get { return _placedRooms; }
        }

        /// <summary>
        /// Generates rooms within the dungeon area.
        /// </summary>
        /// <param name="dungeonSizeX">The width of the dungeon area.</param>
        /// <param name="dungeonSizeY">The height of the dungeon area.</param>
        /// <param name="amountOfRooms">The number of rooms to generate.</param>
        /// <param name="aStar">The AStar instance used for pathfinding.</param>
        internal void GenerateRooms(int dungeonSizeX, int dungeonSizeY, int amountOfRooms, AStar aStar, Room roomPrefab, GameObject roomFloorPrefab)
        {
            GenerateBaseRooms(aStar, roomPrefab, roomFloorPrefab);
            for (int i = 0; i < amountOfRooms; i++)
            {
                Room newRoom = roomPrefab;
                newRoom.transform.localScale = GetRoomScale();
                int positionX, positionY, numberOfTries = 0;
                Vector2 transform = Vector2.zero;
                do
                {
                    numberOfTries++;
                    if (numberOfTries > 100)
                    {
                        break;
                    }

                    positionX = LocalDataStorage.Instance.GameData.Random.Next(5, dungeonSizeX + 5);
                    positionY = LocalDataStorage.Instance.GameData.Random.Next(5, dungeonSizeY + 5);
                    transform = new Vector2(positionX, positionY);
                } while (Physics2D.OverlapBox(transform, newRoom.transform.localScale, 0f));

                PlaceRoom(numberOfTries, transform, newRoom, aStar, roomFloorPrefab);
            }
        }

        private void GenerateBaseRooms(AStar aStar, Room roomPrefab, GameObject roomFloorPrefab)
        {
            int positionX = LocalDataStorage.Instance.GameData.Random.Next(5, 40);
            int positionY = LocalDataStorage.Instance.GameData.Random.Next(5, 40);

            GameManager.Instance.StartRoomLocation = new Vector2(positionX, positionY);

            GenerateRoom(aStar, roomPrefab, roomFloorPrefab, positionX, positionY, RoomType.Start);
            GenerateRoom(aStar, roomPrefab, roomFloorPrefab, 50, 50, RoomType.End);
        }

        private void GenerateRoom(AStar aStar, Room roomPrefab, GameObject roomFloorPrefab, int x, int y, RoomType roomType)
        {
            Room newRoom = roomPrefab;
            newRoom.transform.localScale = GetRoomScale();
            Vector2 transform = new(x, y);
            PlaceRoom(0, transform, newRoom, aStar, roomFloorPrefab, roomType);
        }

        /// <summary>
        /// Gets the scale of a new room.
        /// </summary>
        /// <returns>The scale of the new room.</returns>
        private Vector2 GetRoomScale()
        {
            int randomEvenX = LocalDataStorage.Instance.GameData.Random.Next(1, 4) * 2;
            int randomEvenY = LocalDataStorage.Instance.GameData.Random.Next(1, 4) * 2;
            return new Vector2(randomEvenX + 1, randomEvenY + 1);
        }

        /// <summary>
        /// Places the room within the dungeon area if the number of placement attempts does not exceed the limit.
        /// </summary>
        /// <param name="numberOfTries">The number of attempts made to place the room.</param>
        /// <param name="transform">The position to place the room.</param>
        /// <param name="newRoom">The room to place.</param>
        /// <param name="aStar">The AStar instance used for pathfinding.</param>
        private void PlaceRoom(int numberOfTries, Vector2 transform, Room newRoom, AStar aStar, GameObject roomFloorPrefab, RoomType roomType = RoomType.Normal)
        {
            if (numberOfTries <= 100)
            {
                Room instantiatedRoom = Object.Instantiate(newRoom, transform, Quaternion.identity, _mapGenerator.RoomLayoutSpawnTransform);
                instantiatedRoom.RoomType = roomType;
                aStar.GetGrid().GetXY(transform, out int x, out int y);
                instantiatedRoom.Init(x, y, _mapGenerator);
                PlacedRooms.Add(instantiatedRoom);
                roomFloorPrefab.transform.localScale = new Vector2(newRoom.transform.localScale.x, newRoom.transform.localScale.y);
                GameObject roomObject = Object.Instantiate(roomFloorPrefab, new Vector2(transform.x, transform.y), Quaternion.identity, _mapGenerator.RoomFloorLayoutSpawnTransform);
                SetRoomNodes(roomObject, aStar);
            }
        }

        /// <summary>
        /// Sets the nodes of the A* grid to represent the placement of the room.
        /// </summary>
        /// <param name="newRoom">The newly placed room.</param>
        /// <param name="aStar">The AStar instance used for pathfinding.</param>
        private void SetRoomNodes(GameObject newRoom, AStar aStar)
        {
            Vector2 roomPosition = newRoom.transform.localPosition;
            Vector2 roomScale = newRoom.transform.localScale;

            int minX = Mathf.FloorToInt(roomPosition.x - roomScale.x / 2.1f);
            int maxX = Mathf.CeilToInt(roomPosition.x + roomScale.x / 2.1f);
            int minY = Mathf.FloorToInt(roomPosition.y - roomScale.y / 2.1f);
            int maxY = Mathf.CeilToInt(roomPosition.y + roomScale.y / 2.1f);

            for (int x = minX + 1; x < maxX; x++)
            {
                for (int y = minY + 1; y < maxY; y++)
                {
                    PathNode pathNode = aStar.GetGrid().GetGridObject(x, y);

                    if (pathNode != null)
                    {
                        pathNode.NodeType = NodeType.Room;
                        aStar.GetGrid().SetGridObject(x, y, pathNode);
                    }
                }
            }
        }

        /// <summary>
        /// Builds the walls of each room within the dungeon area.
        /// </summary>
        /// <param name="aStar">The AStar instance used for pathfinding.</param>
        internal void BuildRooms(AStar aStar)
        {
            foreach (Room room in _placedRooms)
            {
                Vector2 roomPosition = room.transform.position;
                Vector2 roomScale = room.transform.localScale;

                aStar.GetGrid().GetXY(roomPosition, out int roomX, out int roomY);

                int startXMin = Mathf.FloorToInt(roomX - (roomScale.x - 1) / 2);
                int startXMax = Mathf.CeilToInt(roomX + (roomScale.x - 1) / 2);
                int startYMin = Mathf.FloorToInt(roomY - (roomScale.y - 1) / 2);
                int startYMax = Mathf.CeilToInt(roomY + (roomScale.y - 1) / 2);

                BuildRoomWalls(aStar, room, startXMin, startXMax, startYMin, startYMax);

                Object.Destroy(room.gameObject);
            }
        }

        /// <summary>
        /// Builds the walls surrounding each room.
        /// </summary>
        private void BuildRoomWalls(AStar aStar, Room room, int startXMin, int startXMax, int startYMin, int startYMax)
        {
            for (int x = startXMin; x <= startXMax; x++)
            {
                for (int y = startYMin; y <= startYMax; y++)
                {
                    if (x == startXMin || x == startXMax || y == startYMin || y == startYMax)
                    {
                        BuildOuterWalls(aStar, room, x, y, startXMin, startXMax, startYMin, startYMax);
                    }
                }
            }
        }

        /// <summary>
        /// Builds the outer walls around a room based on its position in the dungeon area.
        /// </summary>
        private void BuildOuterWalls(AStar aStar, Room room, int x, int y, int startXMin, int startXMax, int startYMin, int startYMax)
        {
            PathNode roomNode = aStar.GetGrid().GetGridObject(x, y);
            if (roomNode != null && IsOuterNode(x, y, startXMin, startXMax, startYMin, startYMax))
            {
                Vector2 tilePosition = new(x, y);
                if (x == startXMin)
                {
                    roomNode.AddWall(WallDirection.West);
                }
                if (x == startXMax)
                {
                    roomNode.AddWall(WallDirection.East);
                }
                if (y == startYMin)
                {
                    roomNode.AddWall(WallDirection.South);
                }
                if (y == startYMax)
                {
                    roomNode.AddWall(WallDirection.North);
                }

                if (x == startXMin || x == startXMax || y == startYMin || y == startYMax)
                {
                    if (x == startXMin && y == startYMin) // Bottom-left corner
                    {
                        BuildCornerWalls(room, tilePosition, new Vector2(-0.5f, 0), new Vector2(0, -0.5f));
                    }
                    else if (x == startXMax && y == startYMin) // Bottom-right corner
                    {
                        BuildCornerWalls(room, tilePosition, new Vector2(0.5f, 0), new Vector2(0, -0.5f));
                    }
                    else if (x == startXMin && y == startYMax) // Top-left corner
                    {
                        BuildCornerWalls(room, tilePosition, new Vector2(-0.5f, 0), new Vector2(0, 0.5f));
                    }
                    else if (x == startXMax && y == startYMax) // Top-right corner
                    {
                        BuildCornerWalls(room, tilePosition, new Vector2(0.5f, 0), new Vector2(0, 0.5f));
                    }
                    else if (x == startXMin || x == startXMax) // Left or Right side
                    {
                        BuildSideWall(room, tilePosition, (x == startXMin) ? new Vector2(-0.5f, 0) : new Vector2(0.5f, 0), new Vector2(0.05f, 1.05f));
                        InstantiateShadowCaster(_emptySpaceFillPrefab, tilePosition, (x == startXMin) ? new Vector2(-0.5f, 0) : new Vector2(0.5f, 0), new Vector2(0.005f, 1f));
                    }
                    else if (y == startYMin || y == startYMax) // Bottom or Top side
                    {
                        BuildSideWall(room, tilePosition, (y == startYMin) ? new Vector2(0, -0.5f) : new Vector2(0, 0.5f), new Vector2(1.05f, 0.05f));
                        InstantiateShadowCaster(_emptySpaceFillPrefab, tilePosition, (y == startYMin) ? new Vector2(0, -0.5f) : new Vector2(0, 0.5f), new Vector2(1f, 0.005f));
                    }
                }
            }
        }

        /// <summary>
        /// Builds walls at the corner of a room based on the given position and offsets.
        /// </summary>
        /// <param name="room">The room to build corner walls for.</param>
        /// <param name="position">The position where the corner walls will be built.</param>
        /// <param name="offset1">The first offset for the corner walls.</param>
        /// <param name="offset2">The second offset for the corner walls.</param>
        private void BuildCornerWalls(Room room, Vector2 position, Vector2 offset1, Vector2 offset2)
        {
            BuildSideWall(room, position, offset1, new Vector2(0.05f, 1.05f));
            InstantiateShadowCaster(_emptySpaceFillPrefab, position, offset1, new Vector2(0.005f, 1f));
            BuildSideWall(room, position, offset2, new Vector2(1.05f, 0.05f));
            InstantiateShadowCaster(_emptySpaceFillPrefab, position, offset2, new Vector2(1f, 0.005f));
        }

        /// <summary>
        /// Determines if the given tile is an outer node of the room.
        /// </summary>
        /// <param name="x">The X coordinate of the tile.</param>
        /// <param name="y">The Y coordinate of the tile.</param>
        /// <param name="startXMin">The minimum X coordinate of the room.</param>
        /// <param name="startXMax">The maximum X coordinate of the room.</param>
        /// <param name="startYMin">The minimum Y coordinate of the room.</param>
        /// <param name="startYMax">The maximum Y coordinate of the room.</param>
        /// <returns>True if the tile is an outer node of the room, otherwise false.</returns>
        private bool IsOuterNode(int x, int y, int startXMin, int startXMax, int startYMin, int startYMax)
        {
            return (x == startXMin || x == startXMax || y == startYMin || y == startYMax);
        }

        /// <summary>
        /// Builds a wall for the specified room at the given tile position with the provided wall offset and scale.
        /// </summary>
        /// <param name="room">The room to build the wall for.</param>
        /// <param name="tilePosition">The position of the tile where the wall will be built.</param>
        /// <param name="wallOffset">The offset from the tile position to place the wall.</param>
        /// <param name="wallScale">The scale of the wall.</param>
        private void BuildSideWall(Room room, Vector2 tilePosition, Vector2 wallOffset, Vector2 wallScale)
        {
            Vector2 wallPosition = tilePosition + wallOffset;
            Object.Instantiate(room, wallPosition, Quaternion.identity, _mapGenerator.RoomLayoutSpawnTransform).transform.localScale = wallScale;
        }

        private void InstantiateShadowCaster(GameObject prefab, Vector2 tilePosition, Vector2 wallOffset, Vector2 wallScale)
        {
            Vector2 wallPosition = tilePosition + wallOffset;
            Object.Instantiate(prefab, wallPosition, Quaternion.identity, _mapGenerator.FillSpawnTransform).transform.localScale = wallScale;
        }
    }
}
