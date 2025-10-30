using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomColorObject : MonoBehaviour
{
    public ColorObjectType type;
    public bool invert;
    public Color addMore;
    public bool reverseAdd;

    Color ApplyModifiers(Color baseColor)
    {
        if (invert) baseColor = new(1f - baseColor.r, 1f - baseColor.g, 1f - baseColor.b);

        if (reverseAdd)
        {
            baseColor.r = Mathf.Clamp01(baseColor.r - addMore.r);
            baseColor.g = Mathf.Clamp01(baseColor.g - addMore.g);
            baseColor.b = Mathf.Clamp01(baseColor.b - addMore.b);
        }
        else
        {
            baseColor.r = Mathf.Clamp01(baseColor.r + addMore.r);
            baseColor.g = Mathf.Clamp01(baseColor.g + addMore.g);
            baseColor.b = Mathf.Clamp01(baseColor.b + addMore.b);
        }

        return baseColor;
    }

    public void SetColor()
    {
        JArray color = null;
        Color colorType = Color.white;
        Color currentColor = Color.white;

        switch (type)
        {
            case ColorObjectType.InGameBackgroundColor:
                color = BazookaManager.Instance.GetColorSettingBackground();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                var obj = gameObject.GetComponent<Camera>();
                currentColor = obj.backgroundColor;
                currentColor.r = colorType.r;
                currentColor.g = colorType.g;
                currentColor.b = colorType.b;
                obj.backgroundColor = currentColor;
                break;
            case ColorObjectType.MenuBackgroundColor:
                color = BazookaManager.Instance.GetColorSettingMenuBackground();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                obj = gameObject.GetComponent<Camera>();
                currentColor = obj.backgroundColor;
                currentColor.r = colorType.r;
                currentColor.g = colorType.g;
                currentColor.b = colorType.b;
                obj.backgroundColor = currentColor;
                break;
            case ColorObjectType.MenuBackgroundColorImage:
                color = BazookaManager.Instance.GetColorSettingMenuBackground();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                var obj2 = gameObject.GetComponent<Image>();
                currentColor = obj2.color;
                currentColor.r = colorType.r;
                currentColor.g = colorType.g;
                currentColor.b = colorType.b;
                obj2.color = currentColor;
                break;
            case ColorObjectType.MenuBackgroundColorText:
                color = BazookaManager.Instance.GetColorSettingMenuBackground();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                var obj3 = gameObject.GetComponent<TMP_Text>();
                currentColor = obj3.color;
                currentColor.r = colorType.r;
                currentColor.g = colorType.g;
                currentColor.b = colorType.b;
                obj3.color = currentColor;
                break;
            case ColorObjectType.ButtonColor:
                color = BazookaManager.Instance.GetColorSettingButton();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                obj2 = gameObject.GetComponent<Image>();
                currentColor = obj2.color;
                currentColor.r = colorType.r;
                currentColor.g = colorType.g;
                currentColor.b = colorType.b;
                obj2.color = currentColor;
                break;
            case ColorObjectType.ButtonColorText:
                color = BazookaManager.Instance.GetColorSettingButton();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                obj3 = gameObject.GetComponent<TMP_Text>();
                currentColor = obj3.color;
                currentColor.r = colorType.r;
                currentColor.g = colorType.g;
                currentColor.b = colorType.b;
                obj3.color = currentColor;
                break;
            case ColorObjectType.TextColor:
                color = BazookaManager.Instance.GetColorSettingText();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                obj3 = gameObject.GetComponent<TMP_Text>();
                currentColor = obj3.color;
                currentColor.r = colorType.r;
                currentColor.g = colorType.g;
                currentColor.b = colorType.b;
                obj3.color = currentColor;
                break;
            case ColorObjectType.TextColorImage:
                color = BazookaManager.Instance.GetColorSettingText();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                obj2 = gameObject.GetComponent<Image>();
                currentColor = obj2.color;
                currentColor.r = colorType.r;
                currentColor.g = colorType.g;
                currentColor.b = colorType.b;
                obj2.color = currentColor;
                break;
            case ColorObjectType.ButtonContentColorImage:
                color = BazookaManager.Instance.GetColorSettingButtonContent();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                obj2 = gameObject.GetComponent<Image>();
                currentColor = obj2.color;
                currentColor.r = colorType.r;
                currentColor.g = colorType.g;
                currentColor.b = colorType.b;
                obj2.color = currentColor;
                break;
            case ColorObjectType.ButtonContentColorText:
                color = BazookaManager.Instance.GetColorSettingButtonContent();
                colorType = new((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
                colorType = ApplyModifiers(colorType);
                obj3 = gameObject.GetComponent<TMP_Text>();
                currentColor = obj3.color;
                currentColor.r = colorType.r;
                currentColor.g = colorType.g;
                currentColor.b = colorType.b;
                obj3.color = currentColor;
                break;
        }
    }

    void Start()
    {
        SetColor();
    }
}