using System;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class TutorialVideoPlayer : Popup
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _title;

    private TutorialStorage _tutorial;
    private int _currentTextIndex = 0;

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

    public void Init(TutorialStorage tutorial)
    {
        _tutorial = tutorial;

        _title.text = _tutorial.Title;
        _description.text = _tutorial.Strings[_currentTextIndex];
        _videoPlayer.clip = _tutorial.VideoClip;

        CurrentMouseClickAction = ShowNextText;
    }

    private void ShowNextText()
    {
        if (_tutorial.Strings.Count > _currentTextIndex + 1)
        {
            _currentTextIndex++;
            _description.text = _tutorial.Strings[_currentTextIndex];
        }
    }
}
