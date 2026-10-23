using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private Transform[] displayPoints;
    private Transform[] inventoryPoints;
    private ShapeItem[] displayShapes;
    private ShapeItem[] inventoryShapes;

    // Called by GameManager once ShopLayout is confirmed to be set up.
    public void Init(ShopLayout layout)
    {
        displayPoints = layout.displaySlots;
        inventoryPoints = layout.inventorySlots;
        displayShapes = new ShapeItem[displayPoints.Length];
        inventoryShapes = new ShapeItem[inventoryPoints.Length];
    }

    public bool HasFreeSlot => FirstFree(inventoryShapes) >= 0 || FirstFree(displayShapes) >= 0;

    // New shapes go to the Inventory Table first. You decide what to put on display.
    public bool TryAdd(ShapeItem shape)
    {
        int index = FirstFree(inventoryShapes);
        if (index >= 0) { Place(shape, false, index); return true; }

        index = FirstFree(displayShapes);
        if (index >= 0) { Place(shape, true, index); return true; }

        Debug.LogWarning("Both tables are full, shape discarded.");
        Destroy(shape.gameObject);
        return false;
    }

    // Display Table <-> Inventory Table. Used by MoveShapeCommand.
    public bool TryMove(ShapeItem shape)
    {
        if (!Owns(shape) || shape.IsStolen) return false;

        bool toDisplay = !shape.IsOnDisplay;
        int index = FirstFree(toDisplay ? displayShapes : inventoryShapes);
        if (index < 0) return false;

        ClearSlot(shape);
        Place(shape, toDisplay, index);
        return true;
    }

    public List<ShapeItem> GetDisplayedShapes()
    {
        List<ShapeItem> result = new List<ShapeItem>();
        foreach (ShapeItem shape in displayShapes)
        {
            if (shape != null && !shape.IsStolen) result.Add(shape);
        }
        return result;
    }

    public ShapeItem GetRandomDisplayedShape()
    {
        List<ShapeItem> shapes = GetDisplayedShapes();
        return shapes.Count == 0 ? null : shapes[Random.Range(0, shapes.Count)];
    }

    public bool IsAvailableForSale(ShapeItem shape)
    {
        return shape != null && Owns(shape) && shape.IsOnDisplay && !shape.IsStolen;
    }

    public void SellShape(ShapeItem shape)
    {
        ClearSlot(shape);
        Destroy(shape.gameObject);
    }

    // Thieves 
    // The slot stays reserved while the thief runs, so a caught shape can go straight back.
    public void MarkStolen(ShapeItem shape) { shape.IsStolen = true; }

    public void ReturnStolen(ShapeItem shape)
    {
        shape.IsStolen = false;
        Place(shape, shape.IsOnDisplay, shape.SlotIndex);
    }

    public void LoseStolen(ShapeItem shape)
    {
        ClearSlot(shape);
        Destroy(shape.gameObject);
    }

    public void Clear()
    {
        ClearArray(displayShapes);
        ClearArray(inventoryShapes);
    }

    // Internals 

    private void Place(ShapeItem shape, bool onDisplay, int index)
    {
        ShapeItem[] slots = onDisplay ? displayShapes : inventoryShapes;
        Transform point = onDisplay ? displayPoints[index] : inventoryPoints[index];

        slots[index] = shape;
        shape.IsOnDisplay = onDisplay;
        shape.SlotIndex = index;

        shape.gameObject.SetActive(true);
        shape.transform.SetParent(null);
        shape.transform.position = point.position;
        shape.transform.rotation = Quaternion.identity;
        shape.transform.localScale = shape.OriginalScale;
    }

    private void ClearSlot(ShapeItem shape)
    {
        if (Owns(shape)) (shape.IsOnDisplay ? displayShapes : inventoryShapes)[shape.SlotIndex] = null;
        shape.SlotIndex = -1;
    }

    private bool Owns(ShapeItem shape)
    {
        if (shape == null || shape.SlotIndex < 0) return false;
        ShapeItem[] slots = shape.IsOnDisplay ? displayShapes : inventoryShapes;
        return shape.SlotIndex < slots.Length && slots[shape.SlotIndex] == shape;
    }

    private static int FirstFree(ShapeItem[] slots)
    {
        return System.Array.IndexOf(slots, null);
    }

    private static void ClearArray(ShapeItem[] slots)
    {
        if (slots == null) return;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null) Destroy(slots[i].gameObject);
            slots[i] = null;
        }
    }
}
