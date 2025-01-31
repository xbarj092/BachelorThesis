using System;
using UnityEngine.Events;

[Serializable]
public class UpdateBindingUIEvent : UnityEvent<RebindActionUI, string, string, string>
{
}
