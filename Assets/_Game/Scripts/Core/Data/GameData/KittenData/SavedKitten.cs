using System;

[Serializable]
public class SavedKitten
{
    public TransformData TransformData;
    public TransformData CurrentTarget;

    public Vector3Data MovementDirection;
    public Vector3Data LastPosition;

    public float TimeToLive;
    public float MatingTimeout;

    public int CurrentState;
    public int CurrentFocusType;

    public int UID;
    public int PotentialPartnerId;

    public bool Male;
    public bool IsCastrated;

    public bool IsDead;
    public bool IsInRangeOfPlayer;
    public bool CanSeeTarget;
    public bool IsApproaching;
    public bool IsMating;
    public bool AlreadyMated = true;
    public bool IsTrapped;
    public bool IsRunningAway;

    public bool Enabled;

    public SavedKitten(TransformData transformData, TransformData currentTarget, Vector3Data movementDirection, 
        Vector3Data lastPosition, float timeToLive, float matingTimeout, int Id, int potentialPartnerId, int currentState, int currentFocusType,
        bool male, bool isCastrated, bool isDead, bool isInRangeOfPlayer, bool canSeeTarget, bool isApproaching, 
        bool isMating, bool alreadyMated, bool isTrapped, bool isRunningAway, bool enabled)
    {
        TransformData = transformData;
        CurrentTarget = currentTarget;

        MovementDirection = movementDirection;
        LastPosition = lastPosition;

        TimeToLive = timeToLive;
        MatingTimeout = matingTimeout;

        CurrentState = currentState;
        CurrentFocusType = currentFocusType;

        UID = Id;
        PotentialPartnerId = potentialPartnerId;

        Male = male;
        IsCastrated = isCastrated;

        IsDead = isDead;
        IsInRangeOfPlayer = isInRangeOfPlayer;
        CanSeeTarget = canSeeTarget;
        IsApproaching = isApproaching;
        IsMating = isMating;
        AlreadyMated = alreadyMated;
        IsTrapped = isTrapped;
        IsRunningAway = isRunningAway;

        Enabled = enabled;
    }

    public void ApplyToKitten(Kitten kitten)
    {
        TransformData.ApplyToTransform(kitten.transform);

        kitten.UID = UID;
        kitten.PartnerUID = PotentialPartnerId;

        kitten.Male = Male;
        kitten.IsCastrated = IsCastrated;

        kitten.IsDead = IsDead;
        kitten.IsInRangeOfPlayer = IsInRangeOfPlayer;
        kitten.CanSeeTarget = CanSeeTarget;
        kitten.IsApproaching = IsApproaching;
        kitten.IsMating = IsMating;
        kitten.AlreadyMated = AlreadyMated;
        kitten.IsTrapped = IsTrapped;
        kitten.IsRunningAway = IsRunningAway;

        kitten.gameObject.SetActive(Enabled);

        kitten.Load(this);
    }
}
