using UnityEngine;

public class ApproachState : BaseState
{
    private Transform _partnerTransform;

    private float _approachSpeed = 1f;
    private float _matingRange = 1f;
    private float _matingDelay = 2f;
    private float _waitTimer;

    public override void OnStateEnter()
    {
        Debug.Log("[ApproachState] - Entered approach state");

        if (_kitten.PotentialPartner != null)
        {
            _partnerTransform = _kitten.PotentialPartner.transform;
        }
        else
        {
            Debug.LogWarning("[ApproachState] - No potential partner assigned!");
        }

        _waitTimer = 0f;
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

        if (_kitten.IsMating && _brain.GetState(StateType.Mating, out BaseState matingState))
        {
            return matingState;
        }

        Vector3 directionToPartner = (_partnerTransform.position - _kitten.transform.position).normalized;
        float distanceToPartner = Vector2.Distance(_kitten.transform.position, _partnerTransform.position);

        if (distanceToPartner > _matingRange)
        {
            float angle = Mathf.Atan2(directionToPartner.y, directionToPartner.x) * Mathf.Rad2Deg;
            _kitten.transform.rotation = Quaternion.Euler(0, 0, angle);

            _kitten.transform.position += (_approachSpeed * Time.deltaTime * directionToPartner);
        }
        else
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= _matingDelay)
            {
                if ((_kitten.IsCastrated || _kitten.PotentialPartner.IsCastrated || _kitten.PotentialPartner.Male == _kitten.Male) && 
                    _brain.GetState(StateType.Roaming, out roamingState))
                {
                    return roamingState;
                }

                _kitten.IsMating = true;
            }
        }

        return null;
    }

    public override void OnStateExit()
    {
        Debug.Log("[ApproachState] - Exited approach state");
        _waitTimer = 0f;
        _kitten.IsApproaching = false;
    }
}
