using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGeneralInfoAction : TutorialAction
{
    [SerializeField] private Image _background;
    [SerializeField] private GameObject _continueButton;

    [Space(5)]
    [SerializeField] private Image _hungerMeterCutout;
    [SerializeField] private Image _timeAliveCutout;

    [Space(5)]
    [SerializeField] private RectTransform _hungerMeterTransform;
    [SerializeField] private RectTransform _timeAliveTransform;

    private event Action _currentMouseClickAction;
    private event Action _nextAction;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _currentMouseClickAction?.Invoke();
        }
    }

    private void OnDisable()
    {
        _currentMouseClickAction = null;
        StopAllCoroutines();
    }

    public override void StartAction()
    {
        Time.timeScale = 0f;
        _background.gameObject.SetActive(true);
        _hungerMeterCutout.gameObject.SetActive(true);
        _tutorialPlayer.SetTextTransform(_hungerMeterTransform);
        _tutorialPlayer.MoveToNextNarratorText();
        StartCoroutine(DelayedClickToContinue());
        _currentMouseClickAction = WaitSkip;
        _nextAction = OnAfterHungerMeter;
    }

    private void OnAfterHungerMeter()
    {
        _hungerMeterCutout.gameObject.SetActive(false);
        _timeAliveCutout.gameObject.SetActive(true);
        _tutorialPlayer.SetTextTransform(_timeAliveTransform);
        _tutorialPlayer.MoveToNextNarratorText();
        StartCoroutine(DelayedClickToContinue());
        _currentMouseClickAction = WaitSkip;
        _nextAction = OnAfterTimeAlive;
    }

    private void OnAfterTimeAlive()
    {
        _currentMouseClickAction = null;
        _background.gameObject.SetActive(false);
        _timeAliveCutout.gameObject.SetActive(false);
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
        yield return new WaitForSecondsRealtime(2);
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
        Time.timeScale = 1f;
        TutorialManager.Instance.InstantiateTutorial(TutorialID.Movement);
    }
}
