using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSaveUI : MonoBehaviour
{
    [SerializeField] private DeleteSaveConfirmPopup _deleteSaveConfirmPopup;

    [SerializeField] private TMP_Text _fileName;
    [SerializeField] private TMP_Text _gameSeed;
    [SerializeField] private Image _fileImage;
    [SerializeField] private TMP_Text _food;
    [SerializeField] private TMP_Text _timeAlive;
    [SerializeField] private GameSaveInventoryUI _inventory;
    [SerializeField] private Button _button;
    [SerializeField] private UIHighlighter _highlighter;

    private GameSave _gameSave;
    private string _saveFileName;

    public event Action<GameSaveUI> OnSaveFileDeleted;
    public event Action<GameSaveUI> OnPlay;

    public void Init(GameSave gameSave, string filePath)
    {
        _gameSave = gameSave;
        _saveFileName = filePath;

        _fileName.text = _gameSave.Name;
        _gameSeed.text = _gameSave.GameSeeds.MapGenerationSeed.ToString();
        _fileImage.sprite = ByteArrayToSprite(gameSave.Image);
        _food.text = _gameSave.PlayerStats.CurrentFood + "/" + _gameSave.PlayerStats.MaxFood;
        _timeAlive.text = _gameSave.PlayerStats.TimeAlive.ToString();
        _inventory.Init(_gameSave.SavedInventoryData);
    }

    public Sprite ByteArrayToSprite(byte[] imageBytes)
    {
        Texture2D texture = new(2, 2);
        bool isLoaded = texture.LoadImage(imageBytes);

        if (isLoaded)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        return null;
    }

    public void PlaySave()
    {
        OnPlay?.Invoke(this);
        _highlighter.Unhighlight();
        StartCoroutine(LocalDataStorage.Instance.LoadData(_gameSave));
    }

    public void DeleteSave()
    {
        DeleteSaveConfirmPopup popup = Instantiate(_deleteSaveConfirmPopup, transform.parent.parent.parent.parent);
        popup.OnSaveFileDeleted += OnDelete;
        popup.Init(_gameSave, _saveFileName);
    }

    private void OnDelete(DeleteSaveConfirmPopup popup)
    {
        popup.OnSaveFileDeleted -= OnDelete;
        OnSaveFileDeleted?.Invoke(this);
    }

    public void SetButtonInteractable(bool enable)
    {
        _button.interactable = enable;
    }
}
