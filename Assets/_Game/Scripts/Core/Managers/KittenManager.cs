using MapGenerator;
using System.Collections.Generic;
using UnityEngine;

public class KittenManager : MonoSingleton<KittenManager>
{
    [SerializeField] private Kitten _kittenPrefab;

    public List<Kitten> Kittens = new();
    public Transform SpawnTransform;
    public AStar AStar;

    private Transform _playerTransform;

    private const float GenderBalanceThreshold = 0.15f;
    private const int MinimumKittensToSpawn = 3;
    private const float CheckInterval = 5f;

    private void Awake()
    {
        _playerTransform = FindFirstObjectByType<Player>()?.transform;
    }

    private void Start()
    {
        InvokeRepeating(nameof(CheckGenderBalance), CheckInterval, CheckInterval);
    }

    public void CreateKitten(Vector3 position, bool? male = null)
    {
        Kitten kitten = Instantiate(_kittenPrefab, position, Quaternion.identity, SpawnTransform);
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

    private bool DetermineKittenGender()
    {
        return Random.Range(0, 2) == 0;
    }

    private void CheckGenderBalance()
    {
        int maleCount = Kittens.FindAll(k => k.Male).Count;
        int femaleCount = Kittens.Count - maleCount;

        if (Kittens.Count == 0)
        {
            return;
        }

        float maleRatio = (float)maleCount / Kittens.Count;
        float femaleRatio = (float)femaleCount / Kittens.Count;

        if (maleRatio < GenderBalanceThreshold)
        {
            SpawnAdditionalKittens(false, MinimumKittensToSpawn);
        }
        else if (femaleRatio < GenderBalanceThreshold)
        {
            SpawnAdditionalKittens(true, MinimumKittensToSpawn);
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
}
