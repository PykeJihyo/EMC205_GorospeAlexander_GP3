using UnityEngine;

public class GameUI : MonoBehaviour
{
    // Which menu panels are open right now
    private bool marketOpen;
    private bool hexagonOpen;
    private bool revealOpen;

    private string revealMessage = "";
    private string popupMessage = "";
    private float popupHideTime;

    // A menu panel is open = not watching the store. Thieve's window.
    public bool IsMenuOpen => marketOpen || hexagonOpen || revealOpen;

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        // Menus close automatically when they no longer make sense.
        bool prep = gm.Phase == GamePhase.Preparation;
        bool open = gm.Phase == GamePhase.ShopOpen;
        if (!prep) marketOpen = false;
        if (!prep && !open) { hexagonOpen = false; revealOpen = false; }
    }


    // Choose difficulty buttons
    public void ChooseEasy() { GameManager.Instance.StartNewGame(new EasyDifficulty()); }
    public void ChooseMedium() { GameManager.Instance.StartNewGame(new MediumDifficulty()); }
    public void ChooseHard() { GameManager.Instance.StartNewGame(new HardDifficulty()); }

    // HUD buttons
    public void ToggleMarket() { bool was = marketOpen; CloseMenus(); marketOpen = !was; }
    public void ToggleHexagonTab() { bool was = hexagonOpen; CloseMenus(); hexagonOpen = !was; }

    public void CheckInventory()
    {
        GameManager gm = GameManager.Instance;
        if (!gm.Haggle.IsOpen) gm.cameraController.ToggleView();
    }

    public void OpenStore()
    {
        CloseMenus();
        GameManager.Instance.OpenShop();
    }

    // Hexagon panel buttons
    public void BuyHexagon() { GameManager.Instance.BuyHexagon(); }

    public void OpenHexagon()
    {
        ShapeItem shape = GameManager.Instance.OpenHexagon();
        if (shape == null)
        {
            ShowMessage("Can't open: no hexagons left, or both tables are full.");
            return;
        }

        revealMessage =
            $"You cracked open a hexagon and got a\n<b>{shape.Perfectness}% {shape.Type}!</b>\n\n" +
            "It's on your Inventory Table.";
        revealOpen = true;
    }

    // Any "Close" / "OK" button on the Market, Hexagon or Reveal panel
    public void CloseMenus()
    {
        marketOpen = false;
        hexagonOpen = false;
        revealOpen = false;
    }

    public void CloseReveal() { revealOpen = false; }

    // Haggle panel buttons
    public void AcceptDeal() { GameManager.Instance.Haggle.Accept(); }
    public void HaggleDeal() { GameManager.Instance.Haggle.Haggle(); }
    public void RefuseDeal() { GameManager.Instance.Haggle.Refuse(); }

    // Report panel buttons
    public void NextDay() { GameManager.Instance.GoToNextDay(); }
    public void RestartFromDay1() { GameManager.Instance.RestartSameDifficulty(); }
    public void ChangeDifficulty() { GameManager.Instance.ReturnToTitle(); }



    public void ShowMessage(string message, float seconds = 2.5f)
    {
        popupMessage = message;
        popupHideTime = Time.time + seconds;
    }

    // UIShowWhen asks: should I be visible right now?
    public bool IsVisible(UIShowCondition condition)
    {
        GameManager gm = GameManager.Instance;
        bool prep = gm.Phase == GamePhase.Preparation;
        bool open = gm.Phase == GamePhase.ShopOpen;
        bool report = gm.Phase == GamePhase.DayReport;

        switch (condition)
        {
            case UIShowCondition.TitleScreen: return gm.Phase == GamePhase.ChooseDifficulty;
            case UIShowCondition.DuringDay: return prep || open;
            case UIShowCondition.BeforeStoreOpens: return prep;
            case UIShowCondition.MarketPanel: return marketOpen;
            case UIShowCondition.HexagonPanel: return hexagonOpen;
            case UIShowCondition.RevealPanel: return revealOpen;
            case UIShowCondition.HagglePanel: return open && gm.Haggle.IsOpen;
            case UIShowCondition.Report: return report;
            case UIShowCondition.ReportPassed: return report && gm.PassedQuota;
            case UIShowCondition.ReportFailed: return report && !gm.PassedQuota;
            case UIShowCondition.PopupMessage: return Time.time < popupHideTime;
            default: return true; // nothing picked yet: stay visible so you notice
        }
    }

    // UIText asks: what should I say right now?
    public string GetText(UITextType type)
    {
        GameManager gm = GameManager.Instance;

        switch (type)
        {
            case UITextType.Time:
                return FormatClock(gm.CurrentHour);

            case UITextType.DayInfo:
                return $"Day {gm.Day}   |   Money: ${gm.Money:0}   |   Sales: ${gm.Stats.salesRevenue:0} / ${gm.Quota:0}";

            case UITextType.CheckInventoryLabel:
                return gm.cameraController.IsLookingAtInventory ? "Back to Store" : "Check Inventory";

            case UITextType.Market:
                if (gm.Difficulty == null) return "";
                return $"<b>MARKET - DAY {gm.Day}</b> ({gm.Difficulty.Name})\n\n" +
                       $"100% Square: <b>${gm.MarketPriceToday:0}</b>\n\n" +
                       "A shape is worth its perfectness % of this price (a 58% Square = 58% of the price).\n" +
                       "Remember it! Prices are hidden once the store opens.\n\n" +
                       $"Today's quota: <b>${gm.Quota:0}</b> in sales";

            case UITextType.Hexagon:
                return "<b>HEXAGONS</b>\n\n" +
                       "Each hexagon holds one random shape, anywhere from 5% to 100% perfect. " +
                       "Crack one open to see what's inside.\n\n" +
                       $"Price: <b>${gm.HexagonCost:0}</b>\n" +
                       $"You have: <b>{gm.HexagonsOwned}</b>\n" +
                       $"Money: ${gm.Money:0}";

            case UITextType.Reveal:
                return revealMessage;

            case UITextType.Haggle:
                return HaggleText(gm);

            case UITextType.AcceptLabel:
                return AcceptLabel(gm);

            case UITextType.HaggleLabel:
                if (!gm.Haggle.IsOpen) return "";
                Customer c = gm.Haggle.Customer;
                return $"{(c.Intent == VisitIntent.Buy ? "Ask for more" : "Ask for less")} ({c.HaggleTriesLeft})";

            case UITextType.Report:
                return ReportText(gm);

            case UITextType.PopupMessage:
                return popupMessage;

            default:
                return "";
        }
    }

    private string HaggleText(GameManager gm)
    {
        HaggleSystem haggle = gm.Haggle;
        if (!haggle.IsOpen) return "";

        Customer customer = haggle.Customer;
        bool buying = customer.Intent == VisitIntent.Buy;

        string what;
        if (buying)
        {
            ShapeItem shape = customer.WantedShape;
            string shapeText = shape != null ? $"{shape.Perfectness}% {shape.Type}" : "shape (it's gone!)";
            what = $"{customer.Profile.Label} customer wants to <b>BUY</b> your {shapeText}.";
        }
        else
        {
            ShapeItem shape = customer.ShapeForSale;
            what = $"{customer.Profile.Label} customer wants to <b>SELL</b> you a {shape.Perfectness}% {shape.Type}.";
        }

        return $"{what}\n\n\"{haggle.Reply}\"\n\n" +
               $"{(buying ? "Offer" : "Asking")}: <b>${customer.CurrentPrice:0}</b>     (your money: ${gm.Money:0})";
    }

    private string AcceptLabel(GameManager gm)
    {
        HaggleSystem haggle = gm.Haggle;
        if (!haggle.IsOpen) return "";

        string reason;
        if (!haggle.CanAccept(out reason)) return reason; // e.g. "Not enough money"

        Customer customer = haggle.Customer;
        return customer.Intent == VisitIntent.Buy
            ? $"Sell for ${customer.CurrentPrice:0}"
            : $"Buy for ${customer.CurrentPrice:0}";
    }

    private string ReportText(GameManager gm)
    {
        DayStats s = gm.Stats;
        return $"<b>DAY {gm.Day} - 6:00 PM, STORE CLOSED</b>\n" +
               (gm.PassedQuota ? "<b>QUOTA MET!</b>" : "<b>QUOTA MISSED - you're out of business.</b>") + "\n\n" +
               $"Sales: ${s.salesRevenue:0} / quota ${gm.Quota:0}\n" +
               $"Shapes sold: {s.shapesSold}\n" +
               $"Shapes bought: {s.shapesBought}   Hexagons bought: {s.hexagonsBought}   (spent ${s.moneySpent:0})\n" +
               $"Shapes stolen: {s.shapesStolen}   (worth ${s.valueLostToThieves:0})\n" +
               $"Thieves caught: {s.thievesCaught}\n" +
               $"Lost by overpaying customers: ${s.lossFromOverpaying:0}\n" +
               $"Lost by underselling your shapes: ${s.lossFromUnderselling:0}\n" +
               $"Money now: ${gm.Money:0}";
    }

    private static string FormatClock(float hour)
    {
        int h = Mathf.FloorToInt(hour);
        int m = Mathf.FloorToInt((hour - h) * 60f);
        int h12 = h % 12 == 0 ? 12 : h % 12;
        return $"{h12}:{m:00} {(h >= 12 ? "PM" : "AM")}";
    }
}