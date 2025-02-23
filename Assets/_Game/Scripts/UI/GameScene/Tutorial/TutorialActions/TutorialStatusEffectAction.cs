using System;
using System.Collections;
using UnityEngine;

public class TutorialStatusEffect : TutorialAction
{
    [SerializeField] private GameObject _statusEffectCutout;
    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject _continueButton;

    private event Action _currentMouseClickAction;
    private event Action _nextAction;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && ScreenManager.Instance.ActiveGameScreen == null)
        {
            _currentMouseClickAction?.Invoke();
        }
    }

    private void OnDisable()
    {
        TutorialManager.Instance.IsPaused = false;
        _currentMouseClickAction = null;
        StopAllCoroutines();
    }

    public override void StartAction()
    {
        TutorialManager.Instance.IsPaused = true;
        _background.SetActive(true);
        _tutorialPlayer.MoveToNextNarratorText();
        StartCoroutine(DelayedClickToContinue());
        _currentMouseClickAction = WaitSkip;
        _nextAction = OnAfterStart;
    }

    private void OnAfterStart()
    {
        _currentMouseClickAction = null;
        _statusEffectCutout.SetActive(true);
        _tutorialPlayer.MoveToNextNarratorText();
        StartCoroutine(DelayedClickToContinue());
        _currentMouseClickAction = WaitSkip;
        _nextAction = OnAfterStatusEffectHighlight;
    }

    private void OnAfterStatusEffectHighlight()
    {
        _currentMouseClickAction = null;
        _tutorialPlayer.MoveToNextNarratorText();
        StartCoroutine(DelayedClickToContinue());
        _currentMouseClickAction = WaitSkip;
        _nextAction = OnAfterVisualShown;
    }

    private void OnAfterVisualShown()
    {
        _currentMouseClickAction = null;
        OnActionFinishedInvoke();
    }

    private void WaitSkip()
    {
        OnAfterWaitTime();
        StopCoroutine(DelayedClickToContinue());
    }

    private IEnumerator DelayedClickToContinue()
    {
        _continueButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        OnAfterWaitTime();
    }

    private void OnAfterWaitTime()
    {
        _continueButton.gameObject.SetActive(true);
        if (_nextAction != null)
        {
            _currentMouseClickAction = new(_nextAction);
            _nextAction = null;
        }
    }

    public override void Exit()
    {
        TutorialManager.Instance.IsPaused = false;
    }
}
