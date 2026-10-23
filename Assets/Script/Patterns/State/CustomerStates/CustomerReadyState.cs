using UnityEngine;

// The customer glows and waits for you to click them. If you take too long, they leave.
public class CustomerReadyState : INPCState
{
    private float patience = 20f;

    public void Enter(NPC npc)
    {
        Customer customer = (Customer)npc;
        customer.SetGlow(true);
        customer.ShowShapeForSale(); // only does something for sellers
    }

    public void Execute(NPC npc)
    {
        Customer customer = (Customer)npc;

        // While you're haggling with them, they wait patiently.
        if (GameManager.Instance.Haggle.Customer == customer) return;

        patience -= Time.deltaTime;
        if (patience <= 0f)
        {
            customer.Say("Took too long...");
            customer.LeaveShop();
        }
    }

    public void Exit(NPC npc)
    {
        ((Customer)npc).SetGlow(false);
    }
}