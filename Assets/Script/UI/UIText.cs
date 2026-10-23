using TMPro;
using UnityEngine;

// Add this to any TextMeshPro text and pick what it should show. It updates itself every frame.
[RequireComponent(typeof(TMP_Text))]
public class UIText : MonoBehaviour
{
    public UITextType show;

    private TMP_Text text;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        text.text = gm.ui.GetText(show);
    }
}