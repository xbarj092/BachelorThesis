using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindActionUI : MonoBehaviour
{
    [SerializeField] private Image _image; 
    [SerializeField] private InputActionReference _action;
    [SerializeField] private string _bindingId;
    [SerializeField] private InputBinding.DisplayStringOptions _displayStringOptions;
    [SerializeField] private TMP_Text _bindingText;
    [SerializeField] private UpdateBindingUIEvent _updateBindingUIEvent;
    [SerializeField] private InteractiveRebindEvent _rebindStartEvent;
    [SerializeField] private InteractiveRebindEvent _rebindStopEvent;

    public InputActionRebindingExtensions.RebindingOperation RebindOperation;

    private static List<RebindActionUI> _rebindActionUIs = null;

    public InputActionReference ActionReference
    {
        get => _action;
        set
        {
            _action = value;
            UpdateBindingDisplay();
        }
    }

    public InputBinding.DisplayStringOptions DisplayStringOptions
    {
        get => _displayStringOptions;
        set
        {
            _displayStringOptions = value;
            UpdateBindingDisplay();
        }
    }

    public event Action<InputActionRebindingExtensions.RebindingOperation> OnRebindStart;
    public event Action<RebindActionUI> OnRebind;
    public event Action<string> OnConflict;

    private void Start()
    {
        LoadActionBinding();
    }

    protected void OnEnable()
    {
        if (_rebindActionUIs == null)
        {
            _rebindActionUIs = new List<RebindActionUI>
            {
                this
            };
        }

        if (_rebindActionUIs.Count == 1)
        {
            InputSystem.onActionChange += OnActionChange;
        }
    }

    protected void OnDisable()
    {
        RebindOperation?.Dispose();
        RebindOperation = null;

        if (_rebindActionUIs != null)
        {
            _rebindActionUIs.Remove(this);
            if (_rebindActionUIs.Count == 0)
            {
                _rebindActionUIs = null;
                InputSystem.onActionChange -= OnActionChange;
            }
        }
    }

#if UNITY_EDITOR
    protected void OnValidate()
    {
        UpdateBindingDisplay();
    }
#endif

    public bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
    {
        bindingIndex = -1;

        action = _action?.action;
        if (action == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(_bindingId))
        {
            return false;
        }

        Guid bindingId = new(_bindingId);
        bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);
        if (bindingIndex == -1)
        {
            Debug.LogError($"Cannot find binding with ID '{bindingId}' on '{action}'", this);
            return false;
        }

        return true;
    }

    public void UpdateBindingDisplay()
    {
        string displayString = string.Empty;
        string deviceLayoutName = default;
        string controlPath = default;

        InputAction action = _action?.action;
        if (action != null)
        {
            int bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == _bindingId);
            if (bindingIndex != -1)
            {
                displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, DisplayStringOptions);
            }
        }

        if (_bindingText != null)
        {
            _bindingText.text = displayString;
        }

        _updateBindingUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);
    }

    public void ResetToDefault()
    {
        if (!ResolveActionAndBinding(out InputAction action, out int bindingIndex))
        {
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
            {
                action.RemoveBindingOverride(i);
            }
        }
        else
        {
            action.RemoveBindingOverride(bindingIndex);
        }

        SetValidBind();
        UpdateBindingDisplay();
    }

    public void StartInteractiveRebind()
    {
        _action.action.Disable();
        if (!ResolveActionAndBinding(out var action, out var bindingIndex))
        {
            return;
        }

        if (action.bindings[bindingIndex].isComposite)
        {
            int firstPartIndex = bindingIndex + 1;
            if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
            {
                PerformInteractiveRebind(action, firstPartIndex, allCompositeParts: true);
            }
        }
        else
        {
            PerformInteractiveRebind(action, bindingIndex);
        }
    }

    private void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
    {
        void CleanUp()
        {
            RebindOperation?.Dispose();
            RebindOperation = null;
            _action.action.Enable();
        }

        RebindOperation?.Cancel();

        RebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .OnCancel(
                operation =>
                {
                    _rebindStopEvent?.Invoke(this, operation);
                    UpdateBindingDisplay();
                    CleanUp();
                })
            .OnComplete(
                operation =>
                {
                    InputBinding newBinding = action.bindings[bindingIndex];
                    if (IsBindingConflict(action, newBinding))
                    {
                        Debug.LogWarning("Binding conflict detected. Reverting to previous binding.");
                        operation.Cancel();
                        SetInvalidBind();
                        OnRebind?.Invoke(this);
                        return;
                    }

                    _rebindStopEvent?.Invoke(this, operation);
                    UpdateBindingDisplay();
                    CleanUp(); 
                    SetValidBind();

                    if (allCompositeParts)
                    {
                        int nextBindingIndex = bindingIndex + 1;
                        if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                        {
                            PerformInteractiveRebind(action, nextBindingIndex, true);
                        }
                    }

                    OnRebind?.Invoke(this);
                });

        OnRebindStart?.Invoke(RebindOperation);

        string partName = default;
        if (action.bindings[bindingIndex].isPartOfComposite)
        {
            partName = $"Binding '{action.bindings[bindingIndex].name}'. ";
        }

        _bindingText.text = "<Waiting...>";

        _rebindStartEvent?.Invoke(this, RebindOperation);

        RebindOperation.Start();
    }

    private bool IsBindingConflict(InputAction action, InputBinding newBinding)
    {
        foreach (InputAction otherAction in action.actionMap.actions)
        {
            foreach (InputBinding binding in otherAction.bindings)
            {
                if (otherAction == action && binding.id == newBinding.id)
                {
                    continue;
                }

                if (binding.effectivePath == newBinding.effectivePath)
                {
                    OnConflict?.Invoke(binding.effectivePath);
                    Debug.LogWarning($"Binding conflict: {newBinding.effectivePath} is already used by {otherAction.name}");
                    return true;
                }
            }
        }

        return false;
    }

    public bool HasBinding(string bindingPath)
    {
        if (!ResolveActionAndBinding(out InputAction action, out int bindingIndex))
        {
            return false;
        }

        if (action.bindings[bindingIndex].effectivePath == bindingPath)
        {
            return true;
        }

        return false;
    }

    public void SetInvalidBind()
    {
        _image.color = Color.red;
        _bindingText.color = Color.white;
    }

    public void SetValidBind()
    {
        _image.color = Color.white;
        _bindingText.color = Color.black;
    }

    private static void OnActionChange(object obj, InputActionChange change)
    {
        if (change != InputActionChange.BoundControlsChanged)
        {
            return;
        }

        InputAction action = obj as InputAction;
        InputActionMap actionMap = action?.actionMap ?? obj as InputActionMap;
        InputActionAsset actionAsset = actionMap?.asset ?? obj as InputActionAsset;

        for (int i = 0; i < _rebindActionUIs.Count; ++i)
        {
            RebindActionUI component = _rebindActionUIs[i];
            InputAction referencedAction = component.ActionReference?.action;
            if (referencedAction == null)
            {
                continue;
            }

            if (referencedAction == action || referencedAction.actionMap == actionMap || referencedAction.actionMap?.asset == actionAsset)
            {
                component.UpdateBindingDisplay();
            }
        }
    }

    public void SaveActionBinding()
    {
        string currentBindings = ActionReference.action.actionMap.SaveBindingOverridesAsJson();
        LocalDataStorage.Instance.PlayerPrefs.SaveActionBinding(currentBindings);
    }

    private void LoadActionBinding()
    {
        string savedBindings = LocalDataStorage.Instance.PlayerPrefs.LoadActionBinding();
        if (!string.IsNullOrEmpty(savedBindings))
        {
            ActionReference.action.actionMap.LoadBindingOverridesFromJson(savedBindings);
            UpdateBindingDisplay();
        }
    }
}
