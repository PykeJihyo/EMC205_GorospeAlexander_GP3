using TMPro;
using UnityEngine;

public class HudView : MonoBehaviour
{
    [Header("HUD texts")]
    public TMP_Text dayText;
    public TMP_Text moneyText;
    public TMP_Text salesText;

    [Header("Feedback")]
    public Color gainColor = Color.green;
    public Color lossColor = Color.red;
    public Color quotaMetColor = new Color(1f, 0.8f, 0.2f); // gold
    public float popScale = 1.3f;   // money text size on a change
    public float settleSpeed = 4f;  // how fast the money text returns to normal

    private HudViewModel viewModel;
    private Color moneyNormalColor;
    private Color salesNormalColor;

    void Start()
    {
        moneyNormalColor = moneyText.color;
        salesNormalColor = salesText.color;

        viewModel = new HudViewModel(GameManager.Instance);
        viewModel.DayTextChanged += ShowDay;
        viewModel.MoneyTextChanged += ShowMoney;
        viewModel.SalesTextChanged += ShowSales;
    }

    void OnDestroy()
    {
        if (viewModel == null) return;
        viewModel.DayTextChanged -= ShowDay;
        viewModel.MoneyTextChanged -= ShowMoney;
        viewModel.SalesTextChanged -= ShowSales;
        viewModel.StopListening();
    }

    void Update()
    {
        // Eases the money text back to its normal color and size.
        float t = settleSpeed * Time.deltaTime;
        moneyText.color = Color.Lerp(moneyText.color, moneyNormalColor, t);
        moneyText.transform.localScale = Vector3.Lerp(moneyText.transform.localScale, Vector3.one, t);
    }

    private void ShowDay(string text)
    {
        dayText.text = text;
    }

    private void ShowMoney(string text, float change)
    {
        moneyText.text = text;
        if (change == 0f) return;

        moneyText.color = change > 0f ? gainColor : lossColor; // green = earned, red = spent
        moneyText.transform.localScale = Vector3.one * popScale;
    }

    private void ShowSales(string text, bool quotaMet)
    {
        salesText.text = text;
        salesText.color = quotaMet ? quotaMetColor : salesNormalColor;
    }
}