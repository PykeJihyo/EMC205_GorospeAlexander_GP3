using UnityEngine;

public class CustomerProfile
{
    public string Label;

    public Vector2 BudgetPercent;    // budget = today's 100% price x random value in this range
    public Vector2 FairOfferRange;   // a buyer's normal offer = true value x random value in this range
    public Vector2 SellerAskRange;   // a seller's normal asking price = true value x random value in this range
    public float TrickChance;        // chance a buyer lowballs, or a seller overprices
    public float AnnoyChance;        // chance they walk away each time you haggle
    public float JustLookingChance;  // chance they only browse and leave without buying anything
    public float SellChance;         // chance they came in to SELL instead of buy
    public Vector3 SellQualityWeights; // weights for shapes they sell: x = 5-40%, y = 40-75%, z = 75-100%

    // Lowball and overprice "tricks" use the same ranges for everyone.
    public static readonly Vector2 LowballRange = new Vector2(0.4f, 0.65f);
    public static readonly Vector2 OverpriceRange = new Vector2(1.4f, 1.8f);

    public float RollSellPerfectness()
    {
        Vector3 w = SellQualityWeights;
        float roll = Random.value * (w.x + w.y + w.z);
        if (roll < w.x) return Random.Range(5f, 40f);
        if (roll < w.x + w.y) return Random.Range(40f, 75f);
        return Random.Range(75f, 100f);
    }

    public static readonly CustomerProfile Low = new CustomerProfile
    {
        Label = "Low-class",
        BudgetPercent = new Vector2(0.3f, 0.7f),
        FairOfferRange = new Vector2(0.8f, 1.05f),   // offers a bit under value
        SellerAskRange = new Vector2(0.75f, 1.05f),  // needs cash: sells cheap
        TrickChance = 0.35f,
        AnnoyChance = 0.35f,
        JustLookingChance = 0.2f,
        SellChance = 0.3f,
        SellQualityWeights = new Vector3(0.75f, 0.25f, 0f) // never sells high-end shapes
    };

    public static readonly CustomerProfile Mid = new CustomerProfile
    {
        Label = "Mid-class",
        BudgetPercent = new Vector2(0.45f, 0.9f),
        FairOfferRange = new Vector2(0.9f, 1.12f),   // offers around value
        SellerAskRange = new Vector2(0.85f, 1.15f),  // asks around value
        TrickChance = 0.2f,
        AnnoyChance = 0.25f,
        JustLookingChance = 0.15f,
        SellChance = 0.3f,
        SellQualityWeights = new Vector3(0.35f, 0.55f, 0.1f)
    };

    public static readonly CustomerProfile High = new CustomerProfile
    {
        Label = "High-class",
        BudgetPercent = new Vector2(0.85f, 1.5f),
        FairOfferRange = new Vector2(0.98f, 1.25f),  // often pays above value
        SellerAskRange = new Vector2(0.95f, 1.25f),  // knows what it's worth: asks more
        TrickChance = 0.1f,
        AnnoyChance = 0.15f,
        JustLookingChance = 0.1f,
        SellChance = 0.25f,
        SellQualityWeights = new Vector3(0.1f, 0.3f, 0.6f)
    };

    public static CustomerProfile For(CustomerClass customerClass)
    {
        switch (customerClass)
        {
            case CustomerClass.Low: return Low;
            case CustomerClass.High: return High;
            default: return Mid;
        }
    }
}