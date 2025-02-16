using System;
using UnityEngine;

public class KittenStateMachine : MonoBehaviour
{
    public BaseState CurrentState { private set; get; }

    public event Action<BaseState> OnStateChanged;

    private void Update()
    {
        if (CurrentState == null || TutorialManager.Instance.IsPaused)
        {
            return;
        }

        BaseState nextState = CurrentState.ExecuteState();
        if (nextState != null && nextState != CurrentState)
        {
            ChangeState(nextState);
        }
    }

    public void ChangeState(BaseState newState)
    {
        if (CurrentState != null)
            CurrentState.OnStateExit();

        if (CurrentState == newState)
            return;

        CurrentState = newState;
        CurrentState.OnStateEnter();

        OnStateChanged?.Invoke(newState);
    }
}
