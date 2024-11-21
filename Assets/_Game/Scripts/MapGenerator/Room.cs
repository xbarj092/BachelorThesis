using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<ItemType, Item> _items = new();
        [SerializeField] private Kitten _kitten;

        public RoomType RoomType;

        private Vector2Int _gridCoordinates;

        [SerializeField] private SerializedDictionary<RoomType, ItemSpawnChance> _spawnChances = new();
        private ItemSpawnChance _spawnChance;

        private MapGenerator _mapGenerator;

        public void Init(int x, int y, MapGenerator mapGenerator)
        {
            _mapGenerator = mapGenerator;
            _gridCoordinates = new Vector2Int(x, y);
            _spawnChance = _spawnChances[RoomType];
            SpawnItems();
        }

        private void SpawnItems()
        {
            // Instantiate(_kitten, transform.position, Quaternion.identity);
            foreach (KeyValuePair<ItemType, float> spawnChances in _spawnChance.ItemSpawnChances)
            {
                int randomNumber = Random.Range(0, 6);
                if (randomNumber < spawnChances.Value * 6)
                {
                    Vector3 roomScale = transform.localScale;
                    Vector3 roomPosition = transform.position;

                    float roomWidth = roomScale.x - 1;
                    float roomHeight = roomScale.y - 1;

                    float minX = roomPosition.x - roomWidth / 2;
                    float maxX = roomPosition.x + roomWidth / 2;
                    float minZ = roomPosition.y - roomHeight / 2;
                    float maxZ = roomPosition.y + roomHeight / 2;

                    float spawnX = Random.Range(minX, maxX);
                    float spawnY = Random.Range(minZ, maxZ);

                    Vector2 spawnPosition = new(spawnX, spawnY);
                    Instantiate(_items[spawnChances.Key], spawnPosition, Quaternion.identity, _mapGenerator.ItemSpawnTransform);
                }
            }
        }
    }
}
