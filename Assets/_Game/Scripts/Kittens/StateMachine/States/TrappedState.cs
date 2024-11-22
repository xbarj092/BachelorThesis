using UnityEngine;

public class TrappedState : BaseState
{
    public override void OnStateEnter()
    {
        Debug.Log("[TrappedState] - entered trapped state");
    }

    public override BaseState ExecuteState()
    {
        if (_kitten.IsDead && _brain.GetState(StateType.Death, out BaseState deathState))
        {
            return deathState;
        }

        if (_kitten.IsRunningAway && _brain.GetState(StateType.RunningAway, out BaseState runningAwayState))
        {
            return runningAwayState;
        }

        if (!_kitten.IsTrapped && _brain.GetState(StateType.Idle, out BaseState idleState))
        {
            return idleState;
        }

        return null;
    }

    public override void OnStateExit()
    {
        Debug.Log("[TrappedState] - exitted trapped state");
    }
}
