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
        if (_timeoutTimer >= _timeoutDuration && _brain.GetState(StateType.Idle, out BaseState idleState))
        {
            return idleState;
        }

        return null;
    }

    public override void OnStateExit()
    {
        _path.Clear();
        _currentTarget = null;
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
            _brain.AStar.GetGrid().GetXY(_kitten.transform.localPosition, out int kittenX, out int kittenY);
            _brain.AStar.GetGrid().GetXY(targetPosition, out int targetX, out int targetY);
            _path = _brain.AStar.FindPath(kittenX, kittenY, targetX, targetY);

            if (_path == null || _path.Count == 0)
            {
                _path = new();
                _isPathSet = false;
                return;
            }

            _currentPathIndex = _path.Count > 1 ? 1 : 0;
            _isPathSet = true;
        }

        if (_isPathSet)
        {
            PathNode currentNode = _path[_currentPathIndex];
            Vector3 nodePosition = _brain.AStar.Grid.GetWorldPosition(currentNode.X, currentNode.Y);
            MoveDirectlyToTarget(nodePosition);

            if (Vector3.Distance(_kitten.transform.position, nodePosition) <= 0.1f)
            {
                _currentPathIndex++;
            }
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
