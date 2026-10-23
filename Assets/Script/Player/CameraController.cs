using UnityEngine;

// Put this on the Main Camera. Aim the camera at the Display Table in the Scene view.
// The Inventory button (or Tab) turns it 180 degrees to face the Inventory Table behind you.
// While you look at the inventory you are "distracted" and thieves get their chance.
public class CameraController : MonoBehaviour
{
    public float turnSpeed = 6f;

    public bool IsLookingAtInventory { get; private set; }

    private Quaternion shopRotation;
    private Quaternion inventoryRotation;

    void Awake()
    {
        shopRotation = transform.rotation; // however YOU placed the camera in the scene
        Vector3 angles = shopRotation.eulerAngles;
        inventoryRotation = Quaternion.Euler(angles.x, angles.y + 180f, angles.z);
    }

    void Update()
    {
        Quaternion target = IsLookingAtInventory ? inventoryRotation : shopRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.deltaTime);
    }

    public void ToggleView()
    {
        IsLookingAtInventory = !IsLookingAtInventory;
    }

    public void LookAtShop()
    {
        IsLookingAtInventory = false;
    }
}
