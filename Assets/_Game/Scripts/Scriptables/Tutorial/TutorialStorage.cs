using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "TutorialStorage", menuName = "Game/TutorialStorage")]
public class TutorialStorage : ScriptableObject
{
    public string Title;
    public VideoClip VideoClip;
    public List<string> Strings;
}
