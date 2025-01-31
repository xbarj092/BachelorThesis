using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem.Utilities;

[CustomEditor(typeof(RebindActionUI))]
public class RebindActionUIEditor : Editor
{
    private SerializedProperty _imageProperty;
    private SerializedProperty _actionProperty;
    private SerializedProperty _bindingIdProperty;
    private SerializedProperty _bindingTextProperty;
    private SerializedProperty _rebindStartEventProperty;
    private SerializedProperty _rebindStopEventProperty;
    private SerializedProperty _updateBindingUIEventProperty;
    private SerializedProperty _displayStringOptionsProperty;

    private GUIContent _bindingLabel = new GUIContent("Binding");
    private GUIContent _displayOptionsLabel = new GUIContent("Display Options");
    private GUIContent _uILabel = new GUIContent("UI");
    private GUIContent _eventsLabel = new GUIContent("Events");
    private GUIContent[] _bindingOptions;
    private string[] _bindingOptionValues;
    private int _selectedBindingOption;

    protected void OnEnable()
    {
        _imageProperty = serializedObject.FindProperty("_image");
        _actionProperty = serializedObject.FindProperty("_action");
        _bindingIdProperty = serializedObject.FindProperty("_bindingId");
        _bindingTextProperty = serializedObject.FindProperty("_bindingText");
        _updateBindingUIEventProperty = serializedObject.FindProperty("_updateBindingUIEvent");
        _rebindStartEventProperty = serializedObject.FindProperty("_rebindStartEvent");
        _rebindStopEventProperty = serializedObject.FindProperty("_rebindStopEvent");
        _displayStringOptionsProperty = serializedObject.FindProperty("_displayStringOptions");

        RefreshBindingOptions();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField(_bindingLabel, Styles.boldLabel);
        using (new EditorGUI.IndentLevelScope())
        {
            EditorGUILayout.PropertyField(_actionProperty);

            int newSelectedBinding = EditorGUILayout.Popup(_bindingLabel, _selectedBindingOption, _bindingOptions);
            if (newSelectedBinding != _selectedBindingOption)
            {
                string bindingId = _bindingOptionValues[newSelectedBinding];
                _bindingIdProperty.stringValue = bindingId;
                _selectedBindingOption = newSelectedBinding;
            }

            InputBinding.DisplayStringOptions optionsOld = (InputBinding.DisplayStringOptions)_displayStringOptionsProperty.intValue;
            InputBinding.DisplayStringOptions optionsNew = (InputBinding.DisplayStringOptions)EditorGUILayout.EnumFlagsField(_displayOptionsLabel, optionsOld);
            if (optionsOld != optionsNew)
            {
                _displayStringOptionsProperty.intValue = (int)optionsNew;
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(_uILabel, Styles.boldLabel);
        using (new EditorGUI.IndentLevelScope())
        {
            EditorGUILayout.PropertyField(_bindingTextProperty);
            EditorGUILayout.PropertyField(_imageProperty);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(_eventsLabel, Styles.boldLabel);
        using (new EditorGUI.IndentLevelScope())
        {
            EditorGUILayout.PropertyField(_rebindStartEventProperty);
            EditorGUILayout.PropertyField(_rebindStopEventProperty);
            EditorGUILayout.PropertyField(_updateBindingUIEventProperty);
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            RefreshBindingOptions();
        }
    }

    protected void RefreshBindingOptions()
    {
        InputActionReference actionReference = (InputActionReference)_actionProperty.objectReferenceValue;
        InputAction action = actionReference?.action;

        if (action == null)
        {
            _bindingOptions = new GUIContent[0];
            _bindingOptionValues = new string[0];
            _selectedBindingOption = -1;
            return;
        }

        ReadOnlyArray<InputBinding> bindings = action.bindings;
        int bindingCount = bindings.Count;

        _bindingOptions = new GUIContent[bindingCount];
        _bindingOptionValues = new string[bindingCount];
        _selectedBindingOption = -1;

        string currentBindingId = _bindingIdProperty.stringValue;
        for (int i = 0; i < bindingCount; ++i)
        {
            InputBinding binding = bindings[i];
            string bindingId = binding.id.ToString();
            bool haveBindingGroups = !string.IsNullOrEmpty(binding.groups);

            InputBinding.DisplayStringOptions displayOptions = InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.IgnoreBindingOverrides;
            if (!haveBindingGroups)
            {
                displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;
            }

            string displayString = action.GetBindingDisplayString(i, displayOptions);

            if (binding.isPartOfComposite)
            {
                displayString = $"{ObjectNames.NicifyVariableName(binding.name)}: {displayString}";
            }

            displayString = displayString.Replace('/', '\\');

            if (haveBindingGroups)
            {
                InputActionAsset asset = action.actionMap?.asset;
                if (asset != null)
                {
                    string controlSchemes = string.Join(", ", binding.groups.Split(InputBinding.Separator).Select(x => asset.controlSchemes.FirstOrDefault(c => c.bindingGroup == x).name));
                    displayString = $"{displayString} ({controlSchemes})";
                }
            }

            _bindingOptions[i] = new GUIContent(displayString);
            _bindingOptionValues[i] = bindingId;

            if (currentBindingId == bindingId)
                _selectedBindingOption = i;
        }
    }

    private static class Styles
    {
        public static GUIStyle boldLabel = new GUIStyle("MiniBoldLabel");
    }
}
