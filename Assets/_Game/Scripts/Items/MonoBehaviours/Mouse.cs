public class Mouse : Item
{
    public override bool IsInteractable => base.IsInteractable && !Used;

    public override void UseStart()
    {
        base.UseStart();
        Unhighlight();
    }
}
