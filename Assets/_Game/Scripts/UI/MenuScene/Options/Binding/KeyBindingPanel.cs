using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindingPanel : OptionPanel
{
    [SerializeField] private List<RebindActionUI> _keyBinds = new();

    private List<RebindActionUI> _invalidBinds = new();

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
        /*foreach (RebindActionUI action in _keyBinds)
        {
            if (!action.HasBinding(rebindActionUI.))
            {
                _invalidBinds.Add(action);
                action.SetInvalidBind();
            }
        }*/
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
    }
}
