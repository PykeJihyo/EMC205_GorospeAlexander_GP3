// The thief has your shape and runs for the exit a little faster than normal.
public class ThiefFleeState : INPCState
{
    public void Enter(NPC npc)
    {
        ((Thief)npc).StartFleeing();
        npc.ReleaseSlot();
    }

    public void Execute(NPC npc)
    {
        if (npc.MoveTo(npc.ExitPosition))
        {
            npc.OnLeftShop(); // escaped: Thief.OnLeftShop records the loss
        }
    }

    public void Exit(NPC npc) { }
}
