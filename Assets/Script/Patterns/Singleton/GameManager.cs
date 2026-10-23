using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)] // runs its Awake before every other script
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene references")]
    public ShopLayout layout;
    public InventoryManager inventory;
    public NPCSpawner spawner;
    public GameUI ui;
    public CameraController cameraController;

    [Header("Day clock")]
    public int openHour = 12;  // 12 PM
    public int closeHour = 18; // 6 PM
    // Real seconds per in-game hour. 20 = a 2-minute shop day
    public float realSecondsPerGameHour = 20f;

    [Header("Economy")]
    public float baseSquarePrice = 100f;
    [Tooltip("Each day the market price is base price x a random number in this range.")]
    public Vector2 dailyPriceSwing = new Vector2(0.8f, 1.3f);
    public int freeHexagonsAtStart = 3;
    // Price of one hexagon, as a % of today's 100% Square price
    // An average hexagon gives a ~52% shape, so 0.35 leaves room for profit
    public float hexagonCostPercent = 0.35f;

    [Header("Live stats")]
    [SerializeField] private GamePhase phase = GamePhase.ChooseDifficulty;
    [SerializeField] private int day;
    [SerializeField] private float currentHour;
    [SerializeField] private float money;
    [SerializeField] private float quota;
    [SerializeField] private float marketPriceToday;
    [SerializeField] private int hexagonsOwned;
    [SerializeField] private DayStats stats = new DayStats();

    public GamePhase Phase => phase;
    public int Day => day;
    public float CurrentHour => currentHour;
    public float Money => money;
    public float Quota => quota;
    public float MarketPriceToday => marketPriceToday;
    public int HexagonsOwned => hexagonsOwned;
    public DayStats Stats => stats;

    public IDifficultyStrategy Difficulty { get; private set; }
    public bool PassedQuota { get; private set; }
    public HaggleSystem Haggle { get; private set; }

    private readonly Dictionary<ShapeType, float> marketPrices = new Dictionary<ShapeType, float>();

    // A thief's chance: you're looking at the Inventory Table, haggling, or reading a menu panel.
    public bool IsPlayerDistracted => cameraController.IsLookingAtInventory || Haggle.IsOpen || ui.IsMenuOpen;

    public float HexagonCost => Mathf.Round(GetMarketPrice(ShapeType.Square) * hexagonCostPercent);

    void Awake()
    {
        // --- The singleton rule ---
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        NPC.Active.Clear();

        Haggle = new HaggleSystem();
        inventory.Init(layout);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        if (phase != GamePhase.ShopOpen) return;

        currentHour += Time.deltaTime / realSecondsPerGameHour;
        if (currentHour >= closeHour) CloseShop();
    }

    // Game flow 

    // Called by the difficulty buttons. The UI passes in the difficulty STRATEGY object.
    public void StartNewGame(IDifficultyStrategy chosenDifficulty)
    {
        Difficulty = chosenDifficulty;

        Haggle.Close();
        spawner.RemoveAllNPCs();
        inventory.Clear();

        day = 1;
        money = Difficulty.StartingMoney;
        hexagonsOwned = freeHexagonsAtStart;

        BeginPreparation();
    }

    private void BeginPreparation()
    {
        phase = GamePhase.Preparation;
        currentHour = openHour;
        stats = new DayStats();

        RollMarketPrices();
        quota = CalculateQuota();
        cameraController.LookAtShop();
    }

    // "Open Store" button.
    public void OpenShop()
    {
        if (phase != GamePhase.Preparation) return;

        phase = GamePhase.ShopOpen;
        cameraController.LookAtShop();
        spawner.BeginDay();
    }

    private void CloseShop()
    {
        currentHour = closeHour;
        Haggle.Close();
        spawner.RemoveAllNPCs(); // a thief still holding a shape at closing time gets away with it

        PassedQuota = stats.salesRevenue >= quota;
        phase = GamePhase.DayReport;
        cameraController.LookAtShop();
    }

    // "Next Day" button on the report panel (only shown if you passed).
    public void GoToNextDay()
    {
        day++;
        BeginPreparation();
    }

    // "Restart from Day 1" button: same difficulty, everything reset.
    public void RestartSameDifficulty()
    {
        StartNewGame(Difficulty);
    }

    // "Change Difficulty" button: back to the title panel.
    public void ReturnToTitle()
    {
        Haggle.Close();
        spawner.RemoveAllNPCs();
        inventory.Clear();
        phase = GamePhase.ChooseDifficulty;
    }

    // Market & quota 

    private void RollMarketPrices()
    {
        marketPrices.Clear();
        float swing = Random.Range(dailyPriceSwing.x, dailyPriceSwing.y);
        marketPrices[ShapeType.Square] = Mathf.Round(baseSquarePrice * swing);
        // add circles and triangles here next update

        marketPriceToday = marketPrices[ShapeType.Square];
    }

    // Price of a 100% perfect shape today.
    public float GetMarketPrice(ShapeType type)
    {
        float price;
        return marketPrices.TryGetValue(type, out price) ? price : 0f;
    }

    // Quota follows today's market price,
    // is scaled by difficulty, and grows by a % every day (compounding, like interest).
    // Example, Medium: Day 1 = 0.7 x price, Day 2 = x1.15, Day 3 = x1.15 again, ...
    private float CalculateQuota()
    {
        float growth = Mathf.Pow(1f + Difficulty.QuotaGrowthPerDay, day - 1);
        return Mathf.Round(GetMarketPrice(ShapeType.Square) * Difficulty.QuotaFactor * growth);
    }

    // Money & hexagons 

    public void AddMoney(float amount)
    {
        money += amount;
    }

    public bool TrySpendMoney(float amount)
    {
        if (amount > money) return false;
        money -= amount;
        return true;
    }

    private bool ShopIsActive => phase == GamePhase.Preparation || phase == GamePhase.ShopOpen;

    public bool CanBuyHexagon => ShopIsActive && money >= HexagonCost;
    public bool CanOpenHexagon => ShopIsActive && hexagonsOwned > 0 && inventory.HasFreeSlot;

    // Buy Hexagon
    public void BuyHexagon()
    {
        if (!CanBuyHexagon) return;

        TrySpendMoney(HexagonCost);
        stats.moneySpent += HexagonCost;
        stats.hexagonsBought++;
        hexagonsOwned++;
    }

    // Open Hexagon
    public ShapeItem OpenHexagon()
    {
        if (!CanOpenHexagon) return null;

        hexagonsOwned--;
        ShapeItem shape = ShapeDirector.CrackHexagon(); // Builder pattern
        inventory.TryAdd(shape);                        // goes to the Inventory Table
        return shape;
    }

    // Runs a player action that was wrapped as a Command object (see MoveShapeCommand).
    public void RunCommand(ICommand command)
    {
        command.Execute();
    }
}