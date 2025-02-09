using UnityEngine;

public class Item<T> : MonoBehaviour where T : ItemBaseSO
{
    [field: SerializeField] public T Stats { get; private set; }

    protected ItemStrategyBase _strategy;
    protected StrategyFactory _strategyFactory = new();
}
