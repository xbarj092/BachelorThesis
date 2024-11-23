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

        if ((_kitten.PotentialPartner.IsDead || _kitten.PotentialPartner.IsTrapped) && _brain.GetState(StateType.Roaming, out BaseState roamingState))
        {
            return roamingState;
        }

        if (_kitten.IsTrapped && _brain.GetState(StateType.Trapped, out BaseState trappedState))
        {
            return trappedState;
        }

        if (IsDoneWaiting() && _brain.GetState(StateType.Idle, out BaseState idleState))
        {
            if (_kitten.Male)
            {
                Vector3 spawnPosition = (_kitten.transform.position + _kitten.PotentialPartner.transform.position) / 2f;
                Kitten kitten = Instantiate(_kitten, spawnPosition, Quaternion.identity);
                kitten.IsMating = false;
            }

            _kitten.IsMating = false;
            _kitten.AlreadyMated = true;
            _kitten.PotentialPartner = null;
            return idleState;
        }

        return null;
    }

    public override void OnStateExit()
    {
        _kitten.IsMating = false;
        _kitten.PotentialPartner = null;
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
