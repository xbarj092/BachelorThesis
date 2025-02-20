using System;
using UnityEngine;

public class TutorialStatusEffect : TutorialAction
{
    [SerializeField] private GameObject _statusEffectCutout;
    [SerializeField] private GameObject _background;

    private event Action CurrentMouseClickAction;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CurrentMouseClickAction?.Invoke();
        }
    }

    private void OnDisable()
    {
        CurrentMouseClickAction = null;
    }

    public override void StartAction()
    {
        _background.SetActive(true);
        _tutorialPlayer.MoveToNextNarratorText();
        CurrentMouseClickAction = OnAfterStart;
    }

    private void OnAfterStart()
    {
        CurrentMouseClickAction = null;
        _statusEffectCutout.SetActive(true);
        _tutorialPlayer.MoveToNextNarratorText();
        CurrentMouseClickAction = OnAfterStatusEffectHighlight;
    }

    private void OnAfterStatusEffectHighlight()
    {
        CurrentMouseClickAction = null;
        _tutorialPlayer.MoveToNextNarratorText();
        CurrentMouseClickAction = OnAfterVisualShown;
    }

    private void OnAfterVisualShown()
    {
        CurrentMouseClickAction = null;
        OnActionFinishedInvoke();
    }

    public override void Exit()
    {
        TutorialManager.Instance.IsPaused = false;
    }
}
