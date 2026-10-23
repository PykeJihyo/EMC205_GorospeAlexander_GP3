using UnityEngine;

// The deal you're making with the customer you clicked.
// GameUI draws the window and calls Accept / Haggle / Refuse.
public class HaggleSystem
{
    public Customer Customer { get; private set; }
    public bool IsOpen => Customer != null;
    public string Reply { get; private set; } // what the customer just said

    public void Open(Customer customer)
    {
        GameManager gm = GameManager.Instance;
        if (IsOpen || gm.Phase != GamePhase.ShopOpen) return;

        if (customer.Intent == VisitIntent.Buy && !customer.WantedShapeStillAvailable())
        {
            customer.Say("Aw, it's gone.");
            customer.LeaveShop();
            return;
        }

        Customer = customer;
        Reply = customer.Intent == VisitIntent.Buy ? "I'd like to buy this one." : "Want to buy this off me?";
    }

    // Can the player accept right now? If not, 'reason' says why (shown on the button).
    public bool CanAccept(out string reason)
    {
        reason = "";
        if (!IsOpen) return false;

        if (Customer.Intent == VisitIntent.Buy)
        {
            if (!Customer.WantedShapeStillAvailable()) { reason = "That shape is gone!"; return false; }
            return true;
        }

        GameManager gm = GameManager.Instance;
        if (gm.Money < Customer.CurrentPrice) { reason = "Not enough money"; return false; }
        if (!gm.inventory.HasFreeSlot) { reason = "Inventory full"; return false; }
        return true;
    }

    public void Accept()
    {
        string reason;
        if (!CanAccept(out reason)) return;

        GameManager gm = GameManager.Instance;
        DayStats stats = gm.Stats;
        float price = Customer.CurrentPrice;

        if (Customer.Intent == VisitIntent.Buy)
        {
            ShapeItem shape = Customer.WantedShape;
            float trueValue = shape.GetTrueValue();

            gm.AddMoney(price);
            stats.salesRevenue += price;
            stats.shapesSold++;
            stats.lossFromUnderselling += Mathf.Max(0f, trueValue - price);

            gm.inventory.SellShape(shape);
        }
        else
        {
            ShapeItem shape = Customer.GiveShapeForSale();
            float trueValue = shape.GetTrueValue();

            gm.TrySpendMoney(price);
            stats.moneySpent += price;
            stats.shapesBought++;
            stats.lossFromOverpaying += Mathf.Max(0f, price - trueValue);

            gm.inventory.TryAdd(shape);
        }

        Customer.Say("Pleasure doing business!");
        EndDeal();
    }

    public void Haggle()
    {
        if (!IsOpen) return;

        switch (Customer.TryHaggle())
        {
            case HaggleResult.Improved:
                Reply = Customer.Intent == VisitIntent.Buy
                    ? $"Fine... ${Customer.CurrentPrice:0}, then."
                    : $"Okay, okay. ${Customer.CurrentPrice:0}.";
                break;

            case HaggleResult.FinalOffer:
                Reply = "That's my final offer.";
                break;

            case HaggleResult.WalkedAway:
                Customer.Say("Forget it!");
                EndDeal();
                break;
        }
    }

    public void Refuse()
    {
        if (!IsOpen) return;
        Customer.Say("Hmph.");
        EndDeal();
    }

    // Used at closing time / restart.
    public void Close()
    {
        if (IsOpen) EndDeal();
    }

    private void EndDeal()
    {
        Customer leaving = Customer;
        Customer = null;
        if (leaving != null) leaving.LeaveShop();
    }
}
