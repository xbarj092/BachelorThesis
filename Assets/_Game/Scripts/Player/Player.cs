using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;

    private Vector2 _moveInput;

    private void Update()
    {
        // DepleteFood();
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _moveInput;

        if (_moveInput.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(_moveInput.y, _moveInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, targetAngle + 90);
        }
    }

    private void OnMove(InputValue inputValue)
    {
        _moveInput = inputValue.Get<Vector2>();
    }

    public void PickupItem(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out Item item))
        {
            if (item.Dropped)
            {
                return;
            }

            InventoryData inventoryData = LocalDataStorage.Instance.PlayerData.InventoryData;
            for (int i = 0; i < inventoryData.ItemsInInventory.Count; i++)
            {
                if (inventoryData.ItemsInInventory[i] == null)
                {
                    inventoryData.ItemsInInventory[i] = item;
                    item.PickUp();
                    LocalDataStorage.Instance.PlayerData.InventoryData = inventoryData;
                    break;
                }
            }
        }
    }

    public void PickupFood(GameObject foodObject)
    {
        PlayerStats stats = LocalDataStorage.Instance.PlayerData.PlayerStats;
        if (stats.CurrentFood < stats.MaxFood)
        {
            stats.CurrentFood++;
        }
        else
        {
            stats.CurrentTimeToEatFood = stats.TimeToEatFood;
        }
        LocalDataStorage.Instance.PlayerData.PlayerStats = stats;

        Destroy(foodObject);
    }

    private void DepleteFood()
    {
        PlayerStats stats = LocalDataStorage.Instance.PlayerData.PlayerStats;
        stats.CurrentTimeToEatFood -= Time.deltaTime;

        if (stats.CurrentTimeToEatFood <= 0)
        {
            EatFood(stats);
            stats.CurrentTimeToEatFood = stats.TimeToEatFood;
        }

        LocalDataStorage.Instance.PlayerData.PlayerStats = stats;
    }

    public void EatFood(PlayerStats stats)
    {
        if (stats.CurrentFood > 1)
        {
            stats.CurrentFood -= 1;
            LocalDataStorage.Instance.PlayerData.PlayerStats = stats;
        }
        else
        {
            ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Death);

            // TODO: death
        }
    }
}
