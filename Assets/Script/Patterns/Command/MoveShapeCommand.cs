// Clicking a shape moves it between the Inventory Table (behind you) and the Display Table.
public class MoveShapeCommand : ICommand
{
    private readonly ShapeItem shape;
    private readonly InventoryManager inventory;

    public MoveShapeCommand(ShapeItem shape, InventoryManager inventory)
    {
        this.shape = shape;
        this.inventory = inventory;
    }

    public void Execute()
    {
        if (!inventory.TryMove(shape))
        {
            string full = shape.IsOnDisplay ? "The Inventory Table" : "The Display Table";
            GameManager.Instance.ui.ShowMessage($"{full} is full.");
        }
    }
}
