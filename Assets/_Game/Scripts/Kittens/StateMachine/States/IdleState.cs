using UnityEngine;

public class IdleState : BaseState
{
    [SerializeField] private float _totalEnterWaitTime = 2f;

    public override void OnStateEnter()
    {
        Debug.Log("[IdleState] - entered idle state");
        _kitten.Rigidbody.velocity = Vector3.zero;
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

        if (_kitten.IsMating && _brain.GetState(StateType.Mating, out BaseState matingState))
        {
            return matingState;
        }

        if ((_kitten.IsInRangeOfPlayer || _kitten.CanSeePlayer) && _brain.GetState(StateType.Focused, out BaseState focusedState))
        {
            return focusedState;
        }

        if (IsDoneWaiting() && _brain.GetState(StateType.Roaming, out BaseState roamingState))
        {
            return roamingState;
        }

        return null;
    }

    public override void OnStateExit()
    {
        Debug.Log("[IdleState] - exitted idle state");
        _currentTime = 0f;
    }

    private bool IsDoneWaiting()
    {
        _currentTime += Time.deltaTime;
        if (_totalEnterWaitTime <= _currentTime)
        {
            return true;
        }

        return false;
    }
}
