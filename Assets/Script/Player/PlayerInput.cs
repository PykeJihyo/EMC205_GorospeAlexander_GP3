using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || Mouse.current == null) return;

        bool leftClick = Mouse.current.leftButton.wasPressedThisFrame;
        bool rightClick = Mouse.current.rightButton.wasPressedThisFrame;
        bool shopIsActive = gm.Phase == GamePhase.Preparation || gm.Phase == GamePhase.ShopOpen;
        if (!shopIsActive || (!leftClick && !rightClick)) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, 200f)) return;

        IInteractable clicked = hit.collider.GetComponentInParent<IInteractable>();
        if (clicked == null) return;

        ICommand command = leftClick ? clicked.GetLeftClickCommand() : clicked.GetRightClickCommand();
        if (command != null) command.Execute();
    }
}