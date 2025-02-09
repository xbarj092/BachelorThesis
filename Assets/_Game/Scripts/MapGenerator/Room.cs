using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public class Room : MonoBehaviour
    {
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
                SpawnConsumables();
            }
        }

        private void SpawnKittens()
        {
            int kittensSpawned = 0;
            int randomNumber;
            if (!_mapGenerator.LoadedData)
            {
                randomNumber = LocalDataStorage.Instance.GameData.Random.Next(0, 6);
            }
            else
            {
                LocalDataStorage.Instance.GameData.Random.Next();
                return;
            }

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

        private void SpawnObjects<T>(Dictionary<T, float> spawnChances, System.Action<T> spawnAction)
        {
            foreach (KeyValuePair<T, float> spawnChance in spawnChances)
            {
                int randomNumber;
                if (!_mapGenerator.LoadedData)
                {
                    randomNumber = LocalDataStorage.Instance.GameData.Random.Next(0, 6);
                }
                else
                {
                    LocalDataStorage.Instance.GameData.Random.Next();
                    continue;
                }

                if (randomNumber < spawnChance.Value * 6)
                {
                    spawnAction.Invoke(spawnChance.Key);
                }
            }
        }

        private void SpawnConsumables()
        {
            SpawnObjects(_spawnChance.ConsumableSpawnChances, consumableType =>
            {
                ItemManager.Instance.SpawnConsumable(consumableType, GetRandomCoords(), Quaternion.identity, _mapGenerator.ItemSpawnTransform);
            });
        }

        private void SpawnItems()
        {
            SpawnObjects(_spawnChance.ItemSpawnChances, itemType =>
            {
                ItemManager.Instance.SpawnItem(itemType, GetRandomCoords(), Quaternion.identity, _mapGenerator.ItemSpawnTransform);
            });
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
