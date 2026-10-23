using UnityEngine;

// Easy: high-class and mid-class customers, tight security (few thieves).
public class EasyDifficulty : IDifficultyStrategy
{
    public string Name => "Easy";

    public float StartingMoney => 120f;
    public float ThiefChance => 0.08f;
    public float BoldThiefChance => 0.15f;
    public Vector2 SpawnInterval => new Vector2(3f, 6f);
    public float QuotaFactor => 0.7f;
    public float QuotaGrowthPerDay => 0.2f; // rich customers make money fast, so the quota climbs fast too

    public CustomerClass RollCustomerClass()
    {
        return Random.value < 0.6f ? CustomerClass.High : CustomerClass.Mid; // 60% High, 40% Mid
    }
}