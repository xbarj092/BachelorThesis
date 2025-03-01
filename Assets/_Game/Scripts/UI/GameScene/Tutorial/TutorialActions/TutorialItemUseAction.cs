using System;
using System.Collections;
using UnityEngine;

public class TutorialItemUseAction : TutorialAction
{
    [SerializeField] private TutorialVideoPlayer _player;
    [SerializeField] private GameObject _continueButton;

    private event Action CurrentMouseClickAction;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && ScreenManager.Instance.ActiveGameScreen == null)
        {
            CurrentMouseClickAction?.Invoke();
        }
    }

    private void OnDisable()
    {
        CurrentMouseClickAction = null;
        TutorialManager.Instance.IsPaused = false;
        TutorialManager.Instance.CanUseItem = true;
        TutorialManager.Instance.CanDropItem = true;
    }

    public override void StartAction()
    {
        AudioManager.Instance.Play(SoundType.TutorialPopup);
        TutorialManager.Instance.IsPaused = true;
        TutorialManager.Instance.CanUseItem = false;
        TutorialManager.Instance.CanDropItem = false;
        _continueButton.gameObject.SetActive(false);
        _player.Init(_tutorialPlayer.TutorialStorage);
        StartCoroutine(DelayedClickToContinue());
        CurrentMouseClickAction = WaitSkip;
    }

    private void WaitSkip()
    {
        OnAfterWaitTime();
        StopCoroutine(DelayedClickToContinue());
    }

    private IEnumerator DelayedClickToContinue()
    {
        yield return new WaitForSeconds(2);
        OnAfterWaitTime();
    }

    private void OnAfterWaitTime()
    {
        _continueButton.gameObject.SetActive(true);
        CurrentMouseClickAction = OnActionFinishedInvoke;
    }

    public override void Exit()
    {
        TutorialManager.Instance.IsPaused = false;
        TutorialManager.Instance.CanUseItem = true;
        TutorialManager.Instance.CanDropItem = true;
    }
}
