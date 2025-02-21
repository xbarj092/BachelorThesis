using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public class MapGenerator : MonoBehaviour
    {
        [Header("Map Parts")]
        [SerializeField] private Room _roomPrefab;
        [SerializeField] private GameObject _floorPrefab;
        [Space]
        [SerializeField] private GameObject _hallwayPrefab;
        [SerializeField] private GameObject _hallwayFloorPrefab;
        [Space]
        [SerializeField] private GameObject _emptySpaceFillPrefab;

        [Header("Map Parameters")]
        [SerializeField] private int _dungeonSizeX;
        [SerializeField] private int _dungeonSizeY;
        [Space]
        [SerializeField] private int _numberOfRooms;

        [Header("Spawn Transforms")]
        public Transform RoomFloorLayoutSpawnTransform;
        public Transform RoomLayoutSpawnTransform;
        public Transform HallwayFloorLayoutSpawnTransform;
        public Transform HallwayLayoutSpawnTransform;
        public Transform FillSpawnTransform;
        public Transform ItemSpawnTransform;
        public Transform KittenSpawnTransform;
        public Transform FoodSpawnTransform;

        private AStar _aStar;
        public AStar AStar => _aStar;
        private PrimsAlg _primsAlg;
        private RoomGenerator _roomGenerator;
        private HallwayGenerator _hallwayGenerator;
        private BowyerWatson _bowyerWatson;

        public bool LoadedData = false;

        private void Awake()
        {
            _primsAlg = new PrimsAlg();
            _bowyerWatson = new BowyerWatson();
            _aStar = new AStar();
            _roomGenerator = new RoomGenerator(this);
            _hallwayGenerator = new HallwayGenerator(this);
        }

        private void OnEnable()
        {
            DataEvents.OnDataSaved += SaveData;
        }

        private void OnDisable()
        {
            DataEvents.OnDataSaved -= SaveData;
        }

        private void SaveData()
        {
            SaveFoodLayout();
            SaveItemLayout();
        }

        private void SaveFoodLayout()
        {
            List<TransformData> foodTransforms = GatherChildTransforms(FoodSpawnTransform);
            LocalDataStorage.Instance.GameData.FoodData = new FoodData(foodTransforms);
        }

        private void SaveItemLayout()
        {
            ItemManager.Instance.SaveItems();
        }

        private List<TransformData> GatherChildTransforms(Transform parentTransform)
        {
            List<TransformData> transformDataList = new List<TransformData>();

            foreach (Transform child in parentTransform)
            {
                transformDataList.Add(new TransformData(child));
            }

            return transformDataList;
        }

        public IEnumerator GenerateMap()
        {
            if (!LoadedData)
            {
                yield return SpawnMap();
            }
            else
            {
                yield return SpawnMap();

                GameData gameData = LocalDataStorage.Instance.GameData;
                LoadItems();
                LoadKittens();
            }
        }

        private IEnumerator SpawnMap()
        {
            _roomGenerator.GenerateRooms(_dungeonSizeX, _dungeonSizeY, _numberOfRooms, _aStar, _roomPrefab, _floorPrefab);
            _hallwayGenerator.GenerateHallways(_bowyerWatson.GenerateTriangularMesh(_roomGenerator.PlacedRooms),
                _roomGenerator.PlacedRooms, _aStar, _primsAlg, _hallwayPrefab, _hallwayFloorPrefab);
            _roomGenerator.BuildRooms(_aStar);
            FillEmptySpace();
            yield return StartCoroutine(WaitForHallways());
        }

        private void FillEmptySpace()
        {
            for (int i = 0; i < _aStar.Grid.GetWidth(); i++)
            {
                for (int j = 0; j < _aStar.Grid.GetHeight(); j++)
                {
                    if (_aStar.Grid.GetGridObject(i, j).NodeType == NodeType.None)
                    {
                        Instantiate(_emptySpaceFillPrefab, FillSpawnTransform).transform.position = new Vector3(i, j, 0);
                    }
                }
            }
        }

        private void LoadItems()
        {
            ItemManager.Instance.LoadItems();

            foreach (UseableItem item in ItemManager.Instance.SpawnedItems)
            {
                item.transform.SetParent(ItemSpawnTransform);
            }

            foreach (ConsumableItem item in ItemManager.Instance.SpawnedConsumables)
            {
                item.transform.SetParent(FoodSpawnTransform);
            }
        }

        private void LoadKittens()
        {
            KittenManager.Instance.LoadKittens();

            foreach (Kitten kitten in KittenManager.Instance.Kittens)
            {
                kitten.transform.SetParent(KittenSpawnTransform);
            }
        }

        private IEnumerator WaitForHallways()
        {
            yield return new WaitForSeconds(0.5f);
            _hallwayGenerator.MakeRoomEntrances();
        }
    }
}
