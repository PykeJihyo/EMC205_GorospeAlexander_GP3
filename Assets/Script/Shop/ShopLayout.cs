using UnityEngine;

// Holds every important position in the shop. You build these objects in the scene
// and drag them in here. Every other script reads positions from this one place.
public class ShopLayout : MonoBehaviour
{
    [Header("Doors")]
    public Transform entranceDoor;
    public Transform exitDoor;

    [Header("Slots")]
    public Transform[] npcSlots;
    public Transform[] displaySlots;
    public Transform[] inventorySlots;
}