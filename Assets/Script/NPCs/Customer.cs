using System.Collections.Generic;
using UnityEngine;

public class Customer : NPC, IInteractable
{
    [Header("Glow when ready to talk")]
    public Color glowColor = Color.yellow;
    [Tooltip("Above 1 makes it bloom if your scene has a Bloom post-processing effect.")]
    public float glowIntensity = 2f;

    public CustomerProfile Profile { get; private set; }
    public VisitIntent Intent { get; private set; }
    public float Budget { get; private set; }

    public ShapeItem WantedShape { get; private set; }  // the shape on your Display Table they want
    public ShapeItem ShapeForSale { get; private set; } // the shape they brought

    public float CurrentPrice { get; private set; }     // what they're offering / asking right now
    public int HaggleTriesLeft { get; private set; } = 2;

    // Buyer: the most they'd ever pay. Seller: the least they'd ever accept. Hidden from the player.
    private float walkAwayPrice;

    public bool IsReady => CurrentState is CustomerReadyState;

    public void Setup(CustomerProfile profile, VisitIntent intent, float budget, ShapeItem shapeForSale, float speed)
    {
        Profile = profile;
        Intent = intent;
        Budget = budget;
        MoveSpeed = speed;
        name = $"{profile.Label} Customer";

        if (shapeForSale != null)
        {
            ShapeForSale = shapeForSale;
            shapeForSale.transform.SetParent(transform);
            shapeForSale.gameObject.SetActive(false); // hidden until they're ready to talk
        }
    }

    public override void OnArrivedAtSlot()
    {
        ChangeState(new CustomerBrowseState());
    }

    // How long they browse
    public float GetBrowseTime()
    {
        return Intent == VisitIntent.JustLooking ? Random.Range(8f, 20f) : Random.Range(2.5f, 6f);
    }

    // Called at the end of browsing. Returns true if they want to talk to you
    public bool DecideWhatToDo()
    {
        if (Intent == VisitIntent.JustLooking)
        {
            Say("Just looking.");
            return false;
        }

        if (Intent == VisitIntent.Sell)
        {
            PrepareSellingPrice();
            return true;
        }

        List<ShapeItem> displayed = GameManager.Instance.inventory.GetDisplayedShapes();
        List<ShapeItem> affordable = displayed.FindAll(shape => shape.GetTrueValue() <= Budget);

        if (affordable.Count == 0)
        {
            Say(displayed.Count == 0 ? "Nothing for sale?" : "Too pricey for me.");
            return false;
        }

        WantedShape = affordable[Random.Range(0, affordable.Count)];
        PrepareBuyingOffer(WantedShape.GetTrueValue());
        return true;
    }

    // Buyer: usually offers around the true value, sometimes lowballs.
    private void PrepareBuyingOffer(float trueValue)
    {
        bool lowball = Random.value < Profile.TrickChance;
        Vector2 range = lowball ? CustomerProfile.LowballRange : Profile.FairOfferRange;

        CurrentPrice = Mathf.Round(Mathf.Min(trueValue * Random.Range(range.x, range.y), Budget));
        walkAwayPrice = Mathf.Round(Mathf.Min(Budget, trueValue * Random.Range(1f, Profile.FairOfferRange.y + 0.05f)));
        walkAwayPrice = Mathf.Max(walkAwayPrice, CurrentPrice);
    }

    // Seller: asks around the true value (depends on class), sometimes tries to rip you off.
    private void PrepareSellingPrice()
    {
        float trueValue = ShapeForSale.GetTrueValue();
        bool overprice = Random.value < Profile.TrickChance;
        Vector2 range = overprice ? CustomerProfile.OverpriceRange : Profile.SellerAskRange;

        CurrentPrice = Mathf.Max(1f, Mathf.Round(trueValue * Random.Range(range.x, range.y)));
        walkAwayPrice = Mathf.Max(1f, Mathf.Round(trueValue * Random.Range(0.8f, 1f)));
        walkAwayPrice = Mathf.Min(walkAwayPrice, CurrentPrice);
    }

    // pressed "Haggle". Moves the price toward the walk-away price, or they get annoyed.
    public HaggleResult TryHaggle()
    {
        if (HaggleTriesLeft <= 0 || Mathf.Approximately(CurrentPrice, walkAwayPrice))
            return HaggleResult.FinalOffer;

        HaggleTriesLeft--;

        if (Random.value < Profile.AnnoyChance)
            return HaggleResult.WalkedAway;

        float newPrice = Mathf.Round(CurrentPrice + (walkAwayPrice - CurrentPrice) * Random.Range(0.4f, 0.7f));
        if (Mathf.Approximately(newPrice, CurrentPrice)) newPrice = walkAwayPrice;
        CurrentPrice = newPrice;
        return HaggleResult.Improved;
    }

    public bool WantedShapeStillAvailable()
    {
        return GameManager.Instance.inventory.IsAvailableForSale(WantedShape);
    }

    // Sellers hold their shape above their head while waiting, so you can inspect it.
    public void ShowShapeForSale()
    {
        if (ShapeForSale == null) return;
        ShapeForSale.gameObject.SetActive(true);
        ShapeForSale.transform.localPosition = new Vector3(0f, 1.7f, 0f);
        ShapeForSale.transform.localScale = ShapeForSale.OriginalScale * 0.6f;
    }

    // Hands the shape over after you buy it (it's no longer a child of this customer).
    public ShapeItem GiveShapeForSale()
    {
        ShapeItem shape = ShapeForSale;
        ShapeForSale = null;
        return shape;
    }

    // Steady glow using the material's Emission. With a Bloom effect in the scene, it blooms.
    public void SetGlow(bool isGlowing)
    {
        if (body == null) return;
        Material mat = body.material;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", isGlowing ? glowColor * glowIntensity : Color.black);
    }

    public void OnClicked()
    {
        if (IsReady) GameManager.Instance.Haggle.Open(this);
    }
}