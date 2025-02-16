using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    [field: SerializeField] public bool TutorialsEnabled { get; private set; } = true;
    [field: SerializeField] public List<TutorialPlayer> TutorialList { get; private set; }

    [HideInInspector] public List<TutorialID> CompletedTutorialIDs = new();
    [HideInInspector] public List<TutorialPlayer> CompletedTutorials => TutorialList.Where(tutorial => CompletedTutorialIDs.Contains(tutorial.TutorialID)).ToList();

    public TutorialPlayer CurrentTutorial { get; private set; }
    [HideInInspector] public UseableItem CurrentItemInRange;
    [HideInInspector] public Kitten CurrentKittenInRange;
    [HideInInspector] public UseableItem CurrentItemToUse;
    public bool IsPaused = false;
    public bool CanUseItem = true;

    public event Action<TutorialID> OnTutorialEnd;

    private void Awake() 
    {
        TutorialSettings tutorialSettings = LocalDataStorage.Instance.PlayerPrefs.LoadCompletedTutorials();
        if (tutorialSettings != null)
        {
            if (tutorialSettings.CompletedTutorials != null && tutorialSettings.CompletedTutorials.Count > 0)
            {
                CompletedTutorialIDs = tutorialSettings.CompletedTutorials.Select(tutorial => (TutorialID)tutorial).ToList();
            }

            TutorialsEnabled = tutorialSettings.TutorialEnabled;
        }
    }

    protected override void Init() { }

    public void InstantiateTutorial(ItemType itemType, bool allowMultipleTutorialsAtOnce = false)
    {
        TutorialID tutorialId = GetTutorialIDFromItemType(itemType);
        InstantiateTutorial(tutorialId, allowMultipleTutorialsAtOnce);
    }

    private TutorialID GetTutorialIDFromItemType(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Clothespin => TutorialID.ClothespinUse,
            ItemType.CastrationKit => TutorialID.CastrationKitUse,
            ItemType.CardboardBox => TutorialID.CardboardBoxUse,
            ItemType.Laser => TutorialID.LaserUse,
            ItemType.Mouse => TutorialID.MouseUse,
            ItemType.Towel => TutorialID.TowelUse,
            _ => default
        };
    }

    public void InstantiateTutorial(ConsumableType consumableType, bool allowMultipleTutorialsAtOnce = false)
    {
        TutorialID tutorialId = GetTutorialIDFromConsumableType(consumableType);
        InstantiateTutorial(tutorialId, allowMultipleTutorialsAtOnce);
    }

    private TutorialID GetTutorialIDFromConsumableType(ConsumableType consumableType)
    {
        return consumableType switch
        {
            ConsumableType.Steak or ConsumableType.Fish => TutorialID.FoodPickup,
            ConsumableType.InvisibilityPotion => TutorialID.InvisibilityPotionPickup,
            _ => default
        };
    }

    public void InstantiateTutorial(TutorialID tutorialID, bool allowMultipleTutorialsAtOnce = false)
    {
        if (!TutorialsEnabled)
        {
            return;
        }

        if (!allowMultipleTutorialsAtOnce)
        {
            if (CurrentTutorial != null)
            {
                Debug.LogError($"[TutorialManager] - Trying to spawn tutorial {tutorialID}, but there is already tutorial {CurrentTutorial.TutorialID} playing. Returning");
                return;
            }
        }
        
        foreach (TutorialPlayer tutorial in TutorialList)
        {
            if (tutorial.TutorialID == tutorialID)
            {
                SpawnTutorial(tutorial);
                return;
            }
        }

        Debug.LogError($"[TutorialManager] - Cannot spawn tutorial {tutorialID}! Check if it exists!");
    }

    public void SkipCurrentTutorial()
    {
        if (CurrentTutorial != null)
        {
            Destroy(CurrentTutorial.gameObject);
            CurrentTutorial.Action.OnActionFinishedInvoke();
        }
    }

    private void SpawnTutorial(TutorialPlayer tutorial)
    {
        CurrentTutorial = Instantiate(tutorial, FindObjectOfType<BaseCanvasController>().transform);
        if (CurrentTutorial == null)
        {
            Debug.LogError($"[TutorialManager] - Cannot instantiate tutorial {tutorial.TutorialID}! Check if it exists!");
        }
        else
        {
            CurrentTutorial.OnTutorialEnd += OnCurrentTutorialEnd;
        }
    }

    private void OnCurrentTutorialEnd(TutorialID tutorialID)
    {
        if (CurrentTutorial != null)
        {
            CurrentTutorial.OnTutorialEnd -= OnCurrentTutorialEnd;
            CurrentTutorial = null;
        }

        CompletedTutorialIDs.Add(tutorialID);
        List<int> tutorialInts = CompletedTutorialIDs.Select(tutorials => (int)tutorials).ToList();
        LocalDataStorage.Instance.PlayerPrefs.SaveTutorialSettings(new(tutorialInts, TutorialsEnabled));
        OnTutorialEnd?.Invoke(tutorialID);
    }

    public void ToggleTutorials()
    {
        TutorialsEnabled = !TutorialsEnabled;
        List<int> tutorialInts = CompletedTutorialIDs.Select(tutorial => (int)tutorial).ToList();
        LocalDataStorage.Instance.PlayerPrefs.SaveTutorialSettings(new(tutorialInts, TutorialsEnabled));
    }

    public void ResetTutorials()
    {
        CompletedTutorialIDs.Clear();
        CompletedTutorials.Clear();
        TutorialsEnabled = true;

        List<int> tutorialInts = CompletedTutorialIDs.Select(tutorial => (int)tutorial).ToList();
        LocalDataStorage.Instance.PlayerPrefs.SaveTutorialSettings(new(tutorialInts, TutorialsEnabled));
    }

    public bool IsTutorialPlaying(TutorialID tutorialID)
    {
        if (CurrentTutorial == null)
        {
            return false;
        }

        return CurrentTutorial.TutorialID == tutorialID;
    }

    public bool IsTutorialPlaying() => CurrentTutorial != null;
    public bool IsTutorialCompleted(TutorialID tutorialID) => CompletedTutorialIDs.Contains(tutorialID);
}
