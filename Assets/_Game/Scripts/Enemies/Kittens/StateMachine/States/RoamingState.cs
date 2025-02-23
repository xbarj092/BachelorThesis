using MapGenerator;
using System.Collections.Generic;
using UnityEngine;

public class RoamingState : BaseState
{
    [SerializeField] private float _moveSpeed = 0.5f;
    [SerializeField] private float _boostedSpeedMultiplier = 5f;

    private List<PathNode> _path;
    private int _currentPathIndex;
    private bool _isPathSet;

    private Camera _mainCamera;
    private int _frameCounter;
    private int _frameCheckInterval = 5;

    private static readonly List<Vector2Int> _neighborOffsets = new()
    {
        new(-1, -1), new(1, -1), new(-1, 1), new(1, 1),
        new(1, 0), new(-1, 0), new(0, 1), new(0, -1)
    };

    public override void OnStateEnter()
    {
        Debug.Log("[RoamingState] - entered roaming state");
        _mainCamera = Camera.main;
        _frameCounter = Random.Range(0, 5);
        _brain.AStar.GetGrid().GetXY(_kitten.transform.localPosition, out int kittenX, out int kittenY);

        PathNode currentNode = _brain.AStar.Grid.GetGridObject(kittenX, kittenY);

        if (currentNode == null || !IsValidNode(currentNode))
        {
            FindClosestValidNode(ref kittenX, ref kittenY);
        }

        PathNode nextNode = KittenManager.Instance.GetNextPosition(_kitten.transform.localPosition);
        if (nextNode != null)
        {
            _path = _brain.AStar.FindPath(kittenX, kittenY, nextNode.X, nextNode.Y);
            if (_path != null && _path.Count > 0)
            {
                UpdateClosestPathIndex();
            }
        }
    }

    private bool IsKittenOutOfView()
    {
        if (_mainCamera == null)
        {
            return false;
        }

        Vector3 kittenViewportPos = _mainCamera.WorldToViewportPoint(_kitten.transform.position);
        return kittenViewportPos.x < -0.2f || kittenViewportPos.x > 1.2f || kittenViewportPos.y < -0.2f || kittenViewportPos.y > 1.2f;
    }

    public override BaseState ExecuteState()
    {
        if (_kitten.IsDead && _brain.GetState(StateType.Death, out BaseState deathState))
        {
            return deathState;
        }

        if (_kitten.IsTrapped && _brain.GetState(StateType.Trapped, out BaseState trappedState))
        {
            return trappedState;
        }

        if (_kitten.IsApproaching && _kitten.CanMate() && _brain.GetState(StateType.Approach, out BaseState approachState))
        {
            return approachState;
        }

        if ((_kitten.IsInRangeOfPlayer || _kitten.CanSeeTarget) && _brain.GetState(StateType.Focused, out BaseState focusedState))
        {
            return focusedState;
        }

        _frameCounter++;
        int checkInterval = IsKittenOutOfView() ? _frameCheckInterval : _frameCheckInterval / 5;

        if (_isPathSet && _frameCounter % checkInterval == 0)
        {
            FollowPath();

            if (HasReachedTarget())
            {
                if (_brain.GetState(StateType.Idle, out BaseState idleState))
                {
                    return idleState;
                }
            }
        }

        return null;
    }

    public override void OnStateExit()
    {
        Debug.Log("[RoamingState] - exited roaming state");
        _isPathSet = false;
    }

    private void FindClosestValidNode(ref int x, ref int y)
    {
        PathNode closestNode = null;
        float closestDistance = float.MaxValue;

        foreach (Vector2Int offset in _neighborOffsets)
        {
            PathNode neighborNode = _brain.AStar.Grid.GetGridObject(x + offset.x, y + offset.y);

            if (IsValidNode(neighborNode))
            {
                float distance = Vector3.Distance(
                    _kitten.transform.localPosition,
                    _brain.AStar.Grid.GetWorldPosition(neighborNode.X, neighborNode.Y));

                if (distance < closestDistance)
                {
                    closestNode = neighborNode;
                    closestDistance = distance;
                }
            }
        }

        if (closestNode != null)
        {
            x = closestNode.X;
            y = closestNode.Y;
        }
    }

    private bool IsValidNode(PathNode node)
    {
        return node != null && node.NodeType != NodeType.None && _brain.AStar.IsNodeWalkable(node);
    }

    private PathNode GetRandomWalkableNode()
    {
        List<PathNode> potentialNodes = _brain.AStar.GetAllWalkableNodes();
        if (potentialNodes.Count == 0) return null;

        int index = Random.Range(0, potentialNodes.Count);
        return potentialNodes[index];
    }

    private void FollowPath()
    {
        if (_currentPathIndex >= _path.Count)
        {
            _isPathSet = false;
            return;
        }

        Vector3 targetPosition = _brain.AStar.GetGrid().GetWorldPosition(_path[_currentPathIndex].X, _path[_currentPathIndex].Y);
        Vector3 direction = (targetPosition - _kitten.transform.position).normalized;
        float currentSpeed = IsKittenOutOfView() ? _moveSpeed * _boostedSpeedMultiplier : _moveSpeed;
        _kitten.transform.position += (currentSpeed * Time.deltaTime * direction);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        _kitten.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Vector3.Distance(_kitten.transform.position, targetPosition) <= 0.1f)
        {
            _currentPathIndex++;
        }
    }

    private void UpdateClosestPathIndex()
    {
        _currentPathIndex = 0;
        _isPathSet = true;

        int maxNodesToCheck = Mathf.Min(3, _path.Count);
        float closestDistance = float.MaxValue;

        for (int i = 0; i < maxNodesToCheck; i++)
        {
            PathNode pathNode = _path[i];
            Vector3 nodeWorldPosition = _brain.AStar.Grid.GetWorldPosition(pathNode.X, pathNode.Y);
            float distanceToPlayer = Vector3.Distance(_kitten.transform.localPosition, nodeWorldPosition);

            if (distanceToPlayer < closestDistance)
            {
                closestDistance = distanceToPlayer;
                _currentPathIndex = i;
            }
        }
    }

    private bool HasReachedTarget()
    {
        return _currentPathIndex >= _path.Count;
    }
}
