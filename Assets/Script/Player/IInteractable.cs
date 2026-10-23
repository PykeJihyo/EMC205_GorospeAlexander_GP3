// Anything the player can click on in the 3D world (customers, thieves, shapes).
// PlayerInput doesn't need to know WHAT it clicked, it just calls OnClicked().
public interface IInteractable
{
    void OnClicked();
}
