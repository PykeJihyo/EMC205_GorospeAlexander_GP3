
public static class ShapeDirector // It knows the recipes (which steps to call on the builder) for the two places shapes come from in the game.
{
    // Cracking a hexagon: any perfectness from 5% to 100%.
    public static ShapeItem CrackHexagon()
    {
        return new ShapeBuilder()
            .OfType(ShapeType.Square)
            .WithRandomPerfectness(5f, 100f)
            .Build();
    }

    // A shape a customer brings to sell you. Its quality depends on their class
    // (low-class customers never carry high-perfectness shapes).
    public static ShapeItem BuildShapeForCustomer(CustomerProfile profile)
    {
        return new ShapeBuilder()
            .OfType(ShapeType.Square)
            .WithPerfectness(profile.RollSellPerfectness())
            .Build();
    }
}
