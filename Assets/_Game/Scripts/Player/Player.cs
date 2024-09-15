using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;

    private void Update()
    {
        Move();
        DepleteFood();
    }

    private void Move()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        _rigidbody.velocity = new(horizontalMove, verticalMove);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GlobalConstants.Tags.Food.ToString()))
        {
            PickupFood(collision.gameObject);
        }
    }

    private void PickupFood(GameObject foodObject)
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

    private void EatFood(PlayerStats stats)
    {
        if (stats.CurrentFood > 1)
        {
            stats.CurrentFood -= 1;
        }
        else
        {
            ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Death);

            // TODO: death
        }
    }
}
