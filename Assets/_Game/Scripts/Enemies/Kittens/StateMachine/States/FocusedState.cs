using MapGenerator;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FocusedState : BaseState
{
    [SerializeField] private float _chaseSpeed = 1f;
    [SerializeField] private float _chaseSpeedAfter1Min = 0.9f;
    [SerializeField] private float _chaseSpeedAfter2Mins = 1.15f;
    [SerializeField] private float _timeoutDuration = 3f;

    private List<PathNode> _path = new();
    private int _currentPathIndex;
    private bool _isPathSet;
    private bool _targetLost;

    private float _timeoutTimer;

    private Transform _currentTarget;

    private Vector3 _lastKnownPosition;

    public override void OnStateEnter()
    {
        Debug.Log("[FocusedState] - Entered focused state");
        _path.Clear();
        _currentPathIndex = 0;
        _timeoutTimer = 0;

        UpdateFocusTarget();
        if (_currentTarget != null)
        {
            _lastKnownPosition = _currentTarget.position;
        }
    }

    public override BaseState ExecuteState()
    {
        if (_kitten.IsDead && _brain.GetState(StateType.Death, out BaseState deathState))
        {
            _kitten.CanSeeTarget = false;
            return deathState;
        }
        
        if (_kitten.IsTrapped && _brain.GetState(StateType.Trapped, out BaseState trappedState))
        {
            _kitten.CanSeeTarget = false;
            return trappedState;
        }
        
        if (_kitten.IsRunningAway && _brain.GetState(StateType.RunningAway, out BaseState runningAwayState))
        {
            _kitten.CanSeeTarget = false;
            return runningAwayState;
        }

        if ((_currentTarget != null && _currentTarget.CompareTag(GlobalConstants.Tags.Player.ToString()) && 
            LocalDataStorage.Instance.PlayerData.PlayerStats.StatusEffects.Any(effect => effect.Type == (int)StatusEffectType.Invisibility) ||
            _targetLost) && _brain.GetState(StateType.Idle, out BaseState idleState))
        {
            return idleState;
        }

        UpdateFocusTarget();

        if (_currentTarget != null)
        {
            _lastKnownPosition = _currentTarget.position;

            if (_kitten.IsInRangeOfPlayer || _kitten.CanSeeTarget)
            {
                _timeoutTimer = 0;
                _path.Clear();
                MoveDirectlyToTarget(_currentTarget.position);
                return null;
            }
            else
            {
                _currentTarget = null;
            }
        }

        if (_currentTarget == null)
        {
            FollowPathTo(_lastKnownPosition);
        }

        _timeoutTimer += Time.deltaTime;
        if (_timeoutTimer >= _timeoutDuration && _brain.GetState(StateType.Idle, out idleState))
        {
            return idleState;
        }

        return null;
    }

    public override void OnStateExit()
    {
        _path.Clear();
        _currentTarget = null;
        _targetLost = false;
        Debug.Log("[FocusedState] - exited focused state");
    }

    private void UpdateFocusTarget()
    {
        if (_brain.MouseTransform != null)
        {
            _currentTarget = _brain.MouseTransform;
        }
        else if (_brain.LaserTransform != null)
        {
            _currentTarget = _brain.LaserTransform;
        }
        else if (_brain.PlayerTransform != null)
        {
            _currentTarget = _brain.PlayerTransform;
        }
        else
        {
            _currentTarget = null;
        }
    }

    private void FollowPathTo(Vector3 targetPosition)
    {
        if (_path.Count == 0 || _currentPathIndex >= _path.Count)
        {
            if (Vector2.Distance(_kitten.transform.localPosition, targetPosition) < 1f)
            {
                _targetLost = true;
            }

            _brain.AStar.GetGrid().GetXY(_kitten.transform.localPosition, out int kittenX, out int kittenY);
            _brain.AStar.GetGrid().GetXY(targetPosition, out int targetX, out int targetY);

            _path = _brain.AStar.FindPath(kittenX, kittenY, targetX, targetY);

            if (_path != null)
            {
                CorrectInvalidNodesInPath(_path);
            }

            if (_path == null || _path.Count == 0)
            {
                List<Vector2Int> neighborOffsets = new()
            {
                new(-1, -1), new(1, -1),
                new(-1, 1), new(1, 1),
                new(1, 0), new(-1, 0),
                new(0, 1), new(0, -1)
            };

                Vector2Int closestNeighbor = Vector2Int.zero;
                float closestDistance = float.MaxValue;

                foreach (Vector2Int offset in neighborOffsets)
                {
                    int neighborX = targetX + offset.x;
                    int neighborY = targetY + offset.y;

                    if (_brain.AStar.IsNodeWalkable(_brain.AStar.GetGrid().GetGridObject(neighborX, neighborY)))
                    {
                        float distanceToTarget = Vector3.Distance(
                            _brain.AStar.Grid.GetWorldPosition(neighborX, neighborY),
                            targetPosition
                        );

                        if (distanceToTarget < closestDistance)
                        {
                            closestDistance = distanceToTarget;
                            closestNeighbor = new Vector2Int(neighborX, neighborY);
                        }
                    }
                }

                if (closestDistance < float.MaxValue)
                {
                    _path = _brain.AStar.FindPath(kittenX, kittenY, closestNeighbor.x, closestNeighbor.y);

                    if (_path != null)
                    {
                        CorrectInvalidNodesInPath(_path);
                    }

                    if (_path != null && _path.Count > 0)
                    {
                        _currentPathIndex = 0;
                        _isPathSet = true;
                        UpdateClosestPathIndex();
                        return;
                    }
                }

                _path = new();
                _isPathSet = false;
                return;
            }

            _currentPathIndex = 0;
            _isPathSet = true;
            UpdateClosestPathIndex();
        }

        if (_isPathSet)
        {
            PathNode currentNode = _path[_currentPathIndex];

            if (!_brain.AStar.IsNodeWalkable(currentNode))
            {
                ReplaceWithClosestWalkableNeighbor(_currentPathIndex);
            }

            Vector3 nodePosition = _brain.AStar.Grid.GetWorldPosition(currentNode.X, currentNode.Y);
            MoveDirectlyToTarget(nodePosition);

            if (Vector3.Distance(_kitten.transform.position, nodePosition) <= 0.1f)
            {
                _currentPathIndex++;
            }
        }
    }

    private void CorrectInvalidNodesInPath(List<PathNode> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            if (!_brain.AStar.IsNodeWalkable(path[i]))
            {
                ReplaceWithClosestWalkableNeighbor(i);
            }
        }
    }

    private void ReplaceWithClosestWalkableNeighbor(int pathIndex)
    {
        PathNode node = _path[pathIndex];

        List<Vector2Int> neighborOffsets = new()
        {
            new(-1, -1), new(1, -1),
            new(-1, 1), new(1, 1),
            new(1, 0), new(-1, 0),
            new(0, 1), new(0, -1)
        };

        float closestDistance = float.MaxValue;
        PathNode closestNode = null;

        foreach (Vector2Int offset in neighborOffsets)
        {
            int neighborX = node.X + offset.x;
            int neighborY = node.Y + offset.y;

            PathNode neighborNode = _brain.AStar.GetGrid().GetGridObject(neighborX, neighborY);

            if (_brain.AStar.IsNodeWalkable(neighborNode))
            {
                float distance = Vector3.Distance(
                    _brain.AStar.Grid.GetWorldPosition(neighborNode.X, neighborNode.Y),
                    _brain.AStar.Grid.GetWorldPosition(node.X, node.Y)
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNode = neighborNode;
                }
            }
        }

        if (closestNode != null)
        {
            _path[pathIndex] = closestNode;
        }
    }

    private void UpdateClosestPathIndex()
    {
        int maxNodesToCheck = Mathf.Min(3, _path.Count);
        float closestDistance = float.MaxValue;

        for (int i = 0; i < maxNodesToCheck; i++)
        {
            PathNode pathNode = _path[i];
            if (!_brain.AStar.IsNodeWalkable(pathNode))
            {
                continue;
            }

            Vector3 nodeWorldPosition = _brain.AStar.Grid.GetWorldPosition(pathNode.X, pathNode.Y);
            float distanceToPlayer = Vector3.Distance(_kitten.transform.localPosition, nodeWorldPosition);

            if (distanceToPlayer < closestDistance)
            {
                closestDistance = distanceToPlayer;
                _currentPathIndex = i;
            }
        }
    }

    private void MoveDirectlyToTarget(Vector3 targetPosition)
    {
        _brain.AStar.GetGrid().GetXY(targetPosition, out int targetX, out int targetY);

        if (!_brain.AStar.IsNodeWalkable(_brain.AStar.GetGrid().GetGridObject(targetX, targetY)))
        {
            PathNode closestWalkableNode = FindClosestWalkableNeighbor(targetX, targetY);

            if (closestWalkableNode != null)
            {
                targetPosition = _brain.AStar.Grid.GetWorldPosition(closestWalkableNode.X, closestWalkableNode.Y);
            }
            else
            {
                _targetLost = true;
                return;
            }
        }

        Vector3 direction = (targetPosition - _kitten.transform.position).normalized;
        float chaseSpeed = GetChaseSpeed(LocalDataStorage.Instance.PlayerData.PlayerStats.TimeAlive);
        _kitten.transform.position += (_chaseSpeed * Time.deltaTime * direction);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        _kitten.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private float GetChaseSpeed(int timeAlive)
    {
        if (timeAlive > 120)
        {
            return _chaseSpeedAfter2Mins;
        }
        else if (timeAlive > 60)
        {
            return _chaseSpeedAfter1Min;
        }
        else
        {
            return _chaseSpeed;
        }
    }

    private PathNode FindClosestWalkableNeighbor(int nodeX, int nodeY)
    {
        List<Vector2Int> neighborOffsets = new()
        {
            new(-1, -1), new(1, -1),
            new(-1, 1), new(1, 1),
            new(1, 0), new(-1, 0),
            new(0, 1), new(0, -1)
        };

        PathNode closestNode = null;
        float closestDistance = float.MaxValue;

        foreach (Vector2Int offset in neighborOffsets)
        {
            int neighborX = nodeX + offset.x;
            int neighborY = nodeY + offset.y;

            PathNode neighborNode = _brain.AStar.GetGrid().GetGridObject(neighborX, neighborY);

            if (_brain.AStar.IsNodeWalkable(neighborNode))
            {
                float distance = Vector3.Distance(
                    _brain.AStar.Grid.GetWorldPosition(neighborNode.X, neighborNode.Y),
                    _brain.AStar.Grid.GetWorldPosition(nodeX, nodeY)
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNode = neighborNode;
                }
            }
        }

        return closestNode;
    }
}
