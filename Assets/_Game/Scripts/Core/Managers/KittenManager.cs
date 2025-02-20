using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KittenManager : MonoSingleton<KittenManager>
{
    [SerializeField] private Kitten _kittenPrefab;

    public List<Kitten> Kittens = new();
    public Transform SpawnTransform;
    public AStar AStar;

    private List<int> _kittenIDs = new();

    private Transform _playerTransform;

    private float _genderBalanceThreshold = 0.15f;
    private int _minimumKittensToSpawn = 3;
    private float _checkInterval = 5f;

    private float[,] _influenceMap;
    private int _coarseWidth, _coarseHeight;
    private int _gridDivisionSize = 10;

    private float _decayRate = 0.5f;
    private float _randomWeight = 0.5f;

    private void Awake()
    {
        _playerTransform = FindFirstObjectByType<Player>()?.transform;
    }

    private void OnEnable()
    {
        DataEvents.OnDataSaved += OnDataSaved;
    }

    private void OnDisable()
    {
        DataEvents.OnDataSaved -= OnDataSaved;
    }

    public void ResetManager()
    {
        StopAllCoroutines();
        CancelInvoke();
        Kittens.Clear();
    }

    private void OnDataSaved()
    {
        LocalDataStorage.Instance.GameData.KittenData.SavedKittens.Clear();

        foreach (Kitten kitten in Kittens)
        {
            SavedKitten savedKitten = kitten.Save();
            LocalDataStorage.Instance.GameData.KittenData.SavedKittens.Add(savedKitten);
        }
    }

    public void LoadKittens()
    {
        Kittens.Clear();

        foreach (SavedKitten savedKitten in LocalDataStorage.Instance.GameData.KittenData.SavedKittens)
        {
            Kitten kitten = Instantiate(_kittenPrefab);
            savedKitten.ApplyToKitten(kitten);
            Kittens.Add(kitten);
        }

        foreach (Kitten kitten in Kittens)
        {
            kitten.PotentialPartner = GetKittenFromUID(kitten.PartnerUID);
        }
    }

    public void Initialize()
    {
        InvokeRepeating(nameof(CheckGenderBalance), 0, _checkInterval);
        StartCoroutine(InitializeCoarseInfluenceMap());
        InvokeRepeating(nameof(DecayInfluenceMap), 0, 1f);
    }

    private IEnumerator InitializeCoarseInfluenceMap()
    {
        yield return new WaitUntil(() => GameManager.Instance.MapInitialized);
        _coarseWidth = Mathf.CeilToInt(AStar.Grid.GetWidth() / (float)_gridDivisionSize);
        _coarseHeight = Mathf.CeilToInt(AStar.Grid.GetHeight() / (float)_gridDivisionSize);
        _influenceMap = new float[_coarseWidth, _coarseHeight];
    }

    private void UpdateInfluenceMap(int x, int y, float delta)
    {
        int coarseX = x / _gridDivisionSize;
        int coarseY = y / _gridDivisionSize;

        if (coarseX >= 0 && coarseX < _coarseWidth && coarseY >= 0 && coarseY < _coarseHeight)
        {
            _influenceMap[coarseX, coarseY] += delta;
        }
    }

    private float GetInfluence(int x, int y)
    {
        int coarseX = x / _gridDivisionSize;
        int coarseY = y / _gridDivisionSize;

        if (coarseX < 0 || coarseX >= _coarseWidth || coarseY < 0 || coarseY >= _coarseHeight)
        {
            return float.MaxValue;
        }

        return _influenceMap[coarseX, coarseY];
    }

    internal PathNode GetNextPosition(Vector2 currentPosition)
    { 
        List<PathNode> walkableNodes = AStar.GetAllWalkableNodes();
        PathNode currentNode = AStar.Grid.GetGridObject(currentPosition);
        if (currentNode == null || walkableNodes.Count == 0)
        {
            return null;
        }

        PathNode bestNode = null;
        float bestScore = float.MaxValue;

        foreach (PathNode node in walkableNodes)
        {
            if (node == currentNode)
            {
                continue;
            }

            float influence = GetInfluence(node.X, node.Y);
            float score = influence + _randomWeight * Random.Range(0f, 1f);

            if (score < bestScore)
            {
                bestScore = score;
                bestNode = node;
            }
        }

        if (bestNode != null)
        {
            Debug.Log("[KittenManager] - bestNode is not null");
            UpdateInfluenceMap(bestNode.X, bestNode.Y, 1f);
        }

        return bestNode;
    }

    private void DecayInfluenceMap()
    {
        if (TutorialManager.Instance.IsPaused)
        {
            return;
        }

        for (int x = 0; x < _coarseWidth; x++)
        {
            for (int y = 0; y < _coarseHeight; y++)
            {
                _influenceMap[x, y] *= _decayRate;
            }
        }
    }

    #region kitten spawning

    public void CreateKitten(Vector3 position, bool? male = null)
    {
        Kitten kitten = Instantiate(_kittenPrefab, position, Quaternion.identity, SpawnTransform);
        kitten.UID = SetKittenUID();
        kitten.Init();
        if (male != null)
        {
            kitten.Male = male.Value;
        }
        else
        {
            kitten.Male = DetermineKittenGender();
        }

        kitten.IsApproaching = false;
        Kittens.Add(kitten);
    }

    private int SetKittenUID()
    {
        int uid = 0;
        do
        {
            uid = Random.Range(0, 100000);
        }
        while (_kittenIDs.Contains(uid));
        return uid;
    }

    public Kitten GetKittenFromUID(int uid)
    {
        return Kittens.FirstOrDefault(kitten => kitten.UID == uid);
    }

    private bool DetermineKittenGender()
    {
        return Random.Range(0, 2) == 0;
    }

    #endregion

    #region gender balancer

    private void CheckGenderBalance()
    {
        if (TutorialManager.Instance.IsPaused)
        {
            return;
        }

        int maleCount = Kittens.FindAll(k => k.Male).Count;
        int femaleCount = Kittens.Count - maleCount;

        if (Kittens.Count == 0)
        {
            return;
        }

        float maleRatio = (float)maleCount / Kittens.Count;
        float femaleRatio = (float)femaleCount / Kittens.Count;

        if (maleRatio < _genderBalanceThreshold)
        {
            SpawnAdditionalKittens(false, _minimumKittensToSpawn);
        }
        else if (femaleRatio < _genderBalanceThreshold)
        {
            SpawnAdditionalKittens(true, _minimumKittensToSpawn);
        }
    }

    private void SpawnAdditionalKittens(bool male, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition = GetRandomSpawnPosition();
            CreateKitten(randomPosition, male);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        List<PathNode> potentialNodes = AStar.GetAllWalkableNodes();
        List<PathNode> filteredNodes = FilterNodes(potentialNodes);
        int nodeIndex = Random.Range(0, potentialNodes.Count);
        PathNode nextNode = potentialNodes[nodeIndex];
        return AStar.Grid.GetWorldPosition(nextNode.X, nextNode.Y);
    }

    private List<PathNode> FilterNodes(List<PathNode> potentialNodes)
    {
        if (_playerTransform == null)
        {
            _playerTransform = FindFirstObjectByType<Player>()?.transform;
        }

        Vector3 playerPosition = _playerTransform.position;
        float exclusionRadius = 15f;

        List<PathNode> filteredNodes = new();

        foreach (PathNode node in potentialNodes)
        {
            Vector3 nodePosition = AStar.Grid.GetWorldPosition(node.X, node.Y);
            float distanceToPlayer = Vector3.Distance(playerPosition, nodePosition);

            if (distanceToPlayer >= exclusionRadius)
            {
                filteredNodes.Add(node);
            }
        }

        return filteredNodes;
    }

    #endregion
}
