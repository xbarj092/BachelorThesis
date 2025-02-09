public interface IState
{
    void OnStateEnter();
    BaseState ExecuteState();
    void OnStateExit();
}
