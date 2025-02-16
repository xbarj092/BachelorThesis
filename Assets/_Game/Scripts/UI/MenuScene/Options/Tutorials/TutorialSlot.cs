using TMPro;
using UnityEngine;

public class TutorialSlot : MonoBehaviour
{
    [SerializeField] private TutorialVideoPlayer _tutorialVideoPlayer;
    [SerializeField] private TMP_Text _tutorialTitle;

    private TutorialStorage _tutorial;
    private Transform _spawnTransform;

    public void Init(TutorialStorage tutorial, Transform spawnTransform)
    {
        _tutorial = tutorial;
        _spawnTransform = spawnTransform;

        _tutorialTitle.text = tutorial.Title;
        // image
    }

    public void ShowTutorialVideo()
    {
        TutorialVideoPlayer tutorialPlayer = Instantiate(_tutorialVideoPlayer, _spawnTransform);
        tutorialPlayer.Init(_tutorial);
    }
}
