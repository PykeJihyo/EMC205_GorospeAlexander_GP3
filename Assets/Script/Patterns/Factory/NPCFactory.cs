using UnityEngine;

public class NPCFactory : MonoBehaviour
{
    [Tooltip("Customer prefab")]
    public GameObject customerPrefab;
    [Tooltip("Thief prefab")]
    public GameObject thiefPrefab;

    public float customerSpeed = 3f;
    [Tooltip("Thieves walk multiplier")]
    public float thiefSpeedBonus = 1.15f;

    public NPC CreateNPC(IDifficultyStrategy difficulty, Vector3 spawnPosition)
    {
        if (customerPrefab == null || thiefPrefab == null)
        {
            Debug.LogError("drag your Customer and Thief prefabs into the NPCFactory.");
            return null;
        }

        // A thief is disguised as a class that fits this difficulty, so it blends in.
        CustomerClass visitorClass = difficulty.RollCustomerClass();

        if (Random.value < difficulty.ThiefChance)
        {
            bool isBold = Random.value < difficulty.BoldThiefChance;
            return CreateThief(visitorClass, isBold, spawnPosition);
        }

        return CreateCustomer(visitorClass, spawnPosition);
    }

    private Customer CreateCustomer(CustomerClass customerClass, Vector3 spawnPosition)
    {
        Customer customer = SpawnBody<Customer>(customerPrefab, spawnPosition);
        CustomerProfile profile = CustomerProfile.For(customerClass);

        VisitIntent intent;
        if (Random.value < profile.JustLookingChance) intent = VisitIntent.JustLooking;
        else if (Random.value < profile.SellChance) intent = VisitIntent.Sell;
        else intent = VisitIntent.Buy;

        float marketPrice = GameManager.Instance.GetMarketPrice(ShapeType.Square);
        float budget = Mathf.Round(marketPrice * Random.Range(profile.BudgetPercent.x, profile.BudgetPercent.y));

        ShapeItem shapeForSale = null;
        if (intent == VisitIntent.Sell)
        {
            shapeForSale = ShapeDirector.BuildShapeForCustomer(profile); // Builder pattern
        }

        customer.Setup(profile, intent, budget, shapeForSale, customerSpeed);
        return customer;
    }

    private Thief CreateThief(CustomerClass disguise, bool isBold, Vector3 spawnPosition)
    {
        Thief thief = SpawnBody<Thief>(thiefPrefab, spawnPosition);
        thief.Setup(disguise, isBold, customerSpeed * thiefSpeedBonus);
        return thief;
    }

    // Creates a copy of the prefab at the Entrance Door and returns its NPC script.
    private T SpawnBody<T>(GameObject prefab, Vector3 position) where T : NPC
    {
        GameObject body = Instantiate(prefab, position, Quaternion.identity);

        T npc = body.GetComponent<T>();
        if (npc == null)
        {
            Debug.LogError($"NPCFactory: the prefab '{prefab.name}' is missing its {typeof(T).Name} script. Adding it for now.");
            npc = body.AddComponent<T>();
        }
        return npc;
    }
}