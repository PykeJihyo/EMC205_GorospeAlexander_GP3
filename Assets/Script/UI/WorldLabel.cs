using TMPro;
using UnityEngine;

// A small 3D text that floats above something and always faces the camera.
public class WorldLabel : MonoBehaviour
{
    // If the labels look too big or too small in your scene, change this number.
    public const float DefaultFontSize = 4f;

    public bool Visible { get; set; } = true;

    private Transform target;
    private Vector3 offset;
    private TextMeshPro text;

    public static WorldLabel Create(Transform target, Vector3 offset, Color color)
    {
        GameObject labelObject = new GameObject(target.name + " Label");
        WorldLabel label = labelObject.AddComponent<WorldLabel>();

        label.text = labelObject.AddComponent<TextMeshPro>();
        label.text.fontSize = DefaultFontSize;
        label.text.alignment = TextAlignmentOptions.Center;
        label.text.color = color;
        label.text.rectTransform.sizeDelta = new Vector2(8f, 1.5f);

        label.target = target;
        label.offset = offset;
        return label;
    }

    public void SetText(string message)
    {
        text.text = message;
    }

    void LateUpdate()
    {
        // The thing we follow was destroyed (sold, left the shop...): remove the label too.
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        text.enabled = Visible && target.gameObject.activeInHierarchy && !string.IsNullOrEmpty(text.text);
        transform.position = target.position + offset;

        if (Camera.main != null) transform.rotation = Camera.main.transform.rotation;
    }
}