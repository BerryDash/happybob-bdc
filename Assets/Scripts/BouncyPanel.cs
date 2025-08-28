using UnityEngine;

public class BouncyPanel : MonoBehaviour
{
    public float frequency = 8f;
    public float minSize = 1f;
    public float maxSize = 1.1f;

    void Update()
    {
        float newsize = (Mathf.Sin(Time.time * frequency) + 1f) / 2f;
        var size = Mathf.Lerp(minSize, maxSize, newsize);
        gameObject.transform.localScale = new(size, size, size);
    }
}