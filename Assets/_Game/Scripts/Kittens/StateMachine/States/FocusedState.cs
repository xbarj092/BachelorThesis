using MapGenerator;
using System.Collections.Generic;
using UnityEngine;

public class FocusedState : BaseState
{
    [SerializeField] private float _chaseSpeed = 1f;
    [SerializeField] private float _timeoutDuration = 3f;

    private List<PathNode> _path = new();
    private int _currentPathIndex;
    private bool _isPathSet;

    private float _timeoutTimer;

    public override void OnStateEnter()
    {
        Debug.Log("[FocusedState] - entered focused state");
        _path.Clear();
        _currentPathIndex = 0;
        _timeoutTimer = 0;
    }

    public override BaseState ExecuteState()
    {
        if (_kitten.IsDead && _brain.GetState(StateType.Death, out BaseState deathState))
        {
            _kitten.CanSeePlayer = false;
            return deathState;
        }

        if (_kitten.IsTrapped && _brain.GetState(StateType.Trapped, out BaseState trappedState))
        {
            _kitten.CanSeePlayer = false;
            return trappedState;
        }

        if (_kitten.IsRunningAway && _brain.GetState(StateType.RunningAway, out BaseState runningAwayState))
        {
            _kitten.CanSeePlayer = false;
            return runningAwayState;
        }

        if (_kitten.IsInRangeOfPlayer || _kitten.CanSeePlayer)
        {
            _timeoutTimer = 0;
            _path.Clear();
            MoveDirectlyToTarget(_brain.PlayerTransform.position);
            return null;
        }

        _timeoutTimer += Time.deltaTime;

        if (_timeoutTimer >= _timeoutDuration && _brain.GetState(StateType.Idle, out BaseState idleState))
        {
            return idleState;
        }

        if (_path.Count == 0 || _currentPathIndex >= _path.Count)
        {
            Vector3 lastKnownPosition = _brain.PlayerTransform.localPosition;
            _brain.AStar.GetGrid().GetXY(_kitten.transform.localPosition, out int kittenX, out int kittenY);
            _brain.AStar.GetGrid().GetXY(lastKnownPosition, out int targetX, out int targetY);
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
                            _currentPathIndex = 1;
                            _isPathSet = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                _currentPathIndex = 1;
                _isPathSet = true;
            }
        }

        if (_isPathSet)
        {
            FollowPath();
        }

        return null;
    }

    public override void OnStateExit()
    {
        _path.Clear();
        Debug.Log("[FocusedState] - exited focused state");
    }

    private void FollowPath()
    {
        if (_path.Count == 0 || _currentPathIndex >= _path.Count)
        {
            return;
        }

        PathNode currentNode = _path[_currentPathIndex];
        Vector3 targetPosition = _brain.AStar.Grid.GetWorldPosition(currentNode.X, currentNode.Y);

        MoveDirectlyToTarget(targetPosition);

        if (Vector3.Distance(_kitten.transform.position, targetPosition) <= 0.1f)
        {
            _currentPathIndex++;
        }
    }

    private void MoveDirectlyToTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - _kitten.transform.position).normalized;
        _kitten.transform.position += (_chaseSpeed * Time.deltaTime * direction);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _kitten.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
