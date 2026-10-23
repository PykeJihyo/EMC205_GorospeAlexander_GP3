using UnityEngine;

// Medium: mostly mid-class, some low-class, rare high-class. Average security.
public class MediumDifficulty : IDifficultyStrategy
{
    public string Name => "Medium";

    public float StartingMoney => 100f;
    public float ThiefChance => 0.15f;
    public float BoldThiefChance => 0.25f;
    public Vector2 SpawnInterval => new Vector2(3.5f, 7f);
    public float QuotaFactor => 0.7f;
    public float QuotaGrowthPerDay => 0.15f;

    public CustomerClass RollCustomerClass()
    {
        float roll = Random.value;
        if (roll < 0.15f) return CustomerClass.High; // 15%
        if (roll < 0.80f) return CustomerClass.Mid;  // 65%
        return CustomerClass.Low;                    // 20%
    }
}