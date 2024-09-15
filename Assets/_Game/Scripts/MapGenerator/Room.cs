using UnityEngine;

namespace MapGenerator
{
    public class Room : MonoBehaviour
    {
        public GameObject enemyPrefab;
        public int enemyCount = 2;

        private Vector2Int _gridCoordinates;

        public void Init(int x, int y)
        {
            _gridCoordinates = new Vector2Int(x, y);
            // SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            Vector3 roomScale = transform.localScale;
            Vector3 roomPosition = transform.localPosition;

            float roomWidth = roomScale.x - 1;
            float roomHeight = roomScale.z - 1;

            float minX = roomPosition.x - roomWidth / 2;
            float maxX = roomPosition.x + roomWidth / 2;
            float minZ = roomPosition.z - roomHeight / 2;
            float maxZ = roomPosition.z + roomHeight / 2;

            for (int i = 0; i < enemyCount; i++)
            {
                float spawnX = Random.Range(minX, maxX);
                float spawnY = Random.Range(minZ, maxZ);

                Vector3 spawnPosition = new(spawnX, 0, spawnY);
                Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}
