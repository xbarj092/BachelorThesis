using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private GameObject _food;

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

            if (!_mapGenerator.LoadedData)
            {
                SpawnItems();

                if (RoomType == RoomType.Normal)
                {
                    SpawnKittens();
                    SpawnFood();
                }
            }
        }

        private void SpawnKittens()
        {
            int kittensSpawned = 0;
            int randomNumber = Random.Range(0, 6);
            if (randomNumber > 3)
            {
                kittensSpawned = 2;
            }
            if (randomNumber > 1)
            {
                kittensSpawned = 1;
            }

            for (int i = 0; i < kittensSpawned; i++)
            {
                KittenManager.Instance.CreateKitten(GetRandomCoords());
            }
        }

        private void SpawnFood()
        {
            int foodSpawned = 0;
            int randomNumber = Random.Range(0, 10);
            if (randomNumber > 8)
            {
                foodSpawned = 3;
            }
            else if (randomNumber > 5)
            {
                foodSpawned = 2;
            }
            else if (randomNumber > 2)
            {
                foodSpawned = 1;
            }

            for (int i = 0; i < foodSpawned; i++)
            {
                Instantiate(_food, GetRandomCoords(), Quaternion.identity, _mapGenerator.FoodSpawnTransform);
            }
        }

        private void SpawnItems()
        {
            foreach (KeyValuePair<ItemType, float> spawnChances in _spawnChance.ItemSpawnChances)
            {
                int randomNumber = Random.Range(0, 6);
                if (randomNumber < spawnChances.Value * 6)
                {
                    ItemManager.Instance.SpawnItem(spawnChances.Key, GetRandomCoords(), Quaternion.identity, _mapGenerator.ItemSpawnTransform);
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
