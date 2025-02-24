public class GoldenAppleItemStrategy : FoodItemStrategy
{
    public override void PickUp(ConsumableItem item)
    {
        base.PickUp(item);
        GameEvents.OnAppleEatenInvoke();
    }
}
