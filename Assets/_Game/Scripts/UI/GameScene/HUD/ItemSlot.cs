using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private ProgressBarUI _batteryProgressBar; 

    [SerializeField] private Image _borderImage;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _highlightedSprite;

    public bool Occupied = false;
    public UseableItem Item;

    public void Init(UseableItem item)
    {
        Item = item;
        _batteryProgressBar.gameObject.SetActive(false);
        _itemImage.sprite = item.Stats.Sprite;
        Occupied = true;

        if (item is Laser laser)
        {
            ((Laser)Item).OnBatteryChanged -= OnBatteryChanged;
            SetUpLaser(laser);
        }
    }

    private void SetUpLaser(Laser laser)
    {
        _batteryProgressBar.gameObject.SetActive(true);
        _batteryProgressBar.SetProgress(laser.Battery / ((LaserItem)laser.Stats).Battery);
        laser.OnBatteryChanged += OnBatteryChanged;
    }

    private void OnBatteryChanged(float battery)
    {
        if (battery > 0)
        {
            _batteryProgressBar.SetProgress(battery);
        }
        else
        {
            _batteryProgressBar.gameObject.SetActive(false);
            ((Laser)Item).OnBatteryChanged -= OnBatteryChanged;
        }
    }

    public void Highlight()
    {
        _borderImage.sprite = _highlightedSprite;
    }

    public void Unhighlight()
    {
        _borderImage.sprite = _defaultSprite;
    }

    public void Disable()
    {
        if (Item is Laser laser)
        {
            ((Laser)Item).OnBatteryChanged -= OnBatteryChanged;
            _batteryProgressBar.SetProgress(1);
            _batteryProgressBar.gameObject.SetActive(false);
        }
    }
}
