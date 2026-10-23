// Shared: walk calmly to the exit and disappear.
public class LeaveState : INPCState
{
    public void Enter(NPC npc)
    {
        npc.ReleaseSlot(); // free the spot right away so a new visitor can come in
    }

    public void Execute(NPC npc)
    {
        if (npc.MoveTo(npc.ExitPosition))
        {
            npc.OnLeftShop();
        }
    }

    public void Exit(NPC npc) { }
}
