// The customer "thinks" for a while, then decides: talk to you (Ready) or leave.
public class CustomerBrowseState : INPCState
{
    private float thinkTime;

    public void Enter(NPC npc)
    {
        thinkTime = ((Customer)npc).GetBrowseTime();
    }

    public void Execute(NPC npc)
    {
        thinkTime -= UnityEngine.Time.deltaTime;
        if (thinkTime > 0f) return;

        Customer customer = (Customer)npc;
        if (customer.DecideWhatToDo())
            customer.ChangeState(new CustomerReadyState());
        else
            customer.LeaveShop();
    }

    public void Exit(NPC npc) { }
}