using UnityEngine;

public class Thief : NPC, IInteractable
{
    public float fleeSpeedMultiplier = 1.3f;

    public bool IsBold { get; private set; } // bold thieves steal even while you're watching
    public ShapeItem StolenShape { get; private set; }

    private float normalSpeed;

    public void Setup(CustomerClass disguise, bool isBold, float speed)
    {
        IsBold = isBold;
        MoveSpeed = speed;
        normalSpeed = speed;

        // same name as a real customer of that class 
        name = $"{CustomerProfile.For(disguise).Label} Customer";
    }

    public override void OnArrivedAtSlot()
    {
        ChangeState(new ThiefLookoutState());
    }

    // Grabs a random shape from your Display Table
    public bool TrySteal()
    {
        InventoryManager inventory = GameManager.Instance.inventory;
        ShapeItem target = inventory.GetRandomDisplayedShape();
        if (target == null) return false;

        inventory.MarkStolen(target);
        StolenShape = target;

        // Carry it above its head: noticeable if you're watching, easy to miss if you're not.
        target.transform.SetParent(transform);
        target.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        target.transform.localScale = target.OriginalScale * 0.5f;

        ChangeState(new ThiefFleeState());
        return true;
    }

    public void StartFleeing()
    {
        MoveSpeed = normalSpeed * fleeSpeedMultiplier;
    }

    public void OnClicked()
    {
        if (CurrentState is ThiefFleeState) Catch();
    }

    private void Catch()
    {
        GameManager gm = GameManager.Instance;
        gm.inventory.ReturnStolen(StolenShape);
        StolenShape = null;
        gm.Stats.thievesCaught++;
        gm.ui.ShowMessage("Thief caught! Shape recovered.");

        Say("Busted...");
        MoveSpeed = normalSpeed;
        LeaveShop(); // a caught thief walks out empty-handed and can't be clicked again
    }

    // Got out the door with your shape
    public override void OnLeftShop()
    {
        if (StolenShape != null)
        {
            GameManager gm = GameManager.Instance;
            gm.Stats.shapesStolen++;
            gm.Stats.valueLostToThieves += StolenShape.GetTrueValue();
            gm.inventory.LoseStolen(StolenShape);
            StolenShape = null;
        }
        base.OnLeftShop();
    }
}