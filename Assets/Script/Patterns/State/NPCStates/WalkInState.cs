// Shared by customers and thieves: walk from the entrance to your spot at the counter.
public class WalkInState : INPCState
{
    public void Enter(NPC npc) { }

    public void Execute(NPC npc)
    {
        if (npc.MoveTo(npc.SlotPosition))
        {
            npc.OnArrivedAtSlot(); // Customer -> Browse, Thief -> Lookout
        }
    }

    public void Exit(NPC npc) { }
}
