using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialVideoPlayer : Popup
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private RawImage _videoPlayerProjection;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _apologyText;

    private TutorialStorage _tutorial;
    private int _currentTextIndex = 0;

    public void Init(TutorialStorage tutorial)
    {
        _tutorial = tutorial;

        _title.text = _tutorial.Title;

        if (_tutorial.Replayable)
        {
            _description.text = _tutorial.Strings[_currentTextIndex];
            _videoPlayer.clip = _tutorial.VideoClip;
        }
        else
        {
            _description.gameObject.SetActive(false);
            _videoPlayerProjection.gameObject.SetActive(false);
            _apologyText.gameObject.SetActive(true);
        }
    }
}
