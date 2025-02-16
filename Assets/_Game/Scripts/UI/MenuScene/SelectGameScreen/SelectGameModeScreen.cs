using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectGameModeScreen : BaseScreen
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private TMP_Text _continueButtonText;
    [SerializeField] private EventTrigger _continueButtonTrigger;

    private List<string> _saveFiles = new();

    private void Start()
    {
        GetSaveFiles();
        SetUpButton();
    }

    private void SetUpButton()
    {
        if (_saveFiles.Count == 0)
        {
            _continueButton.interactable = false;
            _continueButtonText.color = Color.gray;
            _continueButtonTrigger.enabled = false;
        }
    }

    private void GetSaveFiles()
    {
        string path = GlobalConstants.SavedDataPaths.BASE_PATH;
        if (Directory.Exists(path))
        {
            _saveFiles = Directory.GetFiles(path, "*.gg").Select(Path.GetFileName).ToList();
        }
    }

    public void ContinueGame()
    {
        GameSave gameSave = LocalDataStorage.Instance.GetSaveFileFromPath(_saveFiles.Last());
        StartCoroutine(LocalDataStorage.Instance.LoadData(gameSave));
    }

    public void NewGame()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.NewGame);
    }

    public void LoadGame()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.LoadGame);
    }
}
