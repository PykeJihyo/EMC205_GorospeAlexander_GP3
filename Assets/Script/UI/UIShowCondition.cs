// When something should be visible. Pick one from the dropdown on the UIShowWhen script.
public enum UIShowCondition
{
    TitleScreen = 1,      // Title panel
    DuringDay = 2,        // HUD panel (preparation + store open)
    BeforeStoreOpens = 3, // Open Market and Open Store buttons
    MarketPanel = 4,
    HexagonPanel = 5,
    RevealPanel = 6,
    HagglePanel = 7,
    Report = 8,           // Report panel
    ReportPassed = 9,     // Next Day button
    ReportFailed = 10,    // Restart and Change Difficulty buttons
    PopupMessage = 11     // the "Thief caught!" text
}