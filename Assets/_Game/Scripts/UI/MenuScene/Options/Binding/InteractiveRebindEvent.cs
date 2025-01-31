using System;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[Serializable]
public class InteractiveRebindEvent : UnityEvent<RebindActionUI, InputActionRebindingExtensions.RebindingOperation>
{
}
