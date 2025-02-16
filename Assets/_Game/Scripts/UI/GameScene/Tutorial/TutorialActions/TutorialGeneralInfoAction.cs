using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGeneralInfoAction : TutorialAction
{
    [SerializeField] private Image _background;

    [SerializeField] private Image _hungerMeterCutout;
    [SerializeField] private Image _timeAliveCutout;

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
        Time.timeScale = 0f;
        _background.gameObject.SetActive(true);
        _hungerMeterCutout.gameObject.SetActive(true);
        _tutorialPlayer.MoveToNextNarratorText();
        CurrentMouseClickAction = OnAfterHungerMeter;
    }

    private void OnAfterHungerMeter()
    {
        _hungerMeterCutout.gameObject.SetActive(false);
        _timeAliveCutout.gameObject.SetActive(true);
        _tutorialPlayer.MoveToNextNarratorText();
        CurrentMouseClickAction = OnAfterTimeAlive;
    }

    private void OnAfterTimeAlive()
    {
        CurrentMouseClickAction = null;
        _background.gameObject.SetActive(false);
        _timeAliveCutout.gameObject.SetActive(false);
        OnActionFinishedInvoke();
    }

    public override void Exit()
    {
        Time.timeScale = 1f;
        TutorialManager.Instance.InstantiateTutorial(TutorialID.Movement);
    }
}
