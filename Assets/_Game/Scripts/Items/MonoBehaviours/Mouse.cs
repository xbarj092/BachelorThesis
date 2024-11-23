public class Mouse : Item
{
    public override bool IsInteractable => base.IsInteractable && !Used;
}
