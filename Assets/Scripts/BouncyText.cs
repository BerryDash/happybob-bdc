using UnityEngine;
using TMPro;

public class BouncyText : MonoBehaviour
{
    public float frequency = 2f;
    public float minSize = 10f;
    public float maxSize = 12f;
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        float newsize = (Mathf.Sin(Time.time * frequency) + 1f) / 2f;
        text.fontSize = Mathf.Lerp(minSize, maxSize, newsize);
    }
}
