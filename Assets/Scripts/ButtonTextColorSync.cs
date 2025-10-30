using TMPro;
using UnityEngine;

public class ImageColorSync : MonoBehaviour
{
    public TMP_Text originalText;
    public TMP_Text toSync;

    void Update()
    {
        Color baseColor = originalText.color;
        Color graphicColor = originalText.canvasRenderer.GetColor();

        toSync.color = new Color(
            baseColor.r * graphicColor.r,
            baseColor.g * graphicColor.g,
            baseColor.b * graphicColor.b,
            baseColor.a * graphicColor.a
        );
    }
}
