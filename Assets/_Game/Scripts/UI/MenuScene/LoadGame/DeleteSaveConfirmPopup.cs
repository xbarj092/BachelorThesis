using System;
using System.IO;
using TMPro;
using UnityEngine;

public class DeleteSaveConfirmPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text _confirmText;

    private GameSave _gameSave;
    private string _saveName;

    public event Action<DeleteSaveConfirmPopup> OnSaveFileDeleted;

    private const string CONFIRM_TEXT_PREFIX = "ARE YOU SURE THAT YOU WANT TO DELETE ";
    private const string CONFIRM_TEXT_SUFFIX = "? THIS ACTION IS IRREVERSIBLE.";

    public void Init(GameSave gameSave, string saveName)
    {
        _gameSave = gameSave;
        _saveName = saveName;

        _confirmText.text = CONFIRM_TEXT_PREFIX + _gameSave.Name + CONFIRM_TEXT_SUFFIX;
    }

    public void Delete()
    {
        string path = Path.Combine(GlobalConstants.SavedDataPaths.BASE_PATH, _saveName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        OnSaveFileDeleted?.Invoke(this);
        Destroy(gameObject);
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
