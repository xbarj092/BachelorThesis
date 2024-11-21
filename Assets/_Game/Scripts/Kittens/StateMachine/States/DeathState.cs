using UnityEngine;

public class DeathState : BaseState
{
    public override void OnStateEnter()
    {
        Debug.Log("[DeathState] - entered death state");
    }

    public override BaseState ExecuteState()
    {
        return null;
    }

    public override void OnStateExit()
    {
        Debug.Log("[DeathState] - exitted death state");
    }
}
