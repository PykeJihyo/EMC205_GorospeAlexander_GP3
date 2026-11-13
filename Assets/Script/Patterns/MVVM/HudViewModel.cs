public class HudViewModel
{
    public event System.Action<string> DayTextChanged;
    public event System.Action<string, float> MoneyTextChanged; // text, amount gained (+) or spent (-)
    public event System.Action<string, bool> SalesTextChanged;  // text, true when the quota is met

    private readonly GameManager model;
    private float lastMoney;

    public HudViewModel(GameManager model)
    {
        this.model = model;
        model.MoneyChanged += OnMoneyChanged;
        model.SalesChanged += OnSalesChanged;
        model.DayStarted += OnDayStarted;
    }

    // Unsubscribes from the Model when the View is destroyed.
    public void StopListening()
    {
        model.MoneyChanged -= OnMoneyChanged;
        model.SalesChanged -= OnSalesChanged;
        model.DayStarted -= OnDayStarted;
    }

    private void OnMoneyChanged()
    {
        float change = model.Money - lastMoney;
        lastMoney = model.Money;
        MoneyTextChanged?.Invoke(MoneyText(), change);
    }

    private void OnSalesChanged()
    {
        bool quotaMet = model.Stats.salesRevenue >= model.Quota;
        SalesTextChanged?.Invoke(SalesText(), quotaMet);
    }

    private void OnDayStarted()
    {
        lastMoney = model.Money;
        DayTextChanged?.Invoke($"Day {model.Day}");
        MoneyTextChanged?.Invoke(MoneyText(), 0f); // 0 = no gain or loss, so no flash
        OnSalesChanged();
    }

    private string MoneyText() => $"Money: ${model.Money:0}";
    private string SalesText() => $"Sales: ${model.Stats.salesRevenue:0} / ${model.Quota:0}";
}