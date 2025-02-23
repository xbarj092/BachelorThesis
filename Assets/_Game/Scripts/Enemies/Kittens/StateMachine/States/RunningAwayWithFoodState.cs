using MapGenerator;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RunningAwayWithFoodState : BaseState
{
    [SerializeField] private float _moveSpeed = 0.5f;

    private List<PathNode> _path;
    private int _currentPathIndex;
    private bool _isPathSet;

    public override void OnStateEnter()
    {
        Debug.Log("[RunningAwayWithFoodState] - entered running state");
        _brain.AStar.GetGrid().GetXY(_kitten.transform.localPosition, out int kittenX, out int kittenY);

        PathNode currentNode = _brain.AStar.Grid.GetGridObject(kittenX, kittenY);

        if (currentNode.NodeType == NodeType.None)
        {
            List<Vector2Int> neighborOffsets = new()
            {
                new(1, 0),
                new(-1, 0),
                new(0, 1),
                new(0, -1)
            };

            PathNode closestNode = null;
            float closestDistance = float.MaxValue;

            foreach (Vector2Int offset in neighborOffsets)
            {
                int neighborX = kittenX + offset.x;
                int neighborY = kittenY + offset.y;

                PathNode neighborNode = _brain.AStar.Grid.GetGridObject(neighborX, neighborY);

                if (_brain.AStar.IsNodeWalkable(neighborNode))
                {
                    Vector3 neighborWorldPos = _brain.AStar.Grid.GetWorldPosition(neighborX, neighborY);
                    float distance = Vector3.Distance(_kitten.transform.localPosition, neighborWorldPos);

                    if (distance < closestDistance)
                    {
                        closestNode = neighborNode;
                        closestDistance = distance;
                    }
                }
            }

            if (closestNode != null)
            {
                kittenX = closestNode.X;
                kittenY = closestNode.Y;
            }
        }

        List<PathNode> potentialNodes = _brain.AStar.GetAllWalkableNodes();
        PathNode nextNode = potentialNodes.First(node => Mathf.RoundToInt(Vector2.Distance(new(node.X, node.Y), new(kittenX, kittenY))) >= 10);
        Vector3 targetPosition = _brain.AStar.Grid.GetWorldPosition(nextNode.X, nextNode.Y);
        _brain.AStar.GetGrid().GetXY(targetPosition, out int targetX, out int targetY);

        _path = _brain.AStar.FindPath(kittenX, kittenY, targetX, targetY);

        if (_path == null || _path.Count == 0)
        {
            _isPathSet = false;

            List<Vector2Int> neighborOffsets = new()
            {
                new(-1, -1),
                new(1, -1),
                new(-1, 1),
                new(1, 1),
                new(1, 0),
                new(-1, 0),
                new(0, 1),
                new(0, -1)
            };
            
            foreach (Vector2Int offset in neighborOffsets)
            {
                int neighborX = targetX + offset.x;
                int neighborY = targetY + offset.y;

                if (_brain.AStar.IsNodeWalkable(_brain.AStar.GetGrid().GetGridObject(neighborX, neighborY)))
                {
                    _path = _brain.AStar.FindPath(kittenX, kittenY, neighborX, neighborY);

                    if (_path != null && _path.Count > 0)
                    {
                        UpdateClosestPathIndex();
                        break;
                    }
                }
            }
        }
        else
        {
            UpdateClosestPathIndex();
        }
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

        if (HasReachedTarget() && _brain.GetState(StateType.Idle, out BaseState idleState))
        {
            return idleState;
        }

        if (_isPathSet)
        {
            FollowPath();
        }

        return null;
    }

    public override void OnStateExit()
    {
        Debug.Log("[RunningAwayWithFoodState] - exitted running state");
        _kitten.IsRunningAway = false;
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
        _kitten.transform.position += (_moveSpeed * Time.deltaTime * direction);

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
