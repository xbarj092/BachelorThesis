using System.Collections;
using UnityEngine;

namespace MapGenerator
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private Room _roomPrefab;
        [SerializeField] private GameObject _floorPrefab;

        [SerializeField] private GameObject _hallwayPrefab;
        [SerializeField] private GameObject _hallwayFloorPrefab;

        public Transform LayoutSpawnTransform;
        public Transform ItemSpawnTransform;

        private AStar _aStar;
        public AStar AStar => _aStar;
        private PrimsAlg _primsAlg;
        private RoomGenerator _roomGenerator;
        private HallwayGenerator _hallwayGenerator;
        private BowyerWatson _bowyerWatson;

        private const int DUNGEON_SIZE_X = 50;
        private const int DUNGEON_SIZE_Y = 50;

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
            // TODO - generate 2 rooms at the start - start room and boss room
            // put the player in the start room
            _roomGenerator.GenerateRooms(DUNGEON_SIZE_X, DUNGEON_SIZE_Y, 50, _aStar, _roomPrefab, _floorPrefab);
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
