using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyBindingsScreen : BaseScreen
{
    [SerializeField] private List<RebindActionUI> _keyBinds = new();
    [SerializeField] private Popup _bindConflictPopup;
    [SerializeField] private GameObject _raycastPreventer;

    private InputActionRebindingExtensions.RebindingOperation _actionRebinding;
    private bool _isRebinding = false;

    private List<RebindActionUI> _invalidBinds = new();
    private Popup _bindConflictPopupInstantiated;

    private void OnEnable()
    {
        foreach (RebindActionUI action in _keyBinds)
        {
            action.OnRebindStart += OnRebindStart;
            action.OnRebind += OnRebind;
            action.OnConflict += OnConflict;
        }
    }

    private void OnDisable()
    {
        foreach (RebindActionUI action in _keyBinds)
        {
            action.OnRebindStart -= OnRebindStart;
            action.OnRebind -= OnRebind;
            action.OnConflict -= OnConflict;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (_isRebinding && Input.GetKeyDown(KeyCode.Escape))
        {
            _raycastPreventer.SetActive(false);
            _isRebinding = false;
            _actionRebinding?.Cancel();
        }
    }

    protected override bool CanClose()
    {
        return !_isRebinding && base.CanClose();
    }

    private void OnRebindStart(InputActionRebindingExtensions.RebindingOperation operation)
    {
        _raycastPreventer.SetActive(true);
        _isRebinding = true;
        _actionRebinding = operation;
    }

    private void OnRebind(RebindActionUI rebindActionUI)
    {
        _raycastPreventer.SetActive(false);
        _isRebinding = false;
        _actionRebinding = null;

        rebindActionUI.ResolveActionAndBinding(out InputAction inputAction, out int bindingIndex);
        ValidateAllBindings();

        if (_invalidBinds.Count == 0)
        {
            rebindActionUI.SaveActionBinding();
        }
    }

    private void ValidateAllBindings()
    {
        _invalidBinds.Clear();

        Dictionary<RebindActionUI, string> actionPaths = new();
        foreach (RebindActionUI actionUI in _keyBinds)
        {
            actionUI.ResolveActionAndBinding(out InputAction action, out int bindingIndex);
            string effectivePath = action.bindings[bindingIndex].effectivePath;

            if (!string.IsNullOrEmpty(effectivePath))
            {
                actionPaths[actionUI] = effectivePath;
            }
        }

        foreach (KeyValuePair<RebindActionUI, string> actionA in actionPaths)
        {
            foreach (KeyValuePair<RebindActionUI, string> actionB in actionPaths)
            {
                if (actionA.Key == actionB.Key)
                { 
                    continue;
                }

                if (actionA.Value == actionB.Value)
                {
                    if (!_invalidBinds.Contains(actionA.Key))
                    {
                        _invalidBinds.Add(actionA.Key);
                        actionA.Key.SetInvalidBind();
                    }

                    if (!_invalidBinds.Contains(actionB.Key))
                    {
                        _invalidBinds.Add(actionB.Key);
                        actionB.Key.SetInvalidBind();
                    }
                }
            }
        }

        foreach (RebindActionUI actionUI in _keyBinds)
        {
            if (!_invalidBinds.Contains(actionUI))
            {
                actionUI.SetValidBind();
            }
        }
    }

    public void OnConflict(string bindingPath)
    {
        foreach (RebindActionUI action in _keyBinds)
        {
            if (action.HasBinding(bindingPath))
            {
                if (!_invalidBinds.Contains(action))
                {
                    Debug.Log("[KeyBindingsScreen] - adding conflict: " + bindingPath);
                    _invalidBinds.Add(action);
                    action.SetInvalidBind();
                }
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

        _keyBinds.FirstOrDefault().SaveActionBinding();
    }
}
