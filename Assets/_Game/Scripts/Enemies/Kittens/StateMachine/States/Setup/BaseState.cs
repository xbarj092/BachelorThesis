using UnityEngine;

public abstract class BaseState : MonoBehaviour, IState
{
    public StateType StateType;

    protected StateMachineBrain _brain;
    protected Kitten _kitten;
    protected float _currentTime = 0;

    public void Init(Kitten kitten, StateMachineBrain brain)
    {
        _kitten = kitten;
        _brain = brain;
    }

    public abstract void OnStateEnter();

    public abstract BaseState ExecuteState();

    public abstract void OnStateExit();
}
