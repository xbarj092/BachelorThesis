using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineBrain : MonoBehaviour
{
    [SerializeField] private KittenStateMachine _kittenStateMachine;
    [SerializeField] private List<BaseState> _allValidStates = new List<BaseState>();
    [SerializeField] private int _initState = 0;

    public WalkableAStar AStar;
    public Transform PlayerTransform;

    public Transform MouseTransform;
    public Transform LaserTransform;

    public IEnumerator SetUpBrain(Kitten kittenToControl)
    {
        yield return new WaitUntil(() => GameManager.Instance.MapInitialized);
        AStar aStar = FindFirstObjectByType<MapGenerator.MapGenerator>().AStar;
        AStar = new(aStar.Grid);

        foreach (BaseState state in _allValidStates)
        {
            state.Init(kittenToControl, brain: this);
        }

        _kittenStateMachine.ChangeState(_allValidStates[_initState]);
    }

    public void SetBrainActivity(bool value)
    {
        _kittenStateMachine.enabled = value;
    }

    public bool GetState(StateType type, out BaseState state)
    {
        state = null;

        foreach (BaseState validState in _allValidStates)
        {
            if (validState.StateType == type)
            {
                state = validState;
                return true;
            }
        }

        return false;
    }
}
