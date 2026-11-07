public class SendAwayCommand : ICommand
{
    private readonly NPC npc;

    public SendAwayCommand(NPC npc)
    {
        this.npc = npc;
    }

    public void Execute()
    {
        if (npc.CurrentState is LeaveState || npc.CurrentState is ThiefFleeState) return;

        npc.Say("Hmph.");

        HaggleSystem haggle = GameManager.Instance.Haggle;
        if (haggle.Customer == npc) haggle.Close();
        else npc.LeaveShop();
    }
}