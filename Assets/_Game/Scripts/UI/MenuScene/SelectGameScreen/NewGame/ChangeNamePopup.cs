using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class ChangeNamePopup : Popup
{
    [SerializeField] private int _shortPlayerNameThreshold;
    [SerializeField] private int _longPlayerNameThreshold;

    [SerializeField] private TMP_Text _errorText;

    [SerializeField] private TMP_InputField _inputField;

    public event Action<string> OnNameChanged;

    private const string ERROR_EMPTY_PLAYER_NAME = "Please enter a player name";
    private const string ERROR_SHORT_PLAYER_NAME = "Player name must be longer than ";
    private const string ERROR_LONG_PLAYER_NAME = "Player name must be shorter than ";
    private const string ERROR_WRONG_CHARACTERS = "Please use only letters and numbers";

    public void ChangeName()
    {
        if (HandleErrorMessages())
        {
            return;
        }

        LootLockerManager.Instance.SetPlayerName(_inputField.text);
        OnNameChanged?.Invoke(_inputField.text);
        Close();
    }

    private bool HandleErrorMessages()
    {
        Dictionary<Func<bool>, string> validations = new()
        {
            { () => string.IsNullOrEmpty(_inputField.text), ERROR_EMPTY_PLAYER_NAME },
            { () => _inputField.text.Length < _shortPlayerNameThreshold, ERROR_SHORT_PLAYER_NAME + _shortPlayerNameThreshold.ToString() },
            { () => _inputField.text.Length > _longPlayerNameThreshold, ERROR_LONG_PLAYER_NAME + _longPlayerNameThreshold.ToString() },
            { () => !Regex.IsMatch(_inputField.text, @"^[a-zA-Z\d]+$"), ERROR_WRONG_CHARACTERS },
        };

        foreach (KeyValuePair<Func<bool>, string> validation in validations)
        {
            if (validation.Key())
            {
                ShowErrorText(validation.Value);
                return true;
            }
        }

        HideErrorText();
        return false;
    }

    private void ShowErrorText(string errorText)
    {
        _errorText.gameObject.SetActive(true);
        _errorText.text = errorText;
    }

    private void HideErrorText()
    {
        _errorText.gameObject.SetActive(false);
    }
}
