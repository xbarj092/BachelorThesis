using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyBindingPanel : OptionPanel
{
    [SerializeField] private List<RebindActionUI> _keyBinds = new();
    [SerializeField] private Popup _bindConflictPopup;

    private List<RebindActionUI> _invalidBinds = new();
    private Popup _bindConflictPopupInstantiated;

    private void OnEnable()
    {
        foreach (RebindActionUI action in _keyBinds)
        {
            action.OnRebind += OnRebind;
            action.OnConflict += OnConflict;
        }
    }

    private void OnDisable()
    {
        foreach (RebindActionUI action in _keyBinds)
        {
            action.OnRebind -= OnRebind;
            action.OnConflict -= OnConflict;
        }
    }

    private void OnRebind(RebindActionUI rebindActionUI)
    {
        bool hasBinding = false;
        RebindActionUI unblockedAction = null;
        rebindActionUI.ResolveActionAndBinding(out InputAction inputAction, out int bindingIndex);

        foreach (RebindActionUI action in _invalidBinds.Where(invalidAction => invalidAction != rebindActionUI))
        {
            if (action.HasBinding(inputAction.bindings[bindingIndex].effectivePath))
            {
                hasBinding = true;
                action.SetInvalidBind();
            }
            else
            {
                unblockedAction = action;
            }
        }

        if (!hasBinding && unblockedAction != null)
        {
            _invalidBinds.Remove(unblockedAction);
            unblockedAction.SetValidBind();
            unblockedAction.SaveActionBinding();
        }
    }

    public void OnConflict(string bindingPath)
    {
        foreach (RebindActionUI action in _keyBinds)
        {
            if (action.HasBinding(bindingPath))
            {
                _invalidBinds.Add(action);
                action.SetInvalidBind();
            }
        }

        if (_bindConflictPopupInstantiated != null)
        {
            _bindConflictPopupInstantiated.Close();
        }

        _bindConflictPopupInstantiated = Instantiate(_bindConflictPopup, transform);
    }

    public void ResetBinding()
    {
        foreach (RebindActionUI action in _keyBinds)
        {
            action.ResetToDefault();
        }
    }
}
