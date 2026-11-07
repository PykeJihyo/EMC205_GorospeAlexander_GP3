public class CatchThiefCommand : ICommand
{
    private readonly Thief thief;

    public CatchThiefCommand(Thief thief)
    {
        this.thief = thief;
    }

    public void Execute()
    {
        if (!(thief.CurrentState is ThiefFleeState)) return;

        GameManager gm = GameManager.Instance;
        gm.inventory.ReturnStolen(thief.GiveBackShape());
        gm.Stats.thievesCaught++;
        gm.ui.ShowMessage("Thief caught! Shape recovered.");

        thief.Say("Busted...");
        thief.LeaveShop();
    }
}