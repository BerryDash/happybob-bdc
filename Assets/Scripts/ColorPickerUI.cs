using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class ColorPickerUI : MonoBehaviour
{
    public RawImage hueImage;
    public RawImage svImage;
    public RectTransform svCursor;
    public RectTransform hueCursor;
    public event Action<Color> OnColorChanged;

    private Texture2D hueTexture;
    private Texture2D svTexture;

    private const int hueHeight = 256;
    private const int svSize = 256;

    private float selectedHue = 0f;
    private float selectedS = 1f;
    private float selectedV = 1f;

    private enum DragTarget { None, Hue, SV }
    private DragTarget currentDrag = DragTarget.None;

    private Color lastColor;

    void Start()
    {
        GenerateHueBar();
        GenerateSVBox(selectedHue);
        UpdateSVCursor();
        UpdateHueCursor();
        lastColor = GetSelectedColor();
        OnColorChanged?.Invoke(lastColor);
    }

    void Update()
    {
        if (Pointer.current == null)
            return;

        bool pressed = Pointer.current.press.isPressed;
        Vector2 mousePos = Pointer.current.position.ReadValue();

        if (pressed && currentDrag == DragTarget.None)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(hueImage.rectTransform, mousePos, null))
                currentDrag = DragTarget.Hue;
            else if (RectTransformUtility.RectangleContainsScreenPoint(svImage.rectTransform, mousePos, null))
                currentDrag = DragTarget.SV;
        }

        if (!pressed)
        {
            currentDrag = DragTarget.None;
            return;
        }

        bool colorChanged = false;

        if (currentDrag == DragTarget.Hue)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(hueImage.rectTransform, mousePos, null, out Vector2 localPoint);
            float y = Mathf.Clamp01((localPoint.y + hueImage.rectTransform.rect.height / 2f) / hueImage.rectTransform.rect.height);
            float newHue = 1f - y;
            if (!Mathf.Approximately(newHue, selectedHue))
            {
                selectedHue = newHue;
                GenerateSVBox(selectedHue);
                UpdateSVCursor();
                UpdateHueCursor();
                colorChanged = true;
            }
        }
        else if (currentDrag == DragTarget.SV)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(svImage.rectTransform, mousePos, null, out Vector2 localPoint);
            float rectWidth = svImage.rectTransform.rect.width;
            float rectHeight = svImage.rectTransform.rect.height;
            float x = (localPoint.x + rectWidth / 2f) / rectWidth;
            float y = (localPoint.y + rectHeight / 2f) / rectHeight;
            float newS = Mathf.Clamp01(x);
            float newV = Mathf.Clamp01(y);
            if (!Mathf.Approximately(newS, selectedS) || !Mathf.Approximately(newV, selectedV))
            {
                selectedS = newS;
                selectedV = newV;
                UpdateSVCursor();
                colorChanged = true;
            }
        }

        if (colorChanged)
        {
            Color currentColor = GetSelectedColor();
            if (currentColor != lastColor)
            {
                lastColor = currentColor;
                OnColorChanged?.Invoke(currentColor);
            }
        }
    }

    void UpdateSVCursor()
    {
        if (svCursor == null) return;
        Rect rect = svImage.rectTransform.rect;
        float x = selectedS * rect.width;
        float y = selectedV * rect.height;
        x = Mathf.Clamp(x, 0, rect.width);
        y = Mathf.Clamp(y, 0, rect.height);
        Vector2 clampedPos = new Vector2(x - rect.width / 2f, y - rect.height / 2f);
        svCursor.anchoredPosition = clampedPos;
    }

    void UpdateHueCursor()
    {
        if (hueCursor == null) return;
        Rect rect = hueImage.rectTransform.rect;
        float y = (1f - selectedHue) * rect.height;
        y = Mathf.Clamp(y, 0, rect.height);
        Vector2 pos = new Vector2(0, y - rect.height / 2f);
        hueCursor.anchoredPosition = pos;
    }

    void GenerateHueBar()
    {
        hueTexture = new Texture2D(1, hueHeight);
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < hueHeight; y++)
        {
            float h = 1f - (float)y / (hueHeight - 1);
            Color color = Color.HSVToRGB(h, 1f, 1f);
            hueTexture.SetPixel(0, y, color);
        }
        hueTexture.Apply();
        hueImage.texture = hueTexture;
    }

    void GenerateSVBox(float hue)
    {
        svTexture = new Texture2D(svSize, svSize);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < svSize; y++)
        {
            float v = (float)y / (svSize - 1);
            for (int x = 0; x < svSize; x++)
            {
                float s = (float)x / (svSize - 1);
                Color color = Color.HSVToRGB(hue, s, v);
                svTexture.SetPixel(x, y, color);
            }
        }
        svTexture.Apply();
        svImage.texture = svTexture;
    }

    public Color GetSelectedColor()
    {
        return Color.HSVToRGB(selectedHue, selectedS, selectedV);
    }

    public void SetSelectedColor(float r, float g, float b)
    {
        Color32 color = new((byte)r, (byte)g, (byte)b, 255);

        if (color.r == 0 && color.g == 0 && color.b == 0)
        {
            selectedS = 0;
            selectedV = 0;
        }
        else
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            selectedHue = h;
            selectedS = s;
            selectedV = v;
        }

        GenerateSVBox(selectedHue);
        UpdateSVCursor();
        UpdateHueCursor();

        var current = GetSelectedColor();
        if (current != lastColor)
        {
            lastColor = current;
            OnColorChanged?.Invoke(current);
        }
    }
}