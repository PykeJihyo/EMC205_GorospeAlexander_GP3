public class StartHaggleCommand : ICommand
{
    private readonly Customer customer;

    public StartHaggleCommand(Customer customer)
    {
        this.customer = customer;
    }

    public void Execute()
    {
        if (customer.IsReady) GameManager.Instance.Haggle.Open(customer);
    }
}