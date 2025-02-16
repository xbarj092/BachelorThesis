using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class TutorialVideoPlayer : Popup
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private TMP_Text _description;

    public void Init(TutorialStorage tutorial)
    {
        _description.text = tutorial.Strings[0];
        _videoPlayer.clip = tutorial.VideoClip;
    }
}
