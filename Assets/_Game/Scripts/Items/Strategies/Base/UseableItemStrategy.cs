using UnityEngine;

public abstract class UseableItemStrategy : StrategyBase<UseableItem, UseableItemSO, SavedUseableItem>, IUseableStrategy
{
    public abstract bool CanUse(UseableItem item);
    public abstract void Use(UseableItem item);
    public override void PickUp(UseableItem item)
    {
        InventoryData inventoryData = LocalDataStorage.Instance.PlayerData.InventoryData;
        if (item.Dropped || inventoryData.ItemsInInventory.Contains(item))
        {
            return;
        }

        for (int i = 0; i < inventoryData.ItemsInInventory.Count; i++)
        {
            if (inventoryData.ItemsInInventory[i] == null)
            {
                if (inventoryData.IsInventoryEmpty())
                {
                    inventoryData.CurrentHighlightIndex = 0;
                }

                if (!AudioManager.Instance.IsPlaying(SoundType.ItemUsed))
                {
                    AudioManager.Instance.Play(SoundType.ItemPickedUp);
                }

                LocalDataStorage.Instance.PlayerData.UnlockedCollectibleData.AddUseable(item.Stats);
                if (!TutorialManager.Instance.IsTutorialCompleted(item.Stats.ItemType) && !TutorialManager.Instance.IsTutorialPlaying())
                {
                    TutorialManager.Instance.InstantiateTutorial(item.Stats.ItemType);
                }
                inventoryData.ItemsInInventory[i] = item;
                LocalDataStorage.Instance.PlayerData.InventoryData = inventoryData;
                break;
            }
        }

        base.PickUp(item);
    }

    protected void PlaceOnMousePosition(UseableItem item)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        PlayerInteraction player = GameObject.FindFirstObjectByType<PlayerInteraction>();
        Vector3 playerPosition = player.transform.position;

        Vector3 validPosition = player.GetValidPlacementPosition(playerPosition, mousePosition);

        item.gameObject.transform.position = validPosition;
        item.gameObject.SetActive(true);

        LocalDataStorage.Instance.PlayerData.InventoryData.RemoveItemFromInventory(item);
    }
}
