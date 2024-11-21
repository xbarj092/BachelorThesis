using UnityEngine;

public class MatingState : BaseState
{
    [SerializeField] private float _matingTime = 2f;

    public override void OnStateEnter()
    {
        Debug.Log("[MatingState] - entered mating state");
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

        if (IsDoneWaiting() && _brain.GetState(StateType.Idle, out BaseState idleState))
        {
            return idleState;
        }

        return null;
    }

    public override void OnStateExit()
    {
        _kitten.IsMating = false;
        Debug.Log("[MatingState] - exitted mating state");
    }

    private bool IsDoneWaiting()
    {
        _currentTime += Time.deltaTime;
        if (_matingTime <= _currentTime)
        {
            return true;
        }

        return false;
    }
}
