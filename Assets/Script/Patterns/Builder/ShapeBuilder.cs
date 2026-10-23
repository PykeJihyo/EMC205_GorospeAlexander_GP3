using UnityEngine;

public class ShapeBuilder
{
    // Settings collected before Build(). These defaults are used if a step is skipped.
    private ShapeType type = ShapeType.Square;
    private float perfectness = 50f;

    public ShapeBuilder OfType(ShapeType shapeType)
    {
        type = shapeType;
        return this;
    }

    public ShapeBuilder WithPerfectness(float percent)
    {
        perfectness = Mathf.Clamp(Mathf.Round(percent), 1f, 100f); // whole numbers make the mental math easier
        return this;
    }

    public ShapeBuilder WithRandomPerfectness(float min, float max)
    {
        return WithPerfectness(Random.Range(min, max));
    }

    // The final step: actually create the GameObject.
    public ShapeItem Build()
    {
        GameObject shapeObject = GameObject.CreatePrimitive(GetPrimitive(type));
        shapeObject.name = $"{type} [{perfectness}%]";
        shapeObject.GetComponent<Renderer>().sharedMaterial = GameManager.Instance.shapeMaterial;

        ApplyImperfectLook(shapeObject);

        ShapeItem item = shapeObject.AddComponent<ShapeItem>();
        item.Initialize(type, perfectness);
        return item;
    }

    private static PrimitiveType GetPrimitive(ShapeType shapeType)
    {
        switch (shapeType)
        {
            case ShapeType.Circle: return PrimitiveType.Sphere;
            default: return PrimitiveType.Cube; // Square (and Rectangle, stretched below)
        }
    }

    // Less perfect = greyer and more squashed. A 100% shape is a clean white cube.
    private void ApplyImperfectLook(GameObject shapeObject)
    {
        float quality = perfectness / 100f;
        Color color = Color.Lerp(Color.gray * 0.6f, Color.white, quality);
        Material mat = shapeObject.GetComponent<Renderer>().material;
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color); // URP Lit shader
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);         // Built-in Standard shader

        float maxDistortion = 0.6f * (1f - quality);
        Vector3 scale = new Vector3(
            1f + Random.Range(-maxDistortion, maxDistortion),
            1f + Random.Range(-maxDistortion, maxDistortion),
            1f + Random.Range(-maxDistortion, maxDistortion));

        if (type == ShapeType.Rectangle) scale.x *= 1.8f;

        shapeObject.transform.localScale = scale * 0.8f;
    }
}
