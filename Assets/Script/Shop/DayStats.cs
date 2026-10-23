
// [System.Serializable] lets you see it in the GameManager's Inspector while playing.
[System.Serializable]
public class DayStats
{
    public float salesRevenue;        // money earned from selling shapes (this is what the quota checks)
    public int shapesSold;

    public float moneySpent;          // shapes bought from customers + hexagons bought
    public int shapesBought;
    public int hexagonsBought;

    public int shapesStolen;
    public float valueLostToThieves;
    public int thievesCaught;

    public float lossFromOverpaying;  // you paid more than the shape was really worth
    public float lossFromUnderselling;// you sold for less than the shape was really worth
}