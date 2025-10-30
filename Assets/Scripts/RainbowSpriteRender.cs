using UnityEngine;

public class RainbowSpriteRender : MonoBehaviour
{
    public float frequency = 1f;
    private SpriteRenderer targetImage;

    void Awake()
    {
        targetImage = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float t = Time.time * frequency;
        float r = Mathf.Sin(t) * 0.5f + 0.5f;
        float g = Mathf.Sin(t + 2f) * 0.5f + 0.5f;
        float b = Mathf.Sin(t + 4f) * 0.5f + 0.5f;
        targetImage.color = new Color(r, g, b);
    }
}
