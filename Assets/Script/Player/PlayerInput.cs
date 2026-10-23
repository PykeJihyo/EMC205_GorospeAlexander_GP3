using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Turns mouse clicks into OnClicked() calls on whatever 3D object is under the cursor.
// Uses the new Input System (Mouse.current).
public class PlayerInput : MonoBehaviour
{
    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || Mouse.current == null) return;

        bool shopIsActive = gm.Phase == GamePhase.Preparation || gm.Phase == GamePhase.ShopOpen;
        if (!shopIsActive || !Mouse.current.leftButton.wasPressedThisFrame) return;

        // Clicked a button or panel? Then don't also click the world behind it.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f))
        {
            IInteractable clickable = hit.collider.GetComponentInParent<IInteractable>();
            if (clickable != null) clickable.OnClicked();
        }
    }
}