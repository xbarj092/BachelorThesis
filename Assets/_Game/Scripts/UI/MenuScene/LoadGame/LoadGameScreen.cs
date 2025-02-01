using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class LoadGameScreen : BaseScreen
{
    [SerializeField] private RectTransform _spawnTransform;
    [SerializeField] private GameSaveUI _gameSavePrefab;
    [SerializeField] private TMP_Text _noSaveFiles;

    private Vector3 _defaultSize;

    private void Awake()
    {
        _defaultSize = _spawnTransform.sizeDelta;
        SetUpGameSaves();
    }

    private void SetUpGameSaves()
    {
        List<string> saveFiles = GetAllSaveFiles();
        _noSaveFiles.gameObject.SetActive(saveFiles.Count == 0);
        float prefabHeight = _gameSavePrefab.GetComponent<RectTransform>().sizeDelta.y;
        float spacing = 25f;
        float initialOffsetY = -150;

        Vector2 spawnPosition = new(0, initialOffsetY);

        foreach (string filePath in saveFiles)
        {
            GameSave gameSave = LocalDataStorage.Instance.GetSaveFileFromPath(filePath);
            GameSaveUI saveUI = Instantiate(_gameSavePrefab, _spawnTransform);

            RectTransform rectTransform = saveUI.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = spawnPosition;

            spawnPosition -= new Vector2(0, prefabHeight + spacing);

            saveUI.Init(gameSave, filePath);
            saveUI.OnSaveFileDeleted += OnSaveFileDeleted;
            saveUI.OnPlay += OnPlay;
        }

        AdjustContentHeight(prefabHeight, spacing, saveFiles.Count);
    }

    private void OnPlay(GameSaveUI saveUI)
    {
        saveUI.OnPlay += OnPlay;

        foreach (Transform child in _spawnTransform)
        {
            child.GetComponent<GameSaveUI>().SetButtonInteractable(false);
        }
    }

    private void OnSaveFileDeleted(GameSaveUI saveUI)
    {
        saveUI.OnSaveFileDeleted -= OnSaveFileDeleted;
        _spawnTransform.sizeDelta = _defaultSize;

        foreach (Transform child in _spawnTransform)
        {
            Destroy(child.gameObject);
        }

        SetUpGameSaves();
    }

    private List<string> GetAllSaveFiles()
    {
        string path = GlobalConstants.SavedDataPaths.BASE_PATH;
        if (Directory.Exists(path))
        {
            return Directory.GetFiles(path, "*.gg").Select(Path.GetFileName).ToList();
        }

        return new List<string>();
    }

    private void AdjustContentHeight(float prefabHeight, float spacing, int itemCount)
    {
        if (itemCount == 0)
        {
            return;
        }

        float totalHeight = (prefabHeight + spacing) * itemCount + 50;
        Vector2 newSize = _spawnTransform.sizeDelta;
        newSize.y = Mathf.Max(totalHeight, _spawnTransform.sizeDelta.y);
        _spawnTransform.sizeDelta = newSize;
    }
}
