using UnityEngine;

public interface IDifficultyStrategy
{
    string Name { get; }

    float StartingMoney { get; }

    // Chance (0-1) that each new visitor is a thief instead of a customer.
    float ThiefChance { get; }

    // Chance (0-1) that a thief is "bold" and steals even while you're watching.
    float BoldThiefChance { get; }

    // Seconds between visitors (random between x and y).
    Vector2 SpawnInterval { get; }

    // Day 1 quota = today's 100% Square price x this number.
    float QuotaFactor { get; }

    // Quota grows by this % every day, on top of the previous day (0.15 = +15% per day).
    float QuotaGrowthPerDay { get; }

    // Decides the social class of the next visitor.
    CustomerClass RollCustomerClass();
}