using TMPro;
using UnityEngine;

public class ReplayTutorialScreen : BaseScreen
{
    [SerializeField] private TutorialSlot _tutorialSlotPrefab;
    [SerializeField] private RectTransform _spawnTransform;
    [SerializeField] private TMP_Text _noTutorials;

    private Vector3 _defaultSize;

    private void Start()
    {
        _defaultSize = _spawnTransform.sizeDelta;
        SetUpCompletedTutorials();
    }

    private void SetUpCompletedTutorials()
    {
        _noTutorials.gameObject.SetActive(TutorialManager.Instance.CompletedTutorials.Count == 0);
        float prefabHeight = _tutorialSlotPrefab.GetComponent<RectTransform>().sizeDelta.y;
        float spacing = 50f;

        foreach (TutorialPlayer tutorial in TutorialManager.Instance.CompletedTutorials)
        {
            TutorialSlot tutorialSlot = Instantiate(_tutorialSlotPrefab, _spawnTransform);
            tutorialSlot.Init(tutorial.TutorialStorage, transform);
        }

        AdjustContentHeight(prefabHeight, spacing, TutorialManager.Instance.CompletedTutorials.Count);
    }

    private void AdjustContentHeight(float prefabHeight, float spacing, int itemCount)
    {
        if (itemCount == 0)
        {
            return;
        }

        float totalHeight = (prefabHeight + spacing) * itemCount + 50;
        Vector2 newSize = _spawnTransform.sizeDelta;
        newSize.y = Mathf.Max(totalHeight, _spawnTransform.sizeDelta.y);
        _spawnTransform.sizeDelta = newSize;
    }
}
