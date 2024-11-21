using MapGenerator;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RunningAwayWithFoodState : BaseState
{
    [SerializeField] private float _moveSpeed = 0.5f;

    private Vector3 _targetPosition;

    private List<PathNode> _path;
    private int _currentPathIndex;
    private bool _isPathSet;

    public override void OnStateEnter()
    {
        List<PathNode> validNodes = new();
        for (int i = 0; i < _brain.AStar.Grid.GetWidth(); i++)
        {
            for (int j = 0; j < _brain.AStar.Grid.GetHeight(); j++)
            {
                PathNode node = _brain.AStar.Grid.GetGridObject(i, j);
                if (_brain.AStar.IsNodeWalkable(node))
                {
                    validNodes.Add(node);
                }
            }
        }

        int randomDistance = Random.Range(10, 30);

        PathNode targetNode = validNodes.First(node => Mathf.RoundToInt(Vector2.Distance(_brain.AStar.Grid.GetWorldPosition(node.X, node.Y), _kitten.transform.position)) == randomDistance);

        _targetPosition = _brain.AStar.Grid.GetWorldPosition(targetNode.X, targetNode.Y);
        _brain.AStar.GetGrid().GetXY(_kitten.transform.localPosition, out int kittenX, out int kittenY);
        _brain.AStar.GetGrid().GetXY(_targetPosition, out int targetX, out int targetY);
        _path = _brain.AStar.FindPath(kittenX, kittenY, targetX, targetY);

        _isPathSet = _path != null && _path.Count > 0;

        Debug.Log("[RunningAwayWithFoodState] - entered running state");
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

        if (IsOnTargetPosition() && _brain.GetState(StateType.Idle, out BaseState idleState))
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

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _kitten.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Vector3.Distance(_kitten.transform.position, targetPosition) <= 0.1f)
        {
            _currentPathIndex++;
        }
    }

    private bool IsOnTargetPosition()
    {
        return Vector3.Distance(_kitten.transform.position, _targetPosition) <= 0.1f;
    }
}
