using UnityEngine;

// Hard: mostly low-class, fewer mid-class, no high-class. Weak security: thieves are regular.
public class HardDifficulty : IDifficultyStrategy
{
    public string Name => "Hard";

    public float StartingMoney => 80f;
    public float ThiefChance => 0.2f;
    public float BoldThiefChance => 0.35f;
    // Busier shop than Medium Difficulty. Poor customers buy little, so you need more of them to survive.

    public Vector2 SpawnInterval => new Vector2(2.5f, 5.5f);
    public float QuotaFactor => 0.45f;
    public float QuotaGrowthPerDay => 0.1f;

    public CustomerClass RollCustomerClass()
    {
        return Random.value < 0.35f ? CustomerClass.Mid : CustomerClass.Low; // 35% Mid, 65% Low
    }
}