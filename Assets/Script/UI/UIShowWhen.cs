using UnityEngine;

// Add this to a panel or button and pick when it should show.
[RequireComponent(typeof(CanvasGroup))]
public class UIShowWhen : MonoBehaviour
{
    public UIShowCondition showWhen;

    private CanvasGroup group;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        bool visible = gm.ui.IsVisible(showWhen);
        group.alpha = visible ? 1f : 0f;
        group.interactable = visible;
        group.blocksRaycasts = visible; // hidden panels don't block your clicks
    }
}