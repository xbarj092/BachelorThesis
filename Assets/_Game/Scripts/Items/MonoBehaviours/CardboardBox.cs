public class CardboardBox : Item
{
    public bool HasKitten;
    public override bool IsInteractable => !HasKitten && base.IsInteractable;
}
