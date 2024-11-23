using System.Collections;
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

        [Header("Map Parameters")]
        [SerializeField] private int _dungeonSizeX;
        [SerializeField] private int _dungeonSizeY;
        [Space]
        [SerializeField] private int _numberOfRooms;

        [Header("Spawn Transforms")]
        public Transform LayoutSpawnTransform;
        public Transform ItemSpawnTransform;
        public Transform KittenSpawnTransform;

        private AStar _aStar;
        public AStar AStar => _aStar;
        private PrimsAlg _primsAlg;
        private RoomGenerator _roomGenerator;
        private HallwayGenerator _hallwayGenerator;
        private BowyerWatson _bowyerWatson;

        private void Awake()
        {
            _primsAlg = new PrimsAlg();
            _bowyerWatson = new BowyerWatson();
            _aStar = new AStar();
            _roomGenerator = new RoomGenerator(this);
            _hallwayGenerator = new HallwayGenerator(this);
        }

        private void Start()
        {
            _roomGenerator.GenerateRooms(_dungeonSizeX, _dungeonSizeY, _numberOfRooms, _aStar, _roomPrefab, _floorPrefab);
            _hallwayGenerator.GenerateHallways(_bowyerWatson.GenerateTriangularMesh(_roomGenerator.PlacedRooms), 
                _roomGenerator.PlacedRooms, _aStar, _primsAlg, _hallwayPrefab, _hallwayFloorPrefab);
            _roomGenerator.BuildRooms(_aStar);
            StartCoroutine(WaitForHallways());
        }

        private IEnumerator WaitForHallways()
        {
            yield return new WaitForSeconds(0.5f);
            _hallwayGenerator.MakeRoomEntrances();
        }
    }
}
