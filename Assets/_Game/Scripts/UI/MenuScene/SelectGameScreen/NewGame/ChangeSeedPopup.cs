using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeSeedPopup : Popup
{
    [SerializeField] private TMP_Text _errorText;

    [SerializeField] private TMP_InputField _inputField;

    public event Action<string> OnSeedChanged;

    private const string ERROR_EMPTY_SEED = "Please enter new seed";
    private const string ERROR_INVALID_SEED = "Invalid seed";

    public void ChangeSeed()
    {
        if (HandleErrorMessages())
        {
            return;
        }

        OnSeedChanged?.Invoke(_inputField.text);
        Close();
    }

    private bool HandleErrorMessages()
    {
        Dictionary<Func<bool>, string> validations = new()
        {
            { () => string.IsNullOrEmpty(_inputField.text), ERROR_EMPTY_SEED },
            { () => !int.TryParse(_inputField.text, out int seed), ERROR_INVALID_SEED },
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
