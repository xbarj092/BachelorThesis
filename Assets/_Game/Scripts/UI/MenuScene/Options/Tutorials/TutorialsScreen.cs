using UnityEngine;
using TMPro;

public class TutorialsScreen : BaseScreen
{
    [SerializeField] private TMP_Text _enabledText;

    private const string ENABLED_TEXT_PREFIX = "ENABLED: ";
    private const string ENABLED_TEXT_ON = "YES";
    private const string ENABLED_TEXT_OFF = "NO";

    private void Start()
    {
        UpdateText();
    }

    public void ReplayTutorials()
    {
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.ReplayTutorial);
        Destroy(gameObject);
    }

    public void ToggleTutorials()
    {
        TutorialManager.Instance.ToggleTutorials();
        UpdateText();
    }

    public void ResetTutorials()
    {
        TutorialManager.Instance.ResetTutorials();
        UpdateText();
    }

    private void UpdateText()
    {
        _enabledText.text = ENABLED_TEXT_PREFIX + (TutorialManager.Instance.TutorialsEnabled ? ENABLED_TEXT_ON : ENABLED_TEXT_OFF);
    }
}
