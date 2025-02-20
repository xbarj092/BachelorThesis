using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class TutorialMovementAction : TutorialAction
{
    [SerializeField] private InputActionReference _movementAction;

    private void OnDisable()
    {
        TutorialEvents.OnPlayerMoved -= OnPlayerMoved;
    }

    public override void StartAction()
    {
        _tutorialPlayer.MoveToNextNarratorText();

        ReadOnlyArray<InputBinding> bindings = _movementAction.action.bindings;
        List<string> bindingStrings = new();

        for (int i = 0; i < bindings.Count; i++)
        {
            if (bindings[i].isPartOfComposite)
            {
                continue;
            }

            bindingStrings.Add(_movementAction.action.GetBindingDisplayString(i));
        }

        _tutorialPlayer.PublicText.text = string.Format(_tutorialPlayer.PublicText.text, string.Join("", bindingStrings));
        TutorialEvents.OnPlayerMoved += OnPlayerMoved;
    }

    private void OnPlayerMoved()
    {
        TutorialEvents.OnPlayerMoved -= OnPlayerMoved;
        OnActionFinishedInvoke();
    }

    public override void Exit()
    {
    }
}
