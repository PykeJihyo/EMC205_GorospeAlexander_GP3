using UnityEngine;

public class ShapeItem : MonoBehaviour, IInteractable
{
    public ShapeType Type { get; private set; }
    public float Perfectness { get; private set; } // 1 - 100
    public Vector3 OriginalScale { get; private set; }

    // Where it sits on your tables. Only InventoryManager should change these.
    public bool IsOnDisplay { get; set; }
    public int SlotIndex { get; set; } = -1;   // -1 = not on your tables (e.g. a customer's shape)
    public bool IsStolen { get; set; }

    private WorldLabel label;

    public void Initialize(ShapeType type, float perfectness)
    {
        Type = type;
        Perfectness = perfectness;
        OriginalScale = transform.localScale;

        label = WorldLabel.Create(transform, Vector3.up * 0.8f, Color.white);
        label.SetText($"{perfectness}%");
    }

    void LateUpdate()
    {
        if (label != null) label.Visible = !IsStolen;
    }

    // The hidden "fair" price: today's 100% price x perfectness.
    public float GetTrueValue()
    {
        float marketPrice = GameManager.Instance.GetMarketPrice(Type);
        return Mathf.Round(marketPrice * Perfectness / 100f);
    }

    public void OnClicked()
    {
        // A shape held by a thief or a customer: treat the click as clicking that person.
        if (IsStolen || SlotIndex < 0)
        {
            IInteractable holder = transform.parent != null
                ? transform.parent.GetComponentInParent<IInteractable>()
                : null;
            if (holder != null) holder.OnClicked();
            return;
        }

        GameManager gm = GameManager.Instance;
        gm.RunCommand(new MoveShapeCommand(this, gm.inventory));
    }
}