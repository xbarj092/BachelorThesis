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

            if (RoomType == RoomType.Normal)
            {
                SpawnKittens();
            }
        }

        private void SpawnKittens()
        {
            int kittensSpawned = 0;
            int randomNumber = Random.Range(0, 6);
            if (randomNumber == 5)
            {
                kittensSpawned = 2;
            }
            else if (randomNumber > 3)
            {
                kittensSpawned = 1;
            }

            for (int i = 0; i < kittensSpawned; i++)
            {
                
                KittenManager.Instance.CreateKitten(GetRandomCoords());
            }
        }

        private void SpawnItems()
        {
            foreach (KeyValuePair<ItemType, float> spawnChances in _spawnChance.ItemSpawnChances)
            {
                int randomNumber = Random.Range(0, 6);
                if (randomNumber < spawnChances.Value * 6)
                {
                    Instantiate(_items[spawnChances.Key], GetRandomCoords(), Quaternion.identity, _mapGenerator.ItemSpawnTransform);
                }
            }
        }

        private Vector2 GetRandomCoords()
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

            return new Vector2(spawnX, spawnY);
        }
    }
}
